Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory

Public Class iApplication

    Private _Index As Integer = 0
    Private _ID As Integer = 0
    Private _shortcut As String = ""
    Private _dll As String = ""
    Private _Directory As String = "\"
    Private _Name As String = ""
    Private _NameSearch As String = ""
    Private _arg As String = ""
    Private _Path As String = ""
    Private _Icon As Integer = 0
    Private _TabIndex As Integer = 0
    Private _SubTabIndex As Integer = 0
    Private _Enabled As Boolean = False
    Private _EveryEnabled As Boolean = False
    Private _Description As String = ""
    Private _DescriptionSearch As String = ""
    Private _key As String = ""
    Private _favorite As Boolean = False

    Sub New()

    End Sub

    Sub New(ByVal App As iApplication)
        _ID = App.ID
        _shortcut = App.ShortCut
        _dll = App.Library
        _Directory = App.Directory
        _Name = App.Name
        _NameSearch = App.NameSearch
        _arg = App.Arg
        _Path = App.Path
        _Icon = App.Icon
        _TabIndex = App.TabIndex
        _SubTabIndex = App.SubTabIndex
        _Enabled = App.Enabled
        _EveryEnabled = EveryEnabled
        _Description = App.Description
        _DescriptionSearch = App.DescriptionSearch
        _key = App.Key
        _favorite = App.Favorite
    End Sub

    Sub New(ByVal Index As Integer, ByVal AppId As Integer, ByVal ShortCut As String, ByVal TabIndex As Integer, ByVal SubTabIndex As Integer, ByVal Library As String, ByVal Directory As String, ByVal Name As String, ByVal Description As String, ByVal Path As String, ByVal Icon As Integer, ByVal Enable As Boolean, ByVal EveryEnable As Boolean)
        _Index = Index
        _ID = AppId
        _shortcut = ShortCut
        _TabIndex = TabIndex
        _SubTabIndex = SubTabIndex
        _dll = Library
        _Directory = Directory
        _Name = Name
        _NameSearch = _Name.Replace(" ", "")
        _Description = Description
        _DescriptionSearch = _Description.Replace(" ", "")
        _Path = Path
        _Icon = Icon
        _Enabled = Enable
        _EveryEnabled = EveryEnable
        _key = SystemFunction.Cryptography.GetHashString(_Index & _Name)
        _key = _key.Replace("\", "#")
        _key = _key.Replace("/", "$")
    End Sub

    Public Property Index() As Integer
        Get
            Return _Index
        End Get
        Set(ByVal value As Integer)
            _Index = value
        End Set
    End Property

    Public Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property

    Public Property ShortCut() As String
        Get
            Return _shortcut
        End Get
        Set(ByVal value As String)
            _shortcut = value
        End Set
    End Property

    Public Property Library() As String
        Get
            Return _dll
        End Get
        Set(ByVal value As String)
            _dll = value
        End Set
    End Property

    Public Property Directory() As String
        Get
            Return _Directory
        End Get
        Set(ByVal value As String)
            _Directory = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Friend Property NameSearch() As String
        Get
            Return _NameSearch
        End Get
        Set(ByVal value As String)
            _NameSearch = value
        End Set
    End Property

    Public Property Arg() As String
        Get
            Return _arg
        End Get
        Set(ByVal value As String)
            _arg = value
        End Set
    End Property

    Public Property Path() As String
        Get
            Return _Path
        End Get
        Set(ByVal value As String)
            _Path = value
        End Set
    End Property

    Public Property Icon() As Integer
        Get
            Return _Icon
        End Get
        Set(ByVal value As Integer)
            _Icon = value
        End Set
    End Property

    Public Property TabIndex() As Integer
        Get
            Return _TabIndex
        End Get
        Set(ByVal value As Integer)
            _TabIndex = value
        End Set
    End Property

    Public Property SubTabIndex() As Integer
        Get
            Return _SubTabIndex
        End Get
        Set(ByVal value As Integer)
            _SubTabIndex = value
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            Return _Enabled
        End Get
        Set(ByVal value As Boolean)
            _Enabled = value
        End Set
    End Property

    Public Property EveryEnabled() As Boolean
        Get
            Return _EveryEnabled
        End Get
        Set(ByVal value As Boolean)
            _EveryEnabled = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property

    Friend Property DescriptionSearch() As String
        Get
            Return _DescriptionSearch
        End Get
        Set(ByVal value As String)
            _DescriptionSearch = value
        End Set
    End Property

    Public Property Key() As String
        Get
            Return _key
        End Get
        Set(ByVal value As String)
            _key = value
        End Set
    End Property

    Public Property Favorite() As Boolean
        Get
            Return _favorite
        End Get
        Set(ByVal value As Boolean)
            _favorite = value
        End Set
    End Property

End Class

Public Class ApplicationCollection

    Private _fname As String = GetSystemDirectory() & "\AppList.xml"
    Private _favorite As New List(Of String)
    Private _applist As New List(Of iApplication)
    Private _tabapplist As New List(Of iApplication)
    Private _tabs As New List(Of TabsItem)
    Private _search As String = ""
    Private _tabindex As Integer = 0
    Private _tabfavoritesname As String = "Favoriti"
    Private _enablefavorites As Boolean = True
    Private _searchonlyappname As Boolean = False

    Public Property Search() As String
        Get
            Return _search
        End Get
        Set(ByVal value As String)
            _search = value
        End Set
    End Property

    Public Property SearchOnlyApplicationName() As Boolean
        Get
            Return _searchonlyappname
        End Get
        Set(ByVal value As Boolean)
            _searchonlyappname = value
        End Set
    End Property

    Public Property TabIndex() As Integer
        Get
            Return _tabindex
        End Get
        Set(ByVal value As Integer)
            _tabindex = value
        End Set
    End Property

    Public Property AppList() As List(Of iApplication)
        Get
            Return _applist
        End Get
        Set(ByVal value As List(Of iApplication))
            _applist = value
        End Set
    End Property

    Public ReadOnly Property Tabs() As List(Of TabsItem)
        Get
            Return _tabs
        End Get
    End Property

    Public ReadOnly Property CurrentTab() As TabsItem
        Get
            Return _tabs(_tabindex)
        End Get
    End Property

    Public ReadOnly Property CurrentSubTab() As SubTabsItem
        Get
            Return _tabs(_tabindex).CurrentSubTab
        End Get
    End Property

    Public ReadOnly Property EnableFavorites() As Boolean
        Get
            Return _enablefavorites
        End Get
    End Property

    Public ReadOnly Property TabFavoritesName() As String
        Get
            Return _tabfavoritesname
        End Get
    End Property

    Public Property TabAppList() As List(Of iApplication)
        Get
            Return _tabapplist
        End Get
        Set(ByVal value As List(Of iApplication))
            _tabapplist = value
        End Set
    End Property

    Public Function GetFileName() As String
        Return _fname
    End Function

    Public Function GetTabsList(Optional ByVal ExcludeTabSystem As Boolean = False) As List(Of TabsItem)

        Dim t As New List(Of TabsItem)

        t.Add(New TabsItem("Applicazioni"))
        t(0).SubTab.Add(New SubTabsItem("Sistema", "Applicazioni", "", -1, -1, False, False, False, "", "", -1, -1))
        't.Add(New TabsItem("Utilità"))
        't(1).SubTab.Add(New SubTabsItem("Sistema", "Link", "", -1, -1, False, False, False, "", "", -1, -1))
        t.Add(New TabsItem("Link"))
        t(1).SubTab.Add(New SubTabsItem("Sistema", "Link", "", -1, -1, False, False, False, "", "", 4, 10))

        If ExcludeTabSystem = False Then
            If _enablefavorites Then
                t.Add(New TabsItem(_tabfavoritesname))
                t(t.Count - 1).SubTab.Add(New SubTabsItem("Lista", "", "", -1, -1, False, False, False, "", "", -1, -1))
            End If
            t.Add(New TabsItem("Opzioni"))
            t(t.Count - 1).SubTab.Add(New SubTabsItem("Sistema", "", "", -1, -1, True, False, False, "", "", -1, -1))
            t.Add(New TabsItem("Info"))
            t(t.Count - 1).SubTab.Add(New SubTabsItem("Info", "", "", -1, -1, True, False, False, "", "", -1, -1))
        End If

        Return t

    End Function

    Public Function GetTabsNameList(Optional ByVal ExcludeTabSystem As Boolean = False) As List(Of String)

        Dim t As List(Of TabsItem) = GetTabsList(ExcludeTabSystem)
        Dim str As New List(Of String)
        For i As Integer = 0 To t.Count - 1
            str.Add(t(i).Name)
        Next

        Return str

    End Function

    Sub SetFavorite(ByVal Favorites As List(Of String))
        _favorite = Favorites
    End Sub

    Sub ReadApplication(ByVal LoadTabList As Boolean, Optional ByVal ExcludeTabSystem As Boolean = False)

        Try
            'Compilo la lista delle tab'
            If LoadTabList Then _tabs = GetTabsList(ExcludeTabSystem)

            _applist.Clear()
            If IO.File.Exists(_fname) = True Then
                Dim ds As New DataSet
                ds.ReadXml(_fname)

                Dim exlib As Boolean = ds.Tables(0).Columns.Contains("Library")

                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim a As New iApplication
                    a.Index = i
                    a.ID = CInt(ds.Tables(0).DefaultView.Item(i).Item("ID"))
                    a.ShortCut = ds.Tables(0).DefaultView.Item(i).Item("ShortCut").ToString
                    If exlib Then a.Library = ds.Tables(0).DefaultView.Item(i).Item("library").ToString Else a.Library = ""
                    a.Directory = ds.Tables(0).DefaultView.Item(i).Item("Directory").ToString
                    a.Name = ds.Tables(0).DefaultView.Item(i).Item("Name").ToString
                    a.NameSearch = a.Name.Replace(" ", "")
                    a.Arg = ds.Tables(0).DefaultView.Item(i).Item("Arg").ToString
                    a.Path = ds.Tables(0).DefaultView.Item(i).Item("Path").ToString.Replace("#", "&")
                    a.Icon = CInt(ds.Tables(0).DefaultView.Item(i).Item("Icon"))
                    a.TabIndex = CInt(ds.Tables(0).DefaultView.Item(i).Item("Tab"))
                    a.SubTabIndex = CInt(ds.Tables(0).DefaultView.Item(i).Item("SubTab"))
                    a.Enabled = CBool(ds.Tables(0).DefaultView.Item(i).Item("Enable"))
                    a.EveryEnabled = CBool(ds.Tables(0).DefaultView.Item(i).Item("EveryEnable"))
                    a.Description = ds.Tables(0).DefaultView.Item(i).Item("Description").ToString
                    a.DescriptionSearch = a.Description.Replace(" ", "")
                    a.Key = SystemFunction.Cryptography.GetHashString(a.Index & a.Name)
                    a.Key = a.Key.Replace("\", "#")
                    a.Key = a.Key.Replace("/", "$")
                    _applist.Add(a)
                Next
            End If

        Catch ex As Exception

        End Try

    End Sub

    Sub Save()

        'Creazione file task.xml'
        Dim ds As New DataSet("AppList")
        ds.Tables.Add(New DataTable("Application"))
        ds.Tables(0).Columns.Add("Index", System.Type.GetType("System.Int32"), "")
        ds.Tables(0).Columns.Add("ID", System.Type.GetType("System.Int32"), "")
        ds.Tables(0).Columns.Add("Library", System.Type.GetType("System.String"), "")
        ds.Tables(0).Columns.Add("Directory", System.Type.GetType("System.String"), "")
        ds.Tables(0).Columns.Add("Name", System.Type.GetType("System.String"), "")
        ds.Tables(0).Columns.Add("Arg", System.Type.GetType("System.String"), "")
        ds.Tables(0).Columns.Add("Path", System.Type.GetType("System.String"), "")
        ds.Tables(0).Columns.Add("Icon", System.Type.GetType("System.Int32"), "")
        ds.Tables(0).Columns.Add("Tab", System.Type.GetType("System.Int32"), "")
        ds.Tables(0).Columns.Add("SubTab", System.Type.GetType("System.Int32"), "")
        ds.Tables(0).Columns.Add("Enable", System.Type.GetType("System.Boolean"), "")
        ds.Tables(0).Columns.Add("EveryEnable", System.Type.GetType("System.Boolean"), "")
        ds.Tables(0).Columns.Add("Description", System.Type.GetType("System.String"), "")

        For i As Integer = 0 To _applist.Count - 1
            Dim s() As String = {CStr(i), CStr(_applist(i).ID), CStr(_applist(i).Library), CStr(_applist(i).Directory), CStr(_applist(i).Name), CStr(_applist(i).Arg), CStr(_applist(i).Path), CStr(_applist(i).Icon), CStr(_applist(i).TabIndex), CStr(_applist(i).SubTabIndex), CStr(_applist(i).Enabled), CStr(_applist(i).EveryEnabled), CStr(AppList(i).Description)}
            ds.Tables(0).Rows.Add(s)
        Next

        ds.WriteXml(_fname, XmlWriteMode.WriteSchema)
        ds.Dispose()

    End Sub

    Sub Load(ByVal ShowDirectory As Boolean, ByVal SubDirectory As Boolean)
        Call Load(ShowDirectory, SubDirectory, False)
    End Sub

    Sub Load(ByVal ShowDirectory As Boolean, ByVal SubDirectory As Boolean, ByVal Favorite As Boolean)

        Try
            'Cerco le applicazioni'
            If _search Is Nothing Then _search = ""
            Dim strs As String = _search.ToLower
            _tabapplist.Clear()
            If _tabs(_tabindex).CurrentSubTab.ReadLocalFiles = False Then
                For i As Integer = 0 To _applist.Count - 1
                    _applist(i).Index = i
                    If ShowDirectory OrElse (_applist(i).ID <> 0) Then
                        If _enablefavorites AndAlso _favorite.Contains(_applist(i).Name) Then _applist(i).Favorite = True Else _applist(i).Favorite = False
                        If (_applist(i).TabIndex = _tabindex AndAlso _applist(i).SubTabIndex = _tabs(_tabindex).SubTabIndex) OrElse (Favorite AndAlso _applist(i).Favorite) Then
                            If (SubDirectory = False AndAlso _applist(i).Directory = _tabs(_tabindex).CurrentSubTab.Directory) OrElse (SubDirectory = True AndAlso _applist(i).Directory.StartsWith(_tabs(_tabindex).CurrentSubTab.Directory)) Then
                                If _search = "" OrElse _applist(i).NameSearch.ToLower.Contains(strs) OrElse _applist(i).Name.ToLower.Contains(strs) OrElse _searchonlyappname OrElse _applist(i).DescriptionSearch.ToLower.Contains(strs) OrElse _applist(i).Description.ToLower.Contains(strs) Then
                                    _tabapplist.Add(_applist(i))
                                End If
                            End If
                        End If
                    End If
                Next
                If Favorite Then
                    _tabapplist = Sort(_tabapplist, "name")
                End If
            Else
                LoadLocalFiles(True, ShowDirectory, SubDirectory)
            End If
        Catch ex As Exception

        End Try

    End Sub

    Sub LoadFavorite()
        Call Load(False, True, True)
        _tabapplist = Sort(_tabapplist, "name")
    End Sub

    Function GetApplicationNameList(ByVal ShowDirectory As Boolean, ByVal SubDirectory As Boolean, ByVal OnlyExternal As Boolean) As List(Of String)

        Dim app As New List(Of iApplication)
        Dim str As New List(Of String)

        app = GetApplicationList(ShowDirectory, SubDirectory, OnlyExternal)

        For i As Integer = 0 To app.Count - 1
            str.Add(app.Item(i).Name)
        Next

        Return str

    End Function

    Function GetApplicationList(ByVal ShowDirectory As Boolean, ByVal SubDirectory As Boolean, ByVal OnlyExternal As Boolean) As List(Of iApplication)

        Dim app As New List(Of iApplication)
        If _search Is Nothing Then _search = ""
        Dim strs As String = _search.ToLower

        For i As Integer = 0 To _applist.Count - 1
            _applist(i).Index = i
            If _tabindex = -1 OrElse _applist(i).TabIndex = _tabindex Then
                If _tabindex = -1 OrElse (SubDirectory = False AndAlso _applist(i).Directory = _tabs(_tabindex).CurrentSubTab.Directory) OrElse (SubDirectory = True AndAlso _applist(i).Directory.StartsWith(_tabs(_tabindex).CurrentSubTab.Directory)) Then
                    If ShowDirectory OrElse (_applist(i).ID <> 0) Then
                        If _search = "" OrElse _applist(i).NameSearch.ToLower.Contains(strs) OrElse _applist(i).Name.ToLower.Contains(strs) OrElse _applist(i).DescriptionSearch.ToLower.Contains(strs) OrElse _applist(i).Description.ToLower.Contains(strs) Then
                            If OnlyExternal = False OrElse (_applist(i).Path <> "") OrElse (_applist(i).Library <> "") Then
                                app.Add(_applist(i))
                            End If
                        End If
                    End If
                End If
            End If
        Next
        If _search = "" OrElse "monet".Contains(strs) Then
            app.Add(New iApplication(app.Count, 1000, "", 0, 0, "", "", "ifanta", "", "ifanta.exe", 1000, True, True))
        End If

        Return app

    End Function

    Sub LoadLocalFiles(ByVal CompilePage As Boolean, ByVal ShowDirectory As Boolean, ByVal SubDirectory As Boolean)

        Dim app As New List(Of iApplication)
        Dim count As Integer = 0

        'Determino la lista delle macro e delle sottocartelle'
        app = GetLocalFilesList(ShowDirectory, SubDirectory)
        app = Sort(app, "id,name")
        For i As Integer = 0 To app.Count - 1
            _tabapplist.Add(app(i))
        Next

    End Sub

    Function GetLocalFilesList(ByVal ShowDirectory As Boolean, ByVal SubDirectory As Boolean) As List(Of iApplication)

        Dim app As New List(Of iApplication)

        Dim f As New List(Of String)

        Dim folder As String = My.Application.Info.DirectoryPath & "\" & _tabs(_tabindex).CurrentSubTab.DefaultFolder & _tabs(_tabindex).CurrentSubTab.Directory

        f = GetLocalFilesList(f, folder, ShowDirectory, SubDirectory)

        For i As Integer = 0 To f.Count - 1
            Dim a As New iApplication
            a.Index = i
            If f(i).StartsWith("[D]") Then
                a.ID = 0
                a.Name = Path.GetFileName(f(i))
                a.Icon = 80
                a.Description = "(DIR)"
            Else
                a.ID = 5
                a.Path = _tabs(_tabindex).CurrentSubTab.DefaultFolder & _tabs(_tabindex).CurrentSubTab.Directory & Path.GetFileName(f(i))
                a.Name = Path.GetFileNameWithoutExtension(f(i))
                a.Icon = 12
                a.Description = _tabs(_tabindex).CurrentSubTab.DefaultFolder & _tabs(_tabindex).CurrentSubTab.Directory & Path.GetFileName(f(i))
            End If
            a.Directory = _tabs(_tabindex).CurrentSubTab.Directory
            a.TabIndex = -1
            a.Key = SystemFunction.Cryptography.GetHashString(a.Index & a.Name)
            a.Key = a.Key.Replace("\", "#")
            a.Key = a.Key.Replace("/", "$")
            a.Enabled = True
            If ShowDirectory OrElse (_applist(i).ID <> 0) Then app.Add(a)
        Next

        Return app

    End Function

    Private Function GetLocalFilesList(ByVal FileList As List(Of String), ByVal Folder As String, ByVal ShowDirectory As Boolean, ByVal SubDirectory As Boolean) As List(Of String)

        Dim d As New List(Of String)
        Dim f As New List(Of String)

        Dim ss As String = _search.ToLower

        'Carico la lista delle sotto cartelle'
        If ShowDirectory = True OrElse SubDirectory = True Then

            d.AddRange(IO.Directory.GetDirectories(Folder))
            d.Sort()

            For i As Integer = 0 To d.Count - 1
                If ShowDirectory = True AndAlso System.Text.RegularExpressions.Regex.Match(Path.GetFileName(d(i).ToLower), ".*" & ss & ".*").Length > 0 Then
                    FileList.Add("[D]" & d(i))
                End If
            Next

        End If

        'Ricerco i file contenuti nella cartella'
        If _search Is Nothing Then _search = ""
        Dim patt() As String = _tabs(_tabindex).CurrentSubTab.PatternFiles.Split(CChar("|"))

        'For i As Integer = 0 To patt.Length - 1
        f.AddRange(IO.Directory.GetFiles(Folder, "*" & ss & "*"))
        'Next
        f.Sort()

        For i As Integer = 0 To f.Count - 1
            If f.Item(i).Contains("Backup of") = False AndAlso f.Item(i).ToLower.EndsWith("\function.tel") = False Then
                For k As Integer = 0 To patt.Length - 1
                    If f.Item(i).ToLower Like patt(k) Then
                        FileList.Add(f(i))
                        Exit For
                    End If
                Next
            End If
        Next

        'Eseguo la ricerca per le sotto cartelle'
        If SubDirectory = True Then
            For i As Integer = 0 To d.Count - 1
                GetLocalFilesList(FileList, d.Item(i), ShowDirectory, SubDirectory)
            Next
        End If

        Return FileList

    End Function

    Function GetAppIcon(ByVal AppIcon As Integer, ByVal IconSize As String) As Drawing.Image

        Dim img As New Drawing.Bitmap(My.Resources.unk48)
        If IconSize = "16x16" Then img = My.Resources.unk16
        If IconSize = "24x24" Then img = My.Resources.unk24
        If IconSize = "32x32" Then img = My.Resources.unk32

        If IO.File.Exists(My.Application.Info.DirectoryPath & "\IMG\" & IconSize & "\" & AppIcon & ".png") = True Then
            img = CType(Drawing.Image.FromFile(My.Application.Info.DirectoryPath & "\IMG\" & IconSize & "\" & AppIcon & ".png"), Drawing.Bitmap)
        End If

        Return img

    End Function

    Public Shared Function Sort(ByVal Item As List(Of iApplication), ByVal Fields As String) As List(Of iApplication)
        Dim a() As iApplication = Item.ToArray
        Dim n As New List(Of iApplication)
        Dim _sortbyproperty As New AppSorter(Fields)
        _sortbyproperty.Fields = Fields
        Array.Sort(a, _sortbyproperty)
        n.AddRange(a)
        Return n
    End Function

    Public Class AppSorter
        Implements IComparer

        Dim _fields As String = ""

        Sub New(ByVal Fields As String)
            _fields = Fields
        End Sub

        Public Property Fields() As String
            Get
                Return _fields
            End Get
            Set(ByVal value As String)
                _fields = value
            End Set
        End Property

        Public Overridable Overloads Function Compare(ByVal Item1 As Object, ByVal Item2 As Object) As Integer Implements IComparer.Compare

            Dim d1 As iApplication = CType(Item1, iApplication)
            Dim d2 As iApplication = CType(Item2, iApplication)
            Dim ris As Integer = 1
            Dim str1 As String = ""
            Dim str2 As String = ""
            Dim s() As String = _fields.Split(CChar(","))

            For i As Integer = 0 To s.Length - 1
                Select Case s(i)
                    Case "id"
                        str1 = str1 & CStr(d1.ID).PadLeft(4, CChar("0"))
                        str2 = str2 & CStr(d2.ID).PadLeft(4, CChar("0"))
                    Case "index"
                        str1 = str1 & CStr(d1.Index).PadLeft(4, CChar("0"))
                        str2 = str2 & CStr(d2.Index).PadLeft(4, CChar("0"))
                    Case "tabindex"
                        str1 = str1 & CStr(d1.TabIndex).PadLeft(4, CChar("0"))
                        str2 = str2 & CStr(d2.TabIndex).PadLeft(4, CChar("0"))
                    Case "name"
                        str1 = str1 & d1.Name.ToLower.PadRight(100, CChar(" "))
                        str2 = str2 & d2.Name.ToLower.PadRight(100, CChar(" "))
                    Case "directory"
                        str1 = str1 & d1.Directory.ToLower.PadRight(100, CChar(" "))
                        str2 = str2 & d2.Directory.ToLower.PadRight(100, CChar(" "))
                End Select
            Next

            ris = String.Compare(str1, str2)

            Return ris

        End Function

    End Class

    Public Class Tab

        Dim _property As New TabsItem
        Dim _app As iApplication

        ReadOnly Property TabProperty() As TabsItem
            Get
                Return _property
            End Get
        End Property

        Public ReadOnly Property Application() As iApplication
            Get
                Return _app
            End Get
        End Property

    End Class

    Public Class TabsItem

        Private _name As String = ""
        Private _subtabindex As Integer = 0
        Private _subtabs As New List(Of SubTabsItem)

        Sub New()

        End Sub

        Sub New(ByVal Name As String)
            _name = Name
        End Sub

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property SubTabIndex() As Integer
            Get
                Return _subtabindex
            End Get
            Set(ByVal value As Integer)
                _subtabindex = value
            End Set
        End Property

        Public Property SubTab() As List(Of SubTabsItem)
            Get
                Return _subtabs
            End Get
            Set(ByVal value As List(Of SubTabsItem))
                _subtabs = value
            End Set
        End Property

        Public Property CurrentSubTab() As SubTabsItem
            Get
                Return _subtabs(_subtabindex)
            End Get
            Set(ByVal value As SubTabsItem)
                _subtabs(_subtabindex) = value
            End Set
        End Property
    End Class


    Public Class SubTabsItem

        Private _System As Boolean = False
        Private _Editable As Boolean = False
        Private _name As String = ""
        Private _applicationtype As String = ""
        Private _filetype As New List(Of String)
        Private _defaultfoldericon As Integer = -1
        Private _defaultfileicon As Integer = -1
        Private _readlocalfiles As Boolean = False
        Private _patternfiles As String = "*.*"
        Private _directory As String = "\"
        Private _scrollindex As Integer = 0
        Private _defaultfolder As String = ""
        Private _view As Integer = -1
        Private _fontsize As Integer = -1

        Sub New()

        End Sub

        Sub New(ByVal Name As String, ByVal ApplicationType As String, ByVal FileType As String, ByVal DefaultFolderIcon As Integer, ByVal DefaultFileIcon As Integer, ByVal System As Boolean, ByVal Editable As Boolean, ByVal ReadLocalFiles As Boolean, ByVal PatternFiles As String, ByVal DefaultFolder As String, ByVal View As Integer, ByVal FontSize As Integer)
            _name = Name
            _applicationtype = ApplicationType
            _filetype.Clear()
            _filetype.AddRange(FileType.Split(CChar(",")))
            _defaultfoldericon = DefaultFolderIcon
            _defaultfileicon = DefaultFileIcon
            _System = System
            _Editable = Editable
            _readlocalfiles = ReadLocalFiles
            If PatternFiles <> "" Then _patternfiles = PatternFiles Else _patternfiles = "*.*"
            _defaultfolder = DefaultFolder
            _view = View
            _fontsize = FontSize
        End Sub

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property ApplicationType() As String
            Get
                Return _applicationtype
            End Get
            Set(ByVal value As String)
                _applicationtype = value
            End Set
        End Property

        Public Property FileType() As List(Of String)
            Get
                Return _filetype
            End Get
            Set(ByVal value As List(Of String))
                _filetype = value
            End Set
        End Property

        Public Property View() As Integer
            Get
                Return _view
            End Get
            Set(ByVal value As Integer)
                _view = value
            End Set
        End Property

        Public Property FontSize() As Integer
            Get
                Return _fontsize
            End Get
            Set(ByVal value As Integer)
                _fontsize = value
            End Set
        End Property

        Public Property DefaultFolderIcon() As Integer
            Get
                Return _defaultfoldericon
            End Get
            Set(ByVal value As Integer)
                _defaultfoldericon = value
            End Set
        End Property

        Public Property DefaultFileIcon() As Integer
            Get
                Return _defaultfileicon
            End Get
            Set(ByVal value As Integer)
                _defaultfileicon = value
            End Set
        End Property

        Public Property System() As Boolean
            Get
                Return _System
            End Get
            Set(ByVal value As Boolean)
                _System = value
            End Set
        End Property

        Public Property Editable() As Boolean
            Get
                Return _Editable
            End Get
            Set(ByVal value As Boolean)
                _Editable = value
            End Set
        End Property

        Public Property ReadLocalFiles() As Boolean
            Get
                Return _readlocalfiles
            End Get
            Set(ByVal value As Boolean)
                _readlocalfiles = value
            End Set
        End Property

        Public Property PatternFiles() As String
            Get
                Return _patternfiles
            End Get
            Set(ByVal value As String)
                _patternfiles = value
            End Set
        End Property

        Public Property DefaultFolder() As String
            Get
                Return _defaultfolder
            End Get
            Set(ByVal value As String)
                _defaultfolder = value
            End Set
        End Property

        Public Property Directory() As String
            Get
                Return _directory
            End Get
            Set(ByVal value As String)
                _directory = value
            End Set
        End Property

        Public Property ScrollIndex() As Integer
            Get
                Return _scrollindex
            End Get
            Set(ByVal value As Integer)
                _scrollindex = value
            End Set
        End Property

    End Class

    Public Class Page

        Dim _app As New List(Of iApplication)

        Public Property Application() As List(Of iApplication)
            Get
                Return _app
            End Get
            Set(ByVal value As List(Of iApplication))
                _app = value
            End Set
        End Property

    End Class
End Class

