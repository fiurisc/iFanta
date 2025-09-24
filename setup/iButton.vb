
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports System.ComponentModel

<System.ComponentModel.DefaultEvent("Click")> _
Public Class iButton

    Dim over As Boolean = False
    Dim _Image As Drawing.Image
    Dim _text As String = "Botton"
    Dim _Color1 As Color = Color.FromArgb(253, 253, 253)
    Dim _Color2 As Color = Color.FromArgb(235, 235, 235)
    Dim _Color3 As Color = Color.WhiteSmoke
    Dim _FlatStyle As Boolean = False
    Dim _dull As Boolean = True
    Dim _borderColor1 As Color = Color.DarkGray
    Dim _borderColor2 As Color = Color.Gainsboro
    Dim _BorderSize As Integer = 1
    Dim _resume As Boolean = False
    Dim _SuspendLayout As Boolean = False
    Dim _textalias As Boolean = False
    Dim _shadows As Boolean = False
    Dim _glow As Boolean = False
    Dim _tooltip As String = ""
    Dim _Background As Image

#Region "Property"

    <Browsable(False)> _
    Public Overrides Property BackgroundImage() As Image
        Get
            Return MyBase.BackgroundImage
        End Get
        Set(ByVal value As Image)
            MyBase.BackgroundImage = value
        End Set
    End Property

    <Browsable(False)> _
    Public Overrides Property BackgroundImageLayout() As System.Windows.Forms.ImageLayout
        Get
            Return MyBase.BackgroundImageLayout
        End Get
        Set(ByVal Value As System.Windows.Forms.ImageLayout)
            MyBase.BackgroundImageLayout = Value
        End Set
    End Property

    <Browsable(False)> _
    Public Property SuspundeLayout() As Boolean
        Get
            Return _SuspendLayout
        End Get
        Set(ByVal value As Boolean)
            If _SuspendLayout <> value Then _SuspendLayout = value : Call draw()
        End Set
    End Property

    Public Property Background() As Image
        Get
            Return _Background
        End Get
        Set(ByVal Value As Image)
            If Value IsNot Nothing Then
                If _Background Is Nothing OrElse _Background.Equals(Value) = False Then _Background = Value : Call draw()
            Else
                If _Background IsNot Nothing Then _Background = Value : Call draw()
            End If
        End Set
    End Property

    Public Property BorderSize() As Integer
        Get
            Return _BorderSize
        End Get
        Set(ByVal value As Integer)
            If _BorderSize <> value Then _BorderSize = value : Call draw()
        End Set
    End Property

    Public Property BorderColorEnalbed() As Color
        Get
            Return _borderColor1
        End Get
        Set(ByVal value As Color)
            If _borderColor1.Equals(value) = False Then _borderColor1 = value : Call draw()
        End Set
    End Property

    Public Property BorderColorDisabled() As Color
        Get
            Return _borderColor2
        End Get
        Set(ByVal value As Color)
            If _borderColor2.Equals(value) = False Then _borderColor2 = value : Call draw()
        End Set
    End Property

    Public Property Dull() As Boolean
        Get
            Return _dull
        End Get
        Set(ByVal value As Boolean)
            If Dull <> value Then _dull = value : Call draw()
        End Set
    End Property

    Public Property ColorLeave() As Color
        Get
            Return _Color1
        End Get
        Set(ByVal value As Color)
            If _Color1.Equals(value) = False Then _Color1 = value : Call draw()
        End Set
    End Property

    Public Property ColorOver() As Color
        Get
            Return _Color2
        End Get
        Set(ByVal Value As Color)
            If _Color2.Equals(Value) = False Then _Color2 = Value : Call draw()
        End Set
    End Property

    Public Property ColorDisabled() As Color
        Get
            Return _Color3
        End Get
        Set(ByVal Value As Color)
            If _Color3.Equals(Value) = False Then _Color3 = Value : Call draw()
        End Set
    End Property

    Public Property Image() As Image
        Get
            Return _Image
        End Get
        Set(ByVal Value As Image)
            If Value IsNot Nothing Then
                If _Image Is Nothing OrElse _Image.Equals(Value) = False Then _Image = Value : Call draw()
            Else
                If _Image IsNot Nothing Then _Image = Value : Call draw()
            End If
        End Set
    End Property

    <System.ComponentModel.Browsable(True)> _
    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)> _
       Public Overrides Property Text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            If _text <> value Then _text = value : Call draw()
        End Set
    End Property

    Public Property FlatStyle() As Boolean
        Get
            Return _FlatStyle
        End Get
        Set(ByVal value As Boolean)
            If _FlatStyle <> value Then _FlatStyle = value : Call draw()
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

    Private Sub draw()

        If _resume = False OrElse _SuspendLayout = True Then Exit Sub
        If Me.Width = 0 OrElse Me.Height = 0 Then Exit Sub

        Try
            Dim myGraphicsPath As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

            Dim colore As New Color

            Dim gr As Graphics
            Dim b1 As Bitmap
            b1 = New Bitmap(Me.Width, Me.Height)

            gr = Graphics.FromImage(b1)

            If over = True Then
                colore = _Color2
            Else
                colore = _Color1
            End If

            If Me.Enabled = False Then
                colore = _Color3
            End If

            Dim br As New LinearGradientBrush(New Rectangle(0, 0, Me.Width, Me.Height), Color.Transparent, Color.FromArgb(25, 0, 0, 0), 90)

            
            If Me.Enabled = True Then
                gr.FillRectangle(New SolidBrush(_borderColor1), 0, 0, Me.Width, Me.Height)
            Else
                gr.FillRectangle(New SolidBrush(_borderColor2), 0, 0, Me.Width, Me.Height)
            End If

            gr.FillRectangle(New SolidBrush(Me.BackColor), _BorderSize, _BorderSize, Me.Width - _BorderSize * 2, Me.Height - _BorderSize * 2)

            gr.FillRectangle(New SolidBrush(colore), _BorderSize + 1, _BorderSize + 1, Me.Width - _BorderSize * 2 - 2, Me.Height - _BorderSize * 2 - 2)
            If _FlatStyle = False Then
                gr.FillRectangle(br, _BorderSize + 1, _BorderSize + 1, Me.Width - _BorderSize * 2 - 2, Me.Height - _BorderSize * 2 - 2)
                Dim bl As New Drawing2D.ColorBlend
                bl.Colors = New Color() {Color.Transparent, Color.Transparent, Color.FromArgb(15, 0, 0, 0), Color.FromArgb(15, 0, 0, 0)}
                bl.Positions = New Single() {0, 0.5, 0.5, 1}
                br.InterpolationColors = bl
                gr.FillRectangle(br, _BorderSize + 1, _BorderSize + 1, Me.Width - _BorderSize * 2 - 2, Me.Height - _BorderSize * 2 - 2)
            End If
           
            br.Dispose()

            'Disegno l'eventuale immagine'
            Dim dx As Single = 0

            If Not _Image Is Nothing Then
                Dim dy As Single
                dx = _Image.Width \ 2
                dy = CSng(Me.Height / 2 - _Image.Height / 2)
                If Me.Enabled = True Then
                    gr.DrawImage(_Image, 3, dy)
                Else
                    'gr.DrawImage(ConvGray(_Image), 3, dy)
                End If
            End If

            If _text <> "" Then
                'Disegno il testo del bottone'
                Dim s As New SizeF

                s = gr.MeasureString(_text, Me.Font, Me.Width)

                Dim dxg As Integer = 0
                Dim sh As Integer = 0

                If _shadows = True Then sh = 2
                If _glow = True Then dxg = 2

                'Imposto l'eventuale effetto AntiAliasing'
                If _textalias = True Then gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

                Dim format As New StringFormat
                format.Alignment = StringAlignment.Center

                'Disegno il testo'
                If Me.Enabled = True Then
                    gr = DrawGlowText(gr, _text, Me.Font, Me.ForeColor, Me.ForeColor, Color.Black, True, dxg, sh, 22, CInt(Me.Width - dx), Me.Height, CInt(dx), Me.Height \ 2 - CInt(s.Height) \ 2 - dxg \ 2, format)
                Else
                    gr = DrawGlowText(gr, _text, Me.Font, Color.FromArgb(160, 130, 130, 130), Color.FromArgb(160, 130, 130, 130), Color.Black, True, dxg, sh, 22, CInt(Me.Width - dx), Me.Height, CInt(dx), Me.Height \ 2 - CInt(s.Height) \ 2 - dxg \ 2, format)
                End If

                format.Dispose()

            End If

            'Disegno l'eventuale rettangolo di selezione'
            If MyBase.Focused = True Then
                Dim p As New Pen(Color.FromArgb(90, 0, 0, 0))
                p.DashStyle = DashStyle.Dot
                gr.DrawRectangle(p, New Rectangle(2 + _BorderSize, 2 + _BorderSize, Me.Width - 5 - _BorderSize * 2, Me.Height - 5 - _BorderSize * 2))
                p.Dispose()
            End If

            MyBase.BackgroundImage = CType(b1.Clone, Image)
            b1.Dispose()
            gr.Dispose()

            Call SetDullRegion(_dull, Me)

        Catch ex As Exception

        End Try

    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        If _resume = False Then
            _resume = True
            Call draw()
            Me.TabStop = True
        End If
    End Sub

#End Region

#Region "Event on control"

    Protected Overrides Sub OnGotFocus(ByVal e As System.EventArgs)
        Call draw()
        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnLostFocus(ByVal e As System.EventArgs)
        Call draw()
        MyBase.OnLostFocus(e)
    End Sub

    Private Sub iButton_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
        Call draw()
    End Sub

    Private Sub iBotton_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Call draw()
    End Sub

    Private Sub iBotton_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.MouseEnter
        over = True
        Call draw()
    End Sub

    Private Sub iBotton_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.MouseLeave
        over = False
        Call draw()
    End Sub

    Private Sub iBotton_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.EnabledChanged
        Call draw()
    End Sub

    Private Sub iButton_ForeColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ForeColorChanged
        Call draw()
    End Sub

    Private Sub iButton_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            MyBase.InvokeOnClick(Me, New System.EventArgs)
        End If
    End Sub

#End Region

End Class
