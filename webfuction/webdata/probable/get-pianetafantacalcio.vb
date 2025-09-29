Imports webfuction.Torneo

Namespace WebData
    Partial Class ProbableFormations

        Shared Function GetPianetaFantacalcio(ReturnData As Boolean) As String

            Dim dirt As String = Functions.DataPath & "\temp"
            Dim dird As String = Functions.DataPath & "\data\pforma"
            Dim filet As String = dirt & "\pform-pianeta-fantacalcio.txt"
            Dim filed As String = dird & "\pform-pianeta-fantacalcio.txt"
            Dim filep As String = dird & "\pform-pianeta-fantacalcio-player.txt"
            Dim site As String = "PianetaFantacalcio"
            Dim currgg As Integer = -1

            Try

                Players.Data.LoadPlayers(False)
                MatchsData.LoadWebMatchs()

                Dim html As String = Functions.GetPage("https://www.pianetafanta.it/probabili-formazioni-complete-serie-a-live.asp")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding("iso-8859-1"))

                    Dim start As Boolean = False
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""

                    Dim line() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding("iso-8859-1"))
                    Dim wpd As New Dictionary(Of String, ProbablePlayer.Player)
                    Dim wpl As New Dictionary(Of String, Players.PlayerMatch)

                    For i As Integer = 0 To line.Length - 1

                        line(i) = line(i).Replace(vbTab, "")

                        If line(i) <> "" Then

                            If line(i).Contains("><h4><strong>") Then
                                currgg = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(line(i), "(?<=\>)\d+(?=\s+Giornata)").Value)
                            ElseIf line(i).Contains("top-squadre-selezionate") Then
                                sq.Clear()
                            ElseIf line(i).Contains("<h2 class=""TeamNome"">") Then
                                sq.Add(Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(line(i), "(?<=\>)\w+(?=\<\/h)").Value.ToUpper()))
                            ElseIf line(i).Contains("style=""text-align:center"">TITOLARI</th>") Then
                                start = True
                                pstate = "Titolare"
                            ElseIf line(i).Contains("th colspan=""2"">PANCHINA</th>") Then
                                start = True
                                pstate = "Panchina"
                            ElseIf line(i).Contains("<td style=""text-align:left""") OrElse line(i).Contains("<td class=""left"">") Then
                                team = sq(1)
                            ElseIf line(i).Contains("<td style=""text-align:right""") Then
                                team = sq(0)
                            ElseIf line(i).Contains("<span class=""team-probabili"">") Then
                                team = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=team-probabili"">).*(?=\<\/span)").Value.Replace("'", "’").Trim()
                            ElseIf System.Text.RegularExpressions.Regex.Match(line(i), "href=""giocatori-statistiche-personali.asp?").Success Then

                                line(i) = line(i).Replace(vbTab, "").Trim()

                                Dim name As String = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=nomegio=)(.*?)(?="")").Value.Replace("'", "’")
                                Dim ruolo As String = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=ruolo=)\w{1}").Value
                                Dim info As String = ""

                                If line(i).Contains("FERGUSON") Then
                                    line(i) = line(i)
                                End If

                                If line(i).Contains("title=""Ballottaggio") Then

                                    Dim s() As String = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=Ballottaggio\s+)(.*?)(?=\"")").Value.Replace("(", "|").Replace(")", "").Replace("/", "|").Split(CChar("|"))
                                    If s.Length = 4 Then
                                        name = Players.Data.ResolveName(ruolo, s(0), team, wpl, False).GetName()
                                        info = "In ballottagio con " & s(2).Trim() & " [" & s(1).Trim() & "]"
                                        Call AddInfo(name, team, site, pstate, info, -1, wpd)
                                        name = Players.Data.ResolveName(ruolo, s(2), team, wpl, False).GetName()
                                        info = "In ballottagio con " & s(0).Trim() & " [" & s(3).Trim() & "]"
                                        Call AddInfo(name, team, site, "Panchina", info, -1, wpd)
                                    End If
                                Else
                                    name = Players.Data.ResolveName(ruolo, name, team, wpl, False).GetName()
                                    Call AddInfo(name, team, site, pstate, info, -1, wpd)
                                End If

                            ElseIf line(i).Contains("<div class=""giocatori-indisponibili"">") Then

                                Dim name As String = System.Text.RegularExpressions.Regex.Match(line(i + 1), "(?<=\<a.*\>).*(?=\<\/a\>)").Value.Trim
                                Dim info As String = Functions.NormalizeText(System.Text.RegularExpressions.Regex.Match(line(i + 2), "(?<=strong\>).*(?=\<\/div\>)").Value.Trim)

                                If name.Contains("LUVU") Then
                                    name = name
                                End If

                                If name <> "" Then
                                    pstate = "Infortunato"
                                    name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                    Call AddInfo(name, team, site, pstate, info, -1, wpd)
                                End If

                            ElseIf line(i).Contains("<div class=""giocatori-squalificati"">") Then

                                Dim name As String = System.Text.RegularExpressions.Regex.Match(line(i + 2), ".*(?=\<\/a\>)").Value.Trim
                                Dim info As String = Functions.NormalizeText(System.Text.RegularExpressions.Regex.Match(line(i + 3), ".*(?=\<\/div\>)").Value.Trim.Replace("(", "").Replace(")", ""))

                                If name <> "" Then
                                    pstate = "Squalificato"
                                    name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                    Call AddInfo(name, team, site, pstate, info, -1, wpd)
                                End If

                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        Dim out As String = WriteData(currgg, wpd, filed)
                        If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(wpl, filep)
                        Return out.Replace(System.Environment.NewLine, "</br>")
                    Else
                        Return ""
                    End If
                Else
                    Return ""
                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

        End Function
    End Class
End Namespace