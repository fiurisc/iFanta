Imports System.IO
Imports System.Runtime.InteropServices.ComTypes
Imports System.Text

Namespace WebData
    Partial Class ProbableFormations

        Public Function GetGazzetta(ReturnData As Boolean) As String

            Dim site As String = "Gazzetta"
            Dim fileJson As String = GetDataFileName(site)
            Dim fileTemp As String = dirTemp & site.ToLower() & ".txt"
            Dim fileData As String = dirData & site.ToLower() & ".json"
            Dim filePlayers As String = dirData & site.ToLower() & "-players.txt"
            Dim fileLog As String = dirData & site.ToLower() & ".log"

            Dim currgg As Integer = -1
            Dim srLog As New IO.StreamWriter(fileLog)
            Dim rmsg As String = ""

            Try

                Dim html As String = Functions.GetPage(appSett, "http://www.gazzetta.it/Calcio/prob_form/", "UTF-8")

                If html <> "" Then

                    IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.Default)

                    Dim line() As String = IO.File.ReadAllLines(fileTemp, System.Text.Encoding.Default)
                    Dim start As Boolean = False
                    Dim plaryersData As New Torneo.ProbablePlayers.Probable
                    Dim playersLog As New Dictionary(Of String, Players.PlayerMatch)
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""

                    srLog.WriteLine("Year -> " & appSett.Year)
                    srLog.WriteLine("Calendario match:")
                    srLog.WriteLine("---------------------------")
                    For Each t As String In mdataw.KeyMatchs.Keys
                        srLog.WriteLine(mdataw.KeyMatchs(t).Giornata & " -> " & t)
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
                                sq.Add(Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(line(i + 2).Trim(), "(?<=\>)[\w\s]{1,}(?=\<\/a)").Value.ToUpper))
                            ElseIf line(i).Contains("class=""match-details__info"">") Then

                                'Cerco di determinare la giornata di riferiemnto'
                                If sq.Count = 2 AndAlso currgg = -1 Then

                                    Dim match As String = sq(0) & "-" & sq(1)

                                    srLog.WriteLine("match trovato -> " & match)

                                    For Each key As String In mdataw.KeyMatchs.Keys
                                        If key = match Then
                                            currgg = mdataw.KeyMatchs(key).Giornata
                                            plaryersData.Day = currgg
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

                                name = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=""lineup-team__name\"">)[\w\s+\-]{1,}(?=\<)").Value.Replace("-", " ").ToUpper().Replace("'", "’")
                                If name = "LAUTARO" Then name = "MARTINEZ L."
                                If name.Contains("MILINK") Then
                                    name = name
                                End If
                                If name <> "" Then
                                    name = Players.Data.ResolveName("", name, sq(sqid), playersLog, False).GetName()
                                    Call AddInfo(name, sq(sqid), site, "Titolare", "", 100, plaryersData.Players)
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

                                    If pstate <> "Infortunato" Then

                                        If value.Contains("Pellegrini") Then
                                            value = value
                                        End If

                                        value = value.Replace("<strong>Panchina: </strong>", "").Replace("<strong>Ballottaggio: </strong>", "").Replace("<strong>Squalificato: </strong>", "").Replace("<strong>Squalificati: </strong>", "")
                                        If pstate = "Panchina" Then
                                            value += line(i + 1).Replace("</p>", "")
                                            value = System.Text.RegularExpressions.Regex.Replace(value.Trim(), "(?<=\w\s)(\d)", ",$1")
                                        End If

                                    End If
                                    Dim items() As String = value.Replace(" e ", ",").Replace(") ", "),").Trim().Split(CChar(","))
                                    For Each item In items
                                        Try
                                            If item.Trim() <> "" Then
                                                If item.Contains("Brescianini-Ndour") Then
                                                    item = item
                                                End If
                                                If pstate = "Ballottaggio" Then
                                                    Dim subItems() As String = item.Replace("%", "").Split(CChar(" "))
                                                    If subItems.Length = 2 Then
                                                        Dim nomi() As String = RegularExpressions.Regex.Match(item.Trim(), "[a-zA-Z\s]{1,}-[a-zA-Z\s]{1,}").Value.Split(CChar("-"))
                                                        Dim perc() As String = RegularExpressions.Regex.Match(item.Trim().Replace("%", ""), "\d+\s?-\s?\d+").Value.Split(CChar("-"))
                                                        If nomi.Length = perc.Length Then
                                                            For n As Integer = 0 To nomi.Length - 1
                                                                If perc(n) <> "" Then
                                                                    Dim nome = Functions.NormalizeText(nomi(n))
                                                                    nome = Players.Data.ResolveName("", nome, sq(sqid), playersLog, False).GetName()
                                                                    Call AddInfo(nome, sq(sqid), site, "", "", CInt(perc(n)), plaryersData.Players)
                                                                End If
                                                            Next
                                                        End If
                                                    End If
                                                Else
                                                    Dim info As String = ""
                                                    Dim Nome As String = item.Trim()
                                                    If RegularExpressions.Regex.Match(Nome, "^\d+").Success Then
                                                        Nome = Nome.Substring(Nome.IndexOf(" "))
                                                    End If
                                                    If RegularExpressions.Regex.Match(Nome, "\(").Success Then
                                                        info = Nome.Substring(Nome.IndexOf("(") + 1).Replace(")", "").Trim()
                                                        Nome = Nome.Substring(0, Nome.IndexOf("("))
                                                    End If
                                                    Nome = Nome.Trim().ToUpper()
                                                    Nome = Functions.NormalizeText(Nome)
                                                    info = Functions.NormalizeText(info)
                                                    Nome = Players.Data.ResolveName("", Nome, sq(sqid), playersLog, False).GetName()
                                                    Call AddInfo(Nome, sq(sqid), site, pstate, info, 0, plaryersData.Players)
                                                End If
                                            End If
                                        Catch ex As Exception
                                            Debug.WriteLine(ex.Message)
                                        End Try
                                    Next
                                End If
                            End If
                        End If
                    Next

                    If currgg <> -1 Then
                        If dicMatchDays(currgg) > 0 Then WriteBackupProbableHtml(fileTemp, dirData & currgg & "\" & site.ToLower() & ".txt")
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