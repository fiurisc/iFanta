Imports System.Drawing.Text
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D

<System.ComponentModel.DefaultEvent("ChangeValue")> _
Public Class iProgressBar

    Event ChangeValue(ByVal sender As Object, ByVal e As System.EventArgs)

    Dim _max As Double = 10
    Dim _value As Double = 0
    Dim _bordercolor As System.Drawing.Color = Color.Silver
    Dim _barcolor As Color = Color.Lime
    Dim _text As String
    Dim _FlatStyle As Boolean = False
    Dim _Dull As Boolean = True
    Dim _smooth As Boolean = True
    Dim _SuspendLayout As Boolean = False
    Dim _resume As Boolean = False

#Region "Property"

    <System.ComponentModel.Browsable(False)> _
    Public Overrides Property BackgroundImage() As Image
        Get
            Return MyBase.BackgroundImage
        End Get
        Set(ByVal value As Image)
            MyBase.BackgroundImage = value
        End Set
    End Property

    <System.ComponentModel.Browsable(False)> _
    Public Overrides Property BackgroundImageLayout() As System.Windows.Forms.ImageLayout
        Get
            Return MyBase.BackgroundImageLayout
        End Get
        Set(ByVal Value As System.Windows.Forms.ImageLayout)
            MyBase.BackgroundImageLayout = Value
        End Set
    End Property

    <System.ComponentModel.Browsable(False)> _
Public Property SuspundeLayout() As Boolean
        Get
            Return _SuspendLayout
        End Get
        Set(ByVal value As Boolean)
            _SuspendLayout = value
            If value = False Then Call Draw()
        End Set
    End Property

    Public Property Smooth() As Boolean
        Get
            Return _smooth
        End Get
        Set(ByVal value As Boolean)
            _smooth = value
            Call Draw()
        End Set
    End Property

    Public Property Dull() As Boolean
        Get
            Return _Dull
        End Get
        Set(ByVal value As Boolean)
            _Dull = value
            Call SetDullRegion(_Dull, Me)
        End Set
    End Property

    Public Property FlatStyle() As Boolean
        Get
            Return _FlatStyle
        End Get
        Set(ByVal value As Boolean)
            _FlatStyle = value
            Call Draw()
        End Set
    End Property

    Public Property Max() As Double
        Get
            Return _max
        End Get
        Set(ByVal Value As Double)
            _max = Value
            Call Draw()
        End Set
    End Property

    Public Property Value() As Double
        Get
            Return _value
        End Get
        Set(ByVal Value As Double)
            _value = Value
            Call Draw()
            RaiseEvent ChangeValue(Me, New System.EventArgs)
        End Set
    End Property

    Public Property BorderColor() As System.Drawing.Color
        Get
            Return _bordercolor
        End Get
        Set(ByVal Value As System.Drawing.Color)
            _bordercolor = Value
            Call Draw()
        End Set
    End Property

    Public Property BarColor() As Color
        Get
            Return _barcolor
        End Get
        Set(ByVal value As Color)
            _barcolor = value

            Call Draw()

        End Set
    End Property

    <System.ComponentModel.Browsable(True)> _
    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)> _
    Public Overrides Property Text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            _text = value
            Call Draw()
        End Set
    End Property

#End Region

#Region "Function"

    Private Sub Draw()

        If _resume = False OrElse _SuspendLayout = True Then Exit Sub

        Try
            Me.SuspendLayout()

            Dim dx1 As Integer

            Dim gr As Graphics
            Dim b1 As New Bitmap(Me.Width, Me.Height)

            gr = Graphics.FromImage(b1)

            If Me.BackgroundImage IsNot Nothing Then gr.DrawImage(Me.BackgroundImage, 0, 0)
            gr.SmoothingMode = SmoothingMode.AntiAlias

            Dim br1 As LinearGradientBrush
            Dim br2 As LinearGradientBrush

            If _FlatStyle = False Then
                br1 = New LinearGradientBrush(New Rectangle(0, 0, Me.Width, Me.Height), Color.White, _barcolor, 90)
                br2 = New LinearGradientBrush(New Rectangle(0, 0, Me.Width, Me.Height), Color.White, Me.BackColor, 90)
            Else
                br1 = New LinearGradientBrush(New Rectangle(0, 0, Me.Width, Me.Height), _barcolor, _barcolor, 90)
                br2 = New LinearGradientBrush(New Rectangle(0, 0, Me.Width, Me.Height), Me.BackColor, Me.BackColor, 90)
            End If


            gr.FillRectangle(br2, 0, 0, Me.Width, Me.Height - 1)

            dx1 = CInt((Me.Width / _max) * _value)
            gr.FillRectangle(br1, 0, 0, dx1, Me.Height - 1)
            gr.FillRectangle(New SolidBrush(_barcolor), 0, Me.Height \ 2, dx1, Me.Height \ 2 - 1)

            br1.Dispose()
            br2.Dispose()

            '****Contorni*****'
            Dim pen1 As New Pen(_bordercolor)
            gr.DrawRectangle(pen1, 0, 0, Me.Width - 1, Me.Height - 1)
            pen1 = New Pen(Color.FromArgb(250, 255, 255, 255))
            gr.DrawRectangle(pen1, 1, 1, Me.Width - 3, Me.Height - 3)
            pen1.Brush = New LinearGradientBrush(New Rectangle(0, 0, Me.Width, Me.Height), Color.FromArgb(20, 0, 0, 0), Color.FromArgb(80, 0, 0, 0), LinearGradientMode.Vertical)
            gr.DrawRectangle(pen1, 2, 2, dx1 - 2, Me.Height - 5)

            pen1.Dispose()

            'Disegno l'eventuale testo"
            If _text <> "" Then
                Dim bs As New SolidBrush(Me.ForeColor)
                Dim x As Single = 0
                Dim y As Single = 0

                y = gr.MeasureString("Ty", Me.Font, Me.Width).Height
                y = Me.Height \ 2 - CLng(y) \ 2

                x = gr.MeasureString(_text, Me.Font, 300).Width
                x = Me.Width \ 2 - CLng(x) \ 2

                gr.DrawString(_text, Me.Font, bs, x, y)
            End If

            If _smooth = False Then
                For i As Double = 1 To Value - 1
                    dx1 = CInt((Me.Width / _max) * i)
                    gr.DrawLine(Pens.White, dx1, 1, dx1, Me.Height - 2)
                Next
            End If

            gr = Me.CreateGraphics
            gr.DrawImage(b1, 0, 0)

            b1.Dispose()
            gr.Dispose()

        Catch ex As Exception

        End Try

    End Sub

    Protected Overrides Sub OnLayout(ByVal levent As LayoutEventArgs)
        If _resume = False Then
            _resume = True
            Call Draw()
        End If
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        'If _resume = False Then
        '    _resume = True
        '    Call Draw()
        'End If
        _resume = True
        Call Draw()
    End Sub

    Protected Overloads Sub OnLoad(ByVal e As PaintEventArgs)
        Call SetDullRegion(_Dull, Me)
    End Sub

#End Region

#Region "Event on control"

    Private Sub iProgressBar_BackColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.BackColorChanged
        Call Draw()
    End Sub

    Private Sub iProgressBar_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
        Call Draw()
    End Sub

    Private Sub iProgressBar_ForeColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ForeColorChanged
        Call Draw()
    End Sub

    Private Sub iProgressBar_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Call Draw()
        Call SetDullRegion(_Dull, Me)
    End Sub

#End Region

End Class
