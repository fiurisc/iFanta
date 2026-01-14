Imports System.Data
Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
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

        Public Function GetFormazioni(Day As String, TeamId As String, Top As Boolean) As List(Of Formazione)
            Return GetFormazioni(Day, TeamId, If(Top, "formazioni_top", "formazioni"))
        End Function

        Public Function GetFormazioni(Day As String, TeamId As String, Table As String) As List(Of Formazione)

            Dim list As List(Of Formazione) = GetFormazioniFromDb(Day, TeamId, Table)

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

        Public Function ApiGetFormazioniAutomatiche(Day As String, TeamId As String) As String

            Dim json As String = ""

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Richiesta formazioni automatiche giornata: " & Day & " per il team: " & TeamId)

            Try
                Dim list As New List(Of Formazione)
                For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                    If TeamId = "-1" OrElse i.ToString() = TeamId Then
                        list.Add(GetFormazioneAutomatica(i, CInt(Day), i <> 0))
                    End If
                Next
                Dim dicForma As Dictionary(Of String, Formazione) = list.ToDictionary(Function(x) x.TeamId.ToString(), Function(x) x)
                Return WebData.Functions.SerializzaOggetto(dicForma, True)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return json

        End Function

        Sub GetFormazioniAutomatiche(ByVal Giornata As Integer)
            Dim formaList As New Dictionary(Of Integer, Formazione)
            For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                formaList.Add(i, GetFormazioneAutomatica(i, Giornata, i <> 0))
            Next
        End Sub

        Function GetFormazioneAutomatica(ByVal IdTeam As Integer, ByVal Giornata As Integer, AppendLog As Boolean) As Formazione

            ' Determino la rosa con i dati statistici'

            Dim fileLog As String = appSett.WebDataPath & "\temp\autoforma.log"
            Dim addlastpresence As Boolean = True
            Dim addpostionrank As Boolean = True
            Dim addhomerank As Boolean = True
            Dim addprobable As Boolean = True
            Dim probable As New Dictionary(Of String, Probable)
            Dim sr As New IO.StreamWriter(fileLog, AppendLog)
            Dim daydata As Integer = Giornata - 1
            Dim pforma As New List(Of PlayerFormazione)
            Dim maxday As Integer = 0

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT max(gio) as gio FROM tbrank")
                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                    maxday = CInt(ds.Tables(0).Rows(0)("gio"))
                    If Giornata > maxday + 1 Then
                        daydata = maxday
                    End If
                End If

                If addprobable Then
                    Dim probdata As New Torneo.ProbablePlayers(appSett)
                    probable = probdata.GetProbableFormation("")
                End If

                sr.WriteLine("**** Dertermino la rosa (" & IdTeam & ") con tutti i valori utili aggiornati alla giornata (" & Giornata & ") ****")

                '************************************************************************************************
                '** i seguenti paramentri servono solo se si desidera compilare una formazione                 **
                '** automatica delle giornate passate, perchè a sistema non è presente uno storico delle rose  **
                '************************************************************************************************
                Dim tbref As String = If(Giornata > maxday, "tbrose", "tbformazioni")
                Dim tbwhere As String = If(Giornata > maxday, "tbr.idteam=" & IdTeam, "tbr.idteam=" & IdTeam & " AND tbr.gio=" & Giornata & " AND tbr.type<3")
                '************************************************************************************************
                Dim fc As New List(Of PlayerRosaFormazione)
                Dim nday As Integer = If(daydata > 5, 5, daydata)
                Dim daydiff As Integer = 5
                Dim sqlstr As New Text.StringBuilder
                sqlstr.AppendLine("SELECT tbd.*,tbr.pos as posb,int((tbr.pos -1) / 5) AS posgrb FROM (")
                sqlstr.AppendLine(" SELECT tbd.*,tbr.pos as posa,int((tbr.pos -1) / 5) AS posgra FROM (")
                sqlstr.AppendLine("  SELECT tbd.*,tbm.teama,teamb,iif(tbd.squadra=teama,1,0) as home,timem,iif(CDate(timem)>Now(),1,0) as available,DateDiff('h', Now(), CDate(timem)) AS tleft FROM (")
                sqlstr.AppendLine("   SELECT tbd.*,tbp.squadra FROM (")
                sqlstr.AppendLine("    SELECT tbd.idrosa,tbd.ruolo, tbd.nome,sum(tbd.gf) as gf,sum(tbd.gs) as gs,sum(tbd.ass) as ass,sum(tbd.pt) as pt, IIf(Sum(tbd.pt)>0,CInt(Avg(tbd.pt)),0) AS avg_pt, Count(*) AS pgio, Sum(tbt.tit) AS tit, Sum(tbt.sos) AS sos, Sum(tbt.sub) AS sub, Sum(tbt.mm) AS mm, iif(Sum(tbt.mm) > 0,CInt (Sum(tbt.mm)) / " & nday & ",0 ) AS avg_mm FROM (")
                sqlstr.AppendLine("     SELECT tbr.idrosa,tbr.ruolo,tbr.nome,tbd.gio,tbd.gf,tbd.gs,tbd.ass,tbd.pt")
                sqlstr.AppendLine("     FROM  " & tbref & " as tbr")
                sqlstr.AppendLine("     LEFT JOIN tbdati as tbd on (tbd.nome=tbr.nome AND tbd.pt > -100 AND tbd.gio >" & daydata - (daydiff + 2) & " and tbd.gio<=" & daydata & ")")
                sqlstr.AppendLine("     WHERE " & tbwhere & ") as tbd")
                sqlstr.AppendLine("    LEFT JOIN tbtabellini AS tbt ON (tbd.gio = tbt.gio) AND (tbd.nome = tbt.nome)")
                sqlstr.AppendLine("    GROUP BY tbd.idrosa,tbd.ruolo, tbd.nome) as tbd")
                sqlstr.AppendLine("   LEFT JOIN tbplayer as tbp on tbp.nome=tbd.nome) as tbd")
                sqlstr.AppendLine("  LEFT JOIN tbmatch as tbm ON (tbm.gio = " & Giornata & " AND (tbd.squadra = tbm.teama OR tbd.squadra = tbm.teamb))) as tbd")
                sqlstr.AppendLine(" LEFT JOIN tbrank as tbr ON (tbd.teama=tbr.squadra and tbr.gio=" & daydata & ")) as tbd")
                sqlstr.AppendLine("LEFT JOIN tbrank as tbr ON (tbd.teamb=tbr.squadra and tbr.gio=" & daydata & ")")
                sqlstr.AppendLine("ORDER BY idrosa")

                Dim a As String = sqlstr.ToString()

                ds = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim p As New PlayerRosaFormazione
                        p.RosaId = Functions.ReadFieldIntegerData("idrosa", row, 0)
                        p.Ruolo = Functions.ReadFieldStringData("ruolo", row, "")
                        p.Nome = Functions.ReadFieldStringData("nome", row, "")
                        p.Squadra = Functions.ReadFieldStringData("squadra", row, "")
                        p.Gf = Functions.ReadFieldIntegerData("gf", row, 0)
                        p.Gs = Functions.ReadFieldIntegerData("gs", row, 0)
                        p.Ass = Functions.ReadFieldIntegerData("ass", row, 0)
                        p.Pt = Functions.ReadFieldIntegerData("pt", row, 0)
                        p.AvgPt = Functions.ReadFieldIntegerData("avg_pt", row, 0)
                        p.pGiocate = Functions.ReadFieldIntegerData("pgio", row, 0)
                        p.Titolare = Functions.ReadFieldIntegerData("tit", row, 0)
                        p.Sostituito = Functions.ReadFieldIntegerData("sos", row, 0)
                        p.Minuti = Functions.ReadFieldIntegerData("mm", row, 0)
                        p.AvgMinuti = Functions.ReadFieldIntegerData("avg_mm", row, 0)
                        p.TimeMatch = Functions.ReadFieldStringData("timem", row, "")
                        p.TimeLeft = Functions.ReadFieldIntegerData("tleft", row, 0)
                        p.Available = Functions.ReadFieldIntegerData("available", row, 0)
                        p.TeamA = Functions.ReadFieldStringData("teama", row, "")
                        p.TeamB = Functions.ReadFieldStringData("teamb", row, "")
                        p.PosA = Functions.ReadFieldIntegerData("posa", row, 1)
                        p.PosB = Functions.ReadFieldIntegerData("posb", row, 1)
                        p.PosGroupA = Functions.ReadFieldIntegerData("posgra", row, 0)
                        p.PosGroupB = Functions.ReadFieldIntegerData("posgrb", row, 0)
                        fc.Add(p)
                    Next
                End If

                Dim dicPosGroup As New Dictionary(Of String, Double)
                Dim minPosFact As Double = 60
                Dim maxPosFact As Double = 60
                Dim maxposvalue As Integer = 110

                If addpostionrank Then

                    For Each p As PlayerRosaFormazione In fc
                        Dim key As String = p.Ruolo & "-" & If(p.Squadra = p.TeamA, "1", "0") & "-" & If(p.Squadra = p.TeamA, p.PosGroupA, p.PosGroupB) & "-" & If(p.Squadra = p.TeamA, p.PosGroupB, p.PosGroupA)
                        If dicPosGroup.ContainsKey(key) = False Then dicPosGroup.Add(key, 60)
                    Next

                    sqlstr = New Text.StringBuilder
                    sqlstr.AppendLine("SELECT ruolo, casa, pos, avv, avg(pt) AS fact FROM (")
                    sqlstr.AppendLine(" SELECT tb.*, int((tbrank.pos -1) / 5) AS avv FROM (")
                    sqlstr.AppendLine("  SELECT tbdati.gio, tbdati.ruolo, tbdati.squadra, int((tbrank.pos -1) / 5) AS pos, tbdati.voto as pt,iif(tbdati.squadra = tbmatch.teama, 1, 0) AS casa, iif( tbdati.squadra = tbmatch.teama, tbmatch.teamb, tbmatch.teama) AS avversaria FROM (")
                    sqlstr.AppendLine("  tbdati LEFT JOIN tbmatch ON tbdati.gio = tbmatch.gio AND ( tbmatch.teama = tbdati.squadra OR tbmatch.teamb = tbdati.squadra))")
                    sqlstr.AppendLine("  LEFT JOIN tbrank ON tbdati.gio = tbrank.gio AND tbdati.squadra = tbrank.squadra WHERE tbdati.pt > -100 AND tbdati.gio>" & daydata - 10 & " AND tbdati.gio<=" & daydata & ") AS tb")
                    sqlstr.AppendLine(" LEFT JOIN tbrank ON tb.gio = tbrank.gio AND tb.avversaria = tbrank.squadra) AS tb")
                    sqlstr.AppendLine("GROUP BY ruolo,casa,pos,avv")
                    sqlstr.AppendLine("ORDER BY ruolo,casa,pos, avv;")

                    ds = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

                    If ds.Tables.Count > 0 Then
                        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            Dim row As DataRow = ds.Tables(0).Rows(i)
                            Dim key As String = ds.Tables(0).Rows(i)("ruolo").ToString() & "-" & ds.Tables(0).Rows(i)("casa").ToString() & "-" & ds.Tables(0).Rows(i)("pos").ToString() & "-" & ds.Tables(0).Rows(i)("avv").ToString()
                            Dim factGroup As Double = CDbl(ds.Tables(0).Rows(i)("fact").ToString())
                            'If dicPosGroup.ContainsKey(key) = False Then dicPosGroup.Add(key, factGroup)
                            If dicPosGroup.ContainsKey(key) Then dicPosGroup(key) = factGroup
                            If factGroup < minPosFact Then minPosFact = factGroup
                            If factGroup > maxPosFact Then maxPosFact = factGroup
                        Next
                    End If

                End If

                sr.WriteLine("**** Determino i fattori di rating su base punti/presenza ultime giornate ****")

                Dim ptMax As Single = 0
                Dim ptMin As Single = 0
                Dim maxavgvalue As Integer = 40

                If fc.Count > 0 Then
                    ptMax = fc.Select(Function(x) If(x.pGiocate > daydiff, x.AvgPt * daydiff, x.AvgPt * x.pGiocate)).ToList().Max
                    ptMin = fc.Select(Function(x) If(x.pGiocate > daydiff, x.AvgPt * daydiff, x.AvgPt * x.pGiocate)).ToList().Min
                End If

                Dim factpt As Double = If(ptMax <> ptMin, maxavgvalue / (ptMax - ptMin), 0)

                sr.WriteLine("Rating dei singoli giocatori")

                For Each p As PlayerRosaFormazione In fc

                    If p.Nome = "YILDIZ" Then
                        p.Nome = p.Nome
                    End If

                    Dim rat1 As Double = 0
                    Dim rat2 As Double = 0
                    Dim rat3 As Double = 0
                    Dim rat4 As Double = 0
                    Dim rat5 As Double = 0

                    If p.pGiocate > daydiff Then
                        rat1 = CInt(Math.Floor(If(factpt = 0, maxavgvalue, factpt * (p.AvgPt * daydiff - ptMin))))
                    Else
                        rat1 = CInt(Math.Floor(If(factpt = 0, maxavgvalue, factpt * (p.AvgPt * p.pGiocate - ptMin))))
                    End If

                    Dim factpos As Double = If(maxPosFact <> minPosFact, maxposvalue / (maxPosFact - minPosFact), 0)

                    If addpostionrank AndAlso minPosFact < 1000 Then
                        Dim key As String = p.Ruolo & "-" & If(p.Squadra = p.TeamA, "1", "0") & "-" & If(p.Squadra = p.TeamA, p.PosGroupA, p.PosGroupB) & "-" & If(p.Squadra = p.TeamA, p.PosGroupB, p.PosGroupA)
                        Dim prat As Integer = CInt(Math.Floor(If(factpos = 0, maxposvalue, factpos * (dicPosGroup(key) - minPosFact))))
                        If dicPosGroup.ContainsKey(key) Then rat2 = prat
                    End If

                    rat3 = p.Gf * 3 + p.Ass * 2

                    If p.Ruolo = "C" Then rat4 = 10
                    If p.Ruolo = "A" Then rat4 = 30

                    If addlastpresence AndAlso p.Minuti > 0 Then
                        rat5 = CInt(p.Minuti / 450 * 30)
                    End If

                    p.Rating = CInt(rat1 + rat2 + rat3 + rat4 + rat5)

                    If addprobable Then

                        'se abilitato tengo conto anche delle probabili formazioni attuali'
                        Dim val As Double = -1

                        For Each site As String In probable.Keys
                            If probable(site).Day = Giornata Then
                                If val < 0 Then val = 0
                                Dim keyp As String = p.Nome & "/" & p.Squadra
                                If probable(site).Players.ContainsKey(keyp) Then
                                    If probable(site).Players(keyp).State = "Titolare" Then
                                        val += 1
                                    ElseIf probable(site).Players(keyp).State = "Panchina" Then
                                        val += 0.8
                                    End If
                                End If
                            End If
                        Next

                        If val >= 0 Then
                            If val > 3 Then val = 3
                            val /= 3
                            p.Rating = CInt(p.Rating * val)
                        End If

                    End If

                    sr.WriteLine(p.RosaId & vbTab & p.Ruolo & vbTab & p.Nome & vbTab & p.Squadra & vbTab & p.Rating & vbTab & rat1 & vbTab & rat2 & vbTab & rat3 & vbTab & rat4 & vbTab & rat5)

                Next

                sr.WriteLine("**** Ordino i giocatori su base rating ****")

                fc = fc.OrderByDescending(Function(x) x.Rating).ToList()

                For i As Integer = 0 To fc.Count - 1
                    sr.WriteLine(i + 1 & vbTab & fc(i).Ruolo & vbTab & fc(i).Nome & vbTab & fc(i).Squadra & vbTab & fc(i).Rating)
                Next

                sr.WriteLine("**** Determino i titolari ****")

                For i As Integer = 0 To fc.Count - 1
                    Dim p As New PlayerFormazione
                    p.RosaId = fc(i).RosaId
                    p.Ruolo = fc(i).Ruolo
                    p.Nome = fc(i).Nome
                    p.Squadra = fc(i).Squadra
                    p.InCampo = 0
                    p.Type = 0
                    pforma.Add(p)
                Next

                Dim np As Integer = 0
                Dim nd As Integer = 0
                Dim nc As Integer = 0
                Dim na As Integer = 0
                Dim ntit As Integer = 1
                Dim npanc As Integer = 1
                Dim portteam As String = ""
                Dim ruoliPanc As New List(Of String) From {"P", "A", "C", "D"}
                Dim ruoliPancNbr As New Dictionary(Of String, Integer) From {{"P", 0}, {"A", 0}, {"C", 0}, {"D", 0}}
                Dim ruoliPancMax As New Dictionary(Of String, Integer) From {{"P", 1}, {"A", 3}, {"C", 3}, {"D", 3}}
                Dim indrp As Integer = 0

                Do Until ntit > 11 AndAlso indrp > 3
                    For Each p As PlayerFormazione In pforma
                        If p.Type = 0 Then
                            If CheckMudule(p.Ruolo, np, nd, nc, na) Then
                                If (p.Ruolo = "P" AndAlso np = 0) OrElse ntit < 12 Then
                                    p.Type = 1
                                    Select Case p.Ruolo
                                        Case "P" : np += 1 : portteam = p.Squadra
                                        Case "D" : nd += 1
                                        Case "C" : nc += 1
                                        Case "A" : na += 1
                                    End Select
                                    ntit += 1
                                End If
                                If ntit > 11 Then Exit For
                            ElseIf npanc < 11 AndAlso ruoliPanc(indrp) = p.Ruolo AndAlso ruoliPancNbr(ruoliPanc(indrp)) < ruoliPancMax(ruoliPanc(indrp)) Then
                                p.Type = 2
                                p.FormaId = npanc + 11
                                npanc += 1
                                ruoliPancNbr(ruoliPanc(indrp)) += 1
                                If ruoliPancNbr(ruoliPanc(indrp)) = ruoliPancMax(ruoliPanc(indrp)) Then indrp += 1 : Exit For
                            End If
                        End If
                    Next
                Loop

                For Each p In pforma.Where(Function(x) x.Type = 1)
                    p.FormaId = p.RosaId
                Next

                pforma = pforma.OrderBy(Function(x) x.RosaId).ToList()

                Dim fid As Integer = 1

                sr.WriteLine("**** Titolari ****")

                For i As Integer = 0 To pforma.Count - 1
                    If pforma(i).Type = 1 Then
                        pforma(i).FormaId = fid
                        sr.WriteLine(pforma(i).FormaId & vbTab & pforma(i).Ruolo & vbTab & pforma(i).Nome & vbTab & pforma(i).Squadra)
                        fid += 1
                    End If
                Next

                sr.WriteLine("**** Panchinari ****")

                pforma = pforma.OrderBy(Function(x) x.FormaId).ToList()

                For i As Integer = 0 To pforma.Count - 1
                    If pforma(i).Type = 2 Then
                        sr.WriteLine(pforma(i).FormaId & vbTab & pforma(i).Ruolo & vbTab & pforma(i).Nome & vbTab & pforma(i).Squadra)
                    End If
                Next

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            sr.Close()
            sr.Dispose()

            Dim forma As New Formazione With {
                .Giornata = Giornata,
                .TeamId = IdTeam,
                .Players = pforma
            }

            Return forma

        End Function

        Public Shared Function CheckMudule(ByVal Ruolo As String, ByVal CurrP As Integer, ByVal CurrD As Integer, ByVal CurrC As Integer, ByVal CurrA As Integer) As Boolean

            Dim ris As Boolean = False

            Dim tot As Integer = CurrP + CurrD + CurrC + CurrA + 1

            Select Case Ruolo
                Case "P" : CurrP += 1
                Case "D" : CurrD += 1
                Case "C" : CurrC += 1
                Case "A" : CurrA += 1
            End Select

            If CurrP < 2 AndAlso CurrD < 4 AndAlso CurrC < 5 AndAlso CurrA < 4 Then '343'
                ris = True
                'ris = Not (CurrD = 3 AndAlso CurrC = 4 AndAlso CurrA = 5)
            ElseIf CurrP < 2 AndAlso CurrD < 4 AndAlso CurrC < 6 AndAlso CurrA < 3 Then '352'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 4 AndAlso CurrA < 4 Then '433'
                ris = True
                ris = Not (CurrD = 4 AndAlso CurrC = 4 AndAlso CurrA = 3)
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 5 AndAlso CurrA < 3 Then '442'
                ris = True
                'ris = Not (CurrD = 4 AndAlso CurrC = 4 AndAlso CurrA = 2)
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 6 AndAlso CurrA < 2 Then '451'
                ris = True
                'ris = Not (CurrD = 4 AndAlso CurrC = 5 AndAlso CurrA = 1)
                'ElseIf CurrP < 2 AndAlso CurrD < 6 AndAlso CurrC < 4 AndAlso CurrA < 3 Then '532'
                '    ris = True
                '    ris = Not (CurrD = 5 AndAlso CurrC = 3 AndAlso CurrA = 2)
                'ElseIf CurrP < 2 AndAlso CurrD < 6 AndAlso CurrC < 5 AndAlso CurrA < 2 Then '541'
                '    ris = True
                '    ris = Not (CurrD = 5 AndAlso CurrC = 4 AndAlso CurrA = 1)
            End If


            Return ris

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

        Public Class PlayerRosaFormazione
            Public Property RosaId() As Integer = 0
            Public Property Ruolo As String = ""
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Gs() As Integer = 0
            Public Property Gf() As Integer = 0
            Public Property Ass() As Integer = 0
            Public Property Pt As Single = 0
            Public Property AvgPt As Single = 0
            Public Property nPartite() As Integer = 0
            Public Property pGiocate() As Integer = 0
            Public Property Titolare() As Integer = 0
            Public Property Sostituito() As Integer = 0
            Public Property Subentrato() As Integer = 0
            Public Property Minuti() As Integer = 0
            Public Property AvgMinuti() As Integer = 0
            Public Property Available As Integer = 0
            Public Property TimeMatch As String = ""
            Public Property TimeLeft As Integer = 0
            Public Property Home() As Integer = 0
            Public Property TeamA As String = ""
            Public Property TeamB As String = ""
            Public Property PosA() As Integer = 0
            Public Property PosGroupA() As Integer = 0
            Public Property PosB() As Integer = 0
            Public Property PosGroupB() As Integer = 0
            Public Property Rating() As Integer = 0

        End Class
    End Class

End Namespace