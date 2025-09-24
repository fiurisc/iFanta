Imports System.Linq
Imports iFanta.SystemFunction

Public Class frmcompformazioni


    Private fromIndex As Integer
    Private dragIndex As Integer
    Private dragRect As Rectangle


    Private _gg As Integer = 1
    Private _team As String = ""
    Private _start As Boolean = True
    Private _idteam As Integer = 0
    Private f As New LegaObject.Formazione()
    Private oldrow As Integer = -1
    Private frm As New frmformazioni
    Private pfree As New Dictionary(Of String, Dictionary(Of String, String))
    Private key() As String
    Private bginfo As Bitmap
    Private bginfo2 As Bitmap
    Private bginfo3 As Bitmap
    Private jlist As New Dictionary(Of Integer, Integer)
    Private tshirt As New Dictionary(Of String, Image)
    Private imgforma As Bitmap

    Sub SetParameater(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal FrmOrigin As frmformazioni)
        _gg = Giornata
        _idteam = IdTeam
        frm = FrmOrigin
    End Sub

    Private Sub frmcompformazioni_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click
        Call HideEdit()
    End Sub

    Private Sub frmcompformazioni_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        Call HideInfo()
        Call HideChart()
    End Sub

    Private Sub frmcompformazioni_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call CheckSaveState()
        currlega.CompileFormationsDetail = tlbhelp.Button(1).State
        AppSett.SaveSettings()
    End Sub

    Private Sub frmcompformazioni_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName

        If My.Computer.Name.ToLower <> "e0220634" Then
            mnuautoforma.Visible = False
        End If

        tlb1.Tag = _idteam
        tlb2.Tag = _gg - 1

        dtg1.RowTemplate.Height = 16
        dtg2.RowTemplate.Height = 16

        Dim p As System.Reflection.PropertyInfo = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        p.SetValue(dtg1, True, Nothing)
        p.SetValue(dtg2, True, Nothing)

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)
        mnu1.ImageScalingSize = New Size(16, 16)

        tlbhelp.Button(1).State = currlega.CompileFormationsDetail
        tlbhelp.draw(True)

        'Carico la lsita dei jolly giocati dai vari tema'
        jlist = currlega.GetNumbersJollyUsedByTeams

        'Verifico se sono attive le funzionalita' controllo match e player'
        If webdata.EnableFeatureData("Match") = False AndAlso webdata.EnableFeatureData("ProbableLineUps") = False Then
            tlbaction.Button(0).Visible = False
            tlbaction.Button(1).Visible = False
        End If

        'Setto il tema corrente'
        Call SetTheme()

        _start = False

        Call SetTeamName()
        Call SetDaysList()
        Call loadFormazione()
        Call SetViewMode()
        Call SetToolTipParameater()
        Call SetFreePlayerList()

        Me.Top = My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2

    End Sub

    Sub SetFreePlayerList()
        Try
            Dim r As New List(Of String)
            If currlega.Settings.Jolly.EnableJollyPlayerGoalkeeper Then r.Add("P")
            If currlega.Settings.Jolly.EnableJollyPlayerDefender Then r.Add("D")
            If currlega.Settings.Jolly.EnableJollyPlayerMidfielder Then r.Add("C")
            If currlega.Settings.Jolly.EnableJollyPlayerForward Then r.Add("A")
            pfree = currlega.GetDictionaryFreePlayerList(r)
            'Determino la lista delle key'
            ReDim key(pfree.Count - 1)
            pfree.Keys.CopyTo(key, 0)
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Sub SetToolTipParameater()

        dtginfo.BorderColor = Color.FromArgb(80, 80, 80)
        dtginfo.InternalBorderColor = Color.FromArgb(80, 80, 80)
        dtginfo.BackgroundColor = Color.FromArgb(245, 245, 245)
        dtginfo.DefaultCellStyle.BackColor = dtginfo.BackgroundColor
        dtginfo.DefaultCellStyle.SelectionBackColor = dtginfo.BackgroundColor
        dtginfo.CellBorderStyle = DataGridViewCellBorderStyle.None
        dtginfo.Columns(1).Width = dtginfo.Width - dtginfo.Columns(0).Width

        dtginfo2.BorderColor = Color.FromArgb(80, 80, 80)
        dtginfo2.InternalBorderColor = Color.FromArgb(80, 80, 80)
        dtginfo2.BackgroundColor = Color.FromArgb(245, 245, 245)
        dtginfo2.DefaultCellStyle.BackColor = dtginfo.BackgroundColor
        dtginfo2.DefaultCellStyle.SelectionBackColor = dtginfo.BackgroundColor
        dtginfo2.CellBorderStyle = DataGridViewCellBorderStyle.None

        dtginfo3.BorderColor = Color.FromArgb(80, 80, 80)
        dtginfo3.InternalBorderColor = Color.FromArgb(80, 80, 80)
        dtginfo3.BackgroundColor = Color.FromArgb(245, 245, 245)
        dtginfo3.DefaultCellStyle.BackColor = dtginfo.BackgroundColor
        dtginfo3.DefaultCellStyle.SelectionBackColor = dtginfo.BackgroundColor
        dtginfo3.CellBorderStyle = DataGridViewCellBorderStyle.None

        gra.ChartBackColor1 = Color.White
        gra.ChartBackColor2 = Color.White
        gra.BorderSize = 0
        gra.BorderColor = Color.FromArgb(50, 50, 50)
        gra.InternalBorderSize = 0
        gra.InternalBorderColor = Color.FromArgb(120, 120, 120)
        gra.Type = iChart.iChart.ChartType.Bar
        gra.ColumnCount = currlega.Settings.NumberOfDays + 1
        gra.RowCount = 1
        gra.Axsis(iChart.Axsis.Axsis.X).MinDivisionFont = New Font("tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
        gra.Axsis(iChart.Axsis.Axsis.X).TextAlignment = iChart.Axsis.Alignment.Center
        gra.Axsis(iChart.Axsis.Axsis.Y).MinDivisionFont = New Font("tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
        gra.Axsis(iChart.Axsis.Axsis.X).VisibleMinDivisionLabel = False
        gra.Series.Item(0).Style.InternalBorderSize = 1
        gra.Series.Item(0).Style.BorderSize = 1
        If AppSett.Personal.Theme.FlatStyle Then
            gra.Series.Item(0).Style.Brush.Color1 = AppSett.Personal.Theme.SelectionColor2
        Else
            gra.Series.Item(0).Style.Brush.Color1 = AppSett.Personal.Theme.SelectionColor1
            gra.Series.Item(0).Style.Brush.Color2 = AppSett.Personal.Theme.SelectionColor2
        End If
        For i As Integer = 0 To gra.ColumnCount - 1
            gra.Item(0, i) = ""
            gra.Axsis(iChart.Axsis.Axsis.X).Item(i).Value = CStr(i + 1)
        Next
        gra.Max = 10
        gra.AutoScale = iChart.iChart.eAutoScale.None
        gra.Info.Type = iChart.Info.eType.Value
        gra.Info.Font = New Font("tahoma", 11, FontStyle.Bold, GraphicsUnit.Pixel)
        gra.BH = -10
        gra.TH = 2
        gra.Top = 0
        gra.Left = 0
        gra.Width = pnlchart.Width - 15
        gra.Height = pnlchart.Height - 40

        Dim myGraphicsPath As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

        myGraphicsPath.AddRectangle(New Rectangle(1, 0, pnlchart.Width - 2, pnlchart.Height))
        myGraphicsPath.AddRectangle(New Rectangle(0, 1, 1, pnlchart.Height - 2))
        myGraphicsPath.AddRectangle(New Rectangle(pnlchart.Width - 1, 1, 1, pnlchart.Height - 2))

        'Associo l'area al pnlchart'
        pnlchart.Region = New Region(myGraphicsPath)

        myGraphicsPath.Dispose()

    End Sub

    Sub SetTheme()

        tlb1.Left = IForm1.LX + padd
        tlb1.Top = padd \ 2 + IForm1.TY + 6
        tlb1.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlb1.BorderColor = txtsearch.BorderColor
        tlb1.BorderColorDropDown = txtsearch.BorderColor
        txtsearch.Left = tlb1.Left + tlb1.Width - 1
        txtsearch.Top = tlb1.Top
        txtsearch.BringToFront()

        tlb2.Top = txtsearch.Top
        tlb2.Left = txtsearch.Left + txtsearch.Width - 1
        tlb2.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlb2.Button(0).SubWidth = txtsearch.Width + tlb2.Button(0).Width - 1
        tlb2.BorderColor = txtsearch.BorderColor
        tlb2.BorderColorDropDown = txtsearch.BorderColor

        txtgg.Left = tlb2.Left + tlb2.Width - 1
        txtgg.Top = tlb2.Top
        txtgg.BringToFront()
        txtgg.BorderColor = txtsearch.BorderColor

        tlbgglist.Left = txtgg.Left + txtgg.Width - 1
        tlbgglist.Top = tlb2.Top
        tlbgglist.Height = txtgg.Height
        tlbgglist.BorderColor = txtgg.BorderColor
        tlbgglist.BorderColorDropDown = tlbgglist.BorderColor
        tlbgglist.Button(0).SubWidth = txtgg.Width + tlbgglist.Button(0).Width - 1
        tlbgglist.Button(0).SubItemsAutoSize = False
        tlbgglist.FlatStyle = AppSett.Personal.Theme.FlatStyle

        tlbmodulo.Top = tlb1.Top + 1
        tlbmodulo.Left = IForm1.RX - tlbmodulo.Width - padd - 2

        lntop.Left = padd + IForm1.LX
        lntop.Top = txtsearch.Top + txtsearch.Height + 6
        lntop.Width = IForm1.RX - IForm1.LX - padd * 2

        dtg1.Left = padd + IForm1.LX
        dtg1.Top = lntop.Top + lntop.Height - 3
        dtg1.Height = dtg1.RowTemplate.Height * 25 + dtg1.ColumnHeadersHeight + 5

        For i As Integer = 0 To dtg1.ColumnCount - 1
            dtg1.Columns(i).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            dtg1.Columns(i).SortMode = DataGridViewColumnSortMode.Programmatic
        Next


        dtg2.Height = dtg2.RowTemplate.Height * currlega.Settings.NumberOfReserve + dtg2.ColumnHeadersHeight

        piccampo.Top = dtg1.Top + 1
        piccampo.Left = IForm1.RX - padd - piccampo.Width
        piccampo.Height = lnbot.Top - dtg2.Height - 33 - lnpanc.Height - piccampo.Top
        If piccampo.Height > 260 Then
            piccampo.Height = 260
        End If

        lnpanc.Top = piccampo.Top + piccampo.Height + 10
        lnpanc.Width = piccampo.Width
        lnpanc.Left = piccampo.Left
        dtg2.Top = lnpanc.Top + lnpanc.Height

        If (currlega.Settings.NumberOfReserve > 7) Then
            'lnpanc.Top = lntop.Top
            'piccampo.Visible = False
        Else
            lnpanc.Top = piccampo.Top + piccampo.Height + 10
        End If

        dtg2.Left = lnpanc.Left
        'dtg2.Top = lnpanc.Top + lnpanc.Height + 3
        dtg2.Width = piccampo.Width

        dtg2.Columns(0).Width = 20
        dtg2.Columns(1).Width = 110
        dtg2.Columns(2).Width = dtg2.Width - 178
        dtg2.Columns(3).Width = 16
        dtg2.Columns(4).Width = 16
        dtg2.Columns(5).Width = 16
        dtg2.Columns(1).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
        dtg2.Columns(2).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        lnbot.Top = dtg1.Top + dtg1.Height
        lnbot.Left = dtg1.Left
        lnbot.Width = lntop.Width

        tlbaction.Left = IForm1.RX - tlbaction.Width - padd
        tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
        tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle
        For i As Integer = 0 To tlbaction.Button.Count - 1
            tlbaction.Button(i).BorderColor = Color.DimGray
        Next

        tlbhelp.Top = lnbot.Top + lnbot.Height
        tlbhelp.Left = dtg1.Left

        dtg1.Columns("pgiol").DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 140)
        dtg1.Columns("ptitl").DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 140)

        dtginfo.BringToFront()
        dtginfo2.BringToFront()
        dtginfo3.BringToFront()

        Me.Height = dtg1.Top + dtg1.Height + 55

    End Sub

    Sub HideChart()
        pnlchart.Visible = False
    End Sub

    Sub HideInfo()
        oldrow = -1
        dtginfo.Visible = False
        dtginfo2.Visible = False
        dtginfo3.Visible = False
    End Sub

    Sub HideEdit()
        txtstar1.Visible = False
    End Sub

    Sub CheckSaveState()
        If tlbaction.Button(3).Enabled Then
            If iControl.iMsgBox.ShowMessage("Salvare i cambiamenti effettuati?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.OK Then
                Call Save()
            Else
                tlbaction.Button(3).Enabled = False
                tlbaction.draw(True)
            End If
        End If
    End Sub

    Sub loadFormazione()

        'Carico la formazione'

        Try

            dtg1.Visible = False
            dtg2.Visible = False

            Call HideChart()
            Call HideInfo()
            Call HideEdit()

            tlb2.Button(0).ClearSubButtonSelection()
            tlb2.Button(0).SetSubButtonSelection(txtsearch.Text, True)

            tlbgglist.Button(0).ClearSubButtonSelection()
            tlbgglist.Button(0).SetSubButtonSelection(CStr(_gg), True)

            f.IdTeam = _idteam
            f.Giornata = _gg
            f.Nome = txtsearch.Text
            f.Load()

            dtg1.Rows.Clear()
            dtg2.Rows.Clear()
            dtg1.RowCount = 25
            dtg2.RowCount = currlega.Settings.NumberOfReserve

            Dim ennjolly As Boolean = False

            For i As Integer = 0 To f.Players.Count - 1

                Dim p As LegaObject.Formazione.PlayerFormazione = f.Players(i)

                If p.Type <> 3 Then

                    Dim idf As Integer = p.IdRosa - 1
                    Dim enajolly As Boolean = currlega.Settings.Jolly.EnableJollyPlayer

                    f.Players(i).Rating = SystemFunction.General.GetRatingForma(p, _gg)

                    If jlist.ContainsKey(_idteam) AndAlso jlist(_idteam) >= currlega.Settings.Jolly.MaximumNumberJollyPlayable Then
                        enajolly = False
                    End If

                    dtg1.Rows(idf).Height = 16

                    dtg1.Rows(idf).Tag = i

                    If enajolly Then
                        Select Case p.Ruolo
                            Case "P" : enajolly = currlega.Settings.Jolly.EnableJollyPlayerGoalkeeper
                            Case "D" : enajolly = currlega.Settings.Jolly.EnableJollyPlayerDefender
                            Case "C" : enajolly = currlega.Settings.Jolly.EnableJollyPlayerMidfielder
                            Case "A" : enajolly = currlega.Settings.Jolly.EnableJollyPlayerForward
                        End Select
                    End If
                    If enajolly Then
                        If p.Jolly = 1 Then
                            dtg1.Item("star", idf).Value = My.Resources.star_sel
                            dtg1.Item("star", idf).Tag = 1
                        Else
                            dtg1.Item("star", idf).Value = My.Resources.star_uns
                            dtg1.Item("star", idf).Tag = 0
                        End If
                    Else
                        dtg1.Item("star", idf).Value = My.Resources.empty
                        dtg1.Item("star", idf).Tag = -1
                    End If

                    dtg1.Item("gtit", idf).Value = My.Resources.ok_dis16
                    dtg1.Item("gtit", idf).Tag = 0
                    dtg1.Item("gpanc", idf).Value = My.Resources.ok_dis16
                    dtg1.Item("gpanc", idf).Tag = 0

                    If p.Type = 1 Then
                        dtg1.Item("gtit", idf).Value = My.Resources.ok16
                        dtg1.Item("gtit", idf).Tag = 1
                    End If
                    If p.Type = 2 Then
                        dtg1.Item("gpanc", idf).Value = My.Resources.ok16b
                        dtg1.Item("gpanc", idf).Tag = 1
                    End If
                    dtg1.Item("ruolo", idf).Style.ForeColor = SystemFunction.General.GetRuoloForeColor(p.Ruolo)
                    dtg1.Item("ruolo", idf).Value = p.Ruolo
                    dtg1.Item("nome", idf).Value = p.Nome

                    dtg1.Item("squadra", idf).Tag = p.Squadra
                    If p.Squadra.Length > 3 Then
                        dtg1.Item("squadra", idf).Value = p.Squadra.Substring(0, 3)
                    Else
                        dtg1.Item("squadra", idf).Value = p.Squadra
                    End If
                    dtg1.Item("squadra", idf).Tag = p.Squadra
                    dtg1.Item("gs", idf).Value = General.SetFieldIntegerData(p.StatisticAll.Gs, "-")
                    dtg1.Item("gf", idf).Value = General.SetFieldIntegerData(p.StatisticAll.Gf, "-")
                    dtg1.Item("amm", idf).Value = General.SetFieldIntegerData(p.StatisticAll.Amm, "-")
                    dtg1.Item("esp", idf).Value = General.SetFieldIntegerData(p.StatisticAll.Esp, "-")
                    dtg1.Item("ass", idf).Value = General.SetFieldIntegerData(p.StatisticAll.Ass, "-")
                    dtg1.Item("pgio", idf).Value = General.SetFieldIntegerData(p.StatisticAll.pGiocate, "-")
                    dtg1.Item("avgpt", idf).Value = General.SetFieldSingleData(p.StatisticAll.Avg_Pt, "-")
                    dtg1.Item("pgiol", idf).Value = General.SetFieldIntegerData(p.StatisticLast.pGiocate, "-")
                    dtg1.Item("ptitl", idf).Value = General.SetFieldIntegerData(p.StatisticLast.Titolare, "-")
                    dtg1.Item("var", idf).Value = DrawingAndImage.GetPlayerVariationImage(p.Variation)

                    If webdata.WebPlayers.ContainsKey(f.Giornata & "/" & p.Nome & "/" & p.Squadra) Then
                        Dim wp As wData.wPlayer = webdata.WebPlayers(f.Giornata & "/" & p.Nome & "/" & p.Squadra)
                        dtg1.Item("presenza", idf).Tag = wp
                        'Controllo se e' infortunato o squalificarti'
                        Dim t As String = ""
                        For k As Integer = 0 To wp.Info.Count - 1
                            If wp.Info(k).State = "Infortunato" Then
                                t = "i"
                                Exit For
                            ElseIf wp.Info(k).State = "Squalificato" Then
                                t = "s"
                                Exit For
                            End If
                        Next
                        If t <> "" Then
                            dtg1.Item("infort", idf).Tag = wp
                            If t = "i" Then
                                dtg1.Item("infort", idf).Value = My.Resources.infortunato
                            Else
                                dtg1.Item("infort", idf).Value = My.Resources.espulso
                            End If
                        Else
                            dtg1.Item("infort", idf).Tag = Nothing
                            dtg1.Item("infort", idf).Value = My.Resources.empty
                        End If
                    Else
                        dtg1.Item("infort", idf).Value = My.Resources.empty
                        dtg1.Item("infort", idf).Tag = Nothing
                        dtg1.Item("presenza", idf).Tag = Nothing
                    End If

                    dtg1.Item("dtmatch", idf).Value = p.Match.Time.ToString("dd/MM HH:mm")
                    dtg1.Item("rating", idf).ToolTipText = CStr(p.Rating)
                    dtg1.Item("match", idf).Tag = Nothing
                    Select Case p.Variation
                        Case 1
                            dtg1.Item("var", idf).Value = My.Resources.import14
                        Case 0
                            dtg1.Item("var", idf).Value = My.Resources.w4
                        Case Else
                            dtg1.Item("var", idf).Value = My.Resources.export14
                    End Select
                End If
            Next


            ' Compilo la lista dei panchinari '
            For i As Integer = 0 To dtg2.Rows.Count - 1
                dtg2.Item("psquadra", i).Tag = ""
            Next

            For i As Integer = 0 To f.Players.Count - 1
                If f.Players(i).Type = 2 Then
                    Dim r As Integer = f.Players(i).IdFormazione - 12
                    If r > -1 AndAlso r < dtg2.RowCount Then
                        Dim ns As String = f.Players(i).Squadra
                        If ns.Length > 3 Then ns = ns.Substring(0, 3)
                        dtg2.Item("pruolo", r).Style.ForeColor = SystemFunction.General.GetRuoloForeColor(f.Players(i).Ruolo)
                        dtg2.Item("pruolo", r).Value = f.Players(i).Ruolo
                        dtg2.Item("pnome", r).Value = f.Players(i).Nome
                        dtg2.Item("psquadra", r).Value = ns
                        dtg2.Item("psquadra", r).Tag = f.Players(i).Squadra
                    End If
                End If
            Next

            dtg1.Visible = True
            dtg2.Visible = True

            Call DrawFormazione()

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

    End Sub

    Sub RemovePanchina(ByVal Nome As String)
        For i As Integer = 0 To dtg2.RowCount - 1
            If CStr(dtg2.Item("pnome", i).Value) = Nome Then
                dtg2.Item("pruolo", i).Value = ""
                dtg2.Item("pnome", i).Value = ""
                dtg2.Item("psquadra", i).Value = ""
                dtg2.Item("psquadra", i).Tag = ""
            End If
        Next
    End Sub

    Sub AddPanchina(Ruolo As String, ByVal Nome As String, Squadra As String)
        For i As Integer = 0 To dtg2.RowCount - 1
            If CStr(dtg2.Item("pnome", i).Value) = "" Then
                If (i > 0 OrElse (Ruolo = "P" OrElse currlega.Settings.ForcedGoalkeeperAsFirstReserve = False)) Then
                    dtg2.Item("pruolo", i).Style.ForeColor = SystemFunction.General.GetRuoloForeColor(Ruolo)
                    dtg2.Item("pruolo", i).Value = Ruolo
                    dtg2.Item("pnome", i).Value = Nome
                    dtg2.Item("psquadra", i).Tag = Squadra
                    Dim ns As String = f.Players(i).Squadra
                    If ns.Length > 3 Then ns = ns.Substring(0, 3)
                    dtg2.Item("psquadra", i).Value = ns
                    Exit For
                End If
            End If
        Next
    End Sub

    Function CheckPanchina() As Boolean

        Dim ris As Boolean = False

        For i As Integer = 0 To dtg2.RowCount - 1
            If CStr(dtg2.Item("pnome", i).Value) = "" Then
                ris = True
                Exit For
            End If
        Next
        Return ris

    End Function

    Sub SetTeamName()
        'Imposto l'elenco delle squadre'
        tlb2.Button(0).ClearSubButtonSelection()
        For i As Integer = 0 To currlega.Teams.Count - 1
            tlb2.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(currlega.Teams(i).Nome))
            'If currlega.Teams(i).IdTeam = _idteam Then tlb2.Button(0).SubItems(i).State = True
        Next
        txtsearch.Text = currlega.Teams(_idteam).Nome
    End Sub

    Sub SetDaysList()
        'Imposto l'elenco delle squadre'
        tlbgglist.Button(0).ClearSubButtonSelection()
        For i As Integer = 0 To currlega.Settings.NumberOfDays - 1
            tlbgglist.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(CStr(i + 1)))
            If i + 1 = _gg Then tlbgglist.Button(0).SubItems(i).State = True
        Next
        txtgg.Text = CStr(_gg)
    End Sub

    Function CheckModule(Ruolo As String) As Boolean

        Dim p As Integer = 0
        Dim d As Integer = 0
        Dim c As Integer = 0
        Dim a As Integer = 0
        Dim tot As Integer

        For i As Integer = 0 To dtg1.RowCount - 1
            If CInt(dtg1.Item("gtit", i).Tag) = 1 Then
                Select Case dtg1.Item("ruolo", i).Value.ToString()
                    Case "P" : p = p + 1
                    Case "D" : d = d + 1
                    Case "C" : c = c + 1
                    Case "A" : a = a + 1
                End Select
            End If
        Next

        tot = p + d + c + a

        If tot = 11 Then
            Return False
        Else
            Return SystemFunction.General.CheckMudule(Ruolo, p, d, c, a)
        End If

    End Function

    Sub DrawFormazione()

        Dim p As New List(Of PlayerForma)
        Dim d As New List(Of PlayerForma)
        Dim c As New List(Of PlayerForma)
        Dim a As New List(Of PlayerForma)

        For i As Integer = 0 To dtg1.RowCount - 1
            If CInt(dtg1.Item(1, i).Tag) = 1 Then
                Dim r As String = dtg1.Item("ruolo", i).Value.ToString
                Dim n As String = dtg1.Item("nome", i).Value.ToString
                Dim s As String = dtg1.Item("squadra", i).Value.ToString
                If n.Length > 8 Then n = n.Substring(0, 8)
                Select Case r
                    Case "P" : p.Add(New PlayerForma(n, s))
                    Case "D" : d.Add(New PlayerForma(n, s))
                    Case "C" : c.Add(New PlayerForma(n, s))
                    Case "A" : a.Add(New PlayerForma(n, s))
                End Select
            End If
        Next

        Dim gr As Graphics
        Dim b1 As New Bitmap(piccampo.Width, piccampo.Height)
        gr = Graphics.FromImage(b1)
        gr.DrawImage(My.Resources.campo, 0, 0)
        Dim dy As Integer = 10
        If p.Count > 0 Then
            dy = DrawFormazionePlayer(gr, p, dy)
        End If
        If d.Count > 0 Then
            dy = DrawFormazionePlayer(gr, d, dy)
        End If
        If c.Count > 0 Then
            dy = DrawFormazionePlayer(gr, c, dy)
        End If
        If a.Count > 0 Then
            dy = DrawFormazionePlayer(gr, a, dy)
        End If
        gr.DrawImage(My.Resources.campo_botton, New PointF(0, piccampo.Height - My.Resources.campo_botton.Height))
        piccampo.Image = CType(b1.Clone, Image)
        b1.Dispose()
        gr.Dispose()

        tlbmodulo.Button(1).Text = CStr(d.Count) & "-" & CStr(c.Count) & "-" & CStr(a.Count)
        tlbmodulo.draw(True)

        Call CheckJollyPlayable()

        imgforma = Nothing

    End Sub

    Sub CheckJollyPlayable()

        'Controlla la disponibilita' di inserimento jolly'
        Dim njolly As Integer = 0
        Dim enajolly As Boolean = currlega.Settings.Jolly.EnableJollyPlayer

        If enajolly Then
            If jlist.ContainsKey(_idteam) AndAlso jlist(_idteam) >= currlega.Settings.Jolly.MaximumNumberJollyPlayable Then
                enajolly = False
            Else
                For i As Integer = 0 To dtg1.RowCount - 1
                    If CInt(dtg1.Item(0, i).Tag) = 1 Then
                        njolly += 1
                        If njolly >= currlega.Settings.Jolly.MaximumNumberJollyPlayableForDay OrElse (jlist.ContainsKey(_idteam) AndAlso njolly + jlist(_idteam) >= currlega.Settings.Jolly.MaximumNumberJollyPlayable) Then enajolly = False : Exit For
                    End If
                Next
            End If
        End If

        Call SetEnableJollyPlayer(enajolly)

    End Sub

    Sub SetEnableJollyPlayer(ByVal EnableJolly As Boolean)
        For i As Integer = 0 To dtg1.RowCount - 1
            If EnableJolly Then
                If CInt(dtg1.Item("star", i).Tag) <> 1 Then
                    Dim ena As Boolean = False
                    Select Case CStr(dtg1.Item("ruolo", i).Value)
                        Case "P" : If currlega.Settings.Jolly.EnableJollyPlayerGoalkeeper Then ena = True
                        Case "D" : If currlega.Settings.Jolly.EnableJollyPlayerDefender Then ena = True
                        Case "C" : If currlega.Settings.Jolly.EnableJollyPlayerMidfielder Then ena = True
                        Case "A" : If currlega.Settings.Jolly.EnableJollyPlayerForward Then ena = True
                    End Select
                    If ena Then
                        dtg1.Item("star", i).Value = My.Resources.star_uns
                        dtg1.Item("star", i).Tag = 0
                    Else
                        dtg1.Item("star", i).Value = My.Resources.empty
                        dtg1.Item("star", i).Tag = -1
                    End If
                End If
            Else
                If CInt(dtg1.Item("star", i).Tag) <> -1 AndAlso CInt(dtg1.Item("star", i).Tag) <> 1 Then
                    dtg1.Item("star", i).Value = My.Resources.empty
                    dtg1.Item("star", i).Tag = -1
                End If
            End If
        Next

    End Sub

    Function DrawFormazionePlayer(ByVal gr As Graphics, ByVal plist As List(Of PlayerForma), ByVal topspace As Integer) As Integer

        Dim ft As New StringFormat
        Dim paddf As Integer = 0
        Dim wg As Integer = piccampo.Width - paddf * 2
        Dim w As Integer = 0

        ft.Alignment = StringAlignment.Center

        If plist.Count < 4 Then
            w = wg \ plist.Count
        Else
            w = wg \ 3
        End If

        Dim img As Image = My.Resources.empty
        Dim col As Integer = 0
        Dim row As Integer = 0
        Dim dyrow = (piccampo.Height - 50) \ 4

        For i As Integer = 0 To plist.Count - 1
            gr.DrawImage(SystemFunction.DrawingAndImage.DrawGlowText3(plist(i).Nome, New Font("Tahoma", 9, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, Color.Black, 20, False, False, ft, w, 15), (w * col) + paddf, topspace + row * dyrow)
            gr.DrawImage(My.Resources.point, (w * col) + paddf + w \ 2 - My.Resources.point.Width \ 2, topspace + 13 + row * dyrow)
            col = col + 1
            If col > 2 AndAlso i < plist.Count - 1 Then
                col = 0
                row = row + 1
                If plist.Count - row * 3 < 4 Then
                    w = wg \ (plist.Count - row * 3)
                Else
                    w = wg \ 3
                End If
            End If
        Next
        Return topspace + (row + 1) * dyrow
    End Function

    Sub SetViewMode()

        Dim s As Boolean = tlbhelp.Button(1).State
        Dim w As Integer = 0

        For i As Integer = 0 To dtg1.Columns.Count - 1
            Select Case dtg1.Columns(i).Name
                Case "gs", "gf", "amm", "esp", "ass", "pgio", "avgpt", "pgiol", "ptitl"
                    dtg1.Columns(i).Visible = s
                    If s Then w = w + dtg1.Columns(i).Width
                Case Else
                    w = w + dtg1.Columns(i).Width
            End Select
        Next

        If CDbl(txtgg.Text) = webdata.DayProbableFormation Then
            dtg1.Columns("infort").Visible = True
            dtg1.Columns("presenza").Visible = True
        Else
            dtg1.Columns("infort").Visible = False
            dtg1.Columns("presenza").Visible = False
            w = w - dtg1.Columns("infort").Width - dtg1.Columns("presenza").Width
        End If

        dtg1.Width = w + 5

        Me.Width = dtg1.Left + w + piccampo.Width + padd + 20
        Me.Left = My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2

    End Sub

    Sub SetButtonSaveState(ByVal Enable As Boolean)
        If _start Then Exit Sub
        If tlbaction.Button(3).Enabled <> Enable Then
            tlbaction.Button(3).Enabled = Enable
            tlbaction.draw(True)
        End If
        mnusave.Enabled = tlbaction.Button(3).Enabled
    End Sub

    Sub Save()
        Try
            'Salvo la formazione'
            Dim fs As New LegaObject.Formazione
            Dim idf As Integer = 1

            fs.Giornata = f.Giornata
            fs.IdTeam = f.IdTeam

            For i As Integer = 0 To dtg1.RowCount - 1

                Dim r As String = f.Players(i).Ruolo
                Dim n As String = f.Players(i).Nome
                Dim sq As String = f.Players(i).Squadra
                Dim tit As Integer = CInt(dtg1.Item(1, i).Tag)
                Dim panc As Integer = CInt(dtg1.Item(2, i).Tag)
                Dim jolly As Integer = CInt(dtg1.Item(0, i).Tag)

                If panc = 0 Then
                    If jolly = 1 Then
                        r = CStr(dtg1.Item("ruolo", i).Value)
                        n = CStr(dtg1.Item("nome", i).Value)
                        sq = CStr(dtg1.Item("squadra", i).Tag)
                    Else
                        jolly = 0
                    End If
                    If tit = 1 Then
                        fs.Players.Add(New LegaObject.Formazione.PlayerFormazione(f.Players(i).IdRosa, jolly, 1, idf, r, n, 1))
                        idf += 1
                    Else
                        fs.Players.Add(New LegaObject.Formazione.PlayerFormazione(f.Players(i).IdRosa, jolly, 0, 0, r, n, 1))
                    End If
                Else
                    Dim idfp = 12
                    For j As Integer = 0 To dtg2.RowCount - 1
                        If CStr(dtg2.Item("pnome", j).Value) <> "" Then
                            If CStr(dtg2.Item("pruolo", j).Value) = r AndAlso CStr(dtg2.Item("pnome", j).Value) = n AndAlso CStr(dtg2.Item("psquadra", j).Tag) = sq Then
                                fs.Players.Add(New LegaObject.Formazione.PlayerFormazione(f.Players(i).IdRosa, jolly, 2, idfp, r, n, 0))
                                Exit For
                            End If
                            idfp += 1
                        End If
                    Next
                End If
            Next

            'Elimino la predente formazione'
            LegaObject.Formazione.Delete(_gg, _idteam, False)
            LegaObject.Formazione.Delete(_gg, _idteam, True)
            'Salvo la formazione'
            fs.SaveBasic()
            'Cambio lo stato del pulsante di salvataggio'
            Call SetButtonSaveState(False)
            'Aggiorno il form padre'
            frm.LoadData()
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            ShowError("Errore", ex.Message)
            Call WriteError("CompFormazioni", "Save", ex.Message)
        End Try
    End Sub

    Sub Action(ByVal Act As Integer)

        mnu1.Hide()
        Try
            Select Case Act
                Case 0
                    'Controllo lo stato della connessione'
                    Dim intconn As New InternetConnection.ConnType
                    intconn = InternetConnection.Type
                    If intconn <> InternetConnection.ConnType.offline Then
                        Dim frm As New frmupdateprobforma
                        frm.ShowDialog()
                        dtg1.Refresh()
                    Else
                        ShowAlert("Avviso", "Connessione internet assente o non buona")
                    End If
                Case 2
                    LegaObject.Formazione.Delete(_gg, _idteam, False)
                    LegaObject.Formazione.Delete(_gg, _idteam, True)
                    frm.LoadData()
                    Call loadFormazione()
                Case 3
                    Call Save()
                Case 4
                    Me.Close()
                Case 5
                    'rimuovo tutti i titolari'
                    For i As Integer = 0 To dtg1.RowCount - 2
                        Dim ena As Boolean = False
                        If currlega.Settings.Jolly.EnableJollyPlayer Then
                            Select Case CStr(dtg1.Item(2, i).Value)
                                Case "P" : If currlega.Settings.Jolly.EnableJollyPlayerGoalkeeper Then ena = True
                                Case "D" : If currlega.Settings.Jolly.EnableJollyPlayerDefender Then ena = True
                                Case "C" : If currlega.Settings.Jolly.EnableJollyPlayerMidfielder Then ena = True
                                Case "A" : If currlega.Settings.Jolly.EnableJollyPlayerForward Then ena = True
                            End Select
                        End If
                        If ena Then
                            dtg1.Item(0, i).Value = My.Resources.star_uns
                            dtg1.Item(0, i).Tag = 0
                        Else
                            dtg1.Item(0, i).Value = My.Resources.empty
                            dtg1.Item(0, i).Tag = -1
                        End If
                        dtg1.Item(1, i).Tag = 0
                        dtg1.Item(1, i).Value = My.Resources.ok_dis16
                    Next
                    Call DrawFormazione()
                    Call SetButtonSaveState(True)
                Case 6
                    'rimuovo tutti i panchinari'
                    For i As Integer = 0 To dtg1.RowCount - 2
                        dtg1.Item(2, i).Tag = 0
                        dtg1.Item(2, i).Value = My.Resources.ok_dis16
                    Next
                    For i As Integer = 0 To dtg2.RowCount - 1
                        dtg2.Item(0, i).Value = ""
                        dtg2.Item(1, i).Value = ""
                        dtg2.Item(2, i).Value = ""
                        dtg2.Item(2, i).Tag = ""
                    Next
                    Call SetButtonSaveState(True)
                Case 7
                    Dim frm As New frmsvincolati
                    frm.Giornata = _gg
                    frm.Show(Me)
                Case 8
                    My.Computer.Clipboard.SetImage(SystemFunction.DrawingAndImage.ConvertDatagridToImage(dtg1))
                Case 9
                    Dim dlg As New Windows.Forms.FolderBrowserDialog
                    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Dim fname As String = dlg.SelectedPath & "\FORMA-COMP-" & txtsearch.Text.ToUpper & "-" & txtgg.Text.ToUpper.PadLeft(2, CChar("0")) & ".png"
                        SystemFunction.DrawingAndImage.SaveDatagridToImage(dtg1, fname)
                    End If
                Case 10
                    Call CompileAutomatic()
            End Select
        Catch ex As Exception
            ShowError("Errore", ex.Message)
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

    End Sub

    Sub CompileAutomatic()

        Dim fc As New List(Of LegaObject.Formazione.PlayerFormazione)
        Dim ind As Integer = 0

        'Resetto la formazione corrente'
        Call Action(5)
        Call Action(6)

        'Determino i giocatori della formazione automatica'
        fc = currlega.GetAutomaticFormation(f.IdTeam, _gg)

        'Imposto i giocatori'
        For i As Integer = 0 To dtg1.RowCount - 1
            dtg1.Item(1, i).Tag = 0
            dtg1.Item(1, i).Value = My.Resources.ok_dis16
        Next
        For i As Integer = 0 To fc.Count - 1
            Select Case fc(i).Type
                Case 0
                    For k As Integer = 0 To f.Players.Count - 1
                        If fc(i).Nome = f.Players(k).Nome AndAlso fc(i).Squadra = f.Players(k).Squadra Then
                            dtg1.Item(1, k).Tag = 1
                            dtg1.Item(1, k).Value = My.Resources.ok16
                            Exit For
                        End If
                    Next
                Case 1
                    If ind < dtg2.RowCount + 1 Then
                        Dim ns As String = fc(i).Squadra
                        If ns.Length > 3 Then ns = ns.Substring(0, 3)
                        ind = fc(i).IdRosa - 1
                        dtg2.Item(0, ind).Style.ForeColor = SystemFunction.General.GetRuoloForeColor(fc(i).Ruolo)
                        dtg2.Item(0, ind).Value = fc(i).Ruolo
                        dtg2.Item(1, ind).Value = fc(i).Nome
                        dtg2.Item(2, ind).Value = ns
                    End If
            End Select
        Next

        'Aggiorno la schermata'
        Call DrawFormazione()
        Call SetButtonSaveState(True)

    End Sub

    Sub ShowEdit1(ByVal Ruolo As String, ByVal Nome As String, ByVal RowIndex As Integer)
        Try
            txtstar1.Visible = False
            txtstar1.Text = Nome
            txtstar1.AutoCompleteList.Clear()
            If pfree.ContainsKey(Ruolo) Then txtstar1.AutoCompleteList.AddRange(pfree(Ruolo).Keys.ToList())
            txtstar1.Tag = RowIndex
            txtstar1.ForeColor = dtg1.DefaultCellStyle.ForeColor
            Dim rec As Rectangle = dtg1.GetCellDisplayRectangle(3, RowIndex, True)
            txtstar1.Location = New Point(rec.Left + dtg1.Left, rec.Top + dtg1.Top - 1)
            txtstar1.Size = New Size(rec.Width, rec.Height + 1)
            txtstar1.Visible = True
            txtstar1.Focus()
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If _start Then Exit Sub
        Call SetTheme()
    End Sub

    Private Sub tlb2_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlb2.SubButtonClick
        Call CheckSaveState()
        txtsearch.Text = tlb2.Button(0).SubItems(SubButtonIndex).Text
        _idteam = SubButtonIndex
        tlb2.Button(0).ClearSubButtonSelection()
        tlb2.Button(0).SubItems(SubButtonIndex).State = True
        Call loadFormazione()
    End Sub

    Private Sub dtg1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellClick

        If e.RowIndex = -1 Then Exit Sub

        Try

            Call HideEdit()

            If e.ColumnIndex = 0 Then

                Dim ris As Boolean = CheckModule(dtg1.Item("ruolo", e.RowIndex).Value.ToString())
                If CInt(dtg1.Item(1, e.RowIndex).Tag) = 1 OrElse CInt(dtg1.Item(2, e.RowIndex).Tag) = 1 Then
                    ris = True
                End If
                Dim star As Integer = CInt(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag)
                If star = -1 Then Exit Sub
                If star = 0 Then star = 1 Else star = 0
                If ris = False Then star = 0
                dtg1.Item(e.ColumnIndex, e.RowIndex).Tag = star

                If ris Then
                    If star = 1 Then
                        dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.star_sel
                        txtstar1.Visible = False
                        txtstar1.Text = ""
                        txtstar1.AutoCompleteList.Clear()
                        If pfree.ContainsKey(f.Players(e.RowIndex).Ruolo) Then txtstar1.AutoCompleteList.AddRange(pfree(f.Players(e.RowIndex).Ruolo).Keys.ToList())
                        txtstar1.Tag = e.RowIndex
                        txtstar1.ForeColor = dtg1.DefaultCellStyle.ForeColor
                        Dim rec As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex + 4, e.RowIndex, True)
                        txtstar1.Location = New Point(rec.Left + dtg1.Left, rec.Top + dtg1.Top - 1)
                        txtstar1.Size = New Size(rec.Width, rec.Height + 1)
                        txtstar1.Visible = True
                        txtstar1.Focus()
                    Else
                        dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.star_uns
                        dtg1.Item("nome", e.RowIndex).Value = f.Players(e.RowIndex).Nome
                        txtstar1.Visible = False
                    End If
                    Call SetButtonSaveState(True)
                    Call DrawFormazione()
                End If

            ElseIf e.ColumnIndex = 1 Then

                Dim ris As Boolean = CheckModule(dtg1.Item("ruolo", e.RowIndex).Value.ToString())
                Dim st As Integer = CInt(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag)

                If st = 0 Then
                    st = 1
                Else
                    st = 0
                End If
                If ris = False Then st = 0

                dtg1.Item(e.ColumnIndex, e.RowIndex).Tag = st

                If st = 1 Then
                    If ris Then
                        dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.ok16
                        If CInt(dtg1.Item(2, e.RowIndex).Tag) = 1 Then
                            dtg1.Item(2, e.RowIndex).Tag = 0
                            dtg1.Item(2, e.RowIndex).Value = My.Resources.ok_dis16
                            RemovePanchina(Convert.ToString(dtg1.Item("nome", e.RowIndex).Value))
                        End If
                        Call DrawFormazione()
                        Call SetButtonSaveState(True)
                    End If
                Else
                    dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.ok_dis16
                    Call DrawFormazione()
                    Call SetButtonSaveState(True)
                End If

            ElseIf e.ColumnIndex = 2 Then

                Dim st As Integer = CInt(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag)

                If st = 0 Then
                    st = 1
                Else
                    st = 0
                End If
                If st = 1 AndAlso CheckPanchina() Then
                    If CInt(dtg1.Item(1, e.RowIndex).Tag) = 1 Then
                        dtg1.Item(1, e.RowIndex).Value = My.Resources.ok_dis16
                        dtg1.Item(1, e.RowIndex).Tag = 0
                        Call DrawFormazione()
                    End If
                    dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.ok16b
                    Call AddPanchina(Convert.ToString(dtg1.Item("ruolo", e.RowIndex).Value), Convert.ToString(dtg1.Item("nome", e.RowIndex).Value), Convert.ToString(dtg1.Item("squadra", e.RowIndex).Tag))
                    Call SetButtonSaveState(True)
                Else
                    dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.ok_dis16
                    Call RemovePanchina(Convert.ToString(dtg1.Item("nome", e.RowIndex).Value))
                    Call SetButtonSaveState(True)
                End If
                dtg1.Item(e.ColumnIndex, e.RowIndex).Tag = st
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

    End Sub

    Private Sub dtg1_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellMouseEnter
        dtginfo2.Visible = False
        If e.RowIndex = -1 Then Call HideChart() : Call HideInfo() : Exit Sub
        Try
            Select Case dtg1.Columns(e.ColumnIndex).HeaderText
                Case "amm", "esp", "ass", "p.g.", "m.p.", "m.v.", "gs", "gf"
                    If CStr(dtg1.Item("nome", e.RowIndex).Value) <> "" AndAlso CStr(dtg1.Item(e.ColumnIndex, e.RowIndex).Value) <> "-" Then
                        Timer1.Stop()
                        Timer1.Start()
                        Dim r As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                        If r.Top + r.Height + pnlchart.Height + dtg1.Top > Me.Height Then
                            pnlchart.Location = New Point(r.Left + r.Width \ 2 - pnlchart.Width \ 2, r.Top + dtg1.Top - gra.Height + 1)
                        Else
                            pnlchart.Location = New Point(r.Left + r.Width \ 2 - pnlchart.Width \ 2, r.Top + dtg1.Top + r.Height - 1)
                        End If
                        gra.Tag = dtg1.Columns(e.ColumnIndex).HeaderText & "," & CStr(dtg1.Item("nome", e.RowIndex).Value)
                    End If
                Case Else
                    Timer1.Stop()
                    Call HideChart()
            End Select

            If dtg1.Columns(e.ColumnIndex).Name = "infort" OrElse dtg1.Columns(e.ColumnIndex).Name = "presenza" OrElse dtg1.Columns(e.ColumnIndex).Name = "match" Then
                If dtg1.Columns(e.ColumnIndex).Name = "infort" OrElse dtg1.Columns(e.ColumnIndex).Name = "presenza" Then
                    If dtg1.Item(e.ColumnIndex, e.RowIndex).Tag IsNot Nothing Then

                        Dim wp As wData.wPlayer = CType(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag, wData.wPlayer)
                        Dim r As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                        Dim x As Integer = r.Left + r.Width + dtg1.Left
                        Dim y As Integer = r.Top + dtg1.Top + r.Height \ 2

                        bginfo = SystemFunction.Gui.ShowPopUpInfoPlayer(dtginfo, wp, x, y, IForm1)

                    End If
                Else
                    Call HideInfo()
                End If
            Else
                Call HideInfo()
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            Call HideInfo()
            Call HideChart()
        End Try
    End Sub

    Private Sub tlbhelp_buttonClick(ByVal sender As Object, ByVal e As System.EventArgs, buttonindex As Integer) Handles tlbhelp.ButtonClick
        Call SetViewMode()
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Try
            Call Action(ButtonIndex)
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Private Sub tlbgg_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbgglist.SubButtonClick
        Call CheckSaveState()
        txtgg.Text = CStr(SubButtonIndex + 1)
        _gg = SubButtonIndex + 1
        Call SetViewMode()
        Call loadFormazione()
    End Sub

    Private Sub dtginfo_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtginfo.CellMouseEnter
        Try
            If dtginfo.Tag IsNot Nothing Then

                Dim site As String = dtginfo.Item(1, e.RowIndex).Value.ToString.Replace("Prob. form.", "Probabili formazioni ").ToLower
                Dim key As String = CStr(dtginfo.Item(1, e.RowIndex).Tag).ToLower & "-" & site

                If webdata.WebTeamPlayers.ContainsKey(key) Then

                    Dim r As Rectangle = dtginfo.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                    Dim p As wData.wPlayer = CType(dtginfo.Tag, wData.wPlayer)

                    If p IsNot Nothing Then
                        Dim t As String = p.Team
                        If CStr(dtginfo3.Tag) <> t & e.RowIndex Then
                            dtginfo3.Tag = t & e.RowIndex
                            Call ShowInfo3(r, p.Team, p.Name, site)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Private Sub dtginfo_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles dtginfo.RowPostPaint
        SystemFunction.Gui.DrawInfoPresence(dtginfo, e)
    End Sub

    Private Sub dtginfo_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtginfo.RowPrePaint
        Dim r As Rectangle = New Rectangle(e.RowBounds.Left, e.RowBounds.Top, e.RowBounds.Width, e.RowBounds.Height + 1)
        e.Graphics.DrawImage(bginfo, r, r, GraphicsUnit.Pixel)
    End Sub

    Private Sub dtginfo2_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles dtginfo2.RowPostPaint
        'If dtginfo.Item(2, e.RowIndex).Value IsNot Nothing Then
        '    Dim site As String = dtginfo.Item(1, e.RowIndex).Value.ToString.ToLower
        '    Dim info As String = dtginfo.Item(2, e.RowIndex).Value.ToString
        '    Dim format As New StringFormat
        '    format.Alignment = StringAlignment.Far
        '    If info.Contains("Titolare") Then
        '        e.Graphics.DrawString(info, New Font("Arial", 11, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.RoyalBlue, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
        '    ElseIf info.Contains("Panchina") OrElse info.Contains("A disposizione") Then
        '        e.Graphics.DrawString(info, New Font("Arial", 11, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.Red, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
        '    Else
        '        e.Graphics.DrawString(info, New Font("Arial", 11, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.DimGray, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
        '    End If
        'End If
        'If CInt(dtginfo.Rows(e.RowIndex).Tag) = 1 Then
        '    e.Graphics.DrawLine(New Pen(Color.Gainsboro, 2), 4, e.RowBounds.Top + e.RowBounds.Height - 1, e.RowBounds.Width - 4, e.RowBounds.Top + e.RowBounds.Height - 1)
        'End If
    End Sub

    Private Sub dtginfo2_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtginfo2.RowPrePaint
        Dim r As Rectangle = New Rectangle(e.RowBounds.Left, e.RowBounds.Top, e.RowBounds.Width, e.RowBounds.Height + 1)
        e.Graphics.DrawImage(bginfo2, r, r, GraphicsUnit.Pixel)
        If e.RowIndex = 0 Then
            e.Graphics.DrawLine(New Pen(Color.Silver, 1), 0, e.RowBounds.Top + e.RowBounds.Height - 2, e.RowBounds.Width, e.RowBounds.Top + e.RowBounds.Height - 2)
        End If
    End Sub

    Private Sub dtginfo3_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles dtginfo3.MouseMove
        dtginfo3.Visible = False
    End Sub

    Private Sub dtginfo3_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtginfo3.RowPrePaint
        Dim r As Rectangle = New Rectangle(e.RowBounds.Left, e.RowBounds.Top, e.RowBounds.Width, e.RowBounds.Height + 1)
        'Disegno lo sfondo'
        e.Graphics.DrawImage(bginfo3, r, r, GraphicsUnit.Pixel)
        'Disegno il box del modulo'
        If dtginfo3.Rows(e.RowIndex).Tag IsNot Nothing Then
            e.Graphics.DrawLine(New Pen(Color.Silver, 1), 3, e.RowBounds.Top, e.RowBounds.Width - 2, e.RowBounds.Top)
            e.Graphics.FillRectangle(New System.Drawing.Drawing2D.LinearGradientBrush(e.RowBounds, Color.FromArgb(120, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), Drawing2D.LinearGradientMode.Vertical), New Rectangle(r.Width - 78, r.Top, 72, 16))
            e.Graphics.DrawRectangle(New Pen(Color.Silver, 1), New Rectangle(r.Width - 78, r.Top, 72, 15))
        End If
        If e.RowIndex = 1 Then
            e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(220, 255, 255, 255)), r)
        End If
    End Sub

    Private Sub dtginfo_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtginfo.SelectionChanged
        dtginfo.ClearSelection()
    End Sub

    Private Sub dtginfo2_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtginfo2.SelectionChanged
        dtginfo2.ClearSelection()
    End Sub

    Private Sub txtstar1_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtstar1.Validated

        Dim id As Integer = CInt(txtstar1.Tag)
        Dim r As String = CStr(dtg1.Item("ruolo", id).Value)
        Dim n As String = txtstar1.Text

        If pfree.ContainsKey(r) = False OrElse pfree(r).ContainsKey(n) = False Then
            ShowError("Errore", "Giocatore jolly non valido!")
            dtg1.Item("nome", id).Value = ""
        Else
            dtg1.Item("nome", id).Value = n
            If pfree(r)(n).Length > 3 Then
                dtg1.Item("squadra", id).Value = pfree(r)(n).Substring(0, 3)
            Else
                dtg1.Item("squadra", id).Value = pfree(r)(n)
            End If
        End If
        txtstar1.Visible = False
        Call DrawFormazione()
    End Sub

    Private Sub txtstar1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtstar1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Try
                Dim id As Integer = CInt(txtstar1.Tag)
                dtg1.Item("nome", id).Value = txtstar1.Text
                txtstar1.Visible = False
                Call DrawFormazione()
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
        End If
    End Sub

    Private Sub dtg1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtg1.SelectionChanged
        dtg1.ClearSelection()
    End Sub

    Private Sub dtg1_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellDoubleClick
        If e.ColumnIndex = 3 Then
            Dim star As Integer = CInt(dtg1.Item(0, e.RowIndex).Tag)
            If star = 1 Then
                Call ShowEdit1(CStr(dtg1.Item(e.ColumnIndex - 1, e.RowIndex).Value), CStr(dtg1.Item(e.ColumnIndex, e.RowIndex).Value), e.RowIndex)
            End If
        End If
    End Sub

    Private Sub frmcompformazioni_MinimumSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MinimumSizeChanged
        Call HideEdit()
    End Sub

    Private Sub piccampo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles piccampo.Click
        Call HideEdit()
    End Sub

    Private Sub dtg1_CellMouseLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellMouseLeave
        Call HideChart()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick


        Timer1.Stop()

        Try
            If gra.Tag IsNot Nothing Then

                If imgforma Is Nothing Then imgforma = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                Dim gr As Graphics
                Dim b1 As New Bitmap(pnlchart.Width, pnlchart.Height)
                Dim w As Integer = pnlchart.Width
                Dim h As Integer = pnlchart.Height

                gr = Graphics.FromImage(b1)

                gr.Clear(Color.White)
                gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

                'Disegno lo sfondo i bordi della finestra'
                Dim br2 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, w, h - 1), Color.FromArgb(130, 130, 130), Color.DimGray, Drawing2D.LinearGradientMode.Vertical)

                gr.DrawImage(iControl.CommonFunction.GetAreaImage(imgforma, pnlchart.Left, pnlchart.Top, pnlchart.Width, pnlchart.Height), 0, 0)

                For i As Integer = 0 To 2
                    Dim br1 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, w, h - 1), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(30 + i * 10, 0, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                    gr.FillPath(br1, SystemFunction.DrawingAndImage.GetBorderDullPath1(gr, New Rectangle(i, i, w - i * 2, h - i * 2 - 20), 16 - i * 2))
                Next

                'gr.SmoothingMode = Drawing2D.SmoothingMode.Default
                gr.FillPath(br2, SystemFunction.DrawingAndImage.GetBorderDullPath1(gr, New Rectangle(3, 3, w - 6, h - 28), 10))

                gr.FillPath(Brushes.White, SystemFunction.DrawingAndImage.GetBorderDullPath1(gr, New Rectangle(4, 4, w - 8, h - 30), 12))

                pnlchart.Visible = False
                'gra.Visible = True
                gra.Title.Visible = True
                gra.Title.Font = New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel)
                gra.BackgroundImage = Nothing

                Dim s() As String = CStr(gra.Tag).Split(CChar(","))

                If s.Length = 2 Then

                    Dim auto As Boolean = False
                    Dim max As Integer = 10
                    Dim div As Integer = 1

                    Select Case CStr(s(0))
                        Case "gs" : max = 10 : auto = True : div = 5 : gra.Title.Text = s(1) & " [Goal subiti]"
                        Case "gf" : max = 10 : auto = True : div = 5 : gra.Title.Text = s(1) & " [Goal fatti]"
                        Case "ass" : max = 4 : auto = True : div = 5 : gra.Title.Text = s(1) & " [Assist]"
                        Case "amm" : max = 2 : auto = False : div = 2 : gra.Title.Text = s(1) & " [Ammonizioni]"
                        Case "esp" : max = 1 : auto = False : div = 1 : gra.Title.Text = s(1) & " [Espulsioni]"
                        Case "p.g." : max = 1 : auto = False : div = 1 : gra.Title.Text = s(1) & " [Partitre giocate]"
                        Case "m.v." : max = 10 : auto = True : div = 5 : gra.Title.Text = s(1) & " [Voti]"
                        Case "m.p." : max = 10 : auto = True : div = 5 : gra.Title.Text = s(1) & " [Punti]"
                    End Select

                    If auto Then
                        gra.AutoScale = iChart.iChart.eAutoScale.MaxAutoScale
                    Else
                        gra.AutoScale = iChart.iChart.eAutoScale.None
                    End If

                    gra.Axsis(iChart.Axsis.Axsis.Y).Division = div
                    gra.Max = max

                    Dim d As List(Of Double) = currlega.GetDataChart(s(0), s(1))

                    For i As Integer = 0 To d.Count - 1
                        If d(i) > -10 Then
                            gra.Item(0, i) = CStr(d(i))
                        Else
                            gra.Item(0, i) = ""
                        End If
                    Next
                    pnlchart.Visible = True
                    gra.Draw(True)

                    gr.DrawImage(gra.GetImage, 7, 12)

                    pnlchart.Image = CType(b1.Clone, Image)

                    b1.Dispose()
                    gr.Dispose()
                Else
                    Call HideChart()
                End If
            Else
                Call HideChart()
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            Call HideChart()
        End Try

    End Sub

    Private Sub dtg1_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtg1.RowPrePaint
        Try

            Dim p As LegaObject.Formazione.PlayerFormazione = f.Players(CInt(dtg1.Rows(e.RowIndex).Tag))

            Call SystemFunction.Gui.DrawInfoMatchAndPresenze(dtg1, e.RowIndex, p, _gg)

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Private Sub mnu1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnu1.ItemClicked
        Call Action(CInt(e.ClickedItem.Tag))
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Call CompileAutomatic()
    End Sub

    Class PlayerForma

        Dim _nome As String = ""
        Dim _squadra As String = ""

        Sub New(ByVal Nome As String, ByVal Squadra As String)
            _nome = Nome
            _squadra = Squadra
        End Sub

        Public Property Nome() As String
            Get
                Return _nome
            End Get
            Set(ByVal value As String)
                _nome = value
            End Set
        End Property

        Public Property Squadra() As String
            Get
                Return _squadra
            End Get
            Set(ByVal value As String)
                _squadra = value
            End Set
        End Property
    End Class

    Private Sub dtg1_CellMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dtg1.CellMouseMove
        Try
            If dtg1.Columns(e.ColumnIndex).Name = "infort" OrElse dtg1.Columns(e.ColumnIndex).Name = "presenza" OrElse dtg1.Columns(e.ColumnIndex).Name = "match" AndAlso e.RowIndex <> -1 Then
                If dtg1.Columns(e.ColumnIndex).Name = "match" AndAlso dtg1.Item("match", e.RowIndex).Tag IsNot Nothing Then

                    'Controllo che la giornata selezionata sia quella relativa alle probabili formazioni e in
                    'tal caso visualizzo il popup contenente i possibili schieramenti delle squadre per i
                    'diversi siti internet

                    Dim gg As Integer = CInt(txtgg.Text)

                    If gg = webdata.DayProbableFormation Then

                        Dim p As LegaObject.Formazione.PlayerFormazione = f.Players(CInt(dtg1.Rows(e.RowIndex).Tag))
                        Dim r As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                        Dim t As String = p.Squadra

                        If e.Location.X < r.Width \ 2 Then
                            t = p.Match.TeamA
                        Else
                            t = p.Match.TeamB
                        End If

                        If CStr(dtginfo3.Tag) <> t & e.RowIndex Then
                            dtginfo3.Tag = t & e.RowIndex
                            Call ShowInfo3(r, t, p.Nome, "")
                        End If

                    Else
                        dtginfo3.Visible = False
                    End If
                Else
                    dtginfo3.Visible = False
                End If
            Else
                Call HideInfo()
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Sub ShowInfo3(ByVal r As Rectangle, team As String, name As String, ByVal fltsite As String)

        Try

            Dim ncol As Integer = 6 'Numero massiomo di giocatori per riga'
            Dim row As Integer = 2

            dtginfo3.Rows.Clear()
            dtginfo3.ColumnCount = ncol

            For i As Integer = 0 To ncol - 1
                dtginfo3.Columns(i).Width = 75
            Next

            dtginfo3.Rows.Add(New DataGridViewRow)
            dtginfo3.Rows.Add(New DataGridViewRow)

            'Intestazione tabella con nome squadra'
            dtginfo3.DefaultCellStyle.Font = New Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel)
            dtginfo3.Item(0, 1).Value = team
            dtginfo3.Item(0, 1).Style.ForeColor = Color.RoyalBlue
            dtginfo3.Rows(1).DefaultCellStyle.Font = New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel)
            dtginfo3.Rows(0).Height = 0
            dtginfo3.Rows(1).Height = dtginfo3.RowTemplate.Height + 6
            dtginfo3.Width = ncol * dtginfo3.Columns(0).Width

            dtginfo3.Rows.Add(New DataGridViewRow)

            For k As Integer = 0 To webdata.WebSiteList.Count - 1

                If webdata.WebSiteList(k).StartsWith("Probabili formazioni") Then

                    Dim site As String = System.Text.RegularExpressions.Regex.Match(webdata.WebSiteList(k).ToLower, "(?<=\"").*(?=\"")").Value.Replace(" ", "")
                    Dim key As String = _gg & "-" & team.ToLower & "-" & site

                    If webdata.WebTeamPlayers.ContainsKey(key) AndAlso (fltsite = "" OrElse site = fltsite) Then

                        Dim col As Integer = 0
                        Dim pd As Integer = 0
                        Dim pc As Integer = 0
                        Dim pa As Integer = 0
                        Dim st As String = webdata.WebSiteList(k).Replace("Probabili formazioni", "").Replace("""", "").Trim

                        'Stampo il nome del sito'
                        dtginfo3.RowTemplate.Height = 14
                        If st.Length > 12 Then
                            dtginfo3.Item(0, row).Value = st.Substring(0, 11) & "."
                        Else
                            dtginfo3.Item(0, row).Value = st
                        End If
                        dtginfo3.Rows(row).Tag = "r"
                        dtginfo3.Item(col, row).Style.ForeColor = Color.RoyalBlue
                        dtginfo3.Rows(row).Height = dtginfo3.RowTemplate.Height + 4

                        'Determino il modulo della formazioni'
                        For i As Integer = 0 To webdata.WebTeamPlayers(key).Titolari.Count - 1
                            Select Case webdata.WebTeamPlayers(key).Titolari(i).Ruolo
                                Case "D" : pd += 1
                                Case "C" : pc += 1
                                Case "A" : pa += 1
                            End Select
                        Next

                        'Stampo il modulo della formazione'
                        dtginfo3.Item(ncol - 1, row).Value = "Mod :" & CStr(pd) & "-" & CStr(pc) & "-" & CStr(pa)
                        dtginfo3.Item(ncol - 1, row).Style.Alignment = DataGridViewContentAlignment.MiddleLeft

                        dtginfo3.Rows.Add(New DataGridViewRow)
                        row += 1

                        'Stampo l'elenco dei titolari e panchinari'
                        row = AddPlayersInfo3(name, webdata.WebTeamPlayers(key).Titolari, True, row, ncol)
                        row = AddPlayersInfo3(name, webdata.WebTeamPlayers(key).Panchinari, False, row, ncol)

                    End If
                End If
            Next

            If row > 2 Then

                HideChart()

                dtginfo3.Rows(0).DefaultCellStyle.BackColor = Color.Transparent

                'Dimensione popup'
                Dim h As Integer = dtginfo3.Rows(1).Height

                For i As Integer = 2 To dtginfo3.RowCount - 1
                    dtginfo3.Rows(i).DefaultCellStyle.BackColor = Color.Transparent
                    If dtginfo3.Rows(i).Tag Is Nothing Then
                        dtginfo3.Rows(i).Height = dtginfo3.RowTemplate.Height
                    Else
                        dtginfo3.Rows(i - 1).Height = dtginfo3.RowTemplate.Height + 3
                    End If
                    h += dtginfo3.Rows(i).Height
                Next

                dtginfo3.Height = h + 2

                'Background popup'
                Dim br As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, dtginfo3.Width, dtginfo3.Height), Color.White, Color.Gainsboro, Drawing2D.LinearGradientMode.Vertical)
                Dim gr As Graphics

                bginfo3 = New Bitmap(dtginfo3.Width, dtginfo3.Height)
                gr = Graphics.FromImage(bginfo3)
                gr.Clear(Color.White)
                gr.FillRectangle(br, New Rectangle(0, 0, dtginfo3.Width, dtginfo3.Height))
                gr.Dispose()

                'Posizionamento popup'
                If fltsite = "" Then
                    Dim pt As Point = New Point(r.Left - dtginfo3.Width + 20, r.Top + dtg1.Top + r.Height \ 2 - dtginfo3.Height \ 2)
                    If pt.Y < 30 Then pt.Y = 50
                    If pt.Y + dtginfo3.Height > Me.Height - 30 Then pt.Y = Me.Height - 30 - dtginfo3.Height
                    dtginfo3.Location = pt
                Else
                    dtginfo3.Left = dtginfo.Left - dtginfo3.Width
                    If dtginfo.Top + h > Me.Height - 100 Then
                        dtginfo3.Top = Me.Height - 100 - h
                    Else
                        dtginfo3.Top = dtginfo.Top
                    End If
                End If
                dtginfo3.Visible = True
            Else
                dtginfo3.Visible = False
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

    End Sub

    Function AddPlayersInfo3(pname As String, plist As List(Of wData.wTeamPlayers.wTeamPlayer), titolare As Boolean, row As Integer, ncol As Integer) As Integer

        Dim col As Integer = 0

        For i As Integer = 0 To plist.Count - 1

            Dim name As String = plist(i).Nome
            If name.Length > 9 Then name = name.Substring(0, 9) & "."

            dtginfo3.Item(col, row).Value = name

            'se il nome del giocatore e' lo stesso di quello selezionato allora lo evidenzio in rosso'
            If titolare Then
                If pname <> "" AndAlso plist(i).Nome = pname Then
                    dtginfo3.Item(col, row).Style.ForeColor = Color.FromArgb(255, 0, 0)
                Else
                    dtginfo3.Item(col, row).Style.ForeColor = Color.FromArgb(30, 30, 30)
                End If
            Else
                If pname <> "" AndAlso plist(i).Nome = pname Then
                    dtginfo3.Item(col, row).Style.ForeColor = Color.FromArgb(220, 20, 20)
                Else
                    dtginfo3.Item(col, row).Style.ForeColor = Color.FromArgb(220, 120, 40)
                End If
            End If

            col += 1

            If col > ncol - 1 Then
                col = 0
                row += 1
                dtginfo3.Rows.Add(New DataGridViewRow)
            End If

        Next

        If col <> 0 Then
            row += 1
            dtginfo3.Rows.Add(New DataGridViewRow)
        End If

        Return row

    End Function

    Private Sub gra_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles gra.MouseMove
        pnlchart.Visible = False
    End Sub

    Private Sub tlb1_ButtonClick(sender As Object, e As EventArgs, ButtonIndex As Integer) Handles tlb1.ButtonClick
        Dim ind As Integer = CInt(tlb1.Tag)
        Select Case ButtonIndex
            Case 2
                If ind > 0 Then
                    ind -= 1
                    txtsearch.Text = tlb2.Button(0).SubItems(ind).Text
                    _idteam = ind
                    Call loadFormazione()
                End If
            Case 3
                If ind < tlb2.Button(0).SubItems.Count - 1 Then
                    ind += 1
                    _idteam = ind
                    txtsearch.Text = tlb2.Button(0).SubItems(ind).Text
                    Call loadFormazione()
                End If
        End Select
        tlb1.Tag = ind
    End Sub

    Private Sub tlbarr2_ButtonClick(sender As Object, e As EventArgs, ButtonIndex As Integer) Handles tlb2.ButtonClick
        Select Case ButtonIndex
            Case 3
                If _gg > 1 Then
                    _gg -= 1
                    txtgg.Text = tlbgglist.Button(0).SubItems(_gg - 1).Text
                    Call SetViewMode()
                    Call loadFormazione()
                End If
            Case 4
                If _gg < tlbgglist.Button(0).SubItems.Count Then
                    _gg += 1
                    txtgg.Text = tlbgglist.Button(0).SubItems(_gg - 1).Text
                    Call SetViewMode()
                    Call loadFormazione()
                End If
        End Select
    End Sub

    Private Sub dtg2_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles dtg2.DragDrop
        Dim p As Point = dtg2.PointToClient(New Point(e.X, e.Y))
        dragIndex = dtg2.HitTest(p.X, p.Y).RowIndex
        If (e.Effect = DragDropEffects.Move) Then
            Dim dragRow As DataGridViewRow = CType(e.Data.GetData(GetType(DataGridViewRow)), DataGridViewRow)
            dtg2.Rows.RemoveAt(fromIndex)
            dtg2.Rows.Insert(dragIndex, dragRow)
            Call SetButtonSaveState(True)
        End If
    End Sub

    Private Sub dtg2_DragOver(ByVal sender As Object, ByVal e As DragEventArgs) Handles dtg2.DragOver
        e.Effect = DragDropEffects.Move
    End Sub

    Private Sub dtg2_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles dtg2.MouseDown
        fromIndex = dtg2.HitTest(e.X, e.Y).RowIndex
        If fromIndex > -1 Then
            Dim dragSize As Size = SystemInformation.DragSize
            dragRect = New Rectangle(New Point(e.X - (dragSize.Width \ 2), e.Y - (dragSize.Height \ 2)), dragSize)
        Else
            dragRect = Rectangle.Empty
        End If
    End Sub

    Private Sub DataGridView1_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles dtg2.MouseMove
        If (e.Button And MouseButtons.Left) = MouseButtons.Left Then
            If (dragRect <> Rectangle.Empty AndAlso Not dragRect.Contains(e.X, e.Y)) Then
                dtg2.DoDragDrop(dtg2.Rows(fromIndex), DragDropEffects.Move)
            End If
        End If
    End Sub

End Class
