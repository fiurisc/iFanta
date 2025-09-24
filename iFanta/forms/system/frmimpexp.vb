Imports iFanta.SystemFunction.FileAndDirectory

Public Class frmimpexp

    Enum TypeOfOperation As Integer
        Import = 0
        Export = 1
    End Enum

    Private start As Boolean = True
    Private _type As TypeOfOperation = TypeOfOperation.Export
    Private ris As DialogResult = Windows.Forms.DialogResult.Cancel

    Private Sub frmimpexp_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Me.DialogResult = ris
    End Sub

    Private Sub fromimpexp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)
        chkdefault.Value = AppSett.Session.UseDefaultDirectoryForImportAndExport
        If AppSett.Session.LastImportAndExportDirectory <> "" Then
            txt1.Text = AppSett.Session.LastImportAndExportDirectory
        Else
            txt1.Text = My.Computer.FileSystem.SpecialDirectories.Desktop
        End If
        If _type = TypeOfOperation.Export Then
            IForm1.WindowsTitle = "Esporta"
            chkdefault.Text = "Esporta in cartella di sistema (" & GetLegaExpDataDirectory.Replace(My.Application.Info.DirectoryPath, "") & ")"
            tlbaction.Button(0).Image = My.Resources.exportb16
            tlbaction.Button(0).Text = "Esporta"
        Else
            IForm1.WindowsTitle = "Importa"
            chkdefault.Text = "Importa da cartella di sistema (" & GetLegaExpDataDirectory.Replace(My.Application.Info.DirectoryPath, "") & ")"
            tlbaction.Button(0).Image = My.Resources.importb16
            tlbaction.Button(0).Text = "Importa"
        End If
        txt1.Enabled = Not (chkdefault.Value)

        Call SetTheme()

        start = False

    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Sub SetParameater(ByVal Type As TypeOfOperation)
        _type = Type
    End Sub

    Function GetDirectory() As String
        If chkdefault.Value Then
            Return GetLegaExpDataDirectory()
        Else
            Return txt1.Text
        End If
    End Function

    Sub SetTheme()

        Try

            chkdefault.Left = IForm1.LX + padd
            chkdefault.Top = IForm1.TY + padd \ 2
            txt1.Left = chkdefault.Left
            txt1.Top = chkdefault.Top + chkdefault.Height + 3
            txt1.Width = IForm1.RX - IForm1.LX - padd * 2
            txt1.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbaction.draw(True)
            tlbaction.Left = IForm1.RX - padd - tlbaction.Width
            tlbaction.Top = txt1.Top + txt1.Height + 8

            Me.Height = tlbaction.Top + tlbaction.Height + padd \ 2 + IForm1.BH7

        Catch ex As Exception

        End Try

    End Sub

    Private Sub chkdefault_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkdefault.Click
        txt1.Enabled = Not (chkdefault.Value)
    End Sub

    Private Sub txt1_Command1Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt1.Command1Click
        Dim dlg As New Windows.Forms.FolderBrowserDialog
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            txt1.Text = dlg.SelectedPath
        End If
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Call Action(ButtonIndex)
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            If Me.OwnedForms.Length = 0 Then
                If keyData = Keys.Enter Then
                    Call Action(0)
                ElseIf keyData = Keys.Cancel OrElse keyData = Keys.Escape Then
                    Call Action(1)
                End If
            End If
        Catch ex As Exception

        End Try
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Sub Action(Act As Integer)
        Select Case Act
            Case 0 : ris = Windows.Forms.DialogResult.OK
            Case 1 : ris = Windows.Forms.DialogResult.Cancel
        End Select
        AppSett.Session.UseDefaultDirectoryForImportAndExport = chkdefault.Value
        AppSett.Session.LastImportAndExportDirectory = txt1.Text
        Me.Close()
    End Sub
End Class
