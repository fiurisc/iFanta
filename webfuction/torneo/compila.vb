
Namespace Torneo

    Public Class CompilaData

        Dim appSett As New PublicVariables
        Dim gen As General
        Dim forma As FormazioniData

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
            gen = New General(appSett)
            forma = New FormazioniData(appSett)
        End Sub

        Private Function GetStatusFileName() As String
            Return appSett.TorneoPath & "compila.json"
        End Function

        Public Function ApiCompila(giornata As String) As String

            Try

                Dim fname As String = appSett.WebDataPath & "temp\WD" & giornata.PadLeft(2, CChar("0")) & ".txt"
                Dim fdata As String = appSett.WebDataPath & "temp\DD" & giornata.PadLeft(2, CChar("0")) & ".txt"
                Dim max As Integer = 7

                WebData.Players.Data.LoadPlayers(appSett, True)

                gen.ReadSettings()

                'eseguo il backup del database'
                Functions.BackupDatabase(appSett, False)

                'Scarico il file'
                UpdateStatus("Download file from...", 1, max)
                Call DownloadFile(giornata, fname)

                'Analizzo i dati'
                UpdateStatus("Estrazione dati...", 2, max)
                Call ExtractData(fname, fdata)

                'Compilazione dati'
                UpdateStatus("Inserimento dati...", 3, max)
                Call InsertData(giornata, fdata)

                'Compilazione dati'
                UpdateStatus("Compilazione formazioni...", 4, max)
                Dim lst1 As New List(Of FormazioniData.Formazione)
                For i As Integer = 0 To appSett.Settings.NumberOfTeams - 1
                    lst1.Add(CompileDataForma(giornata, i))
                Next

                UpdateStatus("Salvataggio formazioni...", 5, max)
                'forma.ApiDeleteFormazioni(giornata, "-1", False)
                forma.SaveFormazioni(CInt(giornata), lst1, False)

                'Compilazione dati top player'
                UpdateStatus("Compilazione top formazioni...", 6, max)
                Dim lst2 As List(Of FormazioniData.Formazione) = CompileTopFormazioni(giornata)
                UpdateStatus("Salvataggio top formazioni...", 7, max)
                'forma.ApiDeleteFormazioni(giornata, "-1", True)
                forma.SaveFormazioni(CInt(giornata), lst2, True)

                ResetStatus()

                Return "{""Compilazione"": ""eseguita""}"

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                Return "{'Compilazione': '" & ex.Message & "'}"
            End Try


        End Function

        Public Function Compila() As String
            Dim match As New MatchsData(appSett)
            Return ApiCompila(match.ApiGetMatchsCurrentDay())
        End Function

        Public Function ApiGetStatus() As String
            Try
                Dim fname As String = GetStatusFileName()
                If IO.File.Exists(fname) Then
                    Return IO.File.ReadAllText(fname)
                Else
                    Return WebData.Functions.SerializzaOggetto(New CompileStatus(), True)
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
            Return WebData.Functions.SerializzaOggetto(New CompileStatus(), True)
        End Function

        Private Sub ResetStatus()
            Try
                Dim fname As String = GetStatusFileName()
                If IO.File.Exists(fname) Then IO.File.Delete(fname)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub UpdateStatus(text As String, value As Integer, max As Integer)
            Try
                Dim stato As New CompileStatus(text, value.ToString(), max.ToString())
                System.IO.File.WriteAllText(GetStatusFileName(), WebData.Functions.SerializzaOggetto(stato, True))
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Sub DownloadFile(ByVal Giornata As String, ByVal FileName As String)
            Dim html As String = WebData.Functions.GetPage(appSett, "https://www.pianetafanta.it/Voti-Ufficiali-Excel.asp?giornataScelta=" & Giornata & "&searchBonus=")
            If html <> "" Then IO.File.WriteAllText(FileName, html, System.Text.Encoding.GetEncoding("ISO-8859-1"))
        End Sub

        Private Function ExtractData(ByVal SourceFileName As String, ByVal DestFileName As String) As Integer

            Dim ngio As Integer = 0

            If IO.File.Exists(SourceFileName) Then

                Dim sw As New IO.StreamWriter(DestFileName, False, System.Text.Encoding.UTF8)

                Try

                    Dim start As Boolean = False
                    Dim fname As String = ""
                    Dim str As New System.Text.StringBuilder
                    Dim kplay As New List(Of String)
                    Dim totplayer As Integer = 0
                    Dim totplayernotfound As Integer = 0

                    sw.WriteLine("RUOLO|NAME|SQUADRA|AMM|ESP|GOAL F|GOAL S|AUTO GOL|ASS|RIG TRA|RIG SB|RIG PAR|VOTO|PT")

                    Dim line() As String = System.Text.RegularExpressions.Regex.Replace(IO.File.ReadAllText(SourceFileName), "\<tr\>", "|").Split(CChar("|"))

                    For i As Integer = 0 To line.Length - 1

                        If line(i).Contains("Legenda Visualizzabile nel sito") = False Then

                            Dim cell() As String = System.Text.RegularExpressions.Regex.Replace(line(i), "\<\/td\>", "|").Split(CChar("|"))

                            If cell.Length = 36 Then

                                For k As Integer = 0 To cell.Length - 1
                                    cell(k) = System.Text.RegularExpressions.Regex.Match(cell(k), "(?<=\>).*").Value.Trim
                                Next

                                Dim datplayer As New Dictionary(Of String, PtPlayer)
                                Dim name As String = cell(1)
                                Dim ruolo As String = cell(2)
                                Dim squadra As String = cell(4)
                                Dim amm As String = cell(23)
                                Dim esp As String = cell(24)
                                Dim rigs As String = cell(27)
                                Dim rigp As String = cell(28)
                                Dim rigt As String = cell(29)

                                If name.Contains("CARNESECCHI M.") Then
                                    name = name
                                End If

                                If ruolo = "P" OrElse ruolo = "D" OrElse ruolo = "C" OrElse ruolo = "A" Then

                                    name = WebData.Functions.NormalizeText(name)
                                    If name.Contains("ROMAGNOLI") Then
                                        name = name
                                    End If

                                    Dim pmath As WebData.Players.PlayerMatch = WebData.Players.Data.ResolveName(ruolo, name, squadra, True)

                                    fname = pmath.MatchedPlayer.Name

                                    If pmath.Matched Then
                                        totplayer += 1
                                    Else
                                        totplayernotfound += 1
                                        fname = "#" & fname
                                    End If

                                    For k As Integer = 0 To appSett.Settings.Points.SiteReferenceForPoints.Count - 1
                                        datplayer.Add(appSett.Settings.Points.SiteReferenceForPoints(k), New PtPlayer)
                                    Next

                                    Dim vtf As Integer = 0
                                    Dim ptf As Integer = 0
                                    Dim nvt As Integer = 0

                                    If appSett.Settings.Points.SiteReferenceForPoints.Contains("gazzetta") Then
                                        datplayer("gazzetta").vt = cell(6).Replace(".", ",")
                                        datplayer("gazzetta").pt = cell(32).Replace(".", ",")
                                        datplayer("gazzetta").gf = cell(7)
                                        datplayer("gazzetta").gs = cell(8)
                                        datplayer("gazzetta").aut = cell(9)
                                        datplayer("gazzetta").ass = cell(10)
                                    End If
                                    If appSett.Settings.Points.SiteReferenceForPoints.Contains("corriere") Then
                                        datplayer("corriere").vt = cell(11).Replace(".", ",")
                                        datplayer("corriere").pt = cell(33).Replace(".", ",")
                                        datplayer("corriere").gf = cell(12)
                                        datplayer("corriere").gs = cell(13)
                                        datplayer("corriere").aut = cell(14)
                                        datplayer("corriere").ass = cell(15)
                                    End If
                                    If appSett.Settings.Points.SiteReferenceForPoints.Contains("tuttosport") Then
                                        datplayer("tuttosport").vt = cell(16).Replace(".", ",")
                                        datplayer("tuttosport").pt = cell(34).Replace(".", ",")
                                        datplayer("tuttosport").gf = cell(17)
                                        datplayer("tuttosport").gs = cell(18)
                                        datplayer("tuttosport").aut = cell(19)
                                        datplayer("tuttosport").ass = cell(20)
                                    End If

                                    nvt = 0

                                    For Each s As String In datplayer.Keys

                                        'In caso di s.v.ma con presenza di bonus/malus fisso il voto a 6 come da regolamento'
                                        If datplayer(s).vt = "" OrElse datplayer(s).vt = "s,v," Then
                                            If (esp <> "" AndAlso esp <> "0") Then
                                                datplayer(s).vt = "4"
                                            ElseIf (datplayer(s).ass <> "" AndAlso datplayer(s).ass <> "0") OrElse (datplayer(s).gf <> "" AndAlso datplayer(s).gf <> "0") OrElse (datplayer(s).gs <> "" AndAlso datplayer(s).gs <> "0") OrElse (datplayer(s).aut <> "" AndAlso datplayer(s).aut <> "0") OrElse (rigp <> "" AndAlso rigp <> "0") OrElse (rigs <> "" AndAlso rigs <> "0") Then
                                                datplayer(s).vt = "6"
                                            End If
                                        End If

                                        If datplayer(s).vt = "" OrElse datplayer(s).vt = "s,v," Then
                                            datplayer(s).pt = "-200"
                                        Else
                                            datplayer(s).vt = datplayer(s).vt.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                            datplayer(s).vt = CStr(Math.Truncate(CSng(datplayer(s).vt) * 10))
                                            datplayer(s).pt = datplayer(s).vt
                                            If esp <> "" AndAlso esp <> "0" Then
                                                datplayer(s).pt = AddBonus(datplayer(s).pt, -10)
                                            ElseIf amm <> "" AndAlso amm <> "0" Then
                                                datplayer(s).pt = AddBonus(datplayer(s).pt, -5)
                                            End If
                                            If datplayer(s).ass <> "" Then datplayer(s).pt = AddBonus(datplayer(s).pt, CInt(datplayer(s).ass) * 10)
                                            If datplayer(s).aut <> "" Then
                                                If ruolo = "P" Then
                                                    datplayer(s).pt = AddBonus(datplayer(s).pt, -CInt(datplayer(s).aut) * 10)
                                                Else
                                                    datplayer(s).pt = AddBonus(datplayer(s).pt, -CInt(datplayer(s).aut) * 20)
                                                End If
                                            End If
                                            If rigs <> "" Then datplayer(s).pt = AddBonus(datplayer(s).pt, -CInt(rigs) * 30)
                                            If rigp <> "" Then datplayer(s).pt = AddBonus(datplayer(s).pt, CInt(rigp) * 30)
                                            If datplayer(s).gf <> "" Then datplayer(s).pt = AddBonus(datplayer(s).pt, CInt(datplayer(s).gf) * 30)
                                            If datplayer(s).gs <> "" Then datplayer(s).pt = AddBonus(datplayer(s).pt, -CInt(datplayer(s).gs) * 10)
                                            vtf = CInt(vtf + CDbl(datplayer(s).vt))
                                            nvt += 1
                                        End If
                                    Next

                                    If nvt > 0 Then

                                        Dim t As Double = CInt(Math.Truncate(((vtf / (nvt * 10)) + 0.25) * 2) / 0.2)
                                        vtf = CInt(Math.Truncate(((vtf / (nvt * 10)) + 0.25) * 2) / 0.2)
                                        ptf = vtf

                                        If esp <> "" AndAlso esp <> "0" Then
                                            ptf = ptf + appSett.Settings.Points.Expulsion
                                        ElseIf amm <> "" AndAlso amm <> "0" Then
                                            ptf = ptf + appSett.Settings.Points.Admonition
                                        End If
                                        If datplayer(appSett.Settings.Points.SiteReferenceForBonus).ass <> "" Then ptf = ptf + CInt(datplayer(appSett.Settings.Points.SiteReferenceForBonus).ass) * appSett.Settings.Points.Assist(ruolo)
                                        If datplayer(appSett.Settings.Points.SiteReferenceForBonus).aut <> "" Then ptf = ptf + CInt(datplayer(appSett.Settings.Points.SiteReferenceForBonus).aut) * appSett.Settings.Points.OwnGoal(ruolo)
                                        If rigs <> "" Then ptf = ptf + CInt(rigs) * appSett.Settings.Points.MissedPenalty(ruolo)
                                        If rigp <> "" Then ptf = ptf + CInt(rigp) * 30
                                        If datplayer(appSett.Settings.Points.SiteReferenceForBonus).gf <> "" Then ptf = ptf + CInt(datplayer(appSett.Settings.Points.SiteReferenceForBonus).gf) * appSett.Settings.Points.GoalScored(ruolo)
                                        If datplayer(appSett.Settings.Points.SiteReferenceForBonus).gs <> "" Then ptf = ptf + CInt(datplayer(appSett.Settings.Points.SiteReferenceForBonus).gs) * appSett.Settings.Points.GoalConceded

                                    Else
                                        vtf = -200
                                        ptf = -200
                                    End If

                                    Dim key As String = ruolo & "|" & fname & "|" & squadra

                                    If kplay.Contains(key) = False Then
                                        sw.WriteLine(ruolo & "|" & name & "|" & fname & "|" & squadra & "|" & datplayer(appSett.Settings.Points.SiteReferenceForBonus).gf & "|" & datplayer(appSett.Settings.Points.SiteReferenceForBonus).gs & "|" & amm & "|" & esp & "|" & datplayer(appSett.Settings.Points.SiteReferenceForBonus).ass & "|" & rigt & "|" & rigs & "|" & rigp & "|" & datplayer(appSett.Settings.Points.SiteReferenceForBonus).aut & "|" & vtf & "|" & ptf)
                                        kplay.Add(key)
                                    End If

                                End If
                            End If
                        End If
                    Next

                    sw.WriteLine("#------------------------------------------")
                    sw.WriteLine("#total player=" & totplayer)
                    sw.WriteLine("#total player found=" & totplayer - totplayernotfound)
                    sw.WriteLine("#total player not found=" & totplayernotfound)
                    sw.WriteLine("#percentage total player not found=" & CInt((totplayernotfound * 100) / totplayer) & "%")
                    sw.WriteLine("#------------------------------------------")

                Catch ex As Exception
                    WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                End Try

                sw.Dispose()

            Else
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Dati non trovati")
            End If

            Return ngio

        End Function

        Private Sub InsertData(ByVal Giornata As String, ByVal FileName As String)

            If IO.File.Exists(FileName) Then

                Try
                    Dim line() As String = IO.File.ReadAllLines(FileName)
                    Dim decsep As String = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator

                    'Elimino i dati precedenti'
                    Functions.ExecuteSql(appSett, "DELETE FROM tbdati WHERE gio=" & Giornata & ";")

                    Dim str As New System.Text.StringBuilder
                    Dim sins As String = "INSERT INTO tbdati (gio,ruolo,nome,squadra,gf,gs,amm,esp,ass,rigt,rigs,rigp,autogol,voto,pt) VALUES "
                    Dim r As Integer = 0

                    'Analizzo i dati'
                    For i As Integer = 0 To line.Length - 1
                        If line(i).Contains("#") = False Then
                            Dim s() As String = line(i).Split(CChar("|"))
                            If s.Length = 15 AndAlso i > 0 Then
                                For k As Integer = 3 To s.Length - 1
                                    If s(k) = "" Then s(k) = "0"
                                Next
                                Functions.ExecuteSql(appSett, sins & "(" & Giornata & ",'" & s(0) & "','" & s(2) & "','" & s(3) & "'," & s(4) & "," & s(5) & "," & s(6) & "," & s(7) & "," & s(8) & "," & s(9) & "," & s(10) & "," & s(11) & "," & s(12) & "," & s(13) & "," & s(14) & ")")
                            End If
                        End If
                    Next

                Catch ex As Exception
                    WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                End Try
            Else
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Not found " & FileName)
            End If

        End Sub

        Private Function CompileDataForma(ByVal Giornata As String, ByVal TeamId As Integer) As FormazioniData.Formazione

            Dim f As New FormazioniData.Formazione()

            Try

                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Compilazione formazione  teamid: " & TeamId & " giornata: " & Giornata)

                f.Giornata = CInt(Giornata)
                f.TeamId = TeamId

                Dim str As New System.Text.StringBuilder
                str.Append("SELECT f.idrosa,f.type,f.idformazione,f.ruolo,f.nome,f.squadra,")
                str.Append("d.amm,d.esp,d.ass,d.autogol,d.gs,d.gf,d.rigp,d.rigs,d.voto,d.pt ")
                str.Append("FROM tbformazioni as f LEFT JOIN tbdati as d ON (f.nome = d.nome AND (f.squadra = d.squadra or f.squadra='') AND f.gio = d.gio) ")
                str.Append("WHERE f.gio=" & Giornata & " AND idteam=" & TeamId & " AND type<10 ")
                str.Append("ORDER BY f.type,f.idformazione;")

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, str.ToString)

                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then

                    Dim svp As Integer = 0
                    Dim svd As Integer = 0
                    Dim svc As Integer = 0
                    Dim sva As Integer = 0
                    Dim numsos As Integer = 0
                    Dim idforma As New List(Of String)

                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim t As Integer = CInt(ds.Tables(0).Rows(i).Item("type"))
                        Dim keyp As String = ds.Tables(0).Rows(i).Item("idformazione").ToString()
                        Dim data As System.Data.DataRow = ds.Tables(0).Rows(i)

                        Dim p As New FormazioniData.PlayerFormazione

                        'Dati generali'
                        p.RosaId = Functions.ReadFieldIntegerData(data.Item("idrosa"), -1)
                        p.Type = Functions.ReadFieldIntegerData(data.Item("type"))
                        p.FormaId = Functions.ReadFieldIntegerData(data.Item("idformazione"))
                        p.Ruolo = Functions.ReadFieldStringData(data.Item("ruolo"), "?")
                        p.Nome = Functions.ReadFieldStringData(data.Item("nome"))
                        p.Squadra = Functions.ReadFieldStringData(data.Item("squadra"))
                        p.Ammonito = Functions.ReadFieldIntegerData(data.Item("amm"))
                        p.Espulso = Functions.ReadFieldIntegerData(data.Item("esp"))
                        p.Assists = Functions.ReadFieldIntegerData(data.Item("ass"))
                        p.AutoGoal = Functions.ReadFieldIntegerData(data.Item("autogol"))
                        p.GoalSubiti = Functions.ReadFieldIntegerData(data.Item("gs"))
                        p.GoalFatti = Functions.ReadFieldIntegerData(data.Item("gf"))
                        p.RigoriSbagliati = Functions.ReadFieldIntegerData(data.Item("rigs"))
                        p.RigoriParati = Functions.ReadFieldIntegerData(data.Item("rigp"))
                        p.Voto = Functions.ReadFieldIntegerData(data.Item("voto"), -200)
                        p.Punti = Functions.ReadFieldIntegerData(data.Item("pt"), -200)

                        'Dati giornata'
                        If idforma.Contains(keyp) = False AndAlso ((p.FormaId < 12 AndAlso p.Type = 1) OrElse (p.FormaId >= 12 AndAlso p.FormaId <= 11 + appSett.Settings.NumberOfReserve AndAlso p.Type = 2)) Then


                            If p.Type <> 0 Then
                                If p.Type = 1 Then
                                    If p.Punti <> -200 Then
                                        p.InCampo = 1
                                    Else
                                        p.InCampo = 0
                                        Select Case p.Ruolo
                                            Case "P" : svp += 1
                                            Case "D" : svd += 1
                                            Case "C" : svc += 1
                                            Case "A" : sva += 1
                                        End Select
                                    End If
                                End If
                            Else
                                p.InCampo = 0
                            End If
                            idforma.Add(keyp)
                        End If
                        f.Players.Add(p)
                    Next

                    'Determino le eventuali sostituzioni'
                    Dim npp As Integer = 0
                    Dim npd As Integer = 0
                    Dim npc As Integer = 0
                    Dim npa As Integer = 0

                    f.Players = f.Players.OrderBy(Function(x) x.FormaId).ToList()

                    For i As Integer = 0 To f.Players.Count - 1
                        If f.Players(i).Type = 2 Then
                            Select Case f.Players(i).Ruolo
                                Case "P" : npp += 1
                                Case "D" : npd += 1
                                Case "C" : npc += 1
                                Case "A" : npa += 1
                            End Select
                        End If
                    Next

                    Dim nsm As Integer = 0

                    If svp > 0 OrElse svd > 0 OrElse svc > 0 OrElse sva > 0 Then
                        If appSett.Settings.SubstitutionType = TorneoSettings.eSubstitutionType.ChangeModule Then

                            'Determino il modulo attuale'
                            Dim nm() As Integer = {0, 0, 0}
                            Dim oldmodule As String = ""
                            Dim newmodule As String = ""

                            For i As Integer = 0 To f.Players.Count - 1
                                If f.Players(i).Type = 1 Then
                                    Select Case f.Players(i).Ruolo
                                        Case "D" : nm(0) = nm(0) + 1
                                        Case "C" : nm(1) = nm(1) + 1
                                        Case "A" : nm(2) = nm(2) + 1
                                    End Select
                                End If
                            Next

                            oldmodule = nm(0) & "-" & nm(1) & "-" & nm(2)

                            nm(0) = 0
                            nm(1) = 0
                            nm(2) = 0

                            For i As Integer = 0 To f.Players.Count - 1
                                If f.Players(i).InCampo = 1 Then
                                    Select Case f.Players(i).Ruolo
                                        Case "D" : nm(0) += 1
                                        Case "C" : nm(1) += 1
                                        Case "A" : nm(2) += 1
                                    End Select
                                End If
                            Next

                            For i As Integer = 0 To f.Players.Count - 1
                                If f.Players(i).Type = 2 Then
                                    Select Case f.Players(i).Ruolo
                                        Case "P"
                                            If svp > 0 Then
                                                If f.Players(i).Punti > -100 Then
                                                    f.Players(i).InCampo = 1
                                                    numsos += 1
                                                    svp -= 1
                                                End If
                                            End If
                                            npp -= 1
                                        Case Else
                                            If f.Players(i).Punti > -100 Then
                                                If CheckMudule(f.Players(i).Ruolo, 1, nm(0), nm(1), nm(2)) Then
                                                    Select Case f.Players(i).Ruolo
                                                        Case "D" : nm(0) += 1
                                                        Case "C" : nm(1) += 1
                                                        Case "A" : nm(2) += 1
                                                    End Select
                                                    f.Players(i).InCampo = 1
                                                    numsos += 1
                                                    nsm -= 1
                                                End If
                                            End If
                                    End Select
                                    If numsos >= appSett.Settings.NumberOfSubstitution Then Exit For
                                End If
                            Next

                            nm(0) = 0
                            nm(1) = 0
                            nm(2) = 0

                            For i As Integer = 0 To f.Players.Count - 1
                                If f.Players(i).InCampo = 1 AndAlso f.Players(i).Punti > -100 Then
                                    Select Case f.Players(i).Ruolo
                                        Case "D" : nm(0) += 1
                                        Case "C" : nm(1) += 1
                                        Case "A" : nm(2) += 1
                                    End Select
                                End If
                            Next

                            newmodule = nm(0) & "-" & nm(1) & "-" & nm(2)

                            If oldmodule <> newmodule Then f.CambioModulo = 1

                        Else

                            For i As Integer = 0 To f.Players.Count - 1
                                If f.Players(i).Type = 2 Then
                                    Select Case f.Players(i).Ruolo
                                        Case "P"
                                            If svp > 0 Then
                                                If f.Players(i).Punti > -100 Then
                                                    f.Players(i).InCampo = 1
                                                    numsos += 1
                                                    svp -= 1
                                                End If
                                            End If
                                            npp = npp - 1
                                        Case "D"
                                            If svd > 0 Then
                                                If f.Players(i).Punti > -100 Then
                                                    f.Players(i).InCampo = 1
                                                    numsos += 1
                                                    svd -= 1
                                                End If
                                            End If
                                            npd -= 1
                                        Case "C"
                                            If svc > 0 Then
                                                If f.Players(i).Punti > -100 Then
                                                    f.Players(i).InCampo = 1
                                                    numsos += 1
                                                    svc -= 1
                                                End If
                                            End If
                                            npc -= 1
                                        Case "A"
                                            If sva > 0 Then
                                                If f.Players(i).Punti > -100 Then
                                                    f.Players(i).InCampo = 1
                                                    numsos += 1
                                                    sva -= 1
                                                End If
                                            End If
                                            npa -= 1
                                    End Select
                                    If numsos >= appSett.Settings.NumberOfSubstitution Then Exit For
                                End If
                                If npp = 0 AndAlso svp > 0 Then
                                    If appSett.Settings.SubstitutionType = TorneoSettings.eSubstitutionType.Normal Then numsos += 1
                                    svp = 0
                                End If
                                If npd = 0 AndAlso svd > 0 Then
                                    If appSett.Settings.SubstitutionType = TorneoSettings.eSubstitutionType.Normal Then numsos += 1
                                    nsm += svd : svd = 0
                                End If
                                If npc = 0 AndAlso svc > 0 Then
                                    If appSett.Settings.SubstitutionType = TorneoSettings.eSubstitutionType.Normal Then numsos += 1
                                    nsm += svc : svc = 0
                                End If
                                If npa = 0 AndAlso sva > 0 Then
                                    If appSett.Settings.SubstitutionType = TorneoSettings.eSubstitutionType.Normal Then numsos += 1
                                    nsm += sva : sva = 0
                                End If

                            Next
                        End If

                        If appSett.Settings.SubstitutionType = TorneoSettings.eSubstitutionType.NormalAndChangeModule Then

                            If nsm > 0 Then

                                Dim nm() As Integer = {0, 0, 0}
                                Dim oldmodule As String = ""
                                Dim newmodule As String = ""

                                For i As Integer = 0 To f.Players.Count - 1
                                    If f.Players(i).Type = 1 Then
                                        Select Case f.Players(i).Ruolo
                                            Case "D" : nm(0) += 1
                                            Case "C" : nm(1) += 1
                                            Case "A" : nm(2) += 1
                                        End Select
                                    End If
                                Next

                                oldmodule = nm(0) & "-" & nm(1) & "-" & nm(2)

                                nm(0) = 0
                                nm(1) = 0
                                nm(2) = 0

                                For i As Integer = 0 To f.Players.Count - 1
                                    If f.Players(i).InCampo = 1 Then
                                        Select Case f.Players(i).Ruolo
                                            Case "D" : nm(0) += 1
                                            Case "C" : nm(1) += 1
                                            Case "A" : nm(2) += 1
                                        End Select
                                    End If
                                Next

                                For i As Integer = 0 To f.Players.Count - 1
                                    If f.Players(i).Type = 2 AndAlso f.Players(i).InCampo = 0 AndAlso f.Players(i).Ruolo <> "P" AndAlso f.Players(i).Punti > -100 Then
                                        If CheckMudule(f.Players(i).Ruolo, 1, nm(0), nm(1), nm(2)) Then
                                            Select Case f.Players(i).Ruolo
                                                Case "D" : nm(0) += 1
                                                Case "C" : nm(1) += 1
                                                Case "A" : nm(2) += 1
                                            End Select
                                            f.Players(i).InCampo = 1
                                            numsos += 1
                                            nsm -= 1
                                        End If
                                    End If
                                    If numsos >= appSett.Settings.NumberOfSubstitution OrElse nsm = 0 Then Exit For
                                Next

                                nm(0) = 0
                                nm(1) = 0
                                nm(2) = 0

                                For i As Integer = 0 To f.Players.Count - 1
                                    If f.Players(i).InCampo = 1 AndAlso f.Players(i).Punti > -100 Then
                                        Select Case f.Players(i).Ruolo
                                            Case "D" : nm(0) += 1
                                            Case "C" : nm(1) += 1
                                            Case "A" : nm(2) += 1
                                        End Select
                                    End If
                                Next

                                newmodule = nm(0) & "-" & nm(1) & "-" & nm(2)

                                If oldmodule <> newmodule Then f.CambioModulo = 1

                            End If
                        End If

                    End If

                    'Determino i bonus difesa/centrocampo/attacco'
                    Dim ndgoodd As Integer = 0
                    Dim ndgoodc As Integer = 0
                    Dim ndgooda As Integer = 0
                    Dim ndt As Integer = 0
                    Dim nct As Integer = 0
                    Dim nat As Integer = 0

                    'Determino il bonus centrocampo'
                    For i As Integer = 0 To f.Players.Count - 1
                        If f.Players(i).Type > 0 AndAlso f.Players(i).InCampo = 1 AndAlso f.Players(i).Voto > -100 Then
                            Select Case f.Players(i).Ruolo
                                Case "D"
                                    If appSett.Settings.Bonus.EnableBonusDefense Then ndgoodd = ndgoodd + GetGoodForBonus(f.Players(i))
                                    ndt += 1
                                Case "C"
                                    If appSett.Settings.Bonus.EnableCenterField Then ndgoodc = ndgoodc + GetGoodForBonus(f.Players(i))
                                    nct += 1
                                Case "A"
                                    If appSett.Settings.Bonus.EnableCenterField Then ndgooda = ndgooda + GetGoodForBonus(f.Players(i))
                                    nat += 1
                            End Select
                        End If
                    Next

                    WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Difensori con voto >=6: " & ndgoodd & "/" & ndt)

                    If appSett.Settings.Bonus.EnableBonusDefense AndAlso ndgoodd = ndt Then
                        f.BonusDifesa = BonusDefense(ndgoodd)
                    Else
                        f.BonusDifesa = 0
                    End If

                    WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Bonus defense: " & f.BonusDifesa)

                    If appSett.Settings.Bonus.EnableCenterField AndAlso ndgoodc = nct Then
                        f.BonusCentrocampo = BonusCenterField(ndgoodc)
                    Else
                        f.BonusCentrocampo = 0
                    End If

                    If appSett.Settings.Bonus.EnableBonusAttack AndAlso ndgooda = nat Then
                        f.BonusAttacco = BonusAttack(ndgooda)
                    Else
                        f.BonusAttacco = 0
                    End If

                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return f

        End Function

        Private Function CompileTopFormazioni(giornata As String) As List(Of FormazioniData.Formazione)

            Dim lst As List(Of FormazioniData.Formazione) = forma.GetFormazioni(giornata, "-1", False)

            Try

                For k As Integer = 0 To lst.Count - 1

                    Dim ntit As Integer = 0
                    Dim np As Integer = 0
                    Dim nd As Integer = 0
                    Dim nc As Integer = 0
                    Dim na As Integer = 0
                    Dim np1 As Integer = 0
                    Dim nd1 As Integer = 0
                    Dim nc1 As Integer = 0
                    Dim na1 As Integer = 0
                    Dim ndgoodd As Integer = 0
                    Dim ndgoodc As Integer = 0
                    Dim ndgooda As Integer = 0

                    For i As Integer = 0 To lst(k).Players.Count - 1
                        If lst(k).Players(i).Type >= 0 AndAlso lst(k).Players(i).Punti > -100 Then
                            Select Case lst(k).Players(i).Ruolo
                                Case "P" : np1 += 1
                                Case "D" : nd1 += 1
                                Case "C" : nc1 += 1
                                Case "A" : na1 += 1
                            End Select
                        End If
                    Next

                    lst(k).CambioModulo = 0
                    lst(k).Players = lst(k).Players.OrderByDescending(Function(x) x.Punti).ToList()

                    For i As Integer = 0 To lst(k).Players.Count - 1
                        lst(k).Players(i).FormaId = 0
                        lst(k).Players(i).Type = 0
                        lst(k).Players(i).InCampo = 0
                        If lst(k).Players(i).Type >= 0 AndAlso lst(k).Players(i).Punti > -100 Then
                            If CheckMudule(lst(k).Players(i).Ruolo, np, nd, nc, na) Then
                                Select Case lst(k).Players(i).Ruolo
                                    Case "P"
                                        np += 1
                                    Case "D"
                                        If appSett.Settings.Bonus.EnableBonusDefense Then ndgoodd += GetGoodForBonus(lst(k).Players(i))
                                        nd += 1
                                    Case "C"
                                        If appSett.Settings.Bonus.EnableCenterField Then ndgoodc += GetGoodForBonus(lst(k).Players(i))
                                        nc += 1
                                    Case "A"
                                        If appSett.Settings.Bonus.EnableCenterField Then ndgooda += GetGoodForBonus(lst(k).Players(i))
                                        na += 1
                                End Select
                                lst(k).Players(i).Type = 1
                                lst(k).Players(i).InCampo = 1
                                ntit += 1
                            End If
                        End If
                    Next

                    Dim idforma As Integer = 1
                    lst(k).Players = lst(k).Players.OrderBy(Function(x) x.RosaId).ToList()
                    For i As Integer = 0 To lst(k).Players.Count - 1
                        If lst(k).Players(i).InCampo = 1 Then
                            lst(k).Players(i).FormaId = idforma
                            idforma += 1
                        End If
                    Next

                    If appSett.Settings.Bonus.EnableBonusDefense AndAlso ndgoodd = nd Then
                        lst(k).BonusDifesa = BonusDefense(ndgoodd)
                    Else
                        lst(k).BonusDifesa = 0
                    End If

                    If appSett.Settings.Bonus.EnableCenterField AndAlso ndgoodc = nc Then
                        lst(k).BonusCentrocampo = BonusCenterField(ndgoodc)
                    Else
                        lst(k).BonusCentrocampo = 0
                    End If

                    If appSett.Settings.Bonus.EnableBonusAttack AndAlso ndgooda = na Then
                        lst(k).BonusAttacco = BonusAttack(ndgooda)
                    Else
                        lst(k).BonusAttacco = 0
                    End If
                Next

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return lst

        End Function
        Private Function BonusDefense(nplayer As Integer) As Integer
            Dim bonus As Integer = 0
            Select Case nplayer
                Case 3 : bonus = appSett.Settings.Bonus.BonusDefense("3")
                Case 4 : bonus = appSett.Settings.Bonus.BonusDefense("4")
                Case Is > 4 : bonus = appSett.Settings.Bonus.BonusDefense("5")
            End Select
            Return bonus
        End Function

        Private Function BonusCenterField(nplayer As Integer) As Integer
            Dim bonus As Integer = 0
            Select Case nplayer
                Case 3 : bonus = appSett.Settings.Bonus.BonusCenterField("3")
                Case 4 : bonus = appSett.Settings.Bonus.BonusCenterField("4")
                Case Is > 4 : bonus = appSett.Settings.Bonus.BonusCenterField("5")
            End Select
            Return bonus
        End Function

        Private Function BonusAttack(nplayer As Integer) As Integer
            Dim bonus As Integer = 0
            Select Case nplayer
                Case 2 : bonus = appSett.Settings.Bonus.BonusAttack("2")
                Case Is > 2 : bonus = appSett.Settings.Bonus.BonusAttack("3")
            End Select
            Return bonus
        End Function

        Public Function CheckMudule(ByVal Ruolo As String, ByVal CurrP As Integer, ByVal CurrD As Integer, ByVal CurrC As Integer, ByVal CurrA As Integer) As Boolean

            Dim ris As Boolean = False

            Dim tot As Integer = CurrP + CurrD + CurrC + CurrA + 1

            Select Case Ruolo
                Case "P" : CurrP += 1
                Case "D" : CurrD += 1
                Case "C" : CurrC += 1
                Case "A" : CurrA += 1
            End Select

            If CurrP < 2 AndAlso CurrD < 4 AndAlso CurrC < 5 AndAlso CurrA < 4 Then
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 3 AndAlso CurrC < 5 AndAlso CurrA < 3 Then '343'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 4 AndAlso CurrC < 6 AndAlso CurrA < 3 Then '352'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 4 AndAlso CurrA < 4 Then '433'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 5 AndAlso CurrA < 3 Then '442'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 6 AndAlso CurrA < 2 Then '451'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 6 AndAlso CurrC < 4 AndAlso CurrA < 3 Then '532'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 6 AndAlso CurrC < 5 AndAlso CurrA < 2 Then '541'
                ris = True
            End If

            Return ris

        End Function

        Private Function GetGoodForBonus(ByVal p As FormazioniData.PlayerFormazione) As Integer
            If appSett.Settings.Bonus.EnableBonusDefense Then
                Dim val As Integer = p.Punti
                If appSett.Settings.Bonus.BonudAttackSource = "vote" Then val = p.Voto
                If val >= appSett.Settings.Bonus.BonusAttackOverEqual * 10 Then
                    Return 1
                Else
                    Return 0
                End If
            Else
                Return 0
            End If
        End Function

        Private Function AddBonus(pt As String, bonus As Integer) As String
            Return CStr(CDbl(pt) - bonus)
        End Function

        Public Class CompileStatus
            Public Property Text As String = ""
            Public Property Value As String = "0"
            Public Property Max As String = "0"

            Sub New()

            End Sub

            Sub New(Text As String, Value As String, Max As String)
                Me.Text = Text
                Me.Value = Value
                Me.Max = Max
            End Sub
        End Class

        Public Class PtPlayer
            Public Property vt As String = ""
            Public Property pt As String = ""
            Public Property ass As String = ""
            Public Property aut As String = ""
            Public Property gs As String = ""
            Public Property gf As String = ""
            Public Property rigp As String = ""
            Public Property rigs As String = ""
        End Class

        Public Class DataRow
            Public Property id As Integer = 0
            Public Property giornata As Integer = 0
            Public Property ruolo As String = ""
            Public Property nome As String = ""
            Public Property squadra As String = ""
            Public Property gf As Integer = 0
            Public Property gs As Integer = 0
            Public Property amm As Integer = 0
            Public Property esp As Integer = 0
            Public Property ass As Integer = 0
            Public Property rigp As Integer = 0
            Public Property rigs As Integer = 0
            Public Property rigt As Integer = 0
            Public Property autogol As Integer = 0
            Public Property voto As Integer = 0
            Public Property punti As Integer = 0
        End Class
    End Class

End Namespace