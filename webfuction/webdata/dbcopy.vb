Imports System.Data.SQLite
Imports System.Data.OleDb

Public Class SQLiteToAccessCopier
    Public Shared Sub CopyData(sqlitePath As String, accessPath As String)
        ' Connessione a SQLite
        Dim sqliteConn As New SQLiteConnection($"Data Source={sqlitePath};Version=3;")
        sqliteConn.Open()

        ' Connessione a Access
        Dim accessConn As New OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={accessPath};Persist Security Info=False;")
        accessConn.Open()

        ' Recupera tutte le tabelle SQLite
        Dim getTablesCmd As New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", sqliteConn)
        Dim reader = getTablesCmd.ExecuteReader()

        While reader.Read()
            Dim tableName As String = reader("name").ToString()

            ' Recupera i dati dalla tabella SQLite
            Dim selectCmd As New SQLiteCommand($"SELECT * FROM [{tableName}];", sqliteConn)
            Dim sqliteDataReader = selectCmd.ExecuteReader()

            ' Prepara comando di inserimento per Access
            Dim schemaTable = sqliteDataReader.GetSchemaTable()
            Dim columnNames As New List(Of String)
            For Each row As DataRow In schemaTable.Rows
                columnNames.Add(row("ColumnName").ToString())
            Next

            Dim columnsStr = String.Join(",", columnNames)
            Dim placeholders = String.Join(",", columnNames.Select(Function(c) "?"))

            Dim insertCmd As New OleDbCommand($"INSERT INTO [{tableName}] ({columnsStr}) VALUES ({placeholders})", accessConn)

            For Each col In columnNames
                insertCmd.Parameters.Add(New OleDbParameter())
            Next

            ' Inserisce i dati
            While sqliteDataReader.Read()
                For i = 0 To columnNames.Count - 1
                    insertCmd.Parameters(i).Value = sqliteDataReader(columnNames(i))
                Next
                insertCmd.ExecuteNonQuery()
            End While

            sqliteDataReader.Close()
        End While

        reader.Close()
        accessConn.Close()
        sqliteConn.Close()

        MsgBox("Dati copiati con successo da SQLite a Access.")
    End Sub
End Class
