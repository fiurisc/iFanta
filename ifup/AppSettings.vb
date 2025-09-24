Imports System.IO
Imports System.Text.RegularExpressions

Public Class GeneralSettings

    Implements ICloneable

    Private Shared myfname As String = My.Application.Info.DirectoryPath & "\system\my.txt"
    Private Shared sysfname As String = My.Application.Info.DirectoryPath & "\system\general.txt"

    Public Function Clone() As Object Implements ICloneable.Clone
        Return Me.MemberwiseClone()
    End Function

    Public Property DefaultApplication() As String = ""
    Public Property Update() As String = "https://www.ifantacalcio.it/public/ifanta/update"
    Public Property Theme() As ThemeSettings = New ThemeSettings
    Public Property FlatStyle() As Boolean = False

    Shared Function GetMySettingsFileName() As String
        Return myfname
    End Function

    Shared Function GetSysSettingsFileName() As String
        Return sysfname
    End Function

    Sub ReadSettings()

        If File.Exists(myfname) = False Then Exit Sub

        Try

            Dim lines() As String = IO.File.ReadAllLines(myfname)

            For i As Integer = 0 To lines.Length - 1

                Dim line As String = lines(i)

                Dim para As String = Regex.Match(line, ".+(?=\= ')").Value.Trim
                Dim value As String = Regex.Match(line, "(?<= ').+(?=')").Value
                If para <> "" AndAlso value <> "" Then
                    Try
                        Select Case para
                            Case "Server update" : _update = value
                            Case "Theme" : _theme.Name = value
                            Case "FlatStyle" : _FlatStyle = CBool(value)
                        End Select
                    Catch ex As Exception

                    End Try
                End If
            Next

        Catch ex As Exception

        End Try

    End Sub

    Sub ReadSystemSettings()

        WriteOnLog("Reading settings file (" & sysfname & ")")

        If IO.File.Exists(sysfname) Then
            Try

                Dim txtsett As String = ""
                Dim sett As New List(Of String)
                If IO.File.Exists(myfname) Then
                    sett.AddRange(IO.File.ReadAllLines(myfname))
                    txtsett = IO.File.ReadAllText(myfname)
                End If

                Dim lines() As String = IO.File.ReadAllLines(sysfname)

                For i As Integer = 0 To lines.Length - 1

                    Dim line As String = lines(i)

                    Dim type As String = Regex.Match(line, "(?<=\[).+(?=\])").Value.ToLower
                    Dim para As String = Regex.Match(line, "(?<=\]\s+).+(?=\= ')").Value.Trim
                    Dim value As String = Regex.Match(line, "(?<= ').+(?=')").Value

                    If type <> "" AndAlso para <> "" AndAlso value <> "" Then
                        Try
                            Select Case type
                                Case "sys"
                                    Select Case para
                                        Case "Default application" : DefaultApplication = value
                                        Case "Server update" : _update = value
                                    End Select
                            End Select

                        Catch ex As Exception

                        End Try
                    End If
                Next

                'Aggiorno il file con le impostazioni personali'
                WriteOnLog("Saving personal settings file (" & myfname & ")")
                IO.File.WriteAllLines(myfname, sett.ToArray)

            Catch ex As Exception
                WriteOnLog("Reading settings file (" & sysfname & ") error -> " & ex.Message)
            End Try
        Else
            WriteOnLog("Reading settings file (" & sysfname & ") -> not found")
        End If

    End Sub

    Public Class ThemeSettings

        Dim _name As String = "red"
        Dim mtx(23, 1) As Color
        Dim _cbox As Integer = 0

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Item(ByVal Index As Integer, ByVal Color As Integer) As Color
            Get
                Return mtx(Index, Color)
            End Get
            Set(ByVal value As Color)
                mtx(Index, Color) = value
            End Set
        End Property

        Public Property ControlBoxStyle() As Integer
            Get
                Return _cbox
            End Get
            Set(ByVal value As Integer)
                _cbox = value
            End Set
        End Property

        Sub ReadSettings()
            Call ReadSettings(_name)
        End Sub

        Sub ReadSettings(ByVal Theme As String)

            'Carico i valori di default'

            For i As Integer = 0 To 23
                mtx(i, 0) = Color.FromArgb(250, 58, 37)
                mtx(i, 1) = Color.FromArgb(250, 58, 37)
            Next

            If File.Exists(My.Application.Info.DirectoryPath & "\THEME\" & Theme & "\theme.ini") = True Then

                Dim sr As StreamReader
                Dim para() As String

                Try

                    sr = New StreamReader(My.Application.Info.DirectoryPath & "\THEME\" & Theme & "\theme.ini")

                    Do Until sr.Peek = -1

                        para = sr.ReadLine().Split(CChar(","))

                        If para(0) = "CB" Then
                            _cbox = CInt(para(1))
                        Else
                            If para.Length > 5 Then
                                mtx(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                mtx(CInt(para(0)), 1) = Color.FromArgb(CInt(para(4)), CInt(para(5)), CInt(para(6)))
                            Else
                                mtx(CInt(para(0)), 0) = Color.FromArgb(CInt(para(1)), CInt(para(2)), CInt(para(3)))
                                mtx(CInt(para(0)), 1) = mtx(CInt(para(0)), 0)
                            End If
                        End If
                    Loop

                    sr.Close()
                    sr.Dispose()

                Catch ex As Exception

                End Try
            End If

        End Sub

        Sub SaveSettings()
            Call SaveSettings(_name)
        End Sub

        Sub SaveSettings(ByVal Theme As String)

            Dim sw As New StreamWriter(My.Application.Info.DirectoryPath & "\THEME\" & Theme & "\theme.ini", False)

            For i As Integer = 0 To 23
                If mtx(i, 0) = mtx(i, 1) Then
                    sw.WriteLine(i & "," & mtx(i, 0).R & "," & mtx(i, 0).G & "," & mtx(i, 0).B)
                Else
                    sw.WriteLine(i & "," & mtx(i, 0).R & "," & mtx(i, 0).G & "," & mtx(i, 0).B & "," & mtx(i, 1).R & "," & mtx(i, 1).G & "," & mtx(i, 1).B)
                End If
            Next

            sw.WriteLine("CB," & _cbox)

            sw.Close()
            sw.Dispose()

        End Sub

    End Class

End Class