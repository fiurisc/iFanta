Module PublicData

    Public makefileplayer As Boolean = True ' Abilita la generazione dei file con la lista dei giocatori trovati'
    Public webplayers As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
    Public webmatchs As New Dictionary(Of String, Integer)
    Public dirs As String = ""
    'COGNOME NOME/NOME LENGHT/LIST NOME ASSOCIATI'
    Public wplayer As New Dictionary(Of String, WebData.wPlayerKey)

End Module

Public Class WebData

    Public Class wPlayerKey
        Public Property key As New Dictionary(Of String, wPlayerKey)
        Public Property Name As String = ""
        Public Property Role As String = ""

        Sub New()

        End Sub

        Sub New(Name As String, Role As String)
            Me.Name = Name
            Me.Role = Role
        End Sub

    End Class

    Public Class wPlayerKeyMatch

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

    Public Shared Sub MakeDirectory(ServerPath As String, Year As Integer)

        Dim dirt As String = ServerPath & "\web\" & CStr(Year) & "\temp"
        Dim dird As String = ServerPath & "\web\" & CStr(Year) & "\data"
        Dim dirdpf As String = ServerPath & "\web\" & CStr(Year) & "\data\pforma"
        Dim dirdmt As String = ServerPath & "\web\" & CStr(Year) & "\data\matchs"

        If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
        If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)
        If IO.Directory.Exists(dirdpf) = False Then IO.Directory.CreateDirectory(dirdpf)
        If IO.Directory.Exists(dirdmt) = False Then IO.Directory.CreateDirectory(dirdmt)

    End Sub

    Public Shared Function ExtractIntegerData(Value As String, RegEx As String, DefaultValue As Integer) As Integer

        Try
            Dim s As String = System.Text.RegularExpressions.Regex.Match(Value, RegEx).Value
            If s <> "" Then DefaultValue = CInt(s)
        Catch ex As Exception

        End Try

        Return DefaultValue

    End Function

    Public Shared Function NormalizeText(ByVal txt As String) As String

        If txt.Contains("MARTINEZ") Then
            txt = txt
        End If

        txt = txt.Replace("Í", "I")
        txt = txt.Replace("í", "i")
        txt = txt.Replace("Ç", "C")
        txt = txt.Replace("-", " ")
        txt = txt.Replace("'", "’")
        txt = txt.Replace("&#39;", "’")
        txt = txt.Replace("Ò", "O’")
        txt = txt.Replace("Ô", "O")
        txt = txt.Replace("&#210;", "O’")
        txt = txt.Replace("É", "E’")
        txt = txt.Replace("&#201;", "E’")
        txt = txt.Replace("&#XEC;", "I")
        txt = txt.Replace("&#193;", "A")
        txt = txt.Replace("Ã²", "O’")
        txt = txt.Replace("Ã¨", "A")
        txt = txt.Replace("Ã", "A")
        txt = txt.Replace("Ó", "O")
        txt = txt.Replace("&#211;", "O")
        txt = txt.Replace("Í", "I")
        txt = txt.Replace("&#205;", "I")
        txt = txt.Replace("È", "E")
        txt = txt.Replace("&#200;", "E")
        txt = txt.Replace("Á", "A")
        txt = txt.Replace("À", "A’")
        txt = txt.Replace("&#192;", "A’")
        txt = txt.Replace("Ú", "U")
        txt = txt.Replace("&#218;", "U")
        txt = txt.Replace("Ñ", "N")
        txt = txt.Replace("&#209;", "N")
        txt = txt.Replace("&#39;", "'")
        txt = txt.Replace("â", "A’")
        txt = txt.Replace("&#X2019;", "’").Replace("&#x2019;", "’")
        txt = txt.Replace("&#X2019;", "’").Replace("&#x2019;", "’")
        txt = txt.Replace("&#X27;", "’").Replace("&#x27;", "’")
        txt = txt.Replace("&apos;", "’")
        txt = txt.Replace("&#XE0;", "a'").Replace("&#xE0;", "a'")
        txt = txt.Replace("&#XE8;", "E’").Replace("&#xE8;", "E’")
        txt = txt.Replace("&#XF2;", "O").Replace("&#xF2;", "O")
        Dim regex As New System.Text.RegularExpressions.Regex("[0-9a-zA-Z\'\s\.\-\’]{0,}")
        Dim newtxt As String = ""

        For Each match As System.Text.RegularExpressions.Match In regex.Matches(txt)
            If match.Value.Trim <> "" Then newtxt = newtxt & match.Value
        Next

        Return newtxt

    End Function

    Public Shared Function CleanSpecialChar(txt As String) As String
        txt = txt.Replace("’", "")
        txt = txt.Replace(".", "")
        Return txt
    End Function

    Public Shared Function ResolveName(Role As String, Name As String, Team As String, FindAllTeam As Boolean) As PlayerMatch
        Return ResolveName(Role, Name, Team, Nothing, FindAllTeam)
    End Function

    Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, PlayerMatch), FindAllTeam As Boolean) As PlayerMatch
        Return ResolveName(Role, Name, Team, wp, FindAllTeam, True)
    End Function

    Public Shared Function ResolveName(Role As String, Name As String, Team As String, wp As Dictionary(Of String, PlayerMatch), FindAllTeam As Boolean, AddPlayerToList As Boolean) As PlayerMatch

        Dim pm As New PlayerMatch(Role, Name, Team)

        If wplayer.ContainsKey(Team) Then

            Name = Name.ToUpper().Trim()
            Name = NormalizeText(Name)

            If Name.Contains("MARTINEZ") Then
                Name = Name
            End If

            Dim keylist As List(Of String) = GetKeyWordList(CleanSpecialChar(Name))
            Dim macthlist As New SortedDictionary(Of Integer, List(Of wPlayerKeyMatch))
            Dim macth As wPlayerKeyMatch = CheckName(wplayer(Team), Team, keylist)

            If macth IsNot Nothing Then
                If macthlist.ContainsKey(macth.KeyLength) = False Then macthlist.Add(macth.KeyLength, New List(Of wPlayerKeyMatch))
                macthlist(macth.KeyLength).Add(macth)
            ElseIf FindAllTeam Then
                For Each t As String In wplayer.Keys
                    macth = CheckName(wplayer(t), t, keylist)
                    If macth IsNot Nothing Then
                        If macth.Name = Name Then macth.KeyLength = Name.Length
                        If macthlist.ContainsKey(macth.KeyLength) = False Then macthlist.Add(macth.KeyLength, New List(Of wPlayerKeyMatch))
                        macthlist(macth.KeyLength).Add(macth)
                    End If
                Next
            End If

            If macthlist.Keys.Count > 0 Then
                Dim key(macthlist.Keys.Count - 1) As Integer
                macthlist.Keys.CopyTo(key, 0)
                If macthlist(key(key.Length - 1)).Count = 1 Then
                    pm.MatchedPlayer = New WebPlayer(macthlist(key(key.Length - 1))(0).Role, macthlist(key(key.Length - 1))(0).Name, macthlist(key(key.Length - 1))(0).Team)
                End If
            End If

        End If

        If AddPlayerToList AndAlso wp IsNot Nothing Then If wp.ContainsKey(Name) = False Then wp.Add(Name, pm)

        Return pm

    End Function

    Public Shared Function CheckName(wk As wPlayerKey, Team As String, keylist As List(Of String)) As wPlayerKeyMatch

        For k As Integer = 0 To keylist.Count - 1
            Dim subkey() As String = keylist(k).Split(CChar("/"))
            Dim macth As wPlayerKey = CheckName(wk, subkey, 0)
            If macth IsNot Nothing Then
                Return New wPlayerKeyMatch(macth.Name, Team, macth.Role, subkey(0).Length)
                Exit For
            End If
        Next

        Return Nothing

    End Function

    Public Shared Function CheckName(wk As wPlayerKey, subkey As String(), ind As Integer) As wPlayerKey

        If wk.key.ContainsKey(subkey(ind)) AndAlso subkey(ind).Length > 2 Then
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

    'Public Shared Function CheckName(ByVal Name As String) As PlayerMatch
    '    Return CheckName("", Name, "")
    'End Function

    'Public Shared Function CheckName(ByVal Name As String, Team As String) As PlayerMatch
    '    Return CheckName("", Name, Team)
    'End Function

    'Public Shared Function CheckName(Role As String, ByVal Name As String, Team As String) As PlayerMatch

    '    Name = Name.Trim

    '    Dim s() As String = Name.Split(CChar(" "))
    '    Dim check As New PlayerMatch(Role, Name, Team)

    '    If Name.Contains("RODRIGUEZ R.") Then
    '        Name = Name
    '    End If

    '    If s.Length = 3 Then
    '        'Nel caso in cui la stringa e' formata da 3 sotto stringhe
    '        'significa che siamo in presenza di un cognome composto
    '        'e quindi ricostrisco il cognome completo unendo le due sotto stringhe
    '        'ex. DE LAURENTIS MIRCO'
    '        'S(0)=DE  -  S(1)=LAURENTIS  -  S(2)=MIRCO
    '        'S1=s(0)+s(1) -> DE LAURENTIS
    '        If wplayer.ContainsKey(s(0) & " " & s(1)) Then
    '            check = CheckName(s(0) & " " & s(1), s(2), Role, Team)
    '        End If
    '        If check.Matched = False AndAlso wplayer.ContainsKey(s(1) & " " & s(2)) Then
    '            check = CheckName(s(1) & " " & s(2), s(0), Role, Team)
    '        End If
    '        If check.Matched = False AndAlso wplayer.ContainsKey(s(0)) Then
    '            check = CheckName(s(0), s(1), Role, Team)
    '            If check.Matched = False Then
    '                check = CheckName(s(0), s(2), Role, Team)
    '            End If
    '        End If
    '        If check.Matched = False AndAlso wplayer.ContainsKey(s(1)) Then
    '            check = CheckName(s(1), s(0), Role, Team)
    '            If check.Matched = False Then
    '                check = CheckName(s(1), s(2), Role, Team)
    '            End If
    '        End If
    '        If check.Matched = False AndAlso wplayer.ContainsKey(s(2)) Then
    '            check = CheckName(s(2), s(0), Role, Team)
    '            If check.Matched = False Then
    '                check = CheckName(s(2), s(1), Role, Team)
    '            End If
    '        End If
    '    ElseIf s.Length = 2 Then
    '        'Cerco assumendo che il primo campo sia il cognome Ex. MALDINI PAOLO'
    '        If wplayer.ContainsKey(s(0)) Then
    '            check = CheckName(s(0), s(1), Role, Team)
    '        End If
    '        'Cerco assumendo che il primo campo sia il nome Ex. PAOLO MALDINI'
    '        If check.Matched = False Then
    '            If wplayer.ContainsKey(s(1)) Then
    '                check = CheckName(s(1), s(0), Role, Team)
    '            ElseIf wplayer.ContainsKey(s(1)) Then 'rieseguo la richerca eliminando gli apostrofi'
    '                check = CheckName(s(1), s(0), Role, Team)
    '            End If
    '        End If
    '        'Cerco assumendo che i due valori facciano riferimento ad un cognome composto Es. DE FRANCESCO'
    '        If check.Matched = False Then
    '            If wplayer.ContainsKey(s(0) & " " & s(1)) Then
    '                check = CheckName(s(0) & " " & s(1), "", Role, Team)
    '            End If
    '        End If
    '    Else
    '        'Cerco assumendo che il valore faccia riferimento al solo cognome Es. FRANCESCHINI'
    '        If wplayer.ContainsKey(s(0)) Then
    '            check = CheckName(s(0), "", Role, Team)
    '        ElseIf wplayer.ContainsKey(s(0)) Then 'rieseguo la richerca eliminando gli apostrofi'
    '            check = CheckName(s(0), "", Role, Team)
    '        End If
    '    End If

    '    Return check

    'End Function

    'Private Shared Function CheckName(S1 As String, S2 As String, Ruolo As String, Squadra As String) As PlayerMatch

    '    Dim text As String = S1 & " " & S2
    '    Dim check As New PlayerMatch(text.Trim)

    '    Try

    '        'Dim key(wplayer(S1).Keys.Count - 1) As Integer
    '        'Dim c As Integer = 0

    '        'text = text.Trim

    '        'If S1.Contains("PERES") OrElse S2.Contains("PERES") Then
    '        '    S1 = S1
    '        'End If

    '        'For Each l As Integer In wplayer(S1).Keys
    '        '    key(c) = l
    '        '    c = c + 1
    '        'Next

    '        'Array.Sort(key)
    '        'Array.Reverse(key)

    '        'For Each l As Integer In key

    '        '    For k As Integer = 0 To wplayer(S1)(l).Count - 1

    '        '        Dim sname As String = wplayer(S1)(l)(k).Name.Replace(".", "")

    '        '        If S2 = "" Then sname = S1

    '        '        If text.StartsWith(sname) Then
    '        '            If Squadra = "" OrElse Squadra = wplayer(S1)(l)(k).Team Then
    '        '                If Ruolo = "" OrElse Ruolo = wplayer(S1)(l)(k).Role Then
    '        '                    check.MatchedPlayer.Role = wplayer(S1)(l)(k).Role
    '        '                    check.MatchedPlayer.Name = wplayer(S1)(l)(k).Name
    '        '                    check.MatchedPlayer.Team = wplayer(S1)(l)(k).Team
    '        '                    Exit For
    '        '                End If
    '        '            End If
    '        '        End If
    '        '    Next

    '        '    If check.Matched Then Exit For

    '        'Next
    '    Catch ex As Exception
    '        Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
    '    End Try

    '    Return check

    'End Function

    Public Function GetDicNatCodeList(fname As String) As Dictionary(Of String, String)

        Dim dicnatcode As New Dictionary(Of String, String)

        If IO.File.Exists(fname) Then
            Dim line() As String = IO.File.ReadAllLines(fname)
            For i As Integer = 0 To line.Length - 1
                Dim s() As String = line(i).Split(CChar(","))
                If s.Length = 2 Then
                    If dicnatcode.ContainsKey(s(0)) = False Then dicnatcode.Add(s(0), s(1))
                End If
            Next
        End If

        Return dicnatcode

    End Function

    Public Shared Sub LoadWebPlayers(fname As String)

        If webplayers.Count = 0 Then

            If IO.File.Exists(fname) Then

                Dim lines() As String = IO.File.ReadAllLines(fname, System.Text.Encoding.GetEncoding("ISO-8859-1"))

                For i As Integer = 0 To lines.Length - 1

                    Dim s() As String = lines(i).Split(CChar("|"))

                    If i > 0 AndAlso s.Length = 5 Then

                        Dim role As String = s(0)
                        Dim name As String = NormalizeText(s(1))
                        Dim team As String = s(2)

                        If name.Contains("RANOCCHIA") Then
                            name = name
                        End If
                        If webplayers.ContainsKey(team) = False Then webplayers.Add(team, New Dictionary(Of String, List(Of String)))
                        If webplayers(team).ContainsKey(role) = False Then webplayers(team).Add(role, New List(Of String))

                        webplayers(team)(role).Add(name)

                        If wplayer.ContainsKey(team) = False Then wplayer.Add(team, New wPlayerKey)

                        Dim keylist As List(Of String) = GetKeyWordList(CleanSpecialChar(name))

                        For k As Integer = 0 To keylist.Count - 1
                            Dim subkey() As String = keylist(k).Split(CChar("/"))
                            Call AddwPlayerWordKey(wplayer(team), subkey, 0, name, role)
                        Next

                    End If
                Next
            End If

        End If

        fname = fname

    End Sub

    Public Shared Function GetKeyWordList(Name As String) As List(Of String)

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

    Public Shared Sub AddwPlayerWordKey(wk As wPlayerKey, subkey As String(), ind As Integer, Name As String, Role As String)
        If wk.key.ContainsKey(subkey(ind)) = False Then
            wk.key.Add(subkey(ind), New wPlayerKey(Name, Role))
        End If
        ind += 1
        If ind < subkey.Length Then
            Call AddwPlayerWordKey(wk.key(subkey(ind - 1)), subkey, ind, Name, Role)
        End If
    End Sub

    Public Shared Sub LoadWebMatchs(fname As String)

        If webmatchs.Count = 0 Then

            If IO.File.Exists(fname) Then

                Dim lines() As String = IO.File.ReadAllLines(fname)

                For i As Integer = 0 To lines.Length - 1
                    Dim s() As String = lines(i).Split(CChar("|"))
                    If s.Length > 3 Then

                        Dim gg As Integer = CInt(s(0)) 'giornata'
                        Dim t1 As String = s(2) 'squadra in casa'
                        Dim t2 As String = s(3) 'squadra fuori casa'
                        Dim key As String = t1 & "-" & t2

                        If webmatchs.ContainsKey(key) = False Then webmatchs.Add(key, gg)

                    End If
                Next
            End If

        End If

    End Sub

    Public Shared Function CheckTeamName(ByVal Squadra As String) As String
        Squadra = Squadra.ToUpper.Replace("CHIEVOVERONA", "CHIEVO").Trim
        Squadra = Squadra.ToUpper.Replace("CHIEVO-VERONA", "CHIEVO").Trim
        Squadra = Squadra.ToUpper.Replace("CHIEVO VERONA", "CHIEVO").Trim
        Squadra = Squadra.ToUpper.Replace("HELLASVERONA", "VERONA").Trim
        Squadra = Squadra.ToUpper.Replace("HELLAS-VERONA", "VERONA").Trim
        Squadra = Squadra.ToUpper.Replace("HELLAS VERONA", "VERONA").Trim
        If Squadra = "GEN" Then Squadra = Squadra = "GENOA"
        If Squadra = "CAG" Then Squadra = Squadra = "CAGLIARI"
        If Squadra.ToUpper = "JUVE" Then Squadra = "JUVENTUS"
        Return Squadra
    End Function

    Public Function GetNatCode(natcode As String) As String
        Select Case natcode
            Case "REP" : natcode = "CZE"
            Case "ING" : natcode = "GBR"
            Case "ENG" : natcode = "GBR"
            Case "CHI" : natcode = "CHL"
            Case "GRE" : natcode = "GRC"
            Case "GUI" : natcode = "GIN"
            Case "CRO" : natcode = "HRV"
            Case "SUI" : natcode = "CHE"
            Case "DAN" : natcode = "DNK"
            Case "GAM" : natcode = "GMB"
            Case "POR" : natcode = "PRT"
            Case "PAR" : natcode = "PRY"
            Case "NED" : natcode = "NLD"
            Case "GER" : natcode = "DEU"
            Case "CRC" : natcode = "CRI"
            Case "BUL" : natcode = "BGR"
            Case "URU" : natcode = "URY"
            Case "ALG" : natcode = "DZA"
            Case "SLO" : natcode = "SVK"
            Case "CIL" : natcode = "CHL"
            Case "MOL" : natcode = "MDA"
            Case "SVE" : natcode = "SWE"
            Case "SER" : natcode = "SVK"
            Case "SPA" : natcode = "ESP"
            Case "ROM" : natcode = "ROU"
            Case "OLA" : natcode = "NLD"
            Case "COS" : natcode = "CIV"
            Case "SVI" : natcode = "SWZ"
            Case "BOS" : natcode = "BIH"
            Case "GIA" : natcode = "JAM"
            Case "SNV" : natcode = "SVN"
            Case "UNG" : natcode = "HUN"
            Case "LIB" : natcode = "LBY"
            Case "LIT" : natcode = "LTU"
            Case "NIG" : natcode = "NER"
            Case "IRA" : natcode = "IRN"
            Case "MON" : natcode = "MNE"
            Case "EGI" : natcode = "EGY"
            Case "ANG" : natcode = "AGO"
            Case "MES" : natcode = "MEX"
            Case "CAM" : natcode = "CMR"
            Case "MAL" : natcode = "MLI"
            Case "COR" : natcode = "KOR"
            Case "GUA" : natcode = "GLP"
            Case "DEN" : natcode = "DNK"
            Case "KVX" : natcode = "RKS"
        End Select
        Return natcode
    End Function

   Public Shared Sub WriteLog(dirs As String,ByVal Form As String, ByVal SubName As String, ByVal Text As String)
        Try
            If IO.Directory.Exists(dirs) Then IO.File.AppendAllText(dirs & "\debug.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & Form & "|" & SubName & "|" & Text & System.Environment.NewLine)
        Catch ex As Exception

        End Try
    End Sub
	
    Public Shared Sub WriteError(ByVal Form As String, ByVal SubName As String, ByVal ErrMsg As String)
        Try
            If IO.Directory.Exists(dirs) Then IO.File.AppendAllText(dirs & "\error.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & Form & "|" & SubName & "|" & ErrMsg & System.Environment.NewLine)
        Catch ex As Exception

        End Try
    End Sub

    Public Function GetDataPlayerMatchedData(wp As Dictionary(Of String, PlayerMatch), PlayerList As Boolean) As String

        Dim strdata As New System.Text.StringBuilder

        Try


            Dim notfound As New List(Of WebPlayer)

            For Each pkey As String In wp.Keys
                If wp(pkey).Matched = False Then notfound.Add(wp(pkey).SourcePlayer)
            Next

            If PlayerList Then
                strdata.AppendLine("***List players***")
                For Each pkey As String In wp.Keys
                    strdata.AppendLine(wp(pkey).SourcePlayer.Role & "|" & wp(pkey).SourcePlayer.Name & "|" & wp(pkey).SourcePlayer.Team & "|" & wp(pkey).MatchedPlayer.Role & "|" & wp(pkey).MatchedPlayer.Name & "|" & wp(pkey).MatchedPlayer.Team)
                Next
                strdata.AppendLine("")
                strdata.AppendLine("***List players not found***")
                For i As Integer = 0 To notfound.Count - 1
                    strdata.AppendLine(notfound(i).Role & "|" & notfound(i).Name & "|" & notfound(i).Team)
                Next
                strdata.AppendLine("")
            End If

            strdata.AppendLine("***Report data***")
            strdata.AppendLine("Number of players = " & wp.Count)
            strdata.AppendLine("Number of players not found = " & notfound.Count)
            If wp.Count > 0 Then
                strdata.AppendLine("Percentage players not found = " & CInt((notfound.Count * 1000) / wp.Count) / 10 & "%")
            Else
                strdata.AppendLine("Percentage players not found = 0%")
            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function

    Public Shared Sub WriteDataPlayerMatch(wp As Dictionary(Of String, PlayerMatch), filed As String)
        Try

            Dim strdata As New System.Text.StringBuilder
            Dim notfound As New List(Of WebPlayer)

            strdata.AppendLine("***List players***")

            For Each pkey As String In wp.Keys
                strdata.AppendLine(wp(pkey).SourcePlayer.Role & "|" & wp(pkey).SourcePlayer.Name & "|" & wp(pkey).SourcePlayer.Team & "|" & wp(pkey).MatchedPlayer.Role & "|" & wp(pkey).MatchedPlayer.Name & "|" & wp(pkey).MatchedPlayer.Team)
                If wp(pkey).Matched = False Then notfound.Add(wp(pkey).SourcePlayer)
            Next
            strdata.AppendLine("")
            strdata.AppendLine("***List players not found***")
            For i As Integer = 0 To notfound.Count - 1
                strdata.AppendLine(notfound(i).Role & "|" & notfound(i).Name & "|" & notfound(i).Team)
            Next
            strdata.AppendLine("")
            strdata.AppendLine("***Report data***")
            strdata.AppendLine("Number of players = " & wp.Count)
            strdata.AppendLine("Number of players not found = " & notfound.Count)
            If wp.Count > 0 Then
                strdata.AppendLine("Percentage players not found = " & CInt((notfound.Count * 1000) / wp.Count) / 10 & "%")
            Else
                strdata.AppendLine("Percentage players not found = 0%")
            End If

            IO.File.WriteAllText(filed, strdata.ToString, System.Text.Encoding.UTF8)

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Public Shared Function GetMatchYear(ServerPath As String, year As String, Day As String) As String

        Dim strdata As New System.Text.StringBuilder

        Try

            Dim fname As String = ServerPath & "\web\" & year & "\data\matchs\matchs-data.txt"
            Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
            For Each line As String In lines
                If Day = "-1" OrElse line.Split(Convert.ToChar("|"))(0) = Day Then
                    strdata.AppendLine(line)
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function

    Public Shared Function GetMatchDetailsYear(ServerPath As String, year As String, startDay As Integer, endDay As Integer) As String

        Dim strdata As New System.Text.StringBuilder
        Dim years As New Dictionary(Of Integer, Boolean)

        For Each y As Integer In years.Keys
            strdata.AppendLine(y)
        Next

        Try

            For i As Integer = 1 To 38
                If (startDay = -1 OrElse startDay >= startDay) AndAlso (endDay = -1 OrElse endDay >= endDay) Then
                    Dim fname As String = ServerPath & "\web\" & year & "\data\matchs\matchs-detail-data-" & CStr(i) & ".txt"
                    If IO.File.Exists(fname) Then
                        Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                        For Each line As String In lines
                            strdata.AppendLine(line.Replace(vbCrLf, "").Replace(vbCr, ""))
                        Next
                    End If
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            Return ex.Message
        End Try

        Return strdata.ToString()

    End Function

    Public Shared Function GetFormazioniTorneo(ServerPath As String, year As String, gio As String) As String

        Dim strdata As New System.Text.StringBuilder

        Try

            Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\formazioni.txt"
            Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
            For Each line As String In lines
                If gio = "-1" OrElse line.Split(Convert.ToChar("|"))(0) = gio Then
                    strdata.AppendLine(line)
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function

    Public Shared Function GetSvincolatiTorneo(ServerPath As String, year As String, role As String) As String

        Dim strdata As New System.Text.StringBuilder

        Try

            Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\svincolati.txt"
            Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
            For Each line As String In lines
                If role = "Tutti" OrElse line.Split(Convert.ToChar("|"))(2) = role Then
                    strdata.AppendLine(line)
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function


    Public Shared Function GetTeamTorneo(ServerPath As String, year As String) As String

        Dim strdata As New System.Text.StringBuilder

        Try

            Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\team.txt"
            Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
            For Each line As String In lines
                strdata.AppendLine(line)
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function

    Public Shared Function GetRoseTorneo(ServerPath As String, year As String, teamId As String) As String

        Dim strdata As New System.Text.StringBuilder

        Try

            Dim fquota As String = ServerPath & "\web\" & CStr(year) & "\data\players-quote.txt"
            Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\rose.txt"
            Dim quotes As New Dictionary(Of String, String)
            Dim lines As List(Of String)

            lines = IO.File.ReadAllLines(fquota).ToList()

            For Each line As String In lines
                Dim s() As String = line.Split("|")
                If s.Length = 5 Then
                    Dim key As String = s(0) & "|" & s(1) & "|" & s(2)
                    If quotes.ContainsKey(key) = False Then quotes.Add(key, s(4))
                End If
            Next

            lines = IO.File.ReadAllLines(fname).ToList()

            For Each line As String In lines
                Dim s() As String = line.Split("|")
                If s.Length > 5 Then
                    Dim key As String = s(2) & "|" & s(4) & "|" & s(5)
                    If quotes.ContainsKey(key) Then
                        s(9) = quotes(key)
                    End If
                    strdata.AppendLine(ConvertListStringToString(s.ToList(), "|"))
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function

    Public Shared Function ConvertListStringToString(List As List(Of String), Separator As String) As String
        Dim str As String = ""
        For i As Integer = 0 To List.Count - 1
            str = str & Separator & List(i)
        Next
        If str.Length > 0 Then
            Return str.Substring(1)
        Else
            Return ""
        End If
    End Function

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
End Class
