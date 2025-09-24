Imports System.Drawing.Text
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports System.IO

<System.ComponentModel.DefaultEvent("ThemeChange")> _
Public Class iFormControl

    Event ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs)
    Event BeginWindowStateChange(ByVal sender As Object, ByVal OldWindowState As _WindowStateE, ByVal NewWindowState As _WindowStateE)
    Event EndWindowStateChange(ByVal sender As Object, ByVal OldWindowState As _WindowStateE, ByVal NewWindowState As _WindowStateE)

    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const WM_NCHITTEST As Integer = &H84
    Private Const WM_LBUTTONUP As Integer = &H202
    Private Const WH_MOUSE_LL As Integer = 14

    Private Const WS_CAPTION As Integer = &HC00000
    Private Const WS_OVERLAPPED As Integer = &H0&
    Private Const GWL_STYLE As Integer = (-16)
    Private Const WS_SYSMENU As Integer = &H80000
    Private Const WS_THICKFRAME As Integer = &H40000
    Private Const WS_DLGFRAME As Integer = &H400000
    Private Const WS_MAXIMIZEBOX As Integer = &H10000
    Private Const WS_MINIMIZEBOX As Integer = &H20000
    Private Const WS_BORDER As Integer = &H800000

    Private Const SWP_FRAMECHANGED As Integer = &H20
    Private Const SWP_NOMOVE As Integer = &H2
    Private Const SWP_NOZORDER As Integer = &H4
    Private Const SWP_NOSIZE As Integer = &H1

    Private Const HTCAPTION As Integer = 2
    Private Const HTBOTTOM As Integer = 15
    Private Const HTBOTTOMLEFT As Integer = 16
    Private Const HTBOTTOMRIGHT As Integer = 17
    Private Const HTTOP As Integer = 12
    Private Const HTTOPLEFT As Integer = 13
    Private Const HTTOPRIGTH As Integer = 14
    Private Const HTLEFT As Integer = 10
    Private Const HTRIGHT As Integer = 11
    Private Const HTSYSMENU As Integer = 3

    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function SetWindowLong(ByVal hwnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function GetWindowLong(ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function SetWindowPos(ByVal hwnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal flags As Integer) As Boolean
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function FlashWindow(ByVal hwnd As IntPtr, ByVal bInvert As Boolean) As Integer
    End Function

    Private Sub ShowSysMenu(ByVal Visible As Boolean)
        If _showsysmenu = True Then
            If Not Me.ParentForm Is Nothing Then
                If Visible = True Then
                    Dim OldStyle As Integer = GetWindowLong(Me.ParentForm.Handle, GWL_STYLE)
                    OldStyle = OldStyle Or WS_SYSMENU Or WS_MINIMIZEBOX
                    If (OldStyle And WS_MAXIMIZEBOX) = WS_MAXIMIZEBOX Then
                        OldStyle = OldStyle And Not WS_MAXIMIZEBOX
                    End If
                    SetWindowLong(Me.ParentForm.Handle, GWL_STYLE, OldStyle)
                Else
                    Me.ParentForm.FormBorderStyle = FormBorderStyle.None
                End If
                pRedraw()
            End If
        End If
    End Sub

    Public Sub pRedraw()
        ' Redraw window with new style.
        Const swpFlags As Long = _
           SWP_FRAMECHANGED Or SWP_NOMOVE Or _
           SWP_NOZORDER Or SWP_NOSIZE
        Call SetWindowPos(Me.ParentForm.Handle, CType(0, IntPtr), 0, 0, 0, 0, swpFlags)
    End Sub

    Protected Overrides Sub OnDoubleClick(ByVal e As EventArgs)
        MyBase.OnDoubleClick(e)
        FlashWindow(Me.ParentForm.Handle, True)
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        _resume = True
        MyBase.OnLoad(e)
        ShowSysMenu(True)
    End Sub

    Enum _WindowStateE As Integer
        Normal = 0
        Minimize = 1
        Maximized = 2
    End Enum

    Enum align As Integer
        Left
        Center
        Right
    End Enum

    Dim PicArray(13) As PictureBox
    Dim MtxImageBorder(13) As Bitmap
    Dim MtxImageCtlBox(6) As Bitmap
    Dim MtxImageLogos(1) As Bitmap
    Dim ImageBar As Bitmap
    Dim MtxColor(26, 1) As Color

    Dim _Sizable As Boolean = True
    Dim _controlbox As Boolean = True
    Dim min As Boolean = True
    Dim max As Boolean = True
    Dim cls As Boolean = True
    Dim diff As Point
    Dim _WindowState As _WindowStateE = _WindowStateE.Normal
    Dim OldLocation As System.Drawing.Point
    Dim OldSize As Size

    Dim _TH1 As Integer = 35
    Dim _TH2 As Integer = 1
    Dim _TH3 As Integer = 20
    Dim _TH4 As Integer = 1
    Dim _TH5 As Integer = 20
    Dim _TH6 As Integer = 1

    Dim _BH1 As Integer = 1
    Dim _BH2 As Integer = 18
    Dim _BH3 As Integer = 1
    Dim _BH4 As Integer = 18
    Dim _BH5 As Integer = 1
    Dim _BH6 As Integer = 35

    Dim _septop1 As Boolean = False
    Dim _septop2 As Boolean = True
    Dim _septop3 As Boolean = True

    Dim _sepbot1 As Boolean = True
    Dim _sepbot2 As Boolean = True
    Dim _sepbot3 As Boolean = False

    Dim _FlatStyle As Boolean = False
    Dim _rel As String = ""
    Dim _Title As String = "Form"
    Dim _WindowsTitle As String = ""
    Dim _Vislogo1 As Boolean = True
    Dim _Vislogo2 As Boolean = True
    Dim _Theme As String = "Red"
    Dim _FontTitleWindows As New Font("Arial", 9, FontStyle.Bold)
    Dim _ForeColorTitleWindows As Color = Color.White
    Dim _Icon As Image

    Dim _Logo1 As Image
    Dim _Logo2 As Image

    Dim _resume As Boolean = False
    Dim _TitleAlign As align = align.Left
    Dim _clipping As Boolean = False
    Dim _showsysmenu As Boolean = True

    Dim tt As Date = Date.Now

    Declare Function ReleaseCapture Lib "user32.dll" () As Int32
    Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As Int32) As Int32

#Region "Property"

    Public ReadOnly Property LogoRightWidth() As Integer
        Get
            Return PicLogo.Width
        End Get
    End Property

    Public Property WindowsClipping() As Boolean
        Get
            Return _clipping
        End Get
        Set(ByVal value As Boolean)
            _clipping = value
        End Set
    End Property

    Public Property ShowSystemMenu() As Boolean
        Get
            Return _showsysmenu
        End Get
        Set(ByVal value As Boolean)
            _showsysmenu = value
        End Set
    End Property

    Public Property TitleAlign() As align
        Get
            Return _TitleAlign
        End Get
        Set(ByVal value As align)
            _TitleAlign = value
            Call DrawTitle()
        End Set
    End Property

    Public Property ForeColorTitleWindows() As Color
        Get
            Return _ForeColorTitleWindows
        End Get
        Set(ByVal value As Color)
            _ForeColorTitleWindows = value
            Call DrawWindowsTitle()
        End Set
    End Property

    Public Property FontTitleWindows() As Font
        Get
            Return _FontTitleWindows
        End Get
        Set(ByVal value As Font)
            _FontTitleWindows = value
            Call DrawWindowsTitle()
        End Set
    End Property

    Public Property Theme() As String
        Get
            Return _Theme
        End Get
        Set(ByVal value As String)
            If _Theme <> value Then
                _Theme = value
                Call LoadTheme()
                RaiseEvent ThemeChange(Me, New System.EventArgs)
            End If
        End Set
    End Property

    Public Property Rel() As String
        Get
            Return _rel
        End Get
        Set(ByVal value As String)
            _rel = value

            Call DrawTitle()

        End Set
    End Property

    Public Property Title() As String
        Get
            Return _Title
        End Get
        Set(ByVal value As String)
            _Title = value
            Call DrawTitle()
        End Set
    End Property

    Public Property WindowsTitle() As String
        Get
            Return _WindowsTitle
        End Get
        Set(ByVal value As String)
            _WindowsTitle = value
            Call DrawWindowsTitle()
        End Set
    End Property

    Public Property Icon() As Image
        Get
            Return _Icon
        End Get
        Set(ByVal value As Image)
            _Icon = value
            Call DrawWindowsTitle()
        End Set
    End Property

    Public Property Sizable() As Boolean
        Get
            Return _Sizable
        End Get
        Set(ByVal value As Boolean)
            _Sizable = value
            Call SetBorderWindowsCursor()
            Call SetControlBox()
        End Set
    End Property

    Public Property ControlBox() As Boolean
        Get
            Return _controlbox
        End Get
        Set(ByVal value As Boolean)
            _controlbox = value

            If value = False Then
                cnt1.Visible = value
                cnt2.Visible = value
                cnt3.Visible = value
            Else
                cnt1.Visible = min
                cnt2.Visible = max
                cnt3.Visible = cls
            End If
            Call SetControlBox()

        End Set
    End Property

    Public Property MinimizeButton() As Boolean
        Get
            Return min
        End Get
        Set(ByVal value As Boolean)
            min = value
            cnt1.Visible = value
            Call iResize()
        End Set
    End Property

    Public Property MaximizeButton() As Boolean
        Get
            Return max
        End Get
        Set(ByVal value As Boolean)
            max = value
            cnt2.Visible = value
            Call iResize()
        End Set
    End Property

    Public Property CloseButton() As Boolean
        Get
            Return cls
        End Get
        Set(ByVal value As Boolean)
            cls = value
            cnt3.Visible = value
            Call iResize()
        End Set
    End Property

    Public Property ShowLogoRight() As Boolean
        Get
            Return _Vislogo2
        End Get
        Set(ByVal value As Boolean)
            _Vislogo2 = value
            Call DrawTitle()
        End Set
    End Property

    Public Property ShowLogoLeft() As Boolean
        Get
            Return _Vislogo1
        End Get
        Set(ByVal value As Boolean)
            _Vislogo1 = value
            Call DrawTitle()
        End Set
    End Property

    Public Property LogoLeft() As Image
        Get
            Return MtxImageLogos(0)
        End Get
        Set(ByVal value As Image)
            MtxImageLogos(0) = CType(value, Bitmap)
            Call DrawTitle()
        End Set
    End Property

    Public Property LogoRight() As Image
        Get
            Return MtxImageLogos(1)
        End Get
        Set(ByVal value As Image)
            MtxImageLogos(1) = CType(value, Bitmap)
            Call DrawTitle()
        End Set
    End Property

    Public ReadOnly Property TH0() As Integer
        Get
            Return _Pic2.Height
        End Get
    End Property

    Public Property TH1() As Integer
        Get
            Return _TH1
        End Get
        Set(ByVal value As Integer)
            _TH1 = value
            Call DrawBar()
            Call DrawTitle()
        End Set
    End Property

    Public Property TH2() As Integer
        Get
            Return _TH2
        End Get
        Set(ByVal value As Integer)
            _TH2 = value
            Call DrawBar()
        End Set
    End Property

    Public Property TH3() As Integer
        Get
            Return _TH3
        End Get
        Set(ByVal value As Integer)
            _TH3 = value
            Call DrawBar()
        End Set
    End Property

    Public Property TH4() As Integer
        Get
            Return _TH4
        End Get
        Set(ByVal value As Integer)
            _TH4 = value
            Call DrawBar()
        End Set
    End Property

    Public Property TH5() As Integer
        Get
            Return _TH5
        End Get
        Set(ByVal value As Integer)
            _TH5 = value
            Call DrawBar()
        End Set
    End Property

    Public Property TH6() As Integer
        Get
            Return _TH6
        End Get
        Set(ByVal value As Integer)
            _TH6 = value
            Call DrawBar()
        End Set
    End Property

    Public Property BH1() As Integer
        Get
            Return _BH1
        End Get
        Set(ByVal value As Integer)
            _BH1 = value
            Call DrawBar()
        End Set
    End Property

    Public Property BH2() As Integer
        Get
            Return _BH2
        End Get
        Set(ByVal value As Integer)
            _BH2 = value
            Call DrawBar()
        End Set
    End Property

    Public Property BH3() As Integer
        Get
            Return _BH3
        End Get
        Set(ByVal value As Integer)
            _BH3 = value
            Call DrawBar()
        End Set
    End Property

    Public Property BH4() As Integer
        Get
            Return _BH4
        End Get
        Set(ByVal value As Integer)
            _BH4 = value
            Call DrawBar()
        End Set
    End Property

    Public Property BH5() As Integer
        Get
            Return _BH5
        End Get
        Set(ByVal value As Integer)
            _BH5 = value
            Call DrawBar()
        End Set
    End Property

    Public Property BH6() As Integer
        Get
            Return _BH6
        End Get
        Set(ByVal value As Integer)
            _BH6 = value
            Call DrawBar()
        End Set
    End Property

    Public ReadOnly Property BH7() As Integer
        Get
            Return _pic9.Height
        End Get
    End Property

    Public ReadOnly Property TY() As Integer
        Get
            Return _TH1 + _TH2 + _TH3 + _TH4 + _TH5 + _TH6 + Pic2.Height
        End Get
    End Property

    Public ReadOnly Property BY() As Integer
        Get
            Return Me.Height - (_BH1 + _BH2 + _BH3 + _BH4 + _BH5 + _BH6) - pic9.Height
        End Get
    End Property

    Public ReadOnly Property LX() As Integer
        Get
            Return pic12.Width
        End Get
    End Property

    Public ReadOnly Property RX() As Integer
        Get
            Return Me.Width - Pic6.Width
        End Get
    End Property

    Public ReadOnly Property LW() As Integer
        Get
            Return pic12.Width
        End Get
    End Property

    Public ReadOnly Property RW() As Integer
        Get
            Return Pic6.Width
        End Get
    End Property

    Public Property FlatStyle() As Boolean
        Get
            Return _FlatStyle
        End Get
        Set(ByVal value As Boolean)
            If _FlatStyle <> value Then
                _FlatStyle = value
                Call LoadTheme()
                RaiseEvent ThemeChange(Me, New System.EventArgs)
            End If
        End Set
    End Property

#End Region

#Region "Function"

    Sub SetTopBarColor(ByVal Index As Integer, ByVal Color1 As Color, ByVal Color2 As Color)
        MtxColor(14 + Index * 2, 0) = Color1
        MtxColor(14 + Index * 2, 0) = Color2
        Call DrawWindowsTitle()
        Call DrawBar()
    End Sub

    Sub SetTopBarColor(ByVal Index As Integer, ByVal Color() As Color)
        MtxColor(14 + Index * 2, 0) = Color(0)
        MtxColor(14 + Index * 2, 1) = Color(1)
        Call DrawWindowsTitle()
        Call DrawBar()
    End Sub

    Sub SetBottonBarColor(ByVal Index As Integer, ByVal Color1 As Color, ByVal Color2 As Color)
        MtxColor(20 + Index * 2, 0) = Color1
        MtxColor(20 + Index * 2, 1) = Color2
        Call DrawBar()
    End Sub

    Sub SetBottonBarColor(ByVal Index As Integer, ByVal Color() As Color)
        MtxColor(20 + Index * 2, 0) = Color(0)
        MtxColor(20 + Index * 2, 1) = Color(1)
        Call DrawBar()
    End Sub

    Function GetTopBarTopPosition(ByVal Index As Integer) As Integer
        Dim y As Integer = Pic1.Height
        Select Case Index
            Case 2 : y = y + _TH1 + TH2
            Case 3 : y = y + _TH1 + _TH2 + _TH3 + _TH4
        End Select
        Return y
    End Function

    Function GetBottonBarTopPosition(ByVal Index As Integer) As Integer
        Dim y As Integer = BY
        Select Case Index
            Case 1 : y = y + _BH1
            Case 2 : y = y + _BH1 + _BH2 + _BH3
            Case 3 : y = y + _BH1 + _BH2 + _BH3 + _BH4 + _BH5
        End Select
        Return y
    End Function

    Function GetTopBarSepColor(ByVal Index As Integer) As Color()
        Dim mtx() As Color = {MtxColor(13 + Index * 2, 0), MtxColor(12 + Index * 2, 1)}
        Return mtx
    End Function

    Function GetTopBarSepColor(ByVal Index As Integer, ByVal Color As Integer) As Color
        Return MtxColor(13 + Index * 2, Color)
    End Function

    Function GetTopBarColor(ByVal Index As Integer) As Color()
        Dim mtx() As Color = {MtxColor(12 + Index * 2, 0), MtxColor(12 + Index * 2, 1)}
        Return mtx
    End Function

    Function GetTopBarColor(ByVal Index As Integer, ByVal Color As Integer) As Color
        Return MtxColor(12 + Index * 2, Color)
    End Function

    Function GetBottonBarSepColor(ByVal Index As Integer) As Color()
        Dim mtx() As Color = {MtxColor(19 + Index * 2, 0), MtxColor(20 + Index * 2, 1)}
        Return mtx
    End Function

    Function GetBottonBarSepColor(ByVal Index As Integer, ByVal Color As Integer) As Color
        Return MtxColor(19 + Index * 2, Color)
    End Function

    Function GetBottonBarColor(ByVal Index As Integer) As Color()
        Dim mtx() As Color = {MtxColor(20 + Index * 2, 0), MtxColor(20 + Index * 2, 1)}
        Return mtx
    End Function

    Function GetBottonBarColor(ByVal Index As Integer, ByVal Color As Integer) As Color
        Return MtxColor(20 + Index * 2, Color)
    End Function

    Private Sub SetBorderWindowsCursor()

        If _Sizable = True AndAlso _WindowState = _WindowStateE.Normal Then
            PicArray(0).Cursor = Cursors.SizeNWSE
            PicArray(1).Cursor = Cursors.SizeNS
            PicArray(2).Cursor = Cursors.SizeNESW
            PicArray(3).Cursor = Cursors.SizeWE
            PicArray(4).Cursor = Cursors.SizeWE
            PicArray(5).Cursor = Cursors.SizeWE
            PicArray(6).Cursor = Cursors.SizeWE
            PicArray(7).Cursor = Cursors.SizeNWSE
            PicArray(8).Cursor = Cursors.SizeNS
            PicArray(9).Cursor = Cursors.SizeNESW
            PicArray(10).Cursor = Cursors.SizeWE
            PicArray(11).Cursor = Cursors.SizeWE
            PicArray(12).Cursor = Cursors.SizeWE
            PicArray(13).Cursor = Cursors.SizeWE
        Else
            For i As Integer = 0 To 13
                PicArray(i).Cursor = Cursors.Default
            Next
        End If
    End Sub

    Private Sub Clipping()

        If _resume = False Then Exit Sub
        If _clipping = False Then Exit Sub

        Try

            'Gestione contorni smussati della finestra'

            Dim Pth As New Drawing2D.GraphicsPath

            If Me.DesignMode = False Then

                Dim myPath As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

                Dim form As Form
                form = Me.FindForm

                If form Is Nothing Then Exit Sub

                Dim b As New Point(0, 0)
                Dim c As New Point(0, 0)

                b = Me.PointToScreen(b)
                c = form.Location
                b.Offset(-c.X, -c.Y)

                'Disegno i rettangoli'
                myPath.ClearMarkers()
                myPath.AddRectangle(New Rectangle(Pic1.Width + b.X, 0 + b.Y, Me.Width - (Pic1.Width + Pic3.Width), Me.Height))
                myPath.AddRectangle(New Rectangle(b.X, Pic1.Height + b.Y, Pic1.Width, Me.Height - (Pic1.Height + pic10.Height)))
                myPath.AddRectangle(New Rectangle(Me.Width - Pic3.Width + b.X, Pic4.Top + b.Y, Pic3.Width, Me.Height - (Pic3.Height + Pic8.Height)))

                For i As Integer = 0 To 9

                    If i = 0 OrElse i = 9 Then

                        Dim myPat As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

                        For j As Integer = 0 To MtxImageBorder(i).Height - 1

                            Dim esi As Boolean = False

                            For k As Integer = 0 To MtxImageBorder(i).Width - 1
                                If i = 9 Then
                                    If j + PicArray(i).Top >= PicArray(i - 1).Top Then
                                        myPat.AddRectangle(New Rectangle(k, j, 1, 1))
                                    Else
                                        If MtxImageBorder(i).GetPixel(k, j).A > 0 Then
                                            myPat.AddRectangle(New Rectangle(k, j, 1, 1))
                                        Else
                                            If esi = True AndAlso j + PicArray(i).Top < Me.BY Then Pth.AddRectangle(New Rectangle(k + PicArray(i + 1).Left, j + PicArray(i + 1).Top, 1, 1))
                                        End If
                                    End If
                                End If
                                If MtxImageBorder(i).GetPixel(k, j).A > 0 Then esi = True
                                If esi = True Then myPath.AddRectangle(New Rectangle(b.X + PicArray(i).Left + k, b.Y + j + PicArray(i).Top, 1, 1))
                            Next
                        Next
                        If i = 9 Then PicArray(i).Region = New Region(myPat)
                        myPat.Dispose()
                    End If

                    If i = 2 OrElse i = 7 Then

                        Dim myPat As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

                        For j As Integer = 0 To MtxImageBorder(i).Height - 1

                            Dim esi As Boolean = False

                            For k As Integer = MtxImageBorder(i).Width - 1 To 0 Step -1
                                If i = 7 Then
                                    If j + PicArray(i).Top >= PicArray(i + 1).Top Then
                                        myPat.AddRectangle(New Rectangle(k, j, 1, 1))
                                    Else
                                        If MtxImageBorder(i).GetPixel(k, j).A > 0 Then
                                            myPat.AddRectangle(New Rectangle(k, j, 1, 1))
                                        Else
                                            If esi = True AndAlso j + PicArray(i).Top < Me.BY Then Pth.AddRectangle(New Rectangle(k + PicArray(i + 1).Left, j + PicArray(i + 1).Top, 1, 1))
                                        End If
                                    End If
                                End If
                                If MtxImageBorder(i).GetPixel(k, j).A > 0 Then esi = True
                                If esi = True Then myPath.AddRectangle(New Rectangle(b.X + PicArray(i).Left + k, b.Y + j + PicArray(i).Top, 1, 1))
                            Next
                        Next
                        If i = 7 Then PicArray(i).Region = New Region(myPat)
                        myPat.Dispose()
                    End If
                Next
                If _clipping Then form.Region = New Region(myPath)
                myPath.Dispose()
            End If

            If pic10.Top < Me.BY Then
                Pth.AddRectangle(New Rectangle(MtxImageBorder(11).Width, Me.TY, Me.Width - MtxImageBorder(11).Width - MtxImageBorder(5).Width, Me.BY - Me.TY))
                Pth.AddRectangle(New Rectangle(pic10.Width, Me.Height - pic10.Height, Me.Width - Pic8.Width - pic10.Width, Me.BY - pic10.Top))
            Else
                Pth.ClearMarkers()
                Pth.AddRectangle(New Rectangle(MtxImageBorder(11).Width, Me.TY, Me.Width - MtxImageBorder(11).Width - MtxImageBorder(5).Width, Me.BY - Me.TY))
            End If

            Dim Rgn As New Region(New Rectangle(0, 0, Me.Width, Me.Height))
            Rgn.Exclude(Pth)
            Me.Region = Rgn.Clone

            Rgn = New Region(New Rectangle(1, 1, Pnl3.Width, Pnl3.Height - 1))
            Pnl3.Region = Rgn.Clone
            Rgn.Dispose()
            Pth.Dispose()

        Catch ex As Exception

        End Try

    End Sub

    Public Function GetWindowState() As _WindowStateE
        Return _WindowState
    End Function

    Public Sub SetWindowState(ByVal State As _WindowStateE)

        RaiseEvent BeginWindowStateChange(Me, _WindowState, State)

        Select Case State
            Case _WindowStateE.Maximized
                Call MaximizedForm()
            Case _WindowStateE.Minimize
                OldLocation = Me.ParentForm.Location
                OldSize = Me.ParentForm.Size
                Me.ParentForm.WindowState = FormWindowState.Minimized
            Case _WindowStateE.Normal
                Me.ParentForm.Location = OldLocation
                Me.ParentForm.Size = OldSize
        End Select

        If State <> _WindowState Then
            If State <> _WindowStateE.Minimize Then _WindowState = State
            RaiseEvent EndWindowStateChange(Me, _WindowState, State)
        End If
        If State <> _WindowStateE.Minimize Then _WindowState = State

        Call SetBorderWindowsCursor()

    End Sub

    Private Sub MaximizedForm()

        Try

            Dim b As New Point(0, 0)
            Dim c As New Point(0, 0)
            Dim d As Size = SystemInformation.WorkingArea.Size

            OldLocation = Me.ParentForm.Location
            OldSize = Me.ParentForm.Size

            b = Me.PointToScreen(b)
            c = Me.ParentForm.Location
            c.Offset(-b.X, -b.Y)

            Me.ParentForm.Location = c
            d.Width = d.Width + (Me.ParentForm.Size.Width - Me.Width)
            d.Height = d.Height + (Me.ParentForm.Size.Height - Me.Height)

            Me.ParentForm.Size = d

            Call Clipping()

        Catch ex As Exception

        End Try

    End Sub

    Public Sub LoadTheme()

        Debug.WriteLine("LoadTheme")

        Call SetDefaultColor()
        Call SetDefaultImage()
        Call LoadColor()
        Call SetTheme()
        Call SetBorderWindowsCursor()

    End Sub

    Public Sub SetTheme(ByVal Name As String, ByVal FlatStyle As Boolean)
        _Theme = Name
        _FlatStyle = FlatStyle
        Call LoadTheme()
        RaiseEvent ThemeChange(Me, New System.EventArgs)
    End Sub

    Private Sub SetTheme()

        Debug.WriteLine("SetTheme")

        Try
            For i As Integer = 0 To 13
                If MtxImageBorder(i) IsNot Nothing Then
                    If MtxImageBorder(i).Width > 0 AndAlso MtxImageBorder(i).Height > 0 Then
                        Dim b1 As New Bitmap(MtxImageBorder(i).Width, MtxImageBorder(i).Height)
                        Dim gr As Graphics
                        gr = Graphics.FromImage(b1)
                        gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, 0, b1.Width, b1.Height), MtxColor(i, 0), MtxColor(i, 1), LinearGradientMode.Vertical), 0, 0, b1.Width, b1.Height)
                        If MtxImageBorder(i) IsNot Nothing Then gr.DrawImage(MtxImageBorder(i), 0, 0)
                        PicArray(i).BackgroundImage = CType(b1.Clone, Image)
                        If i <> 0 AndAlso i <> 2 AndAlso i <> 7 AndAlso i <> 9 Then MtxImageBorder(i) = CType(b1.Clone, Bitmap)
                        gr.Dispose()
                        b1.Dispose()
                    End If
                End If
            Next

            Call SetControlBox()
            Call iResize()
            Call DrawBar()
            Call DrawTitle()
            Call DrawWindowsTitle()
            Call iResize()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub LoadColor()

        Dim para() As String

        Debug.WriteLine("LoadColor")

        Dim dirgen As String = My.Application.Info.DirectoryPath & "\Theme"
        Dim dirtheme As String = dirgen & "\" & _Theme
        Dim dirthemestyle As String = dirtheme & "\Normal"
        If _FlatStyle = True Then dirthemestyle = dirtheme & "\Flat"

        'Carico le impostazioni dei colori'
        Try

            If File.Exists(dirthemestyle & "\theme.ini") = True Then

                Dim line() As String = IO.File.ReadAllLines(dirthemestyle & "\theme.ini")

                For i As Integer = 0 To line.Length - 1
                    para = line(i).Split(CChar(","))
                    If para.Length > 3 Then
                        MtxColor(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                        If para.Length = 7 Then
                            MtxColor(CInt(para(0)), 1) = Color.FromArgb(CInt(para(4)), CInt(para(5)), CInt(para(6)))
                        Else
                            MtxColor(CInt(para(0)), 1) = MtxColor(CInt(para(0)), 0)
                        End If
                    End If
                Next
            End If

        Catch ex As Exception

        End Try

        'Carico l'eventuali logo di destra'
        Try
            If File.Exists(dirthemestyle & "\logo.png") = True Then
                MtxImageLogos(1) = New Bitmap(dirthemestyle & "\logo.png")
            Else
                If IO.File.Exists(dirtheme & "\logo.png") = True Then
                    MtxImageLogos(1) = New Bitmap(dirtheme & "\logo.png")
                Else
                    If IO.File.Exists(dirgen & "\logo.png") = True Then
                        MtxImageLogos(1) = New Bitmap(dirgen & "\logo.png")
                    Else
                        MtxImageLogos(1) = New Bitmap(Panel2.BackgroundImage)
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

        'Carico le immagini con i bordi della finestra'
        Try
            For i As Integer = 0 To 13
                If File.Exists(dirthemestyle & "\" & i & ".png") Then
                    MtxImageBorder(i) = New Bitmap(dirthemestyle & "\" & i & ".png")
                Else
                    If File.Exists(dirtheme & "\" & i & ".png") Then
                        MtxImageBorder(i) = New Bitmap(dirtheme & "\" & i & ".png")
                    End If
                End If
            Next
        Catch ex As Exception

        End Try

        'Carico l'immagine della top bar 1'
        Try
            If _FlatStyle = False Then
                If File.Exists(dirthemestyle & "\bar1.png") Then
                    ImageBar = New Bitmap(dirthemestyle & "\bar1.png")
                Else
                    If File.Exists(dirtheme & "\bar1.png") Then
                        ImageBar = New Bitmap(dirtheme & "\bar1.png")
                    End If
                End If
            Else
                If File.Exists(dirthemestyle & "\bar2.png") Then
                    ImageBar = New Bitmap(dirthemestyle & "\bar2.png")
                Else
                    If File.Exists(dirtheme & "\bar2.png") Then
                        ImageBar = New Bitmap(dirtheme & "\bar2.png")
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

        'Carico le immagini dei controllo della control box'
        Try
            For i As Integer = 0 To 6
                If File.Exists(dirthemestyle & "\c" & i & ".png") Then
                    MtxImageCtlBox(i) = New Bitmap(dirthemestyle & "\c" & i & ".png")
                Else
                    If File.Exists(dirtheme & "\c" & i & ".png") Then
                        MtxImageCtlBox(i) = New Bitmap(dirtheme & "\c" & i & ".png")
                    End If
                End If
            Next
        Catch ex As Exception

        End Try

    End Sub

    Public Sub SetControlBox()

        If _resume = False Then Exit Sub

        Debug.WriteLine("SetControlBox")

        Try

            If Pic2.BackgroundImage IsNot Nothing Then
                cnt1.BackgroundImage = Pic2.BackgroundImage
                cnt2.BackgroundImage = Pic2.BackgroundImage
                cnt3.BackgroundImage = Pic2.BackgroundImage
            End If

            cnt1.Image = MtxImageCtlBox(0)
            cnt1.Width = MtxImageCtlBox(0).Width
            cnt1.Height = MtxImageCtlBox(0).Height

            If _Sizable = True Then
                cnt2.Image = MtxImageCtlBox(2)
                cnt2.Width = MtxImageCtlBox(2).Width
                cnt2.Height = MtxImageCtlBox(2).Height
            Else
                cnt2.Image = MtxImageCtlBox(4)
                cnt2.Width = MtxImageCtlBox(4).Width
                cnt2.Height = MtxImageCtlBox(4).Height
            End If

            cnt3.Image = MtxImageCtlBox(5)
            cnt3.Width = MtxImageCtlBox(5).Width
            cnt3.Height = MtxImageCtlBox(5).Height

            cnt1.Top = 0
            cnt2.Top = 0
            cnt3.Top = 0

            If Pic3.BackgroundImage IsNot Nothing Then cnt3.Left = Me.Width - cnt3.Width - (Pic3.BackgroundImage.Width + 3)

            If cls = True Then
                cnt2.Left = cnt3.Left - cnt2.Width
            Else
                cnt2.Left = Me.Width - 50
            End If

            If max = True Then
                cnt1.Left = cnt2.Left - cnt1.Width
            Else
                If cls = True Then
                    cnt1.Left = cnt3.Left - cnt1.Width
                Else
                    cnt1.Left = Me.Width - 50
                End If
            End If

            cnt1.Visible = min
            cnt2.Visible = max
            cnt3.Visible = cls

            If _controlbox = False Then
                cnt1.Visible = False
                cnt2.Visible = False
                cnt3.Visible = False
            End If

        Catch ex As Exception

        End Try

    End Sub

    Public Function GetTopBarImage(ByVal ind As Integer) As Bitmap

        Debug.WriteLine("GetTopBarImage " & ind)

        Dim b1 As New Bitmap(10, 10)
        Dim gr As Graphics

        gr = Graphics.FromImage(b1)

        Select Case ind
            Case 1
                b1 = New Bitmap(60, _TH1)
                gr = Graphics.FromImage(b1)
                gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, 0, 70, _TH1), MtxColor(14, 0), MtxColor(14, 1), LinearGradientMode.Vertical), 0, 0, 70, _TH1)
                gr.DrawImage(ImageBar, 0, 0, 70, _TH1)
            Case 2
                b1 = New Bitmap(60, _TH3)
                gr = Graphics.FromImage(b1)
                Dim br As New LinearGradientBrush(New Rectangle(0, 0, 70, _TH3), MtxColor(16, 0), MtxColor(16, 1), LinearGradientMode.Vertical)
                gr.FillRectangle(br, 0, 0, 70, _TH3)
            Case 3
                b1 = New Bitmap(60, _TH5)
                gr = Graphics.FromImage(b1)
                Dim br As New LinearGradientBrush(New Rectangle(0, 0, 70, _TH5), MtxColor(18, 0), MtxColor(18, 1), LinearGradientMode.Vertical)
                gr.FillRectangle(br, 0, 0, 70, _TH5)
        End Select

        gr.Dispose()

        Return CType(b1.Clone, Bitmap)

        b1.Dispose()

    End Function

    Public Function GetBottonBarImage(ByVal ind As Integer) As Bitmap

        Debug.WriteLine("GetBottonBarImage " & ind)

        Dim b1 As New Bitmap(10, 10)
        Dim gr As Graphics

        gr = Graphics.FromImage(b1)

        Select Case ind
            Case 1
                b1 = New Bitmap(40, _BH2)
                gr = Graphics.FromImage(b1)
                Dim br As New LinearGradientBrush(New Rectangle(0, 0, 50, _BH2), MtxColor(22, 0), MtxColor(22, 1), LinearGradientMode.Vertical)
                gr.FillRectangle(br, 0, 0, 50, _BH2)
                br.Dispose()
            Case 2
                b1 = New Bitmap(40, _BH4)
                gr = Graphics.FromImage(b1)
                Dim br As New LinearGradientBrush(New Rectangle(0, 0, 50, _BH4), MtxColor(24, 0), MtxColor(24, 1), LinearGradientMode.Vertical)
                gr.FillRectangle(br, 0, 0, 50, _BH4)
                br.Dispose()
            Case 3
                b1 = New Bitmap(40, _BH6)
                gr = Graphics.FromImage(b1)
                Dim br As New LinearGradientBrush(New Rectangle(0, 0, 50, _BH6), MtxColor(26, 0), MtxColor(26, 1), LinearGradientMode.Vertical)
                gr.FillRectangle(br, 0, 0, 50, _BH6)
                br.Dispose()
        End Select

        gr.Dispose()

        Return CType(b1.Clone, Bitmap)

        b1.Dispose()

    End Function

    Function GetLogoRightWidth() As Integer
        Return PicLogo.Width
    End Function

    Public Sub DrawBar()

        If _resume = False Then Exit Sub

        Debug.WriteLine("DrawBar")

        Dim gr As Graphics
        Dim b1 As New Bitmap(10, 10)

        Dim vet1(2) As Integer
        Dim vet2(2) As Integer
        Dim vet3(2) As Boolean
        Dim h As Integer
        Dim y As Integer

        gr = Graphics.FromImage(b1)

        Try
            'Dimensiono le barre'
            If Me.TY > Pic2.Height Then

                Picbar1.Height = _TH1 + _TH2 + _TH3 + _TH4 + _TH5 + _TH6

                picsep1.Height = Picbar1.Height
                picsep2.Height = Picbar1.Height

                b1 = New Bitmap(12, Picbar1.Height)
                gr = Graphics.FromImage(b1)

                'Disegno le barre'

                'Bar1'
                If _TH1 > 0 Then gr.DrawImage(GetTopBarImage(1), 0, 0, 12, _TH1)
                If _TH2 > 0 Then gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, _TH1, 70, _TH2), MtxColor(15, 0), MtxColor(15, 1), LinearGradientMode.Vertical), 0, _TH1, 12, _TH2)
                If _TH3 > 0 Then gr.DrawImage(GetTopBarImage(2), 0, _TH1 + _TH2, 12, _TH3)
                If _TH4 > 0 Then gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, _TH1 + _TH2 + _TH3, 70, _TH4), MtxColor(17, 0), MtxColor(17, 1), LinearGradientMode.Vertical), 0, _TH1 + _TH2 + _TH3, 12, _TH4)
                If _TH5 > 0 Then gr.DrawImage(GetTopBarImage(3), 0, _TH1 + _TH2 + _TH3 + _TH4, 12, _TH5)
                If _TH6 > 0 Then gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, _TH1 + _TH2 + _TH3 + _TH4 + _TH5, 70, _TH6), MtxColor(19, 0), MtxColor(19, 1), LinearGradientMode.Vertical), 0, _TH1 + _TH2 + _TH3 + _TH4 + _TH5, 12, _TH6)

                Picbar1.BackgroundImage = CType(b1.Clone, Image)
                Picbar1.BackgroundImageLayout = ImageLayout.Stretch

                'Separatori barra1'

                b1 = New Bitmap(12, Picbar1.Height)
                gr = Graphics.FromImage(b1)

                vet1(0) = _TH1
                vet1(1) = _TH3
                vet1(2) = _TH5

                vet2(0) = 0
                vet2(1) = _TH1 + _TH2
                vet2(2) = _TH1 + _TH2 + _TH3 + _TH4

                vet3(0) = _septop1
                vet3(1) = _septop2
                vet3(2) = _septop3

                gr.DrawImage(Picbar1.BackgroundImage, 0, 0)

                For i As Integer = 0 To 2
                    h = vet1(i) - 12
                    y = vet2(i) + vet1(i) \ 2 - h \ 2
                    If h > 4 AndAlso vet3(i) = True Then
                        gr.FillRectangle(New SolidBrush(Color.FromArgb(40, 0, 0, 0)), 3, y, 2, h)
                        gr.FillRectangle(New SolidBrush(Color.FromArgb(40, 0, 0, 0)), 7, y, 2, h)
                    End If
                Next

                picsep1.BackgroundImage = CType(b1.Clone, Image)
                picsep2.BackgroundImage = CType(b1.Clone, Image)

                picsep1.Height = Picbar1.Height
                picsep2.Height = Picbar1.Height
                Picbar1.Visible = True
                picsep1.Visible = True
                picsep2.Visible = True
            Else
                Picbar1.Visible = False
                picsep1.Visible = False
                picsep2.Visible = False
            End If

            If Me.BY < pic9.Top Then

                PicBar2.Height = _BH1 + _BH2 + _BH3 + _BH4 + _BH5 + _BH6

                picsep3.Top = BY
                picsep4.Top = BY
                picsep3.Height = PicBar2.Height
                picsep4.Height = PicBar2.Height

                b1 = New Bitmap(12, PicBar2.Height)
                gr = Graphics.FromImage(b1)

                'Bar2'

                If _BH1 > 0 Then gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, 0, 12, _BH1), MtxColor(21, 0), MtxColor(21, 1), LinearGradientMode.Vertical), 0, 0, 12, _BH1)
                If _BH2 > 0 Then gr.DrawImage(GetBottonBarImage(1), 0, _BH1, 12, _BH2)
                If _BH3 > 0 Then gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, _BH1 + _BH2, 12, _BH3), MtxColor(23, 0), MtxColor(23, 1), LinearGradientMode.Vertical), 0, _BH1 + _BH2, 12, _BH3)
                If _BH4 > 0 Then gr.DrawImage(GetBottonBarImage(2), 0, _BH1 + _BH2 + _BH3, 12, _BH4)
                If _BH5 > 0 Then gr.FillRectangle(New LinearGradientBrush(New Rectangle(0, _BH1 + _BH2 + _BH3 + _BH4, 12, _BH5), MtxColor(25, 0), MtxColor(25, 1), LinearGradientMode.Vertical), 0, _BH1 + _BH2 + _BH3 + _BH4, 12, _BH5)
                If _BH6 > 0 Then gr.DrawImage(GetBottonBarImage(3), 0, _BH1 + _BH2 + _BH3 + _BH4 + _BH5, 12, _BH6)

                PicBar2.BackgroundImage = CType(b1.Clone, Image)
                PicBar2.BackgroundImageLayout = ImageLayout.Tile
                PicBar2.Top = BY
                picsep3.Top = PicBar2.Top
                picsep4.Top = PicBar2.Top
                picsep3.Height = PicBar2.Height
                picsep4.Height = PicBar2.Height

                'Separatori barra2'

                b1 = New Bitmap(12, PicBar2.Height)
                gr = Graphics.FromImage(b1)

                vet1(0) = _BH2
                vet1(1) = _BH4
                vet1(2) = _BH6

                vet2(0) = _BH1
                vet2(1) = _BH1 + _BH2 + _BH3
                vet2(2) = _BH1 + _BH2 + _BH3 + _BH4 + _BH5

                vet3(0) = _sepbot1
                vet3(1) = _sepbot2
                vet3(2) = _sepbot3

                gr.DrawImage(PicBar2.BackgroundImage, 0, 0)

                For i As Integer = 0 To 2
                    h = vet1(i) - 12
                    y = vet2(i) + vet1(i) \ 2 - h \ 2
                    If h > 4 AndAlso vet3(i) = True Then
                        gr.FillRectangle(New SolidBrush(Color.FromArgb(40, 0, 0, 0)), 3, y, 2, h)
                        gr.FillRectangle(New SolidBrush(Color.FromArgb(40, 0, 0, 0)), 7, y, 2, h)
                    End If
                Next

                picsep3.BackgroundImage = CType(b1.Clone, Image)
                picsep4.BackgroundImage = CType(b1.Clone, Image)
                PicBar2.Visible = True
                picsep3.Visible = True
                picsep4.Visible = True
            Else
                PicBar2.Visible = False
                picsep3.Visible = False
                picsep4.Visible = False
            End If

            b1.Dispose()
            gr.Dispose()


        Catch ex As Exception

        End Try

        Call Clipping()

    End Sub

    Sub iResize()

        Try
            'Posizionamento e ridimensionamento bordi finestra'
            If Pic1.BackgroundImage IsNot Nothing Then
                Pic1.Height = Pic1.BackgroundImage.Height : Pic1.Width = Pic1.BackgroundImage.Width : Pic1.Top = 0 : Pic1.Left = 0
                Pic2.Height = Pic2.BackgroundImage.Height : Pic2.Width = Me.Width - Pic1.BackgroundImage.Width - Pic3.BackgroundImage.Width : Pic2.Top = 0 : Pic2.Left = Pic1.BackgroundImage.Width
                Pnl3.Height = Pic2.BackgroundImage.Height : Pnl3.Top = 0 : Pnl3.Left = Pic1.BackgroundImage.Width
                Pic3.Height = Pic3.BackgroundImage.Height : Pic3.Width = Pic3.BackgroundImage.Width : Pic3.Top = 0 : Pic3.Left = Me.Width - Pic3.BackgroundImage.Width
                Pic4.Height = _TH1 : Pic4.Width = Pic4.BackgroundImage.Width : Pic4.Top = Pic3.BackgroundImage.Height : Pic4.Left = Me.Width - Pic4.BackgroundImage.Width
                Pic5.Height = _TH1 + _TH2 + _TH3 + _TH4 + _TH5 + _TH6 : Pic5.Width = Pic5.BackgroundImage.Width : Pic5.Top = Pic4.Top + Pic4.Height : Pic5.Left = Me.Width - Pic5.BackgroundImage.Width
                Pic6.Height = BY - TY : Pic6.Width = Pic6.BackgroundImage.Width : Pic6.Top = TY : Pic6.Left = Me.Width - Pic6.BackgroundImage.Width
                Pic7.Height = _BH1 + _BH2 + _BH3 + _BH4 + _BH5 + _BH6 : Pic7.Width = Pic7.BackgroundImage.Width : Pic7.Top = Pic6.Top + Pic6.Height : Pic7.Left = Me.Width - Pic7.BackgroundImage.Width
                Pic8.Height = Pic8.BackgroundImage.Height : Pic8.Width = Pic8.BackgroundImage.Width : Pic8.Top = Me.Height - Pic8.BackgroundImage.Height : Pic8.Left = Me.Width - Pic8.BackgroundImage.Width
                pic9.Height = pic9.BackgroundImage.Height : pic9.Width = Me.Width - pic10.BackgroundImage.Width - Pic8.BackgroundImage.Width : pic9.Top = Me.Height - pic9.BackgroundImage.Height : pic9.Left = pic10.BackgroundImage.Width
                pic10.Height = pic10.BackgroundImage.Height : pic10.Width = pic10.BackgroundImage.Width : pic10.Top = Me.Height - pic10.BackgroundImage.Height : pic10.Left = 0
                pic11.Height = Pic7.Height : pic11.Width = pic11.BackgroundImage.Width : pic11.Top = Pic7.Top : pic11.Left = 0
                pic12.Height = Pic6.Height : pic12.Width = pic12.BackgroundImage.Width : pic12.Top = Pic6.Top : pic12.Left = 0
                pic13.Height = Pic5.Height : pic13.Width = pic13.BackgroundImage.Width : pic13.Top = Pic5.Top : pic13.Left = 0
                pic14.Height = Pic4.Height : pic14.Width = pic14.BackgroundImage.Width : pic14.Top = Pic4.Top : pic14.Left = 0
            End If

            Dim vis As Boolean = True

            'Titolo'
            PicTestata.Left = pic14.Width
            PicTestata.Top = Pic2.Height

            'Separatori Barra1'
            picsep1.Top = Pic2.Height
            picsep1.Left = pic13.Width
            picsep1.Width = 12
            picsep2.Top = Pic2.Height
            picsep2.Left = Me.Width - Pic4.Width - picsep1.Width
            picsep2.Width = 12

            picsep1.Visible = vis
            picsep2.Visible = vis

            'Barra1'
            If TY > Pic2.Height Then vis = True Else vis = False
            Picbar1.Visible = vis
            Picbar1.Top = Pic2.Height
            Picbar1.Width = Me.Width - (picsep2.Width * 2 + Pic4.Width + pic13.Width)
            Picbar1.Left = pic13.Width + picsep2.Width

            'Separatori Barra 2'
            picsep3.Top = BY
            picsep3.Left = pic11.Width
            picsep3.Width = 12
            picsep4.Top = BY
            picsep4.Left = Me.Width - Pic4.Width - picsep4.Width
            picsep4.Width = 12

            picsep3.Visible = vis
            picsep4.Visible = vis

            'Barra2'
            If BY < pic9.Top Then vis = True Else vis = False
            PicBar2.Visible = vis
            PicBar2.Top = BY
            PicBar2.Width = Me.Width - (picsep3.Width * 2 + Pic7.Width + pic11.Width)
            PicBar2.Left = pic11.Width + picsep2.Width

            'Logo'
            If Pic4.BackgroundImage IsNot Nothing Then PicLogo.Left = Me.Width - PicLogo.Width - (Pic4.BackgroundImage.Width + 3)
            If Pic3.BackgroundImage IsNot Nothing Then PicLogo.Top = Pic3.BackgroundImage.Height

            'Windows title'
            Pnl3.Width = Me.Width - Pic1.Width - Pic3.Width

            Call Clipping()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub DrawWindowsTitle()

        If _resume = False Then Exit Sub

        Debug.WriteLine("DrawWindowsTitle")

        Dim y As Integer = 0
        Dim wi As Integer = 20

        Dim gr As Graphics
        Dim b1 As New Bitmap(10, Pnl3.Height)

        gr = Graphics.FromImage(b1)
        wi = CInt(gr.MeasureString(_WindowsTitle, _FontTitleWindows, 1000).Width + 20)

        b1 = New Bitmap(wi, Pnl3.Height)
        gr = Graphics.FromImage(b1)

        gr.SmoothingMode = SmoothingMode.AntiAlias

        y = MtxImageBorder(3).Width \ 2 + Pnl3.Height \ 2 - CInt(gr.MeasureString("re", _FontTitleWindows).Height) \ 2 - 1

        If Not Pic2.BackgroundImage Is Nothing Then gr.DrawImage(Pic2.BackgroundImage, 0, 0, Pnl3.Width + 300, Pnl3.Height)

        Try

            If Not _Icon Is Nothing Then gr.DrawImage(_Icon, 0, y, 16, 16)

            If _WindowsTitle <> "" Then
                Dim format As New StringFormat
                format.Alignment = StringAlignment.Near
                If Not _Icon Is Nothing Then
                    gr = DrawGlowText(gr, _WindowsTitle, _FontTitleWindows, _ForeColorTitleWindows, _ForeColorTitleWindows, Color.Black, True, 2, 1, 30, PicTestata.Width, PicTestata.Height, 18, y, format)
                Else
                    gr = DrawGlowText(gr, _WindowsTitle, _FontTitleWindows, _ForeColorTitleWindows, _ForeColorTitleWindows, Color.Black, True, 2, 1, 30, PicTestata.Width, PicTestata.Height, 0, y, format)
                End If
                format.Dispose()
            End If

        Catch ex As Exception

        End Try

        Pnl3.BackgroundImage = Pic2.BackgroundImage
        Pnl3.Image = CType(b1.Clone, Image)

        b1.Dispose()
        gr.Dispose()

    End Sub

    Private Sub DrawTitle()

        If _resume = False Then Exit Sub

        Debug.WriteLine("DrawTitle")

        Try

            If _TH1 > 0 Then
                If _Title <> "" OrElse _rel <> "" OrElse (MtxImageLogos(0) IsNot Nothing AndAlso _Vislogo1 = True) Then

                    Dim y2 As Integer = 0
                    Dim dx As Integer = 5
                    Dim dy As Integer = 0
                    Dim delta As Integer = 0
                    Dim FontSize As Integer = 25
                    Dim gr As Graphics
                    Dim b1 As Bitmap

                    PicTestata.Top = Pic2.Height
                    PicTestata.Width = 400
                    PicTestata.Height = _TH1

                    FontSize = _TH1 - 15
                    If FontSize > 25 Then FontSize = 25
                    If FontSize < 10 Then FontSize = 10

                    b1 = New Bitmap(PicTestata.Width, PicTestata.Height)
                    gr = Graphics.FromImage(b1)

                    'Disegno l'eventuale logo a sinistra'
                    If MtxImageLogos(0) IsNot Nothing AndAlso _Vislogo1 = True Then
                        gr.DrawImage(MtxImageLogos(0), 1, _TH1 \ 2 - MtxImageLogos(0).Height \ 2)
                        delta = MtxImageLogos(0).Width - 4
                    End If

                    If _Title <> "" OrElse _rel <> "" Then

                        Dim privateFontCollection As New PrivateFontCollection

                        'Disegno il titolo della finestra'
                        Dim familyName As String = "bauhs93"
                        Dim font1 As Font = New Font("Arial", FontSize, FontStyle.Bold, GraphicsUnit.Pixel)
                        Dim path As String = ""

                        ' to load in the font add the font file to the private collection.  
                        If System.IO.File.Exists(My.Application.Info.DirectoryPath & "\Theme\" & _Theme & "\font.ttf") = True Then
                            path = My.Application.Info.DirectoryPath & "\Theme\" & _Theme & "\font.ttf"
                        Else
                            path = My.Application.Info.DirectoryPath & "\font.ttf"
                        End If

                        If System.IO.File.Exists(path) = True Then
                            Dim thisFont As FontFamily
                            privateFontCollection.AddFontFile(path)
                            thisFont = privateFontCollection.Families(0)
                            font1 = New Font(thisFont, FontSize, FontStyle.Regular, GraphicsUnit.Pixel)
                            privateFontCollection.Families(0).Dispose()
                            thisFont.Dispose()
                        End If

                        Dim sf1 As SizeF = gr.MeasureString(_Title, New Font(font1.FontFamily, 30, FontStyle.Regular), 600)

                        Dim fs As Integer = (30 * (_TH1 - (15 + (_TH1 - 45) \ 2))) \ CInt(sf1.Height)

                        If fs > 8 Then

                            font1 = New Font(font1.FontFamily, fs, FontStyle.Regular)

                            Dim fs2 As SizeF = gr.MeasureString(_Title, font1, 600)

                            If _Title <> "" Then dx = CInt(gr.MeasureString(_Title, font1, 400).Width + 5) Else dx = 0
                            dy = CInt(gr.MeasureString("Yy", font1, 400).Height + 5)

                            If _rel <> "" Then
                                PicTestata.Width = CInt(gr.MeasureString(_Title & _rel, font1, 400).Width + 13)
                            Else
                                PicTestata.Width = CInt(gr.MeasureString(_Title & _rel, font1, 400).Width + 6)
                            End If

                            y2 = _TH1 \ 2 - CInt(fs2.Height) \ 2

                            'Scrivo il titolo della finestra'
                            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                            gr.SmoothingMode = SmoothingMode.AntiAlias

                            Dim format As New StringFormat
                            format.Alignment = StringAlignment.Near

                            If _Title <> "" Then
                                gr = DrawGlowText(gr, _Title, font1, Color.White, Color.Gainsboro, Color.Black, True, 2, 2, 15, PicTestata.Width, PicTestata.Height, 7 + delta, y2, format)
                            End If

                            If _rel <> "" Then
                                gr = DrawGlowText(gr, _rel, font1, Color.FromArgb(255, 211, 168), Color.FromArgb(225, 140, 51), Color.Black, True, 2, 2, 15, PicTestata.Width, PicTestata.Height, dx + 2 + delta, y2, format)
                            End If
                            format.Dispose()
                        End If

                        font1.Dispose()

                        privateFontCollection.Dispose()

                    End If

                    PicTestata.BackgroundImage = GetTopBarImage(1)
                    PicTestata.Image = CType(b1.Clone, Image)
                    PicTestata.BackgroundImageLayout = ImageLayout.Stretch

                    b1.Dispose()
                    gr.Dispose()
                    PicTestata.Visible = True
                Else
                    PicTestata.Visible = False
                End If
            Else
                PicTestata.Visible = False
            End If

            If _TH1 > 0 Then
                If _Vislogo2 = True AndAlso MtxImageLogos(1) IsNot Nothing AndAlso _TH1 > 0 Then

                    Dim gr As Graphics
                    Dim b1 As New Bitmap(MtxImageLogos(1))
                    Dim wi As Integer
                    Dim he As Integer
                    Dim dx As Integer = 5
                    Dim dy As Integer = 0

                    If MtxImageLogos(1) IsNot Nothing Then b1 = New Bitmap(MtxImageLogos(1))

                    wi = b1.Width
                    he = b1.Height

                    If he > (_TH1 * 0.8) Then he = CInt(_TH1 * 0.8) : wi = CInt(wi * (he / b1.Height))
                    dy = _TH1 \ 2 - he \ 2

                    PicLogo.Height = _TH1
                    PicLogo.Width = wi
                    PicLogo.Left = Me.Width - PicLogo.Width - (Pic5.Width + 3)
                    PicLogo.Top = Pic2.Height

                    b1 = New Bitmap(wi, _TH1)
                    gr = Graphics.FromImage(b1)

                    gr.DrawImage(MtxImageLogos(1), 0, dy, wi, he)

                    PicLogo.BackgroundImage = GetTopBarImage(1)
                    PicLogo.Image = CType(b1.Clone, Image)
                    PicLogo.BackgroundImageLayout = ImageLayout.Stretch

                    b1.Dispose()
                    gr.Dispose()

                    PicLogo.Visible = True
                Else
                    PicLogo.Visible = False
                End If
            Else
                PicLogo.Visible = False
            End If

            Select Case _TitleAlign
                Case align.Left
                    PicTestata.Left = pic13.Width
                Case align.Center
                    PicTestata.Left = Me.Width \ 2 - PicTestata.Width \ 2
                Case align.Right
                    If PicLogo.Visible = True Then
                        PicTestata.Left = Me.Width - PicTestata.Width - (8 + Pic4.Width) - PicLogo.Width
                    Else
                        PicTestata.Left = Me.Width - PicTestata.Width - Pic4.Width
                    End If
            End Select
        Catch ex As Exception
            PicTestata.Visible = False
            PicLogo.Visible = False
        End Try

    End Sub

    Private Function MakePath(ByVal bmp As Bitmap, ByVal Transparent As Color) As Drawing2D.GraphicsPath

        Dim Pth As New Drawing2D.GraphicsPath

        Try

            Dim X, Y As Integer
            For X = 0 To bmp.Width - 1
                For Y = 0 To bmp.Height - 1
                    If bmp.GetPixel(X, Y).ToArgb.ToString = Transparent.ToArgb.ToString Then
                        Pth.AddRectangle(New Rectangle(X, Y, 1, 1))
                    End If
                Next
            Next

        Catch ex As Exception

        End Try

        Return Pth
        Pth.Dispose()

    End Function

    Sub Start()

        Debug.WriteLine("Start")

        PicArray(0) = Pic1
        PicArray(1) = Pic2
        PicArray(2) = Pic3
        PicArray(3) = Pic4
        PicArray(4) = Pic5
        PicArray(5) = Pic6
        PicArray(6) = Pic7
        PicArray(7) = Pic8
        PicArray(8) = pic9
        PicArray(9) = pic10
        PicArray(10) = pic11
        PicArray(11) = pic12
        PicArray(12) = pic13
        PicArray(13) = pic14

        'Call LoadTheme()

    End Sub

    Private Sub SetDefaultColor()

        Dim str As String = ""

        Debug.WriteLine("SetDefaultColor")

        If _FlatStyle Then
            str = My.Resources.themeflat
        Else
            str = My.Resources.theme
        End If

        Try
            Dim lines() As String = Str.Split(CChar(System.Environment.NewLine))
            For i As Integer = 0 To lines.Length - 1
                Dim para() As String = lines(i).Trim.Split(CChar(","))
                If para.Length > 0 Then
                    If para(0) <> "" Then
                        Select Case para(0).ToLower
                            Case "sel1"
                            Case "sel2"
                            Case "cb"
                            Case "back"
                            Case Else
                                If para.Length > 5 Then
                                    MtxColor(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                    MtxColor(CInt(para(0)), 1) = Color.FromArgb(CInt(para(4)), CInt(para(5)), CInt(para(6)))
                                Else
                                    MtxColor(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                    MtxColor(CInt(para(0)), 1) = MtxColor(CInt(para(0)), 0)
                                End If
                        End Select
                        If para(0) <> "cb" Then

                        End If
                    End If
                End If
            Next
        Catch ex As Exception

        End Try

        'If _FlatStyle = False Then
        '    'Border top'
        '    MtxColor(0, 0) = Color.FromArgb(250, 138, 117)
        '    MtxColor(1, 0) = Color.FromArgb(250, 138, 117)
        '    MtxColor(2, 0) = Color.FromArgb(250, 138, 117)
        '    MtxColor(0, 1) = Color.FromArgb(250, 58, 37)
        '    MtxColor(1, 1) = Color.FromArgb(250, 58, 37)
        '    MtxColor(2, 1) = Color.FromArgb(250, 58, 37)
        '    'Topar'
        '    MtxColor(3, 0) = Color.FromArgb(250, 58, 37)
        '    MtxColor(3, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(4, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(4, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(12, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(12, 1) = Color.FromArgb(220, 0, 0)
        '    'Center'
        '    MtxColor(5, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(5, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(11, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(11, 1) = Color.FromArgb(220, 0, 0)
        '    'Bottonbar'
        '    MtxColor(6, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(6, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(10, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(10, 1) = Color.FromArgb(220, 0, 0)
        '    'Botton'
        '    MtxColor(7, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(8, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(9, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(7, 1) = Color.FromArgb(200, 0, 0)
        '    MtxColor(8, 1) = Color.FromArgb(200, 0, 0)
        '    MtxColor(9, 1) = Color.FromArgb(200, 0, 0)

        '    'Topbar1'
        '    MtxColor(14, 0) = Color.FromArgb(250, 58, 37)
        '    MtxColor(14, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(15, 0) = Color.FromArgb(170, 0, 0)
        '    MtxColor(15, 1) = Color.FromArgb(170, 0, 0)
        '    'Topbar2'
        '    MtxColor(16, 0) = Color.FromArgb(253, 253, 253)
        '    MtxColor(16, 1) = Color.FromArgb(220, 220, 220)
        '    MtxColor(17, 0) = Color.FromArgb(180, 180, 180)
        '    MtxColor(17, 1) = Color.FromArgb(180, 180, 180)
        '    'Topbar3'
        '    MtxColor(18, 0) = Color.FromArgb(210, 210, 210)
        '    MtxColor(18, 1) = Color.FromArgb(180, 180, 180)
        '    MtxColor(19, 0) = Color.FromArgb(160, 160, 160)
        '    MtxColor(19, 1) = Color.FromArgb(160, 160, 160)
        '    'Center'
        '    MtxColor(20, 0) = Color.FromArgb(250, 58, 37)
        '    MtxColor(20, 1) = Color.FromArgb(220, 0, 0)
        '    'Bottonbar1'
        '    MtxColor(21, 0) = Color.FromArgb(160, 160, 160)
        '    MtxColor(21, 1) = Color.FromArgb(160, 160, 160)
        '    MtxColor(22, 0) = Color.FromArgb(215, 215, 215)
        '    MtxColor(22, 1) = Color.FromArgb(190, 190, 190)
        '    'Bottonbar1'
        '    MtxColor(23, 0) = Color.FromArgb(180, 180, 180)
        '    MtxColor(23, 1) = Color.FromArgb(180, 180, 180)
        '    MtxColor(24, 0) = Color.FromArgb(253, 253, 253)
        '    MtxColor(24, 1) = Color.FromArgb(220, 220, 220)
        '    'Bottonbar1'
        '    MtxColor(25, 0) = Color.FromArgb(180, 180, 180)
        '    MtxColor(25, 1) = Color.FromArgb(180, 180, 180)
        '    MtxColor(26, 0) = Color.FromArgb(253, 253, 253)
        '    MtxColor(26, 1) = Color.FromArgb(220, 220, 220)
        'Else
        '    'Border top'
        '    MtxColor(0, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(1, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(2, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(0, 1) = Color.FromArgb(248, 0, 0)
        '    MtxColor(1, 1) = Color.FromArgb(248, 0, 0)
        '    MtxColor(2, 1) = Color.FromArgb(248, 0, 0)
        '    'Topar'
        '    MtxColor(3, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(3, 1) = Color.FromArgb(248, 0, 0)
        '    MtxColor(4, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(4, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(12, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(12, 1) = Color.FromArgb(220, 0, 0)
        '    'Center'
        '    MtxColor(5, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(5, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(11, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(11, 1) = Color.FromArgb(220, 0, 0)
        '    'Bottonbar'
        '    MtxColor(6, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(6, 1) = Color.FromArgb(220, 0, 0)
        '    MtxColor(10, 0) = Color.FromArgb(220, 0, 0)
        '    MtxColor(10, 1) = Color.FromArgb(220, 0, 0)
        '    'Botton'
        '    MtxColor(7, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(8, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(9, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(7, 1) = Color.FromArgb(248, 0, 0)
        '    MtxColor(8, 1) = Color.FromArgb(248, 0, 0)
        '    MtxColor(9, 1) = Color.FromArgb(248, 0, 0)

        '    'Topbar1'
        '    MtxColor(14, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(14, 1) = Color.FromArgb(248, 0, 0)
        '    MtxColor(15, 0) = Color.FromArgb(170, 0, 0)
        '    MtxColor(15, 1) = Color.FromArgb(170, 0, 0)
        '    'Topbar2'
        '    MtxColor(16, 0) = Color.FromArgb(253, 253, 253)
        '    MtxColor(16, 1) = Color.FromArgb(253, 253, 253)
        '    MtxColor(17, 0) = Color.FromArgb(180, 180, 180)
        '    MtxColor(17, 1) = Color.FromArgb(180, 180, 180)
        '    'Topbar3'
        '    MtxColor(18, 0) = Color.FromArgb(220, 220, 220)
        '    MtxColor(18, 1) = Color.FromArgb(220, 220, 220)
        '    MtxColor(19, 0) = Color.FromArgb(160, 160, 160)
        '    MtxColor(19, 1) = Color.FromArgb(160, 160, 160)
        '    'Center'
        '    MtxColor(20, 0) = Color.FromArgb(248, 0, 0)
        '    MtxColor(20, 1) = Color.FromArgb(248, 0, 0)
        '    'Bottonbar1'
        '    MtxColor(21, 0) = Color.FromArgb(180, 180, 180)
        '    MtxColor(21, 1) = Color.FromArgb(180, 180, 180)
        '    MtxColor(22, 0) = Color.FromArgb(220, 220, 220)
        '    MtxColor(22, 1) = Color.FromArgb(220, 220, 220)
        '    'Bottonbar1'
        '    MtxColor(23, 0) = Color.FromArgb(180, 180, 180)
        '    MtxColor(23, 1) = Color.FromArgb(180, 180, 180)
        '    MtxColor(24, 0) = Color.FromArgb(253, 253, 253)
        '    MtxColor(24, 1) = Color.FromArgb(253, 253, 253)
        '    'Bottonbar1'
        '    MtxColor(25, 0) = Color.FromArgb(180, 180, 180)
        '    MtxColor(25, 1) = Color.FromArgb(180, 180, 180)
        '    MtxColor(26, 0) = Color.FromArgb(253, 253, 253)
        '    MtxColor(26, 1) = Color.FromArgb(253, 253, 253)
        'End If

    End Sub

    Private Sub SetDefaultImage()

        Debug.WriteLine("SetDefaultImage")

        If _FlatStyle = False Then
            MtxImageBorder(0) = My.Resources.n0
            MtxImageBorder(1) = My.Resources.n1
            MtxImageBorder(2) = My.Resources.n2
            MtxImageBorder(3) = My.Resources.n3
            MtxImageBorder(4) = My.Resources.n3
            MtxImageBorder(5) = My.Resources.n3
            MtxImageBorder(6) = My.Resources.n3
            MtxImageBorder(7) = My.Resources.n4
            MtxImageBorder(8) = My.Resources.n5
            MtxImageBorder(9) = My.Resources.n6
            MtxImageBorder(10) = My.Resources.n7
            MtxImageBorder(11) = My.Resources.n7
            MtxImageBorder(12) = My.Resources.n7
            MtxImageBorder(13) = My.Resources.n7
            ImageBar = My.Resources.bar1
        Else
            MtxImageBorder(0) = My.Resources.f0
            MtxImageBorder(1) = My.Resources.f1
            MtxImageBorder(2) = My.Resources.f2
            MtxImageBorder(3) = My.Resources.f3
            MtxImageBorder(4) = My.Resources.f3
            MtxImageBorder(5) = My.Resources.f3
            MtxImageBorder(6) = My.Resources.f3
            MtxImageBorder(7) = My.Resources.f4
            MtxImageBorder(8) = My.Resources.f5
            MtxImageBorder(9) = My.Resources.f6
            MtxImageBorder(10) = My.Resources.f7
            MtxImageBorder(11) = My.Resources.f7
            MtxImageBorder(12) = My.Resources.f7
            MtxImageBorder(13) = My.Resources.f7
            ImageBar = My.Resources.bar2
        End If

        Call SetDefaultControlBoxImage()

        If Not _Logo1 Is Nothing Then
            MtxImageLogos(0) = CType(_Logo1, Bitmap)
        End If

        If Not _Logo2 Is Nothing Then
            MtxImageLogos(1) = CType(_Logo2, Bitmap)
        Else
            MtxImageLogos(1) = CType(Panel2.BackgroundImage, Bitmap)
        End If

    End Sub

    Private Sub SetDefaultControlBoxImage()
        Debug.WriteLine("SetDefaultControlBoxImage")
        MtxImageCtlBox(0) = My.Resources.minuns
        MtxImageCtlBox(1) = My.Resources.minsel
        MtxImageCtlBox(2) = My.Resources.maxuns
        MtxImageCtlBox(3) = My.Resources.maxsel
        MtxImageCtlBox(4) = My.Resources.maxdis
        MtxImageCtlBox(5) = My.Resources.closeuns
        MtxImageCtlBox(6) = My.Resources.closesel
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        If _resume = False Then
            _resume = True
            Call DrawTitle()
            Call DrawWindowsTitle()
            Call DrawBar()
        End If
    End Sub

#End Region

#Region "Event on control"

    Private Sub iForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Call LoadTheme()
        'Call SetControlBox()
        'Call DrawTitle()
        'Call DrawWindowsTitle()
        'Call DrawBar()

        'If Me.DesignMode = False Then Call Clipping()

    End Sub

    Private Sub iForm_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Not Me.ParentForm Is Nothing Then
            If Me.ParentForm.WindowState <> FormWindowState.Minimized Then
                Call Clipping()
            End If
            ShowSysMenu(True)
        End If
    End Sub

    'Gestione minimized maximizaed close windows'
    Private Sub cnt1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cnt1.Click
        Me.SetWindowState(_WindowStateE.Minimize)
    End Sub

    Private Sub cnt2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cnt2.Click

        If _Sizable = False Then Exit Sub

        If _WindowState = _WindowStateE.Normal Then
            Me.SetWindowState(_WindowStateE.Maximized)
        Else
            Me.SetWindowState(_WindowStateE.Normal)
        End If

    End Sub

    Private Sub Pnl3_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Pnl3.DoubleClick

        If _Sizable = False Then Exit Sub

        If _WindowState = _WindowStateE.Normal Then
            Me.SetWindowState(_WindowStateE.Maximized)
        Else
            Me.SetWindowState(_WindowStateE.Normal)
        End If

    End Sub

    'Gestione passagio mouse su pulsanti control box''
    Private Sub cnt3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cnt3.Click
        If Not Me.ParentForm Is Nothing Then
            Me.ParentForm.Close()
        End If
    End Sub

    Private Sub cnt1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt1.MouseEnter
        cnt1.Image = MtxImageCtlBox(1)
    End Sub

    Private Sub cnt1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt1.MouseLeave
        cnt1.Image = MtxImageCtlBox(0)
    End Sub

    Private Sub cnt2_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt2.MouseEnter
        If _Sizable = True Then cnt2.Image = MtxImageCtlBox(3)
    End Sub

    Private Sub cnt2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt2.MouseLeave
        If _Sizable = True Then cnt2.Image = MtxImageCtlBox(2)
    End Sub

    Private Sub cnt3_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt3.MouseEnter
        cnt3.Image = MtxImageCtlBox(6)
    End Sub

    Private Sub cnt3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt3.MouseLeave
        cnt3.Image = MtxImageCtlBox(5)
    End Sub

    Public Sub New()
        InitializeComponent()
        Call Start()
    End Sub

    'Gestione move windows'
    Private Sub pnl3_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pnl3.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized Then
            If Date.Now.Subtract(tt).Milliseconds > 100 Then
                ReleaseCapture()
                SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTCAPTION, 0)
            End If
        End If
        tt = Date.Now
    End Sub

    Private Sub pnl3_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pnl3.MouseUp
        Call ReleaseCapture()
        tt = Date.Now
    End Sub

    'Gestione size windows'

    Private Sub PicAll_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pic1.MouseUp, Pic2.MouseUp, Pic3.MouseUp, Pic4.MouseUp, Pic5.MouseUp, Pic6.MouseUp, Pic7.MouseUp, Pic8.MouseUp, pic9.MouseUp, pic10.MouseUp, pic11.MouseUp, pic12.MouseUp, pic13.MouseUp
        ShowSysMenu(True)
        Call ReleaseCapture()
    End Sub

    Private Sub pic1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pic1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTTOPLEFT, 0)
        End If
    End Sub

    Private Sub pic2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pic2.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTTOP, 0)
        End If
    End Sub

    Private Sub pic3_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pic3.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTTOPRIGTH, 0)
        End If
    End Sub

    Private Sub Pic4_5_6_7_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pic4.MouseDown, Pic5.MouseDown, Pic6.MouseDown, Pic7.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTRIGHT, 0)
        End If
    End Sub

    Private Sub Pic8_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Pic8.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTBOTTOMRIGHT, 0)
        End If
    End Sub

    Private Sub pic9_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic9.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.Parent.Handle), WM_NCLBUTTONDOWN, HTBOTTOM, 0)
        End If
    End Sub

    Private Sub pic10_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic10.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTBOTTOMLEFT, 0)
        End If
    End Sub

    Private Sub pic11_12_13_14_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic11.MouseDown, pic12.MouseDown, pic13.MouseDown, pic14.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso _WindowState <> _WindowStateE.Maximized AndAlso _Sizable = True Then
            ShowSysMenu(False)
            ReleaseCapture()
            SendMessage(CInt(Me.ParentForm.Handle), WM_NCLBUTTONDOWN, HTLEFT, 0)
        End If
    End Sub

#End Region

End Class