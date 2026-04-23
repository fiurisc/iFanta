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
                        clasa = clasa.OrderByDescending(Function(x) x.Punti.Totali).ToList()
                        For i As Integer = 0 To clasa.Count - 1
                            If clasa(i).Punti.Totali <> oldpt Then pos = i + 1
                            oldpt = clasa(i).Punti.Totali
                            clasa(i).Pos = pos
                        Next
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

    End Class

End Namespace
