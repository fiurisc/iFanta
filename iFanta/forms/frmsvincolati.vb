Public Class frmsvincolati

    Private r As String = "PORTIERI"
    Private lst As New List(Of LegaObject.Formazione.PlayerFormazione)
    Private oldrow As Integer = -1
    Private bginfo As Bitmap
    Private _start As Boolean = True
    Private _gg As Integer = 1

    Private Sub frmsvincolati_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            tlbshow.Button(1).SubItems.Clear()
            If currlega.Settings.Jolly.EnableJollyPlayerGoalkeeper Then
                tlbshow.Button(1).SubItems.Add(New iControl.ToolBarButtonSubItem("Portieri"))
            End If
            If currlega.Settings.Jolly.EnableJollyPlayerDefender Then
                tlbshow.Button(1).SubItems.Add(New iControl.ToolBarButtonSubItem("Difensori"))
            End If
            If currlega.Settings.Jolly.EnableJollyPlayerMidfielder Then
                tlbshow.Button(1).SubItems.Add(New iControl.ToolBarButtonSubItem("Centrocampisti"))
            End If
            If currlega.Settings.Jolly.EnableJollyPlayerForward Then
                tlbshow.Button(1).SubItems.Add(New iControl.ToolBarButtonSubItem("Attaccanti"))
            End If
            tlbshow.Button(1).Text = tlbshow.Button(1).SubItems(0).Text
            tlbshow.draw(True)

            txtsearch.Text = ""

            Dim p As System.Reflection.PropertyInfo = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
            p.SetValue(dtg1, True, Nothing)

            dtg1.Columns("nome").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Verifico se sono attive le funzionalita' controllo match e player'
            dtg1.Columns("dtmatch").Visible = webdata.EnableFeatureData("Match")
            dtg1.Columns("match").Visible = webdata.EnableFeatureData("Match")
            dtg1.Columns("presenza").Visible = webdata.EnableFeatureData("ProbableLineUps")

            Call SetToolTipParameater()
            Call SetTheme()
            Call loadData()

        Catch ex As Exception

        End Try
       
        _start = False

    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If _start Then Exit Sub
        Call SetTheme()
    End Sub

    Public Property Giornata() As Integer
        Get
            Return _gg
        End Get
        Set(ByVal value As Integer)
            _gg = value
        End Set
    End Property

    Sub SetTheme()

        Dim w As Integer = 0

        tlbshow.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbshow.Top = IForm1.TY + padd
        tlbshow.Left = padd + IForm1.LX

        dtg1.Top = tlbshow.Top + tlbshow.Height + padd \ 2
        dtg1.Left = tlbshow.Left
        dtg1.Height = IForm1.BY - dtg1.Top - padd
        dtg1.Width = IForm1.RX - IForm1.LX - padd * 2

        txtsearch.Top = tlbshow.Top
        txtsearch.Left = IForm1.RX - txtsearch.Width - padd

        tlbsearch.Top = tlbshow.Top
        tlbsearch.draw(True)
        tlbsearch.Left = txtsearch.Left - tlbsearch.Width

        For i As Integer = 0 To dtg1.RowCount - 1
            dtg1.Rows(i).Tag = Nothing
        Next

        dtg1.Refresh()

        For i As Integer = 0 To dtg1.Columns.Count - 1
             w = w + dtg1.Columns(i).Width
        Next

        If CDbl(_gg) = webdata.DayProbableFormation Then
            dtg1.Columns("presenza").Visible = True
        Else
            dtg1.Columns("presenza").Visible = False
            w = w - dtg1.Columns("presenza").Width
        End If

        Me.Width = dtg1.Left + w + padd + 20

    End Sub

    Sub loadData()

        dtg1.Rows.Clear()
        For i As Integer = 0 To dtg1.Columns.Count - 1
            dtg1.Columns(i).Tag = Nothing
            dtg1.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
        Next
        dtg1.RowTemplate.Height = 17

        Try
            'Carico i dati della squadra e della rosa'
            lst = SystemFunction.Convertion.ConvertTeamPlayerList(currlega.Teams(0).GetPlayer("SVINCOLATI", tlbshow.Button(1).Text.ToUpper, True, _gg))
            For i As Integer = 0 To lst.Count - 1
                lst(i).Rating = SystemFunction.General.GetRatingForma(lst(i), currlega.GiornataCorrente)
            Next
            Call DisplayData()
        Catch

        End Try
    End Sub

    Sub DisplayData()
        Try
            dtg1.Visible = False
            dtg1.RowCount = lst.Count
            For i As Integer = 0 To lst.Count - 1
                dtg1.Rows(i).Tag = i
                SystemFunction.Gui.SetPlayerFormazioneToDataGrid(dtg1.Rows(i), lst(i), _gg)
            Next
            'dtg1.Refresh()
            Call DisplayRows()
            dtg1.Visible = True
        Catch ex As Exception

        End Try
    End Sub

    Sub DisplayRows()
        For i As Integer = 0 To lst.Count - 1
            If txtsearch.Text = "" OrElse lst(i).Nome.Contains(txtsearch.Text) Then
                dtg1.Rows(i).Visible = True
            Else
                dtg1.Rows(i).Visible = False
            End If
        Next
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

    Sub HideInfo()
        oldrow = -1
        dtginfo.Visible = False
    End Sub

    Private Sub dtg1_ColumnHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dtg1.ColumnHeaderMouseClick
        Try
            Select Case dtg1.Columns(e.ColumnIndex).Name
                Case "devvt", "devpt", "imgpt", "var", "chart", "infort", "dtmatch", "presenza", "match"
                Case Else
                    Dim rev As Boolean = True
                    Dim f As String = dtg1.Columns(e.ColumnIndex).HeaderText.ToLower
                    If f = "" Then f = "idrosa"
                    If dtg1.Columns(e.ColumnIndex).Tag Is Nothing Then
                        rev = True
                    Else
                        rev = Not CBool(dtg1.Columns(e.ColumnIndex).Tag)
                    End If
                    dtg1.Columns(e.ColumnIndex).Tag = rev
                    lst = LegaObject.Formazione.Sort(lst, f, rev)
                    Call DisplayData()
            End Select
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dtg1_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dtg1.CellMouseEnter
        If e.RowIndex = -1 Then Call HideInfo() : Exit Sub
        Try
            If dtg1.Columns(e.ColumnIndex).Name = "infort" OrElse dtg1.Columns(e.ColumnIndex).Name = "presenza" Then
                If dtg1.Item(e.ColumnIndex, e.RowIndex).Tag IsNot Nothing Then

                    Dim wp As wData.wPlayer = CType(dtg1.Item(e.ColumnIndex, e.RowIndex).Tag, wData.wPlayer)
                    Dim r As Rectangle = dtg1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
                    Dim x As Integer = r.Left + dtg1.Left - dtginfo.Width - 2
                    Dim y As Integer = r.Top + dtg1.Top + r.Height \ 2

                    bginfo = SystemFunction.Gui.ShowPopUpInfoPlayer(dtginfo, wp, x, y, IForm1)

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

    Private Sub dtg1_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtg1.RowPrePaint
        Try

            Dim p As LegaObject.Formazione.PlayerFormazione = lst(CInt(dtg1.Rows(e.RowIndex).Tag))

            Call SystemFunction.Gui.DrawInfoMatchAndPresenze(dtg1, e.RowIndex, p, _gg)

        Catch ex As Exception
            WriteError(Me.Name, "RowPrePaint", ex.Message)
        End Try
    End Sub

    Private Sub dtg1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtg1.SelectionChanged
        dtg1.ClearSelection()
    End Sub

    Private Sub dtginfo_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles dtginfo.RowPostPaint
        SystemFunction.Gui.DrawInfoPresence(dtginfo, e)
    End Sub

    Private Sub dtginfo_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles dtginfo.RowPrePaint
        Dim r As Rectangle = New Rectangle(e.RowBounds.Left, e.RowBounds.Top, e.RowBounds.Width, e.RowBounds.Height + 1)
        e.Graphics.DrawImage(bginfo, r, r, GraphicsUnit.Pixel)
    End Sub

    Private Sub tlbshow_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbshow.SubButtonClick
        tlbshow.HideDropDown()
        tlbshow.Visible = False
        tlbshow.Button(ButtonIndex).Text = tlbshow.Button(ButtonIndex).SubItems(SubButtonIndex).Text
        tlbshow.draw(True)
        tlbshow.Visible = True
        Call loadData()
    End Sub

    Private Sub frm_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
        Call HideInfo()
    End Sub

    Private Sub frm_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        Call HideInfo()
    End Sub

    Private Sub txtsearch_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtsearch.KeyPress
        Timer1.Stop()
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        Call DisplayRows()
    End Sub
End Class
