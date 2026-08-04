Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.IO
Imports System.Web
Imports System.Web.Script.Serialization

<WebService(Namespace:="http://ifantacalcio.it/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ScriptService()>
Public Class SavePush
    Inherits WebService

    Private ReadOnly filePath As String = HttpContext.Current.Server.MapPath("~/subscriptions.json")

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function Save(subscriptionJson As String) As String
        Dim serializer As New JavaScriptSerializer()
        Dim subscription = serializer.Deserialize(Of Dictionary(Of String, Object))(subscriptionJson)

        If Not File.Exists(filePath) Then
            File.WriteAllText(filePath, "[]")
        End If

        Dim listJson As String = File.ReadAllText(filePath)
        Dim list = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(listJson)

        ' Evita duplicati
        For Each subItem In list
            If subItem("endpoint").ToString() = subscription("endpoint").ToString() Then
                Return "{""status"":""Already saved""}"
            End If
        Next

        list.Add(subscription)

        File.WriteAllText(filePath, serializer.Serialize(list))

        Return "{""status"":""Subscription saved""}"
    End Function

End Class
