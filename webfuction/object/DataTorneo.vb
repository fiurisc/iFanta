Imports System.Data.OleDb
Imports System.Reflection
Imports System.Web.Script.Serialization

Public Class DataTorneo

    Public Shared DataOnDb As Boolean = True

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
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
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
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
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
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function

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
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function


    Public Shared Function GetTeamTorneo(ServerPath As String, year As String) As String

        Dim strdata As New System.Text.StringBuilder

        Try

            Dim fname As String = ServerPath & "\web\" & CStr(year) & "\torneo\team.txt"
            Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
            For Each line As String In lines
                strdata.AppendLine(line)
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
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
                    strdata.AppendLine(ConvertListStringToString(s.ToList(), "|"))
                End If
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return strdata.ToString()

    End Function

    Public Shared Function GetRecordIdFromUpdate(ServerPath As String, year As String, table As String, lastRecordId As Integer) As Integer

        Dim query As String = "SELECT max(id) as lastid FROM " & table & " WHERE id<=" & lastRecordId & ";"
        Dim lastId As Integer = -1

        Using conn As New OleDbConnection(GetDbConnectionString(ServerPath, year))
            Try
                conn.Open()
                Using cmd As New OleDbCommand(query, conn)
                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            lastId = CInt(reader("lastid"))
                        End While
                    End Using
                End Using
            Catch ex As Exception
                Console.WriteLine("Errore: " & ex.Message)
            End Try
        End Using

        Return lastId

    End Function

    Public Shared Sub UpdateMatchData(ServerPath As String, year As String, newdata As SortedDictionary(Of Integer, SortedDictionary(Of Integer, WebData.MatchData.Match)))
        Try
            Dim ds As System.Data.DataSet = ExecuteSqlReturnDataSet(ServerPath, year, "SELECT * FROM tbmatch")
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
                        ElseIf GetCustomHashCode(olddata(g)(mi)) <> GetCustomHashCode(newdata(g)(mi)) Then
                            sqlupdate.Add("UPDATE tbmatch SET teama='" & newdata(g)(mi).TeamA & "',teamb='" & newdata(g)(mi).TeamB & "',timem='" & newdata(g)(mi).Time.ToString("yyyy/MM/dd HH:mm:ss") & "',goala='" & newdata(g)(mi).GoalA & "',goalb='" & newdata(g)(mi).GoalB & "' WHERE gio=" & g & " AND idmatch=" & mi)
                        End If
                    Next
                Next
                ExecuteSql(ServerPath, year, sqlinsert)
                ExecuteSql(ServerPath, year, sqlupdate)

            End If
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Public Shared Sub ExecuteSql(ServerPath As String, year As String, ByVal SqlString As String)
        ExecuteSql(ServerPath, year, New List(Of String) From {SqlString})
    End Sub

    Public Shared Sub ExecuteSql(ServerPath As String, year As String, ByVal SqlString As List(Of String))
        If SqlString.Count = 0 Then Exit Sub
        Using conn As New OleDbConnection(GetDbConnectionString(ServerPath, year))
            conn.Open()
            For Each s In SqlString
                Using cmd As New OleDbCommand(s, conn)
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
    End Sub

    Public Shared Function ExecuteSqlReturnDataSet(ServerPath As String, year As String, ByVal SqlString As String) As System.Data.DataSet

        Dim ds As New System.Data.DataSet

        Using conn As New OleDbConnection(GetDbConnectionString(ServerPath, year))
            conn.Open()
            Using da As New OleDbDataAdapter(SqlString, conn)
                da.Fill(ds, "tabella")
            End Using
        End Using

        Return ds

    End Function

    Private Shared Function GetDbConnectionString(ServerPath As String, year As String)
        Return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & ServerPath & "\" & year & ".accdb;"
    End Function

    Public Shared Sub WriteError(ByVal Form As String, ByVal SubName As String, ByVal ErrMsg As String)
        Try
            If IO.Directory.Exists(dirs) Then IO.File.AppendAllText(dirs & "\error.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & Form & "|" & SubName & "|" & ErrMsg & System.Environment.NewLine)
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Function ConvertListStringToString(List As List(Of String), Separator As String) As String
        Dim str As String = ""
        For i As Integer = 0 To List.Count - 1
            str = str & Separator & List(i)
        Next
        If str.Length > 0 Then
            Return str.Substring(1)
        Else
            Return ""
        End If
    End Function

    Public Shared Function GetCustomHashCode(obj As Object) As Long

        If obj Is Nothing Then Return 0

        Dim hash As Long = 17 ' Usa Long per evitare overflow immediato
        Dim props As PropertyInfo() = obj.GetType().GetProperties(BindingFlags.Public Or BindingFlags.Instance)

        For Each prop In props
            Dim value = prop.GetValue(obj, Nothing)
            If value IsNot Nothing Then
                hash = hash * 23 + value.GetHashCode()
            End If
        Next

        ' Riduci il valore a Integer in modo sicuro
        Return Math.Abs(CLng(hash Mod Long.MaxValue))

    End Function

End Class
