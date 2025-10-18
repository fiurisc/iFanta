Imports webfuction.Torneo

Namespace Torneo
    Public Class ClassificaSerieA
        Public Shared Function ApiGetData() As String

            WebData.Functions.WriteLog(WebData.Functions.eMessageType.Info, "Richiedo la classifica della serie a dell'anno: " & PublicVariables.Year)

            Dim fname As String = WebData.Classifica.GetDataFileName()

            If IO.File.Exists(fname) Then
                Dim json As String = IO.File.ReadAllText(fname)
                Dim dicdata As List(Of WebData.Classifica.ClassificaItem) = WebData.Functions.DeserializeJson(Of List(Of WebData.Classifica.ClassificaItem))(json)
                Return WebData.Functions.SerializzaOggetto(dicdata, True)
            Else
                Return "{}"
            End If

        End Function
    End Class
End Namespace