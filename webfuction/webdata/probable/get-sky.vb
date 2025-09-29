Namespace WebData
    Partial Class ProbableFormations

        Shared Function GetSky(ReturnData As Boolean) As String

            Dim dirt As String = Functions.DataPath & "\temp"
            Dim dird As String = Functions.DataPath & "\data\pforma"
            Dim filet As String = dirt & "\pform-sky.txt"
            Dim filed As String = dird & "\pform-sky.txt"
            Dim filep As String = dird & "\pform-sky-player.txt"
            Dim site As String = "Sky"
            Dim enc As String = "utf-8"
            Dim currgg As Integer = -1

            Try

                Players.Data.LoadPlayers(False)
                MatchsData.LoadWebMatchs()

                Dim html As String = Functions.GetPage("https://sport.sky.it/calcio/serie-a/probabili-formazioni")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding(enc))
                    Dim wpd As New Dictionary(Of String, Torneo.ProbablePlayer.Player)
                    Dim wpl As New Dictionary(Of String, Players.PlayerMatch)
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""
                    Dim name As String = ""
                    Dim sez As String = "header"

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            If line.Contains("Titolari</td>") Then
                                sez = "header"
                                sq.Clear()
                                Dim s() As String = line.Split(New String() {"</div>"}, StringSplitOptions.None)
                                s = s
                            ElseIf line.Contains("<div class=""content"">") Then
                                sez = "player"
                            ElseIf line.Contains("<div class=""team-1 left"">") Then
                                sqid = 0
                            ElseIf line.Contains("<div class=""team-2 right"">") Then
                                sqid = 1
                            ElseIf line.Contains("<ul class=""playerslist"">") Then
                                pstate = "Titolare"
                            ElseIf line.Contains("<dt>Panchina:</dt>") Then
                                pstate = "Panchina"
                            ElseIf line.Contains("<dt>Squalificati:</dt>") Then
                                pstate = "Squalificati"
                            ElseIf line.Contains("<dt>Indisponibili:</dt>") Then
                                pstate = "Indisponibile"
                            ElseIf line.Contains("<dt>Allenatore:</dt>") Then
                                pstate = ""
                            ElseIf line.Contains("<div class=""lineup"">") Then

                            End If

                            If line.Contains("<span class=""name"">") Then

                                Dim val As String = System.Text.RegularExpressions.Regex.Match(line, "(?<=>).*(?=\<)").Value.ToUpper

                                Select Case sez
                                    Case "header"
                                        'Aggiungo la squadra alla lista di quelle che disputano il match'
                                        sq.Add(Functions.CheckTeamName(val.ToUpper))
                                        'Cerco di determinare la giornata di riferiemnto'
                                        If sq.Count = 2 AndAlso currgg = -1 Then

                                            Dim match As String = sq(0) & "-" & sq(1)

                                            For Each key As String In MatchsData.KeyMatchs.Keys
                                                If key = match Then
                                                    currgg = MatchsData.KeyMatchs(key)
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    Case "player"
                                        team = sq(sqid)
                                        name = val.Replace("-", " ").Trim
                                        If name <> "" Then
                                            name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                            Call AddInfo(name, team, site, pstate, "", -1, wpd)
                                        End If
                                End Select

                            ElseIf line.Contains("</dt><dd>") AndAlso pstate <> "" Then

                                Dim s() As String = System.Text.RegularExpressions.Regex.Match(line, "(?<=\<dd\>).*(?=\<\/dd\>)").Value.Split(CChar(","))

                                For k As Integer = 0 To s.Length - 1
                                    name = s(k).Replace("-", " ").Trim
                                    If name <> "" Then
                                        name = Players.Data.ResolveName("", name, team, wpl, False).GetName()
                                        Call AddInfo(name, team, site, pstate, "", -1, wpd)
                                    End If
                                Next
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