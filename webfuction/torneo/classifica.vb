Imports System.Data

Namespace Torneo
    Public Class Classifica

        Public Shared fname1 As String = PublicVariables.DataPath & "torneo/classifica.json"
        Public Shared fname2 As String = PublicVariables.DataPath & "torneo/classifica-top.json"

        Public Shared Function apiGetLastDay() As String

            Dim cday As String = "1"

            Try
                If PublicVariables.dataFromDatabase Then
                    Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT MAX(gio) AS currgio FROM tbformazioni")
                    If ds.Tables.Count > 0 Then
                        cday = ds.Tables(0).Rows(0).Item("currgio").ToString()
                    End If
                Else
                    Dim j As String = IO.File.ReadAllText(fname1)
                    Dim dicdata As Dictionary(Of String, List(Of sClassificaItem)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, List(Of sClassificaItem)))(j)
                    cday = dicdata.Keys.Last()
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return cday

        End Function

        Public Shared Function apiGetClassifica(day As String, top As Boolean) As String

            Try
                If PublicVariables.dataFromDatabase Then
                    Return WebData.Functions.SerializzaOggetto(GetClassificaGiornata(CInt(day), top), True)
                Else
                    Dim j As String = IO.File.ReadAllText(If(top, fname2, fname1))
                    Dim dicdata As Dictionary(Of String, List(Of sClassificaItem)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, List(Of sClassificaItem)))(j)
                    If dicdata.ContainsKey(day) Then
                        Return WebData.Functions.SerializzaOggetto(dicdata(day), True)
                    End If
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return ""

        End Function

        Shared Function GetClassificaGiornata(ByVal Giornata As Integer, ByVal Top As Boolean) As List(Of sClassificaItem)

            Dim curr As New List(Of sClassificaItem)

            Try

                Dim prev As New List(Of sClassificaItem)
                Dim topf As New List(Of sClassificaItem)

                'Determino i punti totali della giornata scora
                prev = GetClassificaData(Giornata - 1, Top, False)
                'Determino i punti totali della giornata corrente
                curr = GetClassificaData(Giornata, Top, True)

                CalcoloPreviewPostion(curr, prev)

                'Determino i punti delle topformazioni'
                If Top = False Then
                    topf = GetClassificaData(Giornata, True, False)
                    CalcoloPuntiPersi(curr, topf)
                End If

                CalcoloFantaMister(curr)

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return curr

        End Function

        Private Shared Sub CalcoloPreviewPostion(curr As List(Of sClassificaItem), prev As List(Of sClassificaItem))
            For i As Integer = 0 To curr.Count - 1
                For k As Integer = 0 To prev.Count - 1
                    If prev(k).IdTeam = curr(i).IdTeam Then
                        curr(i).PreviewPostion = prev(k).Postion
                        Exit For
                    End If
                Next
            Next
        End Sub

        Private Shared Sub CalcoloPuntiPersi(curr As List(Of sClassificaItem), topf As List(Of sClassificaItem))
            For i As Integer = 0 To curr.Count - 1
                For k As Integer = 0 To topf.Count - 1
                    If topf(k).IdTeam = curr(i).IdTeam Then
                        curr(i).PuntiPersi = topf(k).Pt - curr(i).Pt
                        curr(i).PercentualePuntiPersi = CInt((curr(i).PuntiPersi / (curr(i).Pt + curr(i).PuntiPersi)) * 1000) / 10
                        Exit For
                    End If
                Next
            Next
        End Sub

        Private Shared Sub CalcoloFantaMister(curr As List(Of sClassificaItem))

            'Calcolo punti fanta mister'
            Dim p As Integer = 0
            Dim oldv As Integer = -10000000
            Dim fm As New Dictionary(Of Integer, Integer)

            curr.OrderByDescending(Function(x) x.DiffQ)

            For i As Integer = 0 To curr.Count - 1
                If oldv <> curr(i).DiffQ Then
                    p = i + 1
                    oldv = curr(i).DiffQ
                End If
                If fm.ContainsKey(curr(i).IdTeam) = False Then fm.Add(curr(i).IdTeam, 0)
                fm(curr(i).IdTeam) = fm(curr(i).IdTeam) + p
            Next

            curr.OrderBy(Function(x) x.NumeroGiocateIn10)

            p = 0
            oldv = -10000000
            For i As Integer = 0 To curr.Count - 1
                If oldv <> curr(i).NumeroGiocateIn10 Then
                    p = i + 1
                    oldv = curr(i).NumeroGiocateIn10
                End If
                If fm.ContainsKey(curr(i).IdTeam) = False Then fm.Add(curr(i).IdTeam, 0)
                fm(curr(i).IdTeam) = fm(curr(i).IdTeam) + p
            Next

            curr.OrderBy(Function(x) x.SumAmmEsp)

            p = 0
            oldv = -10000000
            For i As Integer = 0 To curr.Count - 1
                If oldv <> curr(i).SumAmmEsp Then
                    p = i + 1
                    oldv = curr(i).SumAmmEsp
                End If
                If fm.ContainsKey(curr(i).IdTeam) = False Then fm.Add(curr(i).IdTeam, 0)
                fm(curr(i).IdTeam) = fm(curr(i).IdTeam) + p
            Next

            curr.OrderByDescending(Function(x) x.Pt)

            For i As Integer = 0 To curr.Count - 1
                If fm.ContainsKey(curr(i).IdTeam) Then
                    curr(i).FantaMister = fm(curr(i).IdTeam)
                Else
                    curr(i).FantaMister = 0
                End If
                If curr(i).Postion < 4 Then
                    Select Case curr(i).Postion
                        Case 1 : curr(i).FantaMister = curr(i).FantaMister + 3
                        Case 2 : curr(i).FantaMister = curr(i).FantaMister + 2
                        Case 3 : curr(i).FantaMister = curr(i).FantaMister + 1
                    End Select
                End If
            Next
        End Sub

        Private Shared Function GetClassificaData(ByVal Giornata As Integer, ByVal Top As Boolean, ByVal CalculateMinMax As Boolean) As List(Of sClassificaItem)

            Dim clasa As New List(Of sClassificaItem)
            Dim str As New System.Text.StringBuilder
            Dim pos As Integer = 1
            Dim pt1 As Integer = 0
            Dim ptp As Integer = 0
            Dim tb As String = "tbformazioni"
            Dim tbptmax As String = "ptmax"
            Dim colptmax As String = "maxpt1"
            Dim stot As String = "sum(" & tb & ".pt) as stot"

            If Top Then tb = "" & tb & "top" : tbptmax = tbptmax & "top"

            If PublicVariables.Settings.ConteggiaGoalFattiPerVittoria AndAlso PublicVariables.Settings.ConteggiaGoalSubitiPerVittoria Then
                colptmax = "maxpt4"
                stot = "sum(f.pt+2*(f.gf)-f.gs)"
            ElseIf PublicVariables.Settings.ConteggiaGoalSubitiPerVittoria Then
                colptmax = "maxpt2"
                stot = "sum(f.pt-f.gs) as stot"
            ElseIf PublicVariables.Settings.ConteggiaGoalFattiPerVittoria Then
                colptmax = "maxpt3"
                stot = "sum(f.pt+f.gf) as stot"
            End If

            str.Append("SELECT tb1.idteam,teamq.nome,teamq.allenatore,teamq.diff,SUM(tb1.pt) AS tot,SUM(tb1.ptgio) AS ptgio,")
            str.Append("AVG(tb1.pt) AS avgpt, min(pt) AS minpt, max(pt) AS maxpt,sum(tb1.amm) AS amm,")
            str.Append("sum(tb1.esp) AS esp, sum(tb1.ass) AS ass,sum(tb1.gs) AS gs,")
            str.AppendLine("sum(tb1.gf) AS gf,sum(tb1.vitt) as vitt,sum(tb1.vittgio) as vittgio,sum(11-tb1.numg) as n10,sum(tb1.bonus) as bonus")
            str.AppendLine("FROM (")
            str.Append("SELECT f.idteam,f.gio,")
            str.Append("sum(f.pt) as pt,IIF(f.gio=" & Giornata & ",sum(f.pt),0) as ptgio," & stot & " as stot,sum(f.amm) as amm,")
            str.Append("sum(f.esp) as esp,sum(f.ass) as ass,")
            str.Append("sum(f.gs) as gs,sum(f.gf) as gf,")
            str.Append("sum(incampo) as numg,IIF(" & stot & "=p." & colptmax & ",1,0) as vitt,IIF(" & stot & "=p." & colptmax & " and f.gio=" & Giornata - 1 & ",1,0) as vittgio,")
            str.AppendLine("sum(IIF(f.type>9,f.pt,0)) as bonus")
            str.AppendLine("FROM " & tb & " as f LEFT JOIN " & tbptmax & " as p ON p.gio=f.gio ")
            str.AppendLine("WHERE (incampo=1 OR type=10) and f.pt>-100 and f.gio<=" & Giornata & " ")
            str.AppendLine("GROUP BY f.idteam,f.gio,p." & colptmax)
            str.AppendLine(") AS tb1 ")
            str.AppendLine("INNER JOIN teamq ON teamq.idteam=tb1.idteam ")
            str.Append("GROUP BY tb1.idteam,teamq.nome,teamq.allenatore,teamq.diff ORDER BY sum(tb1.pt) DESC;")

            Dim ds As DataSet = Functions.ExecuteSqlReturnDataSet(str.ToString)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim citem As New sClassificaItem
                    Dim pt As Integer = CInt(ds.Tables(0).Rows(i).Item("tot"))
                    If i = 0 Then
                        pt1 = pt
                        ptp = pt
                    End If
                    If pt <> ptp Then pos = i + 1
                    citem.Postion = pos
                    citem.IdTeam = CInt(ds.Tables(0).Rows(i).Item("idteam"))
                    citem.Nome = ds.Tables(0).Rows(i).Item("nome").ToString()
                    citem.Allenatore = ds.Tables(0).Rows(i).Item("allenatore").ToString()
                    citem.Pt = pt / 10
                    citem.PtGio = CInt(ds.Tables(0).Rows(i).Item("ptgio")) / 10
                    citem.PtFirst = (pt1 - pt) / 10
                    citem.PtPreviews = (ptp - pt) / 10
                    citem.Avg = CInt(ds.Tables(0).Rows(i).Item("avgpt")) / 10
                    citem.Min = CInt(ds.Tables(0).Rows(i).Item("minpt")) / 10
                    citem.Max = CInt(ds.Tables(0).Rows(i).Item("maxpt")) / 10
                    citem.Ammonizioni = CInt(ds.Tables(0).Rows(i).Item("amm"))
                    citem.Espulsioni = CInt(ds.Tables(0).Rows(i).Item("esp"))
                    citem.SumAmmEsp = citem.Ammonizioni + citem.Espulsioni * 2
                    citem.Espulsioni = CInt(ds.Tables(0).Rows(i).Item("esp"))
                    citem.Assist = CInt(ds.Tables(0).Rows(i).Item("ass"))
                    citem.GoalSubiti = CInt(ds.Tables(0).Rows(i).Item("gs"))
                    citem.GoalFatti = CInt(ds.Tables(0).Rows(i).Item("gf"))
                    citem.NumeroGiocateIn10 = CInt(ds.Tables(0).Rows(i).Item("n10"))
                    citem.NbrWinner = CInt(ds.Tables(0).Rows(i).Item("vitt"))
                    citem.WinnerDay = CInt(ds.Tables(0).Rows(i).Item("vittgio"))
                    citem.PtBonus = CInt(ds.Tables(0).Rows(i).Item("bonus")) / 10
                    If ds.Tables(0).Rows(i).Item("diff") IsNot DBNull.Value Then
                        citem.DiffQ = CInt(ds.Tables(0).Rows(i).Item("diff"))
                    Else
                        citem.DiffQ = 0
                    End If
                    clasa.Add(citem)
                    ptp = pt
                Next
            End If
            ds.Dispose()

            Return clasa

        End Function

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

    End Class
End Namespace
