Imports System.Data.OleDb
Imports System.Data.SqlTypes

Namespace Torneo
    Public Class RoseData

        Public Shared Function GetSvincolatiTorneo(role As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fname As String = PublicVariables.DataPath & "\export\svincolati.txt"
                Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()
                For Each line As String In lines
                    If role = "Tutti" OrElse line.Split(Convert.ToChar("|"))(2) = role Then
                        strdata.AppendLine(line)
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Function ApiGetTeamsTorneo() As String

            Dim strdata As New System.Text.StringBuilder

            Try

                If PublicVariables.DataFromDatabase Then

                    Dim teams As New Dictionary(Of String, Object)

                    Try
                        Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM tbteam")

                        If ds.Tables.Count > 0 Then
                            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                                Dim team As New TeamItem
                                team.idTeam = CInt(ds.Tables(0).Rows(i).Item("idteam"))
                                team.Name = ds.Tables(0).Rows(i).Item("Nome").ToString()
                                team.Coach = ds.Tables(0).Rows(i).Item("allenatore").ToString()
                                team.President = ds.Tables(0).Rows(i).Item("presidente").ToString()
                                teams.Add(team.idTeam.ToString(), team)
                            Next
                        End If
                    Catch ex As Exception
                        WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
                    End Try

                    Return WebData.Functions.SerializzaOggetto(teams, True)

                Else
                    Return WebData.Functions.CompactJson(IO.File.ReadAllText(PublicVariables.DataPath & "export\teams.json"))
                End If

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Function ApiGetRoseTorneo(teamId As String, gio As String) As String

            Dim json As String = ""

            Try
                Dim dicp As New Dictionary(Of String, List(Of Player))
                If PublicVariables.DataFromDatabase Then
                    dicp = GetRoseFromDb(teamId, gio)
                Else
                    dicp = GetRoseFromTxt(teamId, gio)
                End If
                Return WebData.Functions.SerializzaOggetto(dicp, True)
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return json

        End Function

        'Public Shared Function ApiGetRoseTorneo(teamId As Integer) As String

        '    Dim strdata As New System.Text.StringBuilder

        '    Try

        '        Dim fname As String = PublicVariables.DataPath & "export\rose.txt"
        '        Dim quotes As New Dictionary(Of String, String)
        '        Dim lines As List(Of String)

        '        Dim j As String = IO.File.ReadAllText(WebData.PlayersQuotes.GetDataFileName())
        '        Dim mtxquotes As List(Of Players.PlayerQuotesItem) = WebData.Functions.DeserializeJson(Of List(Of Players.PlayerQuotesItem))(j)

        '        For Each p As Players.PlayerQuotesItem In mtxquotes
        '            Dim key As String = p.Ruolo & "|" & p.Nome & "|" & p.Squadra
        '            If quotes.ContainsKey(key) = False Then quotes.Add(key, p.Qcur.ToString())
        '        Next

        '        lines = IO.File.ReadAllLines(fname).ToList()

        '        For Each line As String In lines
        '            Dim s() As String = line.Split(CChar("|"))
        '            If s.Length > 5 Then
        '                Dim key As String = s(2) & "|" & s(4) & "|" & s(5)
        '                If quotes.ContainsKey(key) Then
        '                    s(9) = quotes(key)
        '                End If
        '                strdata.AppendLine(WebData.Functions.ConvertListStringToString(s.ToList(), "|"))
        '            End If
        '        Next
        '    Catch ex As Exception
        '        strdata.Append(ex.Message)
        '        WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        '    End Try

        '    Return strdata.ToString()

        'End Function

        Private Shared Function GetRoseFromDb(TeamId As String, gio As String) As Dictionary(Of String, List(Of Player))

            Dim list As New Dictionary(Of String, List(Of Player))

            Return list

        End Function

        Private Shared Function GetRoseFromTxt(TeamId As String, gio As String) As Dictionary(Of String, List(Of Player))

            Dim list As New Dictionary(Of String, List(Of Player))

            Try

                Dim quotes As New Dictionary(Of String, Integer)
                Dim json As String = IO.File.ReadAllText(WebData.PlayersQuotes.GetDataFileName())
                Dim mtxquotes As List(Of Players.PlayerQuotesItem) = WebData.Functions.DeserializeJson(Of List(Of Players.PlayerQuotesItem))(json)

                For Each p As Players.PlayerQuotesItem In mtxquotes
                    Dim key As String = p.Ruolo & "|" & p.Nome & "|" & p.Squadra
                    If quotes.ContainsKey(key) = False Then quotes.Add(key, p.Qcur)
                Next

                json = IO.File.ReadAllText(WebData.MatchsData.GetMatchFileName())
                Dim matchs As New Dictionary(Of String, MatchsData.Match)
                Dim mtxmatchs As Dictionary(Of String, Dictionary(Of String, MatchsData.Match)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, MatchsData.Match)))(json)

                For Each g As String In mtxmatchs.Keys
                    If g = gio Then
                        For Each mid As String In mtxmatchs(g).Keys
                            matchs.Add(mtxmatchs(g)(mid).TeamA, mtxmatchs(g)(mid))
                            matchs.Add(mtxmatchs(g)(mid).TeamB, mtxmatchs(g)(mid))
                        Next
                    End If
                Next

                Dim fname As String = PublicVariables.DataPath & "\export\rose.txt"
                Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()

                For Each line As String In lines

                    Dim values() As String = line.Split(Convert.ToChar("|"))
                    Dim tid As String = values(1)

                    If TeamId = "-1" OrElse tid = TeamId Then

                        If list.ContainsKey(tid) = False Then list.Add(tid, New List(Of Player))

                        Dim p As New Player
                        p.IdTeam = CInt(values(0))
                        p.IdRosa = CInt(values(1))
                        p.Ruolo = values(2)
                        p.NatCode = values(3)
                        p.Nome = values(4)
                        p.Squadra = values(5)
                        p.Riconfermato = CInt(values(6))
                        p.Costo = CInt(values(7))
                        p.Qini = CInt(values(8))
                        p.Qcur = CInt(values(9))
                        p.StatisticAll.Gs = CInt(values(10))
                        p.StatisticAll.Gf = CInt(values(11))
                        p.StatisticAll.Amm = CInt(values(12))
                        p.StatisticAll.Esp = CInt(values(13))
                        p.StatisticAll.Ass = CInt(values(14))
                        p.StatisticAll.Avg_Vt = CInt(values(15))
                        p.StatisticAll.pGiocate = CInt(values(16))
                        p.StatisticAll.Titolare = CInt(values(17))
                        p.StatisticLast.pGiocate = CInt(values(18))
                        p.StatisticLast.Titolare = CInt(values(19))
                        p.StatisticLast.mm = CInt(values(20))

                        Dim key As String = p.Ruolo & "|" & p.Nome & "|" & p.Squadra
                        If quotes.ContainsKey(key) Then p.Qcur = quotes(key)
                        If matchs.ContainsKey(p.Squadra) Then
                            Dim m As MatchsData.Match = matchs(p.Squadra)
                            p.Match.TeamA = m.TeamA
                            p.Match.TeamB = m.TeamB
                            p.Match.Time = m.Time
                        End If

                        list(tid).Add(p)
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return list

        End Function

        Public Class Player

            Sub New()

            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String)
                Me.IdRosa = IdRosa
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String, ByVal Costo As Integer)
                Me.IdRosa = IdRosa
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
                Me.Costo = Costo
            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String, ByVal Costo As Integer, ByVal Qini As Integer)
                Me.IdRosa = IdRosa
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
                Me.Costo = Costo
                Me.Qini = Qini
            End Sub

            Public Property IdTeam() As Integer = 0
            Public Property IdRosa() As Integer = 0
            Public Property Ruolo() As String = ""
            Public Property Nome() As String = ""
            Public Property Squadra() As String = ""
            Public Property Nat() As String = ""
            Public Property NatCode() As String = ""
            Public Property Costo() As Integer = 0
            Public Property Qini() As Integer = 0
            Public Property Qcur() As Integer = 0
            Public Property Riconfermato As Integer = 0
            Public Property Variation() As Integer = 0
            Public Property Rating() As Integer = 0
            Public Property Tag() As String = ""
            Public Property Match As PlayerMatch = New PlayerMatch()
            Public Property StatisticAll As StatisticData = New StatisticData()
            Public Property StatisticLast As StatisticData = New StatisticData()

            Public Function GetStatsticAllDataFromDataRow(data As Data.DataRow) As StatisticData

                Dim d As New StatisticData

                'Dati statistici totali'
                d.Gs = Functions.ReadFieldIntegerData(data.Item("gs_tot"))
                d.Gf = Functions.ReadFieldIntegerData(data.Item("gf_tot"))
                d.Amm = Functions.ReadFieldIntegerData(data.Item("amm_tot"))
                d.Esp = Functions.ReadFieldIntegerData(data.Item("esp_tot"))
                d.Ass = Functions.ReadFieldIntegerData(data.Item("ass_tot"))
                d.RigT = Functions.ReadFieldIntegerData(data.Item("rigt_tot"))
                d.RigS = Functions.ReadFieldIntegerData(data.Item("rigs_tot"))
                d.RigP = Functions.ReadFieldIntegerData(data.Item("rigp_tot"))
                d.Sum_Vt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_vt_tot")) / 10)
                d.Avg_Vt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_vt")) / 10)
                d.Sum_Pt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_pt_tot")) / 10)
                d.Avg_Pt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_pt")) / 10)
                d.nPartite = Functions.ReadFieldIntegerData(data.Item("nump_tot"))
                d.pGiocate = Functions.ReadFieldIntegerData(data.Item("pgio_tot"))
                d.Titolare = Functions.ReadFieldIntegerData(data.Item("tit_tot"))
                d.Sostituito = Functions.ReadFieldIntegerData(data.Item("sos_tot"))
                d.Subentrato = Functions.ReadFieldIntegerData(data.Item("sub_tot"))
                d.mm = Functions.ReadFieldIntegerData(data.Item("mm_tot"))

                Return d

            End Function

            Public Function GetStatsticLastlDataFromDataRow(data As Data.DataRow) As StatisticData

                Dim d As New StatisticData

                'Dati statistici ultime giornate'
                d.Gs = Functions.ReadFieldIntegerData(data.Item("gs_last"))
                d.Gf = Functions.ReadFieldIntegerData(data.Item("gf_last"))
                d.Ass = Functions.ReadFieldIntegerData(data.Item("ass_last"))
                d.Sum_Vt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_vt_last")) / 10)
                d.Avg_Vt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_vt_last")) / 10)
                d.Sum_Pt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_pt_last")) / 10)
                d.Avg_Pt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_pt_last")) / 10)
                d.nPartite = Functions.ReadFieldIntegerData(data.Item("nump_last"))
                d.pGiocate = Functions.ReadFieldIntegerData(data.Item("pgio_last"))
                d.Titolare = Functions.ReadFieldIntegerData(data.Item("tit_last"))
                d.Sostituito = Functions.ReadFieldIntegerData(data.Item("sos_last"))
                d.Subentrato = Functions.ReadFieldIntegerData(data.Item("sub_last"))
                d.mm = Functions.ReadFieldIntegerData(data.Item("mm_last"))
                d.Avg_mm = Functions.ReadFieldIntegerData(data.Item("avg_mm_last"))
                Return d

            End Function

            Public Function GetVariation(d1 As StatisticData, d2 As StatisticData) As Integer

                Dim var As Integer = -2

                'Calcolo variazioni ultimi giorni
                If d1.pGiocate > 0 AndAlso d1.nPartite > 0 AndAlso d2.nPartite > 0 Then

                    Dim val2 As Single = d2.Sum_Pt \ d2.nPartite
                    Dim val1 As Single = d1.Sum_Pt \ d1.nPartite

                    If val2 > val1 + 0.3 Then
                        var = 1
                    ElseIf val2 < val1 - 0.3 Then
                        var = -1
                    Else
                        var = 0
                    End If
                Else
                    var = -2
                End If

                Return var

            End Function

            Public Class PlayerMatch
                Public Property TeamA As String = ""
                Public Property TeamB As String = ""
                Public Property Time As String = Now.ToString("yyyy/MM/dd HH:mm:ss")

            End Class

            Public Class StatisticData
                Public Property Gs() As Integer = 0
                Public Property Gf() As Integer = 0
                Public Property Amm() As Integer = 0
                Public Property Esp() As Integer = 0
                Public Property Ass() As Integer = 0
                Public Property RigT() As Integer = 0
                Public Property RigS() As Integer = 0
                Public Property RigP() As Integer = 0
                Public Property nPartite() As Integer = 0
                Public Property pGiocate() As Integer = 0
                Public Property Titolare() As Integer = 0
                Public Property Sostituito() As Integer = 0
                Public Property Subentrato() As Integer = 0
                Public Property mm() As Integer = 0
                Public Property Avg_mm() As Integer = 0
                Public Property Sum_Vt As Integer = 0
                Public Property Avg_Vt As Single = 0
                Public Property Sum_Pt As Integer = 0
                Public Property Avg_Pt As Single = 0
            End Class
        End Class

        Public Class TeamItem
            Public Property idTeam As Integer
            Public Property Name As String
            Public Property Coach As String
            Public Property President As String
        End Class
    End Class
End Namespace

