Namespace WebData

    Public Class Classifica

        Private Shared dirt As String = ""
        Private Shared dird As String = ""

        Private Shared Sub SetFolder()
            dirt = Functions.DataPath & "temp\"
            dird = Functions.DataPath & "data\"
        End Sub

        Shared Function GetDataFileName() As String
            SetFolder()
            Return dird & "classifica-seriea.json"
        End Function

        Public Shared Function GetClassifica(ReturnData As Boolean) As String

            Dim filed As String = GetDataFileName()
            Dim filet As String = dirt & "classifica-seriea.txt"

            Dim clasa As New List(Of ClassificaItem)

            Try

                Dim html As String = Functions.GetPage("http://www.gazzetta.it/calcio/serie-a/classifica/")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html)

                    Dim line() As String = IO.File.ReadAllLines(filet)

                    Dim tname As String = ""
                    Dim pt() As Integer = {0, 0, 0} 'punti tot / punti dentro / punit fuori'
                    Dim pg() As Integer = {0, 0, 0} 'partite giocate tot / partite giocate dentro / partite giocate fuori'
                    Dim vit() As Integer = {0, 0, 0} 'vittorie tot / vittorie dentro / vittorie fuori'
                    Dim per() As Integer = {0, 0, 0} 'sconfitte tot / sconfitte dentro / sconfitte fuori'
                    Dim par() As Integer = {0, 0, 0} 'pareggi tot / pareggi dentro / pareggi fuori'
                    Dim gf() As Integer = {0, 0, 0} 'goal fatti tot / goal fatti dentro / goal fatti fuori'
                    Dim gs() As Integer = {0, 0, 0} 'goal subiti tot / goal subiti dentro / goal subiti fuori'

                    For i As Integer = 0 To line.Length - 1

                        Dim sreg As String = "(homeRanking|awayRanking)\"":{[a-zA-Z0-9\s\"":,\/_\[\]]{0,}({[a-zA-Z0-9\s\"":,_]{0,})?"

                        If System.Text.RegularExpressions.Regex.Match(line(i), sreg).Value.Length > 0 Then

                            Dim m As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(line(i), sreg)

                            For k As Integer = 0 To m.Count - 1
                                tname = System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=team_?[nN]ame\"":"")\w+").Value.ToUpper
                                If tname <> "" Then

                                    Dim t As Integer = 1

                                    If m(k).Value.Contains("awayRanking") Then t = 2
                                    If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsConceded\"":)\d+").Value.Length > 0 Then
                                        gs(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsConceded\"":)\d+").Value)
                                    End If
                                    If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsMade\"":)\d+").Value.Length > 0 Then
                                        gf(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=goalsMade\"":)\d+").Value)
                                    End If
                                    If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=lost\"":)\d+").Value.Length > 0 Then
                                        per(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=lost\"":)\d+").Value)
                                    End If
                                    If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=played\"":)\d+").Value.Length > 0 Then
                                        pg(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=played\"":)\d+").Value)
                                    End If
                                    If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=points\"":)\d+").Value.Length > 0 Then
                                        pt(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=points\"":)\d+").Value)
                                    End If
                                    If System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=won\"":)\d+").Value.Length > 0 Then
                                        vit(t) = CInt(System.Text.RegularExpressions.Regex.Match(m(k).Value, "(?<=won\"":)\d+").Value)
                                        par(t) = pg(t) - vit(t) - per(t)
                                    End If

                                    If m(k).Value.Contains("awayRanking") Then

                                        'Determino i totali dai valori di dentro e fuori casa'
                                        pt(0) = pt(1) + pt(2)
                                        pg(0) = pg(1) + pg(2)
                                        vit(0) = vit(1) + vit(2)
                                        per(0) = per(1) + per(2)
                                        par(0) = par(1) + par(2)
                                        gf(0) = gf(1) + gf(2)
                                        gs(0) = gs(1) + gs(2)

                                        Dim item As New ClassificaItem
                                        item.Nome = tname
                                        item.Punti.Totali = pt(0).ToString()
                                        item.Punti.Dentro = pt(1).ToString()
                                        item.Punti.Fuori = pt(2).ToString()
                                        item.PartiteGiocate.Totali = pg(0).ToString()
                                        item.PartiteGiocate.Dentro = pg(1).ToString()
                                        item.PartiteGiocate.Fuori = pg(2).ToString()
                                        item.Vittorie.Totali = vit(0).ToString()
                                        item.Vittorie.Dentro = vit(1).ToString()
                                        item.Vittorie.Fuori = vit(2).ToString()
                                        item.Pareggi.Totali = par(0).ToString()
                                        item.Pareggi.Dentro = par(1).ToString()
                                        item.Pareggi.Fuori = par(2).ToString()
                                        item.Sconfitte.Totali = per(0).ToString()
                                        item.Sconfitte.Dentro = per(1).ToString()
                                        item.Sconfitte.Fuori = per(2).ToString()
                                        item.GoalFatti.Totali = gf(0).ToString()
                                        item.GoalFatti.Dentro = gf(1).ToString()
                                        item.GoalFatti.Fuori = gf(2).ToString()
                                        item.GoalSubiti.Totali = gs(0).ToString()
                                        item.GoalSubiti.Dentro = gs(1).ToString()
                                        item.GoalSubiti.Fuori = gs(2).ToString()
                                        clasa.Add(item)
                                    End If
                                End If
                            Next
                        End If

                    Next

                    IO.File.WriteAllText(filed, Functions.SerializzaOggetto(clasa, False))

                End If

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Ranking (" & Functions.Year & "):</span></br>" & WebData.Functions.SerializzaOggetto(clasa, False).Replace(System.Environment.NewLine, "</br>") & "</br>"
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Ranking (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(WebData.Functions.eMessageType.Errors, ex.Message)
                Return ex.Message
            End Try

        End Function

        Public Class ClassificaItem
            Public Property Nome As String = ""
            Public Property Punti As New SubItem
            Public Property PartiteGiocate As New SubItem
            Public Property Vittorie As New SubItem
            Public Property Pareggi As New SubItem
            Public Property Sconfitte As New SubItem
            Public Property GoalFatti As New SubItem
            Public Property GoalSubiti As New SubItem

            Public Class SubItem
                Public Property Totali As String = ""
                Public Property Dentro As String = ""
                Public Property Fuori As String = ""
            End Class
        End Class

    End Class

End Namespace
