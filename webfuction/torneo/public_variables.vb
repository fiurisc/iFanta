Namespace Torneo
    Public Class PublicVariables
        Public Property Year As String = ""
        Public Property DataFromDatabase As Boolean = False
        Public Property DatabaseTorneo As String = ""
        Public Property DatabaseUsers As String = ""
        Public Property RootTorneiPath As String = ""
        Public Property TorneoPath As String = ""
        Public Property RootWebDataPath As String = ""
        Public Property WebDataPath As String = ""
        Public Property SettingsLoaded As Boolean = False
        Public Property Settings As New TorneoSettings
        Public Property Accounts As New List(Of General.Account)

        Public Sub InitPath(rootDataPath As String, rootDatabasePath As String, torneo As String, year As String)

            Me.Year = year
            RootTorneiPath = rootDataPath & "tornei\"
            RootWebDataPath = rootDataPath & "webdata\"

            If torneo <> "" AndAlso year <> "" Then
                TorneoPath = RootTorneiPath & torneo & "\" & year & "\"
                WebDataPath = rootDataPath & "webdata\" & year & "\"
                DatabaseTorneo = rootDatabasePath & year & ".accdb"
                MakeDirectory(True)
            Else
                MakeDirectory(False)
            End If
            DatabaseUsers = rootDatabasePath & "users.accdb"

        End Sub

        Private Sub MakeDirectory(all As Boolean)

            If WebDataPath <> "" Then

                Dim dirt As String = WebDataPath & "temp"
                Dim dird As String = WebDataPath & "data"
                Dim dirdpf As String = WebDataPath & "data\pforma"
                Dim dirdmt As String = WebDataPath & "data\matchs"

                If IO.Directory.Exists(TorneoPath) = False Then IO.Directory.CreateDirectory(TorneoPath)
                If IO.Directory.Exists(WebDataPath) = False Then IO.Directory.CreateDirectory(WebDataPath)

                If all Then
                    If IO.Directory.Exists(dirt) = False Then IO.Directory.CreateDirectory(dirt)
                    If IO.Directory.Exists(dird) = False Then IO.Directory.CreateDirectory(dird)
                    If IO.Directory.Exists(dirdpf) = False Then IO.Directory.CreateDirectory(dirdpf)
                    If IO.Directory.Exists(dirdmt) = False Then IO.Directory.CreateDirectory(dirdmt)
                End If

            End If

        End Sub

    End Class
End Namespace
