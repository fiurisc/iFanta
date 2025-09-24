Imports System.IO

Namespace SystemFunction
    Public Class Gui

        Shared Sub SetTorneoLabel(lbtorneo1 As iControl.iLabel, lbtorneo2 As iControl.iLabel, iform1 As iControl.iForm)
            lbtorneo2.Text = currlega.Settings.Nome
            lbtorneo2.Top = iform1.GetTopBarTopPosition(1)
            lbtorneo2.Height = iform1.TH1
            lbtorneo2.Left = iform1.RX - 12 - lbtorneo2.Width
            lbtorneo2.Background = iform1.GetTopBarImage(1)
            lbtorneo1.Top = lbtorneo2.Top
            lbtorneo1.Height = lbtorneo2.Height
            lbtorneo1.Left = lbtorneo2.Left - lbtorneo1.Width
            lbtorneo1.Background = lbtorneo2.Background
        End Sub

        Public Shared Sub DrawInfoPresence(dtginfo As DataGridView, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs)

            If dtginfo.Item(2, e.RowIndex).Value IsNot Nothing Then

                Dim site As String = dtginfo.Item(1, e.RowIndex).Value.ToString.ToLower
                Dim info As String = dtginfo.Item(2, e.RowIndex).Value.ToString
                Dim format As New StringFormat

                format.Alignment = StringAlignment.Far

                If info.Contains("Titolare") Then
                    e.Graphics.DrawString(info, New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.RoyalBlue, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
                ElseIf info.Contains("Panchina") OrElse info.Contains("A disposizione") Then
                    e.Graphics.DrawString(info, New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Red, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
                Else
                    e.Graphics.DrawString(info, New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.DimGray, dtginfo.Width - 10, e.RowBounds.Top + 2, format)
                End If

            End If

            If CInt(dtginfo.Rows(e.RowIndex).Tag) = 1 Then
                e.Graphics.DrawLine(New Pen(Color.Gainsboro, 2), 4, e.RowBounds.Top + e.RowBounds.Height - 1, e.RowBounds.Width - 4, e.RowBounds.Top + e.RowBounds.Height - 1)
            End If
        End Sub

        Public Shared Sub SetPlayerFormazioneToDataGrid(datarow As DataGridViewRow, p As LegaObject.Formazione.PlayerFormazione, Giornata As Integer)

            datarow.Cells("ruolo").Style.ForeColor = SystemFunction.General.GetRuoloForeColor(p.Ruolo)
            datarow.Cells("ruolo").Value = p.Ruolo
            datarow.Cells("nome").Value = p.Nome

            datarow.Cells("squadra").Tag = p.Squadra
            If p.Squadra.Length > 3 Then
                datarow.Cells("squadra").Value = p.Squadra.Substring(0, 3)
            Else
                datarow.Cells("squadra").Value = p.Squadra
            End If

            datarow.Cells("gs").Value = General.SetFieldIntegerData(p.StatisticAll.Gs, "-")
            datarow.Cells("gf").Value = General.SetFieldIntegerData(p.StatisticAll.Gf, "-")
            datarow.Cells("amm").Value = General.SetFieldIntegerData(p.StatisticAll.Amm, "-")
            datarow.Cells("esp").Value = General.SetFieldIntegerData(p.StatisticAll.Esp, "-")
            datarow.Cells("ass").Value = General.SetFieldIntegerData(p.StatisticAll.Ass, "-")
            datarow.Cells("pgio").Value = General.SetFieldIntegerData(p.StatisticAll.pGiocate, "-")
            datarow.Cells("avgpt").Value = General.SetFieldSingleData(p.StatisticAll.Avg_Pt, "-")
            datarow.Cells("pgiol").Value = General.SetFieldIntegerData(p.StatisticLast.pGiocate, "-")
            datarow.Cells("ptitl").Value = General.SetFieldIntegerData(p.StatisticLast.Titolare, "-")
            If p.StatisticLast.pGiocate > 5 Then
                p.StatisticLast.Titolare = p.StatisticLast.Titolare
            End If
            datarow.Cells("var").Value = DrawingAndImage.GetPlayerVariationImage(p.Variation)

            If webdata.WebPlayers.ContainsKey(Giornata & "/" & p.Nome & "/" & p.Squadra) Then
                Dim wp As wData.wPlayer = webdata.WebPlayers(Giornata & "/" & p.Nome & "/" & p.Squadra)
                datarow.Cells("presenza").Tag = wp
                'Controllo se e' infortunato o squalificarti'
                Dim t As String = ""
                For k As Integer = 0 To wp.Info.Count - 1
                    If wp.Info(k).State = "Infortunato" Then
                        t = "i"
                        Exit For
                    ElseIf wp.Info(k).State = "Squalificato" Then
                        t = "s"
                        Exit For
                    End If
                Next
                If t <> "" Then
                    datarow.Cells("infort").Tag = wp
                    If t = "i" Then
                        datarow.Cells("infort").Value = My.Resources.infortunato
                    Else
                        datarow.Cells("infort").Value = My.Resources.espulso
                    End If
                Else
                    datarow.Cells("infort").Tag = Nothing
                    datarow.Cells("infort").Value = My.Resources.empty
                End If
            Else
                datarow.Cells("infort").Value = My.Resources.empty
                datarow.Cells("infort").Tag = Nothing
                datarow.Cells("presenza").Tag = Nothing
            End If

            datarow.Cells("dtmatch").Value = p.Match.Time.ToString("dd/MM HH:mm")
            datarow.Cells("rating").ToolTipText = CStr(p.Rating)
            datarow.Cells("match").Tag = Nothing
            Select Case p.Variation
                Case 1
                    datarow.Cells("var").Value = My.Resources.import14
                Case 0
                    datarow.Cells("var").Value = My.Resources.w4
                Case Else
                    datarow.Cells("var").Value = My.Resources.export14
            End Select

        End Sub

        Public Shared Sub DrawInfoMatchAndPresenze(dtg As DataGridView, r As Integer, p As LegaObject.Formazione.PlayerFormazione, gg As Integer)

            'Rating player'
            If dtg.Item("match", r).Tag Is Nothing Then
              
                Dim rat As Integer = p.Rating

                If rat > 100 Then rat = 100

                If matchratingkey.ContainsKey(rat) = False Then
                    Dim rec As New Rectangle(0, 0, dtg.Columns("rating").Width, dtg.Rows(r).Height)
                    matchratingkey.Add(rat, SystemFunction.DrawingAndImage.GetImageRating(rec, rat, 100, 0))
                End If

                dtg.Item("rating", r).Value = matchratingkey(rat)

                'Presenza in campo'
                Dim wp As wData.wPlayer = CType(dtg.Item("presenza", r).Tag, wData.wPlayer)

                If wp IsNot Nothing Then

                    Dim key As String = CStr(wp.Titolare).PadLeft(2, CChar("0")) & "-" & CStr(wp.Panchina).PadLeft(2, CChar("0")) & "-" & CStr(webdata.NumSitePlayer).PadLeft(2, CChar("0"))

                    If matchpreskey.ContainsKey(key) = False Then
                        Dim rec As New Rectangle(0, 0, dtg.Columns("presenza").Width, dtg.Rows(r).Height)
                        matchpreskey.Add(key, SystemFunction.DrawingAndImage.GetImagePresence(rec, wp, gg))
                    End If
                    dtg.Item("presenza", r).Value = matchpreskey(key)
                Else
                    dtg.Item("presenza", r).Value = My.Resources.empty
                End If

                'Match'
                Dim keym As String = (p.Squadra & "-" & p.Match.TeamA & "-" & p.Match.TeamB).ToLower

                If matchimgkey.ContainsKey(keym) = False Then
                    Dim rec As New Rectangle(0, 0, dtg.Columns("match").Width, dtg.Rows(r).Height)
                    matchimgkey.Add(keym, SystemFunction.DrawingAndImage.GetImageMatch(rec, p.Squadra, p.Match.TeamA, p.Match.TeamB))
                End If

                dtg.Item("match", r).Value = matchimgkey(keym)
                dtg.Item("match", r).Tag = "e"

            End If

        End Sub

        Public Shared Function ShowPopUpInfoPlayer(dtginfo As DataGridView, wp As wData.wPlayer, x As Integer, y As Integer, iform As iControl.iForm) As Bitmap

            Dim row As Integer = 0
            Dim h As Integer = 0

            dtginfo.RowCount = 0

            For i As Integer = 0 To wp.Info.Count - 1

                'Aggiungo la riga con l'informazione del sito di dello stato di presenza del giocatore'
                dtginfo.Tag = wp
                dtginfo.Rows.Add(New DataGridViewRow)
                dtginfo.Item(1, row).Tag = wp.Team
                dtginfo.Item(1, row).Value = wp.Info(i).Site.Replace("Probabili formazioni ", "Prob. form.")
                If wp.Info(i).Percentage <> -1 AndAlso wp.Info(i).Percentage <> 100 Then
                    dtginfo.Item(2, row).Value = "" & CStr(wp.Info(i).Percentage).PadLeft(2, CChar(" ")) & "%-" & wp.Info(i).State
                Else
                    dtginfo.Item(2, row).Value = wp.Info(i).State
                End If
                dtginfo.Rows(row).Height = dtginfo.RowTemplate.Height
                dtginfo.Item(1, row).Style.ForeColor = Color.FromArgb(50, 50, 50)
                dtginfo.Item(2, row).Style.Font = New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel)
                dtginfo.Rows(row).DefaultCellStyle.BackColor = Color.Transparent
                h = h + dtginfo.Rows(row).Height
                row = row + 1

                'Se e' presente, aggiungo la riga contenente l'informazione testuale'
                If wp.Info(i).Info <> "" Then
                    dtginfo.Rows.Add(New DataGridViewRow)
                    dtginfo.Item(1, row).Tag = wp.Team
                    'dtginfo.Item(1, row).Value = "''" & wp.Info(i).Info.Trim & "''"
                    dtginfo.Item(1, row).Value = wp.Info(i).Info
                    If wp.Info(i).Info.Length > 50 Then
                        dtginfo.Rows(row).Height = dtginfo.RowTemplate.Height * 2
                    Else
                        dtginfo.Rows(row).Height = dtginfo.RowTemplate.Height
                    End If
                    dtginfo.Item(1, row).Style.ForeColor = Color.FromArgb(90, 90, 90)
                    h = h + dtginfo.Rows(row).Height
                    If i < wp.Info.Count - 1 Then dtginfo.Rows(row).Tag = 1 Else dtginfo.Rows(row).Tag = 0
                    dtginfo.Rows(row).DefaultCellStyle.BackColor = Color.Transparent
                    row = row + 1
                End If
            Next

            'Impostazione dimensione popup'
            dtginfo.Height = h + 4
            dtginfo.Location = New Point(x, y - dtginfo.Height \ 2)
            If dtginfo.Top + dtginfo.Height > iform.BY - 3 Then dtginfo.Top = iform.BY - dtginfo.Height - 3
            If dtginfo.Top < iform.TY + 3 Then dtginfo.Top = iform.TY + 3

            dtginfo.CellBorderStyle = DataGridViewCellBorderStyle.None

            'Draw background popup'
            Dim br As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, dtginfo.Width, dtginfo.Height), Color.White, Color.Gainsboro, Drawing2D.LinearGradientMode.Vertical)
            Dim gr As Graphics

            Dim bginfo As New Bitmap(dtginfo.Width, dtginfo.Height)
            gr = Graphics.FromImage(bginfo)
            gr.Clear(Color.White)
            gr.FillRectangle(br, New Rectangle(0, 0, dtginfo.Width, dtginfo.Height))
            gr.Dispose()
            dtginfo.Visible = True

            Return bginfo

        End Function

    End Class
End Namespace