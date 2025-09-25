Namespace WebData

    Public Class Ranking

        Public Shared Function GetRanking(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(Functions.Year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(Functions.Year) & "\data"
            Dim filet As String = dirt & "\ranking-data.txt"
            Dim filed As String = dird & "\ranking-data.txt"
            Dim strdata As New System.Text.StringBuilder

            Functions.Dirs = ServerPath

            Try

                Dim html As String = Functions.GetPage("http://www.gazzetta.it/calcio/serie-a/classifica/", "POST", "")

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

                                        strdata.Append("name=" & tname & "|")

                                        strdata.Append("pt_t=" & pt(0) & "|") 'punti totali;
                                        strdata.Append("pt_d=" & pt(1) & "|") 'punti dentro casa'
                                        strdata.Append("pt_f=" & pt(2) & "|") 'punti fuori casa'

                                        strdata.Append("pg_t=" & pg(0) & "|") 'partite giocate'
                                        strdata.Append("pg_d=" & pg(1) & "|") 'partite giocate dentro casa'
                                        strdata.Append("pg_f=" & pg(2) & "|") 'partite giocate fuori casa'

                                        strdata.Append("vit_t=" & vit(0) & "|") 'vittorie'
                                        strdata.Append("vit_d=" & vit(1) & "|") 'vittorie dentro casa'
                                        strdata.Append("vit_f=" & vit(2) & "|") 'vittorie fuori casa'

                                        strdata.Append("par_t=" & par(0) & "|") 'pareggi'
                                        strdata.Append("par_d=" & par(1) & "|") 'pareggi dentro casa'
                                        strdata.Append("par_f=" & par(2) & "|") 'pareggi fuori casa'

                                        strdata.Append("per_t=" & per(0) & "|") 'sconfitte'
                                        strdata.Append("per_d=" & per(1) & "|") 'sconfitte dentro casa'
                                        strdata.Append("per_f=" & per(2) & "|") 'sconfitte fuori casa'

                                        strdata.Append("gf_t=" & gf(0) & "|") 'goal fatti'
                                        strdata.Append("gf_d=" & gf(1) & "|") 'goal fatti dentro casa'
                                        strdata.Append("gf_f=" & gf(2) & "|") 'goal fatti fuori casa'

                                        strdata.Append("gs_t=" & gs(0) & "|") 'goal subiti'
                                        strdata.Append("gs_d=" & gs(1) & "|") 'goal subiti dentro casa'
                                        strdata.Append("gs_f=" & gs(2)) 'goal subiti fuori casa'

                                        strdata.AppendLine()

                                    End If
                                End If
                            Next
                        End If

                    Next

                    IO.File.WriteAllText(filed, strdata.ToString)

                End If

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Ranking (" & Functions.Year & "):</span></br>" & strdata.ToString.Replace(System.Environment.NewLine, "</br>") & "</br>"
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Ranking (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

        End Function
    End Class

End Namespace
