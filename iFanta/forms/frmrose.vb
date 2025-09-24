Imports iFanta.SystemFunction.FileAndDirectory

Public Class frmrose

    Dim idteam As Integer = 0
    Dim start As Boolean = True
    Dim ris As DialogResult = Windows.Forms.DialogResult.Abort
    Dim frm As frmroseall
    Private imgforma As Bitmap
    Dim plist As List(Of String) = currlega.GetAllPlayerList

    Private Sub frmrose_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call CheckSaveState()
        AppSett.SaveSettings()
        Me.DialogResult = ris
    End Sub

    Private Sub frmrose_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        pnl1.Visible = False
        dtg1.Visible = False

        IForm1.WindowsTitle = My.Application.Info.ProductName

        txtsearch.Text = ""

        'Impostazioni di default'
        For i As Integer = 0 To dtg1.Columns.Count - 1
            dtg1.Columns(i).ReadOnly = True
            dtg1.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            dtg1.Columns(i).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            dtg1.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        'Impostazioni di base tabella'
        dtg1.RowTemplate.Height = 16
        dtg1.RowCount = 25
        dtg1.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 11, FontStyle.Regular,GraphicsUnit.Pixel)
        dtg1.Columns("idrosa").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dtg1.Columns("ruolo").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dtg1.Columns("nome").DefaultCellStyle.Format.ToUpper()

        dtg1.Columns("costo").DefaultCellStyle.ForeColor = Color.Blue
        dtg1.Columns("amm_tot").DefaultCellStyle.ForeColor = Color.Orange
        dtg1.Columns("esp_tot").DefaultCellStyle.ForeColor = Color.Red
        dtg1.Columns("avg_pt_tot").DefaultCellStyle.ForeColor = Color.Blue

        Dim p As System.Reflection.PropertyInfo = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        p.SetValue(dtg1, True, Nothing)

        'Imposto l'elenco delle squadre'
        Call SetTeamName()

        lbby.Text = My.Application.Info.Copyright
        mnu1.ImageScalingSize = New Size(16, 16)
        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        'Setto il tema corrente'
        Call SetTheme()
        Call SetWindowsWidth()

        start = False

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


        Timer1.Enabled = True

    End Sub

    Overrides Sub ResetTorneo()
        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)
        Call LoadData(True)
    End Sub

    Sub SetParameater(ByVal Id As Integer, ByVal FormOrigin As frmroseall)
        idteam = Id
        frm = FormOrigin
    End Sub

    Sub SetTheme()

        Try

            SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)

            picdetail.Left = padd
            picdetail.Top = padd \ 2 + IForm1.TY + 8
            chkdetail.Left = picdetail.Left + picdetail.Width + 1
            chkdetail.Top = picdetail.Top - 2

            tlbsearch.Left = IForm1.RX - padd - 1 - tlbsearch.Width
            tlbsearch.Top = padd \ 2 + IForm1.TY + 6
            tlbsearch.Height = txtsearch.Height
            tlbsearch.BorderColor = txtsearch.BorderColor
            tlbsearch.BorderColorDropDown = tlbsearch.BorderColor
            tlbsearch.Button(0).SubWidth = txtsearch.Width + tlbsearch.Button(0).Width
            tlbsearch.Button(0).SubItemsAutoSize = False
            tlbsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle

            txtsearch.Left = tlbsearch.Left - txtsearch.Width + 1
            txtsearch.Top = tlbsearch.Top
            txtsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle

            tlbnavsearch.Left = txtsearch.Left - tlbnavsearch.Width + 1
            tlbnavsearch.Top = padd \ 2 + IForm1.TY + 6
            tlbnavsearch.Height = txtsearch.Height
            tlbnavsearch.BorderColor = txtsearch.BorderColor
            tlbnavsearch.BorderColorDropDown = tlbsearch.BorderColor
            tlbnavsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle

            dtg1.Left = padd
            dtg1.Top = txtsearch.Top + txtsearch.Height + 15
            dtg1.Height = dtg1.RowTemplate.Height * dtg1.RowCount + dtg1.ColumnHeadersHeight

            pnl1.Top = dtg1.Top
            pnl1.Left = IForm1.RX - pnl1.Width - padd
            picstemma.Top = pic1.Top + 2
            picstemma.Left = pic1.Left + 2
            picstemma.Width = pic1.Width - 4
            picstemma.Height = pic1.Height - 4

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

            tlbaction.Left = IForm1.RX - tlbaction.Width - padd
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            For i As Integer = 0 To tlbaction.Button.Count - 1
                tlbaction.Button(i).BorderColor = Color.DimGray
            Next

            lnbot.Top = tlbaction.Top - lnbot.Height - 5
            lnbot.Left = dtg1.Left
            lnbot.Width = IForm1.RX - IForm1.LX - padd * 2

            tlbtotali.Button(1).MinWidth = dtg1.Columns("costo").Width
            tlbtotali.Button(2).MinWidth = dtg1.Columns("qini").Width
            tlbtotali.Button(3).MinWidth = dtg1.Columns("qcur").Width
            tlbtotali.Button(4).MinWidth = dtg1.Columns("qdiff").Width
            tlbtotali.draw(True)

        Catch ex As Exception

        End Try

    End Sub

    Private Sub mnu1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnu1.ItemClicked
        Call Action(CInt(e.ClickedItem.Tag))
    End Sub

    Private Sub Action(ByVal Act As Integer)
        Try
            If idteam > -1 AndAlso idteam < currlega.Settings.NumberOfTeams Then
                Select Case Act
                    Case 0
                        Dim frm As New frmimpexp
                        frm.SetParameater(frmimpexp.TypeOfOperation.Import)
                        If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                            Call ImpExp.ImpExpRose.ImportHtml(idteam, currlega.Teams(idteam).Nome, frm.GetDirectory)
                            Call LoadData(True)
                        End If
                        frm.Dispose()
                    Case 1
                        Dim frm As New frmimpexp
                        frm.SetParameater(frmimpexp.TypeOfOperation.Export)
                        If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                            Call ImpExp.ImpExpRose.ExportHtml(idteam, currlega.Teams(idteam).Nome, frm.GetDirectory)
                        End If
                        frm.Dispose()
                    Case 2
                        If iControl.iMsgBox.ShowMessage("Sei sicuro di voler eliminare la rosa?" & System.Environment.NewLine & "Agendo in questo modo, andranno persi tutti i dati correnti", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.OK Then
                            currlega.Teams(idteam).Delete()
                            Call LoadData(True)
                        End If
                    Case 3 : Call Save()
                    Case 4 : Me.Close()
                    Case 5
                        My.Computer.Clipboard.SetImage(SystemFunction.DrawingAndImage.ConvertDatagridToImage(dtg1))
                    Case 6
                        Dim dlg As New Windows.Forms.FolderBrowserDialog
                        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                            Dim fname As String = ImpExp.ImpExpRose.GetHtmlFileName(idteam, currlega.Teams(idteam).Nome, dlg.SelectedPath, "png")
                            SystemFunction.DrawingAndImage.SaveDatagridToImage(dtg1, fname)
                        End If
                    Case 7
                        SystemFunction.General.CopyDataTable(False, dtg1)
                    Case 8
                        SystemFunction.General.CopyDataTable(True, dtg1)
                    Case 9, 10
                        For i As Integer = 0 To currlega.Teams(idteam).Players.Count - 1
                            currlega.Teams(idteam).Players(i).QIni = currlega.Teams(idteam).Players(i).QCur
                        Next
                        currlega.Teams(idteam).Save()

                        'Impostazione quotazione inziale'
                        'If dtg1.SelectedRows.Count > 0 Then

                        '    For i As Integer = 0 To dtg1.SelectedRows.Count - 1

                        '        Dim id As Integer = dtg1.SelectedRows(i).Index
                        '        Dim key As String = dtg1.Item("nome", id).Value & "-" & dtg1.Item("squadra", id).Value

                        '        If webdata.AllPlayers.ContainsKey(key) Then
                        '            'Dim nat As String = webdata.AllPlayer(key).
                        '            'If imgnatcode.ContainsKey(nat) Then
                        '            '    dtg1.Item("nation", id).Value = imgnatcode(nat)
                        '            '    dtg1.Item("nation", id).ToolTipText = webdata.WebOtherPlayerData(key).Nation
                        '            'Else
                        '            '    dtg1.Item("nation", id).Value = My.Resources.empty
                        '            '    dtg1.Item("nation", id).ToolTipText = ""
                        '            'End If
                        '        Else
                        '            dtg1.Item("nation", id).Value = My.Resources.empty
                        '            dtg1.Item("nation", id).ToolTipText = ""
                        '        End If

                        '        dtg1.Item("qini", dtg1.SelectedRows(0).Index).Value = 3
                        '        dtg1.Rows(dtg1.SelectedRows(0).Index).Cells(2).Value = ""
                        '        If Act = 9 Then Exit For
                        '    Next

                        'End If
                End Select
            End If
        Catch ex As Exception
            ShowError("Errore", ex.Message)
        End Try
    End Sub

    Sub SetTeamName()
        Try
            'Imposto l'elenco delle squadre'
            txtsearch.AutoCompleteList.Clear()
            tlbsearch.Button(0).SubItems.Clear()
            For i As Integer = 0 To currlega.Teams.Count - 1
                txtsearch.AutoCompleteList.Add(currlega.Teams(i).Nome)
                tlbsearch.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(currlega.Teams(i).Nome))
                If currlega.Teams(i).IdTeam = idteam Then tlbsearch.Button(0).SubItems(i).State = True
            Next
        Catch ex As Exception

        End Try
    End Sub

    Sub SetWindowsWidth()

        Dim w As Integer = 0

        Try

            dtg1.Columns("squadra").Visible = Not (chkdetail.Value)
            dtg1.Columns("squadra_small").Visible = chkdetail.Value

            dtg1.Columns("gs_tot").Visible = chkdetail.Value
            dtg1.Columns("gf_tot").Visible = chkdetail.Value
            dtg1.Columns("amm_tot").Visible = chkdetail.Value
            dtg1.Columns("esp_tot").Visible = chkdetail.Value
            dtg1.Columns("ass_tot").Visible = chkdetail.Value
            dtg1.Columns("avg_pt_tot").Visible = chkdetail.Value

            dtg1.Columns("rating").Visible = chkdetail.Value
            dtg1.Columns("var").Visible = chkdetail.Value
            dtg1.Columns("pgio_tot").Visible = chkdetail.Value
            dtg1.Columns("tit_tot").Visible = chkdetail.Value
            dtg1.Columns("pgio_last").Visible = chkdetail.Value
            dtg1.Columns("tit_last").Visible = chkdetail.Value
            dtg1.Columns("mm_last").Visible = False
            dtg1.Columns("avg_mm_last").Visible = chkdetail.Value

            For i As Integer = 0 To dtg1.Columns.Count - 1
                If dtg1.Columns(i).Visible Then
                    w = w + dtg1.Columns(i).Width
                End If
            Next

            w = w + 20
            dtg1.Width = w

            Me.Width = w + padd * 2 + pnl1.Width + 15
            Me.Top = My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2
            Me.Left = My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2

            dtg1.Columns("pgio_last").DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 140)
            dtg1.Columns("tit_last").DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 140)
            dtg1.Columns("mm_last").DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 140)
            dtg1.Columns("avg_mm_last").DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 140)

        Catch ex As Exception

        End Try

    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try

            If keyData = Keys.PageUp Then
                Dim ind As Integer = idteam
                ind = ind - 1
                If ind < 0 Then
                    ind = 0
                End If
                If ind <> idteam Then
                    idteam = ind
                    Call LoadData(False)
                End If
            ElseIf keyData = Keys.PageDown Then
                Dim ind As Integer = idteam
                ind = ind + 1
                If ind > currlega.Settings.NumberOfTeams - 1 Then
                    ind = currlega.Settings.NumberOfTeams - 1
                End If
                If ind <> idteam Then
                    idteam = ind
                    Call LoadData(False)
                End If
            End If
        Catch ex As Exception

        End Try
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Sub LoadData(ByVal Reload As Boolean)

        Dim edit As Boolean = True

        Call HideChart()

        Try
            If idteam <> -1 Then

                edit = True

                tlbsearch.Button(0).ClearSubButtonSelection()
                tlbsearch.Button(0).SubItems(idteam).State = True

                'Carico i dati della squadra e della rosa'
                currlega.Teams(idteam).Load(True, True)

                txtnome.Text = currlega.Teams(idteam).Nome
                txtalle.Text = currlega.Teams(idteam).Allenatore
                txtpre.Text = currlega.Teams(idteam).Presidente
                txtsearch.Text = currlega.Teams(idteam).Nome

                dtg1.Rows.Clear()
                dtg1.RowCount = 25

                If Reload Then Call SetTeamName()

                currlega.Teams(0).RatingType = LegaObject.Team.eRatingType.Rendimento

                Dim id As Integer = 0

                dtg1.DefaultCellStyle.Font = New Font("Tahoma", 10.5, FontStyle.Regular, GraphicsUnit.Pixel)

                For i As Integer = 0 To currlega.Teams(idteam).Players.Count - 1

                    Dim p As LegaObject.Team.Player = currlega.Teams(idteam).Players(i)

                    id = currlega.Teams(idteam).Players(i).IdRosa - 1

                    dtg1.Rows(id).Tag = Nothing
                    dtg1.Item("idrosa", id).Value = currlega.Teams(idteam).Players(i).IdRosa
                    dtg1.Item("ruolo", id).Value = SystemFunction.General.GetRuoloFromId(i)
                    dtg1.Item("ruolo", id).Style.ForeColor = SystemFunction.General.GetRuoloForeColor(i)
                    dtg1.Item("nome", id).Value = currlega.Teams(idteam).Players(i).Nome
                    dtg1.Item("squadra", id).Value = currlega.Teams(idteam).Players(i).Squadra

                    If currlega.Teams(idteam).Players(i).Squadra.Length > 3 Then
                        dtg1.Item("squadra_small", id).Value = currlega.Teams(idteam).Players(i).Squadra.Substring(0, 3)
                    Else
                        dtg1.Item("squadra_small", id).Value = currlega.Teams(idteam).Players(i).Squadra
                    End If

                    dtg1.Item("costo", id).Value = currlega.Teams(idteam).Players(i).Costo
                    dtg1.Item("qini", id).Value = currlega.Teams(idteam).Players(i).QIni
                    dtg1.Item("qcur", id).Value = currlega.Teams(idteam).Players(i).QCur

                    Dim qdiff As Integer = currlega.Teams(idteam).Players(i).QCur - currlega.Teams(idteam).Players(i).QIni

                    dtg1.Item("qdiff", id).Value = qdiff

                    Select Case qdiff
                        Case Is < 0 : dtg1.Item("qdiff", id).Style.ForeColor = Color.Red
                        Case Is > 0 : dtg1.Item("qdiff", id).Style.ForeColor = Color.RoyalBlue
                        Case Else : dtg1.Item("qdiff", id).Style.ForeColor = dtg1.DefaultCellStyle.ForeColor
                    End Select

                    If currlega.Settings.EnableTraceReconfirmations Then
                        If currlega.Teams(idteam).Players(i).Riconfermato = 1 Then
                            dtg1.Item("riconf", id).Value = My.Resources.star10r
                            dtg1.Item("riconf", id).Tag = "1"
                        Else
                            dtg1.Item("riconf", id).Value = My.Resources.empty
                            dtg1.Item("riconf", id).Tag = "0"
                        End If
                        dtg1.Columns("riconf").Visible = True
                    Else
                        dtg1.Columns("riconf").Visible = False
                    End If

                    dtg1.Item("gs_tot", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Gs, "-")
                    dtg1.Item("gf_tot", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Gf, "-")
                    dtg1.Item("amm_tot", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Amm, "-")
                    dtg1.Item("esp_tot", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Esp, "-")
                    dtg1.Item("ass_tot", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Ass, "-")
                    dtg1.Item("pgio_tot", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.pGiocate, "-")
                    dtg1.Item("tit_tot", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticAll.Titolare, "-")
                    dtg1.Item("avg_pt_tot", id).Value = SystemFunction.General.SetFieldSingleData(p.StatisticAll.Avg_Pt, "-")
                    dtg1.Item("pgio_last", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticLast.pGiocate, "-")
                    dtg1.Item("tit_last", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticLast.Titolare, "-")
                    dtg1.Item("mm_last", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticLast.mm, "-")
                    dtg1.Item("avg_mm_last", id).Value = SystemFunction.General.SetFieldIntegerData(p.StatisticLast.Avg_mm, "-")
                    dtg1.Item("rating", i).ToolTipText = CStr(p.Rating)
                    dtg1.Item("rating", i).Value = p.Rating
                    dtg1.Item("var", id).Value = SystemFunction.DrawingAndImage.GetPlayerVariationImage(p.Variation)
                    dtg1.Item("nation", id).Value = SystemFunction.DrawingAndImage.GetImageNationFlags(p.NatCode)
                    dtg1.Item("nation", id).ToolTipText = p.Nat

                Next

            Else

                edit = False

                txtnome.Text = ""
                txtalle.Text = ""
                txtpre.Text = ""
                txtsearch.Text = ""

                For i As Integer = 0 To dtg1.RowCount - 1

                    Dim r As String = SystemFunction.General.GetRuoloFromId(i)

                    dtg1.Rows(i).Tag = "no"
                    dtg1.Item("idrosa", i).Value = i + 1
                    dtg1.Item("ruolo", i).Value = r
                    dtg1.Item("ruolo", i).Style.ForeColor = SystemFunction.General.GetRuoloForeColor(r)

                    For k As Integer = 0 To dtg1.Columns.Count - 1
                        Select Case dtg1.Columns(k).Name
                            Case "var", "chart"
                            Case "costo", "qini", "qcurr", "qdiff"
                                dtg1.Item(k, i).Value = "0"
                                If dtg1.Columns(k).Name = "qdiff" Then
                                    dtg1.Item("qdiff", i).Style.ForeColor = dtg1.DefaultCellStyle.ForeColor
                                End If
                            Case Else
                                dtg1.Item(k, i).Value = "-"
                        End Select
                    Next

                    dtg1.Item("rating", i).ToolTipText = ""
                    dtg1.Item("rating", i).Value = My.Resources.empty
                    dtg1.Item("var", i).Value = My.Resources.empty
                    dtg1.Item("chart", i).Value = My.Resources.empty

                Next
            End If

            'Setto lo stemma'
            Dim flogo As String = GetLegheDirectory() & "\" & currlega.Settings.Nome & "\stemmi\" & idteam & "-200x200.jpg"

            If IO.File.Exists(flogo) Then
                picstemma.Image = New Bitmap(flogo)
            Else
                picstemma.Image = Nothing
            End If

            tlbhelp.Top = dtg1.Top + dtg1.Height
            tlbhelp.Left = dtg1.Left
            tlbhelp.Button(0).Visible = currlega.Settings.EnableTraceReconfirmations
            tlbhelp.Button(1).Visible = currlega.Settings.EnableTraceReconfirmations
            tlbhelp.Button(2).Visible = chkdetail.Value
            tlbhelp.draw(True)

            'Visualizzo i totali'
            tlbtotali.Left = dtg1.Left + dtg1.Width - tlbtotali.Width - 20
            tlbtotali.Top = dtg1.Top + dtg1.Height

            Call CalculateBadget()

            For i As Integer = 0 To dtg1.Columns.Count - 1
                Select Case dtg1.Columns(i).Name
                    Case "nome", "costo", "qini"
                        dtg1.Columns(i).ReadOnly = Not (edit)
                    Case Else
                        dtg1.Columns(i).ReadOnly = True
                End Select
            Next

            txtnome.TextLocked = Not (edit)
            txtalle.TextLocked = Not (edit)
            txtpre.TextLocked = Not (edit)
            txtsearch.TextLocked = Not (edit)

            dtg1.ClearSelection()

            dtg1.Visible = True
            pnl1.Visible = True

        Catch ex As Exception
            Call WriteError("Rose", "LoadData", ex.Message)
        End Try

        imgforma = Nothing

        Call SetSaveState(False)

    End Sub

    Sub Save()
        Try
            Dim add As Boolean = False
            If idteam <> -1 Then
                'Salvo i dati generali del team'
                currlega.Teams(idteam).Nome = txtnome.Text
                currlega.Teams(idteam).Allenatore = txtalle.Text.Replace("'", "’")
                currlega.Teams(idteam).Presidente = txtpre.Text.Replace("'", "’")
                'Salvo la rosa di giocatori'
                If currlega.Teams(idteam).Players.Count = 0 Then
                    add = True
                End If
                Dim id As Integer = 0
                Dim tmp As String = ""
                For i As Integer = 0 To dtg1.RowCount - 1
                    If add Then
                        Dim c As Integer = 0
                        Dim q As Integer = 0
                        If dtg1.Item("costo", i).Value IsNot Nothing Then
                            tmp = dtg1.Item("costo", i).Value.ToString
                            If tmp <> "" AndAlso SystemFunction.General.IsNumeric(tmp) Then
                                c = CInt(tmp)
                            Else
                                c = 0
                            End If
                        Else
                            c = 0
                        End If
                        If dtg1.Item("qini", i).Value IsNot Nothing Then
                            tmp = dtg1.Item("qini", i).Value.ToString
                            If tmp <> "" AndAlso SystemFunction.General.IsNumeric(tmp) Then
                                q = CInt(tmp)
                            Else
                                q = 0
                            End If
                        Else
                            q = 0
                        End If
                        currlega.Teams(idteam).Players.Add(New LegaObject.Team.Player(i + 1, dtg1.Item("ruolo", i).Value.ToString, dtg1.Item("nome", i).Value.ToString, "", c, q))
                    Else
                        id = CInt(dtg1.Item("idrosa", i).Value) - 1
                        currlega.Teams(idteam).Players(id).Ruolo = dtg1.Item("ruolo", i).Value.ToString
                        If dtg1.Item("nome", i).Value IsNot Nothing Then currlega.Teams(idteam).Players(id).Nome = dtg1.Item("nome", i).Value.ToString Else currlega.Teams(idteam).Players(id).Nome = ""
                        currlega.Teams(idteam).Players(id).Nome = currlega.Teams(idteam).Players(id).Nome.ToUpper
                        tmp = dtg1.Item("costo", i).Value.ToString
                        If tmp <> "" AndAlso SystemFunction.General.IsNumeric(tmp) Then
                            currlega.Teams(idteam).Players(id).Costo = CInt(tmp)
                        Else
                            currlega.Teams(idteam).Players(id).Costo = 0
                        End If
                        If dtg1.Item("qini", i).Value IsNot Nothing Then
                            tmp = dtg1.Item("qini", i).Value.ToString
                            If tmp <> "" AndAlso SystemFunction.General.IsNumeric(tmp) Then
                                currlega.Teams(idteam).Players(id).QIni = CInt(tmp)
                            Else
                                currlega.Teams(idteam).Players(id).QIni = 0
                            End If
                        Else
                            currlega.Teams(idteam).Players(id).QIni = 0
                        End If

                        If CStr(dtg1.Item("riconf", i).Tag) = "1" Then
                            currlega.Teams(idteam).Players(id).Riconfermato = 1
                        Else
                            currlega.Teams(idteam).Players(id).Riconfermato = 0
                        End If
                    End If
                Next
                currlega.Teams(idteam).Save()
                Call LoadData(True)
                tlbaction.Button(3).Enabled = False
                tlbaction.draw(True)
                ris = Windows.Forms.DialogResult.OK
                If frm IsNot Nothing Then
                    frm.SetTeamName()
                    frm.DrawTeam()
                End If
                Call SetSaveState(False)

            End If
        Catch ex As Exception
            Call WriteError("Rose", "Save", ex.Message)
        End Try

    End Sub

    Sub CheckSaveState()
        If tlbaction.Button(3).Enabled Then
            If iControl.iMsgBox.ShowMessage("Salvare i cambiamenti effettuati?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.OK Then
                Call Save()
            End If
        End If
    End Sub

    Sub CalculateBadget()

        Dim c As Integer = 0
        Dim q As Integer = 0

        Try
            For i As Integer = 0 To dtg1.RowCount - 1
                If dtg1.Item("costo", i).Value IsNot Nothing AndAlso dtg1.Item("costo", i).Value.ToString <> "" Then
                    c = c + CInt(dtg1.Item("costo", i).Value)
                End If
                If dtg1.Item("qini", i).Value IsNot Nothing AndAlso dtg1.Item("qini", i).Value.ToString <> "" Then
                    q = q + CInt(dtg1.Item("qini", i).Value)
                End If
            Next
        Catch ex As Exception

        End Try
        tlbtotali.Button(1).Text = CStr(c)
        tlbtotali.Button(2).Text = CStr(q)
        tlbtotali.Button(3).Text = CStr(currlega.Teams(idteam).QcurTot)
        tlbtotali.Button(4).Text = CStr(currlega.Teams(idteam).QcurTot - q)
        Select Case currlega.Teams(idteam).QcurTot - currlega.Teams(idteam).QiniTot
            Case Is < 0 : tlbtotali.Button(4).ForeColor = Color.Red
            Case Is > 0 : tlbtotali.Button(4).ForeColor = Color.RoyalBlue
            Case Else : tlbtotali.Button(4).ForeColor = tlbtotali.Button(3).ForeColor
        End Select
        tlbtotali.draw(True)

    End Sub

    Sub SetSaveState(ByVal State As Boolean)
        If tlbaction.Button(3).Enabled <> State Then
            tlbaction.Button(3).Enabled = State
            tlbaction.draw(True)
        End If
        mnusave.Enabled = tlbaction.Button(3).Enabled
    End Sub

    Private Sub dtg1_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dtg1.CellMouseClick
        If dtg1.Columns(e.ColumnIndex).Name = "riconf" Then
            If CStr(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag) = "0" Then
                dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.star10r
                dtg1.Item(e.ColumnIndex, e.RowIndex).Tag = "1"
            Else
                dtg1.Item(e.ColumnIndex, e.RowIndex).Value = My.Resources.empty
                dtg1.Item(e.ColumnIndex, e.RowIndex).Tag = "0"
            End If
            Call SetSaveState(True)
        End If
    End Sub

    Private Sub dtg1_CellValidated(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellValidated
        If e.ColumnIndex = 13 OrElse e.ColumnIndex = 14 Then
            Call CalculateBadget()
        End If
    End Sub

    Private Sub dtg1_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles dtg1.EditingControlShowing
        Try
            Dim dtg As DataGridView = CType(sender, DataGridView)
            If dtg.Columns(dtg.CurrentCell.ColumnIndex).HeaderText = "Nome" Then
                Dim autoText As TextBox = TryCast(e.Control, TextBox)
                If autoText IsNot Nothing Then
                    autoText.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                    autoText.AutoCompleteSource = AutoCompleteSource.CustomSource
                    Dim DataCollection As New AutoCompleteStringCollection()
                    For i As Integer = 0 To plist.Count - 1
                        DataCollection.Add(plist(i))
                    Next
                    autoText.AutoCompleteCustomSource = DataCollection
                End If
            Else
                Dim autoText As TextBox = TryCast(e.Control, TextBox)
                If autoText IsNot Nothing Then
                    autoText.AutoCompleteCustomSource = Nothing
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dtg1_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtg1.RowPrePaint

        If e.RowIndex = -1 Then Exit Sub

        Try
            If dtg1.Rows(e.RowIndex).Tag Is Nothing Then
                'Rating'
                If dtg1.Item("rating", e.RowIndex).ToolTipText <> "" Then
                    Dim rat As Integer = 0
                    rat = CInt(dtg1.Item("rating", e.RowIndex).ToolTipText)
                    If rat > 100 Then rat = 200
                    If matchratingkey.ContainsKey(rat) = False Then
                        Dim r As New Rectangle(0, 0, dtg1.Columns("rating").Width, dtg1.Rows(e.RowIndex).Height)
                        Dim team As String = CStr(dtg1.Item("squadra", e.RowIndex).Value)
                        matchratingkey.Add(rat, SystemFunction.DrawingAndImage.GetImageRating(r, rat, 100, 0))
                    End If
                    dtg1.Item("rating", e.RowIndex).Value = matchratingkey(rat)
                Else
                    dtg1.Item("rating", e.RowIndex).Value = My.Resources.empty
                End If
                dtg1.Rows(e.RowIndex).Tag = "e"
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DataGridView1_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles dtg1.EditingControlShowing
        Dim c As Control
        c = e.Control
        AddHandler c.KeyPress, AddressOf Handle_KeyPress
    End Sub

    Protected Sub Handle_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
        Try
            If start Then Exit Sub
            If dtg1.CurrentCell.ColumnIndex = 2 Then
                If System.Text.RegularExpressions.Regex.Match(e.KeyChar, "[a-z]{1}").Success Then
                    e.KeyChar = CChar(e.KeyChar.ToString.ToUpper)
                End If
                If System.Text.RegularExpressions.Regex.Match(e.KeyChar, "[A-Z\s+\’\'\.]{1}").Success = False AndAlso e.KeyChar <> Convert.ToChar(8) Then
                    e.Handled = True
                End If
            End If
            If dtg1.CurrentCell.ColumnIndex = 13 OrElse dtg1.CurrentCell.ColumnIndex = 14 Then
                If System.Text.RegularExpressions.Regex.Match(e.KeyChar, "[0-9]{1}").Success = False AndAlso e.KeyChar <> Convert.ToChar(8) Then
                    e.Handled = True
                End If
            End If
            If e.Handled <> True Then
                Call SetSaveState(True)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub txtsearch_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call CheckSaveState()
            idteam = -1
            For i As Integer = 0 To txtsearch.AutoCompleteList.Count - 1
                If txtsearch.Text = txtsearch.AutoCompleteList(i) Then idteam = i : Exit For
            Next
            Call LoadData(False)
        End If
    End Sub

    Private Sub tlbsearch_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbsearch.SubButtonClick
        Call CheckSaveState()
        txtsearch.Text = tlbsearch.Button(0).SubItems(SubButtonIndex).Text
        idteam = SubButtonIndex
        Call LoadData(False)
    End Sub

    Private Sub chkdetail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkdetail.Click
        Call SetWindowsWidth()
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Call Action(ButtonIndex)
    End Sub

    Sub HideChart()
        pnlchart.Visible = False
    End Sub

    Private Sub dtg1_CellMouseLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellMouseLeave
        Call HideChart()
    End Sub

    Private Sub dtg1_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellMouseEnter
        If e.RowIndex = -1 Then Call HideChart() : Exit Sub
        Try
            Select Case dtg1.Columns(e.ColumnIndex).HeaderText
                Case "amm", "esp", "ass", "p.g.", "m.p.", "m.v.", "gs", "gf"
                    If CStr(dtg1.Item("nome", e.RowIndex).Value) <> "" AndAlso CStr(dtg1.Item(e.ColumnIndex, e.RowIndex).Value) <> "-" Then
                        Timer2.Stop()
                        Timer2.Start()
                        Dim r As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                        If r.Top + r.Height + pnlchart.Height + dtg1.Top > Me.Height Then
                            pnlchart.Location = New Point(r.Left + r.Width \ 2 - pnlchart.Width \ 2, r.Top + dtg1.Top - gra.Height + 1)
                        Else
                            pnlchart.Location = New Point(r.Left + r.Width \ 2 - pnlchart.Width \ 2, r.Top + dtg1.Top + r.Height - 1)
                        End If
                        gra.Tag = dtg1.Columns(e.ColumnIndex).HeaderText & "," & CStr(dtg1.Item("nome", e.RowIndex).Value)
                    End If
                Case Else
                    Timer2.Stop()
                    Call HideChart()
            End Select

        Catch ex As Exception
            Call HideChart()
        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        Call LoadData(False)
    End Sub

    Private Sub Timer2_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer2.Tick

        Timer2.Stop()

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
                    'gra.ColumnCount = d.Count
                    'gra.RowCount = 1
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
            Call HideChart()
        End Try

    End Sub

    Private Sub txt_TextChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtnome.TextChange, txtalle.TextChange, txtpre.TextChange
        If start Then Exit Sub
        Call SetSaveState(True)
    End Sub

    Private Sub tlbnavsearch_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbnavsearch.ButtonClick
        If ButtonIndex > 1 Then
            Dim ind As Integer = idteam
            Select Case ButtonIndex
                Case 2 : ind = ind - 1
                Case 3 : ind = ind + 1
            End Select
            If ind < 0 Then
                ind = 0
            ElseIf ind > currlega.Settings.NumberOfTeams - 1 Then
                ind = currlega.Settings.NumberOfTeams - 1
            End If
            If ind <> idteam Then
                idteam = ind
                Call LoadData(False)
            End If
        End If
    End Sub

    Private Sub dtg1_SelectionChanged(sender As Object, e As EventArgs) Handles dtg1.SelectionChanged
        If dtg1.Columns(dtg1.CurrentCell.ColumnIndex).Name = "riconf" Then
            dtg1.ClearSelection()
        End If
    End Sub
End Class
