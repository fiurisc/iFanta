Imports System.Text.RegularExpressions
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase

Partial Class LegaObject

    Public Class sClassifica

        Private _ptmin As Integer = 10000
        Private _ptmax As Integer = 0
        Private _lst1 As New List(Of sClassificaItem)
        Private _lst2 As New List(Of sClassificaHistory)

        Public ReadOnly Property PtMin() As Integer
            Get
                Return _ptmin
            End Get
        End Property

        Public ReadOnly Property PtMax() As Integer
            Get
                Return _ptmax
            End Get
        End Property

        Public Property Item() As List(Of sClassificaItem)
            Get
                Return _lst1
            End Get
            Set(ByVal value As List(Of sClassificaItem))
                _lst1 = value
            End Set
        End Property

        Public Property History() As List(Of sClassificaHistory)
            Get
                Return _lst2
            End Get
            Set(ByVal value As List(Of sClassificaHistory))
                _lst2 = value
            End Set
        End Property

        Sub LoadHistory()

            If _lst2.Count = 0 Then

                For i As Integer = 0 To currlega.Teams.Count - 1
                    _lst2.Add(New sClassificaHistory(currlega.Teams(i).IdTeam, currlega.Teams(i).Nome, currlega.Teams(i).Allenatore))
                    For k As Integer = 0 To currlega.Settings.NumberOfDays
                        _lst2.Item(i).Giornate.Add(New sClassificaHistory.sClassificaHistoryDay)
                    Next
                    _lst2.Item(i).Giornate(0).Pt = 0
                Next

                Dim str As New System.Text.StringBuilder
                Dim tb As String = "tbformazioni"
                Dim tbtop As String = "tbformazionitop"
                Dim tbptmax As String = "ptmax"
                Dim colptmax As String = "maxpt1"
                Dim stot As String = tb & ".pt"

                If currlega.Settings.ConteggiaGoalFattiPerVittoria AndAlso currlega.Settings.ConteggiaGoalSubitiPerVittoria Then
                    colptmax = "maxpt4"
                    stot = "f.pt+f.gf-f.gs"
                ElseIf currlega.Settings.ConteggiaGoalSubitiPerVittoria Then
                    colptmax = "maxpt2"
                    stot = "f.pt-f.gs"
                ElseIf currlega.Settings.ConteggiaGoalFattiPerVittoria Then
                    colptmax = "maxpt3"
                    stot = "f.pt+f.gf"
                End If

                str.Append("SELECT f.idteam as idteam,f.gio,")
                str.Append("sum(case when incampo=1 OR type=10 then pt end) as pt,sum(case when incampo=1 OR type=10 then " & stot & " end) as stot,sum(case when incampo=1 then f.amm end) as amm,")
                str.Append("sum(case when incampo=1 then f.esp end) as esp,sum( case when incampo=1 then f.ass end) as ass,")
                str.Append("sum(case when incampo=1 then f.gs end) as gs,sum(case when incampo=1 then f.gf end) as gf,")
                str.Append("sum(case when incampo=1 then 1 end) as numg,case when " & stot & "=p." & colptmax & " then 1 else 0 end as vitt,sum(case when f.type=2 or f.type=3 then 1 else 0 end) as jolly,sum(case when f.type>9 then f.pt else 0 end) as bonus ")
                str.Append("FROM " & tb & " as f LEFT JOIN " & tbptmax & " as p ON p.gio=f.gio ")
                str.Append("GROUP BY f.idteam,f.gio,p." & colptmax & " ORDER BY f.idteam")

                Dim ds As DataSet = ExecuteSqlReturnDataSet(str.ToString, conn)

                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then

                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim c As New sClassificaItem
                        Dim idteam As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("idteam"), -1)
                        Dim pt As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("pt"))

                        If pt > 0 AndAlso idteam <> -1 Then

                            Dim gio As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("gio"))
                            Dim amm As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("amm"))
                            Dim esp As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("esp"))
                            Dim ass As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("ass"))
                            Dim gs As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("gs"))
                            Dim gf As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("gf"))
                            Dim vitt As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("vitt"))
                            Dim ptbonus As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("bonus"))
                            Dim jolly As Integer = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("jolly"))
                            Dim n10 As Integer = 0

                            If ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("numg")) < 11 Then
                                n10 = 1
                            End If

                            _lst2(idteam).Giornate(gio).IdTeam = idteam
                            _lst2(idteam).Giornate(gio).Pt = pt

                            _lst2(idteam).Giornate(gio).Ammonizioni = amm
                            _lst2(idteam).Giornate(gio).Espulsioni = esp
                            _lst2(idteam).Giornate(gio).Assist = ass
                            _lst2(idteam).Giornate(gio).GoalSubiti = gs
                            _lst2(idteam).Giornate(gio).GoalFatti = gf
                            _lst2(idteam).Giornate(gio).Vittoria = vitt
                            _lst2(idteam).Giornate(gio).GiocataIn10 = n10
                            _lst2(idteam).Giornate(gio).Jolly = jolly
                            _lst2(idteam).Giornate(gio).PtBonus = ptbonus
                            _lst2(idteam).Giornate(gio).Posizione = 1
                            _lst2(idteam).Giornate(gio).PosizioneGenerale = 1

                            _lst2(idteam).Giornate(0).IdTeam = idteam
                            _lst2(idteam).Giornate(0).Pt = _lst2(idteam).Giornate(0).Pt + pt
                            _lst2(idteam).Giornate(0).PtTot = _lst2(idteam).Giornate(0).Pt
                            _lst2(idteam).Giornate(0).Ammonizioni = _lst2(idteam).Giornate(0).Ammonizioni + amm
                            _lst2(idteam).Giornate(0).Espulsioni = _lst2(idteam).Giornate(0).Espulsioni + esp
                            _lst2(idteam).Giornate(0).Assist = _lst2(idteam).Giornate(0).Assist + ass
                            _lst2(idteam).Giornate(0).GoalSubiti = _lst2(idteam).Giornate(0).GoalSubiti + gs
                            _lst2(idteam).Giornate(0).GoalFatti = _lst2(idteam).Giornate(0).GoalFatti + gf
                            _lst2(idteam).Giornate(0).Vittoria = _lst2(idteam).Giornate(0).Vittoria + vitt
                            _lst2(idteam).Giornate(0).GiocataIn10 = _lst2(idteam).Giornate(0).GiocataIn10 + n10
                            _lst2(idteam).Giornate(0).Jolly = _lst2(idteam).Giornate(0).Jolly + jolly
                            _lst2(idteam).Giornate(0).PtBonus = _lst2(idteam).Giornate(0).PtBonus + ptbonus
                            _lst2(idteam).Giornate(0).Posizione = 1
                            _lst2(idteam).Giornate(0).PosizioneGenerale = 1
                            _lst2(idteam).Giornate(gio).PtTot = _lst2(idteam).Giornate(0).PtTot
                        End If
                    Next
                End If

                'Determino i dati sui punti persi rispetto alle formazioni top'
                str = New System.Text.StringBuilder
                str.Append("SELECT f.idteam as idteam,f.gio,")
                str.Append("sum(case when incampo=1 OR type=10 then f.pt else 0 end) as pt ")
                str.Append("FROM " & tbtop & " as f ")
                str.Append("WHERE f.pt>0 ")
                str.Append("GROUP BY f.idteam,f.gio ORDER BY f.idteam")
                ds = ExecuteSqlReturnDataSet(str.ToString, conn)

                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then

                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim idteam As Integer = CInt(ds.Tables(0).Rows(i).Item("idteam"))
                        Dim pt As Integer = CInt(ds.Tables(0).Rows(i).Item("pt"))
                        Dim gio As Integer = CInt(ds.Tables(0).Rows(i).Item("gio"))

                        _lst2(idteam).Giornate(gio).PtPersi = pt - _lst2(idteam).Giornate(gio).Pt
                        _lst2(idteam).Giornate(0).PtPersi = _lst2(idteam).Giornate(0).PtPersi + _lst2(idteam).Giornate(gio).PtPersi
                        _lst2(idteam).Giornate(0).PtPersiTot = _lst2(idteam).Giornate(0).PtPersi
                        _lst2(idteam).Giornate(gio).PtPersiTot = _lst2(idteam).Giornate(0).PtPersiTot
                        _lst2(idteam).Giornate(gio).PercPtPersi = CInt((_lst2(idteam).Giornate(gio).PtPersiTot / (_lst2(idteam).Giornate(gio).PtPersiTot + _lst2(idteam).Giornate(gio).PtTot)) * 1000) / 10
                        _lst2(idteam).Giornate(0).PercPtPersi = _lst2(idteam).Giornate(gio).PercPtPersi
                        _lst2(idteam).Giornate(gio).PercPtPersi = _lst2(idteam).Giornate(0).PercPtPersi
                    Next
                End If

                'Posizione classifica punteggio giornata' 
                For i As Integer = 0 To currlega.Settings.NumberOfDays

                    Dim pos As Integer = 0
                    Dim ptp As Double = 0
                    Dim d As New List(Of sClassificaHistory.sClassificaHistoryDay)

                    For k As Integer = 0 To _lst2.Count - 1
                        d.Add(_lst2(k).Giornate(i))
                    Next
                    d = sClassifica.sClassificaHistory.Sort(d, "", True)
                    For k As Integer = 0 To d.Count - 1
                        If d(k).Pt <> ptp Then pos = k + 1
                        ptp = d(k).Pt
                        _lst2(d(k).IdTeam).Giornate(i).Posizione = pos
                    Next
                Next

                'Posizione classifica generale' 
                For i As Integer = 0 To currlega.Settings.NumberOfDays

                    Dim pos As Integer = 0
                    Dim ptp As Double = 0
                    Dim ptf As Double = -1
                    Dim d As New List(Of sClassificaHistory.sClassificaHistoryDay)

                    For k As Integer = 0 To _lst2.Count - 1
                        d.Add(_lst2(k).Giornate(i))
                    Next
                    d = sClassifica.sClassificaHistory.Sort(d, "puntitot", True)
                    For k As Integer = 0 To d.Count - 1
                        If d(k).PtTot <> ptp Then pos = k + 1
                        ptp = d(k).PtTot
                        If ptf = -1 Then ptf = _lst2(d(k).IdTeam).Giornate(i).PtTot
                        _lst2(d(k).IdTeam).Giornate(i).PtDiff = ptf - _lst2(d(k).IdTeam).Giornate(i).PtTot
                        _lst2(d(k).IdTeam).Giornate(i).PosizioneGenerale = pos
                    Next
                Next
                ds.Dispose()

            End If
        End Sub

        Sub Load(ByVal Giornata As Integer, ByVal Top As Boolean)
            Try

                Dim old As New List(Of sClassificaItem)
                Dim tc As New List(Of sClassificaItem)

                _ptmin = 100000
                _ptmax = 0

                'Determino i punti totali della giornata scora
                old = GetClassifica(Giornata, Top, False)
                'Determino i punti totali della giornata corrente
                _lst1 = GetClassifica(Giornata + 1, Top, True)

                'Determino i punti delle topformazioni'
                If Top = False Then
                    tc = GetClassifica(Giornata + 1, True, False)
                End If
                For i As Integer = 0 To _lst1.Count - 1
                    For k As Integer = 0 To old.Count - 1
                        If old(k).IdTeam = _lst1(i).IdTeam Then
                            _lst1(i).PreviewPostion = old(k).Postion
                            Exit For
                        End If
                    Next
                Next
                For i As Integer = 0 To _lst1.Count - 1
                    For k As Integer = 0 To tc.Count - 1
                        If tc(k).IdTeam = _lst1(i).IdTeam Then
                            _lst1(i).PuntiPersi = tc(k).Pt - _lst1(i).Pt
                            _lst1(i).PercentualePuntiPersi = CInt((_lst1(i).PuntiPersi / (_lst1(i).Pt + _lst1(i).PuntiPersi)) * 1000) / 10
                            Exit For
                        End If
                    Next
                Next

                Dim p As Integer = 0
                Dim oldv As Integer = -10000000
                Dim fm As New Dictionary(Of Integer, Integer)

                _lst1 = LegaObject.sClassifica.Sort(_lst1, "diffq", True)
                For i As Integer = 0 To _lst1.Count - 1
                    If oldv <> _lst1(i).DiffQ Then
                        p = i + 1
                        oldv = _lst1(i).DiffQ
                    End If
                    If fm.ContainsKey(_lst1(i).IdTeam) = False Then fm.Add(_lst1(i).IdTeam, 0)
                    fm(_lst1(i).IdTeam) = fm(_lst1(i).IdTeam) + p
                Next
                _lst1 = LegaObject.sClassifica.Sort(_lst1, "n10", False)
                p = 0
                oldv = -10000000
                For i As Integer = 0 To _lst1.Count - 1
                    If oldv <> _lst1(i).NumeroGiocateIn10 Then
                        p = i + 1
                        oldv = _lst1(i).NumeroGiocateIn10
                    End If
                    If fm.ContainsKey(_lst1(i).IdTeam) = False Then fm.Add(_lst1(i).IdTeam, 0)
                    fm(_lst1(i).IdTeam) = fm(_lst1(i).IdTeam) + p
                Next
                _lst1 = LegaObject.sClassifica.Sort(_lst1, "sae", False)
                p = 0
                oldv = -10000000
                For i As Integer = 0 To _lst1.Count - 1
                    If oldv <> _lst1(i).SumAmmEsp Then
                        p = i + 1
                        oldv = _lst1(i).SumAmmEsp
                    End If
                    If fm.ContainsKey(_lst1(i).IdTeam) = False Then fm.Add(_lst1(i).IdTeam, 0)
                    fm(_lst1(i).IdTeam) = fm(_lst1(i).IdTeam) + p
                Next

                _lst1 = LegaObject.sClassifica.Sort(_lst1, "punti", True)
                For i As Integer = 0 To _lst1.Count - 1
                    If fm.ContainsKey(_lst1(i).IdTeam) Then
                        _lst1(i).FantaMister = fm(_lst1(i).IdTeam)
                    Else
                        _lst1(i).FantaMister = 0
                    End If
                    If _lst1(i).Postion < 4 Then
                        Select Case _lst1(i).Postion
                            Case 1 : _lst1(i).FantaMister = _lst1(i).FantaMister + 3
                            Case 2 : _lst1(i).FantaMister = _lst1(i).FantaMister + 2
                            Case 3 : _lst1(i).FantaMister = _lst1(i).FantaMister + 1
                        End Select
                    End If
                Next

            Catch ex As Exception

            End Try

        End Sub

        Public Function GetClassifica(ByVal Giornata As Integer, ByVal Top As Boolean, ByVal CalculateMinMax As Boolean) As List(Of sClassificaItem)

            Dim cl As New List(Of sClassificaItem)
            Dim str As New System.Text.StringBuilder
            Dim pos As Integer = 1
            Dim pt1 As Integer = 0
            Dim ptp As Integer = 0
            Dim tb As String = "tbformazioni"
            Dim tbptmax As String = "ptmax"
            Dim colptmax As String = "maxpt1"
            Dim stot As String = "sum(" & tb & ".pt) as stot"

            If Top Then tb = "" & tb & "top" : tbptmax = tbptmax & "top"

            If currlega.Settings.ConteggiaGoalFattiPerVittoria AndAlso currlega.Settings.ConteggiaGoalSubitiPerVittoria Then
                colptmax = "maxpt4"
                stot = "sum(f.pt+2*(f.gf)-f.gs)"
            ElseIf currlega.Settings.ConteggiaGoalSubitiPerVittoria Then
                colptmax = "maxpt2"
                stot = "sum(f.pt-f.gs) as stot"
            ElseIf currlega.Settings.ConteggiaGoalFattiPerVittoria Then
                colptmax = "maxpt3"
                stot = "sum(f.pt+f.gf) as stot"
            End If

            str.Append("SELECT tb1.idteam,teamq.nome,teamq.allenatore,teamq.diff,SUM(tb1.pt) AS tot,SUM(tb1.ptgio) AS ptgio,")
            str.Append("AVG(tb1.pt) AS avgpt, min(pt) AS minpt, max(pt) AS maxpt,sum(tb1.amm) AS amm,")
            str.Append("sum(tb1.esp) AS esp, sum(tb1.ass) AS ass,sum(tb1.gs) AS gs,")
            str.Append("sum(tb1.gf) AS gf,sum(tb1.vitt) as vitt,sum(tb1.vittgio) as vittgio,sum(11-tb1.numg) as n10,sum(tb1.bonus) as bonus ")
            str.Append("FROM (SELECT f.idteam,f.gio,")
            str.Append("sum(f.pt) as pt,case when f.gio=" & Giornata - 1 & " then sum(f.pt) else 0 end as ptgio," & stot & " as stot,sum(f.amm) as amm,")
            str.Append("sum(f.esp) as esp,sum(f.ass) as ass,")
            str.Append("sum(f.gs) as gs,sum(f.gf) as gf,")
            str.Append("sum(incampo) as numg,case when " & stot & "=p." & colptmax & " then 1 else 0 end as vitt,case when " & stot & "=p." & colptmax & " and f.gio=" & Giornata - 1 & " then 1 else 0 end as vittgio,")
            str.Append("sum(case when f.type>9 then f.pt else 0 end) as bonus ")
            str.Append("FROM " & tb & " as f LEFT JOIN " & tbptmax & " as p ON p.gio=f.gio ")
            str.Append("WHERE (incampo=1 OR type=10) and f.pt>-100 and f.gio<" & Giornata & " ")
            str.Append("GROUP BY f.idteam,f.gio,p." & colptmax & ") AS tb1 ")
            str.Append("INNER JOIN teamq ON teamq.idteam=tb1.idteam ")
            str.Append("GROUP BY tb1.idteam,teamq.nome,teamq.allenatore,teamq.diff ORDER BY sum(tb1.pt) DESC;")

            Dim ds As DataSet = ExecuteSqlReturnDataSet(str.ToString, Conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim c As New sClassificaItem
                    Dim pt As Integer = CInt(ds.Tables(0).Rows(i).Item("tot"))
                    If i = 0 Then
                        pt1 = pt
                        ptp = pt
                    End If
                    If pt <> ptp Then pos = i + 1
                    c.Postion = pos
                    c.IdTeam = CInt(ds.Tables(0).Rows(i).Item("idteam"))
                    c.Nome = ds.Tables(0).Rows(i).Item("nome").ToString
                    c.Allenatore = ds.Tables(0).Rows(i).Item("allenatore").ToString
                    c.Pt = pt / 10
                    c.PtGio = CInt(ds.Tables(0).Rows(i).Item("ptgio")) / 10
                    c.PtFirst = (pt1 - pt) / 10
                    c.PtPreviews = (ptp - pt) / 10
                    c.Avg = CInt(ds.Tables(0).Rows(i).Item("avgpt")) / 10
                    c.Min = CInt(ds.Tables(0).Rows(i).Item("minpt")) / 10
                    c.Max = CInt(ds.Tables(0).Rows(i).Item("maxpt")) / 10
                    c.Ammonizioni = CInt(ds.Tables(0).Rows(i).Item("amm"))
                    c.Espulsioni = CInt(ds.Tables(0).Rows(i).Item("esp"))
                    c.SumAmmEsp = c.Ammonizioni + c.Espulsioni * 2
                    c.Espulsioni = CInt(ds.Tables(0).Rows(i).Item("esp"))
                    c.Assist = CInt(ds.Tables(0).Rows(i).Item("ass"))
                    c.GoalSubiti = CInt(ds.Tables(0).Rows(i).Item("gs"))
                    c.GoalFatti = CInt(ds.Tables(0).Rows(i).Item("gf"))
                    c.NumeroGiocateIn10 = CInt(ds.Tables(0).Rows(i).Item("n10"))
                    c.NbrWinner = CInt(ds.Tables(0).Rows(i).Item("vitt"))
                    c.WinnerDay = CInt(ds.Tables(0).Rows(i).Item("vittgio"))
                    c.PtBonus = CInt(ds.Tables(0).Rows(i).Item("bonus")) / 10
                    If ds.Tables(0).Rows(i).Item("diff") IsNot DBNull.Value Then
                        c.DiffQ = CInt(ds.Tables(0).Rows(i).Item("diff"))
                    Else
                        c.DiffQ = 0
                    End If
                    cl.Add(c)
                    ptp = pt
                    If CalculateMinMax Then
                        If c.Pt > _ptmax Then _ptmax = CInt(c.Pt)
                        If c.Pt < _ptmin Then _ptmin = CInt(c.Pt)
                    End If
                Next
            End If
            ds.Dispose()

            Return cl

        End Function

        Public Shared Function Sort(ByVal Data As List(Of sClassificaItem), ByVal Type As String, ByVal Reverse As Boolean) As List(Of sClassificaItem)
            Dim a() As sClassificaItem = Data.ToArray
            Dim s As New sClassificaSorter(Type, Reverse)
            Array.Sort(a, s)
            Dim ris As New List(Of sClassificaItem)
            ris.AddRange(a)
            Return ris
        End Function

        Public Class sClassificaSorter
            Implements IComparer

            Private _type As String = ""
            Private _revers As Boolean = False

            Sub New()

            End Sub

            Sub New(ByVal Type As String, ByVal Revers As Boolean)
                _type = Type
                _revers = Revers
            End Sub

            Public Property Type() As String
                Get
                    Return _type
                End Get
                Set(ByVal value As String)
                    _type = value
                End Set
            End Property

            Public Property Revers() As Boolean
                Get
                    Return _revers
                End Get
                Set(ByVal value As Boolean)
                    _revers = value
                End Set
            End Property

            Public Overridable Overloads Function Compare(ByVal Item1 As Object, ByVal Item2 As Object) As Integer Implements IComparer.Compare

                Dim d1 As sClassificaItem = CType(Item1, sClassificaItem)
                Dim d2 As sClassificaItem = CType(Item2, sClassificaItem)
                Dim ris As Integer = 1
                Dim str1 As String = ""
                Dim str2 As String = ""

                Select Case Type.ToLower
                    Case "pos"
                        str1 = str1 & CStr(d1.Postion).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.Postion).PadLeft(3, CChar("0"))
                    Case "varpos"
                        str1 = str1 & CStr((d1.PreviewPostion - d1.Postion)).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr((d2.PreviewPostion - d2.Postion)).PadLeft(3, CChar("0"))
                    Case "nome"
                        str1 = str1 & CStr(d1.Nome).PadRight(50, CChar("0"))
                        str2 = str2 & CStr(d2.Nome).PadRight(50, CChar("0"))
                    Case "punti", ""
                        str1 = str1 & CStr(d1.Pt * 10).PadLeft(10, CChar("0"))
                        str2 = str2 & CStr(d2.Pt * 10).PadLeft(10, CChar("0"))
                    Case "diff1"
                        str1 = str1 & CStr(d1.PtFirst * 10).PadLeft(10, CChar("0"))
                        str2 = str2 & CStr(d2.PtFirst * 10).PadLeft(10, CChar("0"))
                    Case "diff2"
                        str1 = str1 & CStr(d1.PtPreviews * 10).PadLeft(10, CChar("0"))
                        str2 = str2 & CStr(d2.PtPreviews * 10).PadLeft(10, CChar("0"))
                    Case "avgpt"
                        str1 = str1 & CStr(CInt(d1.Avg * 100)).PadLeft(10, CChar("0"))
                        str2 = str2 & CStr(CInt(d2.Avg * 100)).PadLeft(10, CChar("0"))
                    Case "ptmin"
                        str1 = str1 & CStr(d1.Min * 10).PadLeft(10, CChar("0"))
                        str2 = str2 & CStr(d2.Min * 10).PadLeft(10, CChar("0"))
                    Case "ptmax"
                        str1 = str1 & CStr(d1.Max * 10).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.Max * 10).PadLeft(5, CChar("0"))
                    Case "amm"
                        str1 = str1 & CStr(d1.Ammonizioni).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.Ammonizioni).PadLeft(5, CChar("0"))
                    Case "esp"
                        str1 = str1 & CStr(d1.Espulsioni).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.Espulsioni).PadLeft(5, CChar("0"))
                    Case "sae"
                        str1 = str1 & CStr(d1.SumAmmEsp).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.SumAmmEsp).PadLeft(5, CChar("0"))
                    Case "ass"
                        str1 = str1 & CStr(d1.Assist).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.Assist).PadLeft(5, CChar("0"))
                    Case "gs"
                        str1 = str1 & CStr(d1.GoalSubiti).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.GoalSubiti).PadLeft(5, CChar("0"))
                    Case "gf"
                        str1 = str1 & CStr(d1.GoalFatti).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.GoalFatti).PadLeft(5, CChar("0"))
                    Case "pp"
                        str1 = str1 & CStr(d1.PuntiPersi).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.PuntiPersi).PadLeft(5, CChar("0"))
                    Case "percpp"
                        str1 = str1 & CStr(CInt(d1.PercentualePuntiPersi * 100)).PadLeft(10, CChar("0"))
                        str2 = str2 & CStr(CInt(d2.PercentualePuntiPersi * 100)).PadLeft(10, CChar("0"))
                    Case "ptbonus"
                        str1 = str1 & CStr(d1.PtBonus).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.PtBonus).PadLeft(5, CChar("0"))
                    Case "nbrvitt"
                        str1 = str1 & CStr(d1.NbrWinner).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.NbrWinner).PadLeft(5, CChar("0"))
                    Case "n10"
                        str1 = str1 & CStr(d1.NumeroGiocateIn10).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.NumeroGiocateIn10).PadLeft(5, CChar("0"))
                        'Case "jolly"
                        '    str1 = str1 & CStr(d1.).PadLeft(5, CChar("0"))
                        '    str2 = str2 & CStr(d2.NbrWinner).PadLeft(5, CChar("0"))
                    Case "diffq"
                        str1 = str1 & CStr(d1.DiffQ + 1000).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.DiffQ + 1000).PadLeft(5, CChar("0"))
                    Case "fm"
                        str1 = str1 & CStr(d1.FantaMister).PadLeft(5, CChar("0"))
                        str2 = str2 & CStr(d2.FantaMister).PadLeft(5, CChar("0"))
                End Select

                ris = String.Compare(str1, str2)
                If _revers Then
                    ris = -ris
                End If

                Return ris

            End Function

        End Class

        Public Class sClassificaItem

            Private _pos As Integer = 0
            Private _prevpos As Integer = 0
            Private _idteam As Integer = 0
            Private _nome As String = ""
            Private _allenatore As String = ""
            Private _pt As Double = 0
            Private _ptgio As Double = 0
            Private _vittgio As Integer = 0
            Private _ptfirst As Double = 0
            Private _ptpreviews As Double = 0
            Private _avg As Double = 0
            Private _min As Double = 0
            Private _max As Double = 0
            Private _amm As Integer = 0
            Private _esp As Integer = 0
            Private _sae As Integer = 0
            Private _ass As Integer = 0
            Private _gs As Integer = 0
            Private _gf As Integer = 0
            Private _pp As Double = 0
            Private _percpp As Double = 0
            Private _n10 As Integer = 0
            Private _nbrwinner As Integer = 0
            Private _ptbonus As Double = 0
            Private _diffq As Integer = 0
            Private _fm As Integer = 0

            Sub New()

            End Sub

            Public Property Postion() As Integer
                Get
                    Return _pos
                End Get
                Set(ByVal value As Integer)
                    _pos = value
                End Set
            End Property

            Public Property PreviewPostion() As Integer
                Get
                    Return _prevpos
                End Get
                Set(ByVal value As Integer)
                    _prevpos = value
                End Set
            End Property

            Public Property IdTeam() As Integer
                Get
                    Return _idteam
                End Get
                Set(ByVal value As Integer)
                    _idteam = value
                End Set
            End Property

            Public Property Nome() As String
                Get
                    Return _nome
                End Get
                Set(ByVal value As String)
                    _nome = value
                End Set
            End Property

            Public Property Allenatore() As String
                Get
                    Return _allenatore
                End Get
                Set(ByVal value As String)
                    _allenatore = value
                End Set
            End Property

            Public Property Pt() As Double
                Get
                    Return _pt
                End Get
                Set(ByVal value As Double)
                    _pt = value
                End Set
            End Property

            Public Property PtGio() As Double
                Get
                    Return _ptgio
                End Get
                Set(ByVal value As Double)
                    _ptgio = value
                End Set
            End Property

            Public Property PtFirst() As Double
                Get
                    Return _ptfirst
                End Get
                Set(ByVal value As Double)
                    _ptfirst = value
                End Set
            End Property

            Public Property PtPreviews() As Double
                Get
                    Return _ptpreviews
                End Get
                Set(ByVal value As Double)
                    _ptpreviews = value
                End Set
            End Property

            Public Property Avg() As Double
                Get
                    Return _avg
                End Get
                Set(ByVal value As Double)
                    _avg = value
                End Set
            End Property

            Public Property Min() As Double
                Get
                    Return _min
                End Get
                Set(ByVal value As Double)
                    _min = value
                End Set
            End Property

            Public Property Max() As Double
                Get
                    Return _max
                End Get
                Set(ByVal value As Double)
                    _max = value
                End Set
            End Property

            Public Property Ammonizioni() As Integer
                Get
                    Return _amm
                End Get
                Set(ByVal value As Integer)
                    _amm = value
                End Set
            End Property

            Public Property Espulsioni() As Integer
                Get
                    Return _esp
                End Get
                Set(ByVal value As Integer)
                    _esp = value
                End Set
            End Property

            Public Property SumAmmEsp() As Integer
                Get
                    Return _sae
                End Get
                Set(ByVal value As Integer)
                    _sae = value
                End Set
            End Property

            Public Property Assist() As Integer
                Get
                    Return _ass
                End Get
                Set(ByVal value As Integer)
                    _ass = value
                End Set
            End Property

            Public Property GoalSubiti() As Integer
                Get
                    Return _gs
                End Get
                Set(ByVal value As Integer)
                    _gs = value
                End Set
            End Property

            Public Property GoalFatti() As Integer
                Get
                    Return _gf
                End Get
                Set(ByVal value As Integer)
                    _gf = value
                End Set
            End Property

            Public Property PuntiPersi() As Double
                Get
                    Return _pp
                End Get
                Set(ByVal value As Double)
                    _pp = value
                End Set
            End Property

            Public Property PercentualePuntiPersi() As Double
                Get
                    Return _percpp
                End Get
                Set(ByVal value As Double)
                    _percpp = value
                End Set
            End Property

            Public Property NumeroGiocateIn10() As Integer
                Get
                    Return _n10
                End Get
                Set(ByVal value As Integer)
                    _n10 = value
                End Set
            End Property

            Public Property NbrWinner() As Integer
                Get
                    Return _nbrwinner
                End Get
                Set(ByVal value As Integer)
                    _nbrwinner = value
                End Set
            End Property

            Public Property WinnerDay() As Integer
                Get
                    Return _vittgio
                End Get
                Set(ByVal value As Integer)
                    _vittgio = value
                End Set
            End Property

            Public Property PtBonus() As Double
                Get
                    Return _ptbonus
                End Get
                Set(ByVal value As Double)
                    _ptbonus = value
                End Set
            End Property

            Public Property DiffQ() As Integer
                Get
                    Return _diffq
                End Get
                Set(ByVal value As Integer)
                    _diffq = value
                End Set
            End Property

            Public Property FantaMister() As Integer
                Get
                    Return _fm
                End Get
                Set(ByVal value As Integer)
                    _fm = value
                End Set
            End Property

        End Class

        Public Class sClassificaHistory

            Private _idteam As Integer = 0
            Private _nome As String = ""
            Private _allenatore As String = ""
            Private _days As New List(Of sClassificaHistoryDay)

            Sub New()

            End Sub

            Sub New(ByVal idTeam As Integer, ByVal Nome As String, ByVal Allenatore As String)
                _idteam = idTeam
                _nome = Nome
                _allenatore = Allenatore
            End Sub

            Public Property IdTeam() As Integer
                Get
                    Return _idteam
                End Get
                Set(ByVal value As Integer)
                    _idteam = value
                End Set
            End Property

            Public Property Nome() As String
                Get
                    Return _nome
                End Get
                Set(ByVal value As String)
                    _nome = value
                End Set
            End Property

            Public Property Allenatore() As String
                Get
                    Return _allenatore
                End Get
                Set(ByVal value As String)
                    _allenatore = value
                End Set
            End Property

            Public Property Giornate() As List(Of sClassificaHistoryDay)
                Get
                    Return _days
                End Get
                Set(ByVal value As List(Of sClassificaHistoryDay))
                    _days = value
                End Set
            End Property

            Public Shared Function Sort(ByVal Data As List(Of sClassificaHistoryDay), ByVal Type As String, ByVal Reverse As Boolean) As List(Of sClassificaHistoryDay)
                Dim a() As sClassificaHistoryDay = Data.ToArray
                Dim s As New sClassificaHistoryDaySorter(Type, Reverse)
                Array.Sort(a, s)
                Dim ris As New List(Of sClassificaHistoryDay)
                ris.AddRange(a)
                Return ris
            End Function

            Public Class sClassificaHistoryDaySorter
                Implements IComparer

                Private _type As String = ""
                Private _revers As Boolean = False

                Sub New()

                End Sub

                Sub New(ByVal Type As String, ByVal Revers As Boolean)
                    _type = Type
                    _revers = Revers
                End Sub

                Public Property Type() As String
                    Get
                        Return _type
                    End Get
                    Set(ByVal value As String)
                        _type = value
                    End Set
                End Property

                Public Property Revers() As Boolean
                    Get
                        Return _revers
                    End Get
                    Set(ByVal value As Boolean)
                        _revers = value
                    End Set
                End Property

                Public Overridable Overloads Function Compare(ByVal Item1 As Object, ByVal Item2 As Object) As Integer Implements IComparer.Compare

                    Dim d1 As sClassificaHistoryDay = CType(Item1, sClassificaHistoryDay)
                    Dim d2 As sClassificaHistoryDay = CType(Item2, sClassificaHistoryDay)
                    Dim ris As Integer = 1
                    Dim str1 As String = ""
                    Dim str2 As String = ""

                    Select Case Type.ToLower
                        Case "punti", ""
                            str1 = str1 & CStr(d1.Pt).PadLeft(5, CChar("0"))
                            str2 = str2 & CStr(d2.Pt).PadLeft(5, CChar("0"))
                        Case "puntitot"
                            str1 = str1 & CStr(d1.PtTot).PadLeft(5, CChar("0"))
                            str2 = str2 & CStr(d2.PtTot).PadLeft(5, CChar("0"))
                    End Select

                    ris = String.Compare(str1, str2)
                    If _revers Then
                        ris = -ris
                    End If

                    Return ris

                End Function

            End Class

            Public Class sClassificaHistoryDay

                Private _idteam As Integer = 0
                Private _pos As Integer = 1
                Private _posgen As Integer = 1
                Private _pt As Double = -1
                Private _ptdiff As Double = -1
                Private _pttot As Double = 0
                Private _amm As Integer = 0
                Private _esp As Integer = 0
                Private _ass As Integer = 0
                Private _gs As Integer = 0
                Private _gf As Integer = 0
                Private _ptpersi As Double = 0
                Private _ptpersitot As Double = 0
                Private _percptpersi As Double = 0
                Private _vitt As Integer = 0
                Private _n10 As Integer = 0
                Private _jolly As Integer = 0
                Private _ptbonus As Double = 0

                Sub New()

                End Sub

                Sub New(ByVal Dato As sClassificaHistoryDay)
                    _idteam = Dato.IdTeam
                    _pos = Dato.Posizione
                    _posgen = Dato.PosizioneGenerale
                    _pt = Dato.Pt
                    _pttot = Dato.PtTot
                End Sub

                Sub New(ByVal Pt As Double, ByVal Ammonizioni As Integer, ByVal Espulsioni As Integer, ByVal Assist As Integer, ByVal GoalSubiti As Integer, ByVal GoalFatti As Integer, ByVal Vittoria As Integer, ByVal GiocataIn10 As Integer)
                    _pt = Pt
                    _amm = Ammonizioni
                    _esp = Espulsioni
                    _ass = Assist
                    _gs = GoalSubiti
                    _gf = GoalFatti
                    _vitt = Vittoria
                    _n10 = GiocataIn10
                End Sub

                Public Property IdTeam() As Integer
                    Get
                        Return _idteam
                    End Get
                    Set(ByVal value As Integer)
                        _idteam = value
                    End Set
                End Property

                Public Property Posizione() As Integer
                    Get
                        Return _pos
                    End Get
                    Set(ByVal value As Integer)
                        _pos = value
                    End Set
                End Property

                Public Property PosizioneGenerale() As Integer
                    Get
                        Return _posgen
                    End Get
                    Set(ByVal value As Integer)
                        _posgen = value
                    End Set
                End Property

                Public Property Pt() As Double
                    Get
                        Return _pt
                    End Get
                    Set(ByVal value As Double)
                        _pt = value
                    End Set
                End Property

                Public Property PtTot() As Double
                    Get
                        Return _pttot
                    End Get
                    Set(ByVal value As Double)
                        _pttot = value
                    End Set
                End Property

                Public Property PtDiff() As Double
                    Get
                        Return _ptdiff
                    End Get
                    Set(ByVal value As Double)
                        _ptdiff = value
                    End Set
                End Property

                Public Property Ammonizioni() As Integer
                    Get
                        Return _amm
                    End Get
                    Set(ByVal value As Integer)
                        _amm = value
                    End Set
                End Property

                Public Property Espulsioni() As Integer
                    Get
                        Return _esp
                    End Get
                    Set(ByVal value As Integer)
                        _esp = value
                    End Set
                End Property

                Public Property Assist() As Integer
                    Get
                        Return _ass
                    End Get
                    Set(ByVal value As Integer)
                        _ass = value
                    End Set
                End Property

                Public Property GoalSubiti() As Integer
                    Get
                        Return _gs
                    End Get
                    Set(ByVal value As Integer)
                        _gs = value
                    End Set
                End Property

                Public Property GoalFatti() As Integer
                    Get
                        Return _gf
                    End Get
                    Set(ByVal value As Integer)
                        _gf = value
                    End Set
                End Property

                Public Property PtPersi() As Double
                    Get
                        Return _ptpersi
                    End Get
                    Set(ByVal value As Double)
                        _ptpersi = value
                    End Set
                End Property

                Public Property PtPersiTot() As Double
                    Get
                        Return _ptpersitot
                    End Get
                    Set(ByVal value As Double)
                        _ptpersitot = value
                    End Set
                End Property

                Public Property PercPtPersi() As Double
                    Get
                        Return _percptpersi
                    End Get
                    Set(ByVal value As Double)
                        _percptpersi = value
                    End Set
                End Property

                Public Property Vittoria() As Integer
                    Get
                        Return _vitt
                    End Get
                    Set(ByVal value As Integer)
                        _vitt = value
                    End Set
                End Property

                Public Property GiocataIn10() As Integer
                    Get
                        Return _n10
                    End Get
                    Set(ByVal value As Integer)
                        _n10 = value
                    End Set
                End Property

                Public Property Jolly() As Integer
                    Get
                        Return _jolly
                    End Get
                    Set(ByVal value As Integer)
                        _jolly = value
                    End Set
                End Property

                Public Property PtBonus() As Double
                    Get
                        Return _ptbonus
                    End Get
                    Set(ByVal value As Double)
                        _ptbonus = value
                    End Set
                End Property
            End Class
        End Class
    End Class

End Class
