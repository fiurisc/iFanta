Imports System.Data.OleDb
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.Cryptography
Imports iFanta.SystemFunction.DataBase

Module comune

    Public legalist As New List(Of String)
    Public cntype As InternetConnection.ConnType = InternetConnection.ConnType.offline
    Public conn As New System.Data.SQLite.SQLiteConnection()
    Public tbforma As String = "tbformazioni"
    Public tbdati As String = "tbdati"
    Public tbteam As String = "tbteam"
    Public tbrose As String = "tbrose"
    Public tbplayer As String = "tbplayer"
    Public padd As Integer = 20
    Public localip As String = "-1"
    Public exemod As Boolean = False
    Public matchimgkey As New Dictionary(Of String, Bitmap)
    Public matchpreskey As New Dictionary(Of String, Bitmap)
    Public matchratingkey As New Dictionary(Of Integer, Bitmap)
    Public webdata As New wData
    Public blkrec As Integer = 300
    Public linksitedata As String = "https://www.ifantacalcio.it/public/ifanta/web/"

    Sub MakeLegaDirectoryes()
        Try
            Call MakeSystemFolder(GetTempDirectory() & "\webdata\" & currlega.Settings.Year)
            Call MakeSystemFolder(GetLegheDirectory() & "\" & currlega.Settings.Nome & "\stemmi")
            Call MakeSystemFolder(GetLegheDirectory() & "\" & currlega.Settings.Nome & "\backup")
            Call DeleteDirectory(GetLegheDirectory() & "\" & currlega.Settings.Nome & "\temp", True)
            Call DeleteFileOnDirectory(GetLegheDirectory() & "\" & currlega.Settings.Nome & "\data", "*.mdb")
            Call DeleteFileOnDirectory(GetLegheDirectory() & "\" & currlega.Settings.Nome & "\data", "*.ldb")
        Catch ex As Exception
            Call WriteError("Comune", "MakeLegaDirectoryes", ex.Message)
        End Try
    End Sub
End Module
