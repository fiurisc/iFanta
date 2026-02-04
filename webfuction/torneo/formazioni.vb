Imports System.Data
Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports System.Runtime.Remoting
Imports webfuction.Torneo.ProbablePlayers

Namespace Torneo

    Public Class FormazioniData

        Dim appSett As New PublicVariables

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function ApiAddFormazioni(Day As String, TeamId As String, Top As Boolean, json As String) As String

            If json = "" Then Throw New Exception("Json not valid")

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Inserimento formazione giornata: " & Day & " per il team: " & TeamId & " top: " & Top.ToString())

            Dim tb As String = If(Top, "tbformazionitop", "tbformazioni")
            Dim mData As MetaData = WebData.Functions.DeserializeJson(Of MetaData)(json)

            If mData.teamId <> TeamId Then Throw New Exception("Json not related to right teamid")

            If mData IsNot Nothing AndAlso mData.data.Count > 0 Then
                SaveFormazioni(CInt(Day), mData.data, False)
            End If

            Return ""

        End Function

        Public Sub ApiDeleteFormazioni(Day As String, TeamId As String, Top As Boolean)
            DeleteFormazioni(Day, TeamId, If(Top, "tbformazionitop", "tbformazioni"))
        End Sub

        Public Sub DeleteFormazioni(Day As String, TeamId As String, Table As String)
            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Cancellazione formazione giornata: " & Day & " per il team: " & TeamId & " table: " & Table)
            Functions.ExecuteSql(appSett, "DELETE FROM " & Table & " WHERE gio=" & Day & If(TeamId <> "-1", " AND idteam=" & TeamId, ""))
        End Sub

        Public Function ApiGetFormazione(Day As String, TeamId As String, Top As Boolean) As String

            Dim json As String = ""

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Richiesta formazione giornata: " & Day & " per il team: " & TeamId & " top: " & Top.ToString())

            Try
                Dim list As List(Of Formazione) = GetFormazioni(Day, TeamId, Top)
                If list.Count > 0 Then
                    Return WebData.Functions.SerializzaOggetto(list(0), True)
                Else
                    Return "{}"
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return json

        End Function

        Public Function ApiGetFormazioni(Day As String, TeamId As String, Top As Boolean) As String

            Dim json As String = ""

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Richiesta formazioni giornata: " & Day & " per il team: " & TeamId & " top: " & Top.ToString())

            Try
                Dim list As List(Of Formazione) = GetFormazioni(Day, TeamId, Top)
                Dim dicForma As Dictionary(Of String, Formazione) = list.ToDictionary(Function(x) x.TeamId.ToString(), Function(x) x)
                Return WebData.Functions.SerializzaOggetto(dicForma, True)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return json

        End Function

        Public Sub BackupFormazione(day As String, data As MetaData)
            Try

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Sub SaveFormazioni(day As Integer, lst As List(Of Formazione), top As Boolean)
            Dim tb As String = If(top, "tbformazionitop", "tbformazioni")
            SaveFormazioni(day, lst, tb)
        End Sub

        Public Sub SaveFormazioni(day As Integer, lst As List(Of Formazione), Table As String)

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Salvataggio formazioni: " & day & " table: " & Table)

            For Each forma As Formazione In lst

                Dim sqlinsert As New List(Of String)

                If forma.BonusDifesa > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & Table & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",10," & forma.BonusDifesa & ")")
                    sqlinsert.Add(sqlp.ToString())
                End If

                If forma.BonusCentrocampo > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & Table & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",20," & forma.BonusDifesa & ")")
                    sqlinsert.Add(sqlp.ToString())
                End If

                If forma.BonusAttacco > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & Table & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",30," & forma.BonusDifesa & ")")
                    sqlinsert.Add(sqlp.ToString())
                End If

                If forma.CambioModulo > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & Table & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",40,1)")
                    sqlinsert.Add(sqlp.ToString())
                End If

                For Each p As PlayerFormazione In forma.Players
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & Table & " (gio,idteam,idrosa,jolly,type,idformazione,incampo,ruolo,nome,squadra,vote,amm,esp,ass,autogol,gs,gf,rigs,rigp,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & "," & p.RosaId & "," & p.Jolly & "," & p.Type & "," & p.FormaId & "," & p.InCampo & ",'" & p.Ruolo & "',")
                    sqlp.AppendLine("'" & p.Nome.ToUpper() & "','" & p.Squadra.ToUpper() & "'," & p.Voto & "," & p.Ammonito & "," & p.Espulso & "," & p.Assists & "," & p.AutoGoal & ",")
                    sqlp.AppendLine(p.GoalSubiti & "," & p.GoalFatti & "," & p.RigoriSbagliati & "," & p.RigoriParati & "," & p.Punti & ")")
                    sqlinsert.Add(sqlp.ToString())
                Next

                Functions.ExecuteSql(appSett, "DELETE FROM " & Table & " WHERE gio=" & day + 1000 & " AND idteam=" & forma.TeamId)
                Functions.ExecuteSql(appSett, sqlinsert)
                DeleteFormazioni(day.ToString(), forma.TeamId.ToString(), Table)
                Functions.ExecuteSql(appSett, "UPDATE " & Table & " SET gio=gio-1000 WHERE gio=" & day + 1000 & " AND idteam=" & forma.TeamId)

            Next

        End Sub

        Public Function GetFormazione(Day As String, TeamId As String, Top As Boolean) As Formazione
            Dim list As List(Of Formazione) = GetFormazioniFromDb(Day, TeamId, If(Top, "formazioni_top", "formazioni"))
            If list.Count > 0 Then
                Return list(0)
            Else
                Return New Formazione
            End If
        End Function

        Public Function GetFormazioni(Day As String, TeamId As String, Top As Boolean) As List(Of Formazione)
            Return GetFormazioni(Day, TeamId, If(Top, "formazioni_top", "formazioni"))
        End Function

        Public Function GetFormazioni(Day As String, TeamId As String, Table As String) As List(Of Formazione)

            Dim list As List(Of Formazione) = GetFormazioniFromDb(Day, TeamId, Table)

            For Each forma As Formazione In list
                CalculatePuntiFormazione(forma)
            Next

            Return list

        End Function

        Public Sub CalculatePuntiFormazione(forma As Formazione)

            'Determino il modulo'

            forma.Punti = 0
            forma.Modulo.Difensori = 0
            forma.Modulo.Centrocampisti = 0
            forma.Modulo.Attaccanti = 0
            forma.PlayersInCampo = 0

            For Each p As PlayerFormazione In forma.Players
                forma.PlayersInCampo += p.InCampo
                If p.InCampo = 1 AndAlso p.Punti > -100 Then forma.Punti += p.Punti
                If p.Type = 1 Then
                    Select Case p.Ruolo
                        Case "D" : forma.Modulo.Difensori += 1
                        Case "C" : forma.Modulo.Centrocampisti += 1
                        Case "A" : forma.Modulo.Attaccanti += 1
                    End Select
                End If
            Next

            forma.Modulo.Display = forma.Modulo.Difensori & "-" & forma.Modulo.Centrocampisti & "-" & forma.Modulo.Attaccanti

            'Aggiungo punti provenieni dai bonus
            forma.Punti += forma.BonusDifesa
            forma.Punti += forma.BonusCentrocampo
            forma.Punti += forma.BonusAttacco

        End Sub

        Private Function GetFormazioniFromDb(Day As String, TeamId As String, Table As String) As List(Of Formazione)

            Dim list As New Dictionary(Of Integer, Formazione)

            Try

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM " & Table & " WHERE gio=" & Day & If(TeamId <> "-1", " AND idteam = " & TeamId, "") & " ORDER BY idteam,idformazione")

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim type As Integer = Functions.ReadFieldIntegerData("type", row, 0)
                        Dim tid As Integer = Functions.ReadFieldIntegerData("idteam", row, 0)

                        If list.ContainsKey(tid) = False Then list.Add(tid, New Formazione())

                        list(tid).Giornata = CInt(Day)
                        list(tid).TeamId = tid

                        If type < 10 Then
                            Dim p As New PlayerFormazione
                            p.RosaId = Functions.ReadFieldIntegerData("idrosa", row, 0)
                            p.Jolly = Functions.ReadFieldIntegerData("jolly", row, 0)
                            p.Type = type
                            p.FormaId = Functions.ReadFieldIntegerData("idformazione", row, 0)
                            p.InCampo = Functions.ReadFieldIntegerData("incampo", row, 0)
                            p.Ruolo = Functions.ReadFieldStringData("ruolo", row, "")
                            p.Nome = Functions.ReadFieldStringData("nome", row, "")
                            p.Squadra = Functions.ReadFieldStringData("squadra", row, "")
                            p.Voto = Functions.ReadFieldIntegerData("vote", row, 0)
                            p.Ammonito = Functions.ReadFieldIntegerData("amm", row, 0)
                            p.Espulso = Functions.ReadFieldIntegerData("esp", row, 0)
                            p.Assists = Functions.ReadFieldIntegerData("ass", row, 0)
                            p.AutoGoal = Functions.ReadFieldIntegerData("autogol", row, 0)
                            p.GoalSubiti = Functions.ReadFieldIntegerData("gs", row, 0)
                            p.GoalFatti = Functions.ReadFieldIntegerData("gf", row, 0)
                            p.RigoriSbagliati = Functions.ReadFieldIntegerData("rigs", row, 0)
                            p.RigoriParati = Functions.ReadFieldIntegerData("rigp", row, 0)
                            p.Punti = Functions.ReadFieldIntegerData("pt", row, 0)
                            list(tid).Players.Add(p)
                        ElseIf type = 10 AndAlso appSett.Settings.Bonus.EnableBonusDefense Then
                            list(tid).BonusDifesa = Functions.ReadFieldIntegerData("pt", row, 0)
                        ElseIf type = 20 AndAlso appSett.Settings.Bonus.EnableCenterField Then
                            list(tid).BonusCentrocampo = Functions.ReadFieldIntegerData("pt", row, 0)
                        ElseIf type = 30 AndAlso appSett.Settings.Bonus.EnableBonusAttack Then
                            list(tid).BonusAttacco = Functions.ReadFieldIntegerData("pt", row, 0)
                        ElseIf type = 40 AndAlso appSett.Settings.SubstitutionType <> TorneoSettings.eSubstitutionType.Normal Then
                            list(tid).CambioModulo = Functions.ReadFieldIntegerData("pt", row, 0)
                        End If
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
            Return list.Values.ToList()
        End Function

        <Serializable()>
        Public Class MetaData
            Public Property type As String = ""
            Public Property giornata() As String = ""
            Public Property teamId() As String = ""
            Public Property data As List(Of Formazione)
        End Class

        <Serializable()>
        Public Class Formazione

            Public Property Giornata() As Integer = 1
            Public Property TeamId() As Integer = 0
            Public Property Modulo() As ModuloFormazione = New ModuloFormazione()
            Public Property PlayersInCampo() As Integer = 0
            Public Property BonusDifesa() As Integer = 0
            Public Property BonusCentrocampo() As Integer = 0
            Public Property BonusAttacco() As Integer = 0
            Public Property CambioModulo() As Integer = 0
            Public Property Punti() As Integer = 0
            Public Property Players() As List(Of PlayerFormazione) = New List(Of PlayerFormazione)
            Public Property Tag As String = ""

        End Class

        <Serializable()>
        Public Class ModuloFormazione
            Public Property Display() As String = "0-0-0"
            Public Property Difensori() As Integer = 0
            Public Property Centrocampisti() As Integer = 0
            Public Property Attaccanti() As Integer = 0
        End Class

        <Serializable()>
        Public Class PlayerFormazione

            Sub New()

            End Sub

            Public Property RosaId() As Integer = 0
            Public Property Jolly() As Integer = 0
            Public Property Type() As Integer = 0
            Public Property FormaId() As Integer = 0
            Public Property InCampo() As Integer = 0
            Public Property Ruolo As String = ""
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Voto As Integer = 0
            Public Property Ammonito() As Integer = 0
            Public Property Espulso() As Integer = 0
            Public Property Assists() As Integer = 0
            Public Property AutoGoal() As Integer = 0
            Public Property GoalSubiti() As Integer = 0
            Public Property GoalFatti() As Integer = 0
            Public Property RigoriTirati() As Integer = 0
            Public Property RigoriSbagliati() As Integer = 0
            Public Property RigoriParati() As Integer = 0
            Public Property Punti As Integer = 0

        End Class

    End Class

End Namespace