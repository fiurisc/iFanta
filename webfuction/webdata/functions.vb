Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Script.Serialization

Namespace WebData
    Public Class Functions

        Public Enum eMessageType As Integer
            Info = 0
            Alert = 1
            Errors = 2
            Execution = 3
        End Enum

        Public Shared lastClean As Date = Date.Now
        Public Shared makefileplayer As Boolean = True ' Abilita la generazione dei file con la lista dei giocatori trovati'

        Public Shared Function GetJsonPropertyName(Value As String) As String
            Return System.Text.RegularExpressions.Regex.Match(Value, ".*(?=:)").Value().Replace(Convert.ToChar(34), "").Trim()
        End Function

        Public Shared Function GetJsonPropertyValue(Value As String) As String
            Return System.Text.RegularExpressions.Regex.Match(Value, "(?<=:).*").Value().Replace(",", "").Replace(Convert.ToChar(34), "").Trim()
        End Function

        Public Shared Sub WriteLog(appSett As Torneo.PublicVariables, ByVal MessageType As eMessageType, ByVal Message As String)

            Dim flog As String = appSett.RootTorneiPath
            If IO.Directory.Exists(appSett.TorneoPath) Then flog = appSett.TorneoPath

            Try

                Dim sframe As String() = Environment.StackTrace.Split(New String() {vbCrLf}, StringSplitOptions.None)

                'Detemrino gli oggetti chiamanti'
                Dim callers As List(Of String) = New List(Of String)
                Dim methodfilter As List(Of String) = New List(Of String) From {"Functions.WriteLog", "System.Environment.GetStackTrace", "System.Environment.get_StackTrace"}

                For i As Integer = 0 To sframe.Length - 1
                    If sframe(i).Contains(" at ") OrElse sframe(i).Contains(") in ") Then
                        Dim method As String = sframe(i).Replace(" at ", "").Trim().Replace(" in ", "|")
                        If method.Contains("|") Then method = method.Split(Convert.ToChar("|"))(0)
                        If method.Contains("(") Then method = method.Substring(0, method.IndexOf("("))
                        If methodfilter.Where(Function(x) method.Contains(x)).ToList().Count() = 0 Then
                            If callers.Contains(method) Then callers.Remove(method)
                            callers.Add(method)
                        End If
                    End If
                Next
                callers.Reverse()

                Dim methodname As String = If(callers.Count > 0, callers(callers.Count - 1), "")
                Dim msgtypes As String = "INFO"

                If methodname.Length > 200 Then methodname = methodname.Substring(0, 200)
                If Message.Length > 500 Then Message = Message.Substring(0, 500)

                If MessageType = eMessageType.Alert Then
                    msgtypes = "ALERT"
                ElseIf MessageType = eMessageType.Errors Then
                    msgtypes = "ERROR"
                ElseIf (MessageType = eMessageType.Execution) Then
                    msgtypes = "EXE"
                End If

                IO.File.AppendAllText(flog & "debug.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & msgtypes & "|" & methodname & "|" & Message & System.Environment.NewLine)
                If msgtypes = "ERROR" Then IO.File.AppendAllText(flog & "errors.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & msgtypes & "|" & methodname & "|" & Message & System.Environment.NewLine)

                If Date.Now.Subtract(lastClean).TotalHours > 1 Then
                    ResizeFile(flog & "debug.log")
                    ResizeFile(flog & "errors.log")
                    lastClean = Date.Now
                End If

            Catch ex As Exception

            End Try
        End Sub

        Private Shared Sub ResizeFile(fname As String)

            If IO.File.Exists(fname) Then

                Dim lines As List(Of String) = File.ReadAllLines(fname).ToList()
                Dim maxlines As Integer = 1000

                If lines.Count > maxlines Then
                    lines.RemoveRange(0, lines.Count - maxlines)
                    IO.File.WriteAllLines(fname, lines)
                End If
            End If

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
                If prop.Name <> "RecordId" Then
                    Dim value = prop.GetValue(obj, Nothing)
                    If value IsNot Nothing Then
                        hash.Append(value.ToString())
                    End If
                End If
            Next

            ' Riduci il valore a Integer in modo sicuro
            Return hash.ToString()

        End Function

        Public Shared Function DeserializeJson(Of T)(json As String) As T
            Dim serializer As New JavaScriptSerializer()
            ' Esegue la deserializzazione JSON -> T
            Dim obj As T = serializer.Deserialize(Of T)(json)
            Return obj
        End Function

        Public Shared Function SerializzaOggetto(obj As Object, compatta As Boolean) As String
            If obj Is Nothing Then Return "{}"
#If DEBUG Then
            compatta = False
#End If
            Dim serializer As New JavaScriptSerializer()
            Dim json As String = serializer.Serialize(obj)
            If compatta Then
                Return serializer.Serialize(obj)
            Else
                Return FormatJson(serializer.Serialize(obj))
            End If
        End Function

        Public Shared Function FormatJson(json As String) As String

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

            If txt.Contains("distrazione muscolare al bicipite") Then
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
            txt = txt.Replace("a\u0027", "a'")
            Dim regex As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\'\s\.\-\’ª]{0,}")
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
            Squadra = Squadra.ToUpper.Replace("CHIEVOVERONA", "CHIEVO")
            Squadra = Squadra.ToUpper.Replace("CHIEVO-VERONA", "CHIEVO")
            Squadra = Squadra.ToUpper.Replace("CHIEVO VERONA", "CHIEVO")
            Squadra = Squadra.ToUpper.Replace("HELLASVERONA", "VERONA")
            Squadra = Squadra.ToUpper.Replace("HELLAS-VERONA", "VERONA")
            Squadra = Squadra.ToUpper.Replace("HELLAS VERONA", "VERONA")
            Squadra = Squadra.ToUpper.Replace("HELLAS", "VERONA")
            Squadra = Squadra.ToUpper.Replace("SSC ", "")
            Squadra = Squadra.ToUpper.Replace(" SSC", "")
            Squadra = Squadra.ToUpper.Replace(" CALCIO", "")
            Squadra = Squadra.ToUpper.Replace("CALCIO ", "")
            Squadra = Squadra.ToUpper.Replace("FC ", "")
            Squadra = Squadra.ToUpper.Replace(" FC", "")
            If Squadra = "GEN" Then Squadra = "GENOA"
            If Squadra = "CAG" Then Squadra = "CAGLIARI"
            If Squadra.ToUpper() = "JUVE" Then Squadra = "JUVENTUS"
            Squadra = Squadra.Trim()
            Return Squadra
        End Function

        Public Shared Function GetTeamNameFromCode(Squadra As String) As String
            Select Case Squadra
                Case "ATA" : Squadra = "ATALANTA"
                Case "BAR" : Squadra = "BARI"
                Case "BOL" : Squadra = "BOLOGNA"
                Case "CAG" : Squadra = "CAGLIARI"
                Case "CAT" : Squadra = "CATANIA"
                Case "COM" : Squadra = "COMO"
                Case "CRE" : Squadra = "CREMONESE"
                Case "EMP" : Squadra = "EMPOLI"
                Case "FIO" : Squadra = "FIORENTINA"
                Case "FRO" : Squadra = "FROSINONE"
                Case "GEN" : Squadra = "GENOA"
                Case "INT" : Squadra = "INTER"
                Case "JUV" : Squadra = "JUVENTUS"
                Case "LAZ" : Squadra = "LAZIO"
                Case "LEC" : Squadra = "LECCE"
                Case "MIL" : Squadra = "MILAN"
                Case "MON" : Squadra = "MONZA"
                Case "NAP" : Squadra = "NAPOLI"
                Case "PAL" : Squadra = "PALERMO"
                Case "PAR" : Squadra = "PARMA"
                Case "PIS" : Squadra = "PISA"
                Case "ROM" : Squadra = "ROMA"
                Case "SAL" : Squadra = "SALERNITANA"
                Case "SAM" : Squadra = "SAMPDORIA"
                Case "SAS" : Squadra = "SASSUOLO"
                Case "SPE" : Squadra = "SPEZIA"
                Case "TOR" : Squadra = "TORINO"
                Case "UDI" : Squadra = "UDINESE"
                Case "VEN" : Squadra = "VENEZIA"
                Case "VER" : Squadra = "VERONA"
            End Select
            Return Squadra
        End Function

        Public Shared Function GetNatCode(NatCode As String) As String
            Select Case NatCode
                Case "REP" : NatCode = "CZE"
                Case "ING" : NatCode = "GBR"
                Case "ENG" : NatCode = "GBR"
                Case "CHI" : NatCode = "CHL"
                Case "GRE" : NatCode = "GRC"
                Case "GUI" : NatCode = "GIN"
                Case "CRO" : NatCode = "HRV"
                Case "SUI" : NatCode = "CHE"
                Case "DAN" : NatCode = "DNK"
                Case "GAM" : NatCode = "GMB"
                Case "POR" : NatCode = "PRT"
                Case "PAR" : NatCode = "PRY"
                Case "NED" : NatCode = "NLD"
                Case "GER" : NatCode = "DEU"
                Case "CRC" : NatCode = "CRI"
                Case "BUL" : NatCode = "BGR"
                Case "URU" : NatCode = "URY"
                Case "ALG" : NatCode = "DZA"
                Case "SLO" : NatCode = "SVK"
                Case "CIL" : NatCode = "CHL"
                Case "MOL" : NatCode = "MDA"
                Case "SVE" : NatCode = "SWE"
                Case "SER" : NatCode = "SVK"
                Case "SPA" : NatCode = "ESP"
                Case "ROM" : NatCode = "ROU"
                Case "OLA" : NatCode = "NLD"
                Case "COS" : NatCode = "CIV"
                Case "SVI" : NatCode = "SWZ"
                Case "BOS" : NatCode = "BIH"
                Case "GIA" : NatCode = "JAM"
                Case "SNV" : NatCode = "SVN"
                Case "UNG" : NatCode = "HUN"
                Case "LIB" : NatCode = "LBY"
                Case "LIT" : NatCode = "LTU"
                Case "NIG" : NatCode = "NER"
                Case "IRA" : NatCode = "IRN"
                Case "MON" : NatCode = "MNE"
                Case "EGI" : NatCode = "EGY"
                Case "ANG" : NatCode = "AGO"
                Case "MES" : NatCode = "MEX"
                Case "CAM" : NatCode = "CMR"
                Case "MAL" : NatCode = "MLI"
                Case "COR" : NatCode = "KOR"
                Case "GUA" : NatCode = "GLP"
                Case "DEN" : NatCode = "DNK"
                Case "KVX" : NatCode = "RKS"
            End Select
            Return NatCode
        End Function

        Public Shared Function GetDicNatCodeList(fname As String) As Dictionary(Of String, String)

            Dim dicNatCode As New Dictionary(Of String, String)

            If IO.File.Exists(fname) Then
                Dim line() As String = IO.File.ReadAllLines(fname)
                For i As Integer = 0 To line.Length - 1
                    Dim s() As String = line(i).Split(CChar(","))
                    If s.Length = 2 Then
                        If dicNatCode.ContainsKey(s(0)) = False Then dicNatCode.Add(s(0), s(1))
                    End If
                Next
            End If

            Return dicNatCode

        End Function

        Public Shared Function GetDataPlayerMatchedData(appSett As Torneo.PublicVariables, wp As Dictionary(Of String, Players.PlayerMatch), PlayerList As Boolean) As String

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
                WriteLog(appSett, eMessageType.Errors, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Sub WriteDataPlayerMatch(appSett As Torneo.PublicVariables, wp As Dictionary(Of String, Players.PlayerMatch), filed As String)
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
                WriteLog(appSett, eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Shared Function GetPage(appSett As Torneo.PublicVariables, ByVal Url As String, Optional Encoding As String = "ISO-8859-1") As String

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
                WriteLog(appSett, eMessageType.Errors, ex.Message)
                responseFromServer = ex.Message
            End Try

            Return responseFromServer

        End Function

    End Class
End Namespace