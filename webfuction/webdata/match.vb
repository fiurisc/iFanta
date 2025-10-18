Imports System.Text.RegularExpressions

Namespace WebData
    Public Class MatchsData

        Private Shared thrmatch As New List(Of Threading.Thread)
        Private Shared diclinkdaymatch As New SortedDictionary(Of String, Dictionary(Of String, String))
        Private Shared dirt As String = ""
        Private Shared dird As String = ""

        Private Shared matchs As New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.Match))
        Private Shared matchsplayers As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)))
        Private Shared matchsevent As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchEvent))))

        Public Shared Property KeyMatchs As New Dictionary(Of String, Integer)

        Public Shared Sub ResetCacheData()
            KeyMatchs.Clear()
        End Sub

        Private Shared Sub SetFolder()
            dirt = Functions.DataPath & "temp\"
            dird = Functions.DataPath & "data\matchs\"
        End Sub

        Shared Function GetMatchFileName() As String
            SetFolder()
            Return dird & "matchs-data.json"
        End Function

        Shared Function GetMatchPlayersFileName() As String
            SetFolder()
            Return dird & "matchs-players-data.json"
        End Function

        Shared Function GetMatchPlayersDayFileName(day As String) As String
            Return dird & "matchs-players-data-" & day & ".json"
        End Function

        Shared Function GetMatchEventFileName() As String
            SetFolder()
            Return dird & "matchs-events-data.json"
        End Function

        Public Shared Sub LoadWebMatchs()

            If KeyMatchs.Count = 0 Then

                Dim fname As String = GetMatchFileName()

                If IO.File.Exists(fname) Then

                    Dim j As String = IO.File.ReadAllText(fname)
                    Dim dicdata As SortedDictionary(Of String, SortedDictionary(Of String, Torneo.MatchsData.Match)) = Functions.DeserializeJson(Of SortedDictionary(Of String, SortedDictionary(Of String, Torneo.MatchsData.Match)))(j)

                    For Each d As String In dicdata.Keys
                        For Each mid As String In dicdata(d).Keys
                            Dim key As String = dicdata(d)(mid).TeamA & "-" & dicdata(d)(mid).TeamB
                            If KeyMatchs.ContainsKey(key) = False Then KeyMatchs.Add(key, CInt(d))
                        Next
                    Next
                End If

            End If

        End Sub

        Shared Function GetDataMatchs(ReturnData As Boolean) As String

            Dim strresp As New System.Text.StringBuilder
            Dim year As String = Functions.Year

            Functions.MakeDirectory()
            Players.Data.LoadPlayers(False)

            matchs.Clear()
            matchsplayers.Clear()
            matchsevent.Clear()

            'Leggo il calendario delle partite con i risultati'
            strresp.AppendLine(GetCalendarMatchs(ReturnData))

            'Leggo i tabelli delle partite'
            Call GetMatchsPlayersData()

            Return strresp.ToString()

        End Function

        Private Shared Function GetCalendarMatchs(ReturnData As Boolean) As String

            Try

                Dim filed As String = GetMatchFileName()

                'Creo la lista di thread da eseguire'
                thrmatch.Clear()
                For i As Integer = 1 To 38
                    Dim t As New Threading.Thread(AddressOf GetMatchsDay)
                    t.Name = CStr(i)
                    thrmatch.Add(t)
#If DEBUG Then
                    'If i > 2 Then Exit For
