Imports System.Drawing.Drawing2D

Public Class frmeditcoppa

    Dim cup As New LegaObject.Coppa
    Dim ris As Windows.Forms.DialogResult = Windows.Forms.DialogResult.Cancel
    Dim dictlbqf As New Dictionary(Of String, iControl.iToolBar)
    Dim dictlbgr As New Dictionary(Of String, iControl.iToolBar)
    Dim dictlbsf As New Dictionary(Of String, iControl.iToolBar)
    Dim dictlbfi As New Dictionary(Of String, iControl.iToolBar)

    Private Sub frmeditcoppa_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If tlbaction.Button(0).Enabled Then
            If iControl.iMsgBox.ShowMessage("Salvare i cambiamenti effettuati?", "Attenzione", iControl.iMsgBox.MsgStyle.YesNo, iControl.iMsgBox.Icona.MsgAlert) = Windows.Forms.DialogResult.OK Then
                Call Save()
            Else
                tlbaction.Button(0).Enabled = False
                tlbaction.draw(True)
            End If
        End If
        Me.DialogResult = ris
    End Sub

    Private Sub frmeditcoppa_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName
        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        lbby.Text = My.Application.Info.Copyright

        Me.Width = pnl1.Width + padd * 2 + IForm1.LX * 2

        Me.Location = New Point(My.Computer.Screen.WorkingArea.Width \ 2 - Me.Width \ 2, My.Computer.Screen.WorkingArea.Height \ 2 - Me.Height \ 2)

        Call Display()
        Call SetTheme()

    End Sub

    Sub SetTheme()

        Dim sp As Integer = 3

        Call SetTorneoLabel()

        pnl1.Left = IForm1.LX + padd
        pnl1.Top = IForm1.TY + padd

        tlbaction.Left = IForm1.RX - tlbaction.Width - padd
        tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
        tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

        lnbot.Top = tlbaction.Top - lnbot.Height - 5
        lnbot.Left = IForm1.LX + padd
        lnbot.Width = IForm1.RX - IForm1.LX - padd * 2

        lbby.Left = IForm1.LX + padd - 3
        lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

    End Sub

    Sub SetTorneoLabel()
        lbtorneo2.Text = currlega.Settings.Nome
        lbtorneo2.Top = IForm1.GetTopBarTopPosition(1)
        lbtorneo2.Height = IForm1.TH1
        lbtorneo2.Left = IForm1.RX - 12 - lbtorneo2.Width
        lbtorneo2.Background = IForm1.GetTopBarImage(1)
        lbtorneo1.Top = lbtorneo2.Top
        lbtorneo1.Height = lbtorneo2.Height
        lbtorneo1.Left = lbtorneo2.Left - lbtorneo1.Width
        lbtorneo1.Background = lbtorneo2.Background
    End Sub

    Sub Display()
        Try
     
            cup.Load()

            'Agggiungo i controlli necessari'
            If currlega.Teams.Count > 0 Then
                Call AddMatch(4, "qf", dictlbqf)
                Call AddMatch(2, "sf", dictlbsf)
                Call AddMatch(1, "fi", dictlbfi)
                Call AddGironi(3, dictlbgr)
            End If

            '*********Setto i parametri*********'

            If cup.TipoSecondoTurno = "quarti" Then
                tlboptqf.Button(0).State = True
                tlboptqf.Button(1).State = False
            Else
                tlboptqf.Button(0).State = False
                tlboptqf.Button(1).State = True
            End If
            tlboptqf.draw(True)

            'Play off'

            For k As Integer = 0 To 2
                'Girone 1'
                If cup.PlayOff(0).Clasa(k).TeamId <> -1 Then
                    dictlbgr("grt-" & CStr(k)).Button(0).Text = currlega.Teams(cup.PlayOff(0).Clasa(k).TeamId).Nome
                End If
                dictlbgr("grt-" & CStr(k)).Button(0).Tag = CStr(cup.PlayOff(0).Clasa(k).TeamId)
                dictlbgr("grt-" & CStr(k)).Button(0).SetSubButtonSelection(dictlbgr("grt-" & CStr(k)).Button(0).Text, True)
                'Girone 2'
                If cup.PlayOff(1).Clasa(k).TeamId <> -1 Then
                    dictlbgr("grt-" & CStr(k)).Button(2).Text = currlega.Teams(cup.PlayOff(1).Clasa(k).TeamId).Nome
                End If
                dictlbgr("grt-" & CStr(k)).Button(2).Tag = CStr(cup.PlayOff(1).Clasa(k).TeamId)
                dictlbgr("grt-" & CStr(k)).Button(2).SetSubButtonSelection(dictlbgr("grt-" & CStr(k)).Button(2).Text, True)
                'Partite girone 1'
                If cup.PlayOff(0).Partite(k).GiornataAndata <> -1 Then
                    dictlbgr("grp-0").Button(k * 2 + 1).Text = CStr(cup.PlayOff(0).Partite(k).GiornataAndata)
                Else
                    dictlbgr("grp-0").Button(k * 2 + 1).Text = ""
                End If
                dictlbgr("grp-0").Button(k * 2 + 1).Tag = CStr(cup.PlayOff(0).Partite(k).GiornataAndata)
                dictlbgr("grp-0").Button(k * 2 + 1).SetSubButtonSelection(dictlbgr("grp-0").Button(k * 2 + 1).Text, True)
                'Partite girone 2'
                If cup.PlayOff(1).Partite(k).GiornataAndata <> -1 Then
                    dictlbgr("grp-1").Button(k * 2 + 1).Text = CStr(cup.PlayOff(1).Partite(k).GiornataAndata)
                Else
                    dictlbgr("grp-1").Button(k * 2 + 1).Text = ""
                End If
                dictlbgr("grp-1").Button(k * 2 + 1).Tag = CStr(cup.PlayOff(1).Partite(k).GiornataAndata)
                dictlbgr("grp-1").Button(k * 2 + 1).SetSubButtonSelection(dictlbgr("grp-1").Button(k * 2 + 1).Text, True)

            Next

            'Quarti di finale'

            For k As Integer = 0 To 3
                'Team 1'
                If cup.QuartiDiFinale(k).TeamId1 <> -1 Then
                    dictlbqf("qf-" & CStr(k)).Button(0).Text = currlega.Teams(cup.QuartiDiFinale(k).TeamId1).Nome
                End If
                dictlbqf("qf-" & CStr(k)).Button(0).Tag = CStr(cup.QuartiDiFinale(k).TeamId1)
                dictlbqf("qf-" & CStr(k)).Button(0).SetSubButtonSelection(dictlbqf("qf-" & CStr(k)).Button(0).Text, True)
                'Team 2'
                If cup.QuartiDiFinale(k).TeamId2 <> -1 Then
                    dictlbqf("qf-" & CStr(k)).Button(2).Text = currlega.Teams(cup.QuartiDiFinale(k).TeamId2).Nome
                End If
                dictlbqf("qf-" & CStr(k)).Button(2).Tag = CStr(cup.QuartiDiFinale(k).TeamId2)
                dictlbqf("qf-" & CStr(k)).Button(2).SetSubButtonSelection(dictlbqf("qf-" & CStr(k)).Button(2).Text, True)
                'Giornata di andata'
                If cup.QuartiDiFinale(k).GiornataAndata <> -1 Then
                    dictlbqf("qf-" & CStr(k)).Button(4).Text = CStr(cup.QuartiDiFinale(k).GiornataAndata)
                Else
                    dictlbqf("qf-" & CStr(k)).Button(4).Text = ""
                End If
                dictlbqf("qf-" & CStr(k)).Button(4).Tag = CStr(cup.QuartiDiFinale(k).GiornataAndata)
                dictlbqf("qf-" & CStr(k)).Button(4).SetSubButtonSelection(dictlbqf("qf-" & CStr(k)).Button(4).Text, True)
                'Giornata di ritorno'
                If cup.QuartiDiFinale(k).GiornataRitorno <> -1 Then
                    dictlbqf("qf-" & CStr(k)).Button(6).Text = CStr(cup.QuartiDiFinale(k).GiornataRitorno)
                Else
                    dictlbqf("qf-" & CStr(k)).Button(6).Text = ""
                End If
                dictlbqf("qf-" & CStr(k)).Button(6).Tag = CStr(cup.QuartiDiFinale(k).GiornataRitorno)
                dictlbqf("qf-" & CStr(k)).Button(6).SetSubButtonSelection(dictlbqf("qf-" & CStr(k)).Button(6).Text, True)
            Next

            'Semifinale'

            For k As Integer = 0 To 1
                'Team 1'
                If cup.SemiFinali(k).TeamId1 <> -1 Then
                    dictlbsf("sf-" & CStr(k)).Button(0).Text = currlega.Teams(cup.SemiFinali(k).TeamId1).Nome
                End If
                dictlbsf("sf-" & CStr(k)).Button(0).Tag = CStr(cup.SemiFinali(k).TeamId1)
                dictlbsf("sf-" & CStr(k)).Button(0).SetSubButtonSelection(dictlbsf("sf-" & CStr(k)).Button(0).Text, True)
                'Team 2'
                If cup.SemiFinali(k).TeamId2 <> -1 Then
                    dictlbsf("sf-" & CStr(k)).Button(2).Text = currlega.Teams(cup.SemiFinali(k).TeamId2).Nome
                End If
                dictlbsf("sf-" & CStr(k)).Button(2).Tag = CStr(cup.SemiFinali(k).TeamId2)
                dictlbsf("sf-" & CStr(k)).Button(2).SetSubButtonSelection(dictlbsf("sf-" & CStr(k)).Button(2).Text, True)
                'Giornata di andata'
                If cup.SemiFinali(k).GiornataAndata <> -1 Then
                    dictlbsf("sf-" & CStr(k)).Button(4).Text = CStr(cup.SemiFinali(k).GiornataAndata)
                Else
                    dictlbsf("sf-" & CStr(k)).Button(4).Text = ""
                End If
                dictlbsf("sf-" & CStr(k)).Button(4).Tag = CStr(cup.SemiFinali(k).GiornataAndata)
                dictlbsf("sf-" & CStr(k)).Button(4).SetSubButtonSelection(dictlbsf("sf-" & CStr(k)).Button(4).Text, True)
                'Giornata di ritorno'
                If cup.SemiFinali(k).GiornataRitorno <> -1 Then
                    dictlbsf("sf-" & CStr(k)).Button(6).Text = CStr(cup.SemiFinali(k).GiornataRitorno)
                Else
                    dictlbsf("sf-" & CStr(k)).Button(6).Text = ""
                End If
                dictlbsf("sf-" & CStr(k)).Button(6).Tag = CStr(cup.SemiFinali(k).GiornataRitorno)
                dictlbsf("sf-" & CStr(k)).Button(6).SetSubButtonSelection(dictlbsf("sf-" & CStr(k)).Button(6).Text, True)
            Next

            'Finale'

            If cup.Finale(0).TeamId1 <> -1 Then
                dictlbfi("fi-0").Button(0).Text = currlega.Teams(cup.Finale(0).TeamId1).Nome
            End If
            dictlbfi("fi-0").Button(0).Tag = CStr(cup.Finale(0).TeamId1)
            dictlbfi("fi-0").Button(0).SetSubButtonSelection(dictlbfi("fi-0").Button(0).Text, True)

            If cup.Finale(0).TeamId2 <> -1 Then
                dictlbfi("fi-0").Button(2).Text = currlega.Teams(cup.Finale(0).TeamId2).Nome
            End If
            dictlbfi("fi-0").Button(2).Tag = CStr(cup.Finale(0).TeamId2)
            dictlbfi("fi-0").Button(2).SetSubButtonSelection(dictlbfi("fi-0").Button(2).Text, True)

            If cup.Finale(0).GiornataRitorno <> -1 Then
                dictlbfi("fi-0").Button(4).Text = CStr(cup.Finale(0).GiornataAndata)
            Else
                dictlbfi("fi-0").Button(4).Text = ""
            End If
            dictlbfi("fi-0").Button(4).SetSubButtonSelection(dictlbfi("fi-0").Button(4).Text, True)
            dictlbfi("fi-0").Button(4).Tag = CStr(cup.Finale(0).GiornataAndata)

            If cup.Finale(0).GiornataRitorno <> -1 Then
                dictlbfi("fi-0").Button(6).Text = CStr(cup.Finale(0).GiornataRitorno)
            Else
                dictlbfi("fi-0").Button(6).Text = ""
            End If
            dictlbfi("fi-0").Button(6).SetSubButtonSelection(dictlbfi("fi-0").Button(6).Text, True)
            dictlbfi("fi-0").Button(6).Tag = CStr(cup.Finale(0).GiornataRitorno)

            Call PositionControl()

        Catch ex As Exception
            Call WriteError("frmeditcoppa", "Display", ex.Message)
        End Try
    End Sub

    Sub AddMatch(NumberMatch As Integer, Phase As String, dictlb As Dictionary(Of String, iControl.iToolBar))

        Try
            Dim tlbh As New iControl.iToolBar
            tlbh.Size = New Size(601, 19)
            tlbh.ForeColor = Color.Brown
            tlbh.Button.Clear()
            tlbh.Button.Add(New iControl.ToolbarButton("", "Team 1", iControl.ToolbarButton.iType.Label))
            tlbh.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.Label))
            tlbh.Button.Add(New iControl.ToolbarButton("", "Team 2", iControl.ToolbarButton.iType.Label))
            tlbh.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.Label))
            tlbh.Button.Add(New iControl.ToolbarButton("", "And", iControl.ToolbarButton.iType.Label))
            tlbh.Button.Add(New iControl.ToolbarButton("", "-", iControl.ToolbarButton.iType.Label))
            tlbh.Button.Add(New iControl.ToolbarButton("", "Rit", iControl.ToolbarButton.iType.Label))
            tlbh.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbh.Button(0).MinWidth = 195
            tlbh.Button(0).TextAlign = HorizontalAlignment.Left
            tlbh.Button(1).MinWidth = 40
            tlbh.Button(1).TextAlign = HorizontalAlignment.Center
            tlbh.Button(2).MinWidth = 195
            tlbh.Button(2).TextAlign = HorizontalAlignment.Left
            tlbh.Button(3).MinWidth = 20
            tlbh.Button(3).TextAlign = HorizontalAlignment.Center
            tlbh.Button(4).MinWidth = 40
            tlbh.Button(4).TextAlign = HorizontalAlignment.Center
            tlbh.Button(5).MinWidth = 20
            tlbh.Button(5).TextAlign = HorizontalAlignment.Center
            tlbh.Button(6).MinWidth = 40
            tlbh.Button(6).TextAlign = HorizontalAlignment.Center
            tlbh.draw()

            dictlb.Add(Phase & "h", tlbh)
            pnl1.Controls.Add(tlbh)

            For l As Integer = 0 To NumberMatch - 1

                Dim tlbp As New iControl.iToolBar
                tlbp.Size = New Size(601, 19)
                tlbp.FlatStyle = AppSett.Personal.Theme.FlatStyle
                tlbp.BorderColor = Color.DimGray
                tlbp.BorderColorDropDown = Color.DimGray
                tlbp.Button.Clear()
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.ButtonDropDown))
                tlbp.Button.Add(New iControl.ToolbarButton("", "vs", iControl.ToolbarButton.iType.Label))
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.ButtonDropDown))
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.Label))
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.ButtonDropDown))
                tlbp.Button.Add(New iControl.ToolbarButton("", "-", iControl.ToolbarButton.iType.Label))
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.ButtonDropDown))

                tlbp.Button(1).MinWidth = 40
                tlbp.Button(1).TextAlign = HorizontalAlignment.Center
                tlbp.Button(3).MinWidth = 26
                tlbp.Button(3).TextAlign = HorizontalAlignment.Center
                tlbp.Button(3).TextAlign = HorizontalAlignment.Center
                tlbp.Button(5).MinWidth = 20
                tlbp.Button(5).TextAlign = HorizontalAlignment.Center


                For i As Integer = 0 To 2 Step 2
                    tlbp.Button(i).RowsHeightAutoSize = False
                    tlbp.Button(i).State = True
                    tlbp.Button(i).MinWidth = 195
                    tlbp.Button(i).Width = 195
                    tlbp.Button(i).SubWidth = 194
                    tlbp.Button(i).SubItemsAutoSize = False
                    tlbp.Button(i).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                    tlbp.Button(i).SubItems(0).Tag = "-1"
                    For j As Integer = 0 To currlega.Teams.Count - 1
                        tlbp.Button(i).SubItems.Add(New iControl.ToolBarButtonSubItem(currlega.Teams(j).Nome, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                        tlbp.Button(i).SubItems(j + 1).Tag = CStr(j)
                    Next
                Next

                For i As Integer = 4 To 6 Step 2
                    tlbp.Button(i).State = True
                    tlbp.Button(i).MinWidth = 40
                    tlbp.Button(i).RowsHeightAutoSize = False
                    tlbp.Button(i).SubItemsAutoSize = False
                    tlbp.Button(i).Width = 40
                    tlbp.Button(i).SubWidth = 39
                    tlbp.Button(i).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                    tlbp.Button(i).SubItems(0).Tag = "-1"
                    For j As Integer = 1 To currlega.Settings.NumberOfDays
                        tlbp.Button(i).SubItems.Add(New iControl.ToolBarButtonSubItem(CStr(j), iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                        tlbp.Button(i).SubItems(j).Tag = CStr(j)
                    Next
                Next

                dictlb.Add(Phase & "-" & CStr(l), tlbp)
                AddHandler tlbp.SubButtonClick, AddressOf tlb_SubButtonClick
                pnl1.Controls.Add(tlbp)

            Next
        Catch ex As Exception
            Call WriteError("frmeditcoppa", "AddMatch", ex.Message)
        End Try
       
    End Sub

    Sub AddGironi(NumberTeam As Integer, dictlb As Dictionary(Of String, iControl.iToolBar))
        Try
            Dim tlbh As New iControl.iToolBar
            tlbh.FlatStyle = AppSett.Personal.Theme.FlatStyle
            tlbh.Size = New Size(601, 19)
            tlbh.ForeColor = Color.Brown
            tlbh.Button.Clear()
            tlbh.Button.Add(New iControl.ToolbarButton("", "Girone 1", iControl.ToolbarButton.iType.Label))
            tlbh.Button.Add(New iControl.ToolbarButton("", "Girone 2", iControl.ToolbarButton.iType.Label))
            tlbh.Button(0).MinWidth = 298
            tlbh.Button(0).TextAlign = HorizontalAlignment.Left
            tlbh.Button(1).MinWidth = 275
            tlbh.Button(1).TextAlign = HorizontalAlignment.Left
            tlbh.draw()

            dictlb.Add("grh", tlbh)
            pnl1.Controls.Add(tlbh)

            For l As Integer = 0 To NumberTeam - 1

                Dim tlbp As New iControl.iToolBar
                tlbp.Size = New Size(601, 19)
                tlbp.FlatStyle = AppSett.Personal.Theme.FlatStyle
                tlbp.BorderColor = Color.DimGray
                tlbp.BorderColorDropDown = Color.DimGray
                tlbp.Button.Clear()
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.ButtonDropDown))
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.Label))
                tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.ButtonDropDown))

                tlbp.Button(0).MinWidth = 258
                tlbp.Button(1).MinWidth = 40
                tlbp.Button(2).MinWidth = 258

                For i As Integer = 0 To 2 Step 2
                    tlbp.Button(i).RowsHeightAutoSize = False
                    tlbp.Button(i).State = True
                    tlbp.Button(i).SubWidth = 257
                    tlbp.Button(i).SubItemsAutoSize = False
                    tlbp.Button(i).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                    tlbp.Button(i).SubItems(0).Tag = "-1"
                    For j As Integer = 0 To currlega.Teams.Count - 1
                        tlbp.Button(i).SubItems.Add(New iControl.ToolBarButtonSubItem(currlega.Teams(j).Nome, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                        tlbp.Button(i).SubItems(j + 1).Tag = CStr(j)
                    Next
                Next

                dictlb.Add("grt-" & CStr(l), tlbp)
                AddHandler tlbp.SubButtonClick, AddressOf tlb_SubButtonClick
                pnl1.Controls.Add(tlbp)

            Next

            For k As Integer = 0 To 1
                Dim gg As Integer = 1
                Dim tlbp As New iControl.iToolBar
                tlbp.Size = New Size(270, 19)
                tlbp.BorderColor = Color.DimGray
                tlbp.BorderColorDropDown = Color.DimGray
                tlbp.Button.Clear()
                tlbp.FlatStyle = AppSett.Personal.Theme.FlatStyle
                For l As Integer = 0 To 4 Step 2
                    tlbp.Button.Add(New iControl.ToolbarButton("", "Gio n°" & CStr(gg), iControl.ToolbarButton.iType.Label))
                    tlbp.Button.Add(New iControl.ToolbarButton("", "", iControl.ToolbarButton.iType.ButtonDropDown))
                    tlbp.Button(l + 1).State = True
                    tlbp.Button(l + 1).MinWidth = 40
                    tlbp.Button(l + 1).RowsHeightAutoSize = False
                    tlbp.Button(l + 1).SubItemsAutoSize = False
                    tlbp.Button(l + 1).Width = 40
                    tlbp.Button(l + 1).SubWidth = 39
                    tlbp.Button(l + 1).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                    tlbp.Button(l + 1).SubItems(0).Tag = "-1"
                    For j As Integer = 1 To currlega.Settings.NumberOfDays
                        tlbp.Button(l + 1).SubItems.Add(New iControl.ToolBarButtonSubItem(CStr(j), iControl.ToolBarButtonSubItem.SubButtonStype.Item))
                        tlbp.Button(l + 1).SubItems(j).Tag = CStr(j)
                    Next
                    gg = gg + 1
                Next

                dictlb.Add("grp-" & CStr(k), tlbp)
                AddHandler tlbp.SubButtonClick, AddressOf tlb_SubButtonClick
                pnl1.Controls.Add(tlbp)

            Next

        Catch ex As Exception
            Call WriteError("frmeditcoppa", "AddGironi", ex.Message)
        End Try
    End Sub

    Sub PositionControl()
        Try
            'Disposizione controlli'
            Dim dy As Integer = 0

            'Quarti di finale / Play off"
            ln1.Top = 0
            ln1.Left = 0
            ln1.Width = pnl1.Width - 1
            tlboptqf.Left = 0
            tlboptqf.Top = ln1.Top + ln1.Height + 3

            dy = tlboptqf.Top + tlboptqf.Height + 6

            If tlboptqf.Button(0).State Then
                dictlbqf("qfh").Top = dy
                dictlbqf("qfh").Left = 0
                For l As Integer = 0 To 3
                    dictlbqf("qf-" & CStr(l)).Top = l * 22 + 22 + dy
                    dictlbqf("qf-" & CStr(l)).Left = 0
                    dictlbqf("qf-" & CStr(l)).draw(True)
                Next
                For Each c As String In dictlbqf.Keys
                    dictlbqf(c).Visible = True
                Next
                For Each c As String In dictlbgr.Keys
                    dictlbgr(c).Visible = False
                Next
            Else
                dictlbgr("grh").Top = dy
                dictlbgr("grh").Left = 0
                For l As Integer = 0 To 2
                    dictlbgr("grt-" & CStr(l)).Top = l * 22 + 22 + dy
                    dictlbgr("grt-" & CStr(l)).Left = 0
                    dictlbgr("grt-" & CStr(l)).draw(True)
                Next
                dictlbgr("grp-0").Top = dictlbgr("grt-2").Top + dictlbgr("grt-2").Height + 10
                dictlbgr("grp-0").Left = 0
                dictlbgr("grp-0").draw(True)
                dictlbgr("grp-1").Top = dictlbgr("grp-0").Top
                dictlbgr("grp-1").Left = 298
                dictlbgr("grp-1").draw(True)
                For Each c As String In dictlbgr.Keys
                    dictlbgr(c).Visible = True
                Next
                For Each c As String In dictlbqf.Keys
                    dictlbqf(c).Visible = False
                Next
            End If

            'Semi finali'
            If tlboptqf.Button(0).State Then
                ln2.Top = dictlbqf("qf-3").Top + dictlbqf("qf-3").Height + 20
            Else
                ln2.Top = dictlbgr("grp-0").Top + dictlbgr("grp-0").Height + 20
            End If
            ln2.Left = 0
            ln2.Width = pnl1.Width - 1

            dy = ln2.Top + ln2.Height + 3

            dictlbsf("sfh").Top = dy
            dictlbsf("sfh").Left = 0

            For l As Integer = 0 To 1
                dictlbsf("sf-" & CStr(l)).Top = l * 22 + 22 + dy
                dictlbsf("sf-" & CStr(l)).Left = 0
                dictlbsf("sf-" & CStr(l)).draw(True)
            Next

            'Finali'
            ln3.Top = dictlbsf("sf-1").Top + dictlbsf("sf-1").Height + 20
            ln3.Left = 0
            ln3.Width = pnl1.Width - 1

            dy = ln3.Top + ln3.Height + 3

            dictlbfi("fih").Top = dy
            dictlbfi("fih").Left = 0
            dictlbfi("fi-0").Top = 22 + dy
            dictlbfi("fi-0").Left = 0
            dictlbfi("fi-0").draw(True)

        Catch ex As Exception
            Call WriteError("frmeditcoppa", "PositionControl", ex.Message)
        End Try
    End Sub

    Sub Save()

        'PlayOff'
        For k As Integer = 0 To 1
            For j As Integer = 0 To 2
                cup.PlayOff(k).Clasa(j).TeamId = CInt(dictlbgr("grt-" & CStr(j)).Button(k * 2).Tag)
                cup.PlayOff(k).Partite(j).GiornataAndata = CInt(dictlbgr("grp-" & CStr(k)).Button(j * 2 + 1).Tag)
            Next
        Next
        For i As Integer = 0 To 2
            currlega.Settings.Coppa.PlayOffGiorone1Team(i) = cup.PlayOff(0).Clasa(i).TeamId
            currlega.Settings.Coppa.PlayOffGiorone2Team(i) = cup.PlayOff(1).Clasa(i).TeamId
            currlega.Settings.Coppa.PlayOffGiorone1Match(i) = cup.PlayOff(0).Partite(i).GiornataAndata
            currlega.Settings.Coppa.PlayOffGiorone2Match(i) = cup.PlayOff(1).Partite(i).GiornataAndata
        Next

        'Quarti di finale'
        If tlboptqf.Button(0).State Then
            cup.TipoSecondoTurno = "quarti"
        Else
            cup.TipoSecondoTurno = "playoff"
        End If
        currlega.Settings.Coppa.TipoSecondoTurno = cup.TipoSecondoTurno

        For i As Integer = 0 To 3
            cup.QuartiDiFinale(i).TeamId1 = CInt(dictlbqf("qf-" & CStr(i)).Button(0).Tag)
            cup.QuartiDiFinale(i).TeamId2 = CInt(dictlbqf("qf-" & CStr(i)).Button(2).Tag)
            cup.QuartiDiFinale(i).GiornataAndata = CInt(dictlbqf("qf-" & CStr(i)).Button(4).Tag)
            cup.QuartiDiFinale(i).GiornataRitorno = CInt(dictlbqf("qf-" & CStr(i)).Button(6).Tag)
        Next
        currlega.Settings.Coppa.QuartiDiFinale1Team(0) = cup.QuartiDiFinale(0).TeamId1
        currlega.Settings.Coppa.QuartiDiFinale1Team(1) = cup.QuartiDiFinale(0).TeamId2
        currlega.Settings.Coppa.QuartiDiFinale2Team(0) = cup.QuartiDiFinale(1).TeamId1
        currlega.Settings.Coppa.QuartiDiFinale2Team(1) = cup.QuartiDiFinale(1).TeamId2
        currlega.Settings.Coppa.QuartiDiFinale3Team(0) = cup.QuartiDiFinale(2).TeamId1
        currlega.Settings.Coppa.QuartiDiFinale3Team(1) = cup.QuartiDiFinale(2).TeamId2
        currlega.Settings.Coppa.QuartiDiFinale4Team(0) = cup.QuartiDiFinale(3).TeamId1
        currlega.Settings.Coppa.QuartiDiFinale4Team(1) = cup.QuartiDiFinale(3).TeamId2

        currlega.Settings.Coppa.QuartiDiFinale1Match(0) = cup.QuartiDiFinale(0).GiornataAndata
        currlega.Settings.Coppa.QuartiDiFinale1Match(1) = cup.QuartiDiFinale(0).GiornataRitorno
        currlega.Settings.Coppa.QuartiDiFinale2Match(0) = cup.QuartiDiFinale(1).GiornataAndata
        currlega.Settings.Coppa.QuartiDiFinale2Match(1) = cup.QuartiDiFinale(1).GiornataRitorno
        currlega.Settings.Coppa.QuartiDiFinale3Match(0) = cup.QuartiDiFinale(2).GiornataAndata
        currlega.Settings.Coppa.QuartiDiFinale3Match(1) = cup.QuartiDiFinale(2).GiornataRitorno
        currlega.Settings.Coppa.QuartiDiFinale4Match(0) = cup.QuartiDiFinale(3).GiornataAndata
        currlega.Settings.Coppa.QuartiDiFinale4Match(1) = cup.QuartiDiFinale(3).GiornataRitorno

        'Semifinale'
        For i As Integer = 0 To 1
            cup.SemiFinali(i).TeamId1 = CInt(dictlbsf("sf-" & CStr(i)).Button(0).Tag)
            cup.SemiFinali(i).TeamId2 = CInt(dictlbsf("sf-" & CStr(i)).Button(2).Tag)
            cup.SemiFinali(i).GiornataAndata = CInt(dictlbsf("sf-" & CStr(i)).Button(4).Tag)
            cup.SemiFinali(i).GiornataRitorno = CInt(dictlbsf("sf-" & CStr(i)).Button(6).Tag)
        Next
        currlega.Settings.Coppa.Semifinale1Team(0) = cup.SemiFinali(0).TeamId1
        currlega.Settings.Coppa.Semifinale1Team(1) = cup.SemiFinali(0).TeamId2
        currlega.Settings.Coppa.Semifinale1Match(0) = cup.SemiFinali(0).GiornataAndata
        currlega.Settings.Coppa.Semifinale1Match(1) = cup.SemiFinali(0).GiornataRitorno
        currlega.Settings.Coppa.Semifinale2Team(0) = cup.SemiFinali(1).TeamId1
        currlega.Settings.Coppa.Semifinale2Team(1) = cup.SemiFinali(1).TeamId2
        currlega.Settings.Coppa.Semifinale2Match(0) = cup.SemiFinali(1).GiornataAndata
        currlega.Settings.Coppa.Semifinale2Match(1) = cup.SemiFinali(1).GiornataRitorno

        'Finale'
        cup.Finale(0).TeamId1 = CInt(dictlbfi("fi-0").Button(0).Tag)
        cup.Finale(0).TeamId2 = CInt(dictlbfi("fi-0").Button(2).Tag)
        cup.Finale(0).GiornataAndata = CInt(dictlbfi("fi-0").Button(4).Tag)
        cup.Finale(0).GiornataRitorno = CInt(dictlbfi("fi-0").Button(6).Tag)
        currlega.Settings.Coppa.FinaleTeam(0) = cup.Finale(0).TeamId1
        currlega.Settings.Coppa.FinaleTeam(1) = cup.Finale(0).TeamId2
        currlega.Settings.Coppa.FinaleMatch(0) = cup.Finale(0).GiornataAndata
        currlega.Settings.Coppa.FinaleMatch(1) = cup.Finale(0).GiornataRitorno

        currlega.SaveSettings()
        tlbaction.Button(0).Enabled = False
        tlbaction.draw(True)
        ris = Windows.Forms.DialogResult.OK

    End Sub

    Private Sub tlb_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer)
        Dim tlb As iControl.iToolBar = CType(sender, iControl.iToolBar)
        tlb.Button(ButtonIndex).Text = tlb.Button(ButtonIndex).SubItems(SubButtonIndex).Text
        tlb.Button(ButtonIndex).ClearSubButtonSelection()
        tlb.Button(ButtonIndex).SetSubButtonSelection(tlb.Button(ButtonIndex).Text, True)
        tlb.Button(ButtonIndex).Tag = tlb.Button(ButtonIndex).SubItems(SubButtonIndex).Tag
        tlbaction.Button(0).Enabled = True
        tlbaction.draw(True)
    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0 : Call Save()
            Case 1 : Me.Close()
        End Select
    End Sub

    Private Sub tlboptqf_SubButtonClick(sender As Object, e As EventArgs, ButtonIndex As Integer) Handles tlboptqf.ButtonClick
        tlbaction.Button(0).Enabled = True
        tlbaction.draw(True)
        Call PositionControl()
    End Sub

End Class
