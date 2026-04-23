Imports System.Collections.Concurrent
Imports System.Data
Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports System.Data.Entity.Core.Common.EntitySql
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports webfuction.Torneo.AutoFormazioniData.AutoFormazione.ParamenterValues
Imports webfuction.Torneo.ProbablePlayers

Namespace Torneo
    Public Class AutoFormazioniData

        Dim appSett As New PublicVariables
        Dim maxday As Integer = -1
        Dim daydata As Integer = -10

        Dim probable As New Dictionary(Of String, Probable)

        Dim probableloaded As Boolean = False
        Dim addprobable As Boolean = True
        Dim enapreanalisi As Boolean = True
        Dim preanalisiphase As Boolean = False

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

        Public Sub LoadProbable()
            Dim probdata As New Torneo.ProbablePlayers(appSett)
            Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("")
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

        End Function

        Public Function GetFormazioneAutomatica(ByVal Giornata As Integer, ByVal IdTeam As Integer, hist As List(Of AutoFormazione.ParamenterValues)) As AutoFormazione

            Try

                If addprobable AndAlso probableloaded = False Then
                    LoadProbable()
                End If

                CheckMaxDayData()

                Dim paraold As New AutoFormazione.ParamenterValues
                paraold.EnableProbable = addprobable

                Dim para As New AutoFormazione.ParamenterValues
                para.EnableProbable = addprobable
                SetDayData(Giornata)
                If Giornata > 0 AndAlso enapreanalisi Then
                    para = PreAnalisi(Giornata, IdTeam, hist)
                End If

                preanalisiphase = False

                Dim autoForma As AutoFormazione = GetInternalFormazioneAutomatica(Giornata, IdTeam, para)

                Return autoForma

            Catch ex As Exception
                Return New AutoFormazione
            End Try

        End Function

        Private Function PreAnalisi(ByVal Giornata As Integer, ByVal IdTeam As Integer, hist As List(Of AutoFormazione.ParamenterValues)) As AutoFormazione.ParamenterValues

            Dim avgmteam As Quartili = If(hist.Count > 0, Quartili.CalcolaValoreMedio(hist.SelectMany(Function(x) x.BestRealParams).Select(Function(y) y.HistoricalPlayerData).ToList()), New Quartili(10, 20, 15))
            Dim avghist As Quartili = If(hist.Count > 0, Quartili.CalcolaValoreMedio(hist.SelectMany(Function(x) x.BestRealParams).Select(Function(y) y.HistoricalPlayerData).ToList()), New Quartili(5, 7, 6))
            Dim avgmhistw As Quartili = If(hist.Count > 0, Quartili.CalcolaValoreMedio(hist.SelectMany(Function(x) x.BestRealParams).Select(Function(y) y.HistoricalPlayerWeight).ToList()), New Quartili(5, 7, 6))
            Dim avgmhistp As Quartili = If(hist.Count > 0, Quartili.CalcolaValoreMedio(hist.SelectMany(Function(x) x.BestRealParams).Select(Function(y) y.HistoricalPosistionData).ToList()), New Quartili(20, 25, 22))

            'If avgmteam.Q3 - avgmteam.Q1 < 10 Then
            '    avgmteam.Q1 = avgmteam.Avg - 5
            '    avgmteam.Q3 = avgmteam.Q1 + 10
            'End If
            'If avghist.Q3 - avghist.Q1 < 5 Then
            '    avghist.Q1 = avghist.Avg - 2
            '    avghist.Q3 = avghist.Q1 + 5
            'End If
            'If avgmhistw.Q3 - avgmhistw.Q1 < 3 Then
            '    avgmhistw.Q1 = avgmhistw.Avg - 1
            '    avgmhistw.Q3 = avgmhistw.Q1 + 3
            'End If
            'If avgmhistp.Q3 - avgmhistp.Q1 < 10 Then avgmhistp.Q3 = avgmhistp.Q1 + 10

            Dim nstep As Integer = 4

            Dim avgpt As New ParaValues(100, 80)
            Dim posv As New ParaValues(35, 65)
            Dim lastm As New ParaValues(20, 10)

            avgpt.Fact = (avgpt.ValueEnd - avgpt.ValueStart) / nstep
            posv.Fact = (posv.ValueEnd - posv.ValueStart) / nstep
            lastm.Fact = (lastm.ValueEnd - lastm.ValueStart) / nstep

            preanalisiphase = True

            Dim teamrankList As New List(Of Integer) From {0, 10, 20}
            Dim historicalList As New List(Of Integer) From {5, 10, 15}
            Dim historicalWeightList As New List(Of Integer) From {4, 7}
            Dim historicalPos As New List(Of Integer) From {20, 30}
            Dim PostionGroupSizeList As New List(Of Integer) From {5}
            Dim PositionWidtList As New List(Of Integer)
            Dim AvarangePointWidthList As New List(Of Integer)
            Dim LastPresenzeWidthList As New List(Of Integer)
            Dim roleRanikingList As New List(Of Integer) From {6}

            Dim results As New ConcurrentBag(Of AutoFormazione)

            Dim ptmaxprev As Integer = 0
            Dim ptmaxreal As Integer = 0

            Dim bestTeamWidth As New List(Of Integer)
            Dim bestHistorical As New List(Of Integer)
            Dim bestHistoricalWeight As New List(Of Integer)
            Dim bestHistoricalPos As New List(Of Integer)
            Dim bestPostionGroupSize As New List(Of Integer)
            Dim bestPositionWidth As New List(Of Integer)
            Dim bestAvarangePointWidth As New List(Of Integer)
            Dim bestLastPresenzeWidth As New List(Of Integer)

            Dim dicTeamRank As New Dictionary(Of Integer, Dictionary(Of String, Double))

            For Each mteam In teamrankList
                dicTeamRank.Add(mteam, GetTeamRankData(Giornata, mteam))
            Next

            Try
                For Each mhist In historicalList
                    For Each mhistwe In historicalWeightList
                        For Each mposgr In PostionGroupSizeList
                            Dim plist As List(Of PlayerAutoFormazione) = GetPlayerStatisticData(Giornata, IdTeam, New AutoFormazione.ParamenterValues(mhist, mposgr))
                            For Each mposhist In historicalPos
                                Dim dicPosGroup As Dictionary(Of String, Double) = GetPositionRankData(Giornata, mposgr, mposhist, plist.ToList())
                                For Each mteam In teamrankList
                                    PreAnalisiGetData(Giornata, IdTeam, mteam, mhist, mhistwe, mposhist, mposgr, plist, dicPosGroup, dicTeamRank(mteam), results)
                                Next
                            Next
                        Next
                    Next
                Next
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try

            Dim para As New AutoFormazione.ParamenterValues
            para.BestRealParams = AnalisiReal(Giornata, IdTeam, Torneo.Functions.Clone(results))

            Dim teamrank As Dictionary(Of String, Double) = GetTeamRankData(Giornata, avgmteam.Avg)
            Dim plistf As List(Of PlayerAutoFormazione) = GetPlayerStatisticData(Giornata, IdTeam, New AutoFormazione.ParamenterValues(avghist.Avg, 5))
            Dim dicPosGroupf As Dictionary(Of String, Double) = GetPositionRankData(Giornata, 5, avgmhistp.Avg, plistf.ToList())
            results = New ConcurrentBag(Of AutoFormazione)
            PreAnalisiGetData(Giornata, IdTeam, avgmteam.Avg, avghist.Avg, avgmhistw.Avg, avgmhistp.Avg, 5, plistf, dicPosGroupf, teamrank, results)

            'Determino il punteggio massimo'
            For Each result As AutoFormazione In results
                If result IsNot Nothing AndAlso result.Parameters.HistoricalPlayerData >= avghist.Q1 AndAlso result.Parameters.HistoricalPlayerData <= avghist.Q3 AndAlso result.Parameters.HistoricalPlayerWeight >= avgmhistw.Q1 AndAlso result.Parameters.HistoricalPlayerWeight <= avgmhistw.Q3 AndAlso result.Parameters.HistoricalPosistionData >= avgmhistp.Q1 AndAlso result.Parameters.HistoricalPosistionData <= avgmhistp.Q3 Then
                    If result.Formazione.Punti > ptmaxprev Then
                        ptmaxprev = result.Formazione.Punti
                    End If
                    result.Formazione.Tag = "ok"
                End If
            Next

            For Each result As AutoFormazione In results
                If result IsNot Nothing AndAlso result.Formazione.Tag = "ok" AndAlso result.Formazione.Punti = ptmaxprev Then
                    bestTeamWidth.Add(result.Parameters.TeamWidth)
                    bestHistorical.Add(result.Parameters.HistoricalPlayerData)
                    bestHistoricalWeight.Add(result.Parameters.HistoricalPlayerWeight)
                    bestHistoricalPos.Add(result.Parameters.HistoricalPosistionData)
                    bestPostionGroupSize.Add(result.Parameters.PostionGroupSize)
                    bestAvarangePointWidth.Add(result.Parameters.AvarangePointsWitdh)
                    bestPositionWidth.Add(result.Parameters.PositionWidth)
                    bestLastPresenzeWidth.Add(result.Parameters.LastPresenceWitdh)
                End If
            Next

            para.DayRef = Giornata
            para.TeamWidth = Quartili.CalcolaValoreMedio(bestTeamWidth).Avg
            para.HistoricalPlayerData = Quartili.CalcolaValoreMedio(bestHistorical).Avg
            para.HistoricalPlayerWeight = Quartili.CalcolaValoreMedio(bestHistoricalWeight).Avg
            para.HistoricalPosistionData = Quartili.CalcolaValoreMedio(bestHistoricalPos).Avg
            para.PostionGroupSize = Quartili.CalcolaValoreMedio(bestPostionGroupSize).Avg
            para.AvarangePointsWitdh = Quartili.CalcolaValoreMedio(bestAvarangePointWidth).Avg
            para.EnableProbable = addprobable
            para.PositionWidth = CInt(posv.ValueStart + Math.Abs((para.AvarangePointsWitdh - avgpt.ValueStart) / avgpt.Fact) * posv.Fact)
            para.LastPresenceWitdh = CInt(lastm.ValueStart + Math.Abs((para.AvarangePointsWitdh - avgpt.ValueStart) / avgpt.Fact) * lastm.Fact)
            para.BestPoints = ptmaxprev

            Return para

        End Function

        Private Sub PreAnalisiGetData(giornata As Integer, IdTeam As Integer, mteam As Integer, mhist As Integer, mhistwe As Integer, mposhist As Integer, mposgr As Integer, plist As List(Of PlayerAutoFormazione), dicPosGroup As Dictionary(Of String, Double), teamRank As Dictionary(Of String, Double), results As ConcurrentBag(Of AutoFormazione))

            Dim comp As New Torneo.CompilaData(appSett)
            Dim data As New Torneo.FormazioniData(appSett)

            Dim nstep As Integer = 4

            Dim avgpt As New ParaValues(100, 80)
            Dim posv As New ParaValues(35, 65)
            Dim lastm As New ParaValues(20, 10)
            Dim PositionWidtList As New List(Of Integer)
            Dim AvarangePointWidthList As New List(Of Integer)
            Dim LastPresenzeWidthList As New List(Of Integer)

            avgpt.Fact = (avgpt.ValueEnd - avgpt.ValueStart) / nstep
            posv.Fact = (posv.ValueEnd - posv.ValueStart) / nstep
            lastm.Fact = (lastm.ValueEnd - lastm.ValueStart) / nstep

            For i As Integer = 0 To nstep
                AvarangePointWidthList.Add(CInt(avgpt.ValueStart + i * avgpt.Fact))
                PositionWidtList.Add(CInt(posv.ValueStart + i * posv.Fact))
                LastPresenzeWidthList.Add(CInt(lastm.ValueStart + i * lastm.Fact))
            Next

            Dim paraListIndex As New List(Of Integer)
            Dim paraList As New List(Of AutoFormazione.ParamenterValues)

            Dim paraInt1 As New AutoFormazione.ParamenterValues
            paraInt1.HistoricalPlayerData = mhist
            paraInt1.PostionGroupSize = mposgr
            paraInt1.HistoricalPosistionData = mposhist

            For ind As Integer = 0 To AvarangePointWidthList.Count - 1
                Dim paraInt As New AutoFormazione.ParamenterValues
                paraInt.Preanalisi = True
                paraInt.HistoricalPlayerData = paraInt1.HistoricalPlayerData
                paraInt.HistoricalPosistionData = paraInt1.HistoricalPosistionData
                paraInt.HistoricalPlayerWeight = mhistwe
                paraInt.PostionGroupSize = paraInt1.PostionGroupSize
                paraInt.AvarangePointsWitdh = AvarangePointWidthList(ind)
                paraInt.PositionWidth = PositionWidtList(ind)
                paraInt.LastPresenceWitdh = LastPresenzeWidthList(ind)
                paraInt.TeamWidth = mteam
                paraInt.EnableProbable = addprobable
                paraList.Add(paraInt)
                paraListIndex.Add(ind)
            Next

            Parallel.ForEach(paraListIndex, Sub(n)

                                                Dim autoForma As AutoFormazione = GetInternalFormazioneAutomatica(giornata, IdTeam, Torneo.Functions.Clone(paraList(n)), Torneo.Functions.Clone(plist), Torneo.Functions.Clone(dicPosGroup), teamRank)

                                                autoForma.Formazione.Players.RemoveAll(Function(x) x.Type = 0)

                                                Dim ptmin As Integer = autoForma.Formazione.Players.Select(Function(x) x.Punti).ToList().Min

                                                For Each p As Torneo.FormazioniData.PlayerFormazione In autoForma.Formazione.Players
                                                    'If dicpt.ContainsKey(p.Nome) Then p.Punti = dicpt(p.Nome)
                                                    If p.Punti = 0 Then p.Punti = ptmin
                                                    'p.Punti = 60
                                                Next
                                                autoForma.Formazione = comp.CompileDataForma(autoForma.Formazione, False)
                                                data.CalculatePuntiFormazione(autoForma.Formazione)

                                                results.Add(autoForma)

                                            End Sub)

        End Sub

        'Private Function PreAnalisiOld(ByVal Giornata As Integer, ByVal IdTeam As Integer, hist As List(Of AutoFormazione.ParamenterValues)) As AutoFormazione.ParamenterValues

        '    Dim avgmteam As Integer = If(hist.Count > 0, CalcolaValoreMedio(hist.Select(Function(x) x.TeamWidth).ToList()), 10)
        '    Dim avghist As Integer = If(hist.Count > 0, CalcolaValoreMedio(hist.Select(Function(x) x.HistoricalPlayerData).ToList()), 7)
        '    Dim avgmhistw As Integer = If(hist.Count > 0, CalcolaValoreMedio(hist.Select(Function(x) x.HistoricalPlayerWeight).ToList()), 5)

        '    Dim nstep As Integer = 6
        '    'Dim avgpt As New ParaValues(100, 80)
        '    'Dim posv As New ParaValues(35, 65)
        '    'Dim lastm As New ParaValues(20, 10)

        '    Dim avgpt As New ParaValues(100, 80)
        '    Dim posv As New ParaValues(35, 65)
        '    Dim lastm As New ParaValues(20, 10)

        '    avgpt.Fact = (avgpt.ValueEnd - avgpt.ValueStart) / nstep
        '    posv.Fact = (posv.ValueEnd - posv.ValueStart) / nstep
        '    lastm.Fact = (lastm.ValueEnd - lastm.ValueStart) / nstep

        '    preanalisiphase = True

        '    'Dim teamrankList As New List(Of Integer) From {0, 10, 20}
        '    'Dim historicalList As New List(Of Integer) From {5, 10, 15, 20}
        '    'Dim historicalWeightList As New List(Of Integer) From {1, 3, 6}
        '    'Dim historicalPos As New List(Of Integer) From {10, 20, 30}
        '    'Dim PostionGroupSizeList As New List(Of Integer) From {5}
        '    'Dim PositionWidtList As New List(Of Integer)
        '    'Dim AvarangePointWidthList As New List(Of Integer)
        '    'Dim LastPresenzeWidthList As New List(Of Integer)
        '    'Dim roleRanikingList As New List(Of Integer) From {6}

        '    Dim teamrankList As New List(Of Integer) From {avgmteam - 5, avgmteam + 5}
        '    Dim historicalList As New List(Of Integer) From {avghist - 2, avghist + 1}
        '    Dim historicalWeightList As New List(Of Integer) From {avgmhistw - 2, avgmhistw + 2}
        '    Dim historicalPos As New List(Of Integer) From {25}
        '    Dim PostionGroupSizeList As New List(Of Integer) From {5}
        '    Dim PositionWidtList As New List(Of Integer)
        '    Dim AvarangePointWidthList As New List(Of Integer)
        '    Dim LastPresenzeWidthList As New List(Of Integer)
        '    Dim roleRanikingList As New List(Of Integer) From {6}

        '    Dim totresults As New Dictionary(Of String, List(Of AutoFormazione))

        '    Dim ptmaxprev As New Dictionary(Of String, Integer)
        '    Dim ptmaxreal As New Dictionary(Of String, Integer)

        '    Dim bestTeamWidth As New Dictionary(Of String, List(Of Integer))
        '    Dim bestHistorical As New Dictionary(Of String, List(Of Integer))
        '    Dim bestHistoricalWeight As New Dictionary(Of String, List(Of Integer))
        '    Dim bestHistoricalPos As New Dictionary(Of String, List(Of Integer))
        '    Dim bestPostionGroupSize As New Dictionary(Of String, List(Of Integer))
        '    Dim bestPositionWidth As New Dictionary(Of String, List(Of Integer))
        '    Dim bestAvarangePointWidth As New Dictionary(Of String, List(Of Integer))
        '    Dim bestLastPresenzeWidth As New Dictionary(Of String, List(Of Integer))

        '    Dim dicTeamRank As New Dictionary(Of Integer, Dictionary(Of String, Double))

        '    For Each mteam In teamrankList
        '        dicTeamRank.Add(mteam, GetTeamRankData(Giornata, mteam))
        '    Next

        '    Dim comp As New Torneo.CompilaData(appSett)
        '    Dim data As New Torneo.FormazioniData(appSett)

        '    Try
        '        For Each mhist In historicalList

        '            AvarangePointWidthList.Clear()
        '            PositionWidtList.Clear()
        '            LastPresenzeWidthList.Clear()

        '            For i As Integer = 0 To nstep
        '                AvarangePointWidthList.Add(CInt(avgpt.ValueStart + i * avgpt.Fact))
        '                PositionWidtList.Add(CInt(posv.ValueStart + i * posv.Fact))
        '                LastPresenzeWidthList.Add(CInt(lastm.ValueStart + i * lastm.Fact))
        '            Next

        '            For Each mhistwe In historicalWeightList
        '                For Each mposgr In PostionGroupSizeList

        '                    Dim plist As List(Of PlayerAutoFormazione) = GetPlayerStatisticData(Giornata, IdTeam, New AutoFormazione.ParamenterValues(mhist, mposgr))

        '                    For Each mposhist In historicalPos

        '                        Dim dicPosGroup As Dictionary(Of String, Double) = GetPositionRankData(Giornata, mposgr, mposhist, plist.ToList())

        '                        For Each mteam In teamrankList

        '                            Dim paraListIndex As New List(Of Integer)
        '                            Dim paraList As New List(Of AutoFormazione.ParamenterValues)

        '                            Dim key As String = mteam & "-" & mhist & "-" & mposhist & "-" & mposgr

        '                            Dim paraInt1 As New AutoFormazione.ParamenterValues
        '                            paraInt1.HistoricalPlayerData = mhist
        '                            paraInt1.PostionGroupSize = mposgr
        '                            paraInt1.HistoricalPosistionData = mposhist

        '                            If totresults.ContainsKey(key) = False Then totresults.Add(key, New List(Of AutoFormazione))

        '                            If ptmaxprev.ContainsKey(key) = False Then ptmaxprev.Add(key, 0)
        '                            If bestTeamWidth.ContainsKey(key) = False Then bestTeamWidth.Add(key, New List(Of Integer))
        '                            If bestHistorical.ContainsKey(key) = False Then bestHistorical.Add(key, New List(Of Integer))
        '                            If bestHistoricalWeight.ContainsKey(key) = False Then bestHistoricalWeight.Add(key, New List(Of Integer))
        '                            If bestHistoricalPos.ContainsKey(key) = False Then bestHistoricalPos.Add(key, New List(Of Integer))
        '                            If bestPostionGroupSize.ContainsKey(key) = False Then bestPostionGroupSize.Add(key, New List(Of Integer))
        '                            If bestPositionWidth.ContainsKey(key) = False Then bestPositionWidth.Add(key, New List(Of Integer))
        '                            If bestAvarangePointWidth.ContainsKey(key) = False Then bestAvarangePointWidth.Add(key, New List(Of Integer))
        '                            If bestLastPresenzeWidth.ContainsKey(key) = False Then bestLastPresenzeWidth.Add(key, New List(Of Integer))

        '                            For ind As Integer = 0 To AvarangePointWidthList.Count - 1
        '                                Dim paraInt As New AutoFormazione.ParamenterValues
        '                                paraInt.Preanalisi = True
        '                                paraInt.HistoricalPlayerData = paraInt1.HistoricalPlayerData
        '                                paraInt.HistoricalPosistionData = paraInt1.HistoricalPosistionData
        '                                paraInt.HistoricalPlayerWeight = mhistwe
        '                                paraInt.PostionGroupSize = paraInt1.PostionGroupSize
        '                                paraInt.AvarangePointsWitdh = AvarangePointWidthList(ind)
        '                                paraInt.PositionWidth = PositionWidtList(ind)
        '                                paraInt.LastPresenceWitdh = LastPresenzeWidthList(ind)
        '                                paraInt.TeamWidth = mteam
        '                                paraInt.EnableProbable = addprobable
        '                                paraList.Add(paraInt)
        '                                paraListIndex.Add(ind)
        '                            Next

        '                            Dim results As New ConcurrentBag(Of AutoFormazione)

        '                            Parallel.ForEach(paraListIndex, Sub(n)

        '                                                                Dim autoForma As AutoFormazione = GetInternalFormazioneAutomatica(Giornata, IdTeam, Torneo.Functions.Clone(paraList(n)), Torneo.Functions.Clone(plist), Torneo.Functions.Clone(dicPosGroup), dicTeamRank(mteam))

        '                                                                autoForma.Formazione.Players.RemoveAll(Function(x) x.Type = 0)

        '                                                                Dim ptmin As Integer = autoForma.Formazione.Players.Select(Function(x) x.Punti).ToList().Min

        '                                                                For Each p As Torneo.FormazioniData.PlayerFormazione In autoForma.Formazione.Players
        '                                                                    'If dicpt.ContainsKey(p.Nome) Then p.Punti = dicpt(p.Nome)
        '                                                                    If p.Punti = 0 Then p.Punti = ptmin
        '                                                                    'p.Punti = 60
        '                                                                Next
        '                                                                autoForma.Formazione = comp.CompileDataForma(autoForma.Formazione, False)
        '                                                                data.CalculatePuntiFormazione(autoForma.Formazione)

        '                                                                results.Add(autoForma)

        '                                                            End Sub)
        '                            ptmaxprev(key) = 0
        '                            totresults(key).AddRange(results)
        '                        Next
        '                    Next
        '                Next
        '            Next
        '        Next

        '    Catch ex As Exception
        '        Debug.WriteLine(ex.Message)
        '    End Try

        '    'Determino il punteggio massimo'
        '    Dim indf As Integer = 0
        '    For Each key As String In totresults.Keys
        '        For Each result As AutoFormazione In totresults(key)
        '            If result IsNot Nothing AndAlso result.Formazione.Punti > ptmaxprev(key) Then
        '                If ptmaxprev.ContainsKey(key) = False Then ptmaxprev.Add(key, 0)
        '                ptmaxprev(key) = result.Formazione.Punti
        '            End If
        '            result.Formazione.Tag = indf.ToString()
        '            indf += 1
        '        Next
        '    Next

        '    For Each key As String In totresults.Keys
        '        For Each result As AutoFormazione In totresults(key)
        '            If result IsNot Nothing AndAlso result.Formazione.Punti = ptmaxprev(key) Then
        '                bestTeamWidth(key).Add(result.Parameters.TeamWidth)
        '                bestHistorical(key).Add(result.Parameters.HistoricalPlayerData)
        '                bestHistoricalWeight(key).Add(result.Parameters.HistoricalPlayerWeight)
        '                bestHistoricalPos(key).Add(result.Parameters.HistoricalPosistionData)
        '                bestPostionGroupSize(key).Add(result.Parameters.PostionGroupSize)
        '                bestAvarangePointWidth(key).Add(result.Parameters.AvarangePointsWitdh)
        '                bestPositionWidth(key).Add(result.Parameters.PositionWidth)
        '                bestLastPresenzeWidth(key).Add(result.Parameters.LastPresenceWitdh)
        '            End If
        '        Next
        '    Next

        '    Dim ptmaxprevfinal As Integer = ptmaxprev.Values.Max()

        '    For Each key As String In ptmaxprev.Keys
        '        If ptmaxprev(key) = ptmaxprevfinal Then

        '            Dim para As New AutoFormazione.ParamenterValues
        '            para.DayRef = Giornata
        '            para.TeamWidth = CalcolaValoreMedio(bestTeamWidth(key))
        '            para.HistoricalPlayerData = CalcolaValoreMedio(bestHistorical(key))
        '            para.HistoricalPlayerWeight = CalcolaValoreMedio(bestHistoricalWeight(key))
        '            para.HistoricalPosistionData = CalcolaValoreMedio(bestHistoricalPos(key))
        '            para.PostionGroupSize = CalcolaValoreMedio(bestPostionGroupSize(key))
        '            para.AvarangePointsWitdh = CalcolaValoreMedio(bestAvarangePointWidth(key))
        '            para.EnableProbable = addprobable
        '            para.PositionWidth = CInt(posv.ValueStart + Math.Abs((para.AvarangePointsWitdh - avgpt.ValueStart) / avgpt.Fact) * posv.Fact)
        '            para.LastPresenceWitdh = CInt(lastm.ValueStart + Math.Abs((para.AvarangePointsWitdh - avgpt.ValueStart) / avgpt.Fact) * lastm.Fact)
        '            para.BestPoints = ptmaxprev(key)

        '            'para.TeamWidth = 15
        '            'para.HistoricalPlayerData = 5
        '            'para.HistoricalPlayerWeight = 4
        '            'para.HistoricalPosistionData = 25
        '            'para.PostionGroupSize = 5
        '            'para.AvarangePointsWitdh = 90
        '            'para.EnableProbable = addprobable
        '            'para.PositionWidth = 50
        '            'para.LastPresenceWitdh = 15
        '            'para.BestPoints = ptmaxprev(key)

        '            para.BestRealParams = AnalisiReal(Giornata, IdTeam, Functions.Clone(totresults))

        '            Return para
        '        End If
        '    Next


        '    Return New AutoFormazione.ParamenterValues

        'End Function

        Public Function AnalisiReal(Giornata As Integer, IdTeam As Integer, results As ConcurrentBag(Of AutoFormazione)) As List(Of AutoFormazione.ParamenterValues)

            Dim dicpt As Dictionary(Of String, Integer) = GetPlayerPuntiData(Giornata, IdTeam)
            Dim comp As New Torneo.CompilaData(appSett)
            Dim data As New Torneo.FormazioniData(appSett)
            Dim ptmax As Integer = 0
            Dim best As New List(Of AutoFormazione.ParamenterValues)

            For Each result As AutoFormazione In results
                If result IsNot Nothing Then
                    For Each p As Torneo.FormazioniData.PlayerFormazione In result.Formazione.Players
                        If dicpt.ContainsKey(p.Nome) Then
                            p.Punti = dicpt(p.Nome)
                        Else
                            p.Punti = -100
                        End If
                        p.InCampo = 0
                    Next
                    result.Formazione = comp.CompileDataForma(result.Formazione, False)
                    data.CalculatePuntiFormazione(result.Formazione)
                    If result.Formazione.Punti > ptmax Then
                        ptmax = result.Formazione.Punti
                    End If
                End If
            Next

            For Each result As AutoFormazione In results
                If result IsNot Nothing Then
                    If result.Formazione.Punti = ptmax Then
                        result.Parameters.BestPoints = ptmax
                        best.Add(result.Parameters)
                    End If
                End If
            Next

            Return best

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
                    sqlstr.AppendLine("    SELECT tb.idrosa,tb.ruolo, tb.nome,tb.sqf,sum(tb.gf) as gf,sum(tb.gs) as gs,sum(tb.ass) as ass,sum(tb.pt) as pt, IIf(Sum(tb.pt)>0,CInt(Avg(tb.pt)),0) AS avg_pt, IIf(Sum(tb.voto)>0,CInt(Avg(tb.voto)),0) AS avg_vt, Count(*) AS pgio, Sum(tbt.tit) AS tit, Sum(tbt.sos) AS sos, Sum(tbt.sub) AS sub, Sum(tbt.mm*" & varp & ") AS mm, iif(Sum(tbt.mm) > 0,CInt (Sum(tbt.mm)) / " & var0 & ",0 ) AS avg_mm FROM (")
                    sqlstr.AppendLine("     SELECT tb.idrosa,tb.ruolo,tb.nome," & If(tbref = "tbrose", "null", "tb.squadra") & " as sqf,tbd.gio,tbd.gf*" & varp & " as gf,tbd.gs*" & varp & " as gs,tbd.ass*" & varp & " as ass,tbd.pt*" & varp & " as pt,tbd.voto")
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
                If Parameters.EnableAvarangePointsRanking Then
                    If p.pGiocate > dayLimit Then
                        p.Rating.Rating1 = CInt(Math.Floor(If(factpt = 0, Parameters.AvarangePointsWitdh, factpt * (p.AvgPt * dayLimit - ptMin))))
                    Else
                        p.Rating.Rating1 = CInt(Math.Floor(If(factpt = 0, Parameters.AvarangePointsWitdh, factpt * (p.AvgPt * p.pGiocate - ptMin))))
                    End If
                End If

                'calcolo rating 2'
                If Parameters.EnablePostionRanking Then
                    Dim key As String = p.Ruolo & "-" & If(p.Squadra = p.TeamA, "1", "0") & "-" & If(p.Squadra = p.TeamA, p.PosGroupA, p.PosGroupB) & "-" & If(p.Squadra = p.TeamA, p.PosGroupB, p.PosGroupA)
                    If dicPosGroup.ContainsKey(key) Then
                        Dim prat As Integer = CInt(Math.Floor(If(factpos = 0, Parameters.PositionWidth, factpos * (dicPosGroup(key) - minPosFact))))
                        p.Rating.Rating2 = prat
                    End If
                End If

                ' calcolo rating 3'
                If Parameters.EnableBonusRanking Then p.Rating.Rating3 = p.Gf * 3 + p.Ass * 1

                If (p.Nome = "MCTOMINAY") AndAlso preanalisiphase = False Then
                    p.Nome = p.Nome
                End If

                ' calcolo rating 4'
                If Parameters.EnableLastPresenceRanking AndAlso p.Minuti > 0 Then
                    'p.Rating.Rating4 = CInt(factmin * (p.Minuti - minMin))
                    p.Rating.Rating4 = CInt(p.Minuti / (90 * Parameters.HistoricalPlayerData) * Parameters.LastPresenceWitdh)
                End If

                ' calcolo rating 5'
                If Parameters.EnableRoleRanking AndAlso p.Minuti > 40 Then

                    If p.Ruolo = "C" Then
                        If Parameters.RoleRanking = 0 AndAlso (p.RuoloMantra = "T" OrElse p.RuoloMantra = "W") Then
                            p.Rating.Rating5 = 20
                        Else
                            p.Rating.Rating5 = 10
                        End If
                    ElseIf p.Ruolo = "A" Then
                        p.Rating.Rating5 = 30
                    Else
                        p.Rating.Rating5 = 0
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

                If Parameters.EnableProbable AndAlso probable.Values.ToList().Where(Function(x) x.Day <> -1).Count > 0 Then

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

            'For Each p As PlayerAutoFormazione In plist
            '    If p.Rating.Total > 0 Then Continue For
            '    Debug.WriteLine(p.Ruolo & vbTab & p.Nome & vbTab & p.Squadra & vbTab & p.Rating.Total & vbTab & p.Rating.Rating1 & vbTab & p.Rating.Rating2 & vbTab & p.Rating.Rating3 & vbTab & p.Rating.Rating4 & vbTab & p.Rating.Rating5 & vbTab & p.Rating.Rating6)
            'Next

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

        Public Class Quartili
            Public Property Q1 As Integer = 0
            Public Property Q3 As Integer = 0
            Public Property Avg As Integer = 0

            Sub New()

            End Sub

            Sub New(Q1 As Integer, Q3 As Integer, Avg As Integer)
                Me.Q1 = Q1
                Me.Q3 = Q3
                Me.Avg = Avg
            End Sub

            Public Shared Function CalcolaValoreMedio(valori As List(Of Integer), Optional LastValue As Integer = -1) As Quartili

                Dim res As New Quartili

                If LastValue <> -1 Then valori.Add(LastValue)

                If valori Is Nothing OrElse valori.Count = 0 Then
                    Return res
                End If

                ' Ordina i valori
                Dim sorted = valori.OrderBy(Function(x) x).ToList()
                Dim n As Integer = sorted.Count

                ' Funzione locale per calcolare un percentile con interpolazione
                Dim percentile = Function(p As Double)
                                     Dim pos As Double = p * (n + 1)
                                     Dim k As Integer = CInt(Math.Floor(pos))
                                     Dim d As Double = pos - k

                                     If k <= 0 Then
                                         Return sorted(0)
                                     ElseIf k >= n Then
                                         Return sorted(n - 1)
                                     Else
                                         Return sorted(k - 1) + d * (sorted(k) - sorted(k - 1))
                                     End If
                                 End Function

                Dim Q1 As Double = percentile(0.25)
                Dim Q3 As Double = percentile(0.75)

                Dim tot As Integer = valori.Where(Function(x) x >= Q1 AndAlso x <= Q3).Sum
                Dim ntot As Integer = valori.Where(Function(x) x >= Q1 AndAlso x <= Q3).Count

                If ntot > 0 Then
                    res.Avg = CInt(tot / ntot)
                    res.Q1 = CInt(Q1)
                    res.Q3 = CInt(Q3)
                End If

                Return res

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
                Public Property TeamWidth As Integer = 15
                Public Property AvarangePointsWitdh As Integer = 80
                Public Property RoleRanking As Integer = 10
                Public Property LastPresenceWitdh As Integer = 30
                Public Property EnableAvarangePointsRanking As Boolean = True 'rank punti ultime giornate'
                Public Property EnablePostionRanking As Boolean = True 'rank determinato sulle posizioni in classifica delle scquadre coivolte nel match del giocatore'
                Public Property EnableBonusRanking As Boolean = True 'rank goal e assist'
                Public Property EnableLastPresenceRanking As Boolean = True 'rank relativo ai mi nuti giocati nelle ultime giornate'
                Public Property EnableRoleRanking As Boolean = True 'rank ruolo giocatore'
                Public Property EnableProbable As Boolean = True
                Public Property EanblePreanalisys As Boolean = True
                Public Property BestPoints As Integer = 0
                Public Property BestRealParams As New List(Of ParamenterValues)

                Public Sub New()

                End Sub

                Public Sub New(HistoricalPlayerData As Integer, PostionGroupSize As Integer)
                    Me.HistoricalPlayerData = HistoricalPlayerData
                    Me.PostionGroupSize = PostionGroupSize
                End Sub

                <Serializable()>
                Public Class ParaValues
                    Public Property ValueStart As Integer = 0
                    Public Property ValueEnd As Integer = 0
                    Public Property Fact As Double = 0

                    Public Sub New(ValueStart As Integer, ValueEnd As Integer)
                        Me.ValueStart = ValueStart
                        Me.ValueEnd = ValueEnd
                    End Sub

                End Class
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