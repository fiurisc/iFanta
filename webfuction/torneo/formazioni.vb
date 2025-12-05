Imports System.Data

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
            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Cancellazione formazione giornata: " & Day & " per il team: " & TeamId & " top: " & Top.ToString())
            Dim tb As String = If(Top, "tbformazionitop", "tbformazioni")
            Functions.ExecuteSql(appSett, "DELETE FROM " & tb & " WHERE gio=" & Day & If(TeamId <> "-1", " AND idteam=" & TeamId, ""))
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

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Salvataggio formazioni: " & day & " top: " & top.ToString())

            Dim tb As String = If(top, "tbformazionitop", "tbformazioni")

            For Each forma As Formazione In lst

                Dim sqlinsert As New List(Of String)

                If forma.BonusDifesa > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & tb & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",10," & forma.BonusDifesa & ")")
                    sqlinsert.Add(sqlp.ToString())
                End If

                If forma.BonusCentrocampo > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & tb & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",20," & forma.BonusDifesa & ")")
                    sqlinsert.Add(sqlp.ToString())
                End If

                If forma.BonusAttacco > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & tb & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",30," & forma.BonusDifesa & ")")
                    sqlinsert.Add(sqlp.ToString())
                End If

                If forma.CambioModulo > 0 Then
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & tb & " (gio,idteam,type,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & ",40,1)")
                    sqlinsert.Add(sqlp.ToString())
                End If

                For Each p As PlayerFormazione In forma.Players
                    Dim sqlp As New System.Text.StringBuilder
                    sqlp.AppendLine("INSERT INTO " & tb & " (gio,idteam,idrosa,jolly,type,idformazione,incampo,ruolo,nome,squadra,vote,amm,esp,ass,autogol,gs,gf,rigs,rigp,pt) values (")
                    sqlp.AppendLine(day + 1000 & "," & forma.TeamId & "," & p.RosaId & "," & p.Jolly & "," & p.Type & "," & p.FormaId & "," & p.InCampo & ",'" & p.Ruolo & "',")
                    sqlp.AppendLine("'" & p.Nome.ToUpper() & "','" & p.Squadra.ToUpper() & "'," & p.Voto & "," & p.Ammonito & "," & p.Espulso & "," & p.Assists & "," & p.AutoGoal & ",")
                    sqlp.AppendLine(p.GoalSubiti & "," & p.GoalFatti & "," & p.RigoriSbagliati & "," & p.RigoriParati & "," & p.Punti & ")")
                    sqlinsert.Add(sqlp.ToString())
                Next

                Functions.ExecuteSql(appSett, "DELETE FROM " & tb & " WHERE gio=" & day + 1000 & " AND idteam=" & forma.TeamId)
                Functions.ExecuteSql(appSett, sqlinsert)
                ApiDeleteFormazioni(day.ToString(), forma.TeamId.ToString(), top)
                Functions.ExecuteSql(appSett, "UPDATE " & tb & " SET gio=gio-1000 WHERE gio=" & day + 1000 & " AND idteam=" & forma.TeamId)

            Next


        End Sub

        Public Function GetFormazioni(Day As String, TeamId As String, Top As Boolean) As List(Of Formazione)

            Dim list As List(Of Formazione) = GetFormazioniFromDb(Day, TeamId, Top)

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

        Private Function GetFormazioniFromDb(Day As String, TeamId As String, Top As Boolean) As List(Of Formazione)

            Dim list As New Dictionary(Of Integer, Formazione)

            Try
                Dim tb As String = If(Top, "formazioni_top", "formazioni")
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM " & tb & " WHERE gio=" & Day & If(TeamId <> "-1", " AND idteam = " & TeamId, "") & " ORDER BY idteam,idformazione")

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
                            list(tid).BonusDifesa = Functions.ReadFieldIntegerData("pt", row, 0)
                        ElseIf type = 30 AndAlso appSett.Settings.Bonus.EnableBonusAttack Then
                            list(tid).BonusDifesa = Functions.ReadFieldIntegerData("pt", row, 0)
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

        Sub GetAutomaticFormation(ByVal Giornata As Integer)
            Dim formaList As New Dictionary(Of Integer, Formazione)
            For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                formaList.Add(i, GetAutomaticFormation(i, Giornata))
            Next
        End Sub

        Function GetAutomaticFormation(ByVal IdTeam As Integer, ByVal Giornata As Integer) As Formazione

            'Dim fc As New List(Of Formazione.PlayerFormazione)
            'Dim ft As New List(Of Formazione.PlayerFormazione)
            Dim f As New Formazione
            'Dim r As New LegaObject.Team(IdTeam, "")
            'Dim rp As New List(Of LegaObject.Team.Player)
            'Dim portteam As String = ""

            ''Carico la rosa'
            'r.Load(True, True, True)

            'rp.AddRange(r.Players)

            'Dim dir As String = GetLegaTemDirectory()
            'Dim str As New System.Text.StringBuilder

            'For i As Integer = 0 To rp.Count - 1
            '    str.AppendLine(rp(i).Nome & "|" & rp(i).Squadra & "|" & rp(i).Rating)
            'Next
            'IO.File.WriteAllText(dir & "\" & IdTeam & "-rating.txt", str.ToString)

            ''Calcolo il rating dei vari giocatori'
            'rp = CalculateRatingPresenze(Giornata, IdTeam, False, rp)

            ''Determino la formazione automatica'
            'f = DetectAutomaticForma(IdTeam, False, rp)

            ''Controllo la formazione'
            'Dim recheck As Boolean = False
            'Dim pc As New List(Of String)
            'For i As Integer = f.Count - 1 To 0 Step -1
            '    If f(i).Tag = "1" AndAlso f(i).Type = 0 Then
            '        'Verifico un panchinaro utile'
            '        Dim good As Boolean = False
            '        For j As Integer = 0 To f.Count - 1
            '            If f(j).Type = 1 AndAlso f(i).Ruolo = f(j).Ruolo AndAlso pc.Contains(f(j).Nome & "-" & f(j).Squadra) = False AndAlso pc.Count < 3 Then
            '                If webdata.WebPlayers.ContainsKey(Giornata & "-" & f(j).Nome & "-" & f(j).Squadra) Then
            '                    Dim wp As wData.wPlayer = webdata.WebPlayers(Giornata & "-" & f(j).Nome & "-" & f(j).Squadra)
            '                    If wp.Titolare / webdata.NumSitePlayer > 0.79 Then
            '                        good = True
            '                        pc.Add(f(j).Nome & "-" & f(j).Squadra)
            '                        Exit For
            '                    End If
            '                End If
            '            End If
            '        Next
            '        If good = False Then
            '            recheck = True
            '        Else
            '            For h As Integer = 0 To rp.Count - 1
            '                If f(i).Nome = rp(h).Nome AndAlso f(i).Squadra = rp(h).Squadra Then
            '                    rp(h).Tag = "0"
            '                    Exit For
            '                End If
            '            Next
            '        End If
            '    End If
            'Next
            'If recheck Then
            '    For h As Integer = rp.Count - 1 To 0 Step -1
            '        rp(h).Type = 0
            '        rp(h).Schierato = 0
            '    Next
            '    'Calcolo il rating dei vari giocatori'
            '    rp = CalculateRatingPresenze(Giornata, IdTeam, True, rp)
            '    'Determino la formazione automatica'
            '    f = DetectAutomaticForma(IdTeam, True, rp)
            'End If

            ''Verifico se ci sno attaccanti della stessa squadra'
            'If CheckSameTeamForward(f) Then
            '    f = DetectAutomaticForma(IdTeam, True, f)
            'End If

            Return f

        End Function

        Private Function DetectAutomaticForma(ByVal IdTeam As Integer, ByVal rechek As Boolean, ByRef rp As List(Of PlayerFormazione)) As List(Of PlayerFormazione)

            Dim f As New List(Of PlayerFormazione)
            Dim ft As New List(Of PlayerFormazione)
            Dim fc As New List(Of RoseData.Player)

            Dim portteam As String = ""

            'Ordino i giocatori sulla base del loro rating'
            Dim rs As New RoseData(appSett)
            Dim rosa As Dictionary(Of String, List(Of RoseData.Player)) = rs.GetPlayersFromDb(IdTeam.ToString(), "", "")

            If rosa.Count > 0 AndAlso rosa.ContainsKey(IdTeam.ToString()) Then
                Dim pl As New List(Of RoseData.Player)
                For Each p As RoseData.Player In rosa(IdTeam.ToString())
                    p.Rating = GetRating(p)
                Next
            End If
            'fc = rs.GetPlayersFromDb(IdTeam.ToString(), "", "")(IdTeam.ToString()).ToList()
            'ft(0).
            ''Determino i titolari'
            'Dim ntit As Integer = 0
            'Dim np As Integer = 0
            'Dim nd As Integer = 0
            'Dim nc As Integer = 0
            'Dim na As Integer = 0

            'For i As Integer = 0 To fc.Count - 1
            '    fc(i).Type = 0
            '    fc(i).Schierato = 0
            '    If CheckMudule1(fc(i).Ruolo, np, nd, nc, na) Then
            '        fc(i).Schierato = 1
            '        fc(i).Type = 0
            '        fc(i).IdRosa = i
            '        Select Case fc(i).Ruolo
            '            Case "P" : np = np + 1 : portteam = fc(i).Squadra
            '            Case "D" : nd = nd + 1
            '            Case "C" : nc = nc + 1
            '            Case "A" : na = na + 1
            '        End Select
            '        ntit = ntit + 1
            '        'If ntit > 11 Then Exit For
            '    End If
            'Next

            ''Ordino i giocatori sulla base ruolo'
            'ft = LegaObject.Team.Sort(fc, "", False)
            'For i As Integer = 0 To ft.Count - 1
            '    ft(i).IdRosa = i + 1
            'Next
            'f.AddRange(ft)

            ''Determino i panchinari'
            'Dim p() As String = {"P", "A", "C", "D"}
            'Dim ind As Integer = 1
            'Dim s As Integer = 0

            ''Controllo se esiste un secondo portire della stessa squadra'
            'For i As Integer = 0 To fc.Count - 1
            '    If fc(i).Schierato = 0 AndAlso fc(i).Ruolo = "P" AndAlso portteam = fc(i).Squadra Then
            '        f.Add(New Team.Player(1, fc(i).Ruolo, fc(i).Nome, fc(i).Squadra, 1, 1, 0))
            '        ind = ind + 1
            '        s = 1
            '        Exit For
            '    End If
            'Next

            ''Determino il resto dei panchinari'
            'For i As Integer = s To p.Length - 1

            '    Dim nump As Integer = 1
            '    Dim maxp As Integer = 2

            '    If p(i) = "P" Then maxp = 1

            '    For k As Integer = 0 To fc.Count - 1
            '        If fc(k).Schierato = 0 AndAlso fc(k).Ruolo = p(i) Then
            '            f.Add(New Team.Player(ind, fc(k).Ruolo, fc(k).Nome, fc(k).Squadra, 1, 1, 0))
            '            nump = nump + 1
            '            ind = ind + 1
            '        End If
            '        If nump > maxp Then Exit For
            '    Next
            'Next

            'Dim dir As String = GetLegaTemDirectory()
            'Dim str As New System.Text.StringBuilder
            'For i As Integer = 0 To f.Count - 1
            '    str.AppendLine(f(i).Ruolo & "|" & f(i).Nome & "|" & f(i).Squadra & "|" & f(i).Rating & "|" & f(i).Schierato & "|" & f(i).Type)
            'Next
            'If rechek Then
            '    IO.File.WriteAllText(dir & "\" & IdTeam & "-rating-tot.txt", str.ToString)
            'Else
            '    IO.File.WriteAllText(dir & "\" & IdTeam & "-rating-tot-recheck.txt", str.ToString)
            'End If

            Return f
        End Function

        Private Function GetRating(p As RoseData.Player) As Integer

            Dim value As Integer = 0

            Return value

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