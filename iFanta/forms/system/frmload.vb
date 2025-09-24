Imports iFanta.SystemFunction.FileAndDirectory

Public Class frmload

    Dim dir As String = ""
    Dim dt As New Date
    Dim d As New List(Of String)
    Dim ind As Integer = 0
    Dim l As Boolean = False

    Public ReadOnly Property Directory() As String
        Get
            Return dir
        End Get
    End Property

    Sub SetTheme()

        Try

            pictorneo.Top = IForm1.TY + padd \ 2
            pictorneo.Left = padd \ 2
            lbtorneo.Left = pictorneo.Left + pictorneo.Width
            dtg1.Left = lbtorneo.Left
            dtg1.Top = lbtorneo.Top + lbtorneo.Height
            dtg1.Width = IForm1.RX - dtg1.Left - padd
            dtg1.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            Dim h As Integer = dtg1.RowCount * dtg1.RowTemplate.Height + 4
            If h > 130 Then
                'dtg1.Columns(1).Width = dtg1.Width - 20
                h = 130
            Else
                'dtg1.Columns(1).Width = dtg1.Width
            End If
            dtg1.Height = h
            tlbaction.Top = dtg1.Top + dtg1.Height + 8

            Me.Height = tlbaction.Top + tlbaction.Height + padd \ 2 + IForm1.BH7

        Catch ex As Exception

        End Try

    End Sub

    Private Sub frmload_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.KeyPreview = True

        IForm1.WindowsTitle = My.Application.Info.ProductName
        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        dtg1.AllowUserToResizeColumns = False
        dtg1.AllowUserToResizeRows = False
        dtg1.ScrollBars = Windows.Forms.ScrollBars.Vertical

        d.AddRange(IO.Directory.GetDirectories(GetLegheDirectory))
        dtg1.RowCount = d.Count
        If d.Count > 0 Then
            For i As Integer = 0 To d.Count - 1
                dtg1.Item(1, i).Value = IO.Path.GetFileName(d(i))
                dtg1.Item(0, i).Tag = d(i)
                If IO.Path.GetFileName(d(i)) = AppSett.Session.LastLegaSelected Then
                    ind = i
                End If
            Next
        End If
        Call SetTheme()
        dtg1.Rows(ind).Selected = True
        dtg1.FirstDisplayedScrollingRowIndex = ind

    End Sub

    Private Sub dtg1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dtg1.KeyDown
        e.SuppressKeyPress = True
    End Sub

    Private Sub frm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        'Try
        '    Select Case e.KeyData
        '        Case Keys.Down
        '            If dt.Subtract(Date.Now).TotalMilliseconds < -200 Then
        '                dt = Date.Now
        '                SelectLega(+1)
        '                'Return True
        '            End If
        '        Case Keys.Up
        '            If dt.Subtract(Date.Now).TotalMilliseconds < -200 Then
        '                dt = Date.Now
        '                SelectLega(-1)
        '            End If
        '        Case Keys.Enter
        '            If l = False Then
        '                l = True
        '                AppSett.LastLegaSelected = CStr(dtg1.Item(1, ind).Value)
        '                dir = d(ind)
        '                RaiseEvent SelectLege(Me, New System.EventArgs, dir)
        '                Me.DialogResult = Windows.Forms.DialogResult.OK
        '            End If
        '        Case Keys.Escape, Keys.Cancel
        '            Me.DialogResult = Windows.Forms.DialogResult.Cancel
        '            'Return True
        '    End Select
        'Catch ex As Exception

        'End Try
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            If Me.OwnedForms.Length = 0 Then

                Select Case keyData
                    Case Keys.Down
                        If dt.Subtract(Date.Now).TotalMilliseconds < -200 Then
                            dt = Date.Now
                            SetLega(+1)
                            'Return True
                        End If
                    Case Keys.Up
                        If dt.Subtract(Date.Now).TotalMilliseconds < -200 Then
                            dt = Date.Now
                            SetLega(-1)
                        End If
                    Case Keys.Enter
                        If l = False Then
                            l = True
                            AppSett.Session.LastLegaSelected = CStr(dtg1.Item(1, ind).Value)
                            dir = d(ind)
                            Me.DialogResult = Windows.Forms.DialogResult.OK
                        End If
                    Case Keys.Escape, Keys.Cancel
                        Me.DialogResult = Windows.Forms.DialogResult.Cancel
                        'Return True
                End Select
            End If
        Catch ex As Exception

        End Try
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    'Public Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean Implements IMessageFilter.PreFilterMessage
    '    Try
    '        Dim keyCode As Keys = CType(m.WParam.ToInt32(), Keys) And Keys.KeyCode
    '        Select Case keyCode
    '            Case Keys.Down
    '                If dt.Subtract(Date.Now).TotalMilliseconds < -200 Then
    '                    dt = Date.Now
    '                    SelectLega(+1)
    '                    'Return True
    '                End If
    '            Case Keys.Up
    '                If dt.Subtract(Date.Now).TotalMilliseconds < -200 Then
    '                    dt = Date.Now
    '                    SelectLega(-1)
    '                    m.Msg = -1
    '                    m.Result = CType(-1, IntPtr)
    '                End If
    '            Case Keys.Enter
    '                If l = False Then
    '                    l = True
    '                    AppSett.LastLegaSelected = CStr(dtg1.Item(1, ind).Value)
    '                    dir = d(ind)
    '                    RaiseEvent SelectLege(Me, New System.EventArgs, dir)
    '                    Me.Close()
    '                End If
    '                'Return True
    '        End Select
    '    Catch ex As Exception

    '    End Try
    'End Function

    Sub SetLega(ByVal dir As Integer)
        If dtg1.RowCount > 1 Then
            ind = ind + dir
            If ind < 0 Then ind = 0
            If ind > dtg1.RowCount - 1 Then ind = dtg1.RowCount - 1
            dtg1.Rows(ind).Selected = True
            dtg1.FirstDisplayedScrollingRowIndex = ind - 2
        End If
    End Sub

    Private Sub dtg1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles dtg1.MouseClick
        ind = dtg1.SelectedRows(0).Index
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0
                AppSett.Session.LastLegaSelected = CStr(dtg1.Item(1, ind).Value)
                dir = d(ind)
                Me.DialogResult = Windows.Forms.DialogResult.OK
            Case Else
                Me.DialogResult = Windows.Forms.DialogResult.Cancel
        End Select
    End Sub
End Class
