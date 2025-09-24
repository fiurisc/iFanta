Public Class frmeditvoti

    Private _gg As Integer = 1
    Private _idteam As Integer = 0
    Private _team As String = ""
    Private start As Boolean = True
    Private frm As New frmformazioni
    Private f As New LegaObject.Formazione()

    Private Sub frmeditvoti_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        Call SetTheme()
        Call SetDaysList()
        Call SetTheme()

        start = False

    End Sub

    Sub SetParameater(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal FrmOrigin As frmformazioni)
        _gg = Giornata
        _idteam = IdTeam
        frm = FrmOrigin
    End Sub

    Sub SetTeamName()
        'Imposto l'elenco delle squadre'
        For i As Integer = 0 To currlega.Teams.Count - 1
            tlbsearch.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(currlega.Teams(i).Nome))
            If currlega.Teams(i).IdTeam = _idteam Then tlbsearch.Button(0).SubItems(i).State = True
        Next
        txtsearch.Text = currlega.Teams(_idteam).Nome
    End Sub

    Sub SetDaysList()
        'Imposto l'elenco delle squadre'
        For i As Integer = 0 To currlega.Settings.NumberOfDays - 1
            tlbgg.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(CStr(i + 1)))
            If i + 1 = _gg Then tlbgg.Button(0).SubItems(i).State = True
        Next
        txtgg.Text = CStr(_gg)
    End Sub

    Sub SetTheme()

        picsearch.Left = IForm1.LX + padd
        picsearch.Top = padd \ 2 + IForm1.TY + 7

        txtsearch.Left = picsearch.Left + picsearch.Width + 3
        txtsearch.Top = picsearch.Top - 1

        tlbsearch.Left = txtsearch.Left + txtsearch.Width - 1
        tlbsearch.Top = txtsearch.Top
        tlbsearch.Height = txtsearch.Height
        tlbsearch.BorderColor = txtsearch.BorderColor
        tlbsearch.BorderColorDropDown = tlbsearch.BorderColor
        tlbsearch.Button(0).SubWidth = txtsearch.Width + tlbsearch.Button(0).Width
        tlbsearch.Button(0).SubItemsAutoSize = False
        tlbsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle

        picgg.Left = tlbsearch.Left + tlbsearch.Width + 5
        picgg.Top = tlbsearch.Top + 1

        txtgg.Left = picgg.Left + picgg.Width + 3
        txtgg.Top = tlbsearch.Top

        tlbgg.Left = txtgg.Left + txtgg.Width - 1
        tlbgg.Top = txtsearch.Top
        tlbgg.Height = txtgg.Height
        tlbgg.BorderColor = txtgg.BorderColor
        tlbgg.BorderColorDropDown = tlbgg.BorderColor
        tlbgg.Button(0).SubWidth = txtgg.Width + tlbgg.Button(0).Width
        tlbgg.Button(0).SubItemsAutoSize = False
        tlbgg.FlatStyle = AppSett.Personal.Theme.FlatStyle

        tlbaction.Left = IForm1.RX - tlbaction.Width - padd
        tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
        tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle
        For i As Integer = 0 To tlbaction.Button.Count - 1
            tlbaction.Button(i).BorderColor = Color.DimGray
        Next

    End Sub

    Sub LoadFormazione()

    End Sub

    Sub Save()

    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Try
            Select Case ButtonIndex
                Case 0
                    LegaObject.Formazione.Delete(_gg, _idteam, False)
                    LegaObject.Formazione.Delete(_gg, _idteam, True)
                    frm.LoadData()
                    Call loadFormazione()
                Case 1
                    Call Save()
                Case 2 : Me.Close()
            End Select
        Catch ex As Exception

        End Try
    End Sub

    Private Sub tlbsearch_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbsearch.SubButtonClick
        txtsearch.Text = tlbsearch.Button(0).SubItems(SubButtonIndex).Text
        _idteam = SubButtonIndex
        Call loadFormazione()
    End Sub

    Private Sub tlbgg_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbgg.SubButtonClick
        txtgg.Text = CStr(SubButtonIndex + 1)
        _gg = SubButtonIndex + 1
        Call loadFormazione()
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

End Class
