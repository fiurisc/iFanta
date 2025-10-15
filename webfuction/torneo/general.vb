Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Reflection

Namespace Torneo
    Public Class General

        Shared Function GetApplication(role As String) As List(Of WebApplication)

            Dim apps As New List(Of WebApplication)

            apps.Add(New WebApplication("edit-formazioni.png", "Compila formazione", "Compila la formazione da inviare", "compila.html", ""))

            Return apps

        End Function

        Shared Function GetAccountByUsername(Username As String) As Account

            Dim acc As New Account

            Try

                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Info, "Get account per: " & Username)

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM users where nome='" & Username & "';", True)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As System.Data.DataRow = ds.Tables(0).Rows(i)
                        acc.Id = Functions.ReadFieldIntegerData("id", row, 0)
                        acc.Username = Functions.ReadFieldStringData("nome", row, "")
                        acc.Password = Functions.ReadFieldStringData("password", row, "")
                        acc.Role = Functions.ReadFieldStringData("role", row, "")
                        acc.Mail = Functions.ReadFieldStringData("mail", row, "")
                        acc.TeamId = Functions.ReadFieldStringData("teamid", row, "0")
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return acc

        End Function

        Shared Sub SendPassword(Username As String)

            WebData.Functions.WriteLog(WebData.Functions.eMessageType.Info, "Send password per: " & Username)

            Dim acc As Account = GetAccountByUsername(Username)
            If acc.Username <> "" AndAlso acc.Mail <> "" Then
                SendMail(acc.Mail, "", "iFantacalcio", "Password account", "la password per l'account (" & acc.Username & ") è: <b>" & acc.Password & "</b>", New List(Of String))
            Else
                Throw New Exception("errore interno")
            End If
        End Sub

        Shared Function GetSettingsFileName(Year As String) As String
            Return PublicVariables.RootDataPath & Year & "/settings.txt"
        End Function

        Shared Function ApiGetYearAct() As String

            Dim years As List(Of YearTorneo) = ApiGetYearsList()

            For Each y As YearTorneo In years
                If y.Active Then
                    Return y.Year
                End If
            Next

            Return ""

        End Function

        Shared Function ApiGetYearsList() As List(Of YearTorneo)

            Dim years As New List(Of YearTorneo)

            Dim d() As String = IO.Directory.GetDirectories(PublicVariables.RootDataPath)

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

        Shared Function ApiGetSettings(Year As String) As String
            Dim sett As TorneoSettings = GetSettings(Year)
            Return WebData.Functions.SerializzaOggetto(sett, True)
        End Function

        Shared Sub ReadSettings()
            If PublicVariables.SettingsLoaded Then Exit Sub
            PublicVariables.Settings = GetSettings(PublicVariables.Year)
        End Sub

        Private Shared Function GetSettings(Year As String) As TorneoSettings

            WebData.Functions.WriteLog(WebData.Functions.eMessageType.Info, "Lettura delle impostazioni per il torneo: " & Year)

            Dim fname As String = GetSettingsFileName(Year)
            Dim sett As New TorneoSettings

            sett.Bonus.BonusDefense.Clear()
            sett.Bonus.BonusCenterField.Clear()
            sett.Bonus.BonusAttack.Clear()
            sett.Bonus.BonusDefense.Add("3", 10)
            sett.Bonus.BonusDefense.Add("4", 20)
            sett.Bonus.BonusDefense.Add("5", 30)
            sett.Bonus.BonusCenterField.Add("3", 10)
            sett.Bonus.BonusCenterField.Add("4", 20)
            sett.Bonus.BonusCenterField.Add("5", 30)
            sett.Bonus.BonusAttack.Add("2", 10)
            sett.Bonus.BonusAttack.Add("3", 20)

            If File.Exists(fname) Then

                Try

                    Dim lines As New List(Of String)
                    If File.Exists(fname) Then lines.AddRange(IO.File.ReadAllLines(fname))

                    For i As Integer = 0 To lines.Count - 1

                        Dim line As String = lines(i)

                        Dim para As String = Regex.Match(line, ".+(?=\= ')").Value.Trim
                        Dim value As String = Regex.Match(line, "(?<= ').+(?=')").Value
                        If para <> "" AndAlso value <> "" Then
                            Try
                                Select Case para
                                    Case "Active" : sett.Active = CBool(value)
                                    Case "Year" : sett.Year = value
                                    Case "Number teams" : sett.NumberOfTeams = CInt(value)
                                    Case "Number days" : sett.NumberOfDays = CInt(value)
                                    Case "Enable trace reconfirmations" : sett.EnableTraceReconfirmations = CBool(value)
                                    Case "Site reference for points"
                                        sett.Points.SiteReferenceForPoints.Clear()
                                        sett.Points.SiteReferenceForPoints.AddRange(value.Split(CChar(",")))
                                    Case "Site reference for bonus" : sett.Points.SiteReferenceForBonus = value
                                    Case "Points Admonition" : sett.Points.Admonition = CInt(value)
                                    Case "Points Expulsion" : sett.Points.Expulsion = CInt(value)
                                    Case "Points Assist [P]" : sett.Points.Assist("P") = CInt(value)
                                    Case "Points Assist [D]" : sett.Points.Assist("D") = CInt(value)
                                    Case "Points Assist [C]" : sett.Points.Assist("C") = CInt(value)
                                    Case "Points Assist [A]" : sett.Points.Assist("A") = CInt(value)
                                    Case "Points GoalConceded" : sett.Points.GoalConceded = CInt(value)
                                    Case "Points GoalScored [P]" : sett.Points.GoalScored("P") = CInt(value)
                                    Case "Points GoalScored [D]" : sett.Points.GoalScored("D") = CInt(value)
                                    Case "Points GoalScored [C]" : sett.Points.GoalScored("C") = CInt(value)
                                    Case "Points GoalScored [A]" : sett.Points.GoalScored("A") = CInt(value)
                                    Case "Points OwnGoal [P]" : sett.Points.OwnGoal("P") = CInt(value)
                                    Case "Points OwnGoal [D]" : sett.Points.OwnGoal("D") = CInt(value)
                                    Case "Points OwnGoal [C]" : sett.Points.OwnGoal("C") = CInt(value)
                                    Case "Points OwnGoal [A]" : sett.Points.OwnGoal("A") = CInt(value)
                                    Case "Points Missed penalty points [P]" : sett.Points.MissedPenalty("P") = CInt(value)
                                    Case "Points Missed penalty points [D]" : sett.Points.MissedPenalty("D") = CInt(value)
                                    Case "Points Missed penalty points [C]" : sett.Points.MissedPenalty("C") = CInt(value)
                                    Case "Points Missed penalty points [A]" : sett.Points.MissedPenalty("A") = CInt(value)
                                    Case "Counts goals scored for winner of day" : sett.ConteggiaGoalFattiPerVittoria = CBool(value)
                                    Case "Counts goals conceded for winner of day" : sett.ConteggiaGoalSubitiPerVittoria = CBool(value)
                                    Case "Number of reserver" : sett.NumberOfReserve = CInt(value)
                                    Case "Forced goalkeeper as first reserve" : sett.ForcedGoalkeeperAsFirstReserve = CBool(value)
                                    Case "Enable bonus defense" : sett.Bonus.EnableBonusDefense = CBool(value)
                                    Case "Number of substitution" : sett.NumberOfSubstitution = CInt(value)
                                    Case "Substitution type" : sett.SubstitutionType = CType(value, TorneoSettings.eSubstitutionType)
                                    Case "Bonus defense source" : If value <> "" Then sett.Bonus.BonudDefenseSource = value
                                    Case "Bonus defense over and equal value" : sett.Bonus.BonusDefenseOverEqual = CInt(value)
                                    Case "Bonus defense 3" : sett.Bonus.BonusDefense("3") = CInt(value)
                                    Case "Bonus defense 4" : sett.Bonus.BonusDefense("4") = CInt(value)
                                    Case "Bonus defense 5" : sett.Bonus.BonusDefense("5") = CInt(value)
                                    Case "Enable bonus center field" : sett.Bonus.EnableCenterField = CBool(value)
                                    Case "Bonus center field source" : If value <> "" Then sett.Bonus.BonudCenterFieldSource = value
                                    Case "Bonus center field over and equal value" : sett.Bonus.BonusCenterFieldOverEqual = CInt(value)
                                    Case "Bonus center field 3" : sett.Bonus.BonusCenterField("3") = CInt(value)
                                    Case "Bonus center field 4" : sett.Bonus.BonusCenterField("4") = CInt(value)
                                    Case "Bonus center field 5" : sett.Bonus.BonusCenterField("5") = CInt(value)
                                    Case "Enable bonus attack" : sett.Bonus.EnableBonusAttack = CBool(value)
                                    Case "Bonus attack source" : If value <> "" Then sett.Bonus.BonudAttackSource = value
                                    Case "Bonus attack over and equal value" : sett.Bonus.BonusAttackOverEqual = CInt(value)
                                    Case "Bonus attack 2" : sett.Bonus.BonusAttack("2") = CInt(value)
                                    Case "Bonus attack 3" : sett.Bonus.BonusAttack("3") = CInt(value)
                                    Case "Enable jolly player" : sett.Jolly.EnableJollyPlayer = CBool(value)
                                    Case "Enable jolly goalkeeper" : sett.Jolly.EnableJollyPlayerGoalkeeper = CBool(value)
                                    Case "Enable jolly defender" : sett.Jolly.EnableJollyPlayerDefender = CBool(value)
                                    Case "Enable jolly midfielder" : sett.Jolly.EnableJollyPlayerMidfielder = CBool(value)
                                    Case "Enable jolly forward" : sett.Jolly.EnableJollyPlayerForward = CBool(value)
                                    Case "Maximum number jolly playable" : sett.Jolly.MaximumNumberJollyPlayable = CInt(value)
                                    Case "Maximum number jolly playable for day" : sett.Jolly.MaximumNumberJollyPlayableForDay = CInt(value)
                                    Case "Type of second round" : sett.Coppa.TipoSecondoTurno = value
                                    Case "PlayOff 1 Player 1" : sett.Coppa.PlayOffGiorone1Team(0) = CInt(value)
                                    Case "PlayOff 1 Player 2" : sett.Coppa.PlayOffGiorone1Team(1) = CInt(value)
                                    Case "PlayOff 1 Player 3" : sett.Coppa.PlayOffGiorone1Team(2) = CInt(value)
                                    Case "PlayOff 2 Player 1" : sett.Coppa.PlayOffGiorone2Team(0) = CInt(value)
                                    Case "PlayOff 2 Player 2" : sett.Coppa.PlayOffGiorone2Team(1) = CInt(value)
                                    Case "PlayOff 2 Player 3" : sett.Coppa.PlayOffGiorone2Team(2) = CInt(value)
                                    Case "PlayOff 1 Matchs 1" : sett.Coppa.PlayOffGiorone1Match(0) = CInt(value)
                                    Case "PlayOff 1 Matchs 2" : sett.Coppa.PlayOffGiorone1Match(1) = CInt(value)
                                    Case "PlayOff 1 Matchs 3" : sett.Coppa.PlayOffGiorone1Match(2) = CInt(value)
                                    Case "PlayOff 2 Matchs 1" : sett.Coppa.PlayOffGiorone2Match(0) = CInt(value)
                                    Case "PlayOff 2 Matchs 2" : sett.Coppa.PlayOffGiorone2Match(1) = CInt(value)
                                    Case "PlayOff 2 Matchs 3" : sett.Coppa.PlayOffGiorone2Match(2) = CInt(value)
                                    Case "Quartersfinal 1 Player 1" : sett.Coppa.QuartiDiFinale1Team(0) = CInt(value)
                                    Case "Quartersfinal 1 Player 2" : sett.Coppa.QuartiDiFinale1Team(1) = CInt(value)
                                    Case "Quartersfinal 2 Player 1" : sett.Coppa.QuartiDiFinale2Team(0) = CInt(value)
                                    Case "Quartersfinal 2 Player 2" : sett.Coppa.QuartiDiFinale2Team(1) = CInt(value)
                                    Case "Quartersfinal 3 Player 1" : sett.Coppa.QuartiDiFinale3Team(0) = CInt(value)
                                    Case "Quartersfinal 3 Player 2" : sett.Coppa.QuartiDiFinale3Team(1) = CInt(value)
                                    Case "Quartersfinal 4 Player 1" : sett.Coppa.QuartiDiFinale4Team(0) = CInt(value)
                                    Case "Quartersfinal 4 Player 2" : sett.Coppa.QuartiDiFinale4Team(1) = CInt(value)
                                    Case "Quartersfinal 1 Matchs 1" : sett.Coppa.QuartiDiFinale1Match(0) = CInt(value)
                                    Case "Quartersfinal 1 Matchs 2" : sett.Coppa.QuartiDiFinale1Match(1) = CInt(value)
                                    Case "Quartersfinal 2 Matchs 1" : sett.Coppa.QuartiDiFinale2Match(0) = CInt(value)
                                    Case "Quartersfinal 2 Matchs 2" : sett.Coppa.QuartiDiFinale2Match(1) = CInt(value)
                                    Case "Quartersfinal 3 Matchs 1" : sett.Coppa.QuartiDiFinale3Match(0) = CInt(value)
                                    Case "Quartersfinal 3 Matchs 2" : sett.Coppa.QuartiDiFinale3Match(1) = CInt(value)
                                    Case "Quartersfinal 4 Matchs 1" : sett.Coppa.QuartiDiFinale4Match(0) = CInt(value)
                                    Case "Quartersfinal 4 Matchs 2" : sett.Coppa.QuartiDiFinale4Match(1) = CInt(value)
                                    Case "Semifinal 1 Player 1" : sett.Coppa.Semifinale1Team(0) = CInt(value)
                                    Case "Semifinal 1 Player 2" : sett.Coppa.Semifinale1Team(1) = CInt(value)
                                    Case "Semifinal 2 Player 1" : sett.Coppa.Semifinale2Team(0) = CInt(value)
                                    Case "Semifinal 2 Player 2" : sett.Coppa.Semifinale2Team(1) = CInt(value)
                                    Case "Semifinal 1 Matchs 1" : sett.Coppa.Semifinale1Match(0) = CInt(value)
                                    Case "Semifinal 1 Matchs 2" : sett.Coppa.Semifinale1Match(1) = CInt(value)
                                    Case "Semifinal 2 Matchs 1" : sett.Coppa.Semifinale2Match(0) = CInt(value)
                                    Case "Semifinal 2 Matchs 2" : sett.Coppa.Semifinale2Match(1) = CInt(value)
                                    Case "Final Player 1" : sett.Coppa.FinaleTeam(0) = CInt(value)
                                    Case "Final Player 2" : sett.Coppa.FinaleTeam(1) = CInt(value)
                                    Case "Final Matchs 1" : sett.Coppa.FinaleMatch(0) = CInt(value)
                                    Case "Final Matchs 2" : sett.Coppa.FinaleMatch(1) = CInt(value)
                                End Select

                            Catch ex As Exception
                                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
                            End Try
                        End If
                    Next

                    If sett.Points.SiteReferenceForPoints.Count = 0 Then sett.Points.SiteReferenceForPoints.Add("gazzetta")
                    If sett.Points.SiteReferenceForBonus = "" Then sett.Points.SiteReferenceForBonus = "gazzetta"

                Catch ex As Exception
                    WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
                End Try
            End If

            Return sett

        End Function

        ''' <summary>Consente di salvare le impostazioni su disco</summary>
        Sub SaveSettings(Year As String)

            WebData.Functions.WriteLog(WebData.Functions.eMessageType.Info, "Salvataggio impostazioni per il torneo: " & Year)

            Dim fname As String = GetSettingsFileName(PublicVariables.Year)

            Try
                Dim str As New System.Text.StringBuilder
                str.AppendLine("[Lega]")
                str.AppendLine("Active = '" & PublicVariables.Settings.Active & "'")
                str.AppendLine("Year = '" & PublicVariables.Settings.Year & "'")
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
                str.AppendLine("Bonus defense 3 = '" & PublicVariables.Settings.Bonus.BonusDefense("3") & "'")
                str.AppendLine("Bonus defense 4 = '" & PublicVariables.Settings.Bonus.BonusDefense("4") & "'")
                str.AppendLine("Bonus defense 5 = '" & PublicVariables.Settings.Bonus.BonusDefense("5") & "'")
                str.AppendLine("Enable bonus center field = '" & PublicVariables.Settings.Bonus.EnableCenterField & "'")
                str.AppendLine("Bonus center field source = '" & PublicVariables.Settings.Bonus.BonudCenterFieldSource & "'")
                str.AppendLine("Bonus center field over and equal value = '" & PublicVariables.Settings.Bonus.BonusCenterFieldOverEqual & "'")
                str.AppendLine("Bonus center field 3 = '" & PublicVariables.Settings.Bonus.BonusCenterField("3") & "'")
                str.AppendLine("Bonus center field 4 = '" & PublicVariables.Settings.Bonus.BonusCenterField("4") & "'")
                str.AppendLine("Bonus center field 5 = '" & PublicVariables.Settings.Bonus.BonusCenterField("5") & "'")
                str.AppendLine("Enable bonus attack = '" & PublicVariables.Settings.Bonus.EnableBonusAttack & "'")
                str.AppendLine("Bonus attack source = '" & PublicVariables.Settings.Bonus.BonudAttackSource & "'")
                str.AppendLine("Bonus attack over and equal value = '" & PublicVariables.Settings.Bonus.BonusAttackOverEqual & "'")
                str.AppendLine("Bonus attack 2 = '" & PublicVariables.Settings.Bonus.BonusAttack("2") & "'")
                str.AppendLine("Bonus attack 3 = '" & PublicVariables.Settings.Bonus.BonusAttack("3") & "'")
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
                IO.File.WriteAllText(fname, str.ToString)
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Shared Function SendMail(ToAddress As String, CcAddress As String, Display As String, Subject As String, Body As String, AttachFileList As List(Of String)) As Boolean

            Dim ris As Boolean = True
            Dim cred As New Net.NetworkCredential("formazioni@ifantacalcio.it", "Anxanum1969!")
            Dim smtp As New System.Net.Mail.SmtpClient
            Dim mail As New System.Net.Mail.MailMessage()

            Try

                If ToAddress <> "" AndAlso System.Text.RegularExpressions.Regex.Match(ToAddress, "^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$").Value = ToAddress Then


                    smtp.UseDefaultCredentials = False
                    smtp.Credentials = cred
                    smtp.DeliveryMethod = Net.Mail.SmtpDeliveryMethod.Network
                    smtp.Port = 25
                    smtp.EnableSsl = False
                    smtp.Host = "smtp.aruba.it"

                    If Display = "" Then Display = cred.UserName

                    mail.From = New System.Net.Mail.MailAddress(cred.UserName, Display)
                    mail.To.Add(ToAddress)
                    If CcAddress <> "" Then
                        mail.CC.Add(CcAddress)
                    End If
                    mail.Subject = Subject
                    mail.IsBodyHtml = True
                    mail.Body = Body

                    For i As Integer = 0 To AttachFileList.Count - 1
                        mail.Attachments.Add(New System.Net.Mail.Attachment(AttachFileList(i)))
                    Next

                    smtp.Send(mail)

                Else
                    WebData.Functions.WriteLog(WebData.Functions.eMessageType.Info, "Mail administrator lega missing or not valid")
                    ris = False

                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
                ris = False
            End Try

            mail.Dispose()
            smtp = Nothing

            Return ris

        End Function

        Public Class WebApplication
            Public Property Icon As String = ""
            Public Property Text As String = ""
            Public Property Description As String = ""
            Public Property Link As String = ""
            Public Property CustomCssStyle As String = ""

            Sub New()

            End Sub

            Sub New(Icon As String, Text As String, Description As String, Link As String, CustomCssStyle As String)
                Me.Icon = Icon
                Me.Text = Text
                Me.Description = Description
                Me.Link = Link
                Me.CustomCssStyle = CustomCssStyle
            End Sub

        End Class

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

        Public Class Account

            Public Id As Integer = -1
            Public Username As String = ""
            Public Password As String = ""
            Public TeamId As String = "-1"
            Public Role As String = "user"
            Public Mail As String = ""
            Public Token As String = ""

            Public Sub New()

            End Sub

            Public Sub New(Username As String, Password As String, TeamId As String, Mail As String)
                Me.Username = Username
                Me.Password = Password
                Me.TeamId = TeamId
                Me.Mail = Mail
            End Sub

            Public Sub New(Username As String, Password As String, TeamId As String, Mail As String, role As String)
                Me.Username = Username
                Me.Password = Password
                Me.TeamId = TeamId
                Me.Mail = Mail
                Me.Role = role
            End Sub

        End Class
    End Class
End Namespace

