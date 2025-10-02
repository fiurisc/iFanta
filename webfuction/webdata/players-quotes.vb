Imports System.IO
Imports System.Net
Imports System.Text

Namespace WebData

    Public Class PlayersQuotes

        Shared Function GetDataFileName() As String
            Return Functions.DataPath & "data\players-quotes.json"
        End Function

        Shared Function GetPlayersQuotes(ReturnData As Boolean) As String

            Dim dirTemp As String = Functions.DataPath & "temp\"
            Dim dirData As String = Functions.DataPath & "data\"
            Dim fileJson As String = GetDataFileName()
            Dim fileTemp As String = dirTemp & Path.GetFileNameWithoutExtension(GetDataFileName()) & ".html.txt"
            Dim fileLog As String = dirData & Path.GetFileNameWithoutExtension(GetDataFileName) & ".log"
            Dim strdata As String = ""
            Dim playersq As New List(Of Torneo.Players.PlayerQuotesItem)

            Try

                Functions.MakeDirectory()

                Dim html As String = Functions.GetPage("https://www.fantacalcio.it/quotazioni-fantacalcio")

                If html <> "" Then

                    IO.File.WriteAllText(fileTemp, html)

                    Dim line() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.GetEncoding("ISO-8859-1"))
                    Dim Nome As String = ""
                    Dim Ruolo As String = ""
                    Dim Squadra As String = ""
                    Dim Qini As String = ""
                    Dim Qcur As String = ""

                    For i As Integer = 0 To line.Length - 1

                        'Leggo il Nome'
                        If line(i).Contains("data-filter-keywords") Then
                            Nome = Functions.NormalizeText(System.Text.RegularExpressions.Regex.Match(line(i), "(?<="").*(?="")").Value.ToUpper().Trim().Replace("&#X27;", "'"))
                        End If
                        'Leggo il Ruolo'
                        If line(i).Contains("data-filter-role-classic") Then
                            Ruolo = System.Text.RegularExpressions.Regex.Match(line(i), "(?<="").*(?="")").Value.ToUpper().Trim()
                        End If
                        'Leggo la Squadra'
                        If line(i).Contains("<td class=""player-team"" data-col-key=""sq"">") Then
                            Squadra = Functions.GetTeamNameFromCode(line(i + 1).Trim())
                        End If
                        'Leggo la quotazione iniziale'
                        If line(i).Contains("player-classic-initial-price") Then
                            Qini = line(i + 1).Trim()
                        End If
                        'Leggo la quotazione iniziale'
                        If line(i).Contains("player-classic-current-price") Then
                            Qcur = line(i + 1).Trim()
                            playersq.Add(New Torneo.Players.PlayerQuotesItem(Ruolo, Nome, Squadra, CInt(Qini), CInt(Qcur)))
                        End If
                    Next

                    If Torneo.PublicVariables.dataFromDatabase Then
                        Torneo.Players.UpdatePlayersQuotes(playersq)
                    End If

                    strdata = Functions.SerializzaOggetto(playersq, False)

                    IO.File.WriteAllText(fileJson, strdata)

                End If

                Players.Data.LoadPlayers(True)

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

        Shared Function GetPlayersQuoteFantacalcio2(ReturnData As Boolean) As String

            Dim dirt As String = Functions.DataPath & "\temp"
            Dim dird As String = Functions.DataPath & "\data"
            Dim filet1 As String = dirt & "\players-quote.zip"
            Dim filet2 As String = dirt & "\players-quote.zip"
            Dim filed As String = dird & "\players-quote.txt"
            Dim strdata As New System.Text.StringBuilder

            strdata.AppendLine("Ruolo|Nome|Squadra|Qini|Qcur")

            Try

                Dim html As String = Functions.GetPage("https://www.fantacalcio.it/quotazioni-fantacalcio")
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

                Players.Data.LoadPlayers(True)

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
