Imports System.Data.OleDb
Imports System.Data.SqlTypes

Namespace Torneo
    Public Class RoseData

        Public Shared Sub ApiAddRosa(TeamId As String, json As String)

            If json = "" Then Throw New Exception("Json not valid")

            Dim mData As MetaData = WebData.Functions.DeserializeJson(Of MetaData)(json)

            If mData.teamId <> TeamId Then Throw New Exception("Json not valid")

            ApiDeleteRose(TeamId)

            If mData IsNot Nothing AndAlso mData.data.Count > 0 Then
                For Each tid As String In mData.data.Keys
                    Dim sqlinsert As New List(Of String)
                    For Each p As Player In mData.data(tid)
                        Dim sqlp As New System.Text.StringBuilder
                        sqlp.AppendLine("INSERT INTO tbrose (idteam,idrosa,ruolo,nome,costo,qini,riconfermato) values (")
                        sqlp.AppendLine(tid & "," & p.RosaId & ",'" & p.Ruolo & "','" & p.Nome.ToUpper() & "'," & p.Costo & "," & p.Qini & "," & p.Riconfermato & ")")
                        sqlinsert.Add(sqlp.ToString())
                    Next
                    Functions.ExecuteSql(sqlinsert)
                Next
            End If

        End Sub

        Public Shared Sub ApiDeleteRose(TeamId As String)
            Functions.ExecuteSql("DELETE FROM tbrose" & If(TeamId <> "-1", " WHERE idteam=" & TeamId, ""))
        End Sub

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

        Public Shared Function ApiGetPlayersTorneo(teamId As String, role As String) As String

            Dim json As String = ""

            Try
                Dim dicp As New Dictionary(Of String, List(Of Player))
                If PublicVariables.DataFromDatabase Then
                    dicp = GetPlayersFromDb(teamId, role)
                Else
                    dicp = GetPlayersFromTxt(teamId, role)
                End If
                Return WebData.Functions.SerializzaOggetto(dicp, True)
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return json

        End Function

        Private Shared Function GetPlayersFromDb(TeamId As String, role As String) As Dictionary(Of String, List(Of Player))

            Dim list As New Dictionary(Of String, List(Of Player))

            Try
                Dim strTeadId As String = "idteam = " & TeamId
                If TeamId = "-1" Then
                    strTeadId = "idteam >= 0"
                ElseIf TeamId = "-2" Then
                    strTeadId = "idteam is null"
                End If

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM player WHERE " & strTeadId & If(role <> "all", " AND ruolo = " & role, "") & " ORDER BY idteam,idrosa")

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim type As Integer = Functions.ReadFieldIntegerData("type", row, 0)
                        Dim tid As String = If(TeamId = "-2", "-1", Functions.ReadFieldIntegerData("idteam", row, 0).ToString())
                        Dim r As String = Functions.ReadFieldStringData("ruolo", row, "D")
                        Dim key As String = If(TeamId = "-2", r, tid)

                        If list.ContainsKey(key) = False Then list.Add(key, New List(Of Player))

                        Dim p As New Player
                        p.TeamId = CInt(tid)
                        p.RosaId = Functions.ReadFieldIntegerData("idrosa", row, 0)
                        p.Ruolo = Functions.ReadFieldStringData("ruolo", row, "D")
                        p.NatCode = Functions.ReadFieldStringData("natcode", row, "")
                        p.Nome = Functions.ReadFieldStringData("nome", row, "D")
                        p.Squadra = Functions.ReadFieldStringData("squadra", row, "D")
                        p.Riconfermato = Functions.ReadFieldIntegerData("riconfermato", row, 0)
                        p.Costo = Functions.ReadFieldIntegerData("costo", row, 0)
                        p.Qini = Functions.ReadFieldIntegerData("qini", row, 0)
                        p.Qcur = Functions.ReadFieldIntegerData("qcur", row, 0)
                        p.StatisticAll.Gs = Functions.ReadFieldIntegerData("gs_tot", row, 0)
                        p.StatisticAll.Gf = Functions.ReadFieldIntegerData("gf_tot", row, 0)
                        p.StatisticAll.Amm = Functions.ReadFieldIntegerData("amm_tot", row, 0)
                        p.StatisticAll.Esp = Functions.ReadFieldIntegerData("esp_tot", row, 0)
                        p.StatisticAll.Ass = Functions.ReadFieldIntegerData("ass_tot", row, 0)
                        p.StatisticAll.RigT = Functions.ReadFieldIntegerData("rigt_tot", row, 0)
                        p.StatisticAll.RigS = Functions.ReadFieldIntegerData("rigs_tot", row, 0)
                        p.StatisticAll.RigP = Functions.ReadFieldIntegerData("rip_tot", row, 0)
                        p.StatisticAll.AvgVt = Functions.ReadFieldIntegerData("avg_vt", row, 0)
                        p.StatisticAll.AvgPt = Functions.ReadFieldIntegerData("avg_pt", row, 0)
                        p.StatisticAll.pGiocate = Functions.ReadFieldIntegerData("pgio_tot", row, 0)
                        p.StatisticAll.Titolare = Functions.ReadFieldIntegerData("tit_tot", row, 0)
                        p.StatisticAll.Sostituito = Functions.ReadFieldIntegerData("sos_tot", row, 0)
                        p.StatisticAll.Subentrato = Functions.ReadFieldIntegerData("sub_tot", row, 0)
                        p.StatisticAll.Minuti = Functions.ReadFieldIntegerData("mm_tot", row, 0)
                        p.StatisticLast.Gs = Functions.ReadFieldIntegerData("gs_last", row, 0)
                        p.StatisticLast.Gf = Functions.ReadFieldIntegerData("gf_last", row, 0)
                        p.StatisticLast.pGiocate = Functions.ReadFieldIntegerData("pgio_last", row, 0)
                        p.StatisticLast.Titolare = Functions.ReadFieldIntegerData("tit_last", row, 0)
                        p.StatisticLast.Sostituito = Functions.ReadFieldIntegerData("sos_last", row, 0)
                        p.StatisticLast.Subentrato = Functions.ReadFieldIntegerData("sub_last", row, 0)
                        p.StatisticLast.Minuti = Functions.ReadFieldIntegerData("mm_last", row, 0)

                        list(key).Add(p)

                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return list

        End Function

        Private Shared Function GetPlayersFromTxt(TeamId As String, role As String) As Dictionary(Of String, List(Of Player))

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

                Dim fname As String = If(TeamId <> "-2", PublicVariables.DataPath & "\export\rose.txt", PublicVariables.DataPath & "\export\svincolati.txt")
                Dim lines As List(Of String) = IO.File.ReadAllLines(fname).ToList()

                For Each line As String In lines

                    Dim values() As String = line.Split(Convert.ToChar("|"))
                    Dim tid As String = values(1)
                    Dim r As String = values(2)

                    If (TeamId = "-1" OrElse tid = TeamId) AndAlso (role = "all" OrElse r = role) Then

                        If list.ContainsKey(tid) = False Then list.Add(tid, New List(Of Player))

                        Dim p As New Player
                        p.TeamId = CInt(values(0))
                        p.RosaId = CInt(values(1))
                        p.Ruolo = r
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
                        p.StatisticAll.AvgPt = CInt(values(15))
                        p.StatisticAll.pGiocate = CInt(values(16))
                        p.StatisticAll.Titolare = CInt(values(17))
                        p.StatisticLast.pGiocate = CInt(values(18))
                        p.StatisticLast.Titolare = CInt(values(19))
                        p.StatisticLast.Minuti = CInt(values(20))

                        Dim key As String = p.Ruolo & "|" & p.Nome & "|" & p.Squadra
                        If quotes.ContainsKey(key) Then p.Qcur = quotes(key)

                        list(tid).Add(p)
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return list

        End Function

        Public Class MetaData
            Public Property type As String = ""
            Public Property teamId() As String = ""
            Public Property data As Dictionary(Of String, List(Of Player))
        End Class

        Public Class Player

            Sub New()

            End Sub

            Sub New(ByVal RosaId As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String)
                Me.RosaId = RosaId
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
            End Sub

            Sub New(ByVal RosaId As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String, ByVal Costo As Integer)
                Me.RosaId = RosaId
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
                Me.Costo = Costo
            End Sub

            Sub New(ByVal RosaId As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String, ByVal Costo As Integer, ByVal Qini As Integer)
                Me.RosaId = RosaId
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.Squadra = Squadra
                Me.Costo = Costo
                Me.Qini = Qini
            End Sub

            Public Property TeamId() As Integer = 0
            Public Property RosaId() As Integer = 0
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
                d.SumVt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_vt_tot")) / 10)
                d.AvgVt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_vt")) / 10)
                d.SumPt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_pt_tot")) / 10)
                d.AvgPt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_pt")) / 10)
                d.nPartite = Functions.ReadFieldIntegerData(data.Item("nump_tot"))
                d.pGiocate = Functions.ReadFieldIntegerData(data.Item("pgio_tot"))
                d.Titolare = Functions.ReadFieldIntegerData(data.Item("tit_tot"))
                d.Sostituito = Functions.ReadFieldIntegerData(data.Item("sos_tot"))
                d.Subentrato = Functions.ReadFieldIntegerData(data.Item("sub_tot"))
                d.Minuti = Functions.ReadFieldIntegerData(data.Item("mm_tot"))

                Return d

            End Function

            Public Function GetStatsticLastlDataFromDataRow(data As Data.DataRow) As StatisticData

                Dim d As New StatisticData

                'Dati statistici ultime giornate'
                d.Gs = Functions.ReadFieldIntegerData(data.Item("gs_last"))
                d.Gf = Functions.ReadFieldIntegerData(data.Item("gf_last"))
                d.Ass = Functions.ReadFieldIntegerData(data.Item("ass_last"))
                d.SumVt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_vt_last")) / 10)
                d.AvgVt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_vt_last")) / 10)
                d.SumPt = CInt(Functions.ReadFieldIntegerData(data.Item("sum_pt_last")) / 10)
                d.AvgPt = CSng(Functions.ReadFieldIntegerData(data.Item("avg_pt_last")) / 10)
                d.nPartite = Functions.ReadFieldIntegerData(data.Item("nump_last"))
                d.pGiocate = Functions.ReadFieldIntegerData(data.Item("pgio_last"))
                d.Titolare = Functions.ReadFieldIntegerData(data.Item("tit_last"))
                d.Sostituito = Functions.ReadFieldIntegerData(data.Item("sos_last"))
                d.Subentrato = Functions.ReadFieldIntegerData(data.Item("sub_last"))
                d.Minuti = Functions.ReadFieldIntegerData(data.Item("mm_last"))
                d.AvgMinuti = Functions.ReadFieldIntegerData(data.Item("avg_mm_last"))
                Return d

            End Function

            Public Function GetVariation(d1 As StatisticData, d2 As StatisticData) As Integer

                Dim var As Integer = -2

                'Calcolo variazioni ultimi giorni
                If d1.pGiocate > 0 AndAlso d1.nPartite > 0 AndAlso d2.nPartite > 0 Then

                    Dim val2 As Single = d2.SumPt \ d2.nPartite
                    Dim val1 As Single = d1.SumPt \ d1.nPartite

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
                Public Property Minuti() As Integer = 0
                Public Property AvgMinuti() As Integer = 0
                Public Property SumVt As Integer = 0
                Public Property AvgVt As Single = 0
                Public Property SumPt As Integer = 0
                Public Property AvgPt As Single = 0
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

