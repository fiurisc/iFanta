Imports System.IO

Namespace WebData

    Public Class PlayersData

        Dim appSett As Torneo.PublicVariables

        Sub New(appSett As Torneo.PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function GetDataFileName() As String
            Return appSett.WebDataPath & "data\players-data.json"
        End Function

        Public Function GetPlayersData(ReturnData As Boolean) As String


            Dim dirTemp As String = appSett.WebDataPath & "temp\"
            Dim dirData As String = appSett.WebDataPath & "data\"
            Dim fileJson As String = GetDataFileName()
            Dim fileLog As String = dirData & Path.GetFileNameWithoutExtension(GetDataFileName) & ".log"
            Dim strdata As String = ""
            Dim playersd As New List(Of Torneo.Players.PlayerDataItem)
            Dim strnameerr As New System.Text.StringBuilder
            Dim strplayer As New System.Text.StringBuilder

            Try

                Players.Data.LoadPlayers(appSett, False)

                Dim dicNatCode As Dictionary(Of String, String) = Functions.GetDicNatCodeList(appSett.RootTorneiPath & "\code.txt")
                Dim sqlink As New Dictionary(Of String, String)
                Dim team As List(Of String) = GetTeamList()
                Dim wpl As New Dictionary(Of String, WebData.Players.PlayerMatch)
                Dim npla As Integer = 1
                Dim nerr As Integer = 1

                For i As Integer = 0 To team.Count - 1
                    sqlink.Add(team(i), "https://sport.sky.it/calcio/squadre/" & team(i).ToLower() & "/rosa")
                Next

                For Each sq As String In sqlink.Keys

                    Dim html As String = Functions.GetPage(appSett, sqlink(sq))

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

                                    If p.ToUpper().Contains("PROVEDEL") Then
                                        Dim a As Integer = 0
                                    End If

                                    If pdata.Length = 11 Then

                                        Dim role As String = ""
                                        Dim nat As String = ""
                                        Dim NatCode As String = ""
                                        Dim birthdays As String = ""
                                        Dim anni As Integer = 0

                                        Dim peso As String = ""
                                        Dim altezza As String = ""
                                        Dim name As String = ""
                                        Dim surname As String = ""

                                        Dim name1 As String = ""
                                        Dim name2 As String = ""

                                        For Each pdato In pdata
                                            If pdato.StartsWith("role:") Then role = pdato.Replace("role:", "")
                                            If pdato.StartsWith("flag:") Then NatCode = pdato.Replace("flag:", "")
                                            If pdato.StartsWith("birthdate:") Then birthdays = System.Text.RegularExpressions.Regex.Match(pdato, "\d{1,}-\d{1,}-\d{1,}").Value
                                            If pdato.StartsWith("name:") Then name = Functions.NormalizeText(pdato.Replace("name:", "").ToUpper()).Trim()
                                            If pdato.StartsWith("surname:") Then surname = Functions.NormalizeText(pdato.Replace("surname:", "").ToUpper()).Trim()
                                            If pdato.StartsWith("weight:") Then peso = pdato.Replace("weight:", "")
                                            If pdato.StartsWith("height:") Then altezza = pdato.Replace("height:", "")
                                        Next

                                        If name.Contains("MILINKOVIC") OrElse surname.Contains("MILINKOVIC") Then
                                            name = "MILINKOVIC-SAVIC"
                                        End If

                                        If name.Contains("ORSO") Then
                                            name = name
                                        End If

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

                                        If role <> "" AndAlso name <> "" AndAlso surname <> "" Then

                                            name1 = surname & " " & name.Substring(0, 1)
                                            name2 = name & " " & surname.Substring(0, 1)

                                            If birthdays <> "" Then
                                                Dim birthday As Date = CDate(birthdays)
                                                anni = Date.Now.Year - birthday.Year
                                                If Date.Now.Date < birthday.AddYears(anni) Then
                                                    anni -= 1
                                                End If
                                            End If

                                            If NatCode = "SCT" Then NatCode = "GBR"
                                            If NatCode = "CIV" Then NatCode = "CIV"
                                            If NatCode = "XKX" Then NatCode = "KOS"

                                            If dicNatCode.ContainsKey(NatCode) Then nat = dicNatCode(NatCode) Else nat = ""

                                            nat = Functions.NormalizeText(nat)

                                            Dim playerm As WebData.Players.PlayerMatch = WebData.Players.Data.ResolveName(role, name1, sq, wpl, True, False)
                                            If playerm.Matched = False Then playerm = WebData.Players.Data.ResolveName(role, name2, sq, wpl, True, False)
                                            If playerm.Matched = False Then playerm = WebData.Players.Data.ResolveName("", name1, sq, wpl, True, False)

                                            If wpl.ContainsKey(name1) = False Then wpl.Add(name1, playerm)

                                            If playerm.Matched Then
                                                Dim newname As String = playerm.GetName()
                                                If dicname.Contains(newname) = False Then
                                                    playersd.Add(New Torneo.Players.PlayerDataItem(role, newname, sq, nat, NatCode, anni, birthdays, altezza, peso))
                                                    strplayer.AppendLine(npla.ToString().PadRight(3, CChar("x")).Replace("x", "&nbsp;") & " - " & role & " - " & name1 & " -> " & playerm.MatchedPlayer.Role & " - " & newname & " - " & playerm.MatchedPlayer.Team & " - " & nat & " - " & NatCode & " - " & birthdays)
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

                Dim uppdata As New Torneo.Players(appSett)
                uppdata.UpdatePlayersData(playersd)

                strdata = Functions.SerializzaOggetto(playersd, False)

                IO.File.WriteAllText(fileJson, strdata)

                If Functions.makefileplayer Then IO.File.WriteAllText(fileLog, Functions.GetDataPlayerMatchedData(appSett, wpl, True), System.Text.Encoding.UTF8)

                If ReturnData Then
                    Return "</br><span style=color:red;font-size:bold;'>Players data (" & appSett.Year & "):</span></br>" & strplayer.ToString.Replace(System.Environment.NewLine, "</br>") & "</br><span style='color:red;font-size:bold;'>Name resolution error:</span></br>" & strnameerr.ToString.Replace(System.Environment.NewLine, "</br>") & "</br><span style='color:red;font-size:bold;'>Details:</span></br>" & Functions.GetDataPlayerMatchedData(appSett, wpl, False).Replace(System.Environment.NewLine, "</br>")
                Else
                    Return ("</br><span style=color:red;font-size:bold;'>Players data (" & appSett.Year & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                End If

            Catch ex As Exception
                Functions.WriteLog(appSett, Functions.eMessageType.Info, ex.Message)
                Return ex.Message
            End Try

        End Function

        Private Function GetTeamList() As List(Of String)
            Return Players.Data.players.Keys.ToList()
        End Function

    End Class

End Namespace
