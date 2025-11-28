Imports webfuction.Torneo.MatchsData

Namespace WebData

    Public Class Classifica

        Dim appSett As Torneo.PublicVariables

        Sub New(appSett As Torneo.PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function GetDataFileName(giornata As Integer) As String
            Return appSett.WebDataPath & "data\classifica-seriea-" & giornata & ".json"
        End Function

        Public Function GetClassifica(ReturnData As Boolean) As String
            Try
                Dim outstr As New System.Text.StringBuilder

                For i As Integer = GetLastRankDayLoaded() To 38
                    If GetClassificaGiornata(i) Then
                        outstr.AppendLine("<span>Giornata " & i & " => OK")
                    Else
                        Exit For
                    End If
                Next
                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Ranking (" & appSett.Year & "):</span></br>" & outstr.ToString().Replace(System.Environment.NewLine, "</br>") & "</br>"
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Ranking (" & appSett.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                Return ex.Message
            End Try

        End Function

        Public Function GetClassificaGiornata(giornata As Integer) As Boolean

            Dim filed As String = GetDataFileName(giornata)
            Dim filet As String = appSett.WebDataPath & "temp\classifica-seriea-" & giornata & ".txt"
            Dim dataFound As Boolean = False
            Dim clasa As New List(Of Torneo.ClassificaSerieA.ClassificaItem)

            Try

                Dim html As String = Functions.GetPage(appSett, "https://www.datasport.it/calcio/serie-a" & appSett.Year & "-" & CInt(appSett.Year) + 1 & "/classifica/" & giornata & ".html")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html)

                    Dim lines() As String = IO.File.ReadAllLines(filet)
                    Dim tname As String = ""
                    Dim start As Boolean = False
                    Dim datas As New List(Of Integer)
                    Dim oldpt As Integer = -1
                    Dim pos As Integer = 0

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            If line.Contains("Giornata non ancora disputata") Then
                                dataFound = False
                                Exit For
                            ElseIf line.Contains("data-responsive-width=""4-5""><") Then
                                start = True
                                dataFound = True
                            ElseIf start AndAlso line.Contains("</tr>") Then

                                start = False

                                If datas.Count > 0 Then

                                    Dim item As New Torneo.ClassificaSerieA.ClassificaItem
                                    item.Squadra = tname
                                    item.Giornata = giornata
                                    item.Punti.Totali = datas(0)
                                    item.PartiteGiocate.Totali = datas(1)
                                    item.Vittorie.Totali = datas(2)
                                    item.Pareggi.Totali = datas(3)
                                    item.Sconfitte.Totali = datas(4)
                                    item.GoalFatti.Totali = datas(5)
                                    item.GoalSubiti.Totali = datas(6)

                                    item.PartiteGiocate.Dentro = datas(7)
                                    item.Vittorie.Dentro = datas(8)
                                    item.Pareggi.Dentro = datas(9)
                                    item.Sconfitte.Dentro = datas(10)
                                    item.GoalFatti.Dentro = datas(11)
                                    item.GoalSubiti.Dentro = datas(12)
                                    item.Punti.Dentro = (datas(8) * 3 + datas(9))

                                    item.PartiteGiocate.Fuori = datas(13)
                                    item.Vittorie.Fuori = datas(14)
                                    item.Pareggi.Fuori = datas(15)
                                    item.Sconfitte.Fuori = datas(16)
                                    item.GoalFatti.Fuori = datas(17)
                                    item.GoalSubiti.Fuori = datas(18)
                                    item.Punti.Fuori = (datas(14) * 3 + datas(15))
                                    If item.Punti.Totali <> oldpt Then pos = clasa.Count() + 1
                                    oldpt = item.Punti.Totali
                                    item.Pos = pos

                                    clasa.Add(item)

                                    datas.Clear()

                                End If

                            ElseIf start AndAlso line.Contains("data-responsive-width=""20-40""") Then
                                tname = System.Text.RegularExpressions.Regex.Match(line, "(?<=\>)\w+(?=\<\/a)").Value.ToUpper()
                                tname = WebData.Functions.CheckTeamName(tname)
                            ElseIf start AndAlso (line.Contains("data-responsive-width=""4-10""") OrElse line.Contains("data-responsive-width=""4-0""")) Then
                                datas.Add(CInt(System.Text.RegularExpressions.Regex.Match(line, "(?<=\>)\w+(?=\<)").Value))
                            End If
                        End If
                    Next

                    If dataFound Then
                        Dim cdata As New Torneo.ClassificaSerieA(appSett)
                        cdata.UpdateClassificaData(giornata, clasa)
                        IO.File.WriteAllText(filed, Functions.SerializzaOggetto(clasa, False))
                    End If
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return dataFound

        End Function

        Private Function GetLastRankDayLoaded() As Integer
            For i As Integer = 1 To 38
                If IO.File.Exists(GetDataFileName(i)) = False Then
                    If i > 1 Then
                        Return i - 1
                    Else
                        Return 1
                    End If
                End If
            Next
            Return 38
        End Function

        'Public Function GetClassifica1(ReturnData As Boolean) As String

        '    Dim filed As String = GetDataFileName(appSett)
        '    Dim filet As String = appSett.WebDataPath & "temp\classifica-seriea.txt"

        '    Dim clasa As New List(Of ClassificaItem)

        '    Try

        '        Dim html As String = Functions.GetPage(appSett, "http://www.gazzetta.it/calcio/serie-a/classifica/")

        '        If html <> "" Then

        '            IO.File.WriteAllText(filet, html)

        '            Dim line() As String = IO.File.ReadAllLines(filet)

        '            Dim tname As String = ""
        '            Dim pt() As Integer = {0, 0, 0} 'punti tot / punti dentro / punit fuori'
        '            Dim pg() As Integer = {0, 0, 0} 'partite giocate tot / partite giocate dentro / partite giocate fuori'
        '            Dim vit() As Integer = {0, 0, 0} 'vittorie tot / vittorie dentro / vittorie fuori'
        '            Dim per() As Integer = {0, 0, 0} 'sconfitte tot / sconfitte dentro / sconfitte fuori'
        '            Dim par() As Integer = {0, 0, 0} 'pareggi tot / pareggi dentro / pareggi fuori'
        '            Dim gf() As Integer = {0, 0, 0} 'goal fatti tot / goal fatti dentro / goal fatti fuori'
        '            Dim gs() As Integer = {0, 0, 0} 'goal subiti tot / goal subiti dentro / goal subiti fuori'

        '            For i As Integer = 0 To line.Length - 1

        '                Dim sreg As String = "(homeRanking|awayRanking)\"":{[a-zA-Z0-9\s\"":,\/_\[\]]{0,}({[a-zA-Z0-9\s\"":,_]{0,})?"

        '                If System.Text.RegularExpressions.Regex.Match(line(i), sreg).Value.Length > 0 Then

        '                    Dim m As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(line(i), sreg)

        '                    For k As Integer = 0 To m.Count - 1
        '                        tname = System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=team_?[nN]ame\"":"")\w+").Value.ToUpper
        '                        If tname <> "" Then

        '                            tname = WebData.Functions.CheckTeamName(tname)

        '                            Dim t As Integer = 1

        '                            If m(k).Value.Contains("awayRanking") Then t = 2
        '                            If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsConceded\"":)\d+").Value.Length > 0 Then
        '                                gs(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsConceded\"":)\d+").Value)
        '                            End If
        '                            If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsMade\"":)\d+").Value.Length > 0 Then
        '                                gf(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsMade\"":)\d+").Value)
        '                            End If
        '                            If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=lost\"":)\d+").Value.Length > 0 Then
        '                                per(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=lost\"":)\d+").Value)
        '                            End If
        '                            If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=played\"":)\d+").Value.Length > 0 Then
        '                                pg(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=played\"":)\d+").Value)
        '                            End If
        '                            If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=points\"":)\d+").Value.Length > 0 Then
        '                                pt(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=points\"":)\d+").Value)
        '                            End If
        '                            If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=won\"":)\d+").Value.Length > 0 Then
        '                                vit(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=won\"":)\d+").Value)
        '                                par(t) = pg(t) - vit(t) - per(t)
        '                            End If

        '                            If m(k).Value.Contains("awayRanking") Then

        '                                'Determino i totali dai valori di dentro e fuori casa'
        '                                pt(0) = pt(1) + pt(2)
        '                                pg(0) = pg(1) + pg(2)
        '                                vit(0) = vit(1) + vit(2)
        '                                per(0) = per(1) + per(2)
        '                                par(0) = par(1) + par(2)
        '                                gf(0) = gf(1) + gf(2)
        '                                gs(0) = gs(1) + gs(2)

        '                                Dim item As New ClassificaItem
        '                                item.Nome = tname
        '                                item.Punti.Totali = pt(0).ToString()
        '                                item.Punti.Dentro = pt(1).ToString()
        '                                item.Punti.Fuori = pt(2).ToString()
        '                                item.PartiteGiocate.Totali = pg(0).ToString()
        '                                item.PartiteGiocate.Dentro = pg(1).ToString()
        '                                item.PartiteGiocate.Fuori = pg(2).ToString()
        '                                item.Vittorie.Totali = vit(0).ToString()
        '                                item.Vittorie.Dentro = vit(1).ToString()
        '                                item.Vittorie.Fuori = vit(2).ToString()
        '                                item.Pareggi.Totali = par(0).ToString()
        '                                item.Pareggi.Dentro = par(1).ToString()
        '                                item.Pareggi.Fuori = par(2).ToString()
        '                                item.Sconfitte.Totali = per(0).ToString()
        '                                item.Sconfitte.Dentro = per(1).ToString()
        '                                item.Sconfitte.Fuori = per(2).ToString()
        '                                item.GoalFatti.Totali = gf(0).ToString()
        '                                item.GoalFatti.Dentro = gf(1).ToString()
        '                                item.GoalFatti.Fuori = gf(2).ToString()
        '                                item.GoalSubiti.Totali = gs(0).ToString()
        '                                item.GoalSubiti.Dentro = gs(1).ToString()
        '                                item.GoalSubiti.Fuori = gs(2).ToString()
        '                                clasa.Add(item)
        '                            End If
        '                        End If
        '                    Next
        '                End If

        '            Next

        '            IO.File.WriteAllText(filed, Functions.SerializzaOggetto(clasa, False))

        '        End If

        '        If ReturnData Then
        '            Return "</br><span style=color:red;font-size:bold;'>Ranking (" & appSett.Year & "):</span></br>" & WebData.Functions.SerializzaOggetto(clasa, False).Replace(System.Environment.NewLine, "</br>") & "</br>"
        '        Else
        '            Return ("</br><span style=color:red;font-size:bold;'>Ranking (" & appSett.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
        '        End If

        '    Catch ex As Exception
        '        WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
        '        Return ex.Message
        '    End Try

        'End Function

    End Class

End Namespace
