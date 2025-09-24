Imports System.IO

Namespace SystemFunction

    Public Class FileAndDirectory

        Public Shared Function GetThemeList() As List(Of String)
            Dim d() As String = IO.Directory.GetDirectories(GetThemeDirectory)
            Array.Sort(d)
            Dim f As New List(Of String)
            f.Add("Default")
            For i As Integer = 0 To d.Length - 1
                f.Add(IO.Path.GetFileName(d(i)).ToLower)
            Next
            Return f
        End Function

        Public Shared Function GetLegheDirectory() As String
            Return My.Application.Info.DirectoryPath & "\tornei"
        End Function

        Public Shared Function GetTempDirectory() As String
            Return My.Application.Info.DirectoryPath & "\temp"
        End Function

        Public Shared Function GetTempImageDirectory() As String
            Return My.Application.Info.DirectoryPath & "\temp\image"
        End Function

        Public Shared Function GetLegaBackupDirectory() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome & "\backup"
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetLegaDataDirectory() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome & "\data"
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetLegaDirectory() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetLegaSettingsFileName() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome & "\settings.txt"
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetMyLegaSettingsFileName() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome & "\mysettings.txt"
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetLegaCoatOfArmsLegsDirectory() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome & "\stemmi"
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetLegaExpDataDirectory() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome & "\exp"
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetLegaTemDirectory() As String
            If currlega.Settings.Nome <> "" Then
                Return GetLegheDirectory() & "\" & currlega.Settings.Nome & "\temp"
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetSystemDirectory() As String
            Return My.Application.Info.DirectoryPath & "\system"
        End Function

        Public Shared Function GetThemeDirectory() As String
            Return My.Application.Info.DirectoryPath & "\theme"
        End Function

        Public Shared Sub MakeSystemFolder(ByVal Directory As String)
            If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory
            If IO.Directory.Exists(Directory) = False Then IO.Directory.CreateDirectory(Directory)
        End Sub

        ''' <summary>Consente di eliminare i file piu' vecchi di una certa data</summary>
        ''' <param name="Directory">Directory da cui eliminare i file</param>
        ''' <param name="Pattern">Pattern per il filtraggio dei file nella directory</param>
        ''' <param name="DateReference">Data di riferiemnto per la cancellazione dei file</param>
        Public Shared Sub DeleteOldFiles(ByVal Directory As String, ByVal Pattern As String, ByVal DateReference As Date)

            If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory

            If IO.Directory.Exists(Directory) Then

                Dim f() As String = IO.Directory.GetFiles(Directory, Pattern)

                Try
                    For k As Integer = 0 To f.Length - 1
                        If IO.File.GetLastWriteTime(f(k)) < DateReference Then IO.File.Delete(f(k))
                    Next
                Catch ex As Exception
                    Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                End Try
            End If

        End Sub

        ''' <summary>Consente di eliminare i file piu' vecchi di una certa data a partire da una directory</summary>
        ''' <param name="Directory">Directory da cui eliminare i file</param>
        ''' <param name="DateReference">Data di riferiemnto per la cancellazione dei file</param>
        ''' <param name="Pattern">Pattern per il filtraggio dei file nella directory</param>
        Public Shared Sub DeleteOldFiles(ByVal Directory As String, ByVal Pattern As String, ByVal DateReference As Date, ByVal SubFolder As Boolean)

            If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory

            Dim d() As String = IO.Directory.GetDirectories(Directory, Pattern)
            Dim f() As String = IO.Directory.GetFiles(Directory, Pattern)

            Try
                For k As Integer = 0 To f.Length - 1
                    If IO.File.GetLastWriteTime(f(k)) < DateReference Then IO.File.Delete(f(k))
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Try
                If SubFolder = True Then
                    For k As Integer = 0 To d.Length - 1
                        DeleteOldFiles(d(k), Pattern, DateReference, SubFolder)
                    Next
                End If
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub

        ''' <summary>Consente di eliminare i file piu' vecchi di una certa data</summary>
        ''' <param name="Directory">Directory da cui eliminare i file</param>
        ''' <param name="Pattern">Pattern per il filtraggio dei file nella directory</param>
        Public Shared Sub DeleteFileOnDirectory(ByVal Directory As String, ByVal Pattern As String)

            If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory

            Dim f() As String = IO.Directory.GetFiles(Directory, Pattern)

            Try
                For k As Integer = 0 To f.Length - 1
                    IO.File.Delete(f(k))
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub

        ''' <summary>Consente di eliminare i file piu' vecchi di una certa data a partire da una directory</summary>
        ''' <param name="Directory">Directory da cui eliminare i file</param>
        ''' <param name="Pattern">Pattern per il filtraggio dei file nella directory</param>
        Public Shared Sub DeleteFileOnDirectory(ByVal Directory As String, ByVal Pattern As String, ByVal SubFolder As Boolean)

            If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory

            Dim d() As String = IO.Directory.GetDirectories(Directory, Pattern)
            Dim f() As String = IO.Directory.GetFiles(Directory, Pattern)

            Try
                For k As Integer = 0 To f.Length - 1
                    IO.File.Delete(f(k))
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try

            Try
                If SubFolder = True Then
                    For k As Integer = 0 To d.Length - 1
                        DeleteFileOnDirectory(d(k), Pattern, SubFolder)
                    Next
                End If
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub

        ''' <summary>Consente di eliminare i file vuoti presenti in una cartella</summary>
        ''' <param name="Directory">Directory da cui eliminare i file</param>
        ''' <param name="Pattern">Pattern per il filtraggio dei file nella directory</param>
        Public Shared Sub DeleteEmptyFileOnDirectory(ByVal Directory As String, ByVal Pattern As String)

            If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory

            Dim f() As String = IO.Directory.GetFiles(Directory, Pattern)

            Try
                For k As Integer = 0 To f.Length - 1
                    Dim fi As New FileInfo(f(k))
                    If fi.Length = 0 Then IO.File.Delete(f(k))
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub

        ''' <summary>Consente di eliminare le directory vuote</summary>
        ''' <param name="Directory">Directory da cui eliminare le directory vuove</param>
        Public Shared Sub DeleteEmptyDirectory(ByVal Directory As String)
            If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory
            Dim d() As String = IO.Directory.GetDirectories(Directory)
            Try
                For k As Integer = 0 To d.Length - 1
                    If IO.Directory.GetFileSystemEntries(d(k)).Length = 0 Then
                        IO.Directory.Delete(d(k))
                    End If
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        ''' <summary>Consente di eliminare una directory</summary>
        ''' <param name="Directory">Directory da eliminare</param>
        Public Shared Sub DeleteDirectory(ByVal Directory As String, Recursive As Boolean)
            Try
                If IO.Directory.Exists(Directory) Then
                    IO.Directory.Delete(Directory, Recursive)
                End If
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        ''' <summary>Consente di eliminare un file se presente</summary>
        ''' <param name="FileName">Path file completo</param>
        Public Shared Sub DeleteFile(ByVal FileName As String)
            Try
                If IO.File.Exists(FileName) = True Then IO.File.Delete(FileName)
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

    End Class

End Namespace
