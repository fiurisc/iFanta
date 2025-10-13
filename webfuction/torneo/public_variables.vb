Namespace Torneo
    Public Class PublicVariables
        Public Shared Property Year As String = ""
        Public Shared Property DataFromDatabase As Boolean = False
        Public Shared Property DatabaseTorneo As String = ""
        Public Shared Property DatabaseUsers As String = ""
        Public Shared Property RootDataPath As String = ""
        Public Shared Property DataPath As String = ""
        Public Shared Property SettingsLoaded As Boolean = False
        Public Shared Property Settings As New TorneoSettings
        Public Shared Property Accounts As New List(Of General.Account)
    End Class
End Namespace
