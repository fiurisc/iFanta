Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.LinkLabel
Imports Newtonsoft.Json.Linq
Imports webfuction.Torneo.MatchsData
Imports webfuction.WebData.Players


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
        Private Shared matchsevent As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent))))

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
            'strresp.AppendLine(GetCalendarMatchs(ReturnData))

            'Leggo i tabelli delle partite'
            Call LoadWebMatchs()
            Call GetMatchsPlayersDataNew()
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

                    'Creo la lista di thread da eseguire'
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
            Return -1
        End Function

        Private Sub GetMatchsPlayersDataByDay()
            Try
                'Determino la giornata di riferimento'
                Dim d As String = Threading.Thread.CurrentThread.Name

                If matchsplayers.ContainsKey(d) = False Then matchsplayers.Add(d, New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer))))
                If matchsevent.ContainsKey(d) = False Then matchsevent.Add(d, New Dictionary(Of String, Dictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent))))

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
                If matchsevent(day).ContainsKey(MatchId) = False Then matchsevent(day).Add(MatchId, New Dictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent)))

                Dim matchp As Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer)) = matchsplayers(day)(MatchId)
                Dim matche As Dictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent)) = matchsevent(day)(MatchId)

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
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Ammonizione", min, team, n1))
                                End If
                                If line(z).Trim.Contains("Espulsione") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).Espulsione += 1
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Espulsione", min, team, n1))
                                End If
                                If line(z).Trim.Contains("Gol subito") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).GoalSubiti += 1
                                End If
                                If line(z).Trim.Contains("Rigore sbagliato") AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).RigoriSbagliati += 1
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Rigore sbagliato", min, team, n1))
                                End If
                                If (line(z).Trim.Contains("Gol segnato") OrElse line(z).Trim.Contains("Rigore segnato")) AndAlso p.Count > 0 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).GoalFatti += 1
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Goal", min, team, n1))
                                    If p.Count > 1 Then
                                        AddPlayer(matchp, CInt(day), CInt(MatchId), team, r2, n2)
                                        matchp(team)(n2).Assists += 1
                                        matche(min).Add(New Torneo.MatchsData.MatchEvent("Assist", min, team, n2))
                                    End If
                                End If
                                If line(z).Trim.Contains("Subentrato") AndAlso p.Count > 1 Then
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r1, n1)
                                    matchp(team)(n1).Subentrato = 1
                                    matchp(team)(n1).Minuti = min
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("Subentrato", min, team, n1))
                                    AddPlayer(matchp, CInt(day), CInt(MatchId), team, r2, n2)
                                    matchp(team)(n2).Sostituito = 1
                                    matchp(team)(n2).Minuti = min
                                    matche(min).Add(New Torneo.MatchsData.MatchEvent("sostituito", min, team, n2))
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
                                teamA = GetJsonPropertyValue(lines(i + 2)).ToUpper().Replace("CALCIO", "").Trim()
                            ElseIf line.Contains("""T2"": [") Then
                                If teamA <> "" Then
                                    Dim teamB As String = GetJsonPropertyValue(lines(i + 2)).ToUpper().Replace("CALCIO", "").Trim()
                                    Dim key As String = Functions.CheckTeamName(teamA) & "-" & Functions.CheckTeamName(teamB)
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
                If matchsevent.ContainsKey(giornata) = False Then matchsevent.Add(giornata, New Dictionary(Of String, Dictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent))))

                For Each m As String In diclinkdaymatch(giornata).Keys
                    Call GetMatchLiveScoreByMatchId(giornata, m)
                Next

                IO.File.WriteAllText(GetMatchPlayersDayFileName(appSett, giornata), WebData.Functions.SerializzaOggetto(matchsplayers(giornata), False))
                'IO.File.WriteAllText(GetMatchEventsDayFileName(appSett, giornata), WebData.Functions.SerializzaOggetto(matchsevent(d), False))

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub GetMatchLiveScoreByMatchId(giornata As String, MatchId As String)

            Dim filet1 As String = dirt & "match-livescore-day-" & giornata & "-matchid-" & MatchId & "_summery.json"
            Dim filet2 As String = dirt & "match-livescore-day-" & giornata & "-matchid-" & MatchId & "_lineup.json"
            Dim enc As String = "utf-8"
            Dim locMatchPlayer As New Dictionary(Of String, Dictionary(Of String, Torneo.MatchsData.MatchPlayer))
            Dim locMatchsevent As New SortedDictionary(Of Integer, List(Of Torneo.MatchsData.MatchEvent))


            Try
                If diclinkdaymatch.ContainsKey(giornata) AndAlso diclinkdaymatch(giornata).ContainsKey(MatchId) Then

                    Dim json As String = Functions.GetPage(appSett, diclinkdaymatch(giornata)(MatchId))

                    If json <> "" Then

                        Dim code As String = System.Text.RegularExpressions.Regex.Match(json, "(?<=buildid\=)[\w\-]{1,}").Value()

                        If code <> "" Then

                            Dim link1 As String = diclinkdaymatch(giornata)(MatchId).Replace("/it/", "/" & code & "/it/")
                            Dim link2 As String = link1.Replace(".json?", "/lineups.json?")

                            json = Functions.GetPage(appSett, link2, enc)

                            If json <> "" Then

                                json = Functions.FormatJson(json)
                                IO.File.WriteAllText(filet2, json, System.Text.Encoding.GetEncoding(enc))

                                Dim lines() As String = json.Replace(vbCrLf, vbCr).Replace(vbLf, "").Split(Convert.ToChar(13))
                                Dim eid As String = ""
                                Dim team As String = ""
                                Dim teamA As String = ""
                                Dim teamB As String = ""
                                Dim type As String = ""
                                Dim name As String = ""
                                Dim nbrspace As Integer = 0

                                For i As Integer = 0 To lines.Length - 1

                                    If lines(i) <> "" Then

                                        Dim line As String = lines(i)

                                        If line.Contains("team1") Then
                                            teamA = Functions.CheckTeamName(GetJsonPropertyValue(line.ToUpper().Replace("CALCIO", "").Trim))
                                        ElseIf line.Contains("team2") Then
                                            teamB = Functions.CheckTeamName(GetJsonPropertyValue(line.ToUpper().Replace("CALCIO", "").Trim))
                                        ElseIf line.Contains("homeStarters") Then
                                            team = teamA
                                            nbrspace = line.Length - line.Trim().Length
                                            type = "Titolari"
                                        ElseIf line.Contains("awayStarters") Then
                                            team = teamB
                                            nbrspace = line.Length - line.Trim().Length
                                            type = "Titolari"
                                        ElseIf line.Contains("homeSubs") Then
                                            team = teamA
                                            nbrspace = line.Length - line.Trim().Length
                                            type = "Panchina"
                                        ElseIf line.Contains("awaySubs") Then
                                            team = teamB
                                            nbrspace = line.Length - line.Trim().Length
                                            type = "Panchina"
                                        ElseIf line.Contains("shortName") Then
                                            If type <> "" Then
                                                name = GetJsonPropertyValue(line.ToUpper())
                                                Dim pm As PlayerMatch = Players.Data.ResolveName("", name, team, False)
                                                name = pm.GetName
                                                Dim ruolo As String = pm.GetRole()
                                                If locMatchPlayer.ContainsKey(team) = False Then locMatchPlayer.Add(team, New Dictionary(Of String, MatchPlayer))
                                                If locMatchPlayer(team).ContainsKey(name) = False Then locMatchPlayer(team).Add(name, New MatchPlayer)
                                                Dim m As MatchPlayer = locMatchPlayer(team)(name)
                                                m.Giornata = CInt(giornata)
                                                m.MatchId = CInt(MatchId)
                                                m.Ruolo = ruolo
                                                m.Nome = name
                                                If type = "Titolari" Then
                                                    m.Titolare = 1
                                                Else
                                                    m.Titolare = 0
                                                End If
                                            End If
                                        ElseIf line.Contains("""sub"": {") AndAlso type = "Titolari" Then
                                            Dim m As MatchPlayer = locMatchPlayer(team)(name)
                                            m.Sostituito = 1
                                            If type = "Titolari" Then
                                                m.Titolare = 1
                                            Else
                                                m.Titolare = 0
                                            End If
                                            m.Minuti = CInt(GetJsonPropertyValue(lines(i + 9)))
                                        ElseIf line.Contains("},") OrElse line.Contains("],") Then
                                            Dim g As Integer = line.Length - line.Trim().Length
                                            If nbrspace = line.Length - line.Trim().Length Then
                                                type = ""
                                            End If
                                        End If

                                        If line.Contains("shortName") Then
                                            name = GetJsonPropertyValue(line)
                                        End If
                                        If line.Contains("position") Then

                                        End If
                                    End If
                                Next
                            End If

                            json = Functions.GetPage(appSett, link1)

                            If json <> "" Then
                                json = Functions.FormatJson(json)
                                IO.File.WriteAllText(filet1, json, System.Text.Encoding.GetEncoding(enc))
                            End If

                        End If
                    End If
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

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