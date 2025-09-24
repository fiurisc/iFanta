Imports iFanta.SystemFunction.DataBase
Imports iFanta.SystemFunction.FileAndDirectory

Public Class frmcompila

    Dim website As String = "www.pianetafantacalcio.it"
    Dim start As Boolean = True

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName
        lbby.Text = My.Application.Info.Copyright

        For i As Integer = 1 To currlega.Settings.NumberOfDays
            cmb1.Add(CStr(i))
        Next

        Try
            Dim g As Integer = currlega.GiornataCorrente - 1
            If g < 0 Then g = 0
            If g > currlega.Settings.NumberOfDays - 1 Then g = currlega.Settings.NumberOfDays - 1
            cmb1.SelectedIndex = g
        Catch ex As Exception

        End Try
       
        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        'Setto il tema corrente'
        Call SetTheme()
        'Setto l'altezza della finestra'
        Call SetWindowsHeight()

        start = False

    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Sub ResetTorneo()
        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)
    End Sub

    Sub SetTheme()

        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)

        tlbaction.Left = IForm1.RX - padd - tlbaction.Width
        tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
        tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

        ILine1.Top = tlbaction.Top - ILine1.Height - 5
        ILine1.Width = IForm1.RX - IForm1.LX - padd * 2
        ILine1.Left = IForm1.LX + padd
        prb1.Top = ILine1.Top - prb1.Height - 5
        prb1.Width = ILine1.Width
        prb1.Left = ILine1.Left
        prb1.FlatStyle = AppSett.Personal.Theme.FlatStyle
        lbstatus.Left = ILine1.Left
        lbstatus.Top = prb1.Top - lbstatus.Height - 5

        pic1.Left = padd
        pic1.Top = padd + IForm1.TY
        lbmain.Left = pic1.Left + pic1.Width + 5
        lbmain.Top = pic1.Top

        lbmain.Text = "Download e compilazione dati da : |" & website
        chkdown.Text = "Download dati da " & website

        lbby.Left = IForm1.LX + padd - 3
        lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

        cmb1.FlatStyle = AppSett.Personal.Theme.FlatStyle

    End Sub

    Sub RunCompile()

        Dim fname As String = GetTempDirectory() & "\WD" & cmb1.Text.PadLeft(2, CChar("0")) & ".txt"
        Dim fdata As String = GetTempDirectory() & "\DD" & cmb1.Text.PadLeft(2, CChar("0")) & ".txt"
        Dim nplayer As Integer = 0
        Dim gio As Integer = CInt(cmb1.Text)

        prb1.Max = 4

        webdata.LoadWebPlayersKey()

        Try

            'Scarico il file'
            UpdateStatus("Download file from " & website & "...", 1)
            If chkopt.Value = False OrElse chkdown.Value Then Call DownloadFile(gio, fname)

            'Analizzo i dati'
            UpdateStatus("Estrazione dati...", 2)
            If chkopt.Value = False OrElse chkextract.Value Then Call ExtractData(fname, fdata)

            'Compilazione dati'
            UpdateStatus("Inserimento dati...", 3)
            If chkopt.Value = False OrElse chkextract.Value Then Call InsertData(gio, fdata)

            'Compilazione dati'
            prb1.Max = currlega.Teams.Count
            If chkopt.Value = False OrElse chkform.Value Then
                Dim lst As New List(Of LegaObject.Formazione)
                For i As Integer = 0 To currlega.Teams.Count - 1
                    UpdateStatus("Compilazione dati formazione " & currlega.Teams(i).Nome & "...", i + 1)
                    lst.Add(CompileDataForma(gio, i))
                Next
                UpdateStatus("Salvataggio dati formazione...", currlega.Teams.Count)
                LegaObject.Formazione.Delete(gio, False)
                LegaObject.Formazione.Save(lst, gio, False)
            End If

            'Compilazione dati top player'
            prb1.Max = currlega.Teams.Count
            If chkopt.Value = False OrElse chkformtop.Value Then
                Dim lst As New List(Of LegaObject.Formazione)
                For i As Integer = 0 To currlega.Teams.Count - 1
                    UpdateStatus("Compilazione dati top formazione " & currlega.Teams(i).Nome & "...", i + 1)
                    lst.Add(currlega.CompileTopFormazione(gio, i))
                Next
                UpdateStatus("Salvataggio dati top formazioni...", currlega.Teams.Count)
                LegaObject.Formazione.Delete(gio, True)
                LegaObject.Formazione.Save(lst, gio, True)
            End If

            prb1.Max = 2
            UpdateStatus("Aggiornamento dati statistici...", 1)
            Call webdata.UpdatePlayerTbFromView(False)
            System.Threading.Thread.Sleep(1000)

            lbstatus.Text = "Completato"

        Catch ex As Exception
            ShowError("Errore", ex.Message)
            Call WriteError("Compile", "RunCompile", ex.Message)
            lbstatus.Text = "Errore in fase di compilazione!"
        End Try

        prb1.Value = 0

    End Sub

    Sub UpdateStatus(ByVal Txt As String, ByVal Phase As Integer)
        'Compilazione dati'
        lbstatus.Text = Txt
        lbstatus.Refresh()
        prb1.Value = Phase
    End Sub

    Sub DownloadFile(ByVal Giornata As Integer, ByVal FileName As String)
        Dim html As String = wData.GetPage("https://www.pianetafanta.it/Voti-Ufficiali-Excel.asp?giornataScelta=" & CStr(Giornata) & "&searchBonus=", "POST", "")
        If html <> "" Then IO.File.WriteAllText(FileName, html, System.Text.Encoding.GetEncoding("ISO-8859-1"))
    End Sub

    Sub DownloadFileOld(ByVal Giornata As Integer, ByVal FileName As String)
        Dim html As String = wData.GetPage("https://www.pianetafanta.it/Voti-Ufficiali.asp?GiornataA=" & CStr(Giornata) & "&Tipolink=0&TipoVoti=", "POST", "")
        If html <> "" Then IO.File.WriteAllText(FileName, html, System.Text.Encoding.GetEncoding("ISO-8859-1"))
    End Sub

    Function ExtractData(ByVal SourceFileName As String, ByVal DestFileName As String) As Integer

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

                            If name.Contains("DOUVIKAS") Then
                                name = name
                            End If

                            name = name.Replace("MILINKOVIC V.", "MILINKOVIC SAVIC V.").Replace("MILINKOVIC S.", "MILINKOVIC SAVIC")

                            If ruolo = "P" OrElse ruolo = "D" OrElse ruolo = "C" OrElse ruolo = "A" Then

                                name = wData.NormalizeText(name)

                                fname = webdata.ResolveName("", name, squadra, True)
                                totplayer += 1
                                If fname.StartsWith("#") Then
                                    totplayernotfound += 1
                                End If

                                For k As Integer = 0 To currlega.Settings.Points.SiteReferenceForPoints.Count - 1
                                    datplayer.Add(currlega.Settings.Points.SiteReferenceForPoints(k), New PtPlayer)
                                Next

                                Dim vtf As Integer = 0
                                Dim ptf As Integer = 0
                                Dim nvt As Integer = 0

                                If currlega.Settings.Points.SiteReferenceForPoints.Contains("gazzetta") Then
                                    datplayer("gazzetta").vt = cell(6).Replace(".", ",")
                                    datplayer("gazzetta").pt = cell(32).Replace(".", ",")
                                    datplayer("gazzetta").gf = cell(7)
                                    datplayer("gazzetta").gs = cell(8)
                                    datplayer("gazzetta").aut = cell(9)
                                    datplayer("gazzetta").ass = cell(10)
                                End If
                                If currlega.Settings.Points.SiteReferenceForPoints.Contains("corriere") Then
                                    datplayer("corriere").vt = cell(11).Replace(".", ",")
                                    datplayer("corriere").pt = cell(33).Replace(".", ",")
                                    datplayer("corriere").gf = cell(12)
                                    datplayer("corriere").gs = cell(13)
                                    datplayer("corriere").aut = cell(14)
                                    datplayer("corriere").ass = cell(15)
                                End If
                                If currlega.Settings.Points.SiteReferenceForPoints.Contains("tuttosport") Then
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
                                        datplayer(s).vt = datplayer(s).vt.Replace(",", My.Application.Culture.NumberFormat.NumberDecimalSeparator)
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
                                        ptf = ptf + currlega.Settings.Points.Expulsion
                                    ElseIf amm <> "" AndAlso amm <> "0" Then
                                        ptf = ptf + currlega.Settings.Points.Admonition
                                    End If
                                    If datplayer(currlega.Settings.Points.SiteReferenceForBonus).ass <> "" Then ptf = ptf + CInt(datplayer(currlega.Settings.Points.SiteReferenceForBonus).ass) * currlega.Settings.Points.Assist(ruolo)
                                    If datplayer(currlega.Settings.Points.SiteReferenceForBonus).aut <> "" Then ptf = ptf + CInt(datplayer(currlega.Settings.Points.SiteReferenceForBonus).aut) * currlega.Settings.Points.OwnGoal(ruolo)
                                    If rigs <> "" Then ptf = ptf + CInt(rigs) * currlega.Settings.Points.MissedPenalty(ruolo)
                                    If rigp <> "" Then ptf = ptf + CInt(rigp) * 30
                                    If datplayer(currlega.Settings.Points.SiteReferenceForBonus).gf <> "" Then ptf = ptf + CInt(datplayer(currlega.Settings.Points.SiteReferenceForBonus).gf) * currlega.Settings.Points.GoalScored(ruolo)
                                    If datplayer(currlega.Settings.Points.SiteReferenceForBonus).gs <> "" Then ptf = ptf + CInt(datplayer(currlega.Settings.Points.SiteReferenceForBonus).gs) * currlega.Settings.Points.GoalConceded

                                Else
                                    vtf = -200
                                    ptf = -200
                                End If

                                Dim key As String = ruolo & "|" & fname & "|" & squadra

                                If kplay.Contains(key) = False Then
                                    sw.WriteLine(ruolo & "|" & name & "|" & fname & "|" & squadra & "|" & datplayer(currlega.Settings.Points.SiteReferenceForBonus).gf & "|" & datplayer(currlega.Settings.Points.SiteReferenceForBonus).gs & "|" & amm & "|" & esp & "|" & datplayer(currlega.Settings.Points.SiteReferenceForBonus).ass & "|" & rigt & "|" & rigs & "|" & rigp & "|" & datplayer(currlega.Settings.Points.SiteReferenceForBonus).aut & "|" & vtf & "|" & ptf)
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
                Call WriteError("Compile", "ExtractData", ex.Message)
            End Try

            sw.Dispose()
        Else
            ShowError("Errore", "Dati non trovati")
        End If

        Return ngio

    End Function

    Private Function AddBonus(pt As String, bonus As Integer) As String
        Return CStr(CDbl(pt) - bonus)
    End Function

    Sub InsertData(ByVal Giornata As Integer, ByVal FileName As String)

        If IO.File.Exists(FileName) Then

            Try
                Dim line() As String = IO.File.ReadAllLines(FileName)
                Dim decsep As String = My.Application.Culture.NumberFormat.CurrencyDecimalSeparator

                'Elimino i dati precedenti'
                Call ExecuteSql("DELETE FROM " & tbdati & " WHERE gio=" & Giornata & ";", conn)

                Dim str As New System.Text.StringBuilder
                Dim sins As String = "INSERT INTO " & tbdati & " (gio,ruolo,nome,squadra,gf,gs,amm,esp,ass,rigt,rigs,rigp,autogol,voto,pt) VALUES "
                Dim r As Integer = 0

                'Analizzo i dati'
                For i As Integer = 0 To line.Length - 1
                    If line(i).Contains("#") = False Then
                        Dim s() As String = line(i).Split(CChar("|"))
                        If s.Length = 15 AndAlso i > 0 Then
                            For k As Integer = 3 To s.Length - 1
                                If s(k) = "" Then s(k) = "0"
                            Next
                            str.Append(",(" & Giornata & ",'" & s(0) & "','" & s(2) & "','" & s(3) & "'," & s(4) & "," & s(5) & "," & s(6) & "," & s(7) & "," & s(8) & "," & s(9) & "," & s(10) & "," & s(11) & "," & s(12) & "," & s(13) & "," & s(14) & ")")
                            r += 1
                            If r > blkrec Then
                                Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                str = New System.Text.StringBuilder
                                r = 0
                            End If
                        End If
                    End If
                Next
                If r > 0 Then
                    Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                End If

            Catch ex As Exception
                Call WriteError("Compile", "InsertData", ex.Message)
            End Try
        Else
            Call WriteError("Compile", "InsertData", "Not found " & FileName)
        End If

    End Sub

    Function CompileDataForma(ByVal Giornata As Integer, ByVal IdTeam As Integer) As LegaObject.Formazione

        Dim f As New LegaObject.Formazione(Giornata, IdTeam)

        Try

            Dim str As New System.Text.StringBuilder
            str.Append("SELECT f.idrosa,f.type,f.idformazione,f.ruolo,f.nome,f.squadra,")
            str.Append("d.amm,d.esp,d.ass,d.autogol,d.gs,d.gf,d.rigp,d.rigs,d.voto,d.pt ")
            str.Append("FROM tbformazioni as f LEFT JOIN tbdati as d ON (f.nome = d.nome AND (f.squadra = d.squadra or f.squadra='') AND f.gio = d.gio) ")
            str.Append("WHERE f.gio=" & Giornata & " AND idteam=" & IdTeam & " AND type<10 ")
            str.Append("ORDER BY f.type,f.idformazione;")

            Dim ds As DataSet = ExecuteSqlReturnDataSet(str.ToString, conn)

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
                    Dim data As DataRow = ds.Tables(0).Rows(i)

                    Dim p As New LegaObject.Formazione.PlayerFormazione

                    'Dati generali'
                    p.IdTeam = IdTeam
                    p.IdRosa = ReadFieldIntegerData(data.Item("idrosa"), -1)
                    p.Type = ReadFieldIntegerData(data.Item("type"))
                    p.IdFormazione = ReadFieldIntegerData(data.Item("idformazione"))
                    p.Ruolo = ReadFieldStringData(data.Item("ruolo"), "?")
                    p.Nome = ReadFieldStringData(data.Item("nome"))
                    p.Squadra = ReadFieldStringData(data.Item("squadra"))

                    'Dati giornata'
                    If idforma.Contains(keyp) = False AndAlso ((p.IdFormazione < 12 AndAlso p.Type = 1) OrElse (p.IdFormazione >= 12 AndAlso p.IdFormazione <= 11 + currlega.Settings.NumberOfReserve AndAlso p.Type = 2)) Then

                        p.Dati.Amm = ReadFieldIntegerData(data.Item("amm"))
                        p.Dati.Esp = ReadFieldIntegerData(data.Item("esp"))
                        p.Dati.Ass = ReadFieldIntegerData(data.Item("ass"))
                        p.Dati.AutG = ReadFieldIntegerData(data.Item("autogol"))
                        p.Dati.Gs = ReadFieldIntegerData(data.Item("gs"))
                        p.Dati.Gf = ReadFieldIntegerData(data.Item("gf"))
                        p.Dati.RigS = ReadFieldIntegerData(data.Item("rigs"))
                        p.Dati.RigP = ReadFieldIntegerData(data.Item("rigp"))
                        p.Dati.Vt = CSng(ReadFieldIntegerData(data.Item("voto"), -200) / 10)
                        p.Dati.Pt = CSng(ReadFieldIntegerData(data.Item("pt"), -200) / 10)

                        If p.Type <> 0 Then
                            If p.Type = 1 Then
                                If p.Dati.Pt <> -20 Then
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

                    Else
                        p.IdRosa = p.IdRosa
                    End If
                    f.Players.Add(p)
                Next

                'Determino le eventuali sostituzioni'
                Dim npp As Integer = 0
                Dim npd As Integer = 0
                Dim npc As Integer = 0
                Dim npa As Integer = 0

                f.Players = f.Players.OrderBy(Function(x) x.IdFormazione).ToList()

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
                    If currlega.Settings.SubstitutionType = LegaObject.LegaSettings.eSubstitutionType.ChangeModule Then

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
                                            If f.Players(i).Dati.Pt > -10 Then
                                                f.Players(i).InCampo = 1
                                                numsos += 1
                                                svp -= 1
                                            End If
                                        End If
                                        npp -= 1
                                    Case Else
                                        If f.Players(i).Dati.Pt > -10 Then
                                            If SystemFunction.General.CheckMudule(f.Players(i).Ruolo, 1, nm(0), nm(1), nm(2)) Then
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
                                If numsos >= currlega.Settings.NumberOfSubstitution Then Exit For
                            End If
                        Next

                        nm(0) = 0
                        nm(1) = 0
                        nm(2) = 0

                        For i As Integer = 0 To f.Players.Count - 1
                            If f.Players(i).InCampo = 1 AndAlso f.Players(i).Dati.Pt > -10 Then
                                Select Case f.Players(i).Ruolo
                                    Case "D" : nm(0) += 1
                                    Case "C" : nm(1) += 1
                                    Case "A" : nm(2) += 1
                                End Select
                            End If
                        Next

                        newmodule = nm(0) & "-" & nm(1) & "-" & nm(2)

                        If oldmodule <> newmodule Then f.ModuleSubstitution = True

                    Else

                        For i As Integer = 0 To f.Players.Count - 1
                            If f.Players(i).Type = 2 Then
                                Select Case f.Players(i).Ruolo
                                    Case "P"
                                        If svp > 0 Then
                                            If f.Players(i).Dati.Pt > -10 Then
                                                f.Players(i).InCampo = 1
                                                numsos += 1
                                                svp -= 1
                                            End If
                                        End If
                                        npp = npp - 1
                                    Case "D"
                                        If svd > 0 Then
                                            If f.Players(i).Dati.Pt > -10 Then
                                                f.Players(i).InCampo = 1
                                                numsos += 1
                                                svd -= 1
                                            End If
                                        End If
                                        npd -= 1
                                    Case "C"
                                        If svc > 0 Then
                                            If f.Players(i).Dati.Pt > -10 Then
                                                f.Players(i).InCampo = 1
                                                numsos += 1
                                                svc -= 1
                                            End If
                                        End If
                                        npc -= 1
                                    Case "A"
                                        If sva > 0 Then
                                            If f.Players(i).Dati.Pt > -10 Then
                                                f.Players(i).InCampo = 1
                                                numsos += 1
                                                sva -= 1
                                            End If
                                        End If
                                        npa -= 1
                                End Select
                                If numsos >= currlega.Settings.NumberOfSubstitution Then Exit For
                            End If
                            If npp = 0 AndAlso svp > 0 Then
                                If currlega.Settings.SubstitutionType = LegaObject.LegaSettings.eSubstitutionType.Normal Then numsos = numsos + 1
                                svp = 0
                            End If
                            If npd = 0 AndAlso svd > 0 Then
                                If currlega.Settings.SubstitutionType = LegaObject.LegaSettings.eSubstitutionType.Normal Then numsos = numsos + 1
                                nsm += svd : svd = 0
                            End If
                            If npc = 0 AndAlso svc > 0 Then
                                If currlega.Settings.SubstitutionType = LegaObject.LegaSettings.eSubstitutionType.Normal Then numsos = numsos + 1
                                nsm += svc : svc = 0
                            End If
                            If npa = 0 AndAlso sva > 0 Then
                                If currlega.Settings.SubstitutionType = LegaObject.LegaSettings.eSubstitutionType.Normal Then numsos = numsos + 1
                                nsm += sva : sva = 0
                            End If

                        Next
                    End If

                    If currlega.Settings.SubstitutionType = LegaObject.LegaSettings.eSubstitutionType.NormalAndChangeModule Then

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
                                If f.Players(i).Type = 2 AndAlso f.Players(i).InCampo = 0 AndAlso f.Players(i).Ruolo <> "P" AndAlso f.Players(i).Dati.Pt > -10 Then
                                    If SystemFunction.General.CheckMudule(f.Players(i).Ruolo, 1, nm(0), nm(1), nm(2)) Then
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
                                If numsos >= currlega.Settings.NumberOfSubstitution OrElse nsm = 0 Then Exit For
                            Next

                            nm(0) = 0
                            nm(1) = 0
                            nm(2) = 0

                            For i As Integer = 0 To f.Players.Count - 1
                                If f.Players(i).InCampo = 1 AndAlso f.Players(i).Dati.Pt > -10 Then
                                    Select Case f.Players(i).Ruolo
                                        Case "D" : nm(0) += 1
                                        Case "C" : nm(1) += 1
                                        Case "A" : nm(2) += 1
                                    End Select
                                End If
                            Next

                            newmodule = nm(0) & "-" & nm(1) & "-" & nm(2)

                            If oldmodule <> newmodule Then f.ModuleSubstitution = True

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
                    If f.Players(i).Type > 0 AndAlso f.Players(i).InCampo = 1 AndAlso f.Players(i).Dati.Vt > -10 Then
                        Select Case f.Players(i).Ruolo
                            Case "D"
                                If currlega.Settings.Bonus.EnableBonusDefense Then ndgoodd = ndgoodd + LegaObject.GetGoodForBonus(f.Players(i))
                                ndt += 1
                            Case "C"
                                If currlega.Settings.Bonus.EnableCenterField Then ndgoodc = ndgoodc + LegaObject.GetGoodForBonus(f.Players(i))
                                nct += 1
                            Case "A"
                                If currlega.Settings.Bonus.EnableCenterField Then ndgooda = ndgooda + LegaObject.GetGoodForBonus(f.Players(i))
                                nat += 1
                        End Select
                    End If
                Next
                If currlega.Settings.Bonus.EnableBonusDefense AndAlso ndgoodd = ndt Then
                    Dim bonus As Single = 0
                    Select Case ndgoodd
                        Case 3 : bonus = CSng(currlega.Settings.Bonus.BonusDefense(3) / 10)
                        Case 4 : bonus = CSng(currlega.Settings.Bonus.BonusDefense(4) / 10)
                        Case Is > 4 : bonus = CSng(currlega.Settings.Bonus.BonusDefense(5) / 10)
                    End Select
                    f.BonusDifesa = bonus
                Else
                    f.BonusDifesa = 0
                End If
                If currlega.Settings.Bonus.EnableCenterField AndAlso ndgoodc = nct Then
                    Dim bonus As Single = 0
                    Select Case ndgoodc
                        Case 3 : bonus = CSng(currlega.Settings.Bonus.BonusCenterField(3) / 10)
                        Case 4 : bonus = CSng(currlega.Settings.Bonus.BonusCenterField(4) / 10)
                        Case Is > 4 : bonus = CSng(currlega.Settings.Bonus.BonusCenterField(5) / 10)
                    End Select
                    f.BonusCentroCampo = bonus
                Else
                    f.BonusCentroCampo = 0
                End If
                If currlega.Settings.Bonus.EnableBonusAttack AndAlso ndgooda = nat Then
                    Dim bonus As Single = 0
                    Select Case ndgooda
                        Case 2 : bonus = CSng(currlega.Settings.Bonus.BonusAttack(2) / 10)
                        Case Is > 2 : bonus = CSng(currlega.Settings.Bonus.BonusAttack(3) / 10)
                    End Select
                    f.BonusAttacco = bonus
                Else
                    f.BonusAttacco = 0
                End If

            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, "Gio=" & Giornata & " IdTeam=" & IdTeam & " Ex=" & ex.Message)
        End Try

        Return f

    End Function

    Sub SetWindowsHeight()

        chkdown.Visible = chkopt.Value
        chkextract.Visible = chkopt.Value
        chkform.Visible = chkopt.Value
        chkformtop.Visible = chkopt.Value

        If chkopt.Value Then
            Me.Height = chkformtop.Top + 120
        Else
            Me.Height = chkopt.Top + 120
        End If

    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0 : Call RunCompile()
            Case 1 : Me.Close()
        End Select
    End Sub

    Private Sub chkopt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkopt.Click
        Call SetWindowsHeight()
    End Sub

    Public Class PtPlayer

        Dim _vt As String = ""
        Dim _pt As String = ""
        Dim _ass As String = ""
        Dim _aut As String = ""
        Dim _gs As String = ""
        Dim _gf As String = ""
        Dim _rigp As String = ""
        Dim _rigs As String = ""

        Public Property vt As String
            Get
                Return _vt
            End Get
            Set(value As String)
                _vt = value
            End Set
        End Property

        Public Property pt As String
            Get
                Return _pt
            End Get
            Set(value As String)
                _pt = value
            End Set
        End Property

        Public Property ass As String
            Get
                Return _ass
            End Get
            Set(value As String)
                _ass = value
            End Set
        End Property

        Public Property aut As String
            Get
                Return _aut
            End Get
            Set(value As String)
                _aut = value
            End Set
        End Property

        Public Property gs As String
            Get
                Return _gs
            End Get
            Set(value As String)
                _gs = value
            End Set
        End Property

        Public Property gf As String
            Get
                Return _gf
            End Get
            Set(value As String)
                _gf = value
            End Set
        End Property

        Public Property rigp As String
            Get
                Return _rigp
            End Get
            Set(value As String)
                _rigp = value
            End Set
        End Property

        Public Property rigs As String
            Get
                Return _rigs
            End Get
            Set(value As String)
                _rigs = value
            End Set
        End Property

    End Class
End Class
