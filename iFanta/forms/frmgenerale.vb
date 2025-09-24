Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase
Imports System.Threading

Public Class frmgenerale

    Private _winmenuleft As Integer = 6
    Private _winmenupadding As Integer = 9
    Private _winmenuminwidth As Integer = -1
    Private Thrd As Thread

    Private pic(7) As PictureBox
    Private ctlimg() As Bitmap
    Private img() As Bitmap

    Private apppage As Integer = 14
    Private tt As Date = Date.Now
    Declare Function ReleaseCapture Lib "user32.dll" () As Int32
    Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As Int32) As Int32

    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTCAPTION As Integer = 2

    Private pad As Integer = 10
    Private wtab As Integer = 60
    Private wsubtab As Integer = 60
    Private yt1 As Integer = 85
    Private yt2 As Integer = 65
    Private yb1 As Integer = Me.Height - 87
    Private yb2 As Integer = Me.Height - 65
    Private dx As Integer = 14
    Private dxapp As Integer = 23
    Private dyapp As Integer = 100
    Private dxfield As Integer = 65
    Private rowheight1 As Integer = 70
    Private napprow As Integer = 1

    Private ctheme As Integer = 1
    Private _oldind As Integer = -1

    Private recttab As New List(Of Rectangle)
    Private rectsubtab As New List(Of Rectangle)
    Private rectpath As New List(Of Rectangle)
    Private rectapp As New List(Of Rectangle)
    Private rectcopy As New List(Of Rectangle)
    Private rectdel As New List(Of Rectangle)
    Private rectfav As New List(Of Rectangle)

    Private rectflat As New Rectangle(0, 0, 10, 10)
    Private recttheme As New List(Of Rectangle)

    Private imgappstring As New Dictionary(Of String, Bitmap)
    Private imgtempext As String = ".tmp"
    Private dirimgtemp As String = My.Application.Info.DirectoryPath & "\temp\image\"
    Private _scrollthemeindex As Integer = 0
    Private t As New List(Of String)
    Private numthemecol As Integer = 4
    Private numthemerow As Integer = 6

    Private wsize As New List(Of Size)

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

    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function SetWindowLong(ByVal hwnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function GetWindowLong(ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Shared Function SetWindowPos(ByVal hwnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal flags As Integer) As Boolean
    End Function

#Region "Function"

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)
        ShowSysMenu(True)
    End Sub

    Public Sub pRedraw()
        ' Redraw window with new style.
        Const swpFlags As Long = _
           SWP_FRAMECHANGED Or SWP_NOMOVE Or _
           SWP_NOZORDER Or SWP_NOSIZE
        Call SetWindowPos(Me.Handle, CType(0, IntPtr), 0, 0, 0, 0, swpFlags)
    End Sub

    Private Sub ShowSysMenu(ByVal Visible As Boolean)
        If Visible = True Then
            Dim OldStyle As Integer = GetWindowLong(Me.Handle, GWL_STYLE)
            OldStyle = OldStyle Or WS_SYSMENU Or WS_MINIMIZEBOX
            If (OldStyle And WS_MAXIMIZEBOX) = WS_MAXIMIZEBOX Then
                OldStyle = OldStyle And Not WS_MAXIMIZEBOX
            End If
            SetWindowLong(Me.Handle, GWL_STYLE, OldStyle)
        Else
            Me.FormBorderStyle = FormBorderStyle.None
        End If
        pRedraw()
    End Sub

    Protected Overridable Function DefaultCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        Dim val As Boolean = MyBase.ProcessCmdKey(msg, keyData)
        Return val
    End Function

    Function KeyScrollPage(ByVal keyData As Keys) As Boolean
        Dim ris As Boolean = False
        Try
            Select Case keyData
                Case Keys.Up, Keys.PageUp
                    If vsrc1.Visible Then
                        If keyData = Keys.Up Then
                            applist.CurrentSubTab.ScrollIndex = applist.CurrentSubTab.ScrollIndex - 1
                        End If
                        If keyData = Keys.PageUp Then
                            applist.CurrentSubTab.ScrollIndex = applist.CurrentSubTab.ScrollIndex - apppage
                        End If
                        If applist.CurrentSubTab.ScrollIndex < 0 Then applist.CurrentSubTab.ScrollIndex = 0
                        vsrc1.Value = applist.CurrentSubTab.ScrollIndex
                        Call DrawApp()
                        ris = True
                    End If
                    If vsrc2.Visible Then
                        If keyData = Keys.Up Then
                            _scrollthemeindex = _scrollthemeindex - 1
                        End If
                        If keyData = Keys.PageUp Then
                            _scrollthemeindex = _scrollthemeindex - numthemecol
                        End If
                        If _scrollthemeindex < 0 Then _scrollthemeindex = 0
                        vsrc2.Value = _scrollthemeindex
                        Call DrawTheme()
                        ris = True
                    End If
                Case Keys.Down, Keys.PageDown
                    If vsrc1.Visible Then
                        If keyData = Keys.Down Then
                            applist.CurrentSubTab.ScrollIndex = applist.CurrentSubTab.ScrollIndex + 1
                        End If
                        If keyData = Keys.PageDown Then
                            applist.CurrentSubTab.ScrollIndex = applist.CurrentSubTab.ScrollIndex + apppage
                        End If
                        If applist.CurrentSubTab.ScrollIndex > vsrc1.Max Then applist.CurrentSubTab.ScrollIndex = vsrc1.Max
                        vsrc1.Value = applist.CurrentSubTab.ScrollIndex
                        Call DrawApp()
                        ris = True
                    End If
                    If vsrc2.Visible Then
                        If keyData = Keys.Down Then
                            _scrollthemeindex = _scrollthemeindex + 1
                        End If
                        If keyData = Keys.PageDown Then
                            _scrollthemeindex = _scrollthemeindex + numthemecol
                        End If
                        If _scrollthemeindex > vsrc2.Max Then _scrollthemeindex = vsrc2.Max
                        vsrc2.Value = _scrollthemeindex
                        Call DrawTheme()
                        ris = True
                    End If
                Case Else
                    For i As Integer = 0 To applist.TabAppList.Count - 1
                        Dim kc As KeysConverter = New KeysConverter()
                        If applist.TabAppList(i).ShortCut <> "" Then
                            Dim o As Object = kc.ConvertFromString(applist.TabAppList(i).ShortCut)
                            Dim keyCode As Keys = CType(o, Keys)
                            If keyData = Keys.Alt + keyCode Then
                                RunApplication(applist.TabAppList(i))
                                ris = True
                                Exit For
                            End If
                        End If
                    Next
            End Select
        Catch ex As Exception

        End Try
        Return ris
    End Function

    Sub SetHotKeys()
        mnu1.Items.Clear()
        For i As Integer = 1 To 12
            If AppSett.MainWindows.HotKeys.ContainsKey(i) Then
                mnu1.Items.Add("F" & CStr(i) & " -   " & AppSett.MainWindows.HotKeys.Item(i).AppName)
            Else
                mnu1.Items.Add("F" & CStr(i) & " -   Empty")
            End If
            mnu1.Items(i - 1).Tag = CStr(i)
        Next
    End Sub

    Public Shared Sub AddSystemMenu(ByVal tlb As iControl.iToolBar, ByVal MainMenu As String)

        tlb.Button.Clear()
        tlb.Button.Add(New iControl.ToolbarButton("Main", MainMenu, iControl.ToolbarButton.iType.ButtonDropDown))
        tlb.Visible = True
        tlb.Enabled = True
        tlb.Button(0).ShowColumnImage = True
        tlb.Button(0).SubWidth = 140
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Carica torneo", My.Resources.load_16, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Separator))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Nuovo torneo", My.Resources.new_torneo16, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Modifica torneo", My.Resources.mod_torneo16, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Elimina torneo", My.Resources.del_torneo16, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Separator))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Aggiorna dati torneo su server remoto", My.Resources.update14, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Separator))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Backup dati", My.Resources.backup16, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Ripristino dati da backup", My.Resources.restore16, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Separator))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Controlla aggiornamenti " & My.Application.Info.AssemblyName, My.Resources.update14, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Ripristina " & My.Application.Info.AssemblyName, iControl.ToolBarButtonSubItem.SubButtonStype.Item))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("", iControl.ToolBarButtonSubItem.SubButtonStype.Separator))
        tlb.Button(0).SubItems.Add(New iControl.ToolBarButtonSubItem("Impostazioni iFanta", My.Resources.serttings_16, iControl.ToolBarButtonSubItem.SubButtonStype.Item))

        tlb.Button(0).SubItems(0).Tag = "8"
        tlb.Button(0).SubItems(2).Tag = "0"
        tlb.Button(0).SubItems(3).Tag = "1"
        tlb.Button(0).SubItems(4).Tag = "2"
        tlb.Button(0).SubItems(6).Tag = "9"
        tlb.Button(0).SubItems(8).Tag = "3"
        tlb.Button(0).SubItems(9).Tag = "4"
        tlb.Button(0).SubItems(11).Tag = "5"
        tlb.Button(0).SubItems(12).Tag = "6"
        tlb.Button(0).SubItems(14).Tag = "7"

        tlb.Button(0).SubItems(5).Visible = currlega.Settings.Admin
        tlb.Button(0).SubItems(6).Visible = currlega.Settings.Admin

    End Sub

    Sub SetEnabledItemsMenuSystem()

        Dim ena As Boolean = True

        If currlega.Settings.Nome = "" Then ena = False

        tlbmenu.Button(0).SubItems(3).Enabled = ena
        tlbmenu.Button(0).SubItems(4).Enabled = ena
        tlbmenu.Button(0).SubItems(6).Enabled = ena
        tlbmenu.Button(0).SubItems(7).Enabled = ena
       
    End Sub

    Sub SetTheme(ByVal UpdateScrollIndex As Boolean)

        Me.Size = wsize(AppSett.MainWindows.WindowsSize)

        If AppSett.MainWindows.WindowsSize = 1 Then
            numthemecol = 5
            numthemerow = 9
        Else
            numthemecol = 4
            numthemerow = 6
        End If
        tlb1.draw(True)

        'Setto il tema'

        yb1 = Me.Height - 62
        yb2 = Me.Height - 42

        picfind.Left = dx

        txt3.Top = Me.Height - 84
        txt3.Left = picfind.Left + picfind.Width + 3
        picfind.Top = Me.Height - 85

        vsrc1.Visible = False
        vsrc2.Visible = False

        AppSett.Personal.Theme.ReadSettings()

        Call LoadPage(UpdateScrollIndex)
        Call SetControlBox()
        Call Clipping()

        vsrc1.Visible = False
        vsrc1.ShowArrow = False
        vsrc1.BorderColor = Color.Transparent
        vsrc1.BorderSize = 1
        vsrc1.BackColor = Color.Transparent
        vsrc1.Left = Me.Width - 18
        vsrc1.Top = 95
        vsrc1.Height = Me.Height - vsrc1.Top - 70
        vsrc1.Background = iControl.CommonFunction.GetAreaImage(picback.BackgroundImage, vsrc1.Left, vsrc1.Top, vsrc1.Width, vsrc1.Height)
        vsrc1.FlatStyle = AppSett.Personal.Theme.FlatStyle
        If AppList.TabAppList.Count - apppage > 0 Then
            vsrc1.Draw(True)
            If AppList.CurrentTab.CurrentSubTab.System = False Then vsrc1.Visible = True
        End If

        vsrc2.ShowArrow = False
        vsrc2.BorderColor = Color.Transparent
        vsrc2.BorderSize = 1
        vsrc2.BackColor = Color.Transparent
        vsrc2.Left = Me.Width - dxfield + 2
        vsrc2.Top = 228
        vsrc2.Height = numthemerow * 20 + 12
        vsrc2.Background = iControl.CommonFunction.GetAreaImage(picback.BackgroundImage, vsrc2.Left, vsrc2.Top, vsrc2.Width, vsrc2.Height)
        vsrc2.FlatStyle = AppSett.Personal.Theme.FlatStyle
        If t.Count - numthemerow * numthemecol > 0 Then
            vsrc2.Draw(True)
            If AppList.CurrentTab.CurrentSubTab.System Then vsrc2.Visible = True
        End If

        tlbmenu.MaxDropDownItems = -1
        tlbmenu.Button(0).SubItemsAutoSize = True
        tlbmenu.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbmenu.draw(True)

        tlb1.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlb1.Top = picfind.Top - 1
        tlb1.Left = Me.Width - dx - tlb1.Width - 3
        tlb1.BorderColor = Color.FromArgb(70, 0, 0, 0)
        tlb1.SelectionColor = Color.FromArgb(60, 255, 255, 255)
        tlb1.Background = iControl.CommonFunction.GetAreaImage(picback.BackgroundImage, tlb1.Left, tlb1.Top, tlb1.Width, tlb1.Height)

    End Sub

    Sub LoadPage(ByVal UpdateScrollIndex As Boolean)
        AppList.Search = txt3.Text
        If AppList.Tabs.Count > 0 AndAlso AppList.CurrentSubTab.System = False Then
            Dim fav As Boolean = False
            Dim showdir As Boolean = True
            Dim subdir As Boolean = False
            If AppList.Search <> "" Then showdir = True : subdir = True
            If AppList.CurrentTab.Name.ToLower = AppList.TabFavoritesName.ToLower Then fav = True : showdir = False : subdir = True
            AppList.Load(showdir, subdir, fav)
        End If
        Call LoadAppImageCache()
        Call DisplayPage(UpdateScrollIndex)
    End Sub

    Sub LoadAppImageCache()
        imgappstring.Clear()
        If AppList.Tabs.Count > 0 Then
            For i As Integer = AppList.CurrentSubTab.ScrollIndex To AppList.CurrentSubTab.ScrollIndex + apppage - 1
                If i < AppList.TabAppList.Count Then
                    LoadAppImageCache(i)
                End If
            Next
        End If
    End Sub

    Function LoadAppImageCache(ByVal Index As Integer) As Boolean
        Return LoadAppImageCache(Index, AppList.TabAppList(Index).Enabled)
    End Function

    Function LoadAppImageCache(ByVal Index As Integer, ByVal Enable As Boolean) As Boolean
        Dim key As String = GetAppImageKey(Index, Enable)
        Dim fname1 As String = dirimgtemp & key & imgtempext
        If IO.File.Exists(fname1) Then
            imgappstring.Add(key, New Bitmap(fname1))
            Return True
        Else
            Return False
        End If
    End Function

    Sub DisplayPage(ByVal UpdateScrollIndex As Boolean)
        Call Draw()
        If AppList.Tabs.Count > 0 Then
            Select Case AppList.CurrentTab.Name.ToLower
                Case "opzioni"
                    picfind.Visible = False
                    txt3.Visible = False
                    vsrc1.Visible = False
                    picback.Image = Nothing
                    tlb1.Visible = False
                    t = GetThemeList()
                    If UpdateScrollIndex Then
                        Dim a As Integer = GetThemeSelectIndex()
                        _scrollthemeindex = CInt(Math.Ceiling((GetThemeSelectIndex() + 1) / numthemecol)) - numthemerow
                        If _scrollthemeindex < 0 Then _scrollthemeindex = 0
                    End If
                    Call DrawSettings()
                    cmdconne.Visible = True
                    cmdupd.Visible = True
                    cmdrest.Visible = True
                    vsrc1.BackColor = Color.Transparent
                    If t.Count - numthemerow * numthemecol > 0 Then
                        vsrc2.Max = CInt(Math.Ceiling((t.Count - numthemerow * numthemecol) / numthemecol))
                        vsrc2.Value = _scrollthemeindex
                        vsrc2.Draw(True)
                        vsrc2.Visible = True
                    Else
                        vsrc2.Visible = False
                    End If
                    vsrc2.Draw(True)
                Case "info"
                    picfind.Visible = False
                    txt3.Visible = False
                    vsrc1.Visible = False
                    vsrc2.Visible = False
                    cmdconne.Visible = False
                    cmdupd.Visible = False
                    cmdrest.Visible = False
                    tlb1.Visible = False
                    Call DrawInfo()
                Case Else
                    rowheight1 = GetRowHeight()
                    apppage = CInt(Math.Floor((yb1 - yt1 - 30) / (rowheight1 + 1)))
                    napprow = GetNumberAppRow()
                    apppage = apppage * napprow
                    If AppList.CurrentSubTab.ScrollIndex > (AppList.TabAppList.Count - apppage) \ napprow Then AppList.CurrentSubTab.ScrollIndex = 0
                    picfind.Visible = True
                    txt3.Visible = True
                    vsrc2.Visible = False
                    cmdconne.Visible = False
                    cmdupd.Visible = False
                    cmdrest.Visible = False
                    vsrc1.BackColor = Color.Transparent
                    If AppList.TabAppList.Count - apppage > 0 Then
                        vsrc1.Max = (AppList.TabAppList.Count - apppage) \ napprow
                        vsrc1.Value = AppList.CurrentSubTab.ScrollIndex
                        vsrc1.Draw(True)
                        vsrc1.Visible = True
                    Else
                        vsrc1.Visible = False
                    End If
                    If AppList.CurrentSubTab.DefaultFolder <> "" Then
                        tlb1.Button(4).Enabled = True
                    Else
                        tlb1.Button(4).Enabled = False
                    End If
                    If applist.CurrentSubTab.View = -1 Then
                        tlb1.Button(2).Enabled = True
                        tlb1.Button(3).Enabled = True
                    Else
                        tlb1.Button(2).Enabled = False
                        tlb1.Button(3).Enabled = False
                    End If
                    tlb1.draw(True)
                    tlb1.Visible = True
                    Call DrawApp()
            End Select
        Else
            picfind.Visible = False
            txt3.Visible = False
            vsrc1.Visible = False
            vsrc2.Visible = False
            cmdconne.Visible = False
            cmdupd.Visible = False
            cmdrest.Visible = False
        End If
    End Sub

    Function GetThemeSelectIndex() As Integer
        Dim ind As Integer = 0
        For i As Integer = 0 To t.Count - 1
            If t(i).ToLower = AppSett.Personal.Theme.Name.ToLower Then
                ind = i
                Exit For
            End If
        Next
        Return ind
    End Function

    Function GetAppEnabled(ByVal Index As Integer) As Boolean

        Dim ena As Boolean = True

        Try
            If applist.AppList.Count > Index Then
                If applist.AppList(Index).Enabled = False AndAlso currlega.Settings.Admin = False Then ena = False
                If applist.AppList(Index).Name = "Aggiorna prob. formazioni" OrElse applist.AppList(Index).Name = "Aggiorna dati server" Then
                    ena = currlega.Settings.Active
                End If
                If legalist.Count = 0 OrElse currlega.Settings.Nome = "" Then ena = False
                Return ena
            Else
                ena = False
            End If
        Catch ex As Exception

        End Try

        Return ena

    End Function

    Sub RunApplication(ByVal App As iApplication)
        Try
            If App.Enabled = True Then
                Select Case App.Library
                    Case "isystem.dll"

                    Case Else
                        Select Case App.ID
                            Case 0
                                applist.CurrentSubTab.ScrollIndex = 0
                                applist.CurrentSubTab.Directory = applist.CurrentSubTab.Directory & App.Name & "\"
                                Call LoadPage(True)
                            Case 1
                                Dim frm As New frmrose
                                frm.Show()
                            Case 2
                                Dim frm As New frmroseall
                                frm.Show()
                            Case 3
                                Process.Start(App.Path.Replace("&amp;", "&"), "")
                            Case 4
                                Dim frm As New frmformazioni
                                frm.Show()
                            Case 5
                                Dim frm As New frmcoppa
                                frm.Show()
                            Case 6
                                Dim frm As New frmclassifica
                                frm.Show()
                            Case 7
                                Dim frm As New frmstat
                                frm.Show()
                            Case 8
                                Dim frm As New frmbackup
                                frm.Show()
                            Case 9
                                Dim frm As New frmcompila
                                frm.Show()
                            Case 10
                                Dim frm As New frmupdateserver
                                frm.Show()
                            Case 11
                                Dim frm As New frmupdateprobforma
                                frm.Show()
                            Case 12
                                Dim frm As New frmrestore
                                frm.Show()
                            Case 13
                                System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\ifup.exe", "")
                        End Select
                End Select
            End If
        Catch ex As Exception

        End Try
    End Sub

    Sub Clipping()

        Dim myPath As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

        'Disegno i rettangoli'
        myPath.ClearMarkers()
        myPath.AddRectangle(New Rectangle(img(0).Width, 0, Me.Width - (img(0).Width + img(2).Width), img(0).Height))
        myPath.AddRectangle(New Rectangle(0, img(0).Height, Me.Width, Me.Height - (img(0).Height + img(5).Height)))
        myPath.AddRectangle(New Rectangle(img(6).Width, Me.Height - img(5).Height, Me.Width - (img(4).Width + img(6).Width), img(5).Height))

        For i As Integer = 0 To 7
            If i = 0 OrElse i = 2 OrElse i = 4 OrElse i = 6 Then
                For j As Integer = 0 To img(i).Height - 1
                    For k As Integer = 0 To img(i).Width - 1
                        If img(i).GetPixel(k, j).A > 0 Then
                            myPath.AddRectangle(New Rectangle(k + pic(i).Left, j + pic(i).Top, 1, 1))
                        End If
                    Next
                Next
            End If
        Next

        'Associo l'area al controllo'
        Me.Region = New Region(myPath)

        myPath.Dispose()

    End Sub

    Sub DrawBorder(ByVal Pic As PictureBox, ByVal Image As Image, ByVal BackImage As Image)
        Dim gr As Graphics
        Dim b1 As Bitmap
        Dim delta As Integer = 0
        b1 = New Bitmap(Pic.Width, Pic.Height)
        gr = Graphics.FromImage(b1)
        gr.DrawImage(BackImage, New Rectangle(0, 0, Pic.Width, Pic.Height), New Rectangle(Pic.Left, Pic.Top, Pic.Width, Pic.Height), GraphicsUnit.Pixel)
        For i As Integer = 0 To Pic.Width Step Image.Width
            For j As Integer = 0 To Pic.Height Step Image.Height
                gr.DrawImage(Image, i, j)
            Next
        Next
        Pic.BackgroundImageLayout = ImageLayout.Tile
        Pic.BackgroundImage = CType(b1.Clone, Drawing.Image)
        b1.Dispose()
        gr.Dispose()
    End Sub

    Sub Draw()

        Try

            Dim ctl(7) As PictureBox
            Dim gr As Graphics
            Dim b1 As Bitmap

            pic(0) = pic0
            pic(1) = pic1
            pic(2) = pic2
            pic(3) = pic3
            pic(4) = pic4
            pic(5) = pic5
            pic(6) = pic6
            pic(7) = pic7

            b1 = New Bitmap(picback.Width, picback.Height)
            gr = Graphics.FromImage(b1)

            'Impostazioni qualita' immagine'
            gr.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

            Call DrawBackground(gr)

            'Carico le immagini di default dei bordi della finestra'
            img = iControl.CommonFunction.GetDefaultThemeBorderMinimalImage(AppSett.Personal.Theme.FlatStyle)

            'Carico le immagini custom del thema'
            For i As Integer = 0 To 7
                Dim ind As Integer = i
                If i > 3 Then ind = i + 3
                Dim subdir As String = "Normal"
                If AppSett.Personal.Theme.FlatStyle Then subdir = "Flat"
                Dim fname1 As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name & "\" & subdir & "\" & ind & ".png"
                Dim fname2 As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name & "\" & ind & ".png"
                If IO.File.Exists(fname1) Then
                    img(i) = New Bitmap(fname1)
                ElseIf IO.File.Exists(fname2) Then
                    img(i) = New Bitmap(fname2)
                End If
            Next

            'Carico le immagini di default della control box'
            ctlimg = iControl.CommonFunction.GetDefaultControlBoxImage(AppSett.Personal.Theme.FlatStyle)

            'Carico le immagini custom del thema'
            For i As Integer = 0 To 6
                Dim subdir As String = "Normal"
                If AppSett.Personal.Theme.FlatStyle Then subdir = "Flat"
                Dim fname1 As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name & "\" & subdir & "\c" & i & ".png"
                Dim fname2 As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name & "\c" & i & ".png"
                If IO.File.Exists(fname1) Then
                    ctlimg(i) = New Bitmap(fname1)
                ElseIf IO.File.Exists(fname2) Then
                    ctlimg(i) = New Bitmap(fname2)
                End If
            Next

            'Posizioni i controlli ai bordi della finestra'
            pic(0).Top = 0 : pic(0).Left = 0 : pic(0).Width = img(0).Width : pic(0).Height = img(0).Height
            pic(1).Top = 0 : pic(1).Left = pic(0).Left + pic(0).Width : pic(1).Width = picback.Width - img(0).Width * 2 : pic(1).Height = img(1).Height
            pic(2).Top = 0 : pic(2).Left = pic(1).Left + pic(1).Width : pic(2).Width = img(2).Width : pic(2).Height = img(2).Height
            pic(3).Top = pic(2).Top + pic(2).Height : pic(3).Left = picback.Width - img(3).Width : pic(3).Height = picback.Height - img(2).Height - img(4).Height : pic(3).Width = img(3).Width
            pic(4).Top = picback.Height - img(4).Height : pic(4).Left = picback.Width - img(4).Width : pic(4).Height = img(4).Height : pic(4).Width = img(4).Width
            pic(5).Top = picback.Height - img(5).Height : pic(5).Left = img(6).Width : pic(5).Height = img(5).Height : pic(5).Width = picback.Width - img(4).Width - img(6).Width
            pic(6).Top = picback.Height - img(6).Height : pic(6).Left = 0 : pic(6).Height = img(6).Height : pic(6).Width = img(6).Width
            pic(7).Top = img(0).Height : pic(7).Left = 0 : pic(7).Height = picback.Height - img(0).Height - img(6).Height : pic(7).Width = img(7).Width

            'Dipingo i bordi della finestra'
            For i As Integer = 0 To 7
                Call DrawBorder(pic(i), img(i), b1)
            Next

            'Disegno il titolo della finestra'
            Dim privateFontCollection As New PrivateFontCollection
            Dim FontSize As Integer = 21
            Dim familyName As String = "bauhs93"
            Dim font1 As Font = New Font("Arial", FontSize, FontStyle.Bold, GraphicsUnit.Pixel)
            Dim path As String = ""

            'Determino il path da dove prelevare il font'  
            If System.IO.File.Exists(My.Application.Info.DirectoryPath & "\Theme\" & AppSett.Personal.Theme.Name & "\font.ttf") = True Then
                path = My.Application.Info.DirectoryPath & "\Theme\" & AppSett.Personal.Theme.Name & "\font.ttf"
            Else
                path = My.Application.Info.DirectoryPath & "\font.ttf"
            End If

            'Carico il font'
            If System.IO.File.Exists(path) = True Then
                Dim thisFont As FontFamily
                privateFontCollection.AddFontFile(path)
                thisFont = privateFontCollection.Families(0)
                font1 = New Font(thisFont, FontSize, FontStyle.Regular, GraphicsUnit.Pixel)
                privateFontCollection.Families(0).Dispose()
                thisFont.Dispose()
            End If

            'Titolo programma'
            Dim format As New StringFormat
            format.Alignment = StringAlignment.Near
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            gr.SmoothingMode = SmoothingMode.AntiAlias
            gr = iControl.CommonFunction.DrawGlowText(gr, My.Application.Info.AssemblyName, font1, Color.White, Color.Gainsboro, Color.Black, True, 2, 2, 15, 180, 30, 9, 32, format)
            gr = iControl.CommonFunction.DrawGlowText(gr, "rel " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & " " & My.Application.Info.Description, font1, Color.FromArgb(255, 211, 168), Color.FromArgb(225, 140, 51), Color.Black, True, 2, 2, 15, 180, 30, 80, 32, format)

            Dim tnome As String = currlega.Settings.Nome

            If tnome = "" Then tnome = "xxxxx"

            Dim ft1 As Font = New Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Pixel)
            Dim w1 As Integer = CInt(gr.MeasureString(tnome, ft1).Width)

            format.Alignment = StringAlignment.Far
            gr = iControl.CommonFunction.DrawGlowText(gr, "Torneo:", ft1, Color.White, Color.Gainsboro, Color.Black, True, 2, 2, 15, Me.Width - w1 - 30, 30, 9, 36, format)
            gr = iControl.CommonFunction.DrawGlowText(gr, tnome, ft1, Color.FromArgb(255, 211, 168), Color.FromArgb(225, 140, 51), Color.Black, True, 2, 2, 15, Me.Width - 20, 30, 0, 36, format)

            'gr.DrawImage(ConvDisable(My.Resources.user24, 0.6), Me.Width - (dx + My.Resources.user24.Width - 3), 33)
            'format.Alignment = StringAlignment.Far
            'gr.TextRenderingHint = TextRenderingHint.AntiAlias

            'gr.DrawImage(ConvDisable(DrawGlowText3(usr, New Font("Arial", 11, FontStyle.Bold,GraphicsUnit.Pixel), Color.White, Color.White, Color.FromArgb(70, 70, 70), 120, False, False, format, Me.Width - dx - 12), 0.6), 0, 34)
            ''gr = iControl.CommonFunction.DrawGlowText(gr, usr, New Font("Arial", 12, FontStyle.Bold,GraphicsUnit.Pixel), Color.Red, Color.CornflowerBlue, Color.White, True, 1, 1, 15, Me.Width - dx * 2, 30, 0, 34, format)

            privateFontCollection.Dispose()

            'Disegno il path corrente'
            format.Alignment = StringAlignment.Near
            gr.SmoothingMode = SmoothingMode.AntiAlias
            Call DrawPathDirectory(gr, format)


            '****SOTTO SCHEDE****'

            'Determino le aree delle sotto schede'
            rectsubtab.Clear()
            Dim curdx As Integer = dx
            For i As Integer = applist.CurrentTab.SubTab.Count To 1 Step -1
                rectsubtab.Add(New Rectangle(Me.Width - dx - pad - i * wtab, yt2, wtab, yt1 - yt2))
            Next
            gr.SmoothingMode = SmoothingMode.AntiAlias
            'Disegno le schede'
            If AppSett.Personal.Theme.FlatStyle = False Then
                DrawTab(gr, rectsubtab, 2, Color.FromArgb(10, 0, 0, 0), 3, applist.CurrentTab.SubTabIndex, False)
                DrawTab(gr, rectsubtab, 2, Color.FromArgb(40, 0, 0, 0), 2, applist.CurrentTab.SubTabIndex, False)
                DrawTab(gr, rectsubtab, 1, Color.FromArgb(90, 0, 0, 0), 1, applist.CurrentTab.SubTabIndex, False)
                DrawTab(gr, rectsubtab, 0, Color.White, 1, applist.CurrentTab.SubTabIndex, False)
            Else
                DrawTab(gr, rectsubtab, 1, Color.FromArgb(90, 0, 0, 0), 1, applist.CurrentTab.SubTabIndex, False)
                DrawTab(gr, rectsubtab, 0, Color.White, 1, applist.CurrentTab.SubTabIndex, False)
            End If

            'Disegno la sfumatura della sotto scheda selezionata'
            Dim r As New Rectangle(rectsubtab(applist.CurrentTab.SubTabIndex).X + 1, rectsubtab(applist.CurrentTab.SubTabIndex).Y, rectsubtab(applist.CurrentTab.SubTabIndex).Width - 2, rectsubtab(applist.CurrentTab.SubTabIndex).Height)
            If AppSett.Personal.Theme.FlatStyle = False Then
                gr.FillRectangle(New LinearGradientBrush(r, Color.FromArgb(100, 255, 255, 255), Color.Transparent, LinearGradientMode.Vertical), New Rectangle(r.Left, r.Top, r.Width, r.Height - 1))
            End If
            'Disegno i nomi delle sotto schede'
            format.Alignment = StringAlignment.Center
            For i As Integer = 0 To applist.CurrentTab.SubTab.Count - 1
                gr = iControl.CommonFunction.DrawGlowText(gr, applist.CurrentTab.SubTab(i).Name, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, Color.Black, True, 1, 1, 90, wtab, 30, Me.Width - dx - pad - (applist.CurrentTab.SubTab.Count - i) * wtab, yt1 - 16, format)
            Next

            '****SCHEDE****'

            'Determino le dimensioni delle schede'
            wtab = (Me.Width - pad * 2 - dx * 2) \ applist.Tabs.Count
            'Determino le aree delle schede'
            recttab.Clear()
            Dim fonttab As Font = New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel)
            Dim t As New List(Of String)
            For i As Integer = 0 To applist.Tabs.Count - 1
                t.Add(applist.Tabs(i).Name)
            Next
            Dim w As List(Of Integer) = GetTextWidth(gr, t, fonttab, Me.Width - pad * 2 - dx * 2)
            Dim dxt As Integer = 0
            For i As Integer = 0 To applist.Tabs.Count - 1
                recttab.Add(New Rectangle(dx + pad + dxt, yb1, w(i), yb2 - yb1))
                dxt = dxt + w(i)
            Next
            'Disegno la sfumatura della scheda selezionata'
            r = New Rectangle(recttab(applist.TabIndex).X, recttab(applist.TabIndex).Y, recttab(applist.TabIndex).Width, recttab(applist.TabIndex).Height)
            If AppSett.Personal.Theme.FlatStyle = False Then gr.FillRectangle(New LinearGradientBrush(r, Color.Transparent, Color.FromArgb(70, 255, 255, 255), LinearGradientMode.Vertical), r)
            'Disegno le schede'
            If AppSett.Personal.Theme.FlatStyle = False Then
                DrawTab(gr, recttab, 2, Color.FromArgb(20, 0, 0, 0), 3, applist.TabIndex, True)
                DrawTab(gr, recttab, 2, Color.FromArgb(60, 0, 0, 0), 2, applist.TabIndex, True)
                DrawTab(gr, recttab, 1, Color.FromArgb(130, 0, 0, 0), 1, applist.TabIndex, True)
                DrawTab(gr, recttab, 0, Color.White, 1, applist.TabIndex, True)
            Else
                DrawTab(gr, recttab, 1, Color.FromArgb(90, 0, 0, 0), 1, applist.TabIndex, True)
                DrawTab(gr, recttab, 0, Color.White, 1, applist.TabIndex, True)
            End If

            For i As Integer = 1 To recttab.Count - 1
                gr.DrawLine(New Pen(New LinearGradientBrush(New Rectangle(0, yb1, 1, yb2 - yb1), Color.FromArgb(50, 0, 0, 0), Color.Transparent, LinearGradientMode.Vertical), 1), recttab(i).Left, recttab(i).Top + 1, recttab(i).Left, recttab(i).Top + (yb2 - yb1) - 2)
            Next

            'Disegno i nomi delle schede'
            format.Alignment = StringAlignment.Center
            For i As Integer = 0 To applist.Tabs.Count - 1
                gr = iControl.CommonFunction.DrawGlowText(gr, applist.Tabs(i).Name, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, Color.Black, True, 1, 1, 70, recttab(i).Width, 30, recttab(i).X, yb1 + 4, format)
            Next

            'Barra di stato connesisoni e info copyright'
            gr.SmoothingMode = SmoothingMode.Default
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
            If AppSett.Personal.Theme.FlatStyle = False Then
                gr.FillRectangle(New LinearGradientBrush(New Rectangle(dx, Me.Height - 34, Me.Width - dx * 2, 21), Color.Transparent, Color.FromArgb(30, 0, 0, 0), LinearGradientMode.Vertical), New Rectangle(dx, Me.Height - 34, Me.Width - dx * 2, 21))
            Else
                gr.FillRectangle(New LinearGradientBrush(New Rectangle(dx, Me.Height - 34, Me.Width - dx * 2, 21), Color.FromArgb(10, 0, 0, 0), Color.FromArgb(10, 0, 0, 0), LinearGradientMode.Vertical), New Rectangle(dx, Me.Height - 34, Me.Width - dx * 2, 21))
            End If
            gr.DrawRectangle(New Pen(Color.FromArgb(50, 0, 0, 0), 1), New Rectangle(dx, Me.Height - 34, Me.Width - dx * 2, 21))
            'Stato connesisone'
            format.Alignment = StringAlignment.Near
            If conn.State = ConnectionState.Open Then
                gr.DrawImage(My.Resources.connect_ok16, dx + 3, Me.Height - 31)
                gr = iControl.CommonFunction.DrawGlowText(gr, "Connesso", New Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), Color.Black, True, 1, 1, 15, Me.Width, 30, dx + 20, Me.Height - 30, format)
            Else
                gr.DrawImage(My.Resources.connect_notok16, dx + 3, Me.Height - 31)
                gr = iControl.CommonFunction.DrawGlowText(gr, "Disconnesso", New Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), Color.Black, True, 1, 1, 15, Me.Width, 30, dx + 20, Me.Height - 30, format)
            End If
            'CopyRight'
            format.Alignment = StringAlignment.Far
            gr = iControl.CommonFunction.DrawGlowText(gr, My.Application.Info.Copyright, New Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), Color.Black, True, 1, 1, 15, Me.Width - dx * 2 - 5, 30, dx, Me.Height - 30, format)
            picback.BackgroundImage = CType(b1.Clone, Image)
            b1.Dispose()
            gr.Dispose()

            rectapp.Clear()
            rectcopy.Clear()
            rectdel.Clear()
            rectfav.Clear()

            picfind.BackgroundImage = iControl.CommonFunction.GetAreaImage(picback.BackgroundImage, picfind.Left, picfind.Top, picfind.Width, picfind.Height)

            Call DrawMenuTilte()

        Catch ex As Exception

        End Try

    End Sub

    Function GetTextWidth(ByVal gr As Graphics, ByVal t As List(Of String), ByVal Font As Font, ByVal Wi As Integer) As List(Of Integer)
        Dim w As New List(Of Integer)
        Dim wm As Integer = Wi \ t.Count
        Dim wo As Integer = 0
        Dim tu As Integer = 0
        Dim wtot As Integer = 0
        For i As Integer = 0 To t.Count - 1
            Dim a As Integer = CInt(gr.MeasureString(t(i), Font).Width + 10)
            w.Add(a)
            If a < wm Then tu = tu + 1 Else wo = wo + a
        Next
        If wo > 0 Then
            Dim dw As Integer = (Wi - wo) \ tu
            For i As Integer = 0 To w.Count - 1
                If w(i) < wm Then w(i) = dw
                wtot = wtot + w(i)
            Next
        Else
            For i As Integer = 0 To w.Count - 1
                w(i) = wm
                wtot = wtot + w(i)
            Next
        End If
        If wtot < Wi Then
            Dim dw As Integer = (Wi - wtot)
            For i As Integer = 0 To dw - 1
                w(i) = w(i) + 1
            Next
        End If
        Return w
    End Function

    Public Sub DrawMenuTilte()

        Debug.WriteLine("DrawMenuTitle")

        'Carico le immagini di default della control box'
        Dim MtxMenu() As Bitmap = iControl.CommonFunction.GetDefaultThemeMenuImage(AppSett.Personal.Theme.FlatStyle)

        'Carico le immagini custom del thema'
        For i As Integer = 0 To 2
            Dim subdir As String = "Normal"
            If AppSett.Personal.Theme.FlatStyle Then subdir = "Flat"
            Dim fname1 As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name & "\" & subdir & "\m" & i & ".png"
            Dim fname2 As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name & "\m" & i & ".png"
            If IO.File.Exists(fname1) Then
                MtxMenu(i) = New Bitmap(fname1)
            ElseIf IO.File.Exists(fname2) Then
                MtxMenu(i) = New Bitmap(fname2)
            End If
        Next

        'Posiziono la toolbar'
        tlbmenu.Top = 2
        tlbmenu.Height = MtxMenu(2).Height - 4
        tlbmenu.BackColor = Color.White
        tlbmenu.ToolbarPadding = 0
        tlbmenu.FlatStyle = AppSett.Personal.Theme.FlatStyle
        tlbmenu.draw(True)

        'Posiziono la picture con l'immagine del menu'
        picmenu.Top = 0
        picmenu.Left = pic0.Width + _winmenuleft
        If _winmenuminwidth <> -1 AndAlso tlbmenu.Width + _winmenupadding * 2 < _winmenuminwidth Then
            picmenu.Width = _winmenuminwidth
        Else
            picmenu.Width = tlbmenu.Width + _winmenupadding * 2
        End If
        tlbmenu.Left = picmenu.Left + (picmenu.Width \ 2) - tlbmenu.Width \ 2

        'Disegno il menu'
        Dim b1 As New Bitmap(picmenu.Width, pic1.Height)
        Dim gr As Graphics

        gr = Graphics.FromImage(b1)
        If Not pic1.BackgroundImage Is Nothing Then gr.DrawImage(pic1.BackgroundImage, 0, 0, pic1.Width + 300, pic1.Height)
        gr.DrawImage(MtxMenu(0), 0, 0)
        gr.DrawImage(MtxMenu(1), MtxMenu(0).Width, 0, picmenu.Width - MtxMenu(0).Width - MtxMenu(2).Width + 2, MtxMenu(1).Height)
        gr.DrawImage(MtxMenu(2), picmenu.Width - MtxMenu(2).Width, 0)
        picmenu.BackgroundImage = CType(b1.Clone, Image)
        Dim rect As New Rectangle(3, 2, tlbmenu.Width - 6, tlbmenu.Height)
        tlbmenu.Background = b1.Clone(rect, b1.PixelFormat)
        tlbmenu.draw(True)
        picmenu.Visible = True
        tlbmenu.Visible = True
        b1.Dispose()
        gr.Dispose()

    End Sub

    Sub DrawPathDirectory(ByVal gr As Graphics, ByVal format As StringFormat)
        Try

            Dim dxp As Integer = dx
            Dim dir() As String
            If applist.Tabs.Count > 0 Then
                dir = ("Root" & applist.CurrentSubTab.Directory).Split(CChar("\"))
            Else
                dir = ("Root\").Split(CChar("\"))
            End If
            rectpath.Clear()

            For i As Integer = 0 To dir.Length - 2
                Dim f As New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel)
                Dim s As SizeF = gr.MeasureString(dir(i), f)
                rectpath.Add(New Rectangle(dxp, 65, CInt(s.Width), CInt(s.Height)))
                gr = iControl.CommonFunction.DrawGlowText(gr, dir(i), f, Color.White, Color.WhiteSmoke, Color.Black, True, 1, 1, 90, Me.Width, 30, dxp, 65, format)
                gr = iControl.CommonFunction.DrawGlowText(gr, "\", f, Color.White, Color.WhiteSmoke, Color.Black, True, 1, 1, 90, Me.Width, 30, CInt(dxp + s.Width), 65, format)
                dxp = CInt(dxp + s.Width + 7)
            Next
        Catch ex As Exception

        End Try
    End Sub

    Sub DrawTab(ByVal gr As Graphics, ByVal rec As List(Of Rectangle), ByVal Delta As Integer, ByVal Cl As Color, ByVal PenWidth As Integer, ByVal TabSelectIndex As Integer, ByVal Bottom As Boolean)
        Try
            Dim pen As New Pen(Cl, PenWidth)
            Dim pts As New List(Of Point)
            If Bottom Then
                pts.Add(New Point(dx, rec(0).Top + Delta))
                pts.Add(New Point(rec(TabSelectIndex).Left - Delta, rec(TabSelectIndex).Top + Delta))
                pts.Add(New Point(rec(TabSelectIndex).Left - Delta, rec(TabSelectIndex).Bottom + Delta))
                pts.Add(New Point(rec(TabSelectIndex).Right + Delta, rec(TabSelectIndex).Bottom + Delta))
                pts.Add(New Point(rec(TabSelectIndex).Right + Delta, rec(TabSelectIndex).Top + Delta))
                pts.Add(New Point(Me.Width - dx, rec(TabSelectIndex).Top + Delta))
            Else
                pts.Add(New Point(dx, rec(0).Bottom - Delta))
                pts.Add(New Point(rec(TabSelectIndex).Left - Delta, rec(TabSelectIndex).Bottom - Delta))
                pts.Add(New Point(rec(TabSelectIndex).Left - Delta, rec(TabSelectIndex).Top - Delta))
                pts.Add(New Point(rec(TabSelectIndex).Right + Delta, rec(TabSelectIndex).Top - Delta))
                pts.Add(New Point(rec(TabSelectIndex).Right + Delta, rec(TabSelectIndex).Bottom - Delta))
                pts.Add(New Point(Me.Width - dx, rec(TabSelectIndex).Bottom - Delta))
            End If
            pen.Alignment = PenAlignment.Right
            If AppSett.Personal.Theme.FlatStyle Then
                gr.SmoothingMode = SmoothingMode.Default
            Else
                pen.LineJoin = LineJoin.Round
                gr.SmoothingMode = SmoothingMode.AntiAlias
            End If
            gr.DrawLines(pen, pts.ToArray)
        Catch ex As Exception

        End Try
    End Sub

    Sub DrawApp()

        Try

            Dim gr As Graphics
            Dim b1 As Bitmap

            b1 = New Bitmap(picback.Width, picback.Height)
            gr = Graphics.FromImage(b1)

            'Disegno la lista degli applicativi'
            Dim icon As New Bitmap(My.Resources.empty)

            Dim r As Integer = 0

            For i As Integer = applist.CurrentSubTab.ScrollIndex To applist.CurrentSubTab.ScrollIndex + apppage \ napprow - 1
                For k As Integer = 0 To napprow - 1
                    Dim aindex As Integer = i * napprow + k
                    If aindex < applist.TabAppList.Count Then
                        If r = 0 Then
                            icon = CType(applist.GetAppIcon(applist.TabAppList(aindex).Icon, GetIconSize), Bitmap)
                        Else
                            If applist.TabAppList(aindex).Icon <> applist.TabAppList(aindex - 1).Icon Then icon = CType(applist.GetAppIcon(applist.TabAppList(aindex).Icon, GetIconSize), Bitmap)
                        End If
                        DrawSingleApp(gr, r, aindex, icon, False)
                        r = r + 1
                    End If
                Next
            Next

            gr.DrawImage(DrawGlowText3("| " & CStr(applist.TabAppList.Count) & " Elementi", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, False, Nothing, 200, 30), txt3.Left + txt3.Width + 5, Me.Height - 85)

            picback.Image = CType(b1.Clone, Image)

            icon.Dispose()

            gr.Dispose()
            b1.Dispose()

        Catch ex As Exception

        End Try
    End Sub

    Function GetIconSize() As String
        Dim icos As String = "16x16"
        Select Case GetViewMode()
            Case 0, 2 : icos = "48x48"
            Case 1, 3 : icos = "32x32"
            Case 4
                Select Case GetFontSize()
                    Case Is < 8 : icos = "16x16"
                    Case Is > 10 : icos = "32x32"
                    Case Else : icos = "24x24"
                End Select
            Case 5
                'Select Case AppSett.MenuFontSize
                '    Case Is < 10 : icos = "16x16"
                '    Case Else : icos = "24x24"
                'End Select
                icos = "16x16"
        End Select
        Return icos
    End Function

    Function GetRowHeight() As Integer
        Dim gr As Graphics
        gr = Graphics.FromHwnd(txt3.Handle)
        Dim rowheight As Integer = GetRowHeight(gr)
        gr.Dispose()
        Return rowheight
    End Function

    Function GetRowHeight(ByVal gr As Graphics) As Integer
        Dim rowheight As Integer = 25
        Select Case GetViewMode()
            Case 0 : rowheight = CInt(gr.MeasureString("Dy", New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel)).Height) + 68
            Case 1 : rowheight = CInt(gr.MeasureString("Dy", New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel)).Height) * 2 + 39
            Case 2 : rowheight = 55
            Case 3 : rowheight = 37
            Case 4 : rowheight = CInt(gr.MeasureString("Dy", New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel)).Height) * 2 + 9
            Case 5 : rowheight = CInt(gr.MeasureString("Dy", New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel)).Height) + 7
        End Select
        Return rowheight
    End Function

    Function GetNumberAppRow() As Integer
        Dim gr As Graphics
        gr = Graphics.FromHwnd(txt3.Handle)
        Dim napp As Integer = GetNumberAppRow(gr)
        gr.Dispose()
        Return napp
    End Function

    Function GetNumberAppRow(ByVal gr As Graphics) As Integer
        Dim napp As Integer = 25
        Select Case GetViewMode()
            Case 0, 1 : napp = (Me.Width - dxapp * 2) \ CInt(gr.MeasureString("DDDDDDDDDD", New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel)).Width)
            Case 2, 3 : napp = (Me.Width - dxapp * 2) \ CInt(gr.MeasureString("DDDDDDDDDDDDD", New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel)).Width)
            Case Else : napp = 1
        End Select
        Return napp
    End Function

    Function GetViewMode() As Integer
        If applist.CurrentSubTab.View <> -1 Then
            Return applist.CurrentSubTab.View
        Else
            Return AppSett.MainWindows.WindowsView
        End If
    End Function

    Function GetFontSize() As Integer
        If applist.CurrentSubTab.FontSize <> -1 Then
            Return applist.CurrentSubTab.FontSize
        Else
            Return AppSett.MainWindows.MenuFontSize
        End If
    End Function

    Sub DrawSingleApp(ByVal gr As Graphics, ByVal r As Integer, ByVal i As Integer, ByVal Icon As Bitmap, ByVal Selection As Boolean)

        Try
            'Determino key e lo stato dell'applicazione'

            Dim ena As Boolean = applist.TabAppList(i).Enabled
            Dim key As String = GetAppImageKey(i, ena)

            Select Case GetViewMode()

                Case 0, 1

                    Dim dy As Integer = (r \ napprow) * rowheight1 + dyapp
                    Dim w As Integer = (picback.Width - dxapp * 2) \ napprow
                    Dim dx As Integer = (r Mod napprow) * w + dxapp

                    'Aggiungo l'area dell'appliczione'
                    rectapp.Add(New Rectangle(dx + 3, dy, w, rowheight1 - 4))
                    If applist.CurrentSubTab.ReadLocalFiles = True Then
                        rectdel.Add(New Rectangle(dx + w - 15, dy, 17, 17))
                    Else
                        If applist.TabAppList(i).Enabled AndAlso applist.EnableFavorites AndAlso applist.TabAppList(i).ID <> 0 Then
                            rectfav.Add(New Rectangle(dx + w - 15, dy, 17, 17))
                        End If
                    End If

                    'Determino l'eventuale rettangolo di selezione'
                    If Selection AndAlso ena Then gr.FillRectangle(New SolidBrush(Color.FromArgb(50, 0, 0, 0)), New Rectangle(dx + 3, dy - 5, w, rowheight1))

                    'Disegno l'applicazione'
                    If imgappstring.ContainsKey(key) OrElse LoadAppImageCache(i, ena) Then
                        'Disegno l'applicazione usando la copia in cache'
                        gr.DrawImage(imgappstring.Item(key), dx, dy)
                    Else
                        'Creo l'immagine dell'applicazione'
                        Dim img As Bitmap = GetAppImage(rowheight1, w, i, Icon, applist.CurrentSubTab.ReadLocalFiles)
                        If ena = False Then img = ConvDisable(img)
                        'Disegna l'applicazione usando quella appena creata'
                        gr.DrawImage(img, dx, dy)
                        'Aggiungo l'immagine nella cache'
                        imgappstring.Add(key, img)
                        'Salvo l'immagine di cache sul disco' 
                        imgappstring.Item(key).Save(dirimgtemp & key & imgtempext)
                    End If

                    If Selection AndAlso ena Then
                        If applist.CurrentTab.CurrentSubTab.ReadLocalFiles = False AndAlso applist.EnableFavorites Then
                            If applist.TabAppList(i).ID <> 0 Then
                                If applist.CurrentTab.Name = applist.TabFavoritesName Then
                                    gr.DrawImage(My.Resources.del_white16, dx + w - 15, dy + 1)
                                Else
                                    If applist.TabAppList(i).Favorite Then
                                        gr.DrawImage(My.Resources.favorites16, dx + w - 15, dy + 1)
                                    Else
                                        gr.DrawImage(My.Resources.not_favorites16, dx + w - 15, dy + 1)
                                    End If
                                End If
                            End If
                        Else
                            If applist.TabAppList(i).ID <> 0 Then
                                gr.DrawImage(My.Resources.del_white16, dx + w - 15, dy + 1)
                            End If
                        End If
                    End If

                Case 2, 3

                    Dim rowheight As Integer = GetRowHeight(gr)
                    Dim napp As Integer = GetNumberAppRow(gr)
                    Dim dy1 As Integer = (r \ napp) * rowheight + dyapp
                    Dim w As Integer = (picback.Width - dxapp * 2) \ napp
                    Dim dx1 As Integer = (r Mod napp) * w + dxapp

                    'Aggiungo l'area dell'appliczione'
                    rectapp.Add(New Rectangle(dx1, dy1, w, rowheight - 4))
                    If applist.CurrentSubTab.ReadLocalFiles = True Then
                        rectdel.Add(New Rectangle(dx1 + w - 15, dy1, 17, 17))
                    Else
                        If applist.TabAppList(i).Enabled AndAlso applist.EnableFavorites AndAlso applist.TabAppList(i).ID <> 0 Then
                            rectfav.Add(New Rectangle(dx1 + w - 15, dy1, 17, 17))
                        End If
                    End If

                    'Determino l'eventuale rettangolo di selezione'
                    If Selection Then gr.FillRectangle(New SolidBrush(Color.FromArgb(50, 0, 0, 0)), New Rectangle(dx1, dy1, w, rowheight))

                    'Disegno l'applicazione'
                    If imgappstring.ContainsKey(key) OrElse LoadAppImageCache(i, ena) Then
                        'Disegno l'applicazione usando la copia in cache'
                        gr.DrawImage(imgappstring.Item(key), dx1, dy1)
                    Else
                        'Creo l'immagine dell'applicazione'
                        Dim img As Bitmap = GetAppImage(rowheight, w, i, Icon, applist.CurrentSubTab.ReadLocalFiles)
                        If ena = False Then img = ConvDisable(img)
                        'Disegna l'applicazione usando quella appena creata'
                        gr.DrawImage(img, dx1, dy1)
                        'Aggiungo l'immagine nella cache'
                        imgappstring.Add(key, img)
                        'Salvo l'immagine di cache sul disco' 
                        imgappstring.Item(key).Save(dirimgtemp & key & imgtempext)
                    End If

                    If Selection AndAlso ena Then
                        If applist.CurrentTab.CurrentSubTab.ReadLocalFiles = False AndAlso applist.EnableFavorites Then
                            If applist.TabAppList(i).ID <> 0 Then
                                If applist.CurrentTab.Name = applist.TabFavoritesName Then
                                    gr.DrawImage(My.Resources.del_white16, dx1 + w - 15, dy1 + 1)
                                Else
                                    If applist.TabAppList(i).Favorite Then
                                        gr.DrawImage(My.Resources.favorites16, dx1 + w - 15, dy1 + 1)
                                    Else
                                        gr.DrawImage(My.Resources.not_favorites16, dx1 + w - 15, dy1 + 1)
                                    End If
                                End If
                            End If
                        Else
                            If applist.TabAppList(i).ID <> 0 Then
                                gr.DrawImage(My.Resources.del_white16, dx1 + w - 15, dy1 + 1)
                            End If
                        End If
                    End If

                Case 4

                    Dim rowheight As Integer = GetRowHeight(gr)
                    Dim dy As Integer = r * rowheight + dyapp

                    'Aggiungo l'area dell'appliczione'
                    rectapp.Add(New Rectangle(dx + 3, dy, Me.Width - dx * 2 - 6, rowheight - 4))
                    If applist.CurrentSubTab.ReadLocalFiles = True Then
                        rectdel.Add(New Rectangle(Me.Width - dx - 20, dy, 17, 17))
                        If applist.TabAppList(i).ID <> 0 Then
                            rectcopy.Add(New Rectangle(Me.Width - dx - 37, dy, 17, 17))
                        Else
                            rectcopy.Add(New Rectangle(Me.Width - dx - 37, dy, 17, 0))
                        End If
                    Else
                        If applist.TabAppList(i).Enabled AndAlso applist.EnableFavorites AndAlso applist.TabAppList(i).ID <> 0 Then
                            rectfav.Add(New Rectangle(Me.Width - dx - 20, dy, 17, 17))
                        End If
                    End If

                    'Determino l'eventuale rettangolo di selezione'
                    If Selection Then gr.FillRectangle(New SolidBrush(Color.FromArgb(50, 0, 0, 0)), New Rectangle(dx + 3, dy, Me.Width - dx * 2 - 6, rowheight - 4))

                    'Disegno l'applicazione'
                    If imgappstring.ContainsKey(key) OrElse LoadAppImageCache(i, ena) Then
                        'Disegno l'applicazione usando la copia in cache'
                        gr.DrawImage(imgappstring.Item(key), 0, dy)
                    Else
                        'Creo l'immagine dell'applicazione'
                        Dim img As Bitmap = GetAppImage(rowheight, 0, i, Icon, applist.CurrentSubTab.ReadLocalFiles)
                        If ena = False Then img = ConvDisable(img)
                        'Disegna l'applicazione usando quella appena creata'
                        gr.DrawImage(img, 0, dy)
                        'Aggiungo l'immagine nella cache'
                        imgappstring.Add(key, img)
                        'Salvo l'immagine di cache sul disco' 
                        imgappstring.Item(key).Save(dirimgtemp & key & imgtempext)
                    End If

                    If applist.CurrentTab.CurrentSubTab.ReadLocalFiles = False AndAlso applist.EnableFavorites Then
                        If applist.TabAppList(i).ID <> 0 Then
                            If applist.CurrentTab.Name = applist.TabFavoritesName Then
                                gr.DrawImage(My.Resources.del_white16, Me.Width - dx - 20, dy + 3)
                            Else
                                If applist.TabAppList(i).Favorite Then
                                    gr.DrawImage(My.Resources.favorites16, picback.Width - dx - 20, dy + 3)
                                Else
                                    gr.DrawImage(My.Resources.not_favorites16, picback.Width - dx - 20, dy + 3)
                                End If
                            End If
                        End If
                    End If

                    'Disegno le linee di separazione tra un'applicazione e un'altra'
                    'gr.DrawLine(New Pen(Color.FromArgb(70, 0, 0, 0), 1), dx + 3, dy + rowheight - 3, picback.Width - dx - 6, dy + rowheight - 3)
                    'If AppSett.Personal.Theme.FlatStyle = False Then gr.DrawLine(New Pen(Color.FromArgb(100, 255, 255, 255), 1), dx + 3, dy + rowheight - 2, picback.Width - dx - 6, dy + rowheight - 2)

                    r = r + 1

                Case 5

                    'Dim rowheight As Integer = CInt(AppSett.MenuFontSize * 1.5 + 8)
                    Dim rowheight As Integer = GetRowHeight(gr)
                    Dim dy As Integer = r * rowheight + dyapp

                    'Aggiungo l'area dell'appliczione'
                    rectapp.Add(New Rectangle(dx + 3, dy, Me.Width - dx * 2 - 6, rowheight - 4))
                    If applist.CurrentSubTab.ReadLocalFiles = True Then
                        rectdel.Add(New Rectangle(Me.Width - dx - 20, dy, 17, rowheight - 4))
                        If applist.TabAppList(i).ID <> 0 Then
                            rectcopy.Add(New Rectangle(Me.Width - dx - 37, dy, 17, 17))
                        Else
                            rectcopy.Add(New Rectangle(Me.Width - dx - 37, dy, 17, 0))
                        End If
                    Else
                        If applist.TabAppList(i).Enabled AndAlso applist.EnableFavorites AndAlso applist.TabAppList(i).ID <> 0 Then
                            rectfav.Add(New Rectangle(Me.Width - dx - 20, dy, 17, 17))
                        End If
                    End If

                    'Determino l'eventuale rettangolo di selezione'
                    If Selection Then gr.FillRectangle(New SolidBrush(Color.FromArgb(50, 0, 0, 0)), New Rectangle(dx + 3, dy, Me.Width - dx * 2 - 6, rowheight - 4))

                    'Disegno l'applicazione'
                    If imgappstring.ContainsKey(key) OrElse LoadAppImageCache(i, ena) Then
                        'Disegno l'applicazione usando la copia in cache'
                        gr.DrawImage(imgappstring.Item(key), 0, dy)
                    Else
                        'Creo l'immagine dell'applicazione'
                        Dim img As Bitmap = GetAppImage(rowheight, 0, i, Icon, applist.CurrentSubTab.ReadLocalFiles)
                        If ena = False Then img = ConvDisable(img)
                        'Disegna l'applicazione usando quella appena creata'
                        gr.DrawImage(img, 0, dy)
                        'Aggiungo l'immagine nella cache'
                        imgappstring.Add(key, img)
                        'Salvo l'immagine di cache sul disco' 
                        imgappstring.Item(key).Save(dirimgtemp & key & imgtempext)
                    End If

                    If ena Then
                        If applist.CurrentTab.CurrentSubTab.ReadLocalFiles = False AndAlso applist.EnableFavorites Then
                            If applist.TabAppList(i).ID <> 0 Then
                                If applist.CurrentTab.Name = applist.TabFavoritesName Then
                                    gr.DrawImage(My.Resources.del_white16, Me.Width - dx - 20, dy + 1)
                                Else
                                    If applist.TabAppList(i).Favorite Then
                                        gr.DrawImage(My.Resources.favorites16, picback.Width - dx - 20, dy + 1)
                                    Else
                                        gr.DrawImage(My.Resources.not_favorites16, picback.Width - dx - 20, dy + 1)
                                    End If
                                End If
                            End If
                        End If
                    End If

                    'Disegno le linee di separazione tra un'applicazione e un'altra'
                    gr.DrawLine(New Pen(Color.FromArgb(70, 0, 0, 0), 1), dx + 3, dy + rowheight - 3, picback.Width - dx - 6, dy + rowheight - 3)
                    If AppSett.Personal.Theme.FlatStyle = False Then gr.DrawLine(New Pen(Color.FromArgb(100, 255, 255, 255), 1), dx + 3, dy + rowheight - 2, picback.Width - dx - 6, dy + rowheight - 2)

                    r = r + 1

            End Select

        Catch ex As Exception

        End Try

    End Sub

    Function GetAppImageKey(ByVal Index As Integer, ByVal Enable As Boolean) As String

        Dim view As Integer = GetViewMode()
        Dim fs As Integer = GetFontSize()
        Dim key As String = applist.TabAppList(Index).Key & "-" & AppSett.MainWindows.WindowsSize & "-" & view & "-" & "-" & fs
        Dim g As Graphics = Me.CreateGraphics()

        key = key & "-" & g.DpiX & "x" & g.DpiY

        If Enable Then
            key = key & "E"
        Else
            key = key & "D"
        End If

        g.Dispose()

        Return key

    End Function

    Function GetAppImage(ByVal rowheight As Integer, ByVal width As Integer, ByVal i As Integer, ByVal icon As Bitmap, ByVal Local As Boolean) As Bitmap

        Dim gr As Graphics
        Dim b1 As Bitmap
        Dim dxico As Integer = icon.Width + 14

        b1 = New Bitmap(picback.Width, rowheight)
        gr = Graphics.FromImage(b1)

        Try

            Select Case GetViewMode()

                Case 0, 1

                    Dim name As String = applist.TabAppList(i).Name
                    Dim format As New StringFormat
                    format.Alignment = StringAlignment.Center
                    gr.DrawImage(icon, width \ 2 - icon.Width \ 2, 1)
                    gr.DrawImage(DrawGlowText3(name, New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, False, format, width, GetFontSize() + 24), 0, icon.Height)

                    If applist.TabAppList(i).ID = 3 Then
                        gr.DrawImage(My.Resources.link2, width \ 2 - icon.Width \ 2 - 15, 0)
                    End If

                    If applist.TabAppList(i).ID = 0 Then
                        gr.DrawImage(My.Resources.folder_app12, width \ 2 - icon.Width \ 2 - 15, 0)
                    End If

                Case 2, 3

                    Dim name As String = applist.TabAppList(i).Name
                    Dim format As New StringFormat
                    format.LineAlignment = StringAlignment.Center
                    gr.DrawImage(icon, 5, rowheight \ 2 - icon.Height \ 2)
                    gr.DrawImage(DrawGlowText3(name, New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, False, format, width - icon.Width, GetFontSize() + 14), icon.Width + 5, rowheight \ 2 - CLng(gr.MeasureString("DD", New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel)).Height) \ 2)

                    If applist.TabAppList(i).ID = 3 Then
                        gr.DrawImage(My.Resources.link2, 0, 0)
                    End If

                    If applist.TabAppList(i).ID = 0 Then
                        gr.DrawImage(My.Resources.folder_app12, 0, 0)
                    End If

                Case 4

                    Dim name As String = applist.TabAppList(i).Name
                    Dim desc As String = applist.TabAppList(i).Description
                    Dim txth As Integer = rowheight \ 2 - 3

                    gr.DrawImage(icon, dxapp, 1)
                    If applist.TabAppList(i).ID = 3 Then
                        gr.DrawImage(My.Resources.link2, dxapp - 15, 0)
                    End If
                    If applist.TabAppList(i).ID = 0 Then
                        gr.DrawImage(My.Resources.folder_app12, dxapp - 13, (dxico - 23) \ 2)
                    End If
                    gr.DrawImage(DrawGlowText3(name, New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, False, Nothing, Me.Width - dx * 2 - (dxico + 10), txth), dx + dxico, 1)
                    If desc <> "" Then
                        Dim img As Image
                        Dim fs As Integer = GetFontSize()
                        If fs > 8 Then fs = fs - 1
                        If Local Then
                            img = DrawGlowText3(desc, New Font("Arial", fs, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, Local, Nothing, Me.Width - dx * 2 - (dxico + 40), txth)
                        Else
                            img = DrawGlowText3(desc, New Font("Arial", fs, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, Local, Nothing, Me.Width - dx * 2 - (dxico + 10), txth)
                        End If
                        img = ConvDisable(img, 0.7)
                        gr.DrawImage(img, dx + dxico, CInt(GetFontSize() * 1.5 + 3))
                        img.Dispose()
                    End If

                    If applist.CurrentSubTab.ReadLocalFiles = True Then
                        If applist.TabAppList(i).ID <> 0 Then gr.DrawImage(My.Resources.copy_white16, Me.Width - dx - 37, 0)
                        gr.DrawImage(My.Resources.del_white16, Me.Width - dx - 20, 2)
                    End If

                Case 5

                    Dim name As String = applist.TabAppList(i).Name
                    Dim desc As String = applist.TabAppList(i).Description
                    Dim txth As Integer = rowheight - 3

                    gr.DrawImage(icon, dxapp, 0)
                    If applist.TabAppList(i).ID = 3 Then
                        gr.DrawImage(My.Resources.link2, dxapp - 15, 0)
                    End If
                    gr.DrawImage(DrawGlowText3(name, New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, False, Nothing, 100, txth), dx + 30, 1)
                    If desc <> "" Then
                        Dim img As Image
                        If Local Then
                            img = DrawGlowText3(desc, New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, Local, Nothing, Me.Width - dx * 2 - 180, txth)
                        Else
                            img = DrawGlowText3(desc, New Font("Arial", GetFontSize, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.WhiteSmoke, 30, Local, Nothing, Me.Width - dx * 2 - 150, txth)
                        End If
                        img = ConvDisable(img, 0.7)
                        gr.DrawImage(img, dx + 140, 1)
                        img.Dispose()
                    End If

                    If applist.CurrentSubTab.ReadLocalFiles = True Then
                        If applist.TabAppList(i).ID <> 0 Then gr.DrawImage(My.Resources.copy_white16, Me.Width - dx - 38, 0)
                        gr.DrawImage(My.Resources.del_white16, Me.Width - dx - 20, 2)
                    End If

            End Select

        Catch ex As Exception

        End Try

        gr.Dispose()

        Return CType(b1.Clone, Bitmap)

        b1.Dispose()

    End Function

    Public Function ConvDisable(ByVal img As Image) As Bitmap
        Return ConvDisable(img, -1)
    End Function

    Public Function ConvDisable(ByVal img As Image, ByVal Trasparency As Single) As Bitmap

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

    Sub DrawInfo()

        Try

            Dim gr As Graphics
            Dim b1 As Bitmap

            Dim delta As Integer = 0

            b1 = New Bitmap(picback.Width, picback.Height)
            gr = Graphics.FromImage(b1)

            'Impostazioni qualita' immagine'
            gr.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

            'Titolo programma'
            Dim y As Integer = 120
            Dim x As Integer = 130

            'Icona programma'
            gr.DrawImage(My.Resources._100, x - 103, y + 4)

            Select Case AppSett.MainWindows.WindowsSize
                Case 0
                    gr.DrawImage(DrawGlowText3(My.Application.Info.AssemblyName & " rel " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & " " & My.Application.Info.Description, New Font("Arial", 32, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 40, False, Nothing), x - 2, y)
                    'Versione'
                    gr.DrawImage(DrawGlowText3("build version : " & My.Application.Info.Version.ToString, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.Khaki, Color.Orange, 30, False, Nothing), x + 3, y + 36)
                    'Separatore'
                    gr.DrawLine(New Pen(Color.FromArgb(255, 255, 255, 255), 1), x + 5, y + 54, Me.Width - 40, y + 54)
                    gr.DrawLine(New Pen(Color.FromArgb(80, 0, 0, 0), 1), x + 5, y + 55, Me.Width - 40, y + 55)
                    gr.DrawLine(New Pen(Color.FromArgb(50, 0, 0, 0), 1), x + 5, y + 56, Me.Width - 40, y + 56)
                    'Informazioni programma'
                    gr.DrawImage(DrawGlowText3("Programma per la gestioni di uno o piu tornei di fantacalcio", New Font("Arial", 22, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 30, False, Nothing, 350), x, y + 59)
                    'Informazioni licenza'
                    gr.DrawImage(DrawGlowText3("Licence type : Shareware", New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 250), x + 3, y + 120)
                    gr.DrawImage(DrawGlowText3("Expired date : 2013/07/01", New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 250), x + 3, y + 134)
                    gr.DrawImage(DrawGlowText3("Expired : False", New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 250), x + 3, y + 148)
                    gr.DrawImage(DrawGlowText3(My.Application.Info.Copyright, New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 250), x + 3, y + 162)
                    'Altri collaboratori'
                    gr.DrawImage(DrawGlowText3("Si ringrazia per le info anche : Matteo Simone - Gianluca Iurisci", New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), Color.WhiteSmoke, Color.WhiteSmoke, 30, False, Nothing, 300), x + 3, y + 184)
                Case 1
                    gr.DrawImage(DrawGlowText3(My.Application.Info.AssemblyName & " rel " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & " " & My.Application.Info.Description, New Font("Arial", 38, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 40, False, Nothing), x - 2, y)
                    'Versione'
                    gr.DrawImage(DrawGlowText3("build version : " & My.Application.Info.Version.ToString, New Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel), Color.Khaki, Color.Orange, 30, False, Nothing), x + 3, y + 44)
                    'Separatore'
                    gr.DrawLine(New Pen(Color.FromArgb(255, 255, 255, 255), 1), x + 5, y + 68, Me.Width - 40, y + 68)
                    gr.DrawLine(New Pen(Color.FromArgb(80, 0, 0, 0), 1), x + 5, y + 69, Me.Width - 40, y + 69)
                    gr.DrawLine(New Pen(Color.FromArgb(50, 0, 0, 0), 1), x + 5, y + 70, Me.Width - 40, y + 70)
                    'Informazioni programma'
                    gr.DrawImage(DrawGlowText3("Programma per la gestioni di uno o piu tornei di fantacalcio", New Font("Arial", 26, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 30, False, Nothing, 380), x, y + 79)
                    'Informazioni licenza'
                    gr.DrawImage(DrawGlowText3("Licence type : Shareware", New Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 250), x + 3, y + 145)
                    gr.DrawImage(DrawGlowText3("Expired date : 2013/07/01", New Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 250), x + 3, y + 162)
                    gr.DrawImage(DrawGlowText3("Expired : False", New Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 250), x + 3, y + 179)
                    gr.DrawImage(DrawGlowText3(My.Application.Info.Copyright, New Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.White, 30, False, Nothing, 280), x + 3, y + 196)
                    'Altri collaboratori'
                    gr.DrawImage(DrawGlowText3("Si ringrazia per le info anche : Matteo Simone - Gianluca Iurisci", New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.WhiteSmoke, Color.WhiteSmoke, 30, False, Nothing, 350), x + 3, y + 228)
            End Select

            picback.Image = CType(b1.Clone, Image)
            b1.Dispose()
            gr.Dispose()

        Catch ex As Exception

        End Try

    End Sub

    Sub DrawSettings()

        Try

            Dim gr As Graphics
            Dim b1 As Bitmap

            Dim delta As Integer = 0

            b1 = New Bitmap(picback.Width, picback.Height)
            gr = Graphics.FromImage(b1)

            gr.DrawImage(picback.BackgroundImage, 0, 0)

            ''Impostazioni qualita' immagine'
            gr.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

            Dim dxicon As Integer = 25

            Dim cmdbc As Color = Color.FromArgb(200, 255, 255, 255)
            Dim cmdcl As Color = Color.FromArgb(230, 255, 255, 255)
            Dim cmdco As Color = Color.FromArgb(255, 255, 255, 255)
            Dim cmdbk As Color = Color.FromArgb(0, 255, 255, 255)
            Dim flbc As Color = Color.FromArgb(200, 255, 255, 255)
            Dim flbk As Color = Color.FromArgb(220, 255, 255, 255)

            cmdconne.Top = 148
            cmdconne.Left = Me.Width - dxfield - cmdconne.Width + 1
            cmdconne.FlatStyle = AppSett.Personal.Theme.FlatStyle

            cmdupd.Top = 197
            cmdupd.Left = Me.Width - dxfield * 2 - cmdupd.Width - 1
            cmdupd.FlatStyle = AppSett.Personal.Theme.FlatStyle

            cmdrest.Top = 197
            cmdrest.Left = Me.Width - dxfield - cmdrest.Width + 1
            cmdrest.FlatStyle = AppSett.Personal.Theme.FlatStyle

            cmdconne.BorderColorEnalbed = flbc
            cmdconne.ColorLeave = cmdcl
            cmdconne.ColorOver = cmdco
            cmdconne.BackColor = cmdbk
            cmdconne.Background = iControl.CommonFunction.GetAreaImage(picback.BackgroundImage, cmdconne.Left, cmdconne.Top, cmdconne.Width, cmdconne.Height)

            cmdupd.BorderColorEnalbed = flbc
            cmdupd.ColorLeave = cmdcl
            cmdupd.ColorOver = cmdco
            cmdupd.BackColor = cmdbk
            cmdupd.Background = iControl.CommonFunction.GetAreaImage(picback.BackgroundImage, cmdupd.Left, cmdupd.Top, cmdupd.Width, cmdupd.Height)

            cmdrest.BorderColorEnalbed = flbc
            cmdrest.ColorLeave = cmdcl
            cmdrest.ColorOver = cmdco
            cmdrest.BackColor = cmdbk
            cmdrest.Background = iControl.CommonFunction.GetAreaImage(picback.BackgroundImage, cmdrest.Left, cmdrest.Top, cmdrest.Width, cmdrest.Height)

            'Icona server'
            gr.DrawImage(My.Resources.PC_Case, dxicon, 110)

            'Campo indirizzo server'
            gr.DrawImage(DrawGlowText3("Server :", New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 40, False, Nothing), dxfield, 110)
            gr.DrawRectangle(New Pen(flbc, 1), New Rectangle(dxfield, 127, Me.Width - 130, 18))
            gr.FillRectangle(New SolidBrush(flbk), New Rectangle(dxfield + 1, 128, Me.Width - 132, 16))

            'Campo indirizzo server degli update'
            gr.DrawImage(DrawGlowText3("Server aggiornamenti", New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 40, False, Nothing), dxfield, 159)
            gr.DrawRectangle(New Pen(flbc, 1), New Rectangle(dxfield, 176, Me.Width - 130, 18))
            gr.FillRectangle(New SolidBrush(flbk), New Rectangle(dxfield + 1, 177, Me.Width - 132, 16))

            'Icona server'
            gr.DrawImage(My.Resources.colorize, dxicon, 205)

            'Campo thema'
            gr.DrawImage(DrawGlowText3("Temi", New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 40, False, Nothing), dxfield, 210)
            gr.DrawRectangle(New Pen(flbc, 1), New Rectangle(dxfield, 227, Me.Width - 130, numthemerow * 20 + 14))
            gr.FillRectangle(New SolidBrush(flbk), New Rectangle(dxfield + 1, 228, Me.Width - 132, numthemerow * 20 + 12))

            'Checkbox flatStyle'
            gr.DrawImage(DrawGlowText3("Stile 2D", New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel), Color.White, Color.Gainsboro, 40, False, Nothing), dxfield + 15, 210 + numthemerow * 20 + 38)

            gr.DrawRectangle(New Pen(flbc, 1), New Rectangle(dxfield, 210 + numthemerow * 20 + 38, 12, 12))
            gr.FillRectangle(New SolidBrush(flbk), New Rectangle(dxfield + 1, 210 + numthemerow * 20 + 39, 10, 10))

            If AppSett.Personal.Theme.FlatStyle Then
                gr.FillRectangle(New SolidBrush(Color.FromArgb(255, 80, 255, 0)), New Rectangle(dxfield + 2, 210 + numthemerow * 20 + 40, 8, 8))
                gr.DrawRectangle(New Pen(Color.FromArgb(120, 0, 0, 0), 1), New Rectangle(dxfield + 1, 210 + numthemerow * 20 + 39, 10, 10))
            End If

            rectflat = New Rectangle(dxfield, 210 + numthemerow * 20 + 39, 70, 20)

            'Campilo i campi server'
            gr.SmoothingMode = SmoothingMode.AntiAlias
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
            gr.DrawString(GenSett.Server, New Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(220, 0, 0, 0)), 70, 130)
            gr.DrawString(GenSett.Update, New Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(220, 0, 0, 0)), 70, 179)

            picback.BackgroundImage = CType(b1.Clone, Image)

            b1.Dispose()
            gr.Dispose()

            'vsrc2.BackColor = Color.Transparent
            'If t.Count - 5 > 0 Then
            '    vsrc2.Max = AppList.TabAppList.Count - apppage
            '    vsrc2.Value = AppList.CurrentSubTab.ScrollIndex
            '    vsrc2.Draw(True)
            '    'vsrc2.Visible = True
            'Else
            '    vsrc2.Visible = False
            'End If

            Call DrawTheme()

        Catch ex As Exception

        End Try

    End Sub

    Sub DrawTheme()

        Try

            Dim gr As Graphics
            Dim b1 As Bitmap

            b1 = New Bitmap(picback.Width, picback.Height)
            gr = Graphics.FromImage(b1)

            gr.DrawImage(picback.BackgroundImage, 0, 0)

            recttheme.Clear()

            Dim c As Integer = 0
            Dim r As Integer = 0
            Dim w As Single = (picback.Width - dxfield * 2 - 10) \ numthemecol
            For i As Integer = _scrollthemeindex * numthemecol To t.Count - 1
                Dim ind As Integer = i
                If ind < t.Count Then
                    Dim rec As New Rectangle(dxfield + 6 + CInt(c * w), 235 + r * 20, CInt(w), 18)
                    If t(ind).ToLower = AppSett.Personal.Theme.Name.ToLower Then
                        If AppSett.Personal.Theme.FlatStyle Then
                            gr.FillRectangle(New SolidBrush(Color.FromArgb(255, 255, 255, 255)), rec)
                        Else
                            gr.FillRectangle(New LinearGradientBrush(rec, Color.FromArgb(220, 255, 255, 255), Color.FromArgb(255, 235, 235, 235), LinearGradientMode.Vertical), rec)
                        End If
                        gr.DrawRectangle(New Pen(Color.FromArgb(60, 0, 0, 0), 1), rec)
                    End If
                    gr.DrawString(t(ind), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(200, 0, 0, 0)), rec.Left + 6, rec.Top + 2)
                    recttheme.Add(rec)
                    c = c + 1
                    If c > numthemecol - 1 Then c = 0 : r = r + 1
                    If r > numthemerow - 1 Then Exit For
                End If
            Next
            picback.Image = CType(b1.Clone, Image)

            b1.Dispose()
            gr.Dispose()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub DrawOverApplicationRectangle(ByVal Ind As Integer)

        If Ind <> _oldind Then

            Dim dy As Integer = rectapp(Ind).Top
            Dim index As Integer = Ind + applist.CurrentSubTab.ScrollIndex * napprow

            picback.Refresh()

            Dim icon As Bitmap = My.Resources.del12
            Dim gr As Drawing.Graphics = picback.CreateGraphics()

            icon = CType(applist.GetAppIcon(applist.TabAppList(index).Icon, GetIconSize), Bitmap)

            Call DrawSingleApp(gr, Ind, index, icon, True)

            gr.Dispose()

        End If

        _oldind = Ind

    End Sub

    Private Sub DrawOverThemeRectangle(ByVal Ind As Integer)

        If Ind <> _oldind Then

            Dim index As Integer = Ind + _scrollthemeindex * numthemecol
            Dim gr As Drawing.Graphics = picback.CreateGraphics()

            picback.Refresh()

            If AppSett.Personal.Theme.FlatStyle Then
                gr.FillRectangle(New SolidBrush(Color.FromArgb(255, 255, 255, 255)), recttheme(Ind))
            Else
                gr.FillRectangle(New LinearGradientBrush(recttheme(Ind), Color.FromArgb(220, 255, 255, 255), Color.FromArgb(255, 235, 235, 235), LinearGradientMode.Vertical), recttheme(Ind))
            End If
            gr.DrawRectangle(New Pen(Color.FromArgb(60, 0, 0, 0), 1), recttheme(Ind))
            gr.DrawString(t(index), New Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Pixel), New SolidBrush(Color.FromArgb(200, 0, 0, 0)), recttheme(Ind).Left + 6, recttheme(Ind).Top + 2)

            gr.Dispose()

        End If

        _oldind = Ind

    End Sub

    Private Sub DrawBackground(ByVal gr As Graphics)

        Try

            'Disegno l'iimagine di sfondo'
            Dim img1 As Bitmap = Nothing
            Dim subdir As String = "Normal"
            Dim drawclback As Boolean = True
            Dim drawwave As Boolean = True
            Dim drawlogo As Boolean = True
            If AppSett.Personal.Theme.FlatStyle Then subdir = "Flat"
            Dim dirtheme As String = My.Application.Info.DirectoryPath & "\theme\" & AppSett.Personal.Theme.Name
            Dim fname1 As String = dirtheme & "\" & subdir & "\back.png"
            Dim fname2 As String = dirtheme & "\" & subdir & "\back.jpg"
            Dim fname3 As String = dirtheme & "\back.png"
            Dim fname4 As String = dirtheme & "\back.jpg"

            If IO.Directory.Exists(dirtheme) Then
                If IO.File.Exists(fname1) Then
                    img1 = New Bitmap(fname1)
                    drawwave = False
                    drawlogo = False
                ElseIf IO.File.Exists(fname2) Then
                    img1 = New Bitmap(fname2)
                    drawclback = False
                    drawwave = False
                    drawlogo = False
                ElseIf IO.File.Exists(fname3) Then
                    img1 = New Bitmap(fname3)
                    drawlogo = False
                ElseIf IO.File.Exists(fname4) Then
                    img1 = New Bitmap(fname4)
                    drawclback = False
                    drawlogo = False
                End If
            Else
                img1 = My.Resources.back
            End If

            If drawclback Then
                'Colore di sfondo'
                If AppSett.Personal.Theme.FlatStyle Then
                    gr.FillRectangle(New SolidBrush(AppSett.Personal.Theme.Item(20, 0)), 0, 0, picback.Width, picback.Height)
                Else
                    Dim br As New Drawing2D.LinearGradientBrush(New Rectangle(0, 2, picback.Width, picback.Height), Drawing.Color.AliceBlue, Drawing.Color.AntiqueWhite, Drawing2D.LinearGradientMode.Vertical)
                    Dim bl As New Drawing2D.ColorBlend
                    bl.Colors = New Color() {AppSett.Personal.Theme.Item(1, 0), AppSett.Personal.Theme.Item(1, 1), AppSett.Personal.Theme.Item(14, 0), AppSett.Personal.Theme.Item(14, 1), AppSett.Personal.Theme.Item(20, 0), AppSett.Personal.Theme.Item(20, 1), AppSett.Personal.Theme.Item(9, 0), AppSett.Personal.Theme.Item(9, 1)}
                    bl.Positions = New Single() {0, 0.1, 0.1, 0.3, 0.3, 0.9, 0.9, 1}
                    br.InterpolationColors = bl
                    gr.FillRectangle(br, New Rectangle(0, 0, picback.Width, picback.Height))
                End If
            End If

            If img1 IsNot Nothing Then
                Select Case AppSett.Personal.Theme.BackgroundPosition
                    Case AppSettings.ThemeSettings.eBackPosition.TopLeft
                        gr.DrawImage(img1, 0, 0)
                    Case AppSettings.ThemeSettings.eBackPosition.TopCenter
                        gr.DrawImage(img1, picback.Width \ 2 - img1.Width \ 2, 0)
                    Case AppSettings.ThemeSettings.eBackPosition.TopRight
                        gr.DrawImage(img1, picback.Width - img1.Width, 0)
                    Case AppSettings.ThemeSettings.eBackPosition.MiddleLeft
                        gr.DrawImage(img1, 0, picback.Height \ 2 - img1.Height \ 2)
                    Case AppSettings.ThemeSettings.eBackPosition.MiddleCenter
                        gr.DrawImage(img1, picback.Width \ 2 - img1.Width \ 2, picback.Height \ 2 - img1.Height \ 2)
                    Case AppSettings.ThemeSettings.eBackPosition.MiddleRight
                        gr.DrawImage(img1, picback.Width - img1.Width, picback.Height \ 2 - img1.Height \ 2)
                    Case AppSettings.ThemeSettings.eBackPosition.BottomLeft
                        gr.DrawImage(img1, 0, picback.Height - img1.Height)
                    Case AppSettings.ThemeSettings.eBackPosition.BottomCenter
                        gr.DrawImage(img1, picback.Width \ 2 - img1.Width \ 2, picback.Height - img1.Height)
                    Case AppSettings.ThemeSettings.eBackPosition.BottomRight
                        gr.DrawImage(img1, picback.Width - img1.Width, picback.Height - img1.Height)
                    Case AppSettings.ThemeSettings.eBackPosition.Stretch
                        Dim fw As Single = CSng(picback.Width / img1.Width)
                        Dim fh As Single = CSng(picback.Height / img1.Height)
                        Dim fp As Single = picback.Width
                        If fw > fh Then
                            Dim sr As New Rectangle(0, 0, img1.Width, CInt(img1.Width * (picback.Height / picback.Width)))
                            Dim dr As Rectangle = picback.ClientRectangle
                            gr.DrawImage(img1, dr, sr, GraphicsUnit.Pixel)
                        Else
                            Dim sr As New Rectangle(0, 0, CInt(img1.Height * (picback.Width / picback.Height)), img1.Height)
                            Dim dr As Rectangle = picback.ClientRectangle
                            gr.DrawImage(img1, dr, sr, GraphicsUnit.Pixel)
                        End If

                    Case Else
                        gr.DrawImage(img1, picback.Width - img1.Width, picback.Height - img1.Height)
                End Select

            End If

            If AppSett.Personal.Theme.FlatStyle = False AndAlso drawwave Then
                img1 = My.Resources.t_normal
                gr.DrawImage(img1, picback.Width \ 2 - img1.Width \ 2, 0)
            End If

            If AppSett.Personal.Theme.FlatStyle = False AndAlso drawwave Then
                img1 = My.Resources.b_normal
                gr.DrawImage(img1, picback.Width \ 2 - img1.Width \ 2, picback.Height - img1.Height)
            End If

            If img1 IsNot Nothing Then img1.Dispose()

        Catch ex As Exception

        End Try

    End Sub

    Public Sub SetControlBox()

        Try

            cnt1.Image = ctlimg(0)
            cnt1.Width = ctlimg(0).Width
            cnt1.Height = ctlimg(0).Height

            cnt2.Image = ctlimg(4)
            cnt2.Width = ctlimg(4).Width
            cnt2.Height = ctlimg(4).Height

            cnt3.Image = ctlimg(5)
            cnt3.Width = ctlimg(5).Width
            cnt3.Height = ctlimg(5).Height

            cnt1.Top = 0
            cnt2.Top = 0
            cnt3.Top = 0

            If pic2.BackgroundImage IsNot Nothing Then cnt3.Left = Me.Width - cnt3.Width - (pic2.BackgroundImage.Width + 3)

            cnt2.Left = cnt3.Left - cnt2.Width
            cnt1.Left = cnt2.Left - cnt1.Width

            If pic1.BackgroundImage IsNot Nothing Then
                cnt1.BackgroundImage = iControl.CommonFunction.GetAreaImage(pic1.BackgroundImage, cnt1.Left, cnt1.Top, cnt1.Width, cnt1.Height, pic1.Left, 0)
                cnt2.BackgroundImage = iControl.CommonFunction.GetAreaImage(pic1.BackgroundImage, cnt2.Left, cnt2.Top, cnt2.Width, cnt2.Height, pic1.Left, 0)
                cnt3.BackgroundImage = iControl.CommonFunction.GetAreaImage(pic1.BackgroundImage, cnt3.Left, cnt3.Top, cnt3.Width, cnt3.Height, pic1.Left, 0)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Function DrawGlowText3(ByVal Text As String, ByVal TextFont As Font, ByVal FontColor1 As Color, ByVal FontColor2 As Color, ByVal Trasparency As Integer, ByVal Local As Boolean, ByVal format As StringFormat, Optional ByVal Width As Integer = -1, Optional ByVal Height As Integer = -1) As Bitmap
        Return DrawGlowText3(Text, TextFont, FontColor1, FontColor2, Color.Black, Trasparency, True, Local, format, Width, Height)
    End Function

    Private Function DrawGlowText3(ByVal Text As String, ByVal TextFont As Font, ByVal FontColor1 As Color, ByVal FontColor2 As Color, ByVal GlowColor As Color, ByVal Trasparency As Integer, ByVal ShowShadows As Boolean, ByVal Local As Boolean, ByVal format As StringFormat, Optional ByVal Width As Integer = -1, Optional ByVal Height As Integer = -1) As Bitmap

        Dim bm As New Bitmap(50, 50)
        Dim s As New Size(10, 10)

        Try
            If Width = -1 Then Width = picback.Width
            If Height = -1 Then Height = picback.Height

            Dim gr As Graphics = Graphics.FromImage(bm)
            Dim br As Brush = New SolidBrush(Color.FromArgb(Trasparency, GlowColor.R, GlowColor.G, GlowColor.B))

            Dim good As Boolean = False
            Do Until good = True
                s = gr.MeasureString(Text, TextFont, New SizeF(Width - 5, Me.Height)).ToSize
                If s.Width > Width OrElse s.Height > Height Then
                    If Local Then
                        If Text.Contains("...") Then
                            Text = "..." & Text.Substring(4, Text.Length - 4)
                        Else
                            Text = "..." & Text.Substring(1)
                        End If
                    Else
                        Text = Text.Substring(0, Text.Length - 4) & "..."
                    End If
                Else
                    good = True
                End If
            Loop

            bm = New Bitmap(Width, s.Height)
            gr = Graphics.FromImage(bm)

            gr.SmoothingMode = SmoothingMode.AntiAlias
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

            Dim e As Integer = 2
            If ShowShadows = False Then e = 1

            For x As Integer = -1 To e
                For y As Integer = -1 To e
                    If format Is Nothing Then
                        gr.DrawString(Text, TextFont, br, New RectangleF(x, y, Width, s.Height))
                    Else
                        gr.DrawString(Text, TextFont, br, New RectangleF(x, y, Width, s.Height), format)
                    End If

                Next
            Next

            gr.SmoothingMode = SmoothingMode.AntiAlias
            gr.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

            Dim br1 As New LinearGradientBrush(New Rectangle(0, 0, pic1.Width, s.Height), FontColor1, FontColor2, LinearGradientMode.Vertical)
            If format Is Nothing Then
                gr.DrawString(Text, TextFont, br1, New RectangleF(0, 0, Width, s.Height))
            Else
                gr.DrawString(Text, TextFont, br1, New RectangleF(0, 0, Width, s.Height), format)
            End If
            'gr.DrawString(Text, TextFont, br1, 0, 0)
            gr.Dispose()
            br.Dispose()
            br1.Dispose()

        Catch ex As Exception

        End Try

        Return bm

    End Function

    Function OverArea(ByVal pt As Point, ByVal RectList As List(Of Rectangle)) As Integer
        Dim ind As Integer = -1
        For i As Integer = 0 To RectList.Count - 1
            If RectList(i).Contains(pt) Then
                ind = i
                Exit For
            End If
        Next
        Return ind
    End Function

    Sub UpdateTheme()

        Dim frm As Form
        Dim wind As iControl.iForm
        Dim theme As String = "red"

        iControl.Setting.Theme(AppSett.Personal.Theme.Name)
        iControl.Setting.FlatStyle(AppSett.Personal.Theme.FlatStyle)

        Call SetTheme(False)
        Call SystemFunction.General.LoadImageCache()

        Try
            'Aggiorno il tema per tutte le finestre aperte'
            For i As Integer = 0 To Application.OpenForms.Count - 1
                frm = Application.OpenForms(i)
                If frm.Name <> Me.Name Then
                    Try
                        wind = CType(frm.Controls.Item("iform1"), iControl.iForm)
                        If wind IsNot Nothing Then
                            wind.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)
                        End If
                    Catch ex As Exception
                    End Try
                End If
            Next
        Catch ex As Exception

        End Try

    End Sub

    Sub SetToolBarState()
        'Windows size'
        For i As Integer = 0 To tlb1.Button(1).SubItems.Count - 1
            tlb1.Button(1).SubItems(i).State = False
        Next
        tlb1.Button(1).SubItems(AppSett.MainWindows.WindowsSize).State = True
        'Type view'
        For i As Integer = 0 To tlb1.Button(2).SubItems.Count - 1
            tlb1.Button(2).SubItems(i).State = False
        Next
        tlb1.Button(2).SubItems(AppSett.MainWindows.WindowsView).State = True
        'Font size'
        For i As Integer = 0 To tlb1.Button(3).SubItems.Count - 1
            If CInt(tlb1.Button(3).SubItems(i).Text.Replace("pt", "")) = GetFontSize() Then
                tlb1.Button(3).SubItems(i).State = True
            Else
                tlb1.Button(3).SubItems(i).State = False
            End If
        Next
    End Sub

