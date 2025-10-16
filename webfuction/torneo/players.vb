Imports System.Data

Namespace Torneo
    Public Class Players

        Public Shared Function ApiGetPlayersName() As String

            Dim mtxdata As List(Of PlayerQuotesItem)

            If PublicVariables.DataFromDatabase Then
                mtxdata = GetPlayersQuotesData("")
            Else
                Dim json As String = IO.File.ReadAllText(WebData.PlayersQuotes.GetDataFileName())
                mtxdata = WebData.Functions.DeserializeJson(Of List(Of PlayerQuotesItem))(json)
            End If
            Dim dicNames As New Dictionary(Of String, List(Of String))
            For Each p As PlayerQuotesItem In mtxdata
                If dicNames.ContainsKey(p.Ruolo) = False Then dicNames.Add(p.Ruolo, New List(Of String))
                dicNames(p.Ruolo).Add(p.Nome)
            Next
            Return WebData.Functions.SerializzaOggetto(dicNames, True)

        End Function

        Public Shared Function ApiGetPlayersQuotes(Ruolo As String) As String

            Dim mtxdata As New List(Of PlayerQuotesItem)

            If PublicVariables.DataFromDatabase Then
                mtxdata = GetPlayersQuotesData("")
            Else
                Dim json As String = IO.File.ReadAllText(WebData.PlayersQuotes.GetDataFileName())
                mtxdata = WebData.Functions.DeserializeJson(Of List(Of PlayerQuotesItem))(json)
                If Ruolo <> "" Then mtxdata.RemoveAll(Function(x) x.Ruolo <> Ruolo)
            End If

            Return WebData.Functions.SerializzaOggetto(mtxdata, True)

        End Function

        Private Shared Function GetPlayersQuotesData(Ruolo As String) As List(Of PlayerQuotesItem)
            If Ruolo <> "" Then
                Return GetPlayersQuotesData(New List(Of String) From {Ruolo})
            Else
                Return GetPlayersQuotesData(New List(Of String))
            End If
        End Function

        Private Shared Function GetPlayersQuotesData(ruoli As List(Of String)) As List(Of PlayerQuotesItem)

            Dim mtxtdata As New List(Of PlayerQuotesItem)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM tbplayer " & If(ruoli.Count > 0, " WHERE Ruolo IN ('" & WebData.Functions.ConvertListStringToString(ruoli, "','") & "')", ""))

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim p As New PlayerQuotesItem
                        p.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        p.Ruolo = Functions.ReadFieldStringData(row.Item("Ruolo").ToString())
                        p.Nome = Functions.ReadFieldStringData(row.Item("Nome").ToString())
                        p.Squadra = Functions.ReadFieldStringData(row.Item("Squadra").ToString())
                        p.Qini = Functions.ReadFieldIntegerData(row.Item("Qini"), 0)
                        p.Qcur = Functions.ReadFieldIntegerData(row.Item("Qcur"), 0)
                        p.OutOfGame = Functions.ReadFieldIntegerData(row.Item("outofgame"), 0)
                        mtxtdata.Add(p)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Shared Sub UpdatePlayersQuotes(newmtxdata As List(Of PlayerQuotesItem))

            Dim ckey As String

            Try

                Dim mtxdata As List(Of PlayerQuotesItem) = GetPlayersQuotesData("")
                Dim olddata As New Dictionary(Of String, PlayerQuotesItem)
                Dim newdata As New Dictionary(Of String, PlayerQuotesItem)

                For Each p As PlayerQuotesItem In mtxdata
                    If olddata.ContainsKey(p.Nome) = False Then olddata.Add(p.Nome, p)
                Next

                For Each p As PlayerQuotesItem In newmtxdata
                    If newdata.ContainsKey(p.Nome) = False Then newdata.Add(p.Nome, p)
                Next

                Dim sqlinsert As New List(Of String)
                Dim sqlupdate As New List(Of String)

                For Each key In newdata.Keys
                    ckey = key
                    Dim p As PlayerQuotesItem = newdata(key)
                    If olddata.ContainsKey(key) = False Then
                        sqlinsert.Add("INSERT INTO tbplayer (ruolo,nome,squadra,qini,qcur, outfogame) values ('" & p.Ruolo & "','" & p.Nome & "','" & p.Squadra & "'," & p.Qini & "," & p.Qcur & "," & p.OutOfGame & ")")
                    Else
                        olddata(key).RecordId = -1
                        If WebData.Functions.GetCustomHashCode(olddata(key)) <> WebData.Functions.GetCustomHashCode(p) Then
                            sqlupdate.Add("UPDATE tbplayer SET ruolo='" & p.Ruolo & "',squadra='" & p.Squadra & "',qini=" & p.Qini & ",qcur=" & p.Qcur & ",outofgame=" & p.OutOfGame & " WHERE Nome='" & p.Nome & "'")
                        End If
                    End If
                Next

                Dim sqldelete As New List(Of String)

                For Each g In olddata.Keys
                    If olddata(g).RecordId <> -1 Then
                        sqldelete.Add("DELETE FROM tbplayer WHERE id=" & olddata(g).RecordId)
                    End If
                Next

                Functions.ExecuteSql(sqlinsert)
                Functions.ExecuteSql(sqlupdate)
                Functions.ExecuteSql(sqldelete)

                'Cancello gli eventuali duplicati'
                Dim sql As String = "DELETE FROM tbplayer WHERE ID NOT IN (SELECT MIN(ID) FROM tbplayer GROUP BY Nome);"
                Functions.ExecuteSql(sql)

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Shared Function ApiGetPlayersData() As String

            If PublicVariables.DataFromDatabase Then
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
                        p.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        p.Ruolo = Functions.ReadFieldStringData(row.Item("Ruolo").ToString())
                        p.Nome = Functions.ReadFieldStringData(row.Item("Nome").ToString())
                        p.Squadra = Functions.ReadFieldStringData(row.Item("Squadra").ToString())
                        p.Nazione = Functions.ReadFieldStringData(row.Item("nat").ToString())
                        p.NatCode = Functions.ReadFieldStringData(row.Item("NatCode").ToString())
                        mtxtdata.Add(p)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Shared Sub UpdatePlayersData(newmtxdata As List(Of PlayerDataItem))
            Try

                Dim mtxdata As List(Of PlayerDataItem) = GetPlayersData()
                Dim olddata As New Dictionary(Of String, PlayerDataItem)
                Dim newdata As New Dictionary(Of String, PlayerDataItem)

                For Each p As PlayerDataItem In mtxdata
                    If olddata.ContainsKey(p.Nome) = False Then olddata.Add(p.Nome, p)
                Next

                For Each p As PlayerDataItem In newmtxdata
                    If newdata.ContainsKey(p.Nome) = False Then newdata.Add(p.Nome, p)
                Next

                Dim sqlinsert As New List(Of String)
                Dim sqlupdate As New List(Of String)

                For Each key In newdata.Keys
                    Dim p As PlayerDataItem = newdata(key)
                    If olddata.ContainsKey(key) = False Then
                        sqlinsert.Add("INSERT INTO tbplayer_data (Ruolo,Nome,Squadra,nat,NatCode) values ('" & p.Ruolo & "','" & p.Nome & "','" & p.Squadra & "','" & p.Nazione & "','" & p.NatCode & "')")
                    Else
                        olddata(key).RecordId = -1
                        If WebData.Functions.GetCustomHashCode(olddata(key)) <> WebData.Functions.GetCustomHashCode(p) Then
                            sqlupdate.Add("UPDATE tbplayer_data SET Ruolo='" & p.Ruolo & "',Squadra='" & p.Squadra & "',nat='" & p.Nazione & "',NatCode='" & p.NatCode & "' WHERE Nome='" & p.Nome & "'")
                        End If
                    End If
                Next

                Dim sqldelete As New List(Of String)

                For Each g In olddata.Keys
                    If olddata(g).RecordId <> -1 Then
                        sqldelete.Add("DELETE FROM tbplayer WHERE id=" & olddata(g).RecordId)
                    End If
                Next

                Functions.ExecuteSql(sqlinsert)
                Functions.ExecuteSql(sqlupdate)
                Functions.ExecuteSql(sqldelete)

                'Cancello gli eventuali duplicati'
                Dim sql As String = "DELETE FROM tbplayer_data WHERE ID NOT IN (SELECT MIN(ID) FROM tbplayer_data GROUP BY Nome);"
                Functions.ExecuteSql(sql)

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Class PlayerQuotesItem
            Public Property RecordId As Integer = 0
            Public Property Ruolo As String = ""
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Qini As Integer = 0
            Public Property Qcur As Integer = 0
            Public Property OutOfGame As Integer = 0

            Sub New()

            End Sub

            Sub New(Ruolo As String, Nome As String, Squadra As String, Qini As Integer, Qcur As Integer, OutOfGame As Integer)
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
                Me.Qini = Qini
                Me.Qcur = Qcur
                Me.OutOfGame = OutOfGame
            End Sub

        End Class

        Public Class PlayerDataItem
            Public Property RecordId As Integer = 0
            Public Property Ruolo As String = ""
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Nazione As String = "UNK"
            Public Property NatCode As String = "UNK"

            Sub New()

            End Sub

            Sub New(Ruolo As String, Nome As String, Squadra As String, Nazione As String, NatCode As String)
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
                Me.Nazione = Nazione
                Me.NatCode = NatCode
            End Sub
        End Class
    End Class
End Namespace

