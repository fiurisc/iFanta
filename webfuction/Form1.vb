Imports System.Collections.Concurrent
Imports System.Data.OleDb
Imports System.Net
Imports System.Net.Http
Imports System.Net.Mail
Imports Newtonsoft.Json.Linq
Imports webfuction.Torneo.AutoFormazioniData
Imports webfuction.Torneo.ProbablePlayers
Imports webfuction.WebData.Players

Public Class Form1

    Dim year As String = "2025"
    Dim appSett As New Torneo.PublicVariables

    Public Class ArubaSmtpTest
        Public Shared Function TestArubaSmtp() As String
            Try
                Dim mail As New System.Net.Mail.MailMessage()
                mail.From = New MailAddress("postmaster@ifantacalcio.it") ' Inserisci la tua email Aruba
                mail.To.Add("fernando.iurisci@gmail.com") ' Inserisci un destinatario valido
                mail.Subject = "Test SMTP Aruba"
                mail.Body = "Questa è una mail di test inviata da ASP.NET tramite SMTP Aruba."
                mail.IsBodyHtml = False

                Dim smtp As New System.Net.Mail.SmtpClient("smtp.aruba.it", 465)
                smtp.Credentials = New System.Net.NetworkCredential("postmaster@ifantacalcio.it", "Anxanum1969!") ' Inserisci le credenziali
                smtp.EnableSsl = True

                smtp.Send(mail)
                Return "Email inviata con successo!"
            Catch ex As Exception
                Return "Errore durante l'invio: " & ex.Message
            End Try
        End Function
    End Class

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Torneo.General.SendMail("fernando.iurisci@gmail.com", "", "Reset password", "Reset password", "fd", New List(Of String))

        Dim data As String = "FORMAZIONI"
        Dim show As Boolean = True
        Dim dirs As String = My.Application.Info.DirectoryPath
        Dim str As New System.Text.StringBuilder

        appSett.InitPath(My.Application.Info.DirectoryPath & "\", My.Application.Info.DirectoryPath & "\tornei\", "Parenti", year)
        Dim gen As New Torneo.General(appSett)
        gen.ReadSettings()

        'UpdateProbableModuleTeam()
        'GetAutomaticBestHistoricalParameters()
        GetAutomaticFormation()

        End

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim pquotes As New WebData.PlayersQuotes(appSett)
        Dim gen As New Torneo.General(appSett)
        Dim years As List(Of Torneo.General.YearTorneo) = gen.ApiGetYearsList()
        appSett.InitPath(My.Application.Info.DirectoryPath & "\", My.Application.Info.DirectoryPath & "\tornei\", "Parenti", year)
        pquotes.GetPlayersQuotes(False)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim pdata As New WebData.PlayersData(appSett)
        pdata.GetPlayersData(False)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim mdata As New WebData.MatchsData(appSett)
        mdata.GetDataMatchs(False)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim dclasa As New WebData.Classifica(appSett)
        dclasa.GetClassifica(False)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("gazzetta", False)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("fantacalcio", False)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("sky", False)
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("fantapazz", False)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("pianetafantacalcio", False)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        ' WebData.ProbableFormations.GetCds(False)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        'Determino i link delle varie partite'
        Dim enc As String = "utf-8"
        Dim html As String = WebData.Functions.GetPage(appSett, "https://www.fantapazz.com/calcio/fantacalcio/serie-a/probabili-formazioni", enc)

        If html <> "" Then

            IO.File.WriteAllText("prova.txt", html, System.Text.Encoding.GetEncoding(enc))

        End If

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click

        Dim data As New Torneo.FormazioniData(appSett)
        Dim dataauto As New Torneo.AutoFormazioniData(appSett)
        Dim comp As New Torneo.CompilaData(appSett)
        comp.ApiCompila("33")
        'Dim mdata As New WebData.MatchsData(appSett)
        'mdata.UpdateDataFromFile("18")
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click

        'SQLiteToAccessCopier.CopyData(AppContext.BaseDirectory & "tornei\data.db", AppContext.BaseDirectory & "tornei\2025.accdb")
        'Dim dicData As Dictionary(Of String, Probable) = Torneo.ProbablePlayers.GetProbableFormation("")
        'Dim dicName As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Integer)))

        'For Each site As String In dicData.Keys
        '    For Each pkey As String In dicData(site).Players.Keys
        '        Dim p As Probable.Player = dicData(site).Players(pkey)
        '        If dicName.ContainsKey(p.Team) = False Then dicName.Add(p.Team, New Dictionary(Of String, Dictionary(Of String, Integer)))
        '        If dicName(p.Team).ContainsKey(p.Name) = False Then
        '            dicName(p.Team).Add(p.Name, New Dictionary(Of String, Integer))
        '            For Each s As String In dicData.Keys
        '                dicName(p.Team)(p.Name).Add(s, 0)
        '            Next
        '        End If
        '        dicName(p.Team)(p.Name)(p.Site) = 1
        '    Next
        'Next

        'Dim strout As New System.Text.StringBuilder

        'For Each team As String In dicName.Keys
        '    For Each name As String In dicName(team).Keys
        '        strout.Append(team & "|" & name)
        '        For Each s As String In dicName(team)(name).Keys
        '            If dicName(team)(name)(s) = 1 Then
        '                strout.Append("|" & s)
        '            Else
        '                strout.Append("|")
        '            End If
        '        Next
        '        strout.AppendLine()
        '    Next
        'Next

        'IO.File.WriteAllText(AppContext.BaseDirectory & "test.txt", strout.ToString())
        Dim comp As New Torneo.CompilaData(appSett)
        For i As Integer = 29 To 29
            comp.ApiCompila(CStr(i))
        Next

        For i As Integer = 1 To 6
            'Torneo.CompilaData.ApiCompila(CStr(i))
        Next

        'Dim data As String = Torneo.CompilaData.ApiCompila("4")
        'IO.File.WriteAllText(AppContext.BaseDirectory & "test.json", data)
        'data = ""
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        ''Determino i link delle varie partite'
        'Dim year = "2025-26"
        'Dim enc As String = "utf-8"
        'Dim html As String = WebData.Functions.GetPage("https://www.legaseriea.it/api/stats/live/Classificacompleta?CAMPIONATO=A&STAGIONE=" & year & "&TURNO=UNICO&GIRONE=UNI", "POST", "", enc)
        'Dim strout As New System.Text.StringBuilder
        'If html <> "" Then
        '    Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
        '    Dim dict As Dictionary(Of String, Object) = jss.Deserialize(Of Dictionary(Of String, Object))(html)
        '    For Each ab As Object In dict("data")
        '        strout.Append(ab("CODSquadra") & ",")
        '        strout.Append(ab("Giocate") & ",")
        '        strout.Append(ab("VinteCasa") & ",")
        '        strout.Append(ab("VinteFuori") & ",")
        '        strout.Append(ab("PareggiateCasa") & ",")
        '        strout.Append(ab("PareggiateFuori"))
        '        strout.Append(ab("PerseCasa") & ",")
        '        strout.Append(ab("PerseFuori") & ",")
        '        strout.Append(ab("RetiFatteCasa") & ",")
        '        strout.Append(ab("RetiFatteFuori") & ",")
        '        strout.Append(ab("RetiSubiteCasa") & ",")
        '        strout.Append(ab("RetiSubiteFuori"))
        '        strout.AppendLine()
        '    Next
        '    'Dim jsonResulttodict = System.Web.Script.Serialization.JavaScriptConverter JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(rawresp)
        '    'Dim firstItem = jsonResulttodict.item("id")
        '    IO.File.WriteAllText(year & ".txt", strout.ToString())

        'End If
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim pdata As New Torneo.ClassificaData(appSett)
        pdata.ApiGetStoricoClassifica()
    End Sub

    Private Sub cmdcoppa_Click(sender As Object, e As EventArgs) Handles cmdcoppa.Click
        Dim pdata As New Torneo.CoppaData(appSett)
        pdata.ApiGetCoppa()
    End Sub

    Private Sub GetAutomaticBestHistoricalParameters()

        Exit Sub

        'Dim team As List(Of Integer) = Enumerable.Range(0, 10).ToList()
        ''Dim team As List(Of Integer) = Enumerable.Range(9, 1).ToList()

        'For g As Integer = 25 To 25

        '    Console.WriteLine("Giornata: " & g)

        '    'Dim probdata As New Torneo.ProbablePlayers(appSett)
        '    'Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", g)
        '    'Dim modules As Dictionary(Of String, String) = probdata.GetTeamModule(probable)
        '    'Dim autoForma As New Torneo.AutoFormazioniData(appSett)
        '    'autoForma.SetProbable(probable, modules)
        '    'autoForma.BestHitoricalAnalysis(g)

        'Next

        'Dim outp As New System.Text.StringBuilder

        'For g As Integer = 20 To 25
        '    Dim autoForma As New Torneo.AutoFormazioniData(appSett)
        '    For Each t As Integer In team
        '        Dim fname As String = autoForma.GetBestHistricalFileName(g, t)
        '        If IO.File.Exists(fname) Then
        '            Dim lines() As String = IO.File.ReadAllLines(fname)
        '            For Each line In lines
        '                outp.AppendLine(g & "|" & t & "|" & line.Substring(line.IndexOf("|") + 1))
        '            Next
        '        End If
        '    Next
        'Next

        'Dim fn As String = appSett.WebDataPath & "data\autoforma\total.txt"
        'IO.File.WriteAllText(fn, outp.ToString())

    End Sub

    Private Sub UpdateProbableModuleTeam()

        'Exit Sub

        For g As Integer = 25 To 27
            Console.WriteLine("Giornata: " & g)
            Dim probdata As New Torneo.ProbablePlayers(appSett)
            Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", g)
            Dim dicteam As Dictionary(Of String, String) = probdata.GetTeamModule(probable)
            probdata.UpdateTeamModule(g, dicteam)
        Next

    End Sub

    Private Sub GetAutomaticFormation()

        Dim data As New Torneo.FormazioniData(appSett)
        Dim comp As New Torneo.CompilaData(appSett)

        Dim team As List(Of Integer) = Enumerable.Range(0, 10).ToList()
        'Dim team As List(Of Integer) = Enumerable.Range(8, 1).ToList()

        Dim histData As New Dictionary(Of Integer, List(Of Torneo.AutoFormazioniData.AutoFormazione))

        For Each id As Integer In team
            histData.Add(id, New List(Of Torneo.AutoFormazioniData.AutoFormazione))
        Next

        Dim dataauto As New Torneo.AutoFormazioniData(appSett)
        Dim maxday As Integer = dataauto.GetMaxDayData()
        Dim diff As Double = 0

        Dim dicptgio As New Dictionary(Of Integer, List(Of Double))

        Torneo.Functions.EnableQueryCache = True
        Torneo.Functions.ClearQueryCache()

        'For gio As Integer = 22 To 32
        '    Dim dicBest As Dictionary(Of Integer, Dictionary(Of String, Integer)) = dataauto.BestHistoricalParamenters(gio)
        '    If dicBest.Count > 0 Then
        '        For Each id As Integer In dicBest.Keys
        '            Dim pt As Integer = Auto(id, gio, maxday, dicBest(id))
        '            Debug.WriteLine(gio & vbTab & id & vbTab & pt)
        '            If dicptgio.ContainsKey(id) = False Then dicptgio.Add(id, New List(Of Double))
        '            dicptgio(id).Add(pt / 10)
        '        Next
        '        For Each id As Integer In team
        '            Dim strout As New System.Text.StringBuilder
        '            For g As Integer = 0 To dicptgio(id).Count - 1
        '                If g > 0 Then strout.Append(vbTab)
        '                strout.Append(dicptgio(id)(g))
        '            Next
        '            Debug.WriteLine(strout.ToString())
        '        Next
        '    End If
        'Next

        'For g As Integer = 20 To 30
        '    dataauto.SetDayDataForce(g)
        '    dataauto.CheckMatchResult(g)
        'Next

        'Exit Sub

        Dim fileLog1 As String = appSett.WebDataPath & "\temp\autoformaresult1.log"
        Dim fileLog2 As String = appSett.WebDataPath & "\temp\autoformaresult2.log"
        Dim fileLog3 As String = appSett.WebDataPath & "\temp\autoformaresult3.log"
        Dim fileLog4 As String = appSett.WebDataPath & "\temp\autoformaresult4.log"

        If System.IO.File.Exists(fileLog1) Then System.IO.File.Delete(fileLog1)
        If System.IO.File.Exists(fileLog2) Then System.IO.File.Delete(fileLog2)
        If System.IO.File.Exists(fileLog3) Then System.IO.File.Delete(fileLog3)
        If System.IO.File.Exists(fileLog4) Then System.IO.File.Delete(fileLog4)

        Dim dt As Date = Date.Now

        For g As Integer = 32 To 32

            Dim sr1 As New IO.StreamWriter(fileLog1, True)
            Dim sr2 As New IO.StreamWriter(fileLog2, True)
            Dim sr3 As New IO.StreamWriter(fileLog3, True)
            Dim sr4 As New IO.StreamWriter(fileLog4, True)

            Try

                Torneo.Functions.EnableQueryCache = True
                Torneo.Functions.ClearQueryCache()

                Dim probdata As New Torneo.ProbablePlayers(appSett)
                Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", g)
                Dim probableOld As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", g - 1)
                Dim probableModule As Dictionary(Of String, String) = probdata.GetTeamModule(probable)
                Dim result As New ConcurrentBag(Of Torneo.AutoFormazioniData.AutoFormazione)
                Dim dicPlayers As New Dictionary(Of String, Torneo.Players.PlayerQuotesItem)
                Dim gio As Integer = g

                Dim pq As New WebData.PlayersQuotes(appSett)
                Dim fname As String = pq.GetBakupDataFileName(g)

                If IO.File.Exists(fname) = False Then fname = pq.GetDataFileName()
                If IO.File.Exists(fname) Then
                    Dim playersq As List(Of Torneo.Players.PlayerQuotesItem) = WebData.Functions.DeserializeJson(Of List(Of Torneo.Players.PlayerQuotesItem))(System.IO.File.ReadAllText(fname))
                    For Each p As Torneo.Players.PlayerQuotesItem In playersq
                        If dicPlayers.ContainsKey(p.Nome) = False Then dicPlayers.Add(p.Nome, p)
                    Next
                End If

                Parallel.ForEach(team, Sub(teamid)
                                           Dim id As Integer = teamid
                                           Dim dataautoTeam As New Torneo.AutoFormazioniData(appSett)
                                           dataautoTeam.SetProbable(probable, probableOld, probableModule, dicPlayers)
                                           dataautoTeam.MaxDayInArchive = maxday
                                           result.Add(dataautoTeam.GetFormazioneAutomatica(gio, id, True))
                                       End Sub)

                Dim dicResult As Dictionary(Of Integer, Torneo.AutoFormazioniData.AutoFormazione) = result.ToDictionary(Function(x) x.Formazione.TeamId)

                For Each id As Integer In team

                    Dim autoForma As New Torneo.AutoFormazioniData(appSett)
                    Dim dicpt As Dictionary(Of String, Integer) = autoForma.GetPlayerPuntiData(g, id)

                    For Each p As Torneo.FormazioniData.PlayerFormazione In dicResult(id).Formazione.Players
                        If dicpt.ContainsKey(p.Nome) Then p.Punti = dicpt(p.Nome)
                    Next
                    dicResult(id).Formazione = comp.CompileDataForma(dicResult(id).Formazione, False)
                    data.CalculatePuntiFormazione(dicResult(id).Formazione)

                    Dim dizVoto As New Dictionary(Of String, String)
                    Dim dizType As New Dictionary(Of String, Integer)

                    For i As Integer = 0 To dicResult(id).Formazione.Players.Count - 1
                        dizVoto(dicResult(id).Formazione.Players(i).Nome) = If(dicResult(id).Formazione.Players(i).Punti > -100, dicResult(id).Formazione.Players(i).Punti.ToString(), "S.V.")
                        dizType(dicResult(id).Formazione.Players(i).Nome) = dicResult(id).Formazione.Players(i).Type
                    Next

                    Dim res As String = dicResult(id).Formazione.Giornata & vbTab & dicResult(id).Formazione.TeamId & vbTab & dicResult(id).Parameters.Points / 10 & vbTab & dicResult(id).Parameters.GetKey().Replace("|", vbTab)
                    sr2.WriteLine(res)

                    sr3.WriteLine("**** Paramentri rating giornata : " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
                    sr3.WriteLine(dicResult(id).Parameters.Points / 10 & vbTab & dicResult(id).Parameters.GetKey().Replace("|", vbTab))
                    sr3.WriteLine("**** Rating giornata: " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
                    For k As Integer = 0 To dicResult(id).PlayerRating.Count - 1
                        sr3.WriteLine(dicResult(id).PlayerRating(k).Ruolo & vbTab & dicResult(id).PlayerRating(k).Nome & vbTab & dicResult(id).PlayerRating(k).Squadra & vbTab & dicResult(id).PlayerRating(k).Rating.Total1 & vbTab & dicResult(id).PlayerRating(k).Rating.Total2 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating1 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating2 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating3 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating4 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating5 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating6 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating7 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating8 & vbTab & dicResult(id).PlayerRating(k).Minuti & vbTab & dicResult(id).PlayerRating(k).Titolare & vbTab & dicResult(id).PlayerRating(k).Subentrato & vbTab & dizType(dicResult(id).PlayerRating(k).Nome) & vbTab & dizVoto(dicResult(id).PlayerRating(k).Nome))
                    Next
                    sr3.WriteLine("**** Formazione giornata: " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
                    For k As Integer = 0 To dicResult(id).Formazione.Players.Count - 1
                        sr3.WriteLine(dicResult(id).Formazione.Players(k).Ruolo & vbTab & dicResult(id).Formazione.Players(k).Nome & vbTab & dicResult(id).Formazione.Players(k).Squadra & vbTab & dicResult(id).Formazione.Players(k).Type & vbTab & dicResult(id).Formazione.Players(k).InCampo & vbTab & If(dicResult(id).Formazione.Players(k).Punti > -100, dicResult(id).Formazione.Players(k).Punti.ToString(), ""))
                    Next

                    sr4.WriteLine("**** Formazione giornata: " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
                    For k As Integer = 0 To dicResult(id).Formazione.Players.Count - 1
                        If dicResult(id).Formazione.Players(k).Type > 0 Then
                            sr4.WriteLine(dicResult(id).Formazione.Players(k).Type & vbTab & dicResult(id).Formazione.Players(k).Ruolo & vbTab & dicResult(id).Formazione.Players(k).Nome & vbTab & dicResult(id).Formazione.Players(k).Squadra)
                        End If

                    Next
                Next

                Dim lstUser As List(Of Torneo.FormazioniData.Formazione) = data.GetFormazioni(g.ToString(), "-1", Torneo.FormazioniData.TipoFormazioni.Regular)
                Dim lstTop As List(Of Torneo.FormazioniData.Formazione) = data.GetFormazioni(g.ToString(), "-1", Torneo.FormazioniData.TipoFormazioni.Top)

                If lstUser.Count > 0 Then

                    Dim tit As String = "Punteggi giornata: " & g

                    Debug.WriteLine(tit)
                    sr1.WriteLine(tit)

                    For i As Integer = 0 To lstUser.Count - 1

                        If dicResult.ContainsKey(lstUser(i).TeamId) = False Then Continue For

                        Dim lstAuto As Torneo.FormazioniData.Formazione = dicResult(lstUser(i).TeamId).Formazione
                        Dim bestAuto As Integer = PercentageBestPlayer(dicResult(lstUser(i).TeamId).Formazione.Players)
                        Dim bestUser As Integer = PercentageBestPlayer(lstUser(i).Players)

                        Dim res As String = lstAuto.Punti / 10 & vbTab & lstUser(i).Punti / 10 & vbTab & lstTop(i).Punti / 10 & vbTab & lstAuto.PlayersInCampo & vbTab & bestAuto / 10 & vbTab & bestUser / 10 & vbTab & lstAuto.BonusDifesa / 10 & vbTab & lstUser(i).BonusDifesa / 10 & vbTab & lstTop(i).BonusDifesa / 10
                        sr1.WriteLine(res)
                        Debug.WriteLine(res)
                        diff += (lstAuto.Punti - lstUser(i).Punti) / 10
                    Next

                End If
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try

            sr1.Close()
            sr1.Dispose()

            sr2.Close()
            sr2.Dispose()

            sr3.Close()
            sr3.Dispose()

            sr4.Close()
            sr4.Dispose()

            diff += 0

        Next

        Dim ad As ConcurrentDictionary(Of Integer, Double) = Torneo.AutoFormazioniData.matchdd

        Debug.WriteLine("Total pt: " & diff)
        Debug.WriteLine("Seconds: " & Date.Now.Subtract(dt).TotalSeconds())

    End Sub

    'Function PreAuto(idTeam As Integer, Giornata As Integer, maxday As Integer, Value1 As Integer) As Integer

    '    Dim comp As New Torneo.CompilaData(appSett)
    '    Dim pttot As Integer = 0
    '    Dim gStart = Giornata - 7

    '    If gStart < 20 Then gStart = 20

    '    If Giornata - 1 > gStart Then
    '        For g As Integer = gStart To Giornata - 1
    '            pttot += Auto(idTeam, g, maxday, Value1)
    '        Next
    '        Return pttot
    '    Else
    '        Return -1
    '    End If

    'End Function

    Function Auto(idTeam As Integer, Giornata As Integer, maxday As Integer, Value As Dictionary(Of String, Integer)) As Integer

        Dim data As New Torneo.FormazioniData(appSett)
        Dim comp As New Torneo.CompilaData(appSett)
        Dim probdata As New Torneo.ProbablePlayers(appSett)
        Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", Giornata)
        Dim probableOld As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", Giornata - 1)
        Dim probableModule As Dictionary(Of String, String) = probdata.GetTeamModule(probable)
        Dim result As New Torneo.AutoFormazioniData.AutoFormazione
        Dim dicPlayers As New Dictionary(Of String, Torneo.Players.PlayerQuotesItem)

        Dim pq As New WebData.PlayersQuotes(appSett)
        Dim fname As String = pq.GetBakupDataFileName(Giornata)

        If IO.File.Exists(fname) = False Then fname = pq.GetDataFileName()
        If IO.File.Exists(fname) Then
            Dim playersq As List(Of Torneo.Players.PlayerQuotesItem) = WebData.Functions.DeserializeJson(Of List(Of Torneo.Players.PlayerQuotesItem))(System.IO.File.ReadAllText(fname))
            For Each p As Torneo.Players.PlayerQuotesItem In playersq
                If dicPlayers.ContainsKey(p.Nome) = False Then dicPlayers.Add(p.Nome, p)
            Next
        End If

        Dim autoForma As New Torneo.AutoFormazioniData(appSett)
        If Value.ContainsKey("match") Then autoForma.startMatchRank = Value("match")
        If Value.ContainsKey("avg") Then autoForma.startAvgPtRank = Value("avg")
        If Value.ContainsKey("role") Then autoForma.startRouleRank = Value("role")
        If Value.ContainsKey("hp") Then autoForma.defPlayerHistory = Value("hp")
        If Value.ContainsKey("both") Then
            autoForma.startAvgPtRank = Value("both")
            autoForma.startMatchRank = Value("both")
        End If
        autoForma.SetProbable(probable, probableOld, probableModule, dicPlayers)
        autoForma.MaxDayInArchive = maxday
        result = autoForma.GetFormazioneAutomatica(Giornata, idTeam, False)

        Dim dicpt As Dictionary(Of String, Integer) = autoForma.GetPlayerPuntiData(Giornata, idTeam)

        For Each p As Torneo.FormazioniData.PlayerFormazione In result.Formazione.Players
            If dicpt.ContainsKey(p.Nome) Then p.Punti = dicpt(p.Nome)
        Next
        result.Formazione = comp.CompileDataForma(result.Formazione, False)
        data.CalculatePuntiFormazione(result.Formazione)

        Return result.Formazione.Punti

    End Function

    Private Function PercentageBestPlayer(plist As List(Of Torneo.FormazioniData.PlayerFormazione)) As Integer

        Dim bestName As New List(Of String)
        'Dim bestp = plist.GroupBy(Function(x) x.Punti).OrderByDescending(Function(g) g.Key).ToDictionary(Function(g) g.Key, Function(g) g.ToList())


        Dim bestp As List(Of Torneo.FormazioniData.PlayerFormazione) = plist.OrderByDescending(Function(x) x.Punti).ToList()
        Dim n As Integer = 0
        Dim inList As Integer = 0
        Dim outList As Integer = 0
        Dim oldpt As Integer = -1

        For Each p As Torneo.FormazioniData.PlayerFormazione In bestp
            If oldpt > p.Punti AndAlso (n >= 11 OrElse inList >= 11) Then
                Exit For
            Else
                If p.InCampo = 1 Then
                    inList += 1
                Else
                    outList += 1
                End If
                n += 1
                oldpt = p.Punti
            End If
        Next

        Dim perc As Integer = CInt((inList) * 1000 / 11)

        Return perc

    End Function

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("", False)
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("gazzetta", False, 32)
    End Sub
End Class
