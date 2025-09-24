Imports System.IO

Public Class UpdateList

    Private _PathL As String = My.Application.Info.DirectoryPath
    Private _PathR As String = ""
    Private _size As Integer = 0

    Private _UpdateFileList As New List(Of UpdateItem)
    Private _UpdateFileConfigurationList As New List(Of String)
    Private _LocalFileExeList As New List(Of String)
    Private _LocalFileDllList As New List(Of String)
    Private _remotefile As String = "version.txt"

    Private _updateexe As Boolean = False

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

    Public ReadOnly Property UpdateApplication() As Boolean
        Get
            Return _updateexe
        End Get
    End Property

    Public ReadOnly Property UpdateFileCofigurationList() As List(Of String)
        Get
            Return _UpdateFileConfigurationList
        End Get
    End Property

    Public ReadOnly Property UpdateFileList() As List(Of UpdateItem)
        Get
            Return _UpdateFileList
        End Get
    End Property

    Sub ReadUpdate(Optional ByVal restore As Boolean = False)

        WriteOnLog("Reading update list")

        Dim up As Boolean = True

        If _PathL = "" Then Exit Sub

        _UpdateFileList.Clear()
        _size = 0

        Try

            Dim fremup As String = _PathR & "/" & _remotefile
            Dim flocup As String = _PathL & "\version.ini"

            WriteOnLog("Download update file list (" & fremup & ") in prorgess...")

            Dim myWebClient As New System.Net.WebClient()
            'Scarico il file'
            myWebClient.DownloadFile(fremup, flocup)

            WriteOnLog("Download update file list (" & fremup & ") completed!")

            'Leggo il file'

            WriteOnLog("Reading update file list (" & flocup & ") in progress...")

            If File.Exists(flocup) = True Then

                Dim hash As String = ""
                Dim lines() As String = IO.File.ReadAllLines(_PathL & "\version.ini")
                For i As Integer = 0 To lines.Length - 1
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

                        If restore = True AndAlso u.Name.ToLower.Contains(".zip") Then up = True
                        If u.Name.ToLower.EndsWith(".exe") AndAlso u.Name.ToLower.Contains("ifup.exe") = False Then _LocalFileExeList.Add(u.Name.ToLower)

                        If up = True Then
                            If u.Name.ToLower.Contains("\" & _directorysystemfile) AndAlso u.Name.ToLower.Contains("ifup.exe") = False Then
                                _UpdateFileConfigurationList.Add(u.Name)
                            Else
                                If u.Name.ToLower.Contains("ifup.exe") = False Then
                                    _UpdateFileList.Add(u)
                                    _size = _size + u.Size
                                End If
                            End If
                        End If
                    End If
                Next
                WriteOnLog("Number of file settings to update -> " & _UpdateFileConfigurationList.Count)
                WriteOnLog("Number of file to update -> " & _UpdateFileList.Count)
                File.Delete(_PathL & "\version.ini")
            Else
                WriteOnLog("Reading update file list (" & flocup & ") error -> File not exist!")
            End If

        Catch ex As Exception
            WriteOnLog("Reading update list error -> " & ex.Message)
            MsgBox(ex.Message)
        End Try

    End Sub

    Function GetMD5(ByVal filePath As String) As String
        Dim md5string As String = ""

        Try
            Dim fr As Byte() = IO.File.ReadAllBytes(filePath)
            Dim md5 As Security.Cryptography.MD5CryptoServiceProvider = New Security.Cryptography.MD5CryptoServiceProvider

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

    Sub EmptyTempCacheXml()

        WriteOnLog("Empty temp file in progess...")

        Try
            Dim d As String = My.Application.Info.DirectoryPath & "\temp"
            If IO.Directory.Exists(d) Then
                Dim f() As String = IO.Directory.GetFiles(d, "*.*")
                For i As Integer = 0 To f.Length - 1
                    IO.File.Delete(f(i))
                Next
            End If
            d = My.Application.Info.DirectoryPath & "\temp\image"
            If IO.Directory.Exists(d) Then
                Dim f() As String = IO.Directory.GetFiles(d, "*.*")
                For i As Integer = 0 To f.Length - 1
                    IO.File.Delete(f(i))
                Next
            End If
        Catch ex As Exception
            WriteOnLog("Empty temp file in progess error -> " & ex.Message)
        End Try

        WriteOnLog("Empty temp file completed")

    End Sub

    Sub KillApplication()

        WriteOnLog("Kill application in progress...")

        Try
            Dim sProcesses() As System.Diagnostics.Process
            Dim sProcess As System.Diagnostics.Process

            Dim process As String = ""

            sProcesses = System.Diagnostics.Process.GetProcesses()

            For Each sProcess In sProcesses
                Dim a As String = "\" & sProcess.ProcessName.ToLower & ".exe"
                If _LocalFileExeList.Contains(a) = True And a.Contains("update") = False Then
                    WriteOnLog("Kill " & sProcess.ProcessName)
                    sProcess.Kill()
                    System.Threading.Thread.Sleep(100)
                End If
            Next

            WriteOnLog("Kill application finish")

        Catch ex As Exception
            WriteOnLog("Kill application error -> " & ex.Message)
        End Try

    End Sub

    Public Sub KillApplication(ByVal AppName As String)

        Try

            Dim sProcesses() As System.Diagnostics.Process
            Dim sProcess As System.Diagnostics.Process
            Dim process As String = ""

            process = AppName.ToLower.Replace(".exe", "")

            sProcesses = System.Diagnostics.Process.GetProcesses()

            For Each sProcess In sProcesses
                Dim a As String = sProcess.ProcessName.ToLower
                If sProcess.ProcessName.ToLower = process Then
                    sProcess.Kill()
                End If
            Next

        Catch ex As Exception
            WriteOnLog("Kill application " & AppName & " error -> " & ex.Message)
        End Try

    End Sub

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

