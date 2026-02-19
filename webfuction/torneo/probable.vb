
Imports System.Security.Policy

Namespace Torneo
    Public Class ProbablePlayers

        Dim appSett As New PublicVariables

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function ApiGetProbableFormation(state As String) As String
            Dim dicData As Dictionary(Of String, Probable) = GetProbableFormation(state)
            Return WebData.Functions.SerializzaOggetto(dicData, True)
        End Function

        Public Function GetProbableFormation(state As String, Optional Giornata As Integer = -1) As Dictionary(Of String, Probable)

            Dim dicData As New Dictionary(Of String, Probable) From {{"Gazzetta", New Probable}, {"Fantacalcio", New Probable}, {"PianetaFantacalcio", New Probable}, {"Sky", New Probable}, {"FantaPazz", New Probable}}
            Dim states() As String = state.Split(Convert.ToChar(","))
            Dim probData As New WebData.ProbableFormations(appSett)

            For Each site As String In dicData.Keys.ToList()

                Dim fname As String = probData.GetDataFileName(site, Giornata)

                If IO.File.Exists(fname) Then

                    Dim json As String = IO.File.ReadAllText(fname, System.Text.Encoding.GetEncoding(1252))
                    Dim tmp = WebData.Functions.DeserializeJson(Of Probable)(json)
                    Dim dicRuoliTeam As New Dictionary(Of String, Dictionary(Of String, Integer))

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

            Return dicData

        End Function

        Public Function GetTeamModule(Probable As Dictionary(Of String, Probable)) As Dictionary(Of String, String)

            Dim plist As New Torneo.Players(appSett)
            Dim listPlayers As List(Of Torneo.Players.PlayerQuotesItem) = plist.GetPlayersQuotesData("")
            Dim dicPlayers As Dictionary(Of String, Torneo.Players.PlayerQuotesItem) = listPlayers.ToDictionary(Function(x) x.Nome, Function(x) x)
            Dim dicRuoliTeamBySite As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Integer)))
            Dim dicRuoliTeam As New Dictionary(Of String, Dictionary(Of String, List(Of Integer)))
            Dim dicModuleTeam As New Dictionary(Of String, String)

            For Each site As String In Probable.Keys
                dicRuoliTeamBySite.Add(site, New Dictionary(Of String, Dictionary(Of String, Integer)))
                For Each chiave In Probable(site).Players.Keys.ToList()
                    If Probable(site).Players(chiave).State = "Titolare" Then
                        Dim s() As String = chiave.Split(CChar("/"))
                        If s.Length = 2 Then
                            If dicPlayers.ContainsKey(s(0)) Then
                                Dim p As Torneo.Players.PlayerQuotesItem = dicPlayers(s(0))
                                If dicRuoliTeamBySite(site).ContainsKey(p.Squadra) = False Then dicRuoliTeamBySite(site).Add(p.Squadra, New Dictionary(Of String, Integer))
                                If dicRuoliTeamBySite(site)(p.Squadra).ContainsKey(p.Ruolo) = False Then dicRuoliTeamBySite(site)(p.Squadra).Add(p.Ruolo, 0)
                                dicRuoliTeamBySite(site)(p.Squadra)(p.Ruolo) += 1
                            End If
                        End If
                    End If
                Next
            Next

            For Each site As String In dicRuoliTeamBySite.Keys
                For Each t As String In dicRuoliTeamBySite(site).Keys
                    For Each r As String In dicRuoliTeamBySite(site)(t).Keys
                        If dicRuoliTeam.ContainsKey(t) = False Then dicRuoliTeam.Add(t, New Dictionary(Of String, List(Of Integer)))
                        If dicRuoliTeam(t).ContainsKey(r) = False Then dicRuoliTeam(t).Add(r, New List(Of Integer))
                        dicRuoliTeam(t)(r).Add(dicRuoliTeamBySite(site)(t)(r))
                    Next
                Next
            Next

            For Each t As String In dicRuoliTeam.Keys

                Dim moduleList As New List(Of String)
                Dim nd As Integer = 0
                Dim nc As Integer = 0
                Dim na As Integer = 0

                For Each r As String In dicRuoliTeam(t).Keys
                    For i As Integer = 0 To dicRuoliTeam(t)(r).Count - 1
                        Dim np As Integer = dicRuoliTeam(t)(r)(i)
                        If r = "D" Then nd = np
                        If r = "C" Then nc = np
                        If r = "A" Then na = np
                    Next
                Next

                Dim mods As String = nd & "-" & nc & "-" & na
                If nd + nc + na = 10 AndAlso na > 0 AndAlso moduleList.Contains(mods) = False Then moduleList.Add(mods)

                If moduleList.Count > 0 Then
                    dicModuleTeam.Add(t, moduleList(0))
                End If
            Next

            Return dicModuleTeam

        End Function

        Public Sub UpdateTeamModule(giornata As Integer, dicModuleTeam As Dictionary(Of String, String))

            Try
                Torneo.Functions.ExecuteSql(appSett, "CREATE TABLE tbteammodule (ID AUTOINCREMENT PRIMARY KEY,giornata INTEGER,squadra TEXT(50),modulo TEXT(10))")
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try

            Torneo.Functions.ExecuteSql(appSett, "DELETE FROM tbteammodule WHERE giornata=" & giornata)

            Dim sqlinsert As New List(Of String)

            For Each team As String In dicModuleTeam.Keys
                sqlinsert.Add("INSERT INTO tbteammodule (giornata,squadra,modulo) values (" & giornata & ",'" & team & "','" & dicModuleTeam(team) & "')")
            Next

            Torneo.Functions.ExecuteSql(appSett, sqlinsert)

        End Sub

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
                Public Property Infortunio As New InfortunioInfo

                Public Class InfortunioInfo
                    Public Property Giorni As Integer = -1
                    Public Property Severity As Integer = 0
                End Class
            End Class
        End Class
    End Class
End Namespace