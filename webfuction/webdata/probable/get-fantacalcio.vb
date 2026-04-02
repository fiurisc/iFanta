Namespace WebData
    Partial Class ProbableFormations

        Public Function GetFantacalcio(ReturnData As Boolean) As String
            Return GetFantacalcio(ReturnData, False)
        End Function

        Public Function GetFantacalcio(ReturnData As Boolean, FromBackup As Boolean, Optional Giornata As Integer = -1) As String

            Dim currgg As Integer = Giornata
            Dim dirt As String = appSett.WebDataPath & "\temp"
            Dim dird As String = appSett.WebDataPath & "\data\pforma"
            Dim site As String = "Fantacalcio"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".json"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"
            Dim fileBakupHtml As String = GetBackupHtmlDataFileName(site.ToLower(), currgg)

            Dim sr As New IO.StreamWriter(fileLog)
            Dim rmsg As String = ""

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
                    html = Functions.GetPage(appSett, "https://www.fantacalcio.it/probabili-formazioni-serie-A", "UTF-8")
                    IO.File.WriteAllText(fileTemp, html, New System.Text.UTF8Encoding(False))
                End If

                If html <> "" Then

                    sr.WriteLine("Reading html page")

                    Dim lines() As String = IO.File.ReadAllLines(fileTemp, New System.Text.UTF8Encoding(False))
                    Dim wpd As New Torneo.ProbablePlayers.Probable
                    Dim wpl As New Dictionary(Of String, Players.PlayerMatch)
                    Dim pstate As String = "Titolare"
                    Dim sq As New List(Of String)
                    Dim team As String = ""
                    Dim name As String = ""
                    Dim Ruolo As String = ""
                    Dim perc As Integer = 0
                    Dim info As String = ""
                    Dim modTeam As String = ""
                    Dim numLine As Integer = 1
                    Dim indsq As Integer = 0

                    sr.WriteLine("lines => " & lines.Length)

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            If line.Contains("class=""ml-auto"">Giornata") Then
                                currgg = CInt(System.Text.RegularExpressions.Regex.Match(lines(i), "\d+").Value)
                            End If

                            If line.Contains("match-info") OrElse line.Contains("player-list starters") Then
                                pstate = "Titolare"
                            ElseIf line.Contains(">Panchina") Then
                                pstate = "Panchina"
                            ElseIf line.Contains(">Infortunati") Then
                                pstate = "Infortunato"
                            ElseIf line.Contains(">Squalificati") Then
                                pstate = "Squalificato"
                            ElseIf line.Contains("Dettaglio calciatori") Then
                                pstate = ""
                            ElseIf line.Contains("data-team-formation") Then
                                modTeam = System.Text.RegularExpressions.Regex.Match(lines(i), "[\d\-]{2,}").Value.ToUpper()
                                numLine = 1
                                team = sq(indsq)
                                indsq += 1
                            ElseIf line.Contains("class=""team-away """) OrElse line.Contains("class=""team-home """) Then
                                If line.Contains("class=""team-home """) Then
                                    sq.Clear()
                                    indsq = 0
                                End If
                                sq.Add(System.Text.RegularExpressions.Regex.Match(lines(i + 2), "(?<=content\=\"")(.*?)(?=\"")").Value.ToUpper().Trim())
                            ElseIf line.Contains("Campioncino ") Then
                                name = System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=\""Campioncino\s+)(.*?)(?=\"")").Value.ToUpper().Trim()
                                name = Functions.NormalizeText(name)
                                name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                If wpd.ModuleTeam.ContainsKey(team) = False Then wpd.ModuleTeam.Add(team, New Torneo.ProbablePlayers.Probable.ProbableModule())
                                wpd.ModuleTeam(team).ModuleName = modTeam
                                If wpd.ModuleTeam(team).Lines.ContainsKey(numLine.ToString()) = False Then wpd.ModuleTeam(team).Lines.Add(numLine.ToString(), New List(Of String)())
                                wpd.ModuleTeam(team).Lines(numLine.ToString()).Add(name)
                            ElseIf line.Contains("<li class=""separator""></li>") Then
                                numLine += 1
                            End If

                            If pstate <> "" AndAlso line.Contains("href=""https://www.fantacalcio.it/serie-a/squadre/") AndAlso lines(i + 2).Contains("</span>") Then
                                team = Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=www\.fantacalcio\.it\/serie\-a\/squadre\/)\w+(?=\/)").Value.ToUpper())
                                name = lines(i + 2).Replace("<span>", "").Replace("</span>", "").Trim().ToUpper().Replace("'", "’").Replace("&#X27;", "’")
                                name = Functions.NormalizeText(name)
                                If name.Contains("&#XE8;") Then
                                    name = name
                                End If
                                If pstate = "Titolare" OrElse pstate = "Panchina" Then
                                    info = ""
                                    Try
                                        perc = CInt(System.Text.RegularExpressions.Regex.Match(lines(i + 6), "\d+").Value)
                                    Catch ex As Exception
                                        perc = 0
                                    End Try
                                ElseIf pstate = "Infortunato" Then
                                    info = Functions.NormalizeText(lines(i + 5).Trim())
                                End If
                                name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                If name.Contains("CANDE") Then
                                    name = name
                                End If
                                Call AddInfo(name, team, site, pstate, info, perc, wpd.Players)
                                If sq.Contains(team) = False Then sq.Add(team)
                                perc = 0
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

    End Class
End Namespace