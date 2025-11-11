Imports System.Text.RegularExpressions

Namespace WebData
    Public Class MatchsData

        Dim appSett As Torneo.PublicVariables
        Private dirt As String = ""
        Private dird As String = ""

        Sub New(appSett As Torneo.PublicVariables)
            Me.appSett = appSett
            dirt = appSett.TorneoWebDataPath & "temp\"
            dird = appSett.TorneoWebDataPath & "data\matchs\"
        End Sub

        Private Shared thrmatch As New List(Of Threading.Thread)
        Private Shared diclinkdaymatch As New SortedDictionary(Of String, Dictionary(Of String, String))

        'giornata/matchid/match'
        Private Shared matchs As New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.Match))
        'giornata/matchid/squadra/nome'
        Private Shared matchsplayers As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer))))
        'giornata/matchid/minute/events'
        Private Shared matchsevent As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, List(Of Torneo.MatchsData.MatchEvent))))

        Public Property KeyMatchs As New Dictionary(Of String, MatchRound)

        Public Sub ResetCacheData()
            KeyMatchs.Clear()
        End Sub

        Shared Function GetMatchFileName(appsett As Torneo.PublicVariables) As String
            Return appsett.TorneoWebDataPath & "data\matchs\matchs-data.json"
        End Function

        Shared Function GetMatchPlayersFileName(appsett As Torneo.PublicVariables) As String
            Return appsett.TorneoWebDataPath & "data\matchs\matchs-players-data.json"
        End Function

        Shared Function GetMatchPlayersDayFileName(appsett As Torneo.PublicVariables, day As String) As String
            Return appsett.TorneoWebDataPath & "data\matchs\matchs-players-data-" & day & ".json"
        End Function

        Shared Function GetMatchEventsDayFileName(appsett As Torneo.PublicVariables, day As String) As String
            Return appsett.TorneoWebDataPath & "data\matchs\matchs-events-data-" & day & ".json"
        End Function

        Shared Function GetMatchEventFileName(appsett As Torneo.PublicVariables) As String
            Return appsett.TorneoWebDataPath & "data\matchs\matchs-events-data.json"
        End Function

        Public Sub LoadWebMatchs()

            If KeyMatchs.Count = 0 Then

                Dim fname As String = GetMatchFileName(appSett)

                If IO.File.Exists(fname) Then

                    Dim j As String = IO.File.ReadAllText(fname)
                    Dim dicdata As SortedDictionary(Of String, SortedDictionary(Of String, Torneo.MatchsData.Match)) = Functions.DeserializeJson(Of SortedDictionary(Of String, SortedDictionary(Of String, Torneo.MatchsData.Match)))(j)

                    For Each d As String In dicdata.Keys
                        For Each mid As String In dicdata(d).Keys
                            Dim key As String = dicdata(d)(mid).TeamA & "-" & dicdata(d)(mid).TeamB
                            If KeyMatchs.ContainsKey(key) = False Then KeyMatchs.Add(key, New MatchRound(CInt(d), CInt(mid), dicdata(d)(mid).TeamA, dicdata(d)(mid).TeamB))
                        Next
                    Next
                End If

            End If

        End Sub

        Public Function GetDataMatchs(ReturnData As Boolean) As String

            Dim strresp As New System.Text.StringBuilder
            Dim year As String = appSett.Year

            Players.Data.LoadPlayers(appSett, False)

            matchs.Clear()
            matchsplayers.Clear()
            matchsevent.Clear()

            'Leggo il calendario delle partite con i risultati'
            strresp.AppendLine(GetCalendarMatchs(ReturnData))

            'Leggo i tabelli delle partite'
            Call LoadWebMatchs()
            'Call GetMatchsPlayersDataNew()
            Call GetMatchsPlayersData()

            Return strresp.ToString()

        End Function

        Private Function GetCalendarMatchs(ReturnData As Boolean) As String

            Try

                Dim filed As String = GetMatchFileName(appSett)

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

                If appSett.DataFromDatabase Then
                    Dim mdata As New Torneo.MatchsData(appSett)
                    mdata.UpdateMatchData(matchs)
                End If

                IO.File.WriteAllText(filed, Functions.SerializzaOggetto(matchs, False))

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                Return ""
            End Try

            If ReturnData Then
                Return "</br><span style=color:red;font-size:bold;'>Match data (" & appSett.Year & "):</span></br>" & WebData.Functions.SerializzaOggetto(matchs, False).Replace(System.Environment.NewLine, "</br>") & "</br>"
            Else
                Return ("</br><span style=color:red;font-size:bold;'>Match data (" & appSett.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br><span style=color:red;font-size:bold;'>Detail match data:</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
            End If

        End Function

        Private Sub GetMatchsPlayersDataNew()
            Try

                Dim filedetd As String = GetMatchPlayersFileName(appSett)

                'Carico i dati dell'ultima lettura'
                Dim lastday As Integer = GetLastMatchsDayLoaded()

                GetMatchLiveScoreLinks()

                If diclinkdaymatch.Count > 0 Then

                    'Creo la lista di thread da esegu<ire'
                    thrmatch.Clear()
                    For Each d As String In diclinkdaymatch.Keys
                        'Ricarico i dati se nella precedente acquisizione non e' sono stati prelevati i tabellini della giornata
                        'oppure se la giornata non e' piu' vecchia di di due giornate oppure se non tutti i match della giornata
                        'stati disputati'
                        If CInt(d) > lastday - 2 OrElse diclinkdaymatch(d).Count < 10 Then
                            Dim t As New Threading.Thread(Sub(x) GetMatchLiveScoreByDay(d))
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
                        Dim fday As String = GetMatchPlayersDayFileName(appSett, i.ToString())
                        If IO.File.Exists(fday) Then
                            Dim dicdata As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)) = Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)))(IO.File.ReadAllText(fday))
                            dicalldata.Add(CStr(i), dicdata)
                        End If
                    Next

                    If appSett.DataFromDatabase AndAlso matchsplayers.Count > 0 Then
                        Dim mdata As New Torneo.MatchsData(appSett)
                        mdata.UpdateMatchsDataPlayers(matchsplayers)
                        mdata.UpdateMatchsDataEvents(matchsevent)
                    End If

                    IO.File.WriteAllText(filedetd, Functions.SerializzaOggetto(dicalldata, False))

                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub GetMatchsPlayersData()

            Try

                Dim filedetd As String = GetMatchPlayersFileName(appSett)

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
                        Dim fday As String = GetMatchPlayersDayFileName(appSett, i.ToString())
                        If IO.File.Exists(fday) Then
                            Dim dicdata As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)) = Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)))(IO.File.ReadAllText(fday))
                            dicalldata.Add(CStr(i), dicdata)
                        End If
                    Next

                    If appSett.DataFromDatabase AndAlso matchsplayers.Count > 0 Then
                        Dim mdata As New Torneo.MatchsData(appSett)
                        mdata.UpdateMatchsDataPlayers(matchsplayers)
                        mdata.UpdateMatchsDataEvents(matchsevent)
                    End If

                    IO.File.WriteAllText(filedetd, Functions.SerializzaOggetto(dicalldata, False))

                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub GetMatchsDay()
            Try
                'Determino la giornata di riferimento'
                Dim d As String = Threading.Thread.CurrentThread.Name
                Dim datamatch As New Dictionary(Of String, String)
                Dim filet As String = dirt & "match-day-" & d & ".txt"
                Dim filed As String = dirt & "match-day-" & d & ".txt"
                Dim html As String = Functions.GetPage(appSett, "https://www.fantacalcio.it/serie-a/calendario/" & d)

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
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Function GetLastMatchsDayLoaded() As Integer
            For i As Integer = 1 To 38
                If IO.File.Exists(GetMatchPlayersDayFileName(appSett, i.ToString())) = False Then
                    Return i - 1
                End If
            Next
            Return 38
        End Function

        Private Sub GetMatchsPlayersDataByDay()
            Try
                'Determino la giornata di riferimento'
                Dim d As String = Threading.Thread.CurrentThread.Name

                If matchsplayers.ContainsKey(d) = False Then matchsplayers.Add(d, New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer))))
                If matchsevent.ContainsKey(d) = False Then matchsevent.Add(d, New Dictionary(Of String, Dictionary(Of String, List(Of Torneo.MatchsData.MatchEvent))))

                For Each m As String In diclinkdaymatch(d).Keys
                    Call GetMatchsPlayersDataByDayMatchId(d, m, diclinkdaymatch(d)(m))
                Next

                IO.File.WriteAllText(GetMatchPlayersDayFileName(appSett, d), WebData.Functions.SerializzaOggetto(matchsplayers(d), False))
                IO.File.WriteAllText(GetMatchEventsDayFileName(appSett, d), WebData.Functions.SerializzaOggetto(matchsevent(d), False))

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub GetMatchsPlayersDataByDayMatchId(day As String, MatchId As String, Link As String)

            Dim filet As String = dirt & "match-day-" & day & "-matchid-" & MatchId & ".txt"
            Dim str As New System.Text.StringBuilder

            Try

                If matchsplayers(day).ContainsKey(MatchId) = False Then matchsplayers(day).Add(MatchId, New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)))
                If matchsevent(day).ContainsKey(MatchId) = False Then matchsevent(day).Add(MatchId, New Dictionary(Of String, List(Of Torneo.MatchsData.MatchEvent)))

                Dim matchp As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)) = matchsplayers(day)(MatchId)
                Dim matche As New SortedDictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent))

                Dim html As String = Functions.GetPage(appSett, Link & "/riepilogo")

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
                                AddPlayer(matchp, CInt(day), CInt(MatchId), team, Ruolo, name)
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

                            If line(z).Contains("title=""") AndAlso Regex.Match(line(z).Trim, "title=""(Subentrato|Ammonizione|Gol segnato|Gol subito|Autorete|Espulsione|Rigore segnato|Rigore sbagliato)""></figure>").Success Then

                                Dim n1 As String = p(0).GetName()
                                Dim r1 As String = p(0).GetRole()
                                Dim n2 As String = If(p.Count > 1, p(1).GetName(), "")
                                Dim r2 As String = If(p.Count > 1, p(1).GetRole(), "")

                                If matche.ContainsKey(min) = False Then matche.Add(min, New List(Of Torneo.MatchsData.MatchEvent))


                                If line(z).Trim.Contains("Ammonizione") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).Ammonizione += 1
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Ammonizione", min, team, n1, ""))
                                End If
                                If line(z).Trim.Contains("Espulsione") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).Espulsione += 1
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Espulsione", min, team, n1, ""))
                                End If
                                If line(z).Trim.Contains("Gol subito") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).GoalSubiti += 1
                                End If
                                If line(z).Trim.Contains("Rigore sbagliato") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).RigoriSbagliati += 1
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Rigore sbagliato", min, team, n1, ""))
                                End If
                                If (line(z).Trim.Contains("Gol segnato") OrElse line(z).Trim.Contains("Rigore segnato")) AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).GoalFatti += 1
                                    If p.Count > 1 Then
                                        matchp(team)(n2).Assists += 1
                                        matche(min).Add(New Torneo.MatchsData.MatchEvent("Goal", min, team, n1, n2))
                                    Else
                                        matche(min).Add(New Torneo.MatchsData.MatchEvent("Goal", min, team, n1, ""))
                                    End If
                                End If
                                If line(z).Trim.Contains("Subentrato") AndAlso p.Count > 1 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).Subentrato = 1
                                    matchp(team)(n1).Minuti = min
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r2, n2)
                                    matchp(team)(n2).Sostituito = 1
                                    matchp(team)(n2).Minuti = min
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Sostituzione", min, team, n2, n1))
                                End If
                            End If
                        End If
                        If line(z).Contains("title=""") AndAlso Regex.Match(line(z).Trim, "title="".*""></figure>").Success Then
                            p.Clear()
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

                For Each mm As Integer In matche.Keys
                    matchsevent(day)(MatchId).Add(mm.ToString(), matche(mm))
                Next

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

        End Sub

        Private Sub GetMatchLiveScoreLinks()

            Dim filet As String = dirt & "match-livescore-calendar.json"
            Dim enc As String = "utf-8"

            Try
                Dim json As String = Functions.GetPage(appSett, "https://prod-cdn-public-api.livescore.com/v1/api/app/competition/77/details/1/?locale=it")
                If json <> "" Then

                    IO.File.WriteAllText(filet, json, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = Functions.FormatJson(json).Split(Convert.ToChar(13))
                    Dim eid As String = ""
                    Dim teamA As String = ""

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            If line.Contains("Eid") Then
                                eid = GetJsonPropertyValue(line)
                            ElseIf line.Contains("""T1"": [") Then
                                teamA = Functions.CheckTeamName(GetJsonPropertyValue(lines(i + 2)).ToUpper().Trim())
                            ElseIf line.Contains("""T2"": [") Then
                                If teamA <> "" Then
                                    Dim teamB As String = Functions.CheckTeamName(GetJsonPropertyValue(lines(i + 2)).ToUpper().Trim())
                                    Dim key As String = teamA & "-" & teamB
                                    If KeyMatchs.ContainsKey(key) Then
                                        Dim m As MatchRound = KeyMatchs(key)
                                        If diclinkdaymatch.ContainsKey(m.Giornata.ToString()) = False Then diclinkdaymatch.Add(m.Giornata.ToString(), New Dictionary(Of String, String))
                                        If diclinkdaymatch.ContainsKey(m.MatchId.ToString()) = False Then diclinkdaymatch(m.Giornata.ToString()).Add(m.MatchId.ToString(), "https://www.livescore.com/_next/data/it/calcio/italia/serie-a/" & teamA.ToLower() & "-vs-" & teamB.ToLower() & "/" & eid & ".json?")
                                    End If
                                End If
                                teamA = ""
                            End If
                        End If
                    Next

                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub GetMatchLiveScoreByDay(giornata As String)
            Try
                If matchsplayers.ContainsKey(giornata) = False Then matchsplayers.Add(giornata, New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer))))
                If matchsevent.ContainsKey(giornata) = False Then matchsevent.Add(giornata, New Dictionary(Of String, Dictionary(Of String, List(Of Torneo.MatchsData.MatchEvent))))

                For Each m As String In diclinkdaymatch(giornata).Keys
                    Call GetMatchLiveScoreByMatchId(giornata, m)
                Next

                IO.File.WriteAllText(GetMatchPlayersDayFileName(appSett, giornata), WebData.Functions.SerializzaOggetto(matchsplayers(giornata), False))
                IO.File.WriteAllText(GetMatchEventsDayFileName(appSett, giornata), WebData.Functions.SerializzaOggetto(matchsevent(giornata), False))

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub GetMatchLiveScoreByMatchId(giornata As String, MatchId As String)

            Dim filet1 As String = dirt & "match-livescore-day-" & giornata & "-matchid-" & MatchId & "_sum.json"
            Dim filet2 As String = dirt & "match-livescore-day-" & giornata & "-matchid-" & MatchId & "_lineup.json"
            Dim enc As String = "utf-8"
            Dim locMatchPlayer As New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer))
            Dim locMatchsevent As New SortedDictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent))


            Try
                If diclinkdaymatch.ContainsKey(giornata) AndAlso diclinkdaymatch(giornata).ContainsKey(MatchId) Then

                    Dim json As String = Functions.GetPage(appSett, diclinkdaymatch(giornata)(MatchId))

                    If json.Contains("buildid") = False Then json = Functions.GetPage(appSett, diclinkdaymatch(giornata)(MatchId))

                    If json <> "" Then

                        json = Functions.FormatJson(json)

                        Dim code As String = System.Text.RegularExpressions.Regex.Match(json, "(?<=buildid\=)[\w\-]{1,}").Value()

                        If code <> "" Then

                            Dim link1 As String = diclinkdaymatch(giornata)(MatchId).Replace("/it/", "/" & code & "/it/")
                            Dim link2 As String = link1.Replace(".json?", "/lineups.json?")

                            json = Functions.GetPage(appSett, link2)

                            If json <> "" Then
                                json = Functions.FormatJson(json)
                                IO.File.WriteAllText(filet1, json, System.Text.Encoding.GetEncoding(enc))
                            End If

                            json = Functions.GetPage(appSett, link1)

                            If json <> "" Then
                                json = Functions.FormatJson(json)
                                IO.File.WriteAllText(filet2, json, System.Text.Encoding.GetEncoding(enc))
                            End If

                            ReadMatchLiveScoreByMatchId(filet1, filet2, giornata, MatchId)

                        End If
                    End If
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub ReadMatchLiveScoreByMatchId(file1 As String, file2 As String, giornata As String, MatchId As String)

            Dim locMatchPlayer As New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer))
            Dim locMatchsevent As New SortedDictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent))

            Try

                Dim json = IO.File.ReadAllText(file1)
                Dim lines() As String = json.Replace(vbCrLf, vbCr).Replace(vbLf, "").Split(Convert.ToChar(13))
                Dim team As String = ""
                Dim teamA As String = ""
                Dim teamB As String = ""
                Dim name As String = ""
                Dim nameOut As String = ""
                Dim nameIn As String = ""
                Dim min As Integer = 0

                Dim paths As New List(Of String)
                Dim cpath As String = ""

                For i As Integer = 0 To lines.Length - 1
                    If lines(i) <> "" Then

                        Dim line As String = lines(i)

                        Dim pname As String = GetJsonPropertyName(line)
                        Dim pvalue As String = GetJsonPropertyValue(line)

                        If line.EndsWith("{") OrElse line.EndsWith("[") Then
                            paths.Add(pname)
                            cpath = Torneo.Functions.ConvertListStringToString(paths, "/")
                            If pname = "homeStarters" Then
                                team = teamA
                            ElseIf pname = "awayStarters" Then
                                team = teamB
                            ElseIf pname = "homeSubs" Then
                                team = teamA
                            ElseIf pname = "awaySubs" Then
                                team = teamB
                            ElseIf pname = "home" Then
                                team = teamA
                            ElseIf pname = "away" Then
                                team = teamB
                            End If
                        ElseIf (line.EndsWith("],") OrElse line.EndsWith("},") OrElse line.EndsWith("}") OrElse line.EndsWith("]")) Then
                            If paths.Count > 0 Then paths.RemoveAt(paths.Count - 1)
                        ElseIf line.EndsWith(",") Then
                            If pname = "team1" AndAlso cpath = "/pageProps/layoutContext/metaParams" Then
                                teamA = Functions.CheckTeamName(pvalue)
                            ElseIf pname = "team2" AndAlso cpath = "/pageProps/layoutContext/metaParams" Then
                                teamB = Functions.CheckTeamName(pvalue)
                            ElseIf line.Contains("shortName") AndAlso (cpath.EndsWith("homeStarters/") OrElse cpath.EndsWith("awayStarters/") OrElse cpath.EndsWith("homeSubs/") OrElse cpath.EndsWith("awaySubs/")) Then
                                name = pvalue.ToUpper()
                                Dim pm As Players.PlayerMatch = Players.Data.ResolveName("", name, team, False)
                                name = pm.GetName
                                Dim ruolo As String = pm.GetRole()
                                If locMatchPlayer.ContainsKey(team) = False Then locMatchPlayer.Add(team, New Dictionary(Of String, Torneo.MatchsData.MatchPlayer))
                                If locMatchPlayer(team).ContainsKey(name) = False Then locMatchPlayer(team).Add(name, New Torneo.MatchsData.MatchPlayer)
                                Dim m As Torneo.MatchsData.MatchPlayer = locMatchPlayer(team)(name)
                                m.Giornata = CInt(giornata)
                                m.MatchId = CInt(MatchId)
                                m.Ruolo = ruolo
                                m.Nome = name
                                If cpath.Contains("home") Then
                                    m.Titolare = 1
                                Else
                                    m.Titolare = 0
                                End If
                            ElseIf line.Contains("shortName") AndAlso (cpath.EndsWith("subs/home//in") OrElse cpath.EndsWith("subs/home//out") OrElse cpath.EndsWith("subs/away//in") OrElse cpath.EndsWith("subs/away//out/")) Then
                                If cpath.EndsWith("out") Then
                                    nameOut = pvalue.ToUpper()
                                    Dim pm As Players.PlayerMatch = Players.Data.ResolveName("", nameOut, team, False)
                                    nameOut = pm.GetName()
                                Else
                                    nameIn = pvalue.ToUpper()
                                    Dim pm As Players.PlayerMatch = Players.Data.ResolveName("", nameIn, team, False)
                                    nameIn = pm.GetName()
                                End If
                            ElseIf pname = "time" AndAlso (cpath.EndsWith("homeStarters//sub") OrElse cpath.EndsWith("awayStarters//sub")) Then
                                Dim m As Torneo.MatchsData.MatchPlayer = locMatchPlayer(team)(name)
                                m.Sostituito = 1
                                m.Minuti = CInt(pvalue)
                            ElseIf pname = "time" AndAlso (cpath.EndsWith("homeSubs//sub") OrElse cpath.EndsWith("awaySubs//sub")) Then
                                Dim m As Torneo.MatchsData.MatchPlayer = locMatchPlayer(team)(name)
                                m.Subentrato = 1
                                m.Minuti = CInt(pvalue)
                            ElseIf pname = "time" AndAlso (cpath.EndsWith("subs/home//in") OrElse cpath.EndsWith("subs/home//out") OrElse cpath.EndsWith("subs/away//in") OrElse cpath.EndsWith("subs/away//out")) Then
                                min = CInt(pvalue)
                                If locMatchsevent.ContainsKey(min) = False Then locMatchsevent.Add(min, New List(Of Torneo.MatchsData.MatchEvent))

                                If cpath.EndsWith("/in") Then
                                    Dim mev As Torneo.MatchsData.MatchEvent = locMatchsevent(min).Last()
                                    mev.Nome2 = nameIn
                                Else
                                    Dim mev As New Torneo.MatchsData.MatchEvent
                                    mev.Giornata = CInt(giornata)
                                    mev.MatchId = CInt(MatchId)
                                    mev.Minuto = min
                                    mev.Squadra = team
                                    mev.Nome1 = nameOut
                                    mev.EventType = "Sostituzione"
                                    locMatchsevent(min).Add(mev)
                                End If

                            End If
                        End If
                    End If
                Next

                json = IO.File.ReadAllText(file2)
                lines = json.Replace(vbCrLf, vbCr).Replace(vbLf, "").Split(Convert.ToChar(13))

                Dim type As String = ""
                paths = New List(Of String)
                cpath = ""

                For i As Integer = 0 To lines.Length - 1
                    If lines(i) <> "" Then

                        Dim line As String = lines(i)

                        Dim pname As String = GetJsonPropertyName(line)
                        Dim pvalue As String = GetJsonPropertyValue(line)

                        If line.EndsWith("{") OrElse line.EndsWith("[") Then
                            paths.Add(pname)
                            cpath = Torneo.Functions.ConvertListStringToString(paths, "/")
                            If pname = "homeStarters" Then
                                team = teamA
                            ElseIf pname = "awayStarters" Then
                                team = teamB
                            ElseIf pname = "homeSubs" Then
                                team = teamA
                            ElseIf pname = "awaySubs" Then
                                team = teamB
                            ElseIf pname.ToLower() = "home" Then
                                team = teamA
                            ElseIf pname.ToLower() = "away" Then
                                team = teamB
                            End If
                        ElseIf (line.EndsWith("],") OrElse line.EndsWith("},") OrElse line.EndsWith("}") OrElse line.EndsWith("]")) Then
                            If paths.Count > 0 Then paths.RemoveAt(paths.Count - 1)
                        ElseIf line.EndsWith(",") Then
                            If pname = "team1" AndAlso cpath = "/pageProps/layoutContext/metaParams" Then
                                teamA = Functions.CheckTeamName(pvalue.Replace("CALCIO", "").Trim())
                            ElseIf pname = "team2" AndAlso cpath = "/pageProps/layoutContext/metaParams" Then
                                teamB = Functions.CheckTeamName(pvalue.Replace("CALCIO", "").Trim())
                            ElseIf line.Contains("shortName") Then
                                name = pvalue.ToUpper()
                                Dim pm As Players.PlayerMatch = Players.Data.ResolveName("", name, team, False)
                                name = pm.GetName()
                                If cpath.EndsWith("assist/") Then
                                    Dim m As Torneo.MatchsData.MatchEvent = locMatchsevent(min).Last()
                                    m.Nome2 = name
                                End If
                            ElseIf pname = "time" Then
                                Dim s = pvalue.Replace("'", "")
                                If s.Contains(" + ") Then
                                    min = CInt(s.Substring(0, s.IndexOf(" ") - 1))
                                    For k As Integer = 1 To 20
                                        If s.Contains(" + " & CStr(k)) Then
                                            min += k
                                            Exit For
                                        End If
                                    Next
                                Else
                                    min = CInt(pvalue.Replace("'", ""))
                                End If
                            ElseIf line.Contains("type") AndAlso line.Contains("liveactions") = False Then

                                If locMatchsevent.ContainsKey(min) = False Then locMatchsevent.Add(min, New List(Of Torneo.MatchsData.MatchEvent))

                                Dim mev As New Torneo.MatchsData.MatchEvent
                                mev.Giornata = CInt(giornata)
                                mev.MatchId = CInt(MatchId)
                                mev.Minuto = min
                                mev.Squadra = team
                                If pvalue = "FootballGoal" Then
                                    mev.EventType = "Goal"
                                ElseIf pvalue = "FootballYellowCard" Then
                                    mev.EventType = "Ammonizione"
                                ElseIf pvalue = "FootballRedYellowCard" Then
                                    mev.EventType = "Espulsione"
                                ElseIf pvalue = "FootballGoalPenMiss" Then
                                    mev.EventType = "Rigore sbagliato"
                                ElseIf pvalue = "FootballGoalPen" Then
                                    mev.EventType = "Rigore tirato"
                                End If
                                mev.Nome1 = name
                                If mev.EventType <> "" Then locMatchsevent(min).Add(mev)
                            End If

                        End If
                    End If
                Next

                Dim minfin As Integer = locMatchsevent.Keys.Max()
                If minfin < 90 Then minfin = 90

                For Each t As String In locMatchPlayer.Keys
                    For Each n As String In locMatchPlayer(t).Keys
                        If locMatchPlayer(t)(n).Titolare = 0 AndAlso locMatchPlayer(t)(n).Subentrato = 1 Then
                            locMatchPlayer(t)(n).Minuti = minfin - locMatchPlayer(t)(n).Minuti
                        End If
                    Next
                Next
                For Each m As Integer In locMatchsevent.Keys
                    For Each mev As Torneo.MatchsData.MatchEvent In locMatchsevent(m)
                        If locMatchPlayer.ContainsKey(mev.Squadra) AndAlso locMatchPlayer(mev.Squadra).ContainsKey(mev.Nome1) Then
                            Dim mp As Torneo.MatchsData.MatchPlayer = locMatchPlayer(mev.Squadra)(mev.Nome1)
                            If mev.EventType = "Goal" Then
                                mp.GoalFatti += 1
                            ElseIf mev.EventType = "Ammonizione" Then
                                mp.Ammonizione = 1
                            ElseIf mev.EventType = "Espulsione" Then
                                mp.Espulsione = 1
                            ElseIf mev.EventType = "Rigore sbagliato" Then
                                mp.RigoriSbagliati += 1
                            End If
                        End If
                    Next
                Next

                matchsplayers(giornata).Add(MatchId, locMatchPlayer)
                matchsevent(giornata).Add(MatchId, New Dictionary(Of String, List(Of Torneo.MatchsData.MatchEvent)))
                For Each mm As Integer In locMatchsevent.Keys
                    matchsevent(giornata)(MatchId).Add(mm.ToString(), locMatchsevent(mm))
                Next

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Function GetJsonPropertyName(Value As String) As String
            Return System.Text.RegularExpressions.Regex.Match(Value, ".*(?=:)").Value().Replace(Convert.ToChar(34), "").Trim()
        End Function

        Private Function GetJsonPropertyValue(Value As String) As String
            Return System.Text.RegularExpressions.Regex.Match(Value, "(?<=:).*").Value().Replace(",", "").Replace(Convert.ToChar(34), "").Trim()
        End Function

        Private Shared Sub AddPlayer(match As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)), giornata As Integer, matchid As Integer, team As String, ruolo As String, name As String)
            If match.ContainsKey(team) = False Then match.Add(team, New Dictionary(Of String, Torneo.MatchsData.MatchPlayer))
            If match(team).ContainsKey(name) = False Then
                match(team).Add(name, New Torneo.MatchsData.MatchPlayer())
                match(team)(name).Giornata = giornata
                match(team)(name).MatchId = matchid
                match(team)(name).Ruolo = ruolo
                match(team)(name).Nome = name
                match(team)(name).Squadra = team
            Else
                name = name
            End If
        End Sub


        Public Class MatchRound
            Public Property Giornata As Integer = 0
            Public Property MatchId As Integer = 0
            Public Property TeamA As String = ""
            Public Property TeamB As String = ""

            Public Sub New(Giornata As Integer, MatchId As Integer, TeamA As String, TeamB As String)
                Me.Giornata = Giornata
                Me.MatchId = MatchId
                Me.TeamA = TeamA
                Me.TeamB = TeamB
            End Sub

        End Class
    End Class
End Namespace