
Namespace WebData

    Public Class PlayersRanking

        Public Class Player
            Public Property Nome As String
            Public Property Ruolo As String
            Public Property Team As String
            Public Property Rank As Integer
            Public Property Matched As Boolean = False
        End Class

        Public Class JsonRoot
            Public Property status As String
            Public Property data As List(Of Player)
        End Class

        Dim appSett As Torneo.PublicVariables

        Sub New(appSett As Torneo.PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function GetPlayersRankingData(ReturnData As Boolean, Giornata As Integer) As String

            Dim dirTemp As String = appSett.WebDataPath & "temp\"
            Dim dirData As String = appSett.WebDataPath & "data\"
            Dim fileTemp As String = dirTemp & "players-ranking-" & Giornata & ".json"
            Dim strdata As New System.Text.StringBuilder
            Dim playersq As New List(Of Torneo.Players.PlayerQuotesItem)

            Try

                'Players.Data.LoadPlayers(appSett, False)

                'Dim json As String = Functions.GetPage(appSett, "https://www.kickest.it/dev/stats/players/table/getStats?category=1&season=12&matchdays=" & Giornata & "&mode=1&language=it")
                ''Dim json As String = "re"

                'If json <> "" Then

                '    IO.File.WriteAllText(fileTemp, json)

                '    json = IO.File.ReadAllText(fileTemp, System.Text.Encoding.GetEncoding("ISO-8859-1"))

                '    Dim data = EstrarreGiocatori(json)

                '    'Dim root As JsonRoot = JsonConvert.DeserializeObject(Of JsonRoot)(IO.File.ReadAllText(fileTemp, System.Text.Encoding.GetEncoding("ISO-8859-1")))
                '    Dim pList As New List(Of Player)

                '    For Each d In data
                '        Dim p As New Player With {
                '            .Nome = d("player"),
                '            .Ruolo = d("position").Replace("Goalkeeper", "P").Replace("Defender", "D").Replace("Midfielder", "C").Replace("Attacker", "A"),
                '            .Team = Functions.CheckTeamName(d("team_name_long").ToUpper()),
                '            .Rank = CInt(CDbl(d("kickest_pts").Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) * 10)
                '        }
                '        pList.Add(p)
                '    Next

                '    Dim pData As New Dictionary(Of String, Double)

                '    For Each p In pList
                '        Dim playerm As WebData.Players.PlayerMatch = WebData.Players.Data.ResolveName(p.ruolo, p.Nome.ToUpper(), p.Team, False)
                '        p.Matched = playerm.Matched
                '        If playerm.Matched Then
                '            p.Nome = playerm.GetName()
                '            If pData.ContainsKey(p.Nome) = False Then
                '                pData.Add(p.Nome, p.Rank)
                '            Else
                '                p.Matched = False
                '            End If

                '        End If
                '        strdata.AppendLine($"{p.Nome} ({playerm.Matched}) = {p.Rank}")
                '    Next

                '    Dim uppdata As New Torneo.Players(appSett)
                '    uppdata.UpdatePlayersRanking(pData, Giornata)

                'End If

                'If ReturnData Then
                '    Return "</br><span style=color: red;font-size:bold;'>Players ranking (" & Giornata & "):</span></br>" & strdata.ToString().Replace(System.Environment.NewLine, "</br>") & "</br>"
                'Else
                '    Return ("</br><span style=color:red;font-size:bold;'>Players ranking (" & Giornata & "):</span><span style=color:blue;font-size:bold;'>Compleated!!</span></br>")
                'End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                Return ex.Message
            End Try

        End Function

        Function EstrarreGiocatori(json As String) As List(Of Dictionary(Of String, String))

            Dim lista As New List(Of Dictionary(Of String, String))()

            Dim pos As Integer = 0

            While True
                ' Trova l'inizio di un oggetto giocatore
                Dim startObj = json.IndexOf("{", pos)
                If startObj = -1 Then Exit While

                Dim endObj = json.IndexOf("}", startObj)
                If endObj = -1 Then Exit While

                Dim block As String = json.Substring(startObj, endObj - startObj)

                Dim dati As New Dictionary(Of String, String)

                dati("player") = EstrarreValore(block, """player"":")
                dati("position") = EstrarreValore(block, """position"":")
                dati("team_name_long") = EstrarreValore(block, """team_name_long"":")
                dati("kickest_pts") = EstrarreValore(block, """kickest_pts"":")

                ' Aggiungi solo se il blocco contiene un player valido
                If dati("player") <> "" Then
                    lista.Add(dati)
                End If

                pos = endObj + 1
            End While

            Return lista
        End Function


        Function EstrarreValore(block As String, key As String) As String
            Dim p = block.IndexOf(key)
            If p = -1 Then Return ""

            p += key.Length

            ' Salta spazi e virgolette
            While p < block.Length AndAlso (block(p) = " "c Or block(p) = ":"c Or block(p) = """"c)
                p += 1
            End While

            Dim startVal = p

            ' Legge fino a virgola o virgolette o fine oggetto
            While p < block.Length AndAlso block(p) <> ","c AndAlso block(p) <> """"c AndAlso block(p) <> "}"c
                p += 1
            End While

            Return block.Substring(startVal, p - startVal).Trim()
        End Function

    End Class
End Namespace
