Public Class frmbackup

    Dim start As Boolean = True

    Private Sub frmbackup_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            AppSett.Backup.Enable = chkactive.Value
            AppSett.Backup.ExecuteBackupOnlyAfterModify = Not (opt1.Value)
            AppSett.Backup.History = CInt(cmbday.Text)
            AppSett.Backup.EnableCompression = chkcomp.Value
            AppSett.SaveSettings()
            backup.Compress = AppSett.Backup.EnableCompression
            backup.History = AppSett.Backup.History
        Catch ex As Exception

        End Try
    End Sub

    Private Sub frmbackup_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName
        lbby.Text = My.Application.Info.Copyright

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        'Setto il tema corrente'
        Call SetTheme()

        cmbday.ColumnsCount = 1
        cmbday.RowCount = 0
        For i As Integer = 1 To 15
            cmbday.Add(CStr(i))
        Next
        If AppSett.Backup.History < 1 Then AppSett.Backup.History = 1
        If AppSett.Backup.History > 15 Then AppSett.Backup.History = 15

        chkactive.Value = AppSett.Backup.Enable
        opt1.Value = Not (AppSett.Backup.ExecuteBackupOnlyAfterModify)
        opt2.Value = AppSett.Backup.ExecuteBackupOnlyAfterModify
        cmbday.SelectedItem = CStr(AppSett.Backup.History)
        cmbday.Text = CStr(AppSett.Backup.History)
        chkcomp.Value = AppSett.Backup.EnableCompression

        Call EnaDisControll()

        start = False

    End Sub

    Sub EnaDisControll()

        lbopt.Enabled = chkactive.Value
        opt1.Enabled = chkactive.Value
        opt2.Enabled = chkactive.Value
        lbday.Enabled = chkactive.Value
        cmbday.Enabled = chkactive.Value
        chkcomp.Enabled = chkactive.Value

    End Sub

    Sub SetTheme()

        Try

            Dim s As Integer = 2

            tlbaction.Left = IForm1.RX - padd - tlbaction.Width
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            ILine1.Top = tlbaction.Top - ILine1.Height - 5
            ILine1.Width = IForm1.RX - IForm1.LX - padd * 2
            ILine1.Left = IForm1.LX + padd
 
            pic1.Left = padd + IForm1.LX
            pic1.Top = padd + IForm1.TY
            lbmain.Left = pic1.Left + pic1.Width + 5
            lbmain.Top = pic1.Top

            chkactive.Top = pic1.Top + pic1.Height
            chkactive.Left = pic1.Left
            lbopt.Top = chkactive.Top + chkactive.Height + s
            lbopt.Left = chkactive.Left + padd
            opt1.Top = lbopt.Top + lbopt.Height + 2
            opt1.Left = lbopt.Left + padd
            opt2.Top = opt1.Top + opt1.Height + s
            opt2.Left = opt1.Left
            chkcomp.Top = opt2.Top + opt2.Height + s
            chkcomp.Left = lbopt.Left
            lbday.Top = chkcomp.Top + chkcomp.Height + s
            lbday.Left = chkcomp.Left
            cmbday.Top = lbday.Top
            cmbday.Left = lbday.Left + lbday.Width
            cmbday.FlatStyle = AppSett.Personal.Theme.FlatStyle

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = tlbaction.Top + tlbaction.Height \ 2 - lbby.Height \ 2

        Catch ex As Exception

        End Try

    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Try
            Select Case ButtonIndex
                Case 0
                    backup.ExecuteBackup(False)
                    ShowInfo("Avviso", "Backup completato con successo")
                Case 1
                    Me.Close()
            End Select
        Catch ex As Exception
            ShowError("Errore", "Backup fallito" & System.Environment.NewLine & ex.Message)
        End Try
    End Sub

    Private Sub chkactive_Click(sender As Object, e As EventArgs) Handles chkactive.Click
        Call EnaDisControll()
    End Sub
End Class
