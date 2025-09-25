Imports System.Data
Imports System.Data.OleDb
Imports System.Diagnostics.Eventing
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization

Namespace Torneo
    Public Class General

        Public Shared DataOnDb As Boolean = False

        Shared Function GetYearsAct(ServerPath As String) As Integer

            Dim years As List(Of YearTorneo) = GetYearsList(ServerPath)

            For Each y As YearTorneo In years
                If y.Active Then
                    Return y.Year
                End If
            Next

            Return -1

        End Function

        Shared Function GetYearsList(ServerPath As String) As List(Of YearTorneo)

            Dim years As New List(Of YearTorneo)

            Dim d() As String = IO.Directory.GetDirectories(ServerPath & "update/tornei")

            For i As Integer = 0 To d.Length - 1

                Dim torneo As String = IO.Path.GetFileName(d(i))
                Dim line() As String = IO.File.ReadAllLines(d(i) & "/settings.txt")
                Dim act As Boolean = False
                Dim year As Integer = -1

                For k As Integer = 0 To line.Length - 1

                    Dim para As String = Regex.Match(line(k), ".+(?=\= ')").Value.Trim
                    Dim value As String = Regex.Match(line(k), "(?<= ').+(?=')").Value

                    If para = "Year" Then
                        year = CInt(value)
                    End If

                    If line(k).Contains("Active = 'True'") Then
                        act = True
                    End If
                Next

                years.Add(New YearTorneo(year, act))

            Next

            Return years

        End Function

        Public Shared Function GetMatchYear(ServerPath As String, year As String, Day As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fname As String = ServerPath & "\web\" & year & "\data\matchs\matchs-data.txt"
                Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                For Each line As String In lines
                    If Day = "-1" OrElse line.Split(Convert.ToChar("|"))(0) = Day Then
                        strdata.AppendLine(line)
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Function GetMatchDetailsYear(ServerPath As String, year As String, startDay As Integer, endDay As Integer) As String

            Dim strdata As New System.Text.StringBuilder
            Dim years As New Dictionary(Of Integer, Boolean)

            For Each y As Integer In years.Keys
                strdata.AppendLine(y)
            Next

            Try

                For i As Integer = 1 To 38
                    If (startDay = -1 OrElse startDay >= startDay) AndAlso (endDay = -1 OrElse endDay >= endDay) Then
                        Dim fname As String = ServerPath & "\web\" & year & "\data\matchs\matchs-detail-data-" & CStr(i) & ".txt"
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

        Public Shared Function GetFormazioniTorneo(ServerPath As String, year As String, gio As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\formazioni.txt"
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



        Public Shared Sub UpdateMatchData(ServerPath As String, year As String, newdata As SortedDictionary(Of Integer, SortedDictionary(Of Integer, WebData.MatchData.Match)))
            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(ServerPath, year, "SELECT * FROM tbmatch")
                Dim olddata As New SortedDictionary(Of Integer, SortedDictionary(Of Integer, WebData.MatchData.Match))
                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim g As Integer = CInt(row.Item("gio").ToString())
                        Dim mi As Integer = CInt(row.Item("idmatch").ToString())
                        If olddata.ContainsKey(g) = False Then olddata.Add(g, New SortedDictionary(Of Integer, WebData.MatchData.Match))
                        If olddata(g).ContainsKey(mi) = False Then
                            Dim m As New WebData.MatchData.Match
                            m.TeamA = row.Item("teama").ToString()
                            m.TeamB = row.Item("teamb").ToString()
                            m.Time = CDate(row.Item("timem"))
                            m.GoalA = row.Item("goala").ToString()
                            m.GoalB = row.Item("goalb").ToString()
                            olddata(g).Add(mi, m)
                        End If
                    Next

                    Dim sqlinsert As New List(Of String)
                    Dim sqlupdate As New List(Of String)

                    For Each g In newdata.Keys
                        For Each mi In newdata(g).Keys
                            If olddata.ContainsKey(g) = False OrElse olddata(g).ContainsKey(mi) = False Then
                                sqlinsert.Add("INSERT INTO tbmatch (teama,teamb,timem,goala,goalb) values ('" & newdata(g)(mi).TeamA & "','" & newdata(g)(mi).TeamB & "','" & newdata(g)(mi).Time.ToString("yyyy/MM/dd HH:mm:ss") & "','" & newdata(g)(mi).GoalA & "','" & newdata(g)(mi).GoalB & "')")
                            ElseIf WebData.Functions.GetCustomHashCode(olddata(g)(mi)) <> WebData.Functions.GetCustomHashCode(newdata(g)(mi)) Then
                                sqlupdate.Add("UPDATE tbmatch SET teama='" & newdata(g)(mi).TeamA & "',teamb='" & newdata(g)(mi).TeamB & "',timem='" & newdata(g)(mi).Time.ToString("yyyy/MM/dd HH:mm:ss") & "',goala='" & newdata(g)(mi).GoalA & "',goalb='" & newdata(g)(mi).GoalB & "' WHERE gio=" & g & " AND idmatch=" & mi)
                            End If
                        Next
                    Next
                    Functions.ExecuteSql(ServerPath, year, sqlinsert)
                    Functions.ExecuteSql(ServerPath, year, sqlupdate)

                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

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

