Imports System.Net
Imports System.IO

Partial Class WebData
    Public Shared Function GetPage(ByVal Url As String, ByVal Method As String, ByVal PostData As String, Optional Encoding As String = "ISO-8859-1") As String

        Dim responseFromServer As String = ""

        Try

            ' Crea la richiesta
            Dim request As HttpWebRequest = CType(WebRequest.Create(Url), HttpWebRequest)
            request.Method = "GET"
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"

            ' Ottieni la risposta
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(Encoding))
                    responseFromServer = reader.ReadToEnd()
                End Using
            End Using

        Catch ex As Exception
            responseFromServer = ex.Message
        End Try

        Return responseFromServer

    End Function
End Class
