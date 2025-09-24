Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.IO

Public Class frmformazioni

    Implements IMessageFilter

    Private Delegate Function CopyProgressRoutine(ByVal totalFileSize As Int64, ByVal totalBytesTransferred As Int64, ByVal streamSize As Int64, ByVal streamBytesTransferred As Int64, ByVal dwStreamNumber As Int32, ByVal dwCallbackReason As Int32, ByVal hSourceFile As Int32, ByVal hDestinationFile As Int32, ByVal lpData As Int32) As Int32
    Private Declare Auto Function CopyFileEx Lib "kernel32.dll" (ByVal lpExistingFileName As String, ByVal lpNewFileName As String, ByVal lpProgressRoutine As CopyProgressRoutine, ByVal lpData As Int32, ByVal lpBool As Int32, ByVal dwCopyFlags As Int32) As Int32

    Dim p As New List(Of PictureBox)
    Dim l As New List(Of iControl.iLabel)
    Dim t As New List(Of iControl.iToolBar)
    Dim start As Boolean = True
    Dim gio As Integer = currlega.LastDaySelectFormation
    Dim topforma As Boolean = False
    Dim currforma As Integer = -1
    Dim fcopy As New List(Of LegaObject.Formazione)

    Overrides Sub ResetTorneo()
        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)
        Call LoadData()
    End Sub

    Private Sub frmformazioni_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            Application.RemoveMessageFilter(Me)
            currlega.LastDaySelectFormation = gio
            currlega.SaveSettings()
            AppSett.SaveSettings()
            currlega.SaveSettings()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub frmformazioni_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Application.AddMessageFilter(Me)

        IForm1.WindowsTitle = My.Application.Info.ProductName

        lbby.Text = My.Application.Info.Copyright
        txtsearch.Text = ""

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        If My.Computer.Name.ToLower <> "e0220634" Then
            mnuautoforma.Visible = False
        End If
        If currlega.Settings.MailAdmin <> "" Then
            mnusfomramailtoadmin.Enabled = True
        Else
            mnusfomramailtoadmin.Enabled = False
        End If

        mnudeltorneoforma.Visible = currlega.Settings.Admin
        mnusepdatiformatorneo.Visible = currlega.Settings.Admin
        mnuimpdatiformatorneo.Visible = currlega.Settings.Admin
        mnuexpdatiformatorneo.Visible = currlega.Settings.Admin

        'Setto il tema corrente'
        Call SetTheme()
        'Setto l'elenco delle giornate'
        Call SetDaysList()

        start = False

        Me.Width = My.Computer.Screen.WorkingArea.Width - 250
        Me.Height = My.Computer.Screen.WorkingArea.Height - 150
        Me.Top = My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2
        Me.Left = My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2
        mnu1.ImageScalingSize = New Size(16, 16)

        Timer1.Enabled = True

    End Sub

    Sub SetTheme()

        Try

            SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)

            tlbsearch.SuspundeLayout = True
            tlbsearch.Left = IForm1.RX - padd - 1 - tlbsearch.Width
            tlbsearch.Top = padd \ 2 + IForm1.TY + 6
            'tlbsearch.Height = txtsearch.Height
            tlbsearch.BorderColor = txtsearch.BorderColor
            tlbsearch.BorderColorDropDown = tlbsearch.BorderColor
            tlbsearch.Button(0).SubWidth = txtsearch.Width + tlbsearch.Button(0).Width - 1
            tlbsearch.Button(0).SubItemsAutoSize = False
            tlbsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbsearch.SuspundeLayout = False
            tlbsearch.Height = txtsearch.Height

            txtsearch.Left = tlbsearch.Left - txtsearch.Width + 1
            txtsearch.Top = tlbsearch.Top
            txtsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle

            tlbgg2.SuspundeLayout = True
            tlbgg2.Height = txtgg.Height
            tlbgg2.BorderColor = txtgg.BorderColor
            tlbgg2.BorderColorDropDown = tlbgg2.BorderColor
            tlbgg2.Left = txtsearch.Left - tlbgg2.Width - 1
            tlbgg2.Top = tlbsearch.Top
            tlbgg2.Button(0).SubWidth = txtgg.Width + tlbgg2.Button(0).Width - 1
            tlbgg2.Button(0).SubItemsAutoSize = False
            tlbgg2.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbgg2.SuspundeLayout = False

            txtgg.Left = tlbgg2.Left - txtgg.Width + 1
            txtgg.Top = tlbsearch.Top
            txtgg.FlatStyle = AppSett.Personal.Theme.FlatStyle

            tlbgg1.SuspundeLayout = True
            tlbgg1.Top = tlbsearch.Top
            tlbgg1.Left = txtgg.Left - tlbgg1.Width + 1
            tlbgg1.BorderColor = txtgg.BorderColor
            tlbgg1.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbgg1.BorderColorDropDown = tlbgg2.BorderColor
            tlbgg1.SuspundeLayout = False

            tab1.SuspundeLayout = True
            tab1.Left = IForm1.LX + padd + 5
            tab1.Top = tlbgg2.Top - 1
            tab1.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tab1.Tabs(0).Text = "Ufficiali"
            tab1.Tabs(1).Text = "Top"
            tab1.SuspundeLayout = False

            ln1.Left = IForm1.LX + padd
            ln1.Top = tab1.Top + tab1.Height - ln1.Height \ 2 - 1
            ln1.Width = IForm1.RX - IForm1.LX - padd * 2

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

            tlbaction.Left = IForm1.RX - tlbaction.Width - padd
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle
            For i As Integer = 0 To tlbaction.Button.Count - 1
                tlbaction.Button(i).BorderColor = Color.DimGray
            Next

            lnbot.Top = tlbaction.Top - lnbot.Height - 5
            lnbot.Left = IForm1.LX + padd
            lnbot.Width = IForm1.RX - IForm1.LX - padd * 2

            pnl1.Top = txtsearch.Top + txtsearch.Height + 8
            pnl1.Left = padd
            pnl1.Width = IForm1.RX - IForm1.LX - padd * 2 + 4
            pnl1.Height = lnbot.Top - pnl1.Top - 5

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

    Function DrawFormazionePlayer(ByVal gr As Graphics, ByVal plist As List(Of String), ByVal topspace As Integer) As Integer

        Dim ft As New StringFormat
        ft.Alignment = StringAlignment.Center
        Dim paddf As Integer = 0
        Dim wg As Integer = My.Resources.campo.Width - paddf * 2
        Dim w As Integer = 0
        If plist.Count < 4 Then
            w = wg \ plist.Count
        Else
            w = wg \ 3
        End If
        Dim col As Integer = 0
        Dim row As Integer = 0
        For i As Integer = 0 To plist.Count - 1
            gr.DrawImage(SystemFunction.DrawingAndImage.DrawGlowText3(plist(i), New Font("Tahoma", 10, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, Color.Black, 30, False, False, ft, w, 15), (w * col) + paddf, topspace + row * 45)
            gr.DrawImage(My.Resources.point, (w * col) + paddf + w \ 2 - My.Resources.point.Width \ 2, topspace + 13 + row * 43)
            col = col + 1
            If col > 2 AndAlso i < plist.Count - 1 Then
                col = 0
                row = row + 1
                If plist.Count - row * 3 < 4 Then
                    w = wg \ (plist.Count - row * 3)
                Else
                    w = wg \ 3
                End If
            End If
        Next
        Return topspace + (row + 1) * 40
    End Function

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        Timer1.Enabled = False

        Try

            'Setto i nomi nella casella di ricerca'
            Call SetTeamName()

            'Determino le immagini dei vari team'
            Call DrawFormazioni()

            'Posiziono le varie rose sullo schermo'
            Call SetControlPosition()

            lbstartup.Visible = False
            pnl1.Visible = True

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

    Sub SetControlPosition()

        If p.Count = 0 Then Exit Sub

        Try
            Dim x As Integer = 0
            Dim y As Integer = 10


            For i As Integer = 0 To currlega.Formazioni.Count - 1
                p(i).Left = x
                p(i).Top = y - pnl1.VerticalScroll.Value
                l(i).Left = x + 10
                l(i).Top = p(i).Top + p(i).Height - 18
                l(i).Visible = currlega.Settings.Jolly.EnableJollyPlayer
                l(i).Height = 16
                l(i).BringToFront()
                t(i).Left = p(i).Left + p(i).Width - 81 - 6
                t(i).Top = p(i).Top + p(i).Height - 18
                t(i).Height = 16
                t(i).BringToFront()
                x = x + p(i).Width + 10
                If x > pnl1.Width - 20 - p(i).Width Then
                    y = y + p(i).Height + 10
                    x = 0
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

    Sub DrawFormazioni()

        Try
            mnumodforma.Enabled = Not (topforma)
            If txtsearch.Text <> "" OrElse topforma Then
                mnuautoforma.Enabled = False
            Else
                mnuautoforma.Enabled = True
            End If
            gio = CInt(txtgg.Text)
            If txtsearch.Text <> "" Then
                currlega.LoadFormazioni(-1, txtsearch.Text, topforma)
            Else
                currlega.LoadFormazioni(gio, txtsearch.Text, topforma)
            End If
            p.Clear()
            t.Clear()

            Dim h As Integer = 250 + currlega.Settings.NumberOfReserve * 14
            If currlega.Settings.Bonus.EnableBonusDefense Then h = h + 17
            If currlega.Settings.Bonus.EnableCenterField Then h = h + 17
            If currlega.Settings.Bonus.EnableBonusAttack Then h = h + 17
            If currlega.Settings.SubstitutionType <> LegaObject.LegaSettings.eSubstitutionType.Normal Then h = h + 17
            h += 10

            For i As Integer = 0 To currlega.Formazioni.Count - 1
                'Configuro le varie formazione'
                'Aggiungo la picturebox'
                p.Add(New PictureBox)
                p(i).Width = 254
                p(i).Height = h
                p(i).BackColor = Color.White
                p(i).Image = SystemFunction.DrawingAndImage.GetImageForma(currlega.Formazioni(i), txtsearch.Text, False, p(i).Width, p(i).Height, Nothing)
                p(i).Tag = i
                p(i).Name = "FORMA-" & i
                p(i).ContextMenuStrip = mnu1
                'Associo l'evento button click alla funzione'
                AddHandler p(i).MouseEnter, AddressOf FormaMouseEnter '
                'Agguingo la label informativa del jolly'
                l.Add(New iControl.iLabel)
                l(i).Image = My.Resources.star10
                l(i).Text = "Jolly"
                l(i).Font = New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel)
                l(i).Visible = currlega.Settings.Jolly.EnableJollyPlayer
                'Aggiungo la toolbar'
                t.Add(New iControl.iToolBar)
                t(i).AutoSize = True
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(0).Image = My.Resources.edit_voti_black_14
                t(i).Button(0).ToolTips = "Modifica voti"
                t(i).Button(0).Enabled = Not (topforma)
                t(i).Button(0).Tag = "0"
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(1).Image = My.Resources.edit_forma_black_14
                t(i).Button(1).ToolTips = "Modifica formazione"
                t(i).Button(1).Enabled = Not (topforma)
                t(i).Button(1).Tag = "1"
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(2).Image = My.Resources.import_black_14
                t(i).Button(2).ToolTips = "Importa formazione"
                t(i).Button(2).Enabled = Not (topforma)
                t(i).Button(2).Tag = "6"
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(3).Image = My.Resources.export_black_14
                t(i).Button(3).ToolTips = "Esporta formazione"
                t(i).Button(3).Tag = "7"
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(4).Image = My.Resources.del_black_14
                t(i).Button(4).ToolTips = "Elimina formazione"
                t(i).Button(4).Enabled = Not (topforma)
                t(i).Button(4).Tag = "8"

                t(i).BackColor = Color.White
                t(i).BorderColor = Color.DimGray
                t(i).FlatStyle = True
                t(i).draw(True)
                t(i).Tag = i
                'Associo l'evento button click alla funzione'
                AddHandler t(i).ButtonClick, AddressOf ButtonClick
                'Aggiungo i 2 controlli al pannello'
                pnl1.Controls.Add(p(i))
                pnl1.Controls.Add(l(i))
                pnl1.Controls.Add(t(i))

            Next

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

    Private Sub FormaMouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim t As PictureBox = CType(sender, PictureBox)
        If t IsNot Nothing Then currforma = CInt(t.Name.Replace("FORMA-", "")) Else currforma = -1
    End Sub

    Sub ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer)
        Dim t As iControl.iToolBar = CType(sender, iControl.iToolBar)
        Dim id As Integer = CInt(t.Tag)
        Call Action(CInt(t.Button(ButtonIndex).Tag), id)
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            Dim g As Integer = CInt(txtgg.Text)
            If keyData = Keys.Left Then
                g = g - 1
                If g < 1 Then g = 1
                If CStr(g) <> txtgg.Text Then
                    txtgg.Text = CStr(g)
                    Call LoadData()
                End If
                Return True
            ElseIf keyData = Keys.Right Then
                g = g + 1
                If g > currlega.Settings.NumberOfDays Then g = currlega.Settings.NumberOfDays
                If CStr(g) <> txtgg.Text Then
                    txtgg.Text = CStr(g)
                    Call LoadData()
                End If
                Return True
            ElseIf (keyData = (Keys.Control Or Keys.Delete)) Then
                LegaObject.Formazione.Delete(gio, False)
                LegaObject.Formazione.Delete(gio, False)
                Call LoadData()
            ElseIf keyData = Keys.PageUp Then
                Dim v As Integer = pnl1.VerticalScroll.Value
                v = v - pnl1.VerticalScroll.LargeChange
                If v < 0 Then v = 0
                pnl1.VerticalScroll.Value = v
                pnl1.PerformLayout()
            ElseIf keyData = Keys.PageDown Then
                Dim v As Integer = pnl1.VerticalScroll.Value
                v = v + pnl1.VerticalScroll.LargeChange
                If v > pnl1.VerticalScroll.Maximum Then v = pnl1.VerticalScroll.Maximum
                pnl1.VerticalScroll.Value = v
                pnl1.PerformLayout()
            ElseIf keyData = Keys.Alt + Keys.U Then
                tab1.TabSelectIndex = 0
                Call LoadData()
            ElseIf keyData = Keys.Alt + Keys.T Then
                tab1.TabSelectIndex = 1
                Call LoadData()
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Sub Action(ByVal Act As Integer, ByVal Id As Integer)
        Try
            mnu1.Hide()
            Select Case Act
                Case 0
                    'Dim frm As New frmeditvoti
                    'frm.SetParameater(currlega.Formazioni(id).IdTeam)
                    'If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                    'Call DrawFormazioni()
                    'End If
                Case 1
                    Dim frm As New frmcompformazioni
                    If Id = -1 Then
                        frm.SetParameater(gio, 0, Me)
                    Else
                        frm.SetParameater(gio, Id, Me)
                    End If
                    frm.Show(Me)
                Case 2, 6, 21
                    Application.RemoveMessageFilter(Me)
                    Dim frm As New frmimpexp
                    frm.SetParameater(frmimpexp.TypeOfOperation.Import)
                    If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                        If Id = -1 OrElse Act = 2 Then
                            Call ImpExp.ImpExpFormazioni.ImportHtml(gio, -1, "", topforma, frm.GetDirectory, False)
                        ElseIf Act = 6 Then
                            Call ImpExp.ImpExpFormazioni.ImportHtml(currlega.Formazioni(Id).Giornata, currlega.Formazioni(Id).IdTeam, currlega.Formazioni(Id).Nome, frm.GetDirectory, False)
                        ElseIf Act = 21 Then
                            For i As Integer = 1 To currlega.Settings.NumberOfDays
                                Call ImpExp.ImpExpFormazioni.ImportHtml(i, -1, "", topforma, frm.GetDirectory, False)
                            Next
                        End If
                        Call LoadData()
                    End If
                    frm.Dispose()
                    Application.AddMessageFilter(Me)
                Case 3, 7, 22
                    Application.RemoveMessageFilter(Me)
                    Dim frm As New frmimpexp
                    frm.SetParameater(frmimpexp.TypeOfOperation.Export)
                    If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                        If Id = -1 OrElse Act = 3 Then
                            Call ImpExp.ImpExpFormazioni.ExportHtml(gio, currlega.Formazioni, txtsearch.Text, topforma, frm.GetDirectory, False)
                        ElseIf Act = 7 Then
                            Call ImpExp.ImpExpFormazioni.ExportHtml(currlega.Formazioni(Id).Giornata, currlega.Formazioni(Id), topforma, frm.GetDirectory, False)
                        ElseIf Act = 22 Then
                            For i As Integer = 1 To currlega.Settings.NumberOfDays
                                Call ImpExp.ImpExpFormazioni.ExportHtml(i, topforma, frm.GetDirectory, True)
                            Next
                        End If
                    End If
                    frm.Dispose()
                    Application.AddMessageFilter(Me)
                Case 4, 8
                    If Id = -1 OrElse Act = 4 Then
                        If iControl.iMsgBox.ShowMessage("Sei sicuro di voler eliminare tutte le formazioni della giornata?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.Yes Then
                            LegaObject.Formazione.Delete(gio, False)
                            LegaObject.Formazione.Delete(gio, True)
                            Call LoadData()
                        End If
                    Else
                        If iControl.iMsgBox.ShowMessage("Sei sicuro di voler eliminare la formazione?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.Yes Then
                            LegaObject.Formazione.Delete(currlega.Formazioni(Id).Giornata, currlega.Formazioni(Id).IdTeam, False)
                            LegaObject.Formazione.Delete(currlega.Formazioni(Id).Giornata, currlega.Formazioni(Id).IdTeam, True)
                            Call LoadData()
                        End If
                    End If
                Case 5 : Me.Close()
                Case 9 : My.Computer.Clipboard.SetImage(p(Id).Image)
                Case 10
                    CopyDataTable(Id)
                Case 11
                    CopyDataTable(-1)
                Case 12
                    Application.RemoveMessageFilter(Me)
                    Dim dlg As New Windows.Forms.FolderBrowserDialog
                    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Dim fname As String = ImpExp.ImpExpFormazioni.GetHtmlFileName(gio, currlega.Formazioni(Id).IdTeam, currlega.Formazioni(Id).Nome, topforma, dlg.SelectedPath, "png")
                        p(Id).Image.Save(fname)
                    End If
                    Application.AddMessageFilter(Me)
                Case 13
                    If fcopy.Count > 0 Then
                        For i As Integer = 0 To fcopy.Count - 1
                            fcopy(i).Giornata = gio
                            fcopy(i).Save()
                        Next
                        Call LoadData()
                    End If
                Case 14
                    If iControl.iMsgBox.ShowMessage("Sei sicuro di voler inserire le formazioni automatiche?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.OK Then
                        LegaObject.Formazione.Delete(gio, False)
                        LegaObject.Formazione.Delete(gio, True)
                        currlega.GetAutomaticFormation(gio)
                        Call LoadData()
                    End If
                Case 15

                    Application.RemoveMessageFilter(Me)

                    Dim frme As New frmimpexp
                    Dim res As DialogResult = Windows.Forms.DialogResult.OK
                    frme.SetParameater(frmimpexp.TypeOfOperation.Import)
                    res = frme.ShowDialog
                    Dim dir As String = frme.GetDirectory()
                    frme.Dispose()

                    If res = Windows.Forms.DialogResult.OK Then

                        Dim frmw As New frmwait
                        frmw.Show(Me)
                        frmw.lbmain.Text = "Importazione dati in corso..."
                        frmw.Update()
                        System.Threading.Thread.Sleep(100)

                        Dim f() As String = {"tbteam", "tbrose", "tbplayer", "tbplayer_data", "tbmatch", "tbtabellini", "tbdati", "tbformazioni", "tbformazionitop"}
                        'Dim f() As String = {"tbdati"}

                        For i As Integer = 0 To f.Length - 1
                            Call ImpExp.ImportDati(dir & "\" & f(i) & ".xml", f(i))
                            System.Threading.Thread.Sleep(10)
                        Next

                        Call webdata.UpdatePlayerTbFromView(False)

                        frmw.Update()
                        System.Threading.Thread.Sleep(100)

                        frmw.Close()
                        frmw.Dispose()

                        Call LoadData()

                    End If

                    Application.AddMessageFilter(Me)

                Case 16

                    Dim frme As New frmimpexp
                    Dim res As DialogResult = Windows.Forms.DialogResult.OK
                    frme.SetParameater(frmimpexp.TypeOfOperation.Export)
                    res = frme.ShowDialog
                    Dim dir As String = frme.GetDirectory()
                    frme.Dispose()

                    If res = Windows.Forms.DialogResult.OK Then

                        Dim frmw As New frmwait
                        frmw.Show(Me)
                        frmw.lbmain.Text = "Esportazione dati in corso..."
                        frmw.Update()
                        System.Threading.Thread.Sleep(100)

                        Dim f() As String = {"tbteam", "tbrose", "tbplayer", "tbplayer_data", "tbmatch", "tbtabellini", "tbdati", "tbformazioni", "tbformazionitop"}

                        For i As Integer = 0 To f.Length - 1
                            Call ImpExp.ExportDati("select * from " & f(i) & ";", dir & "\" & f(i) & ".xml")
                            System.Threading.Thread.Sleep(10)
                        Next

                        frmw.Update()
                        System.Threading.Thread.Sleep(100)

                        frmw.Close()
                        frmw.Dispose()

                    End If

                Case 17

                    If iControl.iMsgBox.ShowMessage("Sei sicuro di voler eliminare tutte le formazioni del torneo?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.Yes Then
                        LegaObject.Formazione.Delete(False)
                        LegaObject.Formazione.Delete(True)
                        Call LoadData()
                    End If

                Case 18

                    If Id <> -1 Then
                        If iControl.iMsgBox.ShowMessage("Inviare via mail la formazione della squadra " & currlega.Formazioni(Id).Nome & " all'ammministratore del torneo?", "Conferma", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.Yes Then

                            Dim cc As String = ""

                            If AppSett.Personal.SendMailAlsoToMe Then
                                cc = AppSett.Personal.Mail
                            End If

                            Dim intconn As New InternetConnection.ConnType

                            intconn = InternetConnection.Type

                            If intconn <> InternetConnection.ConnType.offline Then
                                If SystemFunction.Mail.SendFormazione(currlega.Settings.MailAdmin, cc, currlega.Formazioni(Id), currlega.Settings.Nome) Then
                                    iControl.iMsgBox.ShowMessage("Mail inviata con successo", "Conferma", iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgInfo)
                                Else
                                    iControl.iMsgBox.ShowMessage("Invio mail fallito, contattare l'amministratore del servizio", "Errore", iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgError)
                                End If
                            Else
                                iControl.iMsgBox.ShowMessage("Invio mail fallito, connessione internet non disponibile", "Errore", iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgError)
                            End If

                        End If

                    End If

                Case 20

                    If Id <> -1 Then
                        Dim frm As New frmmail
                        frm.Attachements.Add(ImpExp.ImpExpFormazioni.ExportHtml(gio, currlega.Formazioni(Id), topforma, True))
                        frm.ShowDialog()
                    End If


            End Select
        Catch ex As Exception
            ShowError("Errore", ex.Message)
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Sub CopyDataTable(ByVal id As Integer)

        Try

            Dim fname1 As String = SystemFunction.FileAndDirectory.GetTempDirectory & "\copy1.txt"
            Dim st As Integer = 6
            Dim sw1 As New IO.StreamWriter(fname1)

            fcopy.Clear()
            mnupasteforma.Enabled = True

            If id = -1 Then
                sw1.WriteLine("<table border='0'>")
                For i As Integer = 0 To currlega.Formazioni.Count - 1 Step st
                    sw1.WriteLine("<tr>")
                    For j As Integer = i To i + st - 1
                        If j < currlega.Formazioni.Count Then
                            If j = id OrElse id = -1 Then
                                sw1.WriteLine("<td>")
                                sw1.WriteLine(CopyFormation(currlega.Formazioni(j)))
                                sw1.WriteLine("</td>")
                            End If
                        End If
                    Next
                    sw1.WriteLine("</tr>")
                    sw1.WriteLine("<tr><td></td></tr>")
                Next
                sw1.WriteLine("</table>")
            Else
                sw1.WriteLine(CopyFormation(currlega.Formazioni(id)))
            End If

            sw1.Flush()
            sw1.Close()
            sw1.Dispose()

            Dim dat As New DataObject()

            'Gestione clipboard html'
            If IO.File.Exists(fname1) Then
                dat = SystemFunction.General.SetDataClipBoardHtmlFormat(IO.File.ReadAllText(fname1), "FORMA_" & id)
            End If

            'Setto i dati nella clipboard'
            My.Computer.Clipboard.SetDataObject(dat)

        Catch ex As Exception
            ShowError("Errore", ex.Message)
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

    Function CopyFormation(ByVal Formazione As LegaObject.Formazione) As String

        Dim str As New System.Text.StringBuilder
        Dim t As New List(Of Integer)
        Dim cind As Integer

        'Gestione copia all'interno del programma'
        fcopy.Add(New LegaObject.Formazione(Formazione.IdTeam, Formazione.Nome, Formazione.Allenatore))
        cind = fcopy.Count - 1
        For k As Integer = 0 To Formazione.Players.Count - 1
            fcopy(cind).BonusDifesa = Formazione.BonusDifesa
            fcopy(cind).BonusCentroCampo = Formazione.BonusCentroCampo
            fcopy(cind).BonusAttacco = Formazione.BonusAttacco
            fcopy(cind).Players.Add(New LegaObject.Formazione.PlayerFormazione(Formazione.Players(k).IdRosa, Formazione.Players(k).Jolly, Formazione.Players(k).Type, Formazione.Players(k).IdFormazione, Formazione.Players(k).Ruolo, Formazione.Players(k).Nome))
        Next

        'Gestione copia in clipboard'
        str.AppendLine("<table border='0'>")
        str.AppendLine("<tr><td colspan=7 style='color:#F00;'>" & Formazione.Nome.Replace("’", "'") & "</td><td colspan=2>Punti:</td><td style='color:#00F;'>" & Formazione.Pt & "</td></tr>")
        str.AppendLine("<tr><td colspan=7>" & Formazione.Allenatore.Replace("’", "'") & "</td><td colspan=2>Modulo:</td><td>&nbsp;" & Formazione.Modulo & "</td></tr>")

        'Titolari'
        str.AppendLine(CopyPlayer(Formazione, 1))
        'Panchinari'
        str.AppendLine(CopyPlayer(Formazione, 2))

        str.AppendLine("</table>")

        Return str.ToString

    End Function

    Function CopyPlayer(ByVal Formazione As LegaObject.Formazione, ByVal type As Integer) As String
        Dim str As New System.Text.StringBuilder
        str.AppendLine("<tr style='color:#F00;'><td bgcolor='#FF8'>R</td><td>Nome</td><td>vt</td><td>amm</td><td>esp</td><td>ass</td><td>ag</td><td>gs/f</td><td>rigs/p</td><td>pt</td><td></td></tr>")
        For k As Integer = 0 To Formazione.Players.Count - 1
            If Formazione.Players(k).Type = type Then
                Dim incampo As Integer = Formazione.Players(k).InCampo
                Dim rowstyle As String = ""
                Dim rolecolor As String = ""
                Dim strvt As String = CStr(Formazione.Players(k).Dati.Vt)
                Dim strpt As String = CStr(Formazione.Players(k).Dati.Pt)
                If strvt = "-20" Then strvt = "s.v."
                If strpt = "-20" Then strpt = "s.v."
                If incampo = 1 Then
                    rolecolor = "style='color:" & System.Drawing.ColorTranslator.ToHtml(SystemFunction.General.GetRuoloForeColor(Formazione.Players(k).Ruolo)) & ";'"
                Else
                    rowstyle = " style='color:#CCC;' "
                    rolecolor = "style='color:" & System.Drawing.ColorTranslator.ToHtml(SystemFunction.General.GetRuoloForeColorDisable(Formazione.Players(k).Ruolo)) & ";'"
                End If
                str.AppendLine("<tr" & rowstyle & "><td " & rolecolor & ">" & Formazione.Players(k).Ruolo & "</td><td>" & Formazione.Players(k).Nome.Replace("’", "'") & "</td><td>" & strvt & "</td><td>" & Formazione.Players(k).Dati.Amm & "</td><td>" & Formazione.Players(k).Dati.Esp & "</td><td>" & Formazione.Players(k).Dati.Ass & "</td><td>" & Formazione.Players(k).Dati.AutG & "</td><td>" & Formazione.Players(k).Dati.Gs & "/" & Formazione.Players(k).Dati.Gf & "</td><td>" & Formazione.Players(k).Dati.RigS & "/" & Formazione.Players(k).Dati.RigP & "</td><td>" & strpt & "</td></tr>")
            End If
        Next
        Return str.ToString
    End Function

    Sub SetTeamName()
        Try
            'Imposto l'elenco delle squadre'
            txtsearch.AutoCompleteList.Clear()
            tlbsearch.Button(0).SubItems.Clear()
            For i As Integer = 0 To currlega.Teams.Count - 1
                txtsearch.AutoCompleteList.Add(currlega.Teams(i).Nome)
                tlbsearch.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(currlega.Teams(i).Nome))
            Next
        Catch ex As Exception

        End Try
    End Sub

    Sub SetDaysList()
        'Imposto l'elenco delle squadre'
        txtgg.AutoCompleteList.Clear()
        For i As Integer = 0 To currlega.Settings.NumberOfDays - 1
            txtgg.AutoCompleteList.Add(CStr(i + 1))
            tlbgg2.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem(CStr(i + 1)))
            If i + 1 = gio Then tlbgg2.Button(0).SubItems(i).State = True
        Next
        txtgg.Text = CStr(gio)
    End Sub

    Sub LoadData()
        pnl1.Controls.Clear()
        pnl1.Update()
        pnl1.Visible = False
        lbstartup.Visible = True
        Call DrawFormazioni()
        Call SetControlPosition()
        lbstartup.Visible = False
        pnl1.Visible = True
    End Sub

    Private Sub mnu1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnu1.ItemClicked
        Call Action(CInt(e.ClickedItem.Tag), currforma)
    End Sub

    Private Sub txtgg_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtgg.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call LoadData()
        End If
    End Sub

    Private Sub tlbgg_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbgg2.SubButtonClick
        txtgg.Text = tlbgg2.Button(ButtonIndex).SubItems(SubButtonIndex).Text
        tlbgg2.Button(ButtonIndex).ClearSubButtonSelection()
        tlbgg2.Button(ButtonIndex).SubItems(SubButtonIndex).State = True
        Call LoadData()
    End Sub

    Private Sub txtsearch_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call LoadData()
        End If
    End Sub

    Private Sub tlbsearch_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbsearch.SubButtonClick
        txtsearch.Text = tlbsearch.Button(ButtonIndex).SubItems(SubButtonIndex).Text
        Call LoadData()
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Call Action(ButtonIndex, -1)
    End Sub

    Private Sub frm_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If start Then Exit Sub
        Call SetControlPosition()
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Private Sub tlbgg1_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbgg1.ButtonClick
        If ButtonIndex > 1 Then
            Dim exe As Boolean = True
            Select Case ButtonIndex
                Case 2 : gio = gio - 1
                Case 3 : gio = gio + 1
            End Select
            If gio < 1 Then gio = 1 : exe = False
            If gio > currlega.Settings.NumberOfDays Then gio = currlega.Settings.NumberOfDays : exe = False

            If exe Then
                tlbgg2.Button(0).ClearSubButtonSelection()
                tlbgg2.Button(0).SubItems(gio - 1).State = True
                txtgg.Text = CStr(gio)
                Call LoadData()
            End If
        End If
    End Sub

    Public Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean Implements IMessageFilter.PreFilterMessage
        If Me.OwnedForms.Length = 0 Then
            Const WM_MOUSEWHEEL As Integer = &H20A
            If m.Msg = WM_MOUSEWHEEL Then
                Try 'this solves too fast wheeling
                    Dim delta As Integer = m.WParam.ToInt32() >> 16
                    Dim v As Integer = pnl1.VerticalScroll.Value
                    v = v - delta
                    If v < 0 Then v = 0
                    If v > pnl1.VerticalScroll.Maximum Then v = pnl1.VerticalScroll.Maximum
                    pnl1.VerticalScroll.Value = v
                    pnl1.PerformLayout()
                Catch ex As Exception
                    Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                End Try
            End If
        End If
        Return False
    End Function

    Private Sub tab1_TabClick(ByVal sender As System.Object, ByVal TabInd As System.Int32) Handles tab1.TabClick
        Select Case TabInd
            Case 0 : topforma = False
            Case 1 : topforma = True
        End Select
        tlbaction.Button(0).Enabled = Not (topforma)
        tlbaction.Button(1).Enabled = Not (topforma)
        tlbaction.Button(2).Enabled = Not (topforma)
        tlbaction.Button(4).Enabled = Not (topforma)
        mnupasteforma.Enabled = Not (topforma) AndAlso fcopy.Count > 0
        mnuimpforma.Enabled = Not (topforma)
        mnuimpallforma.Enabled = Not (topforma)
        mnudelforma.Enabled = Not (topforma)
        mnudelallforma.Enabled = Not (topforma)
        mnuautoforma.Enabled = Not (topforma)
        mnumodforma.Enabled = Not (topforma)
        mnudeltorneoforma.Enabled = Not (topforma)
        mnuimpdatiformatorneo.Enabled = Not (topforma)
        mnuexpdatiformatorneo.Enabled = Not (topforma)
        tlbaction.draw(True)
        Call LoadData()
    End Sub

    Private Sub pnl1_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pnl1.MouseWheel
        pnl1.VerticalScroll.Value = pnl1.VerticalScroll.Value + 10
    End Sub

End Class
