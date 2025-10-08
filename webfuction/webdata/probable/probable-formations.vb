Namespace WebData
    Public Class ProbableFormations

        Public Shared dirTemp As String = Functions.DataPath & "temp\"
        Public Shared dirData As String = Functions.DataPath & "data\pforma\"

        Shared Function GetDataFileName(site As String) As String
            Return dirData & site.ToLower() & ".json"
        End Function

        Shared Sub AddInfo(Name As String, Team As String, Site As String, State As String, Info As String, Percentage As Integer, wp As Dictionary(Of String, Torneo.ProbablePlayers.Probable.Player))
            If wp.ContainsKey(Name & "/" & Team) = False Then
                If State = "Ballottaggio" Then State = "Panchina"
                wp.Add(Name & "/" & Team, New Torneo.ProbablePlayers.Probable.Player(Name, Team, Site, State, Info, Percentage))
            Else
                If wp(Name & "/" & Team).Info <> "" Then Info = "," & Info
                If State = "Ballottaggio" Then State = wp(Name & "/" & Team).State
                wp(Name & "/" & Team).Info = wp(Name & "/" & Team).Info & Info
                wp(Name & "/" & Team).Percentage = Percentage
            End If
        End Sub

        Shared Function WriteData(Data As Torneo.ProbablePlayers.Probable, fileDestiNazione As String) As String

            Dim json As String = ""
            Try
                For Each p As String In Data.Players.Keys
                    Data.Players(p).Info = Data.Players(p).Info.Trim(","c)
                Next
                json = WebData.Functions.SerializzaOggetto(Data, False)
                IO.File.WriteAllText(fileDestiNazione, json)
            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return json

        End Function

    End Class
End Namespace