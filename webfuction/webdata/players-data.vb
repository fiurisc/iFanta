Imports System.Net
Imports System.IO
Imports System.Text

Namespace WebData

    Public Class PlayersData

        Shared Function GetDataFileName() As String
            Return Functions.DataPath & "data\players-data.json"
        End Function

        Shared Function GetPlayersData(ReturnData As Boolean) As String

            Dim dirTemp As String = Functions.DataPath & "temp\"
            Dim dirData As String = Functions.DataPath & "data\"
            Dim fileJson As String = GetDataFileName()
            Dim fileLog As String = dirData & Path.GetFileNameWithoutExtension(GetDataFileName) & ".log"
            Dim strdata As String = ""
            Dim playersd As New List(Of Torneo.Players.PlayerDataItem)

            'Dim dirt As String = Functions.DataPath & "\temp"
            'Dim dird As String = Functions.DataPath & "\data"
            'Dim filed As String = dird & "\players-data.json"
            'Dim strdata As New System.Text.StringBuilder
            Dim strnameerr As New System.Text.StringBuilder
            Dim strplayer As New System.Text.StringBuilder

            Try

                Players.Data.LoadPlayers(False)

                Dim dicnatcode As Dictionary(Of String, String) = Functions.GetDicNatCodeList(Functions.DataPath & "\code.txt")
                Dim sqlink As New Dictionary(Of String, String)
                Dim team As List(Of String) = GetTeamList()
                Dim plist As New List(Of String)
                Dim wpl As New Dictionary(Of String, WebData.Players.PlayerMatch)
                Dim npla As Integer = 1
                Dim nerr As Integer = 1

                For i As Integer = 0 To team.Count - 1
                    sqlink.Add(team(i), "https://sport.sky.it/calcio/squadre/" & team(i).ToLower() & "/rosa")
                Next

                For Each sq As String In sqlink.Keys

                    Dim html As String = Functions.GetPage(sqlink(sq))

                    If IO.Directory.Exists(dirTemp) = False Then IO.Directory.CreateDirectory(dirTemp)
                    If IO.Directory.Exists(dirData) = False Then IO.Directory.CreateDirectory(dirData)

                    sq = Functions.CheckTeamName(sq)

                    If html <> "" Then

                        Dim fileTemp As String = dirTemp & Path.GetFileNameWithoutExtension(GetDataFileName()) & "-" & sq.ToLower & ".txt"
                        Dim dicname As New List(Of String)

                        IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.GetEncoding("ISO-8859-1"))

                        Dim line() As String = IO.File.ReadAllLines(fileTemp)

                        For i As Integer = 0 To line.Length - 1

                            If line(i).ToLower.Contains("playerlist") Then
                                Dim players As String() = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=\[).*(?=\])").Value.Split(New String() {"},"}, StringSplitOptions.None)
                                For Each p As String In players
                                    Dim pdata As String() = p.Replace(",{", "").Replace("""", "").Split(",".ToCharArray())

                                    If pdata.Length = 11 Then

                                        Dim role As String = pdata(2).Replace("role:", "")
                                        Dim nat As String = ""
                                        Dim natcode As String = Functions.GetNatCode(pdata(3).Replace("flag:", ""))
                                        Dim name1 As String = Functions.NormalizeText(pdata(6).Replace("name:", "").ToUpper() & " " & pdata(5).Replace("surname:", "").ToUpper()).Trim()
                                        Dim name2 As String = Functions.NormalizeText(pdata(9).Replace("fullname:", "").ToUpper()).Replace(".", ". ").Replace("  ", " ").Trim()

                                        If natcode = "SCT" Then natcode = "GBR"

                                        If dicnatcode.ContainsKey(natcode) Then nat = dicnatcode(natcode) Else nat = ""

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
                                                    playersd.Add(New Torneo.Players.PlayerDataItem(role, newname, sq, nat, natcode))
                                                    strplayer.AppendLine(npla.ToString().PadRight(3, CChar("x")).Replace("x", "&nbsp;") & " - " & role & " - " & name1 & " -> " & playerm.MatchedPlayer.Role & " - " & newname & " - " & playerm.MatchedPlayer.Team & " - " & nat & " - " & natcode)
                                                    dicname.Add(newname)
                                                Else
                                                    strplayer.AppendLine(npla.ToString().PadRight(3, CChar("x")).Replace("x", "&nbsp;") & " - " & role & " - " & name1 & " -> " & playerm.MatchedPlayer.Role & " - " & newname & " - " & playerm.MatchedPlayer.Team & "&nbsp;&nbsp;<span style=color:red;font-size:bold;'>[Already exist]</span>")
                                                End If
                                            Else
                                                strnameerr.AppendLine(nerr.ToString().PadRight(3, CChar("x")).Replace("x", "&nbsp;") & " - " & role & " - " & name1 & " - " & sq)
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

                If Torneo.General.dataFromDatabase Then
                    Torneo.Players.UpdatePlayersData(playersd)
                End If

                strdata = Functions.SerializzaOggetto(playersd, False)

                IO.File.WriteAllText(fileJson, strdata)

                If Functions.makefileplayer Then IO.File.WriteAllText(fileLog, Functions.GetDataPlayerMatchedData(wpl, True), System.Text.Encoding.UTF8)

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

        Shared Function GetTeamList() As List(Of String)
            Return Players.Data.players.Keys.ToList()
        End Function

    End Class

End Namespace
