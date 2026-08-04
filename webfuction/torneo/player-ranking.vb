Imports System.Data

Namespace Torneo
    Public Class PlayersRanking

        Dim appSett As PublicVariables

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function MiglioriGiocatori(Ruolo As String, Svincolati As Boolean, giornata As Integer) As List(Of PlayerRanking)

            Dim plist As New List(Of PlayerRanking)

            Dim a As String = ""
            Dim sqlstr As New Text.StringBuilder
            sqlstr.AppendLine("SELECT d.*,p.qini,p.qcur FROM (")
            sqlstr.AppendLine("SELECT d.ruolo,d.nome,d.avgvt,d.avgpt,d.pgio,d.ptit,sum(t.mm) as min_last FROM (SELECT d.nome,d.ruolo,avg(voto) AS avgvt,avg(pt) AS avgpt,sum(tit) AS ptit,sum(tit+sub) AS pgio")
            sqlstr.AppendLine("FROM tbdati AS d")
            sqlstr.AppendLine("LEFT JOIN tbtabellini AS t ON (t.nome=d.nome and t.gio=d.gio)")
            sqlstr.AppendLine("WHERE d.gio<" & giornata & " and pt>-200 and d.ruolo='" & Ruolo & "' GROUP BY d.ruolo,d.nome) AS d")
            sqlstr.AppendLine("LEFT JOIN tbtabellini AS t ON (t.nome=d.nome and t.gio>=" & giornata - 5 & " and t.gio<" & giornata & ") GROUP BY d.nome,d.ruolo,d.avgvt,d.avgpt,d.pgio,d.ptit) AS d")
            sqlstr.AppendLine("LEFT JOIN tbplayer AS p ON p.nome=d.nome")

            a = sqlstr.ToString()

            Dim ds As Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstr.ToString())

            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Dim row As DataRow = ds.Tables(0).Rows(i)
                Dim p As New PlayerRanking
                p.Nome = Functions.ReadFieldStringData("nome", row, "")
                p.Ruolo = Functions.ReadFieldStringData("ruolo", row, "P")
                p.MediaVoto = Functions.ReadFieldDoubleData("avgvt", row, 0)
                p.MediaFanta = Functions.ReadFieldDoubleData("avgpt", row, 0)
                p.MinutiUltime5 = Functions.ReadFieldIntegerData("min_last", row, 0)
                p.Trend = Functions.ReadFieldIntegerData("qcur", row, 0) - Functions.ReadFieldIntegerData("qini", row, 0)
                p.Costo = Functions.ReadFieldIntegerData("qcur", row, 0)
                p.Titolarita = CalcolaTitolarita(Functions.ReadFieldIntegerData("ptit", row, 0), Functions.ReadFieldIntegerData("pgio", row, 0), p.MinutiUltime5, 0, False)
                plist.Add(p)
            Next

            plist = MiglioriGiocatori(plist, 10)

            Return plist

        End Function

        Public Function MiglioriGiocatori(lista As List(Of PlayerRanking), Optional topN As Integer = 10) As List(Of PlayerRanking)
            Return lista.OrderByDescending(Function(g) PunteggioFinale(g)).Where(Function(g) g.Titolarita > 0.9).ToList()
        End Function

        Private Function PunteggioFinale(g As PlayerRanking) As Double
            Dim ev = CalcolaEV(g)
            Dim p6 = ProbabilitaVoto6(g)
            Dim form = CalcolaForm(g)
            g.Rank = (ev * 0.5) + (p6 * 0.3) + (form * 0.2)
            Return g.Rank
        End Function

        Private Function CalcolaForm(g As PlayerRanking) As Double
            ' Normalizzazione semplice
            Dim votoNorm = g.MediaVoto / 10.0
            Dim fantaNorm = g.MediaFanta / 15.0
            Dim minutiNorm = Math.Min(g.MinutiUltime5 / 450.0, 1.0)

            Return (votoNorm * 0.5) + (fantaNorm * 0.3) + (minutiNorm * 0.2)
        End Function

        Private Function CalcolaScore(g As PlayerRanking) As Double
            Dim form = CalcolaForm(g)
            Return (form * 0.35) +
           (g.Titolarita * 0.25) +
           (g.Trend * 0.2) +
           (g.Calendario * 0.2)
        End Function

        Private Function CalcolaEV(g As PlayerRanking) As Double
            Dim score = CalcolaScore(g)
            If g.Costo <= 0 Then Return 0
            Return score / g.Costo
        End Function

        Private Function ProbabilitaVoto6(g As PlayerRanking) As Double
            ' Regressione logistica semplificata
            Dim x = (g.MediaVoto * 0.4) +
            (g.Titolarita * 0.3) +
            (g.Calendario * 0.2) +
            (g.MinutiUltime5 / 450.0 * 0.1)

            ' Sigmoide
            Return 1.0 / (1.0 + Math.Exp(-x))
        End Function

        Public Function CalcolaTitolarita(partiteDaTitolare As Integer, partiteGiocate As Integer, minutiUltime5 As Integer, probabilitaBallottaggio As Integer, rientroDaInfortunio As Boolean) As Double

            Dim percTitolare = If(partiteGiocate > 0, partiteDaTitolare / partiteGiocate, 0)
            Dim minutiNorm = Math.Min(minutiUltime5 / 450.0, 1.0)
            Dim ballottaggio = 1.0 - (probabilitaBallottaggio / 100.0)
            Dim injuryFactor = If(rientroDaInfortunio, 0.6, 1.0)

            ' Pesi ottimizzati
            Dim tit = (percTitolare * 0.5) +
              (minutiNorm * 0.3) +
              (ballottaggio * 0.2)

            Return Math.Min(tit * injuryFactor, 1.0)
        End Function

        Public Class PlayerRanking
            Public Property Nome As String
            Public Property Ruolo As String
            Public Property Squadra As String
            Public Property Costo As Integer
            Public Property MediaVoto As Double
            Public Property MediaFanta As Double
            Public Property MinutiUltime5 As Integer
            Public Property Trend As Integer
            Public Property Titolarita As Double   ' 0–1
            Public Property Calendario As Double   ' 0–1
            Public Property Rank As Double = 0
        End Class


    End Class
End Namespace
