Imports System.Text.RegularExpressions
Imports Newtonsoft.Json

Partial Class WebData
    Public Class MatchData

        Private thrmatch As New List(Of Threading.Thread)
        Private diclinkday As New SortedDictionary(Of Integer, String)
        Private diclinkdaymatch As New SortedDictionary(Of Integer, Dictionary(Of Integer, String))
        Private dirt As String = ""
        Private dird As String = ""
        Private strdata As New SortedDictionary(Of Integer, System.Text.StringBuilder)
        Private year As Integer = -1

        Private matchs As New SortedDictionary(Of Integer, SortedDictionary(Of Integer, Match))
        Private matchsdetails As New SortedDictionary(Of Integer, SortedDictionary(Of Integer, Dictionary(Of String, Dictionary(Of String, Player))))

        Sub New(YearLega As Integer)
            year = YearLega
        End Sub

        Function GetCalendarMatchs(ServerPath As String, ReturnData As Boolean) As String

            Dim filed As String = ""
            Dim strdataall As New System.Text.StringBuilder

            dirs = ServerPath
            dirt = ServerPath & "\web\" & CStr(year) & "\temp"
            dird = ServerPath & "\web\" & CStr(year) & "\data\matchs"
            filed = dird & "\matchs-data.txt"

            Call MakeDirectory(ServerPath, year)
            Call LoadWebPlayers(ServerPath & "\web\" & CStr(year) & "\data\players-quote.txt")

            Try

                'Creo la lista di thread da eseguire'
                thrmatch.Clear()
                For i As Integer = 1 To 6
                    Dim t As New Threading.Thread(AddressOf GetMatchsDay)
                    t.Name = CStr(i)
                    thrmatch.Add(t)
                    'If i > 1 Then Exit For
                Next

                'Lancio i vari Thread'
                For i As Integer = 0 To thrmatch.Count - 1
                    thrmatch(i).Priority = Threading.ThreadPriority.Normal
                    thrmatch(i).Start()
                    thrmatch(i).Join()
                    System.Threading.Thread.Sleep(100)
                Next

                If Torneo.General.DataOnDb Then
                    Torneo.General.UpdateMatchData(ServerPath, year, matchs)
                End If

                For Each key As Integer In strdata.Keys
                    strdataall.Append(strdata(key).ToString)
                Next

                IO.File.WriteAllText(filed, strdataall.ToString)

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ""
            End Try

            Call GetMatchsScoresheet()

            If ReturnData Then
                strdataall.Append("</br><span style=color:red;font-size:bold;'>Detail match data:</span></br>")
                For Each key As Integer In strdata.Keys
                    strdataall.Append(strdata(key).ToString)
                Next
                Return "</br><span style=color:red;font-size:bold;'>Match data (" & year & "):</span></br>" & strdataall.ToString.Replace(System.Environment.NewLine, "</br>") & "</br>"
            Else
                Return ("</br><span style=color:red;font-size:bold;'>Match data (" & year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br><span style=color:red;font-size:bold;'>Detail match data:</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
            End If

        End Function

        Private Sub GetMatchsDay()
            Try
                'Determino la giornata di riferimento'
                Dim d As Integer = CInt(Threading.Thread.CurrentThread.Name)

                Dim datamatch As New Dictionary(Of String, String)
                Dim filet As String = dirt & "\match-day-" & CStr(d) & ".txt"
                Dim filed As String = dirt & "\match-day-" & CStr(d) & ".txt"
                Dim html As String = GetPage("https://www.fantacalcio.it/serie-a/calendario/" & CStr(d), "POST", "")

                If html <> "" Then

                    Dim day As String = ""
                    Dim dt As New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day, 0, 0, 0)
                    Dim squadraa As String = ""
                    Dim squadrab As String = ""
                    Dim line() As String
                    Dim matchid As Integer = 1
                    Dim matchplayed As Boolean = False
                    Dim goal1 As String = ""
                    Dim goal2 As String = ""

                    strdata.Add(d, New System.Text.StringBuilder)

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

                                Dim f As String = line(2057)
                                Dim mtach As String = Regex.Match(line(i), "(?<=\d+-\d+\/)\w+-\w+(?=\/\d+)").Value.ToUpper()

                                If dt < Date.Now Then
                                    matchplayed = True
                                Else
                                    matchplayed = False
                                    goal1 = ""
                                    goal2 = ""
                                End If

                                If datamatch.ContainsKey(d & "-" & mtach) = False Then
                                    datamatch.Add(d & "-" & mtach, d & "|" & matchid & "|" & mtach.Replace("-", "|") & "|" & dt.ToString("yyyy/MM/dd HH:mm:ss") & "|" & goal1 & "|" & goal2)
                                    If matchs.ContainsKey(d) = False Then matchs.Add(d, New SortedDictionary(Of Integer, Match))
                                    If matchs(d).ContainsKey(matchid) = False Then
                                        Dim teams() As String = mtach.Split("-")
                                        Dim m As New Match
                                        m.TeamA = teams(0)
                                        m.TeamB = teams(1)
                                        m.Time = dt
                                        m.GoalA = goal1
                                        m.GoalB = goal2
                                        matchs(d).Add(matchid, m)
                                    End If
                                End If

                                If matchplayed Then
                                    Dim linktab As String = Regex.Match(line(i), "(?<=content="").*(?="" />)").Value
                                    If diclinkdaymatch.ContainsKey(d) = False Then diclinkdaymatch.Add(d, New Dictionary(Of Integer, String))
                                    If diclinkdaymatch(d).ContainsKey(matchid) = False Then diclinkdaymatch(d).Add(matchid, linktab)
                                End If
                                matchid += 1
                                goal1 = ""
                                goal2 = ""
                            End If

                        End If
                    Next
                End If

                For Each key As String In datamatch.Keys
                    strdata(d).AppendLine(datamatch(key))
                Next

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Private Sub GetMatchsScoresheet()

            Try
                Dim strdataall As New System.Text.StringBuilder
                Dim filedetd As String = dird & "\matchs-detail-data.txt"

                strdata.Clear()

                'Carico i dati dell'ultima lettura'
                Dim lastday As Integer = GetLastMatchsDayLoaded()

                If diclinkdaymatch.Count > 0 Then

                    'Creo la lista di thread da eseguire'
                    thrmatch.Clear()
                    For Each d As Integer In diclinkdaymatch.Keys
                        'Ricarico i dati se nella precedente acquisizione non e' sono stati prelevati i tabellini della giornata
                        'oppure se la giornata non e' piu' vecchia di di due giornate oppure se non tutti i match della giornata
                        'stati disputati'
                        If d > lastday - 2 OrElse diclinkdaymatch(d).Count < 10 Then
                            If strdata.ContainsKey(d) Then strdata.Remove(d)
                            Dim t As New Threading.Thread(AddressOf GetDayMatchsScoresheet)
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

                    For i As Integer = 1 To 38
                        Dim fday As String = dird & "\matchs-detail-data-" & i & ".txt"
                        If IO.File.Exists(fday) Then
                            Dim line() As String = IO.File.ReadAllLines(fday)
                            For j As Integer = 0 To line.Length - 1
                                strdataall.AppendLine(line(j))
                            Next
                        End If
                    Next

                    IO.File.WriteAllText(filedetd, strdataall.ToString)

                End If
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Private Function GetLastMatchsDayLoaded() As Integer
            For i As Integer = 1 To 38
                If IO.File.Exists(dird + "\matchs-detail-data-" & i & ".txt") = False Then
                    Return i - 1
                End If
            Next
            Return -1
        End Function

        Private Sub GetDayMatchsScoresheet()
            Try
                'Determino la giornata di riferimento'
                Dim d As Integer = CInt(Threading.Thread.CurrentThread.Name)

                strdata.Add(d, New System.Text.StringBuilder)
                If matchsdetails.ContainsKey(d) = False Then matchsdetails.Add(d, New SortedDictionary(Of Integer, Dictionary(Of String, Dictionary(Of String, Player))))

                For Each m As Integer In diclinkdaymatch(d).Keys
                    Call GetDayMatchScoresheet(d, m, diclinkdaymatch(d)(m))
                Next

                IO.File.WriteAllText(dird & "\matchs-detail-data-" & d & ".txt", strdata(d).ToString)
                'IO.File.WriteAllText(dird & "\matchs-detail-data-" & d & ".json", JsonConvert.SerializeObject(matchsdetails(d), Formatting.Indented))

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Private Sub GetDayMatchScoresheet(day As Integer, MatchId As Integer, Link As String)

            Dim filet As String = dirt & "\match-day-" & day & "-matchid-" & MatchId & ".txt"
            Dim str As New System.Text.StringBuilder

            Try

                If matchsdetails(day).ContainsKey(MatchId) = False Then matchsdetails(day).Add(MatchId, New Dictionary(Of String, Dictionary(Of String, Player)))

                Dim match As Dictionary(Of String, Dictionary(Of String, Player)) = matchsdetails(day)(MatchId)

                Dim html As String = GetPage(Link, "POST", "")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html)

                    Dim line() As String = IO.File.ReadAllLines(filet)

                    Dim team As String = ""
                    Dim p As New List(Of PlayerMatch)
                    Dim startCronaca As Boolean = False
                    Dim min As Integer = 0
                    Dim events As New Dictionary(Of String, Integer)

                    System.Threading.Thread.Sleep(1000)

                    For z As Integer = 0 To line.Count - 1

                        If line(z) <> "" Then

                            If line(z).Contains("<div class=""player-role role"" data-value=") AndAlso line(z).Contains("data-value=""{{role}}""") = False Then
                                Dim name As String = ""
                                Dim ruolo As String = Regex.Match(line(z).Trim, "(?<=data-value="")\w+").Value.ToUpper()
                                team = Regex.Match(line(z + 2).Trim, "(?<=squadre\/)\w+(?=\/)").Value.ToUpper
                                name = Regex.Match(line(z + 2).Trim.Replace("-", " "), "(?<=\/)[\w\s]{1,}(?=\/\d+)").Value.ToUpper
                                If name.Contains("FOLORUNSHO") Then
                                    name = name
                                End If
                                name = ResolveName("", name, team, False).GetName()
                                AddPlayer(match, team, name, ruolo)
                                match(team)(name).Minuti = 90
                                match(team)(name).Titolare = 1
                            End If

                            'Determino il minuto dell'evento'
                            If line(z).Contains("minute") AndAlso Regex.Match(line(z).Trim, "<div class=""minute"">\d+").Success Then
                                min = Regex.Match(line(z).Trim, "\d+").Value
                                p.Clear()
                            End If

                            If line(z).Contains("<b>Calcio d&#x27;inizio</b>") Then
                                startCronaca = True
                            End If

                            If startCronaca AndAlso line(z).Contains("https://www.fantacalcio.it/serie-a/squadre") AndAlso line(z).Contains("target=""_self"">") Then
                                team = Regex.Match(line(z).Trim, "(?<=squadre\/)\w+(?=\/)").Value.ToUpper
                                Dim n As String = Regex.Match(line(z).Trim.Replace("-", " "), "(?<=\/)[\w\s]{1,}(?=\/\d+)").Value.ToUpper
                                p.Add(ResolveName("", n, team, False))
                                If n.Contains("FOLO") Or n.Contains("ORSOLINI") Then
                                    n = n
                                End If

                            End If

                            If line(z).Contains("title=""") AndAlso Regex.Match(line(z).Trim, "title=""(Subentrato|Ammonizione|Gol segnato|Gol subito|Autorete|Espulsione)""></figure>").Success Then

                                Dim n1 As String = p(0).GetName()
                                Dim r1 As String = p(0).GetRole()
                                Dim n2 As String = If(p.Count > 1, p(1).GetName(), "")
                                Dim r2 As String = If(p.Count > 1, p(1).GetRole(), "")

                                If line(z).Trim.Contains("Ammonizione") AndAlso p.Count > 0 Then
                                    AddPlayer(match, team, n1, r1)
                                    match(team)(n1).Ammonizione += 1
                                End If
                                If line(z).Trim.Contains("Espulsione") AndAlso p.Count > 0 Then
                                    AddPlayer(match, team, n1, r1)
                                    match(team)(n1).Espulsione += 1
                                End If
                                If line(z).Trim.Contains("Gol subito") AndAlso p.Count > 0 Then
                                    AddPlayer(match, team, n1, r1)
                                    match(team)(n1).GoalSubiti += 1
                                End If
                                If line(z).Trim.Contains("Gol segnato") AndAlso p.Count > 0 Then
                                    AddPlayer(match, team, n1, r1)
                                    match(team)(n1).GoalFatti += 1
                                    If p.Count > 1 Then
                                        AddPlayer(match, team, n2, r2)
                                        match(team)(n2).Assists += 1
                                    End If
                                End If
                                If line(z).Trim.Contains("Subentrato") AndAlso p.Count > 1 Then
                                    AddPlayer(match, team, n1, r1)
                                    match(team)(n1).Subentrato = 1
                                    match(team)(n1).Minuti = min
                                    AddPlayer(match, team, n2, r2)
                                    match(team)(n2).Sostituito = 1
                                    match(team)(n2).Minuti = min
                                End If

                                'p.Clear()

                            End If

                            If line(z).Contains("Rigore") Then
                                line(z) = line(z)
                            End If

                            If line(z).Contains("title=""") AndAlso Regex.Match(line(z).Trim, "title="".*""></figure>").Success Then

                                p.Clear()
                            End If

                        End If
                    Next

                    For Each t As String In match.Keys
                        For Each n As String In match(t).Keys
                            If match(t)(n).Subentrato Then
                                If min > 90 Then
                                    match(t)(n).Minuti = min - match(t)(n).Minuti
                                Else
                                    match(t)(n).Minuti = 90 - match(t)(n).Minuti
                                End If
                            End If
                            If match(t)(n).Minuti > 90 Then match(t)(n).Minuti = 90

                            strdata(day).Append(day & "|" & MatchId & "|" & t & "|" & n & "|" & match(t)(n).Minuti)
                            strdata(day).Append("|" & match(t)(n).Titolare & "|" & match(t)(n).Sostituito & "|" & match(t)(n).Subentrato)
                            strdata(day).Append("|" & match(t)(n).Ammonizione & "|" & match(t)(n).Espulsione & "|" & match(t)(n).Assists)
                            strdata(day).Append("|" & match(t)(n).GoalFatti & "|" & match(t)(n).GoalSubiti & "|" & match(t)(n).AutoGoal)
                            strdata(day).AppendLine("|" & match(t)(n).RigoriParati & "|" & match(t)(n).RigoriSbagliati)
                        Next
                    Next
                End If

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub

        Private Sub AddPlayer(match As Dictionary(Of String, Dictionary(Of String, Player)), team As String, name As String, ruolo As String)
            If match.ContainsKey(team) = False Then match.Add(team, New Dictionary(Of String, Player))
            If match(team).ContainsKey(name) = False Then
                match(team).Add(name, New Player())
                match(team)(name).Ruolo = ruolo
            Else
                name = name
            End If
        End Sub

        Public Class Match
            Public Property TeamA As String = ""
            Public Property TeamB As String = ""
            Public Property Time As DateTime = Now
            Public Property GoalA As String = ""
            Public Property GoalB As String = ""

        End Class

        Public Class Player
            Public Property Ruolo As String = ""
            Public Property Player As String = ""
            Public Property Minuti As Integer = 90
            Public Property Titolare As Integer = 0
            Public Property Sostituito As Integer = 0
            Public Property Subentrato As Integer = 0
            Public Property Ammonizione As Integer = 0
            Public Property Espulsione As Integer = 0
            Public Property Assists As Integer = 0
            Public Property GoalFatti As Integer = 0
            Public Property GoalSubiti As Integer = 0
            Public Property AutoGoal As Integer = 0
            Public Property RigoriParati As Integer = 0
            Public Property RigoriSbagliati As Integer = 0
        End Class

    End Class
End Class