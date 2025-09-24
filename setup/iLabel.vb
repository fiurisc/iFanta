Imports System.Drawing.Text
Imports System.Drawing
Imports System.Windows.Forms

<System.ComponentModel.DefaultEvent("Click")>
Public Class iLabel

    Enum eAutoSizeStyle As Integer
        OnlyWitdh = 0
        OnlyHeight = 1
        Both = 2
    End Enum

    Dim _autosizestyle As eAutoSizeStyle = eAutoSizeStyle.OnlyWitdh
    Dim _TextAlign As System.Windows.Forms.HorizontalAlignment
    Dim _TextVertical As Boolean = False
    Dim _TextRightToLeft As Boolean = False
    Dim _ImageAlign As System.Windows.Forms.HorizontalAlignment
    Dim _LineAlign As System.Drawing.StringAlignment = StringAlignment.Center
    Dim _text As String = String.Empty
    Dim _Picture As Drawing.Image = Nothing
    Dim _BackGround As Drawing.Image = Nothing
    Dim _BorderColor As Color = Color.DimGray
    Dim _BorderSize As Integer = 0
    Dim _InternalBorderColor As Color = Color.White
    Dim _InternalBorderSize As Integer = 0
    Dim _BorderVisible As Boolean = False
    Dim _tooltip As String = String.Empty

    Dim _Dull As Boolean = False
    Dim _resume As Boolean = False
    Dim _textalias As Boolean = False
    Dim _shadows As Boolean = False
    Dim _glow As Boolean = False
    Dim _SuspendLayout As Boolean = False
    Dim _col1 As Color = Color.Transparent
    Dim _col2 As Color = Color.Transparent
    Dim _wordwrap As Boolean = False

#Region "Property"

    Public Property AutoSizeStyle As eAutoSizeStyle
        Get
            Return _autosizestyle
        End Get
        Set(value As eAutoSizeStyle)
            If _autosizestyle <> value Then _autosizestyle = value : Call Draw()
        End Set
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overrides Property BackgroundImage() As Image
        Get
            Return MyBase.BackgroundImage
        End Get
        Set(ByVal value As Image)
            MyBase.BackgroundImage = value
        End Set
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overrides Property BackgroundImageLayout() As System.Windows.Forms.ImageLayout
        Get
            Return MyBase.BackgroundImageLayout
        End Get
        Set(ByVal Value As System.Windows.Forms.ImageLayout)
            MyBase.BackgroundImageLayout = Value
        End Set
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Property SuspundeLayout() As Boolean
        Get
            Return _SuspendLayout
        End Get
        Set(ByVal value As Boolean)
            If _SuspendLayout <> value Then _SuspendLayout = value : Call Draw()
        End Set
    End Property

    Public Property TextVertical() As Boolean
        Get
            Return _TextVertical
        End Get
        Set(ByVal value As Boolean)
            If _TextVertical <> value Then _TextVertical = value : Call Draw()
        End Set
    End Property

    Public Property TextGlowEffetct() As Boolean
        Get
            Return _glow
        End Get
        Set(ByVal value As Boolean)
            If _glow <> value Then _glow = value : Call Draw()
        End Set
    End Property

    Public Property TextShadows() As Boolean
        Get
            Return _shadows
        End Get
        Set(ByVal value As Boolean)
            If _shadows <> value Then _shadows = value : Call Draw()
        End Set
    End Property

    Public Property TextAntiAlis() As Boolean
        Get
            Return _textalias
        End Get
        Set(ByVal value As Boolean)
            If _textalias <> value Then _textalias = value : Call Draw()
        End Set
    End Property

    Public Property WordWrap() As Boolean
        Get
            Return _wordwrap
        End Get
        Set(ByVal value As Boolean)
            If _wordwrap <> value Then _wordwrap = value : Call Draw()
        End Set
    End Property

    Public Property Dull() As Boolean
        Get
            Return _Dull
        End Get
        Set(ByVal value As Boolean)
            If Dull <> value Then _Dull = value : Call Draw()
        End Set
    End Property

    <System.ComponentModel.Browsable(True)>
    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)>
    Public Overrides Property Text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            If _text <> value Then _text = value : Call Draw()
        End Set
    End Property

    Public Property Image() As Image
        Get
            Return _Picture
        End Get
        Set(ByVal Value As Image)
            If Value IsNot Nothing Then
                If _Picture Is Nothing OrElse _Picture.Equals(Value) = False Then _Picture = Value : Call Draw()
            Else
                If _Picture IsNot Nothing Then _Picture = Value : Call Draw()
            End If
        End Set
    End Property

    Public Property BorderColor() As Color
        Get
            Return _BorderColor
        End Get
        Set(ByVal value As Color)
            If _BorderColor.Equals(value) = False Then _BorderColor = value : Call Draw()
        End Set
    End Property

    Public Property BorderSize() As Integer
        Get
            Return _BorderSize
        End Get
        Set(ByVal value As Integer)
            If _BorderSize <> value Then _BorderSize = value : Call Draw()
        End Set
    End Property

    Public Property BorderVisible() As Boolean
        Get
            Return _BorderVisible
        End Get
        Set(ByVal value As Boolean)
            If _BorderVisible <> value Then _BorderVisible = value : Call Draw()
        End Set
    End Property

    Public Property InternalBorderColor() As Color
        Get
            Return _InternalBorderColor
        End Get
        Set(ByVal value As Color)
            If _InternalBorderColor.Equals(value) = False Then _InternalBorderColor = value : Call Draw()
        End Set
    End Property

    Public Property InternalBorderSize() As Integer
        Get
            Return _InternalBorderSize
        End Get
        Set(ByVal value As Integer)
            If _InternalBorderSize <> value Then _InternalBorderSize = value : Call Draw()
        End Set
    End Property

    Public Property Background() As Image
        Get
            Return _BackGround
        End Get
        Set(ByVal Value As Image)
            If Value IsNot Nothing Then
                If _BackGround Is Nothing OrElse _BackGround.Equals(Value) = False Then _BackGround = Value : Call Draw()
            Else
                If _BackGround IsNot Nothing Then _BackGround = Value : Call Draw()
            End If
        End Set
    End Property

    Public Property TextAlign() As System.Windows.Forms.HorizontalAlignment
        Get
            Return _TextAlign
        End Get
        Set(ByVal Value As System.Windows.Forms.HorizontalAlignment)
            If _TextAlign <> Value Then _TextAlign = Value : Call Draw()
        End Set
    End Property

    Public Property ImageAlign() As System.Windows.Forms.HorizontalAlignment
        Get
            Return _ImageAlign
        End Get
        Set(ByVal Value As System.Windows.Forms.HorizontalAlignment)
            If _ImageAlign <> Value Then _ImageAlign = Value : Call Draw()
        End Set
    End Property

    Public Property LineAlignment() As System.Drawing.StringAlignment
        Get
            Return _LineAlign
        End Get
        Set(ByVal Value As System.Drawing.StringAlignment)
            If _LineAlign <> Value Then _LineAlign = Value : Call Draw()
        End Set
    End Property

    Public Property ToolTip() As String
        Get
            Return _tooltip
        End Get
        Set(ByVal value As String)
            _tooltip = value
            Me.ToolTip1.SetToolTip(Me, value)
        End Set
    End Property

