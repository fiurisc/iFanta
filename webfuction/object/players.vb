Partial Class WebData
    Public Class WebPlayer

        Dim _role As String = ""
        Dim _name As String = ""
        Dim _team As String = ""

        Sub New()

        End Sub

        Sub New(Name As String)
            _name = Name
        End Sub

        Sub New(Role As String, Name As String, Team As String)
            _role = Role
            _name = Name
            _team = Team
        End Sub

        Public Property Role As String
            Get
                Return _role
            End Get
            Set(value As String)
                _role = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        Public Property Team As String
            Get
                Return _team
            End Get
            Set(value As String)
                _team = value
            End Set
        End Property

    End Class

End Class