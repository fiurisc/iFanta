'Namespace Torneo
'    Public Class Coppa

'        Dim girel As New List(Of Girone)
'        Dim poff As New List(Of Girone)
'        Dim qf As New List(Of Girone.PartitaGirone)
'        Dim sem As New List(Of Girone.PartitaGirone)
'        Dim fin As New List(Of Girone.PartitaGirone)
'        Dim _tiposecondoturno As String = "playoff"

'        Public Property TipoSecondoTurno As String
'            Get
'                Return _tiposecondoturno
'            End Get
'            Set(value As String)
'                _tiposecondoturno = value
'            End Set
'        End Property

'        Public Property GironiEliminatori() As List(Of Girone)
'            Get
'                Return girel
'            End Get
'            Set(ByVal value As List(Of Girone))
'                girel = value
'            End Set
'        End Property

'        Public Property PlayOff() As List(Of Girone)
'            Get
'                Return poff
'            End Get
'            Set(ByVal value As List(Of Girone))
'                poff = value
'            End Set
'        End Property

'        Public Property QuartiDiFinale() As List(Of Girone.PartitaGirone)
'            Get
'                Return qf
'            End Get
'            Set(ByVal value As List(Of Girone.PartitaGirone))
'                qf = value
'            End Set
'        End Property

'        Public Property SemiFinali() As List(Of Girone.PartitaGirone)
'            Get
'                Return sem
'            End Get
'            Set(ByVal value As List(Of Girone.PartitaGirone))
'                sem = value
'            End Set
'        End Property

'        Public Property Finale() As List(Of Girone.PartitaGirone)
'            Get
'                Return fin
'            End Get
'            Set(ByVal value As List(Of Girone.PartitaGirone))
'                fin = value
'            End Set
'        End Property

'        Function GetGoalString(ByVal Goal As Integer) As String
'            If Goal > -5 Then
'                If Goal > 0 Then
'                    Return CStr(Goal)
'                Else
'                    Return "0"
'                End If
'            Else
'                Return ""
'            End If
'        End Function

'        Sub Load()

'            currlega.Classifica.LoadHistory()

'            _tiposecondoturno = currlega.Settings.Coppa.TipoSecondoTurno

'            girel.Clear()
'            girel.Add(New Girone)
'            girel.Add(New Girone)

'            For i As Integer = 0 To 4
'                girel(0).Clasa.Add(New Girone.ClasaGirone(i, currlega.Teams(i).IdTeam))
'            Next
'            For i As Integer = 0 To 4
'                girel(1).Clasa.Add(New Girone.ClasaGirone(i, currlega.Teams(i + 5).IdTeam))
'            Next

'            girel(0).Partite.Add(New Girone.PartitaGirone(0, 1, 2, 14, 0, 1, 0, 1))
'            girel(0).Partite.Add(New Girone.PartitaGirone(1, 1, 2, 14, 2, 3, 2, 3))
'            girel(0).Partite.Add(New Girone.PartitaGirone(0, 2, 3, 15, 0, 2, 0, 2))
'            girel(0).Partite.Add(New Girone.PartitaGirone(1, 2, 3, 15, 4, 3, 4, 3))
'            girel(0).Partite.Add(New Girone.PartitaGirone(0, 3, 7, 19, 3, 0, 3, 0))
'            girel(0).Partite.Add(New Girone.PartitaGirone(1, 3, 7, 19, 1, 4, 1, 4))
'            girel(0).Partite.Add(New Girone.PartitaGirone(0, 4, 8, 20, 0, 4, 0, 4))
'            girel(0).Partite.Add(New Girone.PartitaGirone(1, 4, 8, 20, 2, 1, 2, 1))
'            girel(0).Partite.Add(New Girone.PartitaGirone(0, 5, 10, 22, 3, 1, 3, 1))
'            girel(0).Partite.Add(New Girone.PartitaGirone(1, 5, 10, 22, 4, 2, 4, 2))

