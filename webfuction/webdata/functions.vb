Imports System.Data.OleDb
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Script.Serialization

Namespace WebData
    Public Class Functions

        Public Shared Property Year As String = ""
        Public Shared Property DataPath As String = ""

        Public Shared makefileplayer As Boolean = True ' Abilita la generazione dei file con la lista dei giocatori trovati'

        Public Shared Sub InitPath(rootDataPath As String, rootdatabasePath As String)
            DataPath = rootDataPath & Year & "\"
            Torneo.Functions.InitPath(rootDataPath, rootdatabasePath, Year)
        End Sub

        Public Shared Sub WriteLog(dirs As String, ByVal Form As String, ByVal SubName As String, ByVal Text As String)
            Try
                If IO.Directory.Exists(dirs) Then IO.File.AppendAllText(dirs & "\debug.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & Form & "|" & SubName & "|" & Text & System.Environment.NewLine)
            Catch ex As Exception

            End Try
        End Sub

        Public Shared Sub WriteError(ByVal Form As String, ByVal SubName As String, ByVal ErrMsg As String)
            Try
                If IO.Directory.Exists(DataPath) Then IO.File.AppendAllText(DataPath & "\error.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & Form & "|" & SubName & "|" & ErrMsg & System.Environment.NewLine)
            Catch ex As Exception

            End Try
        End Sub

        Public Shared Sub MakeDirectory()

            Dim dirt As String = DataPath & "\temp"
            Dim dird As String = DataPath & "\data"
            Dim dirdpf As String = DataPath & "\data\pforma"
            Dim dirdmt As String = DataPath & "\data\matchs"

            If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
            If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)
            If IO.Directory.Exists(dirdpf) = False Then IO.Directory.CreateDirectory(dirdpf)
            If IO.Directory.Exists(dirdmt) = False Then IO.Directory.CreateDirectory(dirdmt)

        End Sub

        Public Shared Function ConvertListIntegerToString(List As List(Of Integer), Separator As String) As String
            Dim str As String = ""
            For i As Integer = 0 To List.Count - 1
                str = str & Separator & List(i)
            Next
            If str.Length > 0 Then
                Return str.Substring(1)
            Else
                Return ""
            End If
        End Function

        Public Shared Function ConvertListStringToString(List As List(Of String), Separator As String) As String
            Dim str As String = ""
            For i As Integer = 0 To List.Count - 1
                str = str & Separator & List(i)
            Next
            If str.Length > 0 Then
                Return str.Substring(1)
            Else
                Return ""
            End If
        End Function

        Public Shared Function ComputeSHA256Hash(ByVal rawData As String) As String
            Using sha256Hash As SHA256 = SHA256.Create()
                ' Convert the input string to a byte array and compute the hash.
                Dim bytes As Byte() = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData))

                ' Convert byte array to a hexadecimal string.
                Dim builder As New StringBuilder()
                For Each b As Byte In bytes
                    builder.Append(b.ToString("x2"))
                Next
                Return builder.ToString()
            End Using
        End Function

        Public Shared Function GetCustomHashCode(obj As Object) As String

            If obj Is Nothing Then Return ""

            Dim hash As New StringBuilder
            Dim props As PropertyInfo() = obj.GetType().GetProperties(BindingFlags.Public Or BindingFlags.Instance)

            For Each prop In props
                Dim value = prop.GetValue(obj, Nothing)
                If value IsNot Nothing Then
                    hash.Append(value.ToString())
                End If
            Next

            ' Riduci il valore a Integer in modo sicuro
            Return hash.ToString()

        End Function


        Public Shared Function DeserializeJson(Of T)(json As String) As T
            Dim serializer As New JavaScriptSerializer()
            Try
                ' Esegue la deserializzazione JSON -> T
                Dim obj As T = serializer.Deserialize(Of T)(json)
                Return obj
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Shared Function SerializzaOggetto(obj As Object, compatta As Boolean) As String
            If obj Is Nothing Then Return "{}"
            Dim serializer As New JavaScriptSerializer()
            Dim json As String = serializer.Serialize(obj)
            If compatta Then
                Return serializer.Serialize(obj)
            Else
                Return FormatJson(serializer.Serialize(obj))
            End If
        End Function

        Private Shared Function FormatJson(json As String) As String

            Dim indent As Integer = 0
            Dim quoted As Boolean = False
            Dim sb As New StringBuilder()

            For Each ch As Char In json
                Select Case ch
                    Case """"c
                        sb.Append(ch)
                        quoted = Not quoted
                    Case "{"c, "["c
                        sb.Append(ch)
                        If Not quoted Then
                            sb.AppendLine()
                            indent += 1
                            sb.Append(New String(" "c, indent * 2))
                        End If
                    Case "}"c, "]"c
                        If Not quoted Then
                            sb.AppendLine()
                            indent -= 1
                            sb.Append(New String(" "c, indent * 2))
                        End If
                        sb.Append(ch)
                    Case ","c
                        sb.Append(ch)
                        If Not quoted Then
                            sb.AppendLine()
                            sb.Append(New String(" "c, indent * 2))
                        End If
                    Case ":"c
                        sb.Append(ch)
                        If Not quoted Then sb.Append(" ")
                    Case Else
                        sb.Append(ch)
                End Select
            Next

            Return sb.ToString()
        End Function

        Public Shared Function CompactJson(jsonFormattato As String) As String
            Dim risultato As New Text.StringBuilder()
            Dim dentroStringa As Boolean = False

            For Each c As Char In jsonFormattato
                Select Case c
                    Case """"c
                        ' Toggle stato: dentro o fuori da una stringa
                        risultato.Append(c)
                        dentroStringa = Not dentroStringa
                    Case " "c, vbTab(0), vbCr(0), vbLf(0)
                        ' Ignora spazi e formattazione se fuori da una stringa
                        If dentroStringa Then
                            risultato.Append(c)
                        End If
                    Case Else
                        risultato.Append(c)
                End Select
            Next

            Return risultato.ToString()
        End Function


        Public Shared Function NormalizeText(ByVal txt As String) As String

            If txt.Contains("MARTINEZ") Then
                txt = txt
            End If

            txt = txt.Replace("Í", "I")
            txt = txt.Replace("í", "i")
            txt = txt.Replace("Ç", "C")
            txt = txt.Replace("-", " ")
            txt = txt.Replace("'", "’")
            txt = txt.Replace("&#39;", "’")
            txt = txt.Replace("Ò", "O’")
            txt = txt.Replace("Ô", "O")
            txt = txt.Replace("&#210;", "O’")
            txt = txt.Replace("É", "E’")
            txt = txt.Replace("&#201;", "E’")
            txt = txt.Replace("&#XEC;", "I")
            txt = txt.Replace("&#193;", "A")
            txt = txt.Replace("Ã²", "O’")
            txt = txt.Replace("Ã¨", "A")
            txt = txt.Replace("Ã", "A")
            txt = txt.Replace("Ó", "O")
            txt = txt.Replace("&#211;", "O")
            txt = txt.Replace("Í", "I")
            txt = txt.Replace("&#205;", "I")
            txt = txt.Replace("È", "E")
            txt = txt.Replace("&#200;", "E")
            txt = txt.Replace("Á", "A")
            txt = txt.Replace("À", "A’")
            txt = txt.Replace("&#192;", "A’")
            txt = txt.Replace("Ú", "U")
            txt = txt.Replace("&#218;", "U")
            txt = txt.Replace("Ñ", "N")
            txt = txt.Replace("&#209;", "N")
            txt = txt.Replace("&#39;", "'")
            txt = txt.Replace("â", "A’")
            txt = txt.Replace("&#X2019;", "’").Replace("&#x2019;", "’")
            txt = txt.Replace("&#X2019;", "’").Replace("&#x2019;", "’")
            txt = txt.Replace("&#X27;", "’").Replace("&#x27;", "’")
            txt = txt.Replace("&apos;", "’")
            txt = txt.Replace("&#XE0;", "a'").Replace("&#xE0;", "a'")
            txt = txt.Replace("&#XE8;", "E’").Replace("&#xE8;", "E’")
            txt = txt.Replace("&#XF2;", "O").Replace("&#xF2;", "O")
            Dim regex As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\'\s\.\-\’]{0,}")
            Dim newtxt As String = ""

            For Each match As System.Text.RegularExpressions.Match In regex.Matches(txt)
                If match.Value.Trim <> "" Then newtxt = newtxt & match.Value
            Next

            Return newtxt

        End Function

        Public Shared Function CleanSpecialChar(txt As String) As String
            txt = txt.Replace("’", "")
            txt = txt.Replace(".", "")
            Return txt
        End Function

        Public Shared Function CheckTeamName(ByVal Squadra As String) As String
            Squadra = Squadra.ToUpper.Replace("CHIEVOVERONA", "CHIEVO").Trim
            Squadra = Squadra.ToUpper.Replace("CHIEVO-VERONA", "CHIEVO").Trim
            Squadra = Squadra.ToUpper.Replace("CHIEVO VERONA", "CHIEVO").Trim
            Squadra = Squadra.ToUpper.Replace("HELLASVERONA", "VERONA").Trim
            Squadra = Squadra.ToUpper.Replace("HELLAS-VERONA", "VERONA").Trim
            Squadra = Squadra.ToUpper.Replace("HELLAS VERONA", "VERONA").Trim
            If Squadra = "GEN" Then Squadra = "GENOA"
            If Squadra = "CAG" Then Squadra = "CAGLIARI"
            If Squadra.ToUpper = "JUVE" Then Squadra = "JUVENTUS"
            Return Squadra
        End Function

        Public Shared Function GetTeamNameFromCode(squadra As String) As String
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
            Return squadra
        End Function

        Public Shared Function GetNatCode(natcode As String) As String
            Select Case natcode
                Case "REP" : natcode = "CZE"
                Case "ING" : natcode = "GBR"
                Case "ENG" : natcode = "GBR"
                Case "CHI" : natcode = "CHL"
                Case "GRE" : natcode = "GRC"
                Case "GUI" : natcode = "GIN"
                Case "CRO" : natcode = "HRV"
                Case "SUI" : natcode = "CHE"
                Case "DAN" : natcode = "DNK"
                Case "GAM" : natcode = "GMB"
                Case "POR" : natcode = "PRT"
                Case "PAR" : natcode = "PRY"
                Case "NED" : natcode = "NLD"
                Case "GER" : natcode = "DEU"
                Case "CRC" : natcode = "CRI"
                Case "BUL" : natcode = "BGR"
                Case "URU" : natcode = "URY"
                Case "ALG" : natcode = "DZA"
                Case "SLO" : natcode = "SVK"
                Case "CIL" : natcode = "CHL"
                Case "MOL" : natcode = "MDA"
                Case "SVE" : natcode = "SWE"
                Case "SER" : natcode = "SVK"
                Case "SPA" : natcode = "ESP"
                Case "ROM" : natcode = "ROU"
                Case "OLA" : natcode = "NLD"
                Case "COS" : natcode = "CIV"
                Case "SVI" : natcode = "SWZ"
                Case "BOS" : natcode = "BIH"
                Case "GIA" : natcode = "JAM"
                Case "SNV" : natcode = "SVN"
                Case "UNG" : natcode = "HUN"
                Case "LIB" : natcode = "LBY"
                Case "LIT" : natcode = "LTU"
                Case "NIG" : natcode = "NER"
                Case "IRA" : natcode = "IRN"
                Case "MON" : natcode = "MNE"
                Case "EGI" : natcode = "EGY"
                Case "ANG" : natcode = "AGO"
                Case "MES" : natcode = "MEX"
                Case "CAM" : natcode = "CMR"
                Case "MAL" : natcode = "MLI"
                Case "COR" : natcode = "KOR"
                Case "GUA" : natcode = "GLP"
                Case "DEN" : natcode = "DNK"
                Case "KVX" : natcode = "RKS"
            End Select
            Return natcode
        End Function

        Public Shared Function GetDicNatCodeList(fname As String) As Dictionary(Of String, String)

            Dim dicnatcode As New Dictionary(Of String, String)

            If IO.File.Exists(fname) Then
                Dim line() As String = IO.File.ReadAllLines(fname)
                For i As Integer = 0 To line.Length - 1
                    Dim s() As String = line(i).Split(CChar(","))
                    If s.Length = 2 Then
                        If dicnatcode.ContainsKey(s(0)) = False Then dicnatcode.Add(s(0), s(1))
                    End If
                Next
            End If

            Return dicnatcode

        End Function

        Public Shared Function GetDataPlayerMatchedData(wp As Dictionary(Of String, Players.PlayerMatch), PlayerList As Boolean) As String

            Dim strdata As New System.Text.StringBuilder

            Try


                Dim notfound As New List(Of Players.WebPlayer)

                For Each pkey As String In wp.Keys
                    If wp(pkey).Matched = False Then notfound.Add(wp(pkey).SourcePlayer)
                Next

                If PlayerList Then
                    strdata.AppendLine("***List players***")
                    For Each pkey As String In wp.Keys
                        strdata.AppendLine(wp(pkey).SourcePlayer.Role & "|" & wp(pkey).SourcePlayer.Name & "|" & wp(pkey).SourcePlayer.Team & "|" & wp(pkey).MatchedPlayer.Role & "|" & wp(pkey).MatchedPlayer.Name & "|" & wp(pkey).MatchedPlayer.Team)
                    Next
                    strdata.AppendLine("")
                    strdata.AppendLine("***List players not found***")
                    For i As Integer = 0 To notfound.Count - 1
                        strdata.AppendLine(notfound(i).Role & "|" & notfound(i).Name & "|" & notfound(i).Team)
                    Next
                    strdata.AppendLine("")
                End If

                strdata.AppendLine("***Report data***")
                strdata.AppendLine("Number of players = " & wp.Count)
                strdata.AppendLine("Number of players not found = " & notfound.Count)
                If wp.Count > 0 Then
                    strdata.AppendLine("Percentage players not found = " & CInt((notfound.Count * 1000) / wp.Count) / 10 & "%")
                Else
                    strdata.AppendLine("Percentage players not found = 0%")
                End If

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Sub WriteDataPlayerMatch(wp As Dictionary(Of String, Players.PlayerMatch), filed As String)
            Try

                Dim strdata As New System.Text.StringBuilder
                Dim notfound As New List(Of Players.WebPlayer)

                strdata.AppendLine("***List players***")

                For Each pkey As String In wp.Keys
                    strdata.AppendLine(wp(pkey).SourcePlayer.Role & "|" & wp(pkey).SourcePlayer.Name & "|" & wp(pkey).SourcePlayer.Team & "|" & wp(pkey).MatchedPlayer.Role & "|" & wp(pkey).MatchedPlayer.Name & "|" & wp(pkey).MatchedPlayer.Team)
                    If wp(pkey).Matched = False Then notfound.Add(wp(pkey).SourcePlayer)
                Next
                strdata.AppendLine("")
                strdata.AppendLine("***List players not found***")
                For i As Integer = 0 To notfound.Count - 1
                    strdata.AppendLine(notfound(i).Role & "|" & notfound(i).Name & "|" & notfound(i).Team)
                Next
                strdata.AppendLine("")
                strdata.AppendLine("***Report data***")
                strdata.AppendLine("Number of players = " & wp.Count)
                strdata.AppendLine("Number of players not found = " & notfound.Count)
                If wp.Count > 0 Then
                    strdata.AppendLine("Percentage players not found = " & CInt((notfound.Count * 1000) / wp.Count) / 10 & "%")
                Else
                    strdata.AppendLine("Percentage players not found = 0%")
                End If

                IO.File.WriteAllText(filed, strdata.ToString, System.Text.Encoding.UTF8)

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Public Shared Function GetPage(ByVal Url As String, Optional Encoding As String = "ISO-8859-1") As String

            Dim responseFromServer As String = ""

#If DEBUG Then
            If Url.Contains("www.fantacalcio.it") Then
                Url = "https://www.ifantacalcio.it/site.php?url=" & Url
            End If
#End If

            Try

                ' Crea la richiesta
                Dim request As HttpWebRequest = CType(WebRequest.Create(Url), HttpWebRequest)
                request.Method = "GET"
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"

                ' Ottieni la risposta
                Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                    Using reader As New StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(Encoding))
                        responseFromServer = reader.ReadToEnd()
                    End Using
                End Using

            Catch ex As Exception
                responseFromServer = ex.Message
            End Try

            Return responseFromServer

        End Function

    End Class
End Namespace