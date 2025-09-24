Public Class BackupData

    Sub New()

    End Sub

    Private _datafile As String = "data.db"

    Public Property TempDirectory As String = ""
    Public Property DataDirectory As String = ""
    Public Property BackupDirectory As String = ""
    Public Property History() As Integer = 15
    Public Property Compress() As Boolean = True
    Public Property BackupState() As Boolean = False

    Function GetBackupList() As List(Of String)
        Dim b As New List(Of String)
        Try
            If BackupDirectory <> "" AndAlso IO.Directory.Exists(BackupDirectory) Then
                Dim f() As String = IO.Directory.GetFiles(BackupDirectory)
                For i As Integer = 0 To f.Length - 1
                    b.Add(IO.Path.GetFileName(f(i)))
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        b.Reverse()
        Return b
    End Function

    Sub DeleteOldBackup()
        Try
            If BackupDirectory <> "" AndAlso IO.Directory.Exists(BackupDirectory) Then
                SystemFunction.FileAndDirectory.DeleteOldFiles(BackupDirectory, "*.*", Date.Now.AddDays(-History))
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Function Restore(ByVal FileName As String) As Boolean

        Dim ris As Boolean = True

        Try
            If Compress Then
                SystemFunction.Zip.UpZipFile(BackupDirectory & "\" & FileName, DataDirectory)
            Else
                Dim fs As String = BackupDirectory & "\" & FileName
                Dim fd As String = DataDirectory & "\" & _datafile
                IO.File.Copy(fs, fd, True)
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

        Return ris

    End Function

    Sub ExecuteBackup(ByVal Automatic As Boolean)
        Try
            If FolderExist() Then
                If Compress Then
                    Dim fs As String = DataDirectory & "\" & _datafile
                    Dim ft As String = TempDirectory & "\" & _datafile
                    Dim fz As String = BackupDirectory & "\" & Date.Now.ToString("yyyMMdd-HHmmss") & ".zip"
                    IO.File.Copy(fs, ft, True)
                    SystemFunction.Zip.ZipFile(ft, fz)
                    If IO.File.Exists(ft) Then IO.File.Delete(ft)
                Else
                    Dim fs As String = DataDirectory & "\" & _datafile
                    Dim fd As String = BackupDirectory & "\" & Date.Now.ToString("yyyMMdd-HHmmss") & IO.Path.GetExtension(_datafile).ToLower
                    IO.File.Copy(fs, fd, True)
                End If
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        If Automatic Then BackupState = True
    End Sub

    Private Function FolderExist() As Boolean
        If DataDirectory <> "" AndAlso IO.Directory.Exists(DataDirectory) AndAlso BackupDirectory <> "" AndAlso IO.Directory.Exists(BackupDirectory) Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
