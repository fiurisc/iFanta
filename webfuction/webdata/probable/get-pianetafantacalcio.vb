
Namespace WebData
    Partial Class ProbableFormations

        Public Function GetPianetaFantacalcio(ReturnData As Boolean) As String
            Return GetPianetaFantacalcio(ReturnData, False)
        End Function

        Public Function GetPianetaFantacalcio(ReturnData As Boolean, FromBackup As Boolean, Optional Giornata As Integer = -1) As String

            FromBackup = False
            Dim currgg As Integer = Giornata
            Dim dirt As String = appSett.WebDataPath & "\temp"
            Dim dird As String = appSett.WebDataPath & "\data\pforma"
            Dim site As String = "PianetaFantacalcio"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".json"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"
            Dim fileBakupHtml As String = GetBackupHtmlDataFileName(site.ToLower(), currgg)
            Dim rmsg As String = ""
            Dim sr As New IO.StreamWriter(fileLog)

            Try

                sr.WriteLine("Loading web player and matchs")
                Players.Data.LoadPlayers(appSett, False)

                sr.WriteLine("Year -> " & appSett.Year)
                sr.WriteLine("Calendario match:")
                sr.WriteLine("---------------------------")
                For Each t As String In mdataw.KeyMatchs.Keys
                    sr.WriteLine(mdataw.KeyMatchs(t).Giornata & " -> " & t)
                Next
                sr.WriteLine("")

                'Determino i link delle varie partite'
                sr.WriteLine("Get Html page")

                Dim html As String = ""

                If FromBackup Then
                    fileTemp = fileBakupHtml
                    If IO.File.Exists(fileBakupHtml) Then html = "ok"
                Else
                    html = GetMatchList(fileTemp)
                End If


                If html <> "" Then

                    Dim line() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.Default)

                    sr.WriteLine("Reading html page")

                    Dim start As Boolean = False
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim ruolo As String = ""
                    Dim wpd As New Torneo.ProbablePlayers.Probable
                    Dim wpl As New Dictionary(Of String, Players.PlayerMatch)

                    Dim lines() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.Default)

                    sr.WriteLine("lines => " & lines.Length)

                    For i As Integer = 0 To lines.Length - 1

                        lines(i) = lines(i).Replace(vbTab, "")

                        If lines(i) <> "" Then

                            If lines(i).Contains("<--matchpartita") Then
                                Dim tmpList As List(Of String) = System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=\=).*(?=\--)").Value.Split(Convert.ToChar("-")).ToList()
                                For Each t As String In tmpList
                                    sq.Add(Functions.CheckTeamName(Functions.NormalizeText(t.ToUpper())))
                                Next
                                sqid = -1
                            ElseIf lines(i).Contains("Giornata <!-- -->") Then

                                Dim ms As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(lines(i), "(Giornata \<!-- --\>\d+)|(TITOLARI \(<!-- -->\d+<!-- -->\))|(PANCHINA \(<!-- -->\d+<!-- -->\))|(INDISPONIBILI\<\/div\>)|(BALLOTTAGGI\<\/div\>)|line-height:1"">[PDCA]{1}<\/span>|(href=""(\/giocatori\/[^""]+)"">([^<]+)<\/a>)|(:\s+<!--\s+-->(.*?)<\/div>)")

                                For k As Integer = 0 To ms.Count - 1
                                    Dim m As System.Text.RegularExpressions.Match = ms(k)
                                    If m.Value.Contains("Giornata") Then
                                        currgg = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(m.Value, "\d+").Value)
                                    ElseIf m.Value.Contains("TITOLARI") Then
                                        pstate = "Titolare"
                                        ruolo = ""
                                        sqid += 1
                                    ElseIf m.Value.Contains("PANCHINA") Then
                                        pstate = "Panchina"
                                        ruolo = ""
                                    ElseIf m.Value.Contains("INDISPONIBILI") Then
                                        pstate = "Infortunato"
                                        ruolo = ""
                                    ElseIf m.Value.Contains("BALLOTTAGGI") Then
                                        pstate = ""
                                        ruolo = ""
                                    ElseIf m.Value.Contains("line-height") Then
                                        ruolo = System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=\>)[PDCA]{1}(?=\<)").Value
                                    ElseIf m.Value.Contains("href=""/giocatori/") AndAlso pstate <> "" AndAlso sqid < 2 Then
                                        Dim name As String = System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=\>).*(?=\<)").Value
                                        Dim info As String = ""
                                        name = Players.Data.ResolveName(ruolo, name, sq(sqid), wpl, True).GetName()
                                        If pstate = "Infortunato" AndAlso ms(k + 1).Value.Contains("<!-- -->") Then
                                            info = Functions.NormalizeText(System.Text.RegularExpressions.Regex.Match(ms(k + 1).Value, "(?<=\>).*(?=\<)").Value)
                                        End If
                                        Call AddInfo(name, sq(sqid), site, pstate, info, -1, wpd.Players)
                                    End If
                                Next
                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        wpd.Day = currgg
                        fileBakupHtml = GetBackupHtmlDataFileName(site.ToLower(), currgg)
                        If dicMatchDays(currgg) > 0 AndAlso FromBackup = False Then WriteBackupProbableHtml(fileTemp, fileBakupHtml)
                        Dim fileBackup As String = dirData & currgg & "\" & site.ToLower() & ".json"
                        Dim out As String = WriteData(wpd, fileData, If(dicMatchDays(currgg) > 0 OrElse Giornata <> -1, fileBackup, ""))
                        If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(appSett, wpl, filePlayers)
                        rmsg = out.Replace(System.Environment.NewLine, "</br>")
                    End If
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                rmsg = ex.Message
            End Try

            sr.Close()

            Return rmsg

        End Function

        Public Function GetMatchList(fileTemp As String) As String

            Dim html As String = Functions.GetPage(appSett, "https://www.pianetafanta.it/probabili-formazioni-fantacalcio", "UTF-8")
            IO.File.WriteAllText(fileTemp, html, New System.Text.UTF8Encoding(False))

            Dim lines() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.Default)
            Dim links As New List(Of String)

            For Each line As String In lines
                If line.Contains("/probabili-formazioni-fantacalcio?") Then
                    Dim ms As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(line, "(?<=href="")(/probabili-formazioni-fantacalcio\?partita[^""]+)")
                    For Each m As System.Text.RegularExpressions.Match In ms
                        links.Add("https://www.pianetafanta.it" & m.Value)
                    Next
                End If
            Next

            Dim stringBuilder As New System.Text.StringBuilder()

            For Each link As String In links
                stringBuilder.AppendLine("<--match" & System.Text.RegularExpressions.Regex.Match(link, "partita=([a-z\-]+)").Value & "-->")
                stringBuilder.AppendLine(Functions.GetPage(appSett, link, "UTF-8"))
            Next

            html = stringBuilder.ToString()

            IO.File.WriteAllText(fileTemp, html, New System.Text.UTF8Encoding(False))

            Return "ok"

        End Function
    End Class
End Namespace