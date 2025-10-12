Imports Newtonsoft
Imports System.Runtime.Remoting

Namespace Torneo
    Public Class ProbablePlayers

        Public Shared Function ApiGetProbableFormation(state As String) As String
            Dim dicData As New Dictionary(Of String, Probable) From {{"Gazzetta", New Probable}, {"Fantacalcio", New Probable}, {"PianetaFantacalcio", New Probable}}
            Dim states() As String = state.Split(Convert.ToChar(","))
            For Each site As String In dicData.Keys.ToList()

                Dim fname As String = WebData.ProbableFormations.GetDataFileName(site)

                If IO.File.Exists(fname) Then

                    Dim json As String = IO.File.ReadAllText(fname)
                    Dim tmp = WebData.Functions.DeserializeJson(Of Probable)(json)

                    If state <> "" Then
                        For Each chiave In tmp.Players.Keys.ToList()
                            If states.Contains(tmp.Players(chiave).State) = False Then
                                tmp.Players.Remove(chiave)
                            End If
                        Next
                    End If
                    dicData(site) = tmp
                End If
            Next

            Return WebData.Functions.SerializzaOggetto(dicData, True)

        End Function

        Public Class Probable
            Public Property Day() As Integer = -1
            Public Property Players() As New Dictionary(Of String, Player)

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
    End Class
End Namespace