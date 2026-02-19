Imports System.Data

Namespace Torneo
    Public Class Players

        Dim appSett As PublicVariables

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function ApiGetPlayersName() As String

            Dim mtxdata As List(Of PlayerQuotesItem) = GetPlayersQuotesData("")
            Dim dicNames As New Dictionary(Of String, List(Of String))
            For Each p As PlayerQuotesItem In mtxdata
                If dicNames.ContainsKey(p.Ruolo) = False Then dicNames.Add(p.Ruolo, New List(Of String))
                dicNames(p.Ruolo).Add(p.Nome)
            Next
            Return WebData.Functions.SerializzaOggetto(dicNames, True)

        End Function

        Public Function ApiGetPlayersQuotes(Ruolo As String) As String
            Dim mtxdata As List(Of PlayerQuotesItem) = GetPlayersQuotesData(Ruolo)
            Return WebData.Functions.SerializzaOggetto(mtxdata, True)
        End Function

        Public Function GetPlayersQuotesData(Ruolo As String) As List(Of PlayerQuotesItem)
            If Ruolo <> "" Then
                Return GetPlayersQuotesData(New List(Of String) From {Ruolo})
            Else
                Return GetPlayersQuotesData(New List(Of String))
            End If
        End Function

        Private Function GetPlayersQuotesData(ruoli As List(Of String)) As List(Of PlayerQuotesItem)

            Dim mtxtdata As New List(Of PlayerQuotesItem)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM tbplayer " & If(ruoli.Count > 0, " WHERE Ruolo IN ('" & WebData.Functions.ConvertListStringToString(ruoli, "','") & "')", ""))

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim p As New PlayerQuotesItem
                        p.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        p.Ruolo = Functions.ReadFieldStringData(row.Item("Ruolo").ToString())
                        p.RuoloMantra = Functions.ReadFieldStringData(row.Item("Ruolomantra").ToString())
                        p.RuoloMantraS = Functions.ReadFieldStringData(row.Item("Ruolomantras").ToString())
                        p.Nome = Functions.ReadFieldStringData(row.Item("Nome").ToString())
                        p.Squadra = Functions.ReadFieldStringData(row.Item("Squadra").ToString())
                        p.Qini = Functions.ReadFieldIntegerData(row.Item("Qini"), 0)
                        p.Qcur = Functions.ReadFieldIntegerData(row.Item("Qcur"), 0)
                        p.OutOfGame = Functions.ReadFieldIntegerData(row.Item("outofgame"), 0)
                        mtxtdata.Add(p)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Sub UpdatePlayersQuotes(newmtxdata As List(Of PlayerQuotesItem))

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
                    If p.Ruolo = "A" Then
                        p.RuoloMantraS = "A"
                    ElseIf p.Ruolo = "C" Then
                        If p.RuoloMantra.Contains("T") Then
                            p.RuoloMantraS = "T"
                        ElseIf p.RuoloMantra.Contains("W") Then
                            p.RuoloMantraS = "W"
                        Else
                            p.RuoloMantraS = "C"
                        End If
                    ElseIf p.Ruolo = "D" Then
                        If p.RuoloMantra.Contains("E") Then
                            p.RuoloMantraS = "E"
                        Else
                            p.RuoloMantraS = "DC"
                        End If
                    End If

                    If olddata.ContainsKey(key) = False Then
                        sqlinsert.Add("INSERT INTO tbplayer (ruolo,ruolomantra,ruolomantraa,nome,squadra,qini,qcur, outofgame) values ('" & p.Ruolo & "','" & p.RuoloMantra & "','" & p.RuoloMantraS & "','" & p.Nome & "','" & p.Squadra & "'," & p.Qini & "," & p.Qcur & "," & p.OutOfGame & ")")
                    Else
                        olddata(key).RecordId = -1
                        If WebData.Functions.GetCustomHashCode(olddata(key)) <> WebData.Functions.GetCustomHashCode(p) Then
                            sqlupdate.Add("UPDATE tbplayer SET ruolo='" & p.Ruolo & "',ruolomantra='" & p.RuoloMantra & "',ruolomantras='" & p.RuoloMantraS & "',squadra='" & p.Squadra & "',qini=" & p.Qini & ",qcur=" & p.Qcur & ",outofgame=" & p.OutOfGame & " WHERE Nome='" & p.Nome & "'")
                        End If
                    End If
                Next

                Dim sqldelete As New List(Of String)

                For Each g In olddata.Keys
                    If olddata(g).RecordId <> -1 Then
                        sqldelete.Add("DELETE FROM tbplayer WHERE id=" & olddata(g).RecordId)
                    End If
                Next

                Functions.ExecuteSql(appSett, sqlinsert)
                Functions.ExecuteSql(appSett, sqlupdate)
                Functions.ExecuteSql(appSett, sqldelete)

                'Cancello gli eventuali duplicati'
                Dim sql As String = "DELETE FROM tbplayer WHERE ID NOT IN (SELECT MIN(ID) FROM tbplayer GROUP BY Nome);"
                Functions.ExecuteSql(appSett, sql)

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Function ApiGetPlayersData() As String
            Dim mtxdata As List(Of PlayerDataItem) = GetPlayersData()
            Return WebData.Functions.SerializzaOggetto(mtxdata, True)
        End Function

        Public Function GetPlayersData() As List(Of PlayerDataItem)

            Dim mtxtdata As New List(Of PlayerDataItem)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM tbplayer_data")

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim p As New PlayerDataItem
                        p.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        p.Ruolo = Functions.ReadFieldStringData(row.Item("ruolo").ToString())
                        p.Nome = Functions.ReadFieldStringData(row.Item("nome").ToString())
                        p.Squadra = Functions.ReadFieldStringData(row.Item("squadra").ToString())
                        p.Nazione = Functions.ReadFieldStringData(row.Item("nat").ToString())
                        p.NatCode = Functions.ReadFieldStringData(row.Item("natcode").ToString())
                        p.Anni = Functions.ReadFieldIntegerData(row.Item("anni").ToString())
                        p.Compleanno = Functions.ReadFieldStringData(row.Item("birthday").ToString())
                        p.Altezza = Functions.ReadFieldStringData(row.Item("altezza").ToString())
                        p.Peso = Functions.ReadFieldStringData(row.Item("peso").ToString())
                        mtxtdata.Add(p)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Sub UpdatePlayersData(newmtxdata As List(Of PlayerDataItem))
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
                        sqlinsert.Add("INSERT INTO tbplayer_data (ruolo, nome, squadra, nat, natcode, anni, birthday, altezza, peso) values ('" & p.Ruolo & "','" & p.Nome & "','" & p.Squadra & "','" & p.Nazione & "','" & p.NatCode & "'," & p.Anni & ",'" & p.Compleanno & "','" & p.Altezza & "','" & p.Peso & "')")
                    Else
                        olddata(key).RecordId = -1
                        If WebData.Functions.GetCustomHashCode(olddata(key)) <> WebData.Functions.GetCustomHashCode(p) Then
                            sqlupdate.Add("UPDATE tbplayer_data SET ruolo='" & p.Ruolo & "',squadra='" & p.Squadra & "',nat='" & p.Nazione.Replace("'", "''") & "',natcode='" & p.NatCode & "',anni=" & p.Anni & ",birthday='" & p.Compleanno & "',altezza='" & p.Altezza & "',peso='" & p.Peso & "' WHERE nome='" & p.Nome & "'")
                        End If
                    End If
                Next

                Dim sqldelete As New List(Of String)

                For Each g In olddata.Keys
                    If olddata(g).RecordId <> -1 Then
                        sqldelete.Add("DELETE FROM tbplayer WHERE id=" & olddata(g).RecordId)
                    End If
                Next

                Functions.ExecuteSql(appSett, sqlinsert)
                Functions.ExecuteSql(appSett, sqlupdate)
                Functions.ExecuteSql(appSett, sqldelete)

                'Cancello gli eventuali duplicati'
                Dim sql As String = "DELETE FROM tbplayer_data WHERE ID NOT IN (SELECT MIN(ID) FROM tbplayer_data GROUP BY Nome);"
                Functions.ExecuteSql(appSett, sql)

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Class PlayerQuotesItem
            Public Property RecordId As Integer = 0
            Public Property Ruolo As String = ""
            Public Property RuoloMantra As String = ""
            Public Property RuoloMantraS As String = ""
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Qini As Integer = 0
            Public Property Qcur As Integer = 0
            Public Property OutOfGame As Integer = 0

            Sub New()

            End Sub

            Sub New(Ruolo As String, RuoloMantra As String, Nome As String, Squadra As String, Qini As Integer, Qcur As Integer, OutOfGame As Integer)
                Me.Ruolo = Ruolo
                Me.RuoloMantra = RuoloMantra
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
            Public Property Anni As Integer = 0
            Public Property Compleanno As String = ""
            Public Property Altezza As String = ""
            Public Property Peso As String = ""

            Sub New()

            End Sub

            Sub New(Ruolo As String, Nome As String, Squadra As String, Nazione As String, NatCode As String, Anni As Integer, Compleanno As String, Altezza As String, Peso As String)
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
                Me.Nazione = Nazione
                Me.NatCode = NatCode
                Me.Anni = Anni
                Me.Compleanno = Compleanno
                Me.Altezza = Altezza
                Me.Peso = Peso
            End Sub
        End Class
    End Class
End Namespace

