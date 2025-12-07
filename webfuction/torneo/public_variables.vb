Namespace Torneo
    Public Class PublicVariables

        Public Property Nome() As String = "TORNEO"
        Public Property Year As String = ""
        Public Property DatabaseTorneo As DatabaseFile = New DatabaseFile()
        Public Property DatabaseUser As DatabaseFile = New DatabaseFile()
        Public Property RootTorneiPath As String = ""
        Public Property TorneoPath As String = ""
        Public Property RootWebDataPath As String = ""
        Public Property WebDataPath As String = ""
        Public Property SettingsLoaded As Boolean = False
        Public Property Settings As New TorneoSettings
        Public Property Accounts As New List(Of General.Account)

        Public Sub InitPath(rootDataPath As String, rootDatabasePath As String, torneo As String, year As String)

            RootTorneiPath = rootDataPath & "tornei\"
            RootWebDataPath = rootDataPath & "webdata\"

            DatabaseUser.FolderPath = rootDatabasePath
            DatabaseUser.BackupPath = DatabaseUser.FolderPath & "backup\"
            DatabaseUser.FileName = DatabaseUser.FolderPath & "users.accdb"

            Me.Year = year
            Me.Nome = torneo

            If torneo <> "" AndAlso year <> "" Then

                TorneoPath = RootTorneiPath & torneo & "\" & year & "\"
                WebDataPath = rootDataPath & "webdata\" & year & "\"

                DatabaseTorneo.FolderPath = rootDatabasePath & torneo & "\" & year & "\"
                DatabaseTorneo.BackupPath = DatabaseTorneo.FolderPath & "backup\"
                DatabaseTorneo.FileName = DatabaseTorneo.FolderPath & "data.accdb"

                MakeDirectory(True)
            Else
                MakeDirectory(False)
            End If

        End Sub

        Private Sub MakeDirectory(all As Boolean)

            If WebDataPath <> "" Then

                Dim dirt As String = WebDataPath & "temp"
                Dim dird As String = WebDataPath & "data"
                Dim dirdpf As String = WebDataPath & "data\pforma"
                Dim dirdmt As String = WebDataPath & "data\matchs"

                If Me.Nome <> "" AndAlso IO.Directory.Exists(TorneoPath) = False Then IO.Directory.CreateDirectory(TorneoPath)
                If IO.Directory.Exists(WebDataPath) = False Then IO.Directory.CreateDirectory(WebDataPath)
                If IO.Directory.Exists(DatabaseUser.FolderPath) = False Then IO.Directory.CreateDirectory(DatabaseUser.FolderPath)
                If IO.Directory.Exists(DatabaseUser.BackupPath) = False Then IO.Directory.CreateDirectory(DatabaseUser.BackupPath)

                If all Then
                    If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
                    If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)
                    If IO.Directory.Exists(dirdpf) = False Then IO.Directory.CreateDirectory(dirdpf)
                    If IO.Directory.Exists(dirdmt) = False Then IO.Directory.CreateDirectory(dirdmt)
                    If Me.Nome <> "" AndAlso IO.Directory.Exists(DatabaseTorneo.FolderPath) = False Then IO.Directory.CreateDirectory(DatabaseTorneo.FolderPath)
                    If Me.Nome <> "" AndAlso IO.Directory.Exists(DatabaseTorneo.BackupPath) = False Then IO.Directory.CreateDirectory(DatabaseTorneo.BackupPath)
                End If

            End If

        End Sub

        Public Class DatabaseFile
            Public Property FileName As String = ""
            Public Property FolderPath As String = ""
            Public Property BackupPath As String = ""
        End Class
    End Class
End Namespace
