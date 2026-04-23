
Imports System.Data
Imports System.Text.RegularExpressions

Namespace WebData
    Namespace Players

        Public Class Data

            Public Shared players As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
            'COGNome Nome/Nome LENGHT/LIST Nome ASSOCIATI'
            'Public Shared keyplayers As New Dictionary(Of String, WebPlayerKey)
            'squadra/ruolo/key/nome
            Public Shared keyplayers As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, String)))
            Public Shared keyplayersNew As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, String)))

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

                            If p.Nome.Contains("TOURE’ I.") Then
                                p.Nome = p.Nome
                            End If

                            If players.ContainsKey(p.Squadra) = False Then players.Add(p.Squadra, New Dictionary(Of String, List(Of String)))
                            If players(p.Squadra).ContainsKey(p.Ruolo) = False Then players(p.Squadra).Add(p.Ruolo, New List(Of String))
                            players(p.Squadra)(p.Ruolo).Add(p.Nome)

                            If keyplayers.ContainsKey(p.Squadra) = False Then keyplayers.Add(p.Squadra, New Dictionary(Of String, Dictionary(Of String, String)))
                            If keyplayers(p.Squadra).ContainsKey(p.Ruolo) = False Then keyplayers(p.Squadra).Add(p.Ruolo, New Dictionary(Of String, String))

                            Dim keylist As List(Of String) = GetKeyWordList(p.Nome, True)

                            For k As Integer = 0 To keylist.Count - 1
                                If keyplayers(p.Squadra)(p.Ruolo).ContainsKey(keylist(k)) = False Then
                                    keyplayers(p.Squadra)(p.Ruolo).Add(keylist(k), p.Nome)
                                End If
                            Next
                        Next
                    End If
                End If

            End Sub

            Private Shared Function GetKeyWordList(Name As String, fromPlayer As Boolean) As List(Of String)

                Return New List(Of String) From {Name}

                'Dim keylist As New List(Of String)
                'Dim NameOptions As New List(Of String) From {Name}

                'If Name.Contains("ANGUISSA") Then
                '    Name = Name
                'End If
                'If Name.Contains("’") Then
                '    NameOptions.Add(Name.Replace("’", ""))
                'End If

                'For Each n As String In NameOptions
                '    If n.Contains(" ") Then

                '        Dim parts = n.Trim().Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                '        Dim pattern As String = "^(.+)\s+(\S+)$"
                '        Dim m As Match = Regex.Match(n.ToUpper(), pattern)

                '        If m.Success Then

                '            Dim cognome As String = m.Groups(1).Value.Trim()
                '            Dim nome As String = m.Groups(2).Value.Trim()

                '            Try
                '                keylist.Add(cognome)
                '                keylist.Add(cognome & "/" & nome)
                '                keylist.Add(cognome & "/" & nome.Substring(0, 1))
                '                If nome.Length > 3 Then
                '                    keylist.Add(nome)
                '                    keylist.Add(nome & "/" & cognome)
                '                    keylist.Add(nome & "/" & cognome.Substring(0, 1))
                '                End If
                '            Catch ex As Exception
                '                nome = Name
                '            End Try

                '        End If

                '    End If

                '    If keylist.Contains(n) = False Then keylist.Add(n.Replace(".", ""))

                'Next

                'Return keylist.Distinct().ToList()

            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, FindAllTeam As Boolean) As Players.PlayerMatch
                Return ResolveName(Role, Name, Team, Nothing, FindAllTeam)
            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, Players.PlayerMatch), FindAllTeam As Boolean) As Players.PlayerMatch
                Return ResolveName(Role, Name, Team, wp, FindAllTeam, True)
            End Function

            Public Shared Function ResolveName1(Role As String, Name As String, Team As String, wp As Dictionary(Of String, Players.PlayerMatch), FindAllTeam As Boolean, AddPlayerToList As Boolean) As Players.PlayerMatch

                If Name.Contains("LAUTARO") Then
                    Name = Name
                End If

                Name = Name.Replace("MILINKOVIC-SAVIC", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC V.", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC S.", "MILINKOVIC SAVIC").Replace("DEL PRATO", "DELPRATO").Replace("DEL PRATO", "DELPRATO")
                Name = Name.Replace("P.ESPOSITO", "ESPOSITO F.P.")
                Name = Name.Replace("ROBERTO S.", "SERGI ROBERTO.")
                Name = Name.Replace("JESUS J.", "JUAN JESUS").Replace("LAUTARO MARTÍNEZ", "MARTINEZ L.").Replace("LAUTARO", "MARTINEZ L.")

                Name = Name.ToUpper().Trim()
                Name = Functions.NormalizeText(Name)

                If Regex.Match(Name, "\w+\s+\w+").Success Then
                    Name = Regex.Match(Name, "[\w+\.]{1,}\s+[\w+\.]{1,}").Value
                End If
                If Regex.Match(Name, "\w{1}\.\s\w{1,}").Success Then
                    Dim s() As String = Name.Split(CChar(" "))
                    Name = s(1) & " " & s(0)
                End If

                Dim pm As New Players.PlayerMatch(Role, Name, Team)
                Dim keylist As List(Of String) = GetKeyWordList(Name, False)

                For Each t As String In keyplayers.Keys
                    If Team = "" OrElse t = Team Then

                        If Name.Contains("MARTINEZ") Then
                            Name = Name
                        End If

                        For Each r As String In keyplayers(t).Keys
                            If keyplayers(t)(r).ContainsKey(Name) Then
                                pm.MatchedPlayer = New WebPlayer(r, keyplayers(t)(r)(Name), t)
                                If AddPlayerToList AndAlso wp IsNot Nothing Then If wp.ContainsKey(Name) = False Then wp.Add(Name, pm)
                                Return pm
                            End If
                        Next

                        Dim pkeyLenght As Integer = 0

                        For Each r As String In keyplayers(t).Keys
                            If Role = "" OrElse r = Role Then
                                For Each pkey As String In keylist
                                    If keyplayers(t)(r).ContainsKey(pkey) Then
                                        If pkey.Length > pkeyLenght Then
                                            pm.MatchedPlayer = New WebPlayer(r, keyplayers(t)(r)(pkey), t)
                                            'If AddPlayerToList AndAlso wp IsNot Nothing Then If wp.ContainsKey(Name) = False Then wp.Add(Name, pm)
                                            pkeyLenght = pkey.Length
                                        End If
                                    End If
                                Next
                            End If
                        Next
                        If pm.Matched = False Then
                            CheckPlayer(pm, "", t, keylist)
                        End If
                    End If
                Next

                If AddPlayerToList AndAlso wp IsNot Nothing Then If wp.ContainsKey(Name) = False Then wp.Add(Name, pm)

                Return pm

            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, Players.PlayerMatch), FindAllTeam As Boolean, AddPlayerToList As Boolean) As Players.PlayerMatch

                If Name.Contains("N’DRI K.") Then
                    Name = Name
                End If

                Name = Name.Replace("MILINKOVIC-SAVIC", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC V.", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC S.", "MILINKOVIC SAVIC").Replace("DEL PRATO", "DELPRATO").Replace("DEL PRATO", "DELPRATO")
                Name = Name.Replace("P.ESPOSITO", "ESPOSITO F.P.")
                Name = Name.Replace("ROBERTO S.", "SERGI ROBERTO.").Replace("GABRIEL T.", "TIAGO GABRIEL")
                Name = Name.Replace("CARLOS D.", "DIEGO CARLOS").Replace("ANGUISSA A.", "ZAMBO ANGUISSA")
                Name = Name.Replace("JESUS J.", "JUAN JESUS").Replace("LAUTARO MARTÍNEZ", "MARTINEZ L.").Replace("LAUTARO", "MARTINEZ L.")

                Name = Name.ToUpper().Trim()
                Name = Functions.NormalizeText(Name)

                Dim pm As New Players.PlayerMatch(Role, Name, Team)
                Dim dicRes As SortedDictionary(Of Integer, List(Of WebPlayer)) = New SortedDictionary(Of Integer, List(Of WebPlayer))

                Dim nameList As New List(Of String) From {Name.Replace("’", "")}

                If Name.Contains(" ") Then
                    nameList.AddRange(Name.Replace("’", "").Split(CChar(" ")).ToList())
                End If
                nameList.RemoveAll(Function(n) n.Length < 3)

                For Each t As String In keyplayers.Keys
                    If Team = "" OrElse t = Team Then
                        For Each r As String In keyplayers(t).Keys
                            If Role = "" OrElse r = Role Then
                                For Each pname As String In keyplayers(t)(r).Keys

                                    Dim pnameList As List(Of String) = pname.Replace("’", "").Split(CChar(" ")).ToList()
                                    pnameList.RemoveAll(Function(n) n.Length < 3)

                                    If nameList.Intersect(pnameList).ToList().Count > 0 Then
                                        Dim res As Integer = LevenshteinDistance(Name, pname.Trim().Replace(".", ""))
                                        If dicRes.ContainsKey(res) = False Then dicRes.Add(res, New List(Of WebPlayer))
                                        dicRes(res).Add(New WebPlayer(r, keyplayers(t)(r)(pname), t))
                                    End If

                                Next
                            End If
                        Next
                    End If
                Next

                If dicRes.Count > 0 Then
                    pm.MatchedPlayer = dicRes(dicRes.Keys.First()).First()
                End If

                If AddPlayerToList AndAlso wp IsNot Nothing Then If wp.ContainsKey(Name) = False Then wp.Add(Name, pm)

                dicRes.Clear()

                Return pm

            End Function

            Private Shared Sub CheckPlayer(pm As Players.PlayerMatch, Role As String, Team As String, keylist As List(Of String))
                Dim pkeyLenght As Integer = 0
                For Each r As String In keyplayers(Team).Keys
                    If Role = "" OrElse r = Role Then
                        For Each pkey As String In keylist
                            If keyplayers(Team)(r).ContainsKey(pkey) Then
                                If pkey.Length > pkeyLenght Then
                                    pm.MatchedPlayer = New WebPlayer(r, keyplayers(Team)(r)(pkey), Team)
                                    pkeyLenght = pkey.Length
                                End If
                            End If
                        Next
                    End If
                Next
            End Sub

            'Public Shared Function CheckName(wk As Dictionary(Of String, Players.WebPlayerKey), Team As String, keylist As List(Of String)) As Players.WebPlayerKeyMatch

            '    For Each r As String In wk.Keys
            '        For k As Integer = 0 To keylist.Count - 1
            '            If keylist(k).Contains("/") = False AndAlso wk(r).key.ContainsKey(keylist(k)) Then
            '                Return New Players.WebPlayerKeyMatch(keylist(k), Team, r, keylist(k).Length)
            '                Exit For
            '            End If
            '        Next
            '    Next

            '    For Each r As String In wk.Keys
            '        For k As Integer = 0 To keylist.Count - 1
            '            If keylist(k).Contains("/") Then
            '                Dim subkey() As String = keylist(k).Split(CChar("/"))
            '                Dim macth As Players.WebPlayerKey = CheckName(wk(r), subkey, 0)
            '                If macth IsNot Nothing Then
            '                    Return New Players.WebPlayerKeyMatch(macth.Name, Team, macth.Role, subkey(0).Length)
            '                    Exit For
            '                End If
            '            End If

            '        Next
            '    Next

            '    Return Nothing

            'End Function

            'Public Shared Function CheckName(wk As Players.WebPlayerKey, Team As String, keylist As List(Of String)) As Players.WebPlayerKeyMatch

            '    For k As Integer = 0 To keylist.Count - 1
            '        Dim subkey() As String = keylist(k).Split(CChar("/"))
            '        Dim macth As Players.WebPlayerKey = CheckName(wk, subkey, 0)
            '        If macth IsNot Nothing Then
            '            Return New Players.WebPlayerKeyMatch(macth.Name, Team, macth.Role, subkey(0).Length)
            '            Exit For
            '        End If
            '    Next

            '    Return Nothing

            'End Function

            'Public Shared Function CheckName(wk As Players.WebPlayerKey, subkey As String(), ind As Integer) As Players.WebPlayerKey

            '    If wk.key.ContainsKey(subkey(ind)) AndAlso subkey(ind) <> "UNK" AndAlso subkey(ind).Length > 2 Then
            '        If wk.key(subkey(ind)).key.Count = 0 Then
            '            Return wk.key(subkey(ind))
            '        ElseIf wk.key(subkey(ind)).key.Count = 1 Then
            '            Return wk.key(subkey(ind))
            '        Else
            '            If subkey.Count > 1 Then
            '                For Each w As String In wk.key(subkey(0)).key.Keys
            '                    If w.StartsWith(subkey(1)) OrElse subkey(1).StartsWith(w) Then
            '                        If wk.key(w).key.Count = 0 Then
            '                            Return wk.key(w)
            '                        ElseIf wk.key(w).key.Count = 1 Then
            '                            Return wk.key(w)
            '                        Else
            '                            Return Nothing
            '                        End If
            '                    End If
            '                Next
            '            End If
            '        End If
            '    Else
            '        If ind > 0 Then
            '            For Each w As String In wk.key.Keys
            '                If w.StartsWith(subkey(ind)) OrElse subkey(ind).StartsWith(w) Then
            '                    If wk.key(w).key.Count = 0 Then
            '                        Return wk.key(w)
            '                    ElseIf wk.key(w).key.Count = 1 Then
            '                        Return wk.key(w)
            '                    Else
            '                        Return Nothing
            '                    End If
            '                End If
            '            Next
            '            Return Nothing
            '        Else
            '            Return Nothing
            '        End If
            '    End If
            '    Return Nothing
            'End Function

            Public Shared Function LevenshteinDistance(ByVal s As String, ByVal t As String) As Integer

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

                        d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1),
                                   d(i - 1, j - 1) + cost)
                    Next
                Next

                Return d(n, m)
            End Function

        End Class


        Public Class WebPlayer

            Dim _role As String = ""
            Dim _name As String = ""
            Dim _team As String = ""

            Sub New()

            End Sub

            Sub New(Name As String)
                _name = Name
            End Sub

            Sub New(Role As String, Name As String, Team As String)
                _role = Role
                _name = Name
                _team = Team
            End Sub

            Public Property Role As String
                Get
                    Return _role
                End Get
                Set(value As String)
                    _role = value
                End Set
            End Property

            Public Property Name As String
                Get
                    Return _name
                End Get
                Set(value As String)
                    _name = value
                End Set
            End Property

            Public Property Team As String
                Get
                    Return _team
                End Get
                Set(value As String)
                    _team = value
                End Set
            End Property

        End Class

        Public Class PlayerMatch

            Dim _source As New WebPlayer
            Dim _new As New WebPlayer

            Sub New(Name As String)
                _source.Name = Name
            End Sub

            Sub New(Role As String, Name As String)
                _source.Role = Role
                _source.Name = Name
            End Sub

            Sub New(Role As String, Name As String, Team As String)
                _source.Role = Role
                _source.Name = Name
                _source.Team = Team
            End Sub

            Public Property SourcePlayer As WebPlayer
                Get
                    Return _source
                End Get
                Set(value As WebPlayer)
                    _source = value
                End Set
            End Property

            Public Property MatchedPlayer As WebPlayer
                Get
                    Return _new
                End Get
                Set(value As WebPlayer)
                    _new = value
                End Set
            End Property

            Public ReadOnly Property Matched As Boolean
                Get
                    If _new.Name = "" Then
                        Return False
                    Else
                        Return True
                    End If
                End Get
            End Property

            Public Function GetName() As String
                If _new.Name <> "" Then
                    Return _new.Name
                Else
                    Return _source.Name
                End If
            End Function

            Public Function GetRole() As String
                If _new.Role <> "" Then
                    Return _new.Role
                Else
                    Return _source.Role
                End If
            End Function

            Public Function GetWebPlayer() As WebPlayer
                If Me.Matched Then
                    Return _new
                Else
                    Return _source
                End If
            End Function

        End Class

        'Public Class NewWebPlayerKey
        '    Public Property key As New Dictionary(Of String, WebPlayerKey)
        '    Public Property Name As String = ""
        '    Public Property Role As String = ""

        '    Sub New()

        '    End Sub

        '    Sub New(Name As String, Role As String)
        '        Me.Name = Name
        '        Me.Role = Role
        '    End Sub

        'End Class

        'Public Class WebPlayerKey
        '    Public Property Name As String = ""
        '    Public Property Role As String = ""

        '    Sub New()

        '    End Sub

        '    Sub New(Name As String, Role As String)
        '        Me.Name = Name
        '        Me.Role = Role
        '    End Sub

        'End Class

        'Public Class WebPlayerKeyMatch

        '    Public Property Name As String = ""
        '    Public Property Team As String = ""
        '    Public Property Role As String = ""
        '    Public Property KeyLength As Integer = 0

        '    Sub New()

        '    End Sub

        '    Sub New(Name As String, Team As String, Role As String, KeyLength As Integer)
        '        Me.Name = Name
        '        Me.Team = Team
        '        Me.Role = Role
        '        Me.KeyLength = KeyLength
        '    End Sub

        'End Class
    End Namespace
End Namespace
