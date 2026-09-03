Namespace WebData
    Namespace Players

        Public Class Data

            Public Shared players As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
            'squadra/ruolo/key/nome
            Public Shared keyplayers As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, String)))

            Public Shared Sub ResetCacheData()
                players.Clear()
            End Sub

            Public Shared Function GetPlayers() As List(Of String)

                Dim plist As New List(Of String)

                For Each sq As String In players.Keys
                    For Each r As String In players(sq).Keys
                        For Each p As String In players(sq)(r)
                            If plist.Contains(p) = False Then
                                plist.Add(p)
                            End If
                        Next
                    Next
                Next

                Return plist

            End Function

            Public Shared Sub LoadPlayers(appSett As Torneo.PublicVariables, forceRelod As Boolean)

                Dim pquotes As New PlayersQuotes(appSett)
                Dim fdata As String = pquotes.GetDataFileName()

                If forceRelod Then players.Clear()

                If players.Count = 0 Then

                    If IO.File.Exists(fdata) Then

                        Dim playersq As List(Of Torneo.Players.PlayerQuotesItem) = Functions.DeserializeJson(Of List(Of Torneo.Players.PlayerQuotesItem))(System.IO.File.ReadAllText(fdata))

                        keyplayers.Clear()

                        For Each p As Torneo.Players.PlayerQuotesItem In playersq

                            If players.ContainsKey(p.Squadra) = False Then players.Add(p.Squadra, New Dictionary(Of String, List(Of String)))
                            If players(p.Squadra).ContainsKey(p.Ruolo) = False Then players(p.Squadra).Add(p.Ruolo, New List(Of String))
                            players(p.Squadra)(p.Ruolo).Add(p.Nome)

                            If keyplayers.ContainsKey(p.Squadra) = False Then keyplayers.Add(p.Squadra, New Dictionary(Of String, Dictionary(Of String, String)))
                            If keyplayers(p.Squadra).ContainsKey(p.Ruolo) = False Then keyplayers(p.Squadra).Add(p.Ruolo, New Dictionary(Of String, String))

                            Dim keylist As New List(Of String) From {p.Nome}

                            For k As Integer = 0 To keylist.Count - 1
                                If keyplayers(p.Squadra)(p.Ruolo).ContainsKey(keylist(k)) = False Then
                                    keyplayers(p.Squadra)(p.Ruolo).Add(keylist(k), p.Nome)
                                End If
                            Next
                        Next
                    End If
                End If

            End Sub

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, FindAllTeam As Boolean) As Players.PlayerMatch
                Return ResolveName(Role, Name, Team, Nothing, FindAllTeam)
            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, Players.PlayerMatch), FindAllTeam As Boolean) As Players.PlayerMatch
                Return ResolveName(Role, Name, Team, wp, FindAllTeam, True)
            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, Players.PlayerMatch), FindAllTeam As Boolean, AddPlayerToList As Boolean) As Players.PlayerMatch

                If Name.Contains("TRAORA") Then
                    Name = Name
                End If

                Name = Name.ToUpper().Trim()

                Name = Name.Replace("MILINKOVIC-SAVIC", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC V.", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC S.", "MILINKOVIC SAVIC").Replace("DEL PRATO", "DELPRATO").Replace("DEL PRATO", "DELPRATO")
                Name = Name.Replace("P.ESPOSITO", "ESPOSITO F.P.")
                Name = Name.Replace("ROBERTO S.", "SERGI ROBERTO.").Replace("GABRIEL T.", "TIAGO GABRIEL")
                Name = Name.Replace("CARLOS D.", "DIEGO CARLOS").Replace("ANGUISSA A.", "ZAMBO ANGUISSA")
                Name = Name.Replace("JESUS J.", "JUAN JESUS").Replace("LAUTARO MARTÍNEZ", "MARTINEZ L.").Replace("LAUTARO", "MARTINEZ L.")

                Name = Functions.NormalizeText(Name).Trim()

                Dim pm As New PlayerMatch(Role, Name, Team)
                Dim dicRes As SortedDictionary(Of Integer, List(Of WebPlayer)) = New SortedDictionary(Of Integer, List(Of WebPlayer))

                Dim nameList As New List(Of String) From {Name}

                If Name.Contains(" ") Then
                    nameList.AddRange(Name.Replace("’", "").Split(CChar(" ")).ToList())
                End If
                nameList.RemoveAll(Function(n) n.Length < 3)

                For Each t As String In keyplayers.Keys
                    For Each r As String In keyplayers(t).Keys
                        For Each pname As String In keyplayers(t)(r).Keys

                            Dim pnameList As List(Of String) = pname.Replace("’", "").Split(CChar(" ")).ToList()
                            pnameList.RemoveAll(Function(n) n.Length < 3)

                            If nameList.Intersect(pnameList).ToList().Count > 0 Then
                                Dim res As Integer = 0
                                If Role = r Then res -= 1
                                If t = Team Then res -= 1
                                If dicRes.ContainsKey(res) = False Then dicRes.Add(res, New List(Of WebPlayer))
                                dicRes(res).Add(New WebPlayer(r, keyplayers(t)(r)(pname), t))
                            ElseIf Name.Length > 4 Then
                                For Each pn As String In pnameList
                                    For Each n As String In nameList
                                        Dim res As Integer = LevenshteinDistance(n, pn.Trim().Replace(".", ""))
                                        If res < 4 Then
                                            If Role = r Then res -= 1
                                            If t = Team Then res -= 1
                                            If dicRes.ContainsKey(res) = False Then dicRes.Add(res, New List(Of WebPlayer))
                                            dicRes(res).Add(New WebPlayer(r, keyplayers(t)(r)(pname), t))
                                        End If
                                    Next
                                Next
                            End If
                        Next
                    Next
                Next

                If dicRes.Count > 0 Then
                    If dicRes.Keys.First() < 1 Then
                        pm.MatchedPlayer = dicRes(dicRes.Keys.First()).First()
                        pm.MatchedPlayer.Rank = dicRes.Keys.First()
                    End If
                End If

                If AddPlayerToList AndAlso wp IsNot Nothing Then If wp.ContainsKey(Name) = False Then wp.Add(Name, pm)

                dicRes.Clear()

                Return pm

            End Function

            Private Shared Function LevenshteinDistance(ByVal s As String, ByVal t As String) As Integer

                Dim n As Integer = s.Length
                Dim m As Integer = t.Length
                Dim d(n + 1, m + 1) As Integer

                If n = 0 Then
                    Return m
                End If

                If m = 0 Then
                    Return n
                End If

                Dim i As Integer
                Dim j As Integer

                For i = 0 To n
                    d(i, 0) = i
                Next

                For j = 0 To m
                    d(0, j) = j
                Next

                For i = 1 To n
                    For j = 1 To m

                        Dim cost As Integer
                        If t(j - 1) = s(i - 1) Then
                            cost = 0
                        Else
                            cost = 1
                        End If

                        d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1), d(i - 1, j - 1) + cost)
                    Next
                Next

                Return d(n, m)
            End Function

        End Class

        Public Class WebPlayer
            Sub New()

            End Sub

            Sub New(Name As String)
                Me.Name = Name
            End Sub

            Sub New(Role As String, Name As String, Team As String)
                Me.Role = Role
                Me.Name = Name
                Me.Team = Team
            End Sub

            Public Property Role As String = ""
            Public Property Name As String = ""
            Public Property Team As String = ""
            Public Property Rank As Integer = 10

        End Class

        Public Class PlayerMatch

            Sub New(Name As String)
                SourcePlayer.Name = Name
            End Sub

            Sub New(Role As String, Name As String)
                SourcePlayer.Role = Role
                SourcePlayer.Name = Name
            End Sub

            Sub New(Role As String, Name As String, Team As String)
                SourcePlayer.Role = Role
                SourcePlayer.Name = Name
                SourcePlayer.Team = Team
            End Sub

            Public Property SourcePlayer As WebPlayer = New WebPlayer
            Public Property MatchedPlayer As WebPlayer = New WebPlayer

            Public ReadOnly Property Matched As Boolean
                Get
                    If MatchedPlayer.Name = "" Then
                        Return False
                    Else
                        Return True
                    End If
                End Get
            End Property

            Public Function GetName() As String
                If MatchedPlayer.Name <> "" Then
                    Return MatchedPlayer.Name
                Else
                    Return SourcePlayer.Name
                End If
            End Function

            Public Function GetRole() As String
                If MatchedPlayer.Role <> "" Then
                    Return MatchedPlayer.Role
                Else
                    Return SourcePlayer.Role
                End If
            End Function

            Public Function GetWebPlayer() As WebPlayer
                If Me.Matched Then
                    Return MatchedPlayer
                Else
                    Return SourcePlayer
                End If
            End Function

        End Class

    End Namespace
End Namespace
