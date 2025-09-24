Public Class frmrestore

    Dim start As Boolean = True

    Private Sub frmrestore_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        IForm1.WindowsTitle = My.Application.Info.ProductName
        lbby.Text = My.Application.Info.Copyright

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        Dim d As List(Of String) = backup.GetBackupList
        lst1.RowCount = d.Count
        For i As Integer = 0 To d.Count - 1
            If d(i).EndsWith(".zip") Then
                lst1.Item(0, i).Value = My.Resources._110
            Else
                lst1.Item(0, i).Value = My.Resources._103
            End If
            lst1.Item(1, i).Value = d(i)
        Next
        If d.Count > 0 Then
            tlbaction.Button(0).Enabled = True
        Else
            tlbaction.Button(0).Enabled = False
        End If

        'Setto il tema corrente'
        Call SetTheme()

        start = False

    End Sub

    Sub SetTheme()

        Try

            tlbaction.Left = IForm1.RX - padd - tlbaction.Width
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            ILine1.Top = tlbaction.Top - ILine1.Height - 5
            ILine1.Width = IForm1.RX - IForm1.LX - padd * 2
            ILine1.Left = IForm1.LX + padd

            pic1.Left = padd
            pic1.Top = padd + IForm1.TY
            lbmain.Left = pic1.Left + pic1.Width + 5
            lbmain.Top = pic1.Top

            lbinfo.Left = pic1.Left
            lbinfo.Top = ILine1.Top - lbinfo.Height - 2

            lbopt.Top = pic1.Top + pic1.Height
            lbopt.Left = pic1.Left
            lst1.Top = lbopt.Top + lbopt.Height
            lst1.Left = lbopt.Left
            lst1.Width = IForm1.RX - IForm1.LX - padd * 2
            lst1.Height = lbinfo.Top - lst1.Top - 3
            lst1.FlatStyle = AppSett.Personal.Theme.FlatStyle

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = tlbaction.Top + tlbaction.Height \ 2 - lbby.Height \ 2

        Catch ex As Exception

        End Try

    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Try
            Select Case ButtonIndex
                Case 0
                    If iControl.iMsgBox.ShowMessage("Sei sicuro di voler avviare il ripristo dati?" & System.Environment.NewLine & "Agendo in questo modo, andranno persi tutti i dati correnti", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.OK Then
                        conn.Close()
                        conn.Dispose()
                        conn = Nothing
                        backup.Restore(CStr(lst1.SelectedRows(0).Cells(1).Value))
                        SystemFunction.DataBase.OpenConnection()
                        ShowInfo("Avviso", "Ripristino dati completato con successo")
                    End If
                Case 1
                    Me.Close()
            End Select
        Catch ex As Exception
            ShowError("Errore", "Ripristino dati fallito" & System.Environment.NewLine & ex.Message)
        End Try
    End Sub

End Class
