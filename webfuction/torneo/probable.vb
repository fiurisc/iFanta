Imports Newtonsoft
Imports System.Runtime.Remoting
Imports webfuction.Torneo.Players

Namespace Torneo
    Public Class ProbablePlayer

        Public Shared Function ApiGetProbableFormation(site As String, day As String, state As String) As String
            Dim dicData As New Dictionary(Of String, Player)
            Dim tmp As New Dictionary(Of String, Dictionary(Of String, Player))
            Dim json As String = IO.File.ReadAllText(WebData.ProbableFormations.GetDataFileName(site))
            tmp = WebData.Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Player)))(json)
            If tmp.ContainsKey(day) Then
                For Each n As String In tmp(day).Keys
                    If state = "" OrElse tmp(day)(n).State = state Then
                        dicData.Add(n, tmp(day)(n))
                    End If
                Next
                Return WebData.Functions.SerializzaOggetto(dicData, True)
            Else
                Return "{}"
            End If
        End Function

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