'            girel(1).Partite.Add(New Girone.PartitaGirone(0, 1, 2, 14, 3, 1, 8, 6))
'            girel(1).Partite.Add(New Girone.PartitaGirone(1, 1, 2, 14, 0, 4, 5, 9))
'            girel(1).Partite.Add(New Girone.PartitaGirone(0, 2, 3, 15, 0, 2, 5, 7))
'            girel(1).Partite.Add(New Girone.PartitaGirone(1, 2, 3, 15, 4, 3, 9, 8))
'            girel(1).Partite.Add(New Girone.PartitaGirone(0, 3, 7, 19, 4, 1, 9, 6))
'            girel(1).Partite.Add(New Girone.PartitaGirone(1, 3, 7, 19, 3, 2, 8, 7))
'            girel(1).Partite.Add(New Girone.PartitaGirone(0, 4, 8, 20, 1, 0, 6, 5))
'            girel(1).Partite.Add(New Girone.PartitaGirone(1, 4, 8, 20, 2, 4, 7, 9))
'            girel(1).Partite.Add(New Girone.PartitaGirone(0, 5, 10, 22, 2, 1, 7, 6))
'            girel(1).Partite.Add(New Girone.PartitaGirone(1, 5, 10, 22, 0, 3, 5, 8))

'            poff.Clear()
'            poff.Add(New Girone)
'            poff.Add(New Girone)

'            For i As Integer = 0 To currlega.Settings.Coppa.PlayOffGiorone1Team.Length - 1
'                poff(0).Clasa.Add(New Girone.ClasaGirone(i, currlega.Settings.Coppa.PlayOffGiorone1Team(i)))
'            Next
'            For i As Integer = 0 To currlega.Settings.Coppa.PlayOffGiorone2Team.Length - 1
'                poff(1).Clasa.Add(New Girone.ClasaGirone(i, currlega.Settings.Coppa.PlayOffGiorone2Team(i)))
'            Next

'            poff(0).Partite.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.PlayOffGiorone1Match(0), -1, 0, 2, poff(0).Clasa(0).TeamId, poff(0).Clasa(2).TeamId))
'            poff(0).Partite.Add(New Girone.PartitaGirone(0, 2, currlega.Settings.Coppa.PlayOffGiorone1Match(1), -1, 1, 2, poff(0).Clasa(1).TeamId, poff(0).Clasa(2).TeamId))
'            poff(0).Partite.Add(New Girone.PartitaGirone(0, 3, currlega.Settings.Coppa.PlayOffGiorone1Match(2), -1, 0, 1, poff(0).Clasa(0).TeamId, poff(0).Clasa(1).TeamId))

'            poff(1).Partite.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.PlayOffGiorone2Match(0), -1, 0, 2, poff(1).Clasa(0).TeamId, poff(1).Clasa(2).TeamId))
'            poff(1).Partite.Add(New Girone.PartitaGirone(0, 2, currlega.Settings.Coppa.PlayOffGiorone2Match(1), -1, 1, 2, poff(1).Clasa(1).TeamId, poff(1).Clasa(2).TeamId))
'            poff(1).Partite.Add(New Girone.PartitaGirone(0, 3, currlega.Settings.Coppa.PlayOffGiorone2Match(2), -1, 0, 1, poff(1).Clasa(0).TeamId, poff(1).Clasa(1).TeamId))

