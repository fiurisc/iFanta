
Namespace WebData
    Namespace Players

        Public Class Data

            Public Shared players As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
            'COGNome Nome/Nome LENGHT/LIST Nome ASSOCIATI'
            'Public Shared keyplayers As New Dictionary(Of String, WebPlayerKey)
            Public Shared keyplayers As New Dictionary(Of String, Dictionary(Of String, WebPlayerKey))

            Public Shared Sub ResetCacheData()
                players.Clear()
            End Sub

            Public Shared Sub LoadPlayers(appSett As Torneo.PublicVariables, forceRelod As Boolean)

                Dim pquotes As New PlayersQuotes(appSett)
                Dim fdata As String = pquotes.GetDataFileName()

                If forceRelod Then players.Clear()

                If players.Count = 0 Then

                    If IO.File.Exists(fdata) Then

                        Dim playersq As List(Of Torneo.Players.PlayerQuotesItem) = Functions.DeserializeJson(Of List(Of Torneo.Players.PlayerQuotesItem))(System.IO.File.ReadAllText(fdata))

                        For Each p As Torneo.Players.PlayerQuotesItem In playersq

                            If p.Nome.Contains("BERNA") Then
                                p.Nome = p.Nome
                            End If

                            If players.ContainsKey(p.Squadra) = False Then players.Add(p.Squadra, New Dictionary(Of String, List(Of String)))
                            If players(p.Squadra).ContainsKey(p.Ruolo) = False Then players(p.Squadra).Add(p.Ruolo, New List(Of String))
                            players(p.Squadra)(p.Ruolo).Add(p.Nome)

                            If keyplayers.ContainsKey(p.Squadra) = False Then keyplayers.Add(p.Squadra, New Dictionary(Of String, WebPlayerKey))
                            If keyplayers(p.Squadra).ContainsKey(p.Ruolo) = False Then keyplayers(p.Squadra).Add(p.Ruolo, New WebPlayerKey)

                            Dim keylist As List(Of String) = GetKeyWordList(Functions.CleanSpecialChar(p.Nome))

                            For k As Integer = 0 To keylist.Count - 1
                                Dim subkey() As String = keylist(k).Split(CChar("/"))
                                Call AddwPlayerWordKey(keyplayers(p.Squadra)(p.Ruolo), subkey, 0, p.Nome, p.Ruolo)
                            Next
                        Next
                    End If
                End If

            End Sub

            Private Shared Sub AddwPlayerWordKey(wk As WebPlayerKey, subkey As String(), ind As Integer, Name As String, Role As String)
                If wk.key.ContainsKey(subkey(ind)) = False Then
                    wk.key.Add(subkey(ind), New WebPlayerKey(Name, Role))
                End If
                ind += 1
                If ind < subkey.Length Then
                    Call AddwPlayerWordKey(wk.key(subkey(ind - 1)), subkey, ind, Name, Role)
                End If
            End Sub

            Private Shared Function GetKeyWordList(Name As String) As List(Of String)

                If Name.Contains(" ") = False Then Name += " UNK"

                Dim sn() As String = Name.Split(CChar(" "))
                Dim keylist As New List(Of String)

                For k As Integer = 0 To sn.Length - 1
                    Dim ind As Integer = k
                    Dim key As String = ""
                    For j As Integer = 0 To sn.Length - 1
                        key = key & "/" & sn(ind)
                        ind += 1
                        If ind > sn.Length - 1 Then ind = 0
                    Next
                    key = key.Substring(1)
                    If keylist.Contains(key) = False Then keylist.Add(key)
                Next

                Return keylist

            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, FindAllTeam As Boolean) As Players.PlayerMatch
                Return ResolveName(Role, Name, Team, Nothing, FindAllTeam)
            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, Players.PlayerMatch), FindAllTeam As Boolean) As Players.PlayerMatch
                Return ResolveName(Role, Name, Team, wp, FindAllTeam, True)
            End Function

            Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, Players.PlayerMatch), FindAllTeam As Boolean, AddPlayerToList As Boolean) As Players.PlayerMatch

                Name = Name.Replace("MILINKOVIC-SAVIC", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC V.", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC S.", "MILINKOVIC SAVIC").Replace("DEL PRATO", "DELPRATO").Replace("DEL PRATO", "DELPRATO")
                Name = Name.Replace("P.ESPOSITO", "ESPOSITO F.P.")

                Dim pm As New Players.PlayerMatch(Role, Name, Team)

                If keyplayers.ContainsKey(Team) Then

                    Name = Name.ToUpper().Trim()
                    Name = Functions.NormalizeText(Name)

                    If Name.Contains("MARTINEZ") Then
                        Name = Name
                    End If

                    Dim keylist As List(Of String) = GetKeyWordList(Functions.CleanSpecialChar(Name))
                    Dim macthlist As New SortedDictionary(Of Integer, List(Of Players.WebPlayerKeyMatch))
                    Dim macth As New Players.WebPlayerKeyMatch
                    If Role <> "" Then macth = CheckName(keyplayers(Team)(Role), Team, keylist)
                    If macth Is Nothing OrElse macth.Name = "" Then
                        macth = CheckName(keyplayers(Team), Team, keylist)
                    End If

                    If macth IsNot Nothing Then
                        If macthlist.ContainsKey(macth.KeyLength) = False Then macthlist.Add(macth.KeyLength, New List(Of Players.WebPlayerKeyMatch))
                        macthlist(macth.KeyLength).Add(macth)
                    ElseIf FindAllTeam Then
                        For Each t As String In keyplayers.Keys
                            macth = CheckName(keyplayers(t)(Role), t, keylist)
                            If macth IsNot Nothing Then
                                If macth.Name = Name Then macth.KeyLength = Name.Length
                                If macthlist.ContainsKey(macth.KeyLength) = False Then macthlist.Add(macth.KeyLength, New List(Of Players.WebPlayerKeyMatch))
                                macthlist(macth.KeyLength).Add(macth)
                            End If
                        Next
                    End If

                    If macthlist.Keys.Count > 0 Then
                        Dim key(macthlist.Keys.Count - 1) As Integer
                        macthlist.Keys.CopyTo(key, 0)
                        If macthlist(key(key.Length - 1)).Count = 1 Then
                            pm.MatchedPlayer = New Players.WebPlayer(macthlist(key(key.Length - 1))(0).Role, macthlist(key(key.Length - 1))(0).Name, macthlist(key(key.Length - 1))(0).Team)
                        End If
                    End If

                End If

                If AddPlayerToList AndAlso wp IsNot Nothing Then If wp.ContainsKey(Name) = False Then wp.Add(Name, pm)

                Return pm

            End Function

            Public Shared Function CheckName(wk As Dictionary(Of String, Players.WebPlayerKey), Team As String, keylist As List(Of String)) As Players.WebPlayerKeyMatch

                For Each r As String In wk.Keys
                    For k As Integer = 0 To keylist.Count - 1
                        Dim subkey() As String = keylist(k).Split(CChar("/"))
                        Dim macth As Players.WebPlayerKey = CheckName(wk(r), subkey, 0)
                        If macth IsNot Nothing Then
                            Return New Players.WebPlayerKeyMatch(macth.Name, Team, macth.Role, subkey(0).Length)
                            Exit For
                        End If
                    Next
                Next

                Return Nothing

            End Function

            Public Shared Function CheckName(wk As Players.WebPlayerKey, Team As String, keylist As List(Of String)) As Players.WebPlayerKeyMatch

                For k As Integer = 0 To keylist.Count - 1
                    Dim subkey() As String = keylist(k).Split(CChar("/"))
                    Dim macth As Players.WebPlayerKey = CheckName(wk, subkey, 0)
                    If macth IsNot Nothing Then
                        Return New Players.WebPlayerKeyMatch(macth.Name, Team, macth.Role, subkey(0).Length)
                        Exit For
                    End If
                Next

                Return Nothing

            End Function

            Public Shared Function CheckName(wk As Players.WebPlayerKey, subkey As String(), ind As Integer) As Players.WebPlayerKey

                If wk.key.ContainsKey(subkey(ind)) AndAlso subkey(ind) <> "UNK" AndAlso subkey(ind).Length > 2 Then
                    If wk.key(subkey(ind)).key.Count = 0 Then
                        Return wk.key(subkey(ind))
                    ElseIf wk.key(subkey(ind)).key.Count = 1 Then
                        Return wk.key(subkey(ind))
                    Else
                        If subkey.Count > 1 Then
                            For Each w As String In wk.key(subkey(0)).key.Keys
                                If w.StartsWith(subkey(1)) OrElse subkey(1).StartsWith(w) Then
                                    If wk.key(w).key.Count = 0 Then
                                        Return wk.key(w)
                                    ElseIf wk.key(w).key.Count = 1 Then
                                        Return wk.key(w)
                                    Else
                                        Return Nothing
                                    End If
                                End If
                            Next
                        End If
                    End If
                Else
                    If ind > 0 Then
                        For Each w As String In wk.key.Keys
                            If w.StartsWith(subkey(ind)) OrElse subkey(ind).StartsWith(w) Then
                                If wk.key(w).key.Count = 0 Then
                                    Return wk.key(w)
                                ElseIf wk.key(w).key.Count = 1 Then
                                    Return wk.key(w)
                                Else
                                    Return Nothing
                                End If
                            End If
                        Next
                        Return Nothing
                    Else
                        Return Nothing
                    End If
                End If
                Return Nothing
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

        Public Class NewWebPlayerKey
            Public Property key As New Dictionary(Of String, WebPlayerKey)
            Public Property Name As String = ""
            Public Property Role As String = ""

            Sub New()

            End Sub

            Sub New(Name As String, Role As String)
                Me.Name = Name
                Me.Role = Role
            End Sub

        End Class

        Public Class WebPlayerKey
            Public Property key As New Dictionary(Of String, WebPlayerKey)
            Public Property Name As String = ""
            Public Property Role As String = ""

            Sub New()

            End Sub

            Sub New(Name As String, Role As String)
                Me.Name = Name
                Me.Role = Role
            End Sub

        End Class

        Public Class WebPlayerKeyMatch

            Public Property Name As String = ""
            Public Property Team As String = ""
            Public Property Role As String = ""
            Public Property KeyLength As Integer = 0

            Sub New()

            End Sub

            Sub New(Name As String, Team As String, Role As String, KeyLength As Integer)
                Me.Name = Name
                Me.Team = Team
                Me.Role = Role
                Me.KeyLength = KeyLength
            End Sub

        End Class
    End Namespace
End Namespace
