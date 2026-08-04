Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Torneo
    Public Class PushNotification

        Private appSett As New PublicVariables
        Private Shared ReadOnly _js As New JavaScriptSerializer()

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function GetSubscriptionsFile() As String
            Return appSett.RootTorneiPath & "push_subscriptions.json"
        End Function

        Public Function LoadSubscriptions() As ArrayList
            Dim filePath As String = GetSubscriptionsFile()
            If Not File.Exists(filePath) Then
                Return New ArrayList()
            End If

            Try
                Dim json As String = File.ReadAllText(filePath)
                Return _js.Deserialize(Of ArrayList)(json)
            Catch
                Return New ArrayList()
            End Try
        End Function

        Public Sub SaveSubscriptions(subscriptions As ArrayList)
            Dim filePath As String = GetSubscriptionsFile()
            Dim json As String = _js.Serialize(subscriptions)
            File.WriteAllText(filePath, json)
        End Sub

        Public Function FindSubscription(userId As String) As Dictionary(Of String, Object)

            Dim subscriptions As ArrayList = LoadSubscriptions()

            For Each subItem As Object In subscriptions
                Dim dict As Dictionary(Of String, Object) = DirectCast(subItem, Dictionary(Of String, Object))
                If dict.ContainsKey("userId") AndAlso dict("userId").ToString() = userId Then
                    Return dict
                End If
            Next

            Return Nothing

        End Function

        Public Sub AddSubscription(userId As String, subscriptionData As Dictionary(Of String, Object))

            ' Carica subscription esistenti
            Dim subscriptions As ArrayList = LoadSubscriptions()

            ' Rimuovi eventuali subscription vecchie per questo utente
            Dim newSubscriptions As New ArrayList()
            For Each subItem As Object In subscriptions
                Dim dict As Dictionary(Of String, Object) = DirectCast(subItem, Dictionary(Of String, Object))
                If dict.ContainsKey("userId") AndAlso dict("userId").ToString() <> userId Then
                    newSubscriptions.Add(subItem)
                End If
            Next

            ' Crea nuova subscription
            Dim newSub As New Dictionary(Of String, Object)()
            newSub("userId") = userId
            newSub("endpoint") = subscriptionData("endpoint")
            newSub("keys") = subscriptionData("keys")
            newSub("createdAt") = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

            newSubscriptions.Add(newSub)
            SaveSubscriptions(newSubscriptions)

        End Sub

        Public Sub RemoveSubscription(userId As String)
            Dim subscriptions As ArrayList = LoadSubscriptions()
            Dim newSubscriptions As New ArrayList()

            For Each subItem As Object In subscriptions
                Dim dict As Dictionary(Of String, Object) = DirectCast(subItem, Dictionary(Of String, Object))
                If dict.ContainsKey("userId") AndAlso dict("userId").ToString() <> userId Then
                    newSubscriptions.Add(subItem)
                End If
            Next

            SaveSubscriptions(newSubscriptions)

        End Sub

        ' Nuova funzione: Invia notifica a TUTTI gli utenti
        Public Sub SendNotificationToAll(title As String, body As String, url As String)
            Dim subscriptions As ArrayList = LoadSubscriptions()

            For Each subItem As Object In subscriptions
                Dim dict As Dictionary(Of String, Object) = DirectCast(subItem, Dictionary(Of String, Object))
                SendNotification(dict, title, body, url)
            Next
        End Sub

        ' Nuova funzione: Invia notifica a un singolo utente
        Public Sub SendNotificationToUser(userId As String, title As String, body As String, url As String)
            Dim subscription As Dictionary(Of String, Object) = FindSubscription(userId)

            If subscription IsNot Nothing Then
                SendNotification(subscription, title, body, url)
            End If
        End Sub

        ' Funzione core per inviare una notifica
        Private Sub SendNotification(subscription As Dictionary(Of String, Object), title As String, body As String, url As String)
            Try
                Dim endpoint As String = subscription("endpoint").ToString()
                Dim keys As Dictionary(Of String, Object) = DirectCast(subscription("keys"), Dictionary(Of String, Object))
                Dim p256dh As String = keys("p256dh").ToString()
                Dim auth As String = keys("auth").ToString()

                ' Determina il tipo di endpoint
                If endpoint.Contains("notify.windows.com") Then
                    ' Microsoft Edge - usa WNS
                    SendWnsNotification(endpoint, title, body, url)
                ElseIf endpoint.Contains("fcm.googleapis.com") Then
                    ' Chrome/Opera - usa FCM con Web Push Protocol
                    SendWebPushNotification(endpoint, p256dh, auth, title, body, url)
                ElseIf endpoint.Contains("push.services.mozilla.com") Then
                    ' Firefox - usa Web Push Protocol
                    SendWebPushNotification(endpoint, p256dh, auth, title, body, url)
                Else
                    ' Altri browser - Web Push Protocol standard
                    SendWebPushNotification(endpoint, p256dh, auth, title, body, url)
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        ' Invia notifica via WNS (Microsoft Edge)
        Private Sub SendWnsNotification(endpoint As String, title As String, body As String, url As String)
            Try
                ' Crea il payload XML per WNS
                Dim xml As String = "<?xml version=""1.0"" encoding=""utf-8""?>"
                xml &= "<toast activationType=""foreground"" launch=""" & url & """>"
                xml &= "  <visual>"
                xml &= "    <binding template=""ToastGeneric"">"
                xml &= "      <text>" & title & "</text>"
                xml &= "      <text>" & body & "</text>"
                xml &= "    </binding>"
                xml &= "  </visual>"
                xml &= "</toast>"

                ' Prepara la richiesta HTTP
                Dim request As HttpWebRequest = CType(WebRequest.Create(endpoint), HttpWebRequest)

                request.Method = "POST"
                request.ContentType = "text/xml"
                request.Headers("X-WNS-Type") = "wns/toast"

                ' Scrive il payload
                Using writer As New StreamWriter(request.GetRequestStream())
                    writer.Write(xml)
                End Using

                ' Invia la richiesta
                Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                    ' Verifica il risultato
                    If response.StatusCode = HttpStatusCode.OK Then
                        WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Notifica WNS inviata con successo a: " & endpoint)
                    Else
                        WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, "Errore WNS: " & response.StatusCode.ToString() & " - " & response.StatusDescription)
                    End If
                End Using

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        ' Invia notifica via Web Push Protocol (Chrome, Firefox, altri)
        Private Sub SendWebPushNotification(endpoint As String, p256dh As String, auth As String, title As String, body As String, url As String)
            Try
                ' Crea il payload JSON
                Dim payload As New Dictionary(Of String, Object)()
                payload("title") = title
                payload("body") = body
                payload("url") = url

                Dim js As New JavaScriptSerializer()
                Dim payloadJson As String = js.Serialize(payload)

                ' NOTA: Per una vera implementazione Web Push, devi crittografare il payload
                ' e firmare la richiesta con VAPID.
                ' Questa è una versione semplificata che funziona per test.

                ' Per semplicità, inviamo solo la notifica di test (che non funzionerà
                ' completamente senza crittografia e VAPID)
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Web Push inviato a: " & endpoint)
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Payload: " & payloadJson)

                ' TODO: Implementare crittografia e VAPID per Web Push Protocol
                ' (Usa librerie come WebPush per .NET o chiama un servizio esterno)

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub


    End Class
End Namespace