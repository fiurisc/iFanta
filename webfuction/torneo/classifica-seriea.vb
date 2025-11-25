

Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports System.Runtime.InteropServices.ComTypes
Imports webfuction.Torneo.MatchsData

Namespace Torneo
    Public Class ClassificaSerieA

        Dim appSett As New PublicVariables

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function ApiGetData(giornata As String) As String

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Richiedo la classifica della serie a per giornata: " & giornata & " dell'anno: " & appSett.Year)
            If giornata = "-1" Then giornata = GetLastDay().ToString()
            Dim mtxdata As List(Of ClassificaItem) = GetClassificaData(CInt(giornata))

            Return WebData.Functions.SerializzaOggetto(mtxdata, True)

        End Function

        Public Sub UpdateClassificaData(giornata As Integer, newdata As List(Of ClassificaItem))
            Try

                Dim mtxtdata As List(Of ClassificaItem) = GetClassificaData(giornata)
                Dim olddata As New Dictionary(Of String, ClassificaItem)
                Dim sqlinsert As New List(Of String)

                For Each data As ClassificaItem In mtxtdata
                    Dim c1 As ClassificaItem = CType(data.Clone(), ClassificaItem)
                    c1.RecordId = 1
                    Dim skey As String = WebData.Functions.SerializzaOggetto(c1, True)
                    If olddata.ContainsKey(skey) = False Then
                        olddata.Add(skey, data)
                    End If
                Next

                For Each dat As ClassificaItem In newdata
                    Dim skey As String = WebData.Functions.SerializzaOggetto(dat, True)
                    If olddata.ContainsKey(skey) = False Then
                        Dim sb As New System.Text.StringBuilder
                        sb.Append("INSERT INTO tbrank (gio,pos,squadra,pt, ptd, ptf, pgio, pgiod, pgiof, vitt, vittd, vittf, par, pard, parf, sco, scod, scof, gs, gsd, gsf, gf, gfd, gff) values (")
                        sb.Append(giornata & "," & dat.Pos & ",'" & dat.Squadra & "'")
                        sb.Append("," & dat.Punti.Totali & "," & dat.Punti.Dentro & "," & dat.Punti.Fuori)
                        sb.Append("," & dat.PartiteGiocate.Totali & "," & dat.PartiteGiocate.Dentro & "," & dat.PartiteGiocate.Fuori)
                        sb.Append("," & dat.Vittorie.Totali & "," & dat.Vittorie.Dentro & "," & dat.Vittorie.Fuori)
                        sb.Append("," & dat.Pareggi.Totali & "," & dat.Pareggi.Dentro & "," & dat.Pareggi.Fuori)
                        sb.Append("," & dat.Sconfitte.Totali & "," & dat.Sconfitte.Dentro & "," & dat.Sconfitte.Fuori)
                        sb.Append("," & dat.GoalSubiti.Totali & "," & dat.GoalSubiti.Dentro & "," & dat.GoalSubiti.Fuori)
                        sb.Append("," & dat.GoalFatti.Totali & "," & dat.GoalFatti.Dentro & "," & dat.GoalFatti.Fuori & ")")
                        sqlinsert.Add(sb.ToString())
                    Else
                        olddata(skey).RecordId = -1
                    End If
                Next

                Dim sqldelete As New List(Of String)

                For Each k In olddata.Keys
                    If olddata(k).RecordId <> -1 Then
                        sqldelete.Add("DELETE FROM tbrank WHERE id=" & olddata(k).RecordId)
                    End If
                Next

                Functions.ExecuteSql(appSett, sqlinsert)
                Functions.ExecuteSql(appSett, sqldelete)

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Function GetLastDay() As Integer

            Dim lastDay = 1
            Try

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT max(gio) as lastGio FROM tbrank")

                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                    lastDay = Functions.ReadFieldIntegerData(ds.Tables(0).Rows(0)("lastGio"), 1)
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return lastDay

        End Function

        Private Function GetClassificaData(giornata As Integer) As List(Of ClassificaItem)

            Dim mtxtdata As New List(Of ClassificaItem)

            Try

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM tbrank WHERE gio = " & giornata)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As System.Data.DataRow = ds.Tables(0).Rows(i)
                        Dim m As New ClassificaItem
                        m.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        m.Giornata = Functions.ReadFieldIntegerData(row.Item("gio"), 1)
                        m.Pos = Functions.ReadFieldIntegerData(row.Item("pos"), 0)
                        m.Squadra = Functions.ReadFieldStringData(row.Item("squadra").ToString())
                        m.Punti.Totali = Functions.ReadFieldIntegerData(row.Item("pt"), 0)
                        m.Punti.Dentro = Functions.ReadFieldIntegerData(row.Item("ptd"), 0)
                        m.Punti.Fuori = Functions.ReadFieldIntegerData(row.Item("ptf"), 0)
                        m.PartiteGiocate.Totali = Functions.ReadFieldIntegerData(row.Item("pgio"), 0)
                        m.PartiteGiocate.Dentro = Functions.ReadFieldIntegerData(row.Item("pgiod"), 0)
                        m.PartiteGiocate.Fuori = Functions.ReadFieldIntegerData(row.Item("pgiof"), 0)
                        m.Vittorie.Totali = Functions.ReadFieldIntegerData(row.Item("vitt"), 0)
                        m.Vittorie.Dentro = Functions.ReadFieldIntegerData(row.Item("vittd"), 0)
                        m.Vittorie.Fuori = Functions.ReadFieldIntegerData(row.Item("vittf"), 0)
                        m.Pareggi.Totali = Functions.ReadFieldIntegerData(row.Item("par"), 0)
                        m.Pareggi.Dentro = Functions.ReadFieldIntegerData(row.Item("pard"), 0)
                        m.Pareggi.Fuori = Functions.ReadFieldIntegerData(row.Item("parf"), 0)
                        m.Sconfitte.Totali = Functions.ReadFieldIntegerData(row.Item("sco"), 0)
                        m.Sconfitte.Dentro = Functions.ReadFieldIntegerData(row.Item("scod"), 0)
                        m.Sconfitte.Fuori = Functions.ReadFieldIntegerData(row.Item("scof"), 0)
                        m.GoalSubiti.Totali = Functions.ReadFieldIntegerData(row.Item("gs"), 0)
                        m.GoalSubiti.Dentro = Functions.ReadFieldIntegerData(row.Item("gsd"), 0)
                        m.GoalSubiti.Fuori = Functions.ReadFieldIntegerData(row.Item("gsf"), 0)
                        m.GoalFatti.Totali = Functions.ReadFieldIntegerData(row.Item("gf"), 0)
                        m.GoalFatti.Dentro = Functions.ReadFieldIntegerData(row.Item("gfd"), 0)
                        m.GoalFatti.Fuori = Functions.ReadFieldIntegerData(row.Item("gff"), 0)
                        mtxtdata.Add(m)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Class ClassificaItem

            Implements ICloneable

            Public Property RecordId As Integer = 1
            Public Property Giornata As Integer = 1
            Public Property Pos As Integer = 1
            Public Property Squadra As String = ""
            Public Property Punti As New SubItem
            Public Property PartiteGiocate As New SubItem
            Public Property Vittorie As New SubItem
            Public Property Pareggi As New SubItem
            Public Property Sconfitte As New SubItem
            Public Property GoalFatti As New SubItem
            Public Property GoalSubiti As New SubItem

            Public Function Clone() As Object Implements ICloneable.Clone
                Return Me.MemberwiseClone()
            End Function

            Public Class SubItem
                Public Property Totali As Integer = 0
                Public Property Dentro As Integer = 0
                Public Property Fuori As Integer = 0
            End Class
        End Class

    End Class
End Namespace