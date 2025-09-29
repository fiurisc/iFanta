Imports System.Text.RegularExpressions
Imports System.IO

Namespace Torneo
    Public Class General

        Shared _fname As String = PublicVariables.SettingsPath & "settings.txt"

        Shared Function apiGetYearAct(folderPath As String) As String

            Dim years As List(Of YearTorneo) = apiGetYearsList(folderPath)

            For Each y As YearTorneo In years
                If y.Active Then
                    Return y.Year
                End If
            Next

            Return ""

        End Function

        Shared Function apiGetYearsList(folderPath As String) As List(Of YearTorneo)

            Dim years As New List(Of YearTorneo)

            Dim d() As String = IO.Directory.GetDirectories(folderPath & "update/tornei")

            For i As Integer = 0 To d.Length - 1

                Dim torneo As String = IO.Path.GetFileName(d(i))
                Dim line() As String = IO.File.ReadAllLines(d(i) & "/settings.txt")
                Dim act As Boolean = False
                Dim year As String = ""

                For k As Integer = 0 To line.Length - 1

                    Dim para As String = Regex.Match(line(k), ".+(?=\= ')").Value.Trim
                    Dim value As String = Regex.Match(line(k), "(?<= ').+(?=')").Value

                    If para = "Year" Then
                        year = value
                    End If

                    If line(k).Contains("Active = 'True'") Then
                        act = True
                    End If
                Next

                years.Add(New YearTorneo(year, act))

            Next

            Return years

        End Function

        Sub ReadSettings()

            PublicVariables.Settings.Bonus.BonusDefense.Clear()
            PublicVariables.Settings.Bonus.BonusCenterField.Clear()
            PublicVariables.Settings.Bonus.BonusAttack.Clear()

            PublicVariables.Settings.Bonus.BonusDefense.Add(3, 10)
            PublicVariables.Settings.Bonus.BonusDefense.Add(4, 20)
            PublicVariables.Settings.Bonus.BonusDefense.Add(5, 30)
            PublicVariables.Settings.Bonus.BonusCenterField.Add(3, 10)
            PublicVariables.Settings.Bonus.BonusCenterField.Add(4, 20)
            PublicVariables.Settings.Bonus.BonusCenterField.Add(5, 30)
            PublicVariables.Settings.Bonus.BonusAttack.Add(2, 10)
            PublicVariables.Settings.Bonus.BonusAttack.Add(3, 20)

            If File.Exists(_fname) Then

                Try

                    Dim lines As New List(Of String)
                    If File.Exists(_fname) Then lines.AddRange(IO.File.ReadAllLines(_fname))

                    For i As Integer = 0 To lines.Count - 1

                        Dim line As String = lines(i)

                        Dim para As String = Regex.Match(line, ".+(?=\= ')").Value.Trim
                        Dim value As String = Regex.Match(line, "(?<= ').+(?=')").Value
                        If para <> "" AndAlso value <> "" Then
                            Try
                                Select Case para
                                    Case "Active" : PublicVariables.Settings.Active = CBool(value)
                                    Case "Year" : PublicVariables.Settings.Year = value
                                    Case "Mail administrator" : PublicVariables.Settings.MailAdmin = value
                                    Case "Number teams" : PublicVariables.Settings.NumberOfTeams = CInt(value)
                                    Case "Number days" : PublicVariables.Settings.NumberOfDays = CInt(value)
                                    Case "Enable trace reconfirmations" : PublicVariables.Settings.EnableTraceReconfirmations = CBool(value)
                                    Case "Site reference for points"
                                        PublicVariables.Settings.Points.SiteReferenceForPoints.Clear()
                                        PublicVariables.Settings.Points.SiteReferenceForPoints.AddRange(value.Split(CChar(",")))
                                    Case "Site reference for bonus" : PublicVariables.Settings.Points.SiteReferenceForBonus = value
                                    Case "Points Admonition" : PublicVariables.Settings.Points.Admonition = CInt(value)
                                    Case "Points Expulsion" : PublicVariables.Settings.Points.Expulsion = CInt(value)
                                    Case "Points Assist [P]" : PublicVariables.Settings.Points.Assist("P") = CInt(value)
                                    Case "Points Assist [D]" : PublicVariables.Settings.Points.Assist("D") = CInt(value)
                                    Case "Points Assist [C]" : PublicVariables.Settings.Points.Assist("C") = CInt(value)
                                    Case "Points Assist [A]" : PublicVariables.Settings.Points.Assist("A") = CInt(value)
                                    Case "Points GoalConceded" : PublicVariables.Settings.Points.GoalConceded = CInt(value)
                                    Case "Points GoalScored [P]" : PublicVariables.Settings.Points.GoalScored("P") = CInt(value)
                                    Case "Points GoalScored [D]" : PublicVariables.Settings.Points.GoalScored("D") = CInt(value)
                                    Case "Points GoalScored [C]" : PublicVariables.Settings.Points.GoalScored("C") = CInt(value)
                                    Case "Points GoalScored [A]" : PublicVariables.Settings.Points.GoalScored("A") = CInt(value)
                                    Case "Points OwnGoal [P]" : PublicVariables.Settings.Points.OwnGoal("P") = CInt(value)
                                    Case "Points OwnGoal [D]" : PublicVariables.Settings.Points.OwnGoal("D") = CInt(value)
                                    Case "Points OwnGoal [C]" : PublicVariables.Settings.Points.OwnGoal("C") = CInt(value)
                                    Case "Points OwnGoal [A]" : PublicVariables.Settings.Points.OwnGoal("A") = CInt(value)
                                    Case "Points Missed penalty points [P]" : PublicVariables.Settings.Points.MissedPenalty("P") = CInt(value)
                                    Case "Points Missed penalty points [D]" : PublicVariables.Settings.Points.MissedPenalty("D") = CInt(value)
                                    Case "Points Missed penalty points [C]" : PublicVariables.Settings.Points.MissedPenalty("C") = CInt(value)
                                    Case "Points Missed penalty points [A]" : PublicVariables.Settings.Points.MissedPenalty("A") = CInt(value)
                                    Case "Counts goals scored for winner of day" : PublicVariables.Settings.ConteggiaGoalFattiPerVittoria = CBool(value)
                                    Case "Counts goals conceded for winner of day" : PublicVariables.Settings.ConteggiaGoalSubitiPerVittoria = CBool(value)
                                    Case "Number of reserver" : PublicVariables.Settings.NumberOfReserve = CInt(value)
                                    Case "Forced goalkeeper as first reserve" : PublicVariables.Settings.ForcedGoalkeeperAsFirstReserve = CBool(value)
                                    Case "Enable bonus defense" : PublicVariables.Settings.Bonus.EnableBonusDefense = CBool(value)
                                    Case "Number of substitution" : PublicVariables.Settings.NumberOfSubstitution = CInt(value)
                                    Case "Substitution type" : PublicVariables.Settings.SubstitutionType = CType(value, LegaSettings.eSubstitutionType)
                                    Case "Bonus defense source" : If value <> "" Then PublicVariables.Settings.Bonus.BonudDefenseSource = value
                                    Case "Bonus defense over and equal value" : PublicVariables.Settings.Bonus.BonusDefenseOverEqual = CInt(value)
                                    Case "Bonus defense 3" : PublicVariables.Settings.Bonus.BonusDefense(3) = CInt(value)
                                    Case "Bonus defense 4" : PublicVariables.Settings.Bonus.BonusDefense(4) = CInt(value)
                                    Case "Bonus defense 5" : PublicVariables.Settings.Bonus.BonusDefense(5) = CInt(value)
                                    Case "Enable bonus center field" : PublicVariables.Settings.Bonus.EnableCenterField = CBool(value)
                                    Case "Bonus center field source" : If value <> "" Then PublicVariables.Settings.Bonus.BonudCenterFieldSource = value
                                    Case "Bonus center field over and equal value" : PublicVariables.Settings.Bonus.BonusCenterFieldOverEqual = CInt(value)
                                    Case "Bonus center field 3" : PublicVariables.Settings.Bonus.BonusCenterField(3) = CInt(value)
                                    Case "Bonus center field 4" : PublicVariables.Settings.Bonus.BonusCenterField(4) = CInt(value)
                                    Case "Bonus center field 5" : PublicVariables.Settings.Bonus.BonusCenterField(5) = CInt(value)
                                    Case "Enable bonus attack" : PublicVariables.Settings.Bonus.EnableBonusAttack = CBool(value)
                                    Case "Bonus attack source" : If value <> "" Then PublicVariables.Settings.Bonus.BonudAttackSource = value
                                    Case "Bonus attack over and equal value" : PublicVariables.Settings.Bonus.BonusAttackOverEqual = CInt(value)
                                    Case "Bonus attack 2" : PublicVariables.Settings.Bonus.BonusAttack(2) = CInt(value)
                                    Case "Bonus attack 3" : PublicVariables.Settings.Bonus.BonusAttack(3) = CInt(value)
                                    Case "Enable jolly player" : PublicVariables.Settings.Jolly.EnableJollyPlayer = CBool(value)
                                    Case "Enable jolly goalkeeper" : PublicVariables.Settings.Jolly.EnableJollyPlayerGoalkeeper = CBool(value)
                                    Case "Enable jolly defender" : PublicVariables.Settings.Jolly.EnableJollyPlayerDefender = CBool(value)
                                    Case "Enable jolly midfielder" : PublicVariables.Settings.Jolly.EnableJollyPlayerMidfielder = CBool(value)
                                    Case "Enable jolly forward" : PublicVariables.Settings.Jolly.EnableJollyPlayerForward = CBool(value)
                                    Case "Maximum number jolly playable" : PublicVariables.Settings.Jolly.MaximumNumberJollyPlayable = CInt(value)
                                    Case "Maximum number jolly playable for day" : PublicVariables.Settings.Jolly.MaximumNumberJollyPlayableForDay = CInt(value)
                                    Case "Type of second round" : PublicVariables.Settings.Coppa.TipoSecondoTurno = value
                                    Case "PlayOff 1 Player 1" : PublicVariables.Settings.Coppa.PlayOffGiorone1Team(0) = CInt(value)
                                    Case "PlayOff 1 Player 2" : PublicVariables.Settings.Coppa.PlayOffGiorone1Team(1) = CInt(value)
                                    Case "PlayOff 1 Player 3" : PublicVariables.Settings.Coppa.PlayOffGiorone1Team(2) = CInt(value)
                                    Case "PlayOff 2 Player 1" : PublicVariables.Settings.Coppa.PlayOffGiorone2Team(0) = CInt(value)
                                    Case "PlayOff 2 Player 2" : PublicVariables.Settings.Coppa.PlayOffGiorone2Team(1) = CInt(value)
                                    Case "PlayOff 2 Player 3" : PublicVariables.Settings.Coppa.PlayOffGiorone2Team(2) = CInt(value)
                                    Case "PlayOff 1 Matchs 1" : PublicVariables.Settings.Coppa.PlayOffGiorone1Match(0) = CInt(value)
                                    Case "PlayOff 1 Matchs 2" : PublicVariables.Settings.Coppa.PlayOffGiorone1Match(1) = CInt(value)
                                    Case "PlayOff 1 Matchs 3" : PublicVariables.Settings.Coppa.PlayOffGiorone1Match(2) = CInt(value)
                                    Case "PlayOff 2 Matchs 1" : PublicVariables.Settings.Coppa.PlayOffGiorone2Match(0) = CInt(value)
                                    Case "PlayOff 2 Matchs 2" : PublicVariables.Settings.Coppa.PlayOffGiorone2Match(1) = CInt(value)
                                    Case "PlayOff 2 Matchs 3" : PublicVariables.Settings.Coppa.PlayOffGiorone2Match(2) = CInt(value)
                                    Case "Quartersfinal 1 Player 1" : PublicVariables.Settings.Coppa.QuartiDiFinale1Team(0) = CInt(value)
                                    Case "Quartersfinal 1 Player 2" : PublicVariables.Settings.Coppa.QuartiDiFinale1Team(1) = CInt(value)
                                    Case "Quartersfinal 2 Player 1" : PublicVariables.Settings.Coppa.QuartiDiFinale2Team(0) = CInt(value)
                                    Case "Quartersfinal 2 Player 2" : PublicVariables.Settings.Coppa.QuartiDiFinale2Team(1) = CInt(value)
                                    Case "Quartersfinal 3 Player 1" : PublicVariables.Settings.Coppa.QuartiDiFinale3Team(0) = CInt(value)
                                    Case "Quartersfinal 3 Player 2" : PublicVariables.Settings.Coppa.QuartiDiFinale3Team(1) = CInt(value)
                                    Case "Quartersfinal 4 Player 1" : PublicVariables.Settings.Coppa.QuartiDiFinale4Team(0) = CInt(value)
                                    Case "Quartersfinal 4 Player 2" : PublicVariables.Settings.Coppa.QuartiDiFinale4Team(1) = CInt(value)
                                    Case "Quartersfinal 1 Matchs 1" : PublicVariables.Settings.Coppa.QuartiDiFinale1Match(0) = CInt(value)
                                    Case "Quartersfinal 1 Matchs 2" : PublicVariables.Settings.Coppa.QuartiDiFinale1Match(1) = CInt(value)
                                    Case "Quartersfinal 2 Matchs 1" : PublicVariables.Settings.Coppa.QuartiDiFinale2Match(0) = CInt(value)
                                    Case "Quartersfinal 2 Matchs 2" : PublicVariables.Settings.Coppa.QuartiDiFinale2Match(1) = CInt(value)
                                    Case "Quartersfinal 3 Matchs 1" : PublicVariables.Settings.Coppa.QuartiDiFinale3Match(0) = CInt(value)
                                    Case "Quartersfinal 3 Matchs 2" : PublicVariables.Settings.Coppa.QuartiDiFinale3Match(1) = CInt(value)
                                    Case "Quartersfinal 4 Matchs 1" : PublicVariables.Settings.Coppa.QuartiDiFinale4Match(0) = CInt(value)
                                    Case "Quartersfinal 4 Matchs 2" : PublicVariables.Settings.Coppa.QuartiDiFinale4Match(1) = CInt(value)
                                    Case "Semifinal 1 Player 1" : PublicVariables.Settings.Coppa.Semifinale1Team(0) = CInt(value)
                                    Case "Semifinal 1 Player 2" : PublicVariables.Settings.Coppa.Semifinale1Team(1) = CInt(value)
                                    Case "Semifinal 2 Player 1" : PublicVariables.Settings.Coppa.Semifinale2Team(0) = CInt(value)
                                    Case "Semifinal 2 Player 2" : PublicVariables.Settings.Coppa.Semifinale2Team(1) = CInt(value)
                                    Case "Semifinal 1 Matchs 1" : PublicVariables.Settings.Coppa.Semifinale1Match(0) = CInt(value)
                                    Case "Semifinal 1 Matchs 2" : PublicVariables.Settings.Coppa.Semifinale1Match(1) = CInt(value)
                                    Case "Semifinal 2 Matchs 1" : PublicVariables.Settings.Coppa.Semifinale2Match(0) = CInt(value)
                                    Case "Semifinal 2 Matchs 2" : PublicVariables.Settings.Coppa.Semifinale2Match(1) = CInt(value)
                                    Case "Final Player 1" : PublicVariables.Settings.Coppa.FinaleTeam(0) = CInt(value)
                                    Case "Final Player 2" : PublicVariables.Settings.Coppa.FinaleTeam(1) = CInt(value)
                                    Case "Final Matchs 1" : PublicVariables.Settings.Coppa.FinaleMatch(0) = CInt(value)
                                    Case "Final Matchs 2" : PublicVariables.Settings.Coppa.FinaleMatch(1) = CInt(value)
                                End Select

                            Catch ex As Exception
                                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                            End Try
                        End If
                    Next

                    If PublicVariables.Settings.Points.SiteReferenceForPoints.Count = 0 Then PublicVariables.Settings.Points.SiteReferenceForPoints.Add("gazzetta")
                    If PublicVariables.Settings.Points.SiteReferenceForBonus = "" Then PublicVariables.Settings.Points.SiteReferenceForBonus = "gazzetta"

                Catch ex As Exception
                    WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                End Try
            End If

        End Sub

        ''' <summary>Consente di salvare le impostazioni su disco</summary>
        Sub SaveSettings()
            Try
                Dim str As New System.Text.StringBuilder
                str.AppendLine("[Lega]")
                str.AppendLine("Active = '" & PublicVariables.Settings.Active & "'")
                str.AppendLine("Year = '" & PublicVariables.Settings.Year & "'")
                str.AppendLine("Mail administrator = '" & PublicVariables.Settings.MailAdmin & "'")
                str.AppendLine("Number teams = '" & PublicVariables.Settings.NumberOfTeams & "'")
                str.AppendLine("Number days = '" & PublicVariables.Settings.NumberOfDays & "'")
                str.AppendLine("Enable trace reconfirmations = '" & PublicVariables.Settings.EnableTraceReconfirmations & "'")
                str.AppendLine("Counts goals scored for winner of day = '" & PublicVariables.Settings.ConteggiaGoalFattiPerVittoria & "'")
                str.AppendLine("Counts goals conceded for winner of day = '" & PublicVariables.Settings.ConteggiaGoalSubitiPerVittoria & "'")
                str.AppendLine("Number of reserver = '" & PublicVariables.Settings.NumberOfReserve & "'")
                str.AppendLine("Forced goalkeeper as first reserve = '" & PublicVariables.Settings.ForcedGoalkeeperAsFirstReserve & "'")
                str.AppendLine("Number of substitution = '" & PublicVariables.Settings.NumberOfSubstitution & "'")
                str.AppendLine("Substitution type = '" & PublicVariables.Settings.SubstitutionType & "'")
                str.AppendLine("[Points]")
                str.AppendLine("Site reference for points = '" & Functions.ConvertListStringToString(PublicVariables.Settings.Points.SiteReferenceForPoints, ",") & "'")
                str.AppendLine("Site reference for bonus = '" & PublicVariables.Settings.Points.SiteReferenceForBonus & "'")
                str.AppendLine("Points Admonition = '" & PublicVariables.Settings.Points.Admonition & "'")
                str.AppendLine("Points Expulsion = '" & PublicVariables.Settings.Points.Expulsion & "'")
                str.AppendLine("Points Assist [P] = '" & PublicVariables.Settings.Points.Assist("P") & "'")
                str.AppendLine("Points Assist [D] = '" & PublicVariables.Settings.Points.Assist("D") & "'")
                str.AppendLine("Points Assist [C] = '" & PublicVariables.Settings.Points.Assist("C") & "'")
                str.AppendLine("Points Assist [A] = '" & PublicVariables.Settings.Points.Assist("A") & "'")
                str.AppendLine("Points GoalConceded = '" & PublicVariables.Settings.Points.GoalConceded & "'")
                str.AppendLine("Points GoalScored [P] = '" & PublicVariables.Settings.Points.GoalScored("P") & "'")
                str.AppendLine("Points GoalScored [D] = '" & PublicVariables.Settings.Points.GoalScored("D") & "'")
                str.AppendLine("Points GoalScored [C] = '" & PublicVariables.Settings.Points.GoalScored("C") & "'")
                str.AppendLine("Points GoalScored [A] = '" & PublicVariables.Settings.Points.GoalScored("A") & "'")
                str.AppendLine("Points OwnGoal [P] = '" & PublicVariables.Settings.Points.OwnGoal("P") & "'")
                str.AppendLine("Points OwnGoal [D] = '" & PublicVariables.Settings.Points.OwnGoal("D") & "'")
                str.AppendLine("Points OwnGoal [C] = '" & PublicVariables.Settings.Points.OwnGoal("C") & "'")
                str.AppendLine("Points OwnGoal [A] = '" & PublicVariables.Settings.Points.OwnGoal("A") & "'")
                str.AppendLine("Points Missed penalty points [P] = '" & PublicVariables.Settings.Points.MissedPenalty("P") & "'")
                str.AppendLine("Points Missed penalty points [D] = '" & PublicVariables.Settings.Points.MissedPenalty("D") & "'")
                str.AppendLine("Points Missed penalty points [C] = '" & PublicVariables.Settings.Points.MissedPenalty("C") & "'")
                str.AppendLine("Points Missed penalty points [A] = '" & PublicVariables.Settings.Points.MissedPenalty("A") & "'")
                str.AppendLine("[Bonus roles]")
                str.AppendLine("Enable bonus defense = '" & PublicVariables.Settings.Bonus.EnableBonusDefense & "'")
                str.AppendLine("Bonus defense source = '" & PublicVariables.Settings.Bonus.BonudDefenseSource & "'")
                str.AppendLine("Bonus defense over and equal value = '" & PublicVariables.Settings.Bonus.BonusDefenseOverEqual & "'")
                str.AppendLine("Bonus defense 3 = '" & PublicVariables.Settings.Bonus.BonusDefense(3) & "'")
                str.AppendLine("Bonus defense 4 = '" & PublicVariables.Settings.Bonus.BonusDefense(4) & "'")
                str.AppendLine("Bonus defense 5 = '" & PublicVariables.Settings.Bonus.BonusDefense(5) & "'")
                str.AppendLine("Enable bonus center field = '" & PublicVariables.Settings.Bonus.EnableCenterField & "'")
                str.AppendLine("Bonus center field source = '" & PublicVariables.Settings.Bonus.BonudCenterFieldSource & "'")
                str.AppendLine("Bonus center field over and equal value = '" & PublicVariables.Settings.Bonus.BonusCenterFieldOverEqual & "'")
                str.AppendLine("Bonus center field 3 = '" & PublicVariables.Settings.Bonus.BonusCenterField(3) & "'")
                str.AppendLine("Bonus center field 4 = '" & PublicVariables.Settings.Bonus.BonusCenterField(4) & "'")
                str.AppendLine("Bonus center field 5 = '" & PublicVariables.Settings.Bonus.BonusCenterField(5) & "'")
                str.AppendLine("Enable bonus attack = '" & PublicVariables.Settings.Bonus.EnableBonusAttack & "'")
                str.AppendLine("Bonus attack source = '" & PublicVariables.Settings.Bonus.BonudAttackSource & "'")
                str.AppendLine("Bonus attack over and equal value = '" & PublicVariables.Settings.Bonus.BonusAttackOverEqual & "'")
                str.AppendLine("Bonus attack 2 = '" & PublicVariables.Settings.Bonus.BonusAttack(2) & "'")
                str.AppendLine("Bonus attack 3 = '" & PublicVariables.Settings.Bonus.BonusAttack(3) & "'")
                str.AppendLine("[Jolly]")
                str.AppendLine("Enable jolly player = '" & PublicVariables.Settings.Jolly.EnableJollyPlayer & "'")
                str.AppendLine("Enable jolly goalkeeper = '" & PublicVariables.Settings.Jolly.EnableJollyPlayerGoalkeeper & "'")
                str.AppendLine("Enable jolly defender = '" & PublicVariables.Settings.Jolly.EnableJollyPlayerDefender & "'")
                str.AppendLine("Enable jolly midfielder = '" & PublicVariables.Settings.Jolly.EnableJollyPlayerMidfielder & "'")
                str.AppendLine("Enable jolly forward = '" & PublicVariables.Settings.Jolly.EnableJollyPlayerForward & "'")
                str.AppendLine("Maximum number jolly playable = '" & PublicVariables.Settings.Jolly.MaximumNumberJollyPlayable & "'")
                str.AppendLine("Maximum number jolly playable for day = '" & PublicVariables.Settings.Jolly.MaximumNumberJollyPlayableForDay & "'")
                str.AppendLine("[Cup]")
                str.AppendLine("Type of second round  = '" & PublicVariables.Settings.Coppa.TipoSecondoTurno & "'")
                For i As Integer = 0 To 2
                    str.AppendLine("PlayOff 1 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.PlayOffGiorone1Team(i) & "'")
                Next
                For i As Integer = 0 To 2
                    str.AppendLine("PlayOff 2 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.PlayOffGiorone2Team(i) & "'")
                Next
                For i As Integer = 0 To 2
                    str.AppendLine("PlayOff 1 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.PlayOffGiorone1Match(i) & "'")
                Next
                For i As Integer = 0 To 2
                    str.AppendLine("PlayOff 2 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.PlayOffGiorone2Match(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 1 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale1Team(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 2 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale2Team(i) & "'")
                Next

                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 3 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale3Team(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 4 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale4Team(i) & "'")
                Next

                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 1 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale1Match(i) & "'")
                Next

                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 2 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale2Match(i) & "'")
                Next

                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 3 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale3Match(i) & "'")
                Next

                For i As Integer = 0 To 1
                    str.AppendLine("Quartersfinal 4 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.QuartiDiFinale4Match(i) & "'")
                Next

                For i As Integer = 0 To 1
                    str.AppendLine("Semifinal 1 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.Semifinale1Team(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Semifinal 2 Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.Semifinale2Team(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Semifinal 1 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.Semifinale1Match(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Semifinal 2 Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.Semifinale2Match(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Final Player " & i + 1 & " = '" & PublicVariables.Settings.Coppa.FinaleTeam(i) & "'")
                Next
                For i As Integer = 0 To 1
                    str.AppendLine("Final Matchs " & i + 1 & " = '" & PublicVariables.Settings.Coppa.FinaleMatch(i) & "'")
                Next
                IO.File.WriteAllText(_fname, str.ToString)
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Public Shared Function GetFormazioniTorneo(gio As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fname As String = PublicVariables.DataPath & "\2025\torneo\formazioni.txt"
                Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                For Each line As String In lines
                    If gio = "-1" OrElse line.Split(Convert.ToChar("|"))(0) = gio Then
                        strdata.AppendLine(line)
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Class YearTorneo
            Public Property Year As String = ""
            Public Property Active As Boolean = False

            Sub New()

            End Sub

            Sub New(Year As String, Active As Boolean)
                Me.Year = Year
                Me.Active = Active
            End Sub
        End Class

        Public Class LoginUser
            Public Property Username As String = ""
            Public Property Password As String = ""
            Public Property Hash As Boolean = False
        End Class
    End Class
End Namespace

