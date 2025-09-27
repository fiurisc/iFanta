Imports System.Data
Imports System.Data.OleDb
Imports System.Diagnostics.Eventing
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization

Namespace Torneo
    Public Class General

        Public Shared dataFromDatabase As Boolean = True

        Shared Function GetYearsAct(databasePath As String) As String

            Dim years As List(Of YearTorneo) = GetYearsList(databasePath)

            For Each y As YearTorneo In years
                If y.Active Then
                    Return y.Year
                End If
            Next

            Return ""

        End Function

        Shared Function GetYearsList(databasePath As String) As List(Of YearTorneo)

            Dim years As New List(Of YearTorneo)

            Dim d() As String = IO.Directory.GetDirectories(databasePath & "update/tornei")

            For i As Integer = 0 To d.Length - 1

                Dim torneo As String = IO.Path.GetFileName(d(i))
                Dim line() As String = IO.File.ReadAllLines(d(i) & "/settings.txt")
                Dim act As Boolean = False
                Dim year As String = ""

                For k As Integer = 0 To line.Length - 1

                    Dim para As String = Regex.Match(line(k), ".+(?=\= ')").Value.Trim
                    Dim value As String = Regex.Match(line(k), "(?<= ').+(?=')").Value

                    If para = "Year" Then
                        year = value
                    End If

                    If line(k).Contains("Active = 'True'") Then
                        act = True
                    End If
                Next

                years.Add(New YearTorneo(year, act))

            Next

            Return years

        End Function

        Public Shared Function GetMatchDetailsYear(databasePath As String, year As String, startDay As Integer, endDay As Integer) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                For i As Integer = 1 To 38
                    If (startDay = -1 OrElse startDay >= startDay) AndAlso (endDay = -1 OrElse endDay >= endDay) Then
                        Dim fname As String = databasePath & "\web\" & year & "\data\matchs\matchs-detail-data-" & CStr(i) & ".txt"
                        If IO.File.Exists(fname) Then
                            Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                            For Each line As String In lines
                                strdata.AppendLine(line.Replace(vbCrLf, "").Replace(vbCr, ""))
                            Next
                        End If
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                Return ex.Message
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Function GetFormazioniTorneo(serverPath As String, year As String, gio As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fname As String = serverPath & "\web\" & CStr(year) & "\torneo\formazioni.txt"
                Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                For Each line As String In lines
                    If gio = "-1" OrElse line.Split(Convert.ToChar("|"))(0) = gio Then
                        strdata.AppendLine(line)
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Class YearTorneo
            Public Property Year As String = ""
            Public Property Active As Boolean = False

            Sub New()

            End Sub

            Sub New(Year As String, Active As Boolean)
                Me.Year = Year
                Me.Active = Active
            End Sub
        End Class

        Public Class LoginUser
            Public Property Username As String = ""
            Public Property Password As String = ""
            Public Property Hash As Boolean = False
        End Class
    End Class
End Namespace