#End If
                Next

                'Lancio i vari Thread'
                For i As Integer = 0 To thrmatch.Count - 1
                    thrmatch(i).Priority = Threading.ThreadPriority.Normal
                    thrmatch(i).Start()
                    thrmatch(i).Join()
                    System.Threading.Thread.Sleep(100)
                Next

                If Torneo.PublicVariables.dataFromDatabase Then
                    Torneo.MatchsData.UpdateMatchData(matchs)
                End If

                IO.File.WriteAllText(filed, Functions.SerializzaOggetto(matchs, False))

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
                Return ""
            End Try

            If ReturnData Then
                Return "</br><span style=color:red;font-size:bold;'>Match data (" & Functions.Year & "):</span></br>" & WebData.Functions.SerializzaOggetto(matchs, False).Replace(System.Environment.NewLine, "</br>") & "</br>"
            Else
                Return ("</br><span style=color:red;font-size:bold;'>Match data (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br><span style=color:red;font-size:bold;'>Detail match data:</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
            End If

        End Function

        Private Shared Sub GetMatchsPlayersData()

            Try

                Dim filedetd As String = GetMatchPlayersFileName()

                'Carico i dati dell'ultima lettura'
                Dim lastday As Integer = GetLastMatchsDayLoaded()

                If diclinkdaymatch.Count > 0 Then

                    'Creo la lista di thread da eseguire'
                    thrmatch.Clear()
                    For Each d As String In diclinkdaymatch.Keys
                        'Ricarico i dati se nella precedente acquisizione non e' sono stati prelevati i tabellini della giornata
                        'oppure se la giornata non e' piu' vecchia di di due giornate oppure se non tutti i match della giornata
                        'stati disputati'
                        If CInt(d) > lastday - 2 OrElse diclinkdaymatch(d).Count < 10 Then
                            Dim t As New Threading.Thread(AddressOf GetMatchsPlayersDataByDay)
                            t.Name = CStr(d)
                            thrmatch.Add(t)
                        End If
                    Next

                    'Lancio i vari Thread'
                    For i As Integer = 0 To thrmatch.Count - 1
                        thrmatch(i).Priority = Threading.ThreadPriority.Normal
                        thrmatch(i).Start()
                        thrmatch(i).Join()
                        System.Threading.Thread.Sleep(100)
                    Next

                    Dim dicalldata As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)))

                    For i As Integer = 1 To 38
                        Dim fday As String = GetMatchPlayersDayFileName(i.ToString())
                        If IO.File.Exists(fday) Then
                            Dim dicdata As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)) = Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)))(IO.File.ReadAllText(fday))
                            dicalldata.Add(CStr(i), dicdata)
                        End If
                    Next

                    If Torneo.PublicVariables.DataFromDatabase AndAlso matchsplayers.Count > 0 Then
                        Torneo.MatchsData.UpdateMatchDataPlayers(matchsplayers)
                    End If

                    IO.File.WriteAllText(filedetd, Functions.SerializzaOggetto(dicalldata, False))

                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Shared Sub GetMatchsDay()
            Try
                'Determino la giornata di riferimento'
                Dim d As String = Threading.Thread.CurrentThread.Name
                Dim datamatch As New Dictionary(Of String, String)
                Dim filet As String = dirt & "match-day-" & d & ".txt"
                Dim filed As String = dirt & "match-day-" & d & ".txt"
                Dim html As String = Functions.GetPage("https://www.fantacalcio.it/serie-a/calendario/" & d)

                If html <> "" Then

                    Dim day As String = ""
                    Dim dt As New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day, 0, 0, 0)
                    Dim Squadraa As String = ""
                    Dim Squadrab As String = ""
                    Dim line() As String
                    Dim matchid As Integer = 1
                    Dim matchplayed As Boolean = False
                    Dim goal1 As String = ""
                    Dim goal2 As String = ""

                    IO.File.WriteAllText(filet, html.Replace(vbCr, ""))
                    line = IO.File.ReadAllLines(filet)

                    For i As Integer = 0 To line.Length - 1
                        If line(i) <> "" Then
                            If line(i).Contains("<meta itemprop=""startDate""") Then
                                day = Regex.Match(line(i), "\d+-\d+-\d+").Value
                                Dim days() As String = day.Split(CChar("-"))
                                If days.Length = 3 Then
                                    dt = New Date(CInt(days(0)), CInt(days(1)), CInt(days(2)), 0, 0, 0)
                                End If
                            ElseIf line(i).Contains("<span class=""hours"">") Then
                                day = Regex.Match(line(i), "\d+:\d+").Value
                                Dim days() As String = day.Split(CChar(":"))
                                If days.Length = 2 Then
                                    dt = New Date(dt.Year, dt.Month, dt.Day, CInt(days(0)), CInt(days(1)), 0)
                                End If
                            ElseIf line(i).Contains("score-home") Then
                                goal1 = Regex.Match(line(i), "\d+(?=\</span)").Value.ToUpper()
                            ElseIf line(i).Contains("score-away") Then
                                goal2 = Regex.Match(line(i), "\d+(?=\</span)").Value.ToUpper()
                            ElseIf line(i).Contains("<meta itemprop=""url"" content=") AndAlso line(i).Contains("https://www.fantacalcio.it/serie-a/calendario") Then

                                Dim mtach As String = Regex.Match(line(i), "(?<=\d+-\d+\/)\w+-\w+(?=\/\d+)").Value.ToUpper()

                                If dt < Date.Now Then
                                    matchplayed = True
                                Else
                                    matchplayed = False
                                    goal1 = ""
                                    goal2 = ""
                                End If

                                If matchs.ContainsKey(d) = False Then matchs.Add(d, New Dictionary(Of String, Torneo.MatchsData.Match))
                                If matchs(d).ContainsKey(matchid.ToString()) = False Then
                                    Dim teams() As String = mtach.Split(CChar("-"))
                                    Dim m As New Torneo.MatchsData.Match
                                    m.Giornata = CInt(d)
                                    m.MatchId = matchid
                                    m.TeamA = teams(0)
                                    m.TeamB = teams(1)
                                    m.Time = dt.ToString("yyyy/MM/dd HH:mm:ss")
                                    m.GoalA = goal1
                                    m.GoalB = goal2
                                    matchs(d).Add(matchid.ToString(), m)
                                End If

                                If matchplayed Then
                                    Dim linktab As String = Regex.Match(line(i), "(?<=content="").*(?="" />)").Value
                                    If diclinkdaymatch.ContainsKey(d) = False Then diclinkdaymatch.Add(d, New Dictionary(Of String, String))
                                    If diclinkdaymatch(d).ContainsKey(matchid.ToString()) = False Then diclinkdaymatch(d).Add(matchid.ToString(), linktab)
                                End If
                                matchid += 1
                                goal1 = ""
                                goal2 = ""
                            End If

                        End If
                    Next
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Shared Function GetLastMatchsDayLoaded() As Integer
            For i As Integer = 1 To 38
                If IO.File.Exists(GetMatchPlayersDayFileName(i.ToString())) = False Then
                    Return i - 1
                End If
            Next
            Return -1
        End Function

        Private Shared Sub GetMatchsPlayersDataByDay()
            Try
                'Determino la giornata di riferimento'
                Dim d As String = Threading.Thread.CurrentThread.Name

                If matchsplayers.ContainsKey(d) = False Then matchsplayers.Add(d, New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)))
                If matchsevent.ContainsKey(d) = False Then matchsevent.Add(d, New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchEvent))))

                For Each m As String In diclinkdaymatch(d).Keys
                    Call GetMatchsPlayersDataByDayMatchId(d, m, diclinkdaymatch(d)(m))
                Next

                IO.File.WriteAllText(GetMatchPlayersDayFileName(d), WebData.Functions.SerializzaOggetto(matchsplayers(d), False))

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Shared Sub GetMatchsPlayersDataByDayMatchId(day As String, MatchId As String, Link As String)

            Dim filet As String = dirt & "match-day-" & day & "-matchid-" & MatchId & ".txt"
            Dim str As New System.Text.StringBuilder

            Try

                If matchsevent(day).ContainsKey(MatchId) = False Then matchsevent(day).Add(MatchId, New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchEvent)))

                Dim matchp As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)) = matchsplayers(day)
                Dim matche As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchEvent)) = matchsevent(day)(MatchId)

                Dim html As String = Functions.GetPage(Link & "/riepilogo")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html)

                    Dim line() As String = IO.File.ReadAllLines(filet)

                    Dim team As String = ""
                    Dim p As New List(Of Players.PlayerMatch)
                    Dim startCronaca As Boolean = False
                    Dim min As Integer = 0
                    Dim events As New Dictionary(Of String, Integer)

                    System.Threading.Thread.Sleep(1000)

                    For z As Integer = 0 To line.Count - 1

                        If line(z) <> "" Then

                            If line(z).Contains("<div class=""player-role role"" data-value=") AndAlso line(z).Contains("data-value=""{{role}}""") = False Then
                                Dim name As String = ""
                                Dim Ruolo As String = Regex.Match(line(z).Trim, "(?<=data-value="")\w+").Value.ToUpper()
                                team = Regex.Match(line(z + 2).Trim, "(?<=squadre\/)\w+(?=\/)").Value.ToUpper()
                                name = Regex.Match(line(z + 2).Trim.Replace("-", " "), "(?<=\/)[\w\s\&\#\;]{1,}(?=\/\d+)").Value.ToUpper()
                                name = Functions.NormalizeText(name)
                                If name.Contains("STREFEZZA") Then
                                    name = name
                                End If
                                name = Players.Data.ResolveName(Ruolo, name, team, False).GetName()
                                AddPlayer(matchp, CInt(day), team, Ruolo, name)
                                matchp(team)(name).Minuti = 90
                                matchp(team)(name).Titolare = 1
                            End If

                            'Determino il minuto dell'evento'
                            If line(z).Contains("minute") AndAlso Regex.Match(line(z).Trim, "<div class=""minute"">\d+").Success Then
                                min = Convert.ToInt32(Regex.Match(line(z).Trim, "\d+").Value)
                                p.Clear()
                            End If

                            If line(z).Contains("<b>Calcio d&#x27;inizio</b>") Then
                                startCronaca = True
                            End If

                            If startCronaca AndAlso line(z).Contains("https://www.fantacalcio.it/serie-a/squadre") AndAlso line(z).Contains("target=""_self"">") Then
                                team = Regex.Match(line(z).Trim, "(?<=squadre\/)\w+(?=\/)").Value.ToUpper
                                Dim n As String = Regex.Match(line(z).Trim.Replace("-", " "), "(?<=\/)[\w\s\&\#\;]{1,}(?=\/\d+)").Value.ToUpper()
                                n = Functions.NormalizeText(n)
                                If n.Contains("STREFEZZA") Then
                                    n = n
                                End If
                                p.Add(Players.Data.ResolveName("", n, team, False))
                            End If

                            If line(z).Contains("title=""") AndAlso Regex.Match(line(z).Trim, "title=""(Subentrato|Ammonizione|Gol segnato|Gol subito|Autorete|Espulsione|Rigore sbagliato)""></figure>").Success Then

                                Dim n1 As String = p(0).GetName()
                                Dim r1 As String = p(0).GetRole()
                                Dim n2 As String = If(p.Count > 1, p(1).GetName(), "")
                                Dim r2 As String = If(p.Count > 1, p(1).GetRole(), "")

                                If line(z).Trim.Contains("Ammonizione") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), team, r1, n1)
                                    matchp(team)(n1).Ammonizione += 1
                                End If
                                If line(z).Trim.Contains("Espulsione") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), team, r1, n1)
                                    matchp(team)(n1).Espulsione += 1
                                End If
                                If line(z).Trim.Contains("Gol subito") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), team, r1, n1)
                                    matchp(team)(n1).GoalSubiti += 1
                                End If
                                If line(z).Trim.Contains("Rigore sbagliato") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), team, r1, n1)
                                    matchp(team)(n1).RigoriSbagliati += 1
                                End If
                                If line(z).Trim.Contains("Gol segnato") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), team, r1, n1)
                                    matchp(team)(n1).GoalFatti += 1
                                    If p.Count > 1 Then
                                        AddPlayer(matchp, CInt(day), team, r2, n2)
                                        matchp(team)(n2).Assists += 1
                                    End If
                                End If
                                If line(z).Trim.Contains("Subentrato") AndAlso p.Count > 1 Then
                                    AddPlayer(matchp, CInt(day), team, r1, n1)
                                    matchp(team)(n1).Subentrato = 1
                                    matchp(team)(n1).Minuti = min
                                    AddPlayer(matchp, CInt(day), team, r2, n2)
                                    matchp(team)(n2).Sostituito = 1
                                    matchp(team)(n2).Minuti = min
                                End If

                            End If

                            If line(z).Contains("Rigore") Then
                                line(z) = line(z)
                            End If

                            If line(z).Contains("title=""") AndAlso Regex.Match(line(z).Trim, "title="".*""></figure>").Success Then
                                p.Clear()
                            End If

                        End If
                    Next

                    For Each t As String In matchp.Keys
                        For Each n As String In matchp(t).Keys
                            If matchp(t)(n).Subentrato = 1 Then
                                If min > 90 Then
                                    matchp(t)(n).Minuti = min - matchp(t)(n).Minuti
                                Else
                                    matchp(t)(n).Minuti = 90 - matchp(t)(n).Minuti
                                End If
                            End If
                            If matchp(t)(n).Minuti > 90 Then matchp(t)(n).Minuti = 90
                        Next
                    Next
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

        End Sub

        Private Shared Sub AddPlayer(match As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)), giornata As Integer, team As String, ruolo As String, name As String)
            If match.ContainsKey(team) = False Then match.Add(team, New Dictionary(Of String, Torneo.MatchsData.MatchPlayer))
            If match(team).ContainsKey(name) = False Then
                match(team).Add(name, New Torneo.MatchsData.MatchPlayer())
                match(team)(name).Giornata = giornata
                match(team)(name).Ruolo = ruolo
                match(team)(name).Nome = name
                match(team)(name).Squadra = team
            Else
                name = name
            End If
        End Sub

    End Class
End Namespace