Imports iFanta.LegaObject
Imports Newtonsoft.Json
Imports iFanta.SystemFunction.FileAndDirectory

Public Class MakeWedDataFiles
    Public Shared Function MakeFiles() As List(Of String)

        Dim flist As New List(Of String)

        'Salvo le rose'
        currlega.LoadTeams(True, True)
        flist.Add(MakePlayersFile(currlega.Teams.SelectMany(Function(x) x.Players.ToList).ToList(), GetTempDirectory() & "\rose.txt"))

        'Salvo la lista dei team'
        flist.Add(MakeTeamsFile(currlega.Teams, GetTempDirectory() & "\teams.json"))
        'flist.Add(MakeTeamsFile(currlega.Teams, GetTempDirectory() & "\team.txt"))

        'Salvo la lista degli svincolati'
        flist.Add(MakePlayersFile(currlega.Teams(0).GetPlayer("SVINCOLATI", ""), GetTempDirectory() & "\svincolati.txt"))

        'Salvo la lista dei team'
        flist.AddRange(MakeClassificaFile())

        'Salvo le formazioni'
        flist.AddRange(MakeFormazioniFile())

        'Salvo la coppa'
        flist.Add(MakeCupFile())

        Return flist

    End Function

    Private Shared Function MakePlayersFile(players As List(Of Team.Player), fname As String) As String
        Dim sb As New System.Text.StringBuilder
        For k As Integer = 0 To players.Count - 1
            sb.Append(players(k).IdTeam)
            sb.Append("|" & players(k).IdRosa)
            sb.Append("|" & players(k).Ruolo)
            sb.Append("|" & players(k).NatCode)
            sb.Append("|" & players(k).Nome)
            sb.Append("|" & players(k).Squadra)
            sb.Append("|" & players(k).Riconfermato)
            sb.Append("|" & players(k).Costo)
            sb.Append("|" & players(k).QIni)
            sb.Append("|" & players(k).QCur)
            sb.Append("|" & players(k).StatisticAll.Gs)
            sb.Append("|" & players(k).StatisticAll.Gf)
            sb.Append("|" & players(k).StatisticAll.Amm)
            sb.Append("|" & players(k).StatisticAll.Esp)
            sb.Append("|" & players(k).StatisticAll.Ass)
            sb.Append("|" & players(k).StatisticAll.Avg_Pt)
            sb.Append("|" & players(k).StatisticAll.pGiocate)
            sb.Append("|" & players(k).StatisticAll.Titolare)
            sb.Append("|" & players(k).StatisticLast.pGiocate)
            sb.Append("|" & players(k).StatisticLast.Titolare)
            sb.AppendLine("|" & players(k).StatisticLast.mm)
        Next
        IO.File.WriteAllText(fname, sb.ToString())
        Return fname
    End Function

    Private Shared Function MakeTeamsFile(teams As List(Of Team), fname As String) As String
        'Salvo la lista dei team'
        Dim wteams As New Dictionary(Of Integer, WebTeam)
        For i As Integer = 0 To teams.Count - 1
            Dim team As New WebTeam
            team.idTeam = i
            team.Name = teams(i).Nome
            team.Coach = teams(i).Allenatore
            team.President = teams(i).Presidente
            wteams.Add(i, team)
        Next
        IO.File.WriteAllText(fname, JsonConvert.SerializeObject(wteams, Formatting.Indented))

        'Dim sb As New System.Text.StringBuilder
        'For i As Integer = 0 To teams.Count - 1
        '    sb.AppendLine(i & "|" & teams(i).Nome & "|" & teams(i).Allenatore & "|" & SystemFunction.Convertion.ConvertFileToBase64String(GetLegaCoatOfArmsLegsDirectory() & "\" & teams(i).IdTeam & "-16x16.png") & "|" & SystemFunction.Convertion.ConvertFileToBase64String(GetLegaCoatOfArmsLegsDirectory() & "\" & teams(i).IdTeam & "-24x24.png"))
        'Next
        'IO.File.WriteAllText(fname, sb.ToString())
        Return fname
    End Function

    Private Shared Function MakeClassificaFile() As List(Of String)

        Dim class1 As New Dictionary(Of Integer, List(Of sClassifica.sClassificaItem))
        Dim class2 As New Dictionary(Of Integer, List(Of sClassifica.sClassificaItem))
        Dim fclass1 As String = GetTempDirectory() & "\classifica.json"
        Dim fclass2 As String = GetTempDirectory() & "\classifica-top.json"
        'Dim fclasa3 As String = GetTempDirectory() & "\classifica-his.json"

        For j As Integer = 1 To currlega.GiornataCorrente()
            currlega.Classifica.Load(j, False)
            class1.Add(j, currlega.Classifica.Item)
            currlega.Classifica.Load(j, True)
            class2.Add(j, currlega.Classifica.Item)
        Next

        'currlega.Classifica.LoadHistory()

        IO.File.WriteAllText(fclass1, JsonConvert.SerializeObject(class1, Formatting.Indented))
        IO.File.WriteAllText(fclass2, JsonConvert.SerializeObject(class2, Formatting.Indented))
        'IO.File.WriteAllText(fclass3, JsonConvert.SerializeObject(currlega.Classifica.History, Formatting.Indented))

        Return New List(Of String) From {fclass1, fclass2}

    End Function

    Private Shared Function MakeFormazioniFile() As List(Of String)
        Dim fform As String = GetTempDirectory() & "\formazioni.txt"
        Dim sb As New System.Text.StringBuilder
        For k As Integer = 1 To currlega.GiornataCorrente()
            currlega.LoadFormazioni(k, "", False)
            For j As Integer = 0 To currlega.Formazioni.Count - 1

                Dim Formazione As Formazione = currlega.Formazioni(j)

                Formazione.Players = Formazione.Players.OrderBy(Function(x) x.IdRosa).ToList()

                For i As Integer = 0 To Formazione.Players.Count - 1
                    sb.Append(k)
                    sb.Append("|" & currlega.Formazioni(j).IdTeam)
                    sb.Append("|" & Formazione.Players(i).IdRosa)
                    sb.Append("|" & Formazione.Players(i).Jolly)
                    sb.Append("|" & Formazione.Players(i).Type)
                    sb.Append("|" & Formazione.Players(i).IdFormazione)
                    sb.Append("|" & Formazione.Players(i).InCampo)
                    sb.Append("|" & Formazione.Players(i).Ruolo)
                    sb.Append("|" & Formazione.Players(i).Nome.Replace("’", "'"))
                    sb.Append("|" & Formazione.Players(i).Squadra.Replace("’", "'"))
                    sb.Append("|" & Formazione.Players(i).Dati.Vt * 10)
                    sb.Append("|" & Formazione.Players(i).Dati.Amm)
                    sb.Append("|" & Formazione.Players(i).Dati.Esp)
                    sb.Append("|" & Formazione.Players(i).Dati.Ass)
                    sb.Append("|" & Formazione.Players(i).Dati.AutG)
                    sb.Append("|" & Formazione.Players(i).Dati.Gs)
                    sb.Append("|" & Formazione.Players(i).Dati.Gf)
                    sb.Append("|" & Formazione.Players(i).Dati.RigS)
                    sb.Append("|" & Formazione.Players(i).Dati.RigP)
                    sb.AppendLine("|" & Formazione.Players(i).Dati.Pt * 10)
                Next
                sb.AppendLine(k & "|" & currlega.Formazioni(j).IdTeam & "|" & Formazione.BonusDifesa * 10 & "|" & Formazione.BonusCentroCampo * 10 & "|" & Formazione.BonusAttacco * 10 & "|" & CInt(Formazione.ModuleSubstitution))
            Next
        Next
        IO.File.WriteAllText(fform, sb.ToString())

        Return New List(Of String) From {fform}

    End Function

    Private Shared Function MakeCupFile() As String
        Dim fcup As String = GetTempDirectory() & "\cup.json"
        Dim cup As New Coppa
        cup.Load()
        IO.File.WriteAllText(fcup, JsonConvert.SerializeObject(cup, Formatting.Indented))
        Return fcup
    End Function

    Public Class WebTeam
        Public Property idTeam As Integer
        Public Property Name As String
        Public Property Coach As String
        Public Property President As String
    End Class

End Class
