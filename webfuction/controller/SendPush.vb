Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.IO
Imports System.Text
Imports System.Web
Imports WebPush
Imports System.Web.Script.Serialization

<WebService(Namespace:="http://ifantacalcio.it/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ScriptService()>
Public Class SendPush
    Inherits WebService

    Private ReadOnly filePath As String = HttpContext.Current.Server.MapPath("~/subscriptions.json")

    Private ReadOnly publicKey As String = "BFOTqOPj-vdfcnFceR47mcRtl538cNNStxPDXu-u5rNtBPDDFx4XFiEeZTW9Jqnz37yj_C3ayExaB7wb3c8G10A"
    Private ReadOnly privateKey As String = "GxTPbdeS8_ovE61e8OHDgzR0Mqu5pZN3bMcaLgzr-bM"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function Send(payloadJson As String) As String
        Try
            If Not File.Exists(filePath) Then
                Return "{""error"":""Nessuna subscription salvata""}"
            End If

            Dim subscriptionsJson As String = File.ReadAllText(filePath)
            Dim serializer As New JavaScriptSerializer()
            Dim subscriptions = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(subscriptionsJson)

            Dim vapidDetails As New VapidDetails("mailto:fernando.iurisci@gmail.com", publicKey, privateKey)
            Dim webPushClient As New WebPushClient()

            For Each subItem In subscriptions
                Dim endpoint = subItem("endpoint").ToString()
                Dim keys = CType(subItem("keys"), Dictionary(Of String, Object))
                Dim p256dh = keys("p256dh").ToString()
                Dim auth = keys("auth").ToString()

                Dim subscription As New PushSubscription(endpoint, p256dh, auth)

                Try
                    webPushClient.SendNotification(subscription, payloadJson, vapidDetails)
                Catch ex As Exception
                    ' Ignora errori singoli
                End Try
            Next

            Return "{""status"":""Notifiche inviate""}"

        Catch ex As Exception
            Return "{""error"":""" & ex.Message & """}"
        End Try
    End Function

End Class
