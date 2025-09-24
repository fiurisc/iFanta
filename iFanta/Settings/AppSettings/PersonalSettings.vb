Partial Class AppSettings

    Public Class PersonalSettings

        Private _mail As String = ""
        Private _sendmailalsotome As Boolean = False
        Private _theme As New ThemeSettings

        Public Property Mail As String
            Get
                Return _mail
            End Get
            Set(value As String)
                _mail = value
            End Set
        End Property

        Public Property SendMailAlsoToMe As Boolean
            Get
                Return _sendmailalsotome
            End Get
            Set(value As Boolean)
                _sendmailalsotome = value
            End Set
        End Property

        Public Property Theme() As ThemeSettings
            Get
                Return _theme
            End Get
            Set(ByVal value As ThemeSettings)
                _theme = value
            End Set
        End Property

    End Class
End Class