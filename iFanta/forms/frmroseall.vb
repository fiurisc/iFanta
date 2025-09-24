Public Class frmroseall

    Implements IMessageFilter

    Dim p As New List(Of PictureBox)
    Dim t As New List(Of iControl.iToolBar)
    Dim l As New List(Of iControl.iLabel)
    Dim start As Boolean = True
    Dim currrosa As Integer = -1

    Private Sub froroseall_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.KeyPreview = True

        Application.AddMessageFilter(Me)

        IForm1.WindowsTitle = My.Application.Info.ProductName

        lbby.Text = My.Application.Info.Copyright
        txtsearch.Text = ""

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        'Setto il tema corrente'
        Call SetTheme()

        start = False

        Me.Width = My.Computer.Screen.WorkingArea.Width - 250
        Me.Height = My.Computer.Screen.WorkingArea.Height - 150
        Me.Top = My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2
        Me.Left = My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2
        mnu1.ImageScalingSize = New Size(16, 16)

        Timer1.Enabled = True

    End Sub

    Private Sub frm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Try
            If e.KeyData = Keys.PageUp Then
                Dim v As Integer = pnl1.VerticalScroll.Value
                v = v - pnl1.VerticalScroll.LargeChange
                If v < 0 Then v = 0
                pnl1.VerticalScroll.Value = v
                pnl1.PerformLayout()
            ElseIf e.KeyData = Keys.PageDown Then
                Dim v As Integer = pnl1.VerticalScroll.Value
                v = v + pnl1.VerticalScroll.LargeChange
                If v > pnl1.VerticalScroll.Maximum Then v = pnl1.VerticalScroll.Maximum
                pnl1.VerticalScroll.Value = v
                pnl1.PerformLayout()
            ElseIf (e.KeyData = (Keys.Control Or Keys.Delete)) Then
                currlega.DeleteTeamData()
                Call SetTeamName()
                Call DrawTeam()
            ElseIf e.KeyData = Keys.Scroll Then
                Dim v As Integer = pnl1.VerticalScroll.Value
            End If
        Catch ex As Exception

        End Try
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
                Catch
                End Try
            End If
        End If
        Return False
    End Function

    Overrides Sub ResetTorneo()
        SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)
        currlega.LoadTeams(True, True)
        Call DrawTeam()
    End Sub

    Sub SetTeamName()
        Try
            'Imposto l'elenco delle squadre'
            txtsearch.AutoCompleteList.Clear()
            txtsearch.AutoCompleteList.AddRange(currlega.GetLegaPlayerList)
        Catch ex As Exception

        End Try
    End Sub

    Sub SetTheme()

        Try

            SystemFunction.Gui.SetTorneoLabel(lbtorneo1, lbtorneo2, IForm1)

            lb1.Left = padd
            lb1.Top = padd \ 2 + IForm1.TY + 6

            txtsearch.Left = IForm1.RX - padd - txtsearch.Width
            txtsearch.Top = padd \ 2 + IForm1.TY + 6
            txtsearch.FlatStyle = AppSett.Personal.Theme.FlatStyle

            tlbsearch.Left = txtsearch.Left - tlbsearch.Width - 2
            tlbsearch.Top = txtsearch.Top + 1

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

            tlbaction.Left = IForm1.RX - tlbaction.Width - padd
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            For i As Integer = 0 To tlbaction.Button.Count - 1
                tlbaction.Button(i).BorderColor = Color.DimGray
            Next

            lnbot.Top = tlbaction.Top - lnbot.Height - 5
            lnbot.Left = padd
            lnbot.Width = IForm1.RX - IForm1.LX - padd

            pnl1.Top = txtsearch.Top + txtsearch.Height + 5
            pnl1.Left = padd
            pnl1.Width = IForm1.RX - IForm1.LX - padd * 2 + 4
            pnl1.Height = lnbot.Top - pnl1.Top - 5

        Catch ex As Exception

        End Try

    End Sub

    Sub SetControlPosition()

        If p.Count = 0 Then Exit Sub

        Try
            Dim x As Integer = 0
            Dim y As Integer = 10

            For i As Integer = 0 To currlega.Teams.Count - 1
                If CInt(p(i).Tag) = 1 Then
                    p(i).Left = x
                    p(i).Top = y - pnl1.VerticalScroll.Value
                    l(i).Left = x + 10
                    l(i).Top = p(i).Top + p(i).Height - 18
                    l(i).Height = 18
                    l(i).BringToFront()
                    t(i).Left = p(i).Left + p(i).Width - 67 - 6
                    t(i).Top = p(i).Top + p(i).Height - 18
                    t(i).Height = 16
                    t(i).BringToFront()
                    x = x + p(i).Width + 10
                    If x > pnl1.Width - 20 - p(i).Width Then
                        y = y + p(i).Height + 10
                        x = 0
                    End If
                    p(i).Visible = True
                    t(i).Visible = True
                    l(i).Visible = currlega.Settings.EnableTraceReconfirmations
                Else
                    p(i).Visible = False
                    t(i).Visible = False
                End If
            Next
        Catch ex As Exception

        End Try

    End Sub

    Function GetImage(ByVal t As LegaObject.Team, ByVal W As Integer, ByVal H As Integer) As Bitmap

        Dim gr As Graphics
        Dim b1 As Bitmap
        Dim dy As Integer = 60
        Dim rh As Integer = 14
        Dim str As String = txtsearch.Text
        Dim farms As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & t.IdTeam & "-24x24.png"
        Dim farmsl As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & t.IdTeam & "-200x200.jpg"
        Dim ft As New Font("Arial", 11, FontStyle.Regular,GraphicsUnit.Pixel)

        b1 = New Bitmap(W, H)

        Try
            gr = Graphics.FromImage(b1)
            gr.Clear(Color.White)
            gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

            Dim br2 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H \ 2 - 1), Color.White, Color.FromArgb(255, 255, 255), Drawing2D.LinearGradientMode.Vertical)

            For i As Integer = 0 To 2
                Dim br1 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H \ 2 - 1), Color.White, Color.FromArgb(30 + i * 10, 0, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                gr.FillPath(br1, SystemFunction.DrawingAndImage.GetBorderDullPath(gr, New Rectangle(i, i, W - i * 2, H - i * 2 - 20), 16 - i * 2))
            Next

            gr.FillPath(Brushes.White, SystemFunction.DrawingAndImage.GetBorderDullPath(gr, New Rectangle(3, 3, W - 6, H - 28), 10))
            gr.FillPath(br2, SystemFunction.DrawingAndImage.GetBorderDullPath(gr, New Rectangle(4, 4, W - 8, H - 30), 8))

            gr.FillRectangle(Brushes.White, New Rectangle(0, 0, W, H \ 2))

            If IO.File.Exists(farms) Then
                gr.DrawImage(Image.FromFile(farms), 3, 3)
            End If
            If IO.File.Exists(farmsl) Then
                gr.DrawImage(SystemFunction.DrawingAndImage.ConvDisable(Image.FromFile(farmsl), 0.15), W - 145, H - 170, 140, 140)
            End If

            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            gr.DrawString(t.Nome, New Font("Arial", 15, FontStyle.Bold,GraphicsUnit.Pixel), Brushes.Red, 28, 2)
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.SystemDefault
            gr.DrawString(t.Allenatore, New Font("Arial", 11, FontStyle.Regular,GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(50, 50, 50)), 28, 18)
            gr.DrawLine(Pens.Silver, 8, 33, W - 10, 33)

            Dim f1 As New StringFormat
            Dim f2 As New StringFormat
            Dim f3 As New StringFormat

            f1.Alignment = StringAlignment.Center
            f2.Alignment = StringAlignment.Far
            f3.FormatFlags = StringFormatFlags.NoWrap

            Dim dxr As Integer = 0
            If currlega.Settings.EnableTraceReconfirmations Then
                gr.DrawString("C", ft, Brushes.Red, 17, dy - 20, f1)
                dxr = 15
            End If
            gr.DrawString("R", ft, Brushes.Red, 15 + dxr, dy - 20, f1)
            gr.DrawString("Nome", ft, Brushes.Red, New Rectangle(45 + dxr, dy - 20, 70, 20))
            gr.DrawString("Squadra", ft, Brushes.Red, 140 + dxr, dy - 20)
            gr.DrawString("Cos.", ft, Brushes.Red, W - 81, dy - 20, f2)
            gr.DrawString("QI", ft, Brushes.Red, W - 58, dy - 20, f2)
            gr.DrawString("QA", ft, Brushes.Red, W - 35, dy - 20, f2)
            gr.DrawString("±", ft, Brushes.Red, W - 15, dy - 20, f2)

            Dim cfd As Color = Color.Black

            For i As Integer = 0 To t.Players.Count - 1

                Dim rfc As Color = SystemFunction.General.GetRuoloForeColor(t.Players(i).Ruolo)
                Dim nfc As Color = Color.FromArgb(80, 80, 80)
                Dim cfc As Color = Color.RoyalBlue

                If str <> "" AndAlso t.Players(i).Nome = str Then
                    rfc = Color.White
                    nfc = Color.White
                    cfc = Color.White
                    Dim br As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W - 15, rh - 1), iControl.CommonFunction.GetSelectionColor1, iControl.CommonFunction.GetSelectionColor2, Drawing2D.LinearGradientMode.Vertical)
                    gr.FillRectangle(br, New Rectangle(10, dy + i * rh, W - 25, rh))
                End If

                Dim s As String = t.Players(i).Squadra
                If currlega.Settings.EnableTraceReconfirmations Then
                    If t.Players(i).Riconfermato = 1 Then
                        gr.DrawImage(My.Resources.star10r, 10, dy + i * rh + 1)
                    End If
                End If
                gr.DrawString(t.Players(i).Ruolo, ft, New SolidBrush(rfc), 15 + dxr, dy + i * rh - 1, f1)
                gr.DrawImage(SystemFunction.DrawingAndImage.GetImageNationFlags(t.Players(i).NatCode), 25 + dxr, dy + i * rh + 1)
                'Dim key As String = t.Players(i).Nome & "-" & t.Players(i).Squadra
                'If webdata.WebOtherPlayersData.ContainsKey(key) Then
                '    Dim nat As String = webdata.WebOtherPlayersData(key).NationCode.Trim
                '    If imgnatcode.ContainsKey(nat) Then
                '        gr.DrawImage(imgnatcode(nat), 25 + dxr, dy + i * rh + 1)
                '    End If
                'End If
                gr.DrawString(t.Players(i).Nome, ft, New SolidBrush(nfc), New Rectangle(45 + dxr, dy + i * rh - 1, 70, 20), f3)
                gr.DrawString(s, ft, New SolidBrush(nfc), 140 + dxr, dy + i * rh - 1)
                gr.DrawString(CStr(t.Players(i).Costo), ft, New SolidBrush(cfc), W - 81, dy + i * rh - 1, f2)
                gr.DrawString(CStr(t.Players(i).QIni), ft, New SolidBrush(nfc), W - 58, dy + i * rh - 1, f2)
                gr.DrawString(CStr(t.Players(i).QCur), ft, New SolidBrush(nfc), W - 35, dy + i * rh - 1, f2)
                Dim diff As Integer = t.Players(i).QCur - t.Players(i).QIni
                Select Case diff
                    Case Is < 0 : cfd = Color.Red
                    Case Is > 0 : cfd = cfc
                    Case Else : cfd = nfc
                End Select
                gr.DrawString(CStr(diff), ft, New SolidBrush(cfd), W - 15, dy + i * rh - 1, f2)
                gr.DrawLine(Pens.Silver, 10, dy + (i + 1) * rh - 1, W - 15, dy + (i + 1) * rh - 1)
            Next

            gr.DrawString(CStr(t.CostoTot), ft, Brushes.RoyalBlue, W - 81, dy + rh * 25 + 4, f2)
            gr.DrawString(CStr(t.QiniTot), ft, New SolidBrush(Color.FromArgb(80, 80, 80)), W - 58, dy + rh * 25 + 4, f2)
            gr.DrawString(CStr(t.QcurTot), ft, New SolidBrush(Color.FromArgb(80, 80, 80)), W - 35, dy + rh * 25 + 4, f2)

            Dim diff1 As Integer = t.QcurTot - t.QiniTot

            Select Case diff1
                Case Is < 0 : cfd = Color.Red
                Case Is > 0 : cfd = Color.RoyalBlue
                Case Else : cfd = Color.FromArgb(80, 80, 80)
            End Select

            gr.DrawString(CStr(diff1), ft, New SolidBrush(cfd), W - 15, dy + rh * 25 + 4, f2)

            gr.Dispose()

        Catch ex As Exception

        End Try

        Return CType(b1.Clone, Bitmap)

    End Function

    Private Sub froroseall_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If start Then Exit Sub
        Call SetControlPosition()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        Timer1.Enabled = False

        Try

            'Carico i dati generali della squadra'
            currlega.LoadTeams(True)

            Call SetTeamName()

            'Configuro le varie rose'
            For i As Integer = 0 To currlega.Teams.Count - 1
                'Carico la rosa se necessario'
                'Aggiungo la picturebox'
                p.Add(New PictureBox)
                p(i).Width = 322
                p(i).Height = 461
                p(i).BackColor = Color.White
                p(i).Tag = 1
                p(i).Name = "TEAM-" & i
                p(i).ContextMenuStrip = mnu1
                p(i).Visible = False
                'Associo l'evento button click alla funzione'
                AddHandler p(i).MouseEnter, AddressOf RosaMouseEnter
                'Aggiungo l'etichetta di help'
                l.Add(New iControl.iLabel)
                l(i).Image = My.Resources.star10r
                l(i).Text = "Riconferma"
                l(i).Font = New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel)
                'Aggiungo la toolbar'
                t.Add(New iControl.iToolBar)
                t(i).AutoSize = True
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(0).Image = My.Resources.edit_black_14
                t(i).Button(0).ToolTips = "Modifica rosa"
                t(i).Button(0).Tag = "0"
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(1).Image = My.Resources.import_black_14
                t(i).Button(1).ToolTips = "Importa rosa"
                t(i).Button(1).Tag = "10"
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(2).Image = My.Resources.export_black_14
                t(i).Button(2).ToolTips = "Esporta rosa"
                t(i).Button(2).Tag = "11"
                t(i).Button.Add(New iControl.ToolbarButton(""))
                t(i).Button(3).Image = My.Resources.del_black_14
                t(i).Button(3).ToolTips = "Elimina rosa"
                t(i).Button(3).Tag = "9"
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

            'Determino le immagini dei vari team'
            Call DrawTeam()

            'Posiziono le varie rose sullo schermo'
            Call SetControlPosition()

            lbstartup.Visible = False
            pnl1.Visible = True

        Catch ex As Exception

        End Try

    End Sub

    Private Sub RosaMouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim t As PictureBox = CType(sender, PictureBox)
        If t IsNot Nothing Then currrosa = CInt(t.Name.Replace("TEAM-", "")) Else currrosa = -1
    End Sub

    Sub ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer)
        Dim t As iControl.iToolBar = CType(sender, iControl.iToolBar)
        Dim id As Integer = CInt(t.Tag)
        Call Action(CInt(t.Button(ButtonIndex).Tag), id)
    End Sub

    Sub DrawTeam()

        'Filtro le varie rose le varie rose'
        Dim str As String = txtsearch.Text

        Try
            If str <> "" Then
                Dim ris As Boolean = False
                For i As Integer = 0 To currlega.Teams.Count - 1
                    If ris = False Then
                        ris = VerifyPlayerExixst(txtsearch.Text, i)
                        If ris Then
                            p(i).Image = GetImage(currlega.Teams(i), p(i).Width, p(i).Height)
                            p(i).Tag = 1
                        Else
                            p(i).Tag = 0
                        End If
                    Else
                        p(i).Tag = 0
                    End If
                Next
            Else
                For i As Integer = 0 To currlega.Teams.Count - 1
                    p(i).Image = GetImage(currlega.Teams(i), p(i).Width, p(i).Height)
                    p(i).Tag = 1
                Next
            End If
        Catch ex As Exception

        End Try

    End Sub

    Function VerifyPlayerExixst(ByVal Nome As String, ByVal idTeam As Integer) As Boolean
        Dim ris As Boolean = False
        For i As Integer = 0 To currlega.Teams(idTeam).Players.Count - 1
            If currlega.Teams(idTeam).Players(i).Nome = Nome Then
                ris = True
                Exit For
            End If
        Next
        Return ris
    End Function

    Sub Action(ByVal Act As Integer, ByVal idteam As Integer)
        Try
            mnu1.Hide()
            Select Case Act
                Case 0
                    Dim frm As New frmrose
                    If idteam = -1 Then
                        frm.SetParameater(0, Me)
                    Else
                        frm.SetParameater(idteam, Me)
                    End If
                    frm.Show()
                Case 1, 10
                    Dim frm As New frmimpexp
                    frm.SetParameater(frmimpexp.TypeOfOperation.Import)
                    If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                        If Act = 1 Then
                            Call ImpExp.ImpExpRose.ImportHtml(-1, "", frm.GetDirectory)
                            currlega.LoadTeams(True, True)
                        Else
                            Call ImpExp.ImpExpRose.ImportHtml(idteam, currlega.Teams(idteam).Nome, frm.GetDirectory)
                            currlega.Teams(idteam).Load(False, True)
                        End If
                        Call DrawTeam()
                    End If
                    frm.Dispose()
                Case 2, 11
                    Dim frm As New frmimpexp
                    frm.SetParameater(frmimpexp.TypeOfOperation.Export)
                    If frm.ShowDialog = Windows.Forms.DialogResult.OK Then
                        If idteam = -1 OrElse Act = 2 Then
                            Call ImpExp.ImpExpRose.ExportHtml(currlega.Teams, frm.GetDirectory)
                        Else
                            Call ImpExp.ImpExpRose.ExportHtml(idteam, currlega.Teams(idteam).Nome, frm.GetDirectory)
                        End If
                    End If
                    frm.Dispose()
                Case 3, 9
                    If idteam = -1 OrElse Act = 3 Then
                        If iControl.iMsgBox.ShowMessage("Sei sicuro di voler eliminare tutte le rose?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.Yes Then
                            currlega.DeleteTeamData()
                            currlega.LoadTeams(True)
                            Call SetTeamName()
                            Call DrawTeam()
                        End If
                    Else
                        If iControl.iMsgBox.ShowMessage("Sei sicuro di voler eliminare la rosa?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.Yes Then
                            currlega.Teams(idteam).Delete()
                            currlega.Teams(idteam).SetLoadState(False, False)
                            currlega.Teams(idteam).Load()
                            Call SetTeamName()
                            Call DrawTeam()
                        End If
                    End If
                Case 4 : Me.Close()
                Case 5 : My.Computer.Clipboard.SetImage(p(idteam).Image)
                Case 6
                    Dim dlg As New Windows.Forms.FolderBrowserDialog
                    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                        Dim fname As String = ImpExp.ImpExpRose.GetHtmlFileName(idteam, currlega.Teams(idteam).Nome, dlg.SelectedPath, "png")
                        p(idteam).Image.Save(fname)
                    End If
                Case 7
                    CopyDataTable(-1, True)
                Case 8
                    CopyDataTable(idteam, True)
                Case 12
                    CopyDataTable(-1, False)
            End Select
        Catch ex As Exception
            ShowError("Errore", ex.Message)
            Call WriteError("RoseAll", "Action act " & Act, ex.Message)
        End Try
    End Sub

    Sub CopyDataTable(ByVal id As Integer, allinformation As Boolean)

        Try
            Dim fname1 As String = SystemFunction.FileAndDirectory.GetTempDirectory & "\copy1.txt"
            Dim st As Integer = 10
            Dim sw1 As New IO.StreamWriter(fname1)

            If id = -1 Then
                sw1.WriteLine("<table border='0'>")
                For i As Integer = 0 To currlega.Teams.Count - 1 Step st
                    sw1.WriteLine("<tr>")
                    Dim j As Integer = 0
                    For j = i To i + st - 1
                        If j < currlega.Teams.Count Then
                            If j = id OrElse id = -1 Then
                                sw1.WriteLine("<td>")
                                sw1.WriteLine(CopyTeam(currlega.Teams(j), allinformation))
                                sw1.WriteLine("</td>")
                            End If
                        End If
                    Next
                    sw1.WriteLine("</tr>")
                    sw1.WriteLine("<tr><td colspan=" & j & "></td></tr>")
                Next
                sw1.WriteLine("</table>")
            Else
                sw1.WriteLine(CopyTeam(currlega.Teams(id), allinformation))
            End If

            sw1.Flush()
            sw1.Close()
            sw1.Dispose()

            Dim dat As New DataObject()

            'Gestione clipboard html'
            If IO.File.Exists(fname1) Then
                dat = SystemFunction.General.SetDataClipBoardHtmlFormat(IO.File.ReadAllText(fname1), "ROSA_" & id)
            End If

            'Setto i dati nella clipboard'
            My.Computer.Clipboard.SetDataObject(dat)

        Catch ex As Exception
            ShowError("Error", ex.Message)
        End Try

    End Sub

    Function CopyTeam(ByVal Team As LegaObject.Team, allinformation As Boolean) As String

        Dim str As New System.Text.StringBuilder
        Dim diff As Integer = Team.QcurTot - Team.QiniTot

        str.AppendLine("<table border='0'>")
        If allinformation Then
            str.AppendLine("<tr><td colspan=7 style='color:#F00;'>" & Team.Nome.Replace("’", "'") & "</td><td></td></tr>")
            str.AppendLine("<tr><td colspan=7>" & Team.Allenatore.Replace("’", "'") & "</td><td></td></tr>")
            str.AppendLine(CopyPlayer(Team, allinformation))
            str.AppendLine("<tr>")
            str.AppendLine("<td></td><td></td>")
            str.AppendLine("<td>Totali</td>")
            str.AppendLine("<td style='color:#00F;'>" & Team.CostoTot & "</td>")
            str.AppendLine("<td style='color:#000;'>" & Team.QiniTot & "</td>")
            str.AppendLine("<td style='color:#000;'>" & Team.QcurTot & "</td>")
            If diff < 0 Then
                str.AppendLine("<td style='color:#F00;'>" & diff & "</td>")
            ElseIf diff > 0 Then
                str.AppendLine("<td style='color:#00F;'>" & diff & "</td>")
            Else
                str.AppendLine("<td style='color:#000;'>" & diff & "</td>")
            End If
            str.AppendLine("<td></td>")
            str.AppendLine("</tr>")
        Else
            str.AppendLine("<tr><td colspan='3' style='color:#F00;' align='left'>" & Team.Nome.Replace("’", "'") & "</td></tr>")
            str.AppendLine("<tr><td colspan='3' align='left'>" & Team.Allenatore.Replace("’", "'") & "</td></tr>")
            str.AppendLine(CopyPlayer(Team, allinformation))
            str.AppendLine("<tr>")
            str.AppendLine("<td >Totali</td>")
            str.AppendLine("<td style='color:#F00;'>" & Team.CostoTot & "</td>")
            str.AppendLine("<td style='color:#00F;'>" & Team.QcurTot & "</td>")
            str.AppendLine("</tr>")
        End If

        str.AppendLine("</table>")

        Return str.ToString

    End Function

    Function CopyPlayer(ByVal Team As LegaObject.Team, allinformation As Boolean) As String
        Dim str As New System.Text.StringBuilder
        If allinformation Then
            str.AppendLine("<tr style='color:#F00;'><td bgcolor='#FF8'>R</td><td>Nome</td><td>Squadra</td><td>Costo</td><td>QI</td><td>QA</td><td>Diff.</td></tr>")
            For k As Integer = 0 To Team.Players.Count - 1
                Dim diff As Integer = Team.Players(k).QCur - Team.Players(k).QIni
                Dim rolecolor As String = "style='color:" & System.Drawing.ColorTranslator.ToHtml(SystemFunction.General.GetRuoloForeColor(Team.Players(k).Ruolo)) & ";'"
                str.AppendLine("<tr>")
                str.AppendLine("<td " & rolecolor & ">" & Team.Players(k).Ruolo & "</td>")
                str.AppendLine("<td>" & Team.Players(k).Nome.Replace("’", "'") & "</td>")
                str.AppendLine("<td>" & Team.Players(k).Squadra.Replace("’", "'") & "</td>")
                str.AppendLine("<td style='color:#00F;'>" & Team.Players(k).Costo & "</td>")
                str.AppendLine("<td>" & Team.Players(k).QIni & "</td>")
                str.AppendLine("<td>" & Team.Players(k).QCur & "</td>")
                If diff < 0 Then
                    str.AppendLine("<td style='color:#F00;'>" & diff & "</td>")
                ElseIf diff > 0 Then
                    str.AppendLine("<td style='color:#00F;'>" & diff & "</td>")
                Else
                    str.AppendLine("<td style='color:#000;'>" & diff & "</td>")
                End If
                str.AppendLine("<td></td></tr>")
            Next
        Else
            str.AppendLine("<tr style='color:#F00;'><td>Nome</td><td>Costo</td><td>Q.A.</td></tr>")
            For k As Integer = 0 To Team.Players.Count - 1
                Dim diff As Integer = Team.Players(k).QCur - Team.Players(k).QIni
                Dim st As String = ""
                If Team.Players(k).Riconfermato = 1 Then
                    st = " bgcolor='#FFFFAA'"
                End If
                str.AppendLine("<td" & st & ">" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Team.Players(k).Nome.Replace("’", "'").ToLower) & "</td>")
                str.AppendLine("<td" & st & " style='color:#F00;'>" & Team.Players(k).Costo & "</td>")
                str.AppendLine("<td" & st & " style='color:#00F;'>" & Team.Players(k).QCur & "</td>")
                str.AppendLine("</tr>")
            Next
        End If

        Return str.ToString
    End Function

    Private Sub txtsearch_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            'pnl1.Visible = False
            For i As Integer = 0 To p.Count - 1
                p(i).Visible = False
                t(i).Visible = False
                l(i).Visible = False
            Next
            pnl1.Refresh()
            Call DrawTeam()
            Call SetControlPosition()
            pnl1.Refresh()
        End If
    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Call Action(ButtonIndex, -1)
    End Sub

    Private Sub mnu1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles mnu1.ItemClicked
        Call Action(CInt(e.ClickedItem.Tag), currrosa)
    End Sub

End Class
