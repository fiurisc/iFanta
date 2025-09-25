Imports System.Data.OleDb
Imports System.Data.SqlTypes

Namespace Torneo
    Public Class Rose

        Public Shared Function GetSvincolatiTorneo(ServerPath As String, year As String, role As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\svincolati.txt"
                Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                For Each line As String In lines
                    If role = "Tutti" OrElse line.Split(Convert.ToChar("|"))(2) = role Then
                        strdata.AppendLine(line)
                    End If
                Next
            Catch ex As Exception
                Call WebData.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Function GetTeamsTorneo(ServerPath As String, year As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                If Torneo.General.DataOnDb Then

                    'Dim teams As New List(Of Dictionary(Of Integer, Object))()

                    'Try
                    '    Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(ServerPath, year, "SELECT * FROM tbteam")

                    '    If ds.Tables.Count > 0 Then
                    '        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    '            Dim team As New Dictionary(Of String, Object)()
                    '            For c As Integer = 0 To ds.Tables(0).Columns.Count - 1
                    '                record.Add(ds.Tables(0).Columns(c).ColumnName, ds.Tables(0).Rows(i).Item(c))
                    '            Next
                    '            teams.Add(team)
                    '        Next
                    '    End If
                    'Catch ex As Exception
                    '    Call WebData.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                    'End Try

                    'Return WebData.SerializzaOggetto(risultati)

                    'Return Functions.ExecuteSqlReturnJSON(ServerPath, year, "SELECT * FROM tbteam")
                Else
                    Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\team.txt"
                    Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                    For Each line As String In lines
                        strdata.AppendLine(line)
                    Next
                End If

            Catch ex As Exception
                Call WebData.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Function GetRoseTorneo(ServerPath As String, year As String, teamId As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fquota As String = ServerPath & "\web\" & CStr(year) & "\data\players-quote.txt"
                Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\rose.txt"
                Dim quotes As New Dictionary(Of String, String)
                Dim lines As List(Of String)

                lines = IO.File.ReadAllLines(fquota).ToList()

                For Each line As String In lines
                    Dim s() As String = line.Split("|")
                    If s.Length = 5 Then
                        Dim key As String = s(0) & "|" & s(1) & "|" & s(2)
                        If quotes.ContainsKey(key) = False Then quotes.Add(key, s(4))
                    End If
                Next

                lines = IO.File.ReadAllLines(fname).ToList()

                For Each line As String In lines
                    Dim s() As String = line.Split("|")
                    If s.Length > 5 Then
                        Dim key As String = s(2) & "|" & s(4) & "|" & s(5)
                        If quotes.ContainsKey(key) Then
                            s(9) = quotes(key)
                        End If
                        strdata.AppendLine(WebData.ConvertListStringToString(s.ToList(), "|"))
                    End If
                Next
            Catch ex As Exception
                Call WebData.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Class TeamItem
            Public Property idTeam As Integer

        End Class
    End Class
End Namespace