#End Region

#Region "EventOnControl"

    Private Sub picback_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picback.MouseDown

        Try
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Dim ind As Integer = -1
                ind = OverArea(e.Location, rectapp)
                If ind <> -1 Then
                    Dim appid As Integer = ind + applist.CurrentSubTab.ScrollIndex * napprow
                    mnu1.Tag = New AppSettings.HotKeyItem(applist.TabAppList(appid).ID, applist.TabAppList(appid).Name)
                    mnu1.Show(New Point(e.Location.X + Me.Location.X, e.Location.Y + Me.Location.Y))
                    'frmapphotkeys.StartPosition = FormStartPosition.Manual
                    'frmapphotkeys.Location = New Point(e.Location.X + Me.Location.X, e.Location.Y + Me.Location.Y)
                    'frmapphotkeys.Show(Me)
                End If

            Else
                Dim ind As Integer = -1
                'Controllo se il click e' stato fatto sopra una scheda' 
                ind = OverArea(e.Location, recttab)
                If ind <> -1 Then
                    applist.TabIndex = ind
                    Call LoadPage(True)
                Else
                    'Controllo se il click e' stato fatto sopra una sotto scheda' 
                    ind = OverArea(e.Location, rectsubtab)
                    If ind <> -1 Then
                        applist.CurrentTab.SubTabIndex = ind
                        Call LoadPage(True)
                    Else
                        'Controllo se il click e' stato fatto sopra una directory del path' 
                        ind = OverArea(e.Location, rectpath)
                        If ind <> -1 Then
                            Dim dir() As String = (applist.CurrentSubTab.Directory).Split(CChar("\"))
                            Dim pt As String = "\"
                            If dir(ind) <> "" Then
                                For i As Integer = 0 To ind
                                    If dir(i) <> "" Then pt = pt & dir(i) & "\"
                                Next
                            End If
                            'dir(ind) = dir(ind) & "\"
                            applist.CurrentSubTab.ScrollIndex = 0
                            applist.CurrentSubTab.Directory = pt
                            Call LoadPage(False)
                        Else
                            If applist.CurrentSubTab.System Then
                                If applist.CurrentTab.Name.ToLower = "opzioni" Then
                                    If rectflat.Contains(e.Location) Then
                                        'Salvo le impostazioni del tema'
                                        AppSett.ReadSettings()
                                        AppSett.Personal.Theme.FlatStyle = Not (AppSett.Personal.Theme.FlatStyle)
                                        AppSett.SaveSettings()
                                        'Aggiorno il tema per tutte le finestre aperte'
                                        Call UpdateTheme()
                                    Else
                                        ind = OverArea(e.Location, recttheme)
                                        If ind <> -1 Then
                                            AppSett.ReadSettings()
                                            AppSett.Personal.Theme.Name = t(ind + _scrollthemeindex * numthemecol)
                                            AppSett.SaveSettings()
                                            'Aggiorno il tema per tutte le finestre aperte'
                                            Call UpdateTheme()
                                        End If
                                    End If
                                Else
                                    picback.Cursor = Cursors.Default
                                End If
                            Else
                                If applist.CurrentSubTab.ReadLocalFiles Then
                                    ind = OverArea(e.Location, rectdel)
                                    If ind <> -1 Then
                                        ind = ind + applist.CurrentSubTab.ScrollIndex
                                        Select Case applist.TabAppList(ind).ID
                                            Case 0
                                                If iControl.iMsgBox.ShowMessage("Are you sure you want to delete folder?", "", iControl.iMsgBox.MsgStyle.YesCancel, iControl.iMsgBox.Icona.MsgDelete) = Windows.Forms.DialogResult.OK Then
                                                    Dim fname As String = My.Application.Info.DirectoryPath & "\" & applist.CurrentSubTab.DefaultFolder & applist.TabAppList(ind).Directory & applist.TabAppList(ind).Name
                                                    Try
                                                        IO.Directory.Delete(fname, True)
                                                    Catch ex As Exception

                                                    End Try
                                                    Call LoadPage(True)
                                                End If
                                            Case Else
                                                If iControl.iMsgBox.ShowMessage("Are you sure you want to delete file?", "", iControl.iMsgBox.MsgStyle.YesCancel, iControl.iMsgBox.Icona.MsgDelete) = Windows.Forms.DialogResult.OK Then
                                                    Dim fname As String = My.Application.Info.DirectoryPath & "\" & applist.TabAppList(ind).Path
                                                    Try
                                                        IO.File.Delete(fname)
                                                    Catch ex As Exception

                                                    End Try
                                                    Call LoadPage(True)
                                                End If
                                        End Select
                                    Else
                                        ind = OverArea(e.Location, rectcopy)
                                        If ind <> -1 Then
                                            ind = ind + applist.CurrentSubTab.ScrollIndex
                                            Try
                                                Dim DataObject As New DataObject
                                                Dim file(0) As String
                                                file(0) = My.Application.Info.DirectoryPath & "\" & applist.TabAppList(ind).Path
                                                DataObject.SetData(DataFormats.FileDrop, file)
                                                Clipboard.Clear()
                                                Clipboard.SetDataObject(DataObject, True)
                                            Catch ex As Exception

                                            End Try
                                        End If
                                    End If
                                End If
                                If ind = -1 Then
                                    ind = OverArea(e.Location, rectfav)
                                    If ind <> -1 Then
                                        ind = ind + applist.CurrentSubTab.ScrollIndex
                                        If applist.CurrentTab.Name = applist.TabFavoritesName Then
                                            AppSett.MainWindows.Favorites.Remove(applist.TabAppList(ind).Name)
                                            AppSett.SaveSettings()
                                            Call LoadPage(True)
                                        Else
                                            If applist.TabAppList(ind).Favorite AndAlso AppSett.MainWindows.Favorites.Contains(applist.TabAppList(ind).Name) Then
                                                AppSett.MainWindows.Favorites.Remove(applist.TabAppList(ind).Name)
                                                applist.TabAppList(ind).Favorite = False
                                            Else
                                                AppSett.MainWindows.Favorites.Add(applist.TabAppList(ind).Name)
                                                applist.TabAppList(ind).Favorite = True
                                            End If
                                            AppSett.SaveSettings()
                                            Call DrawApp()
                                        End If
                                    Else
                                        ind = OverArea(e.Location, rectapp)
                                        If ind <> -1 Then
                                            ind = ind + applist.CurrentSubTab.ScrollIndex
                                            Call RunApplication(applist.TabAppList(ind))
                                        Else
                                            If applist.CurrentSubTab.System Then
                                                ctheme = ctheme + 1
                                                If ctheme > 21 Then ctheme = 0
                                                Call SetTheme(True)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub picback_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picback.MouseMove

        Try
            If applist.CurrentSubTab.System Then
                Dim ind As Integer = OverArea(e.Location, recttheme)
                If applist.CurrentTab.Name.ToLower = "opzioni" AndAlso (rectflat.Contains(e.Location) OrElse ind <> -1) Then
                    picback.Cursor = Cursors.Hand
                    If ind <> -1 Then
                        Call DrawOverThemeRectangle(ind)
                    Else
                        picback.Cursor = Cursors.Default
                        picback.Refresh()
                    End If
                Else
                    If _oldind <> -1 Then picback.Refresh()
                    _oldind = ind
                    picback.Cursor = Cursors.Default
                End If
            Else
                If OverArea(e.Location, recttab) <> -1 OrElse OverArea(e.Location, rectsubtab) <> -1 OrElse OverArea(e.Location, rectpath) <> -1 Then
                    picback.Cursor = Cursors.Hand
                    _oldind = -1
                Else
                    Dim ind As Integer = OverArea(e.Location, rectapp)
                    If ind <> -1 AndAlso applist.TabAppList(ind + applist.CurrentSubTab.ScrollIndex * napprow).Enabled Then
                        Call DrawOverApplicationRectangle(ind)
                        If OverArea(e.Location, rectdel) <> -1 OrElse OverArea(e.Location, rectcopy) <> -1 OrElse OverArea(e.Location, rectfav) <> -1 Then
                            picback.Cursor = Cursors.Hand
                        Else
                            picback.Cursor = Cursors.Default
                        End If
                    Else
                        picback.Cursor = Cursors.Default
                        picback.Refresh()
                        _oldind = -1
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub vsrc1_ScrollChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles vsrc1.ScrollChange
        Timer2.Stop()
        Timer2.Enabled = True
    End Sub

    Private Sub vsrc2_ScrollChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles vsrc2.ScrollChange
        _scrollthemeindex = vsrc2.Value
        Call DrawTheme()
    End Sub

    Private Sub Form1_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
        If applist.CurrentSubTab.System Then
            If applist.CurrentTab.Name.ToLower = "opzioni" Then
                If vsrc2.Visible Then
                    If e.Delta < 0 Then
                        If vsrc2.Value < vsrc2.Max Then
                            vsrc2.Value = vsrc2.Value + 1
                            _scrollthemeindex = vsrc2.Value
                            Call DrawTheme()
                        Else
                            vsrc2.Value = vsrc2.Max
                        End If
                    Else
                        If vsrc2.Value > 0 Then
                            vsrc2.Value = vsrc2.Value - 1
                            _scrollthemeindex = vsrc2.Value
                            Call DrawTheme()
                        Else
                            vsrc2.Value = 0
                        End If
                    End If
                End If
            End If
        Else
            If vsrc1.Visible Then
                If e.Delta < 0 Then
                    If vsrc1.Value < vsrc1.Max Then
                        vsrc1.Value = vsrc1.Value + 1
                        applist.CurrentSubTab.ScrollIndex = vsrc1.Value
                        Call DrawApp()
                    Else
                        vsrc1.Value = vsrc1.Max
                    End If
                Else
                    If vsrc1.Value > 0 Then
                        vsrc1.Value = vsrc1.Value - 1
                        applist.CurrentSubTab.ScrollIndex = vsrc1.Value
                        Call DrawApp()
                    Else
                        vsrc1.Value = 0
                    End If
                End If
            End If
        End If

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Text = My.Application.Info.AssemblyName

        'Impostazioni parametri generali'
        picback.Dock = DockStyle.Fill

        'Gestione menu di sistema'
        Call AddSystemMenu(tlbmenu, "Menu")
        Call SetEnabledItemsMenuSystem()

        'Leggo le applicazioni disponibili'
        applist.ReadApplication(True, False)
        applist.SetFavorite(AppSett.MainWindows.Favorites)

        'Imposto lo stato delle applicazioni e aggiorno l'elenco dei favoriti'
        Dim app As New List(Of String)
        For i As Integer = 0 To applist.AppList.Count - 1
            applist.AppList(i).Enabled = GetAppEnabled(i)
            If applist.AppList(i).Enabled Then app.Add(applist.AppList(i).Name)
        Next
        For i As Integer = AppSett.MainWindows.Favorites.Count - 1 To 0 Step -1
            If app.Contains(AppSett.MainWindows.Favorites(i)) = False Then AppSett.MainWindows.Favorites.RemoveAt(i)
        Next

        'Setto la scheda aperta l'ultima volta'
        If AppSett.Session.TabSelect > -1 AndAlso AppSett.Session.TabSelect < applist.Tabs.Count Then
            applist.TabIndex = AppSett.Session.TabSelect
        Else
            applist.TabIndex = 0
        End If
        If AppSett.MainWindows.WindowsView < 0 Then AppSett.MainWindows.WindowsView = 0
        If AppSett.MainWindows.WindowsView > 5 Then AppSett.MainWindows.WindowsView = 5

        wsize.Add(New Size(525, 470))
        wsize.Add(New Size(608, 539))

        For i As Integer = 0 To wsize.Count - 1
            tlb1.Button(1).SubItems.Add(New iControl.ToolBarButtonSubItem(wsize(i).Width & "x" & wsize(i).Height))
        Next

        tlb1.Button(2).SubItems.Add(New iControl.ToolBarButtonSubItem("Riquadri larghi"))
        tlb1.Button(2).SubItems.Add(New iControl.ToolBarButtonSubItem("Riquadri piccoli"))
        tlb1.Button(2).SubItems.Add(New iControl.ToolBarButtonSubItem("Lista larga"))
        tlb1.Button(2).SubItems.Add(New iControl.ToolBarButtonSubItem("Lista piccola"))
        tlb1.Button(2).SubItems.Add(New iControl.ToolBarButtonSubItem("Dettagli larga"))
        tlb1.Button(2).SubItems.Add(New iControl.ToolBarButtonSubItem("Dettagli compatta"))

        For i As Integer = 8 To 16
            tlb1.Button(3).SubItems.Add(New iControl.ToolBarButtonSubItem(CStr(i)))
        Next

        Call SetHotKeys()
        Call SetToolBarState()

        'Setto il tema'
        Call SetTheme(True)

        'Dim lines() As String = IO.File.ReadAllLines(AppContext.BaseDirectory + "tornei\2018-2019\data\tbdati.csv")
        'For Each line As String In lines
        '    Try
        '        Dim s() As String = line.Split(",")
        '        ExecuteSql("INSERT INTO tbdati (gio,ruolo,nome,squadra,gf,gs,amm,esp,ass,rigp,rigs,rigt,autogol,voto,pt) value (" & s(0) & ",'" & s(1) & "','" & s(2) & "','" & s(3) & "'," & s(4) & "," & s(5) & "," & s(6) & "," & s(7) & "," & s(8) & "," & s(9) & "," & s(10) & "," & s(11) & "," & s(12) & "," & s(13) & "," & s(14) & "," & s(15) & ")", conn)

        '    Catch ex As Exception

        '    End Try

        'Next
        'ExecuteSql("CREATE TABLE tbformazioninew ( `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `gio` INTEGER NOT NULL DEFAULT 1, `idteam` INTEGER NOT NULL DEFAULT 0, `idrosa` INTEGER NOT NULL DEFAULT 0, `idformazione` INTEGER NOT NULL DEFAULT 0, `jolly` INTEGER NOT NULL DEFAULT 0, `type` INTEGER NOT NULL DEFAULT 0, `incampo` INTEGER NOT NULL DEFAULT 0, `ruolo` TEXT, `nome` TEXT, `squadra` TEXT, `vote` TEXT, `amm` INTEGER NOT NULL DEFAULT 0, `esp` INTEGER NOT NULL DEFAULT 0, `autogol` INTEGER NOT NULL DEFAULT 0, `gs` INTEGER NOT NULL DEFAULT 0, `gf` INTEGER NOT NULL DEFAULT 0, `ass` INTEGER NOT NULL DEFAULT 0, `rigs` INTEGER NOT NULL DEFAULT 0, `rigp` INTEGER NOT NULL DEFAULT 0, `pt` INTEGER NOT NULL DEFAULT 0 );", conn)
        'ExecuteSql("CREATE TABLE tbformazionitopnew ( `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `gio` INTEGER NOT NULL DEFAULT 1, `idteam` INTEGER NOT NULL DEFAULT 0, `idrosa` INTEGER NOT NULL DEFAULT 0, `idformazione` INTEGER NOT NULL DEFAULT 0, `jolly` INTEGER NOT NULL DEFAULT 0, `type` INTEGER NOT NULL DEFAULT 0, `incampo` INTEGER NOT NULL DEFAULT 0, `ruolo` TEXT, `nome` TEXT, `squadra` TEXT, `vote` TEXT, `amm` INTEGER NOT NULL DEFAULT 0, `esp` INTEGER NOT NULL DEFAULT 0, `autogol` INTEGER NOT NULL DEFAULT 0, `gs` INTEGER NOT NULL DEFAULT 0, `gf` INTEGER NOT NULL DEFAULT 0, `ass` INTEGER NOT NULL DEFAULT 0, `rigs` INTEGER NOT NULL DEFAULT 0, `rigp` INTEGER NOT NULL DEFAULT 0, `pt` INTEGER NOT NULL DEFAULT 0 );", conn)
        'ExecuteSql("CREATE VIEW formazione_top as SELECT f.gio, f.idteam, t.nome AS nometeam, t.allenatore AS allenatore, f.schierato, f.type, f.idformazione as idrosa, f.incampo, f.ruolo, f.nome,p.squadra,p.nat,p.natcode, f.vote, f.amm, f.esp, f.ass, f.autogol, f.gs, f.gf, f.rigp, f.rigs, f.pt,p.nump_tot,p.gs_tot,p.gf_tot,p.amm_tot,p.esp_tot,p.ass_tot,p.rigt_tot,p.rigs_tot,p.rigp_tot,p.sum_vt_tot,p.avg_vt,p.sum_pt_tot,p.avg_pt,p.pgio_tot,p.tit_tot,p.sos_tot,p.sub_tot,p.mm_tot,p.nump_last,p.gs_last,p.gf_last,p.ass_last,p.rigt_last,p.sum_vt_last,p.avg_vt_last,p.sum_pt_last,p.avg_pt_last,p.pgio_last,p.tit_last,p.sos_last,p.sub_last,p.mm_last,p.avg_mm_last,m.timem, m.teama, m.teamb FROM tbformazionitop as f LEFT JOIN tbteam AS t ON f.idteam = t.idteam LEFT JOIN player as p ON f.nome = p.nome LEFT JOIN tbmatch AS m ON (m.teama=p.squadra or m.teamb=p.squadra) AND (m.gio=f.gio)", conn)

        'ExecuteSql("CREATE TABLE tbformazioninew ( `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `gio` INTEGER NOT NULL DEFAULT 1, `idteam` INTEGER NOT NULL DEFAULT 0, `idrosa` INTEGER NOT NULL DEFAULT 0, `idformazione` INTEGER NOT NULL DEFAULT 0, `jolly` INTEGER NOT NULL DEFAULT 0, `type` INTEGER NOT NULL DEFAULT 0, `incampo` INTEGER NOT NULL DEFAULT 0, `ruolo` TEXT, `nome` TEXT, `squadra` TEXT, `vote` TEXT, `amm` INTEGER NOT NULL DEFAULT 0, `esp` INTEGER NOT NULL DEFAULT 0, `autogol` INTEGER NOT NULL DEFAULT 0, `gs` INTEGER NOT NULL DEFAULT 0, `gf` INTEGER NOT NULL DEFAULT 0, `ass` INTEGER NOT NULL DEFAULT 0, `rigs` INTEGER NOT NULL DEFAULT 0, `rigp` INTEGER NOT NULL DEFAULT 0, `pt` INTEGER NOT NULL DEFAULT 0 );", conn)
        'ExecuteSql("CREATE TABLE tbformazionitopnew ( `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `gio` INTEGER NOT NULL DEFAULT 1, `idteam` INTEGER NOT NULL DEFAULT 0, `idrosa` INTEGER NOT NULL DEFAULT 0, `idformazione` INTEGER NOT NULL DEFAULT 0, `jolly` INTEGER NOT NULL DEFAULT 0, `type` INTEGER NOT NULL DEFAULT 0, `incampo` INTEGER NOT NULL DEFAULT 0, `ruolo` TEXT, `nome` TEXT, `squadra` TEXT, `vote` TEXT, `amm` INTEGER NOT NULL DEFAULT 0, `esp` INTEGER NOT NULL DEFAULT 0, `autogol` INTEGER NOT NULL DEFAULT 0, `gs` INTEGER NOT NULL DEFAULT 0, `gf` INTEGER NOT NULL DEFAULT 0, `ass` INTEGER NOT NULL DEFAULT 0, `rigs` INTEGER NOT NULL DEFAULT 0, `rigp` INTEGER NOT NULL DEFAULT 0, `pt` INTEGER NOT NULL DEFAULT 0 );", conn)
        'ExecuteSql("CREATE VIEW formazione_top as SELECT f.gio, f.idteam, t.nome AS nometeam, t.allenatore AS allenatore, f.schierato, f.type, f.idformazione as idrosa, f.incampo, f.ruolo, f.nome,p.squadra,p.nat,p.natcode, f.vote, f.amm, f.esp, f.ass, f.autogol, f.gs, f.gf, f.rigp, f.rigs, f.pt,p.nump_tot,p.gs_tot,p.gf_tot,p.amm_tot,p.esp_tot,p.ass_tot,p.rigt_tot,p.rigs_tot,p.rigp_tot,p.sum_vt_tot,p.avg_vt,p.sum_pt_tot,p.avg_pt,p.pgio_tot,p.tit_tot,p.sos_tot,p.sub_tot,p.mm_tot,p.nump_last,p.gs_last,p.gf_last,p.ass_last,p.rigt_last,p.sum_vt_last,p.avg_vt_last,p.sum_pt_last,p.avg_pt_last,p.pgio_last,p.tit_last,p.sos_last,p.sub_last,p.mm_last,p.avg_mm_last,m.timem, m.teama, m.teamb FROM tbformazionitop as f LEFT JOIN tbteam AS t ON f.idteam = t.idteam LEFT JOIN player as p ON f.nome = p.nome LEFT JOIN tbmatch AS m ON (m.teama=p.squadra or m.teamb=p.squadra) AND (m.gio=f.gio)", conn)

        'For i As Integer = 1 To 38
        '    For k As Integer = 0 To 10

        '        Dim f As New LegaObject.Formazione
        '        Dim pf As New LegaObject.Formazione
        '        Dim idf As Integer = 1

        '        f.IdTeam = k
        '        f.Giornata = i
        '        f.Load()

        '        pf.Giornata = 1
        '        pf.IdTeam = k
        '        pf.BonusAttacco = f.BonusAttacco
        '        pf.BonusCentroCampo = f.BonusCentroCampo
        '        pf.BonusDifesa = f.BonusDifesa

        '        For j As Integer = 0 To f.Players.Count - 1
        '            If f.Players(j).Type = 0 OrElse f.Players(j).Type = 2 Then
        '                Dim p As LegaObject.Formazione.PlayerFormazione = f.Players(j).Clone()
        '                If f.Players(j).Type = 2 Then
        '                    p.Jolly = 1
        '                End If
        '                If f.Players(j).Schierato = 1 Then
        '                    p.Type = 1
        '                    p.IdRosa = f.Players(j).IdRosa
        '                    p.IdFormazione = idf
        '                    idf += 1
        '                Else
        '                    p.Type = 0
        '                    p.IdFormazione = 0
        '                End If
        '                pf.Players.Add(p)
        '            End If
        '        Next

        '        For j As Integer = 0 To f.Players.Count - 1
        '            If f.Players(j).Type = 1 Then
        '                For l As Integer = 0 To pf.Players.Count - 1
        '                    If f.Players(j).Nome = pf.Players(l).Nome Then
        '                        pf.Players(l).Type = 2
        '                        pf.Players(l).IdFormazione = f.Players(j).IdRosa + 11
        '                        pf.Players(l).InCampo = f.Players(j).InCampo
        '                    End If
        '                Next
        '            End If
        '            If f.Players(j).Type = 3 Then
        '                For l As Integer = 0 To pf.Players.Count - 1
        '                    If f.Players(j).Ruolo = pf.Players(l).Ruolo AndAlso pf.Players(l).Type = 0 Then
        '                        pf.Players(l).Type = 2
        '                        pf.Players(l).Jolly = 1
        '                        pf.Players(l).Nome = f.Players(j).Nome
        '                        pf.Players(l).Squadra = f.Players(j).Squadra
        '                        pf.Players(l).IdFormazione = f.Players(j).IdRosa + 11
        '                        pf.Players(l).InCampo = f.Players(j).InCampo
        '                        Exit For
        '                    End If
        '                Next
        '            End If
        '        Next
        '        LegaObject.Formazione.Save1(pf, k, i, False)

        '    Next
        'Next

        'For i As Integer = 1 To 38
        '    For k As Integer = 0 To 10

        '        Dim f As New LegaObject.Formazione
        '        Dim pf As New LegaObject.Formazione
        '        Dim idf As Integer = 1

        '        f.IdTeam = k
        '        f.Giornata = i
        '        f.Load(True)

        '        pf.Giornata = 1
        '        pf.IdTeam = k
        '        pf.BonusAttacco = f.BonusAttacco
        '        pf.BonusCentroCampo = f.BonusCentroCampo
        '        pf.BonusDifesa = f.BonusDifesa

        '        For j As Integer = 0 To f.Players.Count - 1
        '            If f.Players(j).Type = 0 OrElse f.Players(j).Type = 2 Then
        '                Dim p As LegaObject.Formazione.PlayerFormazione = f.Players(j).Clone()
        '                If f.Players(j).Type = 2 Then
        '                    p.Jolly = 1
        '                End If
        '                If f.Players(j).Schierato = 1 Then
        '                    p.Type = 1
        '                    p.IdRosa = f.Players(j).IdRosa
        '                    p.IdFormazione = idf
        '                    idf += 1
        '                Else
        '                    p.Type = 0
        '                    p.IdFormazione = 0
        '                End If
        '                pf.Players.Add(p)
        '            End If
        '        Next

        '        LegaObject.Formazione.Save1(pf, k, i, True)

        '    Next
        'Next

        Dim TStart As New ThreadStart(AddressOf BackgroundActivity)
        Thrd = New Thread(TStart)
        Thrd.Priority = ThreadPriority.Highest
        Thrd.Start()

    End Sub

    Private Sub frmgenerale2_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            AppSett.Session.TabSelect = applist.TabIndex
            AppSett.Session.SubTabSelect = applist.CurrentTab.SubTabIndex
            AppSett.SaveSettings()
            If Thrd.ThreadState = ThreadState.Running Then
                Thrd.Abort()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub pic1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If Date.Now.Subtract(tt).Milliseconds > 100 Then
                ReleaseCapture()
                SendMessage(CInt(Me.Handle), WM_NCLBUTTONDOWN, HTCAPTION, 0)
            End If
        End If
        tt = Date.Now
    End Sub

    Private Sub pnl3_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pic1.MouseUp
        Call ReleaseCapture()
        tt = Date.Now
    End Sub

    'Gestione passagio mouse su pulsanti control box''
    Private Sub cnt1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cnt1.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub cnt3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cnt3.Click
        Me.Close()
    End Sub

    Private Sub cnt1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt1.MouseEnter
        cnt1.Image = ctlimg(1)
    End Sub

    Private Sub cnt1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt1.MouseLeave
        cnt1.Image = ctlimg(0)
    End Sub

    Private Sub cnt3_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt3.MouseEnter
        cnt3.Image = ctlimg(6)
    End Sub

    Private Sub cnt3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnt3.MouseLeave
        cnt3.Image = ctlimg(5)
    End Sub

    Private Sub txt3_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt3.KeyDown
        If KeyScrollPage(e.KeyData) Then
            Call LoadPage(True)
        End If
    End Sub

    Private Sub txt3_TextChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt3.TextChange
        Timer1.Stop()
        Timer1.Enabled = True
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        applist.CurrentSubTab.ScrollIndex = 0
        LoadPage(True)
    End Sub

    Private Sub Timer2_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Timer2.Stop()
        _oldind = -1
        applist.CurrentSubTab.ScrollIndex = vsrc1.Value
        Call DrawApp()
    End Sub

    Private Sub tlb1_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlb1.ButtonClick

        Select Case ButtonIndex
            Case 5
                'Open folder'
                Try
                    If applist.CurrentSubTab.DefaultFolder <> "" Then
                        If applist.CurrentSubTab.ReadLocalFiles AndAlso applist.CurrentSubTab.Directory <> "\" Then
                            Process.Start("explorer.exe", My.Application.Info.DirectoryPath & "\" & applist.CurrentSubTab.DefaultFolder & applist.CurrentSubTab.Directory)
                        Else
                            Process.Start("explorer.exe", My.Application.Info.DirectoryPath & "\" & applist.CurrentSubTab.DefaultFolder)
                        End If
                    End If
                Catch ex As Exception

                End Try
        End Select

    End Sub

    Private Sub tlb1_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlb1.SubButtonClick
        AppSett.ReadSettings()
        Select Case ButtonIndex
            Case 1
                AppSett.MainWindows.WindowsSize = SubButtonIndex
                AppSett.SaveSettings()
                Call SetTheme(True)
            Case 2
                AppSett.MainWindows.WindowsView = SubButtonIndex
                Select Case AppSett.MainWindows.WindowsView
                    Case 0 : AppSett.MainWindows.MenuFontSize = 10
                    Case 1 : AppSett.MainWindows.MenuFontSize = 10
                    Case 2 : AppSett.MainWindows.MenuFontSize = 13
                    Case 3 : AppSett.MainWindows.MenuFontSize = 10
                    Case 4 : AppSett.MainWindows.MenuFontSize = 9
                    Case 5 : AppSett.MainWindows.MenuFontSize = 8
                End Select
                If AppSett.MainWindows.WindowsSize = 1 Then
                    AppSett.MainWindows.MenuFontSize += 2
                End If

                tlb1.Button(ButtonIndex).SetSubButtonSelection(CStr(AppSett.MainWindows.MenuFontSize), True)
            Case 3 : AppSett.MainWindows.MenuFontSize = CInt(tlb1.Button(ButtonIndex).SubItems(SubButtonIndex).Text)
        End Select
        AppSett.SaveSettings()
        Call SetToolBarState()
        Call DisplayPage(True)
    End Sub

    Private Sub cmdrest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdrest.Click
        Call SystemMenu(7, Me)
    End Sub

    Private Sub cmdupd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdupd.Click
        Call SystemMenu(6, Me)
    End Sub

    Private Sub cmdconne_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdconne.Click
        'Call SystemMenu(0, Me)
    End Sub

    Private Sub tlbmenu_SubButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer, ByVal SubButtonIndex As Integer) Handles tlbmenu.SubButtonClick
        Dim act As Integer = CInt(tlbmenu.Button(ButtonIndex).SubItems(SubButtonIndex).Tag)
        Call SystemMenu(act, Me)
        If act = 0 Then
            'Call UpdateData()
            Call DisplayPage(False)
        End If
    End Sub

#End Region

    Sub SystemMenu(ByVal Action As Integer, ByVal frm As Form)

        tlbmenu.HideDropDown()

        Try
            Select Case Action
                Case 8
                    Dim dir As String = ""
                    Dim frm1 As New frmload
                    If frm1.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                        Call LoadLega(frm1.Directory())
                    End If
                    frm1.Close()
                    frm1.Dispose()
                Case 0
                    'Crea torneo'
                Case 1
                    'Modifica torneo'

                Case 2
                    'Elimina torneo

                Case 3
                    'Backup dati'
                    Dim frm1 As New frmbackup
                    frm1.Show(frm)
                Case 4
                    'Rispritino dati'
                    Dim frm1 As New frmrestore
                    frm1.Show(frm)
                Case 5
                    'Update'
                    Try
                        System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\ifup.exe", "")
                    Catch ex As Exception
                        Call ShowError("Errore", "Aggiornamento fallito")
                    End Try
                Case 6
                    'Restore'
                    Try
                        System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\ifup.exe", "restore")
                    Catch ex As Exception
                        Call ShowError("Errore", "Ripritino fallito")
                    End Try
                Case 7
                    'Impostazioni ifanta'
                    Dim frm1 As New frmsettings
                    If frm1.ShowDialog(frm) = Windows.Forms.DialogResult.OK Then

                    End If
                Case 9
                    'Aggiornamento dati su server'
                    Dim frm1 As New frmupdateserver
                    frm1.Show(frm)
            End Select
        Catch ex As Exception
            Call WriteError("Comune", "SystemMenu", ex.Message)
        End Try

    End Sub

    Sub LoadLega(ByVal LegaSelected As String)

        Dim intconn As New InternetConnection.ConnType

        intconn = InternetConnection.Type

        'Carico i dati del torneo'
        currlega = New LegaObject
        currlega.Settings.Nome = IO.Path.GetFileName(LegaSelected)
        currlega.ReadSettings()
        webdata.YearReference = currlega.Settings.Year
        backup.DataDirectory = GetLegaDataDirectory()
        backup.BackupDirectory = GetLegaBackupDirectory()
        Call SetEnabledItemsMenuSystem()

        'Creo le sotto directory del torneo'
        Call MakeLegaDirectoryes()

        Try

            conn.Close()
            Call OpenConnection()

            If conn.State = ConnectionState.Open Then
                currlega.LoadTeams(False)
                currlega.GetCurrentLegaDay()
                webdata.UpdateWebData(currlega.Settings.Active, False, webdata.WebSiteList, intconn)
            Else
                Call ShowError("Errore", "Connessione dati fallita")
                currlega.Settings.Nome = ""
            End If

            'Aggiorno i vari form'
            For i As Integer = 0 To Application.OpenForms.Count - 1
                Try
                    Dim ff As String = Application.OpenForms(i).ToString
                    Dim f As iFantaForm = CType(Application.OpenForms(i), iFantaForm)
                    f.ResetTorneo()
                Catch ex As Exception

                End Try
            Next
        Catch ex As Exception

        End Try

        'Abilito se necessario le applicazioni'
        For i As Integer = 0 To applist.AppList.Count - 1
            applist.AppList(i).Enabled = GetAppEnabled(i)
        Next

        Call Draw()
        Call DisplayPage(True)

    End Sub
End Class
