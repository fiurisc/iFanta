Namespace Torneo
    Public Class PublicVariables
        Public Property Year As String = ""
        Public Property DataFromDatabase As Boolean = False
        Public Property DatabaseTorneo As String = ""
        Public Property DatabaseUsers As String = ""
        Public Property RootTorneiPath As String = ""
        Public Property TorneoPath As String = ""
        Public Property TorneoWebDataPath As String = ""
        Public Property SettingsLoaded As Boolean = False
        Public Property Settings As New TorneoSettings
        Public Property Accounts As New List(Of General.Account)

        Public Sub InitPath(rootDataPath As String, rootDatabasePath As String, year As String)
            Me.Year = year
            RootTorneiPath = rootDataPath
            TorneoPath = rootDataPath & year & "\"
            TorneoWebDataPath = rootDataPath & year & "\webdata\"
            DatabaseTorneo = rootDatabasePath & year & ".accdb"
            DatabaseUsers = rootDatabasePath & "users.accdb"
            MakeDirectory()
        End Sub

        Private Sub MakeDirectory()

            Dim dirt As String = TorneoWebDataPath & "temp"
            Dim dird As String = TorneoWebDataPath & "data"
            Dim dirdpf As String = TorneoWebDataPath & "data\pforma"
            Dim dirdmt As String = TorneoWebDataPath & "data\matchs"

            If IO.Directory.Exists(TorneoPath) = False Then IO.Directory.CreateDirectory(TorneoPath)
            If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
            If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)
            If IO.Directory.Exists(dirdpf) = False Then IO.Directory.CreateDirectory(dirdpf)
            If IO.Directory.Exists(dirdmt) = False Then IO.Directory.CreateDirectory(dirdmt)

        End Sub

    End Class
End Namespace
