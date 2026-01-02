
Imports System.Net.NetworkInformation

Namespace WebData
    Partial Class ProbableFormations

        Public Function GetFantaPazz(ReturnData As Boolean) As String

            Dim dirt As String = appSett.WebDataPath & "\temp"
            Dim dird As String = appSett.WebDataPath & "\data\pforma"
            Dim site As String = "FantaPazz"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".json"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"

            Dim currgg As Integer = -1

            Try

                Players.Data.LoadPlayers(appSett, False)

                Dim html As String = Functions.GetPage(appSett, "https://www.fantapazz.com/calcio/fantacalcio/serie-a/probabili-formazioni", "utf-8")

                If html <> "" Then

                    IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.Default)

                    Dim start As Boolean = False
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""

                    Dim lines() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.Default)
                    Dim plaryersData As New Torneo.ProbablePlayers.Probable
                    Dim playersLog As New Dictionary(Of String, Players.PlayerMatch)

                    For i As Integer = 0 To lines.Length - 1

                        lines(i) = lines(i).Replace(vbTab, "")

                        If lines(i) <> "" Then

                            If lines(i).Contains("<div class=""text-center partita-header-xs"">") Then
                                sq.Clear()
                            ElseIf lines(i).Contains("<div class=""nomeClub""><h2>") Then

                                sq.Add(Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=h2>)\w+(?=\<\/h2)").Value.ToUpper()))

                                'Cerco di determinare la giornata di riferimento'
                                If sq.Count = 2 AndAlso currgg = -1 Then

                                    Dim match As String = sq(0) & "-" & sq(1)

                                    For Each key As String In mdataw.KeyMatchs.Keys
                                        If key = match Then
                                            currgg = mdataw.KeyMatchs(key).Giornata
                                            plaryersData.Day = currgg
                                            Exit For
                                        End If
                                    Next
                                End If

                            ElseIf lines(i).Contains("<span class='calciatore'>") Then
                                start = True
                                pstate = "Titolare"
                                Dim ms As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(lines(i), "(?<=ul\>)(.*?)(?=\<\/ul)")
                                For l As Integer = 0 To ms.Count - 1
                                    Dim sublines() As String = ms(l).Value.Replace("</li>", "|").Split(Convert.ToChar("|"))
                                    For Each sline As String In sublines
                                        Dim pname As String = System.Text.RegularExpressions.Regex.Match(sline, "(?<='calciatore'\>)(.*?)(?=\<\/span)").Value
                                        If pname.Contains("artine") Then
                                            pname = pname
                                        End If
                                        pname = Players.Data.ResolveName("", pname, team, playersLog, False).GetName()
                                        Call AddInfo(pname, team, site, If(l = 0, "Titolare", "Panchina"), "", -1, plaryersData.Players)
                                    Next

                                Next
                            ElseIf lines(i).Contains("<div class=""squalificati""><div class=""titolo"">") OrElse lines(i).Contains("<div class=""indisponibili""><div class=""titolo"">") Then
                                If lines(i).Contains("<div class=""squalificati""><div class=""titolo"">") Then
                                    pstate = "Squalificato"
                                Else
                                    pstate = "Infortunato"
                                End If
                                Dim line As String = System.Text.RegularExpressions.Regex.Match(lines(i), "(?<=ul\>)(.*?)(?=\<\/ul)").Value
                                If line <> "" Then
                                    Dim sublines() = line.Replace("<li>", "").Replace("<span  class='motivazione'>", "").Replace("</span>", "").Replace("[", "@").Replace("]", "").Replace("</li>", "|").Split(Convert.ToChar("|"))
                                    For Each sline As String In sublines
                                        If sline <> "" Then
                                            Dim sfields() As String = sline.Split(CChar("@"))
                                            Dim pname As String = sfields(0).Trim().ToUpper()
                                            Dim pinfo As String = If(sfields.Length > 1, sfields(1), "").Trim()
                                            pname = Players.Data.ResolveName("", pname, team, playersLog, False).GetName()
                                            Call AddInfo(pname, team, site, pstate, pinfo, -1, plaryersData.Players)
                                        End If
                                    Next
                                End If
                            ElseIf lines(i).Contains("<div class=""col-xs-6 home"">") Then
                                team = sq(0)
                            ElseIf lines(i).Contains("<div class=""col-xs-6 away"">") Then
                                team = sq(1)
                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        plaryersData.Day = currgg
                        Dim out As String = WriteData(plaryersData, fileData)
                        If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(appSett, playersLog, filePlayers)
                        Return out.Replace(System.Environment.NewLine, "</br>")
                    Else
                        Return ""
                    End If
                Else
                    Return ""
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                Return ex.Message
            End Try

        End Function
    End Class
End Namespace