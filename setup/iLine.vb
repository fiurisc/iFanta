Imports System.ComponentModel

<System.ComponentModel.DefaultEvent("Click")> _
Public Class iLine

    Dim _line As New iLinePen
    Dim _text As String = ""
    Dim _textalias As Boolean = True
    Dim _shadows As Boolean = False
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

    Public Property TextShadows() As Boolean
        Get
            Return _shadows
        End Get
        Set(ByVal value As Boolean)
            _shadows = value
            Call Draw()
        End Set
    End Property

    Public Property TextAntiAlis() As Boolean
        Get
            Return _textalias
        End Get
        Set(ByVal value As Boolean)
            _textalias = value
            Call Draw()
        End Set
    End Property

    <Browsable(True)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public Property Line() As iLinePen
        Get
            Return _line
        End Get
        Set(ByVal value As iLinePen)
            _line = value
            Call Draw()
        End Set
    End Property

    <Browsable(True)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
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

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        If _resume = False Then
            _resume = True
            Call Draw()
        End If
    End Sub

    Sub Draw()

        If _resume = False OrElse _SuspendLayout = True Then Exit Sub
        If Me.Width = 0 OrElse Me.Height = 0 Then Exit Sub

        Try
            Dim delta1 As Integer = 0
            Dim delta2 As Integer = 0

            Dim gr As Graphics
            Dim b1 As Bitmap
            b1 = New Bitmap(Me.Width, Me.Height)

            gr = Graphics.FromImage(b1)

            gr.Clear(Me.BackColor)

            If _text = "" Then
                gr.DrawLine(_line.Pen, 0, Me.Height \ 2 - _line.Width \ 2, Me.Width, Me.Height \ 2 - _line.Width \ 2)
            Else
                Dim w As Integer = CInt(gr.MeasureString(_text, Me.Font, Me.Width).Width)
                Dim h As Integer = CInt(gr.MeasureString(_text, Me.Font, Me.Width).Height)

                gr.DrawLine(_line.Pen, 0, Me.Height \ 2 - _line.Width \ 2, 6, Me.Height \ 2 - _line.Width \ 2)
                If _textalias = True Then gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                If _shadows = True Then
                    gr.DrawString(_text, Me.Font, New SolidBrush(Color.FromArgb(80, 100, 100, 100)), 10, Me.Height \ 2 - h \ 2 - 1)
                    gr.DrawString(_text, Me.Font, New SolidBrush(Me.ForeColor), 9, Me.Height \ 2 - h \ 2 - 2)
                Else
                    gr.DrawString(_text, Me.Font, New SolidBrush(Me.ForeColor), 9, Me.Height \ 2 - h \ 2 - 2)
                End If
                gr.DrawLine(_line.Pen, 12 + w, Me.Height \ 2 - _line.Width \ 2, Me.Width, Me.Height \ 2 - _line.Width \ 2)
            End If

            gr.Dispose()

            MyBase.BackgroundImage = CType(b1.Clone, Image)

            b1.Dispose()            

        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Event on control"

    Private Sub iLine_BackColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.BackColorChanged
        Call Draw()
    End Sub

    Private Sub iLine_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
        Call Draw()
    End Sub

    Private Sub iLine_ForeColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ForeColorChanged
        Call Draw()
    End Sub

    Private Sub iLine_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Call Draw()
    End Sub

#End Region

End Class

<TypeConverter(GetType(ExpandableObjectConverter))> _
    Public Class iLinePen

    Dim _cl As Color = Color.DimGray
    Dim _style As Drawing.Drawing2D.DashStyle = Drawing2D.DashStyle.Solid
    Dim _wi As Integer = 1

    Public Property Color() As Color
        Get
            Return _cl
        End Get
        Set(ByVal value As Color)
            _cl = value
        End Set
    End Property

    Public Property DashStyle() As Drawing2D.DashStyle
        Get
            Return _style
        End Get
        Set(ByVal value As Drawing2D.DashStyle)
            _style = value
        End Set
    End Property

    Public Property Width() As Integer
        Get
            Return _wi
        End Get
        Set(ByVal value As Integer)
            _wi = value
        End Set
    End Property

    <Browsable(False)> _
    Public ReadOnly Property Pen() As Pen
        Get
            Dim p As New Pen(_cl, _wi)
            p.DashStyle = _style
            Return p
        End Get
    End Property
End Class
