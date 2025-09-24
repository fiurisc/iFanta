Imports System.Drawing.Drawing2D

Public Class frmcoppa

    Implements IMessageFilter

    Dim cup As New LegaObject.Coppa
    Dim pdg As Integer = 40
    Dim picspace As PictureBox
    Dim recg1 As New Dictionary(Of String, Rectangle)
    Dim recg2 As New Dictionary(Of String, Rectangle)
    Dim rep1 As New Dictionary(Of String, Rectangle)
    Dim rep2 As New Dictionary(Of String, Rectangle)
    Dim reqf As New Dictionary(Of String, Rectangle)
    Dim resf As New Dictionary(Of String, Rectangle)
    Dim refi As New Dictionary(Of String, Rectangle)
    Dim imgclasa As Bitmap

    Private Sub frmcoppa_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Application.RemoveMessageFilter(Me)
    End Sub

    Private Sub frmcoppa_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName
        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        Application.AddMessageFilter(Me)

        lbby.Text = My.Application.Info.Copyright

        If My.Computer.Screen.WorkingArea.Width > 1224 Then
            Me.Width = 1000
            pdg = 80
        ElseIf My.Computer.Screen.WorkingArea.Width > 1024 Then
            Me.Width = 900
            pdg = 60
        Else
            Me.Width = 800
        End If
        If My.Computer.Screen.WorkingArea.Height > 900 Then
            Me.Height = 800
        ElseIf My.Computer.Screen.WorkingArea.Height > 700 Then
            Me.Height = 700
        Else
            Me.Height = 550
        End If

        tlbaction.Button(1).Visible = currlega.Settings.Admin

        Me.Location = New Point(My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2, My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2)

        Dim pt1 As Integer = 0
        Dim pt2 As Integer = 0

        'cup.Load()

        'cup.GironiEliminatori.Add(New LegaObject.Coppa.Girone)
        'cup.GironiEliminatori.Add(New LegaObject.Coppa.Girone)

        'For i As Integer = 0 To 4
        '    cup.GironiEliminatori(0).Clasa.Add(New LegaObject.Coppa.Girone.ClasaGirone(i, currlega.Teams(i).IdTeam))
        'Next
        'For i As Integer = 0 To 4
        '    cup.GironiEliminatori(1).Clasa.Add(New LegaObject.Coppa.Girone.ClasaGirone(i, currlega.Teams(i + 5).IdTeam))
        'Next

        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 1, 2, 14, 0, 1, 0, 1))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 1, 2, 14, 2, 3, 2, 3))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 2, 3, 15, 0, 2, 0, 2))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 2, 3, 15, 4, 3, 4, 3))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 3, 7, 19, 3, 0, 3, 0))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 3, 7, 19, 1, 4, 1, 4))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 4, 8, 20, 0, 4, 0, 4))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 4, 8, 20, 2, 1, 2, 1))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 5, 10, 22, 3, 1, 3, 1))
        'cup.GironiEliminatori(0).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 5, 10, 22, 4, 2, 4, 2))

        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 1, 2, 14, 3, 1, 8, 6))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 1, 2, 14, 0, 4, 5, 9))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 2, 3, 15, 0, 2, 5, 7))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 2, 3, 15, 4, 3, 9, 8))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 3, 7, 19, 4, 1, 9, 6))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 3, 7, 19, 3, 2, 8, 7))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 4, 8, 20, 1, 0, 6, 5))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 4, 8, 20, 2, 4, 7, 9))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(0, 5, 10, 22, 2, 1, 7, 6))
        'cup.GironiEliminatori(1).Partite.Add(New LegaObject.Coppa.Girone.PartitaGirone(1, 5, 10, 22, 0, 3, 5, 8))

        'currlega.Classifica.LoadHistory()

        'For j As Integer = 0 To cup.GironiEliminatori.Count - 1
        '    For i As Integer = 0 To cup.GironiEliminatori(j).Partite.Count - 1

        '        cup.GironiEliminatori(j).Partite(i).GoalAnd1 = GetGoal(currlega.Classifica.History(cup.GironiEliminatori(j).Partite(i).TeamId1).Giornate(cup.GironiEliminatori(j).Partite(i).GiornataAndata).Pt, True)
        '        cup.GironiEliminatori(j).Partite(i).GoalAnd2 = GetGoal(currlega.Classifica.History(cup.GironiEliminatori(j).Partite(i).TeamId2).Giornate(cup.GironiEliminatori(j).Partite(i).GiornataAndata).Pt, False)
        '        cup.GironiEliminatori(j).Partite(i).GoalRit1 = GetGoal(currlega.Classifica.History(cup.GironiEliminatori(j).Partite(i).TeamId1).Giornate(cup.GironiEliminatori(j).Partite(i).GiornataRitorno).Pt, False)
        '        cup.GironiEliminatori(j).Partite(i).GoalRit2 = GetGoal(currlega.Classifica.History(cup.GironiEliminatori(j).Partite(i).TeamId2).Giornate(cup.GironiEliminatori(j).Partite(i).GiornataRitorno).Pt, True)

        '        pt1 = GetPtTeam(cup.GironiEliminatori(j).Partite(i).GoalAnd1, cup.GironiEliminatori(j).Partite(i).GoalAnd2)
        '        pt2 = GetPtTeam(cup.GironiEliminatori(j).Partite(i).GoalAnd2, cup.GironiEliminatori(j).Partite(i).GoalAnd1)
        '        If pt1 > -1 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).PartiteGiocate = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).PartiteGiocate + 1
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).Pt = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).Pt + pt1
        '        End If
        '        If pt2 > -1 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).PartiteGiocate = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).PartiteGiocate + 1
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).Pt = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).Pt + pt2
        '        End If

        '        SetTypeOfResult(cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1), pt1)
        '        SetTypeOfResult(cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2), pt2)

        '        pt1 = GetPtTeam(cup.GironiEliminatori(j).Partite(i).GoalRit1, cup.GironiEliminatori(j).Partite(i).GoalRit2)
        '        pt2 = GetPtTeam(cup.GironiEliminatori(j).Partite(i).GoalRit2, cup.GironiEliminatori(j).Partite(i).GoalRit1)
        '        If pt1 > -1 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).PartiteGiocate = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).PartiteGiocate + 1
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).Pt = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).Pt + pt1
        '        End If
        '        If pt2 > -1 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).PartiteGiocate = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).PartiteGiocate + 1
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).Pt = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).Pt + pt2
        '        End If

        '        If cup.GironiEliminatori(j).Partite(i).GoalAnd1 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalFatti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalFatti + cup.GironiEliminatori(j).Partite(i).GoalAnd1
        '        End If
        '        If cup.GironiEliminatori(j).Partite(i).GoalRit1 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalFatti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalFatti + cup.GironiEliminatori(j).Partite(i).GoalRit1
        '        End If
        '        If cup.GironiEliminatori(j).Partite(i).GoalAnd2 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalFatti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalFatti + cup.GironiEliminatori(j).Partite(i).GoalAnd2
        '        End If
        '        If cup.GironiEliminatori(j).Partite(i).GoalRit2 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalFatti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalFatti + cup.GironiEliminatori(j).Partite(i).GoalRit2
        '        End If

        '        If cup.GironiEliminatori(j).Partite(i).GoalAnd2 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalSubiti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalSubiti + cup.GironiEliminatori(j).Partite(i).GoalAnd2
        '        End If
        '        If cup.GironiEliminatori(j).Partite(i).GoalRit2 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalSubiti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1).GoalSubiti + cup.GironiEliminatori(j).Partite(i).GoalRit2
        '        End If
        '        If cup.GironiEliminatori(j).Partite(i).GoalAnd1 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalSubiti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalSubiti + cup.GironiEliminatori(j).Partite(i).GoalAnd1
        '        End If
        '        If cup.GironiEliminatori(j).Partite(i).GoalRit1 > 0 Then
        '            cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalSubiti = cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2).GoalSubiti + cup.GironiEliminatori(j).Partite(i).GoalRit1
        '        End If

        '        SetTypeOfResult(cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId1), pt1)
        '        SetTypeOfResult(cup.GironiEliminatori(j).Clasa(cup.GironiEliminatori(j).Partite(i).TeamGironeId2), pt2)

        '    Next
        'Next

        Call LoadCoppa()

        'Setto il tema corrente'
        Call SetTheme()

    End Sub

    Overrides Sub ResetTorneo()
        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)
        Call LoadCoppa()
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Call Action(ButtonIndex)
    End Sub

    Sub LoadCoppa()

        cup.Load()

        'Determino le immagini dei vari team'
        Call DrawCoppa()

    End Sub

    Sub SetTheme()

        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)

        tlbaction.draw(True)
        tlbaction.Left = IForm1.RX - tlbaction.Width - padd
        tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
        tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbaction.draw(True)

        lnbot.Top = tlbaction.Top - lnbot.Height - 5
        lnbot.Left = IForm1.LX + padd
        lnbot.Width = IForm1.RX - IForm1.LX - padd * 2

        lbby.Left = IForm1.LX + padd - 3
        lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

        pnl1.Top = IForm1.TY + padd
        pnl1.Left = padd
        pnl1.Width = IForm1.RX - IForm1.LX - padd * 2 + 4
        pnl1.Height = lnbot.Top - pnl1.Top - 5

    End Sub

    Private Sub Action(ByVal Act As Integer)
        Try
            Select Case Act
                Case 0
                    Dim frm As New frmimpexp
                    Application.RemoveMessageFilter(Me)
                    frm.SetParameater(frmimpexp.TypeOfOperation.Export)
                    If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Call ImpExp.ImpExpCup.ExportHtml(cup, frm.GetDirectory)
                    End If
                    frm.Dispose()
                    Application.AddMessageFilter(Me)
                Case 1
                    Dim frm As New frmeditcoppa
                    Application.RemoveMessageFilter(Me)
                    If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Call LoadCoppa()
                    End If
                    Application.AddMessageFilter(Me)
                Case 2
                    Me.Close()
                Case 3
                    My.Computer.Clipboard.SetImage(GetImageCoppa)
                Case 4
                    Dim dlg As New Windows.Forms.FolderBrowserDialog
                    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Dim fname As String = ImpExp.ImpExpCup.GetHtmlFileName("ITALIA", dlg.SelectedPath, "png")
                        GetImageCoppa.Save(fname)
                    End If
                Case 7
                    'If iControl.iMsgBox.ShowMessage("Inviare via mail la formazione della squadra " & currlega.Formazioni(Id).Nome & " all'ammministratore del torneo?", "Conferma", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.Yes Then
                    '    If SystemFunction.Mail.SendFormazione(currlega.Settings.MailAdmin, currlega.Formazioni(Id), currlega.Settings.Nome) Then
                    '        iControl.iMsgBox.ShowMessage("Mail inviata con successo", "Conferma", iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgInfo)
                    '    Else
                    '        iControl.iMsgBox.ShowMessage("Invio mail fallito, contattare l'amministratore del servizio", "Errore", iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgError)
                    '    End If
                    'End If
            End Select
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Function GetImageCoppa() As Bitmap

        Dim y As Integer = pnl1.VerticalScroll.Value
        Dim gr As Graphics
        Dim b1 As New Bitmap(pnl1.Width, picspace.Top + y + picspace.Height)

        gr = Graphics.FromImage(b1)
        gr.Clear(Color.White)
        gr.DrawImage(CType(pnl1.Controls("ELIT-0"), PictureBox).Image, CType(pnl1.Controls("ELIT-0"), PictureBox).Left, CType(pnl1.Controls("ELIT-0"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("ELIC-0"), PictureBox).Image, CType(pnl1.Controls("ELIC-0"), PictureBox).Left, CType(pnl1.Controls("ELIC-0"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("ELIP-0"), PictureBox).Image, CType(pnl1.Controls("ELIP-0"), PictureBox).Left, CType(pnl1.Controls("ELIP-0"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("ELIT-1"), PictureBox).Image, CType(pnl1.Controls("ELIT-1"), PictureBox).Left, CType(pnl1.Controls("ELIT-1"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("ELIC-1"), PictureBox).Image, CType(pnl1.Controls("ELIC-1"), PictureBox).Left, CType(pnl1.Controls("ELIC-1"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("ELIP-1"), PictureBox).Image, CType(pnl1.Controls("ELIP-1"), PictureBox).Left, CType(pnl1.Controls("ELIP-1"), PictureBox).Top + y)
        If cup.TipoSecondoTurno = "quarti" Then
            gr.DrawImage(CType(pnl1.Controls("QFT-1"), PictureBox).Image, CType(pnl1.Controls("QFT-1"), PictureBox).Left, CType(pnl1.Controls("QFT-1"), PictureBox).Top + y)
            gr.DrawImage(CType(pnl1.Controls("QFP-1"), PictureBox).Image, CType(pnl1.Controls("QFP-1"), PictureBox).Left, CType(pnl1.Controls("QFP-1"), PictureBox).Top + y)
        Else
            gr.DrawImage(CType(pnl1.Controls("POT-0"), PictureBox).Image, CType(pnl1.Controls("POT-0"), PictureBox).Left, CType(pnl1.Controls("POT-0"), PictureBox).Top + y)
            gr.DrawImage(CType(pnl1.Controls("POC-0"), PictureBox).Image, CType(pnl1.Controls("POC-0"), PictureBox).Left, CType(pnl1.Controls("POC-0"), PictureBox).Top + y)
            gr.DrawImage(CType(pnl1.Controls("POP-0"), PictureBox).Image, CType(pnl1.Controls("POP-0"), PictureBox).Left, CType(pnl1.Controls("POP-0"), PictureBox).Top + y)
            gr.DrawImage(CType(pnl1.Controls("POT-1"), PictureBox).Image, CType(pnl1.Controls("POT-1"), PictureBox).Left, CType(pnl1.Controls("POT-1"), PictureBox).Top + y)
            gr.DrawImage(CType(pnl1.Controls("POC-1"), PictureBox).Image, CType(pnl1.Controls("POC-1"), PictureBox).Left, CType(pnl1.Controls("POC-1"), PictureBox).Top + y)
            gr.DrawImage(CType(pnl1.Controls("POP-1"), PictureBox).Image, CType(pnl1.Controls("POP-1"), PictureBox).Left, CType(pnl1.Controls("POP-1"), PictureBox).Top + y)
        End If
        gr.DrawImage(CType(pnl1.Controls("SFT-1"), PictureBox).Image, CType(pnl1.Controls("SFT-1"), PictureBox).Left, CType(pnl1.Controls("SFT-1"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("SFP-1"), PictureBox).Image, CType(pnl1.Controls("SFP-1"), PictureBox).Left, CType(pnl1.Controls("SFP-1"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("FIT-1"), PictureBox).Image, CType(pnl1.Controls("FIT-1"), PictureBox).Left, CType(pnl1.Controls("FIT-1"), PictureBox).Top + y)
        gr.DrawImage(CType(pnl1.Controls("FIP-1"), PictureBox).Image, CType(pnl1.Controls("FIP-1"), PictureBox).Left, CType(pnl1.Controls("FIP-1"), PictureBox).Top + y)

        Return CType(b1.Clone, Bitmap)

    End Function

    Sub DrawCoppa()

        Dim y As Integer = 15
        Dim w As Integer = Me.Width - padd * 2 - IForm1.LX * 2
        Dim w1 As Integer = w \ 2 - pdg

        pnl1.Controls.Clear()

        picspace = New PictureBox
        picspace.Height = 15

        pnl1.Controls.Add(picspace)

        y = DrawGironi(y, w, w1, cup.GironiEliminatori, "Girone eliminatorio", "ELI", True)
        If cup.TipoSecondoTurno = "quarti" Then
            DrawPartite(y, w \ 4, w, w1, cup.QuartiDiFinale, "Quarti di finale", "QF")
            y = DrawPartite(y, CInt(w * 0.75), w, w1, cup.SemiFinali, "Semifinali", "SF")
            y = DrawPartite(y, CInt(w * 0.75), w, w1, cup.Finale, "Finale", "FI")
        Else
            y = DrawGironi(y, w, w1, cup.PlayOff, "Playoff", "PO", True)
            Call DrawPartite(y, w \ 4, w, w1, cup.SemiFinali, "Semifinali", "SF")
            y = DrawPartite(y, CInt(w * 0.75), w, w1, cup.Finale, "Finale", "FI")
        End If

        picspace.Top = y

    End Sub

    Function DrawGironi(ByVal y As Integer, ByVal w As Integer, ByVal w1 As Integer, ByVal gir As List(Of LegaObject.Coppa.Girone), ByVal Title As String, ByVal Key As String, ByVal Classifica As Boolean) As Integer

        If gir.Count > 0 Then

            Dim y1 As Integer = 0

            For i As Integer = 0 To gir.Count - 1

                Dim x As Integer = 0

                If CBool(i Mod 2) = False Then
                    x = w \ 4
                Else
                    x = CInt(w * 0.75)
                End If

                Dim pt As New PictureBox
                pt.Width = w1
                pt.Height = 40
                pt.Name = Key & "T-" & i
                pt.Image = GetTitle(Title & " n°" & i + 1, pt.Width, pt.Height)
                pt.Top = y
                pt.Left = x - w1 \ 2
                y1 = pt.Top + pt.Height + 5
                pnl1.Controls.Add(pt)

                If Classifica Then
                    'Aggiungo la picturebox'
                    Dim pg As New PictureBox
                    pg.Width = 230
                    pg.Height = 120
                    pg.BackColor = Color.White
                    pg.Image = GetImageGirone(gir(i).Clasa, w1)
                    pg.Tag = i
                    pg.Name = Key & "C-" & i
                    pg.Height = pg.Image.Height
                    pg.Width = pg.Image.Width
                    pg.Left = x - w1 \ 2
                    pg.Top = y1
                    pnl1.Controls.Add(pg)
                    y1 = pg.Top + pg.Height + 5
                End If

                Dim pp As New PictureBox
                pp.Width = 230
                pp.Height = 120
                pp.BackColor = Color.White
                pp.Image = GetImagePartiteGirone(True, Key & "P-" & i, x - w1 \ 2, "", gir(i).Partite, w1)
                pp.Tag = i
                pp.Name = Key & "P-" & i
                pp.Height = pp.Image.Height
                pp.Width = pp.Image.Width
                pp.Left = x - w1 \ 2
                pp.Top = y1
                pnl1.Controls.Add(pp)
                AddHandler pp.MouseMove, AddressOf swc
                'p.ContextMenuStrip = mnu1

                If CBool(i Mod 2) Then
                    y = pp.Top + pp.Height + 5
                End If

            Next
        End If

        Return y

    End Function

    Sub swc(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim pn As PictureBox = CType(sender, PictureBox)
        Dim ff As Boolean = False
        picforma.Height = 393
        If pn.Name = "ELIP-0" Then
            For Each key As String In recg1.Keys
                If recg1(key).Contains(e.Location) Then

                    If imgclasa Is Nothing Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                    Dim s() As String = key.Split(CChar("-"))
                    If s.Length = 3 Then
                        ff = True
                        If picforma.Tag Is Nothing OrElse CStr(picforma.Tag) <> key Then
                            picforma.Tag = key
                            Dim ind As Integer = -1
                            Dim gio As Integer = -1
                            If key.Contains("A") Then
                                gio = cup.GironiEliminatori(0).Partite(CInt(s(0))).GiornataAndata
                                If s(1) = "0" Then
                                    ind = cup.GironiEliminatori(0).Partite(CInt(s(0))).TeamId1
                                Else
                                    ind = cup.GironiEliminatori(0).Partite(CInt(s(0))).TeamId2
                                End If
                            Else
                                gio = cup.GironiEliminatori(0).Partite(CInt(s(0))).GiornataRitorno
                                If s(1) = "0" Then
                                    ind = cup.GironiEliminatori(0).Partite(CInt(s(0))).TeamId1
                                Else
                                    ind = cup.GironiEliminatori(0).Partite(CInt(s(0))).TeamId2
                                End If
                            End If
                            'picforma.Visible = False
                            Dim f As New LegaObject.Formazione
                            f.IdTeam = ind
                            f.Giornata = gio
                            f.Load(False)
                            f.Nome = currlega.Teams(ind).Nome
                            f.Allenatore = currlega.Teams(ind).Allenatore

                            picforma.Left = pn.Left + pnl1.Left + e.X + 20
                            If pn.Top + pnl1.Top + e.Y + picforma.Height > Me.Height - 30 Then
                                picforma.Top = Me.Height - picforma.Height - 30
                            Else
                                picforma.Top = pn.Top + pnl1.Top + e.Y - 40
                            End If

                            picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f, "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                            picforma.Visible = True

                        End If
                    End If
                    Exit For
                End If
            Next
        ElseIf pn.Name = "ELIP-1" Then
            For Each key As String In recg2.Keys
                If recg2(key).Contains(e.Location) Then

                    If imgclasa Is Nothing Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                    Dim s() As String = key.Split(CChar("-"))
                    If s.Length = 3 Then
                        ff = True
                        If picforma.Tag Is Nothing OrElse CStr(picforma.Tag) <> key Then
                            picforma.Tag = key
                            Dim ind As Integer = -1
                            Dim gio As Integer = -1
                            If key.Contains("A") Then
                                gio = cup.GironiEliminatori(1).Partite(CInt(s(0))).GiornataAndata
                                If s(1) = "0" Then
                                    ind = cup.GironiEliminatori(1).Partite(CInt(s(0))).TeamId1
                                Else
                                    ind = cup.GironiEliminatori(1).Partite(CInt(s(0))).TeamId2
                                End If
                            Else
                                gio = cup.GironiEliminatori(1).Partite(CInt(s(0))).GiornataRitorno
                                If s(1) = "0" Then
                                    ind = cup.GironiEliminatori(1).Partite(CInt(s(0))).TeamId1
                                Else
                                    ind = cup.GironiEliminatori(1).Partite(CInt(s(0))).TeamId2
                                End If
                            End If
                            'picforma.Visible = False
                            Dim f As New LegaObject.Formazione
                            f.IdTeam = ind
                            f.Giornata = gio
                            f.Load(False)
                            f.Nome = currlega.Teams(ind).Nome
                            f.Allenatore = currlega.Teams(ind).Allenatore

                            picforma.Left = pn.Left + pnl1.Left + e.X - picforma.Width - 20
                            If pn.Top + pnl1.Top + e.Y + picforma.Height > Me.Height - 30 Then
                                picforma.Top = Me.Height - picforma.Height - 30
                            Else
                                picforma.Top = pn.Top + pnl1.Top + e.Y - 40
                            End If

                            picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f, "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                            picforma.Visible = True

                        End If
                    End If
                    Exit For
                End If
            Next
        ElseIf pn.Name = "POP-0" Then
            For Each key As String In rep1.Keys
                If rep1(key).Contains(e.Location) Then

                    If imgclasa Is Nothing Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                    Dim s() As String = key.Split(CChar("-"))
                    If s.Length = 3 Then
                        ff = True
                        If picforma.Tag Is Nothing OrElse CStr(picforma.Tag) <> key Then
                            picforma.Tag = key
                            Dim ind As Integer = -1
                            Dim gio As Integer = -1
                            gio = cup.PlayOff(0).Partite(CInt(s(0))).GiornataAndata
                            If s(1) = "0" Then
                                ind = cup.PlayOff(0).Partite(CInt(s(0))).TeamId1
                            Else
                                ind = cup.PlayOff(0).Partite(CInt(s(0))).TeamId2
                            End If
                            'picforma.Visible = False
                            Dim f As New LegaObject.Formazione
                            f.IdTeam = ind
                            f.Giornata = gio
                            f.Load(False)
                            f.Nome = currlega.Teams(ind).Nome
                            f.Allenatore = currlega.Teams(ind).Allenatore

                            picforma.Left = pn.Left + pnl1.Left + e.X - picforma.Width - 20
                            If pn.Top + pnl1.Top + e.Y + picforma.Height > Me.Height - 30 Then
                                picforma.Top = Me.Height - picforma.Height - 30
                            Else
                                picforma.Top = pn.Top + pnl1.Top + e.Y - 40
                            End If

                            picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f, "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                            picforma.Visible = True

                        End If
                    End If
                    Exit For
                End If
            Next
        ElseIf pn.Name = "POP-1" Then
            For Each key As String In rep2.Keys
                If rep2(key).Contains(e.Location) Then

                    If imgclasa Is Nothing Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                    Dim s() As String = key.Split(CChar("-"))
                    If s.Length = 3 Then
                        ff = True
                        If picforma.Tag Is Nothing OrElse CStr(picforma.Tag) <> key Then
                            picforma.Tag = key
                            Dim ind As Integer = -1
                            Dim gio As Integer = -1
                            gio = cup.PlayOff(1).Partite(CInt(s(0))).GiornataAndata
                            If s(1) = "0" Then
                                ind = cup.PlayOff(1).Partite(CInt(s(0))).TeamId1
                            Else
                                ind = cup.PlayOff(1).Partite(CInt(s(0))).TeamId2
                            End If
                            If ind <> -1 Then
                                Dim f As New LegaObject.Formazione
                                f.IdTeam = ind
                                f.Giornata = gio
                                f.Load(False)
                                f.Nome = currlega.Teams(ind).Nome
                                f.Allenatore = currlega.Teams(ind).Allenatore

                                picforma.Left = pn.Left + pnl1.Left + e.X - picforma.Width - 20
                                If pn.Top + pnl1.Top + e.Y + picforma.Height > Me.Height - 30 Then
                                    picforma.Top = Me.Height - picforma.Height - 30
                                Else
                                    picforma.Top = pn.Top + pnl1.Top + e.Y - 40
                                End If

                                picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f, "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                                picforma.Visible = True
                            End If
                        End If
                    End If
                    Exit For
                End If
            Next
        ElseIf pn.Name = "QFP-1" Then
            For Each key As String In reqf.Keys
                If reqf(key).Contains(e.Location) Then

                    If imgclasa Is Nothing Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                    Dim s() As String = key.Split(CChar("-"))
                    If s.Length = 3 Then
                        ff = True
                        If picforma.Tag Is Nothing OrElse CStr(picforma.Tag) <> key Then
                            picforma.Tag = key
                            Dim ind As Integer = -1
                            Dim gio As Integer = -1
                            If key.EndsWith("R") Then
                                gio = cup.QuartiDiFinale(CInt(s(0))).GiornataRitorno
                            Else
                                gio = cup.QuartiDiFinale(CInt(s(0))).GiornataAndata
                            End If
                            If s(1) = "0" Then
                                ind = cup.QuartiDiFinale(CInt(s(0))).TeamId1
                            Else
                                ind = cup.QuartiDiFinale(CInt(s(0))).TeamId2
                            End If
                            If ind <> -1 Then
                                Dim f As New LegaObject.Formazione
                                f.IdTeam = ind
                                f.Giornata = gio
                                f.Load(False)
                                f.Nome = currlega.Teams(ind).Nome
                                f.Allenatore = currlega.Teams(ind).Allenatore

                                picforma.Left = pn.Left + pnl1.Left + e.X - picforma.Width - 20
                                If pn.Top + pnl1.Top + e.Y + picforma.Height > Me.Height - 30 Then
                                    picforma.Top = Me.Height - picforma.Height - 30
                                Else
                                    picforma.Top = pn.Top + pnl1.Top + e.Y - 40
                                End If

                                picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f, "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                                picforma.Visible = True
                            End If
                        End If
                    End If
                    Exit For
                End If
            Next
        ElseIf pn.Name = "SFP-1" Then
            For Each key As String In resf.Keys
                If resf(key).Contains(e.Location) Then

                    If imgclasa Is Nothing Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                    Dim s() As String = key.Split(CChar("-"))
                    If s.Length = 3 Then
                        ff = True
                        If picforma.Tag Is Nothing OrElse CStr(picforma.Tag) <> key Then
                            picforma.Tag = key
                            Dim ind As Integer = -1
                            Dim gio As Integer = -1
                            If key.EndsWith("R") Then
                                gio = cup.SemiFinali(CInt(s(0))).GiornataRitorno
                            Else
                                gio = cup.SemiFinali(CInt(s(0))).GiornataAndata
                            End If
                            If s(1) = "0" Then
                                ind = cup.SemiFinali(CInt(s(0))).TeamId1
                            Else
                                ind = cup.SemiFinali(CInt(s(0))).TeamId2
                            End If
                            If ind <> -1 Then
                                Dim f As New LegaObject.Formazione
                                f.IdTeam = ind
                                f.Giornata = gio
                                f.Load(False)
                                f.Nome = currlega.Teams(ind).Nome
                                f.Allenatore = currlega.Teams(ind).Allenatore

                                picforma.Left = pn.Left + pnl1.Left + e.X - picforma.Width - 20
                                If pn.Top + pnl1.Top + e.Y + picforma.Height > Me.Height - 30 Then
                                    picforma.Top = Me.Height - picforma.Height - 30
                                Else
                                    picforma.Top = pn.Top + pnl1.Top + e.Y - 40
                                End If

                                picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f, "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                                picforma.Visible = True
                            End If
                        End If
                    End If
                    Exit For
                End If
            Next
        ElseIf pn.Name = "FIP-1" Then
            For Each key As String In refi.Keys
                If refi(key).Contains(e.Location) Then

                    If imgclasa Is Nothing Then imgclasa = SystemFunction.DrawingAndImage.GetImageAreaForm(Me, New Rectangle(0, 0, Me.Width, Me.Height))

                    Dim s() As String = key.Split(CChar("-"))
                    If s.Length = 3 Then
                        ff = True
                        If picforma.Tag Is Nothing OrElse CStr(picforma.Tag) <> key Then
                            picforma.Tag = key
                            Dim ind As Integer = -1
                            Dim gio As Integer = -1
                            gio = cup.Finale(CInt(s(0))).GiornataAndata
                            If s(1) = "0" Then
                                ind = cup.Finale(CInt(s(0))).TeamId1
                            Else
                                ind = cup.Finale(CInt(s(0))).TeamId2
                            End If
                            If ind <> -1 Then
                                Dim f As New LegaObject.Formazione
                                f.IdTeam = ind
                                f.Giornata = gio
                                f.Load(False)
                                f.Nome = currlega.Teams(ind).Nome
                                f.Allenatore = currlega.Teams(ind).Allenatore

                                picforma.Left = pn.Left + pnl1.Left + e.X - picforma.Width - 20
                                If pn.Top + pnl1.Top + e.Y + picforma.Height > Me.Height - 30 Then
                                    picforma.Top = Me.Height - picforma.Height - 30
                                Else
                                    picforma.Top = pn.Top + pnl1.Top + e.Y - 40
                                End If

                                picforma.Image = SystemFunction.DrawingAndImage.GetImageForma(f, "", True, picforma.Width, picforma.Height, iControl.CommonFunction.GetAreaImage(imgclasa, picforma.Left, picforma.Top, picforma.Width, picforma.Height))

                                picforma.Visible = True
                            End If
                        End If
                    End If
                    Exit For
                End If
            Next
        End If
        If ff = False Then
            picforma.Tag = ""
            picforma.Visible = False
        End If
    End Sub

    Function DrawPartite(ByVal y As Integer, ByVal x As Integer, ByVal w As Integer, ByVal w1 As Integer, ByVal par As List(Of LegaObject.Coppa.Girone.PartitaGirone), ByVal Title As String, ByVal Key As String) As Integer

        If par.Count > 0 Then

            Dim y1 As Integer = 0

            Dim pt As New PictureBox
            pt.Width = w1
            pt.Height = 40
            pt.Name = Key & "T-1"
            pt.Image = GetTitle(Title, pt.Width, pt.Height)
            pt.Top = y
            pt.Left = x - w1 \ 2
            y1 = pt.Top + pt.Height + 5
            pnl1.Controls.Add(pt)

            Dim pp As New PictureBox
            pp.Width = 230
            pp.Height = 120
            pp.BackColor = Color.White
            pp.Image = GetImagePartiteGirone(False, Key & "P-1", x - w1 \ 2, "", par, w1)
            pp.Tag = 1
            pp.Name = Key & "P-1"
            pp.Height = pp.Image.Height
            pp.Width = pp.Image.Width
            pp.Left = x - w1 \ 2
            pp.Top = y1
            pnl1.Controls.Add(pp)
            AddHandler pp.MouseMove, AddressOf swc
            'p.ContextMenuStrip = mnu1

            y = pp.Top + pp.Height + 5

        End If

        Return y

    End Function

    Function GetTitle(ByVal Title As String, ByVal w As Integer, ByVal h As Integer) As Bitmap

        Dim gr As Graphics
        Dim b1 As New Bitmap(w, h)

        Try

            Dim f1 As New StringFormat
            Dim ft As New Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel)
            Dim br As Brush = New SolidBrush(Color.FromArgb(10, 50, 50, 50))
            Dim s As New SizeF(0, 0)

            f1.Alignment = StringAlignment.Center

            gr = Graphics.FromImage(b1)
            gr.Clear(Color.White)
            s = gr.MeasureString(Title, ft, New Size(0, 0), f1)
            s.Width = s.Width + 20
            gr.DrawLine(New Pen(Color.FromArgb(20, 50, 50, 50), 4), 10, CLng(s.Height) \ 2, w - 10, CLng(s.Height) \ 2)
            gr.DrawLine(New Pen(Color.Red, 2), 10, CLng(s.Height) \ 2, w - 10, CLng(s.Height) \ 2)
            gr.FillRectangle(Brushes.White, New Rectangle(w \ 2 - CInt(s.Width) \ 2, 0, CInt(s.Width), h))
            gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

            For x As Integer = -1 To 1
                For y As Integer = -1 To 1
                    gr.DrawString(Title, ft, br, New RectangleF(x, y, w, h), f1)
                Next
            Next

            gr.DrawString(Title, ft, New LinearGradientBrush(New RectangleF(0, 0, w, h), Color.OrangeRed, Color.Red, LinearGradientMode.Vertical), New RectangleF(0, 0, w, h), f1)

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return CType(b1.Clone, Bitmap)

    End Function

    Function GetImageGirone(ByVal Girone As List(Of LegaObject.Coppa.Girone.ClasaGirone), ByVal w As Integer) As Bitmap

        Dim gr As Graphics
        Dim b1 As Bitmap
        Dim dy As Integer = 18
        Dim dx As Integer = 160
        Dim rh As Integer = 19
        Dim nr As Integer = Girone.Count
        Dim g As New List(Of LegaObject.Coppa.Girone.ClasaGirone)

        'Dim farms As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & f.IdTeam & "-24x24.png"
        Dim ft As New Font("Tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim r As Integer = -1
        Dim hb As Integer = 0
        Dim h As Integer = rh * nr + dy * 2
        Dim pd As Integer = 10
        Dim dt As Integer = 0
        Dim dx1 As Integer = (w - dx - pd * 2) \ 7
        Dim f1 As New StringFormat
        Dim f2 As New StringFormat
        Dim f3 As New StringFormat

        f1.Alignment = StringAlignment.Center
        f2.Alignment = StringAlignment.Far
        f3.Alignment = StringAlignment.Near

        b1 = New Bitmap(w, h)

        Dim a() As LegaObject.Coppa.Girone.ClasaGirone = Girone.ToArray
        Dim s As New LegaObject.Coppa.Girone.ClasaSorter("pt", True)
        Array.Sort(a, s)
        g.Clear()
        g.AddRange(a)

        Try
            gr = Graphics.FromImage(b1)
            gr.Clear(Color.White)
            gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

            gr.DrawString("Squadra", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), 29 + pd, dy - 15, f1)
            gr.DrawString("PG", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), New Rectangle(dx, dy - 15, dx1, 15), f1)
            gr.DrawString("V", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), New Rectangle(dx + dx1, dy - 15, dx1, 15), f1)
            gr.DrawString("P", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), New Rectangle(dx + dx1 * 2, dy - 15, dx1, 15), f1)
            gr.DrawString("S", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), New Rectangle(dx + dx1 * 3, dy - 15, dx1, 15), f1)
            gr.DrawString("GF", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), New Rectangle(dx + dx1 * 4, dy - 15, dx1, 15), f1)
            gr.DrawString("GS", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), New Rectangle(dx + dx1 * 5, dy - 15, dx1, 15), f1)
            gr.DrawString("Pt", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), New Rectangle(dx + dx1 * 6, dy - 15, dx1, 15), f1)

            Dim ptmax As Integer = 0
            Dim ptmin As Integer = 1000

            For i As Integer = 0 To nr - 1
                If g(i).Pt < ptmin Then ptmin = g(i).Pt
                If g(i).Pt > ptmax Then ptmax = g(i).Pt
            Next

            For i As Integer = 0 To nr - 1

                If i = 0 Then gr.FillRectangle(New SolidBrush(Color.FromArgb(255, 255, 180)), New Rectangle(5, dy + 2, w - 20, 18))
                Dim farms As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & g(i).TeamId & "-16x16.png"
                If IO.File.Exists(farms) Then
                    gr.DrawImage(Image.FromFile(farms), pd + 2, i * rh + dy + 2)
                End If
                Dim n As String = ""
                If g(i).TeamId <> -1 Then
                    n = currlega.Teams(g(i).TeamId).Nome
                End If

                If n.Length > 15 Then n = n.Substring(0, 15) & "."

                gr.DrawString(n, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), 22 + pd, i * rh + dy + 3)
                gr.DrawString(CStr(g(i).PartiteGiocate), New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(dx, i * rh + dy + 3, dx1, 15), f1)
                gr.DrawString(CStr(g(i).Vittorie), New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(dx + dx1, i * rh + dy + 3, dx1, 15), f1)
                gr.DrawString(CStr(g(i).Pareggi), New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(dx + dx1 * 2, i * rh + dy + 3, dx1, 15), f1)
                gr.DrawString(CStr(g(i).Scofitte), New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(dx + dx1 * 3, i * rh + dy + 3, dx1, 15), f1)
                gr.DrawString(CStr(g(i).GoalFatti), New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(dx + dx1 * 4, i * rh + dy + 3, dx1, 15), f1)
                gr.DrawString(CStr(g(i).GoalSubiti), New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(dx + dx1 * 5, i * rh + dy + 3, dx1, 15), f1)
                gr.DrawString(CStr(g(i).Pt), New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.RoyalBlue), New Rectangle(dx + dx1 * 6, i * rh + dy + 3, dx1, 15), f1)

            Next

            gr.SmoothingMode = Drawing2D.SmoothingMode.Default

            For i As Integer = 0 To nr
                gr.DrawLine(New Pen(Color.FromArgb(50, 0, 0, 0), 1), pd, i * rh + dy, dx + dx1 * 7 + pd, i * rh + dy)
            Next

            For i As Integer = 0 To 7
                gr.DrawLine(New Pen(Color.FromArgb(50, 0, 0, 0), 1), i * dx1 + 160, dy - 10, i * dx1 + 160, h - 10)
            Next

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return CType(b1.Clone, Bitmap)

    End Function

    Function GetImagePartiteGirone(ByVal Girone As Boolean, ByVal Key As String, ByVal DxImage As Integer, ByVal PrefixMatch As String, ByVal PartitaGirone As List(Of LegaObject.Coppa.Girone.PartitaGirone), ByVal w As Integer) As Bitmap

        Dim gr As Graphics
        Dim b1 As Bitmap
        Dim dy As Integer = 20
        Dim dx As Integer = 130
        Dim rh As Integer = 14

        'Dim farms As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & f.IdTeam & "-24x24.png"
        Dim ft As New Font("Tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
        Dim r As Integer = -1
        Dim hb As Integer = 0
        Dim h As Integer = 0
        Dim pd As Integer = 10
        Dim dt As Integer = 0
        Dim numpt As Integer = 24
        Dim dxpt As Integer = (w - pd * 2 - dx) \ numpt

        For i As Integer = 0 To PartitaGirone.Count - 1
            If PartitaGirone(i).Index = 0 Then
                h = h + rh + 5
            End If
            h = h + rh
        Next
        h = h + rh * 2
        If h < 10 Then h = 10

        Dim f1 As New StringFormat
        Dim f2 As New StringFormat
        Dim f3 As New StringFormat

        f1.Alignment = StringAlignment.Center
        f2.Alignment = StringAlignment.Far
        f3.Alignment = StringAlignment.Near

        b1 = New Bitmap(w, h)

        If Key = "ELIP-0" Then
            recg1.Clear()
        ElseIf Key = "ELIP-1" Then
            recg2.Clear()
        ElseIf Key = "POP-0" Then
            rep1.Clear()
        ElseIf Key = "POP-1" Then
            rep2.Clear()
        ElseIf Key = "QFP-1" Then
            reqf.Clear()
        ElseIf Key = "SFP-1" Then
            resf.Clear()
        ElseIf Key = "FIP-1" Then
            refi.Clear()
        End If

        Try

            gr = Graphics.FromImage(b1)
            gr.Clear(Color.White)
            gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

            gr.DrawImage(SystemFunction.DrawingAndImage.GetBackgroundImage(w, h), 0, 0)

            Dim y As Integer = 0

            For i As Integer = 0 To PartitaGirone.Count - 1
                If PartitaGirone(i).Index = 0 Then
                    If i > 0 Then y = y + 5
                    Dim pref As String = PartitaGirone(i).Giornata & "° Giornata"
                    If Girone = False Then
                        pref = PrefixMatch
                    End If
                    If PartitaGirone(i).GiornataRitorno <> -1 Then
                        If Girone Then
                            gr.DrawString(pref & " (" & PartitaGirone(i).GiornataAndata & "° Gio./" & PartitaGirone(i).GiornataRitorno & "° Gio.)", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), pd, y)
                        Else
                            gr.DrawString(pref & " (Andata " & PartitaGirone(i).GiornataAndata & "° Gio./ Ritorno " & PartitaGirone(i).GiornataRitorno & "° Gio.)", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), pd, y)
                        End If
                        gr.DrawString("And.", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), w - 60, y, f1)
                        gr.DrawString("|", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), w - 45, y, f1)
                        gr.DrawString("Rit.", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), w - 30, y, f1)
                    Else
                        gr.DrawString(pref & " (" & PartitaGirone(i).GiornataAndata & "°)", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), pd, y)
                        gr.DrawString("Ris.", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.Red), w - 30, y, f1)
                    End If
                    y = y + rh
                End If
                Dim n1 As String = ""
                Dim n2 As String = ""
                Dim str As String = ""

                If PartitaGirone(i).TeamId1 <> -1 Then
                    n1 = currlega.Teams(PartitaGirone(i).TeamId1).Nome
                End If
                If PartitaGirone(i).TeamId2 <> -1 Then
                    n2 = currlega.Teams(PartitaGirone(i).TeamId2).Nome
                End If

                If n1.Length > 15 Then n1 = n1.Substring(0, 14)
                If n2.Length > 15 Then n2 = n2.Substring(0, 14)

                str = n1 & " - " & n2

                Dim x11 As Integer = pd
                Dim x12 As Integer = CInt(gr.MeasureString(n1, New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel)).Width + 5)


                Dim r1a As New Rectangle(w - 73, y, 13, rh)
                Dim r2a As New Rectangle(w - 60, y, 13, rh)
                Dim r1r As New Rectangle(w - 43, y, 13, rh)
                Dim r2r As New Rectangle(w - 30, y, 13, rh)

                If Key = "ELIP-0" Then
                    recg1.Add(i & "-0-A", r1a)
                    recg1.Add(i & "-1-A", r2a)
                    recg1.Add(i & "-0-R", r1r)
                    recg1.Add(i & "-1-R", r2r)
                ElseIf Key = "ELIP-1" Then
                    recg2.Add(i & "-0-A", r1a)
                    recg2.Add(i & "-1-A", r2a)
                    recg2.Add(i & "-0-R", r1r)
                    recg2.Add(i & "-1-R", r2r)
                ElseIf Key = "POP-0" Then
                    rep1.Add(i & "-0-R", r1r)
                    rep1.Add(i & "-1-R", r2r)
                ElseIf Key = "POP-1" Then
                    rep2.Clear()
                    rep2.Add(i & "-0-R", r1r)
                    rep2.Add(i & "-1-R", r2r)
                ElseIf Key = "QFP-1" Then
                    reqf.Add(i & "-0-A", r1a)
                    reqf.Add(i & "-1-A", r2a)
                    reqf.Add(i & "-0-R", r1r)
                    reqf.Add(i & "-1-R", r2r)
                ElseIf Key = "SFP-1" Then
                    resf.Add(i & "-0-A", r1a)
                    resf.Add(i & "-1-A", r2a)
                    resf.Add(i & "-0-R", r1r)
                    resf.Add(i & "-1-R", r2r)
                ElseIf Key = "FIP-1" Then
                    refi.Add(i & "-0-A", r1a)
                    refi.Add(i & "-1-A", r2a)
                    refi.Add(i & "-0-R", r1r)
                    refi.Add(i & "-1-R", r2r)
                End If

                gr.DrawString(str, New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), pd, y)

                If PartitaGirone(i).GiornataRitorno <> -1 Then
                    gr.DrawString("-", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 60, y, f1)
                    gr.DrawString(cup.GetGoalString(PartitaGirone(i).GoalAnd1), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 63, y, f2)
                    gr.DrawString(cup.GetGoalString(PartitaGirone(i).GoalAnd2), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 57, y, f3)

                    gr.DrawString("|", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 45, y, f1)

                    gr.DrawString("-", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 30, y, f1)
                    gr.DrawString(cup.GetGoalString(PartitaGirone(i).GoalRit1), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 33, y, f2)
                    gr.DrawString(cup.GetGoalString(PartitaGirone(i).GoalRit2), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 27, y, f3)
                Else
                    gr.DrawString("-", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 30, y, f1)
                    gr.DrawString(cup.GetGoalString(PartitaGirone(i).GoalAnd1), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 33, y, f2)
                    gr.DrawString(cup.GetGoalString(PartitaGirone(i).GoalAnd2), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), w - 27, y, f3)
                End If
                y = y + rh
            Next

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return CType(b1.Clone, Bitmap)

    End Function

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
                Catch
                End Try
            End If
        Else
            Me.Text = Me.Text
        End If
        Return False
    End Function

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try
            If keyData = Keys.PageUp Then
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
            ElseIf keyData = (Keys.Control + Keys.E) Then
                Call Action(0)
            ElseIf keyData = (Keys.Control + Keys.C) Then
                Call Action(3)
            ElseIf keyData = (Keys.Control + Keys.S) Then
                Call Action(4)
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub pnl1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pnl1.MouseMove
        picforma.Visible = False
    End Sub

    Private Sub mnu1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnu1.ItemClicked
        Call Action(CInt(e.ClickedItem.Tag))
    End Sub

End Class
