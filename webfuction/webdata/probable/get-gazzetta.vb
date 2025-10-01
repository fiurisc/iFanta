Imports System.IO
Imports System.Runtime.InteropServices.ComTypes
Imports System.Text

Namespace WebData
    Partial Class ProbableFormations

        Shared Function GetGazzetta(ReturnData As Boolean) As String

            Dim site As String = "Gazzetta"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".txt"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"

            Dim currgg As Integer = -1
            Dim srLog As New IO.StreamWriter(fileLog)
            Dim rmsg As String = ""

            Try

                Players.Data.LoadPlayers(False)
                MatchsData.LoadWebMatchs()

                Dim html As String = Functions.GetPage("http://www.gazzetta.it/Calcio/prob_form/", "UTF-8")

                If html <> "" Then

                    IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.GetEncoding("UTF-8"))

                    Dim line() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.GetEncoding("UTF-8"))
                    Dim start As Boolean = False
                    Dim plaryersData As New Dictionary(Of String, Torneo.ProbablePlayer.Player)
                    Dim playersLog As New Dictionary(Of String, Players.PlayerMatch)
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""

                    srLog.WriteLine("Year -> " & Functions.Year)
                    srLog.WriteLine("Calendario match:")
                    srLog.WriteLine("---------------------------")
                    For Each t As String In MatchsData.KeyMatchs.Keys
                        srLog.WriteLine(MatchsData.KeyMatchs(t) & " -> " & t)
                    Next
                    srLog.WriteLine("")
                    srLog.WriteLine("linee file html => " & CStr(line.Length))

                    For i As Integer = 0 To line.Length - 1
                        If line(i) <> "" Then

                            If line(i).Contains("<p class=""lastUpdate"">") Then
                                sq.Clear()
                                sqid = -1
                            ElseIf line(i).Contains("class=""details-team__name""") Then
                                'Aggiungo la Squadra alla lista di quelle che disputano il match'
                                sq.Add(System.Text.RegularExpressions.Regex.Match(line(i + 2).Trim(), "(?<=\>)\w+(?=\<\/a)").Value.ToUpper)
                            ElseIf line(i).Contains("class=""match-details__info"">") Then

                                'Cerco di determinare la giornata di riferiemnto'
                                If sq.Count = 2 AndAlso currgg = -1 Then

                                    Dim match As String = sq(0) & "-" & sq(1)

                                    srLog.WriteLine("match trovato -> " & match)

                                    For Each key As String In MatchsData.KeyMatchs.Keys
                                        If key = match Then
                                            currgg = MatchsData.KeyMatchs(key)
                                            srLog.WriteLine("giornata associata -> " & CStr(currgg))
                                            Exit For
                                        End If
                                    Next
                                End If

                            ElseIf line(i).Contains("<p class=""is--home"">") Then
                                sqid = 0
                            ElseIf line(i).Contains("<p class=""is--away"">") Then
                                sqid = 1
                            ElseIf line(i).Contains("<div class=""lineup-team is--home"">") Then
                                sqid = 0
                            ElseIf line(i).Contains("<div class=""lineup-team is--away"">") Then
                                sqid = 1
                            ElseIf line(i).Contains("<span class=""lineup-team__name"">") Then

                                Dim name As String = ""

                                If line(i).Contains("De Roon") Then
                                    line(i) = line(i)
                                End If

                                name = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=""lineup-team__name\"">)[\w\s+]{1,}(?=\<)").Value.Replace("-", " ").ToUpper().Replace("'", "’")
                                If name = "LAUTARO" Then name = "MARTINEZ L."
                                If name <> "" Then
                                    name = Players.Data.ResolveName("", name, sq(sqid), playersLog, False).GetName()
                                    Call AddInfo(name, sq(sqid), site, "Titolare", "", 100, plaryersData)
                                End If

                            ElseIf RegularExpressions.Regex.Match(line(i), "\<strong\>(Panchina|Ballottaggio|Squalificati|Indisponibili):\s+\<\/strong\>").Value <> "" Then

                                Dim value As String = line(i)

                                If line(i).Contains("Panchina") Then
                                    pstate = "Panchina"
                                ElseIf line(i).Contains("Ballottaggio") Then
                                    pstate = "Ballottaggio"
                                ElseIf line(i).Contains("Squalificati") Then
                                    pstate = "Squalificato"
                                ElseIf line(i).Contains("Indisponibili") Then
                                    pstate = "Infortunato"
                                    value = line(i + 1)
                                End If

                                If value <> "" AndAlso value.Contains("Nessuno") = False Then
                                    Dim list() As String = value.Trim().Split(CChar(","))
                                    For Each Nome In list
                                        Try
                                            Dim info As String = ""
                                            Nome = Nome.Trim()
                                            If RegularExpressions.Regex.Match(Nome, "^\d+").Success Then
                                                Nome = Nome.Substring(Nome.IndexOf(" "))
                                            End If
                                            If RegularExpressions.Regex.Match(Nome, "\(").Success Then
                                                info = Nome.Substring(Nome.IndexOf("(") + 1).Replace(")", "").Trim()
                                                Nome = Nome.Substring(0, Nome.IndexOf("("))
                                            End If
                                            Nome = Nome.Trim().ToUpper()
                                            Nome = Players.Data.ResolveName("", Nome, sq(sqid), playersLog, False).GetName()
                                            Call AddInfo(Nome, sq(sqid), site, pstate, info, 0, plaryersData)
                                        Catch ex As Exception

                                        End Try
                                    Next
                                End If
                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        Dim out As String = WriteData(currgg, plaryersData, fileData)
                        If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(playersLog, filePlayers)
                        rmsg = out.Replace(System.Environment.NewLine, "</br>")
                    End If

                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
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