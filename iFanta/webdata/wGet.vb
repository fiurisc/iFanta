Imports System.Net
Imports System.IO

Partial Class wData
    Public Shared Function GetPage(ByVal Url As String, ByVal Method As String, ByVal PostData As String, Optional Encoding As String = "ISO-8859-1") As String

        Dim responseFromServer As String = ""

        Try

            Dim request As HttpWebRequest
            Dim noCachePolicy As New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.NoCacheNoStore)

            'Create Request
            request = CType(WebRequest.Create(Url), HttpWebRequest)
            Dim cookieContainer As CookieContainer = New CookieContainer()
            request.CookieContainer = cookieContainer
            request.KeepAlive = False
            request.CachePolicy = noCachePolicy
            'Set Credentials
            'request.ClientCertificates.Add(cert)
            request.Credentials = CredentialCache.DefaultCredentials
            'request.AllowAutoRedirect = True
            'Get Server Response
            Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            If response.IsFromCache Then
                response = response
            End If
            'Read Data Stream
            Dim dataStream As IO.Stream = response.GetResponseStream
            Dim reader As New StreamReader(dataStream, System.Text.Encoding.GetEncoding(Encoding))
            responseFromServer = reader.ReadToEnd()
            reader.Close()
            dataStream.Close()
            response.Close()

        Catch ex As Exception
            responseFromServer = ex.Message
        End Try

        Return responseFromServer

    End Function
End Class