'            qf.Clear()
'            qf.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.QuartiDiFinale1Match(0), currlega.Settings.Coppa.QuartiDiFinale1Match(1), 0, 1, currlega.Settings.Coppa.QuartiDiFinale1Team(0), currlega.Settings.Coppa.QuartiDiFinale1Team(1)))
'            qf.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.QuartiDiFinale2Match(0), currlega.Settings.Coppa.QuartiDiFinale2Match(1), 0, 1, currlega.Settings.Coppa.QuartiDiFinale2Team(0), currlega.Settings.Coppa.QuartiDiFinale2Team(1)))
'            qf.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.QuartiDiFinale3Match(0), currlega.Settings.Coppa.QuartiDiFinale3Match(1), 0, 1, currlega.Settings.Coppa.QuartiDiFinale3Team(0), currlega.Settings.Coppa.QuartiDiFinale3Team(1)))
'            qf.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.QuartiDiFinale4Match(0), currlega.Settings.Coppa.QuartiDiFinale4Match(1), 0, 1, currlega.Settings.Coppa.QuartiDiFinale4Team(0), currlega.Settings.Coppa.QuartiDiFinale4Team(1)))

'            sem.Clear()
'            sem.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.Semifinale1Match(0), currlega.Settings.Coppa.Semifinale1Match(1), 0, 1, currlega.Settings.Coppa.Semifinale1Team(0), currlega.Settings.Coppa.Semifinale1Team(1)))
'            sem.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.Semifinale2Match(0), currlega.Settings.Coppa.Semifinale2Match(1), 0, 1, currlega.Settings.Coppa.Semifinale2Team(0), currlega.Settings.Coppa.Semifinale2Team(1)))

'            fin.Clear()
'            fin.Add(New Girone.PartitaGirone(0, 1, currlega.Settings.Coppa.FinaleMatch(0), currlega.Settings.Coppa.FinaleMatch(1), 0, 1, currlega.Settings.Coppa.FinaleTeam(0), currlega.Settings.Coppa.FinaleTeam(1)))

'            For Each gir As Girone In girel
'                SetRisultatiFinali(gir.Partite)
'                SetClassifica(gir.Partite, gir.Clasa)
'            Next

'            For Each p As Girone In poff
'                SetRisultatiFinali(p.Partite)
'                SetClassifica(p.Partite, p.Clasa)
'            Next

'            SetRisultatiFinali(qf)
'            SetRisultatiFinali(sem)
'            SetRisultatiFinali(fin)

'        End Sub

'        Private Sub SetClassifica(partite As List(Of Girone.PartitaGirone), clasa As List(Of Girone.ClasaGirone))


'            For Each partita As Girone.PartitaGirone In partite
'                Dim pt1 As Integer = 0
'                Dim pt2 As Integer = 0

'                pt1 = GetPtTeam(partita.GoalAnd1, partita.GoalAnd2)
'                pt2 = GetPtTeam(partita.GoalAnd2, partita.GoalAnd1)
'                If pt1 > -1 Then
'                    clasa(partita.TeamGironeId1).PartiteGiocate = clasa(partita.TeamGironeId1).PartiteGiocate + 1
'                    clasa(partita.TeamGironeId1).Pt = clasa(partita.TeamGironeId1).Pt + pt1
'                End If
'                If pt2 > -1 Then
'                    clasa(partita.TeamGironeId2).PartiteGiocate = clasa(partita.TeamGironeId2).PartiteGiocate + 1
'                    clasa(partita.TeamGironeId2).Pt = clasa(partita.TeamGironeId2).Pt + pt2
'                End If

'                SetTypeOfResult(clasa(partita.TeamGironeId1), pt1)
'                SetTypeOfResult(clasa(partita.TeamGironeId2), pt2)

'                pt1 = GetPtTeam(partita.GoalRit1, partita.GoalRit2)
'                pt2 = GetPtTeam(partita.GoalRit2, partita.GoalRit1)

'                If pt1 > -1 Then
'                    clasa(partita.TeamGironeId1).PartiteGiocate = clasa(partita.TeamGironeId1).PartiteGiocate + 1
'                    clasa(partita.TeamGironeId1).Pt = clasa(partita.TeamGironeId1).Pt + pt1
'                End If
'                If pt2 > -1 Then
'                    clasa(partita.TeamGironeId2).PartiteGiocate = clasa(partita.TeamGironeId2).PartiteGiocate + 1
'                    clasa(partita.TeamGironeId2).Pt = clasa(partita.TeamGironeId2).Pt + pt2
'                End If

