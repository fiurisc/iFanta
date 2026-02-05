Imports System.Collections.Concurrent
Imports System.Data
Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports webfuction.Torneo.ProbablePlayers

Namespace Torneo
    Public Class AutoFormazioniData

        Dim appSett As New PublicVariables
        Dim maxday As Integer = -1
        Dim daydata As Integer = -10

        Dim probable As New Dictionary(Of String, Probable)
        Dim probableloaded As Boolean = False
        Dim preanalisiphase As Boolean = False

        Public Property EnableAvarangePointsRanking As Boolean = True 'rank punti ultime giornate'
        Public Property EnablePostionRanking As Boolean = True 'rank determinato sulle posizioni in classifica delle scquadre coivolte nel match del giocatore'
        Public Property EnableBonusRanking As Boolean = True 'rank goal e assist'
        Public Property EnableLastPresenceRanking As Boolean = True 'rank relativo ai mi nuti giocati nelle ultime giornate'
        Public Property EnableRoleRanking As Boolean = True 'rank ruolo giocatore'
        Public Property EnableRoleMantraRanking As Boolean = True 'rank ruolo giocatore'
        Public Property EnableProbable As Boolean = True
        Public Property EanblePreanalisys As Boolean = True
        Public Property EanbleHistoricalPreanalisys As Boolean = True
        Public Property BestHistoricalDataByDecisionTree As Boolean = True
        Public Property BestDataByDecisionTree As Boolean = True


        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function GetApiFormazioneAutomatica(Day As String, TeamId As String) As String

            Dim json As String = ""

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Richiesta formazioni automatiche giornata: " & Day & " per il team: " & TeamId)

            Try
                Dim list As New List(Of AutoFormazione)
                For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                    If TeamId = "-1" OrElse i.ToString() = TeamId Then
                        list.Add(GetFormazioneAutomatica(i, CInt(Day)))
                    End If
                Next
                Dim dicForma As Dictionary(Of String, FormazioniData.Formazione) = list.ToDictionary(Function(x) x.Formazione.TeamId.ToString(), Function(x) x.Formazione)
                Return WebData.Functions.SerializzaOggetto(dicForma, True)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return json

        End Function

        Public Sub LoadProbable(Giornata As Integer)
            Dim probdata As New Torneo.ProbablePlayers(appSett)
            Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", Giornata)
            probableloaded = True
        End Sub

        Sub SetProbable(ProbableFormation As Dictionary(Of String, Probable))
            probable = ProbableFormation
            probableloaded = True
        End Sub

        Sub GetFormazioniAutomatiche(ByVal Giornata As Integer)
            Dim formaList As New Dictionary(Of Integer, AutoFormazione)
            For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                formaList.Add(i, GetFormazioneAutomatica(i, Giornata))
            Next
        End Sub

        Public Function GetFormazioneAutomatica(ByVal Giornata As Integer, ByVal IdTeam As Integer) As AutoFormazione

            Try

                If Me.EnableProbable AndAlso probableloaded = False Then
                    LoadProbable(Giornata)
                End If

                CheckMaxDayData()

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

            Dim Parameters As List(Of AutoFormazione.ParamenterValues) = GetAnalysisParametersList(True)
            'Dim Parameters As List(Of AutoFormazione.ParamenterValues) = GetDefaultParametersList(True)

            preanalisiphase = True

            If EanbleHistoricalPreanalisys Then

                Dim oldp As New List(Of String) From {"POSS"}
                Parameters = GetHistoricalParamenters(Giornata, IdTeam, 15, oldp)

                If Parameters.Count > 0 Then
                    If BestHistoricalDataByDecisionTree Then
                        Parameters = GetAnalysisParametersList(Parameters)
                        Parameters = GetBestParamentersByDecisionTree(Parameters, 100)

                    Else
                        'Dim dicHistParames As Dictionary(Of String, Integer) = GetOccurenceDictionary(itemsHist)
                        'Parameters = GetBestParamentersByOccurence(dicHistParames, 2, 40)
                    End If
                End If

            End If

            Dim results As List(Of AutoFormazione) = GetData(Giornata, IdTeam, Parameters, True)
            Parameters = results.Select(Function(x) x.Parameters).ToList()
            Dim maxValue As Integer = Parameters.Select(Function(x) x.Points).Max()
            Parameters.RemoveAll(Function(x) x.Points < maxValue)

            If BestDataByDecisionTree Then
                Parameters = GetBestParamentersByDecisionTree(Parameters, 1)
            Else
                Dim items As List(Of String) = Parameters.Select(Function(x) x.GetKey()).ToList()
                Dim dicParames As Dictionary(Of String, Integer) = GetOccurenceDictionary(items)
                Parameters = GetBestParamentersByOccurence(dicParames, 1, 1)
            End If

            If Parameters.Count > 0 Then
                Return Parameters(0)
            Else
                Return New AutoFormazione.ParamenterValues
            End If

        End Function

        Private Function GetOccurenceDictionary(items As List(Of String)) As Dictionary(Of String, Integer)

            Dim dicParames As New Dictionary(Of String, Integer)

            For Each item In items
                If Not dicParames.ContainsKey(item) Then
                    dicParames(item) = 0
                End If
                dicParames(item) += 1
            Next

            Return dicParames

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

        Private Function GetBestParamentersByDecisionTree(dicParames As Dictionary(Of String, Integer), Levels As Integer, numberOfMaxItems As Integer) As List(Of AutoFormazione.ParamenterValues)

            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)
            Dim maxOccurence As Integer = dicParames.Values.Max
            Dim items As List(Of String) = dicParames.Where(Function(x) x.Value = maxOccurence).Select(Function(x) x.Key).ToList()
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
                If Levels = -1 Then
                    para.SetFromKey(ramo)
                Else
                    Dim subparas As List(Of String) = ramo.Split(CChar("|")).ToList().GetRange(0, Levels)
                    para.SetFromKey(String.Join("|", subparas))
                End If
                Parameters.Add(para)
                npara += 1
                If numberOfMaxItems <> -1 AndAlso npara > numberOfMaxItems Then Exit For
            Next

            Return Parameters

        End Function

        Private Function GetBestParamentersByOccurence(dicParames As Dictionary(Of String, Integer), numberOfTopValue As Integer, numberOfMaxItems As Integer) As List(Of AutoFormazione.ParamenterValues)

            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)

            Dim topMaxValues = dicParames.OrderByDescending(Function(x) x.Value).ToDictionary(Function(x) x.Key, Function(x) x.Value).Select(Function(x) x.Value).Distinct().Take(numberOfTopValue).ToList()

            For Each value As Integer In topMaxValues
                Dim items As List(Of String) = dicParames.Where(Function(x) x.Value = value).Select(Function(x) x.Key).OrderBy(Function(x) x).Take(numberOfMaxItems).ToList()
                For Each item In items
                    Dim para As New AutoFormazione.ParamenterValues
                    para.SetFromKey(item)
                    Parameters.Add(para)
                Next
                If Parameters.Count >= numberOfMaxItems Then Exit For
            Next

            Return Parameters

        End Function

        Private Function GetHistoricalParamenters(Giornata As Integer, IdTeam As Integer, Historical As Integer, SubParameters As List(Of String)) As List(Of AutoFormazione.ParamenterValues)

            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)
            Dim items As New List(Of String)

            For i As Integer = Giornata - Historical To Giornata - 1
                If i > 0 Then
                    Dim fname As String = GetBestHistricalFileName(i, IdTeam)
                    If System.IO.File.Exists(fname) Then
                        For Each line As String In System.IO.File.ReadAllLines(fname)
                            Dim fields As List(Of String) = line.Substring(line.IndexOf("|") + 1).Split(CChar("|")).ToList()
                            Dim para As New AutoFormazione.ParamenterValues()
                            para.TeamWidth = -1
                            para.HistoricalPlayerData = -1
                            para.HistoricalPlayerWeight = -1
                            para.HistoricalPosistionData = -1
                            para.PostionGroupSize = -1
                            para.AvarangePointsWitdh = -1
                            para.PositionWidth = -1
                            para.LastPresenceWitdh = -1
                            para.SetFromKey(String.Join("|", fields.Where(Function(x) SubParameters.Count = 0 OrElse SubParameters.Contains(x.Substring(0, x.IndexOf(":")))).ToList()))
                            Parameters.Add(para)
                        Next
                    End If
                End If
            Next

            Return Parameters

        End Function

        Public Sub BestHitoricalAnalysis(Giornata As Integer)

            Dim teams As List(Of Integer) = Enumerable.Range(0, appSett.Settings.NumberOfTeams).ToList()

            Parallel.ForEach(teams, Sub(team)
                                        BestHitoricalAnalysis(Giornata, team)
                                    End Sub)

        End Sub

        Public Sub BestHitoricalAnalysis(Giornata As Integer, IdTeam As Integer)

            If Me.EnableProbable AndAlso probableloaded = False Then
                LoadProbable(Giornata)
            End If

            CheckMaxDayData()
            SetDayData(Giornata)

            Dim Parameters As List(Of AutoFormazione.ParamenterValues) = GetDefaultParametersList(False)
            Dim results As List(Of AutoFormazione) = GetData(Giornata, IdTeam, Parameters, False)

            Dim dicpt As Dictionary(Of String, Integer) = GetPlayerPuntiData(Giornata, IdTeam)
            Dim comp As New Torneo.CompilaData(appSett)
            Dim data As New Torneo.FormazioniData(appSett)
            Dim best As New List(Of AutoFormazione.ParamenterValues)

            For Each result As AutoFormazione In results
                If result IsNot Nothing Then
                    For Each p As Torneo.FormazioniData.PlayerFormazione In result.Formazione.Players
                        If dicpt.ContainsKey(p.Nome) Then
                            p.Punti = dicpt(p.Nome)
                        Else
                            p.Punti = -200
                        End If
                        p.InCampo = 0
                    Next
                    result.Formazione = comp.CompileDataForma(result.Formazione, False)
                    data.CalculatePuntiFormazione(result.Formazione)
                    result.Formazione.Punti = result.Formazione.Punti
                End If
            Next

            Dim ptMax As List(Of Integer) = results.Select(Function(x) x.Formazione.Punti).Distinct().ToList().OrderByDescending(Function(x) x).ToList()

            For Each pt As Integer In ptMax
                For Each result As AutoFormazione In results
                    If result IsNot Nothing Then
                        If result.Formazione.Punti = pt Then
                            result.Parameters.Points = pt
                            best.Add(result.Parameters)
                        End If
                    End If
                Next
                Exit For
                If best.Count > 100 Then Exit For
            Next

            Dim dataout As New System.Text.StringBuilder

            'If best.Count < 1000 Then
            For k As Integer = 0 To best.Count - 1
                dataout.AppendLine(best(k).Points & "|" & best(k).GetKey)
            Next
            'End If

            System.IO.File.WriteAllText(GetBestHistricalFileName(Giornata, IdTeam), dataout.ToString())

        End Sub

        Private Function GetDefaultRangeParametersList(field As String, limited As Boolean) As List(Of Integer)

            Dim defPara As New AutoFormazione.ParamenterValues

            If field = "TW" Then
                If limited Then
                    Return New List(Of Integer) From {defPara.TeamWidth}
                Else
                    Return New List(Of Integer) From {5, 10, 20}
                End If
            ElseIf field = "HPD" Then
                If limited Then
                    Return New List(Of Integer) From {defPara.HistoricalPlayerData}
                Else
                    Return New List(Of Integer) From {6, 8}
                End If
            ElseIf field = "HPW" Then
                If limited Then
                    Return New List(Of Integer) From {defPara.HistoricalPlayerWeight}
                Else
                    Return New List(Of Integer) From {4, 6}
                End If
            ElseIf field = "POSS" Then
                If limited Then
                    Return New List(Of Integer) From {defPara.PostionGroupSize}
                Else
                    Return New List(Of Integer) From {5}
                End If
            ElseIf field = "HPOSD" Then
                If limited Then
                    Return New List(Of Integer) From {defPara.HistoricalPosistionData}
                Else
                    Return New List(Of Integer) From {20, 25, 30}
                End If
            ElseIf field = "AVGW" Then
                Return New List(Of Integer) From {80, 90, 100, 110}
            ElseIf field = "POSW" Then
                Return New List(Of Integer) From {40, 55, 70, 85}
            ElseIf field = "LASTW" Then
                Return New List(Of Integer) From {15, 20}
            Else
                Return New List(Of Integer)
            End If
        End Function

        Private Function GetAnalysisParametersList(oldParameters As List(Of AutoFormazione.ParamenterValues)) As List(Of AutoFormazione.ParamenterValues)

            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)

            Dim teamrankList As List(Of Integer) = If(oldParameters.Where(Function(x) x.TeamWidth <> -1).Count = 0, GetDefaultRangeParametersList("TW", True), New List(Of Integer) From {-1})
            Dim historicalList As List(Of Integer) = If(oldParameters.Where(Function(x) x.HistoricalPlayerData <> -1).Count = 0, GetDefaultRangeParametersList("HPD", True), New List(Of Integer) From {-1})
            Dim historicalWeightList As List(Of Integer) = If(oldParameters.Where(Function(x) x.HistoricalPlayerWeight <> -1).Count = 0, GetDefaultRangeParametersList("HPW", True), New List(Of Integer) From {-1})
            Dim historicalPos As List(Of Integer) = If(oldParameters.Where(Function(x) x.HistoricalPosistionData <> -1).Count = 0, GetDefaultRangeParametersList("HPOSD", True), New List(Of Integer) From {-1})
            Dim PostionGroupSizeList As List(Of Integer) = If(oldParameters.Where(Function(x) x.PostionGroupSize <> -1).Count = 0, GetDefaultRangeParametersList("POSS", True), New List(Of Integer) From {-1})
            Dim PositionWidthList As List(Of Integer) = If(oldParameters.Where(Function(x) x.PositionWidth <> -1).Count = 0, GetDefaultRangeParametersList("POSW", True), New List(Of Integer) From {-1})
            Dim AvarangePointWidthList As List(Of Integer) = If(oldParameters.Where(Function(x) x.AvarangePointsWitdh <> -1).Count = 0, GetDefaultRangeParametersList("AVGW", True), New List(Of Integer) From {-1})
            Dim LastPresenzeWidthList As List(Of Integer) = If(oldParameters.Where(Function(x) x.LastPresenceWitdh <> -1).Count = 0, GetDefaultRangeParametersList("LASTW", True), New List(Of Integer) From {-1})

            Dim listp As New List(Of String)

            For Each oldpara In oldParameters
                For Each tr As Integer In teamrankList
                    For Each histd As Integer In historicalList
                        For Each histdw As Integer In historicalWeightList
                            For Each posgr As Integer In PostionGroupSizeList
                                For Each histp As Integer In historicalPos
                                    For Each posw As Integer In PositionWidthList
                                        For Each avgw As Integer In AvarangePointWidthList
                                            For Each lastw As Integer In LastPresenzeWidthList
                                                If tr = -1 AndAlso histd = -1 AndAlso histdw = -1 AndAlso posgr = -1 AndAlso histp = -1 AndAlso posw = -1 AndAlso avgw = -1 AndAlso lastw = -1 Then Continue For
                                                Dim keyp As String = oldpara.GetKey()
                                                If listp.Contains(keyp) Then Continue For
                                                listp.Add(keyp)
                                                Dim para As New AutoFormazione.ParamenterValues
                                                para.SetFromKey(keyp)
                                                para.Preanalisi = True
                                                If tr <> -1 Then para.TeamWidth = tr
                                                If histd <> -1 Then para.HistoricalPlayerData = histd
                                                If histdw <> -1 Then para.HistoricalPlayerWeight = histdw
                                                If posgr <> -1 Then para.PostionGroupSize = posgr
                                                If histp <> -1 Then para.HistoricalPosistionData = histp
                                                If posw <> -1 Then para.PositionWidth = posw
                                                If avgw <> -1 Then para.AvarangePointsWitdh = avgw
                                                If lastw <> -1 Then para.LastPresenceWitdh = lastw

                                                Parameters.Add(para)
                                            Next
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

        Private Function GetAnalysisParametersList(Limited As Boolean) As List(Of AutoFormazione.ParamenterValues)

            Dim teamrankList As List(Of Integer) = GetDefaultRangeParametersList("TW", Limited)
            Dim historicalList As List(Of Integer) = GetDefaultRangeParametersList("HPD", Limited)
            Dim historicalWeightList As List(Of Integer) = GetDefaultRangeParametersList("HPW", Limited)
            Dim historicalPos As List(Of Integer) = GetDefaultRangeParametersList("HPOSD", Limited)
            Dim PostionGroupSizeList As List(Of Integer) = GetDefaultRangeParametersList("POSS", Limited)
            Dim PositionWidthList As List(Of Integer) = GetDefaultRangeParametersList("POSW", Limited)
            Dim AvarangePointWidthList As List(Of Integer) = GetDefaultRangeParametersList("AVGW", Limited)
            Dim LastPresenzeWidthList As List(Of Integer) = GetDefaultRangeParametersList("LASTW", Limited)

            Return GetAnalysisParametersList(teamrankList, historicalList, historicalWeightList, PostionGroupSizeList, historicalPos, PositionWidthList, AvarangePointWidthList, LastPresenzeWidthList)

        End Function


        Private Function GetAnalysisParametersList(teamrankList As List(Of Integer), historicalList As List(Of Integer), historicalWeightList As List(Of Integer), PostionGroupSizeList As List(Of Integer), historicalPos As List(Of Integer), PositionWidthList As List(Of Integer), AvarangePointWidthList As List(Of Integer), LastPresenzeWidthList As List(Of Integer)) As List(Of AutoFormazione.ParamenterValues)

            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)

            Dim listp As New List(Of String)

            For Each tr As Integer In teamrankList
                For Each histd As Integer In historicalList
                    For Each histdw As Integer In historicalWeightList
                        For Each posgr As Integer In PostionGroupSizeList
                            For Each histp As Integer In historicalPos
                                For Each posw As Integer In PositionWidthList
                                    For Each avgw As Integer In AvarangePointWidthList
                                        For Each lastw As Integer In LastPresenzeWidthList
                                            Dim para As New AutoFormazione.ParamenterValues
                                            para.Preanalisi = True
                                            para.HistoricalPlayerData = histd
                                            para.HistoricalPosistionData = histp
                                            para.HistoricalPlayerWeight = histdw
                                            para.PostionGroupSize = posgr
                                            para.AvarangePointsWitdh = avgw
                                            para.PositionWidth = posw
                                            para.LastPresenceWitdh = lastw
                                            para.TeamWidth = tr
                                            Dim keyp As String = para.GetKey()
                                            If listp.Contains(keyp) Then Continue For
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

        Private Function GetDefaultParametersList(Limited As Boolean) As List(Of AutoFormazione.ParamenterValues)

            Dim Parameters As New List(Of AutoFormazione.ParamenterValues)

            Dim teamrankList As New List(Of Integer) From {5, 10, 20}
            Dim historicalList As New List(Of Integer) From {6, 8}
            Dim historicalWeightList As New List(Of Integer) From {4, 6}
            Dim historicalPos As New List(Of Integer) From {20, 30}
            Dim PostionGroupSizeList As New List(Of Integer) From {5}
            Dim PositionWidthList As New List(Of Integer) From {40, 55, 70, 85}
            Dim AvarangePointWidthList As New List(Of Integer) From {80, 90, 100, 110}
            Dim LastPresenzeWidthList As New List(Of Integer) From {15, 20}

            If Limited Then
                Dim defPara As New AutoFormazione.ParamenterValues
                teamrankList = New List(Of Integer) From {defPara.TeamWidth}
                historicalList = New List(Of Integer) From {defPara.HistoricalPlayerData}
                historicalWeightList = New List(Of Integer) From {defPara.HistoricalPlayerWeight}
                historicalPos = New List(Of Integer) From {defPara.HistoricalPosistionData}
            End If

            For Each tr As Integer In teamrankList
                For Each histd As Integer In historicalList
                    For Each histdw As Integer In historicalWeightList
                        For Each posgr As Integer In PostionGroupSizeList
                            For Each histp As Integer In historicalPos
                                For Each posw As Integer In PositionWidthList
                                    For Each avgw As Integer In AvarangePointWidthList
                                        For Each lastw As Integer In LastPresenzeWidthList
                                            Dim para As New AutoFormazione.ParamenterValues
                                            para.Preanalisi = True
                                            para.HistoricalPlayerData = histd
                                            para.HistoricalPosistionData = histp
                                            para.HistoricalPlayerWeight = histdw
                                            para.PostionGroupSize = posgr
                                            para.AvarangePointsWitdh = avgw
                                            para.PositionWidth = posw
                                            para.LastPresenceWitdh = lastw
                                            para.TeamWidth = tr
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

        Private Function GetData(Giornata As Integer, IdTeam As Integer, Parameters As List(Of AutoFormazione.ParamenterValues), Compile As Boolean) As List(Of AutoFormazione)

            Dim comp As New Torneo.CompilaData(appSett)
            Dim data As New Torneo.FormazioniData(appSett)

            Dim results As New ConcurrentBag(Of AutoFormazione)

            Dim teamsw As List(Of Integer) = Parameters.Select(Function(x) x.TeamWidth).Distinct().ToList()
            Dim histd As List(Of Integer) = Parameters.Select(Function(x) x.HistoricalPlayerData).Distinct().ToList()
            Dim histdw As List(Of Integer) = Parameters.Select(Function(x) x.HistoricalPlayerWeight).Distinct().ToList()
            Dim histp As List(Of Integer) = Parameters.Select(Function(x) x.HistoricalPosistionData).Distinct().ToList()
            Dim pgr As List(Of Integer) = Parameters.Select(Function(x) x.PostionGroupSize).Distinct().ToList()
            Dim gruppi = Parameters.Select(Function(x, i) New With {.Item = x, .Index = i}).GroupBy(Function(x) x.Index \ 10).Select(Function(g) g.Select(Function(x) x.Item).ToList()).ToList()

            Dim teamrank As New Dictionary(Of Integer, Dictionary(Of String, Double))
            For Each tw As Integer In teamsw
                teamrank.Add(tw, GetTeamRankData(Giornata, tw))
            Next

            Dim plist As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, List(Of PlayerAutoFormazione))))

            For Each gr As Integer In pgr
                plist.Add(gr, New Dictionary(Of Integer, Dictionary(Of Integer, List(Of PlayerAutoFormazione))))
                For Each hw As Integer In histdw
                    plist(gr).Add(hw, New Dictionary(Of Integer, List(Of PlayerAutoFormazione)))
                    For Each h As Integer In histd
                        plist(gr)(hw).Add(h, GetPlayerStatisticData(Giornata, IdTeam, New AutoFormazione.ParamenterValues(h, hw, gr)))
                    Next
                Next
            Next

            Dim dicPosGroup As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, Double)))

            For Each gr As Integer In pgr
                dicPosGroup.Add(gr, New Dictionary(Of Integer, Dictionary(Of String, Double)))
                For Each h As Integer In histp
                    dicPosGroup(gr).Add(h, GetPositionRankData(Giornata, gr, h, plist(pgr(0))(histdw(0))(histd(0)).ToList()))
                Next
            Next

            For Each gr As List(Of AutoFormazione.ParamenterValues) In gruppi

                Dim paraListIndex As New List(Of Integer)
                Dim paraList As New List(Of AutoFormazione.ParamenterValues)

                For ind As Integer = 0 To gr.Count - 1
                    paraList.Add(Torneo.Functions.Clone(gr(ind)))
                    paraListIndex.Add(ind)
                Next

                Parallel.ForEach(paraListIndex, Sub(n)

                                                    Dim para As AutoFormazione.ParamenterValues = Torneo.Functions.Clone(paraList(n))
                                                    Dim plistLoc = Torneo.Functions.Clone(plist(para.PostionGroupSize)(para.HistoricalPlayerWeight)(para.HistoricalPlayerData))
                                                    Dim dicPosGroupLoc = Torneo.Functions.Clone(dicPosGroup(para.PostionGroupSize)(para.HistoricalPosistionData))
                                                    Dim teamRankLoc = Torneo.Functions.Clone(teamrank(para.TeamWidth))
                                                    Dim autoForma As AutoFormazione = GetInternalFormazioneAutomatica(Giornata, IdTeam, para, plistLoc, dicPosGroupLoc, teamRankLoc)

                                                    autoForma.Formazione.Players.RemoveAll(Function(x) x.Type = 0)

                                                    If Compile Then
                                                        Dim ptmin As Integer = autoForma.Formazione.Players.Select(Function(x) x.Punti).ToList().Min

                                                        For Each p As Torneo.FormazioniData.PlayerFormazione In autoForma.Formazione.Players
                                                            If p.Punti = 0 Then p.Punti = ptmin
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

        Private Function GetBestHistricalFileName(Giornata As Integer, IdTeam As Integer) As String
            Dim dirf As String = appSett.WebDataPath & "data\autoforma"
            If System.IO.Directory.Exists(dirf) = False Then System.IO.Directory.CreateDirectory(dirf)
            Return dirf & "\autoformabaest_" & Giornata & "_" & IdTeam & ".txt"
        End Function

        Private Function GetInternalFormazioneAutomatica(ByVal Giornata As Integer, ByVal IdTeam As Integer, Parameters As AutoFormazione.ParamenterValues) As AutoFormazione
            Dim plist As List(Of PlayerAutoFormazione) = GetPlayerStatisticData(Giornata, IdTeam, Parameters)
            Dim dicPosGroup As Dictionary(Of String, Double) = GetPositionRankData(Giornata, Parameters.PostionGroupSize, Parameters.HistoricalPosistionData, plist)
            Dim dicTeamRank As Dictionary(Of String, Double) = GetTeamRankData(Giornata, Parameters.TeamWidth)
            Return GetInternalFormazioneAutomatica(Giornata, IdTeam, Parameters, plist, dicPosGroup, dicTeamRank)
        End Function

        Private Function GetInternalFormazioneAutomatica(ByVal Giornata As Integer, ByVal IdTeam As Integer, Parameters As AutoFormazione.ParamenterValues, plist As List(Of PlayerAutoFormazione), dicPosGroup As Dictionary(Of String, Double), dicTeamRank As Dictionary(Of String, Double)) As AutoFormazione

            Dim autoForma As New AutoFormazione
            autoForma.Formazione.Giornata = Giornata
            autoForma.Formazione.TeamId = IdTeam
            autoForma.Parameters = Parameters
            autoForma.PlayerRating = New List(Of PlayerAutoFormazione)
            Try
                plist = CalculatePlayersRating(Giornata, Parameters, plist, dicPosGroup, dicTeamRank)
                autoForma.PlayerRating = plist
                autoForma.Formazione.Players = GetFormazioneFinale(Giornata, Parameters, plist)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return autoForma

        End Function

        Public Sub SetDayData(Giornata As Integer)

            daydata = Giornata - 1

            If Giornata > maxday + 1 Then
                daydata = maxday
            End If

        End Sub

        Public Sub CheckMaxDayData()

            Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT max(gio) as gio FROM tbrank")
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                maxday = CInt(ds.Tables(0).Rows(0)("gio"))
            End If

        End Sub

        Private Function GetPlayerStatisticData(Giornata As Integer, IdTeam As Integer, Parameters As AutoFormazione.ParamenterValues) As List(Of PlayerAutoFormazione)

            Dim fc As New List(Of PlayerAutoFormazione)
            Dim nday As Integer = If(daydata > Parameters.HistoricalPlayerData, Parameters.HistoricalPlayerData, daydata)
            Dim a As String = ""
            Dim enaVariousWeight As Boolean = True
            Dim minData As Integer = daydata - Parameters.HistoricalPlayerData

            If minData < 1 Then minData = 1

            Dim var0 As Integer = If(daydata >= Parameters.HistoricalPlayerData, Parameters.HistoricalPlayerData, daydata)
            Dim var1 As Integer = Parameters.HistoricalPlayerWeight
            Dim varp As String = "(" & (1 - 1 / var1).ToString().Replace(",", ".") & "+((tbd.gio-" & minData & ")/" & var0 * var1 & "))"

            If enaVariousWeight = False Then
                varp = "1"
            End If

            Try
                '************************************************************************************************
                '** i seguenti paramentri servono solo se si desidera compilare una formazione                 **
                '** automatica delle giornate passate, perchè a sistema non è presente uno storico delle rose  **
                '************************************************************************************************
                Dim tbref As String = If(Giornata > maxday, "tbrose", "tbformazioni")
                Dim sqf As String = If(Giornata > maxday, "tb.sqp", "tb.sqf")
                Dim tbwhere As String = If(Giornata > maxday, "tb.idteam=" & IdTeam, "tb.idteam=" & IdTeam & " AND tb.gio=" & Giornata & " AND tb.type<3")

                If daydata = 0 Then

                    Dim sqlstr As New Text.StringBuilder
                    sqlstr.AppendLine("   SELECT tb.*,tbp.ruolomantra,iif(tb.sqf is null or tb.sqf='',tbp.squadra,tb.sqf) as sqp FROM (")
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
                            p.RuoloMantra = Functions.ReadFieldStringData("ruolomantra", row, "")
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
                    sqlstr.AppendLine("SELECT  tb.*,tbp.voto FROM (")
                    sqlstr.AppendLine("SELECT tb.*,tbr.pos as posb,int((tbr.pos -1) / " & Parameters.PostionGroupSize & ") AS posgrb FROM (")
                    sqlstr.AppendLine(" SELECT tb.*,tbr.pos as posa,int((tbr.pos -1) / " & Parameters.PostionGroupSize & ") AS posgra FROM (")
                    sqlstr.AppendLine("  SELECT tb.*,tbm.teama,teamb,iif(tb.sqp=teama,1,0) as home,timem,iif(CDate(timem)>Now(),1,0) as available,DateDiff('h', Now(), CDate(timem)) AS tleft FROM (")
                    sqlstr.AppendLine("   SELECT tb.*,tbp.ruolomantra,iif(tb.sqf is null or tb.sqf='',tbp.squadra,tb.sqf) as sqp FROM (")
                    sqlstr.AppendLine("    SELECT tb.idrosa,tb.ruolo, tb.nome,tb.sqf,sum(tb.gf) as gf,sum(tb.gs) as gs,sum(tb.ass) as ass,sum(tb.rigt) as rigt,sum(tb.pt) as pt, IIf(Sum(tb.pt)>0,CInt(Avg(tb.pt)),0) AS avg_pt, IIf(Sum(tb.voto)>0,CInt(Avg(tb.voto)),0) AS avg_vt, Count(*) AS pgio, Sum(tbt.tit) AS tit, Sum(tbt.sos) AS sos, Sum(tbt.sub) AS sub, Sum(tbt.mm*" & varp & ") AS mm, iif(Sum(tbt.mm) > 0,CInt (Sum(tbt.mm)) / " & var0 & ",0 ) AS avg_mm FROM (")
                    sqlstr.AppendLine("     SELECT tb.idrosa,tb.ruolo,tb.nome," & If(tbref = "tbrose", "null", "tb.squadra") & " as sqf,tbd.gio,tbd.gf*" & varp & " as gf,tbd.gs*" & varp & " as gs,tbd.ass*" & varp & " as ass,tbd.rigt*" & varp & " as rigt,tbd.pt*" & varp & " as pt,tbd.voto")
                    sqlstr.AppendLine("     FROM " & tbref & " as tb")
                    sqlstr.AppendLine("     LEFT JOIN tbdati as tbd on (tbd.nome=tb.nome AND tbd.pt > -100 AND tbd.gio >" & daydata - Parameters.HistoricalPlayerData & " and tbd.gio<=" & daydata & ")")
                    sqlstr.AppendLine("     WHERE " & tbwhere & ") as tb")
                    sqlstr.AppendLine("    LEFT JOIN tbtabellini AS tbt ON (tb.gio = tbt.gio) AND (tb.nome = tbt.nome)")
                    sqlstr.AppendLine("    GROUP BY tb.idrosa,tb.ruolo,tb.nome,tb.sqf) as tb")
                    sqlstr.AppendLine("   LEFT JOIN tbplayer as tbp on tbp.nome=tb.nome) as tb")
                    sqlstr.AppendLine("  LEFT JOIN tbmatch as tbm ON (tbm.gio = " & Giornata & " AND (tb.sqp = tbm.teama OR tb.sqp = tbm.teamb))) as tb")
                    sqlstr.AppendLine(" LEFT JOIN tbrank as tbr ON (tb.teama=tbr.squadra and tbr.gio=" & daydata & ")) as tb")
                    sqlstr.AppendLine("LEFT JOIN tbrank as tbr ON (tb.teamb=tbr.squadra and tbr.gio=" & daydata & ")")
                    sqlstr.AppendLine("ORDER BY idrosa) as tb")
                    sqlstr.AppendLine("LEFT JOIN tbdati as tbp ON (tb.nome=tbp.nome and tbp.gio=" & daydata & ")")

                    a = sqlstr.ToString()

                    If a.Contains("22") Then
                        a = a
                    End If

                    Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

                    If ds.Tables.Count > 0 Then
                        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            Dim row As DataRow = ds.Tables(0).Rows(i)
                            Dim p As New PlayerAutoFormazione
                            p.RosaId = Functions.ReadFieldIntegerData("idrosa", row, 0)
                            p.Ruolo = Functions.ReadFieldStringData("ruolo", row, "")
                            p.RuoloMantra = Functions.ReadFieldStringData("ruolomantra", row, "")
                            p.Nome = Functions.ReadFieldStringData("nome", row, "")
                            p.Squadra = Functions.ReadFieldStringData("sqp", row, "")
                            p.Gf = Functions.ReadFieldIntegerData("gf", row, 0)
                            p.Gs = Functions.ReadFieldIntegerData("gs", row, 0)
                            p.Ass = Functions.ReadFieldIntegerData("ass", row, 0)
                            p.RigT = Functions.ReadFieldIntegerData("rigt", row, 0)
                            p.Pt = Functions.ReadFieldIntegerData("pt", row, 0)
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
                            p.PosA = Functions.ReadFieldIntegerData("posa", row, 1)
                            p.PosB = Functions.ReadFieldIntegerData("posb", row, 1)
                            p.PosGroupA = Functions.ReadFieldIntegerData("posgra", row, 0)
                            p.PosGroupB = Functions.ReadFieldIntegerData("posgrb", row, 0)
                            p.LastVoto = Functions.ReadFieldIntegerData("voto", row, 0)

                            If p.Nome = "FABBIAN" Then
                                p.Nome = p.Nome
                            End If
                            fc.Add(p)
                        Next
                    End If
                End If

            Catch ex As Exception
                Debug.WriteLine(a)
            End Try

            Return fc

        End Function

        Private Function GetPositionRankData(Giornata As Integer, PostionGroupSize As Integer, HistoricalPosistionData As Integer, plist As List(Of PlayerAutoFormazione)) As Dictionary(Of String, Double)

            Dim dicPosGroup As New Dictionary(Of String, Double)

            For Each p As PlayerAutoFormazione In plist
                Dim key As String = p.Ruolo & "-" & If(p.Squadra = p.TeamA, "1", "0") & "-" & If(p.Squadra = p.TeamA, p.PosGroupA, p.PosGroupB) & "-" & If(p.Squadra = p.TeamA, p.PosGroupB, p.PosGroupA)
                If dicPosGroup.ContainsKey(key) = False Then dicPosGroup.Add(key, 60)
            Next

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT ruolo, casa, pos, avv, avg(pt) AS fact FROM (")
            sqlstr.AppendLine(" SELECT tb.*, int((tbrank.pos -1) / " & PostionGroupSize & ") AS avv FROM (")
            sqlstr.AppendLine("  SELECT tbdati.gio, tbdati.ruolo, tbdati.squadra, int((tbrank.pos -1) / " & PostionGroupSize & ") AS pos, tbdati.voto as pt,iif(tbdati.squadra = tbmatch.teama, 1, 0) AS casa, iif( tbdati.squadra = tbmatch.teama, tbmatch.teamb, tbmatch.teama) AS avversaria FROM (")
            sqlstr.AppendLine("  tbdati LEFT JOIN tbmatch ON tbdati.gio = tbmatch.gio AND ( tbmatch.teama = tbdati.squadra OR tbmatch.teamb = tbdati.squadra))")
            sqlstr.AppendLine("  LEFT JOIN tbrank ON tbdati.gio = tbrank.gio AND tbdati.squadra = tbrank.squadra WHERE tbdati.pt > -100 AND tbdati.gio>" & daydata - HistoricalPosistionData & " AND tbdati.gio<=" & daydata & ") AS tb")
            sqlstr.AppendLine(" LEFT JOIN tbrank ON tb.gio = tbrank.gio AND tb.avversaria = tbrank.squadra) AS tb")
            sqlstr.AppendLine("GROUP BY ruolo,casa,pos,avv")
            sqlstr.AppendLine("ORDER BY ruolo,casa,pos, avv;")

            Dim a As String = sqlstr.ToString()
            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            If a.Contains("22") Then
                a = a
            End If
            If ds.Tables.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim row As DataRow = ds.Tables(0).Rows(i)
                    Dim key As String = ds.Tables(0).Rows(i)("ruolo").ToString() & "-" & ds.Tables(0).Rows(i)("casa").ToString() & "-" & ds.Tables(0).Rows(i)("pos").ToString() & "-" & ds.Tables(0).Rows(i)("avv").ToString()
                    Dim factGroup As Double = CDbl(ds.Tables(0).Rows(i)("fact").ToString())
                    If dicPosGroup.ContainsKey(key) Then dicPosGroup(key) = factGroup
                Next
            End If

            Return dicPosGroup

        End Function

        Private Function GetTeamRankData(Giornata As Integer, teamWidth As Integer) As Dictionary(Of String, Double)

            Dim dicTeam As New Dictionary(Of String, Double)
            If teamWidth = 0 Then Return dicTeam

            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT squadra,avg(pt) as avgpt")
            sqlstr.AppendLine("FROM tbdati")
            sqlstr.AppendLine("WHERE tbdati.pt > -100 AND tbdati.gio>" & daydata - 10 & " AND tbdati.gio<=" & daydata & "")
            sqlstr.AppendLine("GROUP BY squadra")
            sqlstr.AppendLine("ORDER BY squadra,avg(pt);")

            Dim a As String = sqlstr.ToString()

            If a.Contains("22") Then
                a = a
            End If

            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            If ds.Tables.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim key As String = ds.Tables(0).Rows(i)("squadra").ToString()
                    Dim avgpt As Double = CDbl(ds.Tables(0).Rows(i)("avgpt").ToString())
                    If dicTeam.ContainsKey(key) = False Then dicTeam.Add(key, avgpt)
                Next
            End If

            Return dicTeam

        End Function

        Private Function CalculatePlayersRating(Giornata As Integer, Parameters As AutoFormazione.ParamenterValues, plist As List(Of PlayerAutoFormazione), dicPosGroup As Dictionary(Of String, Double), dicTeamRank As Dictionary(Of String, Double)) As List(Of PlayerAutoFormazione)

            Dim lastptWidth As Integer = 0

            Dim factpt As Double = 0
            Dim ptMax As Single = 0
            Dim ptMin As Single = 0

            Dim dayLimit As Integer = Parameters.HistoricalPlayerData

            If plist.Count > 0 Then
                ptMax = plist.Select(Function(x) If(x.pGiocate > dayLimit, x.AvgPt * dayLimit, x.AvgPt * x.pGiocate)).ToList().Max
                ptMin = plist.Select(Function(x) If(x.pGiocate > dayLimit, x.AvgPt * dayLimit, x.AvgPt * x.pGiocate)).ToList().Min
            End If

            factpt = If(ptMax <> ptMin, Parameters.AvarangePointsWitdh / (ptMax - ptMin), 0)

            Dim minPosFact As Double = 60
            Dim maxPosFact As Double = 60
            Dim factpos As Double = 0

            For Each key As String In dicPosGroup.Keys
                Dim fact As Double = dicPosGroup(key)
                If fact < minPosFact Then minPosFact = fact
                If fact > maxPosFact Then maxPosFact = fact
            Next

            factpos = If(maxPosFact <> minPosFact, Parameters.PositionWidth / (maxPosFact - minPosFact), 0)

            Dim minTeamFact As Double = 60
            Dim maxTeamFact As Double = 60
            Dim factTeam As Double = 0

            For Each key As String In dicTeamRank.Keys
                Dim fact As Double = dicTeamRank(key)
                If fact < minTeamFact Then minTeamFact = fact
                If fact > maxTeamFact Then maxTeamFact = fact
            Next

            factTeam = If(minTeamFact <> maxTeamFact, Parameters.TeamWidth / (maxTeamFact - minTeamFact), 0)

            Dim minMax As Single = 0
            Dim minMin As Single = 0
            Dim factmin As Double = 0

            If plist.Count > 0 Then
                minMax = plist.Select(Function(x) x.Minuti).ToList().Max
                minMin = plist.Select(Function(x) x.Minuti).ToList().Min
            End If

            factmin = If(minMax <> minMin, Parameters.LastPresenceWitdh / (minMax - minMin), 0)

            For Each p As PlayerAutoFormazione In plist

                p.Rating.Rating1 = 0
                p.Rating.Rating2 = 0
                p.Rating.Rating3 = 0
                p.Rating.Rating4 = 0
                p.Rating.Rating5 = 0
                p.Rating.Rating6 = 0

                If p.Nome = "LANG" Then
                    p.Nome = p.Nome
                End If

                'calcolo rating 1'
                If Me.EnableAvarangePointsRanking Then
                    If p.pGiocate > dayLimit Then
                        p.Rating.Rating1 = CInt(Math.Floor(If(factpt = 0, Parameters.AvarangePointsWitdh, factpt * (p.AvgPt * dayLimit - ptMin))))
                    Else
                        p.Rating.Rating1 = CInt(Math.Floor(If(factpt = 0, Parameters.AvarangePointsWitdh, factpt * (p.AvgPt * p.pGiocate - ptMin))))
                    End If
                End If

                'calcolo rating 2'
                If Me.EnablePostionRanking Then
                    Dim key As String = p.Ruolo & "-" & If(p.Squadra = p.TeamA, "1", "0") & "-" & If(p.Squadra = p.TeamA, p.PosGroupA, p.PosGroupB) & "-" & If(p.Squadra = p.TeamA, p.PosGroupB, p.PosGroupA)
                    If dicPosGroup.ContainsKey(key) Then
                        Dim prat As Integer = CInt(Math.Floor(If(factpos = 0, Parameters.PositionWidth, factpos * (dicPosGroup(key) - minPosFact))))
                        p.Rating.Rating2 = prat
                    End If
                End If

                ' calcolo rating 3'
                If Me.EnableBonusRanking Then p.Rating.Rating3 = CInt(p.Gf * 3 + p.Ass * 1 + p.RigT * 0)

                If (p.Nome = "MCTOMINAY") AndAlso preanalisiphase = False Then
                    p.Nome = p.Nome
                End If

                ' calcolo rating 4'
                If Me.EnableLastPresenceRanking AndAlso p.Minuti > 0 Then
                    p.Rating.Rating4 = CInt(p.Minuti / (90 * Parameters.HistoricalPlayerData) * Parameters.LastPresenceWitdh)
                End If

                ' calcolo rating 5'
                p.Rating.Rating5 = 0

                If Me.EnableRoleRanking AndAlso p.Minuti > 40 Then
                    If p.Ruolo = "C" Then
                        If Me.EnableRoleMantraRanking Then
                            If p.RuoloMantra = "W" Then
                                p.Rating.Rating5 = 20
                            ElseIf p.RuoloMantra = "W|T" Then
                                p.Rating.Rating5 = 18
                                'ElseIf p.RuoloMantra.Contains("W") Then
                                '    p.Rating.Rating5 = 12
                            ElseIf p.RuoloMantra = "T" Then
                                p.Rating.Rating5 = 15
                                'ElseIf p.RuoloMantra = "M" Then
                                '    p.Rating.Rating5 = 8
                                'ElseIf p.RuoloMantra = "E|M" Then
                                '    p.Rating.Rating5 = 9
                                'ElseIf p.RuoloMantra.Contains("M") Then
                                '    p.Rating.Rating5 = 9
                            Else
                                p.Rating.Rating5 = 10
                            End If
                        End If
                    ElseIf p.Ruolo = "A" Then
                        If (p.RuoloMantra = "PC") AndAlso Me.EnableRoleMantraRanking Then
                            p.Rating.Rating5 = 32
                        Else
                            p.Rating.Rating5 = 30
                        End If
                    ElseIf p.Ruolo = "D" Then
                        'If Me.EnableRoleMantraRanking Then
                        '    If p.RuoloMantra = "E" Then
                        '        p.Rating.Rating5 = 5
                        '    ElseIf p.RuoloMantra = "M" Then
                        '        p.Rating.Rating5 = 2
                        '    End If
                        'End If
                    End If
                End If

                p.Rating.Total = CInt(p.Rating.Rating1 + p.Rating.Rating2 + p.Rating.Rating3 + p.Rating.Rating4 + p.Rating.Rating5 + p.Rating.Rating7)

                Dim pteam As Integer = CInt(Math.Floor(If(factTeam = 0, Parameters.TeamWidth, factTeam * (dicTeamRank(p.Squadra) - minTeamFact))))
                p.Rating.Total += pteam

                If p.Ruolo = "P" Then
                    If p.Nome = "SOMMER" Then 'Inter'
                        p.Rating.Total += 100
                    ElseIf p.Nome = "MARTINEZ JO." Then
                        p.Rating.Total += 50
                    ElseIf p.Nome = "SKORUPSKI" Then 'Bologna
                        p.Rating.Total += 100
                    ElseIf p.Nome = "RAVAGLIA F." Then
                        p.Rating.Total += 50
                    ElseIf p.Nome = "DI GREGORIO" Then 'Juve'
                        p.Rating.Total += 100
                    ElseIf p.Nome = "PERIN" Then
                        p.Rating.Total += 50
                    ElseIf p.Nome = "MILINKOVIC SAVIC V." Then 'Lazio'
                        p.Rating.Total += 100
                    ElseIf p.Nome = "MERET" Then
                        p.Rating.Total += 50
                    End If
                End If

                If Me.EnableProbable AndAlso probable.Values.ToList().Where(Function(x) x.Day <> -1).Count > 0 Then

                    If (p.Nome = "MERET" OrElse p.Nome = "CONTINI") AndAlso preanalisiphase = False Then
                        p.Nome = p.Nome
                    End If

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
                                    npanc += probable(site).Players(keyp).Percentage / 100
                                Else
                                    If p.Ruolo = "P" Then
                                        npanc += 0.2
                                    Else
                                        npanc += 1
                                    End If
                                End If
                            ElseIf probable(site).Players(keyp).State = "Infortunato" Then
                                ninf += 1
                            ElseIf probable(site).Players(keyp).State = "Squalificato" Then
                                nsq += 1
                            End If
                        End If
                    Next

                    If preanalisiphase = False Then
                        'Debug.WriteLine(p.Ruolo & vbTab & p.Nome & vbTab & p.Squadra & vbTab & nsitefound & vbTab & ntit & vbTab & npanc & vbTab & ninf & vbTab & nsq)
                    End If

                    If preanalisiphase = False Then
                        p.Nome = p.Nome
                    End If

                    If val > -10 Then

                        If ntit > 0 Then
                            val = 1
                            'If npanc > 3 Then val = 0.8
                        Else
                            If ninf > 2 Then
                                val = 0.1
                            ElseIf nsq > 2 Then
                                val = 0.1
                            Else
                                If nsitefound > 0 Then
                                    If npanc > 0 Then
                                        val = npanc / nsitefound + 0.4
                                        If val > 1 Then val = 1
                                    Else
                                        val = 0.2
                                    End If
                                Else

                                End If
                            End If
                        End If
                        p.Rating.Rating6 = val
                        p.Rating.Total = CInt(p.Rating.Total * val)

                    Else
                        p.Rating.Total = -1
                    End If

                End If

            Next

            plist = plist.OrderByDescending(Function(x) x.Rating.Total).ThenByDescending(Function(x) x.Rating.Rating6).ToList()

            Return plist

        End Function

        Private Function GetFormazioneFinale(giornata As Integer, Parameters As AutoFormazione.ParamenterValues, plist As List(Of PlayerAutoFormazione)) As List(Of FormazioniData.PlayerFormazione)

            Dim pforma As New List(Of FormazioniData.PlayerFormazione)

            If plist.Count = 0 Then Return pforma

            For Each p As PlayerAutoFormazione In plist
                Dim pf As New FormazioniData.PlayerFormazione
                pf.RosaId = p.RosaId
                pf.Ruolo = p.Ruolo
                pf.Nome = p.Nome
                pf.Squadra = p.Squadra
                pf.InCampo = 0
                pf.Type = 0
                pf.Punti = CInt(p.AvgPt)
                If p.Rating.Total < 15 AndAlso p.Ruolo <> "P" Then
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
            Dim portteam As String = ""
            Dim ruoliPanc As New List(Of String) From {"P", "A", "C", "D"}
            Dim ruoliPancNbr As New Dictionary(Of String, Integer) From {{"P", 0}, {"A", 0}, {"C", 0}, {"D", 0}}
            Dim ruoliPancMax As New Dictionary(Of String, Integer) From {{"P", 1}, {"A", 0}, {"C", 0}, {"D", 0}}
            Dim ruoliPancAva As New Dictionary(Of String, Integer) From {{"P", 0}, {"A", 0}, {"C", 0}, {"D", 0}}
            Dim indrp As Integer = 0

            Dim ncicle As Integer = 0

            Do Until ntit > 10
                For Each p As FormazioniData.PlayerFormazione In pforma
                    If p.Type = 0 Then
                        If CheckMudule(p.Ruolo, np, nd, nc, na) Then
                            If (p.Ruolo = "P" AndAlso np = 0) OrElse ntit < 11 Then
                                p.Type = 1
                                Select Case p.Ruolo
                                    Case "P" : np += 1 : portteam = p.Squadra
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

            npanc = 0

            Do Until indrp > 3
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

            pforma = pforma.OrderBy(Function(x) x.FormaId).ToList()

            Return pforma

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

        Private Shared Function CheckMudule(ByVal Ruolo As String, ByVal CurrP As Integer, ByVal CurrD As Integer, ByVal CurrC As Integer, ByVal CurrA As Integer) As Boolean

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
        Public Class AutoFormazione
            Public Property Formazione() As FormazioniData.Formazione = New FormazioniData.Formazione()
            Public Property PlayerRating() As List(Of PlayerAutoFormazione) = New List(Of PlayerAutoFormazione)
            Public Property Parameters() As ParamenterValues = New ParamenterValues()

            <Serializable()>
            Public Class ParamenterValues
                Public Property DayRef As Integer = 1
                Public Property Preanalisi As Boolean = False
                Public Property HistoricalPlayerData As Integer = 6
                Public Property HistoricalPlayerWeight As Integer = 6
                Public Property HistoricalPosistionData As Integer = 20
                Public Property PostionGroupSize As Integer = 5
                Public Property PositionWidth As Integer = 70
                Public Property TeamWidth As Integer = 10
                Public Property AvarangePointsWitdh As Integer = 80
                Public Property LastPresenceWitdh As Integer = 30
                Public Property Points As Integer = 0

                Public Sub New()
                    SetDefault()
                End Sub

                Public Sub New(HistoricalPlayerData As Integer, PostionGroupSize As Integer)
                    SetDefault()
                    Me.HistoricalPlayerData = HistoricalPlayerData
                    Me.PostionGroupSize = PostionGroupSize
                End Sub

                Public Sub New(HistoricalPlayerData As Integer, HistoricalPlayerWeight As Integer, PostionGroupSize As Integer)
                    SetDefault()
                    Me.HistoricalPlayerData = HistoricalPlayerData
                    Me.HistoricalPlayerWeight = HistoricalPlayerWeight
                    Me.PostionGroupSize = PostionGroupSize
                End Sub

                Public Function GetKey() As String
                    Dim strb As New System.Text.StringBuilder
                    strb.Append("TW:" & TeamWidth & "|HPD:" & HistoricalPlayerData & "|HPW:" & HistoricalPlayerWeight & "|POSS:" & PostionGroupSize & "|HPOSD:" & HistoricalPosistionData & "|AVGW:" & AvarangePointsWitdh & "|POSW:" & PositionWidth & "|LASTW:" & LastPresenceWitdh)
                    Return strb.ToString()
                End Function

                Public Sub SetFromKey(key As String)
                    Dim subvalues As String() = key.Split(CChar("|"))
                    For Each subvalue As String In subvalues
                        If subvalue.StartsWith("TW") Then
                            TeamWidth = CInt(subvalue.Replace("TW:", ""))
                        ElseIf subvalue.StartsWith("HPD") Then
                            HistoricalPlayerData = CInt(subvalue.Replace("HPD:", ""))
                        ElseIf subvalue.StartsWith("HPW") Then
                            HistoricalPlayerWeight = CInt(subvalue.Replace("HPW:", ""))
                        ElseIf subvalue.StartsWith("POSS") Then
                            PostionGroupSize = CInt(subvalue.Replace("POSS:", ""))
                        ElseIf subvalue.StartsWith("HPOSD") Then
                            HistoricalPosistionData = CInt(subvalue.Replace("HPOSD:", ""))
                        ElseIf subvalue.StartsWith("AVGW") Then
                            AvarangePointsWitdh = CInt(subvalue.Replace("AVGW:", ""))
                        ElseIf subvalue.StartsWith("POSW") Then
                            PositionWidth = CInt(subvalue.Replace("POSW:", ""))
                        ElseIf subvalue.StartsWith("LASTW") Then
                            LastPresenceWitdh = CInt(subvalue.Replace("LASTW:", ""))
                        End If
                    Next
                End Sub

                Sub SetDefault()
                    TeamWidth = 10
                    HistoricalPlayerData = 6
                    HistoricalPlayerWeight = 4
                    HistoricalPosistionData = 20
                End Sub

            End Class
        End Class

        <Serializable()>
        Public Class PlayerAutoFormazione
            Public Property RosaId() As Integer = 0
            Public Property Ruolo As String = ""
            Public Property RuoloMantra As String = ""
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Gs() As Integer = 0
            Public Property Gf() As Integer = 0
            Public Property Ass() As Integer = 0
            Public Property RigT() As Integer = 0
            Public Property Pt As Single = 0
            Public Property AvgPt As Single = 0
            Public Property AvgVt As Single = 0
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
            Public Property LastVoto() As Integer = 0
            Public Property Rating() As Ratings = New Ratings

            <Serializable()>
            Public Class Ratings
                Public Property Total() As Integer = 0
                Public Property Rating1() As Integer = 0
                Public Property Rating2() As Integer = 0
                Public Property Rating3() As Integer = 0
                Public Property Rating4() As Integer = 0
                Public Property Rating5() As Integer = 0
                Public Property Rating6() As Double = 0
                Public Property Rating7() As Integer = 0
            End Class
        End Class

    End Class
End Namespace