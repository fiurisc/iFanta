
Namespace WebData
    Public Class ProbableFormations

        Public Shared dirTemp As String = Functions.DataPath & "temp\"
        Public Shared dirData As String = Functions.DataPath & "data\pforma\"
        Public Shared dicMatchDays As New Dictionary(Of Integer, Integer)

        Public Shared Function GetProbableFormation(site As String, show As Boolean) As String

            Dim str As New System.Text.StringBuilder

            Dim matchs As List(Of Torneo.MatchsData.Match) = Torneo.MatchsData.GetMatchsData("-1")

            dicMatchDays.Clear()

            For Each m As Torneo.MatchsData.Match In matchs
                Dim dt As Date = CDate(m.Time)
                Dim ndays As Integer = CInt(dt.Subtract(Date.Now).TotalDays())
                If dicMatchDays.ContainsKey(m.Giornata) = False Then dicMatchDays.Add(m.Giornata, 0)
                If dicMatchDays(m.Giornata) < ndays Then dicMatchDays(m.Giornata) = ndays
            Next

            If site = "gazzetta" OrElse site = "" Then str.Append(GetGazzetta(show))
            If site = "fantacalcio" OrElse site = "" Then str.Append(GetFantacalcio(show))
            If site = "pianetafantacalcio" OrElse site = "" Then str.Append(GetPianetaFantacalcio(show))
            'If site = "sky" OrElse site = "" Then str.Append(GetSky(show))
            ' If site = "cds" OrElse site = "" Then str.Append(GetCds(show))

            Return str.ToString()

        End Function

        Shared Function GetDataFileName(site As String) As String
            Return dirData & site.ToLower() & ".json"
        End Function

        Shared Sub AddInfo(Name As String, Team As String, Site As String, State As String, Info As String, Percentage As Integer, wpList As Dictionary(Of String, Torneo.ProbablePlayers.Probable.Player))

            If wpList.ContainsKey(Name & "/" & Team) = False Then
                If State = "Ballottaggio" Then State = "Panchina"
                wpList.Add(Name & "/" & Team, New Torneo.ProbablePlayers.Probable.Player(Name, Team, Site, State, Info, Percentage))
            Else

                Dim p As Torneo.ProbablePlayers.Probable.Player = wpList(Name & "/" & Team)
                If p.Info <> "" Then Info = "," & Info
                If State = "Ballottaggio" Then State = p.State
                p.Info += Info
                p.Percentage = Percentage
            End If

            Dim wp As Torneo.ProbablePlayers.Probable.Player = wpList(Name & "/" & Team)
            Dim GiorniFineCampionato As Integer = dicMatchDays.Values.Last()

            If Info <> "" AndAlso State = "Infortunato" Then
                Dim m As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(Info, "\d+(?=\s+settiman[ea])")
                If m.Success Then
                    wp.Infortunio.Giorni = CInt(m.Value) * 7
                Else
                    m = System.Text.RegularExpressions.Regex.Match(Info, "\d+(?=\s+mes[ie])")
                    If m.Success Then
                        wp.Infortunio.Giorni = CInt(m.Value) * 30
                    Else
                        m = System.Text.RegularExpressions.Regex.Match(Info, "\d+(?=(ª|a) giornata)")
                        If m.Success Then
                            Dim mday As Integer = CInt(m.Value)
                            If dicMatchDays.ContainsKey(mday) Then
                                wp.Infortunio.Giorni = dicMatchDays(mday)
                            End If
                        Else
                            m = System.Text.RegularExpressions.Regex.Match(Info, "agosto|settembre|ottobre|novembre|dicembre|gennaio|febbraio|marzo|aprile|maggio|giugno")
                            If m.Success Then
                                Dim y As Integer = Date.Now.Year
                                Dim mind As Integer = 1
                                Dim d As Integer = 1

                                If m.Value = "gennaio" Then
                                    mind = 1
                                    If Date.Now.Month > 7 Then y += 1
                                ElseIf m.Value = "febbraio" Then
                                    mind = 2
                                    If Date.Now.Month > 7 Then y += 1
                                ElseIf m.Value = "marzo" Then
                                    mind = 3
                                    If Date.Now.Month > 7 Then y += 1
                                ElseIf m.Value = "aprile" Then
                                    mind = 4
                                    If Date.Now.Month > 7 Then y += 1
                                ElseIf m.Value = "maggio" Then
                                    mind = 5
                                    If Date.Now.Month > 7 Then y += 1
                                ElseIf m.Value = "giugno" Then
                                    mind = 6
                                    If Date.Now.Month > 7 Then y += 1
                                ElseIf m.Value = "agosto" Then
                                    mind = 8
                                ElseIf m.Value = "settembre" Then
                                    mind = 9
                                ElseIf m.Value = "ottobre" Then
                                    mind = 10
                                ElseIf m.Value = "novembre" Then
                                    mind = 11
                                ElseIf m.Value = "dicembre" Then
                                    mind = 12
                                End If
                                If Info.Contains("seconda meta") Then
                                    d = 25
                                ElseIf Info.Contains("meta " & m.Value) Then
                                    d = 15
                                ElseIf Info.Contains("fine " & m.Value) Then
                                    d = 28
                                End If
                                Dim dtr As New Date(y, mind, d)

                                wp.Infortunio.Giorni = CInt(dtr.Subtract(Date.Now).TotalDays)

                            Else
                                If Info.Contains("out contro") Then
                                    wp.Infortunio.Giorni = 7
                                ElseIf Info.Contains("stagione finita") Then
                                    wp.Infortunio.Giorni = GiorniFineCampionato
                                End If
                            End If
                        End If
                    End If
                End If

                If wp.Infortunio.Giorni > 0 Then
                    If GiorniFineCampionato > 0 Then
                        wp.Infortunio.Severity = CInt(wp.Infortunio.Giorni * 100 / GiorniFineCampionato)
                        wp.Infortunio.Severity = wp.Infortunio.Giorni
                        If wp.Infortunio.Severity > 100 Then wp.Infortunio.Severity = 100
                    Else
                        wp.Infortunio.Severity = 0
                    End If
                Else
                    wp.Infortunio.Severity = 0
                End If

            End If

        End Sub

        Shared Function WriteData(Data As Torneo.ProbablePlayers.Probable, fileDestiNazione As String) As String

            Dim json As String = ""
            Try
                For Each p As String In Data.Players.Keys
                    Data.Players(p).Info = Data.Players(p).Info.Trim(","c)
                Next
                json = WebData.Functions.SerializzaOggetto(Data, False)
                IO.File.WriteAllText(fileDestiNazione, json)
            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return json

        End Function

    End Class
End Namespace