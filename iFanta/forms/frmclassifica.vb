Public Class frmclassifica

    Dim start As Boolean = True
    Dim gio As Integer = 1
    Dim jlist As New Dictionary(Of Integer, Integer)
    Dim loadh As Boolean = False
    Dim imgclasa As Bitmap

    Private Sub frmrose_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        AppSett.SaveSettings()
    End Sub

    Private Sub frmclassifica_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName
        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        lbby.Text = My.Application.Info.Copyright
        tlbdet.Button(1).State = currlega.ClassificationDetail
        tlbdet.draw(True)
        gio = currlega.GiornataCorrente
        tab1.Tabs(0).Text = "Ufficiale"
        tab1.Tabs(1).Text = "Top players"
        tab1.Tabs(2).Text = "Storico"
        tab1.Draw(True)
        dtg1.Columns("jolly").Visible = currlega.Settings.Jolly.EnableJollyPlayer
        dtg1.Columns("pt").Width = 45
        dtg1.Columns("pp").Width = 40
        dtg2.Columns("stemma2").Frozen = True
        dtg2.Columns("nome2").Frozen = True
        dtg2.Columns("toth").Frozen = True
        dtg2.Columns("toth").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dtg2.ColumnCount = currlega.Settings.NumberOfDays + 3
        For i As Integer = 3 To dtg2.Columns.Count - 1
            dtg2.Columns(i).Width = 35
            dtg2.Columns(i).HeaderText = CStr(i - 2)
            dtg2.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            If CBool((i Mod 2)) Then
                dtg2.Columns(i).DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
                'dtg2.Columns(i).DefaultCellStyle.BackColor = Color.Transparent
            End If
            'dtg2.Columns(i).DefaultCellStyle.BackColor = Color.Transparent
        Next

        tlbshow.CompileDictionaryButtonList()

        tlbshow.Buttons("showtype").SubItems.Clear()
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Punti", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Scarto punti", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Ammonizioni", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Espulsioni", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Assist", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Goal subiti", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Goal fatti", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Punti persi", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Totale punti persi", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("% Punti persi", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Vittorie", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Giocate in 10", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        If currlega.Settings.Jolly.EnableJollyPlayer Then tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Jolly", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        If currlega.Settings.Bonus.EnableBonusDefense OrElse currlega.Settings.Bonus.EnableCenterField OrElse currlega.Settings.Bonus.EnableBonusAttack Then
            tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Punti da bonus", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        End If
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Posizione giornata", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").SubItems.Add(New iControl.ToolBarButtonSubItem("Posizione generale", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlbshow.Buttons("showtype").ClearSubButtonSelection()
        tlbshow.Buttons("showtype").SubItems(0).State = True
        tlbshow.Buttons("showtype").SubItemsAutoSize = False
        tlbshow.Buttons("showtype").MinSubWidth = tlbshow.Buttons("showtype").Width

        Dim p As System.Reflection.PropertyInfo = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        p.SetValue(dtg1, True, Nothing)
        Call SetTheme()
        Call SetDaysList()
        Call SetWindowsWidth()

        start = False
        Timer1.Enabled = True

    End Sub

    Overrides Sub ResetTorneo()
        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)
        Call LoadClassifica()
    End Sub

    Sub SetTheme()
        Try

            SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)

            tlbshow.ForeColor = txtgg.ForeColor
            tlbshow.ForeColorSelect = txtgg.ForeColor
            tlbshow.ForeColorDropDown = txtgg.ForeColor
            For i As Integer = 0 To tlbshow.Button.Count - 1
                tlbshow.Button(i).ForeColor = txtgg.ForeColor
                tlbshow.Button(i).ForeColorSelect = txtgg.ForeColor
                tlbshow.Button(i).ForeColorDropDown = txtgg.ForeColor
            Next
            tlbshow.Top = padd \ 2 + IForm1.TY + 6
            tlbshow.Left = IForm1.RX - padd - tlbshow.Width

            tlbgg.Left = IForm1.RX - padd - 1 - tlbgg.Width
            tlbgg.Top = padd \ 2 + IForm1.TY + 6
            tlbgg.Height = txtgg.Height
            tlbgg.BorderColor = txtgg.BorderColor
            tlbgg.BorderColorDropDown = tlbgg.BorderColor
            tlbgg.Button(0).SubWidth = txtgg.Width + tlbgg.Button(0).Width - 1
            tlbgg.Button(0).SubItemsAutoSize = False
            tlbgg.FlatStyle = AppSett.Personal.Theme.FlatStyle

            txtgg.Left = tlbgg.Left - txtgg.Width + 1
            txtgg.Top = tlbgg.Top
            txtgg.FlatStyle = AppSett.Personal.Theme.FlatStyle

            tlbgg1.Top = tlbgg.Top
            tlbgg1.Left = txtgg.Left - tlbgg1.Width + 1
            tlbgg1.BorderColor = txtgg.BorderColor
            tlbgg1.BorderColorDropDown = tlbgg.BorderColor

            tab1.Left = IForm1.LX + padd + 5
            tab1.Top = tlbgg.Top
            tab1.FlatStyle = AppSett.Personal.Theme.FlatStyle

            ln1.Left = IForm1.LX + padd
            ln1.Top = tab1.Top + tab1.Height - ln1.Height \ 2 - 1
            ln1.Width = IForm1.RX - IForm1.LX - padd * 2

            tlbaction.Left = IForm1.RX - tlbaction.Width - padd
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle
            For i As Integer = 0 To tlbaction.Button.Count - 1
                tlbaction.Button(i).BorderColor = Color.DimGray
            Next

            lnbot.Top = tlbaction.Top - lnbot.Height - 5
            lnbot.Left = dtg1.Left
            lnbot.Width = IForm1.RX - IForm1.LX - padd * 2

            tlbdet.Top = lnbot.Top - tlbdet.Height
            tlbdet.Left = lnbot.Left
            tlbdet.FlatStyle = AppSett.Personal.Theme.FlatStyle

            tlbhis.Top = lnbot.Top - tlbhis.Height
            tlbhis.Left = lnbot.Left
            tlbhis.FlatStyle = AppSett.Personal.Theme.FlatStyle

            dtg1.Top = ln1.Top + ln1.Height + 8
            dtg1.Left = IForm1.LX + padd
            dtg1.Width = IForm1.RX - IForm1.LX - padd * 2
            dtg1.Height = tlbdet.Top - dtg1.Top - 5
            dtg1.FlatStyle = AppSett.Personal.Theme.FlatStyle

            dtg2.Location = dtg1.Location
            dtg2.Size = New Size(dtg1.Size.Width, dtg1.Height)

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

        Catch ex As Exception
            Call WriteError("Classifica", "SetTheme", ex.Message)
        End Try
    End Sub

    Sub SetDaysList()
        'Imposto l'elenco delle squadre'
        txtgg.AutoCompleteList.Clear()
        For i As Integer = 0 To currlega.Settings.NumberOfDays - 1
            txtgg.AutoCompleteList.Add(CStr(i + 1))
            tlbgg.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(CStr(i + 1)))
            If i + 1 = gio Then tlbgg.Button(0).SubItems(i).State = True
        Next
        txtgg.Text = CStr(gio)
    End Sub

    Sub SetWindowsWidth()

        Select Case tab1.TabSelectIndex
            Case 0, 1
                Dim c As Integer = 5
                Dim w As Integer = 0

                If tlbdet.Button(1).State Then c = -1

                For i As Integer = 0 To dtg1.Columns.Count - 2
                    Select Case dtg1.Columns(i).Name
                        Case "amm", "esp", "ass", "gs", "gf", "ptbonus", "fm"
                            dtg1.Columns(i).Visible = tlbdet.Button(1).State
                    End Select
                    If dtg1.Columns(i).Visible = True Then
                        w = w + dtg1.Columns(i).Width
                    End If
                    If dtg1.Columns(i).Name <> "nome" Then
                        dtg1.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    End If
                Next
                If dtg1.RowCount > 10 Then
                    w = w + 20
                Else
                    w = w + 5
                End If

                dtg1.Width = w

                PictureBox1.Top = dtg1.Top
                PictureBox1.Left = dtg1.Left - 5
                PictureBox1.Width = dtg1.Width + 10
                PictureBox1.Height = dtg1.Height + 25

                PictureBox1.Image = SystemFunction.DrawingAndImage.GetBackgroundImage(PictureBox1.Width, PictureBox1.Height)

                Me.Width = w + padd * 2 + IForm1.LX * 2
                Me.Top = My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2
                Me.Left = My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2
            Case 2
                'Me.Width = 800
                'Me.Top = My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2
                'Me.Left = My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2
        End Select
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            If tab1.TabSelectIndex <> 2 AndAlso txtgg.Text <> "" Then
                Dim g As Integer = CInt(txtgg.Text)
                If keyData = Keys.Left Then
                    g = g - 1
                    If g < 1 Then g = 1
                    If CStr(g) <> txtgg.Text Then
                        txtgg.Text = CStr(g)
                        Call LoadClassifica()
                    End If
                    Return True
                ElseIf keyData = Keys.Right Then
                    g = g + 1
                    If g > currlega.Settings.NumberOfDays Then g = currlega.Settings.NumberOfDays
                    If CStr(g) <> txtgg.Text Then
                        txtgg.Text = CStr(g)
                        Call LoadClassifica()
                    End If
                    Return True
                End If
            Else
                Dim g As Integer = dtg2.FirstDisplayedScrollingColumnIndex
                If keyData = Keys.Left Then
                    g = g - 1
                    If g < 3 Then g = 3
                    If g <> dtg2.FirstDisplayedScrollingColumnIndex Then
                        dtg2.FirstDisplayedScrollingColumnIndex = g
                    End If
                    Return True
                ElseIf keyData = Keys.Right Then
                    g = g + 1
                    If g <> dtg2.FirstDisplayedScrollingColumnIndex Then
                        dtg2.FirstDisplayedScrollingColumnIndex = g
                    End If
                    Return True
                End If
            End If
            If keyData = Keys.Alt + Keys.U Then
                tab1.TabSelectIndex = 0
                Call SetWindowsWidth()
                gra.BackgroundImage = Nothing
                Call LoadClassifica()
            End If
            If keyData = Keys.Alt + Keys.T Then
                tab1.TabSelectIndex = 1
                Call SetWindowsWidth()
                gra.BackgroundImage = Nothing
                Call LoadClassifica()
            End If
            If keyData = Keys.Alt + Keys.S Then
                tab1.TabSelectIndex = 2
                Call SetWindowsWidth()
                gra.BackgroundImage = Nothing
                Call LoadClassifica()
            End If
        Catch ex As Exception

        End Try
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Sub LoadClassifica()

        Dim flog As String = ""
        Dim strgg As String = txtgg.Text

        Try

            Select Case tab1.TabSelectIndex
                Case 0, 1
                    mnucopyimage.Enabled = True
                    mnusaveimage.Enabled = True
                    dtg2.Visible = False
                    gra.Visible = False
                    tlbshow.Visible = False
                    tlbgg1.Visible = True
                    tlbgg.Visible = True
                    txtgg.Visible = True
                    tlbdet.Visible = True
                    tlbhis.Visible = False
                    If strgg = "" Then strgg = "1"
                    gio = CInt(strgg)
                    If gio < 1 Then gio = 1
                    If gio > currlega.Settings.NumberOfDays Then gio = currlega.Settings.NumberOfDays

                    If tab1.TabSelectIndex = 0 Then
                        currlega.Classifica.Load(gio, False)
                    Else
                        currlega.Classifica.Load(gio, True)
                    End If

                    tlbgg.Button(0).ClearSubButtonSelection()
                    tlbgg.Button(0).SubItems(gio - 1).State = True

                    dtg1.RowCount = currlega.Classifica.Item.Count
                    dtg1.RowTemplate.Height = 160
                    dtg1.DefaultCellStyle.Font = New Font("Arial", 10.5, FontStyle.Regular, GraphicsUnit.Pixel)

                    For i As Integer = 0 To currlega.Classifica.Item.Count - 1
                        dtg1.Item("fm", i).Value = currlega.Classifica.Item(i).FantaMister
                        flog = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & currlega.Classifica.Item(i).IdTeam & "-24x24.png"
                        dtg1.Rows(i).Tag = i
                        dtg1.Item("pos", i).Style.ForeColor = Color.Red
                        dtg1.Item("pos", i).Value = currlega.Classifica.Item(i).Postion
                        If currlega.Classifica.Item(i).PreviewPostion > 0 Then
                            If currlega.Classifica.Item(i).PreviewPostion > currlega.Classifica.Item(i).Postion Then
                                dtg1.Item("varpos", i).Value = My.Resources.import16
                            ElseIf currlega.Classifica.Item(i).PreviewPostion < currlega.Classifica.Item(i).Postion Then
                                dtg1.Item("varpos", i).Value = My.Resources.export16
                            Else
                                dtg1.Item("varpos", i).Value = My.Resources.w4
                            End If
                        Else
                            dtg1.Item("varpos", i).Value = My.Resources.empty
                        End If
                        If IO.File.Exists(flog) Then
                            dtg1.Item("stemma", i).Value = New Bitmap(flog)
                        Else
                            dtg1.Item("stemma", i).Value = My.Resources.empty
                        End If
                        dtg1.Item("nome", i).Style.Font = New Font("Arial", 11.5, FontStyle.Bold, GraphicsUnit.Pixel)
                        dtg1.Item("nome", i).Value = currlega.Classifica.Item(i).Nome
                        'dtg1.Item("allenatore", i).Value = currlega.Classifica.Item(i).Allenatore
                        dtg1.Item("pt", i).Style.ForeColor = Color.RoyalBlue
                        dtg1.Item("pt", i).Style.Font = New Font("Arial", 11.5, FontStyle.Bold, GraphicsUnit.Pixel)
                        dtg1.Item("pt", i).Value = currlega.Classifica.Item(i).Pt
                        dtg1.Item("ptgio", i).Value = currlega.Classifica.Item(i).PtGio
                        dtg1.Item("diff1", i).Value = currlega.Classifica.Item(i).PtFirst
                        dtg1.Item("diff2", i).Value = currlega.Classifica.Item(i).PtPreviews
                        dtg1.Item("avgpt", i).Value = currlega.Classifica.Item(i).Avg
                        dtg1.Item("ptmin", i).Value = currlega.Classifica.Item(i).Min
                        dtg1.Item("ptmax", i).Value = currlega.Classifica.Item(i).Max
                        dtg1.Item("amm", i).Value = currlega.Classifica.Item(i).Ammonizioni
                        dtg1.Item("esp", i).Value = currlega.Classifica.Item(i).Espulsioni
                        dtg1.Item("ass", i).Value = currlega.Classifica.Item(i).Assist
                        dtg1.Item("gs", i).Value = currlega.Classifica.Item(i).GoalSubiti
                        dtg1.Item("gf", i).Value = currlega.Classifica.Item(i).GoalFatti
                        dtg1.Item("pp", i).Value = currlega.Classifica.Item(i).PuntiPersi
                        dtg1.Item("percpp", i).Value = currlega.Classifica.Item(i).PercentualePuntiPersi & "%"
                        dtg1.Item("ptbonus", i).Value = currlega.Classifica.Item(i).PtBonus
                        dtg1.Item("n10", i).Value = currlega.Classifica.Item(i).NumeroGiocateIn10
                        dtg1.Item("nbrvitt", i).Value = currlega.Classifica.Item(i).NbrWinner
                        If jlist.ContainsKey(currlega.Classifica.Item(i).IdTeam) Then
                            dtg1.Item("jolly", i).Value = jlist(currlega.Classifica.Item(i).IdTeam)
                        Else
                            dtg1.Item("jolly", i).Value = 0
                        End If
                        If currlega.Classifica.Item(i).WinnerDay = 1 Then
                            dtg1.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 180)
                            dtg1.Item("fm", i).Style.BackColor = Color.FromArgb(165, 255, 50)
                        Else
                            dtg1.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255)
                            dtg1.Item("fm", i).Style.BackColor = Color.FromArgb(196, 255, 196)
                        End If
                        dtg1.Item("chart", i).Tag = 0
                        dtg1.Rows(i).Cells("chart").Style.BackColor = Color.Transparent
                        dtg1.Rows(i).Height = 40
                    Next

                    dtg1.Visible = True
                    dtg1.Refresh()

                Case 2

                    'mnucopyimage.Enabled = False
                    'mnusaveimage.Enabled = False
                    tlbdet.Visible = False
                    tlbhis.Visible = True
                    dtg1.Visible = False
                    tlbgg1.Visible = False
                    tlbgg.Visible = False
                    txtgg.Visible = False
                    tlbshow.Visible = True
                    loadh = True

                    dtg2.DefaultCellStyle.Font = New Font("Arial", 10.5, FontStyle.Regular, GraphicsUnit.Pixel)

                    currlega.Classifica.LoadHistory()

                    If tlbhis.Button(1).State = True Then
                        dtg2.RowCount = currlega.Classifica.History.Count
                        For i As Integer = 0 To currlega.Classifica.History.Count - 1
                            flog = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & currlega.Classifica.History(i).IdTeam & "-24x24.png"
                            dtg2.Rows(i).Tag = i
                            If IO.File.Exists(flog) Then
                                dtg2.Item("stemma2", i).Value = New Bitmap(flog)
                            Else
                                dtg2.Item("stemma2", i).Value = My.Resources.empty
                            End If
                            dtg2.Item("nome2", i).Style.Font = New Font("Arial", 11.5, FontStyle.Bold, GraphicsUnit.Pixel)
                            dtg2.Item("nome2", i).Value = currlega.Classifica.History(i).Nome
                            dtg2.Rows(i).Height = 39
                            For k As Integer = 0 To currlega.Classifica.History(i).Giornate.Count - 1
                                dtg2.Item(k + 2, i).Style.ForeColor = dtg2.DefaultCellStyle.ForeColor
                                dtg2.Item(k + 2, i).Style.Font = dtg2.DefaultCellStyle.Font
                                If k = 0 Then
                                    dtg2.Item(k + 2, i).Style.ForeColor = Color.Blue
                                    dtg2.Item(k + 2, i).Style.Font = New Font(dtg2.DefaultCellStyle.Font.FontFamily, dtg2.DefaultCellStyle.Font.Size, FontStyle.Bold,GraphicsUnit.Pixel)
                                End If
                                If currlega.Classifica.History(i).Giornate(k).Pt <> -1 Then
                                    dtg2.Item(k + 2, i).Style.BackColor = Color.Empty
                                    Select Case tlbshow.Buttons("showtype").Text
                                        Case "Punti"
                                            dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).Pt / 10
                                            Dim cl As Color = Color.Empty
                                            If dtg2.Columns(k).DefaultCellStyle.BackColor <> Color.Empty Then
                                                Select Case currlega.Classifica.History(i).Giornate(k).Pt
                                                    Case 0
                                                        cl = Color.Empty
                                                    Case Is < 600
                                                        cl = Color.FromArgb(255, 235, 120)
                                                    Case Is < 700
                                                        cl = Color.Empty
                                                    Case Is < 800
                                                        cl = Color.FromArgb(190, 230, 255)
                                                    Case Else
                                                        cl = Color.FromArgb(120, 160, 255)
                                                End Select
                                            Else
                                                Select Case currlega.Classifica.History(i).Giornate(k).Pt
                                                    Case 0
                                                        cl = Color.Empty
                                                    Case Is < 600
                                                        cl = Color.FromArgb(255, 255, 140)
                                                    Case Is < 700
                                                        cl = Color.Empty
                                                    Case Is < 800
                                                        cl = Color.FromArgb(210, 240, 255)
                                                    Case Else
                                                        cl = Color.FromArgb(140, 190, 255)
                                                End Select
                                            End If
                                            If k > 0 Then
                                                dtg2.Item(k + 2, i).Style.BackColor = cl
                                            Else
                                                dtg2.Item(k + 2, i).Style.BackColor = Color.Empty
                                            End If
                                            If currlega.Classifica.History(i).Giornate(k).Vittoria = 1 Then
                                                dtg2.Item(k + 2, i).Style.ForeColor = Color.Red
                                                dtg2.Item(k + 2, i).Style.Font = New Font(dtg2.DefaultCellStyle.Font.Name, dtg2.DefaultCellStyle.Font.Size, FontStyle.Bold,GraphicsUnit.Pixel)
                                            Else
                                                dtg2.Item(k + 2, i).Style.ForeColor = dtg2.DefaultCellStyle.ForeColor
                                            End If
                                        Case "Scarto punti"
                                            dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).PtDiff / 10
                                        Case "Ammonizioni"
                                            If currlega.Classifica.History(i).Giornate(k).Ammonizioni > 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).Ammonizioni
                                            Else
                                                dtg2.Item(k + 2, i).Value = "-"
                                            End If
                                        Case "Espulsioni"
                                            If currlega.Classifica.History(i).Giornate(k).Espulsioni > 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).Espulsioni
                                            Else
                                                dtg2.Item(k + 2, i).Value = "-"
                                            End If
                                        Case "Assist"
                                            If currlega.Classifica.History(i).Giornate(k).Assist > 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).Assist
                                            Else
                                                dtg2.Item(k + 2, i).Value = "-"
                                            End If
                                        Case "Goal subiti"
                                            If currlega.Classifica.History(i).Giornate(k).GoalSubiti > 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).GoalSubiti
                                            Else
                                                dtg2.Item(k + 2, i).Value = "-"
                                            End If
                                        Case "Goal fatti"
                                            If currlega.Classifica.History(i).Giornate(k).GoalFatti > 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).GoalFatti
                                            Else
                                                dtg2.Item(k + 2, i).Value = "-"
                                            End If
                                        Case "Punti persi"
                                            dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).PtPersi / 10
                                        Case "Totale punti persi"
                                            dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).PtPersiTot / 10
                                        Case "% Punti persi"
                                            dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).PercPtPersi
                                        Case "Vittorie"
                                            If currlega.Classifica.History(i).Giornate(k).Vittoria > 0 Then
                                                If k = 0 Then
                                                    dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).Vittoria
                                                Else
                                                    dtg2.Item(k + 2, i).Value = "X"
                                                End If
                                            Else
                                                dtg2.Item(k + 2, i).Value = ""
                                            End If
                                        Case "Giocate in 10" : dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).GiocataIn10
                                            If currlega.Classifica.History(i).Giornate(k).GiocataIn10 > 0 Then
                                                If k = 0 Then
                                                    dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).GiocataIn10
                                                Else
                                                    dtg2.Item(k + 2, i).Value = "X"
                                                End If
                                            Else
                                                dtg2.Item(k + 2, i).Value = ""
                                            End If
                                        Case "Jolly"
                                            If currlega.Classifica.History(i).Giornate(k).Jolly > 0 Then
                                                If k = 0 Then
                                                    dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).Jolly
                                                Else
                                                    dtg2.Item(k + 2, i).Value = "X"
                                                End If
                                            Else
                                                dtg2.Item(k + 2, i).Value = ""
                                            End If
                                        Case "Punti da bonus"
                                            If currlega.Classifica.History(i).Giornate(k).PtBonus > 0 OrElse k = 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).PtBonus / 10
                                            Else
                                                dtg2.Item(k + 2, i).Value = ""
                                            End If
                                        Case "Posizione giornata"
                                            If currlega.Classifica.History(i).Giornate(k).Posizione > 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).Posizione
                                                If currlega.Classifica.History(i).Giornate(k).Posizione = 1 Then
                                                    dtg2.Item(k + 2, i).Style.ForeColor = Color.Red
                                                    dtg2.Item(k + 2, i).Style.Font = New Font(dtg2.DefaultCellStyle.Font.FontFamily, dtg2.DefaultCellStyle.Font.Size, FontStyle.Bold,GraphicsUnit.Pixel)
                                                End If
                                            Else
                                                dtg2.Item(k + 2, i).Value = "-"
                                            End If
                                        Case "Posizione generale"
                                            If currlega.Classifica.History(i).Giornate(k).PosizioneGenerale > 0 Then
                                                dtg2.Item(k + 2, i).Value = currlega.Classifica.History(i).Giornate(k).PosizioneGenerale
                                                If currlega.Classifica.History(i).Giornate(k).PosizioneGenerale = 1 Then
                                                    dtg2.Item(k + 2, i).Style.ForeColor = Color.Red
                                                    dtg2.Item(k + 2, i).Style.Font = New Font(dtg2.DefaultCellStyle.Font.FontFamily, dtg2.DefaultCellStyle.Font.Size, FontStyle.Bold,GraphicsUnit.Pixel)
                                                End If
                                            Else
                                                dtg2.Item(k + 2, i).Value = "-"
                                            End If
                                    End Select
                                Else
                                    dtg2.Item(k + 2, i).Value = ""
                                End If
                            Next
                        Next
                        dtg2.Visible = True
                        dtg2.Refresh()
                        gra.Visible = False

                    Else

                        Dim cl() As Color = {Color.Red, Color.Blue, Color.Black, Color.Violet, Color.Yellow, Color.Orange, Color.FromArgb(0, 255, 255), Color.CornflowerBlue, Color.FromArgb(255, 0, 128), Color.FromArgb(0, 255, 0)}
                        Dim indcl As Integer = 0
                        Dim ncl As Integer = cl.Length - 1

                        mnucopyimage.Enabled = True
                        mnusaveimage.Enabled = True
                        dtg2.Visible = False
                        gra.Visible = False
                        gra.SuspundeLayout = True
                        gra.Language = iChart.iChart.eLanguage.Italian
                        gra.Legend.Visible = True
                        gra.Legend.BorderSize = 0
                        gra.Legend.BorderColor = Color.White
                        gra.Legend.CheckBoxShowSerie = True
                        gra.DefaultSeriesStyle.Pen.Width = 2
                        gra.DefaultSeriesStyle.EnabledHighlightLine = True
                        gra.DefaultSeriesStyle.EnabledGlowEffectHighLightLine = True
                        gra.DefaultSeriesStyle.SmoothLines = 0.1
                        gra.RectangleSelection.EnableRectangleSelection = False
                        If AppSett.Personal.Theme.FlatStyle Then
                            gra.ChartAreaBackColor1 = Color.WhiteSmoke
                            gra.ChartAreaBackColor2 = Color.WhiteSmoke
                        Else
                            gra.ChartAreaBackColor1 = Color.White
                            gra.ChartAreaBackColor2 = Color.WhiteSmoke
                        End If
                        gra.Info.Visible = True
                        gra.Info.Type = iChart.Info.eType.SerieAndValue
                        gra.Info.Font = New Font("Arial", 11, FontStyle.Bold,GraphicsUnit.Pixel)
                        gra.Info.BorderColor = Color.Gray
                        gra.Info.BackColor1 = Color.WhiteSmoke
                        gra.Info.BackColor2 = Color.Gainsboro
                        gra.Info.ForeColor = Color.FromArgb(50, 50, 50)
                        gra.BorderSize = 0
                        gra.RowCount = currlega.Classifica.History.Count
                        gra.Axsis(iChart.Axsis.Axsis.X).TextAlignment = iChart.Axsis.Alignment.Normal
                        gra.Axsis(iChart.Axsis.Axsis.Y).VisiblePrimaryAxsis = True
                        gra.Axsis(iChart.Axsis.Axsis.Y).VisibleSecondaryAxsis = True

                        Select Case tlbshow.Buttons("showtype").Text
                            Case "Vittorie", "Jolly", "Giocate in 10"
                                If tlbshow.Buttons("showtype").Text = "Vittorie" Then
                                    gra.DefaultSeriesStyle.EnabledHighlightLine = False
                                    gra.DefaultSeriesStyle.EnabledGlowEffectHighLightLine = False
                                    gra.Type = iChart.iChart.ChartType.Bar
                                    gra.Axsis(iChart.Axsis.Axsis.Y).VisibleMaxDivisionLabel = False
                                    gra.Axsis(iChart.Axsis.Axsis.Y).VisibleMinDivisionLabel = False
                                    gra.Info.Type = iChart.Info.eType.Series
                                    gra.AutoScale = iChart.iChart.eAutoScale.None : gra.Min = 0 : gra.Max = 1 : gra.Axsis(iChart.Axsis.Axsis.Y).Division = 1
                                Else
                                    gra.Type = iChart.iChart.ChartType.BarTotal
                                    gra.AutoScale = iChart.iChart.eAutoScale.None : gra.Min = 0 : gra.Max = currlega.Settings.Jolly.MaximumNumberJollyPlayableForDay * currlega.Settings.NumberOfTeams : gra.Axsis(iChart.Axsis.Axsis.Y).Division = 5
                                End If
                                gra.Axsis(iChart.Axsis.Axsis.X).TextAlignment = iChart.Axsis.Alignment.Center
                                gra.ColumnCount = currlega.Settings.NumberOfDays + 1
                                gra.Axsis(iChart.Axsis.Axsis.Y).VisiblePrimaryAxsis = False
                                gra.Axsis(iChart.Axsis.Axsis.Y).VisibleSecondaryAxsis = False
                            Case "Posizione giornata", "Posizione generale"
                                gra.Type = iChart.iChart.ChartType.Linear
                                gra.AutoScale = iChart.iChart.eAutoScale.None : gra.Min = 1 : gra.Max = currlega.Settings.NumberOfTeams : gra.Axsis(iChart.Axsis.Axsis.Y).Division = currlega.Settings.NumberOfTeams - 1 : gra.Axsis(iChart.Axsis.Axsis.Y).Division = 5
                                gra.ColumnCount = currlega.Settings.NumberOfDays
                            Case "Scarto punti"
                                Dim diffmax As Double = 0
                                Dim min As Integer = 0
                                For i As Integer = 0 To currlega.Classifica.History.Count - 1
                                    For k As Integer = 0 To currlega.Classifica.History(i).Giornate.Count - 1
                                        If currlega.Classifica.History(i).Giornate(k).PtDiff < diffmax Then diffmax = currlega.Classifica.History(i).Giornate(k).PtDiff
                                    Next
                                Next
                                min = CInt(Math.Ceiling(-diffmax / 50) * 5)
                                'gra.Min = -min : gra.Max = 0 : gra.Axsis(iChart.Axsis.Axsis.Y).Division = 5
                                gra.AutoScale = iChart.iChart.eAutoScale.MinMaxAutoScale
                                gra.ColumnCount = currlega.Settings.NumberOfDays
                            Case Else
                                gra.AutoScale = iChart.iChart.eAutoScale.MinMaxAutoScale
                                gra.Axsis(iChart.Axsis.Axsis.Y).Division = 5
                                gra.Type = iChart.iChart.ChartType.Linear
                                gra.ColumnCount = currlega.Settings.NumberOfDays
                        End Select

                        For i As Integer = 0 To currlega.Classifica.History.Count - 1
                            Dim str As String = currlega.Classifica.History(i).Nome
                            If str.Length > 10 Then str = str.Substring(0, 9)
                            gra.Series.Item(i).Name = str
                            gra.Series.Item(i).Style.Pen.Color = cl(indcl)
                            gra.Series.Item(i).Style.Brush.Color1 = cl(indcl)
                            Dim clb1() As Integer = {cl(indcl).R, cl(indcl).G, cl(indcl).B}
                            Dim clb2() As Integer = {cl(indcl).R, cl(indcl).G, cl(indcl).B}
                            If AppSett.Personal.Theme.FlatStyle = False Then
                                For k As Integer = 0 To 2
                                    clb1(k) = clb1(k) - 50
                                    clb2(k) = clb2(k) + 50
                                    If clb1(k) > 255 Then clb1(k) = 255
                                    If clb1(k) < 0 Then clb1(k) = 0
                                    If clb2(k) > 255 Then clb2(k) = 255
                                    If clb2(k) < 0 Then clb2(k) = 0
                                Next
                            End If
                            If gra.Type = iChart.iChart.ChartType.Linear Then
                                gra.Series.Item(i).Style.BorderSize = 0
                                gra.Series.Item(i).Style.InternalBorderSize = 0
                            Else
                                gra.Series.Item(i).Style.BorderSize = 2
                                gra.Series.Item(i).Style.BorderColor = Color.FromArgb(50, 0, 0, 0)
                                gra.Series.Item(i).Style.InternalBorderSize = 1
                                gra.Series.Item(i).Style.InternalBorderColor = Color.FromArgb(50, 255, 255, 255)
                            End If
                            gra.Series.Item(i).Style.Brush.Color1 = Color.FromArgb(clb1(0), clb1(1), clb1(2))
                            gra.Series.Item(i).Style.Brush.Color2 = Color.FromArgb(clb2(0), clb2(1), clb2(2))
                            gra.Axsis(iChart.Axsis.Axsis.Y).DecimalNumber = 0
                            gra.Axsis(iChart.Axsis.Axsis.Y).Inverse = False
                            For k As Integer = 1 To currlega.Classifica.History(i).Giornate.Count - 1
                                If currlega.Classifica.History(i).Giornate(k).Pt <> -1 Then
                                    Select Case tlbshow.Buttons("showtype").Text
                                        Case "Punti"
                                            gra.Title.Text = "Andamento punti giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).Pt / 10)
                                            gra.Axsis(iChart.Axsis.Axsis.Y).DecimalNumber = 1
                                        Case "Scarto punti"
                                            gra.Axsis(iChart.Axsis.Axsis.Y).Inverse = True
                                            gra.Title.Text = "Andamento scarto punti dalla prima in classifica"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).PtDiff / 10)
                                            gra.Axsis(iChart.Axsis.Axsis.Y).DecimalNumber = 1
                                        Case "Ammonizioni"
                                            gra.Title.Text = "Andamento ammonizioni totali giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).Ammonizioni)
                                        Case "Espulsioni"
                                            gra.Title.Text = "Andamento espulsioni totali giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).Espulsioni)
                                        Case "Assist"
                                            gra.Title.Text = "Andamento assist totali giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).Assist)
                                        Case "Goal subiti"
                                            gra.Title.Text = "Andamento goal subiti totali"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).GoalSubiti)
                                        Case "Goal fatti"
                                            gra.Title.Text = "Andamento goal fatti totali"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).GoalFatti)
                                        Case "Punti persi"
                                            gra.Title.Text = "Andamento punti persi giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).PtPersi / 10)
                                            gra.Axsis(iChart.Axsis.Axsis.Y).DecimalNumber = 1
                                        Case "Totale punti persi"
                                            gra.Title.Text = "Andamento totale punti persi giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).PtPersiTot / 10)
                                            gra.Axsis(iChart.Axsis.Axsis.Y).DecimalNumber = 1
                                        Case "% Punti persi"
                                            gra.Title.Text = "Andamento percentuale totale punti persi giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).PercPtPersi)
                                            gra.Axsis(iChart.Axsis.Axsis.Y).DecimalNumber = 1
                                        Case "Vittorie"
                                            gra.Title.Text = "Storico vittorie di giornata"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).Vittoria)
                                        Case "Giocate in 10"
                                            gra.Title.Text = "Storico giocate in 10"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).GiocataIn10)
                                        Case "Jolly"
                                            gra.Title.Text = "Storico jolly giocati"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).Jolly)
                                        Case "Punti da bonus"
                                            gra.Title.Text = "Storico punti da bonus"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).PtBonus / 10)
                                            gra.Axsis(iChart.Axsis.Axsis.Y).DecimalNumber = 1
                                        Case "Posizione giornata"
                                            gra.Title.Text = "Storico posizioni di giornata"
                                            gra.Axsis(iChart.Axsis.Axsis.Y).Inverse = True
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).Posizione)
                                        Case "Posizione generale"
                                            gra.Axsis(iChart.Axsis.Axsis.Y).Inverse = True
                                            gra.Title.Text = "Andamento posizioni in classifica generale"
                                            gra.Item(i, k - 1) = CStr(currlega.Classifica.History(i).Giornate(k).PosizioneGenerale)
                                    End Select
                                Else
                                    'gra.Series.Item(i).Value(k - 1) = ""
                                    gra.Item(i, k - 1) = ""
                                End If
                            Next
                            indcl = indcl + 1
                            If indcl > ncl Then indcl = 0
                        Next
                        gra.Size = dtg2.Size
                        gra.Location = dtg2.Location
                        For i As Integer = 0 To gra.ColumnCount - 1
                            gra.Axsis(iChart.Axsis.Axsis.X).Item(i).Value = CStr(i + 1)
                        Next
                        gra.SuspundeLayout = False
                        'gra.AutoScale = iChart.iChart.eAutoScale.MaxAutoScale
                        gra.Draw()
                        gra.Visible = True
                    End If
            End Select

            imgclasa = Nothing

        Catch ex As Exception
            ShowError("Errore", ex.Message)
            Call WriteError("Classifica", "LoadClassifica", ex.Message)
        End Try

    End Sub

    Sub Action(ByVal Act As Integer)
        Try
            mnu1.Hide()
            Select Case Act
                Case 0
                    Dim frm As New frmimpexp
                    frm.SetParameater(frmimpexp.TypeOfOperation.Export)
                    If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Select Case tab1.TabSelectIndex
                            Case 0 : Call ImpExp.ImpExpRating.ExportHtml(frm.GetDirectory, False)
                            Case 1 : Call ImpExp.ImpExpRating.ExportHtml(frm.GetDirectory, True)
                            Case 2 : Call ImpExp.ImpExpRating.ExportHistoryHtml(tlbshow.Buttons("showtype").Text.ToUpper)
                        End Select
                    End If
                    frm.Dispose()
                Case 1 : Me.Close()
                Case 2
                    Select Case tab1.TabSelectIndex
                        Case 0, 1 : My.Computer.Clipboard.SetImage(SystemFunction.DrawingAndImage.ConvertDatagridToImage(dtg1))
                        Case 2
                            If tlbhis.Button(0).State Then
                                My.Computer.Clipboard.SetImage(SystemFunction.DrawingAndImage.ConvertDatagridToImage(dtg2))
                            Else
                                My.Computer.Clipboard.SetImage(gra.GetImage)
                            End If
                    End Select
                Case 3
                    Select Case tab1.TabSelectIndex
                        Case 0, 1
                            SystemFunction.General.CopyDataTable(True, dtg1)
                        Case 2
                            SystemFunction.General.CopyDataTable(True, dtg2)
                    End Select
                Case 4
                    Dim dlg As New Windows.Forms.FolderBrowserDialog
                    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Select Case tab1.TabSelectIndex
                            Case 0
                                Dim fname As String = ImpExp.ImpExpRating.GetHtmlFileName(False, dlg.SelectedPath, "png")
                                SystemFunction.DrawingAndImage.SaveDatagridToImage(dtg1, fname)
                            Case 1
                                Dim fname As String = ImpExp.ImpExpRating.GetHtmlFileName(True, dlg.SelectedPath, "png")
                                SystemFunction.DrawingAndImage.SaveDatagridToImage(dtg1, fname)
                            Case 2
                                Dim fname As String = ImpExp.ImpExpRating.GetHtmlHistoryFileName(tlbshow.Buttons("showtype").Text.ToUpper, dlg.SelectedPath, "png")
                                If tlbhis.Button(0).State Then
                                    SystemFunction.DrawingAndImage.SaveDatagridToImage(dtg2, fname)
                                Else
                                    gra.GetImage.Save(fname)
                                End If
                        End Select
                    End If
            End Select
        Catch ex As Exception
            ShowError("Errore", ex.Message)
            Call WriteError("Classifica", "Action " & Act, ex.Message)
        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        'Determino il numero dei jolly utilizzati dai vari team'
        jlist = currlega.GetNumbersJollyUsedByTeams
        'Carico la classifica'
        Call LoadClassifica()
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Private Sub dtg1_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellMouseEnter
        Try
            If e.ColumnIndex = 3 And e.RowIndex <> -1 Then

                picforma.Visible = False

                If imgclasa Is Nothing AndAlso dtg1.Visible = True AndAlso start = False Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                pichistory.Visible = False

                Dim ind As Integer = currlega.Classifica.Item(CInt(dtg1.Rows(e.RowIndex).Tag)).IdTeam
                Dim f As New List(Of LegaObject.Formazione)

                If tab1.TabSelectIndex = 0 Then
                    f = currlega.GetFormazioni(gio, ind, False)
                    currlega.Classifica.Load(gio, False)
                Else
                    f = currlega.GetFormazioni(gio, ind, True)
                End If

                picforma.Height = 400
                picforma.Width = 250

                If f.Count > 0 Then

                    Dim rec As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                    picforma.Left = rec.Left + rec.Width + dtg1.Left + 5
                    If rec.Y + +dtg1.Top + picforma.Height > Me.Height - 70 Then
                        picforma.Top = Me.Height - 70 - picforma.Height
                    Else
                        picforma.Top = rec.Top + dtg1.Top
                    End If

                    picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f(0), "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                    'Dim myGraphicsPath As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath
                    'myGraphicsPath.AddRectangle(New Rectangle(1, 0, picforma.Width - 2, picforma.Height))
                    'myGraphicsPath.AddRectangle(New Rectangle(0, 1, 1, picforma.Height - 2))
                    'myGraphicsPath.AddRectangle(New Rectangle(picforma.Width - 1, 1, 1, picforma.Height - 2))
                    'picforma.Region = New Region(myGraphicsPath)
                    'myGraphicsPath.Dispose()

                    picforma.Visible = True
                Else
                    picforma.Visible = False
                End If

            ElseIf tab1.TabSelectIndex = 0 AndAlso (e.ColumnIndex = 0 OrElse e.ColumnIndex = 4 OrElse e.ColumnIndex = 5 OrElse e.ColumnIndex = 6 OrElse e.ColumnIndex = 11 OrElse e.ColumnIndex = 12 OrElse e.ColumnIndex = 13 OrElse e.ColumnIndex = 14 OrElse e.ColumnIndex = 15 OrElse e.ColumnIndex = 19) AndAlso e.RowIndex <> -1 Then

                Dim indteam As Integer = currlega.Classifica.Item(CInt(dtg1.Rows(e.RowIndex).Tag)).IdTeam
                Dim farms As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & indteam & "-24x24.png"
                Dim f1 As New StringFormat
                Dim f2 As New StringFormat
                Dim w As Integer = pichistory.Width
                Dim h As Integer = pichistory.Height
                Dim rec As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)

                picforma.Visible = False
                pichistory.Visible = False

                If imgclasa Is Nothing AndAlso dtg1.Visible = True AndAlso start = False Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                f2.Alignment = StringAlignment.Far
                f1.Alignment = StringAlignment.Center
                f1.LineAlignment = StringAlignment.Center

                'Se necessario carico la classifica'
                If loadh = False Then currlega.Classifica.LoadHistory()
                loadh = True

                'Posiziono la picturebox'
                If rec.X + dtg1.Left + pichistory.Width > Me.Width - 25 Then
                    pichistory.Left = rec.Left - pichistory.Width
                Else
                    pichistory.Left = rec.Left + rec.Width + dtg1.Left + 5
                End If

                If rec.Y + dtg1.Top + pichistory.Height > Me.Height - 70 Then
                    pichistory.Top = Me.Height - 70 - pichistory.Height
                Else
                    pichistory.Top = rec.Top + dtg1.Top
                End If

                Dim gr As Graphics
                Dim b1 As New Bitmap(pichistory.Width, pichistory.Height)

                gr = Graphics.FromImage(b1)

                gr.Clear(Color.White)
                gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

                'Disegno lo sfondo i bordi della finestra'
                Dim br2 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, w, h - 1), Color.FromArgb(160, 160, 160), Color.DimGray, Drawing2D.LinearGradientMode.Vertical)
                'Dim br3 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, w - 3, h - 3), Color.White, Color.FromArgb(230, 230, 230), Drawing2D.LinearGradientMode.Vertical)
                Dim dy As Integer = 40
                Dim dy1 As Integer = 30
                Dim deltaw As Integer = pichistory.Width \ 10
                Dim deltah As Integer = (pichistory.Height - dy - dy1) \ 4

                gr.DrawImage(iControl.CommonFunction.GetAreaImage(imgclasa, pichistory.Left, pichistory.Top, pichistory.Width, pichistory.Height), 0, 0)

                For i As Integer = 0 To 2
                    Dim br1 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, w, h - 1), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(30 + i * 10, 0, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                    gr.FillPath(br1, SystemFunction.DrawingAndImage.GetBorderDullPath1(gr, New Rectangle(i, i, w - i * 2, h - i * 2 - 20), 16 - i * 2))
                Next

                gr.FillPath(br2, SystemFunction.DrawingAndImage.GetBorderDullPath1(gr, New Rectangle(3, 3, w - 6, h - 28), 10))
                gr.FillPath(Brushes.White, SystemFunction.DrawingAndImage.GetBorderDullPath1(gr, New Rectangle(4, 4, w - 8, h - 30), 12))

                'Disegno logo e nome squadra'
                If IO.File.Exists(farms) Then
                    gr.DrawImage(Image.FromFile(farms), 7, 7)
                End If
                gr.DrawString(currlega.Classifica.History(indteam).Nome, New Font("Arial", 15, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.Red, 32, 7)
                gr.DrawString(currlega.Classifica.History(indteam).Allenatore, New Font("Arial", 11, FontStyle.Regular,GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(50, 50, 50)), 32, 21)

                Dim strt As String = ""

                Select Case e.ColumnIndex
                    Case 0 : strt = "Posizione generale"
                    Case 4, 5 : strt = "Punti giornata"
                    Case 6 : strt = "Punti di distacco"
                    Case 11 : strt = "Ammonizioni"
                    Case 12 : strt = "Espulsioni"
                    Case 13 : strt = "Assist"
                    Case 14 : strt = "Goal subiti"
                    Case 15 : strt = "Goal fatti"
                    Case 19 : strt = "Vittorie"
                End Select
                gr.DrawString(strt, New Font("Arial", 14, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.RoyalBlue, pichistory.Width - 6, 10, f2)

                'Disegno la griglia'
                gr.DrawLine(New Pen(Color.Gainsboro, 1), 4, dy - 2, pichistory.Width - 5, dy - 2)
                For i As Integer = 1 To 9
                    gr.DrawLine(New Pen(Color.Gainsboro, 1), deltaw * i, dy, deltaw * i, pichistory.Height - 5 - dy1)
                Next
                For i As Integer = 1 To 3
                    gr.DrawLine(New Pen(Color.Gainsboro, 1), 8, deltah * i + dy, pichistory.Width - 12, deltah * i + dy)
                Next

                'Disegno i valori'
                For i As Integer = 1 To 4
                    For j As Integer = 1 To 10
                        Dim g As Integer = (i - 1) * 10 + j
                        If g < 39 Then
                            gr.DrawString(CStr(g), New Font("Arial", 10, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.Red, deltaw * j - deltaw \ 2, deltah * i - deltah + 8 + dy, f1)
                            If currlega.Classifica.History(indteam).Giornate(g).Pt >= 0 Then
                                Dim str As String = ""
                                Select Case e.ColumnIndex
                                    Case 0 : str = CStr(currlega.Classifica.History(indteam).Giornate(g).PosizioneGenerale)
                                    Case 4, 5 : str = CStr(CSng(currlega.Classifica.History(indteam).Giornate(g).Pt) / 10)
                                    Case 6 : str = CStr(CSng(currlega.Classifica.History(indteam).Giornate(g).PtDiff) / 10)
                                    Case 11 : str = CStr(currlega.Classifica.History(indteam).Giornate(g).Ammonizioni)
                                    Case 12 : str = CStr(currlega.Classifica.History(indteam).Giornate(g).Espulsioni)
                                    Case 13 : str = CStr(currlega.Classifica.History(indteam).Giornate(g).Assist)
                                    Case 14 : str = CStr(currlega.Classifica.History(indteam).Giornate(g).GoalSubiti)
                                    Case 15 : str = CStr(currlega.Classifica.History(indteam).Giornate(g).GoalFatti)
                                    Case 19 : str = CStr(currlega.Classifica.History(indteam).Giornate(g).Vittoria)
                                End Select
                                gr.DrawString(str, New Font("Arial", 11, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.DimGray, deltaw * j - deltaw \ 2, deltah * i - deltah \ 2 + 6 + dy, f1)
                            End If
                        End If
                    Next
                Next

                pichistory.Image = CType(b1.Clone, Image)
                b1.Dispose()
                gr.Dispose()

                pichistory.Visible = True

            Else
                picforma.Visible = False
                pichistory.Visible = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dtg1_ColumnHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dtg1.ColumnHeaderMouseClick
        'Try
        '    Select Case dtg1.Columns(e.ColumnIndex).Name
        '        Case "devvt", "devpt", "imgpt", "var", "chart"
        '        Case Else
        '            Dim rev As Integer = CInt(dtg1.Columns(e.ColumnIndex).Tag)
        '            Dim f As String = dtg1.Columns(e.ColumnIndex).HeaderText
        '            If f = "" Then f = "idrosa"
        '            If rev = -1 OrElse rev = 0 Then
        '                rev = 1
        '            Else
        '                rev = -1
        '            End If
        '            For i As Integer = 0 To dtg1.Columns.Count - 1
        '                dtg1.Columns(i).Tag = 0
        '            Next
        '            dtg1.Columns(e.ColumnIndex).Tag = rev
        '            Call LoadClassifica()
        '    End Select
        'Catch ex As Exception

        'End Try
    End Sub

    Private Sub dtg1_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles dtg1.CellPainting

        Try
            If dtg1.Columns(e.ColumnIndex).Name = "chart" AndAlso e.RowIndex <> -1 Then

                Dim padtop As Integer = 6
                Dim padleft As Integer = 3
                Dim h As Integer = dtg1.Rows(e.RowIndex).Height - padtop * 2 - 1
                Dim w As Integer = e.CellBounds.Width - padleft * 2
                Dim x As Integer = e.CellBounds.Left + padleft
                Dim y As Integer = e.CellBounds.Top + padtop
                Dim wi As Integer = w - 2
                Dim ci1 As Color = Color.White
                Dim ci2 As Color = Color.FromArgb(210, 210, 210)
                Dim cp1 As Color = Color.White
                Dim cp2 As Color = Color.FromArgb(210, 210, 210)
                Dim ind As Integer = CInt(dtg1.Rows(e.RowIndex).Tag)

                Dim pp As Double = 1
                If currlega.Classifica.PtMin <> currlega.Classifica.PtMax Then pp = (dtg1.Columns(e.ColumnIndex).Width - 15) / (currlega.Classifica.PtMax - currlega.Classifica.PtMin + 5)
                pp = 10 + (currlega.Classifica.Item(ind).Pt - currlega.Classifica.PtMin) * pp

                Select Case pp
                    Case Is < 1
                        cp1 = Color.White
                        cp2 = Color.FromArgb(210, 210, 210)
                    Case Else
                        cp1 = Color.FromArgb(0, 255, 0)
                        cp2 = Color.FromArgb(0, 180, 0)
                        wi = CInt(pp)
                End Select

                If AppSett.Personal.Theme.FlatStyle = True Then ci2 = ci1 : cp1 = cp2

                Dim bi1 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, Width - 2, h - 1), ci1, ci2, Drawing2D.LinearGradientMode.Vertical)
                Dim bi2 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, Width - 2, h - 1), cp1, cp2, Drawing2D.LinearGradientMode.Vertical)

                If dtg1.Rows(e.RowIndex).Selected = False Then e.Graphics.FillRectangle(New SolidBrush(dtg1.Rows(e.RowIndex).DefaultCellStyle.BackColor), e.CellBounds)
                e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(120, 120, 120)), New Rectangle(x, y, wi, h))
                'e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(120, 120, 120)), New Rectangle(x, y, w, h))
                'e.Graphics.FillRectangle(bi1, New Rectangle(x + 1, y + 1, w - 2, h - 2))
                e.Graphics.FillRectangle(bi2, New Rectangle(x + 1, y + 1, wi - 2, h - 2))
                If AppSett.Personal.Theme.FlatStyle = False Then e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(80, 255, 255, 255)), New Rectangle(x + 1, y + 1, wi - 2, CInt((h - 2) / 2)))

            End If
        Catch ex As Exception
            Call WriteError("Classifica", "CellPanting", ex.Message)
        End Try

    End Sub

    Private Sub txtgg_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtgg.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call LoadClassifica()
        End If
    End Sub

    Private Sub tlbgg_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbgg.SubButtonClick
        txtgg.Text = tlbgg.Button(ButtonIndex).SubItems(SubButtonIndex).Text
        Call LoadClassifica()
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Call Action(ButtonIndex)
    End Sub

    Private Sub mnu1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnu1.ItemClicked
        Call Action(CInt(e.ClickedItem.Tag))
    End Sub

    Private Sub chkdetail_Click(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbdet.ButtonClick
        If ButtonIndex = 1 Then
            currlega.ClassificationDetail = tlbdet.Button(1).State
            Call SetWindowsWidth()
        End If
    End Sub

    Private Sub tab1_TabClick(ByVal sender As Object, ByVal TabInd As Integer) Handles tab1.TabClick
        picforma.Visible = False
        Call SetWindowsWidth()
        gra.BackgroundImage = Nothing
        Call LoadClassifica()
    End Sub

    'Private Sub dtg2_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtg2.RowPrePaint
    '    e.Graphics.Clear(Color.White)
    'End Sub

    Private Sub dtg1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtg1.SelectionChanged
        dtg1.ClearSelection()
    End Sub

    Private Sub dtg2_CellPainting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles dtg2.CellPainting
        'If e.RowIndex <> -1 AndAlso e.ColumnIndex > 2 Then
        '    e.Graphics.Clear(Color.White)
        '    e.Graphics.DrawLine(New Pen(dtg2.GridColor, 1), e.CellBounds.Left + 1, e.CellBounds.Top, e.CellBounds.Left + 1, e.CellBounds.Top + e.CellBounds.Height)
        '    If CStr(dtg2.Item(e.ColumnIndex, e.RowIndex).Value) <> "" Then
        '        Select Case tlbshow.Button(1).Text
        '            Case "Punti"
        '                Dim ft As New StringFormat
        '                ft.Alignment = StringAlignment.Center
        '                ft.LineAlignment = StringAlignment.Center
        '                e.Graphics.DrawString(CStr(dtg2.Item(e.ColumnIndex, e.RowIndex).Value), dtg1.DefaultCellStyle.Font, New SolidBrush(dtg1.Item(e.ColumnIndex, e.RowIndex).Style.ForeColor), e.CellBounds.Left + e.CellBounds.Width \ 2, e.CellBounds.Top + e.CellBounds.Height \ 2, ft)
        '                'Case "Punti" : dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).Pt / 10
        '                'Case "Ammonizioni"
        '                '    If lega.Classifica.History(i).Giornate(k).Ammonizioni > 0 Then
        '                '        dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).Ammonizioni
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = "-"
        '                '    End If
        '                'Case "Espulsioni"
        '                '    If lega.Classifica.History(i).Giornate(k).Espulsioni > 0 Then
        '                '        dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).Espulsioni
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = "-"
        '                '    End If
        '                'Case "Assist"
        '                '    If lega.Classifica.History(i).Giornate(k).Assist > 0 Then
        '                '        dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).Assist
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = "-"
        '                '    End If
        '                'Case "Goal subiti"
        '                '    If lega.Classifica.History(i).Giornate(k).GoalSubiti > 0 Then
        '                '        dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).GoalSubiti
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = "-"
        '                '    End If
        '                'Case "Goal fatti"
        '                '    If lega.Classifica.History(i).Giornate(k).GoalFatti > 0 Then
        '                '        dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).GoalFatti
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = "-"
        '                '    End If
        '                'Case "Vittorie"
        '                '    If lega.Classifica.History(i).Giornate(k).Vittoria > 0 Then
        '                '        If k = 0 Then
        '                '            dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).Vittoria
        '                '        Else
        '                '            dtg2.Item(k + 2, i).Value = "X"
        '                '        End If
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = ""
        '                '    End If
        '                'Case "Giocate in 10" : dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).GiocataIn10
        '                '    If lega.Classifica.History(i).Giornate(k).GiocataIn10 > 0 Then
        '                '        If k = 0 Then
        '                '            dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).GiocataIn10
        '                '        Else
        '                '            dtg2.Item(k + 2, i).Value = "X"
        '                '        End If
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = ""
        '                '    End If
        '                'Case "Jolly"
        '                '    If lega.Classifica.History(i).Giornate(k).Jolly > 0 Then
        '                '        If k = 0 Then
        '                '            dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).Jolly
        '                '        Else
        '                '            dtg2.Item(k + 2, i).Value = "X"
        '                '        End If
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = ""
        '                '    End If
        '                'Case "Posizione giornata"
        '                '    If lega.Classifica.History(i).Giornate(k).Posizione > 0 Then
        '                '        dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).Posizione
        '                '        If lega.Classifica.History(i).Giornate(k).Posizione = 1 Then
        '                '            dtg2.Item(k + 2, i).Style.ForeColor = Color.Red
        '                '            dtg2.Item(k + 2, i).Style.Font = New Font(dtg2.DefaultCellStyle.Font.FontFamily, dtg2.DefaultCellStyle.Font.Size, FontStyle.Bold,GraphicsUnit.Pixel)
        '                '        End If
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = "-"
        '                '    End If
        '                'Case "Posizione generale"
        '                '    If lega.Classifica.History(i).Giornate(k).PosizioneGenerale > 0 Then
        '                '        dtg2.Item(k + 2, i).Value = lega.Classifica.History(i).Giornate(k).PosizioneGenerale
        '                '        If lega.Classifica.History(i).Giornate(k).PosizioneGenerale = 1 Then
        '                '            dtg2.Item(k + 2, i).Style.ForeColor = Color.Red
        '                '            dtg2.Item(k + 2, i).Style.Font = New Font(dtg2.DefaultCellStyle.Font.FontFamily, dtg2.DefaultCellStyle.Font.Size, FontStyle.Bold,GraphicsUnit.Pixel)
        '                '        End If
        '                '    Else
        '                '        dtg2.Item(k + 2, i).Value = "-"
        '                '    End If
        '        End Select
        '    End If
        'End If
    End Sub

    Private Sub dtg2_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtg2.SelectionChanged
        dtg2.ClearSelection()
    End Sub

    Private Sub tlbgg1_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbgg1.ButtonClick
        If ButtonIndex > 1 Then
            Dim exe As Boolean = True
            Select Case ButtonIndex
                Case 2 : gio = gio - 1
                Case 3 : gio = gio + 1
            End Select
            If gio < 1 Then gio = 1 : exe = False
            If gio > currlega.Settings.NumberOfDays Then gio = currlega.Settings.NumberOfDays : exe = False
            If exe Then
                tlbgg.Button(0).ClearSubButtonSelection()
                tlbgg.Button(0).SubItems(gio - 1).State = True
                txtgg.Text = CStr(gio)
                Call LoadClassifica()
            End If
        End If
    End Sub

    Private Sub tlbshow_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbshow.ButtonClick
        Select Case ButtonIndex
            Case 1, 2

                Dim ind As Integer = 0
                Dim cind As Integer = 0

                For i As Integer = 0 To tlbshow.Buttons("showtype").SubItems.Count - 1
                    If tlbshow.Buttons("showtype").SubItems(i).State Then
                        ind = i
                        Exit For
                    End If
                Next

                cind = ind

                If ButtonIndex = 1 Then
                    ind = ind - 1
                ElseIf ButtonIndex = 2 Then
                    ind = ind + 1
                End If

                If ind < 0 Then
                    ind = 0
                ElseIf ind > tlbshow.Buttons("showtype").SubItems.Count - 1 Then
                    ind = tlbshow.Buttons("showtype").SubItems.Count - 1
                End If

                If ind <> cind Then
                    tlbshow.Buttons("showtype").Text = tlbshow.Buttons("showtype").SubItems(ind).Text
                    tlbshow.Buttons("showtype").ClearSubButtonSelection()
                    tlbshow.Buttons("showtype").SubItems(ind).State = True
                    tlbshow.draw(True)
                    tlbshow.Left = IForm1.RX - padd - tlbshow.Width
                    Call LoadClassifica()
                End If
        End Select
    End Sub

    Private Sub tlbshow_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbshow.SubButtonClick
        tlbshow.Button(ButtonIndex).Text = tlbshow.Button(ButtonIndex).SubItems(SubButtonIndex).Text
        tlbshow.Button(ButtonIndex).ClearSubButtonSelection()
        tlbshow.Button(ButtonIndex).SubItems(SubButtonIndex).State = True
        tlbshow.draw(True)
        tlbshow.Left = IForm1.RX - padd - tlbshow.Width
        Call LoadClassifica()
    End Sub

    Private Sub tlbhis_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbhis.ButtonClick
        Dim s As Boolean = tlbhis.Button(ButtonIndex).State
        Select Case ButtonIndex
            Case 1 : tlbhis.Button(2).State = Not (s)
            Case 2 : tlbhis.Button(3).State = Not (s)
        End Select
        Call LoadClassifica()
    End Sub

End Class
