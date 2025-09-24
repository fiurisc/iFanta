Public Class frmmail

    Dim att As New List(Of String)

    Public Property Attachements As List(Of String)
        Get
            Return att
        End Get
        Set(value As List(Of String))
            att = value
        End Set
    End Property

    Private Sub tlbaction_ButtonClick(sender As Object, e As EventArgs, ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0
                SystemFunction.Mail.SendMail(txtto.Text, "", "", txtsubject.Text, txtbody.Text, att)
            Case 1
                Me.Close()
        End Select
    End Sub

    Private Sub txtbody_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtbody.KeyPress
        Call EnableSend()
    End Sub

    Sub EnableSend()
        If txtto.Text <> "" Then
            tlbaction.Button(0).Enabled = True
        Else
            tlbaction.Button(0).Enabled = False
        End If
        tlbaction.draw(True)
    End Sub

    Private Sub txtto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtto.KeyPress
        Call EnableSend()
    End Sub
End Class