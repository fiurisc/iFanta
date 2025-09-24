Imports System.Text

Partial Class WebData
    Partial Class ProbableFormations

        Function GetGazzetta(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(year) & "\data"
            Dim filet As String = dirt & "\pform-gazzetta.txt"
            Dim filed As String = dird & "\pform-gazzetta.txt"
            Dim filep As String = dird & "\pform-gazzetta-player.txt"
            Dim filel As String = dird & "\pform-gazzetta-log.txt"
            Dim site As String = "Gazzetta"
            Dim currgg As Integer = -1
            Dim sr As New IO.StreamWriter(filel)
            Dim rmsg As String = ""

            dirs = ServerPath

            Try

                Call LoadWebPlayers(ServerPath & "\web\" & CStr(year) & "\data\players-quote.txt")
                Call LoadWebMatchs(ServerPath & "\web\" & CStr(year) & "\data\matchs-data.txt")

                Dim html As String = GetPage("http://www.gazzetta.it/Calcio/prob_form/", "POST", "", "UTF-8")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding("UTF-8"))

                    Dim line() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding("UTF-8"))
                    Dim start As Boolean = False
                    Dim wpd As New Dictionary(Of String, wPlayer)
                    Dim wpl As New Dictionary(Of String, WebData.PlayerMatch)
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""

                    sr.WriteLine("Year -> " & year)
                    sr.WriteLine("Calendario match:")
                    sr.WriteLine("---------------------------")
                    For Each t As String In webmatchs.Keys
                        sr.WriteLine(webmatchs(t) & " -> " & t)
                    Next
                    sr.WriteLine("")

                    sr.WriteLine("linee file html => " & CStr(line.Length))

                    For i As Integer = 0 To line.Length - 1
                        If line(i) <> "" Then

                            If line(i).Contains("<p class=""lastUpdate"">") Then
                                sq.Clear()
                                sqid = -1
                            ElseIf line(i).Contains("class=""details-team__name""") Then
                                'Aggiungo la squadra alla lista di quelle che disputano il match'
                                sq.Add(System.Text.RegularExpressions.Regex.Match(line(i + 2).Trim(), "(?<=\>)\w+(?=\<\/a)").Value.ToUpper)
                            ElseIf line(i).Contains("class=""match-details__info"">") Then

                                'Cerco di determinare la giornata di riferiemnto'
                                If sq.Count = 2 AndAlso currgg = -1 Then

                                    Dim match As String = sq(0) & "-" & sq(1)

                                    sr.WriteLine("match trovato -> " & match)

                                    For Each key As String In webmatchs.Keys
                                        If key = match Then
                                            currgg = webmatchs(key)
                                            sr.WriteLine("giornata associata -> " & CStr(currgg))
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
                                    name = ResolveName("", name, sq(sqid), wpl, False).GetName()
                                    Call AddInfo(name, sq(sqid), site, "Titolare", "", 100, wpd)
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
                                    Dim list() As String = value.Trim().Split(",")
                                    For Each nome In list
                                        Try
                                            Dim info As String = ""
                                            nome = nome.Trim()
                                            If RegularExpressions.Regex.Match(nome, "^\d+").Success Then
                                                nome = nome.Substring(nome.IndexOf(" "))
                                            End If
                                            If RegularExpressions.Regex.Match(nome, "\(").Success Then
                                                info = nome.Substring(nome.IndexOf("(") + 1).Replace(")", "").Trim()
                                                nome = nome.Substring(0, nome.IndexOf("("))
                                            End If
                                            nome = nome.Trim().ToUpper()
                                            nome = ResolveName("", nome, sq(sqid), wpl, False).GetName()
                                            Call AddInfo(nome, sq(sqid), site, pstate, info, 0, wpd)
                                        Catch ex As Exception

                                        End Try

                                    Next

                                End If

                            End If


                            'If start Then


                            '    If line(i).Contains("<div class="" probabili-formazioni hide-for-small""") Then
                            '        start = False
                            '    ElseIf line(i).Contains("<p class="" lastUpdate"">") Then
                            '        sq.Clear()
                            '        sqid = -1
                            '    ElseIf line(i).Contains("class=""details-team__name"">") Then
                            '        'Aggiungo la squadra alla lista di quelle che disputano il match'
                            '        sq.Add(System.Text.RegularExpressions.Regex.Match(line(i + 2), "(?<=\>)\w+(?=\<\/a\>)").Value.ToUpper)

                            '    ElseIf line(i).Contains("class=""match-details__info"">") Then

                            '        'Cerco di determinare la giornata di riferiemnto'
                            '        If sq.Count = 2 AndAlso currgg = -1 Then

                            '            Dim match As String = sq(0) & "-" & sq(1)

                            '            sr.WriteLine("match trovato -> " & match)

                            '            For Each key As String In webmatchs.Keys
                            '                If key = match Then
                            '                    currgg = webmatchs(key)
                            '                    sr.WriteLine("giornata associata -> " & CStr(currgg))
                            '                    Exit For
                            '                End If
                            '            Next
                            '        End If


                            '    ElseIf System.Text.RegularExpressions.Regex.Match(line(i), "\<strong\>(Panchina|Ballottaggio|Squalificati|Indisponibili):\<\/strong\>").Value <> "" Then

                            '        Dim str As String = ""
                            '        Dim info As String = ""

                            '        For w As Integer = 0 To 5
                            '            str = str & line(i + w)
                            '            If line(i + w).Contains("</p>") Then Exit For
                            '        Next
                            '        str = str.Replace("</p>", ",")

                            '        If str.Contains("Panchina") Then
                            '            sqid = sqid + 1
                            '            If sqid <sq.Count Then team= sq(sqid) Else team=""
                            '            pstate = "Panchina"
                                '        ElseIf str.Contains("Ballottaggio") Then
                                '            pstate = "Ballottaggio"
                                '            str = str.Replace("&nbsp;", "|").Replace(" - ", "|")
                                '        ElseIf str.Contains("Squalificati") Then
                                '            pstate = "Squalificato"
                                '        ElseIf str.Contains("Indisponibili") Then
                                '            pstate = "Infortunato"
                                '        End If

                                '        str = System.Text.RegularExpressions.Regex.Match(str, "(?<=\<\/strong\>).*").Value

                                '        If str.Contains("nessuno") = False Then
                                '            Dim s() As String = str.Split(CChar(","))
                                '            For k As Integer = 0 To s.Length - 1
                                '                s(k) = s(k).Trim
                                '                If s(k).Trim <> "" Then
                                '                    If pstate = "Ballottaggio" Then
                                '                        'Gestione ballottaggi'
                                '                        If s(k).StartsWith("|") Then s(k) = s(k).Substring(1)
                                '                        Dim d() As String = s(k).Trim.Split(CChar("|"))
                                '                        If d.Length = 4 Then
                                '                            'primo player'
                                '                            If d(0).Trim <> "" Then
                                '                                name = ResolveName("", d(0).Trim.ToUpper(), team, wpl, False).GetName()
                                '                                Call AddInfo(name, team, site, pstate, "In ballottagio con " & d(1) & " [" & d(2) & "]", CInt(d(2).Replace("%", "")), wpd)
                                '                            End If
                                '                            'secondo player'
                                '                            If d(1).Trim <> "" Then
                                '                                name = ResolveName("", d(1).Trim.ToUpper(), team, wpl, False).GetName()
                                '                                If d(3) = "" Then d(3) = "-1"
                                '                                Call AddInfo(name, team, site, pstate, "In ballottagio con " & d(0) & " [" & d(3) & "]", CInt(d(3).Replace("%", "")), wpd)
                                '                            End If
                                '                        End If
                                '                    Else
                                '                        'Gestione altri Panchina, Squalificati e Indisponibili'
                                '                        If s(k).Contains("ensp;") Then s(k) = System.Text.RegularExpressions.Regex.Match(s(k), "(?<=\;).*").Value
                                '                        If s(k).Contains("(") Then
                                '                            info = NormalizeText(System.Text.RegularExpressions.Regex.Match(s(k).ToUpper(), "(?<=\().*(?=\))").Value)
                                '                            If info <> "" Then s(k) = s(k).Replace(info, "").Replace("(", "").Replace(")", "").Trim
                                '                        End If
                                '                        If s(k).Trim <> "" Then
                                '                            name = ResolveName("", s(k).Trim().ToUpper(), team, wpl, False).GetName()
                                '                            Call AddInfo(name, team, site, pstate, info, 100, wpd)
                                '                        End If
                                '                    End If

                                '                End If
                                '            Next
                                '        End If

                                '    End If

                                'Else
                                '    If line(i).Contains("<p><strong>Altri:</strong>") Then
                                '        start = False
                                '    ElseIf line(i).Contains("<div class=""match fxc-wrap"">") Then
                                '        sq.Clear()
                                '        start = True
                                '    End If
                                'End If

                            End If
                    Next

                    If currgg <> -1 Then
                        Dim out As String = WriteData(currgg, wpd, filed)
                        If makefileplayer Then Call WriteDataPlayerMatch(wpl, filep)
                        rmsg = out.Replace(System.Environment.NewLine, "</br>")
                    End If

                End If

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                rmsg = ex.Message
            End Try

            sr.Close()

            Return rmsg

        End Function
    End Class
End Class