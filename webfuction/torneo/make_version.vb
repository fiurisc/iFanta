'Namespace Torneo
'    Public Class MakeVersion


'        Event ReadRemoteFile(ByVal sender As Object, ByVal FileName As String, ByVal LastWriteTime As Date, ByVal FileSize As Integer, ByVal e As System.EventArgs)

'        Private _PathL As String = System.Web.HttpContext.Current.Server.MapPath("/public/tornei")
'        Private _PathR As String = System.Web.HttpContext.Current.Server.MapPath("/public/tornei")

'        Private _UpdateFileList As New Generic.List(Of UpdateItem)
'        Private _UpdateSystemFileList As New Generic.List(Of String)
'        Private _LocalFileExeList As New Generic.List(Of String)
'        Private _remotefile As String = "version.txt"
'        Private _directorysystemfile As String = "configuration"

'        Public WriteOnly Property ServerUpdate() As String
'            Set(ByVal value As String)
'                _PathR = value
'            End Set
'        End Property

'        Public ReadOnly Property UpdateSystemFileList() As Generic.List(Of String)
'            Get
'                Return _UpdateSystemFileList
'            End Get
'        End Property

'        Public ReadOnly Property UpdateFileList() As Generic.List(Of UpdateItem)
'            Get
'                Return _UpdateFileList
'            End Get
'        End Property

'        Sub MakeFileVersion(ByVal Page As page)
'            'Elimino il precedente file "version.ini"
'            If IO.File.Exists(_PathR & "\version.txt") = True Then IO.File.Delete(_PathR & "\version.txt")
'            'Eseguo la scansione del server degli update'
'            SubFolder(Page, _PathR)
'        End Sub

'        Private Sub SubFolder(ByVal Page As page, ByVal Dir As String)

'            Call SubFiles(Page, Dir)

'            Try
'                Dim d() As String = IO.Directory.GetDirectories(Dir)
'                For i As Integer = 0 To d.Length - 1
'                    Call SubFolder(Page, d(i))
'                Next
'            Catch ex As Exception

'            End Try

'        End Sub

'        Private Sub SubFiles(ByVal Page As page, ByVal Dir As String)

'            Dim ws1 As New IO.StreamWriter(_PathR & "\version.txt", True)

'            Dim f() As String

'            Try
'                f = IO.Directory.GetFiles(Dir, "*.*")


'                For k As Integer = 0 To f.Length - 1

'                    Dim fi As New IO.FileInfo(f(k))

'                    If AvExtension(fi) = True Then
'                        Dim hs As String = GetMD5(fi.FullName).ToUpper
'                        Dim fname As String = fi.FullName.Replace(_PathR, "")
'                        ws1.WriteLine(fname & Chr(9) & hs & Chr(9) & CInt(fi.Length))
'                        Page.Response.Write(fname & Chr(9) & hs & Chr(9) & CInt(fi.Length) & "<br />")
'                    End If
'                Next
'            Catch ex As Exception

'            End Try

'            ws1.Close()
'            ws1.Dispose()

'        End Sub

'        Function GetMD5(ByVal filePath As String) As String

'            Dim md5string As String = ""

'            Try
'                Dim fr As Byte() = System.IO.File.ReadAllBytes(filePath)
'                Dim md5 As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider

'                md5.ComputeHash(fr)


'                Dim hash As Byte() = md5.Hash
'                Dim buff As System.Text.StringBuilder = New System.Text.StringBuilder
'                Dim hashByte As Byte

'                For Each hashByte In hash
'                    buff.Append(String.Format("{0:X2}", hashByte))
'                Next
'                md5string = buff.ToString()

'                md5 = Nothing

'            Catch ex As Exception
'                Console.WriteLine(ex.Message)
'            End Try

'            Return md5string.ToUpper
'        End Function

'        Private Function AvExtension(ByVal fi As IO.FileInfo) As Boolean

'            Dim val As Boolean = False
'            Dim dir As String = fi.Directory.Name

'            If fi.Extension.ToLower = ".exe" Then val = True
'            If fi.Extension.ToLower = ".dll" Then val = True
'            If fi.Extension.ToLower = ".xml" Then val = True
'            If fi.Extension.ToLower = ".txt" Then val = True
'            If fi.Extension.ToLower = ".ttf" Then val = True
'            If fi.Extension.ToLower = ".wav" Then val = True
'            If fi.Extension.ToLower = ".zip" Then val = True

'            If fi.DirectoryName.ToUpper.Contains("SYSTEM") Then val = True

'            If fi.Name.ToLower = "version.ini" Then val = False
'            If fi.Name.ToLower = "version.txt" Then val = False

'            Return val

'        End Function

'        Public Class UpdateItem

'            Dim _name As String = ""
'            Dim _Version As New Date(1970, 1, 1, 0, 0, 0)
'            Dim _Size As Integer = 0

'            Sub New()

'            End Sub

'            Sub New(ByVal Name As String, ByVal Version As Date, ByVal Size As Integer)
'                _name = Name
'                _Version = Version
'                _Size = Size
'            End Sub

'            Public Property Name() As String
'                Get
'                    Return _name
'                End Get
'                Set(ByVal value As String)
'                    _name = value
'                End Set
'            End Property

'            Public Property Version() As Date
'                Get
'                    Return _Version
'                End Get
'                Set(ByVal value As Date)
'                    _Version = value
'                End Set
'            End Property

'            Public Property Size() As Integer
'                Get
'                    Return _Size
'                End Get
'                Set(ByVal value As Integer)
'                    _Size = value
'                End Set
'            End Property

'        End Class

'    End Class
'End Namespace
