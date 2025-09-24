Imports System.IO

Namespace SystemFunction
    Public Class Zip

        Public Shared Sub ZipFile(ByVal FileName As String, ByVal ZipFileName As String)
            ZipFiles(New List(Of String)() From {
            FileName
        }, ZipFileName)
        End Sub

        Public Shared Sub ZipFiles(ByVal Directory As String, ByVal ZipFileName As String)
            ZipFiles(Directory, "*.*", ZipFileName)
        End Sub

        Public Shared Sub ZipFiles(ByVal Directory As String, ByVal Pattern As String, ByVal ZipFileName As String)
            ZipFiles(System.IO.Directory.GetFiles(Directory, Pattern).ToList(), ZipFileName)
        End Sub

        Private Shared Sub ZipFiles(ByVal Files As List(Of String), ByVal ZipFileName As String)
            Dim output As FileStream = New FileStream(ZipFileName, FileMode.Create)

            Using archive As System.IO.Compression.ZipArchive = New System.IO.Compression.ZipArchive(output, System.IO.Compression.ZipArchiveMode.Create)

                For Each f As String In Files

                    If f <> ZipFileName Then
                        Dim entry As System.IO.Compression.ZipArchiveEntry = archive.CreateEntry(System.IO.Path.GetFileName(f), System.IO.Compression.CompressionLevel.Optimal)

                        Using entrystream As Stream = entry.Open()

                            Using fs As FileStream = File.Open(f, FileMode.Open)
                                Dim buffer As Byte() = New Byte(16384) {}
                                Dim bw As BinaryWriter = New BinaryWriter(entrystream)

                                Using br As BinaryReader = New BinaryReader(fs)
                                    Dim read As Integer = br.Read(buffer, 0, buffer.Length)

                                    While (read > 0)
                                        bw.Write(buffer, 0, read)
                                        read = br.Read(buffer, 0, buffer.Length)
                                    End While
                                End Using
                            End Using
                        End Using
                    End If
                Next
            End Using
        End Sub

        Public Shared Function ZipData(ByVal DataName As String, ByVal DataBytes As Byte()) As Byte()
            Return ZipData(New Dictionary(Of String, Byte())() From {
            {DataName, DataBytes}
        })
        End Function

        Public Shared Function ZipData(ByVal DataList As Dictionary(Of String, Byte())) As Byte()
            Dim output As MemoryStream = New MemoryStream()

            Using dstream As System.IO.Compression.ZipArchive = New System.IO.Compression.ZipArchive(output, System.IO.Compression.ZipArchiveMode.Create)

                For Each name As String In DataList.Keys
                    Dim entry As System.IO.Compression.ZipArchiveEntry = dstream.CreateEntry(name, System.IO.Compression.CompressionLevel.Optimal)

                    Using entrystream As Stream = entry.Open()
                        Dim bw As BinaryWriter = New BinaryWriter(entrystream)
                        bw.Write(DataList(name))
                    End Using
                Next
            End Using

            Return output.ToArray()
        End Function

        Public Shared Sub UpZipFile(ByVal ZipFileName As String, ByVal Directory As String)
            System.IO.Compression.ZipFile.ExtractToDirectory(ZipFileName, Directory)
        End Sub

        Public Shared Function UpZipData(ByVal Data As Byte()) As Dictionary(Of String, Byte())
            Dim out As Dictionary(Of String, Byte()) = New Dictionary(Of String, Byte())()
            Dim ms As MemoryStream = New MemoryStream(Data)

            Using archive As System.IO.Compression.ZipArchive = New System.IO.Compression.ZipArchive(ms)

                For Each entry As System.IO.Compression.ZipArchiveEntry In archive.Entries

                    Using reader As System.IO.StreamReader = New StreamReader(entry.Open())
                        Dim ems As MemoryStream = New MemoryStream()
                        reader.BaseStream.CopyTo(ems)
                        out.Add(entry.FullName, ems.ToArray())
                    End Using
                Next
            End Using

            Return out
        End Function

    End Class
End Namespace