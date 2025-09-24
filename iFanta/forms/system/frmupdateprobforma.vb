Public Class frmupdateprobforma

    Dim start As Boolean = True
    Dim c As New List(Of iControl.iCheckBox)
    Dim intconn As New InternetConnection.ConnType
    Dim d As New List(Of String)
    Dim thr As Threading.Thread
    Dim startth As Boolean = False

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName
        lbby.Text = My.Application.Info.Copyright

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        chkopt.Width = Me.Width - chkopt.Left - 20 - IForm1.RW

        For i As Integer = 0 To webdata.WebSiteList.Count - 1
            ''Carico la rosa se necessario'
            'currlega.Teams(i).Load(False, True)
            'Aggiungo la picturebox'
            c.Add(New iControl.iCheckBox)
            c(i).Font = New Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel)
            c(i).Width = chkopt.Width - 20
            c(i).Height = chkopt.Height
            c(i).Location = New Point(chkopt.Left + 20, chkopt.Top + (i + 1) * chkopt.Height)
            c(i).Text = webdata.WebSiteList(i)
            If webdata.WebSiteList(i).Contains("Probabili") Then
                c(i).Value = True
            End If
            c(i).Tag = i
            c(i).Visible = True
            AddHandler c(i).Click, AddressOf chk_Click
            Me.Controls.Add(c(i))
        Next

        lnsel.Location = New Point(chkopt.Left + 20, c(c.Count - 1).Top + c(c.Count - 1).Height)
        lnsel.Width = chkopt.Width - 20
        lbsel.Location = New Point(chkopt.Left + 20, lnsel.Top + lnsel.Height)
        lbsel.Width = chkopt.Width - 20

        'Setto il tema corrente'
        Call SetTheme()
        'Setto l'altezza della finestra'
        Call SetWindowsHeight()

        start = False

    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Sub SetTheme()

        Try

            tlbaction.Left = IForm1.RX - padd - tlbaction.Width
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            ILine1.Top = tlbaction.Top - ILine1.Height - 5
            ILine1.Width = IForm1.RX - IForm1.LX - padd * 2
            ILine1.Left = IForm1.LX + padd
            prb1.Top = ILine1.Top - prb1.Height - 5
            prb1.Width = ILine1.Width
            prb1.Left = ILine1.Left
            prb1.FlatStyle = AppSett.Personal.Theme.FlatStyle
            lbstatus.Left = ILine1.Left
            lbstatus.Top = prb1.Top - lbstatus.Height - 5

            pic1.Left = padd
            pic1.Top = padd + IForm1.TY
            lbmain.Left = pic1.Left + pic1.Width + 5
            lbmain.Top = pic1.Top

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

        Catch ex As Exception

        End Try

    End Sub

    Sub SetWindowsHeight()

        For i As Integer = 0 To c.Count - 1
            c(i).Visible = chkopt.Value
        Next
        lnsel.Visible = chkopt.Value
        lbsel.Visible = chkopt.Value

        If chkopt.Value Then
            Me.Height = lbsel.Top + 120
        Else
            Me.Height = chkopt.Top + 120
        End If

    End Sub

    Sub RunUpdate()


        intconn = InternetConnection.Type

        d.Clear()

        If intconn <> InternetConnection.ConnType.offline Then

            For i As Integer = 0 To c.Count - 1
                If c(i).Value OrElse chkopt.Value = False Then
                    d.Add(c(i).Text)
                End If
            Next

            Dim giornata As Integer = -1
            Dim cwebplayer As Boolean = False
            Dim cont As Integer = 0

            startth = True

            Try
                prb1.Max = d.Count
                Dim t As New Threading.ThreadStart(AddressOf UpdateData)
                thr = New Threading.Thread(t)
                Call EnableControls(False)
                thr.Start()
                lbstatus.Text = "Aggiornamento in corso..."
                Timer1.Enabled = True
            Catch ex As Exception
                prb1.Value = 0
                lbstatus.Text = "Aggiornamento fallito"
            End Try
        Else
            ShowAlert("Avviso", "Connessione internet assente o non buona")
            prb1.Value = 0
            lbstatus.Text = "Aggiornamento fallito"
        End If

    End Sub

    Private Sub EnableControls(ByVal ena As Boolean)
        If ena Then
            tlbaction.Button(0).Image = My.Resources.reload16
            tlbaction.Button(0).Text = "Aggiorna"
            lbstatus.Text = "Ready"
        Else
            tlbaction.Button(0).Image = My.Resources.closeb16
            tlbaction.Button(0).Text = "Annulla"
            lbstatus.Text = "Aggiornamento in corso..."
        End If
        tlbaction.draw(True)
        tlbaction.Left = IForm1.RX - padd - tlbaction.Width
        chkopt.Enabled = ena
        For i As Integer = 0 To c.Count - 1
            c(i).Enabled = ena
        Next
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0
                If startth Then
                    Call StopThread()
                    Call EnableControls(True)
                Else
                    Call RunUpdate()
                End If
            Case 1 : Me.Close()
        End Select
    End Sub

    Private Sub chkopt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkopt.Click
        Call SetWindowsHeight()
    End Sub

    Private Sub UpdateData()
        webdata.StopThread()
        webdata.UpdateWebData(currlega.Settings.Active, False, d, intconn)
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            If thr Is Nothing Then
                If startth = True Then
                    Call EnableControls(True)
                End If
            Else
                If startth = True Then
                    If thr.ThreadState <> Threading.ThreadState.Stopped Then
                        prb1.Value = webdata.Progress
                        Call EnableControls(False)
                    Else
                        Call EnableControls(True)
                        prb1.Value = 0
                        startth = False
                    End If
                End If
            End If
        Catch ex As Exception
            WriteError(Me.Name, "Timer1_Tick", ex.Message)
        End Try
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call StopThread()
    End Sub

    Private Sub StopThread()
        Try
            If thr IsNot Nothing AndAlso thr.ThreadState <> Threading.ThreadState.Stopped Then
                thr.Abort()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub chkselall_Click(sender As Object, e As EventArgs) Handles lbsel.Click

        Dim sel As Boolean = False

        If lbsel.Text = "(Seleziona tutto)" Then
            sel = True
            lbsel.Text = "(Deseleziona tutto)"
        Else
            lbsel.Text = "(Seleziona tutto)"
        End If

        For i As Integer = 0 To c.Count - 1
            c(i).Value = sel
        Next

    End Sub

    Private Sub chk_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim all As Boolean = True

        For i As Integer = 0 To c.Count - 1
            If c(i).Value <> True Then
                all = False
                Exit For
            End If
        Next

        If all Then
            lbsel.Text = "(Deseleziona tutto)"
        Else
            lbsel.Text = "(Seleziona tutto)"
        End If

    End Sub
End Class
