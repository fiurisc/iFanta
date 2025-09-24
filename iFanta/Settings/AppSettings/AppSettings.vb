Imports System.Text.RegularExpressions
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory

Public Class AppSettings

    Private _personal As New PersonalSettings
    Private _update As New UpdateSettings
    Private _session As New SessionSettings
    Private _mainwin As New MainWindowSettings
    Private _backup As New BackupSettings

    Private Shared fname As String = GetSystemDirectory() & "\app.ini"

    Public Property Personal As PersonalSettings
        Get
            Return _personal
        End Get
        Set(ByVal value As PersonalSettings)
            _personal = value
        End Set
    End Property

    Public Property Update As UpdateSettings
        Get
            Return _update
        End Get
        Set(value As UpdateSettings)
            _update = value
        End Set
    End Property

    Public Property Session As SessionSettings
        Get
            Return _session
        End Get
        Set(value As SessionSettings)
            _session = value
        End Set
    End Property

    Public Property MainWindows As MainWindowSettings
        Get
            Return _mainwin
        End Get
        Set(value As MainWindowSettings)
            _mainwin = value
        End Set
    End Property

    Public Property Backup As BackupSettings
        Get
            Return _backup
        End Get
        Set(value As BackupSettings)
            _backup = value
        End Set
    End Property

    Shared Function GetMySettingsFileName() As String
        Return fname
    End Function

    ''' <summary>Consente di caricare le impostazioni salvate su disco</summary>
    Sub ReadSettings()

        If IO.File.Exists(fname) Then
            Try

                Dim lines() As String = IO.File.ReadAllLines(fname)

                For i As Integer = 0 To lines.Length - 1

                    Dim line As String = lines(i)

                    Dim para As String = Regex.Match(line, ".+(?=\= ')").Value.Trim
                    Dim value As String = Regex.Match(line, "(?<= ').+(?=')").Value
                    Dim sz(1) As String

                    If para <> "" AndAlso value <> "" Then

                        If para.Contains("window size") Then sz = value.Split(CChar("x"))

                        Try
                            Select Case para
                                Case "Personal - Mail" : _personal.Mail = value
                                Case "Personal - Send mail also to me" : _personal.SendMailAlsoToMe = CBool(value)
                                Case "Personal - Theme" : _personal.Theme.Name = value
                                Case "Personal - FlatStyle" : _personal.Theme.FlatStyle = CBool(value)
                                Case "Update - Enabled software update" : _update.EnableUpdate = CBool(value)
                                Case "Update - Force check software update startup" : _update.ForceCheckUpdateStartup = CBool(value)
                                Case "Update - Enabled web data update" : webdata.EnableUpdate = CBool(value)
                                Case "Update - Force update web data startup" : webdata.ForceUpdateStartup = CBool(value)
                                Case "Session - Last lega selected" : _session.LastLegaSelected = value
                                Case "Session - Last tab select" : _session.TabSelect = CInt(value)
                                Case "Session - Last subtab select" : _session.SubTabSelect = CInt(value)
                                Case "Session - Use system directory for import and export data" : _session.UseDefaultDirectoryForImportAndExport = CBool(value)
                                Case "Session - Last import and export directory" : _session.LastImportAndExportDirectory = value
                                Case "Main window - Favorites" : SetFavorite(value)
                                Case "Main window - Size" : _mainwin.WindowsSize = CInt(value)
                                Case "Main window - Type list view" : _mainwin.WindowsView = CInt(value)
                                Case "Main window - App list font size" : _mainwin.MenuFontSize = CInt(value)
                                Case "Main window - HotKeys application" : SetHotKeys(value)
                                Case "Backup - Enable" : _backup.Enable = CBool(value)
                                Case "Backup - Enable compression" : _backup.EnableCompression = CBool(value)
                                Case "Backup - Executed only after modify" : _backup.ExecuteBackupOnlyAfterModify = CBool(value)
                                Case "Backup - History" : _backup.History = CInt(value)
                            End Select

                        Catch ex As Exception
                            Call WriteError("Settings", "ReadSettings (AppSettings)", ex.Message)
                        End Try
                    End If
                Next
                If _mainwin.MenuFontSize < 7 Then _mainwin.MenuFontSize = 7
                If _mainwin.MenuFontSize > 14 Then _mainwin.MenuFontSize = 14

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End If

    End Sub

    ''' <summary>Consente di salvare le impostazioni su disco</summary>
    Sub SaveSettings()
        Try
            Dim str As New System.Text.StringBuilder
            'General'
            str.AppendLine("Personal - Mail = '" & _personal.Mail & "'")
            str.AppendLine("Personal - Send mail also to me = '" & _personal.SendMailAlsoToMe & "'")
            str.AppendLine("Personal - Theme = '" & _personal.Theme.Name & "'")
            str.AppendLine("Personal - FlatStyle = '" & _personal.Theme.FlatStyle & "'")
            str.AppendLine("Update - Enabled software update = '" & _update.EnableUpdate & "'")
            str.AppendLine("Update - Force check software update startup = '" & _update.ForceCheckUpdateStartup & "'")
            str.AppendLine("Update - Enabled web data update = '" & webdata.EnableUpdate & "'")
            str.AppendLine("Update - Force update web data startup = '" & webdata.ForceUpdateStartup & "'")
            str.AppendLine("Session - Last lega selected = '" & _session.LastLegaSelected & "'")
            str.AppendLine("Session - Last tab select = '" & _session.TabSelect & "'")
            str.AppendLine("Session - Last subtab select = '" & _session.SubTabSelect & "'")
            str.AppendLine("Session - Use system directory for import and export data = '" & _session.UseDefaultDirectoryForImportAndExport & "'")
            str.AppendLine("Session - Last import and export directory = '" & _session.LastImportAndExportDirectory & "'")
            str.AppendLine("Main window - Favorites = '" & GetFavoriteString() & "'")
            str.AppendLine("Main window - Window type list view = '" & _mainwin.WindowsView & "'")
            str.AppendLine("Main window - Window size = '" & _mainwin.WindowsSize & "'")
            str.AppendLine("Main window - App list font size = '" & _mainwin.MenuFontSize & "'")
            str.AppendLine("Main window - HotKeys Application = '" & GetHotKyesString() & "'")
            str.AppendLine("Backup - Enable backup = '" & _backup.Enable & "'")
            str.AppendLine("Backup - Enable backup compression = '" & _backup.EnableCompression & "'")
            str.AppendLine("Backup - Backup only after mofify = '" & _backup.ExecuteBackupOnlyAfterModify & "'")
            str.AppendLine("Backup - Backup history = '" & _backup.History & "'")

            IO.File.WriteAllText(fname, str.ToString)
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Private Sub SetFavorite(ByVal value As String)
        _mainwin.Favorites.Clear()
        Dim s() As String = value.Split(CChar("|"))
        For i As Integer = 0 To s.Length - 1
            If _mainwin.Favorites.Contains(s(i)) = False Then _mainwin.Favorites.Add(s(i))
        Next
    End Sub

    Private Function GetFavoriteString() As String
        Dim str As New System.Text.StringBuilder
        For i As Integer = 0 To _mainwin.Favorites.Count - 1
            If _mainwin.Favorites(i) <> "" Then str.Append("|" & _mainwin.Favorites(i))
        Next
        If str.Length > 0 Then
            Return str.ToString.Substring(1)
        Else
            Return ""
        End If
    End Function

    Private Sub SetHotKeys(ByVal value As String)
        _mainwin.HotKeys.Clear()
        Try
            Dim s() As String = value.Split(CChar("|"))
            For i As Integer = 0 To s.Length - 1
                Dim d() As String = s(i).Split(CChar(","))
                If d.Length = 3 Then
                    If _mainwin.HotKeys.ContainsKey(CInt(d(0))) = False Then _mainwin.HotKeys.Add(CInt(d(0)), New HotKeyItem(CInt(d(1)), d(2)))
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Private Function GetHotKyesString() As String
        Dim str As New System.Text.StringBuilder
        For i As Integer = 1 To 12
            If _mainwin.HotKeys.ContainsKey(i) Then
                str.Append("|" & CStr(i) & "," & _mainwin.HotKeys(i).AppId & "," & _mainwin.HotKeys(i).AppName)
            End If
        Next
        If str.Length > 0 Then
            Return str.ToString.Substring(1)
        Else
            Return ""
        End If
    End Function

    Public Class HotKeyItem

        Dim _appid As Integer = 0
        Dim _appname As String = ""

        Sub New(ByVal AppId As Integer, ByVal AppName As String)
            _appid = AppId
            _appname = AppName
        End Sub

        Public Property AppId() As Integer
            Get
                Return _appid
            End Get
            Set(ByVal value As Integer)
                _appid = value
            End Set
        End Property

        Public Property AppName() As String
            Get
                Return _appname
            End Get
            Set(ByVal value As String)
                _appname = value
            End Set
        End Property

    End Class

    Public Class ThemeSettings

        Enum eBackPosition As Integer
            TopLeft = 0
            TopCenter = 1
            TopRight = 2
            MiddleLeft = 3
            MiddleCenter = 4
            MiddleRight = 5
            BottomLeft = 6
            BottomCenter = 7
            BottomRight = 8
            Stretch = 9
        End Enum

        Private _name As String = "red"
        Private _flat As Boolean = False
        Private _SelectColor1 As Color = Color.White
        Private _SelectColor2 As Color = Color.White
        Private _backgroundposition As eBackPosition = eBackPosition.Stretch
        Private mtx(26, 1) As Color

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property FlatStyle() As Boolean
            Get
                Return _flat
            End Get
            Set(ByVal value As Boolean)
                _flat = value
            End Set
        End Property

        Public Property SelectionColor1() As Color
            Get
                Return _SelectColor1
            End Get
            Set(ByVal value As Color)
                _SelectColor1 = value
            End Set
        End Property

        Public Property SelectionColor2() As Color
            Get
                Return _SelectColor2
            End Get
            Set(ByVal value As Color)
                _SelectColor2 = value
            End Set
        End Property

        Public Property BackgroundPosition() As eBackPosition
            Get
                Return _backgroundposition
            End Get
            Set(ByVal value As eBackPosition)
                _backgroundposition = value
            End Set
        End Property

        Public Property Item(ByVal Index As Integer, ByVal Color As Integer) As Color
            Get
                Return mtx(Index, Color)
            End Get
            Set(ByVal value As Color)
                mtx(Index, Color) = value
            End Set
        End Property

        Function GetThemeSettings() As Color(,)
            Return mtx
        End Function

        Sub ReadSettings()
            Call ReadSettings(_name, _flat)
        End Sub

        Sub ReadSettings(ByVal Name As String, ByVal FlatStyle As Boolean)

            'Carico i valori di default'
            Dim filethemename As String = GetFileTheme(Name, FlatStyle)

            'Setto i colori di default'
            _backgroundposition = eBackPosition.Stretch

            If File.Exists(filethemename) = True Then
                Call ReadThemeParameater(IO.File.ReadAllText(filethemename))
            Else
                If _flat Then
                    Call ReadThemeParameater(My.Resources.themeflat)
                Else
                    Call ReadThemeParameater(My.Resources.theme)
                End If
            End If

        End Sub

        Private Sub ReadThemeParameater(ByVal Str As String)
            Try
                Dim lines() As String = Str.Split(CChar(System.Environment.NewLine))
                For i As Integer = 0 To lines.Length - 1
                    Dim para() As String = lines(i).Trim.Split(CChar(","))
                    If para.Length > 0 Then
                        If para(0) <> "" Then
                            Select Case para(0).ToLower
                                Case "sel1" : _SelectColor1 = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                Case "sel2" : _SelectColor2 = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                Case "cb"
                                Case "back" : _backgroundposition = CType(CInt(para(1)), eBackPosition)
                                Case Else
                                    If para.Length > 5 Then
                                        mtx(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                        mtx(CInt(para(0)), 1) = Color.FromArgb(CInt(para(4)), CInt(para(5)), CInt(para(6)))
                                    Else
                                        mtx(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                        mtx(CInt(para(0)), 1) = mtx(CInt(para(0)), 0)
                                    End If
                            End Select
                            If para(0) <> "cb" Then

                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Public Function GetThemeDefaultColor(ByVal FlatStyle As Boolean) As Color(,)

            Dim str As String = ""
            Dim mtxcolor(26, 1) As Color

            mtxcolor = iControl.CommonFunction.GetInternalColor(FlatStyle)

            Try

                If FlatStyle Then
                    str = My.Resources.themeflat
                Else
                    str = My.Resources.theme
                End If

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
                                        mtxcolor(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                        mtxcolor(CInt(para(0)), 1) = Color.FromArgb(CInt(para(4)), CInt(para(5)), CInt(para(6)))
                                    Else
                                        mtxcolor(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                        mtxcolor(CInt(para(0)), 1) = mtx(CInt(para(0)), 0)
                                    End If
                            End Select
                        End If
                    End If
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return mtxcolor

        End Function

        Sub SaveSettings()
            Call SaveSettings(_name, _flat)
        End Sub

        Sub SaveSettings(ByVal Name As String, ByVal FlatStyle As Boolean)

            Dim filethemename As String = GetFileTheme(Name, FlatStyle)
            Dim dir As String = IO.Path.GetDirectoryName(filethemename)
            If IO.Directory.Exists(dir) = False Then IO.Directory.CreateDirectory(dir)
            Try
                Dim str As New System.Text.StringBuilder
                For i As Integer = 0 To mtx.GetUpperBound(0) - 1
                    If mtx(i, 0) <> Color.Empty Then
                        If mtx(i, 1) <> Color.Empty Then
                            str.Append(i & "," & mtx(i, 0).R & "," & mtx(i, 0).G & "," & mtx(i, 0).B & "," & mtx(i, 1).R & "," & mtx(i, 1).G & "," & mtx(i, 1).B & System.Environment.NewLine)
                        Else
                            str.Append(i & "," & mtx(i, 0).R & "," & mtx(i, 0).G & "," & mtx(i, 0).B & System.Environment.NewLine)
                        End If
                    End If
                Next
                str.Append("sel1," & _SelectColor1.R & "," & _SelectColor1.G & "," & _SelectColor1.B & System.Environment.NewLine)
                str.Append("sel2," & _SelectColor2.R & "," & _SelectColor2.G & "," & _SelectColor2.B & System.Environment.NewLine)
                str.Append("back," & _backgroundposition & System.Environment.NewLine)
                IO.File.WriteAllText(filethemename, str.ToString)
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Public Sub EmptyColorSettings()
            For i As Integer = 0 To mtx.GetUpperBound(0) - 1
                mtx(i, 0) = Color.Empty
                mtx(i, 1) = Color.Empty
            Next
        End Sub

        Private Function GetFileTheme(ByVal Name As String, ByVal FlatStyle As Boolean) As String
            Dim fname As String = My.Application.Info.DirectoryPath & "\THEME\" & Name & "\Normal\theme.ini"
            If FlatStyle = True Then fname = My.Application.Info.DirectoryPath & "\THEME\" & Name & "\Flat\theme.ini"
            Return fname
        End Function

        Private Sub SetDefaultColor(ByVal FlatStyle As Boolean)

            If FlatStyle = False Then
                'Border top'
                mtx(0, 0) = Color.FromArgb(255, 135, 112)
                mtx(1, 0) = Color.FromArgb(255, 135, 112)
                mtx(2, 0) = Color.FromArgb(255, 135, 112)
                mtx(0, 1) = Color.FromArgb(255, 91, 59)
                mtx(1, 1) = Color.FromArgb(255, 91, 59)
                mtx(2, 1) = Color.FromArgb(255, 91, 59)
                'Topar'
                mtx(3, 0) = Color.FromArgb(255, 91, 59)
                mtx(3, 1) = Color.FromArgb(230, 0, 0)
                mtx(4, 0) = Color.FromArgb(230, 0, 0)
                mtx(4, 1) = Color.FromArgb(230, 0, 0)
                mtx(12, 0) = Color.FromArgb(230, 0, 0)
                mtx(12, 1) = Color.FromArgb(230, 0, 0)
                mtx(13, 0) = Color.FromArgb(255, 91, 59)
                mtx(13, 1) = Color.FromArgb(230, 0, 0)
                'Center'
                mtx(5, 0) = Color.FromArgb(230, 0, 0)
                mtx(5, 1) = Color.FromArgb(195, 6, 6)
                mtx(11, 0) = Color.FromArgb(230, 0, 0)
                mtx(11, 1) = Color.FromArgb(195, 6, 6)
                'Bottonbar'
                mtx(6, 0) = Color.FromArgb(195, 6, 6)
                mtx(6, 1) = Color.FromArgb(195, 6, 6)
                mtx(10, 0) = Color.FromArgb(195, 6, 6)
                mtx(10, 1) = Color.FromArgb(195, 6, 6)
                'Botton'
                mtx(7, 0) = Color.FromArgb(195, 6, 6)
                mtx(8, 0) = Color.FromArgb(195, 6, 6)
                mtx(9, 0) = Color.FromArgb(195, 6, 6)
                mtx(7, 1) = Color.FromArgb(178, 0, 0)
                mtx(8, 1) = Color.FromArgb(178, 0, 0)
                mtx(9, 1) = Color.FromArgb(178, 0, 0)

                'Topbar1'
                mtx(14, 0) = Color.FromArgb(255, 91, 59)
                mtx(14, 1) = Color.FromArgb(230, 0, 0)
                mtx(15, 0) = Color.FromArgb(170, 0, 0)
                mtx(15, 1) = Color.FromArgb(170, 0, 0)
                'Topbar2'
                mtx(16, 0) = Color.FromArgb(253, 253, 253)
                mtx(16, 1) = Color.FromArgb(220, 220, 220)
                mtx(17, 0) = Color.FromArgb(180, 180, 180)
                mtx(17, 1) = Color.FromArgb(180, 180, 180)
                'Topbar3'
                mtx(18, 0) = Color.FromArgb(210, 210, 210)
                mtx(18, 1) = Color.FromArgb(180, 180, 180)
                mtx(19, 0) = Color.FromArgb(160, 160, 160)
                mtx(19, 1) = Color.FromArgb(160, 160, 160)
                'Center'
                mtx(20, 0) = Color.FromArgb(230, 0, 0)
                mtx(20, 1) = Color.FromArgb(195, 6, 6)
                'Bottonbar1'
                mtx(21, 0) = Color.FromArgb(160, 160, 160)
                mtx(21, 1) = Color.FromArgb(160, 160, 160)
                mtx(22, 0) = Color.FromArgb(215, 215, 215)
                mtx(22, 1) = Color.FromArgb(190, 190, 190)
                'Bottonbar1'
                mtx(23, 0) = Color.FromArgb(180, 180, 180)
                mtx(23, 1) = Color.FromArgb(180, 180, 180)
                mtx(24, 0) = Color.FromArgb(253, 253, 253)
                mtx(24, 1) = Color.FromArgb(220, 220, 220)
                'Bottonbar1'
                mtx(25, 0) = Color.FromArgb(180, 180, 180)
                mtx(25, 1) = Color.FromArgb(180, 180, 180)
                mtx(26, 0) = Color.FromArgb(253, 253, 253)
                mtx(26, 1) = Color.FromArgb(220, 220, 220)
            Else
                'Border top'
                mtx(0, 0) = Color.FromArgb(248, 0, 0)
                mtx(1, 0) = Color.FromArgb(248, 0, 0)
                mtx(2, 0) = Color.FromArgb(248, 0, 0)
                mtx(0, 1) = Color.FromArgb(248, 0, 0)
                mtx(1, 1) = Color.FromArgb(248, 0, 0)
                mtx(2, 1) = Color.FromArgb(248, 0, 0)
                'Topar'
                mtx(3, 0) = Color.FromArgb(248, 0, 0)
                mtx(3, 1) = Color.FromArgb(248, 0, 0)
                mtx(4, 0) = Color.FromArgb(220, 0, 0)
                mtx(4, 1) = Color.FromArgb(220, 0, 0)
                mtx(12, 0) = Color.FromArgb(220, 0, 0)
                mtx(12, 1) = Color.FromArgb(220, 0, 0)
                'Center'
                mtx(5, 0) = Color.FromArgb(220, 0, 0)
                mtx(5, 1) = Color.FromArgb(220, 0, 0)
                mtx(11, 0) = Color.FromArgb(220, 0, 0)
                mtx(11, 1) = Color.FromArgb(220, 0, 0)
                'Bottonbar'
                mtx(6, 0) = Color.FromArgb(220, 0, 0)
                mtx(6, 1) = Color.FromArgb(220, 0, 0)
                mtx(10, 0) = Color.FromArgb(220, 0, 0)
                mtx(10, 1) = Color.FromArgb(220, 0, 0)
                'Botton'
                mtx(7, 0) = Color.FromArgb(248, 0, 0)
                mtx(8, 0) = Color.FromArgb(248, 0, 0)
                mtx(9, 0) = Color.FromArgb(248, 0, 0)
                mtx(7, 1) = Color.FromArgb(248, 0, 0)
                mtx(8, 1) = Color.FromArgb(248, 0, 0)
                mtx(9, 1) = Color.FromArgb(248, 0, 0)

                'Topbar1'
                mtx(14, 0) = Color.FromArgb(248, 0, 0)
                mtx(14, 1) = Color.FromArgb(248, 0, 0)
                mtx(15, 0) = Color.FromArgb(170, 0, 0)
                mtx(15, 1) = Color.FromArgb(170, 0, 0)
                'Topbar2'
                mtx(16, 0) = Color.FromArgb(253, 253, 253)
                mtx(16, 1) = Color.FromArgb(253, 253, 253)
                mtx(17, 0) = Color.FromArgb(180, 180, 180)
                mtx(17, 1) = Color.FromArgb(180, 180, 180)
                'Topbar3'
                mtx(18, 0) = Color.FromArgb(220, 220, 220)
                mtx(18, 1) = Color.FromArgb(220, 220, 220)
                mtx(19, 0) = Color.FromArgb(160, 160, 160)
                mtx(19, 1) = Color.FromArgb(160, 160, 160)
                'Center'
                mtx(20, 0) = Color.FromArgb(248, 0, 0)
                mtx(20, 1) = Color.FromArgb(248, 0, 0)
                'Bottonbar1'
                mtx(21, 0) = Color.FromArgb(180, 180, 180)
                mtx(21, 1) = Color.FromArgb(180, 180, 180)
                mtx(22, 0) = Color.FromArgb(220, 220, 220)
                mtx(22, 1) = Color.FromArgb(220, 220, 220)
                'Bottonbar1'
                mtx(23, 0) = Color.FromArgb(180, 180, 180)
                mtx(23, 1) = Color.FromArgb(180, 180, 180)
                mtx(24, 0) = Color.FromArgb(253, 253, 253)
                mtx(24, 1) = Color.FromArgb(253, 253, 253)
                'Bottonbar1'
                mtx(25, 0) = Color.FromArgb(180, 180, 180)
                mtx(25, 1) = Color.FromArgb(180, 180, 180)
                mtx(26, 0) = Color.FromArgb(253, 253, 253)
                mtx(26, 1) = Color.FromArgb(253, 253, 253)
            End If
            _SelectColor1 = Color.CornflowerBlue
            _SelectColor2 = Color.RoyalBlue
        End Sub

    End Class

End Class
