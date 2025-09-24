Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase
Imports System.IO
Imports System.Text
Imports System.Net

Partial Public Class wData

    Sub UpdatePlayerTbFromView(StartUp As Boolean)
        Try

            Dim exi As Boolean = False
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT name FROM sqlite_master WHERE type='table' AND name='tbstat';", conn)

            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then exi = True

            If exi = False OrElse StartUp = False Then
                ExecuteSql("delete from tbstat;", conn)
                ExecuteSql("insert into tbstat select * from stat", conn)
                'ExecuteSql("CREATE TABLE tbstat_new AS SELECT * FROM stat;", conn)
                'If exi Then ExecuteSql("ALTER TABLE tbstat RENAME TO tbstat_old;", conn)
                'ExecuteSql("ALTER TABLE tbstat_new RENAME TO tbstat;", conn)
                'If exi Then ExecuteSql("DROP TABLE IF EXISTS tbstat_old;", conn)
            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Private Sub DetectDayProbableFormation()
        Try
            'Elimino i dati precedenti'
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT * FROM tbmatch;", Conn)

            If ds.Tables.Count > 0 Then

                Dim _dicggdt As New Dictionary(Of Integer, List(Of Date)) 'dictionary date giornate'

                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                    Dim gg As Integer = CInt(ds.Tables(0).Rows(i).Item("gio"))
                    Dim dp As Date = CDate(ds.Tables(0).Rows(i).Item("timem")).Date
                   
                    If _dicggdt.ContainsKey(gg) = False Then _dicggdt.Add(gg, New List(Of Date))
                    If _dicggdt(gg).Contains(dp) = False Then _dicggdt(gg).Add(dp)

                Next

                'Rimuovo dalle singole giornate le partite che si discostano piu' di un giorno dalle altre perche' con molta
                'probabilita' sono state rinviate'
                For Each gg As Integer In _dicggdt.Keys
                    _dicggdt(gg).Sort()
                    For i As Integer = _dicggdt(gg).Count - 1 To 1 Step -1
                        If _dicggdt(gg)(i).Subtract(_dicggdt(gg)(i - 1)).TotalDays > 1 Then
                            _dicggdt(gg).RemoveAt(i)
                        End If
                    Next
                Next

                'Determino la giornata corrente'
                Me.DayProbableFormation = -1

                Dim k(_dicggdt.Keys.Count - 1) As Integer
                _dicggdt.Keys.CopyTo(k, 0)
                Array.Sort(k)
                For toll As Integer = 0 To 3
                    For Each gg As Integer In k
                        If _dicggdt.ContainsKey(gg + 1) Then
                            If _dicggdt(gg + 1).Count = 1 Then
                                _dicggdt(gg + 1).Insert(0, _dicggdt(gg)(_dicggdt(gg).Count - 1).AddDays(-1))
                            Else
                                _dicggdt(gg + 1)(0) = _dicggdt(gg)(_dicggdt(gg).Count - 1).AddDays(-1)
                            End If
                            If _dicggdt(gg)(1).AddDays(toll) > Date.Now AndAlso _dicggdt(gg)(0).AddDays(-toll) < Date.Now Then
                                Me.DayProbableFormation = gg
                                Exit For
                            End If
                        End If
                    Next
                    If Me.DayProbableFormation <> -1 Then Exit For
                Next
                If Me.DayProbableFormation = -1 Then Me.DayProbableFormation = 1

            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Private Function LoadMatchData(FileName As String) As Boolean

        Dim up As Boolean = False

        Try

            '---------------------------------------------------------------------------------------------'
            'La funzione recura da file il calendario della partite della serie a, con le relative data   '
            'oltre a questo determina sulla base delle date dele partite la giornata corrente, giornata   '
            'giornata utilizzata assegnare le probabli formazioni                                         '   
            '---------------------------------------------------------------------------------------------'

            If IO.File.Exists(FileName) Then

                'Elimino i dati precedenti'
                ExecuteSql("DELETE FROM tbmatch WHERE gio>100 or gio<1;", conn)

                Dim dicday As New List(Of Integer)
                Dim dicdatiold As New Dictionary(Of Integer, List(Of String))
                Dim dicdatinew As New Dictionary(Of Integer, List(Of String))

                'Carico i dati vecchi per poi confrontarli con i nuovi'
                Dim ds As DataSet = ExecuteSqlReturnDataSet("select gio,idmatch || '|'|| teama || '|'|| teamb || '|'|| strftime('%Y-%m-%d %H:%M:%S',timem) as word from tbmatch;", conn)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim day As Integer = CInt(ds.Tables(0).Rows(i).Item(0))
                        Dim word As String = ds.Tables(0).Rows(i).Item(1).ToString

                        If dicdatiold.ContainsKey(day) = False Then dicdatiold.Add(day, New List(Of String))
                        dicdatiold(day).Add(word)

                    Next
                End If

                'Carico i nuovi dati'

                Dim lines() As String = IO.File.ReadAllLines(FileName)

                If lines.Length > 0 Then

                    For i As Integer = 0 To lines.Length - 1

                        Dim s() As String = lines(i).Split(CChar("|"))

                        If s.Length = 5 Then

                            If s(4).Length < 15 Then s(4) = s(4) & " 00:00:00"

                            Dim day As Integer = CInt(s(0))
                            Dim word As String = s(1) & "|" & s(2) & "|" & s(3) & "|" & s(4)

                            If dicdatinew.ContainsKey(day) = False Then dicdatinew.Add(day, New List(Of String))
                            dicdatinew(day).Add(word)

                        End If

                    Next

                End If

                'Confronto i dati e inserisco gli aggiornamenti'
                Dim sins As String = "INSERT INTO tbmatch (gio,idmatch,teama,teamb,timem) VALUES "
                Dim r As Integer = 0
                Dim str As New System.Text.StringBuilder

                For Each Day As Integer In dicdatinew.Keys

                    Dim add As Boolean = False

                    If dicdatiold.ContainsKey(Day) = False Then
                        add = True
                    Else
                        For i As Integer = 0 To dicdatinew(Day).Count - 1
                            If dicdatiold(Day).Contains(dicdatinew(Day)(i)) = False Then
                                add = True
                                Exit For
                            Else
                                dicdatiold(Day).Remove(dicdatinew(Day)(i))
                            End If
                        Next
                        If add = False AndAlso dicdatiold(Day).Count > 0 Then
                            add = True
                        End If
                    End If

                    If add Then

                        dicday.Add(Day)
                        up = True

                        For i As Integer = 0 To dicdatinew(Day).Count - 1

                            Dim s() As String = dicdatinew(Day)(i).Split(CChar("|"))

                            If s.Length = 4 Then

                                Dim idmatch As Integer = CInt(s(0))
                                Dim teama As String = s(1)
                                Dim teamb As String = s(2)
                                Dim times As String = s(3)

                                Dim g() As String = times.Split(CChar(" "))
                                Dim dtp As Date = Date.Now 'data e ora partita'

                                If g.Length = 2 Then
                                    Dim d() As String = g(0).Split(CChar("/")) 'Date'
                                    Dim t() As String = g(1).Split(CChar(":")) 'Time'
                                    dtp = New Date(CInt(d(0)), CInt(d(1)), CInt(d(2)), CInt(t(0)), CInt(t(1)), 0)
                                ElseIf g.Length = 1 Then
                                    Dim d() As String = g(0).Split(CChar("/")) 'Date'
                                    dtp = New Date(CInt(d(0)), CInt(d(1)), CInt(d(1)), 0, 0, 0)
                                Else
                                    dtp = Nothing
                                End If

                                If dtp <> Nothing Then

                                    'Aggiungo i dati alla tabella'
                                    str.AppendLine(",(" & Day + 100 & "," & idmatch & ",'" & teama & "','" & teamb & "','" & dtp.ToString("yyyy-MM-dd HH:mm:ss") & "')")
                                    r += 1

                                    If r > blkrec Then
                                        Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                        str = New System.Text.StringBuilder
                                        r = 0
                                        System.Threading.Thread.Sleep(30)
                                    End If
                                End If

                            End If

                        Next

                    End If

                Next

                If r > 0 Then
                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                End If

                For Each day As Integer In dicdatiold.Keys
                    If dicdatinew.ContainsKey(day) = False Then
                        dicday.Add(day)
                    End If
                Next

                If up Then
                    ExecuteSql("DELETE FROM tbmatch WHERE gio <100 AND gio in (" & SystemFunction.Convertion.ConvertListIntegerToString(dicday, ",") & ");", conn)
                    ExecuteSql("UPDATE tbmatch SET gio=gio-100 WHERE gio>100;", conn)
                End If

            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return up

    End Function

    Private Function LoadMatchDetail(FileName As String) As Boolean

        Dim up As Boolean = False

        '---------------------------------------------------------------------------------------------'
        'La funzione aggiorna i dati dei minuti giocati dai vari giocatori                            '
        '---------------------------------------------------------------------------------------------'

        Try


            'Elimino i dati precedenti'
            ExecuteSql("DELETE FROM tbtabellini WHERE nome like 'T-%';", conn)

            If IO.File.Exists(FileName) Then

                Dim dicday As New List(Of Integer)
                Dim dicdatiold As New Dictionary(Of Integer, List(Of String))
                Dim dicdatinew As New Dictionary(Of Integer, List(Of String))

                'Carico i dati vecchi per poi confrontarli con i nuovi'
                Dim ds As DataSet = ExecuteSqlReturnDataSet("select gio,nome || '|'|| squadra || '|'|| mm || '|'|| tit || '|'|| sos || '|'|| sub as word from tbtabellini;", conn)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim day As Integer = CInt(ds.Tables(0).Rows(i).Item(0))
                        Dim word As String = ds.Tables(0).Rows(i).Item(1).ToString

                        If dicdatiold.ContainsKey(day) = False Then dicdatiold.Add(day, New List(Of String))
                        dicdatiold(day).Add(word)

                    Next
                End If

                'Carico i nuovi dati'

                Dim lines() As String = IO.File.ReadAllLines(FileName)

                If lines.Length > 0 Then

                    For i As Integer = 0 To lines.Length - 1

                        Dim s() As String = lines(i).Split(CChar("|"))

                        If s.Length = 8 Then

                            Dim day As Integer = CInt(s(0))
                            Dim word As String = s(3) & "|" & s(2) & "|" & s(4) & "|" & s(5) & "|" & s(6) & "|" & s(7)

                            If dicdatinew.ContainsKey(day) = False Then dicdatinew.Add(day, New List(Of String))
                            dicdatinew(day).Add(word)

                        End If

                    Next

                End If

                'Confronto i dati e inserisco gli aggiornamenti'
                Dim sins As String = "INSERT INTO tbtabellini (gio,nome,squadra,mm,tit,sos,sub) VALUES "
                Dim r As Integer = 0
                Dim str As New System.Text.StringBuilder

                For Each Day As Integer In dicdatinew.Keys

                    Dim add As Boolean = False

                    If dicdatiold.ContainsKey(Day) = False Then
                        add = True
                    Else
                        For i As Integer = 0 To dicdatinew(Day).Count - 1
                            If dicdatiold(Day).Contains(dicdatinew(Day)(i)) = False Then
                                add = True
                                Exit For
                            End If
                        Next
                    End If

                    If add Then

                        dicday.Add(Day)
                        up = True

                        For i As Integer = 0 To dicdatinew(Day).Count - 1

                            Dim s() As String = dicdatinew(Day)(i).Split(CChar("|"))

                            If s.Length = 6 Then

                                Dim nome As String = s(0)
                                Dim squadra As String = s(1)
                                Dim mm As Integer = CInt(s(2))
                                Dim tit As Integer = CInt(s(3))
                                Dim so As Integer = CInt(s(4))
                                Dim su As Integer = CInt(s(5))

                                str.AppendLine(",(" & Day & ",'T-" & nome & "','" & squadra & "'," & mm & "," & tit & "," & so & "," & su & ")")
                                r += 1

                                If r > blkrec Then
                                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                    str = New System.Text.StringBuilder
                                    r = 0
                                    System.Threading.Thread.Sleep(30)
                                End If

                            End If

                        Next

                    End If

                Next

                If r > 0 Then
                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                End If

                For Each day As Integer In dicdatiold.Keys
                    If dicdatinew.ContainsKey(day) = False Then
                        dicday.Add(day)
                    End If
                Next

                If up Then
                    ExecuteSql("DELETE FROM tbtabellini WHERE nome not like 'T-%' AND gio in (" & SystemFunction.Convertion.ConvertListIntegerToString(dicday, ",") & ");", conn)
                    ExecuteSql("UPDATE tbtabellini SET nome=substr(nome,3) WHERE nome like 'T-%';", conn)
                End If

            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return up

    End Function

    Private Sub LoadRankingData(FileName As String)
        Try

            If IO.File.Exists(FileName) Then

                _webteams.Clear()

                Dim lines() As String = IO.File.ReadAllLines(FileName)

                For i As Integer = 0 To lines.Length - 1

                    Dim s() As String = lines(i).Split(CChar("|"))
                    Dim t As New wTeam()

                    For k As Integer = 0 To s.Length - 1

                        Dim d() As String = s(k).Split(CChar("="))
                        If d.Length = 2 Then
                            Select Case d(0)
                                Case "name" : t.Name = d(1)
                                Case "pt_t" : t.Punti = CInt(d(1))
                                Case "pt_d" : t.Punti_Dentro = CInt(d(1))
                                Case "pt_f" : t.Punti_Fuori = CInt(d(1))
                                Case "pg_t" : t.PartiteGiocate = CInt(d(1))
                                Case "pg_d" : t.PartiteGiocate_Dentro = CInt(d(1))
                                Case "pg_f" : t.PartiteGiocate_Fuori = CInt(d(1))
                                Case "vit_t" : t.Vittorie = CInt(d(1))
                                Case "vit_d" : t.Vittorie_Dentro = CInt(d(1))
                                Case "vit_f" : t.Vittorie_Fuori = CInt(d(1))
                                Case "par_t" : t.Pareggi = CInt(d(1))
                                Case "par_d" : t.Pareggi_Dentro = CInt(d(1))
                                Case "par_f" : t.Pareggi_Fuori = CInt(d(1))
                                Case "per_t" : t.Sconfitte = CInt(d(1))
                                Case "per_d" : t.Sconfitte_Dentro = CInt(d(1))
                                Case "per_f" : t.Sconfitte_Fuori = CInt(d(1))
                                Case "gf_t" : t.GoalFatti = CInt(d(1))
                                Case "gf_d" : t.GoalFatti_Dentro = CInt(d(1))
                                Case "gf_f" : t.GoalFatti_Fuori = CInt(d(1))
                                Case "gs_t" : t.GoalSubiti = CInt(d(1))
                                Case "gs_d" : t.GoalSubiti_Dentro = CInt(d(1))
                                Case "gs_f" : t.GoalSubiti_Fuori = CInt(d(1))

                            End Select
                        End If
                    Next
                    If _webteams.ContainsKey(t.Name) = False Then _webteams.Add(t.Name, t)
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Private Function LoadPlayersData(FileName As String) As Boolean

        Dim up As Boolean = False

        Try

            'Elimino i dati precedenti'
            ExecuteSql("DELETE FROM tbplayer_data WHERE nome like 'T-%';", conn)

            If IO.File.Exists(FileName) Then

                Dim dickey As New List(Of String)
                Dim dicdatiold As New Dictionary(Of String, List(Of String))
                Dim dicdatinew As New Dictionary(Of String, List(Of String))
                Dim datiup As New List(Of String)

                'Carico i dati vecchi per poi confrontarli con i nuovi'
                Dim ds As DataSet = ExecuteSqlReturnDataSet("select squadra,ruolo || '|'|| nome || '|'|| squadra || '|'|| natcode as word from tbplayer_data;", conn)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim key As String = ds.Tables(0).Rows(i).Item(0).ToString
                        Dim word As String = ds.Tables(0).Rows(i).Item(1).ToString

                        If dicdatiold.ContainsKey(key) = False Then dicdatiold.Add(key, New List(Of String))
                        dicdatiold(key).Add(word)

                    Next
                End If

                'Carico i nuovi dati'

                Dim lines() As String = IO.File.ReadAllLines(FileName)

                If lines.Length > 0 Then

                    For i As Integer = 0 To lines.Length - 1

                        Dim s() As String = lines(i).Split(CChar("|"))

                        If s.Length = 5 Then

                            Dim key As String = s(2)
                            Dim word As String = s(0) & "|" & s(1) & "|" & s(2) & "|" & s(4)

                            If dicdatinew.ContainsKey(key) = False Then dicdatinew.Add(key, New List(Of String))
                            dicdatinew(key).Add(word)

                        End If

                    Next

                End If

                'Confronto i dati'
                For Each key As String In dicdatinew.Keys

                    Dim add As Boolean = False

                    If dicdatiold.ContainsKey(key) = False Then
                        add = True
                    Else
                        For i As Integer = 0 To dicdatinew(key).Count - 1
                            If dicdatiold(key).Contains(dicdatinew(key)(i)) = False Then
                                add = True
                                Exit For
                            Else
                                dicdatiold(key).Remove(dicdatinew(key)(i))
                            End If
                        Next
                        If add = False AndAlso dicdatiold(key).Count > 0 Then
                            add = True
                        End If
                    End If

                    If add Then
                        up = True
                        dickey.Add(key)
                        For i As Integer = 0 To dicdatinew(key).Count - 1
                            datiup.Add(dicdatinew(key)(i))
                        Next
                    End If
                Next

                For Each key As String In dicdatiold.Keys
                    If dicdatinew.ContainsKey(key) = False Then
                        dickey.Add(key)
                    End If
                Next

                'Inserisco gli aggiornamenti'

                If datiup.Count > 0 OrElse dickey.Count > 0 Then

                    Dim sins As String = "INSERT INTO tbplayer_data (ruolo,nome,squadra,natcode) VALUES "
                    Dim r As Integer = 0
                    Dim str As New System.Text.StringBuilder

                    For i As Integer = 0 To datiup.Count - 1

                        Dim s() As String = datiup(i).Split(CChar("|"))

                        If s.Length = 4 Then

                            Dim ruolo As String = s(0)
                            Dim nome As String = s(1)
                            Dim squadra As String = s(2)
                            Dim natcode As String = s(3)

                            str.AppendLine(",('" & ruolo & "','T-" & nome & "','" & squadra & "','" & natcode & "')")
                            r += 1

                            If r > blkrec Then
                                Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                str = New System.Text.StringBuilder
                                r = 0
                                System.Threading.Thread.Sleep(30)
                            End If

                        End If
                    Next

                    If r > 0 Then
                        Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                    End If

                    ExecuteSql("DELETE FROM tbplayer_data WHERE nome not like 'T-%' AND squadra in (" & SystemFunction.Convertion.ConvertListStringToString(dickey, ",", "'") & ");", conn)
                    ExecuteSql("UPDATE tbplayer_data SET nome=substr(nome,3) WHERE nome like 'T-%';", conn)

                End If

            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return up

    End Function

    Private Function LoadPlayersQuote(FileName As String) As Boolean

        Dim up As Boolean = False

        Try

            'Elimino i dati precedenti'
            ExecuteSql("DELETE FROM tbplayer WHERE nome like 'T-%';", conn)

            If IO.File.Exists(FileName) Then

                Dim dickey As New List(Of String)
                Dim dicdatiold As New Dictionary(Of String, List(Of String))
                Dim dicdatinew As New Dictionary(Of String, List(Of String))
                Dim datiup As New List(Of String)

                'Carico i dati vecchi per poi confrontarli con i nuovi'
                Dim ds As DataSet = ExecuteSqlReturnDataSet("select squadra,ruolo || '|'|| nome || '|'|| squadra || '|'|| qini || '|'|| qcur as word from tbplayer;", conn)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim key As String = ds.Tables(0).Rows(i).Item(0).ToString
                        Dim word As String = ds.Tables(0).Rows(i).Item(1).ToString

                        If dicdatiold.ContainsKey(key) = False Then dicdatiold.Add(key, New List(Of String))
                        dicdatiold(key).Add(word)

                    Next
                End If

                'Carico i nuovi dati'

                Dim lines() As String = IO.File.ReadAllLines(FileName)

                If lines.Length > 0 Then

                    For i As Integer = 0 To lines.Length - 1

                        Dim s() As String = lines(i).Split(CChar("|"))

                        If s.Length = 5 Then

                            Dim key As String = s(2)
                            Dim word As String = s(0) & "|" & s(1) & "|" & s(2) & "|" & s(3) & "|" & s(4)

                            If dicdatinew.ContainsKey(key) = False Then dicdatinew.Add(key, New List(Of String))
                            dicdatinew(key).Add(word)

                        End If

                    Next

                End If

                'Confronto i dati'
                For Each key As String In dicdatinew.Keys

                    Dim add As Boolean = False

                    If dicdatiold.ContainsKey(key) = False Then
                        add = True
                    Else
                        For i As Integer = 0 To dicdatinew(key).Count - 1
                            If dicdatiold(key).Contains(dicdatinew(key)(i)) = False Then
                                add = True
                                Exit For
                            Else
                                dicdatiold(key).Remove(dicdatinew(key)(i))
                            End If
                        Next
                        If add = False AndAlso dicdatiold(key).Count > 0 Then
                            add = True
                        End If
                    End If

                    If add Then
                        up = True
                        dickey.Add(key)
                        For i As Integer = 0 To dicdatinew(key).Count - 1
                            datiup.Add(dicdatinew(key)(i))
                        Next
                    End If
                Next

                For Each key As String In dicdatiold.Keys
                    If dicdatinew.ContainsKey(key) = False Then
                        dickey.Add(key)
                    End If
                Next

                'Inserisco gli aggiornamenti'

                Dim sins As String = "INSERT INTO tbplayer (ruolo,nome,squadra,qini,qcur) VALUES "
                Dim r As Integer = 0
                Dim str As New System.Text.StringBuilder

                If datiup.Count > 0 OrElse dickey.Count > 0 Then

                    For i As Integer = 0 To datiup.Count - 1

                        Dim s() As String = datiup(i).Split(CChar("|"))

                        If s.Length = 5 AndAlso datiup(i).Contains("RUOLO|NOME") = False Then

                            Dim ruolo As String = s(0)
                            Dim nome As String = s(1)
                            Dim squadra As String = s(2)
                            Dim qini As String = s(3)
                            Dim qcur As String = s(4)

                            str.AppendLine(",('" & ruolo & "','T-" & nome.Replace("'", "’") & "','" & squadra & "'," & qini & "," & qcur & ")")
                            r += 1

                            If r > blkrec Then
                                Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                str = New System.Text.StringBuilder
                                r = 0
                                System.Threading.Thread.Sleep(30)
                            End If

                        End If
                    Next

                    If r > 0 Then
                        Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                    End If

                    ExecuteSql("DELETE FROM tbplayer WHERE nome not like 'T-%' AND squadra in (" & SystemFunction.Convertion.ConvertListStringToString(dickey, ",", "'") & ");", conn)
                    ExecuteSql("UPDATE tbplayer SET nome=substr(nome,3) WHERE nome like 'T-%';", conn)

                End If

            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return up

    End Function

    Private Sub LoadProbableLineUpsData(FileName As String)
        Try
            If IO.File.Exists(FileName) Then

                Dim line() As String = IO.File.ReadAllLines(FileName)

                For i As Integer = 0 To line.Length - 1

                    Dim s() As String = line(i).Split(CChar("|"))

                    If s.Length = 7 Then

                        Dim d() As String = s(0).Split(CChar("/"))

                        If d.Length = 3 Then

                            Dim key As String = d(0) & "/" & d(1) & "/" & d(2)
                            Dim name As String = s(1)
                            Dim team As String = s(2)
                            Dim site As String = s(3)
                            Dim state As String = s(4)
                            Dim perc As Integer = CInt(s(5))
                            Dim info As String = s(6)

                            If _webplayers.ContainsKey(key) = False Then
                                _webplayers.Add(key, New wPlayer(CInt(d(0)), name, team, site, state, perc, info))
                            Else
                                _webplayers(key).AddInfo(site, state, perc, info)
                            End If
                        End If
                    End If
                Next
                _numsiteplayer = _numsiteplayer + 1
            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Sub CompileWebTeamPlayers()
        Try

            Dim dic As Dictionary(Of String, String) = currlega.GetDictionaryAllPlayerListAndRoule(Nothing)

            _webteamplayers.Clear()

            For Each s As String In _webplayers.Keys
                For i As Integer = 0 To _webplayers(s).Info.Count - 1
                    Dim keyt As String = _webplayers(s).Giornata & "-" & _webplayers(s).Team.ToLower & "-" & _webplayers(s).Info(i).Site.ToLower
                    Dim keyr As String = _webplayers(s).Team.ToLower & "-" & _webplayers(s).Name.ToLower
                    If _webteamplayers.ContainsKey(keyt) = False Then _webteamplayers.Add(keyt, New wTeamPlayers)
                    Select Case _webplayers(s).Info(i).State.ToLower
                        Case "titolare"
                            Dim r As String = "?"
                            If dic.ContainsKey(keyr) Then
                                r = dic(keyr)
                            End If
                            WebTeamPlayers(keyt).Titolari.Add(New wData.wTeamPlayers.wTeamPlayer(r, _webplayers(s).Name))
                        Case "panchina", "bassa", "a disposizione"
                            Dim r As String = "?"
                            If dic.ContainsKey(keyr) Then
                                r = dic(keyr)
                            End If
                            WebTeamPlayers(keyt).Panchinari.Add(New wData.wTeamPlayers.wTeamPlayer(r, _webplayers(s).Name))
                    End Select
                Next
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub
End Class