'                If partita.GoalAnd1 > 0 Then
'                    clasa(partita.TeamGironeId1).GoalFatti = clasa(partita.TeamGironeId1).GoalFatti + partita.GoalAnd1
'                End If
'                If partita.GoalRit1 > 0 Then
'                    clasa(partita.TeamGironeId1).GoalFatti = clasa(partita.TeamGironeId1).GoalFatti + partita.GoalRit1
'                End If
'                If partita.GoalAnd2 > 0 Then
'                    clasa(partita.TeamGironeId2).GoalFatti = clasa(partita.TeamGironeId2).GoalFatti + partita.GoalAnd2
'                End If
'                If partita.GoalRit2 > 0 Then
'                    clasa(partita.TeamGironeId2).GoalFatti = clasa(partita.TeamGironeId2).GoalFatti + partita.GoalRit2
'                End If
'            Next

'        End Sub

'        Private Sub SetRisultatiFinali(partite As List(Of Girone.PartitaGirone))
'            For Each partita As Girone.PartitaGirone In partite
'                If partita.TeamId1 <> -1 Then partita.GoalAnd1 = GetGoal(currlega.Classifica.History(partita.TeamId1).Giornate(partita.GiornataAndata).Pt, True)
'                If partita.TeamId2 <> -1 Then partita.GoalAnd2 = GetGoal(currlega.Classifica.History(partita.TeamId2).Giornate(partita.GiornataAndata).Pt, False)
'                If partita.TeamId1 <> -1 Then partita.GoalRit1 = GetGoal(currlega.Classifica.History(partita.TeamId1).Giornate(partita.GiornataRitorno).Pt, False)
'                If partita.TeamId2 <> -1 Then partita.GoalRit2 = GetGoal(currlega.Classifica.History(partita.TeamId2).Giornate(partita.GiornataRitorno).Pt, True)
'            Next
'        End Sub

'        Private Sub SetTypeOfResult(ByRef TeamCup As LegaObject.Coppa.Girone.ClasaGirone, ByVal Pt As Integer)
'            Select Case Pt
'                Case 3 : TeamCup.Vittorie = TeamCup.Vittorie + 1
'                Case 1 : TeamCup.Pareggi = TeamCup.Pareggi + 1
'                Case 0 : TeamCup.Scofitte = TeamCup.Scofitte + 1
'            End Select
'        End Sub

'        Private Function GetPtTeam(ByVal Goal1 As Integer, ByVal Goal2 As Integer) As Integer
'            If Goal1 > -10 AndAlso Goal2 > -10 Then
'                If Goal1 > Goal2 Then
'                    Return 3
'                ElseIf Goal1 = Goal2 Then
'                    Return 1
'                Else
'                    Return 0
'                End If
'            Else
'                Return -1
'            End If
'        End Function

'        Private Function GetGoal(ByVal pt As Double, ByVal Dentro As Boolean) As Integer
'            If pt > 0 Then
'                pt = pt - 650
'                If Dentro Then pt = pt + 20
'                pt = CInt(Math.Ceiling(pt / 50))
'                If pt < 0 Then pt = 0
'            Else
'                pt = -10
'            End If
'            Return CInt(pt)
'        End Function

'        Public Class Girone

'            Dim clas As New List(Of ClasaGirone)
'            Dim part As New List(Of PartitaGirone)

'            Public Property Clasa() As List(Of ClasaGirone)
'                Get
'                    Return clas
'                End Get
'                Set(ByVal value As List(Of ClasaGirone))
'                    clas = value
'                End Set
'            End Property

'            Public Property Partite() As List(Of PartitaGirone)
'                Get
'                    Return part
'                End Get
'                Set(ByVal value As List(Of PartitaGirone))
'                    part = value
'                End Set
'            End Property

'            Public Class ClasaGirone

