Namespace Torneo
    Public Class ProbablePlayer

        Public Class Player

            Sub New()

            End Sub

            Sub New(ByVal Name As String, ByVal Team As String, Site As String, ByVal State As String, ByVal Info As String, Percentage As Integer)
                _Name = Name
                _Team = Team
                _Site = Site
                _State = State
                _Info = Info
                _Percentage = Percentage
            End Sub

            Public Property Name() As String = ""
            Public Property Team() As String = ""
            Public Property Site() As String = ""
            Public Property State() As String = "sconosciuto"
            Public Property Info As String = ""
            Public Property Percentage As Integer = 0

        End Class
    End Class
End Namespace