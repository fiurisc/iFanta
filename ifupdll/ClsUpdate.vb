Imports System.IO
Imports System.Management
Imports System.Net

Public Class UpdateList

    Public Enum eLanguage As Integer
        English = 0
        Italian = 1
    End Enum

    Public Enum stype As Integer
        NetBios = 0
        Web = 1
    End Enum

    Event ReadRemoteFile(ByVal sender As Object, ByVal FileName As String, ByVal LastWriteTime As Date, ByVal FileSize As Integer, ByVal e As System.EventArgs)

    Private _servetype As stype = stype.NetBios
    Private _PathL As String = My.Application.Info.DirectoryPath
    Private _PathR As String = "https://www.ifantacalcio.it/public/ifanta/update"
    Private _size As Integer = 0

    Private _UpdateFileList As New List(Of UpdateItem)
    Private _UpdateSystemFileList As New List(Of String)
    Private _LocalFileExeList As New List(Of String)
    Private _flagupdate As String = My.Application.Info.DirectoryPath & "\update.flag"
    Private _remotefile As String = "version.ini"
    Private _directorysystemfile As String = "configuration"
    Private _theme As String = "red"
    Private _flatstyle As Boolean = False
    Private _lang As eLanguage = eLanguage.English

    Public WriteOnly Property ServerUpdate() As String
        Set(ByVal value As String)
            _PathR = value
        End Set
    End Property

    Public ReadOnly Property Size() As Integer
        Get
            Return _size
        End Get
    End Property

    Public ReadOnly Property UpdateSystemFileList() As List(Of String)
        Get
            Return _UpdateSystemFileList
        End Get
    End Property

    Public ReadOnly Property UpdateFileList() As List(Of UpdateItem)
        Get
            Return _UpdateFileList
        End Get
    End Property

    Public Property ServerType() As stype
        Get
            Return _servetype
        End Get
        Set(ByVal value As stype)
            _servetype = value
        End Set
    End Property

    Public Property RemoteFile() As String
        Get
            Return _remotefile
        End Get
        Set(ByVal value As String)
            _remotefile = value
        End Set
    End Property

    Public Property DirectorySystemFile() As String
        Get
            Return _directorysystemfile
        End Get
        Set(ByVal value As String)
            _directorysystemfile = value
        End Set
    End Property

    Public Property Language() As eLanguage
        Get
            Return _lang
        End Get
        Set(ByVal value As eLanguage)
            _lang = value
        End Set
    End Property

    Public Property Theme() As String
        Get
            Return _theme
        End Get
        Set(ByVal value As String)
            _theme = value
        End Set
    End Property

    Public Property FlatStyle() As Boolean
        Get
            Return _flatstyle
        End Get
        Set(ByVal value As Boolean)
            _flatstyle = value
        End Set
    End Property

    Function CheckFlagUpdate() As Boolean
        Try
            Return IO.File.Exists(_flagupdate)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Sub ReadUpdate()

        If CheckApplicationRunning() = False Then

            Dim up As Boolean = True

            If _PathL = "" Then Exit Sub

            _UpdateFileList.Clear()
            _size = 0

            Try
                If IO.File.Exists(_PathL & "\" & _remotefile) Then IO.File.Delete(_PathL & "\version.ini")
                If _servetype = stype.NetBios Then
                    If File.Exists(_PathR & "\" & _remotefile) = True Then
                        File.Copy(_PathR & "\" & _remotefile, _PathL & "\version.ini", True)
                    End If
                Else
                    Dim myWebClient As New System.Net.WebClient()
                    myWebClient.Headers.Add("Cache-Control", "no-cache")
                    myWebClient.DownloadFile(_PathR & "/" & _remotefile, _PathL & "\version.ini")
                End If

                'Leggo il file'
                If File.Exists(_PathL & "\version.ini") Then

                    Dim hash As String = ""
                    Dim lines() As String = IO.File.ReadAllLines(_PathL & "\version.ini")
                    For i As Integer = 0 To lines.Length - 1

                        Try
                            Dim str() As String = lines(i).Split(Microsoft.VisualBasic.ChrW(9))
                            If str.Length > 1 Then

                                Dim u As New UpdateItem
                                u.Name = str(0)
                                'str(1) = str(1).Replace(".", ":")
                                u.Version = str(1)
                                If str.Length > 2 Then u.Size = CInt(str(2))

                                'Controllo se e' necessario aggiornare il file'
                                If IO.File.Exists(My.Application.Info.DirectoryPath & u.Name) = True Then
                                    Dim fi As New FileInfo(My.Application.Info.DirectoryPath & u.Name)
                                    Dim hs As String = GetMD5(My.Application.Info.DirectoryPath & u.Name)
                                    If fi.Name.ToLower.EndsWith(".zip") Then
                                        If IO.Directory.GetFiles(IO.Path.GetDirectoryName(My.Application.Info.DirectoryPath & u.Name)).Length < 2 AndAlso IO.Directory.GetDirectories(IO.Path.GetDirectoryName(My.Application.Info.DirectoryPath & u.Name)).Length < 2 Then
                                            up = True
                                        Else
                                            If hs <> u.Version Then up = True Else up = False
                                        End If
                                    Else
                                        If hs <> u.Version Then up = True Else up = False
                                    End If
                                Else
                                    up = True
                                End If
                                If u.Name.ToLower.EndsWith(".exe") AndAlso u.Name.ToLower.Contains("ifup.exe") = False Then _LocalFileExeList.Add(u.Name.ToLower)

                                If up = True Then
                                    If u.Name.ToLower.Contains("\" & _directorysystemfile) OrElse u.Name.ToLower = "\ifup.exe" Then
                                        _UpdateSystemFileList.Add(u.Name)
                                    Else
                                        _UpdateFileList.Add(u)
                                        _size = _size + u.Size
                                    End If
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                    Next
                End If

                'Scrivo la flag'
                If _UpdateSystemFileList.Count > 0 OrElse _UpdateFileList.Count > 0 Then
                    IO.File.WriteAllText(_flagupdate, "update")
                End If

            Catch ex As Exception

            End Try
        End If

    End Sub

    Function GetMD5(ByVal filePath As String) As String
        Dim md5string As String = ""

        Try
            Dim fr As Byte() = System.IO.File.ReadAllBytes(filePath)
            Dim md5 As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider

            md5.ComputeHash(fr)


            Dim hash As Byte() = md5.Hash
            Dim buff As System.Text.StringBuilder = New System.Text.StringBuilder
            Dim hashByte As Byte

            For Each hashByte In hash
                buff.Append(String.Format("{0:X2}", hashByte))
            Next
            md5string = buff.ToString()

            md5 = Nothing

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        Return md5string.ToUpper
    End Function

    Function CheckApplicationRunning() As Boolean

        Dim ris As Boolean = False

        Dim p As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Process")

        For Each process As ManagementObject In p.Get()

            Dim id As Integer = CInt(process("ProcessId"))
            Dim pnr As String = ""
            Dim pns As String = My.Application.Info.DirectoryPath

            If process("ExecutablePath") IsNot Nothing Then pnr = CStr(process("ExecutablePath")).ToLower.Replace("\\", "\")

            If pnr = pns Then
                ris = True
                Exit For
            End If
        Next

        Return ris

    End Function

    Public Sub UpdateSystemFile()
        Try
            'Aggiorno se necessario i file di configurazione'
            If _UpdateSystemFileList.Count > 0 Then
                For i As Integer = 0 To _UpdateSystemFileList.Count - 1
                    Call Download(_PathR & _UpdateSystemFileList(i), _PathL & _UpdateSystemFileList(i))
                Next
                If _UpdateFileList.Count = 0 Then
                    If IO.File.Exists(_flagupdate) = True Then
                        IO.File.Delete(_flagupdate)
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Sub Download(ByVal fs As String, ByVal fd As String)
        Dim myWebClient As New System.Net.WebClient()
        Try
            myWebClient.Headers.Add("Cache-Control", "no-cache")
            myWebClient.DownloadFile(fs.Replace("\", "/"), fd)
        Catch ex As Exception

        End Try
        myWebClient.Dispose()
    End Sub

    Function ShowPopup() As Windows.Forms.DialogResult
        Dim frm As New frmupdate
        frm.SetItems(Me)
        Return frm.ShowDialog
    End Function

End Class

Public Class UpdateItem

    Dim _name As String = ""
    Dim _Version As String = ""
    Dim _Size As Integer = 0

    Sub New()

    End Sub

    Sub New(ByVal Name As String, ByVal Version As String, ByVal Size As Integer)
        _name = Name
        _Version = Version
        _Size = Size
    End Sub

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Property Version() As String
        Get
            Return _Version
        End Get
        Set(ByVal value As String)
            _Version = value
        End Set
    End Property

    Public Property Size() As Integer
        Get
            Return _Size
        End Get
        Set(ByVal value As Integer)
            _Size = value
        End Set
    End Property

End Class
