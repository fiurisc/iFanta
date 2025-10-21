Imports System.Data

Namespace Torneo
    Public Class ClassificaData

        Public Shared fname1 As String = PublicVariables.DataPath & "export\classifica.json"
        Public Shared fname2 As String = PublicVariables.DataPath & "export\classifica-top.json"

        Public Shared Function ApiGetLastDay() As String

            Dim cday As String = "1"

            Try
                If PublicVariables.DataFromDatabase Then
                    Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT MAX(gio) AS currgio FROM tbformazioni")
                    If ds.Tables.Count > 0 Then
                        cday = ds.Tables(0).Rows(0).Item("currgio").ToString()
                    End If
                Else
                    Dim j As String = IO.File.ReadAllText(fname1)
                    Dim dicdata As Dictionary(Of String, List(Of Classifica)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, List(Of Classifica)))(j)
                    cday = dicdata.Keys.Last()
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return cday

        End Function

        Public Shared Function ApiGetClassifica(day As String, top As Boolean) As String

            Try
                If PublicVariables.DataFromDatabase Then
                    Return WebData.Functions.SerializzaOggetto(GetClassificaGiornata(CInt(day), top), True)
                Else
                    Dim j As String = IO.File.ReadAllText(If(top, fname2, fname1))
                    Dim dicdata As Dictionary(Of String, List(Of Classifica)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, List(Of Classifica)))(j)
                    If dicdata.ContainsKey(day) Then
                        Return WebData.Functions.SerializzaOggetto(dicdata(day), True)
                    End If
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return ""

        End Function

        Shared Function GetClassificaGiornata(ByVal Giornata As Integer, ByVal Top As Boolean) As List(Of Classifica)

            Dim curr As New List(Of Classifica)

            Try

                Dim prev As New List(Of Classifica)
                Dim topf As New List(Of Classifica)

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
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return curr

        End Function

        Private Shared Sub CalcoloPreviewPostion(curr As List(Of Classifica), prev As List(Of Classifica))
            For i As Integer = 0 To curr.Count - 1
                For k As Integer = 0 To prev.Count - 1
                    If prev(k).IdTeam = curr(i).IdTeam Then
                        curr(i).PreviewPostion = prev(k).Postion
                        Exit For
                    End If
                Next
            Next
        End Sub

        Private Shared Sub CalcoloPuntiPersi(curr As List(Of Classifica), topf As List(Of Classifica))
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

        Private Shared Sub CalcoloFantaMister(curr As List(Of Classifica))

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

        Private Shared Function GetClassificaData(ByVal Giornata As Integer, ByVal Top As Boolean, ByVal CalculateMinMax As Boolean) As List(Of Classifica)

            Dim clasa As New List(Of Classifica)
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
                stot = "sum(f.pt)+2*sum(f.gf)-sum(f.gs)"
            ElseIf PublicVariables.Settings.ConteggiaGoalSubitiPerVittoria Then
                colptmax = "maxpt2"
                stot = "sum(f.pt)-sum(f.gs)"
            ElseIf PublicVariables.Settings.ConteggiaGoalFattiPerVittoria Then
                colptmax = "maxpt3"
                stot = "sum(f.pt)+sum(f.gf)"
            End If

            str.AppendLine("SELECT tb.*,teamq.Nome,teamq.allenatore,teamq.diff FROM (")
            str.AppendLine("SELECT tb1.idteam,SUM(tb1.pt) AS tot,SUM(tb1.ptgio) AS ptgio,SUM(tb1.stot) AS stot,AVG(tb1.pt) AS avgpt, min(pt) AS minpt, max(pt) AS maxpt,sum(tb1.amm) AS amm,sum(tb1.esp) AS esp, sum(tb1.ass) AS ass,sum(tb1.gs) AS gs,sum(tb1.gf) AS gf,sum(tb1.vitt) as vitt,iif(sum(tb1.maxpt) =SUM(tb1.stot),1,0) as vittgio,sum(11-tb1.numg) as n10,sum(tb1.bonus) as bonus FROM (")
            str.AppendLine("SELECT f.idteam,f.gio,sum(f.pt) as pt,IIF(f.gio=" & Giornata & ",sum(f.pt),0) as ptgio,IIF(f.gio=" & Giornata & "," & stot & ",0) as stot,sum(f.amm) as amm,sum(f.esp) as esp,sum(f.ass) as ass,sum(f.gs) as gs,sum(f.gf) as gf,sum(incampo) as numg,IIF(" & stot & "=p." & colptmax & ",1,0) as vitt,IIF(f.gio=" & Giornata & ",p." & colptmax & ",0) as maxpt,sum(IIF(f.type>9,f.pt,0)) as bonus")
            str.AppendLine("FROM tbformazioni as f LEFT JOIN " & tbptmax & " as p ON p.gio=f.gio ")
            str.AppendLine("WHERE (incampo=1 OR type=10) and f.pt>-100 and f.gio<=" & Giornata)
            str.AppendLine("GROUP BY f.idteam,f.gio,p.maxpt4")
            str.AppendLine(") AS tb1")
            str.AppendLine("GROUP BY tb1.idteam) as tb")
            str.AppendLine("LEFT JOIN teamq ON teamq.idteam=tb.idteam")
            str.AppendLine("ORDER by tb.tot DESC")

            Dim ds As DataSet = Functions.ExecuteSqlReturnDataSet(str.ToString)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim citem As New Classifica
                    Dim pt As Integer = CInt(ds.Tables(0).Rows(i).Item("tot"))
                    If i = 0 Then
                        pt1 = pt
                        ptp = pt
                    End If
                    If pt <> ptp Then pos = i + 1
                    citem.Postion = pos
                    citem.IdTeam = CInt(ds.Tables(0).Rows(i).Item("idteam"))
                    citem.Nome = ds.Tables(0).Rows(i).Item("Nome").ToString()
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

        Public Class Classifica

            Sub New()

            End Sub

            Public Property Postion() As Integer = 0
            Public Property PreviewPostion() As Integer = 0
            Public Property IdTeam() As Integer = 0
            Public Property Nome() As String = ""
            Public Property Allenatore() As String = ""
            Public Property Pt() As Double = 0
            Public Property PtGio() As Double = 0
            Public Property PtFirst() As Double = 0
            Public Property PtPreviews() As Double = 0
            Public Property Avg() As Double = 0
            Public Property Min() As Double = 0
            Public Property Max() As Double = 0
            Public Property Ammonizioni() As Integer = 0
            Public Property Espulsioni() As Integer = 0
            Public Property SumAmmEsp() As Integer = 0
            Public Property Assist() As Integer = 0
            Public Property GoalSubiti() As Integer = 0
            Public Property GoalFatti() As Integer = 0
            Public Property PuntiPersi() As Double = 0
            Public Property PercentualePuntiPersi() As Double = 0
            Public Property NumeroGiocateIn10() As Integer = 0
            Public Property NbrWinner() As Integer = 0
            Public Property WinnerDay() As Integer = 0
            Public Property PtBonus() As Double = 0
            Public Property DiffQ() As Integer = 0
            Public Property FantaMister() As Integer = 0

        End Class

    End Class
End Namespace
