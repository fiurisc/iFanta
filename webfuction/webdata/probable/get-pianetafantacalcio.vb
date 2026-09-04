
Imports webfuction.Torneo.CompilaData

Namespace WebData
    Partial Class ProbableFormations

        Public Function GetPianetaFantacalcio(ReturnData As Boolean) As String
            Return GetPianetaFantacalcio(ReturnData, False)
        End Function

        Public Function GetPianetaFantacalcio(ReturnData As Boolean, FromBackup As Boolean, Optional Giornata As Integer = -1) As String

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

                    sr.WriteLine("Reading html page")

                    Dim start As Boolean = False
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""

                    Dim lines() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.Default)
                    Dim wpd As New Torneo.ProbablePlayers.Probable
                    Dim wpl As New Dictionary(Of String, Players.PlayerMatch)

                    sr.WriteLine("lines => " & lines.Length)

                    For i As Integer = 0 To lines.Length - 1

                        lines(i) = lines(i).Replace(vbTab, "")

                        If lines(i) <> "" Then

                            If lines(i).Contains("Giornata <!-- -->") Then
                                currgg = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=Giornata \<!-- --\>)\d+").Value)
                            ElseIf lines(i).Contains("top-squadre-selezionate") Then
                                sq.Clear()
                            ElseIf lines(i).Contains("<h2 class=""TeamNome"">") Then
                                sq.Add(Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=\>)\w+(?=\<\/h)").Value.ToUpper()))
                            ElseIf lines(i).Contains("<!-- --> TITOLARI (<!-- -->11<!-- -->") Then
                                start = True
                                pstate = "Titolare"
                            ElseIf lines(i).Contains("th colspan=""2"">PANCHINA</th>") Then
                                start = True
                                pstate = "Panchina"
                            ElseIf lines(i).Contains("<td style=""text-align:left""") OrElse lines(i).Contains("<td class=""left"">") Then
                                team = sq(1)
                            ElseIf lines(i).Contains("<td style=""text-align:right""") Then
                                team = sq(0)
                            ElseIf lines(i).Contains("<span class=""team-probabili"">") Then
                                team = System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=team-probabili"">).*(?=\<\/span)").Value.Replace("'", "’").Trim()
                            ElseIf System.Text.RegularExpressions.Regex.Match(lines(i), "href=""giocatori-statistiche-personali.asp?").Success Then

                                lines(i) = lines(i).Replace(vbTab, "").Trim()

                                Dim name As String = System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=nomegio=)(.*?)(?="")").Value.Replace("'", "’")
                                Dim Ruolo As String = System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=Ruolo=)\w{1}").Value
                                Dim info As String = ""

                                If lines(i).Contains("BAKKER") Then
                                    lines(i) = lines(i)
                                End If

                                If lines(i).Contains("title=""Ballottaggio") Then

                                    Dim s() As String = System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=Ballottaggio\s+)(.*?)(?=\"")").Value.Replace("(", "|").Replace(")", "").Replace("/", "|").Split(CChar("|"))
                                    If s.Length = 4 Then
                                        name = Players.Data.ResolveName(Ruolo, s(0), team, wpl, False).GetName()
                                        info = "In ballottagio con " & s(2).Trim() & " [" & s(1).Trim() & "]"
                                        Call AddInfo(name, team, site, pstate, info, -1, wpd.Players)
                                        name = Players.Data.ResolveName(Ruolo, s(2), team, wpl, False).GetName()
                                        info = "In ballottagio con " & s(0).Trim() & " [" & s(3).Trim() & "]"
                                        Call AddInfo(name, team, site, "Panchina", info, -1, wpd.Players)
                                    End If
                                Else
                                    name = Players.Data.ResolveName(Ruolo, name, team, wpl, False).GetName()
                                    Call AddInfo(name, team, site, pstate, info, -1, wpd.Players)
                                End If

                            ElseIf lines(i).Contains("<div class=""giocatori-indisponibili"">") Then

                                Dim name As String = System.Text.RegularExpressions.Regex.Match(lines(i + 1), "(?<=\<a.*\>).*(?=\<\/a\>)").Value.Trim
                                Dim info As String = Functions.NormalizeText(System.Text.RegularExpressions.Regex.Match(lines(i + 2), "(?<=strong\>).*(?=\<\/div\>)").Value.Trim)

                                If name.Contains("RAMON") Then
                                    name = name
                                End If

                                If name <> "" Then
                                    pstate = "Infortunato"
                                    name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                    Call AddInfo(name, team, site, pstate, info, -1, wpd.Players)
                                End If

                            ElseIf lines(i).Contains("<div class=""giocatori-squalificati"">") Then

                                Dim name As String = System.Text.RegularExpressions.Regex.Match(lines(i + 2), ".*(?=\<\/a\>)").Value.Trim
                                Dim info As String = Functions.NormalizeText(System.Text.RegularExpressions.Regex.Match(lines(i + 3), ".*(?=\<\/div\>)").Value.Trim.Replace("(", "").Replace(")", ""))

                                If name <> "" Then
                                    pstate = "Squalificato"
                                    name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                    Call AddInfo(name, team, site, pstate, info, -1, wpd.Players)
                                End If

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
                stringBuilder.AppendLine(Functions.GetPage(appSett, "https://www.pianetafanta.it/probabili-formazioni-fantacalcio", "UTF-8"))
            Next

            html = stringBuilder.ToString()

            IO.File.WriteAllText(fileTemp, html, New System.Text.UTF8Encoding(False))

            Return html

        End Function
    End Class
End Namespace