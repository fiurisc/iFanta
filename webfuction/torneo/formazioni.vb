Imports System.Data
Imports System.Security.Cryptography

Namespace Torneo

    Public Class FormazioniData

        Public Shared Function ApiAddFormazioni(Day As String, TeamId As String, Top As Boolean, json As String) As String

            If json = "" Then Throw New Exception("Json not valid")

            Dim tb As String = If(Top, "tbformazionitop", "tbformazioni")
            Dim mData As MetaData = WebData.Functions.DeserializeJson(Of MetaData)(json)

            If mData.teamId <> TeamId Then Throw New Exception("Json not related to right teamid")

            ApiDeleteFormazioni(Day, TeamId, Top)

            If mData IsNot Nothing AndAlso mData.data.Count > 0 Then
                Dim dicform As Dictionary(Of Integer, Formazione) = mData.data.ToDictionary(Function(x) x.TeamId, Function(x) x)
                For Each tid As Integer In dicform.Keys
                    Dim sqlinsert As New List(Of String)
                    For Each p As PlayerFormazione In dicform(tid).Players
                        If p.Type >= 10 Then
                            Dim sqlp As New System.Text.StringBuilder
                            sqlp.AppendLine("INSERT INTO " & tb & " (gio,idteam,type,pt) values (")
                            sqlp.AppendLine(Day & "," & tid & "," & p.Type & "," & p.Punti & ")")
                            sqlinsert.Add(sqlp.ToString())
                        Else
                            Dim sqlp As New System.Text.StringBuilder
                            sqlp.AppendLine("INSERT INTO " & tb & " (gio,idteam,idrosa,jolly,type,idformazione,incampo,ruolo,nome,squadra,vote,amm,esp,ass,autogol,gs,gf,rigs,rigp,pt) values (")
                            sqlp.AppendLine(Day & "," & tid & "," & p.RosaId & "," & p.Jolly & "," & p.Type & "," & p.FormaId & "," & p.InCampo & ",'" & p.Ruolo & "',")
                            sqlp.AppendLine("'" & p.Nome.ToUpper() & "','" & p.Squadra.ToUpper() & "'," & p.Voto & "," & p.Ammonito & "," & p.Espulso & "," & p.Assists & "," & p.AutoGoal & ",")
                            sqlp.AppendLine(p.GoalSubiti & "," & p.GoalFatti & "," & p.RigoriSbagliati & "," & p.RigoriParati & "," & p.Punti & ")")
                            sqlinsert.Add(sqlp.ToString())
                        End If
                    Next
                    Functions.ExecuteSql(sqlinsert)
                Next
            End If

            Return ""

        End Function

        Public Shared Sub ApiDeleteFormazioni(Day As String, TeamId As String, Top As Boolean)
            Dim tb As String = If(Top, "tbformazionitop", "tbformazioni")
            Functions.ExecuteSql("DELETE FROM " & tb & " WHERE gio=" & Day & If(TeamId <> "-1", " AND idteam=" & TeamId, ""))
            WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, "DELETE FROM " & tb & " WHERE gio=" & Day & If(TeamId <> "-1", " AND idteam=" & TeamId, ""))
        End Sub

        Public Shared Function ApiGetFormazione(Day As String, TeamId As String, Top As Boolean) As String

            Dim json As String = ""

            Try
                Dim list As List(Of Formazione) = GetFormazioni(Day, TeamId, Top)
                If list.Count > 0 Then
                    Return WebData.Functions.SerializzaOggetto(list(0), True)
                Else
                    Return "{}"
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return json

        End Function

        Public Shared Function ApiGetFormazioni(Day As String, TeamId As String, Top As Boolean) As String

            Dim json As String = ""

            Try
                Dim list As List(Of Formazione) = GetFormazioni(Day, TeamId, Top)
                Dim dicForma As Dictionary(Of String, Formazione) = list.ToDictionary(Function(x) x.TeamId.ToString(), Function(x) x)
                Return WebData.Functions.SerializzaOggetto(dicForma, True)
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return json

        End Function

        Private Shared Function GetFormazioni(Day As String, TeamId As String, Top As Boolean) As List(Of Formazione)

            Dim list As List(Of Formazione)

            If PublicVariables.DataFromDatabase Then
                list = GetFormazioniFromDb(Day, TeamId, Top)
            Else
                list = GetFormazioniFromTxt(Day, TeamId, Top)
            End If

            For Each forma As Formazione In list

                'Determino il modulo'

                For Each p As PlayerFormazione In forma.Players
                    forma.PlayersInCampo += p.InCampo
                    If p.InCampo = 1 AndAlso p.Punti <> -200 Then forma.Punti += p.Punti
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

            Next

            Return list

        End Function

        Private Shared Function GetFormazioniFromDb(Day As String, TeamId As String, Top As Boolean) As List(Of Formazione)

            Dim list As New Dictionary(Of Integer, Formazione)

            Try
                Dim tb As String = If(Top, "formazionitop", "formazioni")
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM " & tb & " WHERE gio=" & Day & If(TeamId <> "-1", " AND idteam = " & TeamId, "") & " ORDER BY idteam,idformazione")

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim type As Integer = Functions.ReadFieldIntegerData("type", row, 0)
                        Dim tid As Integer = Functions.ReadFieldIntegerData("idteam", row, 0)

                        If list.ContainsKey(tid) = False Then list.Add(tid, New Formazione())

                        list(tid).Giornata = CInt(Day)
                        list(tid).TeamId = tid

                        If type <= 10 Then
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
                        ElseIf type = 10 AndAlso PublicVariables.Settings.Bonus.EnableBonusDefense Then
                            list(tid).BonusDifesa = Functions.ReadFieldIntegerData("pt", row, 0)
                        ElseIf type = 20 AndAlso PublicVariables.Settings.Bonus.EnableCenterField Then
                            list(tid).BonusDifesa = Functions.ReadFieldIntegerData("pt", row, 0)
                        ElseIf type = 30 AndAlso PublicVariables.Settings.Bonus.EnableBonusAttack Then
                            list(tid).BonusDifesa = Functions.ReadFieldIntegerData("pt", row, 0)
                        ElseIf type = 40 AndAlso PublicVariables.Settings.SubstitutionType <> TorneoSettings.eSubstitutionType.Normal Then
                            list(tid).CambioModulo = Functions.ReadFieldIntegerData("pt", row, 0)
                        End If
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
            Return list.Values.ToList()
        End Function

        Private Shared Function GetFormazioniFromTxt(Day As String, TeamId As String, Top As Boolean) As List(Of Formazione)

            Dim list As New Dictionary(Of String, Formazione)

            Try
                If Top = False Then
                    Dim fname As String = PublicVariables.DataPath & "\export\formazioni.txt"
                    Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                    For Each line As String In lines
                        Dim values() As String = line.Split(Convert.ToChar("|"))
                        If (Day = "-1" OrElse values(0) = Day) AndAlso TeamId = "-1" OrElse values(1) = TeamId Then
                            Dim tid As String = values(1)
                            If list.ContainsKey(tid) = False Then list.Add(tid, New Formazione())
                            list(tid).Giornata = CInt(Day)
                            list(tid).TeamId = CInt(tid)
                            If values.Length > 10 Then
                                Dim p As New PlayerFormazione
                                p.RosaId = CInt(values(2))
                                p.Jolly = CInt(values(3))
                                p.Type = CInt(values(4))
                                p.FormaId = CInt(values(5))
                                p.InCampo = CInt(values(6))
                                p.Ruolo = values(7)
                                p.Nome = values(8)
                                p.Squadra = values(9)
                                p.Voto = CInt(values(10))
                                p.Ammonito = CInt(values(11))
                                p.Espulso = CInt(values(12))
                                p.Assists = CInt(values(13))
                                p.AutoGoal = CInt(values(14))
                                p.GoalSubiti = CInt(values(15))
                                p.GoalFatti = CInt(values(16))
                                p.RigoriTirati = CInt(values(17))
                                p.RigoriSbagliati = CInt(values(18))
                                p.RigoriParati = CInt(values(19))
                                p.Punti = CInt(values(20))
                                list(tid).Players.Add(p)
                            ElseIf values.Length > 5 Then
                                list(tid).BonusDifesa = CInt(values(2))
                                list(tid).BonusCentrocampo = CInt(values(3))
                                list(tid).BonusAttacco = CInt(values(4))
                                list(tid).CambioModulo = CInt(values(2))
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return list.Values.ToList()

        End Function

        Public Class MetaData
            Public Property type As String = ""
            Public Property giornata() As String = ""
            Public Property teamId() As String = ""
            Public Property data As List(Of Formazione)
        End Class

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

        End Class

        Public Class ModuloFormazione
            Public Property Display() As String = "0-0-0"
            Public Property Difensori() As Integer = 0
            Public Property Centrocampisti() As Integer = 0
            Public Property Attaccanti() As Integer = 0
        End Class

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