#End Region

#Region "Function"

    Sub SetGradientStyle(ByVal col1 As Color, ByVal col2 As Color)
        _col1 = col1
        _col2 = col2
        Call Draw()
    End Sub

    Sub Draw()

        If _resume = False OrElse _SuspendLayout = True Then Exit Sub

        If AutoSize = True Then Call SetAutoSize()

        If Me.Width = 0 OrElse Me.Height = 0 Then Exit Sub

        Try

            If Me.Width > 0 AndAlso Me.Height > 0 Then

                Dim b1 As New Bitmap(Me.Width, Me.Height)
                Dim gr As Graphics
                Dim dy As Integer = 0
                Dim dx As Integer = 0
                Dim x1 As Integer = 0
                Dim picdx As Integer = 0
                Dim picdy As Integer = 0

                gr = Graphics.FromImage(b1)
                gr.Clear(Me.BackColor)

                If _col1 <> Color.Transparent AndAlso _col2 <> Color.Transparent Then
                    Dim br As New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, Me.Width, Me.Height), _col1, _col2, Drawing2D.LinearGradientMode.Vertical)
                    gr.FillRectangle(br, New Rectangle(0, 0, Me.Width, Me.Width))
                    br.Dispose()
                Else
                    If Not _BackGround Is Nothing Then gr.DrawImage(_BackGround, 0, 0, Me.Width + 100, Me.Height)
                End If

                If _BorderSize > 0 AndAlso _BorderVisible = True Then
                    Dim p As New Pen(_BorderColor, _BorderSize)
                    p.Alignment = Drawing2D.PenAlignment.Inset
                    If _BorderSize = 1 Then
                        gr.DrawRectangle(p, 0, 0, Me.Width - 1, Me.Height - 1)
                    Else
                        gr.DrawRectangle(p, 0, 0, Me.Width, Me.Height)
                    End If
                    p.Dispose()
                End If

                If _InternalBorderSize > 0 Then
                    Dim p As New Pen(_InternalBorderColor, _InternalBorderSize)
                    p.Alignment = Drawing2D.PenAlignment.Inset
                    Dim d As Integer = 0
                    If _BorderVisible = True Then d = _BorderSize

                    If _InternalBorderSize = 1 Then
                        gr.DrawRectangle(p, d, d, Me.Width - 1 - d * 2, Me.Height - 1 - d * 2)
                    Else
                        gr.DrawRectangle(p, d, d, Me.Width - d * 2, Me.Height - d * 2)
                    End If
                    p.Dispose()
                End If


                If Not _Picture Is Nothing Then
                    If _TextVertical Then
                        Select Case _LineAlign
                            Case StringAlignment.Near : dx = _BorderSize + _InternalBorderSize + 1
                            Case StringAlignment.Center : dx = CInt(Math.Ceiling(Me.Width / 2 - _Picture.Width / 2))
                            Case StringAlignment.Far : dx = Me.Width - _BorderSize + _InternalBorderSize - 1 - _Picture.Width
                        End Select
                        Select Case _ImageAlign
                            Case HorizontalAlignment.Left : picdy = _BorderSize + 1
                            Case HorizontalAlignment.Center : picdy = Me.Height \ 2 - _Picture.Height \ 2
                            Case HorizontalAlignment.Right : picdy = Me.Height - _BorderSize - 1 - _Picture.Height
                        End Select
                        If Me.Enabled = False Then
                            'gr.DrawImage(ConvGray(_Picture), dx, picdy)
                        Else
                            gr.DrawImage(_Picture, dx, picdy)
                        End If
                    Else
                        Select Case _LineAlign
                            Case StringAlignment.Near : dy = _BorderSize + _InternalBorderSize + 1
                            Case StringAlignment.Center : dy = CInt(Math.Ceiling(Me.Height / 2 - _Picture.Height / 2))
                            Case StringAlignment.Far : dy = Me.Height - _BorderSize + _InternalBorderSize - 1 - _Picture.Height
                        End Select
                        Select Case _ImageAlign
                            Case HorizontalAlignment.Left : picdx = _BorderSize + 1
                            Case HorizontalAlignment.Center : picdx = Me.Width \ 2 - _Picture.Width \ 2
                            Case HorizontalAlignment.Right : picdx = Me.Width - _BorderSize - 1 - _Picture.Width
                        End Select
                        If Me.Enabled = False Then
                            'gr.DrawImage(ConvGray(_Picture), picdx, dy)
                        Else
                            gr.DrawImage(_Picture, picdx, dy)
                        End If
                    End If

                End If

                Dim txt As String = _text.Replace("|", System.Environment.NewLine)

                If _TextVertical Then
                    If _Picture Is Nothing Then
                        dy = _BorderSize + 1
                        dx = 0
                    Else
                        dy = _Picture.Height + _BorderSize + 3
                        dx = Me.Width \ 2 - _Picture.Width \ 2
                    End If
                Else
                    If _Picture Is Nothing Then
                        dy = 0
                        dx = _BorderSize + 1
                    Else
                        dy = Me.Height \ 2 - _Picture.Height \ 2
                        dx = _Picture.Width + _BorderSize + 3
                    End If
                End If

                Dim format As New StringFormat

                Select Case _TextAlign
                    Case HorizontalAlignment.Left
                        format.Alignment = StringAlignment.Near
                        If _ImageAlign = HorizontalAlignment.Left Then x1 = dx Else x1 = _BorderSize + 1
                    Case HorizontalAlignment.Center
                        format.Alignment = StringAlignment.Center
                    Case HorizontalAlignment.Right
                        format.Alignment = StringAlignment.Far
                        If _ImageAlign = HorizontalAlignment.Right Then x1 = _BorderSize + _InternalBorderSize Else x1 = dx
                End Select

                format.LineAlignment = _LineAlign

                If _wordwrap = False Then format.FormatFlags = StringFormatFlags.NoWrap Else format.FormatFlags = 0

                If _TextVertical Then
                    format.FormatFlags = StringFormatFlags.DirectionVertical
                End If

                Dim s As SizeF = gr.MeasureString(txt, Me.Font, New SizeF(Me.Width - _BorderSize * 2 - _InternalBorderSize * 2 - dx, 1000), format)

                Dim dxg As Integer = 0
                Dim sh As Integer = 0

                If _shadows = True Then sh = 2
                If _glow = True Then dxg = 2

                If _textalias = True Then gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

                If _TextVertical Then
                    If Me.Enabled = True Then
                        gr = DrawGlowText(gr, txt, Me.Font, Me.ForeColor, Me.ForeColor, Color.Black, True, dxg, sh, 22, Me.Width - _BorderSize * 2 - _InternalBorderSize * 2, Me.Height - dy, _BorderSize + _InternalBorderSize - 2, dy, format)
                    Else
                        gr = DrawGlowText(gr, txt, Me.Font, Color.FromArgb(160, 130, 130, 130), Color.FromArgb(160, 130, 130, 130), Color.Black, True, dxg, sh, 22, Me.Width - dx, Me.Height, x1, _BorderSize + _InternalBorderSize, format)
                    End If
                Else
                    If Me.Enabled = True Then
                        gr = DrawGlowText(gr, txt, Me.Font, Me.ForeColor, Me.ForeColor, Color.Black, True, dxg, sh, 22, Me.Width - dx, Me.Height - _BorderSize * 2 - _InternalBorderSize * 2, x1, _BorderSize + _InternalBorderSize, format)
                    Else
                        gr = DrawGlowText(gr, txt, Me.Font, Color.FromArgb(160, 130, 130, 130), Color.FromArgb(160, 130, 130, 130), Color.Black, True, dxg, sh, 22, Me.Width - dx, Me.Height, x1, _BorderSize + _InternalBorderSize, format)
                    End If
                End If


                format.Dispose()
                s = Nothing

                MyBase.BackgroundImage = CType(b1.Clone, Image)

                b1.Dispose()
                gr.Dispose()

                Call SetDullRegion(_Dull, Me)

            End If

        Catch

        End Try

    End Sub

    Public Sub SetAutoSize()

        Dim dy As Integer
        Dim dx As Integer
        Dim gr As Graphics
        gr = Me.CreateGraphics

        Dim txt As String = _text.Replace("|", System.Environment.NewLine)


        If _Picture Is Nothing Then
            dy = 0
            dx = _BorderSize + 1
        Else
            dy = Me.Height \ 2 - _Picture.Height \ 2
            dx = _Picture.Width + _BorderSize + 3
        End If

        Dim format As New StringFormat

        Select Case _TextAlign
            Case HorizontalAlignment.Left : format.Alignment = StringAlignment.Near
            Case HorizontalAlignment.Center : format.Alignment = StringAlignment.Center
            Case HorizontalAlignment.Right : format.Alignment = StringAlignment.Far
        End Select

        If _wordwrap = False Then format.FormatFlags = StringFormatFlags.NoWrap

        Dim maxw As Integer = -1
        Dim maxh As Integer = -1
        Dim minw As Integer = 10
        Dim minh As Integer = 10

        If Me.MaximumSize.Width <> 0 Then maxw = Me.MaximumSize.Width
        If Me.MaximumSize.Height <> 0 Then maxh = Me.MaximumSize.Height
        If Me.MinimumSize.Width <> 0 Then minw = Me.MinimumSize.Width
        If Me.MinimumSize.Height <> 0 Then minh = Me.MinimumSize.Height

        If maxw = -1 Then maxw = 1000
        If maxh = -1 Then maxh = 1000

        Dim s As SizeF = gr.MeasureString(txt, Me.Font, New SizeF(maxw, maxh), format)

        _SuspendLayout = True

        If _autosizestyle = eAutoSizeStyle.OnlyWitdh OrElse _autosizestyle = eAutoSizeStyle.Both Then
            Me.Width = CInt(s.Width) + _BorderSize * 2 + _InternalBorderSize * 2 + dx + 3
        End If
        If _autosizestyle = eAutoSizeStyle.OnlyHeight OrElse _autosizestyle = eAutoSizeStyle.Both Then
            Me.Height = CInt(s.Height) + _BorderSize * 2 + _InternalBorderSize * 2
        End If

        _SuspendLayout = False

        s = Nothing
        format.Dispose()
        gr.Dispose()

    End Sub

    Protected Overrides Sub OnLayout(ByVal levent As LayoutEventArgs)
        If _resume = False Then
            _resume = True
            Call Draw()
        End If
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        If _resume = False Then
            _resume = True
            Call Draw()
        End If
    End Sub

#End Region

#Region "Event on control"

    Private Sub iToolBar_AutoSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.AutoSizeChanged
        Call Draw()
    End Sub

    Private Sub iLabel_BackColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.BackColorChanged
        Call Draw()
    End Sub

    Private Sub iLabel_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.EnabledChanged
        Call Draw()
    End Sub

    Private Sub iLabel_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
        Call Draw()
    End Sub

    Private Sub iLabel_ForeColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ForeColorChanged
        Call Draw()
    End Sub

    Private Sub iLabel_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Call Draw()
    End Sub
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub iLabel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Me.Text = "rr r32r32r32 r332r432tr24 t4t43t45y45y 5y5y45y5 y54y45y5y45 5qergcv6cjjk7 cck"
    End Sub

    Private Sub iLabel_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Me.Background = Nothing
    End Sub
End Class
