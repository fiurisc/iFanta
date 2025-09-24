Public Class frmstat

    Private start As Boolean = True
    Private lst As New List(Of LegaObject.Team.Player)
    Private minrow As Integer = 16
    Private bginfo As Bitmap
    Private oldrow As Integer = -1
    Private idteam As Integer = 0

    Private Sub frmstat_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.KeyPreview = True

        IForm1.WindowsTitle = My.Application.Info.ProductName

        lbby.Text = My.Application.Info.Copyright

        dtg1.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 10.5, FontStyle.Regular, GraphicsUnit.Pixel)

        dtg1.Columns("nome").Width = 120
        dtg1.Columns("idrosa").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dtg1.Columns("ruolo").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dtg1.Columns("nome").DefaultCellStyle.Format.ToUpper()
        dtg1.Columns("nome").DefaultCellStyle.Font = New Font("tahoma", 10.5, FontStyle.Bold, GraphicsUnit.Pixel)
        dtg1.Columns("qa").DefaultCellStyle.ForeColor = Color.Blue
        dtg1.Columns("amm").DefaultCellStyle.ForeColor = Color.Orange
        dtg1.Columns("esp").DefaultCellStyle.ForeColor = Color.Red
        dtg1.Columns("avgvt").DefaultCellStyle.ForeColor = Color.Blue
        dtg1.Columns("avgvt").DefaultCellStyle.Font = New Font("tahoma", 10.5, FontStyle.Bold, GraphicsUnit.Pixel)
        dtg1.Columns("avgpt").DefaultCellStyle.ForeColor = Color.Blue
        dtg1.Columns("avgpt").DefaultCellStyle.Font = New Font("tahoma", 10.5, FontStyle.Bold, GraphicsUnit.Pixel)
        dtg1.Columns("idrosa").Tag = 1

        'Setto l'allineamento delle colonne'
        For i As Integer = 3 To dtg1.Columns.Count - 1
            dtg1.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            dtg1.Columns(i).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            If dtg1.Columns(i).Width = 30 Then
                dtg1.Columns(i).Width = 35
            End If
        Next
        dtg1.Columns("nome").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        dtg1.RowTemplate.Height = 17

        'Inibisco l'ordinamento manuale delle colonne'
        dtg1.Rows.Clear()
        For i As Integer = 0 To dtg1.Columns.Count - 1
            dtg1.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        'Resetto la flag di ordinamento colonna'
        For i As Integer = 0 To dtg1.Columns.Count - 1
            dtg1.Columns(i).Tag = 0
        Next

        'Setto le keysort sulle colonne'
        dtg1.Columns("idrosa").DataPropertyName = "idrosa"
        dtg1.Columns("ruolo").DataPropertyName = "ruolo"
        dtg1.Columns("nome").DataPropertyName = "nome"
        dtg1.Columns("squadra").DataPropertyName = "squadra"
        dtg1.Columns("costo").DataPropertyName = "costo"
        dtg1.Columns("qa").DataPropertyName = "qcur"
        dtg1.Columns("gs").DataPropertyName = "gs_tot"
        dtg1.Columns("gf").DataPropertyName = "gf_tot"
        dtg1.Columns("amm").DataPropertyName = "amm_tot"
        dtg1.Columns("esp").DataPropertyName = "esp_tot"
        dtg1.Columns("ass").DataPropertyName = "ass_tot"
        dtg1.Columns("rigt").DataPropertyName = "rigt_tot"
        dtg1.Columns("rigp").DataPropertyName = "rigp_tot"
        dtg1.Columns("rigs").DataPropertyName = "rigs_tot"
        dtg1.Columns("pgio").DataPropertyName = "pgio_tot"
        dtg1.Columns("ptit").DataPropertyName = "tit_tot"
        dtg1.Columns("pgiol").DataPropertyName = "pgio_last"
        dtg1.Columns("ptitl").DataPropertyName = "tit_last"
        dtg1.Columns("mm").DataPropertyName = "avg_mm_last"
        dtg1.Columns("avgvt").DataPropertyName = "avg_vt_tot"
        dtg1.Columns("avgpt").DataPropertyName = "avg_pt_tot"
        dtg1.Columns("infort").DataPropertyName = "infort"
        dtg1.Columns("presenza").DataPropertyName = "presenza"
        dtg1.Columns("var").DataPropertyName = "var"
        dtg1.Columns("rating").DataPropertyName = "rating"
        dtg1.Columns("nation").DataPropertyName = "natcode"

        If currlega.Settings.Active Then
            dtg1.Columns("presenza").Visible = True
        Else
            dtg1.Columns("presenza").Visible = False
        End If
        dtg1.Columns("presenza").Visible = False

        Dim p As System.Reflection.PropertyInfo = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        p.SetValue(dtg1, True, Nothing)
        mnu1.ImageScalingSize = New Size(16, 16)
        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        gra.Tag = "vt."

        dtg1.RowCount = minrow

        'Imposto l'elenco delle squadre'
        Call SetTeamName()

        'Setto il tema corrente'
        Call SetTheme()
        Call SetToolTipParameater()

        Timer1.Enabled = True

        start = False

    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            If Me.OwnedForms.Length = 0 Then

                If keyData = Keys.PageUp Then

                    Dim ind As Integer = idteam

                    ind = ind - 1
                    If ind < 0 Then ind = 0

                    If ind <> idteam Then
                        txtsearch.Text = tlbsearch.Button(0).SubItems(ind).Text
                        idteam = ind
                        Call loadData()
                    End If

                ElseIf keyData = Keys.PageDown Then

                    Dim ind As Integer = idteam

                    ind = ind + 1
                    If ind > tlbsearch.Button(0).SubItems.Count - 1 Then ind = tlbsearch.Button(0).SubItems.Count - 1

                    If ind <> idteam Then
                        txtsearch.Text = tlbsearch.Button(0).SubItems(ind).Text
                        idteam = ind
                        Call loadData()
                    End If

                End If
            End If
        Catch ex As Exception

        End Try
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Sub SetTheme()

        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)

        tlbshow.Left = padd
        tlbshow.Top = padd \ 2 + IForm1.TY + 6
        tlbshow.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbshow.draw(True)

        tlbsearch.Left = IForm1.RX - padd - tlbsearch.Width
        tlbsearch.Top = padd \ 2 + IForm1.TY + 6
        tlbsearch.Height = txtsearch.Height
        tlbsearch.BorderColor = txtsearch.BorderColor
        tlbsearch.BorderColorDropDown = tlbsearch.BorderColor
        tlbsearch.Button(0).SubWidth = txtsearch.Width + tlbsearch.Button(0).Width - 1
        tlbsearch.Button(0).SubItemsAutoSize = False
        tlbsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbsearch.draw(True)

        txtsearch.Left = tlbsearch.Left - txtsearch.Width + 1
        txtsearch.Top = tlbsearch.Top
        txtsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle
        txtsearch.BringToFront()

        tlbnavsearch.Left = txtsearch.Left - tlbnavsearch.Width + 1
        tlbnavsearch.Top = txtsearch.Top
        tlbnavsearch.Height = txtsearch.Height
        tlbnavsearch.BorderColor = txtsearch.BorderColor
        tlbnavsearch.BorderColorDropDown = tlbsearch.BorderColor

        picsearch.Left = tlbnavsearch.Left - picsearch.Width - 2
        picsearch.Top = txtsearch.Top + 1
        lbsearch.Left = picsearch.Left - lbsearch.Width - 2
        lbsearch.Top = txtsearch.Top

        tlbsearch1.Left = IForm1.RX - padd - tlbsearch.Width
        tlbsearch1.Top = gra.Top - tlbsearch1.Height
        tlbsearch1.Height = txtsearch1.Height
        tlbsearch1.BorderColor = txtsearch1.BorderColor
        tlbsearch1.BorderColorDropDown = tlbsearch1.BorderColor
        tlbsearch1.Button(0).SubWidth = txtsearch1.Width + tlbsearch1.Button(0).Width - 1
        tlbsearch1.Button(0).SubItemsAutoSize = False
        tlbsearch1.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbsearch1.draw(True)

        txtsearch1.Left = tlbsearch1.Left - txtsearch1.Width + 1
        txtsearch1.Top = tlbsearch1.Top
        txtsearch1.FlatStyle = AppSett.Personal.Theme.FlatStyle
        txtsearch1.BringToFront()

        tlbnavsearch1.Left = txtsearch1.Left - tlbnavsearch1.Width + 1
        tlbnavsearch1.Top = tlbsearch1.Top
        tlbnavsearch1.Height = txtsearch1.Height
        tlbnavsearch1.BorderColor = txtsearch1.BorderColor
        tlbnavsearch1.BorderColorDropDown = tlbsearch1.BorderColor

        picsearch1.Left = tlbnavsearch1.Left - picsearch1.Width - 2
        picsearch1.Top = txtsearch1.Top + 1
        lbsearch1.Left = picsearch1.Left - lbsearch1.Width - 2
        lbsearch1.Top = txtsearch1.Top

        tlbgra.Left = padd
        tlbgra.Top = tlbsearch1.Top

        dtg1.Left = padd
        dtg1.Top = txtsearch.Top + txtsearch.Height + 15
        dtg1.Height = gra.Top - dtg1.Top - tlbgra.Height - 15
        dtg1.Width = IForm1.RX - IForm1.LX - padd * 2 + 4
        dtg1.FlatStyle = AppSett.Personal.Theme.FlatStyle

        txtsearch.Text = currlega.Teams(0).Nome
        txtsearch1.Text = ""

        tlbgra.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbgra.draw(True)

        tlbaction.Left = IForm1.RX - tlbaction.Width - padd
        tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
        tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

        lnbot.Top = tlbaction.Top - lnbot.Height - 5
        lnbot.Left = dtg1.Left
        lnbot.Width = IForm1.RX - IForm1.LX - padd * 2

        lbby.Left = IForm1.LX + padd - 3
        lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

        gra.Left = padd \ 2
        gra.Width = lnbot.Width + padd
        gra.Top = lnbot.Top - gra.Height

    End Sub

    Sub HideInfo()
        oldrow = -1
        dtginfo.Visible = False
    End Sub

    Sub SetToolTipParameater()
        dtginfo.BorderColor = Color.FromArgb(80, 80, 80)
        dtginfo.InternalBorderColor = Color.FromArgb(80, 80, 80)
        dtginfo.BackgroundColor = Color.FromArgb(245, 245, 245)
        dtginfo.DefaultCellStyle.BackColor = dtginfo.BackgroundColor
        dtginfo.DefaultCellStyle.SelectionBackColor = dtginfo.BackgroundColor
        dtginfo.CellBorderStyle = DataGridViewCellBorderStyle.None
        dtginfo.Columns(1).Width = dtginfo.Width - dtginfo.Columns(0).Width
    End Sub

    Sub SetTeamName()
        Try
            'Imposto l'elenco delle squadre'
            txtsearch.AutoCompleteList.Clear()
            For i As Integer = 0 To currlega.Teams.Count - 1
                txtsearch.AutoCompleteList.Add(currlega.Teams(i).Nome)
                tlbsearch.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(currlega.Teams(i).Nome))
            Next
            txtsearch.AutoCompleteList.Add("TUTTE")
            tlbsearch.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("TUTTE"))
            txtsearch.AutoCompleteList.Add("TUTTI")
            tlbsearch.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("TUTTI"))
            txtsearch.AutoCompleteList.Add("SVINCOLATI")
            tlbsearch.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("SVINCOLATI"))
            Dim p As List(Of String) = currlega.GetAllPlayerList
            txtsearch.AutoCompleteList.AddRange(p)
            txtsearch1.AutoCompleteList.AddRange(p)
        Catch ex As Exception

        End Try
    End Sub

    Sub DrawDefaultChart()
        Try
            gra.SuspundeLayout = True
            gra.Type = iChart.iChart.ChartType.Bar
            gra.ColumnCount = currlega.Settings.NumberOfDays + 1
            gra.RowCount = 1
            gra.SuspundeLayout = False
            gra.Title.Font = New Font("tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
            gra.Title.Visible = False
            gra.BorderSize = 0
            gra.Axsis(iChart.Axsis.Axsis.X).MinDivisionFont = New Font("tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
            gra.Axsis(iChart.Axsis.Axsis.X).TextAlignment = iChart.Axsis.Alignment.Center
            gra.Axsis(iChart.Axsis.Axsis.Y).MinDivisionFont = New Font("tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
            gra.Series.Item(0).Style.InternalBorderSize = 1
            gra.Series.Item(0).Style.BorderSize = 1
            If AppSett.Personal.Theme.FlatStyle Then
                gra.Series.Item(0).Style.Brush.Color1 = Color.CornflowerBlue
            Else
                gra.Series.Item(0).Style.Brush.Color1 = Color.FromArgb(30, 30, 240)
                gra.Series.Item(0).Style.Brush.Color2 = Color.CornflowerBlue
            End If
            For i As Integer = 0 To gra.ColumnCount - 1
                gra.Item(0, i) = ""
                gra.Axsis(iChart.Axsis.Axsis.X).Item(i).Value = CStr(i + 1)
            Next
            gra.Max = 10
            gra.AutoScale = iChart.iChart.eAutoScale.None
            gra.Info.Type = iChart.Info.eType.Value
            gra.Info.Font = New Font("tahoma", 11, FontStyle.Bold, GraphicsUnit.Pixel)
            gra.Info.Visible = True
            gra.Draw()
        Catch ex As Exception

        End Try
    End Sub

    Sub loadData()

        tlbsearch.Button(0).ClearSubButtonSelection()
        tlbsearch.Button(0).SubItems(idteam).State = True

        Try
            'Carico i dati della squadra e della rosa'
            Select Case tlbshow.Button(4).Text.ToLower
                Case "rendimento" : currlega.Teams(0).RatingType = LegaObject.Team.eRatingType.Rendimento
                Case "costo/rendimento" : currlega.Teams(0).RatingType = LegaObject.Team.eRatingType.RendimentoCosto
                Case "quotazione/rendimento" : currlega.Teams(0).RatingType = LegaObject.Team.eRatingType.RendimentoQuotazione
            End Select
            lst = currlega.Teams(0).GetPlayer(txtsearch.Text, tlbshow.Button(1).Text)
            Call DisplayData()
            Call DrawDefaultChart()
            tlbsearch1.Button(0).SubItems.Clear()
            For k As Integer = 0 To lst.Count - 1
                tlbsearch1.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(lst(k).Nome))
            Next
        Catch

        End Try
    End Sub

    Sub DisplayData()
        Try

            'Verifico se e' stato richiesto un ordinamento dei dati'
            Dim str As String = "idrosa"
            Dim rev As Boolean = False

            For i As Integer = 0 To dtg1.ColumnCount - 1
                Dim t As Integer = CInt(dtg1.Columns(i).Tag)
                If t <> 0 Then
                    If t = -1 Then rev = True
                    str = dtg1.Columns(i).DataPropertyName
                    Exit For
                End If
            Next

            'Oridino i dati'
            lst = LegaObject.Team.Sort(lst, str, rev)

            'Setto il numero delle righe'
            dtg1.RowCount = lst.Count

            'Compilo le righe'
            For i As Integer = 0 To lst.Count - 1

                Dim p As LegaObject.Team.Player = lst(i)

                dtg1.Rows(i).Tag = Nothing

                If lst(i).Rating < 30 Then
                    dtg1.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 190)
                ElseIf lst(i).Rating < 40 Then
                    dtg1.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 230)
                ElseIf lst(i).Rating > 70 Then
                    dtg1.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(180, 240, 255)
                ElseIf lst(i).Rating > 65 Then
                    dtg1.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 255)
                Else
                    dtg1.Rows(i).DefaultCellStyle.BackColor = Color.White
                End If

                dtg1.Item("idrosa", i).Value = p.IdTeam & "-" & p.IdRosa

                Select Case p.Ruolo
                    Case "P" : dtg1.Item("ruolo", i).Style.ForeColor = Color.Orange
                    Case "D" : dtg1.Item("ruolo", i).Style.ForeColor = Color.Red
                    Case "C" : dtg1.Item("ruolo", i).Style.ForeColor = Color.Green
                    Case "A" : dtg1.Item("ruolo", i).Style.ForeColor = Color.Blue
                End Select
                dtg1.Item("ruolo", i).Value = p.Ruolo
                dtg1.Item("nome", i).Value = p.Nome
                If lst(i).Squadra.Length > 3 Then
                    dtg1.Item("squadra", i).Value = p.Squadra.Substring(0, 3)
                Else
                    dtg1.Item("squadra", i).Value = p.Squadra
                End If
                dtg1.Item("costo", i).Value = p.Costo
                dtg1.Item("qa", i).Value = p.QCur

                dtg1.Item("gs", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Gs, "-")
                dtg1.Item("gf", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Gf, "-")
                dtg1.Item("amm", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Amm, "-")
                dtg1.Item("esp", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Esp, "-")
                dtg1.Item("ass", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Ass, "-")
                dtg1.Item("rigt", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.RigT, "-")
                dtg1.Item("rigp", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.RigP, "-")
                dtg1.Item("rigs", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.RigS, "-")
                dtg1.Item("pgio", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.pGiocate, "-")
                dtg1.Item("ptit", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Titolare, "-")
                dtg1.Item("avgpt", i).Value = SystemFunction.General.SetFieldSingleData(p.StatisticAll.Avg_Pt, "-")
                dtg1.Item("pgiol", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticLast.pGiocate, "-")
                dtg1.Item("ptitl", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticLast.Titolare, "-")
                dtg1.Item("mm", i).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticLast.Avg_mm, "-")

                dtg1.Item("avgvt", i).Value = SystemFunction.General.SetFieldSingleData(p.StatisticAll.Avg_Vt, "-")
                dtg1.Item("avgpt", i).Value = SystemFunction.General.SetFieldSingleData(p.StatisticAll.Avg_Pt, "-")
                
                dtg1.Item("var", i).Value = SystemFunction.DrawingAndImage.GetPlayerVariationImage(p.Variation)
                dtg1.Item("nation", i).Value = SystemFunction.DrawingAndImage.GetImageNationFlags(p.NatCode)

                dtg1.Item("rating", i).ToolTipText = CStr(lst(i).Rating)

                dtg1.Item("chart", i).Value = My.Resources.chartb16
                If webdata.WebPlayers.ContainsKey(lst(i).Nome & "-" & lst(i).Squadra) Then
                    Dim wp As wData.wPlayer = webdata.WebPlayers(lst(i).Nome & "-" & lst(i).Squadra)
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
                        dtg1.Item("infort", i).Tag = wp
                        If t = "i" Then
                            dtg1.Item("infort", i).Value = My.Resources.infortunato
                        Else
                            dtg1.Item("infort", i).Value = My.Resources.espulso
                        End If
                    Else
                        dtg1.Item("infort", i).Value = My.Resources.empty
                        dtg1.Item("infort", i).Tag = Nothing
                    End If
                Else
                    dtg1.Item("infort", i).Value = My.Resources.empty
                    dtg1.Item("infort", i).Tag = Nothing
                End If
                
            Next
        Catch ex As Exception

        End Try

    End Sub

    Sub LoadChart()

        Try
            tlbsearch1.Tag = 0
            For i As Integer = 0 To tlbsearch1.Button(0).SubItems.Count - 1
                If tlbsearch1.Button(0).SubItems(i).Text = txtsearch1.Text Then
                    tlbsearch1.Button(0).SubItems(i).State = True
                    tlbsearch1.Tag = i
                Else
                    tlbsearch1.Button(0).SubItems(i).State = False
                End If
            Next
            If CInt(tlbsearch1.Tag) = 0 AndAlso tlbsearch1.Button(0).SubItems.Count > 0 Then
                tlbsearch1.Button(0).SubItems(0).State = True
            End If

            Dim auto As Boolean = False
            Dim max As Integer = 10
            Select Case CStr(gra.Tag)
                Case "gs" : max = 10 : auto = True
                Case "gf" : max = 10 : auto = True
                Case "ass" : max = 5 : auto = False
                Case "amm" : max = 2 : auto = False
                Case "esp" : max = 1 : auto = False
                Case "rigt" : max = 10 : auto = True
                Case "rigt" : max = 10 : auto = True
                Case "rigs" : max = 10 : auto = True
                Case "rigp" : max = 10 : auto = True
                Case "p.g." : max = 1 : auto = False
                Case "tit" : max = 1 : auto = False
                Case "m.m", "min" : gra.Tag = "min" : max = 90 : auto = False
                Case "vt.", "m.v." : gra.Tag = "vt." : max = 10 : auto = True
                Case "pt.", "m.p." : gra.Tag = "pt." : max = 10 : auto = True
                Case Else : gra.Tag = "vt." : max = 10 : auto = True
            End Select

            If auto Then
                gra.AutoScale = iChart.iChart.eAutoScale.MaxAutoScale
            Else
                gra.AutoScale = iChart.iChart.eAutoScale.None
            End If

            Dim d As List(Of Double) = currlega.GetDataChart(CStr(gra.Tag), txtsearch1.Text)
            For i As Integer = 0 To d.Count - 1
                If d(i) > -10 Then
                    gra.Item(0, i) = CStr(d(i))
                Else
                    gra.Item(0, i) = ""
                End If
            Next
           
            gra.Max = max
            Select Case max
                Case 1
                    gra.Axsis(iChart.Axsis.Axsis.Y).Division = 1
                Case 2
                    gra.Axsis(iChart.Axsis.Axsis.Y).Division = 2
                Case 3
                    gra.Axsis(iChart.Axsis.Axsis.Y).Division = 3
                Case 4
                    gra.Axsis(iChart.Axsis.Axsis.Y).Division = 4
                Case Else
                    gra.Axsis(iChart.Axsis.Axsis.Y).Division = 5
            End Select
            gra.Draw()

            gra.Info.Type = iChart.Info.eType.Value
            gra.Info.ForeColor = Color.FromArgb(80, 80, 80)
            Dim upt As Boolean = False
            For i As Integer = 0 To tlbgra.Button.Count - 2
                If tlbgra.Button(i).Text = CStr(gra.Tag) Then
                    If tlbgra.Button(i).State = False Then
                        tlbgra.Button(i).State = True
                        tlbgra.Button(i).ForeColor = Color.Red
                        tlbgra.Button(i).ForeColorSelect = Color.Red
                        upt = True
                    End If
                Else
                    tlbgra.Button(i).ForeColor = Nothing
                    tlbgra.Button(i).ForeColorSelect = Nothing
                    tlbgra.Button(i).State = False
                End If
            Next
            If upt Then tlbgra.draw(True)

        Catch ex As Exception
            Call DrawDefaultChart()
        End Try

    End Sub

    Private Sub txtsearch_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call loadData()
        End If
    End Sub

    Private Sub tlbsearch_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbsearch.SubButtonClick
        idteam = SubButtonIndex
        If txtsearch.Text <> tlbsearch.Button(0).SubItems(SubButtonIndex).Text Then
            txtsearch.Text = tlbsearch.Button(0).SubItems(SubButtonIndex).Text
            Call loadData()
        End If
    End Sub

    Private Sub tlbsearch1_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbsearch1.SubButtonClick
        idteam = SubButtonIndex
        If txtsearch1.Text <> tlbsearch1.Button(0).SubItems(SubButtonIndex).Text Then
            txtsearch1.Text = tlbsearch1.Button(0).SubItems(SubButtonIndex).Text
            Call LoadChart()
        End If
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Private Sub txtsearch1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtsearch1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call LoadChart()
        End If
    End Sub

    Private Sub tlbgra_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbgra.ButtonClick
        If tlbgra.Button(ButtonIndex).Text <> "Refresh" Then
            For i As Integer = 0 To tlbgra.Button.Count - 2
                tlbgra.Button(i).State = False
                tlbgra.Button(i).ForeColor = Nothing
                tlbgra.Button(i).ForeColorSelect = Nothing
            Next
            tlbgra.Button(ButtonIndex).State = True
            tlbgra.Button(ButtonIndex).ForeColor = Color.Red
            tlbgra.Button(ButtonIndex).ForeColorSelect = Color.Red
            tlbgra.draw(True)
            gra.Tag = tlbgra.Button(ButtonIndex).Text
            Call LoadChart()
        End If
    End Sub

    Private Sub dtg1_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellMouseEnter
        If e.RowIndex = -1 Then Call HideInfo() : Exit Sub
        Try
            If dtg1.Columns(e.ColumnIndex).Name = "infort" OrElse dtg1.Columns(e.ColumnIndex).Name = "presenza" Then
                If dtg1.Item(e.ColumnIndex, e.RowIndex).Tag IsNot Nothing Then

                    Dim wp As wData.wPlayer = CType(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag, wData.wPlayer)
                    Dim r As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                    Dim x As Integer = r.Left + r.Width + dtg1.Left
                    Dim y As Integer = r.Top + dtg1.Top + r.Height \ 2

                    bginfo = SystemFunction.Gui.ShowPopUpInfoPlayer(dtginfo, wp, x, y, IForm1)

                    'Dim wp As wData.wPlayer = CType(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag, wData.wPlayer)
                    'dtginfo.RowCount = 0
                    'Dim row As Integer = 0
                    'Dim h As Integer = 0
                    'For i As Integer = 0 To wp.Info.Count - 1
                    '    If wp.Site.Count = wp.Info.Count Then
                    '        Dim s() As String = wp.Info(i).Split(CChar(","))
                    '        For k As Integer = 0 To s.Length - 1
                    '            If k = 0 Then
                    '                dtginfo.Rows.Add(New DataGridViewRow)
                    '                If s.Length = 1 Then dtginfo.Rows(row).Tag = 1 Else dtginfo.Rows(row).Tag = 0
                    '                dtginfo.Item(1, row).Value = wp.Site(i)
                    '                dtginfo.Item(2, row).Value = s(k)
                    '                dtginfo.Rows(row).Height = dtginfo.RowTemplate.Height
                    '                dtginfo.Item(1, row).Style.ForeColor = Color.FromArgb(50, 50, 50)
                    '                dtginfo.Item(2, row).Style.Font = New Font("Arial", 11, FontStyle.Bold,GraphicsUnit.Pixel)
                    '                dtginfo.Rows(row).DefaultCellStyle.BackColor = Color.Transparent
                    '                h = h + dtginfo.Rows(row).Height
                    '                row = row + 1
                    '            Else
                    '                dtginfo.Rows.Add(New DataGridViewRow)
                    '                dtginfo.Item(1, row).Value = "''" & s(k).Trim & "''"
                    '                If s(k).Length > 50 Then
                    '                    dtginfo.Rows(row).Height = dtginfo.RowTemplate.Height * 2
                    '                Else
                    '                    dtginfo.Rows(row).Height = dtginfo.RowTemplate.Height
                    '                End If
                    '                dtginfo.Item(1, row).Style.ForeColor = Color.FromArgb(90, 90, 90)
                    '                h = h + dtginfo.Rows(row).Height
                    '                If i < wp.Info.Count - 1 Then dtginfo.Rows(row).Tag = 1 Else dtginfo.Rows(row).Tag = 0
                    '                dtginfo.Rows(row).DefaultCellStyle.BackColor = Color.Transparent
                    '                row = row + 1
                    '            End If
                    '        Next
                    '    End If
                    'Next
                    'dtginfo.Height = h + 4
                    'Dim r As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                    'Dim x As Integer = r.Left + dtg1.Left - dtginfo.Width
                    'Dim y As Integer = r.Top + dtg1.Top + r.Height \ 2 - dtginfo.Height \ 2
                    'If y < dtg1.Top Then
                    '    y = r.Top + dtg1.Top
                    'ElseIf y + dtginfo.Height > dtg1.Top + dtg1.Height Then
                    '    y = dtg1.Top + dtg1.Height - dtginfo.Height
                    'End If
                    'dtginfo.Location = New Point(x, y)
                    'dtginfo.CellBorderStyle = DataGridViewCellBorderStyle.None
                    ''Draw background'
                    'Dim br As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, dtginfo.Width, dtginfo.Height), Color.White, Color.Gainsboro, Drawing2D.LinearGradientMode.Vertical)
                    'Dim gr As Graphics
                    'bginfo = New Bitmap(dtginfo.Width, dtginfo.Height)
                    'gr = Graphics.FromImage(bginfo)
                    'gr.Clear(Color.White)
                    'gr.FillRectangle(br, New Rectangle(0, 0, dtginfo.Width, dtginfo.Height))
                    'gr.Dispose()
                    'dtginfo.Visible = True
                Else
                    Call HideInfo()
                End If
            Else
                Call HideInfo()
            End If
        Catch ex As Exception
            Call HideInfo()
        End Try
    End Sub

    Private Sub dtginfo_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles dtginfo.RowPostPaint
        If dtginfo.Item(2, e.RowIndex).Value IsNot Nothing Then
            Dim site As String = dtginfo.Item(1, e.RowIndex).Value.ToString.ToLower
            Dim info As String = dtginfo.Item(2, e.RowIndex).Value.ToString
            Dim format As New StringFormat
            format.Alignment = StringAlignment.Far
            If info.Contains("Titolare") Then
                e.Graphics.DrawString(info, New Font("Arial", 10.5, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.RoyalBlue, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
            ElseIf info.Contains("Panchina") OrElse info.Contains("A disposizione") Then
                e.Graphics.DrawString(info, New Font("Arial", 10.5, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Red, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
            Else
                e.Graphics.DrawString(info, New Font("Arial", 10.5, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.DimGray, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
            End If
        End If
        If CInt(dtginfo.Rows(e.RowIndex).Tag) = 1 Then
            e.Graphics.DrawLine(New Pen(Color.Gainsboro, 2), 4, e.RowBounds.Top + e.RowBounds.Height - 1, e.RowBounds.Width - 4, e.RowBounds.Top + e.RowBounds.Height - 1)
        End If
    End Sub

    Private Sub dtginfo_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtginfo.RowPrePaint
        Dim r As Rectangle = New Rectangle(e.RowBounds.Left, e.RowBounds.Top, e.RowBounds.Width, e.RowBounds.Height + 1)
        e.Graphics.DrawImage(bginfo, r, r, GraphicsUnit.Pixel)
    End Sub

    Private Sub dtg1_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtg1.RowPrePaint

        If e.RowIndex = -1 OrElse e.RowIndex > lst.Count - 1 Then Exit Sub

        Try

            If dtg1.Rows(e.RowIndex).Tag Is Nothing Then
                'Rating'
                Dim rat As Integer = 0
                'If CStr(dtg1.Item("avgpt", e.RowIndex).Value) <> "-" Then
                '    rat = CInt(CSng(dtg1.Item("avgpt", e.RowIndex).Value) * 10)
                'End If
                rat = lst(e.RowIndex).Rating
                If rat > 100 Then rat = 200
                If matchratingkey.ContainsKey(rat) = False Then
                    Dim r As New Rectangle(0, 0, dtg1.Columns("rating").Width, dtg1.Rows(e.RowIndex).Height)
                    Dim team As String = lst(e.RowIndex).Squadra
                    matchratingkey.Add(rat, SystemFunction.DrawingAndImage.GetImageRating(r, rat, 100, 0))
                End If
                dtg1.Item("rating", e.RowIndex).Value = matchratingkey(rat)
                dtg1.Rows(e.RowIndex).Tag = "e"
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dtg1_ColumnHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dtg1.ColumnHeaderMouseClick
        Try
            Select Case dtg1.Columns(e.ColumnIndex).Name
                Case "presenza", "devpt", "imgpt", "var", "chart"
                Case Else
                    Dim rev As Integer = CInt(dtg1.Columns(e.ColumnIndex).Tag)
                    Dim f As String = dtg1.Columns(e.ColumnIndex).HeaderText
                    If f = "" Then f = "idrosa"
                    If rev = -1 OrElse rev = 0 Then
                        rev = 1
                    Else
                        rev = -1
                    End If
                    For i As Integer = 0 To dtg1.Columns.Count - 1
                        dtg1.Columns(i).Tag = 0
                    Next
                    dtg1.Columns(e.ColumnIndex).Tag = rev
                    Call DisplayData()
            End Select
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dtg1_CellClick(ByVal sender As Object, e As DataGridViewCellEventArgs) Handles dtg1.CellClick
        If e.RowIndex <> -1 Then
            txtsearch1.Text = CStr(dtg1.Item("nome", e.RowIndex).Value)
            gra.Tag = dtg1.Columns(e.ColumnIndex).HeaderText
            Call LoadChart()
        End If
    End Sub

    Private Sub tlbshow_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbshow.ButtonClick
        If ButtonIndex = 6 Then Call loadData()
    End Sub

    Private Sub tlbshow_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbshow.SubButtonClick
        Select Case ButtonIndex
            Case 1
                tlbshow.Button(ButtonIndex).Text = tlbshow.Button(ButtonIndex).SubItems(SubButtonIndex).Text
                tlbshow.draw(True)
                tlbshow.Button(ButtonIndex).SubItemsAutoSize = False
                tlbshow.Button(ButtonIndex).SubWidth = tlbshow.Button(1).Width + tlbshow.Button(0).Width
            Case 4
                tlbshow.Button(ButtonIndex).Text = tlbshow.Button(ButtonIndex).SubItems(SubButtonIndex).Text
                tlbshow.draw(True)
                tlbshow.Button(ButtonIndex).SubItemsAutoSize = False
                tlbshow.Button(ButtonIndex).SubWidth = tlbshow.Button(4).Width + tlbshow.Button(3).Width
        End Select
        Call loadData()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        gra.BackgroundImage = Nothing
        Call DrawDefaultChart()
        Call loadData()
        dtg1.Visible = True
        gra.Visible = True
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Call Action(ButtonIndex)
    End Sub

    Private Sub mnu1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnu1.ItemClicked
        Call Action(CInt(e.ClickedItem.Tag))
    End Sub

    Private Sub Action(ByVal Act As Integer)
        Select Case Act
            Case 0
                Dim frm As New frmimpexp
                frm.SetParameater(frmimpexp.TypeOfOperation.Export)
                If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                    Call ImpExp.ImpExpStatistic.ExportHtml(lst, txtsearch.Text, frm.GetDirectory)
                End If
                frm.Dispose()
            Case 1
                Me.Close()
            Case 2
                SystemFunction.General.CopyDataTable(False, dtg1)
            Case 3
                SystemFunction.General.CopyDataTable(True, dtg1)
        End Select
    End Sub

    Private Sub tlbnavsearch_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbnavsearch.ButtonClick

        Dim ind As Integer = idteam

        Select Case ButtonIndex
            Case 0 : ind = ind - 1
            Case 1 : ind = ind + 1
        End Select

        If ind < 0 Then
            ind = 0
        ElseIf ind > tlbsearch.Button(0).SubItems.Count - 1 Then
            ind = tlbsearch.Button(0).SubItems.Count - 1
        End If

        If ind <> idteam Then
            txtsearch.Text = tlbsearch.Button(0).SubItems(ind).Text
            idteam = ind
            Call loadData()
        End If

    End Sub

    Private Sub frm_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
        Call HideInfo()
    End Sub

    Private Sub frm_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        Call HideInfo()
    End Sub

    Private Sub tlbnavsearch1_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbnavsearch1.ButtonClick
        Dim ind As Integer = CInt(tlbsearch1.Tag)

        Select Case ButtonIndex
            Case 0 : ind = ind - 1
            Case 1 : ind = ind + 1
        End Select

        If ind < 0 Then
            ind = 0
        ElseIf ind > tlbsearch1.Button(0).SubItems.Count - 1 Then
            ind = tlbsearch1.Button(0).SubItems.Count - 1
        End If

        If ind <> CInt(tlbsearch1.Tag) Then
            txtsearch1.Text = tlbsearch1.Button(0).SubItems(ind).Text
            tlbsearch1.Tag = ind
            Call LoadChart()
        End If
    End Sub

    Private Sub dtg1_KeyDown(sender As Object, e As KeyEventArgs) Handles dtg1.KeyDown
        Try
            If e.KeyData = Keys.Left OrElse e.KeyData = Keys.Right Then

                Dim ind As Integer = -1

                For i As Integer = 0 To tlbgra.Button.Count - 3
                    tlbgra.Button(i).State = False
                    tlbgra.Button(i).ForeColor = Nothing
                    tlbgra.Button(i).ForeColorSelect = Nothing
                    If tlbgra.Button(i).Text = CStr(gra.Tag) Then
                        ind = i
                        Exit For
                    End If
                Next

                If ind = -1 Then
                    ind = 0
                Else
                    If ind > 0 AndAlso e.KeyData = Keys.Left Then ind -= 2
                    If ind < tlbgra.Button.Count - 3 AndAlso e.KeyData = Keys.Right Then ind += 2
                End If

                tlbgra.Button(ind).State = True
                tlbgra.Button(ind).ForeColor = Color.Red
                tlbgra.Button(ind).ForeColorSelect = Color.Red

                gra.Tag = tlbgra.Button(ind).Text
                tlbgra.Button(ind).State = True
                tlbgra.draw(True)

                Call LoadChart()

            ElseIf e.KeyData = Keys.Up OrElse e.KeyData = Keys.Down Then

                Dim ind As Integer = 0

                If dtg1.SelectedRows.Count > 0 Then
                    ind = dtg1.SelectedRows(0).Index
                    If e.KeyData = Keys.Up Then
                        If ind > 0 Then ind -= 1
                    Else
                        If ind < dtg1.Rows.Count - 1 Then ind += 1
                    End If
                End If

                dtg1.Rows(ind).Selected = True
                txtsearch1.Text = CStr(dtg1.Item("nome", ind).Value)
                Call LoadChart()

            End If

        Catch ex As Exception

        End Try
    End Sub
End Class
