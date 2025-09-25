Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices.ComTypes
Imports System.Threading

Namespace WebData

    Public Class PlayersQuotes

        Shared Function GetPlayersQuotes(ServerPath As String, GetQuotes_Fantacalcio_it As Boolean, ReturnData As Boolean) As String
            If GetQuotes_Fantacalcio_it Then
                Return GetPlayersQuoteFantacalcio(ServerPath, ReturnData)
            Else
                Return GetPlayersQuotePianetaFantacalcio(ServerPath, ReturnData)
            End If
        End Function

        Shared Function GetPlayersQuotePianetaFantacalcio(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(Functions.Year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(Functions.Year) & "\data"
            Dim filet As String = dirt & "\players-quote.txt"
            Dim filed As String = dird & "\players-quote-re.txt"
            Dim strdata As New System.Text.StringBuilder

            Functions.Dirs = ServerPath

            Try

                If (IO.Directory.Exists(dirt) = False) Then IO.Directory.CreateDirectory(dirt)
                If (IO.Directory.Exists(dird) = False) Then IO.Directory.CreateDirectory(dird)

                Players.Data.LoadPlayers(ServerPath & "\web\" & CStr(Functions.Year) & "\data\players-quote.txt", False)

                Dim html As String = Functions.GetPage("https://www.pianetafanta.it/Giocatori-Quotazioni.asp?giornata=&Nome=&Quota=&Quota1=&Squadre21=T&Cerca=Cerca", "POST", "")
                Dim wpl As New Dictionary(Of String, Players.PlayerMatch)

                If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
                If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)

                If html <> "" Then

                    IO.File.WriteAllText(filet, html)

                    Dim line() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding("ISO-8859-1"))
                    Dim nome As String = ""
                    Dim squadra As String = ""
                    Dim qini As String = ""
                    Dim qcur As String = ""
                    Dim ruolo As String = ""
                    Dim strtmp As New StringBuilder
                    Dim start As Boolean = False

                    For i As Integer = 0 To line.Length - 1

                        If line(i).Contains("<tr class=""generico"">") Then
                            start = True
                        End If

                        If start AndAlso line(i) <> "" Then strtmp.AppendLine(line(i))

                        If line(i).Contains("</tr>") Then
                            If start Then
                                Dim s() As String = strtmp.ToString.Split(CChar(System.Environment.NewLine))
                                For k As Integer = 0 To s.Length - 1
                                    If s(k).Contains("images/transparent.gif"" class=""SpriteSquadre ") Then squadra = Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(s(k), "(?<=title="")[a-zA-Z]{1,}(?="")").Value.ToUpper())
                                    If s(k).ToLower.Contains("giocatori-statistiche-personali.asp?nomegio=") Then
                                        s(k) = s(k).Replace("&#8217;", "’")
                                        nome = System.Text.RegularExpressions.Regex.Match(s(k), "(?<=nomegio\=).*(?=\&Ruolo)").Value.ToUpper.Trim
                                        'If nome.EndsWith(".") = False Then nome = nome & "."
                                        ruolo = System.Text.RegularExpressions.Regex.Match(s(k).ToLower, "(?<=ruolo=)[pdca]").Value.ToUpper
                                        nome = Players.Data.ResolveName("", nome, squadra, wpl, True).GetName()
                                    End If
                                Next
                            End If
                            start = False
                            strtmp = New StringBuilder
                        End If
                    Next

                    If Functions.makefileplayer Then IO.File.WriteAllText(filed, Functions.GetDataPlayerMatchedData(wpl, True), System.Text.Encoding.UTF8)

                End If

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Players quotes (" & Functions.Year & "):</span></br>" & strdata.ToString.Replace(System.Environment.NewLine, "</br>") & "</br><span style='color:red;font-size:bold;'>Details:</span></br>" & Functions.GetDataPlayerMatchedData(wpl, False).Replace(System.Environment.NewLine, "</br>")
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Players quotes (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

        End Function

        Shared Function GetPlayersQuoteFantacalcio(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(Functions.Year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(Functions.Year) & "\data"
            Dim filet As String = dirt & "\players-quote.txt"
            Dim filedpq As String = dird & "\players-quote.txt"
            Dim filedpd As String = dird & "\players-data-player.txt"
            Dim strdatapq As New System.Text.StringBuilder
            Dim strdatapd As New System.Text.StringBuilder

            'filet = ServerPath & "\quote.txt"

            Functions.Dirs = ServerPath

            strdatapq.AppendLine("RUOLO|NOME|SQUADRA|QINI|QCUR")

            Try

                If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
                If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)

                Dim html As String = ""
                Dim url As String = "https://www.fantacalcio.it/quotazioni-fantacalcio"
                Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.115 Safari/537.36"
                request.Method = "GET"

                Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                    Using reader As New StreamReader(response.GetResponseStream())
                        html = reader.ReadToEnd()
                    End Using
                End Using

                Dim link As String = ""


                If html <> "" Then

                    IO.File.WriteAllText(filet, html)

                    Dim line() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding("ISO-8859-1"))
                    Dim nome As String = ""
                    Dim ruolo As String = ""
                    Dim squadra As String = ""
                    Dim qini As String = ""
                    Dim qcur As String = ""

                    For i As Integer = 0 To line.Length - 1
                        If line(i).Contains("data-filter-keywords") Then
                            nome = Functions.NormalizeText(System.Text.RegularExpressions.Regex.Match(line(i), "(?<="").*(?="")").Value.ToUpper().Trim().Replace("&#X27;", "'"))
                        End If
                        If line(i).Contains("data-filter-role-classic") Then
                            ruolo = System.Text.RegularExpressions.Regex.Match(line(i), "(?<="").*(?="")").Value.ToUpper().Trim()
                        End If
                        If line(i).Contains("<td class=""player-team"" data-col-key=""sq"">") Then
                            squadra = line(i + 1).Trim()
                            Select Case squadra
                                Case "ATA" : squadra = "ATALANTA"
                                Case "BAR" : squadra = "BARI"
                                Case "BOL" : squadra = "BOLOGNA"
                                Case "CAG" : squadra = "CAGLIARI"
                                Case "CAT" : squadra = "CATANIA"
                                Case "COM" : squadra = "COMO"
                                Case "CRE" : squadra = "CREMONESE"
                                Case "EMP" : squadra = "EMPOLI"
                                Case "FIO" : squadra = "FIORENTINA"
                                Case "FRO" : squadra = "FROSINONE"
                                Case "GEN" : squadra = "GENOA"
                                Case "INT" : squadra = "INTER"
                                Case "JUV" : squadra = "JUVENTUS"
                                Case "LAZ" : squadra = "LAZIO"
                                Case "LEC" : squadra = "LECCE"
                                Case "MIL" : squadra = "MILAN"
                                Case "MON" : squadra = "MONZA"
                                Case "NAP" : squadra = "NAPOLI"
                                Case "PAL" : squadra = "PALERMO"
                                Case "PAR" : squadra = "PARMA"
                                Case "PIS" : squadra = "PISA"
                                Case "ROM" : squadra = "ROMA"
                                Case "SAL" : squadra = "SALERNITANA"
                                Case "SAM" : squadra = "SAMPDORIA"
                                Case "SAS" : squadra = "SASSUOLO"
                                Case "SPE" : squadra = "SPEZIA"
                                Case "TOR" : squadra = "TORINO"
                                Case "UDI" : squadra = "UDINESE"
                                Case "VEN" : squadra = "VENEZIA"
                                Case "VER" : squadra = "VERONA"
                            End Select
                        End If
                        If line(i).Contains("player-classic-initial-price") Then
                            qini = line(i + 1).Trim()
                        End If
                        If line(i).Contains("player-classic-current-price") Then
                            qcur = line(i + 1).Trim()
                            strdatapq.AppendLine(ruolo & "|" & nome & "|" & squadra & "|" & qini & "|" & qcur)
                            strdatapd.AppendLine(ruolo & "|" & nome & "|" & squadra & "|UNK|UNK")
                        End If
                    Next
                    IO.File.WriteAllText(filedpq, strdatapq.ToString, Encoding.UTF8)
                    IO.File.WriteAllText(filedpd, strdatapd.ToString, Encoding.UTF8)
                End If

                Players.Data.LoadPlayers(filedpq, True)

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Players quotes (" & Functions.Year & "):</span></br>" & strdatapq.ToString.Replace(System.Environment.NewLine, "</br>") & "</br>"
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Players quotes (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

        End Function

        Shared Function GetPlayersQuoteFantacalcio2(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(Functions.Year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(Functions.Year) & "\data"
            Dim filet1 As String = dirt & "\players-quote.zip"
            Dim filet2 As String = dirt & "\players-quote.zip"
            Dim filed As String = dird & "\players-quote.txt"
            Dim strdata As New System.Text.StringBuilder

            Functions.Dirs = ServerPath

            strdata.AppendLine("RUOLO|NOME|SQUADRA|QINI|QCUR")

            Try

                Dim html As String = Functions.GetPage("https://www.fantacalcio.it/quotazioni-fantacalcio", "POST", "")
                Dim link As String = ""

                If html <> "" Then

                    IO.File.WriteAllText(filet1, html)

                    Dim line() As String = IO.File.ReadAllLines(filet1, System.Text.Encoding.GetEncoding("ISO-8859-1"))

                    For i As Integer = 0 To line.Length - 1
                        If line(i).Contains("Excel.ashx?type") Then
                            link = System.Text.RegularExpressions.Regex.Match(line(i), "(?<="").*(?="")").Value
                        End If
                    Next
                End If

                If link <> "" Then
                    Using webClient As WebClient = New WebClient()
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                        Dim sdata As Byte() = webClient.DownloadData(link)
                        IO.File.WriteAllBytes(filet2, sdata)
                    End Using
                    If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
                    If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)
                    If IO.Directory.Exists(dirt + "\unzippq") = False Then IO.Directory.CreateDirectory(dirt + "\unzippq")
                    IO.Directory.Delete(dirt + "\unzippq", True)
                    System.IO.Compression.ZipFile.ExtractToDirectory(filet2, dirt + "\unzippq")

                    Dim fsheet1 As String = dirt + "\unzippq\xl\worksheets\sheet1.xml"
                    Dim fsheet2 As String = dirt + "\unzippq\xl\worksheets\sheet1.txt"
                    Dim fstr1 As String = dirt + "\unzippq\xl\sharedStrings.xml"
                    Dim fstr2 As String = dirt + "\unzippq\xl\sharedStrings.xml"

                    If IO.File.Exists(fsheet1) AndAlso IO.File.Exists(fstr1) Then

                        Dim sharedstr As List(Of String) = GetDictionaryString(fstr1, fstr2)

                        Dim str As String = System.Text.RegularExpressions.Regex.Match(IO.File.ReadAllText(fsheet1), "\<row.*\<\/row\>").Value
                        IO.File.WriteAllText(fsheet2, "<rows>" & str & "</rows>")

                        Dim m_xmld As System.Xml.XmlDocument
                        Dim m_nodelist As System.Xml.XmlNodeList
                        Dim m_node As System.Xml.XmlNode

                        m_xmld = New System.Xml.XmlDocument()
                        m_xmld.Load(fsheet2)

                        m_nodelist = m_xmld.SelectNodes("/rows/row")

                        For Each m_node In m_nodelist

                            Dim cellval As New StringBuilder

                            For Each nodemeas As System.Xml.XmlNode In m_node.ChildNodes

                                Dim cell As String = nodemeas.Attributes("r").Value
                                Dim col As String = System.Text.RegularExpressions.Regex.Match(cell, "[A-Z]").Value
                                Dim row As Integer = CInt(System.Text.RegularExpressions.Regex.Match(cell, "\d+").Value)

                                If row > 2 AndAlso "BCDEF".Contains(col) Then
                                    Dim val As String = ""
                                    If nodemeas.Attributes("t") IsNot Nothing Then
                                        Dim sind As Integer = CInt(nodemeas.InnerText)
                                        val = sharedstr(sind)
                                    Else
                                        val = nodemeas.InnerText
                                    End If
                                    Select Case col
                                        Case "B" : val = val.ToUpper()
                                        Case "C" : val = Functions.NormalizeText(val.ToUpper())
                                        Case "D" : val = Functions.CheckTeamName(Functions.NormalizeText(val.ToUpper()))
                                    End Select
                                    cellval.Append("|" & val)
                                End If

                            Next
                            If cellval.Length > 0 Then
                                Dim s() As String = cellval.ToString().Substring(1).Split(CChar("|"))
                                If s.Length = 5 Then
                                    strdata.AppendLine(s(0) & "|" & s(1) & "|" & s(2) & "|" & s(4) & "|" & s(3))
                                End If
                            End If
                        Next
                    End If

                    IO.File.WriteAllText(filed, strdata.ToString, Encoding.UTF8)

                End If

                Players.Data.LoadPlayers(filed, True)

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Players quotes (" & Functions.Year & "):</span></br>" & strdata.ToString.Replace(System.Environment.NewLine, "</br>") & "</br>"
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Players quotes (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

        End Function

        Public Shared Function GetDictionaryString(f1 As String, f2 As String) As List(Of String)

            Dim lst As New List(Of String)

            Try
                Dim str As String = System.Text.RegularExpressions.Regex.Match(IO.File.ReadAllText(f1), "\<si.*\<\/si\>").Value
                IO.File.WriteAllText(f2, "<str>" & str & "</str>")

                Dim m_xmld As System.Xml.XmlDocument
                Dim m_nodelist As System.Xml.XmlNodeList
                Dim m_node As System.Xml.XmlNode

                m_xmld = New System.Xml.XmlDocument()
                m_xmld.Load(f2)

                m_nodelist = m_xmld.SelectNodes("/str/si")

                For Each m_node In m_nodelist
                    lst.Add(m_node.InnerText)
                Next
            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return lst

        End Function
    End Class
End Namespace
