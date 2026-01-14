Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Reflection

Namespace Torneo
    Public Class Functions

        Public Shared Function ExecuteSqlReturnJSON(appSett As PublicVariables, ByVal SqlString As String, Optional DbUser As Boolean = False) As String

            Dim risultati As New List(Of Dictionary(Of String, Object))()

            Try
                Dim ds As System.Data.DataSet = ExecuteSqlReturnDataSet(appSett, SqlString, DbUser)

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
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return WebData.Functions.SerializzaOggetto(risultati, True)

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
            If obj IsNot System.DBNull.Value AndAlso obj.ToString() <> "" Then
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

        Public Shared Sub ExecuteSql(appSett As PublicVariables, ByVal SqlString As String, Optional DbUser As Boolean = False)
            ExecuteSql(appSett, New List(Of String) From {SqlString}, DbUser)
        End Sub

        Public Shared Sub CloneTableStructure(appSett As PublicVariables, sourceTable As String, targetTable As String, Optional DbUser As Boolean = False)
            Using cn As New OleDbConnection(GetDbConnectionString(appSett, DbUser))
                cn.Open()
                ' Elimina la destinazione se esiste già (facoltativo)
                Using dropCmd As New OleDbCommand($"DROP TABLE [{targetTable}]", cn)
                    Try : dropCmd.ExecuteNonQuery() : Catch : End Try
                End Using

                Dim sql As String = $"SELECT * INTO [{targetTable}] FROM [{sourceTable}] WHERE 1=0;"
                Using cmd As New OleDbCommand(sql, cn)
                    Try : cmd.ExecuteNonQuery() : Catch : End Try
                End Using
            End Using
        End Sub

        Public Shared Sub ExecuteSql(appSett As PublicVariables, ByVal SqlString As List(Of String), Optional DbUser As Boolean = False)
            If SqlString.Count = 0 Then Exit Sub
            Using conn As New OleDbConnection(GetDbConnectionString(appSett, DbUser))
                conn.Open()
                For Each s In SqlString
                    Using cmd As New OleDbCommand(s, conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Using
        End Sub

        Public Shared Function ExecuteSqlReturnDataSet(appSett As PublicVariables, ByVal SqlString As String, Optional DbUser As Boolean = False) As System.Data.DataSet

            Dim ds As New System.Data.DataSet

            'Debug.WriteLine(SqlString)
            Using conn As New OleDbConnection(GetDbConnectionString(appSett, DbUser))
                conn.Open()
                Using da As New OleDbDataAdapter(SqlString, conn)
                    da.Fill(ds, "tabella")
                End Using
            End Using

            Return ds

        End Function

        Public Shared Function GetDbConnectionString(appSett As PublicVariables, DbUser As Boolean) As String
            If DbUser Then
                Return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & appSett.DatabaseUser.FileName & ";"
            Else
                Return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & appSett.DatabaseTorneo.FileName & ";"
            End If
        End Function

        Public Shared Sub BackupDatabase(appSett As PublicVariables, DbObject As PublicVariables.DatabaseFile)
            Dim fbackup As String = DbObject.BackupPath & Path.GetFileNameWithoutExtension(DbObject.FileName) & "_" & Date.Now.ToString("yyyyMMdd_HHmmss") & ".accdb"
            IO.File.Copy(DbObject.FileName, fbackup, True)
            DeleteOldFiles(appSett, DbObject.BackupPath, Date.Now.AddDays(-5))
        End Sub

        Public Shared Sub DeleteOldFiles(appSett As PublicVariables, percorsoCartella As String, dataLimite As DateTime)
            Try
                Dim files As String() = Directory.GetFiles(percorsoCartella)

                For Each file In files
                    Dim info = New FileInfo(file)
                    If info.LastWriteTime < dataLimite Then
                        IO.File.Delete(file)
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

    End Class
End Namespace

