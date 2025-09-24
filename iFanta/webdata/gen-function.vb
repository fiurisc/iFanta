Partial Public Class wData

    Public Shared Function ExtractIntegerData(Value As String, RegEx As String, DefaultValue As Integer) As Integer

        Try
            Dim s As String = System.Text.RegularExpressions.Regex.Match(Value, RegEx).Value
            If s <> "" Then DefaultValue = CInt(s)
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

        Return DefaultValue

    End Function

    Public Shared Function NormalizeText(ByVal txt As String) As String

        If txt.Contains("-") Then
            txt = txt
        End If

        'txt = txt.Replace("-", "")
        txt = txt.Replace("'", "’")
        txt = txt.Replace("&#39;", "’")
        txt = txt.Replace("Ò", "O")
        txt = txt.Replace("&#210;", "O")
        txt = txt.Replace("É", "E’")
        txt = txt.Replace("&#201;", "E’")
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
        txt = txt.Replace("-", " ")

        Dim regex As New System.Text.RegularExpressions.Regex("[A-Z\'\s\.\-\’]{0,}")
        Dim newtxt As String = ""

        For Each match As System.Text.RegularExpressions.Match In regex.Matches(txt)
            If match.Value.Trim <> "" Then newtxt = newtxt & match.Value
        Next

        Return newtxt

    End Function

    Public Shared Function CleanSpecialChar(txt As String) As String
        txt = txt.Replace("’", "")
        txt = txt.Replace(".", "")
        If System.Text.RegularExpressions.Regex.Match(txt, "^\w{2}\s").Success Then
            txt = txt.Replace(System.Text.RegularExpressions.Regex.Match(txt, "^\w{2}\s").Value, System.Text.RegularExpressions.Regex.Match(txt, "^\w{2}\s").Value.Trim)
        End If
        Return txt
    End Function

    Public Function ResolveName(Role As String, Name As String, Team As String, FindAllTeam As Boolean) As String

        If _wplayerkey.ContainsKey(Team) Then

            Name = Name.ToUpper().Trim()
            Name = NormalizeText(Name)
            Name = Name.Replace("AKPAAKPRO", "AKPA AKPRO")

            If Name.Contains("DOUVIKAS") Then
                Name = Name
            End If

            Dim keylist As List(Of String) = GetKeyWordList(CleanSpecialChar(Name))
            Dim macthlist As New SortedDictionary(Of Integer, List(Of String))
            Dim macth As wPlayerKeyMatch = CheckName(_wplayerkey(Team), Team, keylist)

            If macth IsNot Nothing Then
                If macthlist.ContainsKey(macth.KeyLength) = False Then macthlist.Add(macth.KeyLength, New List(Of String))
                macthlist(macth.KeyLength).Add(macth.Name)
            ElseIf FindAllTeam Then
                For Each t As String In _wplayerkey.Keys
                    macth = CheckName(_wplayerkey(t), t, keylist)
                    If macth IsNot Nothing Then
                        If macth.Name = Name Then macth.KeyLength = Name.Length
                        If macthlist.ContainsKey(macth.KeyLength) = False Then macthlist.Add(macth.KeyLength, New List(Of String))
                        macthlist(macth.KeyLength).Add(macth.Name)
                    End If
                Next
            End If

            If macthlist.Keys.Count > 0 Then
                Dim key(macthlist.Keys.Count - 1) As Integer
                macthlist.Keys.CopyTo(key, 0)
                If macthlist(key(key.Length - 1)).Count = 1 Then
                    Name = macthlist(key(key.Length - 1))(0)
                Else
                    Return "#" & Name
                End If
            Else
                Return "#" & Name
            End If

        End If

        Return Name

    End Function

    Public Function CheckName(wk As wPlayerKey, Team As String, keylist As List(Of String)) As wPlayerKeyMatch

        For k As Integer = 0 To keylist.Count - 1
            Dim subkey() As String = keylist(k).Split(CChar("/"))
            Dim macth As wPlayerKey = CheckName(wk, subkey, 0)
            If macth IsNot Nothing Then
                Return New wPlayerKeyMatch(macth.Name, macth.Role, subkey(0).Length)
                Exit For
            End If
        Next

        Return Nothing

    End Function

    Public Function CheckName(wk As wPlayerKey, subkey As String(), ind As Integer) As wPlayerKey

        If wk.key.ContainsKey(subkey(ind)) AndAlso subkey(ind).Length > 1 Then
            If wk.key(subkey(ind)).key.Count = 0 Then
                Return wk.key(subkey(ind))
            ElseIf wk.key(subkey(ind)).key.Count = 1 Then
                Return wk.key(subkey(ind))
            Else
                ind += 1
                If ind < subkey.Length Then
                    Return CheckName(wk.key(subkey(ind - 1)), subkey, ind)
                Else
                    Return Nothing
                End If
            End If
        Else
            If ind > 0 Then
                For Each w As String In wk.key.Keys
                    If w.StartsWith(subkey(ind)) Then
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

    End Function

    'Public Function CheckName(ByVal Nome As String, ByVal Squadra As String, Optional AddFlagFound As Boolean = False) As String
    '    Return CheckName(Nome, Squadra, "", AddFlagFound)
    'End Function

    'Public Function CheckName(ByVal Nome As String, ByVal Squadra As String, ByVal Ruolo As String, Optional AddFlagFound As Boolean = False) As String
    '    Try
    '        If _allplayers.ContainsKey(Squadra) Then

    '            Dim f As Boolean = False
    '            Dim g As New List(Of String)

    '            For Each r As String In _allplayers(Squadra).Keys
    '                If r = Ruolo OrElse Ruolo = "" Then
    '                    g.AddRange(_allplayers(Squadra)(r))
    '                    If Ruolo <> "" Then Exit For
    '                End If
    '            Next

    '            Dim newname As String = CheckName(Nome, g, AddFlagFound)

    '            If newname <> "" Then
    '                Nome = newname
    '            End If

    '        End If
    '    Catch ex As Exception
    '        Call WriteError("wData", "CheckName", ex.Message)
    '    End Try
    '    Return Nome
    'End Function

    'Public Shared Function CheckName(ByVal Nome As String, ByVal NomiUfficiali As List(Of String), AddFlagFound As Boolean) As String

    '    Dim newname As String = ""
    '    Dim f As Boolean = False
    '    Dim s2() As String = Nome.Split(CChar(" "))

    '    Dim name As String = s2(0)
    '    Dim surname As String = ""

    '    If name = "PELLEGRINI" Then
    '        name = name
    '    End If
    '    If s2.Length > 1 Then surname = s2(1)

    '    newname = CheckName(name, surname, NomiUfficiali)

    '    If newname = "" Then newname = CheckName(surname, name, NomiUfficiali)
    '    If newname <> "" Then f = True

    '    If f = False Then
    '        For k As Integer = 0 To NomiUfficiali.Count - 1

    '            Dim s1() As String = NomiUfficiali(k).Replace("-", " ").Split(CChar(" "))

    '            For j As Integer = 0 To s1.Length - 1
    '                If (s1(j).Contains(".") = False AndAlso s1(j).Length > 2) Then
    '                    For d As Integer = 0 To s2.Length - 1
    '                        If s1(j) = s2(d) Then
    '                            newname = NomiUfficiali(k)
    '                            f = True
    '                            Exit For
    '                        Else
    '                            If (s1(j).EndsWith("’") OrElse s2(d).EndsWith("’")) AndAlso s1(j).Replace("’", "") = s2(d).Replace("’", "") Then
    '                                newname = NomiUfficiali(k)
    '                                f = True
    '                                Exit For
    '                            ElseIf s2(d).Length > 10 AndAlso s1(j).Length > s2(d).Length Then
    '                                If s2(d) = s1(j).Substring(0, s2(d).Length) Then
    '                                    newname = NomiUfficiali(k)
    '                                    f = True
    '                                    Exit For
    '                                End If
    '                            End If
    '                        End If
    '                    Next
    '                    If f Then Exit For
    '                End If
    '            Next
    '            If f Then Exit For
    '        Next
    '    End If

    '    If AddFlagFound Then If f Then newname = "$f-" & newname

    '    Return newname

    'End Function

    'Private Shared Function CheckName(ByVal Name As String, ByVal Surname As String, ByVal NomiUfficiali As List(Of String)) As String

    '    Dim newname As String = ""
    '    Dim samenane As New Dictionary(Of Integer, List(Of String))

    '    'Determino le persone che hanno lo stesso nome e creo una lista degli stessi 
    '    For k As Integer = 0 To NomiUfficiali.Count - 1

    '        Dim s1() As String = NomiUfficiali(k).Replace(".", "").Replace("-", " ").Split(CChar(" "))
    '        Dim na As String = s1(0)

    '        If na.Replace("'", "").Replace(".", "") = Name.Replace("'", "").Replace(".", "") Then

    '            Dim len As Integer = Surname.Length
    '            If samenane.ContainsKey(len) = False Then samenane.Add(len, New List(Of String))
    '            samenane(Surname.Length).Add(NomiUfficiali(k))

    '        End If

    '    Next

    '    If samenane.Count > 1 Then
    '        samenane = samenane
    '    End If

    '    If samenane.Count > 0 Then

    '        Dim key(samenane.Count - 1) As Integer
    '        samenane.Keys.CopyTo(key, 0)

    '        If Surname <> "" Then

    '            'Per tutti i giocacatori con nomi coincidenti, verifico l'uguaglianza del cognome totale o parziale partendo dalla lunghezza
    '            'intera dello stesso e man man diminuisco il numero di caratteri sino ad verficare la sola iniziale'

    '            Array.Sort(key) 'Ordino la chiave secondo la lunghezza del cognome'

    '            For k As Integer = key.Length - 1 To 0 Step -1

    '                For h As Integer = 0 To samenane(key(k)).Count - 1

    '                    Dim s1() As String = samenane(key(k))(h).Replace("-", " ").Split(CChar(" "))
    '                    Dim sn As String = s1(1)

    '                    For j As Integer = sn.Length - 1 To 1 Step -1
    '                        If j < Surname.Length AndAlso sn.Substring(0, j) = Surname.Substring(0, j) Then
    '                            newname = samenane(key(k))(h)
    '                            Exit For
    '                        End If
    '                    Next j
    '                    If newname <> "" Then
    '                        Exit For
    '                    End If
    '                Next

    '            Next

    '        Else

    '            newname = samenane(key(0))(0)

    '        End If

    '    End If


    '    Return newname

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

    Public Shared Function CheckTeamName(ByVal Squadra As String) As String
        Squadra = Squadra.ToUpper.Replace("CHIEVOVERONA", "CHIEVO").Trim
        Squadra = Squadra.ToUpper.Replace("CHIEVO-VERONA", "CHIEVO").Trim
        Squadra = Squadra.ToUpper.Replace("CHIEVO VERONA", "CHIEVO").Trim
        Squadra = Squadra.ToUpper.Replace("HELLASVERONA", "VERONA").Trim
        Squadra = Squadra.ToUpper.Replace("HELLAS-VERONA", "VERONA").Trim
        Squadra = Squadra.ToUpper.Replace("HELLAS VERONA", "VERONA").Trim
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

    Public Shared Sub WriteError(ByVal Form As String, ByVal SubName As String, ByVal ErrMsg As String)
        Try
            IO.File.AppendAllText(My.Application.Info.DirectoryPath & "\error.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & Form & "|" & SubName & "|" & ErrMsg & System.Environment.NewLine)
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Function GetKeyWordList(Name As String) As List(Of String)

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

    Public Sub LoadWebPlayersKey()
        For Each t As String In webdata.AllPlayers().Keys
            For Each r As String In webdata.AllPlayers(t).Keys
                For Each n As String In webdata.AllPlayers(t)(r)

                    If _wplayerkey.ContainsKey(t) = False Then _wplayerkey.Add(t, New wPlayerKey)

                    Dim keylist As List(Of String) = GetKeyWordList(CleanSpecialChar(n))

                    For k As Integer = 0 To keylist.Count - 1
                        Dim subkey() As String = keylist(k).Split(CChar("/"))
                        Call AddwPlayerWordKey(_wplayerkey(t), subkey, 0, n, r)
                    Next
                Next
            Next
        Next

    End Sub

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
        Public Property Role As String = ""
        Public Property KeyLength As Integer = 0

        Sub New()

        End Sub

        Sub New(Name As String, Role As String, KeyLength As Integer)
            Me.Name = Name
            Me.Role = Role
            Me.KeyLength = KeyLength
        End Sub

    End Class
End Class