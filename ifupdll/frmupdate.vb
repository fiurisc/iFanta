Friend Class frmupdate

    Dim u As New UpdateList
    Dim ris As Windows.Forms.DialogResult = Windows.Forms.DialogResult.Cancel
    Dim padd2 As Integer = 20

    Private Sub frmupdate_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Me.DialogResult = ris
    End Sub

    Private Sub frmupdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.Theme = u.Theme
        IForm1.FlatStyle = u.FlatStyle

        IButton1.FlatStyle = u.FlatStyle
        IButton2.FlatStyle = u.FlatStyle
        IButton1.Left = Me.Width \ 2 - IButton1.Width - 3
        IButton2.Left = Me.Width \ 2 + 3

        If u.Language = UpdateList.eLanguage.Italian Then
            ILabel1.Text = "C'e' una nuova versione! Vuoi aggiornare ora?"
            IButton1.Text = "Si"
            IForm1.WindowsTitle = "Aggiornamenti"
            Me.Text = "Aggiornamenti"
        End If
        ILabel2.Top = ILabel1.Top + ILabel1.Height - 5
    
    End Sub

    Sub SetItems(ByVal up As UpdateList)

        u = up

        dtg1.Location = ILabel2.Location
        dtg1.Width = Me.Width - dtg1.Left * 2
        dtg1.Width = Me.Width - dtg1.Left - 30

        dtg1.RowCount = u.UpdateFileList.Count
        dtg1.ColumnCount = 3
        dtg1.AllowUserToResizeColumns = False
        dtg1.AllowUserToResizeRows = False

        dtg1.Columns(1).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleRight
        dtg1.Columns(1).DefaultCellStyle.ForeColor = Drawing.Color.Gray

        dtg1.ScrollBars = Windows.Forms.ScrollBars.Vertical

        For i As Integer = 0 To u.UpdateFileList.Count - 1
            dtg1.Item(0, i).Value = u.UpdateFileList.Item(i).Name
            dtg1.Item(1, i).Value = u.UpdateFileList.Item(i).Size \ 1024 & " kb"
        Next

        If u.Language = UpdateList.eLanguage.English Then
            ILabel2.Text = "Show detail (" & u.UpdateFileList.Count & " items ~ " & u.Size \ 1024 & " Kb)"
        Else
            ILabel2.Text = "Visualizza dettagli (" & u.UpdateFileList.Count & " elementi ~ " & u.Size \ 1024 & " Kb)"
        End If
        
    End Sub

    Private Sub ILabel2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ILabel2.Click

        If ILabel2.Text.Contains("Show detail") OrElse ILabel2.Text.Contains("Visualizza dettagli") Then

            Dim h As Integer = u.UpdateFileList.Count * dtg1.RowTemplate.Height + 4
            If h > 130 Then
                dtg1.Columns(0).Width = 180
                dtg1.Columns(1).Width = dtg1.Width - dtg1.Columns(0).Width - 20
                h = 130
            Else
                dtg1.Columns(0).Width = 180
                dtg1.Columns(1).Width = dtg1.Width - dtg1.Columns(0).Width
            End If
            dtg1.Columns(2).Width = 64
            dtg1.Height = h

            Me.Height = 134 + h
            dtg1.Visible = True
            If u.Language = UpdateList.eLanguage.English Then
                ILabel2.Text = "Hide detail (" & u.UpdateFileList.Count & " items ~ " & u.Size \ 1024 & " Kb)"
            Else
                ILabel2.Text = "Nascondi dettagli (" & u.UpdateFileList.Count & " elementi ~ " & u.Size \ 1024 & " Kb)"
            End If
            ILabel2.Image = My.Resources.go_top
            dtg1.Update()
        Else
            Me.Height = 134
            dtg1.Visible = False
            If u.Language = UpdateList.eLanguage.English Then
                ILabel2.Text = "Show detail (" & u.UpdateFileList.Count & " items ~ " & u.Size \ 1024 & " Kb)"
            Else
                ILabel2.Text = "Visualizza dettagli (" & u.UpdateFileList.Count & " elementi ~ " & u.Size \ 1024 & " Kb)"
            End If
            ILabel2.Image = My.Resources.bottom
            
        End If
    End Sub

    Private Sub IButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles IButton1.Click
        ris = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub IButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles IButton2.Click
        ris = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange

    End Sub

    Private Sub frmupdate_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class