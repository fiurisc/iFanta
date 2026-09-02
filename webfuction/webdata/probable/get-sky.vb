Namespace WebData
    Partial Class ProbableFormations

        Public Function GetSky(ReturnData As Boolean) As String
            Return GetSky(ReturnData, False)
        End Function

        Public Function GetSky(ReturnData As Boolean, FromBackup As Boolean, Optional Giornata As Integer = -1) As String

            Dim currgg As Integer = Giornata
            Dim site As String = "Sky"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".json"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"
            Dim fileBakupHtml As String = GetBackupHtmlDataFileName(site.ToLower(), currgg)
            Dim srLog As New IO.StreamWriter(fileLog)
            Dim rmsg As String = ""

            Dim enc As String = "UTF-8"

            Try

                Players.Data.LoadPlayers(appSett, False)

                Dim html As String = ""

                If FromBackup Then
                    fileTemp = fileBakupHtml
                    If IO.File.Exists(fileBakupHtml) Then html = "ok"
                Else
                    html = Functions.GetPage(appSett, "https://sport.sky.it/calcio/serie-a/probabili-formazioni", "UTF-8")
                    IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.GetEncoding("UTF-8"))
                End If

                If html <> "" Then


                    Dim lines() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.GetEncoding("UTF-8"))
                    Dim plaryersData As New Torneo.ProbablePlayers.Probable
                    Dim playersLog As New Dictionary(Of String, Players.PlayerMatch)
                    Dim team As String = ""
                    Dim name As String = ""
                    Dim sez As String = "header"
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim modf As String = ""
                    Dim modp As New List(Of Integer)

                    srLog.WriteLine("Year -> " & appSett.Year)
                    srLog.WriteLine("Calendario match:")
                    srLog.WriteLine("---------------------------")
                    For Each t As String In mdataw.KeyMatchs.Keys
                        srLog.WriteLine(mdataw.KeyMatchs(t).Giornata & " -> " & t)
                    Next
                    srLog.WriteLine("")
                    srLog.WriteLine("linee file html => " & CStr(lines.Length))

                    Dim paths As New List(Of String)
                    Dim cpath As String = ""

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            If line.Contains("competition-predicted-lineups") Then

                                Dim json As String = System.Text.RegularExpressions.Regex.Match(line, "(?<=matchList"":\[).*(?=])").Value().Replace(vbCrLf, vbCr).Replace(vbLf, "")
                                Dim sublines() As String = Functions.FormatJson(json).Split(Convert.ToChar(13))
                                Dim pstate As String = ""

                                For k As Integer = 0 To sublines.Length - 1

                                    Dim linej As String = sublines(k)
                                    Dim pname As String = Functions.GetJsonPropertyName(linej)
                                    Dim pvalue As String = Functions.GetJsonPropertyValue(linej)

                                    If linej.Contains("giornata-") Then
                                        currgg = CInt(System.Text.RegularExpressions.Regex.Match(linej, "(?<=giornata-)\d+").Value)
                                    ElseIf linej.Contains("disqualifieds") Then
                                        pstate = "Squalificato"
                                    ElseIf linej.Contains("substitutes") Then
                                        pstate = "Panchina"
                                    ElseIf linej.Contains("startingLineup") Then
                                        pstate = "Titolare"
                                    ElseIf linej.Contains("unavailables") Then
                                        pstate = "Infortunati"
                                    ElseIf linej.Contains("seoName") Then
                                        team = Functions.CheckTeamName(pvalue.ToUpper())
                                    ElseIf linej.Contains("]") Then
                                        pstate = ""
                                    ElseIf linej.Contains("fullName") AndAlso pstate <> "" Then
                                        Dim plist As Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(pvalue.ToUpper(), "(\w{1,2}[\s\.])?\w{2,}(\s\w{1,2}\.)?")
                                        For Each p As Text.RegularExpressions.Match In plist
                                            Dim pm As Players.PlayerMatch = Players.Data.ResolveName("", p.Value, team, playersLog, False)
                                            name = pm.GetName()
                                            Call AddInfo(name, team, site, pstate, "", 0, plaryersData.Players)
                                        Next
                                    End If

                                Next
                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        plaryersData.Day = currgg
                        fileBakupHtml = GetBackupHtmlDataFileName(site.ToLower(), currgg)
                        If dicMatchDays(currgg) > 0 AndAlso FromBackup = False Then WriteBackupProbableHtml(fileTemp, fileBakupHtml)
                        Dim fileBackup As String = dirData & currgg & "\" & site.ToLower() & ".json"
                        Dim out As String = WriteData(plaryersData, fileData, If(dicMatchDays(currgg) > 0, fileBackup, ""))
                        If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(appSett, playersLog, filePlayers)
                        rmsg = out.Replace(System.Environment.NewLine, "</br>")
                    End If

                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                rmsg = ex.Message
            End Try

            srLog.Close()

            If ReturnData Then
                Return "</br><span style=color:red;font-size:bold;'>Probabili formazioni gazzetta:</span></br>" & rmsg.Replace(System.Environment.NewLine, "</br>") & "</br>"
            Else
                Return "</br><span style=color:red;font-size:bold;'>Probabili formazioni gazzetta:</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>"
            End If

        End Function
    End Class
End Namespace