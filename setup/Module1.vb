Imports System.Drawing.Drawing2D

Module Module1

    Public _directorysystemfile As String = "system"

    Public Function DrawGlowText(ByVal gr As Graphics, ByVal Text As String, ByVal TextFont As Font, ByVal FontColor1 As Color, ByVal FontColor2 As Color, ByVal GlowColor As Color, ByVal AntiAliasing As Boolean, ByVal GlowSize As Integer, ByVal ShodowsShift As Integer, ByVal Trasparency As Integer, ByVal Width As Integer, ByVal Height As Integer, ByVal x As Integer, ByVal y As Integer, ByVal Format As StringFormat) As Graphics

        Dim br As Brush = New SolidBrush(Color.FromArgb(Trasparency, GlowColor))

        If GlowSize > 0 Then
            For x1 As Integer = x To GlowSize + x
                For y1 As Integer = y To GlowSize + y
                    gr.DrawString(Text, TextFont, br, New RectangleF(x1, y1, Width, Height), Format)
                Next
            Next
        End If

        br = New SolidBrush(Color.FromArgb(60, 0, 0, 0))

        If ShodowsShift > 0 Then gr.DrawString(Text, TextFont, br, New RectangleF(x + GlowSize \ 2 + ShodowsShift, y + GlowSize \ 2 + ShodowsShift, Width, Height), Format)

        Dim br1 As New LinearGradientBrush(New Rectangle(x, y, Width, Height), FontColor1, FontColor2, LinearGradientMode.Vertical)
        gr.DrawString(Text, TextFont, br1, New RectangleF(x + GlowSize \ 2, y + GlowSize \ 2, Width, Height), Format)
        br.Dispose()
        br1.Dispose()

        Return gr

    End Function

    Sub SetDullRegion(ByVal Dull As Boolean, ByVal frm As Form)

        Dim myGraphicsPath As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

        'Disegno i rettangoli'

        If Dull = True Then
            myGraphicsPath.AddRectangle(New Rectangle(1, 0, frm.Width - 2, frm.Height))
            myGraphicsPath.AddRectangle(New Rectangle(0, 1, 1, frm.Height - 2))
            myGraphicsPath.AddRectangle(New Rectangle(frm.Width - 1, 1, 1, frm.Height - 2))
        Else
            myGraphicsPath.AddRectangle(New Rectangle(0, 0, frm.Width, frm.Height))
        End If

        'Assiocio(l) 'area al controllo'
        frm.Region = New Region(myGraphicsPath)

        myGraphicsPath.Dispose()

    End Sub

    Sub SetDullRegion(ByVal Dull As Boolean, ByVal usr As UserControl)

        Dim myGraphicsPath As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

        'Disegno i rettangoli'

        If Dull = True Then
            myGraphicsPath.AddRectangle(New Rectangle(1, 0, usr.Width - 2, usr.Height))
            myGraphicsPath.AddRectangle(New Rectangle(0, 1, 1, usr.Height - 2))
            myGraphicsPath.AddRectangle(New Rectangle(usr.Width - 1, 1, 1, usr.Height - 2))
        Else
            myGraphicsPath.AddRectangle(New Rectangle(0, 0, usr.Width, usr.Height))
        End If

        'Assiocio(l) 'area al controllo'
        usr.Region = New Region(myGraphicsPath)

        myGraphicsPath.Dispose()

    End Sub

End Module
