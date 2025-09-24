Public Class frmchart

    Dim start As Boolean = True

    Sub DrawDefaultChart()
        gra.SuspundeLayout = True
        gra.Type = iChart.iChart.ChartType.Bar
        gra.ColumnCount = currlega.Settings.NumberOfDays + 1
        gra.RowCount = 1
        gra.SuspundeLayout = False
        gra.Title.Font = New Font("tahoma", 11, FontStyle.Regular,GraphicsUnit.Pixel)
        gra.Title.Visible = False
        gra.BorderSize = 0
        gra.Axsis(iChart.Axsis.Axsis.X).MinDivisionFont = New Font("tahoma", 11, FontStyle.Regular,GraphicsUnit.Pixel)
        gra.Axsis(iChart.Axsis.Axsis.X).TextAlignment = iChart.Axsis.Alignment.Center
        gra.Axsis(iChart.Axsis.Axsis.Y).MinDivisionFont = New Font("tahoma", 11, FontStyle.Regular,GraphicsUnit.Pixel)
        gra.Series.Item(0).Style.InternalBorderSize = 1
        gra.Series.Item(0).Style.BorderSize = 1
        If AppSett.Personal.Theme.FlatStyle Then
            gra.Series.Item(0).Style.Brush.Color1 = Color.CornflowerBlue
        Else
            gra.Series.Item(0).Style.Brush.Color1 = Color.FromArgb(30, 30, 240)
            gra.Series.Item(0).Style.Brush.Color2 = Color.CornflowerBlue
        End If
        For i As Integer = 0 To gra.ColumnCount - 1
            gra.Item(0, i) = ""
            gra.Axsis(iChart.Axsis.Axsis.X).Item(i).Value = CStr(i + 1)
        Next
        gra.Max = 10
        gra.AutoScale = iChart.iChart.eAutoScale.None
        gra.Info.Type = iChart.Info.eType.Value
        gra.Info.Font = New Font("tahoma", 11, FontStyle.Bold,GraphicsUnit.Pixel)
        gra.Info.Visible = True
        gra.Draw()
    End Sub

    Sub LoadChart()
        Dim d As List(Of Double) = currlega.GetDataChart(CStr(gra.Tag), txtsearch1.Text)
        For i As Integer = 0 To d.Count - 1
            If d(i) > -100 Then
                gra.Item(0, i) = CStr(d(i))
            Else
                gra.Item(0, i) = ""
            End If
        Next
        Dim auto As Boolean = False
        Dim max As Integer = 10
        Select Case CStr(gra.Tag)
            Case "gs" : max = 10 : auto = True
            Case "gf" : max = 10 : auto = True
            Case "amm" : max = 2 : auto = False
            Case "esp" : max = 2 : auto = False
            Case "rigt" : max = 10 : auto = True
            Case "rigt" : max = 10 : auto = True
            Case "rigs" : max = 10 : auto = True
            Case "rigp" : max = 10 : auto = True
            Case "p.g." : max = 2 : auto = False
            Case "vt." : max = 10 : auto = True
            Case "pt." : max = 10 : auto = True
        End Select
        If auto Then
            gra.AutoScale = iChart.iChart.eAutoScale.MaxAutoScale
        Else
            gra.AutoScale = iChart.iChart.eAutoScale.None
        End If
        gra.Max = max
        gra.Draw()
    End Sub

    Sub SetTheme()

    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Private Sub frmchart_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call SetTheme()
        start = False
    End Sub
End Class
