Partial Public Class wData
    Public Class wPlayer

        Private _giornata As Integer = 0
        Private _name As String = ""
        Private _team As String = ""
        Private _titolare As Integer = 0
        Private _panchina As Integer = 0
        Private _info As New List(Of InfoSite)

        Sub New()

        End Sub

        Sub New(Giornata As Integer, ByVal Name As String, ByVal Team As String, ByVal Site As String, ByVal State As String, Percentage As Integer, ByVal Info As String)
            _giornata = Giornata
            _name = Name
            _team = Team
            Select Case State
                Case "Titolare" : _titolare = 1
                Case "Panchina" : _panchina = 1
            End Select
            _info.Add(New InfoSite(Site, State, Percentage, Info))
        End Sub

        Sub AddInfo(ByVal Site As String, ByVal State As String, Percentage As Integer, ByVal Info As String)
            Select Case State
                Case "Titolare" : _titolare += 1
                Case "Panchina" : _panchina += 1
            End Select
            _info.Add(New InfoSite(Site, State, Percentage, Info))
        End Sub

        Public Property Giornata As Integer
            Get
                Return _giornata
            End Get
            Set(value As Integer)
                _giornata = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Team() As String
            Get
                Return _team
            End Get
            Set(ByVal value As String)
                _team = value
            End Set
        End Property

        Public Property Titolare() As Integer
            Get
                Return _titolare
            End Get
            Set(ByVal value As Integer)
                _titolare = value
            End Set
        End Property

        Public Property Panchina() As Integer
            Get
                Return _panchina
            End Get
            Set(ByVal value As Integer)
                _panchina = value
            End Set
        End Property

        Public Property Info() As List(Of InfoSite)
            Get
                Return _info
            End Get
            Set(ByVal value As List(Of InfoSite))
                _info = value
            End Set
        End Property

        Public Class InfoSite

            Dim _site As String = ""
            Dim _state As String = ""
            Dim _perc As Integer = -1
            Dim _info As String = ""

            Sub New()

            End Sub

            Sub New(Site As String, State As String, Percentage As Integer, Info As String)
                _site = Site
                _state = State
                _perc = Percentage
                _info = Info
            End Sub

            Public Property Site As String
                Get
                    Return _site
                End Get
                Set(value As String)
                    _site = value
                End Set
            End Property

            Public Property State As String
                Get
                    Return _state
                End Get
                Set(value As String)
                    _state = value
                End Set
            End Property

            Public Property Percentage As Integer
                Get
                    Return _perc
                End Get
                Set(value As Integer)
                    _perc = value
                End Set
            End Property

            Public Property Info As String
                Get
                    Return _info
                End Get
                Set(value As String)
                    _info = value
                End Set
            End Property
        End Class
    End Class

End Class