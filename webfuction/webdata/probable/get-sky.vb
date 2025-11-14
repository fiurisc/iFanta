Imports System.Web.Script.Serialization

Namespace WebData
    Partial Class ProbableFormations

        Public Function GetSky(ReturnData As Boolean) As String

            Dim site As String = "Sky"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".json"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"
            Dim srLog As New IO.StreamWriter(fileLog)
            Dim rmsg As String = ""
            Dim currgg As Integer = -1
            Dim enc As String = "UTF-8"

            Try

                Players.Data.LoadPlayers(appSett, False)

                Dim html As String = Functions.GetPage(appSett, "https://sport.sky.it/calcio/serie-a/probabili-formazioni", "UTF-8")

                If html <> "" Then

                    IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.GetEncoding(enc))
                    Dim plaryersData As New Torneo.ProbablePlayers.Probable
                    Dim playersLog As New Dictionary(Of String, Players.PlayerMatch)
                    Dim team As String = ""
                    Dim name As String = ""
                    Dim sez As String = "header"
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0

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
                                Dim json As String = System.Text.RegularExpressions.Regex.Match(line, "\{""create.*}(?=' query)").Value().Replace(vbCrLf, vbCr).Replace(vbLf, "")
                                Dim sublines() As String = Functions.FormatJson(json).Split(Convert.ToChar(13))

                                For k As Integer = 0 To sublines.Length - 1

                                    Dim linej As String = sublines(k)
                                    Dim pname As String = Functions.GetJsonPropertyName(linej)
                                    Dim pvalue As String = Functions.GetJsonPropertyValue(linej)

                                    If linej.EndsWith("{") OrElse linej.EndsWith("[") Then
                                        paths.Add(pname)
                                        cpath = Torneo.Functions.ConvertListStringToString(paths, "/")
                                    ElseIf (linej.EndsWith("],") OrElse linej.EndsWith("},") OrElse linej.EndsWith("}") OrElse linej.EndsWith("]")) Then
                                        If paths.Count > 0 Then paths.RemoveAt(paths.Count - 1)
                                    Else
                                        If pname = "seoName" Then
                                            team = Functions.CheckTeamName(pvalue.ToUpper())
                                        End If
                                        If pname = "fullname" AndAlso cpath.Contains("substitutes") Then
                                            Dim plist() As String = pvalue.ToUpper().Split(Convert.ToChar(" "))
                                            For Each p As String In plist
                                                Dim pm As Players.PlayerMatch = Players.Data.ResolveName("", p, team, False)
                                                name = pm.GetName()
                                                Call AddInfo(name, team, site, "Panchina", "", 0, plaryersData.Players)
                                            Next
                                        ElseIf pname = "fullname" AndAlso cpath.Contains("startingLineup") Then
                                            Dim pm As Players.PlayerMatch = Players.Data.ResolveName("", pvalue.ToUpper(), team, False)
                                            name = pm.GetName()
                                            Call AddInfo(name, team, site, "Titolare", "", 0, plaryersData.Players)
                                        ElseIf pname = "round" Then
                                            currgg = Convert.ToInt32(pvalue)
                                        End If

                                    End If
                                Next
                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        Dim out As String = WriteData(plaryersData, fileData)
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