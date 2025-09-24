Partial Class WebData
    Partial Class ProbableFormations

        Private year As Integer = -1

        Sub New(YearLega As Integer)
            year = YearLega
        End Sub

        Public Sub ResetCacheData()
            webplayers.Clear()
            webmatchs.Clear()
        End Sub

        Function GetCds(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(year) & "\data"
            Dim filet As String = dirt & "\pform-cds.txt"
            Dim filed As String = dird & "\pform-cds.txt"
            Dim filep As String = dird & "\pform-cds-player.txt"
            Dim filel As String = dird & "\pform-cds-log.txt"

            Dim site As String = "Corriere"
            Dim enc As String = "utf-8"
            Dim currgg As Integer = -1
            Dim sr As New IO.StreamWriter(filel)
            Dim rmsg As String = ""

            dirs = ServerPath

            Try

                Call LoadWebPlayers(ServerPath & "\web\" & CStr(year) & "\data\players-quote.txt")
                Call LoadWebMatchs(ServerPath & "\web\" & CStr(year) & "\data\matchs-data.txt")

                sr.WriteLine("Year -> " & year)
                sr.WriteLine("Calendario match:")
                sr.WriteLine("---------------------------")
                For Each t As String In webmatchs.Keys
                    sr.WriteLine(webmatchs(t) & " -> " & t)
                Next
                sr.WriteLine("")

                Dim wpd As New Dictionary(Of String, wPlayer)
                Dim wpl As New Dictionary(Of String, WebData.PlayerMatch)
                Dim linkp As New List(Of String)

                'Determino i link delle varie partite'
                Dim html As String = GetPage("https://www.corrieredellosport.it/calcio/serie-a/probabili-formazioni", "POST", "")

                If html <> "" Then

                    IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding(enc))
                    Dim start As Boolean = False
                    
                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)
                       
                        If line <> "" Then
                            If line.Contains("<ul class=""probabili-formazioni-list"">") Then
                                start = True
                            Else
                                If start AndAlso line.Contains("probabili-formazioni") Then

                                    linkp.Add(System.Text.RegularExpressions.Regex.Match(line, "(?<="").*(?=\"")").Value)

                                    If currgg = -1 Then

                                        Dim match As String = System.Text.RegularExpressions.Regex.Match(line, "[\w\s\-]{1,}_-_[\w\s\-]{1,}").Value.ToUpper.Replace("_-_", "-")

                                        sr.WriteLine("match trovato -> " & match)

                                        For Each key As String In webmatchs.Keys
                                            If key = match Then
                                                currgg = webmatchs(key)
                                                sr.WriteLine("giornata associata -> " & CStr(currgg))
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If

                If linkp.Count > 0 Then

                    'Analizzo le proprobili formazioni dei singoli match'
                    For i As Integer = 0 To linkp.Count - 1
                        If linkp(i) <> "" Then
                            Call GetCdsSingleMatch(linkp(i), i, site, wpd, wpl)
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

        Private Sub GetCdsSingleMatch(link As String, indmatch As Integer, site As String, wpd As Dictionary(Of String, wPlayer), wpl As Dictionary(Of String, WebData.PlayerMatch))

            Try

                Dim dirt As String = dirs & "\web\" & CStr(year) & "\temp"
                Dim filet As String = dirt & "\pform-cds-ind-" & indmatch & ".txt"
                Dim enc As String = "utf-8"
            
                'Determino i link delle varie partite'
                Dim html As String = GetPage(link, "POST", "", enc)

                If html <> "" Then

                    IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding(enc))

                    Dim lines() As String = IO.File.ReadAllLines(filet, System.Text.Encoding.GetEncoding(enc))
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"

                    sq.Add("")
                    sq.Add("")

                    For i As Integer = 0 To lines.Length - 1

                        Dim line As String = lines(i)

                        If line <> "" Then

                            Dim name As String = ""
                            Dim perc As Integer = -1
                            Dim info As String = ""

                            If line.Contains("<div class=""team home"">") Then
                                sq(0) = (CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i + 1), "(?<=\>).*(?=\<\/)").Value.ToUpper))
                            End If

                            If line.Contains("<div class=""team away"">") Then
                                sq(1) = (CheckTeamName(System.Text.RegularExpressions.Regex.Match(lines(i + 1), "(?<=\>).*(?=\<\/)").Value.ToUpper))
                            End If

                            If line.Contains(""">Titolari</th>") Then
                                pstate = "Titolare"
                            ElseIf line.Contains(""">Riserve</th>") Then
                                pstate = "Panchina"
                            ElseIf line.Contains(""">Indisponibili</th>") Then
                                pstate = "Infortunato"
                            ElseIf line.Contains(""">Squalificati</th>") Then
                                pstate = "Squalificato"
                            End If

                            If line.Contains("<td class=""a-right"">") Then

                                'Giorcatore squadra in casa'
                                name = System.Text.RegularExpressions.Regex.Match(line, "(?<=\>).*(?=\<\/)").Value.ToUpper.Replace("'", "’")
                                If name <> "" Then
                                    name = ResolveName("", name, sq(0), wpl, False).GetName()
                                    Call AddInfo(name, sq(0), site, pstate, info, perc, wpd)
                                End If
                                'Giorcatore squadra fuori casa'
                                name = System.Text.RegularExpressions.Regex.Match(lines(i + 3), "(?<=\>).*(?=\<\/)").Value.ToUpper.Replace("'", "’")
                                If name <> "" Then
                                    name = ResolveName("", name, sq(1), wpl, False).GetName()
                                    Call AddInfo(name, sq(1), site, pstate, info, perc, wpd)
                                End If

                            ElseIf line.Contains("<th colspan=""2"">") Then

                                'Giorcatore squadra in casa'
                                Dim s1() As String = System.Text.RegularExpressions.Regex.Match(lines(i - 1), "(?<=\>).*(?=\<\/)").Value.Split(CChar(","))
                                For k As Integer = 0 To s1.Length - 1
                                    name = s1(k).Trim.ToUpper
                                    If name <> "" Then
                                        name = ResolveName("", name, sq(0), wpl, False).GetName()
                                        Call AddInfo(name, sq(0), site, pstate, "", perc, wpd)
                                    End If
                                Next

                                'Giorcatore squadra fuori casa'
                                Dim s2() As String = System.Text.RegularExpressions.Regex.Match(lines(i + 1), "(?<=\>).*(?=\<\/)").Value.Split(CChar(","))
                                For k As Integer = 0 To s2.Length - 1
                                    name = s2(k).Trim.ToUpper
                                    If name <> "" Then
                                        name = ResolveName("", s2(k).Trim.ToUpper, sq(0), wpl, False).GetName()
                                        Call AddInfo(name, sq(1), site, pstate, "", perc, wpd)
                                    End If
                                Next

                            End If
                        End If
                    Next

                End If

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub
    End Class
End Class