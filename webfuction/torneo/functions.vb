Imports System.Data
Imports System.Data.OleDb

Namespace Torneo
    Public Class Functions

        Public Shared Sub InitPath(rootDataPath As String, rootdatabasePath As String, year As String)
            PublicVariables.Year = year
            PublicVariables.RootDataPath = rootDataPath
            PublicVariables.DataPath = rootDataPath & year & "\"
            PublicVariables.DatabaseFileName = rootdatabasePath & year & ".accdb"
        End Sub

        Public Shared Function ExecuteSqlReturnJSON(ByVal SqlString As String) As String

            Dim risultati As New List(Of Dictionary(Of String, Object))()

            Try
                Dim ds As System.Data.DataSet = ExecuteSqlReturnDataSet(SqlString)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim record As New Dictionary(Of String, Object)()
                        For c As Integer = 0 To ds.Tables(0).Columns.Count - 1
                            record.Add(ds.Tables(0).Columns(c).ColumnName, ds.Tables(0).Rows(i).Item(c))
                        Next
                        risultati.Add(record)
                    Next
                End If
            Catch ex As Exception
                Call WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return WebData.Functions.SerializzaOggetto(risultati, True)

        End Function

        Public Shared Function ApiGetRecordIdFromUpdate(table As String, lastRecordId As Integer) As Integer

            Dim query As String = "SELECT max(id) as lastid FROM " & table & " WHERE id<=" & lastRecordId & ";"
            Dim lastId As Integer = -1

            Using conn As New OleDbConnection(Functions.GetDbConnectionString())
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
                    Call WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                End Try
            End Using

            Return lastId

        End Function

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

        Public Shared Function ReadFieldStringData(FieldsName As String, DataRow As DataRow, Optional defvalue As String = "") As String
            If DataRow.Table.Columns.Contains(FieldsName) Then
                If DataRow.Item(FieldsName) IsNot System.DBNull.Value Then
                    defvalue = DataRow.Item(FieldsName).ToString
                End If
            End If
            Return defvalue
        End Function

        Public Shared Function ReadFieldBooleanData(FieldsName As String, DataRow As DataRow, Optional defvalue As Boolean = False) As Boolean
            If DataRow.Table.Columns.Contains(FieldsName) Then
                If DataRow.Item(FieldsName) IsNot System.DBNull.Value Then
                    defvalue = CBool(DataRow.Item(FieldsName))
                End If
            End If
            Return defvalue
        End Function

        Public Shared Function ReadFieldIntegerData(FieldsName As String, DataRow As DataRow, Optional defvalue As Integer = 0) As Integer
            If DataRow.Table.Columns.Contains(FieldsName) Then
                If DataRow.Item(FieldsName) IsNot System.DBNull.Value Then
                    defvalue = CInt(DataRow.Item(FieldsName))
                End If
            End If
            Return defvalue
        End Function

        Public Shared Function ReadFieldTimeData(FieldsName As String, DataRow As DataRow, Optional defvalue As Date = Nothing) As Date
            If DataRow.Table.Columns.Contains(FieldsName) Then
                If DataRow.Item(FieldsName) IsNot System.DBNull.Value Then
                    defvalue = CDate(DataRow.Item(FieldsName))
                End If
            End If
            Return defvalue
        End Function

        Public Shared Function ReadFieldStringData(obj As Object, Optional defvalue As String = "") As String
            If obj IsNot System.DBNull.Value Then
                defvalue = obj.ToString
            End If
            Return defvalue
        End Function

        Public Shared Function ReadFieldIntegerData(obj As Object, Optional defvalue As Integer = 0) As Integer
            If obj IsNot System.DBNull.Value Then
                defvalue = CInt(obj)
            End If
            Return defvalue
        End Function

        Public Shared Function ReadFieldTimeData(obj As Object, Optional defvalue As Date = Nothing) As Date
            If obj IsNot System.DBNull.Value Then
                defvalue = CDate(obj)
            End If
            Return defvalue
        End Function

        Public Shared Sub ExecuteSql(ByVal SqlString As String)
            ExecuteSql(New List(Of String) From {SqlString})
        End Sub

        Public Shared Sub ExecuteSql(ByVal SqlString As List(Of String))
            If SqlString.Count = 0 Then Exit Sub
            Using conn As New OleDbConnection(GetDbConnectionString())
                conn.Open()
                For Each s In SqlString
                    Using cmd As New OleDbCommand(s, conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Using
        End Sub

        Public Shared Function ExecuteSqlReturnDataSet(ByVal SqlString As String) As System.Data.DataSet

            Dim ds As New System.Data.DataSet

            Using conn As New OleDbConnection(GetDbConnectionString())
                conn.Open()
                Using da As New OleDbDataAdapter(SqlString, conn)
                    da.Fill(ds, "tabella")
                End Using
            End Using

            Return ds

        End Function

        Public Shared Function GetDbConnectionString() As String
            Return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & PublicVariables.DatabaseFileName & ";"
        End Function

    End Class
End Namespace

