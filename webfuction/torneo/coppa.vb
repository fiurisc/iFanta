Imports System.Data

Namespace Torneo

    Public Class CoppaData


        Sub Load()

            '    currlega.Classifica.LoadHistory()

            '    _tiposecondoturno = Variables.Settings.Coppa.TipoSecondoTurno

            '    Dim cup As New Coppa

            '    cup.GironiEliminatori.Add(New Coppa.Girone)
            '    cup.GironiEliminatori.Add(New Coppa.Girone)

            '    For i As Integer = 0 To 4
            '        cup.GironiEliminatori(0).Clasa.Add(New Coppa.Girone.ClasaGirone(i, i))
            '    Next
            '    For i As Integer = 0 To 4
            '        cup.GironiEliminatori(1).Clasa.Add(New Coppa.Girone.ClasaGirone(i, i + 5))
            '    Next

            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 1, 2, 14, 0, 1, 0, 1))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(1, 1, 2, 14, 2, 3, 2, 3))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 2, 3, 15, 0, 2, 0, 2))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(1, 2, 3, 15, 4, 3, 4, 3))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 3, 7, 19, 3, 0, 3, 0))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(1, 3, 7, 19, 1, 4, 1, 4))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 4, 8, 20, 0, 4, 0, 4))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(1, 4, 8, 20, 2, 1, 2, 1))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 5, 10, 22, 3, 1, 3, 1))
            '    cup.GironiEliminatori(0).Partite.Add(New Coppa.Girone.PartitaGirone(1, 5, 10, 22, 4, 2, 4, 2))

            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 1, 2, 14, 3, 1, 8, 6))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(1, 1, 2, 14, 0, 4, 5, 9))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 2, 3, 15, 0, 2, 5, 7))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(1, 2, 3, 15, 4, 3, 9, 8))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 3, 7, 19, 4, 1, 9, 6))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(1, 3, 7, 19, 3, 2, 8, 7))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 4, 8, 20, 1, 0, 6, 5))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(1, 4, 8, 20, 2, 4, 7, 9))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 5, 10, 22, 2, 1, 7, 6))
            '    cup.GironiEliminatori(1).Partite.Add(New Coppa.Girone.PartitaGirone(1, 5, 10, 22, 0, 3, 5, 8))

            '    poff.Clear()
            '    poff.Add(New Girone)
            '    poff.Add(New Girone)

            '    For i As Integer = 0 To Variables.Settings.Coppa.PlayOffGiorone1Team.Length - 1
            '        poff(0).Clasa.Add(New Girone.ClasaGirone(i, Variables.Settings.Coppa.PlayOffGiorone1Team(i)))
            '    Next
            '    For i As Integer = 0 To Variables.Settings.Coppa.PlayOffGiorone2Team.Length - 1
            '        poff(1).Clasa.Add(New Girone.ClasaGirone(i, Variables.Settings.Coppa.PlayOffGiorone2Team(i)))
            '    Next

            '    poff(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.PlayOffGiorone1Match(0), -1, 0, 2, poff(0).Clasa(0).TeamId, poff(0).Clasa(2).TeamId))
            '    poff(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 2, Variables.Settings.Coppa.PlayOffGiorone1Match(1), -1, 1, 2, poff(0).Clasa(1).TeamId, poff(0).Clasa(2).TeamId))
            '    poff(0).Partite.Add(New Coppa.Girone.PartitaGirone(0, 3, Variables.Settings.Coppa.PlayOffGiorone1Match(2), -1, 0, 1, poff(0).Clasa(0).TeamId, poff(0).Clasa(1).TeamId))

            '    poff(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.PlayOffGiorone2Match(0), -1, 0, 2, poff(1).Clasa(0).TeamId, poff(1).Clasa(2).TeamId))
            '    poff(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 2, Variables.Settings.Coppa.PlayOffGiorone2Match(1), -1, 1, 2, poff(1).Clasa(1).TeamId, poff(1).Clasa(2).TeamId))
            '    poff(1).Partite.Add(New Coppa.Girone.PartitaGirone(0, 3, Variables.Settings.Coppa.PlayOffGiorone2Match(2), -1, 0, 1, poff(1).Clasa(0).TeamId, poff(1).Clasa(1).TeamId))

            '    qf.Clear()
            '    qf.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.QuartiDiFinale1Match(0), Variables.Settings.Coppa.QuartiDiFinale1Match(1), 0, 1, Variables.Settings.Coppa.QuartiDiFinale1Team(0), Variables.Settings.Coppa.QuartiDiFinale1Team(1)))
            '    qf.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.QuartiDiFinale2Match(0), Variables.Settings.Coppa.QuartiDiFinale2Match(1), 0, 1, Variables.Settings.Coppa.QuartiDiFinale2Team(0), Variables.Settings.Coppa.QuartiDiFinale2Team(1)))
            '    qf.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.QuartiDiFinale3Match(0), Variables.Settings.Coppa.QuartiDiFinale3Match(1), 0, 1, Variables.Settings.Coppa.QuartiDiFinale3Team(0), Variables.Settings.Coppa.QuartiDiFinale3Team(1)))
            '    qf.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.QuartiDiFinale4Match(0), Variables.Settings.Coppa.QuartiDiFinale4Match(1), 0, 1, Variables.Settings.Coppa.QuartiDiFinale4Team(0), Variables.Settings.Coppa.QuartiDiFinale4Team(1)))

            '    sem.Clear()
            '    sem.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.Semifinale1Match(0), Variables.Settings.Coppa.Semifinale1Match(1), 0, 1, Variables.Settings.Coppa.Semifinale1Team(0), Variables.Settings.Coppa.Semifinale1Team(1)))
            '    sem.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.Semifinale2Match(0), Variables.Settings.Coppa.Semifinale2Match(1), 0, 1, Variables.Settings.Coppa.Semifinale2Team(0), Variables.Settings.Coppa.Semifinale2Team(1)))

            '    fin.Clear()
            '    fin.Add(New Coppa.Girone.PartitaGirone(0, 1, Variables.Settings.Coppa.FinaleMatch(0), Variables.Settings.Coppa.FinaleMatch(1), 0, 1, Variables.Settings.Coppa.FinaleTeam(0), Variables.Settings.Coppa.FinaleTeam(1)))

            '    For Each gir As Girone In girel
            '        SetRisultatiFinali(gir.Partite)
            '        SetClassifica(gir.Partite, gir.Clasa)
            '    Next

            '    For Each p As Girone In poff
            '        SetRisultatiFinali(p.Partite)
            '        SetClassifica(p.Partite, p.Clasa)
            '    Next

            '    SetRisultatiFinali(qf)
            '    SetRisultatiFinali(sem)
            '    SetRisultatiFinali(fin)

        End Sub

        'Function GetGoalString(ByVal Goal As Integer) As String
        '    If Goal > -5 Then
        '        If Goal > 0 Then
        '            Return CStr(Goal)
        '        Else
        '            Return "0"
        '        End If
        '    Else
        '        Return ""
        '    End If
        'End Function


        'Private Sub SetClassifica(partite As List(Of Coppa.Girone.PartitaGirone), clasa As List(Of Coppa.Girone.ClasaGirone))


        '    For Each partita As Coppa.Girone.PartitaGirone In partite
        '        Dim pt1 As Integer = 0
        '        Dim pt2 As Integer = 0

        '        pt1 = GetPtTeam(partita.GoalAnd1, partita.GoalAnd2)
        '        pt2 = GetPtTeam(partita.GoalAnd2, partita.GoalAnd1)
        '        If pt1 > -1 Then
        '            clasa(partita.TeamGironeId1).PartiteGiocate = clasa(partita.TeamGironeId1).PartiteGiocate + 1
        '            clasa(partita.TeamGironeId1).Pt = clasa(partita.TeamGironeId1).Pt + pt1
        '        End If
        '        If pt2 > -1 Then
        '            clasa(partita.TeamGironeId2).PartiteGiocate = clasa(partita.TeamGironeId2).PartiteGiocate + 1
        '            clasa(partita.TeamGironeId2).Pt = clasa(partita.TeamGironeId2).Pt + pt2
        '        End If

        '        SetTypeOfResult(clasa(partita.TeamGironeId1), pt1)
        '        SetTypeOfResult(clasa(partita.TeamGironeId2), pt2)

        '        pt1 = GetPtTeam(partita.GoalRit1, partita.GoalRit2)
        '        pt2 = GetPtTeam(partita.GoalRit2, partita.GoalRit1)

        '        If pt1 > -1 Then
        '            clasa(partita.TeamGironeId1).PartiteGiocate = clasa(partita.TeamGironeId1).PartiteGiocate + 1
        '            clasa(partita.TeamGironeId1).Pt = clasa(partita.TeamGironeId1).Pt + pt1
        '        End If
        '        If pt2 > -1 Then
        '            clasa(partita.TeamGironeId2).PartiteGiocate = clasa(partita.TeamGironeId2).PartiteGiocate + 1
        '            clasa(partita.TeamGironeId2).Pt = clasa(partita.TeamGironeId2).Pt + pt2
        '        End If

        '        If partita.GoalAnd1 > 0 Then
        '            clasa(partita.TeamGironeId1).GoalFatti = clasa(partita.TeamGironeId1).GoalFatti + partita.GoalAnd1
        '        End If
        '        If partita.GoalRit1 > 0 Then
        '            clasa(partita.TeamGironeId1).GoalFatti = clasa(partita.TeamGironeId1).GoalFatti + partita.GoalRit1
        '        End If
        '        If partita.GoalAnd2 > 0 Then
        '            clasa(partita.TeamGironeId2).GoalFatti = clasa(partita.TeamGironeId2).GoalFatti + partita.GoalAnd2
        '        End If
        '        If partita.GoalRit2 > 0 Then
        '            clasa(partita.TeamGironeId2).GoalFatti = clasa(partita.TeamGironeId2).GoalFatti + partita.GoalRit2
        '        End If
        '    Next

        'End Sub

        'Private Sub SetRisultatiFinali(partite As List(Of Coppa.Girone.PartitaGirone))
        '    For Each partita As Coppa.Girone.PartitaGirone In partite
        '        If partita.TeamId1 <> -1 Then partita.GoalAnd1 = GetGoal(currlega.Classifica.History(partita.TeamId1).Giornate(partita.GiornataAndata).Pt, True)
        '        If partita.TeamId2 <> -1 Then partita.GoalAnd2 = GetGoal(currlega.Classifica.History(partita.TeamId2).Giornate(partita.GiornataAndata).Pt, False)
        '        If partita.TeamId1 <> -1 Then partita.GoalRit1 = GetGoal(currlega.Classifica.History(partita.TeamId1).Giornate(partita.GiornataRitorno).Pt, False)
        '        If partita.TeamId2 <> -1 Then partita.GoalRit2 = GetGoal(currlega.Classifica.History(partita.TeamId2).Giornate(partita.GiornataRitorno).Pt, True)
        '    Next
        'End Sub

        'Private Sub SetTypeOfResult(ByRef TeamCup As Coppa.Girone.ClasaGirone, ByVal Pt As Integer)
        '    Select Case Pt
        '        Case 3 : TeamCup.Vittorie += 1
        '        Case 1 : TeamCup.Pareggi += 1
        '        Case 0 : TeamCup.Scofitte += 1
        '    End Select
        'End Sub

        'Private Function GetPtTeam(ByVal Goal1 As Integer, ByVal Goal2 As Integer) As Integer
        '    If Goal1 > -10 AndAlso Goal2 > -10 Then
        '        If Goal1 > Goal2 Then
        '            Return 3
        '        ElseIf Goal1 = Goal2 Then
        '            Return 1
        '        Else
        '            Return 0
        '        End If
        '    Else
        '        Return -1
        '    End If
        'End Function

        'Private Function GetGoal(ByVal pt As Double, ByVal Dentro As Boolean) As Integer
        '    If pt > 0 Then
        '        pt = pt - 650
        '        If Dentro Then pt = pt + 20
        '        pt = CInt(Math.Ceiling(pt / 50))
        '        If pt < 0 Then pt = 0
        '    Else
        '        pt = -10
        '    End If
        '    Return CInt(pt)
        'End Function

        Public Class Coppa

            Public Property TipoSecondoTurno As String = "playoff"
            Public Property GironiEliminatori() As List(Of Girone) = New List(Of Girone)
            Public Property PlayOff() As List(Of Girone) = New List(Of Girone)
            Public Property QuartiDiFinale() As List(Of Girone.PartitaGirone) = New List(Of Girone.PartitaGirone)
            Public Property SemiFinali() As List(Of Girone.PartitaGirone) = New List(Of Girone.PartitaGirone)
            Public Property Finale() As List(Of Girone.PartitaGirone) = New List(Of Girone.PartitaGirone)

            Public Class Girone

                Public Property Clasa() As List(Of ClasaGirone)
                Public Property Partite() As List(Of PartitaGirone)

                Public Class ClasaGirone

                    Sub New(ByVal TeamGioroneId As Integer, ByVal TeamId As Integer)
                        Me.TeamGioroneId = TeamGioroneId
                        Me.TeamId = TeamId
                    End Sub

                    Public Property TeamGioroneId() As Integer = 0
                    Public Property TeamId() As Integer = 0
                    Public Property PartiteGiocate() As Integer = 0
                    Public Property Pt() As Integer = 0
                    Public Property Vittorie() As Integer = 0
                    Public Property Pareggi() As Integer = 0
                    Public Property Scofitte() As Integer = 0
                    Public Property GoalFatti() As Integer = 0
                    Public Property GoalSubiti() As Integer = 0

                End Class

                Public Class PartitaGirone

                    Sub New()

                    End Sub

                    Sub New(ByVal Index As Integer, ByVal Giornata As Integer, ByVal GiornataAndata As Integer, ByVal GiornataRitorno As Integer, ByVal TeamGironeId1 As Integer, ByVal TeamGironeId2 As Integer, ByVal TeamId1 As Integer, ByVal TeamId2 As Integer)
                        Me.Index = Index
                        Me.Giornata = Giornata
                        Me.GiornataAndata = GiornataAndata
                        Me.GiornataRitorno = GiornataRitorno
                        Me.TeamGironeId1 = TeamGironeId1
                        Me.TeamGironeId2 = TeamGironeId2
                        Me.TeamId1 = TeamId1
                        Me.TeamId2 = TeamId2
                    End Sub

                    Public Property Index() As Integer = 0
                    Public Property Giornata() As Integer = 0
                    Public Property GiornataAndata() As Integer = 0
                    Public Property GiornataRitorno() As Integer = 0
                    Public Property TeamGironeId1() As Integer = 0
                    Public Property TeamGironeId2() As Integer = 0
                    Public Property TeamId1() As Integer = 0
                    Public Property TeamId2() As Integer = 0
                    Public Property GoalAnd1() As Integer = -10
                    Public Property GoalAnd2() As Integer = -10
                    Public Property GoalRit1() As Integer = -10
                    Public Property GoalRit2() As Integer = -10

                End Class
            End Class

        End Class
    End Class

End Namespace