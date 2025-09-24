'Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.Threading

Public Class InternetConnection

#Region "Declarations"

    Private Shared TCPClient As TcpClient
    Private Shared _ip As String = "10.10.10.10"
    Private Shared _port As Integer = 23
    Private Shared _timeout As Integer = 4
    
    Public Enum ConnType
        modem = &H1
        lan = &H2
        proxy = &H4
        ras = &H10
        offline = &H20
        configured = &H40
    End Enum

    Private ConnectionStateString As String

    Private Declare Function InternetGetConnectedState Lib "wininet.dll" (ByRef _
    lpSFlags As Int32, ByVal dwReserved As Int32) As Boolean

#End Region

#Region "Control Methods"

    Public Shared Function Type() As ConnType

        If cntype = ConnType.offline Then

            Dim lngFlags As Integer

            If InternetGetConnectedState(lngFlags, 0) Then
                ' True
                If CBool(lngFlags And ConnType.lan) Then
                    cntype = ConnType.lan
                ElseIf CBool(lngFlags And ConnType.modem) Then
                    cntype = ConnType.modem
                ElseIf CBool(lngFlags And ConnType.configured) Then
                    cntype = ConnType.configured
                ElseIf CBool(lngFlags And ConnType.proxy) Then
                    cntype = ConnType.proxy
                ElseIf CBool(lngFlags And ConnType.ras) Then
                    cntype = ConnType.ras
                ElseIf CBool(lngFlags And ConnType.offline) Then
                    cntype = ConnType.offline
                Else
                    cntype = ConnType.offline
                End If
            Else
                ' False
                cntype = ConnType.offline
            End If

            If cntype = ConnType.lan Then
                If localip = "-1" Then localip = GetLocalIP()
                'If localip.StartsWith("10.") = False Then cntype = ConnType.modem
            End If

        End If

        Return cntype

    End Function

    Public Shared Function GetLocalIP() As String
        'To get local address
        Dim LocalHostName As String
        Dim ip As String = ""
        LocalHostName = Net.Dns.GetHostName()
        Dim IpAdd() As Net.IPAddress = Net.Dns.GetHostAddresses(LocalHostName)
        If IpAdd.Length > 0 Then ip = IpAdd(0).ToString
        Return ip
    End Function

    Public Shared Function CheckAvailableTpcIpConnection(ByVal Ip As String, ByVal Port As Integer) As Boolean
        _ip = Ip
        _port = Port
        _timeout = 10
        Return CheckTcp()
    End Function

    Public Shared Function CheckAvailableTpcIpConnection(ByVal Ip As String, ByVal Port As Integer, ByVal TimeOut As Integer) As Boolean
        _ip = Ip
        _port = Port
        If TimeOut > 10 Then _timeout = TimeOut Else _timeout = 10
        Return CheckTcp()
    End Function

    Private Shared Function CheckTcp() As Boolean

        Dim ris As Boolean = False

        'Dim value As Boolean = My.Computer.Network.Ping(_ip)
        Dim value As Boolean = True

        If value = True Then

            Dim Thrd As Thread
            Dim TStart As New ThreadStart(AddressOf Connect)
            Thrd = New Thread(TStart)
            Thrd.Priority = ThreadPriority.Highest
            Thrd.Start()

            System.Threading.Thread.Sleep(200)

            For i As Integer = 1 To _timeout
                If TCPClient IsNot Nothing AndAlso TCPClient.Connected = True Then Exit For
                System.Threading.Thread.Sleep(200)
            Next

            ris = TCPClient.Connected

            TCPClient.Close()
        End If

        Return ris

    End Function

    Private Shared Sub Connect()
        Try
            TCPClient = New TcpClient()
            TCPClient.Connect(_ip, _port)
        Catch ex As Exception

        End Try
    End Sub

#End Region
End Class
