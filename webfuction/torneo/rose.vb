Imports System.Data.OleDb
Imports System.Data.SqlTypes

Namespace Torneo
    Public Class RoseData

        Public Shared Function GetSvincolatiTorneo(role As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fname As String = PublicVariables.DataPath & "\exp\svincolati.txt"
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
                    Return WebData.Functions.CompactJson(IO.File.ReadAllText(PublicVariables.DataPath & "torneo\teams.json"))
                End If

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

        End Function

        Public Shared Function ApiGetRoseTorneo(teamId As Integer) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                Dim fquota As String = PublicVariables.DataPath & "data\players-quote.txt"
                Dim fname As String = PublicVariables.DataPath & "torneo\rose.txt"
                Dim quotes As New Dictionary(Of String, String)
                Dim lines As List(Of String)

                lines = IO.File.ReadAllLines(fquota).ToList()

                For Each line As String In lines
                    Dim s() As String = line.Split(CChar("|"))
                    If s.Length = 5 Then
                        Dim key As String = s(0) & "|" & s(1) & "|" & s(2)
                        If quotes.ContainsKey(key) = False Then quotes.Add(key, s(4))
                    End If
                Next

                lines = IO.File.ReadAllLines(fname).ToList()

                For Each line As String In lines
                    Dim s() As String = line.Split(CChar("|"))
                    If s.Length > 5 Then
                        Dim key As String = s(2) & "|" & s(4) & "|" & s(5)
                        If quotes.ContainsKey(key) Then
                            s(9) = quotes(key)
                        End If
                        strdata.AppendLine(WebData.Functions.ConvertListStringToString(s.ToList(), "|"))
                    End If
                Next
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString()

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
                Public Property Time As Date = Date.Now

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

