Imports System.Data

Namespace Torneo
    Public Class Players

        Public Shared Function apiGetPlayersQuotes(Ruolo As String) As String

            If PublicVariables.dataFromDatabase Then

                Dim mtxdata As List(Of PlayerQuotesItem) = GetPlayersQuotesData(Ruolo)
                Return WebData.Functions.SerializzaOggetto(mtxdata, True)

            Else

                Dim j As String = IO.File.ReadAllText(WebData.PlayersQuotes.GetDataFileName())
                Dim mtxdata As List(Of PlayerQuotesItem) = WebData.Functions.DeserializeJson(Of List(Of PlayerQuotesItem))(j)

                If Ruolo <> "" Then mtxdata.RemoveAll(Function(x) x.ruolo <> Ruolo)

                Return WebData.Functions.SerializzaOggetto(mtxdata, True)
            End If

        End Function

        Private Shared Function GetPlayersQuotesData(ruolo As String) As List(Of PlayerQuotesItem)
            If ruolo <> "" Then
                Return GetPlayersQuotesData(New List(Of String) From {ruolo})
            Else
                Return GetPlayersQuotesData(New List(Of String))
            End If
        End Function

        Private Shared Function GetPlayersQuotesData(ruoli As List(Of String)) As List(Of PlayerQuotesItem)

            Dim mtxtdata As New List(Of PlayerQuotesItem)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM tbplayer " & If(ruoli.Count > 0, " WHERE ruolo IN ('" & WebData.Functions.ConvertListStringToString(ruoli, "','") & "')", ""))

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim p As New PlayerQuotesItem
                        p.ruolo = row.Item("ruolo").ToString()
                        p.nome = row.Item("nome").ToString()
                        p.squadra = row.Item("squadra").ToString()
                        p.qini = If(row.Item("qini") IsNot DBNull.Value, Convert.ToInt32(row.Item("qini")), 0)
                        p.qcur = If(row.Item("qcur") IsNot DBNull.Value, Convert.ToInt32(row.Item("qcur")), 0)
                        mtxtdata.Add(p)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Shared Sub UpdatePlayersQuotes(newmtxdata As List(Of PlayerQuotesItem))
            Try

                Dim mtxdata As List(Of PlayerQuotesItem) = GetPlayersQuotesData("")
                Dim olddata As New Dictionary(Of String, PlayerQuotesItem)
                Dim newdata As New Dictionary(Of String, PlayerQuotesItem)

                For Each p As PlayerQuotesItem In mtxdata
                    If olddata.ContainsKey(p.nome) = False Then olddata.Add(p.nome, p)
                Next

                For Each p As PlayerQuotesItem In newmtxdata
                    If newdata.ContainsKey(p.nome) = False Then newdata.Add(p.nome, p)
                Next

                Dim sqlinsert As New List(Of String)
                Dim sqlupdate As New List(Of String)

                For Each key In newdata.Keys
                    Dim p As PlayerQuotesItem = newdata(key)
                    If olddata.ContainsKey(key) = False Then
                        sqlinsert.Add("INSERT INTO tbplayer (ruolo,nome,squadra,qini,qcur) values (" & p.ruolo & "," & p.nome & ",'" & p.squadra & "'," & p.qini & "," & p.qcur & ")")
                    ElseIf WebData.Functions.GetCustomHashCode(olddata(key)) <> WebData.Functions.GetCustomHashCode(p) Then
                        sqlupdate.Add("UPDATE tbplayer SET ruolo='" & p.ruolo & "',squadra='" & p.squadra & "',qini=" & p.qini & ",qcur=" & p.qcur & " WHERE nome='" & p.nome & "'")
                    End If
                Next

                Functions.ExecuteSql(sqlinsert)
                Functions.ExecuteSql(sqlupdate)

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Public Shared Function apiGetPlayersData() As String

            If PublicVariables.dataFromDatabase Then
                Dim mtxdata As List(Of PlayerDataItem) = GetPlayersData()
                Return WebData.Functions.SerializzaOggetto(mtxdata, True)
            Else
                Dim j As String = IO.File.ReadAllText(WebData.PlayersData.GetDataFileName())
                Dim mtxdata As List(Of PlayerDataItem) = WebData.Functions.DeserializeJson(Of List(Of PlayerDataItem))(j)
                Return WebData.Functions.SerializzaOggetto(mtxdata, True)
            End If

        End Function

        Private Shared Function GetPlayersData() As List(Of PlayerDataItem)

            Dim mtxtdata As New List(Of PlayerDataItem)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM tbplayer_data")

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim p As New PlayerDataItem
                        p.ruolo = row.Item("ruolo").ToString()
                        p.nome = row.Item("nome").ToString()
                        p.squadra = row.Item("squadra").ToString()
                        p.nation = row.Item("nat").ToString()
                        p.natcode = row.Item("natcode").ToString()
                        mtxtdata.Add(p)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Shared Sub UpdatePlayersData(newmtxdata As List(Of PlayerDataItem))
            Try

                Dim mtxdata As List(Of PlayerDataItem) = GetPlayersData()
                Dim olddata As New Dictionary(Of String, PlayerDataItem)
                Dim newdata As New Dictionary(Of String, PlayerDataItem)

                For Each p As PlayerDataItem In mtxdata
                    If olddata.ContainsKey(p.nome) = False Then olddata.Add(p.nome, p)
                Next

                For Each p As PlayerDataItem In newmtxdata
                    If newdata.ContainsKey(p.nome) = False Then newdata.Add(p.nome, p)
                Next

                Dim sqlinsert As New List(Of String)
                Dim sqlupdate As New List(Of String)

                For Each key In newdata.Keys
                    Dim p As PlayerDataItem = newdata(key)
                    If olddata.ContainsKey(key) = False Then
                        sqlinsert.Add("INSERT INTO tbplayer_data (ruolo,nome,squadra,nat,natcode) values (" & p.ruolo & "," & p.nome & ",'" & p.squadra & "','" & p.nation & "','" & p.natcode & "')")
                    ElseIf WebData.Functions.GetCustomHashCode(olddata(key)) <> WebData.Functions.GetCustomHashCode(p) Then
                        sqlupdate.Add("UPDATE tbplayer_data SET ruolo='" & p.ruolo & "',squadra='" & p.squadra & "',nat='" & p.nation & "',natcode='" & p.natcode & "' WHERE nome='" & p.nome & "'")
                    End If
                Next

                Functions.ExecuteSql(sqlinsert)
                Functions.ExecuteSql(sqlupdate)

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Public Class PlayerQuotesItem
            Public Property ruolo As String = ""
            Public Property nome As String = ""
            Public Property squadra As String = ""
            Public Property qini As Integer = 0
            Public Property qcur As Integer = 0

            Sub New()

            End Sub

            Sub New(ruolo As String, nome As String, squadra As String, qini As Integer, qcur As Integer)
                Me.ruolo = ruolo
                Me.nome = nome
                Me.squadra = squadra
                Me.qini = qini
                Me.qcur = qcur
            End Sub

        End Class

        Public Class PlayerDataItem
            Public Property ruolo As String = ""
            Public Property nome As String = ""
            Public Property squadra As String = ""
            Public Property nation As String = "UNK"
            Public Property natcode As String = "UNK"

            Sub New()

            End Sub

            Sub New(ruolo As String, nome As String, squadra As String, nation As String, natcode As String)
                Me.ruolo = ruolo
                Me.nome = nome
                Me.squadra = squadra
                Me.nation = nation
                Me.natcode = natcode
            End Sub
        End Class
    End Class
End Namespace

