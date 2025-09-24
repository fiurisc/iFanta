Imports System.Data.OleDb

Namespace SystemFunction

    Public Class DataBase

        Public Shared Function ReadFieldStringData(FieldsName As String, DataRow As DataRow, Optional defvalue As String = "") As String
            If DataRow.Table.Columns.Contains(FieldsName) Then
                If DataRow.Item(FieldsName) IsNot System.DBNull.Value Then
                    defvalue = DataRow.Item(FieldsName).ToString
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

        Public Shared Sub OpenConnection()
            Try
                'Apro la connessione verso l'archivio locale'
                Dim fname As String = FileAndDirectory.GetLegaDataDirectory() & "\data.db"
                Dim fnamezip As String = FileAndDirectory.GetLegaDataDirectory() & "\data.zip"
                If IO.File.Exists(fname) = False Then
                    If IO.File.Exists(fnamezip) Then
                        Dim fs As String = fnamezip
                        Zip.UpZipFile(fs, FileAndDirectory.GetLegaDataDirectory())
                        IO.File.Delete(fnamezip)
                    End If
                End If
                If IO.File.Exists(fname) Then
                    conn = New System.Data.SQLite.SQLiteConnection("data source=" & fname)
                    conn.Open()
                End If
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
        End Sub

        ''' <summary>Consente di eseguire una stringa Sql sul database mediante connessione SQlite</summary>
        ''' <param name="Sql">Stringa Sql</param>
        ''' <param name="cn">Connessione</param>
        Public Shared Sub ExecuteSql(ByVal Sql As String, ByVal cn As SQLite.SQLiteConnection)
            If cn.State = ConnectionState.Open Then
                If AppSett.Backup.Enable AndAlso backup.BackupState = False AndAlso AppSett.Backup.ExecuteBackupOnlyAfterModify AndAlso System.Text.RegularExpressions.Regex.Match(Sql, "UPDATE\s+|INTERT\s+|DELETE\s+").Success Then
                    backup.ExecuteBackup(True)
                End If
                Dim cmd As SQLite.SQLiteCommand = cn.CreateCommand
                cmd.CommandText = Sql
                Try
                    cmd.ExecuteNonQuery()
                Catch ex As Exception
                    Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
                    If ex.Message = "not an error" Then
                        System.Threading.Thread.Sleep(300)
                        cmd.ExecuteNonQuery()
                    Else
                        Throw ex
                    End If
                End Try
            End If
        End Sub

        ''' <summary>Consente di eseguire una stringa Sql sul database mediante connessione SQlite</summary>
        ''' <param name="Sql">Stringa Sql</param>
        ''' <param name="cn">Connessione</param>
        Public Shared Sub ExecuteSqlSuppressError(ByVal Sql As String, ByVal cn As SQLite.SQLiteConnection)
            Try
                If cn.State = ConnectionState.Open Then
                    If AppSett.Backup.Enable AndAlso backup.BackupState = False AndAlso AppSett.Backup.ExecuteBackupOnlyAfterModify AndAlso System.Text.RegularExpressions.Regex.Match(Sql, "UPDATE\s+|INTERT\s+|DELETE\s+").Success Then
                        backup.ExecuteBackup(True)
                    End If
                    Dim cmd As SQLite.SQLiteCommand = cn.CreateCommand
                    cmd.CommandText = Sql
                    cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
        End Sub

        ''' <summary>Consente di eseguire una stringa Sql sul database mediante connessione SQlite e ne restituisce il risultato mediante DataSet</summary>
        ''' <param name="Sql">Stringa Sql</param>
        ''' <param name="cn">Connessione</param>
        Public Shared Function ExecuteSqlReturnDataSet(ByVal Sql As String, ByVal cn As SQLite.SQLiteConnection) As DataSet

            Dim da As New SQLite.SQLiteDataAdapter(Sql, cn)
            Dim ds As New DataSet

            da.Fill(ds, "tabella")

            Return ds

            ds.Dispose()
            da.Dispose()

        End Function

        Public Shared Sub CompactDb()
            Try
                Call ExecuteSql("VACUUM;", conn)
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
        End Sub

    End Class

End Namespace