'                Dim _teamgironeid As Integer = 0
'                Dim _teamid As Integer = 0
'                Dim _pt As Integer = 0
'                Dim pg As Integer = 0
'                Dim vitt As Integer = 0
'                Dim par As Integer = 0
'                Dim sco As Integer = 0
'                Dim gs As Integer = 0
'                Dim gf As Integer = 0

'                Sub New(ByVal TeamGioroneId As Integer, ByVal TeamId As Integer)
'                    _teamgironeid = TeamGioroneId
'                    _teamid = TeamId
'                End Sub

'                Public Property TeamGioroneId() As Integer
'                    Get
'                        Return _teamgironeid
'                    End Get
'                    Set(ByVal value As Integer)
'                        _teamgironeid = value
'                    End Set
'                End Property

'                Public Property TeamId() As Integer
'                    Get
'                        Return _teamid
'                    End Get
'                    Set(ByVal value As Integer)
'                        _teamid = value
'                    End Set
'                End Property

'                Public Property PartiteGiocate() As Integer
'                    Get
'                        Return pg
'                    End Get
'                    Set(ByVal value As Integer)
'                        pg = value
'                    End Set
'                End Property

'                Public Property Pt() As Integer
'                    Get
'                        Return _pt
'                    End Get
'                    Set(ByVal value As Integer)
'                        _pt = value
'                    End Set
'                End Property

'                Public Property Vittorie() As Integer
'                    Get
'                        Return vitt
'                    End Get
'                    Set(ByVal value As Integer)
'                        vitt = value
'                    End Set
'                End Property

'                Public Property Pareggi() As Integer
'                    Get
'                        Return par
'                    End Get
'                    Set(ByVal value As Integer)
'                        par = value
'                    End Set
'                End Property

'                Public Property Scofitte() As Integer
'                    Get
'                        Return sco
'                    End Get
'                    Set(ByVal value As Integer)
'                        sco = value
'                    End Set
'                End Property

'                Public Property GoalFatti() As Integer
'                    Get
'                        Return gf
'                    End Get
'                    Set(ByVal value As Integer)
'                        gf = value
'                    End Set
'                End Property

'                Public Property GoalSubiti() As Integer
'                    Get
'                        Return gs
'                    End Get
'                    Set(ByVal value As Integer)
'                        gs = value
'                    End Set
'                End Property

'            End Class

'            Public Class ClasaSorter
'                Implements IComparer

'                Private _type As String = ""
'                Private _revers As Boolean = False

'                Sub New()

'                End Sub

'                Sub New(ByVal Type As String, ByVal Revers As Boolean)
'                    _type = Type
'                    _revers = Revers
'                End Sub

'                Public Property Type() As String
'                    Get
'                        Return _type
'                    End Get
'                    Set(ByVal value As String)
'                        _type = value
'                    End Set
'                End Property

'                Public Property Revers() As Boolean
'                    Get
'                        Return _revers
'                    End Get
'                    Set(ByVal value As Boolean)
'                        _revers = value
'                    End Set
'                End Property

'                Public Overridable Overloads Function Compare(ByVal Item1 As Object, ByVal Item2 As Object) As Integer Implements IComparer.Compare

'                    Dim d1 As ClasaGirone = CType(Item1, ClasaGirone)
'                    Dim d2 As ClasaGirone = CType(Item2, ClasaGirone)
'                    Dim ris As Integer = 1
'                    Dim str1 As String = ""
'                    Dim str2 As String = ""

'                    Select Case Type.ToLower
'                        Case "", "pt"
'                            str1 = str1 & CStr(d1.Pt).PadLeft(3, CChar("0"))
'                            str2 = str2 & CStr(d2.Pt).PadLeft(3, CChar("0"))
'                    End Select

'                    ris = String.Compare(str1, str2)
'                    If _revers Then
'                        ris = -ris
'                    End If

'                    Return ris

'                End Function

'            End Class

'            Public Class PartitaGirone

