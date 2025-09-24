Imports System.Text.RegularExpressions
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory

Public Class GeneralSettings

    Implements ICloneable

    Enum eUpdateType As Integer
        NetBios = 0
        Web = 1
    End Enum

    'System'
    Private _server As String = "http://www.ifantacalcio.it/public/ifanta"
    Private _update As String = "http://www.ifantacalcio.it/public/ifanta/update"
    Private _updatetype As eUpdateType = eUpdateType.Web
    Private _updatefileversion As String = "version.txt"
    Private _systemdirectory As String = "system"
    Private _teamrating As New Dictionary(Of String, Integer)
    Private _fname As String = GetSystemDirectory() & "\general.ini"
    Private _company As String = ""
    
    Public Function Clone() As Object Implements ICloneable.Clone
        Return Me.MemberwiseClone()
    End Function

    Sub New()
       
    End Sub

    Public Property Company() As String
        Get
            Return _company
        End Get
        Set(ByVal value As String)
            _company = value
        End Set
    End Property

    Public Property Server() As String
        Get
            Return _server
        End Get
        Set(ByVal value As String)
            _server = value
        End Set
    End Property

    Public Property Update() As String
        Get
            Return _update
        End Get
        Set(ByVal value As String)
            _update = value
        End Set
    End Property

    Public Property UpdateFileVersion() As String
        Get
            Return _updatefileversion
        End Get
        Set(ByVal value As String)
            _updatefileversion = value
        End Set
    End Property

    Public Property UpdateType() As eUpdateType
        Get
            Return _updatetype
        End Get
        Set(ByVal value As eUpdateType)
            _updatetype = value
        End Set
    End Property

    Public Property SystemDirectory() As String
        Get
            Return _systemdirectory
        End Get
        Set(ByVal value As String)
            _systemdirectory = value
        End Set
    End Property

    Public Property TeamRating() As Dictionary(Of String, Integer)
        Get
            Return _teamrating
        End Get
        Set(ByVal value As Dictionary(Of String, Integer))
            _teamrating = value
        End Set
    End Property

    Shared Function GetThemeList() As List(Of String)
        Dim d() As String = IO.Directory.GetDirectories(GetThemeDirectory)
        Array.Sort(d)
        Dim f As New List(Of String)
        f.Add("Default")
        For i As Integer = 0 To d.Length - 1
            f.Add(IO.Path.GetFileName(d(i)).ToLower)
        Next
        Return f
    End Function

    Public Sub ReadSettings()

        _teamrating.Clear()

        If IO.File.Exists(_fname) Then
            Try

                Dim lines() As String = IO.File.ReadAllLines(_fname)

                For i As Integer = 0 To lines.Length - 1

                    Dim line As String = lines(i)

                    Dim para As String = Regex.Match(line, ".+(?=\= ')").Value.Trim
                    Dim value As String = Regex.Match(line, "(?<= ').+(?=')").Value

                    If para <> "" AndAlso value <> "" Then
                        Try
                            Select Case para
                                Case "Server" : _server = value
                                Case "Server update" : _update = value
                                Case "Server update file version" : _updatefileversion = value
                                Case "Server update type"
                                    If value = "web" Then
                                        _updatetype = eUpdateType.Web
                                    Else
                                        _updatetype = eUpdateType.NetBios
                                    End If
                                Case "System directory" : _systemdirectory = value
                                Case "Team rating"
                                    Dim s() As String = value.Split(CChar("|"))
                                    For k As Integer = 0 To s.Length - 1
                                        Dim f() As String = s(k).Split(CChar(","))
                                        If f.Length = 2 Then
                                            If _teamrating.ContainsKey(f(0)) = False Then _teamrating.Add(f(0), CInt(f(1)))
                                        End If
                                    Next
                            End Select
                        Catch ex As Exception
                            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                        End Try
                    End If
                Next

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End If
    End Sub

    Sub SaveSettings()
        Try
            Dim str As New System.Text.StringBuilder
            str.Append("Server = '" & _server & "'" & System.Environment.NewLine)
            str.Append("Server update = '" & _update & "'" & System.Environment.NewLine)
            str.Append("Server update file version = '" & _updatefileversion & "'" & System.Environment.NewLine)
            If _updatetype = eUpdateType.NetBios Then
                str.Append("Server update type = 'netbios'" & System.Environment.NewLine)
            Else
                str.Append("Server update type = 'web'" & System.Environment.NewLine)
            End If
            str.Append("System directory = '" & _systemdirectory & "'" & System.Environment.NewLine)
            str.Append("Team rating = '" & GetTeamRatingString() & "'" & System.Environment.NewLine)
            IO.File.WriteAllText(_fname, str.ToString)
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Private Function GetTeamRatingString() As String

        Dim str As String = ""

        Dim s(_teamrating.Count - 1) As String
        _teamrating.Keys.CopyTo(s, 0)
        For i As Integer = 0 To s.Length - 1
            str = str & "|" & s(i) & "," & _teamrating(s(i))
        Next
        If str.Length > 0 Then str = str.Substring(1)
        Return str
    End Function

End Class