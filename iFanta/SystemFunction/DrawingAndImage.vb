Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace SystemFunction
    Public Class DrawingAndImage

        Private Declare Auto Function BitBlt Lib "gdi32.dll" _
(ByVal pHdc As IntPtr, ByVal iX As Integer, _
ByVal iY As Integer, ByVal iWidth As Integer, _
ByVal iHeight As Integer, ByVal pHdcSource As IntPtr, _
ByVal iXSource As Integer, ByVal iYSource As Integer, _
ByVal dw As System.Int32) As Boolean

        Private Const SRC As Integer = &HCC0020

        Public Shared Function GetImageAreaForm(ByVal frm As Form, ByVal rec As Rectangle) As Bitmap

            'frm.Refresh()

            Dim g As Graphics = frm.CreateGraphics
            Dim ibitMap As New Bitmap(rec.Width, rec.Height, g)
            Dim iBitMap_gr As Graphics = Graphics.FromImage(ibitMap)
            Dim iBitMap_hdc As IntPtr = iBitMap_gr.GetHdc
            Dim me_hdc As IntPtr = g.GetHdc

            BitBlt(iBitMap_hdc, 0, 0, rec.Width, rec.Height, me_hdc, rec.Left, rec.Top, SRC)
            g.ReleaseHdc(me_hdc)
            iBitMap_gr.ReleaseHdc(iBitMap_hdc)

            Return ibitMap

        End Function

        Public Shared Function ConvertDatagridToImage(ByVal dtg As DataGridView) As Bitmap

            dtg.Refresh()
            dtg.Select()

            Dim g As Graphics = dtg.CreateGraphics
            Dim w As Integer = dtg.Width
            Dim h As Integer = dtg.Height
            If dtg.DisplayedRowCount(False) < dtg.RowCount AndAlso (dtg.ScrollBars = ScrollBars.Both OrElse dtg.ScrollBars = ScrollBars.Vertical) Then
                w = w - 20
            End If
            If dtg.DisplayedColumnCount(False) < dtg.ColumnCount AndAlso (dtg.ScrollBars = ScrollBars.Both OrElse dtg.ScrollBars = ScrollBars.Horizontal) Then
                h = h - 20
            End If
            Dim ibitMap As New Bitmap(w, h, g)
            Dim iBitMap_gr As Graphics = Graphics.FromImage(ibitMap)
            Dim iBitMap_hdc As IntPtr = iBitMap_gr.GetHdc
            Dim me_hdc As IntPtr = g.GetHdc

            BitBlt(iBitMap_hdc, 0, 0, w, _
            h, me_hdc, 0, 0, SRC)
            g.ReleaseHdc(me_hdc)
            iBitMap_gr.ReleaseHdc(iBitMap_hdc)

            Return ibitMap

        End Function

        Public Shared Sub SaveDatagridToImage(ByVal dg As DataGridView, ByVal sFilePath As String)
            Dim ibitMap As Bitmap = ConvertDatagridToImage(dg)
            ibitMap.Save(sFilePath, Imaging.ImageFormat.Png)
        End Sub

        Public Shared Function GetBackgroundImage(ByVal w As Integer, ByVal H As Integer) As Bitmap

            Dim b1 As Bitmap

            b1 = New Bitmap(w, H)
            Dim gr As Graphics

            gr = Graphics.FromImage(b1)

            gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

            Dim br2 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, w, H \ 2 - 1), Color.White, Color.FromArgb(255, 255, 255), Drawing2D.LinearGradientMode.Vertical)

            For i As Integer = 0 To 2
                Dim br1 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, w, H \ 2 - 1), Color.White, Color.FromArgb(30 + i * 10, 0, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                gr.FillPath(br1, GetBorderDullPath(gr, New Rectangle(i, i, w - i * 2, H - i * 2 - 20), 16 - i * 2))
            Next

            gr.FillPath(Brushes.White, GetBorderDullPath(gr, New Rectangle(3, 3, w - 6, H - 28), 10))
            gr.FillPath(br2, GetBorderDullPath(gr, New Rectangle(4, 4, w - 8, H - 30), 8))

            gr.FillRectangle(Brushes.White, New Rectangle(0, 0, w, H \ 2))

            Return CType(b1.Clone, Bitmap)

        End Function

        Public Shared Function GetBackgroundImage() As Bitmap

            Dim img1 As Bitmap = Nothing
            Dim subdir As String = "Normal"
            If AppSett.Personal.Theme.FlatStyle Then subdir = "Flat"
            Dim dirtheme As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name
            Dim fname1 As String = dirtheme & "\" & subdir & "\back.png"
            Dim fname2 As String = dirtheme & "\" & subdir & "\back.jpg"
            Dim fname3 As String = dirtheme & "\back.png"
            Dim fname4 As String = dirtheme & "\back.jpg"

            If IO.Directory.Exists(dirtheme) Then
                If IO.File.Exists(fname1) Then
                    img1 = New Bitmap(fname1)
                ElseIf IO.File.Exists(fname2) Then
                    img1 = New Bitmap(fname2)
                ElseIf IO.File.Exists(fname3) Then
                    img1 = New Bitmap(fname3)
                ElseIf IO.File.Exists(fname4) Then
                    img1 = New Bitmap(fname4)
                End If
            Else
                img1 = My.Resources.back
            End If

            Return img1

        End Function

        Public Shared Function GetImageMatch(ByVal r As Rectangle, ByVal Team As String, TeamA As String, ByVal TeamB As String) As Bitmap

            If Team <> "" AndAlso TeamA <> "" AndAlso TeamB <> "" Then

                Dim cls As Color = Color.FromArgb(205, 0, 0)
                Dim clu As Color = Color.FromArgb(50, 50, 50)
                Dim ci1 As Color = Color.White
                Dim ci2 As Color = Color.FromArgb(210, 210, 210)
                Dim cp1 As Color = Color.White
                Dim cp2 As Color = Color.FromArgb(210, 210, 210)
                Dim padtop As Integer = 2
                Dim padleft As Integer = 3
                Dim h As Integer = r.Height - padtop * 2 - 1
                Dim w As Integer = r.Width - padleft * 2
                Dim x As Integer = r.Left + padleft
                Dim y As Integer = r.Top + padtop
                Dim wi As Integer = w - 2
                Dim keym As String = "?-?-?"
                Dim fs As String = "n-"

                If AppSett.Personal.Theme.FlatStyle Then fs = "f-"

                Dim bm As New Bitmap(r.Width, r.Height)
                Dim gr As Graphics = Graphics.FromImage(bm)

                keym = (Team & "-" & TeamA & "-" & TeamB).ToLower

                Dim clta As Color = clu
                Dim cltb As Color = clu
                Dim sta As String = TeamA.Substring(0, 3)
                Dim stb As String = TeamB.Substring(0, 3)
                Dim bc As Color = Color.FromArgb(150, 150, 150)

                If Team = TeamA Then
                    cp1 = Color.FromArgb(150, 190, 255)
                    If AppSett.Personal.Theme.FlatStyle = False Then
                        cp2 = Color.FromArgb(100, 170, 250)
                    Else
                        cp2 = cp1
                    End If
                    clta = cls
                    bc = Color.FromArgb(0, 0, 185)
                Else
                    cp1 = Color.FromArgb(255, 255, 0)
                    If AppSett.Personal.Theme.FlatStyle = False Then
                        cp2 = Color.FromArgb(255, 170, 0)
                    Else
                        cp2 = cp1
                    End If
                    cltb = cls
                    bc = Color.FromArgb(200, 100, 0)
                End If

                Dim bi1 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, w - 2, h - 1), ci1, ci2, Drawing2D.LinearGradientMode.Vertical)
                Dim bi2 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, w - 2, h - 1), cp1, cp2, Drawing2D.LinearGradientMode.Vertical)

                gr.FillRectangle(New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(x, y, w, h))
                gr.FillRectangle(bi1, New Rectangle(x + 1, y + 1, w - 2, h - 2))
                gr.FillRectangle(bi2, New Rectangle(x + 1, y + 1, wi, h - 2))
                If AppSett.Personal.Theme.FlatStyle = False Then gr.FillRectangle(New SolidBrush(Color.FromArgb(80, 255, 255, 255)), New Rectangle(x + 1, y + 1, w - 2, CInt((h - 2) / 2)))

                Dim ft As New StringFormat
                ft.Alignment = StringAlignment.Center
                'e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                gr.DrawString("-", New Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Pixel), New SolidBrush(clu), r.Width \ 2 + 2, 2, ft)
                ft.Alignment = StringAlignment.Far
                gr.DrawString(sta, New Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Pixel), New SolidBrush(clta), r.Width \ 2 - 1, 2, ft)
                ft.Alignment = StringAlignment.Near
                gr.DrawString(stb, New Font("Arial", 9, FontStyle.Regular, GraphicsUnit.Pixel), New SolidBrush(cltb), r.Width \ 2 + 1, 2, ft)

                keym = keym.Replace("?-?-?", "unkm")
                bm.Save(SystemFunction.FileAndDirectory.GetTempImageDirectory & "\match-" & fs & keym & ".tmp", System.Drawing.Imaging.ImageFormat.Png)

                Return CType(bm.Clone, Bitmap)

            Else
                Return My.Resources.empty
            End If
            
        End Function

        Public Shared Function GetImagePresence(ByVal r As Rectangle, ByVal pres As wData.wPlayer, ByVal giornata As Integer) As Bitmap

            Dim bm As New Bitmap(r.Width, r.Height)
            Dim gr As Graphics = Graphics.FromImage(bm)

            'gr.FillRectangle(Brushes.White, r)

            Dim padtop As Integer = 2
            Dim padleft As Integer = 3
            Dim h As Integer = r.Height - padtop * 2 - 1
            Dim w As Integer = r.Width - padleft * 2
            Dim x As Integer = r.Left + padleft
            Dim y As Integer = r.Top + padtop
            Dim wi As Integer = w - 2
            Dim cu1 As Color = Color.White
            Dim cu2 As Color = Color.FromArgb(210, 210, 210)
            Dim ct1 As Color = Color.FromArgb(80, 140, 255)
            Dim ct2 As Color = Color.FromArgb(30, 80, 255)
            Dim cp1 As Color = Color.FromArgb(255, 255, 0)
            Dim cp2 As Color = Color.FromArgb(255, 100, 0)
            Dim keym As String = "00-00-00"
            Dim fs As String = "n-"

            If AppSett.Personal.Theme.FlatStyle Then fs = "f-"

            Dim ratt As Single = 0
            Dim ratp As Single = 0
            Dim nums As Integer = webdata.NumSitePlayer

            If pres IsNot Nothing Then
                Dim ni As Integer = pres.Info.Count
                If ni >= nums Then nums = ni
                ratt = CSng(pres.Titolare / nums)
                ratp = CSng((pres.Titolare + pres.Panchina) / nums)
                keym = CStr(pres.Titolare).PadLeft(2, CChar("0")) & "-" & CStr(pres.Panchina).PadLeft(2, CChar("0")) & "-" & CStr(webdata.NumSitePlayer).PadLeft(2, CChar("0"))
            End If

            If AppSett.Personal.Theme.FlatStyle = True Then cu2 = cu1 : ct1 = ct2 : cp1 = cp2

            Dim bi1 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, w - 2, h - 1), cu1, cu2, Drawing2D.LinearGradientMode.Vertical)
            Dim bi2 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, w - 2, h - 1), cp1, cp2, Drawing2D.LinearGradientMode.Vertical)
            Dim bi3 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, w - 2, h - 1), ct1, ct2, Drawing2D.LinearGradientMode.Vertical)

            gr.FillRectangle(New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(x, y, w, h))
            gr.FillRectangle(bi1, New Rectangle(x + 1, y + 1, w - 2, h - 2))
            If ratp > 0 Then gr.FillRectangle(bi2, New Rectangle(x + 1, y + 1, CInt(wi * ratp), h - 2))
            If ratt > 0 Then gr.FillRectangle(bi3, New Rectangle(x + 1, y + 1, CInt(wi * ratt), h - 2))
            If AppSett.Personal.Theme.FlatStyle = False Then gr.FillRectangle(New SolidBrush(Color.FromArgb(80, 255, 255, 255)), New Rectangle(x + 1, y + 1, w - 2, CInt((h - 2) / 2)))

            bm.Save(SystemFunction.FileAndDirectory.GetTempImageDirectory & "\pres-" & fs & keym & ".tmp")

            Return CType(bm.Clone, Bitmap)

        End Function

        Public Shared Function GetImageRating(ByVal r As Rectangle, ByVal rat As Integer, ByVal ratmax As Integer, ByVal type As Integer) As Bitmap

            Dim bm As New Bitmap(r.Width, r.Height)
            Dim gr As Graphics = Graphics.FromImage(bm)
            Dim fname As String = ""
            Dim fs As String = "n-"

            If AppSett.Personal.Theme.FlatStyle Then fs = "f-"

            If type = 1 Then
                fname = "rat2-" & fs & CStr(rat) & ".tmp"
            Else
                fname = "rat-" & fs & CStr(rat) & ".tmp"
            End If

            fname = SystemFunction.FileAndDirectory.GetTempImageDirectory & "\" & fname

            If IO.File.Exists(fname) = False Then
                'gr.FillRectangle(Brushes.White, r)

                Dim padtop As Integer = 2
                Dim padleft As Integer = 3
                Dim h As Integer = r.Height - padtop * 2 - 1
                Dim w As Integer = r.Width - padleft * 2
                Dim x As Integer = r.Left + padleft
                Dim y As Integer = r.Top + padtop
                Dim wi As Integer = w - 2
                Dim ci1 As Color = Color.White
                Dim ci2 As Color = Color.FromArgb(210, 210, 210)
                Dim cp1 As Color = Color.White
                Dim cp2 As Color = Color.FromArgb(210, 210, 210)

                Select Case rat
                    Case Is < 1
                        cp1 = Color.White
                        cp2 = Color.FromArgb(210, 210, 210)
                    Case Else
                        cp1 = Color.FromArgb(0, 255, 0)
                        cp2 = Color.FromArgb(0, 180, 0)
                        wi = CInt((w - 2) * rat / ratmax)
                End Select

                If AppSett.Personal.Theme.FlatStyle = True Then ci2 = ci1 : cp1 = cp2

                Dim bi1 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, w - 2, h - 1), ci1, ci2, Drawing2D.LinearGradientMode.Vertical)
                Dim bi2 As New Drawing2D.LinearGradientBrush(New Rectangle(x + 1, y + 1, w - 2, h - 1), cp1, cp2, Drawing2D.LinearGradientMode.Vertical)

                If type = 0 Then
                    gr.FillRectangle(New SolidBrush(Color.FromArgb(80, 80, 80)), New Rectangle(x, y, w, h))
                    gr.FillRectangle(bi1, New Rectangle(x + 1, y + 1, w - 2, h - 2))
                    gr.FillRectangle(bi2, New Rectangle(x + 1, y + 1, wi, h - 2))
                    If AppSett.Personal.Theme.FlatStyle = False Then gr.FillRectangle(New SolidBrush(Color.FromArgb(80, 255, 255, 255)), New Rectangle(x + 1, y + 1, w - 2, CInt((h - 2) / 2)))
                Else
                    gr.FillRectangle(New SolidBrush(Color.FromArgb(120, 120, 120)), New Rectangle(x, y, wi + 2, h))
                    gr.FillRectangle(bi2, New Rectangle(x + 1, y + 1, wi, h - 2))
                    If AppSett.Personal.Theme.FlatStyle = False Then gr.FillRectangle(New SolidBrush(Color.FromArgb(80, 255, 255, 255)), New Rectangle(x + 1, y + 1, wi - 2, CInt((h - 2) / 2)))
                End If

                bm.Save(fname)
            Else
                bm = CType(Image.FromFile(fname), Bitmap)
            End If

            Return CType(bm.Clone, Bitmap)

        End Function

        Public Shared Function ConvDisable(ByVal img As Image, ByVal Trasparency As Single) As Bitmap

            If Trasparency = -1 Then Trasparency = 0.5

            If Trasparency < 1 Then
                Dim value()() As Single = {New Single() {1, 0, 0, 0, 0}, _
                    New Single() {0, 1, 0, 0, 0}, _
                    New Single() {0, 0, 1, 0, 0}, _
                    New Single() {0, 0, 0, Trasparency, 0}, _
                    New Single() {0, 0, 0, 0, 1}}

                Dim ColorMatrix As New Imaging.ColorMatrix(value)
                Dim ImageAttr As New Imaging.ImageAttributes()
                ImageAttr.SetColorMatrix(ColorMatrix, Imaging.ColorMatrixFlag.Default)

                Dim b1 As New Bitmap(img.Width, img.Height)
                Dim gr As Graphics

                gr = Graphics.FromImage(b1)

                Try
                    gr.DrawImage(img, New Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ImageAttr)
                Catch ex As Exception

                End Try

                gr.Dispose()
                ImageAttr.Dispose()

                Return b1

                b1.Dispose()
            Else
                Return CType(img, Bitmap)
            End If

        End Function

        Public Shared Function GetRuoloForeColorDisable(ByVal Ruolo As String) As Color
            Dim cl As Color = Color.White
            Select Case Ruolo
                Case "P" : cl = Color.FromArgb(255, 180, 130)
                Case "D" : cl = Color.FromArgb(130, 255, 130)
                Case "C" : cl = Color.FromArgb(255, 130, 130)
                Case "A" : cl = Color.FromArgb(130, 130, 255)
            End Select
            Return cl
        End Function

        Public Shared Function GetBorderDullPath1(ByVal gr As Graphics, ByVal rect As Rectangle, ByVal d As Integer) As Drawing2D.GraphicsPath

            Dim Bottom As Integer = rect.Y + rect.Height - d
            Dim Right As Integer = rect.X + rect.Width - d

            Dim path As New GraphicsPath

            If d > 0 Then
                With path
                    'AddRectangle(New Rectangle(0, rect.Height \ 2, rect.Width, rect.Height - d))
                    .AddArc(New Rectangle(rect.X, rect.Y, d, d), 180, 90)
                    .AddArc(New Rectangle(Right, rect.Y, d, d), 270, 90)
                    .AddArc(New Rectangle(Right, Bottom, d, d), 0, 90)
                    .AddArc(New Rectangle(rect.X, Bottom, d, d), 90, 90)
                    .CloseFigure()
                End With
            Else
                path.AddRectangle(rect)
            End If

            Return path

        End Function

        Public Shared Function GetBorderDullPath(ByVal gr As Graphics, ByVal rect As Rectangle, ByVal d As Integer) As Drawing2D.GraphicsPath

            Dim Bottom As Integer = rect.Y + rect.Height - d
            Dim Right As Integer = rect.X + rect.Width - d

            Dim path As New GraphicsPath

            If d > 0 Then
                With path
                    'AddRectangle(New Rectangle(0, rect.Height \ 2, rect.Width, rect.Height - d))
                    .AddArc(New Rectangle(rect.X, rect.Y + rect.Height \ 2, d, d), 180, 90)
                    .AddArc(New Rectangle(Right, rect.Y + rect.Height \ 2, d, d), 270, 90)
                    .AddArc(New Rectangle(Right, Bottom, d, d), 0, 90)
                    .AddArc(New Rectangle(rect.X, Bottom, d, d), 90, 90)
                    .CloseFigure()
                End With
            Else
                path.AddRectangle(rect)
            End If

            Return path

        End Function

        Public Shared Function DrawGlowText3(ByVal Text As String, ByVal TextFont As Font, ByVal FontColor1 As Color, ByVal FontColor2 As Color, ByVal Trasparency As Integer, ByVal Local As Boolean, ByVal format As StringFormat, Optional ByVal Width As Integer = -1, Optional ByVal Height As Integer = -1) As Bitmap
            Return DrawGlowText3(Text, TextFont, FontColor1, FontColor2, Color.Black, Trasparency, True, Local, format, Width, Height)
        End Function

        Public Shared Function DrawGlowText3(ByVal Text As String, ByVal TextFont As Font, ByVal FontColor1 As Color, ByVal FontColor2 As Color, ByVal GlowColor As Color, ByVal Trasparency As Integer, ByVal ShowShadows As Boolean, ByVal Local As Boolean, ByVal format As StringFormat, Optional ByVal Width As Integer = -1, Optional ByVal Height As Integer = -1) As Bitmap

            Dim bm As New Bitmap(50, 50)

            Try

                Dim gr As Graphics = Graphics.FromImage(bm)
                Dim br As Brush = New SolidBrush(Color.FromArgb(Trasparency, GlowColor.R, GlowColor.G, GlowColor.B))

                Dim good As Boolean = False

                bm = New Bitmap(Width, Height)
                gr = Graphics.FromImage(bm)

                gr.SmoothingMode = SmoothingMode.AntiAlias
                gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

                Dim e As Integer = 3
                'If ShowShadows = False Then e = 1

                For x As Integer = -2 To e
                    For y As Integer = -2 To e
                        If format Is Nothing Then
                            gr.DrawString(Text, TextFont, br, New RectangleF(x, y, Width, Height))
                        Else
                            gr.DrawString(Text, TextFont, br, New RectangleF(x, y, Width, Height), format)
                        End If

                    Next
                Next

                gr.SmoothingMode = SmoothingMode.AntiAlias
                gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

                Dim br1 As New LinearGradientBrush(New Rectangle(0, 0, Width, Height), FontColor1, FontColor2, LinearGradientMode.Vertical)
                If format Is Nothing Then
                    gr.DrawString(Text, TextFont, br1, New RectangleF(0, 0, Width, Height))
                Else
                    gr.DrawString(Text, TextFont, br1, New RectangleF(0, 0, Width, Height), format)
                End If
                'gr.DrawString(Text, TextFont, br1, 0, 0)
                gr.Dispose()
                br.Dispose()
                br1.Dispose()

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try

            Return bm

        End Function

        Public Shared Function GetImageForma(ByVal f As LegaObject.Formazione, ByVal Filter As String, ByVal ToolTip As Boolean, ByVal W As Integer, ByVal H As Integer, ByVal ImgBack As Bitmap) As Bitmap

            Dim gr As Graphics
            Dim b1 As Bitmap
            Dim dy As Integer = 18
            Dim dyt As Integer = 35
            Dim dyp As Integer = 209
            Dim rh As Integer = 14
            Dim farms As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & f.IdTeam & "-24x24.png"
            Dim farmsl As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & f.IdTeam & "-200x200.jpg"
            Dim ft As New Font("Tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel)
            Dim r As Integer = -1
            Dim hb As Integer = 0
            Dim padtext As Integer = 0

            b1 = New Bitmap(W, H)

            f.Players = LegaObject.Formazione.Sort(f.Players, "forma", False)

            Try
                gr = Graphics.FromImage(b1)
                gr.Clear(Color.White)
                gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

                If ToolTip Then

                    padtext = 7

                    gr.DrawImage(ImgBack, 0, 0)

                    Dim br2 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H - 1), Color.FromArgb(160, 160, 160), Color.DimGray, Drawing2D.LinearGradientMode.Vertical)
                    Dim br3 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W - 3, H - 3), Color.White, Color.FromArgb(230, 230, 230), Drawing2D.LinearGradientMode.Vertical)

                    For i As Integer = 0 To 2
                        Dim br1 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H - 1), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(30 + i * 10, 0, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                        gr.FillPath(br1, GetBorderDullPath1(gr, New Rectangle(i, i, W - i * 2, H - i * 2 - 20), 16 - i * 2))
                    Next

                    gr.FillPath(br2, GetBorderDullPath1(gr, New Rectangle(3, 3, W - 6, H - 28), 10))
                    gr.FillPath(Brushes.White, GetBorderDullPath1(gr, New Rectangle(4, 4, W - 8, H - 30), 12))

                Else

                    Dim br2 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H \ 2 - 1), Color.White, Color.White, Drawing2D.LinearGradientMode.Vertical)
                    Dim br3 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H), Color.White, Color.FromArgb(120, 255, 255, 0), Drawing2D.LinearGradientMode.Vertical)

                    If f.NumberPlayerInCampo < 11 AndAlso f.NumberPlayerInCampo > 0 Then
                        For i As Integer = 0 To 2
                            Dim br1 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H \ 2 - 1), Color.White, Color.FromArgb(30 + i * 10, 255, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                            gr.FillPath(br1, GetBorderDullPath(gr, New Rectangle(i, i, W - i * 2, H - i * 2 - 20), 16 - i * 2))
                        Next
                    Else
                        For i As Integer = 0 To 2
                            Dim br1 As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, W, H \ 2 - 1), Color.White, Color.FromArgb(30 + i * 10, 0, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                            gr.FillPath(br1, GetBorderDullPath(gr, New Rectangle(i, i, W - i * 2, H - i * 2 - 20), 16 - i * 2))
                        Next
                    End If

                    gr.FillPath(Brushes.White, GetBorderDullPath(gr, New Rectangle(3, 3, W - 6, H - 28), 10))
                    gr.FillPath(br2, GetBorderDullPath(gr, New Rectangle(4, 4, W - 8, H - 30), 8))

                    gr.FillRectangle(Brushes.White, New Rectangle(0, 0, W, H \ 2))

                    If f.NumberPlayerInCampo < 11 AndAlso f.NumberPlayerInCampo > 0 Then
                        gr.FillPath(br3, GetBorderDullPath(gr, New Rectangle(4, 4 - H, W - 8, H * 2 - 30), 8))
                    End If
                End If


                Dim f1 As New StringFormat
                Dim f2 As New StringFormat
                Dim f3 As New StringFormat

                f1.Alignment = StringAlignment.Center
                f2.Alignment = StringAlignment.Far
                f3.Alignment = StringAlignment.Near

                gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

                If Filter <> "" Then
                    gr.DrawString(CStr(f.Giornata), New Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Black, 3, 5)
                Else
                    If IO.File.Exists(farms) Then
                        gr.DrawImage(Image.FromFile(farms), 3 + padtext, 3 + padtext)
                    End If
                    If IO.File.Exists(farmsl) AndAlso ToolTip = False Then
                        gr.DrawImage(ConvDisable(Image.FromFile(farmsl), 0.15), W - 145 - padtext, H - 170 - padtext, 140, 140)
                    End If
                End If

                gr.DrawString(f.Nome, New Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.Red, New Rectangle(28 + padtext, 2 + padtext, 140, 20))
                gr.DrawString(CStr(f.Pt), New Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.RoyalBlue, W - 15 - padtext, 2 + padtext, f2)
                gr.TextRenderingHint = Drawing.Text.TextRenderingHint.SystemDefault
                gr.DrawString(f.Allenatore, New Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(50, 50, 50)), New Rectangle(28 + padtext, 18 + padtext, 130, 15))
                gr.DrawString(f.Modulo, New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(50, 50, 50)), W - 15 - padtext, 18 + padtext, f2)
                gr.DrawLine(Pens.Silver, 8 + padtext, 33 + padtext, W - 10 - padtext, 33 + padtext)

                gr.DrawString("Titolari", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(255, 100, 0)), 9 + padtext, dyt + padtext)
                gr.DrawString("Panchina", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(0, 100, 255)), 9 + padtext, dyp + padtext)

                Dim nfc As Color = Color.FromArgb(50, 50, 50)

                For i As Integer = 0 To 11 + currlega.Settings.NumberOfReserve
                    Dim y As Integer = 0
                    If i < 11 Then
                        y = dyt + (i + 1) * rh + 3
                    Else
                        y = dyp + (i - 11) * rh + 3
                    End If
                    gr.DrawLine(Pens.Silver, 10 + padtext, y + rh - 1 + padtext, W - 15 - padtext, y + rh - 1 + padtext)
                Next
                For i As Integer = 0 To f.Players.Count - 1

                    If f.Players(i).Type <> 0 Then

                        Dim rfc As Color = SystemFunction.General.GetRuoloForeColor(f.Players(i).Ruolo)
                        Dim cfc As Color = Color.Blue
                        Dim alpha1 As Integer = 255
                        Dim alpha2 As Integer = 255
                        Dim traspimg As Single = 1
                        Dim y As Integer = 0

                        If f.Players(i).Type > 0 Then
                            r += 1
                        End If

                        If f.Players(i).InCampo = 0 AndAlso f.Players(i).Type = 1 Then
                            rfc = SystemFunction.General.GetRuoloForeColorDisable(f.Players(i).Ruolo)
                            nfc = Color.FromArgb(150, 150, 150)
                            alpha1 = 150 : alpha2 = 70 : traspimg = 0.5
                            cfc = Color.FromArgb(150, 150, 150)
                        Else
                            nfc = Color.FromArgb(50, 50, 50)
                        End If
                        Select Case f.Players(i).Type
                            Case 1 : y = dyt + (f.Players(i).IdFormazione) * rh + 3 + padtext
                            Case 2 : y = dyp + (f.Players(i).IdFormazione - 11) * rh + 3 + padtext
                        End Select

                        If (f.Players(i).IdFormazione > 0 AndAlso f.Players(i).IdFormazione <= 11 + currlega.Settings.NumberOfReserve) Then

                            If f.Players(i).Jolly = 1 Then
                                gr.DrawImage(ConvDisable(My.Resources.star10, traspimg), gr.MeasureString(f.Players(i).Nome, ft, 200).Width + 23, y)
                            End If
                            If f.Players(i).InCampo = 1 AndAlso (f.Players(i).Type = 2) Then
                                gr.DrawString("*", New Font("Tahoma", 10, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.DimGray), 8 + padtext, y + 3, f1)
                            End If
                            gr.DrawString(f.Players(i).Ruolo, ft, New SolidBrush(rfc), 16 + padtext, y - 1, f1)
                            gr.DrawString(f.Players(i).Nome, ft, New SolidBrush(nfc), 23 + padtext, y - 1)
                            If f.Players(i).Dati.Amm > 0 Then
                                gr.DrawImage(ConvDisable(My.Resources.ammonito, traspimg), 108 + padtext, y)
                            End If
                            'Visualizzazione espulsione'
                            If f.Players(i).Dati.Esp > 0 Then
                                gr.DrawImage(ConvDisable(My.Resources.espulso, traspimg), 108 + padtext, y)
                            End If
                            If f.Players(i).Dati.Ass > 0 Then
                                gr.DrawImage(ConvDisable(My.Resources.ass, traspimg), 122 + padtext, y)
                                gr.DrawString(CStr(f.Players(i).Dati.Ass), ft, New SolidBrush(nfc), 133 + padtext, y - 1)
                            End If

                            If f.Players(i).Dati.AutG > 0 Then
                                gr.DrawImage(ConvDisable(My.Resources.autogoal16, traspimg), 143, y)
                                gr.DrawString(CStr(f.Players(i).Dati.AutG), ft, New SolidBrush(nfc), 156, y - 1)
                            End If
                            If f.Players(i).Dati.Gs > 0 OrElse f.Players(i).Dati.Gf > 0 Then
                                gr.DrawImage(ConvDisable(My.Resources.pallone161, traspimg), 167, y)
                                gr.DrawString(CStr(f.Players(i).Dati.Gs + f.Players(i).Dati.Gf), ft, New SolidBrush(nfc), 180, y - 1)
                            End If
                            If f.Players(i).Dati.RigP > 0 Then
                                gr.DrawImage(ConvDisable(My.Resources.rigp, traspimg), 191, y)
                                gr.DrawString(CStr(f.Players(i).Dati.RigP), ft, New SolidBrush(nfc), 204, y - 1)
                            End If
                            If f.Players(i).Dati.RigS > 0 Then
                                gr.DrawImage(ConvDisable(My.Resources.rigs, traspimg), 191, y)
                                gr.DrawString(CStr(f.Players(i).Dati.RigS), ft, New SolidBrush(nfc), 204, y - 1)
                            End If

                            If f.Players(i).Dati.Pt > -10 Then
                                gr.DrawString(CStr(f.Players(i).Dati.Pt), ft, New SolidBrush(cfc), W - 15 - padtext, y - 1, f2)
                            Else
                                If f.Players(i).Dati.Pt = -20 Then
                                    gr.DrawString("s.v.", ft, New SolidBrush(cfc), W - 15 - padtext, y - 1, f2)
                                End If
                            End If

                        End If

                    End If

                Next
                hb = dyp + rh * currlega.Settings.NumberOfReserve

                hb = hb + 23

                Dim sb1 As New SolidBrush(Color.FromArgb(80, 80, 80))
                Dim sb2 As New SolidBrush(Color.FromArgb(180, 180, 180))

                If currlega.Settings.Bonus.EnableBonusDefense Then
                    Dim strb As String = "* Nessun bonus difesa"
                    Dim sb As SolidBrush = sb2
                    If f.BonusDifesa > 0 Then
                        sb = sb1
                        If currlega.Settings.Bonus.BonusDefense(3) = f.BonusDifesa * 10 Then
                            strb = "* Bonus difesa a 3 (+" & currlega.Settings.Bonus.BonusDefense(3) / 10 & "pt)"
                        ElseIf currlega.Settings.Bonus.BonusDefense(4) = f.BonusDifesa * 10 Then
                            strb = "* Bonus difesa a 4 (+" & currlega.Settings.Bonus.BonusDefense(4) / 10 & "pt)"
                        ElseIf currlega.Settings.Bonus.BonusDefense(5) = f.BonusDifesa * 10 Then
                            strb = "* Bonus difesa a 5 (+" & currlega.Settings.Bonus.BonusDefense(5) / 10 & "pt)"
                        End If
                    End If
                    gr.DrawString(strb, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), sb, 10 + padtext, hb)
                    If f.NumberPlayerInCampo > 0 Then
                        If f.NumberPlayerInCampo = 11 Then
                            gr.DrawString(f.NumberPlayerInCampo & "/11", New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(80, 80, 80)), W - 15 - padtext, hb, f2)
                        Else
                            gr.DrawString(f.NumberPlayerInCampo & "/11", New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(255, 0, 0)), W - 15 - padtext, hb, f2)
                        End If
                    End If
                    hb = hb + 17
                End If

                If currlega.Settings.Bonus.EnableCenterField Then
                    Dim strb As String = "* Nessun bonus centrocampo"
                    Dim sb As SolidBrush = sb2
                    If f.BonusCentroCampo > 0 Then
                        sb = sb1
                        If currlega.Settings.Bonus.BonusCenterField(3) = f.BonusCentroCampo * 10 Then
                            strb = "* Bonus centrocampo a 3 (+" & currlega.Settings.Bonus.BonusCenterField(3) / 10 & "pt)"
                        ElseIf currlega.Settings.Bonus.BonusCenterField(4) = f.BonusCentroCampo * 10 Then
                            strb = "* Bonus centrocampo a 4 (+" & currlega.Settings.Bonus.BonusCenterField(4) / 10 & "pt)"
                        ElseIf currlega.Settings.Bonus.BonusCenterField(5) = f.BonusCentroCampo * 10 Then
                            strb = "* Bonus centrocampo a 5 (+" & currlega.Settings.Bonus.BonusCenterField(5) / 10 & "pt)"
                        End If
                    End If
                    gr.DrawString(strb, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), sb, 10 + padtext, hb)
                    hb = hb + 17
                End If
                If currlega.Settings.Bonus.EnableBonusAttack Then
                    Dim strb As String = "* Nessun bonus attacco"
                    Dim sb As SolidBrush = sb2
                    If f.BonusDifesa > 0 Then
                        sb = sb1
                        If currlega.Settings.Bonus.BonusAttack(3) = f.BonusAttacco * 10 Then
                            strb = "* Bonus attacco a 2 (+" & currlega.Settings.Bonus.BonusAttack(2) / 10 & "pt)"
                        ElseIf currlega.Settings.Bonus.BonusAttack(4) = f.BonusAttacco * 10 Then
                            strb = "* Bonus attacco a 3 (+" & currlega.Settings.Bonus.BonusAttack(3) / 10 & "pt)"
                        End If
                    End If
                    gr.DrawString(strb, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), sb, 10 + padtext, hb)
                    hb = hb + 17
                End If
                If currlega.Settings.SubstitutionType <> LegaObject.LegaSettings.eSubstitutionType.Normal Then
                    Dim strb As String = "* Nessun cambio modulo"
                    Dim sb As SolidBrush = sb2
                    If f.ModuleSubstitution = True Then
                        sb = sb1
                        strb = "* Cambio modulo eseguito"
                    End If
                    gr.DrawString(strb, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), sb, 10 + padtext, hb)
                End If
                gr.Dispose()

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try

            Return CType(b1.Clone, Bitmap)

        End Function

        Public Shared Function GetPlayerVariationImage(var As Integer) As Bitmap
            Select Case var
                Case -2 : Return My.Resources.empty
                Case -1 : Return My.Resources.vardown16
                Case 0 : Return My.Resources.w4
                Case Else : Return My.Resources.varup16
            End Select
        End Function

        Public Shared Function GetImageNationFlags(natcode As String) As Bitmap
            If imgnatcode.ContainsKey(natcode) Then
                Return imgnatcode(natcode)
            ElseIf imgnatcode.ContainsKey("UNK") Then
                Return imgnatcode("UNK")
            Else
                Return My.Resources.empty
            End If
        End Function

    End Class
End Namespace