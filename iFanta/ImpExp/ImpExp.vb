Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase

Public Class ImpExp

    Public Shared Sub ShowCompleated(ByVal Type As String, ByVal FileName As String)
        If iControl.iMsgBox.ShowMessage("Esportazione " & Type & " completata!" & System.Environment.NewLine & "Vuoi aprire la cartella?", "Avviso", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgInfo) = DialogResult.Yes Then
            Process.Start("explorer.exe", "/select," & FileName)
        End If
    End Sub

    Shared Function GetLegenda(ByVal Type As String) As String

        Dim str As New System.Text.StringBuilder
        Dim s As New List(Of String)
        Dim efr As Integer = 1

        Select Case Type
            Case "rating"
                s.Add("Pos.|Posizione in classifica")
                s.Add("Nome|Nome team e allenatore")
                s.Add("pt|Punti totali")
                s.Add("pt g|Punti giornata")
                s.Add("diff1|Differenza punti dal primo")
                s.Add("diff2|Differenza punti dalla squadra che precede in classifica")
                s.Add("med.|Media punti")
                s.Add("min|Punteggio minimo realizzato")
                s.Add("max|Punteggio massimo realizzato")
                s.Add("amm|Ammonizioni (giocatori schierati)")
                s.Add("esp|Espulsioni (giocatori schierati)")
                s.Add("ass|Assist totali (giocatori schierati)")
                s.Add("gs|Goal subiti (giocatori schierati)")
                s.Add("gf|Goal fatti (giocatori schierati)")
                s.Add("p.p.|Punti persi rispetto alla squadra top player")
                s.Add("% p.p.|Percentuale punti persi rispetto alla squadra top player")
                s.Add("pt b.|Totale punti da bonus")
                s.Add("n&#176vitt|Numero vittorie di giornata")
                s.Add("n&#176<11|Numero giocate con meno di 11 giocatori")
                s.Add("n&#176(*)|Numero giocatori jolly utilizzati")
                s.Add("fm|Punti fantamister")
                s.Add("*|In giallo la squadra vincente della giornata")
            Case "statistic"
                s.Add("R.|Ruolo giocatore")
                s.Add("Nome|Nome giocatore")
                s.Add("Squadra|Nome squadra")
                s.Add("Costo|Prezzo d'asta")
                s.Add("Q.A.|Quotazione giocatore attuale")
                s.Add("amm|Ammonizioni (giocatori schierati)")
                s.Add("esp|Espulsioni (giocatori schierati)")
                s.Add("ass|Assist (giocatori schierati)")
                s.Add("gs|Goal subiti (giocatori schierati)")
                s.Add("gf|Goal fatti (giocatori schierati)")
                s.Add("rigt|Rigori tirati")
                s.Add("rigs|Rigori sbagliati")
                s.Add("rigp|Rigori parati")
                s.Add("p.g.|Numero partite giocate")
                s.Add("p.g.|Minuti giocati nelle ultime 3 giornate")
                s.Add("p.g.|Numero partite giocate nelle ultime 3 giornate")
                s.Add("m.v.|Media voto")
                s.Add("sd.vt.|Deviazione standard media voto")
                s.Add("m.v.|Media punti")
                s.Add("sd.vt.|Deviazione standard media punti")
            Case "rose"
                s.Add("R.|Ruolo giocatore")
                s.Add("Nome|Nome giocatore")
                s.Add("Squadra|Nome squadra")
                s.Add("Cos.|Prezzo d'asta del giocatore")
                s.Add("Q.I.|Quotazione giocatore di inizio campionato")
                s.Add("Q.A.|Quotazione giocatore attuale")
                s.Add("Diff.|Differenza tra la quotazione attuale e quella iniziale")
            Case "forma"
                s.Add("R.|Ruolo giocatore")
                s.Add("Nome|Nome giocatore")
                s.Add("vt|Voto")
                s.Add("amm|Eventuale ammonizione")
                s.Add("esp|Eventuale espulsione")
                s.Add("ass|Numero assist effettuati")
                s.Add("gs/gf|Numero goal subiti e fatti")
                s.Add("rigs/p|Numero rigori sbagliati o parati")
                s.Add("pt|Punti totali giocatore")
        End Select
        str.Append("<b>LEGENDA:</b>" & System.Environment.NewLine)
        str.Append("<table style='Color:#505050;font-size:11px;font-family:Arial;font-weight:normal;marging:10px'>" & System.Environment.NewLine)
        For i As Integer = 0 To s.Count - 1 Step efr
            str.Append("<tr>" & System.Environment.NewLine)
            For k As Integer = 0 To efr - 1
                Dim f() As String = s(i).Split(CChar("|"))
                str.Append("<td style='color:red;'>" & f(0) & "</td><td>" & f(1) & "</td>" & System.Environment.NewLine)
            Next
            str.Append("</tr>" & System.Environment.NewLine)
        Next
        str.Append("</table>" & System.Environment.NewLine)
        Return str.ToString
    End Function

    Private Shared Function GetHeaderTableStatistic() As String
        Dim str As New System.Text.StringBuilder
        str.AppendLine("              <td align='center' width='15px' title='Ruolo giocatore'>R.</td>")
        str.AppendLine("              <td align='center' title='Nome giocatore'>Nome</td>")
        str.AppendLine("              <td align='center' width='110px' title='Nome squadra'>Squadra</td>")
        str.AppendLine("              <td align='center' width='30px' title='Prezzo d'asta'>Costo</td>")
        str.AppendLine("              <td align='center' width='30px' title='Quotazione giocatore attuale'>Q.A.</td>")
        str.AppendLine("              <td align='center' width='27px' title='Ammonizioni (giocatori schierati)'>amm</td>")
        str.AppendLine("              <td align='center' width='27px' title='Espulsioni (giocatori schierati)'>esp</td>")
        str.AppendLine("              <td align='center' width='27px' title='Assist (giocatori schierati)'>ass</td>")
        str.AppendLine("              <td align='center' width='27px' title='Goal subiti (giocatori schierati)'>gs</td>")
        str.AppendLine("              <td align='center' width='27px' title='Goal fatti (giocatori schierati)'>gf</td>")
        str.AppendLine("              <td align='center' width='27px' title='Rigori tirati'>rigt</td>")
        str.AppendLine("              <td align='center' width='27px' title='Rigori sbagliati'>rigs</td>")
        str.AppendLine("              <td align='center' width='27px' title='Rigori parati'>rigp</td>")
        str.AppendLine("              <td align='center' width='27px' title='Numero partite giocate'>p.g.</td>")
        str.AppendLine("              <td align='center' width='27px' title='Numero partite giocate da titolare'>p.tit</td>")
        str.AppendLine("              <td align='center' width='27px' title='Numero subentri'>p.sub</td>")
        str.AppendLine("              <td align='center' width='27px' title='Goal subiti nelle ultime 5 giornate'>gs</td>")
        str.AppendLine("              <td align='center' width='27px' title='Goal fatti nelle ultime 5 giornate'>gf</td>")
        str.AppendLine("              <td align='center' width='27px' title='Assist fatti nelle ultime 5 giornate'>ass</td>")
        str.AppendLine("              <td align='center' width='27px' title='Numero partite giocate nelle ultime 5 giornate'>p.g.</td>")
        str.AppendLine("              <td align='center' width='27px' title='Numero partite giocate da titolare nelle ultime 5 giornate'>p.tit</td>")
        str.AppendLine("              <td align='center' width='27px' title='Numero subentri nelle ultime 5 giornate'>p.sub</td>")
        str.AppendLine("              <td align='center' width='27px' title='Minuti giocati nelle ultime 5 giornate'>m.m.</td>")
        str.AppendLine("              <td align='center' width='27px' title='Media minuti giocati nelle ultime 5 giornate'>min</td>")
        str.AppendLine("              <td align='center' width='27px' title='Media voto'>m.v.</td>")
        str.AppendLine("              <td align='center' width='27px' title='Media punti'>m.p.</td>")
        Return str.ToString
    End Function

    Private Shared Function GetHeaderClasaGirone() As String
        Dim str As New System.Text.StringBuilder
        str.Append("<td align='center' width='155' title='Nome squadra'>Squadra</td>")
        str.Append("<td align='center' width='35' title='Partite giocate'>PG</td>")
        str.Append("<td align='center' width='35' title='Vittorie'>V</td>")
        str.Append("<td align='center' width='35' title='Pareggi'>P</td>")
        str.Append("<td align='center' width='35' title='Sconfitte'>S</td>")
        str.Append("<td align='center' width='35' title='Goal fatti'>GF</td>")
        str.Append("<td align='center' width='35' title='Goal subiti'>GS</td>")
        str.Append("<td align='center' width='35' title='Punti'>Pt</td>")
        Return str.ToString
    End Function

    Private Shared Function GetHeaderTableRating() As String
        Dim str As New System.Text.StringBuilder
        str.AppendLine("              <td align='center' width='20px' title='Posizione in classifica'>P.</td>")
        str.AppendLine("              <td align='center' width='20px' title='Variazione in classifica'>Var.</td>")
        str.AppendLine("              <td align='center' width='30px' title='Stemma'</td>")
        str.AppendLine("              <td title='Nome team e allenatore'>Nome</td>")
        str.AppendLine("              <td width='45px' align='center' title='Punti totali'>pt</td>")
        str.AppendLine("              <td width='45px' align='center' title='Punti giornata'>pt g</td>")
        str.AppendLine("              <td width='45px' align='center' title='Differenza punti dal primo in classifica'>diff1</td>")
        str.AppendLine("              <td width='45px' align='center' title='Differenza punti dalla squadra che precede in classifica'>diff2</td>")
        str.AppendLine("              <td width='40px' align='center' title='Media punti'>med.</td>")
        str.AppendLine("              <td width='40px' align='center' title='Punteggio minimo realizzato'>min</td>")
        str.AppendLine("              <td width='40px' align='center' title='Punteggio massimo realizzato'>max</td>")
        str.AppendLine("              <td width='28px' align='center' title='Ammonizioni totali (giocatori schierati)'>amm</td>")
        str.AppendLine("              <td width='28px' align='center' title='Espulsioni totali (giocatori schierati)'>esp</td>")
        str.AppendLine("              <td width='28px' align='center' title='Assist totali (giocatori schierati)'>ass</td>")
        str.AppendLine("              <td width='28px' align='center' title='Goal subiti totali (giocatori schierati)'>gs</td>")
        str.AppendLine("              <td width='28px' align='center' title='Goal fatti totali (giocatori schierati)'>gf</td>")
        str.AppendLine("              <td width='30px' align='center' title='Punti persi rispetto alla squadra top player'>p.p.</td>")
        str.AppendLine("              <td width='40px' align='center' title='Percentuale punti persi rispetto alla squadra top player'>% p.p.</td>")
        str.AppendLine("              <td width='35px' align='center' title='Totale punti da bonus'>pt b.</td>")
        str.AppendLine("              <td width='35px' align='center' title='Numero vittorie di giornata'>n&#176vitt</td>")
        str.AppendLine("              <td width='35px' align='center' title='Numero giocate con meno di 11 giocatori'>n&#176<11</td>")
        str.AppendLine("              <td width='35px' align='center' title='Numero giocatori jolly utilizzati'>n&#176(*)</td>")
        str.AppendLine("              <td width='35px' align='center' title='Punteggio fanta mister'>fm</td>")
        str.AppendLine("              <td width='80px' align='center' title='Grafico (Basato sulla differenza punti)'></td>")
        Return str.ToString
    End Function

    Private Shared Function GetHeaderTableRatingHistory() As String
        Dim str As New System.Text.StringBuilder
        str.Append("<td width='200' title='Nome team e allenatore'>Nome</td>")
        For i As Integer = 0 To currlega.Settings.NumberOfDays
            If i = 0 Then
                str.Append("<td width='45' align='center' title='Totale'>Tot</td>")
            Else
                str.Append("<td width='45' align='center' title='Giornata n&#176" & i & "'>" & i & "</td>")
            End If
        Next
        Return str.ToString
    End Function

    Shared Function GetHeaderPage(ByVal Title As String, ByVal Css As String) As String
        Dim str As New System.Text.StringBuilder
        str.Append("<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>" & System.Environment.NewLine)
        str.Append("<html lang='it'><head>" & System.Environment.NewLine)
        str.Append("<meta http-equiv='content-type' content='text/html; charset=ISO-8859-1' />" & System.Environment.NewLine)
        str.Append("<title>" & Title & "</title>" & System.Environment.NewLine)
        str.Append("<style type='text/css'>" & Css & "</style>" & System.Environment.NewLine)
        str.Append("</head>" & System.Environment.NewLine)
        str.Append("<body>" & System.Environment.NewLine)
        str.Append("<div style='padding:10px'>" & System.Environment.NewLine)
        Return str.ToString
    End Function

    Shared Function GetFooterPage() As String
        Dim str As New System.Text.StringBuilder
        str.Append("</div>" & System.Environment.NewLine)
        str.Append("</body>" & System.Environment.NewLine)
        str.Append("</html>" & System.Environment.NewLine)
        Return str.ToString
    End Function

    Shared Sub ImportDati(ByVal Directory As String)

        Dim fname As String = Directory & "\dati.txt"

        If File.Exists(fname) Then

            ExecuteSql("DELETE FROM tbdati;", conn)

            Dim lines() As String = IO.File.ReadAllLines(fname)
            Dim r As Integer = 0
            Dim str As New System.Text.StringBuilder
            Dim sins As String = "INSERT INTO tbdati (gio,ruolo,nome,squadra,gs,gf,amm,esp,ass,rigp,rigs,rigt,autogol,voto,pt) VALUES "

            For i As Integer = 0 To lines.Length - 1

                Dim s() As String = lines(i).Split(CChar("|"))

                If s.Length > 13 Then

                    str.Append(",(" & s(0) & ",")
                    str.Append("'" & s(1) & "',")
                    str.Append("'" & s(2) & "',")
                    str.Append("'" & s(3) & "',")
                    str.Append(s(4) & ",")
                    str.Append(s(5) & ",")
                    str.Append(s(6) & ",")
                    str.Append(s(7) & ",")
                    str.Append(s(8) & ",")
                    str.Append(s(9) & ",")
                    str.Append(s(10) & ",")
                    str.Append(s(11) & ",")
                    str.Append(s(12) & ",")
                    str.Append(s(13) & ",")
                    str.Append(s(14) & ")")
                    r += 1

                    If r > blkrec Then
                        Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                        str = New System.Text.StringBuilder
                        r = 0
                    End If

                End If
            Next

            If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

        Else
            ShowError("Errore", "File non trovato : " & System.Environment.NewLine & fname)
            Call WriteError("ImpExp", "ImportaDati", fname)
        End If

    End Sub

    Shared Sub ImportDati(ByVal FileName As String, Table As String)
        Try
            Dim dsxml As New DataSet
            dsxml.ReadXml(FileName)
            If dsxml.Tables.Count > 0 Then

                ExecuteSql("DELETE FROM " & Table & ";", conn)

                Select Case Table

                    Case "tbteam"

                        Dim sins As String = "INSERT INTO tbteam (idteam,nome,allenatore,presidente) VALUES "
                        Dim r As Integer = 0
                        Dim str As New System.Text.StringBuilder

                        For i As Integer = 0 To dsxml.Tables(0).Rows.Count - 1

                            Dim dr As DataRow = dsxml.Tables(0).Rows(i)
                            Dim idteam As Integer = ReadFieldIntegerData("idteam", dr, 0)
                            Dim nome As String = ReadFieldStringData("nome", dr, "")
                            Dim alle As String = ReadFieldStringData("allenatore", dr, "")
                            Dim pres As String = ReadFieldStringData("presidente", dr, "")

                            If nome <> "" Then
                                str.AppendLine(",(" & idteam & ",'" & nome & "','" & alle & "','" & pres & "')")
                                r += 1

                                If r > blkrec Then
                                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                    str = New System.Text.StringBuilder
                                    r = 0
                                    System.Threading.Thread.Sleep(30)
                                End If
                            End If

                        Next

                        If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                    Case "tbrose"

                        Dim sins As String = "INSERT INTO tbrose (idteam,idrosa,ruolo,nome,costo,qini,riconfermato) VALUES "
                        Dim r As Integer = 0
                        Dim str As New System.Text.StringBuilder

                        For i As Integer = 0 To dsxml.Tables(0).Rows.Count - 1

                            Dim dr As DataRow = dsxml.Tables(0).Rows(i)
                            Dim idteam As Integer = ReadFieldIntegerData("idteam", dr, 0)
                            Dim idrosa As Integer = ReadFieldIntegerData("idrosa", dr, 0)
                            Dim ruolo As String = ReadFieldStringData("ruolo", dr, "")
                            Dim nome As String = ReadFieldStringData("nome", dr, "")
                            Dim costo As Integer = ReadFieldIntegerData("costo", dr, 0)
                            Dim qini As Integer = ReadFieldIntegerData("qini", dr, 0)
                            Dim riconfermato As Integer = ReadFieldIntegerData("riconfermato", dr, 0)

                            If ruolo <> "" AndAlso nome <> "" Then
                                str.AppendLine(",(" & idteam & "," & idrosa & ",'" & ruolo & "','" & nome & "'," & costo & "," & qini & "," & riconfermato & ")")
                                r += 1

                                If r > blkrec Then
                                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                    str = New System.Text.StringBuilder
                                    r = 0
                                    System.Threading.Thread.Sleep(30)
                                End If
                            End If

                        Next

                        If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                    Case "tbplayer"

                        Dim sins As String = "INSERT INTO tbplayer (ruolo,nome,squadra,qini,qcur) VALUES "
                        Dim r As Integer = 0
                        Dim str As New System.Text.StringBuilder

                        For i As Integer = 0 To dsxml.Tables(0).Rows.Count - 1

                            Dim dr As DataRow = dsxml.Tables(0).Rows(i)
                            Dim ruolo As String = ReadFieldStringData("ruolo", dr, "")
                            Dim nome As String = ReadFieldStringData("nome", dr, "")
                            Dim squadra As String = ReadFieldStringData("squadra", dr, "")
                            Dim qini As Integer = ReadFieldIntegerData("qini", dr, 0)
                            Dim qcur As Integer = ReadFieldIntegerData("qcur", dr, 0)

                            If ruolo <> "" AndAlso nome <> "" AndAlso squadra <> "" Then
                                str.AppendLine(",('" & ruolo & "','" & nome & "','" & squadra & "'," & qini & "," & qcur & ")")
                                r += 1

                                If r > blkrec Then
                                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                    str = New System.Text.StringBuilder
                                    r = 0
                                    System.Threading.Thread.Sleep(30)
                                End If
                            End If

                        Next

                        If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                    Case "tbplayer_data"

                        Dim sins As String = "INSERT INTO tbplayer_data (ruolo,nome,squadra,nat,natcode) VALUES "
                        Dim r As Integer = 0
                        Dim str As New System.Text.StringBuilder

                        For i As Integer = 0 To dsxml.Tables(0).Rows.Count - 1

                            Dim dr As DataRow = dsxml.Tables(0).Rows(i)
                            Dim idteam As Integer = ReadFieldIntegerData("idteam", dr, 0)
                            Dim nome As String = ReadFieldStringData("nome", dr, "")
                            Dim squadra As String = ReadFieldStringData("squadra", dr, "")
                            Dim nat As String = ReadFieldStringData("nat", dr, "")
                            Dim natcode As String = ReadFieldStringData("natcode", dr, "")

                            If nome <> "" Then
                                str.AppendLine(",(" & idteam & ",'" & nome & "','" & squadra & "','" & nat & "','" & natcode & "')")
                                r += 1

                                If r > blkrec Then
                                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                    str = New System.Text.StringBuilder
                                    r = 0
                                    System.Threading.Thread.Sleep(30)
                                End If
                            End If

                        Next

                        If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                    Case "tbmatch"

                        Dim sins As String = "INSERT INTO tbmatch (gio,idmatch,teama,teamb,timem) VALUES "
                        Dim r As Integer = 0
                        Dim str As New System.Text.StringBuilder

                        For i As Integer = 0 To dsxml.Tables(0).Rows.Count - 1

                            Dim dr As DataRow = dsxml.Tables(0).Rows(i)
                            Dim gio As Integer = ReadFieldIntegerData("gio", dr, 0)
                            Dim idmatch As Integer = ReadFieldIntegerData("idmatch", dr, 0)
                            Dim teama As String = ReadFieldStringData("teama", dr, "")
                            Dim teamb As String = ReadFieldStringData("teamb", dr, "")
                            Dim timem As Date = ReadFieldTimeData("nat", dr, Date.Now)

                            str.AppendLine(",(" & gio & "," & idmatch & ",'" & teama & "','" & teamb & "','" & timem.ToString("yyyy-MM-dd HH:mm:ss") & "')")
                            r += 1

                            If r > blkrec Then
                                Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                str = New System.Text.StringBuilder
                                r = 0
                                System.Threading.Thread.Sleep(30)
                            End If

                        Next

                        If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                    Case "tbformazioni", "tbformazionitop"

                        Dim sins As String = "INSERT INTO " & Table & " (gio,idteam,schierato,idformazione,incampo,ruolo,nome,type,vote,amm,esp,autogol,gs,gf,ass,rigs,rigp,pt) VALUES "
                        Dim r As Integer = 0
                        Dim str As New System.Text.StringBuilder

                        For i As Integer = 0 To dsxml.Tables(0).Rows.Count - 1

                            Dim dr As DataRow = dsxml.Tables(0).Rows(i)
                            Dim gio As Integer = ReadFieldIntegerData("gio", dr, 0)
                            Dim idteam As Integer = ReadFieldIntegerData("idteam", dr, 0)
                            Dim schierato As Integer = ReadFieldIntegerData("schierato", dr, 0)
                            Dim idforma As Integer = ReadFieldIntegerData("idformazione", dr, 0)
                            Dim incampo As Integer = ReadFieldIntegerData("incampo", dr, 0)
                            Dim ruolo As String = ReadFieldStringData("ruolo", dr, "")
                            Dim nome As String = ReadFieldStringData("nome", dr, "")
                            Dim type As Integer = ReadFieldIntegerData("type", dr, 0)
                            Dim vote As String = ReadFieldStringData("vote", dr, "")
                            Dim amm As Integer = ReadFieldIntegerData("amm", dr, 0)
                            Dim esp As Integer = ReadFieldIntegerData("esp", dr, 0)
                            Dim autogol As Integer = ReadFieldIntegerData("autogol", dr, 0)
                            Dim gs As Integer = ReadFieldIntegerData("gs", dr, 0)
                            Dim gf As Integer = ReadFieldIntegerData("gf", dr, 0)
                            Dim ass As Integer = ReadFieldIntegerData("ass", dr, 0)
                            Dim rigs As Integer = ReadFieldIntegerData("rigs", dr, 0)
                            Dim rigp As Integer = ReadFieldIntegerData("rigp", dr, 0)
                            Dim pt As Integer = ReadFieldIntegerData("pt", dr, 0)

                            str.Append(",(" & gio & "," & idteam & "," & schierato & "," & idforma & "," & incampo & ",'" & ruolo & "'")
                            str.Append(",'" & nome & "'," & type & ",'" & vote & "'," & amm & "," & esp & "," & autogol & "," & gs)
                            str.Append("," & gf & "," & ass & "," & rigs & "," & rigp & "," & pt & ")")

                            r += 1

                            If r > blkrec Then
                                Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                str = New System.Text.StringBuilder
                                r = 0
                                System.Threading.Thread.Sleep(30)
                            End If

                        Next

                        If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                    Case "tbdati"

                        Dim sins As String = "INSERT INTO " & Table & " (gio,ruolo,nome,squadra,gf,gs,amm,esp,ass,rigp,rigs,rigt,autogol,voto,pt) VALUES "
                        Dim r As Integer = 0
                        Dim str As New System.Text.StringBuilder

                        For i As Integer = 0 To dsxml.Tables(0).Rows.Count - 1

                            Dim dr As DataRow = dsxml.Tables(0).Rows(i)
                            Dim gio As Integer = ReadFieldIntegerData("gio", dr, 0)
                            Dim ruolo As String = ReadFieldStringData("ruolo", dr, "")
                            Dim nome As String = ReadFieldStringData("nome", dr, "")
                            Dim squadra As String = ReadFieldStringData("squadra", dr, "")
                            Dim gs As Integer = ReadFieldIntegerData("gs", dr, 0)
                            Dim gf As Integer = ReadFieldIntegerData("gf", dr, 0)
                            Dim amm As Integer = ReadFieldIntegerData("amm", dr, 0)
                            Dim esp As Integer = ReadFieldIntegerData("esp", dr, 0)
                            Dim ass As Integer = ReadFieldIntegerData("ass", dr, 0)
                            Dim rigp As Integer = ReadFieldIntegerData("rigp", dr, 0)
                            Dim rigs As Integer = ReadFieldIntegerData("rigs", dr, 0)
                            Dim rigt As Integer = ReadFieldIntegerData("rigt", dr, 0)
                            Dim autogol As Integer = ReadFieldIntegerData("autogol", dr, 0)
                            Dim voto As Integer = ReadFieldIntegerData("voto", dr, 0)
                            Dim pt As Integer = ReadFieldIntegerData("pt", dr, 0)

                            str.Append(",(" & gio & ",'" & ruolo & "'")
                            str.Append(",'" & nome & "','" & squadra & "'," & gf & "," & gs & "," & amm & "," & esp & "," & ass)
                            str.Append("," & rigp & "," & rigs & "," & rigt & "," & autogol & "," & voto & "," & pt & ")")

                            r += 1

                            If r > blkrec Then
                                Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                str = New System.Text.StringBuilder
                                r = 0
                                System.Threading.Thread.Sleep(30)
                            End If

                        Next

                        If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                End Select
            End If
        Catch ex As Exception

        End Try
    End Sub

    Shared Sub ExportDati(SqlString As String, ByVal FileName As String)
        Try

            Dim ds As DataSet = SystemFunction.DataBase.ExecuteSqlReturnDataSet(SqlString, conn)

            If ds.Tables.Count > 0 Then
                ds.Tables(0).WriteXml(FileName, XmlWriteMode.WriteSchema)
            End If
           
            ds.Dispose()

        Catch ex As Exception

        End Try
       
    End Sub
End Class