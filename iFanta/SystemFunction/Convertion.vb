Imports System.IO

Namespace SystemFunction
    Public Class Convertion

        Public Shared Function ConvertTeamPlayerList(List As List(Of LegaObject.Team.Player)) As List(Of LegaObject.Formazione.PlayerFormazione)

            Dim nlist As New List(Of LegaObject.Formazione.PlayerFormazione)

            For i As Integer = 0 To List.Count - 1
                nlist.Add(New LegaObject.Formazione.PlayerFormazione(List(i)))
            Next

            Return nlist

        End Function

        Public Shared Function ConvertListStringToString(List As List(Of String), Separator As String) As String
            Dim str As String = ""
            For i As Integer = 0 To List.Count - 1
                str = str & "," & List(i)
            Next
            If str.Length > 0 Then
                Return str.Substring(1)
            Else
                Return ""
            End If
        End Function

        Public Shared Function ConvertListStringToString(List As List(Of String), Separator As String, QuoteChar As String) As String
            Dim str As String = ""
            For i As Integer = 0 To List.Count - 1
                str = str & "," & QuoteChar & List(i) & QuoteChar
            Next
            If str.Length > 0 Then
                Return str.Substring(1)
            Else
                Return ""
            End If
        End Function

        Public Shared Function ConvertListIntegerToString(List As List(Of Integer), Separator As String) As String
            Dim str As String = ""
            For i As Integer = 0 To List.Count - 1
                str = str & "," & List(i)
            Next
            If str.Length > 0 Then
                Return str.Substring(1)
            Else
                Return ""
            End If
        End Function

        ''' <summary>Consente di convertire un file in una stringa in formato 64'</summary>
        ''' <param name="Image">Immagine bitmap</param>
        Public Shared Function ConvertBitMapToBase64String(ByVal Image As Bitmap) As String
            Dim dest As String = ""
            Dim memory As New System.IO.MemoryStream
            Try
                Image.Save(memory, Imaging.ImageFormat.Png)
                dest = System.Convert.ToBase64String(memory.ToArray)
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
            memory.Close()
            memory = Nothing
            Return dest
        End Function

        ''' <summary>Consente di convertire un file in una stringa in formato 64'</summary>
        ''' <param name="Image">Immagine bitmap</param>
        Public Shared Function ConvertImageToBase64String(ByVal Image As Bitmap) As String
            Dim dest As String = ""
            Dim memory As New System.IO.MemoryStream
            Try
                Image.Save(memory, Imaging.ImageFormat.Png)
                dest = System.Convert.ToBase64String(memory.ToArray)
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
            memory.Close()
            memory = Nothing
            Return dest
        End Function

        ''' <summary>Consente di convertire un file in una stringa in formato 64'</summary>
        ''' <param name="SourceFileName">Nome file sorgente</param>
        Public Shared Function ConvertFileToBase64String(ByVal SourceFileName As String) As String
            Dim dest As String = ""
            Try
                If IO.File.Exists(SourceFileName) Then
                    Dim srcBT As Byte()
                    Dim sr As New IO.FileStream(SourceFileName, IO.FileMode.Open, FileAccess.Read)
                    ReDim srcBT(CInt(sr.Length))
                    Try
                        sr.Read(srcBT, 0, CInt(sr.Length))
                    Catch ex As Exception

                    End Try
                    sr.Close()
                    sr.Dispose()
                    dest = System.Convert.ToBase64String(srcBT)
                End If
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
            Return dest
        End Function

    End Class
End Namespace
