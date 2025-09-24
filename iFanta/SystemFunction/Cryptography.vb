Namespace SystemFunction

    Public Class Cryptography

        Public Shared Function EncryptData(ByVal Data As String) As String
            Dim enc As New System.Text.UnicodeEncoding
            Dim Buffer As Byte() = enc.GetBytes(Data)
            Return Convert.ToBase64String(Buffer)
        End Function

        Public Shared Function DecryptData(ByVal Data As String) As String
            Try
                Dim enc As New System.Text.UnicodeEncoding
                Dim Buffer As Byte() = Convert.FromBase64String(Data)
                Return enc.GetString(Buffer)
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
                Return ""
            End Try
        End Function

        Public Shared Function EncryptPassword(ByVal Pwd As String) As String
            If Pwd <> "" Then
                Return SystemFunction.Cryptography.EncryptData(System.Environment.UserName.ToLower & "::" & Pwd)
            Else
                Return ""
            End If
        End Function

        Public Shared Function DecryptPassword(ByVal Pwd As String) As String

            Dim str As String = SystemFunction.Cryptography.DecryptData(Pwd)
            Dim usr As String = System.Environment.UserName.ToLower & "::"
            If str.StartsWith(usr) = False OrElse str.Length < usr.Length + 1 Then
                str = ""
            Else
                str = str.Substring(usr.Length)
            End If

            Return str

        End Function

        ''' <summary>Consente di ottenere l'hash di una stringa</summary>
        ''' <param name="Text">Testo da analizzare</param>
        Public Shared Function GetHashString(ByVal Text As String) As String

            'Creo un oggetto Codifica affinchè sia possibile usare
            'il metodo GetBytes per ottenere matrici di byte
            Dim uEncode As New System.Text.UnicodeEncoding
            Dim ByteMatrix() As Byte = uEncode.GetBytes(Text)
            Dim Hash() As Byte
            Dim SHA1 As New System.Security.Cryptography.SHA1CryptoServiceProvider
            Hash = SHA1.ComputeHash(ByteMatrix)
            Return Convert.ToBase64String(Hash)

        End Function

    End Class

End Namespace