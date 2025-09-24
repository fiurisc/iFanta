Public Class frmcopyrosa

    Dim _fields As New List(Of String)
    Dim _sel As New List(Of Boolean)
    Private ris As DialogResult = Windows.Forms.DialogResult.Cancel

    Public Property Fields() As List(Of String)
        Get
            Return _fields
        End Get
        Set(ByVal value As List(Of String))
            _fields = value
        End Set
    End Property

    Private Sub frmcopyrosa_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For i As Integer = 0 To _fields.Count - 1
            chk1.Add(_fields(i), True)
        Next
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0
                ris = Windows.Forms.DialogResult.OK
                _sel.Clear()
                For i As Integer = 0 To chk1.Count - 1
                    _sel.Add(chk1.Item(i).Checked)
                Next
            Case 1 : ris = Windows.Forms.DialogResult.Cancel
        End Select
        Me.Close()
    End Sub
End Class
