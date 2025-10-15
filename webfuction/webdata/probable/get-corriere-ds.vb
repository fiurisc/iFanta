Namespace WebData
    Partial Class ProbableFormations

        Shared Function GetCds(ReturnData As Boolean) As String

            Dim dirt As String = Functions.DataPath & "\temp"
            Dim dird As String = Functions.DataPath & "\data\pforma"
            Dim filep As String = dird & "\player.json"
            Dim filet As String = dirt & "\pform-cds.html"
            Dim filed As String = dird & "\cds.json"
            Dim filel As String = dird & "\pform-cds.log"

            Dim site As String = "Corriere"
            Dim enc As String = "utf-8"
            Dim currgg As Integer = -1
            Dim sr As New IO.StreamWriter(filel)
            Dim rmsg As String = ""
            Try

                Players.Data.LoadPlayers(False)
                MatchsData.LoadWebMatchs()

                sr.WriteLine("Year -> " & Functions.Year)
                sr.WriteLine("Calendario match:")
                sr.WriteLine("---------------------------")
                For Each t As String In MatchsData.KeyMatchs.Keys
                    sr.WriteLine(MatchsData.KeyMatchs(t) & " -> " & t)
                Next
                sr.WriteLine("")

                Dim wpd As New Torneo.ProbablePlayers.Probable
                Dim wpl As New Dictionary(Of String, Players.PlayerMatch)
                Dim linkp As New List(Of String)

                'Determino i link delle varie partite'
                Dim html As String = Functions.GetPage("https://www.corrieredellosport.it/calcio/serie-a/probabili-formazioni")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding(enc))
                    Dim start As Boolean = False

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then
                            If line.Contains("<ul class=""probabili-formazioni-list"">") Then
                                start = True
                            Else
                                If start AndAlso line.Contains("probabili-formazioni") Then

                                    linkp.Add(System.Text.RegularExpressions.Regex.Match(line, "(?<="").*(?=\"")").Value)

                                    If currgg = -1 Then

                                        Dim match As String = System.Text.RegularExpressions.Regex.Match(line, "[\w\s\-]{1,}_-_[\w\s\-]{1,}").Value.ToUpper.Replace("_-_", "-")

                                        sr.WriteLine("match trovato -> " & match)

                                        For Each key As String In MatchsData.KeyMatchs.Keys
                                            If key = match Then
                                                currgg = MatchsData.KeyMatchs(key)
                                                sr.WriteLine("giornata associata -> " & CStr(currgg))
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If

                If linkp.Count > 0 Then

                    'Analizzo le proprobili formazioni dei singoli match'
                    For i As Integer = 0 To linkp.Count - 1
                        If linkp(i) <> "" Then
                            Call GetCdsSingleMatch(linkp(i), i, site, wpd, wpl)
                        End If
                    Next

                    If currgg <> -1 Then
                        wpd.Day = currgg
                        Dim out As String = WriteData(wpd, filed)
                        If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(wpl, filep)
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

        Private Shared Sub GetCdsSingleMatch(link As String, indmatch As Integer, site As String, wpd As Torneo.ProbablePlayers.Probable, wpl As Dictionary(Of String, Players.PlayerMatch))

            Try

                Dim dirt As String = Functions.DataPath & "\temp"
                Dim filet As String = dirt & "\pform-cds-ind-" & indmatch & ".txt"
                Dim enc As String = "utf-8"

                'Determino i link delle varie partite'
                Dim html As String = Functions.GetPage(link, enc)

                If html <> "" Then

                    IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding(enc))
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"

                    sq.Add("")
                    sq.Add("")

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            Dim name As String = ""
                            Dim perc As Integer = -1
                            Dim info As String = ""

                            If line.Contains("<div class=""team home"">") Then
                                sq(0) = (Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i + 1), "(?<=\>).*(?=\<\/)").Value.ToUpper))
                            End If

                            If line.Contains("<div class=""team away"">") Then
                                sq(1) = (Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i + 1), "(?<=\>).*(?=\<\/)").Value.ToUpper))
                            End If

                            If line.Contains(""">Titolari</th>") Then
                                pstate = "Titolare"
                            ElseIf line.Contains(""">Riserve</th>") Then
                                pstate = "Panchina"
                            ElseIf line.Contains(""">Indisponibili</th>") Then
                                pstate = "Infortunato"
                            ElseIf line.Contains(""">Squalificati</th>") Then
                                pstate = "Squalificato"
                            End If

                            If line.Contains("<td class=""a-right"">") Then

                                'Giorcatore Squadra in casa'
                                name = System.Text.RegularExpressions.Regex.Match(line, "(?<=\>).*(?=\<\/)").Value.ToUpper.Replace("'", "’")
                                If name <> "" Then
                                    name = Players.Data.ResolveName("", name, sq(0), wpl, False).GetName()
                                    Call AddInfo(name, sq(0), site, pstate, info, perc, wpd.Players)
                                End If
                                'Giorcatore Squadra fuori casa'
                                name = System.Text.RegularExpressions.Regex.Match(lines(i + 3), "(?<=\>).*(?=\<\/)").Value.ToUpper.Replace("'", "’")
                                If name <> "" Then
                                    name = Players.Data.ResolveName("", name, sq(1), wpl, False).GetName()
                                    Call AddInfo(name, sq(1), site, pstate, info, perc, wpd.Players)
                                End If

                            ElseIf line.Contains("<th colspan=""2"">") Then

                                'Giorcatore Squadra in casa'
                                Dim s1() As String = System.Text.RegularExpressions.Regex.Match(lines(i - 1), "(?<=\>).*(?=\<\/)").Value.Split(CChar(","))
                                For k As Integer = 0 To s1.Length - 1
                                    name = s1(k).Trim.ToUpper
                                    If name <> "" Then
                                        name = Players.Data.ResolveName("", name, sq(0), wpl, False).GetName()
                                        Call AddInfo(name, sq(0), site, pstate, "", perc, wpd.Players)
                                    End If
                                Next

                                'Giorcatore Squadra fuori casa'
                                Dim s2() As String = System.Text.RegularExpressions.Regex.Match(lines(i + 1), "(?<=\>).*(?=\<\/)").Value.Split(CChar(","))
                                For k As Integer = 0 To s2.Length - 1
                                    name = s2(k).Trim.ToUpper
                                    If name <> "" Then
                                        name = Players.Data.ResolveName("", s2(k).Trim.ToUpper, sq(0), wpl, False).GetName()
                                        Call AddInfo(name, sq(1), site, pstate, "", perc, wpd.Players)
                                    End If
                                Next

                            End If
                        End If
                    Next

                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

        End Sub
    End Class
End Namespace