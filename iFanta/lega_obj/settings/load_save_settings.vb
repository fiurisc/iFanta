Imports System.Text.RegularExpressions
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase

Partial Class LegaObject

    Sub ReadSettings()

        _sett.Bonus.BonusDefense.Clear()
        _sett.Bonus.BonusCenterField.Clear()
        _sett.Bonus.BonusAttack.Clear()

        _sett.Bonus.BonusDefense.Add(3, 10)
        _sett.Bonus.BonusDefense.Add(4, 20)
        _sett.Bonus.BonusDefense.Add(5, 30)
        _sett.Bonus.BonusCenterField.Add(3, 10)
        _sett.Bonus.BonusCenterField.Add(4, 20)
        _sett.Bonus.BonusCenterField.Add(5, 30)
        _sett.Bonus.BonusAttack.Add(2, 10)
        _sett.Bonus.BonusAttack.Add(3, 20)

        _fadmin = GetLegaDirectory() & "\admin"
        _sett.Admin = File.Exists(_fadmin)

        _fname = GetLegaSettingsFileName()
        _fnamemy = GetMyLegaSettingsFileName()

        If File.Exists(_fname) OrElse File.Exists(_fnamemy) Then

            Try

                Dim lines As New List(Of String)
                If File.Exists(_fname) Then lines.AddRange(IO.File.ReadAllLines(_fname))
                If File.Exists(_fnamemy) Then lines.AddRange(IO.File.ReadAllLines(_fnamemy))

                For i As Integer = 0 To lines.Count - 1

                    Dim line As String = lines(i)

                    Dim para As String = Regex.Match(line, ".+(?=\= ')").Value.Trim
                    Dim value As String = Regex.Match(line, "(?<= ').+(?=')").Value
                    If para <> "" AndAlso value <> "" Then
                        Try
                            Select Case para
                                Case "Active" : _sett.Active = CBool(value)
                                Case "Year" : _sett.Year = value
                                Case "Mail administrator" : _sett.MailAdmin = value
                                Case "Number teams" : _sett.NumberOfTeams = CInt(value)
                                Case "Number days" : _sett.NumberOfDays = CInt(value)
                                Case "Enable trace reconfirmations" : _sett.EnableTraceReconfirmations = CBool(value)
                                Case "Site reference for points"
                                    _sett.Points.SiteReferenceForPoints.Clear()
                                    _sett.Points.SiteReferenceForPoints.AddRange(value.Split(CChar(",")))
                                Case "Site reference for bonus" : _sett.Points.SiteReferenceForBonus = value
                                Case "Points Admonition" : _sett.Points.Admonition = CInt(value)
                                Case "Points Expulsion" : _sett.Points.Expulsion = CInt(value)
                                Case "Points Assist [P]" : _sett.Points.Assist("P") = CInt(value)
                                Case "Points Assist [D]" : _sett.Points.Assist("D") = CInt(value)
                                Case "Points Assist [C]" : _sett.Points.Assist("C") = CInt(value)
                                Case "Points Assist [A]" : _sett.Points.Assist("A") = CInt(value)
                                Case "Points GoalConceded" : _sett.Points.GoalConceded = CInt(value)
                                Case "Points GoalScored [P]" : _sett.Points.GoalScored("P") = CInt(value)
                                Case "Points GoalScored [D]" : _sett.Points.GoalScored("D") = CInt(value)
                                Case "Points GoalScored [C]" : _sett.Points.GoalScored("C") = CInt(value)
                                Case "Points GoalScored [A]" : _sett.Points.GoalScored("A") = CInt(value)
                                Case "Points OwnGoal [P]" : _sett.Points.OwnGoal("P") = CInt(value)
                                Case "Points OwnGoal [D]" : _sett.Points.OwnGoal("D") = CInt(value)
                                Case "Points OwnGoal [C]" : _sett.Points.OwnGoal("C") = CInt(value)
                                Case "Points OwnGoal [A]" : _sett.Points.OwnGoal("A") = CInt(value)
                                Case "Points Missed penalty points [P]" : _sett.Points.MissedPenalty("P") = CInt(value)
                                Case "Points Missed penalty points [D]" : _sett.Points.MissedPenalty("D") = CInt(value)
                                Case "Points Missed penalty points [C]" : _sett.Points.MissedPenalty("C") = CInt(value)
                                Case "Points Missed penalty points [A]" : _sett.Points.MissedPenalty("A") = CInt(value)
                                Case "Counts goals scored for winner of day" : _sett.ConteggiaGoalFattiPerVittoria = CBool(value)
                                Case "Counts goals conceded for winner of day" : _sett.ConteggiaGoalSubitiPerVittoria = CBool(value)
                                Case "Number of reserver" : _sett.NumberOfReserve = CInt(value)
                                Case "Forced goalkeeper as first reserve" : _sett.ForcedGoalkeeperAsFirstReserve = CBool(value)
                                Case "Enable bonus defense" : _sett.Bonus.EnableBonusDefense = CBool(value)
                                Case "Number of substitution" : _sett.NumberOfSubstitution = CInt(value)
                                Case "Substitution type" : _sett.SubstitutionType = CType(value, LegaSettings.eSubstitutionType)
                                Case "Bonus defense source" : If value <> "" Then _sett.Bonus.BonudDefenseSource = value
                                Case "Bonus defense over and equal value" : _sett.Bonus.BonusDefenseOverEqual = CInt(value)
                                Case "Bonus defense 3" : _sett.Bonus.BonusDefense(3) = CInt(value)
                                Case "Bonus defense 4" : _sett.Bonus.BonusDefense(4) = CInt(value)
                                Case "Bonus defense 5" : _sett.Bonus.BonusDefense(5) = CInt(value)
                                Case "Enable bonus center field" : _sett.Bonus.EnableCenterField = CBool(value)
                                Case "Bonus center field source" : If value <> "" Then _sett.Bonus.BonudCenterFieldSource = value
                                Case "Bonus center field over and equal value" : _sett.Bonus.BonusCenterFieldOverEqual = CInt(value)
                                Case "Bonus center field 3" : _sett.Bonus.BonusCenterField(3) = CInt(value)
                                Case "Bonus center field 4" : _sett.Bonus.BonusCenterField(4) = CInt(value)
                                Case "Bonus center field 5" : _sett.Bonus.BonusCenterField(5) = CInt(value)
                                Case "Enable bonus attack" : _sett.Bonus.EnableBonusAttack = CBool(value)
                                Case "Bonus attack source" : If value <> "" Then _sett.Bonus.BonudAttackSource = value
                                Case "Bonus attack over and equal value" : _sett.Bonus.BonusAttackOverEqual = CInt(value)
                                Case "Bonus attack 2" : _sett.Bonus.BonusAttack(2) = CInt(value)
                                Case "Bonus attack 3" : _sett.Bonus.BonusAttack(3) = CInt(value)
                                Case "Enable jolly player" : _sett.Jolly.EnableJollyPlayer = CBool(value)
                                Case "Enable jolly goalkeeper" : _sett.Jolly.EnableJollyPlayerGoalkeeper = CBool(value)
                                Case "Enable jolly defender" : _sett.Jolly.EnableJollyPlayerDefender = CBool(value)
                                Case "Enable jolly midfielder" : _sett.Jolly.EnableJollyPlayerMidfielder = CBool(value)
                                Case "Enable jolly forward" : _sett.Jolly.EnableJollyPlayerForward = CBool(value)
                                Case "Maximum number jolly playable" : _sett.Jolly.MaximumNumberJollyPlayable = CInt(value)
                                Case "Maximum number jolly playable for day" : _sett.Jolly.MaximumNumberJollyPlayableForDay = CInt(value)
                                Case "Type of second round" : _sett.Coppa.TipoSecondoTurno = value
                                Case "PlayOff 1 Player 1" : _sett.Coppa.PlayOffGiorone1Team(0) = CInt(value)
                                Case "PlayOff 1 Player 2" : _sett.Coppa.PlayOffGiorone1Team(1) = CInt(value)
                                Case "PlayOff 1 Player 3" : _sett.Coppa.PlayOffGiorone1Team(2) = CInt(value)
                                Case "PlayOff 2 Player 1" : _sett.Coppa.PlayOffGiorone2Team(0) = CInt(value)
                                Case "PlayOff 2 Player 2" : _sett.Coppa.PlayOffGiorone2Team(1) = CInt(value)
                                Case "PlayOff 2 Player 3" : _sett.Coppa.PlayOffGiorone2Team(2) = CInt(value)
                                Case "PlayOff 1 Matchs 1" : _sett.Coppa.PlayOffGiorone1Match(0) = CInt(value)
                                Case "PlayOff 1 Matchs 2" : _sett.Coppa.PlayOffGiorone1Match(1) = CInt(value)
                                Case "PlayOff 1 Matchs 3" : _sett.Coppa.PlayOffGiorone1Match(2) = CInt(value)
                                Case "PlayOff 2 Matchs 1" : _sett.Coppa.PlayOffGiorone2Match(0) = CInt(value)
                                Case "PlayOff 2 Matchs 2" : _sett.Coppa.PlayOffGiorone2Match(1) = CInt(value)
                                Case "PlayOff 2 Matchs 3" : _sett.Coppa.PlayOffGiorone2Match(2) = CInt(value)
                                Case "Quartersfinal 1 Player 1" : _sett.Coppa.QuartiDiFinale1Team(0) = CInt(value)
                                Case "Quartersfinal 1 Player 2" : _sett.Coppa.QuartiDiFinale1Team(1) = CInt(value)
                                Case "Quartersfinal 2 Player 1" : _sett.Coppa.QuartiDiFinale2Team(0) = CInt(value)
                                Case "Quartersfinal 2 Player 2" : _sett.Coppa.QuartiDiFinale2Team(1) = CInt(value)
                                Case "Quartersfinal 3 Player 1" : _sett.Coppa.QuartiDiFinale3Team(0) = CInt(value)
                                Case "Quartersfinal 3 Player 2" : _sett.Coppa.QuartiDiFinale3Team(1) = CInt(value)
                                Case "Quartersfinal 4 Player 1" : _sett.Coppa.QuartiDiFinale4Team(0) = CInt(value)
                                Case "Quartersfinal 4 Player 2" : _sett.Coppa.QuartiDiFinale4Team(1) = CInt(value)
                                Case "Quartersfinal 1 Matchs 1" : _sett.Coppa.QuartiDiFinale1Match(0) = CInt(value)
                                Case "Quartersfinal 1 Matchs 2" : _sett.Coppa.QuartiDiFinale1Match(1) = CInt(value)
                                Case "Quartersfinal 2 Matchs 1" : _sett.Coppa.QuartiDiFinale2Match(0) = CInt(value)
                                Case "Quartersfinal 2 Matchs 2" : _sett.Coppa.QuartiDiFinale2Match(1) = CInt(value)
                                Case "Quartersfinal 3 Matchs 1" : _sett.Coppa.QuartiDiFinale3Match(0) = CInt(value)
                                Case "Quartersfinal 3 Matchs 2" : _sett.Coppa.QuartiDiFinale3Match(1) = CInt(value)
                                Case "Quartersfinal 4 Matchs 1" : _sett.Coppa.QuartiDiFinale4Match(0) = CInt(value)
                                Case "Quartersfinal 4 Matchs 2" : _sett.Coppa.QuartiDiFinale4Match(1) = CInt(value)
                                Case "Semifinal 1 Player 1" : _sett.Coppa.Semifinale1Team(0) = CInt(value)
                                Case "Semifinal 1 Player 2" : _sett.Coppa.Semifinale1Team(1) = CInt(value)
                                Case "Semifinal 2 Player 1" : _sett.Coppa.Semifinale2Team(0) = CInt(value)
                                Case "Semifinal 2 Player 2" : _sett.Coppa.Semifinale2Team(1) = CInt(value)
                                Case "Semifinal 1 Matchs 1" : _sett.Coppa.Semifinale1Match(0) = CInt(value)
                                Case "Semifinal 1 Matchs 2" : _sett.Coppa.Semifinale1Match(1) = CInt(value)
                                Case "Semifinal 2 Matchs 1" : _sett.Coppa.Semifinale2Match(0) = CInt(value)
                                Case "Semifinal 2 Matchs 2" : _sett.Coppa.Semifinale2Match(1) = CInt(value)
                                Case "Final Player 1" : _sett.Coppa.FinaleTeam(0) = CInt(value)
                                Case "Final Player 2" : _sett.Coppa.FinaleTeam(1) = CInt(value)
                                Case "Final Matchs 1" : _sett.Coppa.FinaleMatch(0) = CInt(value)
                                Case "Final Matchs 2" : _sett.Coppa.FinaleMatch(1) = CInt(value)

                                    'Impostazini personali'
                                Case "Rose team" : _roseteam = value
                                Case "Rose detail" : _rosedetail = CBool(value)
                                Case "Static team" : _staticteam = value
                                Case "Static detail" : _staticdetail = CBool(value)
                                Case "Compile formations detail" : _comformdetail = CBool(value)
                                Case "Classification detail" : _classificationdetail = CBool(value)
                                Case "Formations last day selected" : _lastdayforma = CInt(value)
                                Case "Last mail to list" : If value <> "" Then _lastmailto.AddRange(value.Split(CChar("|")))
                            End Select

                        Catch ex As Exception
                            Call WriteError("LegaSettings", "ReadSettings", ex.Message)
                        End Try
                    End If
                Next

                If _sett.Points.SiteReferenceForPoints.Count = 0 Then _sett.Points.SiteReferenceForPoints.Add("gazzetta")
                If _sett.Points.SiteReferenceForBonus = "" Then _sett.Points.SiteReferenceForBonus = "gazzetta"

            Catch ex As Exception
                Call WriteError("LegaSettings", "ReadSettings", ex.Message)
            End Try
        End If

        Call Inizialize()

    End Sub

    ''' <summary>Consente di salvare le impostazioni su disco</summary>
    Sub SaveSettings()
        Call SaveSystemSettings()
        Call SaveMySettings()
    End Sub

    ''' <summary>Consente di salvare le impostazioni su disco</summary>
    Sub SaveSystemSettings()
        Try
            Dim str As New System.Text.StringBuilder
            str.AppendLine("[Lega]")
            str.AppendLine("Active = '" & _sett.Active & "'")
            str.AppendLine("Year = '" & _sett.Year & "'")
            str.AppendLine("Mail administrator = '" & _sett.MailAdmin & "'")
            str.AppendLine("Number teams = '" & _sett.NumberOfTeams & "'")
            str.AppendLine("Number days = '" & _sett.NumberOfDays & "'")
            str.AppendLine("Enable trace reconfirmations = '" & _sett.EnableTraceReconfirmations & "'")
            str.AppendLine("Counts goals scored for winner of day = '" & _sett.ConteggiaGoalFattiPerVittoria & "'")
            str.AppendLine("Counts goals conceded for winner of day = '" & _sett.ConteggiaGoalSubitiPerVittoria & "'")
            str.AppendLine("Number of reserver = '" & _sett.NumberOfReserve & "'")
            str.AppendLine("Forced goalkeeper as first reserve = '" & _sett.ForcedGoalkeeperAsFirstReserve & "'")
            str.AppendLine("Number of substitution = '" & _sett.NumberOfSubstitution & "'")
            str.AppendLine("Substitution type = '" & _sett.SubstitutionType & "'")
            str.AppendLine("[Points]")
            str.AppendLine("Site reference for points = '" & SystemFunction.Convertion.ConvertListStringToString(_sett.Points.SiteReferenceForPoints, ",") & "'")
            str.AppendLine("Site reference for bonus = '" & _sett.Points.SiteReferenceForBonus & "'")
            str.AppendLine("Points Admonition = '" & _sett.Points.Admonition & "'")
            str.AppendLine("Points Expulsion = '" & _sett.Points.Expulsion & "'")
            str.AppendLine("Points Assist [P] = '" & _sett.Points.Assist("P") & "'")
            str.AppendLine("Points Assist [D] = '" & _sett.Points.Assist("D") & "'")
            str.AppendLine("Points Assist [C] = '" & _sett.Points.Assist("C") & "'")
            str.AppendLine("Points Assist [A] = '" & _sett.Points.Assist("A") & "'")
            str.AppendLine("Points GoalConceded = '" & _sett.Points.GoalConceded & "'")
            str.AppendLine("Points GoalScored [P] = '" & _sett.Points.GoalScored("P") & "'")
            str.AppendLine("Points GoalScored [D] = '" & _sett.Points.GoalScored("D") & "'")
            str.AppendLine("Points GoalScored [C] = '" & _sett.Points.GoalScored("C") & "'")
            str.AppendLine("Points GoalScored [A] = '" & _sett.Points.GoalScored("A") & "'")
            str.AppendLine("Points OwnGoal [P] = '" & _sett.Points.OwnGoal("P") & "'")
            str.AppendLine("Points OwnGoal [D] = '" & _sett.Points.OwnGoal("D") & "'")
            str.AppendLine("Points OwnGoal [C] = '" & _sett.Points.OwnGoal("C") & "'")
            str.AppendLine("Points OwnGoal [A] = '" & _sett.Points.OwnGoal("A") & "'")
            str.AppendLine("Points Missed penalty points [P] = '" & _sett.Points.MissedPenalty("P") & "'")
            str.AppendLine("Points Missed penalty points [D] = '" & _sett.Points.MissedPenalty("D") & "'")
            str.AppendLine("Points Missed penalty points [C] = '" & _sett.Points.MissedPenalty("C") & "'")
            str.AppendLine("Points Missed penalty points [A] = '" & _sett.Points.MissedPenalty("A") & "'")
            str.AppendLine("[Bonus roles]")
            str.AppendLine("Enable bonus defense = '" & _sett.Bonus.EnableBonusDefense & "'")
            str.AppendLine("Bonus defense source = '" & _sett.Bonus.BonudDefenseSource & "'")
            str.AppendLine("Bonus defense over and equal value = '" & _sett.Bonus.BonusDefenseOverEqual & "'")
            str.AppendLine("Bonus defense 3 = '" & _sett.Bonus.BonusDefense(3) & "'")
            str.AppendLine("Bonus defense 4 = '" & _sett.Bonus.BonusDefense(4) & "'")
            str.AppendLine("Bonus defense 5 = '" & _sett.Bonus.BonusDefense(5) & "'")
            str.AppendLine("Enable bonus center field = '" & _sett.Bonus.EnableCenterField & "'")
            str.AppendLine("Bonus center field source = '" & _sett.Bonus.BonudCenterFieldSource & "'")
            str.AppendLine("Bonus center field over and equal value = '" & _sett.Bonus.BonusCenterFieldOverEqual & "'")
            str.AppendLine("Bonus center field 3 = '" & _sett.Bonus.BonusCenterField(3) & "'")
            str.AppendLine("Bonus center field 4 = '" & _sett.Bonus.BonusCenterField(4) & "'")
            str.AppendLine("Bonus center field 5 = '" & _sett.Bonus.BonusCenterField(5) & "'")
            str.AppendLine("Enable bonus attack = '" & _sett.Bonus.EnableBonusAttack & "'")
            str.AppendLine("Bonus attack source = '" & _sett.Bonus.BonudAttackSource & "'")
            str.AppendLine("Bonus attack over and equal value = '" & _sett.Bonus.BonusAttackOverEqual & "'")
            str.AppendLine("Bonus attack 2 = '" & _sett.Bonus.BonusAttack(2) & "'")
            str.AppendLine("Bonus attack 3 = '" & _sett.Bonus.BonusAttack(3) & "'")
            str.AppendLine("[Jolly]")
            str.AppendLine("Enable jolly player = '" & _sett.Jolly.EnableJollyPlayer & "'")
            str.AppendLine("Enable jolly goalkeeper = '" & _sett.Jolly.EnableJollyPlayerGoalkeeper & "'")
            str.AppendLine("Enable jolly defender = '" & _sett.Jolly.EnableJollyPlayerDefender & "'")
            str.AppendLine("Enable jolly midfielder = '" & _sett.Jolly.EnableJollyPlayerMidfielder & "'")
            str.AppendLine("Enable jolly forward = '" & _sett.Jolly.EnableJollyPlayerForward & "'")
            str.AppendLine("Maximum number jolly playable = '" & _sett.Jolly.MaximumNumberJollyPlayable & "'")
            str.AppendLine("Maximum number jolly playable for day = '" & _sett.Jolly.MaximumNumberJollyPlayableForDay & "'")
            str.AppendLine("[Cup]")
            str.AppendLine("Type of second round  = '" & _sett.Coppa.TipoSecondoTurno & "'")
            For i As Integer = 0 To 2
                str.AppendLine("PlayOff 1 Player " & i + 1 & " = '" & _sett.Coppa.PlayOffGiorone1Team(i) & "'")
            Next
            For i As Integer = 0 To 2
                str.AppendLine("PlayOff 2 Player " & i + 1 & " = '" & _sett.Coppa.PlayOffGiorone2Team(i) & "'")
            Next
            For i As Integer = 0 To 2
                str.AppendLine("PlayOff 1 Matchs " & i + 1 & " = '" & _sett.Coppa.PlayOffGiorone1Match(i) & "'")
            Next
            For i As Integer = 0 To 2
                str.AppendLine("PlayOff 2 Matchs " & i + 1 & " = '" & _sett.Coppa.PlayOffGiorone2Match(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 1 Player " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale1Team(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 2 Player " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale2Team(i) & "'")
            Next

            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 3 Player " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale3Team(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 4 Player " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale4Team(i) & "'")
            Next

            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 1 Matchs " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale1Match(i) & "'")
            Next

            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 2 Matchs " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale2Match(i) & "'")
            Next

            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 3 Matchs " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale3Match(i) & "'")
            Next

            For i As Integer = 0 To 1
                str.AppendLine("Quartersfinal 4 Matchs " & i + 1 & " = '" & _sett.Coppa.QuartiDiFinale4Match(i) & "'")
            Next

            For i As Integer = 0 To 1
                str.AppendLine("Semifinal 1 Player " & i + 1 & " = '" & _sett.Coppa.Semifinale1Team(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Semifinal 2 Player " & i + 1 & " = '" & _sett.Coppa.Semifinale2Team(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Semifinal 1 Matchs " & i + 1 & " = '" & _sett.Coppa.Semifinale1Match(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Semifinal 2 Matchs " & i + 1 & " = '" & _sett.Coppa.Semifinale2Match(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Final Player " & i + 1 & " = '" & _sett.Coppa.FinaleTeam(i) & "'")
            Next
            For i As Integer = 0 To 1
                str.AppendLine("Final Matchs " & i + 1 & " = '" & _sett.Coppa.FinaleMatch(i) & "'")
            Next
            IO.File.WriteAllText(_fname, str.ToString)
        Catch ex As Exception
            Call WriteError("LegaSettings", "SaveSystemSettings", ex.Message)
        End Try
    End Sub

    Sub SaveMySettings()
        Dim str As New System.Text.StringBuilder
        Try
            str.AppendLine("[Rose]")
            str.AppendLine("Rose team = '" & _roseteam & "'")
            str.AppendLine("Rose detail = '" & _rosedetail & "'")
            str.AppendLine("[Statistic]")
            str.AppendLine("Static team = '" & _staticteam & "'")
            str.AppendLine("Static detail = '" & _staticdetail & "'")
            str.AppendLine("[Formation]")
            str.AppendLine("Formations last day selected = '" & _lastdayforma & "'")
            str.AppendLine("Compile formations detail = '" & _comformdetail & "'")
            str.AppendLine("[Classification]")
            str.AppendLine("Classification detail = '" & _classificationdetail & "'")
            str.AppendLine("Last mail to list = '" & SystemFunction.Convertion.ConvertListStringToString(_lastmailto, "|"))
            IO.File.WriteAllText(_fnamemy, str.ToString)
        Catch ex As Exception
            Call WriteError("LegaSettings", "SaveMySettings", ex.Message)
        End Try
    End Sub

End Class