'                Dim _gg As Integer = 0
'                Dim _ggcorr1 As Integer = 0
'                Dim _ggcorr2 As Integer = 0
'                Dim _index As Integer = 0
'                Dim _teamgironeid1 As Integer = 0
'                Dim _teamgironeid2 As Integer = 0
'                Dim _teamid1 As Integer = 0
'                Dim _teamid2 As Integer = 0
'                Dim _goala1 As Integer = -10
'                Dim _goala2 As Integer = -10
'                Dim _goalr1 As Integer = -10
'                Dim _goalr2 As Integer = -10

'                Sub New()

'                End Sub

'                Sub New(ByVal Index As Integer, ByVal Giornata As Integer, ByVal GiornataAndata As Integer, ByVal GiornataRitorno As Integer, ByVal TeamGironeId1 As Integer, ByVal TeamGironeId2 As Integer, ByVal TeamId1 As Integer, ByVal TeamId2 As Integer)
'                    _index = Index
'                    _gg = Giornata
'                    _ggcorr1 = GiornataAndata
'                    _ggcorr2 = GiornataRitorno
'                    _teamgironeid1 = TeamGironeId1
'                    _teamgironeid2 = TeamGironeId2
'                    _teamid1 = TeamId1
'                    _teamid2 = TeamId2
'                End Sub

'                Public Property Index() As Integer
'                    Get
'                        Return _index
'                    End Get
'                    Set(ByVal value As Integer)
'                        _index = value
'                    End Set
'                End Property

'                Public Property Giornata() As Integer
'                    Get
'                        Return _gg
'                    End Get
'                    Set(ByVal value As Integer)
'                        _gg = value
'                    End Set
'                End Property

'                Public Property GiornataAndata() As Integer
'                    Get
'                        Return _ggcorr1
'                    End Get
'                    Set(ByVal value As Integer)
'                        _ggcorr1 = value
'                    End Set
'                End Property

'                Public Property GiornataRitorno() As Integer
'                    Get
'                        Return _ggcorr2
'                    End Get
'                    Set(ByVal value As Integer)
'                        _ggcorr2 = value
'                    End Set
'                End Property

'                Public Property TeamGironeId1() As Integer
'                    Get
'                        Return _teamgironeid1
'                    End Get
'                    Set(ByVal value As Integer)
'                        _teamgironeid1 = value
'                    End Set
'                End Property

'                Public Property TeamGironeId2() As Integer
'                    Get
'                        Return _teamgironeid2
'                    End Get
'                    Set(ByVal value As Integer)
'                        _teamgironeid2 = value
'                    End Set
'                End Property

'                Public Property TeamId1() As Integer
'                    Get
'                        Return _teamid1
'                    End Get
'                    Set(ByVal value As Integer)
'                        _teamid1 = value
'                    End Set
'                End Property

'                Public Property TeamId2() As Integer
'                    Get
'                        Return _teamid2
'                    End Get
'                    Set(ByVal value As Integer)
'                        _teamid2 = value
'                    End Set
'                End Property

'                Public Property GoalAnd1() As Integer
'                    Get
'                        Return _goala1
'                    End Get
'                    Set(ByVal value As Integer)
'                        _goala1 = value
'                    End Set
'                End Property

'                Public Property GoalAnd2() As Integer
'                    Get
'                        Return _goala2
'                    End Get
'                    Set(ByVal value As Integer)
'                        _goala2 = value
'                    End Set
'                End Property

'                Public Property GoalRit1() As Integer
'                    Get
'                        Return _goalr1
'                    End Get
'                    Set(ByVal value As Integer)
'                        _goalr1 = value
'                    End Set
'                End Property

'                Public Property GoalRit2() As Integer
'                    Get
'                        Return _goalr2
'                    End Get
'                    Set(ByVal value As Integer)
'                        _goalr2 = value
'                    End Set
'                End Property

'            End Class
'        End Class

'    End Class
'End Namespace

