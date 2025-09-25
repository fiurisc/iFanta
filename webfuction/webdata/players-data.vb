Imports System.Net
Imports System.IO
Imports System.Text

Namespace WebData

    Public Class PlayersData

        Shared Function GetPlayersData(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(Functions.Year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(Functions.Year) & "\data"
            Dim filed As String = dird & "\players-data.txt"
            Dim filep As String = dird & "\players-data-player.txt"
            Dim strdata As New System.Text.StringBuilder
            Dim strnameerr As New System.Text.StringBuilder
            Dim strplayer As New System.Text.StringBuilder

            Functions.Dirs = ServerPath

            Try

                Players.Data.LoadPlayers(ServerPath & "\web\" & CStr(Functions.Year) & "\data\players-quote.txt", False)

                Dim dicnatcode As Dictionary(Of String, String) = Functions.GetDicNatCodeList(ServerPath & "\web\code.txt")
                Dim sqlink As New Dictionary(Of String, String)
                Dim team As List(Of String) = GetTeamList(ServerPath)
                Dim plist As New List(Of String)
                Dim wpl As New Dictionary(Of String, WebData.Players.PlayerMatch)
                Dim npla As Integer = 1
                Dim nerr As Integer = 1

                For i As Integer = 0 To team.Count - 1
                    sqlink.Add(team(i), "https://sport.sky.it/calcio/squadre/" & team(i).ToLower() & "/rosa")
                Next

                For Each sq As String In sqlink.Keys

                    Dim html As String = Functions.GetPage(sqlink(sq), "POST", "")

                    If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
                    If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)

                    sq = Functions.CheckTeamName(sq)

                    If html <> "" Then

                        Dim filet As String = dirt & "\players-data-" & sq.ToLower & ".txt"
                        Dim dicname As New List(Of String)

                        IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding("ISO-8859-1"))

                        Dim line() As String = IO.File.ReadAllLines(filet)

                        For i As Integer = 0 To line.Length - 1

                            If line(i).ToLower.Contains("playerlist") Then
                                Dim players As String() = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=\[).*(?=\])").Value.Split("},")
                                For Each p As String In players
                                    Dim pdata As String() = p.Replace(",{", "").Replace("""", "").Split(",")

                                    If pdata.Length = 11 Then

                                        Dim role As String = pdata(2).Replace("role:", "")
                                        Dim nat As String = ""
                                        Dim natcode As String = Functions.GetNatCode(pdata(3).Replace("flag:", ""))
                                        Dim name1 As String = Functions.NormalizeText(pdata(6).Replace("name:", "").ToUpper() & " " & pdata(5).Replace("surname:", "").ToUpper()).Trim()
                                        Dim name2 As String = Functions.NormalizeText(pdata(9).Replace("fullname:", "").ToUpper()).Replace(".", ". ").Replace("  ", " ").Trim()

                                        If natcode = "SCT" Then natcode = "GBR"

                                        If dicnatcode.ContainsKey(natcode) Then nat = dicnatcode(natcode) Else nat = "?"

                                        If role = "Goalkeeper" Then
                                            role = "P"
                                        ElseIf role = "Defender" Then
                                            role = "D"
                                        ElseIf role = "Midfielder" Then
                                            role = "C"
                                        ElseIf role = "Forward" Then
                                            role = "A"
                                        Else
                                            role = ""
                                        End If

                                        If role <> "" Then
                                            Dim playerm As WebData.Players.PlayerMatch = WebData.Players.Data.ResolveName(role, name1, sq, wpl, True)
                                            If playerm.Matched = False AndAlso name1 <> name2 Then
                                                playerm = WebData.Players.Data.ResolveName(role, name2, sq, wpl, True, True)
                                                If playerm.Matched Then
                                                    wpl.Remove(name1)
                                                Else
                                                    wpl.Remove(name2)
                                                End If
                                            End If
                                            If playerm.Matched Then
                                                Dim newname As String = playerm.GetName()
                                                If dicname.Contains(newname) = False Then
                                                    strdata.AppendLine(role & "|" & newname & "|" & sq & "|" & nat & "|" & natcode)
                                                    strplayer.AppendLine(npla.ToString().PadRight(3, "x").Replace("x", "&nbsp;") & " - " & role & " - " & name1 & " -> " & playerm.MatchedPlayer.Role & " - " & newname & " - " & playerm.MatchedPlayer.Team & " - " & nat & " - " & natcode)
                                                    dicname.Add(newname)
                                                Else
                                                    strplayer.AppendLine(npla.ToString().PadRight(3, "x").Replace("x", "&nbsp;") & " - " & role & " - " & name1 & " -> " & playerm.MatchedPlayer.Role & " - " & newname & " - " & playerm.MatchedPlayer.Team & "&nbsp;&nbsp;<span style=color:red;font-size:bold;'>[Already exist]</span>")
                                                End If
                                            Else
                                                strnameerr.AppendLine(nerr.ToString().PadRight(3, "x").Replace("x", "&nbsp;") & " - " & role & " - " & name1 & " - " & sq)
                                                nerr += 1
                                            End If

                                            npla += 1
                                        End If

                                    End If
                                Next
                            End If
                        Next
                    End If
                Next

                IO.File.WriteAllText(filed, strdata.ToString, Encoding.UTF8)

                If Functions.makefileplayer Then IO.File.WriteAllText(filep, Functions.GetDataPlayerMatchedData(wpl, True), System.Text.Encoding.UTF8)

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Players data (" & Functions.Year & "):</span></br>" & strplayer.ToString.Replace(System.Environment.NewLine, "</br>") & "</br><span style='color:red;font-size:bold;'>Name resolution error:</span></br>" & strnameerr.ToString.Replace(System.Environment.NewLine, "</br>") & "</br><span style='color:red;font-size:bold;'>Details:</span></br>" & Functions.GetDataPlayerMatchedData(wpl, False).Replace(System.Environment.NewLine, "</br>")
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Players data (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

        End Function

        Function GetPlayersDataOld(ServerPath As String, ReturnData As Boolean) As String

            Dim dirt As String = ServerPath & "\web\" & CStr(Functions.Year) & "\temp"
            Dim dird As String = ServerPath & "\web\" & CStr(Functions.Year) & "\data"
            Dim filed As String = dird & "\players-data.txt"
            Dim filep As String = dird & "\players-data-player.txt"
            Dim strdata As New System.Text.StringBuilder
            Dim strnameerr As New System.Text.StringBuilder
            Dim strplayer As New System.Text.StringBuilder

            Functions.Dirs = ServerPath

            Try

                WebData.Players.Data.LoadPlayers(ServerPath & "\web\" & CStr(Functions.Year) & "\data\players-quote.txt", False)

                Dim dicnatcode As Dictionary(Of String, String) = Functions.GetDicNatCodeList(ServerPath & "\web\code.txt")
                Dim sqlink As New Dictionary(Of String, String)
                Dim team As List(Of String) = GetTeamList(ServerPath)
                Dim plist As New List(Of String)
                Dim wpl As New Dictionary(Of String, WebData.Players.PlayerMatch)

                For i As Integer = 0 To team.Count - 1
                    sqlink.Add(team(i), "http://www.legaseriea.it/it/serie-a/squadre/" & team(i) & "/squadra/" & Functions.Year & "-" & CStr(Functions.Year - 1999))
                Next

                For Each sq As String In sqlink.Keys

                    Dim html As String = Functions.GetPage(sqlink(sq), "POST", "")

                    If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
                    If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)

                    sq = Functions.CheckTeamName(sq)

                    If html <> "" Then

                        Dim filet As String = dirt & "\players-data-" & sq.ToLower & ".txt"

                        IO.File.WriteAllText(filet, html, System.Text.Encoding.GetEncoding("ISO-8859-1"))

                        Dim role As String = ""
                        Dim line() As String = IO.File.ReadAllLines(filet)
                        Dim dicname As New List(Of String)

                        For i As Integer = 0 To line.Length - 1

                            If line(i).ToLower.Contains("trasferiti") Then
                                Exit For
                            End If

                            If line(i).Contains("<a href=""/it/giocatori/") Then

                                If line(i + 5).ToLower.Contains("portiere") Then
                                    role = "P"
                                ElseIf line(i + 5).ToLower.Contains("difensore") Then
                                    role = "D"
                                ElseIf line(i + 5).ToLower.Contains("centrocampista") Then
                                    role = "C"
                                ElseIf line(i + 5).ToLower.Contains("attaccante") Then
                                    role = "A"
                                Else
                                    role = ""
                                End If

                                If role <> "" Then

                                    Dim nome As String = System.Text.RegularExpressions.Regex.Match(line(i + 1), "(?<=span>).*(?=\</span)").Value.ToUpper.Replace("-", " ").Trim.Replace("'", "’")
                                    Dim nat As String = ""
                                    Dim natcode As String = Functions.GetNatCode(System.Text.RegularExpressions.Regex.Match(line(i + 7), "(?<=alt\=\"")\w+(?=\"")").Value.ToUpper)

                                    If dicnatcode.ContainsKey(natcode) Then nat = dicnatcode(natcode) Else nat = "?"

                                    nome = Functions.NormalizeText(nome.Replace("'", ""))

                                    If nome <> "" Then

                                        If nome.Contains("CAICEDO") Then
                                            nome = nome
                                        End If

                                        If wpl.ContainsKey(nome) = False Then
                                            nome = Players.Data.ResolveName("", nome, sq, wpl, True).GetName()
                                            If dicname.Contains(nome) = False Then
                                                strdata.AppendLine(role & "|" & nome & "|" & sq & "|" & nat & "|" & natcode)
                                                dicname.Add(nome)
                                            End If
                                        End If
                                    End If
                                End If

                            End If

                        Next
                    End If

                Next

                IO.File.WriteAllText(filed, strdata.ToString, Encoding.UTF8)
                If Functions.makefileplayer Then Functions.WriteDataPlayerMatch(wpl, filep)

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Players data (" & Functions.Year & "):</span></br>" & strdata.ToString.Replace(System.Environment.NewLine, "</br>") & "</br><span style='color:red;'>Name resolution error:</br>" & strnameerr.ToString.Replace(System.Environment.NewLine, "</br>") & "</span>"
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Players data (" & Functions.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

        End Function

        Shared Function GetTeamList(ServerPath As String) As List(Of String)
            Return Players.Data.players.Keys.ToList()
        End Function

    End Class

End Namespace
