Public Class frmsettings

    Dim start As Boolean = True

    Private Sub fromsettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName & " - Impostazioni"

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        'Setto il tema corrente'
        Call SetTheme()

        chksendtome.Value = AppSett.Personal.SendMailAlsoToMe
        txtmail.Text = AppSett.Personal.Mail
        chkupena.Enabled = AppSett.Update.EnableUpdate
        chkupforcestartup.Value = AppSett.Update.ForceCheckUpdateStartup
        chkupwebena.Value = webdata.EnableUpdate
        chkupwebforcestartup.Value = webdata.ForceUpdateStartup

        chkbckena.Value = AppSett.Backup.Enable
        optbck1.Value = AppSett.Backup.ExecuteBackupOnlyAfterModify
        optbck2.Value = Not AppSett.Backup.ExecuteBackupOnlyAfterModify
        chkbckenacomp.Value = AppSett.Backup.EnableCompression

        cmbbckhis.ColumnsCount = 1
        For i As Integer = 1 To 30
            cmbbckhis.Add(CStr(i))
        Next
        cmbbckhis.Text = CStr(AppSett.Backup.History)
        tlbaction.Button(0).Enabled = False
        tlbaction.draw(True)

        Call EnaDisControl()

        start = False

    End Sub

    Sub SetTheme()

        Try

            Me.Size = New Size(420, 460)

            Dim s1 As Integer = 1
            Dim s2 As Integer = 6
            Dim s3 As Integer = 12

            tlbaction.Left = IForm1.RX - padd - tlbaction.Width
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            ILine1.Top = tlbaction.Top - ILine1.Height - 5
            ILine1.Width = IForm1.RX - IForm1.LX - padd * 2
            ILine1.Left = IForm1.LX + padd

            pnl1.Left = IForm1.LX
            pnl1.Width = IForm1.RX - IForm1.LX

            pnl1.Top = IForm1.TY
            pnl1.Height = ILine1.Top - pnl1.Top

            lnmail.Left = padd
            lnmail.Top = padd + 5
            lnmail.Width = CInt(pnl1.Width - padd * 2)
            picmail.Left = lnmail.Left
            picmail.Top = lnmail.Top + lnmail.Height + s1
            chksendtome.Top = picmail.Top
            chksendtome.Left = picmail.Left + picmail.Width + s3
            chksendtome.Width = lnmail.Width - chksendtome.Left + lnmail.Left
            txtmail.Top = chksendtome.Top + chksendtome.Height + s1
            txtmail.Left = chksendtome.Left
            txtmail.Width = chksendtome.Width

            lnup.Left = lnmail.Left
            lnup.Top = txtmail.Top + txtmail.Height + s3
            lnup.Width = lnmail.Width
            picup.Left = lnup.Left
            picup.Top = lnup.Top + lnup.Height + s1
            chkupena.Top = picup.Top
            chkupena.Left = picup.Left + picup.Width + s3
            chkupena.Width = lnup.Width - chkupena.Left + lnup.Left
            chkupforcestartup.Top = chkupena.Top + chkupena.Height + s1
            chkupforcestartup.Left = chkupena.Left + s3
            chkupforcestartup.Width = chkupena.Width - s3
            chkupwebena.Top = chkupforcestartup.Top + chkupforcestartup.Height + s1
            chkupwebena.Left = chkupena.Left
            chkupwebforcestartup.Top = chkupwebena.Top + chkupwebena.Height + s1
            chkupwebforcestartup.Left = chkupforcestartup.Left

            lnbck.Left = lnmail.Left
            lnbck.Top = chkupwebforcestartup.Top + chkupwebforcestartup.Height + s3
            lnbck.Width = lnmail.Width
            picbck.Left = lnbck.Left
            picbck.Top = lnbck.Top + lnbck.Height + s1
            chkbckena.Top = picbck.Top
            chkbckena.Left = picbck.Left + picbck.Width + s3
            lbbckopt.Top = chkbckena.Top + chkbckena.Height + s1
            lbbckopt.Left = chkbckena.Left
            lbbckopt.Width = chkbckena.Width
            optbck1.Top = lbbckopt.Top + lbbckopt.Height + s1
            optbck1.Left = lbbckopt.Left + padd
            optbck2.Top = optbck1.Top + optbck1.Height + s1
            optbck2.Left = optbck1.Left

            chkbckenacomp.Left = lbbckopt.Left
            chkbckenacomp.Top = optbck2.Top + optbck2.Height + s1
            lbbckhis.Left = lbbckopt.Left
            lbbckhis.Top = chkbckenacomp.Top + chkbckenacomp.Height + s1
            cmbbckhis.Top = lbbckhis.Top
            cmbbckhis.Left = lnbck.Left + lnbck.Width - cmbbckhis.Width

        Catch ex As Exception

        End Try

    End Sub

    Sub EnaDisControl()
        txtmail.Enabled = chksendtome.Value
        chkupforcestartup.Enabled = chkupena.Value
        chkupwebforcestartup.Enabled = chkupwebena.Value
        lbbckopt.Enabled = chkbckena.Value
        optbck1.Enabled = chkbckena.Value
        optbck2.Enabled = chkbckena.Value
        chkbckenacomp.Enabled = chkbckena.Value
        lbbckhis.Enabled = chkbckena.Value
        cmbbckhis.Enabled = chkbckena.Value
    End Sub

    Private Sub chkupena_Click(sender As Object, e As EventArgs) Handles chksendtome.Click, chkupena.Click, chkupwebena.Click, chkbckena.Click
        Call EnaDisControl()
        Call EnaSave()
    End Sub

    Private Sub tlbaction_ButtonClick(sender As Object, e As EventArgs, ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0
                AppSett.Personal.SendMailAlsoToMe = chksendtome.Value
                AppSett.Personal.Mail = txtmail.Text
                AppSett.Update.EnableUpdate = chkupena.Value
                AppSett.Update.ForceCheckUpdateStartup = chkupforcestartup.Value
                webdata.EnableUpdate = chkupwebena.Value
                webdata.ForceUpdateStartup = chkupwebforcestartup.Value
                AppSett.Backup.Enable = chkbckena.Value
                AppSett.Backup.ExecuteBackupOnlyAfterModify = optbck1.Value
                AppSett.Backup.EnableCompression = chkbckenacomp.Value
                AppSett.Backup.History = CInt(cmbbckhis.Text)
                backup.Compress = AppSett.Backup.EnableCompression
                backup.History = AppSett.Backup.History
                AppSett.SaveSettings()
        End Select
        Me.Close()
    End Sub

    Sub EnaSave()
        If start Then Exit Sub
        If tlbaction.Button(0).Enabled = False Then
            tlbaction.Button(0).Enabled = True
            tlbaction.draw(True)
        End If
    End Sub

    Private Sub txtmail_TextChange(sender As Object, e As EventArgs) Handles txtmail.TextChange
        Call EnaSave()
    End Sub

    Private Sub cmbbckhis_SelectItems(sender As Object, e As EventArgs) Handles cmbbckhis.SelectItems
        Call EnaSave()
    End Sub

    Private Sub optbck_Click(sender As Object, e As EventArgs) Handles optbck1.Click, optbck2.Click
        Call EnaSave()
    End Sub
End Class
