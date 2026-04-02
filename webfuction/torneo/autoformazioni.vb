Imports System.Collections.Concurrent
Imports System.Data
Imports System.Security.Cryptography
Imports Newtonsoft.Json.Linq
Imports webfuction.Torneo.ProbablePlayers


Namespace Torneo
    Public Class AutoFormazioniData

        Dim appSett As New PublicVariables
        Dim daydata As Integer = -10

        Dim probable As New Dictionary(Of String, Torneo.ProbablePlayers.Probable)
        Dim probableloaded As Boolean = False
        Dim preanalisiphase As Boolean = False
        Dim modules As New Dictionary(Of String, String)
        Dim dicPlayers As New Dictionary(Of String, Torneo.Players.PlayerQuotesItem)

        Public Property MaxDayInArchive As Integer = -1
        Public Property EnableAvarangePointsRanking As Boolean = True 'rank punti ultime giornate'
        Public Property EnablePostionRanking As Boolean = True 'rank determinato sulle posizioni in classifica delle scquadre coivolte nel match del giocatore'
        Public Property EnableBonusRanking As Boolean = True 'rank goal e assist'
        Public Property EnableLastPresenceRanking As Boolean = True 'rank relativo ai minuti giocati nelle ultime giornate'
        Public Property EnableRoleRanking As Boolean = True 'rank ruolo giocatore'
        Public Property EnableProbable As Boolean = True
        Public Property EanblePreanalisys As Boolean = True
        Public Property FidexRoleSubstitutes As Boolean = False
        Public Property startAvgPtRank As Integer = 85
        Public Property startMatchRank As Integer = 85
        Public Property defMatchHistory As Integer = 20
        Public Property startTeamRank As Integer = 10
        Public Property startRouleRank As Integer = 15
        Public Property defPlayerHistory As Integer = 10

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function GetApiFormazioneAutomatica(Day As String, TeamId As String, Optional SetBestValueFromHistory As Boolean = True) As String

            Dim json As String = ""

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Richiesta formazioni automatiche giornata: " & Day & " per il team: " & TeamId)

            Functions.EnableQueryCache = True
            Functions.ClearQueryCache()

            Try
                Dim list As New List(Of AutoFormazione)
                For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                    If TeamId = "-1" OrElse i.ToString() = TeamId Then
                        list.Add(GetFormazioneAutomatica(CInt(Day), i, SetBestValueFromHistory))
                    End If
                Next
                Dim dicForma As Dictionary(Of String, FormazioniData.Formazione) = list.ToDictionary(Function(x) x.Formazione.TeamId.ToString(), Function(x) x.Formazione)
                Return WebData.Functions.SerializzaOggetto(dicForma, True)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Functions.EnableQueryCache = False

            Return json

        End Function

        Public Sub LoadProbable(Giornata As Integer)

            Dim probdata As New Torneo.ProbablePlayers(appSett)
            probable = probdata.GetProbableFormation("", Giornata)
            modules = probdata.GetTeamModule(probable)

            Dim pq As New WebData.PlayersQuotes(appSett)
            Dim fname As String = pq.GetBakupDataFileName(Giornata)

            If IO.File.Exists(fname) = False Then fname = pq.GetDataFileName()
            If IO.File.Exists(fname) Then
                Dim playersq As List(Of Torneo.Players.PlayerQuotesItem) = WebData.Functions.DeserializeJson(Of List(Of Torneo.Players.PlayerQuotesItem))(System.IO.File.ReadAllText(fname))
                For Each p As Torneo.Players.PlayerQuotesItem In playersq
                    If dicPlayers.ContainsKey(p.Nome) = False Then dicPlayers.Add(p.Nome, p)
                Next
            End If

            probableloaded = True
        End Sub

        Sub SetProbable(ProbableFormation As Dictionary(Of String, Torneo.ProbablePlayers.Probable), ModuleFormation As Dictionary(Of String, String), dPlayers As Dictionary(Of String, Torneo.Players.PlayerQuotesItem))
            probable = ProbableFormation
            modules = ModuleFormation
            dicPlayers = dPlayers
            probableloaded = True
        End Sub

        Sub GetFormazioniAutomatiche(ByVal Giornata As Integer)
            Dim formaList As New Dictionary(Of Integer, AutoFormazione)
            For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                formaList.Add(i, GetFormazioneAutomatica(i, Giornata))
            Next
        End Sub

        Public Function GetFormazioneAutomatica(ByVal Giornata As Integer, ByVal IdTeam As Integer, Optional SetBestValueFromHistory As Boolean = True) As AutoFormazione

            Try

                If Me.EnableProbable AndAlso probableloaded = False Then
                    LoadProbable(Giornata)
                End If

                If MaxDayInArchive = -1 Then CheckMaxDayData()

                'If SetBestValueFromHistory Then SetBestHistoricalValue(Giornata, IdTeam)

                Dim para As New AutoFormazione.ParamenterValues
                SetDayData(Giornata)
                If Giornata > 1 AndAlso Me.EanblePreanalisys Then
                    para = PreAnalisi(Giornata, IdTeam)
                End If

                preanalisiphase = False

                Dim autoForma As AutoFormazione = GetInternalFormazioneAutomatica(Giornata, IdTeam, para)

                Return autoForma

            Catch ex As Exception
                Return New AutoFormazione
            End Try

        End Function

        Private Function PreAnalisi(ByVal Giornata As Integer, ByVal IdTeam As Integer) As AutoFormazione.ParamenterValues

            Dim Parameters As List(Of AutoFormazione.ParamenterValues) = GetDefaultParametersList(True)
            Dim oldparamters As New List(Of String)

            preanalisiphase = True

            Dim results As List(Of AutoFormazione) = GetData(Giornata, IdTeam, Parameters, True)

            If results.Count > 0 Then
                Parameters = results.Select(Function(x) x.Parameters).ToList()
                Dim maxValues As List(Of Integer) = Parameters.Select(Function(x) x.Points).Distinct().ToList().OrderByDescending(Function(x) x).Take(2).ToList()
                Dim tmp As New List(Of AutoFormazione.ParamenterValues)
                For Each maxValue As Integer In maxValues
                    tmp.AddRange(Parameters.Where(Function(x) x.Points = maxValue))
                    If tmp.Count > 1 Then Exit For
                Next
                Parameters = GetBestParamentersByDecisionTree(tmp, 1)
                Return Parameters(0)
            Else
                Return New AutoFormazione.ParamenterValues
            End If

        End Function

        Private Function GetDefaultParametersList(Limited As Boolean) As List(Of AutoFormazione.ParamenterValues)

            Dim defPara As New AutoFormazione.ParamenterValues
            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)
            Dim teamrankList As New List(Of Integer) From {14}
            Dim historicalList As New List(Of Integer) From {defPlayerHistory}
            Dim historicalWeightList As New List(Of Integer) From {defPara.HistoricalPlayerWeight}
            Dim historicalMatchList As New List(Of Integer) From {defMatchHistory}
            Dim matchWidthList As New List(Of Integer) From {startMatchRank, startMatchRank + 10, startMatchRank + 20}
            Dim AvarangePointWidthList As New List(Of Integer) From {startAvgPtRank, startAvgPtRank + 10, startAvgPtRank + 20}
            Dim LastPresenzeWidthList As New List(Of Integer) From {0, 10}
            Dim ruolorankList As New List(Of Integer) From {startRouleRank, startRouleRank + 5, startRouleRank + 10, startRouleRank + 15}

            For Each tr As Integer In teamrankList
                For Each rr As Integer In ruolorankList
                    For Each histd As Integer In historicalList
                        For Each histdw As Integer In historicalWeightList
                            For Each histm As Integer In historicalMatchList
                                For Each matchw As Integer In matchWidthList
                                    For Each avgw As Integer In AvarangePointWidthList
                                        For Each lastw As Integer In LastPresenzeWidthList
                                            Dim para As New AutoFormazione.ParamenterValues
                                            para.Preanalisi = True
                                            para.HistoricalPlayerData = histd
                                            para.HistoricalMatchData = histm
                                            para.HistoricalPlayerWeight = histdw
                                            para.AvarangePointsWitdh = avgw
                                            para.MatchWidth = matchw
                                            para.LastPresenceWitdh = lastw
                                            para.TeamWidth = tr
                                            para.RuoloWidth = rr
                                            Parameters.Add(para)
                                        Next
                                    Next
                                Next
                            Next
                        Next
                    Next
                Next
            Next

            Return Parameters

        End Function

        Private Function GetBestParamentersByDecisionTree(paramsList As List(Of AutoFormazione.ParamenterValues), numberOfMaxItems As Integer) As List(Of AutoFormazione.ParamenterValues)

            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)
            Dim items As List(Of String) = paramsList.Select(Function(x) x.GetKey()).ToList()
            Dim numberfields As Integer = items(0).Split("|"c).Count

            Dim dec As New Torneo.AutoFormazioniData.DecisionTree
            Dim freqItems = dec.FrequenzeGerarchichePerPrefisso(items)

            ' Ordino prima per lunghezza (numero di elementi), poi per frequenza discendente
            Dim ordItems = freqItems.OrderBy(Function(kv) kv.Key.Count(Function(c) c = "!"c)).ThenByDescending(Function(kv) kv.Value)
            Dim puntiRami As New Dictionary(Of String, Integer)

            'vedo i rami con elementi con più occorrenze'
            For Each kv In ordItems

                Dim puntiRamo As Integer = 0
                Dim nFields As Integer = kv.Key.Split(CChar("|")).Count

                If kv.Key.Contains("|") Then
                    Dim pkey As String = kv.Key.Substring(0, kv.Key.LastIndexOf("|"))
                    If puntiRami.ContainsKey(pkey) Then puntiRamo = puntiRami(pkey)
                End If
                Dim pt As Integer = puntiRamo + kv.Value
                puntiRami.Add(kv.Key, pt)

            Next

            'Determino il punteggio massimo dei rami'
            Dim puntiMax As Integer = puntiRami.Select(Function(x) x.Value).Max

            'Determino i rami con il punteggio massimo'
            Dim bestRami = puntiRami.Where(Function(x) x.Value = puntiMax).Select(Function(x) x.Key).OrderBy(Function(x) x).ToList()

            'Converto i risultati in parametri'
            Dim npara As Integer = 0
            For Each ramo In bestRami
                Dim para As New AutoFormazione.ParamenterValues
                para.SetFromKey(ramo)
                Parameters.Add(para)
                npara += 1
                If numberOfMaxItems <> -1 AndAlso npara > numberOfMaxItems Then Exit For
            Next

            Return Parameters

        End Function

        Private Sub SetBestHistoricalValue(Giornata As Integer, Team As Integer)

            Dim fname As String = GetBestHistoricalFileName(Giornata)

            startMatchRank = -1

            If IO.File.Exists(fname) Then
                Dim lines As List(Of String) = System.IO.File.ReadAllLines(fname).ToList()
                For Each line As String In lines
                    Dim skey() As String = line.Split(CChar(vbTab))
                    If skey.Length = 2 AndAlso CInt(skey(0)) = Team Then
                        startMatchRank = CInt(skey(1))
                    End If
                Next
            End If

            If startMatchRank = -1 Then startMatchRank = 95

        End Sub

        Public Function BestHistoricalParamenters(Giornata As Integer) As Dictionary(Of Integer, Dictionary(Of String, Integer))

            Dim sout As New System.Text.StringBuilder
            Dim fout As String = GetBestHistoricalFileName(Giornata)
            Dim res As New Dictionary(Of Integer, Dictionary(Of String, Integer))

            If IO.File.Exists(fout) = False Then
                For idteam As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                    res.Add(idteam, New Dictionary(Of String, Integer))
                    'Dim avgValue As Integer = BestHistoricalParamenters(Giornata, idteam, "BOTH", New List(Of Integer) From {85, 90})
                    'res(idteam).Add("both", avgValue)
                    ''res(idteam).Add("hp", avgValue)
                    ''startAvgPtRank = avgValue
                    'If avgValue <> 80 Then
                    '    avgValue = avgValue
                    'End If
                    Dim matchValue As Integer = BestHistoricalParamenters(Giornata, idteam, "MATCH", New List(Of Integer) From {85, 90, 95})
                    res(idteam).Add("match", matchValue)
                    ' sout.AppendLine(idteam & vbTab & avgValue)

                Next
                'System.IO.File.AppendAllText(fout, sout.ToString())
            End If

            Return res

        End Function

        Public Function BestHistoricalParamenters(Giornata As Integer, Team As Integer, ParaName As String, ParaRange As List(Of Integer)) As Integer

            If MaxDayInArchive = -1 Then CheckMaxDayData()

            Dim dicPt As New Dictionary(Of Integer, Integer)
            For Each Value As Integer In ParaRange
                If ParaName = "MATCH" Then
                    startMatchRank = Value
                ElseIf ParaName = "AVG" Then
                    startAvgPtRank = Value
                ElseIf ParaName = "BOTH" Then
                    startAvgPtRank = Value
                    startMatchRank = Value
                End If
                Dim ptpre As Integer = BestHistoricalParamentersByDay(Giornata, Team)
                If ptpre > -1 Then dicPt.Add(Value, ptpre)
            Next

            Dim pt As Integer = 0
            If dicPt.Count > 0 Then
                Dim maxValue As Integer = dicPt.Values.Max()
                Dim BestValue As Integer = CInt(dicPt.Where(Function(kv) kv.Value = maxValue).Select(Function(kv) kv.Key).ToList()(0))
                Return BestValue
            Else
                Return -1
            End If

        End Function

        Private Function BestHistoricalParamentersByDay(Giornata As Integer, idTeam As Integer) As Integer

            Dim pttot As Integer = 0
            Dim gioStart = Giornata - 3

            If Giornata >= 20 AndAlso gioStart < 20 Then gioStart = 20
            If gioStart < 1 Then gioStart = 1

            If Giornata - 1 > gioStart Then
                For gio As Integer = gioStart To Giornata - 1
                    pttot += BestHistoricalParamentersGetPoints(gio, idTeam)
                Next
                Return pttot
            Else
                Return -1
            End If

        End Function

        Private Function BestHistoricalParamentersGetPoints(Giornata As Integer, idTeam As Integer) As Integer

            Dim data As New Torneo.FormazioniData(appSett)
            Dim comp As New Torneo.CompilaData(appSett)
            Dim probdata As New Torneo.ProbablePlayers(appSett)
            Dim result As New Torneo.AutoFormazioniData.AutoFormazione
            Dim probableForm As Dictionary(Of String, Torneo.ProbablePlayers.Probable) = probdata.GetProbableFormation("", Giornata)
            Dim probableModule As Dictionary(Of String, String) = probdata.GetTeamModule(probableForm)

            Dim pq As New WebData.PlayersQuotes(appSett)
            Dim fname As String = pq.GetBakupDataFileName(Giornata)

            If IO.File.Exists(fname) = False Then fname = pq.GetDataFileName()
            If IO.File.Exists(fname) Then
                Dim playersq As List(Of Torneo.Players.PlayerQuotesItem) = WebData.Functions.DeserializeJson(Of List(Of Torneo.Players.PlayerQuotesItem))(System.IO.File.ReadAllText(fname))
                For Each p As Torneo.Players.PlayerQuotesItem In playersq
                    If dicPlayers.ContainsKey(p.Nome) = False Then dicPlayers.Add(p.Nome, p)
                Next
            End If

            Dim autoForma As New Torneo.AutoFormazioniData(appSett)
            autoForma.SetProbable(probableForm, probableModule, dicPlayers)
            autoForma.MaxDayInArchive = MaxDayInArchive
            autoForma.startMatchRank = startMatchRank
            autoForma.startAvgPtRank = startAvgPtRank
            autoForma.defPlayerHistory = defPlayerHistory
            result = autoForma.GetFormazioneAutomatica(Giornata, idTeam, False)

            Dim dicpt As Dictionary(Of String, Integer) = GetPlayerPuntiData(Giornata, idTeam)

            For Each p As Torneo.FormazioniData.PlayerFormazione In result.Formazione.Players
                If dicpt.ContainsKey(p.Nome) Then p.Punti = dicpt(p.Nome)
            Next
            result.Formazione = comp.CompileDataForma(result.Formazione, False)
            data.CalculatePuntiFormazione(result.Formazione)

            Return result.Formazione.Punti

        End Function

        Public Function GetBestHistoricalFileName(Giornata As Integer) As String
            Dim dirf As String = appSett.WebDataPath & "data\autoforma"
            If System.IO.Directory.Exists(dirf) = False Then System.IO.Directory.CreateDirectory(dirf)
            Return dirf & "\autoformabest_" & Giornata & ".txt"
        End Function

        Public Sub CheckMatchResult(Giornata As Integer)

            LoadProbable(Giornata)

            Dim prev = GetMatchResult(Giornata, 20)

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("Select teama,teamb,goala,goalb")
            sqlstr.AppendLine("FROM tbmatch")
            sqlstr.AppendLine("WHERE gio=" & Giornata & "")
            sqlstr.AppendLine("ORDER BY idmatch")

            Dim a As String = sqlstr.ToString()
            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            If ds.Tables.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim row As DataRow = ds.Tables(0).Rows(i)

                    Dim teama As String = Functions.ReadFieldStringData("teama", row, "")
                    Dim teamb As String = Functions.ReadFieldStringData("teamb", row, "")
                    Dim goala As Integer = Functions.ReadFieldIntegerData("goala", row, 0)
                    Dim goalb As Integer = Functions.ReadFieldIntegerData("goalb", row, 0)
                    Dim res1 As String = "X"
                    Dim res2 As String = "X"
                    Dim ninfa As Integer = GetPercentageOfUnavailables(teama)
                    Dim ninfb As Integer = GetPercentageOfUnavailables(teamb)

                    If goala > goalb Then
                        res1 = "1"
                    ElseIf goalb > goala Then
                        res1 = "2"
                    End If
                    If prev(teama) > prev(teamb) + 15 Then
                        res2 = "1"
                    ElseIf prev(teamb) > prev(teama) + 15 Then
                        res2 = "2"
                    End If
                    System.Diagnostics.Debug.WriteLine(Giornata & vbTab & teama & vbTab & teamb & vbTab & goala & vbTab & goalb & vbTab & res1 & vbTab & res2 & vbTab & prev(teama) & vbTab & prev(teamb) & vbTab & ninfa & vbTab & ninfb)
                Next
            End If
        End Sub

        Private Function GetMatchResult(Giornata As Integer, HistoricalMatchData As Integer) As Dictionary(Of String, Double)

            Dim dicResultMatch As New Dictionary(Of String, Double)
            Dim dicFactTeam As Dictionary(Of String, Dictionary(Of String, Double)) = GetMatchRankData(HistoricalMatchData)
            Dim matchs = GetMatchData(Giornata)
            Dim homeGap As Integer = 0

            For Each m As String In matchs.Keys

                Dim ta As String = matchs(m)(0)
                Dim tb As String = matchs(m)(1)

                Dim valueA As Double = GetMatchResultProbability(ta, tb, dicFactTeam) + homeGap
                Dim valueB As Double = GetMatchResultProbability(tb, ta, dicFactTeam) - homeGap
                Dim infGap As Integer = 7
                Dim ninfa As Integer = GetPercentageOfUnavailables(ta)
                Dim ninfb As Integer = GetPercentageOfUnavailables(tb)

                If Math.Abs((ninfb - ninfa)) > infGap AndAlso (ninfa > 15 OrElse ninfb > 15) AndAlso Math.Abs(valueA - valueB) < 20 Then
                    valueA += (ninfb - ninfa) * 0.8
                    valueB += (ninfa - ninfb) * 0.8
                End If

                dicResultMatch.Add(ta, valueA)
                dicResultMatch.Add(tb, valueB)

            Next

            Return dicResultMatch

        End Function

        Private Function GetData(Giornata As Integer, IdTeam As Integer, Parameters As List(Of AutoFormazione.ParamenterValues), Compile As Boolean) As List(Of AutoFormazione)

            Dim comp As New Torneo.CompilaData(appSett)
            Dim data As New Torneo.FormazioniData(appSett)

            Dim results As New System.Collections.Concurrent.ConcurrentBag(Of AutoFormazione)

            Dim ruoliw As List(Of Integer) = Parameters.Select(Function(x) x.RuoloWidth).Distinct().ToList()
            Dim teamsw As List(Of Integer) = Parameters.Select(Function(x) x.TeamWidth).Distinct().ToList()
            Dim histd As List(Of Integer) = Parameters.Select(Function(x) x.HistoricalPlayerData).Distinct().ToList()
            Dim histdw As List(Of Integer) = Parameters.Select(Function(x) x.HistoricalPlayerWeight).Distinct().ToList()
            Dim histm As List(Of Integer) = Parameters.Select(Function(x) x.HistoricalMatchData).Distinct().ToList()
            Dim gruppi = Parameters.Select(Function(x, i) New With {.Item = x, .Index = i}).GroupBy(Function(x) x.Index \ 10).Select(Function(g) g.Select(Function(x) x.Item).ToList()).ToList()

            Dim teamrank As New Dictionary(Of Integer, Dictionary(Of String, Dictionary(Of String, Double)))
            Dim ruolorank As New Dictionary(Of Integer, Dictionary(Of String, Double))
            Dim dicFactTeam As New Dictionary(Of Integer, Dictionary(Of String, Dictionary(Of String, Double)))
            Dim matchrank As New Dictionary(Of Integer, Dictionary(Of String, Double))

            For Each tw As Integer In teamsw
                teamrank.Add(tw, GetTeamRankData(tw))
            Next

            For Each rw As Integer In ruoliw
                ruolorank.Add(rw, GetRuoloRankData(rw))
            Next

            For Each hm As Integer In histm
                dicFactTeam.Add(hm, GetMatchRankData(hm))
                matchrank.Add(hm, GetMatchResult(Giornata, hm))
            Next

            Dim plist As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, List(Of PlayerAutoFormazione))))

            For Each hw As Integer In histdw
                plist.Add(hw, New Dictionary(Of Integer, Dictionary(Of Integer, List(Of PlayerAutoFormazione))))
                For Each h As Integer In histd
                    plist(hw).Add(h, New Dictionary(Of Integer, List(Of PlayerAutoFormazione)))
                    For Each hm As Integer In histm
                        plist(hw)(h).Add(hm, GetPlayerStatisticData(Giornata, IdTeam, New AutoFormazione.ParamenterValues(h, hw, hm), dicFactTeam(hm), matchrank(hm)))
                    Next
                Next
            Next

            For Each gr As List(Of AutoFormazione.ParamenterValues) In gruppi

                Dim paraListIndex As New List(Of Integer)
                Dim paraList As New List(Of AutoFormazione.ParamenterValues)

                For ind As Integer = 0 To gr.Count - 1
                    paraList.Add(Torneo.Functions.Clone(gr(ind)))
                    paraListIndex.Add(ind)
                Next

                System.Threading.Tasks.Parallel.ForEach(paraListIndex, Sub(n)

                                                                           Dim para As AutoFormazione.ParamenterValues = Torneo.Functions.Clone(paraList(n))
                                                                           Dim plistLoc = Torneo.Functions.Clone(plist(para.HistoricalPlayerWeight)(para.HistoricalPlayerData)(para.HistoricalMatchData))
                                                                           Dim teamRankLoc = Torneo.Functions.Clone(teamrank(para.TeamWidth))
                                                                           Dim ruoloRankLoc = Torneo.Functions.Clone(ruolorank(para.RuoloWidth))
                                                                           Dim autoForma As AutoFormazione = GetInternalFormazioneAutomatica(Giornata, IdTeam, para, plistLoc, teamRankLoc, ruoloRankLoc)
                                                                           autoForma.Formazione.Players.RemoveAll(Function(x) x.Type = 0)

                                                                           If Compile AndAlso Giornata <= MaxDayInArchive AndAlso autoForma.Formazione.Players.Count > 0 Then
                                                                               Dim ptmin As Integer = autoForma.Formazione.Players.Select(Function(x) x.Punti).ToList().Min
                                                                               For Each p As Torneo.FormazioniData.PlayerFormazione In autoForma.Formazione.Players
                                                                                   If p.Punti = 0 Then p.Punti = ptmin
                                                                                   If p.Ruolo = "P" Then p.Punti = 60
                                                                               Next
                                                                               autoForma.Formazione = comp.CompileDataForma(autoForma.Formazione, False)
                                                                               data.CalculatePuntiFormazione(autoForma.Formazione)
                                                                           End If

                                                                           autoForma.Parameters = para
                                                                           autoForma.Parameters.Points = autoForma.Formazione.Punti
                                                                           results.Add(autoForma)

                                                                       End Sub)
            Next

            Return results.ToList()

        End Function

        Private Function GetInternalFormazioneAutomatica(ByVal Giornata As Integer, ByVal IdTeam As Integer, Parameters As AutoFormazione.ParamenterValues) As AutoFormazione
            Dim dicFactTeam = GetMatchRankData(Parameters.HistoricalMatchData)
            Dim dicMatchRank As Dictionary(Of String, Double) = GetMatchResult(Giornata, Parameters.HistoricalMatchData)
            Dim plist As List(Of PlayerAutoFormazione) = GetPlayerStatisticData(Giornata, IdTeam, Parameters, dicFactTeam, dicMatchRank)
            Dim dicTeamRank As Dictionary(Of String, Dictionary(Of String, Double)) = GetTeamRankData(Parameters.TeamWidth)
            Dim dicRuoloRank As Dictionary(Of String, Double) = GetRuoloRankData(Parameters.RuoloWidth)
            Return GetInternalFormazioneAutomatica(Giornata, IdTeam, Parameters, plist, dicTeamRank, dicRuoloRank)
        End Function

        Private Function GetInternalFormazioneAutomatica(ByVal Giornata As Integer, ByVal IdTeam As Integer, Parameters As AutoFormazione.ParamenterValues, plist As List(Of PlayerAutoFormazione), dicTeamRank As Dictionary(Of String, Dictionary(Of String, Double)), dicRuoloRank As Dictionary(Of String, Double)) As AutoFormazione

            Dim autoForma As New AutoFormazione
            autoForma.Formazione.Giornata = Giornata
            autoForma.Formazione.TeamId = IdTeam
            autoForma.Parameters = Parameters
            autoForma.PlayerRating = New List(Of PlayerAutoFormazione)
            Try
                plist = CalculatePlayersRating(Giornata, Parameters, plist, dicTeamRank, dicRuoloRank)
                autoForma.PlayerRating = Torneo.Functions.Clone(plist)
                autoForma.Formazione.Players = GetFormazioneFinale(plist)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return autoForma

        End Function

        Public Sub SetDayData(Giornata As Integer)

            daydata = Giornata - 1

            If Giornata > MaxDayInArchive + 1 Then
                daydata = MaxDayInArchive
            End If

        End Sub

        Public Sub SetDayDataForce(Giornata As Integer)
            daydata = Giornata - 1
        End Sub

        Public Sub CheckMaxDayData()
            MaxDayInArchive = GetMaxDayData()
        End Sub

        Public Function GetMaxDayData() As Integer
            Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "Select max(gio) As gio FROM tbdati")
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                Return CInt(ds.Tables(0).Rows(0)("gio"))
            Else
                Return 0
            End If
        End Function

        Private Function GetPlayerStatisticData(Giornata As Integer, IdTeam As Integer, Parameters As AutoFormazione.ParamenterValues, dicFactTeam As Dictionary(Of String, Dictionary(Of String, Double)), dicMatchRank As Dictionary(Of String, Double)) As List(Of PlayerAutoFormazione)

            Dim fc As New List(Of PlayerAutoFormazione)
            Dim nday As Integer = If(daydata > Parameters.HistoricalPlayerData, Parameters.HistoricalPlayerData, daydata)
            Dim a As String = ""
            Dim enaVariousWeight As Boolean = True
            Dim minData As Integer = daydata - Parameters.HistoricalPlayerData

            If minData < 1 Then minData = 1

            Dim var0 As Integer = If(daydata >= Parameters.HistoricalPlayerData, Parameters.HistoricalPlayerData, daydata)
            Dim var1 As Integer = Parameters.HistoricalPlayerWeight
            var1 = 8

            Dim varp As String

            var0 = If(daydata >= Parameters.HistoricalPlayerData, Parameters.HistoricalPlayerData, daydata)
            If var0 < 2 Then var0 = 2

            varp = "(-" & ((var0 - var0 / var1) / (var0 - 1)).ToString().Replace(",", ".") & "*(" & daydata & "-tbd.gio)+" & var0 & ")/" & var0

            If enaVariousWeight = False Then
                varp = "1"
            End If

            Try
                '************************************************************************************************
                '** i seguenti paramentri servono solo se si desidera compilare una formazione                 **
                '** automatica delle giornate passate, perchè a sistema non è presente uno storico delle rose  **
                '************************************************************************************************
                Dim tbref As String = If(Giornata > MaxDayInArchive, "tbrose", "tbformazioni")
                Dim sqf As String = If(Giornata > MaxDayInArchive, "tb.sqp", "tb.sqf")
                Dim tbwhere As String = If(Giornata > MaxDayInArchive, "tb.idteam=" & IdTeam, "tb.idteam=" & IdTeam & " And tb.gio=" & Giornata & " And tb.type<3")
                Dim dec As String = "exp(-0.2*(" & daydata & "-tb.gio))"

                If daydata = 0 Then

                    Dim sqlstr As New Text.StringBuilder
                    sqlstr.AppendLine("   Select tb.*,tbp.ruolomantras,iif(tb.sqf Is null Or tb.sqf='',tbp.squadra,tb.sqf) as sqp FROM (")
                    sqlstr.AppendLine("     SELECT tb.idrosa,tb.ruolo,tb.nome," & If(tbref = "tbrose", "null", "tb.squadra") & " as sqf")
                    sqlstr.AppendLine("     FROM " & tbref & " as tb")
                    sqlstr.AppendLine("     WHERE " & tbwhere & ") as tb")
                    sqlstr.AppendLine("   LEFT JOIN tbplayer as tbp on tbp.nome=tb.nome")

                    a = sqlstr.ToString()

                    Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

                    If ds.Tables.Count > 0 Then
                        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            Dim row As DataRow = ds.Tables(0).Rows(i)
                            Dim p As New PlayerAutoFormazione
                            p.RosaId = Functions.ReadFieldIntegerData("idrosa", row, 0)
                            p.Ruolo = Functions.ReadFieldStringData("ruolo", row, "")
                            p.RuoloMantra = Functions.ReadFieldStringData("ruolomantras", row, "")
                            p.Nome = Functions.ReadFieldStringData("nome", row, "")
                            p.Squadra = Functions.ReadFieldStringData("sqp", row, "")
                            If p.Nome = "FABBIAN" Then
                                p.Nome = p.Nome
                            End If
                            fc.Add(p)
                        Next
                    End If

                Else

                    Dim sqlstr As New Text.StringBuilder
                    sqlstr.AppendLine("  SELECT tb.*,tbm.teama,teamb,iif(tb.sqp=teama,1,0) as home,timem,iif(CDate(timem)>Now(),1,0) as available,DateDiff('h', Now(), CDate(timem)) AS tleft FROM (")
                    sqlstr.AppendLine("   SELECT tb.*,tbp.ruolomantras,tbp.qcur,iif(tb.sqf is null or tb.sqf='',tbp.squadra,tb.sqf) as sqp FROM (")
                    sqlstr.AppendLine("    SELECT tb.idrosa,tb.ruolo, tb.nome,tb.sqf,sum(tb.amm) as amm,sum(tb.gf) as gf,sum(tb.gs) as gs,sum(tb.ass) as ass,sum(tb.rigt) as rigt,IIf(Sum(tb.pt)>0,CInt(avg(tb.pt)),0) AS avg_pt,sum(tb.ptn) as ptn,IIf(Sum(tb.ptn)>0,CInt(avg(tb.ptn)),0) AS avg_ptn, IIf(Sum(tb.voto)>0,CInt(Avg(tb.voto)),0) AS avg_vt, Count(*) AS pgio, Sum(tbt.tit) AS tit, Sum(tbt.sos) AS sos, Sum(tbt.sub) AS sub, Sum(tbt.mm*" & varp & ") AS mm, iif(Sum(tbt.mm) > 0,CInt (Sum(tbt.mm)) / " & var0 & ",0 ) AS avg_mm FROM (")
                    sqlstr.AppendLine("     SELECT tb.idrosa,tb.ruolo,tb.nome," & If(tbref = "tbrose", "null", "tb.squadra") & " as sqf,tbd.gio,tbd.amm as amm,tbd.gf as gf,tbd.gs as gs,tbd.ass as ass,tbd.rigt as rigt,tbd.pt,(tbd.voto+tbd.gf*30+tbd.ass*10-tbd.gs*10) as ptn,tbd.voto")
                    sqlstr.AppendLine("     FROM " & tbref & " as tb")
                    sqlstr.AppendLine("     LEFT JOIN tbdati as tbd on (tbd.nome=tb.nome AND tbd.pt > -100 AND tbd.gio >" & daydata - Parameters.HistoricalPlayerData & " and tbd.gio<=" & daydata & ")")
                    sqlstr.AppendLine("     WHERE " & tbwhere & ") as tb")
                    sqlstr.AppendLine("    LEFT JOIN tbtabellini AS tbt ON (tb.gio = tbt.gio) AND (tb.nome = tbt.nome)")
                    sqlstr.AppendLine("    GROUP BY tb.idrosa,tb.ruolo,tb.nome,tb.sqf) as tb")
                    sqlstr.AppendLine("   LEFT JOIN tbplayer as tbp on tbp.nome=tb.nome) as tb")
                    sqlstr.AppendLine("  LEFT JOIN tbmatch as tbm ON (tbm.gio = " & Giornata & " AND (tb.sqp = tbm.teama OR tb.sqp = tbm.teamb))")

                    a = sqlstr.ToString()

                    Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

                    If ds.Tables.Count > 0 Then
                        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            Dim row As DataRow = ds.Tables(0).Rows(i)
                            Dim p As New PlayerAutoFormazione
                            p.RosaId = Functions.ReadFieldIntegerData("idrosa", row, 0)
                            p.Ruolo = Functions.ReadFieldStringData("ruolo", row, "")
                            p.RuoloValue = CInt(p.Ruolo.Replace("A", "1").Replace("C", "2").Replace("D", "3").Replace("P", "4"))
                            p.RuoloMantra = Functions.ReadFieldStringData("ruolomantras", row, "")
                            p.qCur = Functions.ReadFieldIntegerData("qcur", row, 0)
                            p.Nome = Functions.ReadFieldStringData("nome", row, "")
                            p.Squadra = Functions.ReadFieldStringData("sqp", row, "")
                            p.Amm = Functions.ReadFieldDoubleData("amm", row, 0)
                            p.Gf = Functions.ReadFieldDoubleData("gf", row, 0)
                            p.Gs = Functions.ReadFieldDoubleData("gs", row, 0)
                            p.Ass = Functions.ReadFieldDoubleData("ass", row, 0)
                            p.AvgPt = Functions.ReadFieldIntegerData("avg_pt", row, 0)
                            p.AvgVt = Functions.ReadFieldIntegerData("avg_vt", row, 0)
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

                            p.Gf = p.Gf / Parameters.HistoricalPlayerData
                            p.Ass = p.Ass / Parameters.HistoricalPlayerData

                            Dim avv As String = If(p.Squadra = p.TeamA, p.TeamB, p.TeamA)

                            p.FattoreSquadra = dicFactTeam(p.Squadra)("GF")
                            p.FattoreAvversaria = dicFactTeam(avv)("GS")

                            If dicMatchRank.ContainsKey(p.Squadra) Then p.PropGolFattiMatch = dicMatchRank(p.Squadra)
                            p.PropGolSubitiMatch = p.PropGolFattiMatch

                            p.goodGrade = GetGoodGradePrabability(Functions.ReadFieldDoubleData("avg_ptn", row, 0) / 10, p.pGiocate, p.Ruolo)

                            p.Rating.Rating3 = CInt((1 - Math.Exp(-p.FattoreSquadra * p.FattoreAvversaria * p.Gf)) * 25 + (1 - Math.Exp(-p.Ass)) * 18 + (1 - Math.Exp(-dicFactTeam("TOT")("AVG_GF") * p.FattoreSquadra * 3 * p.FattoreAvversaria)) * 2 - (1 - Math.Exp(-p.Amm)) * 1)
                            If p.Rating.Rating3 < 0 Then p.Rating.Rating3 = 0

                            fc.Add(p)
                        Next
                    End If
                End If

            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine(a)
            End Try

            Return fc

        End Function

        Private Function GetMatchResultProbability(Squadra As String, Avv As String, dicFactTeam As Dictionary(Of String, Dictionary(Of String, Double))) As Double

            If dicFactTeam.Count < 12 Then Return 66

            ' Forze offensive e difensive
            Dim forzaAttSquadra = dicFactTeam(Squadra)("GF") / dicFactTeam("TOT")("AVG_GF")
            Dim forzaDifSquadra = dicFactTeam(Squadra)("GS") / dicFactTeam("TOT")("AVG_GS")
            Dim forzaAttAvv = dicFactTeam(Avv)("GF") / dicFactTeam("TOT")("AVG_GF")
            Dim forzaDifAvv = dicFactTeam(Avv)("GS") / dicFactTeam("TOT")("AVG_GS")

            ' λ attesi
            Dim lambdaSquadra = forzaAttSquadra * forzaDifAvv * dicFactTeam("TOT")("AVG_GF")
            Dim lambdaAvv = forzaAttAvv * forzaDifSquadra * dicFactTeam("TOT")("AVG_GF")

            ' Limite massimo gol considerati
            Dim maxGol As Integer = 10

            Dim prob1 As Double = 0
            Dim probX As Double = 0
            Dim prob2 As Double = 0

            ' Matrice dei risultati
            For golSquadra = 0 To maxGol

                Dim pCasa = Functions.PoissonProb(golSquadra, lambdaSquadra)

                For golAvv = 0 To maxGol
                    Dim pTrasferta = Functions.PoissonProb(golAvv, lambdaAvv)
                    Dim pRisultato = pCasa * pTrasferta

                    If golSquadra > golAvv Then
                        prob1 += pRisultato
                    ElseIf golSquadra = golAvv Then
                        probX += pRisultato
                    Else
                        prob2 += pRisultato
                    End If
                Next
            Next

            Return (prob1 + probX) * 100

        End Function

        Private Function GetGoodGradePrabability(avgPtn As Double, pGio As Integer, pRuolo As String) As Double

            Dim pLessThan6 As Double = 0
            Dim goodGrade As Double = 0

            If pGio < 2 Then avgPtn = 6

            For k As Integer = 0 To 6
                pLessThan6 += Functions.PoissonProb(k, avgPtn)
            Next
            goodGrade = (1 - pLessThan6) * 100

            If pRuolo = "A" Then
                goodGrade = goodGrade * 0.9
            ElseIf pRuolo = "C" Then
                goodGrade = goodGrade * 0.98
            End If

            Return goodGrade

        End Function

        Private Function GetMatchRankData(HistoricalMatchData As Integer) As Dictionary(Of String, Dictionary(Of String, Double))

            If HistoricalMatchData > daydata Then
                HistoricalMatchData = daydata
            End If

            Dim dic1 As New Dictionary(Of String, Dictionary(Of String, Double))
            Dim avg_gf As Double = 0
            Dim avg_gfd As Double = 0
            Dim avg_gff As Double = 0
            Dim avg_gs As Double = 0
            Dim avg_gsd As Double = 0
            Dim avg_gsf As Double = 0
            Dim dec As String = "1" ' "exp(-0.02*(" & daydata & "-gio))"

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT squadra,iif(sum(tb.npd)+sum(tb.npf),(sum(tb.tgfd)+sum(tb.tgff))/(sum(tb.npd)+sum(tb.npf)),0) as gf,iif(sum(tb.npd)>0,sum(tb.tgfd)/sum(tb.npd),0) as gfd,iif(sum(tb.npf)>0,sum(tb.tgff)/sum(tb.npf),0) as gff,iif(sum(tb.npd)+sum(tb.npf)>0,(sum(tb.tgsd)+sum(tb.tgsf))/(sum(tb.npd)+sum(tb.npf)),0) as gs,iif(sum(tb.npd)>0,sum(tb.tgsd)/sum(tb.npd),0) as gsd,iif(sum(tb.npf)>0,sum(tb.tgsf)/sum(tb.npf),0) as gsf FROM (")
            sqlstr.AppendLine("SELECT teama as squadra,SUM(goala*" & dec & ") as tgfd,SUM(goalb*" & dec & ") as tgsd,'0' as tgff,'0' as tgsf,SUM(" & dec & ") as npd,'0' as npf FROM tbmatch WHERE gio>" & daydata - HistoricalMatchData & " AND gio<=" & daydata & " and goala<>'' and goalb<>'' GROUP BY teama")
            sqlstr.AppendLine("UNION")
            sqlstr.AppendLine("SELECT teamb as squadra,'0' as tgfd,'0' as tgsd,SUM(goalb*" & dec & ") as tgff,SUM(goala*" & dec & ") as tgsf,'0' as npd,SUM(" & dec & ") as npf FROM tbmatch WHERE gio>" & daydata - HistoricalMatchData & " AND gio<=" & daydata & " and goala<>'' and goalb<>'' GROUP BY teamb")
            sqlstr.AppendLine(") as tb")
            sqlstr.AppendLine("GROUP BY squadra")

            Dim a As String = sqlstr.ToString()
            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            dic1.Add("TOT", New Dictionary(Of String, Double))
            dic1("TOT").Add("AVG_GF", 0)
            dic1("TOT").Add("AVG_GS", 0)

            If ds.Tables.Count > 0 Then

                Dim n As Integer = 0
                Dim sgf As Double = 0
                Dim sgfd As Double = 0
                Dim sgff As Double = 0
                Dim sgs As Double = 0
                Dim sgsd As Double = 0
                Dim sgsf As Double = 0

                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                    Dim key As String = ds.Tables(0).Rows(i)("squadra").ToString()

                    Dim gf As Double = CDbl(ds.Tables(0).Rows(i)("gf"))
                    Dim gfd As Double = CDbl(ds.Tables(0).Rows(i)("gfd"))
                    Dim gff As Double = CDbl(ds.Tables(0).Rows(i)("gff"))

                    Dim gs As Double = CDbl(ds.Tables(0).Rows(i)("gs"))
                    Dim gsd As Double = CDbl(ds.Tables(0).Rows(i)("gsd"))
                    Dim gsf As Double = CDbl(ds.Tables(0).Rows(i)("gsf"))

                    If dic1.ContainsKey(key) = False Then dic1.Add(key, New Dictionary(Of String, Double))

                    dic1(key).Add("GF", gf)
                    dic1(key).Add("GFD", gfd)
                    dic1(key).Add("GFF", gff)

                    dic1(key).Add("GS", gs)
                    dic1(key).Add("GSD", gsd)
                    dic1(key).Add("GSF", gsf)

                    n += 1

                    sgf += gf
                    sgfd += gfd
                    sgff += gff

                    sgs += gs
                    sgsd += gsd
                    sgsf += gsf

                Next

                If n = 0 Then Return dic1

                avg_gf = sgf / n
                avg_gfd = sgfd / n
                avg_gff = sgff / n

                avg_gs = sgs / n
                avg_gsd = sgsd / n
                avg_gsf = sgsf / n

                dic1("TOT")("AVG_GF") = avg_gf
                dic1("TOT")("AVG_GFD") = avg_gfd
                dic1("TOT")("AVG_GFF") = avg_gff

                dic1("TOT")("AVG_GS") = avg_gs
                dic1("TOT")("AVG_GSD") = avg_gsd
                dic1("TOT")("AVG_GSF") = avg_gsf

                For Each key As String In dic1.Keys
                    If key <> "TOT" Then

                        dic1(key)("GF") = dic1(key)("GF") / avg_gf
                        dic1(key)("GFD") = dic1(key)("GFD") / avg_gfd
                        dic1(key)("GFF") = dic1(key)("GFF") / avg_gff

                        dic1(key)("GS") = dic1(key)("GS") / avg_gs
                        dic1(key)("GSD") = dic1(key)("GSD") / avg_gsd
                        dic1(key)("GSF") = dic1(key)("GSF") / avg_gsf

                    End If
                Next

            End If

            Return dic1

        End Function

        Private Function GetTeamRankData(teamWidth As Integer) As Dictionary(Of String, Dictionary(Of String, Double))

            Dim dicTeam As New Dictionary(Of String, Dictionary(Of String, Double))
            dicTeam.Add("TOT", New Dictionary(Of String, Double))
            dicTeam.Add("TOTD", New Dictionary(Of String, Double))
            dicTeam.Add("TOTF", New Dictionary(Of String, Double))

            If teamWidth = 0 Then Return dicTeam

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT squadra,(vitt) as tot,vittd as totd,vittf as totf FROM tbrank WHERE gio=" & daydata & "")

            Dim a As String = sqlstr.ToString()
            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            If ds.Tables.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim key As String = ds.Tables(0).Rows(i)("squadra").ToString()
                    Dim tot As Double = CDbl(ds.Tables(0).Rows(i)("tot").ToString())
                    Dim totd As Double = CDbl(ds.Tables(0).Rows(i)("totd").ToString())
                    Dim totf As Double = CDbl(ds.Tables(0).Rows(i)("totf").ToString())
                    If dicTeam("TOT").ContainsKey(key) = False Then dicTeam("TOT").Add(key, tot)
                    If dicTeam("TOTD").ContainsKey(key) = False Then dicTeam("TOTD").Add(key, totd)
                    If dicTeam("TOTF").ContainsKey(key) = False Then dicTeam("TOTF").Add(key, totf)
                Next
            End If

            Return dicTeam

        End Function

        Private Function GetRuoloRankData(ruoloWidth As Integer) As Dictionary(Of String, Double)

            Dim dicRuolo As New Dictionary(Of String, Double)
            If ruoloWidth = 0 Then Return dicRuolo

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT tbp.ruolo,tbp.ruolomantras as ruolom, avg(voto+gf*30+ass*10+rigs*30-amm*5) AS avgpt, count(*) AS tot")
            sqlstr.AppendLine("FROM tbdati AS tbd LEFT JOIN tbplayer AS tbp ON tbp.nome=tbd.nome")
            sqlstr.AppendLine("WHERE tbd.gio>" & daydata - 20 & " AND tbd.gio<=" & daydata & " and tbd.pt>-100 AND tbd.esp=0 AND tbp.ruolo<>'P'")
            sqlstr.AppendLine("GROUP BY tbp.ruolo,tbp.ruolomantras")

            Dim a As String = sqlstr.ToString()
            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            If ds.Tables.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim key As String = ds.Tables(0).Rows(i)("ruolo").ToString() & "-" & ds.Tables(0).Rows(i)("ruolom").ToString()
                    Dim avgpt As Double = CDbl(ds.Tables(0).Rows(i)("avgpt").ToString())
                    If dicRuolo.ContainsKey(key) = False Then dicRuolo.Add(key, avgpt)
                Next
            End If

            Return dicRuolo

        End Function

        Private Function CalculatePlayersRating(Giornata As Integer, Parameters As AutoFormazione.ParamenterValues, plist As List(Of PlayerAutoFormazione), dicTeamRank As Dictionary(Of String, Dictionary(Of String, Double)), dicRuoloRank As Dictionary(Of String, Double)) As List(Of PlayerAutoFormazione)

            Dim factTeam As New FactoryValues

            If dicTeamRank.Count > 0 AndAlso dicTeamRank("TOT").Values.Count > 0 Then
                factTeam.Min = dicTeamRank("TOT").Values.Select(Function(x) x).Min()
                factTeam.Max = dicTeamRank("TOT").Values.Select(Function(x) x).Max()
                factTeam.CalcolaFactory(Parameters.TeamWidth)
            End If

            Dim factRuolo As New FactoryValues
            If dicRuoloRank.Count > 0 Then
                factRuolo.Min = dicRuoloRank.Values.Select(Function(x) x).Min()
                factRuolo.Max = dicRuoloRank.Values.Select(Function(x) x).Max()
                factRuolo.CalcolaFactory(Parameters.RuoloWidth)
            End If

            For Each p As PlayerAutoFormazione In plist

                Dim ratProb As Double = GetProbableRating(p, Giornata)

                p.Rating.Rating6 = ratProb

                'calcolo rating 1'
                If Me.EnableAvarangePointsRanking Then
                    If p.Ruolo <> "P" Then p.Rating.Rating1 = CInt(p.goodGrade / 100 * Parameters.AvarangePointsWitdh)
                End If

                'calcolo rating 2'
                If Me.EnablePostionRanking Then
                    If p.Ruolo = "P" Then
                        p.Rating.Rating2 = CInt(p.PropGolSubitiMatch / 100 * Parameters.MatchWidth)
                    Else
                        p.Rating.Rating2 = CInt(p.PropGolFattiMatch / 100 * Parameters.MatchWidth)
                        If p.Ruolo = "D" Then
                            p.Rating.Rating2 = CInt(p.Rating.Rating2 * 1.05)
                        ElseIf p.Ruolo = "C" Then
                            p.Rating.Rating2 = CInt(p.Rating.Rating2 * 1)
                        Else
                            p.Rating.Rating2 = CInt(p.Rating.Rating2 * 0.9)
                        End If
                    End If
                End If

                'calcolo rating 3'
                If Me.EnableBonusRanking Then
                    'p.Rating.Rating3 = CInt((1 - Math.Exp(-p.FattoreSquadra * p.FattoreAvversaria * (p.Gf * 3))) * 25)
                End If

                ' calcolo rating 4'
                If Me.EnableLastPresenceRanking AndAlso p.Minuti > 0 And p.Ruolo <> "P" Then
                    p.Rating.Rating4 = CInt(p.Minuti / (90 * Parameters.HistoricalPlayerData) * Parameters.LastPresenceWitdh)
                End If

                ' calcolo rating 5'
                p.Rating.Rating5 = GetRoleRating(p, Parameters.RuoloWidth, factRuolo.Min, factRuolo.Fact, dicRuoloRank)

                Dim pteam As Integer = CInt(Math.Floor(If(factTeam.Fact = 0, Parameters.TeamWidth, factTeam.Fact * (dicTeamRank("TOT")(p.Squadra) - factTeam.Min))))

                If p.Ruolo = "P" Then
                    p.Rating.Rating8 = CInt(pteam * 0.55)
                ElseIf p.Ruolo = "D" Then
                    p.Rating.Rating8 = CInt(pteam * 0.85)
                ElseIf p.Ruolo = "C" Then
                    p.Rating.Rating8 = CInt(pteam * 1)
                Else
                    p.Rating.Rating8 = CInt(pteam * 1.1)
                End If

                p.Rating.Total2 = CInt(p.Rating.Rating1 + p.Rating.Rating2 + p.Rating.Rating3 + p.Rating.Rating4 + p.Rating.Rating5 + p.Rating.Rating7 + p.Rating.Rating8)

                If p.Ruolo = "P" Then
                    If p.Nome = "SOMMER" Then 'Inter'
                        p.Rating.Total2 += 30
                    ElseIf p.Nome = "MARTINEZ JO." Then
                        p.Rating.Total2 += 20
                    ElseIf p.Nome = "SILVAR" Then 'Roma
                        p.Rating.Total2 += 30
                    ElseIf p.Nome = "GOLLINI" Then
                        p.Rating.Total2 += 20
                    ElseIf p.Nome = "SKORUPSKI" Then 'Bologna
                        p.Rating.Total2 += 30
                    ElseIf p.Nome = "RAVAGLIA F." Then
                        p.Rating.Total2 += 20
                    ElseIf p.Nome = "DI GREGORIO" Then 'Juve'
                        p.Rating.Total2 += 30
                    ElseIf p.Nome = "PERIN" Then
                        p.Rating.Total2 += 20
                    ElseIf p.Nome = "MILINKOVIC SAVIC V." Then 'Lazio'
                        p.Rating.Total2 += 30
                    ElseIf p.Nome = "MERET" Then
                        p.Rating.Total2 += 20
                    ElseIf p.Nome = "CARNESECCHI" Then 'Atalanta'
                        p.Rating.Total2 += 30
                    ElseIf p.Nome = "ROSSI F." Then
                        p.Rating.Total2 += 20
                    End If
                End If

                If modules.ContainsKey(p.Squadra) Then
                    Dim moduleTeam As String = modules(p.Squadra)
                    If moduleTeam = "4-3-3" OrElse moduleTeam = "3-4-3" Then
                        p.Rating.Total2 += 3
                    ElseIf moduleTeam = "5-4-1" OrElse moduleTeam = "6-3-1" Then
                        p.Rating.Total2 -= 0
                    Else
                        p.Rating.Total2 += 0
                    End If
                End If

                Dim declassPt As Integer = DeclassPlayer(Giornata, p.Ruolo, p.Squadra)

                If declassPt <> 0 Then
                    If p.Rating.Total2 + declassPt < 30 Then
                        p.Rating.Total2 = 30
                    Else
                        p.Rating.Total2 += declassPt
                    End If
                End If

                p.Rating.Total1 = p.Rating.Total2

                If Me.EnableProbable AndAlso probable.Values.ToList().Where(Function(x) x.Day <> -1).Count > 0 Then

                    'Dim ratProb As Double = GetProbableRating(p)

                    p.Rating.Rating6 = ratProb

                    If ratProb = -10 Then
                        p.Rating.Total1 = -1
                        p.Rating.Total2 = -1
                    ElseIf ratProb > -1 Then

                        p.Rating.Total1 = CInt(p.Rating.Total2 * ratProb)

                        If ratProb <= 0.8 Then
                            p.Rating.Total2 = p.Rating.Total1
                        Else
                            ratProb += 0.1
                            If ratProb > 1 Then ratProb = 1
                            p.Rating.Total2 = CInt(p.Rating.Total2 * ratProb)
                        End If

                    End If

                End If

            Next

            plist = plist.OrderByDescending(Function(x) x.Rating.Total1).ThenBy(Function(x) x.RuoloValue).ThenByDescending(Function(x) x.Rating.Rating2).ThenByDescending(Function(x) x.Rating.Rating8).ToList()

            Return plist

        End Function

        Private Function GetPercentageOfUnavailables(squadra As String) As Integer

            Dim np As New List(Of String)
            Dim ninf As New List(Of String)
            Dim nsq As New List(Of String)

            For Each site As String In probable.Keys
                For Each keyp As String In probable(site).Players.Keys
                    If keyp.EndsWith("/" & squadra) Then
                        Dim pname As String = keyp.Split(CChar("/"))(0)
                        If dicPlayers.ContainsKey(pname) Then
                            If np.Contains(pname) = False Then np.Add(pname)
                            If probable(site).Players(keyp).State = "Infortunato" AndAlso probable(site).Players(keyp).Infortunio.Severity > 10 Then
                                If ninf.Contains(pname) = False Then ninf.Add(pname)
                            ElseIf probable(site).Players(keyp).State = "Squalificato" Then
                                If nsq.Contains(pname) = False Then nsq.Add(pname)
                            End If
                        End If

                    End If
                Next
            Next
            If np.Count > 0 Then
                Dim g As Integer = CInt((ninf.Count + nsq.Count) * 100 / np.Count)
                Return g
            Else
                Return 0
            End If

        End Function

        Private Function GetProbableRating(p As PlayerAutoFormazione, Giornata As Integer) As Double

            If Me.EnableProbable AndAlso probable.Values.ToList().Where(Function(x) x.Day <> -1).Count > 0 Then

                'se abilitato tengo conto anche delle probabili formazioni attuali'
                Dim val As Double = -10
                Dim nsitefound As Integer = 0
                Dim ntit As Integer = 0
                Dim npanc As Double = 0
                Dim ninf As Integer = 0
                Dim nsq As Integer = 0

                For Each site As String In probable.Keys

                    Dim keyp As String = p.Nome & "/" & p.Squadra

                    If probable(site).Players.ContainsKey(keyp) Then
                        nsitefound += 1
                        If val = -10 Then val = 0
                        If probable(site).Players(keyp).State = "Titolare" Then
                            ntit += 1
                        ElseIf probable(site).Players(keyp).State = "Panchina" Then
                            If site = "Fantacalcio" OrElse site = "PianetaFantacalcio" Then
                                If probable(site).Players(keyp).Percentage > -1 Then
                                    npanc += probable(site).Players(keyp).Percentage / 100
                                ElseIf probable(site).Players(keyp).Info = "" OrElse probable(site).Players(keyp).Info.Contains("allottag") Then
                                    npanc += 0.8
                                Else
                                    npanc += 0.4
                                End If
                            Else
                                If p.Ruolo = "P" Then
                                    npanc += 0.2
                                ElseIf probable(site).Players(keyp).Info = "" OrElse probable(site).Players(keyp).Info.Contains("allottag") Then
                                    npanc += 0.8
                                Else
                                    npanc += 0.4
                                End If
                            End If
                        ElseIf probable(site).Players(keyp).State = "Infortunato" Then
                            ninf += 1
                        ElseIf probable(site).Players(keyp).State = "Squalificato" Then
                            nsq += 1
                        End If
                    End If
                Next

                If val > -10 Then
                    If ntit > 1 Then
                        If ntit > 2 Then
                            val = 1
                        Else
                            val = 0.9
                        End If
                    Else
                        If npanc > 0 Then
                            If npanc > 0 Then
                                val = npanc / nsitefound + 0.6
                            Else
                                val = 0.2
                            End If
                            If val > 0.9 Then
                                If ntit > 0 AndAlso (p.Minuti > 200) Then
                                    val = 1
                                Else
                                    If p.Rating.Rating3 > 1 Then '60
                                        val = 0.9
                                    Else
                                        val = 0.85
                                    End If

                                End If
                            End If
                        End If
                    End If

                    If ninf > 2 OrElse (ninf >= nsitefound / 2 AndAlso Giornata > 25) Then
                        val = 0.05
                    ElseIf nsq > 2 OrElse (nsq >= nsitefound / 2 AndAlso Giornata > 25) Then
                        val = 0.05
                    End If

                    Return val
                Else
                    Return -10
                End If
            Else
                Return -1
            End If

        End Function

        Private Function GetRoleRating(p As AutoFormazioniData.PlayerAutoFormazione, ruoloWidth As Integer, minRuoloFact As Double, factRuolo As Double, dicRuoloRank As Dictionary(Of String, Double)) As Integer

            Dim rat As Integer = 0
            Dim listModule As New List(Of Torneo.ProbablePlayers.Probable.ProbableModule)
            Dim rint As String = p.Ruolo

            If p.Ruolo = "D" OrElse p.Ruolo = "C" OrElse p.Ruolo = "A" Then
                If Me.EnableProbable AndAlso probable.Values.ToList().Where(Function(x) x.Day <> -1).Count > 0 Then
                    listModule = probable.Values.ToList().Where(Function(x) x.Day <> -1 And x.ModuleTeam.ContainsKey(p.Squadra)).Select(Function(x) x.ModuleTeam(p.Squadra)).ToList()
                    If listModule.Count > 0 Then
                        For Each l As String In listModule(0).Lines.Keys
                            If listModule(0).Lines(l).Contains(p.Nome) Then
                                If p.Ruolo = "C" AndAlso (l = "1" OrElse (listModule(0).Lines.Count = 5 AndAlso l = "2")) Then
                                    rint = "CA"
                                ElseIf p.Ruolo = "D" AndAlso l = "2" OrElse (listModule(0).Lines.Count = 5 AndAlso l = "3") Then
                                    rint = "DC"
                                ElseIf p.Ruolo = "A" AndAlso l = "2" AndAlso listModule(0).Lines.Count < 5 Then
                                    rint = "AC"
                                End If
                            End If
                        Next
                    End If
                End If
            End If

            If Me.EnableRoleRanking AndAlso p.Minuti > 5 Then

                Dim perc As Double = If(p.Minuti > 90 OrElse p.Ruolo = "A", 1, p.Minuti / 90)
                Dim key As String = p.Ruolo & "-" & p.RuoloMantra
                'perc = p.Rating.Rating6

                If rint = "CA" Then key = "A-A"

                If dicRuoloRank.ContainsKey(key) Then
                    rat = CInt(Math.Floor(If(factRuolo = 0, ruoloWidth, factRuolo * (dicRuoloRank(key) - minRuoloFact))))
                End If
                If rint = "C" Then
                    rat += 7
                ElseIf rint = "DC" Then
                    rat += 7
                ElseIf rint = "CA" Then
                    rat += 15
                ElseIf rint = "A" Then
                    rat += 15
                ElseIf rint = "AC" Then
                    rat += 10
                End If
                rat = CInt(rat * perc)

            End If

            If p.Nome = "DIAO" Then
                p.Nome = p.Nome
            End If

            Return rat

        End Function

        Private Function DeclassPlayer(giornata As Integer, ruolo As String, squadra As String) As Integer
            If giornata = 24 Then
                If squadra = "MILAN" OrElse squadra = "COMO" Then
                    Return DeclassValue(ruolo)
                End If
            ElseIf giornata = 16 Then
                If squadra = "NAPOLI" OrElse squadra = "PARMA" OrElse squadra = "INTER" OrElse squadra = "LECCE" OrElse squadra = "VERONA" OrElse squadra = "BOLOGNA" OrElse squadra = "MILAN" OrElse squadra = "COMO" Then
                    Return DeclassValue(ruolo)
                End If
            End If
            Return 0
        End Function

        Private Function DeclassValue(ruolo As String) As Integer
            If ruolo = "P" Then
                Return 120
            ElseIf ruolo = "D" Then
                Return 70
            ElseIf ruolo = "C" Then
                Return -60
            Else
                Return -120
            End If
        End Function

        Private Function GetFormazioneFinale(plist As List(Of PlayerAutoFormazione)) As List(Of FormazioniData.PlayerFormazione)

            Dim pforma As List(Of FormazioniData.PlayerFormazione) = GetFormazioneFinalePreanalisi(plist)
            Dim dicp As Dictionary(Of Integer, PlayerAutoFormazione) = plist.ToDictionary(Function(x) x.RosaId, Function(x) x)
            Dim dicPlayersForTeam As New Dictionary(Of String, Integer)
            Dim recalculate As Boolean = False

            For Each p In pforma
                If p.Type = 1 AndAlso p.Ruolo = "A" AndAlso p.Rating.Rating6 < 1 Then
                    If dicPlayersForTeam.ContainsKey(p.Ruolo) AndAlso dicPlayersForTeam(p.Ruolo) > 0 Then
                        dicp(p.RosaId).Rating.Total1 = CInt(dicp(p.RosaId).Rating.Total1 * 0.9)
                        recalculate = True
                    End If
                End If
                If dicPlayersForTeam.ContainsKey(p.Ruolo) = False Then dicPlayersForTeam.Add(p.Ruolo, 0)
                If p.Type = 1 AndAlso p.Rating.Rating6 < 1 Then dicPlayersForTeam(p.Ruolo) += 1
            Next

            If recalculate Then
                plist = plist.OrderByDescending(Function(x) x.Rating.Total1).ThenBy(Function(x) x.RuoloValue).ThenByDescending(Function(x) x.Rating.Rating2).ThenByDescending(Function(x) x.Rating.Rating8).ToList()
                pforma = GetFormazioneFinalePreanalisi(plist)
            End If

            Return pforma

        End Function

        Private Function GetFormazioneFinalePreanalisi(plist As List(Of PlayerAutoFormazione)) As List(Of FormazioniData.PlayerFormazione)

            Dim pforma As New List(Of FormazioniData.PlayerFormazione)

            If plist.Count = 0 Then Return pforma

            For Each p As PlayerAutoFormazione In plist
                Dim pf As New FormazioniData.PlayerFormazione
                pf.RosaId = p.RosaId
                pf.Ruolo = p.Ruolo
                pf.RuoloValue = p.RuoloValue
                pf.Nome = p.Nome
                pf.Squadra = p.Squadra
                pf.InCampo = 0
                pf.Type = 0
                pf.Rating = Functions.Clone(p.Rating)
                pf.Punti = CInt(p.goodGrade + p.Rating.Rating3)
                If p.Rating.Total1 < 20 AndAlso p.Ruolo <> "P" Then
                    pf.Type = -1
                End If
                pforma.Add(pf)
            Next

            Dim np As Integer = 0
            Dim nd As Integer = 0
            Dim nc As Integer = 0
            Dim na As Integer = 0
            Dim ntit As Integer = 0
            Dim npanc As Integer = 0
            Dim ruoliPanc As New List(Of String) From {"A", "C", "D"}
            Dim ruoliTitMax As New Dictionary(Of String, Integer) From {{"P", 1}, {"A", -1}, {"C", -1}, {"D", -1}}
            Dim ruoliPancNbr As New Dictionary(Of String, Integer) From {{"P", 0}, {"A", 0}, {"C", 0}, {"D", 0}}
            Dim ruoliPancMax As New Dictionary(Of String, Integer) From {{"P", 1}, {"A", 0}, {"C", 0}, {"D", 0}}
            Dim ruoliPancAva As New Dictionary(Of String, Integer) From {{"P", 0}, {"A", 0}, {"C", 0}, {"D", 0}}
            Dim indrp As Integer = 0

            Dim ncicle As Integer = 0
            ruoliTitMax("D") = pforma.Where(Function(x) x.Ruolo = "D" AndAlso x.Type <> -1).Count() - 2
            ruoliTitMax("C") = pforma.Where(Function(x) x.Ruolo = "C" AndAlso x.Type <> -1).Count() - 2
            ruoliTitMax("A") = pforma.Where(Function(x) x.Ruolo = "A" AndAlso x.Type <> -1).Count() - 1

            Do Until ntit > 10
                For Each p As FormazioniData.PlayerFormazione In pforma
                    If p.Type = 0 Then

                        If p.Ruolo = "D" AndAlso nd >= ruoliTitMax(p.Ruolo) Then Continue For
                        If p.Ruolo = "C" AndAlso nc >= ruoliTitMax(p.Ruolo) Then Continue For
                        If p.Ruolo = "A" AndAlso na >= ruoliTitMax(p.Ruolo) Then Continue For

                        If (p.Ruolo = "P" AndAlso np = 1) Then
                            p.Type = 2
                            p.FormaId = 12
                            np += 1
                        ElseIf CheckMudule(p.Ruolo, nd, nc, na, ruoliTitMax("A")) Then
                            If (p.Ruolo <> "P" OrElse np = 0) AndAlso ntit < 11 Then
                                p.Type = 1
                                Select Case p.Ruolo
                                    Case "P" : np += 1
                                    Case "D" : nd += 1
                                    Case "C" : nc += 1
                                    Case "A" : na += 1
                                End Select
                                ntit += 1
                            End If
                        End If
                    End If
                Next
                ncicle += 1
                If ncicle > 3 Then Exit Do
            Loop

            If FidexRoleSubstitutes Then

                pforma = pforma.OrderByDescending(Function(x) x.Rating.Total2).ThenBy(Function(x) x.RuoloValue).ThenByDescending(Function(x) x.Rating.Rating2).ThenByDescending(Function(x) x.Rating.Rating8).ToList()

                Dim npava As Integer = 0

                For Each p As FormazioniData.PlayerFormazione In pforma
                    If p.Type = 0 Then
                        If p.Ruolo <> "P" Then
                            ruoliPancAva(p.Ruolo) += 1
                            npava += 1
                        End If
                    End If
                Next

                npanc = 1

                Dim ncicli As Integer = 0

                If preanalisiphase = False Then
                    preanalisiphase = False
                End If

                Do Until npanc > 9 OrElse ncicli > 9
                    For Each r As String In ruoliPanc
                        If r <> "P" Then
                            If ruoliPancAva(r) > 0 Then
                                ruoliPancMax(r) += 1
                                npanc += 1
                                ruoliPancAva(r) -= 1
                            End If
                        End If
                    Next
                    ncicli += 1
                Loop

                npanc = 1

                Do Until indrp > 2
                    For Each p As FormazioniData.PlayerFormazione In pforma
                        If p.Type = 0 AndAlso npanc < 10 AndAlso ruoliPanc(indrp) = p.Ruolo AndAlso ruoliPancNbr(ruoliPanc(indrp)) < ruoliPancMax(ruoliPanc(indrp)) Then
                            p.Type = 2
                            p.FormaId = npanc + 12
                            npanc += 1
                            ruoliPancNbr(ruoliPanc(indrp)) += 1
                            If ruoliPancNbr(ruoliPanc(indrp)) = ruoliPancMax(ruoliPanc(indrp)) Then Exit For
                        End If
                    Next
                    indrp += 1
                Loop

            Else

                npanc = 1

                pforma = pforma.OrderByDescending(Function(x) x.Rating.Total1).ThenBy(Function(x) x.RuoloValue).ThenByDescending(Function(x) x.Rating.Rating2).ThenByDescending(Function(x) x.Rating.Rating8).ToList()

                For Each p As FormazioniData.PlayerFormazione In pforma
                    If p.Type = 0 Then
                        If (p.Ruolo <> "D") Then
                            p.Type = 2
                            p.FormaId = npanc + 12
                            npanc += 1
                        End If
                        If npanc > 3 Then Exit For
                    End If
                Next

                For Each p As FormazioniData.PlayerFormazione In pforma
                    If p.Type = 0 AndAlso npanc < 10 Then
                        p.Type = 2
                        p.FormaId = npanc + 12
                        npanc += 1
                    End If
                Next
            End If

            For Each p In pforma.Where(Function(x) x.Type = 1)
                p.FormaId = p.RosaId
            Next

            For Each p In pforma.Where(Function(x) x.Type = -1)
                p.Type = 0
            Next

            pforma = pforma.OrderBy(Function(x) x.RosaId).ToList()

            Dim fid As Integer = 1

            For i As Integer = 0 To pforma.Count - 1
                If pforma(i).Type = 1 Then
                    pforma(i).FormaId = fid
                    fid += 1
                End If
            Next

            pforma = pforma.OrderBy(Function(x) x.Type).ThenBy(Function(x) x.FormaId).ToList()

            Return pforma

        End Function

        Public Function GetMatchData(Giornata As Integer) As Dictionary(Of String, List(Of String))

            Dim dicmatch As New Dictionary(Of String, List(Of String))

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT tb.teama,tb.teamb")
            sqlstr.AppendLine("FROM  tbmatch as tb")
            sqlstr.AppendLine("WHERE tb.gio=" & Giornata & ";")

            Dim a As String = sqlstr.ToString()
            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            If ds.Tables.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim ta As String = ds.Tables(0).Rows(i).Item("teama").ToString()
                    Dim tb As String = ds.Tables(0).Rows(i).Item("teamb").ToString()
                    Dim m As String = ta & "-" & tb
                    dicmatch.Add(m, New List(Of String))
                    dicmatch(m).Add(ta)
                    dicmatch(m).Add(tb)
                Next
            End If

            Return dicmatch

        End Function

        Public Function GetPlayerPuntiData(Giornata As Integer, IdTeam As Integer) As Dictionary(Of String, Integer)

            Dim dicpt As New Dictionary(Of String, Integer)

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT tbf.nome,tbf.squadra,tbd.pt")
            sqlstr.AppendLine("FROM  tbformazioni as tbf")
            sqlstr.AppendLine("LEFT join tbdati as tbd on tbd.gio=tbf.gio and tbf.nome=tbd.nome")
            sqlstr.AppendLine("WHERE tbf.gio=" & Giornata & " and tbf.idteam=" & IdTeam & " and type<3;")

            Dim a As String = sqlstr.ToString()
            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            If ds.Tables.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim row As DataRow = ds.Tables(0).Rows(i)
                    Dim key As String = Functions.ReadFieldStringData("nome", row, "")
                    Dim pt As Integer = Functions.ReadFieldIntegerData("pt", row, -200)
                    If dicpt.ContainsKey(key) = False Then dicpt.Add(key, pt)
                Next
            End If

            Return dicpt

        End Function

        Private Shared Function CheckMudule(ByVal Ruolo As String, ByVal CurrD As Integer, ByVal CurrC As Integer, ByVal CurrA As Integer, numA As Integer) As Boolean

            Dim ris As Boolean = False

            Dim tot As Integer = CurrD + CurrC + CurrA + 1

            Select Case Ruolo
                Case "D" : CurrD += 1
                Case "C" : CurrC += 1
                Case "A" : CurrA += 1
            End Select

            If CurrD < 4 AndAlso CurrC < 5 AndAlso CurrA < 4 Then '343'
                ris = True
                'ris = Not (CurrD = 3 AndAlso CurrC = 4 AndAlso CurrA = 5)
            ElseIf CurrD < 4 AndAlso CurrC < 6 AndAlso CurrA < 3 Then '352'
                ris = True
            ElseIf CurrD < 5 AndAlso CurrC < 4 AndAlso CurrA < 4 Then '433'
                ris = True
                ris = Not (CurrD = 4 AndAlso CurrC = 4 AndAlso CurrA = 3)
            ElseIf CurrD < 5 AndAlso CurrC < 5 AndAlso CurrA < 3 Then '442'
                ris = True
                'ris = Not (CurrD = 4 AndAlso CurrC = 4 AndAlso CurrA = 2)
            ElseIf CurrD < 5 AndAlso CurrC < 6 AndAlso CurrA < 2 AndAlso numA < 3 Then '451'
                ris = True
                'ris = Not (CurrD = 4 AndAlso CurrC = 5 AndAlso CurrA = 1)
                'ElseIf CurrP < 2 AndAlso CurrD < 6 AndAlso CurrC < 4 AndAlso CurrA < 3 Then '532'
                '    ris = True
                '    ris = Not (CurrD = 5 AndAlso CurrC = 3 AndAlso CurrA = 2)
                'ElseIf CurrC < 5 AndAlso CurrA < 2 Then '541'
                '    ris = True
                '    ris = Not (CurrD = 5 AndAlso CurrC = 4 AndAlso CurrA = 1)
            End If

            Return ris

        End Function

        Public Sub WriteLog(Giornata As Integer, idTeam As Integer, list As List(Of AutoFormazione))
            Dim fileLog1 As String = appSett.WebDataPath & "\temp\autoformaresult" & Giornata & "-" & idTeam & ".log"
            Dim sw As New IO.StreamWriter(fileLog1, True)
            For Each item As AutoFormazione In list
                WriteLog(item, sw)
            Next
            sw.Close()
            sw.Dispose()
        End Sub

        Public Sub WriteLog(item As AutoFormazione, sw As IO.StreamWriter)
            sw.WriteLine("**** Paramentri rating giornata : " & item.Formazione.Giornata & " team:" & item.Formazione.TeamId)
            sw.WriteLine(item.Parameters.Points / 10 & vbTab & item.Parameters.GetKey().Replace("|", vbTab))
            sw.WriteLine("**** Rating giornata: " & item.Formazione.Giornata & " team:" & item.Formazione.TeamId)
            For k As Integer = 0 To item.PlayerRating.Count - 1
                sw.WriteLine(item.PlayerRating(k).Ruolo & vbTab & item.PlayerRating(k).Nome & vbTab & item.PlayerRating(k).Squadra & vbTab & item.PlayerRating(k).Rating.Total1 & vbTab & item.PlayerRating(k).Rating.Total2 & vbTab & item.PlayerRating(k).Rating.Rating1 & vbTab & item.PlayerRating(k).Rating.Rating2 & vbTab & item.PlayerRating(k).Rating.Rating3 & vbTab & item.PlayerRating(k).Rating.Rating4 & vbTab & item.PlayerRating(k).Rating.Rating5 & vbTab & item.PlayerRating(k).Rating.Rating6 & vbTab & item.PlayerRating(k).Rating.Rating7)
            Next
            sw.WriteLine("**** Formazione giornata: " & item.Formazione.Giornata & " team:" & item.Formazione.TeamId)
            For k As Integer = 0 To item.Formazione.Players.Count - 1
                sw.WriteLine(item.Formazione.Players(k).Ruolo & vbTab & item.Formazione.Players(k).Nome & vbTab & item.Formazione.Players(k).Squadra & vbTab & item.Formazione.Players(k).Type & vbTab & item.Formazione.Players(k).InCampo & vbTab & If(item.Formazione.Players(k).Punti > -100, item.Formazione.Players(k).Punti.ToString(), ""))
            Next
        End Sub

        Public Class DecisionTree
            Public Function FrequenzeGerarchichePerPrefisso(righe As List(Of String)) As Dictionary(Of String, Integer)

                Dim diz As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

                For Each riga In righe
                    Dim parts = riga.Split("|"c) _
                                    .Select(Function(x) x.Trim()) _
                                    .Where(Function(x) x <> "") _
                                    .ToList()

                    If parts.Count = 0 Then Continue For

                    ' Ordino per avere una gerarchia consistente
                    parts.Sort()

                    ' Costruisco tutte le combinazioni "prefisso"
                    Dim corrente As New List(Of String)()

                    For i As Integer = 0 To parts.Count - 1
                        corrente.Add(parts(i))
                        Dim chiave = String.Join("|", corrente)

                        If Not diz.ContainsKey(chiave) Then
                            diz(chiave) = 0
                        End If
                        diz(chiave) += 1
                    Next
                Next

                Return diz
            End Function

        End Class

        <Serializable()>
        Public Class FactoryValues
            Public Property Min() As Double = 0
            Public Property Max As Double = 0
            Public Property Fact As Double = 0

            Sub CalcolaFactory(width As Integer)
                Fact = If(Min <> Max, width / (Max - Min), 0)
            End Sub
        End Class

        <Serializable()>
        Public Class AutoFormazione
            Public Property Formazione() As FormazioniData.Formazione = New FormazioniData.Formazione()
            Public Property PlayerRating() As List(Of PlayerAutoFormazione) = New List(Of PlayerAutoFormazione)
            Public Property Parameters() As ParamenterValues = New ParamenterValues()

            <Serializable()>
            Public Class ParamenterValues
                Public Property DayRef As Integer = 1
                Public Property Preanalisi As Boolean = False
                Public Property HistoricalPlayerData As Integer = 10
                Public Property HistoricalPlayerWeight As Integer = 1
                Public Property HistoricalMatchData As Integer = 20
                Public Property MatchWidth As Integer = 120
                Public Property TeamWidth As Integer = 32
                Public Property RuoloWidth As Integer = 15
                Public Property AvarangePointsWitdh As Integer = 105
                Public Property LastPresenceWitdh As Integer = 25
                Public Property Points As Integer = 0

                Public Sub New()
                    SetDefault()
                End Sub

                Public Sub New(HistoricalPlayerData As Integer)
                    SetDefault()
                    Me.HistoricalPlayerData = HistoricalPlayerData
                End Sub

                Public Sub New(HistoricalPlayerData As Integer, HistoricalPlayerWeight As Integer, HistoricalMatchData As Integer)
                    SetDefault()
                    Me.HistoricalPlayerData = HistoricalPlayerData
                    Me.HistoricalPlayerWeight = HistoricalPlayerWeight
                    Me.HistoricalMatchData = HistoricalMatchData
                End Sub

                Public Function GetKey() As String
                    Dim strb As New System.Text.StringBuilder
                    strb.Append("TW:" & TeamWidth & "|ROLEW:" & RuoloWidth & "|HPD:" & HistoricalPlayerData & "|HPW:" & HistoricalPlayerWeight & "|HMD:" & HistoricalMatchData & "|AVGW:" & AvarangePointsWitdh & "|MATCHW:" & MatchWidth & "|LASTW:" & LastPresenceWitdh)
                    Return strb.ToString()
                End Function

                Public Sub SetFromKey(key As String)
                    Dim subvalues As String() = key.Split(CChar("|"))
                    For Each subvalue As String In subvalues
                        If subvalue.StartsWith("TW") Then
                            TeamWidth = CInt(subvalue.Replace("TW:", ""))
                        ElseIf subvalue.StartsWith("ROLEW") Then
                            RuoloWidth = CInt(subvalue.Replace("ROLEW:", ""))
                        ElseIf subvalue.StartsWith("HPD") Then
                            HistoricalPlayerData = CInt(subvalue.Replace("HPD:", ""))
                        ElseIf subvalue.StartsWith("HPW") Then
                            HistoricalPlayerWeight = CInt(subvalue.Replace("HPW:", ""))
                        ElseIf subvalue.StartsWith("HMD:15") Then
                            HistoricalMatchData = CInt(subvalue.Replace("HMD:", ""))
                        ElseIf subvalue.StartsWith("AVGW") Then
                            AvarangePointsWitdh = CInt(subvalue.Replace("AVGW:", ""))
                        ElseIf subvalue.StartsWith("MATCHW") Then
                            MatchWidth = CInt(subvalue.Replace("MATCHW:", ""))
                        ElseIf subvalue.StartsWith("LASTW") Then
                            LastPresenceWitdh = CInt(subvalue.Replace("LASTW:", ""))
                        End If
                    Next
                End Sub

                Sub SetDefault()
                    TeamWidth = 32
                    RuoloWidth = 10
                    HistoricalPlayerData = 10
                    HistoricalPlayerWeight = 2
                    HistoricalMatchData = 20
                End Sub

            End Class
        End Class

        <Serializable()>
        Public Class BestParameterAutoFormazione

        End Class

        <Serializable()>
        Public Class PlayerAutoFormazione
            Public Property RosaId() As Integer = 0
            Public Property RuoloValue As Integer = 0
            Public Property Ruolo As String = ""
            Public Property RuoloMantra As String = ""
            Public Property qCur() As Integer = 0
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Amm() As Double = 0
            Public Property Gs() As Double = 0
            Public Property Gf() As Double = 0
            Public Property Ass() As Double = 0
            Public Property AvgPt As Double = 0
            Public Property AvgVt As Double = 0
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
            Public Property TeamA As String = ""
            Public Property TeamB As String = ""
            Public Property goodGrade As Double = 0
            Public Property FattoreSquadra As Double = 0
            Public Property FattoreAvversaria As Double = 0
            Public Property PropGolFattiMatch As Double = 0
            Public Property PropGolSubitiMatch As Double = 0
            Public Property Rating() As Ratings = New Ratings

            <Serializable()>
            Public Class Ratings
                Public Property Total1() As Integer = 0
                Public Property Total2() As Integer = 0
                Public Property Rating1() As Integer = 0
                Public Property Rating2() As Integer = 0
                Public Property Rating3() As Integer = 0
                Public Property Rating4() As Integer = 0
                Public Property Rating5() As Integer = 0
                Public Property Rating6() As Double = 0
                Public Property Rating7() As Integer = 0
                Public Property Rating8() As Integer = 0
            End Class
        End Class

    End Class
End Namespace