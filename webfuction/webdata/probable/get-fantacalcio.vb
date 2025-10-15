Namespace WebData
    Partial Class ProbableFormations

        Shared Function GetFantacalcio(ReturnData As Boolean) As String

            Dim dirt As String = Functions.DataPath & "\temp"
            Dim dird As String = Functions.DataPath & "\data\pforma"
            Dim site As String = "Fantacalcio"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".json"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"

            Dim enc As String = "iso-8859-1"
            Dim currgg As Integer = -1
            Dim sr As New IO.StreamWriter(fileLog)
            Dim rmsg As String = ""

            Try

                sr.WriteLine("Loading web player and matchs")
                Players.Data.LoadPlayers(False)
                MatchsData.LoadWebMatchs()

                sr.WriteLine("Year -> " & Functions.Year)
                sr.WriteLine("Calendario match:")
                sr.WriteLine("---------------------------")
                For Each t As String In MatchsData.KeyMatchs.Keys
                    sr.WriteLine(MatchsData.KeyMatchs(t) & " -> " & t)
                Next
                sr.WriteLine("")

                'Determino i link delle varie partite'
                sr.WriteLine("Get Html page")
                Dim html As String = Functions.GetPage("https://www.fantacalcio.it/probabili-formazioni-serie-A")

                If html <> "" Then

                    sr.WriteLine("Reading html page")
                    IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.GetEncoding(enc))
                    Dim wpd As New Torneo.ProbablePlayers.Probable
                    Dim wpl As New Dictionary(Of String, Players.PlayerMatch)
                    Dim pstate As String = "Titolare"
                    Dim sq As New List(Of String)
                    Dim team As String = ""
                    Dim name As String = ""
                    Dim Ruolo As String = ""
                    Dim perc As Integer = 0
                    Dim info As String = ""

                    sr.WriteLine("lines => " & lines.Length)

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            If line.Contains("class=""ml-auto"">Giornata") Then
                                currgg = CInt(System.Text.RegularExpressions.Regex.Match(lines(i), "\d+").Value)
                            End If
                            If line.Contains("match-info") Then
                                pstate = "Titolare"
                                sq.Clear()
                            ElseIf line.Contains(">Panchina") Then
                                pstate = "Panchina"
                            ElseIf line.Contains(">Infortunati") Then
                                pstate = "Infortunato"
                            ElseIf line.Contains(">Squalificati") Then
                                pstate = "Squalificato"
                            ElseIf line.Contains("Dettaglio calciatori") Then
                                pstate = ""
                            End If

                            If pstate <> "" AndAlso line.Contains("href=""https://www.fantacalcio.it/serie-a/squadre/") AndAlso lines(i + 2).Contains("</span>") Then
                                team = Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=www\.fantacalcio\.it\/serie\-a\/squadre\/)\w+(?=\/)").Value.ToUpper())
                                name = lines(i + 2).Replace("<span>", "").Replace("</span>", "").Trim().ToUpper().Replace("'", "’").Replace("&#X27;", "’")
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
                                Call AddInfo(name, team, site, pstate, info, perc, wpd.Players)
                                If sq.Contains(team) = False Then sq.Add(team)
                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        wpd.Day = currgg
                        Dim out As String = WriteData(wpd, fileData)
                        If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(wpl, filePlayers)
                        rmsg = out.Replace(System.Environment.NewLine, "</br>")
                    End If
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
                rmsg = ex.Message
            End Try

            sr.Close()

            Return rmsg

        End Function

    End Class
End Namespace