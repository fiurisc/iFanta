Imports System.Collections.Concurrent
Imports System.Data.OleDb
Imports System.Net
Imports System.Net.Http
Imports System.Net.Mail
Imports webfuction.Torneo.AutoFormazioniData
Imports webfuction.Torneo.ProbablePlayers

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

        GetAutomaticBestHistoricalParameters()
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

        'Torneo.Functions.CloneTableStructure(appSett, "tbformazioni", "tbtemp")

        'data.GetFormazioneAutomatica(1, 1, False)

        Dim probdata As New Torneo.ProbablePlayers(appSett)
        Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", 20)
        dataauto.SetProbable(probable)
        Dim af As Torneo.AutoFormazioniData.AutoFormazione = dataauto.GetFormazioneAutomatica(20, 8)

        'For i As Integer = 0 To 100
        '    dataauto.GetFormazioneAutomatica(2, 9, True, False)
        'Next

        For g As Integer = 7 To 9

            Dim lst As New List(Of Torneo.FormazioniData.Formazione)
            Dim lstOld As List(Of Torneo.FormazioniData.Formazione)

            'For i As Integer = 0 To 9
            '    lst.Add(dataauto.GetFormazioneAutomatica(g, i, True, i <> 0 OrElse g <> 1))
            'Next


            lstOld = data.GetFormazioni(g.ToString(), "-1", False)

            For i As Integer = 0 To 0
                Dim dicpt As Dictionary(Of String, Integer) = dataauto.GetPlayerPuntiData(g, i)
                For Each p As Torneo.FormazioniData.PlayerFormazione In lst(i).Players
                    If dicpt.ContainsKey(p.Nome) Then p.Punti = dicpt(p.Nome)
                Next
                lst(i) = comp.CompileDataForma(lst(i), False)
                data.CalculatePuntiFormazione(lst(i))
            Next

            If lstOld.Count > 0 Then
                Debug.WriteLine("Punteggi giornata: " & g)
                For i As Integer = 0 To 9
                    Debug.WriteLine(lst(i).Punti / 10 & vbTab & lstOld(i).Punti / 10 & vbTab & lst(i).PlayersInCampo)
                Next
            End If

        Next

        '

        'Dim htmlstr As String = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\tornei\2025\export\Rosa.json")
        'Dim json As String = RegularExpressions.Regex.Match(htmlstr, "(?<=\<script\>const data \= ).*(?=;\<\/script\>)").Value
        'Dim rosa As New Torneo.RoseData(appSett)
        'rosa.ApiAddRosa("0", htmlstr)
        'IO.File.WriteAllText(AppContext.BaseDirectory & "test.json", json)
        'Json = ""
        'Dim lastid As Integer = DataTorneo.GetRecordIdFromUpdate(My.Application.Info.DirectoryPath, "2025", "tbdati", 300000)
        'DataTorneo.UpdateMatchData(My.Application.Info.DirectoryPath, "2025")
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
        comp.ApiCompila("23")

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
        pdata.ApiGetClassifica("5", False)
    End Sub

    Private Sub cmdcoppa_Click(sender As Object, e As EventArgs) Handles cmdcoppa.Click
        Dim pdata As New Torneo.CoppaData(appSett)
        pdata.ApiGetCoppa()
    End Sub

    'Private Async Function StartAutomaticFormationAsync() As Task
    '    Await GetAutomaticFormationAsync()
    '    MessageBox.Show("Chiamata completata!")
    'End Function

    Private Sub GetAutomaticBestHistoricalParameters()

        Exit Sub

        'Dim team As List(Of Integer) = Enumerable.Range(0, 10).ToList()
        Dim team As List(Of Integer) = Enumerable.Range(9, 1).ToList()

        For g As Integer = 1 To 23

            Console.WriteLine("Giornata: " & g)

            Dim probdata As New Torneo.ProbablePlayers(appSett)
            Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", g)

            Dim autoForma As New Torneo.AutoFormazioniData(appSett)
            autoForma.SetProbable(probable)
            autoForma.BestHitoricalAnalysis(g)


            'For Each id As Integer In team
            '    Dim autoForma As New Torneo.AutoFormazioniData(appSett)
            '    autoForma.SetProbable(probable)
            '    autoForma.BestHitoricalAnalysis(g, id)
            'Next
        Next

    End Sub

    Private Sub GetAutomaticFormation()

        Dim data As New Torneo.FormazioniData(appSett)
        Dim comp As New Torneo.CompilaData(appSett)

        Dim fileLog1 As String = appSett.WebDataPath & "\temp\autoformaresult1.log"
        Dim fileLog2 As String = appSett.WebDataPath & "\temp\autoformaresult2.log"
        Dim fileLog3 As String = appSett.WebDataPath & "\temp\autoformaresult3.log"
        'Dim team As List(Of Integer) = Enumerable.Range(0, 10).ToList()
        Dim team As List(Of Integer) = Enumerable.Range(9, 1).ToList()

        If System.IO.File.Exists(fileLog1) Then System.IO.File.Delete(fileLog1)
        If System.IO.File.Exists(fileLog2) Then System.IO.File.Delete(fileLog2)
        If System.IO.File.Exists(fileLog3) Then System.IO.File.Delete(fileLog3)

        Dim histData As New Dictionary(Of Integer, List(Of Torneo.AutoFormazioniData.AutoFormazione))

        For Each id As Integer In team
            histData.Add(id, New List(Of Torneo.AutoFormazioniData.AutoFormazione))
        Next

        For g As Integer = 1 To 23

            Dim sr2 As New IO.StreamWriter(fileLog2, True)
            Dim sr3 As New IO.StreamWriter(fileLog3, True)

            Try
                Dim probdata As New Torneo.ProbablePlayers(appSett)
                Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", g)
                Dim result As New ConcurrentBag(Of Torneo.AutoFormazioniData.AutoFormazione)
                Dim gio As Integer = g

                Parallel.ForEach(team, Sub(teamid)
                                           Dim id As Integer = teamid

                                           Dim dataautoTeam As New Torneo.AutoFormazioniData(appSett)
                                           dataautoTeam.SetProbable(probable)
                                           result.Add(dataautoTeam.GetFormazioneAutomatica(gio, id))
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

                    sr3.WriteLine("**** Rating giornata: " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
                    For k As Integer = 0 To dicResult(id).PlayerRating.Count - 1
                        sr3.WriteLine(dicResult(id).PlayerRating(k).Ruolo & vbTab & dicResult(id).PlayerRating(k).Nome & vbTab & dicResult(id).PlayerRating(k).Squadra & vbTab & dicResult(id).PlayerRating(k).Rating.Total & vbTab & dicResult(id).PlayerRating(k).Rating.Rating1 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating2 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating3 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating4 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating5 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating6 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating7)
                    Next

                    sr3.WriteLine("**** Formazione giornata: " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
                    For k As Integer = 0 To dicResult(id).Formazione.Players.Count - 1
                        sr3.WriteLine(dicResult(id).Formazione.Players(k).Type & vbTab & dicResult(id).Formazione.Players(k).Ruolo & vbTab & dicResult(id).Formazione.Players(k).Nome & vbTab & dicResult(id).Formazione.Players(k).Squadra & vbTab & dicResult(id).Formazione.Players(k).InCampo & vbTab & If(dicResult(id).Formazione.Players(k).Punti > -100, dicResult(id).Formazione.Players(k).Punti.ToString(), ""))
                    Next

                    Dim res As String = dicResult(id).Formazione.Giornata & vbTab & dicResult(id).Formazione.TeamId & vbTab & dicResult(id).Parameters.Points / 10 & vbTab & dicResult(id).Parameters.GetKey().Replace("|", vbTab)
                    sr2.WriteLine(res)

                Next

                sr2.Close()
                sr2.Dispose()

                sr3.Close()
                sr3.Dispose()

                Dim lstOld As List(Of Torneo.FormazioniData.Formazione)

                lstOld = data.GetFormazioni(g.ToString(), "-1", False)

                If lstOld.Count > 0 Then

                    Dim tit As String = "Punteggi giornata: " & g
                    Dim sr1 As New IO.StreamWriter(fileLog1, True)

                    Debug.WriteLine(tit)
                    sr1.WriteLine(tit)

                    For i As Integer = 0 To lstOld.Count - 1
                        If dicResult.ContainsKey(lstOld(i).TeamId) = False Then Continue For
                        Dim res As String = dicResult(lstOld(i).TeamId).Formazione.Punti / 10 & vbTab & lstOld(i).Punti / 10 & vbTab & dicResult(lstOld(i).TeamId).Formazione.PlayersInCampo
                        sr1.WriteLine(res)
                        Debug.WriteLine(res)
                    Next

                    sr1.Close()
                    sr1.Dispose()

                End If
            Catch ex As Exception

            End Try

        Next

    End Sub

    'Private Sub GetAutomaticFormationAsync()

    '    Dim data As New Torneo.FormazioniData(appSett)
    '    Dim comp As New Torneo.CompilaData(appSett)

    '    Dim fileLog1 As String = appSett.WebDataPath & "\temp\autoformaresult1.log"
    '    Dim fileLog2 As String = appSett.WebDataPath & "\temp\autoformaresult2.log"
    '    Dim fileLog3 As String = appSett.WebDataPath & "\temp\autoformaresult3.log"
    '    Dim fileLog4 As String = appSett.WebDataPath & "\temp\autoformaresult4.log"

    '    If IO.File.Exists(fileLog1) Then IO.File.Delete(fileLog1)
    '    If IO.File.Exists(fileLog2) Then IO.File.Delete(fileLog2)
    '    If IO.File.Exists(fileLog3) Then IO.File.Delete(fileLog3)
    '    If IO.File.Exists(fileLog4) Then IO.File.Delete(fileLog4)

    '    Dim team As List(Of Integer) = Enumerable.Range(0, 10).ToList()
    '    'Dim team As List(Of Integer) = Enumerable.Range(3, 1).ToList()

    '    Dim histData As New Dictionary(Of Integer, List(Of Torneo.AutoFormazioniData.AutoFormazione))

    '    For Each id As Integer In team
    '        histData.Add(id, New List(Of Torneo.AutoFormazioniData.AutoFormazione))
    '    Next

    '    For g As Integer = 1 To 22

    '        Try

    '            Dim result As New Dictionary(Of Integer, Torneo.AutoFormazioniData.AutoFormazione)

    '            Dim probdata As New Torneo.ProbablePlayers(appSett)
    '            Dim probable As Dictionary(Of String, Probable) = probdata.GetProbableFormation("", g)

    '            For Each id As Integer In team
    '                result.Add(id, New Torneo.AutoFormazioniData.AutoFormazione())
    '            Next

    '            Parallel.ForEach(team, Sub(teamid)
    '                                       Dim id As Integer = teamid
    '                                       Dim dataautoTeam As New Torneo.AutoFormazioniData(appSett)
    '                                       dataautoTeam.SetProbable(probable)
    '                                       dicResult(id) = dataautoTeam.GetFormazioneAutomatica(g, id)
    '                                   End Sub)

    '            Dim sr2 As New IO.StreamWriter(fileLog2, True)
    '            Dim sr3 As New IO.StreamWriter(fileLog3, True)
    '            Dim sr4 As New IO.StreamWriter(fileLog4, True)

    '            For Each id As Integer In team
    '                Dim autoForma As New Torneo.AutoFormazioniData(appSett)
    '                Dim dicpt As Dictionary(Of String, Integer) = autoForma.GetPlayerPuntiData(g, id)
    '                For Each p As Torneo.FormazioniData.PlayerFormazione In dicResult(id).Formazione.Players
    '                    If dicpt.ContainsKey(p.Nome) Then p.Punti = dicpt(p.Nome)
    '                Next
    '                dicResult(id).Formazione = comp.CompileDataForma(result(id).Formazione, False)
    '                data.CalculatePuntiFormazione(result(id).Formazione)

    '                sr3.WriteLine("**** Rating giornata: " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
    '                For k As Integer = 0 To dicResult(id).PlayerRating.Count - 1
    '                    sr3.WriteLine(result(id).PlayerRating(k).Ruolo & vbTab & dicResult(id).PlayerRating(k).Nome & vbTab & dicResult(id).PlayerRating(k).Squadra & vbTab & dicResult(id).PlayerRating(k).Rating.Total & vbTab & dicResult(id).PlayerRating(k).Rating.Rating1 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating2 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating3 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating4 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating5 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating6 & vbTab & dicResult(id).PlayerRating(k).Rating.Rating7)
    '                Next

    '                sr3.WriteLine("**** Formazione giornata: " & dicResult(id).Formazione.Giornata & " team:" & dicResult(id).Formazione.TeamId)
    '                For k As Integer = 0 To dicResult(id).Formazione.Players.Count - 1
    '                    sr3.WriteLine(result(id).Formazione.Players(k).Type & vbTab & dicResult(id).Formazione.Players(k).Ruolo & vbTab & dicResult(id).Formazione.Players(k).Nome & vbTab & dicResult(id).Formazione.Players(k).Squadra & vbTab & dicResult(id).Formazione.Players(k).InCampo & vbTab & If(result(id).Formazione.Players(k).Punti > -100, dicResult(id).Formazione.Players(k).Punti.ToString(), ""))
    '                Next



    '                If g > 1 AndAlso histData(id).Count > 0 Then
    '                    Dim oldaforma As Torneo.AutoFormazioniData.AutoFormazione = histData(id)(histData(id).Count - 1)
    '                    Dim res1 As String = oldaforma.Formazione.Giornata & vbTab & oldaforma.Formazione.TeamId & vbTab & oldaforma.Formazione.Punti / 10 & vbTab & oldaforma.Formazione.PlayersInCampo & vbTab & oldaforma.Parameters.HistoricalPlayerData & vbTab & oldaforma.Parameters.AvarangePointsWitdh & vbTab & oldaforma.Parameters.PositionWidth & vbTab & oldaforma.Parameters.LastPresenceWitdh
    '                    Dim res2 As String = dicResult(id).Parameters.DayRef & vbTab & dicResult(id).Parameters.Points / 10 & vbTab & dicResult(id).Parameters.HistoricalPlayerData & vbTab & dicResult(id).Parameters.AvarangePointsWitdh & vbTab & dicResult(id).Parameters.PositionWidth & vbTab & dicResult(id).Parameters.LastPresenceWitdh
    '                    sr2.WriteLine(res1 & vbTab & vbTab & res2)
    '                End If

    '                histData(id).Add(Torneo.Functions.Clone(result(id)))

    '            Next

    '            sr2.Close()
    '            sr2.Dispose()

    '            sr3.Close()
    '            sr3.Dispose()

    '            sr4.Close()
    '            sr4.Dispose()

    '            Dim lstOld As List(Of Torneo.FormazioniData.Formazione)

    '            lstOld = data.GetFormazioni(g.ToString(), "-1", False)

    '            If lstOld.Count > 0 Then

    '                Dim tit As String = "Punteggi giornata: " & g
    '                Dim sr As New IO.StreamWriter(fileLog1, True)

    '                Debug.WriteLine(tit)
    '                sr.WriteLine(tit)

    '                For i As Integer = 0 To lstOld.Count - 1
    '                    If result.ContainsKey(lstOld(i).TeamId) = False Then Continue For
    '                    Dim res As String = result(lstOld(i).TeamId).Formazione.Punti / 10 & vbTab & lstOld(i).Punti / 10 & vbTab & result(lstOld(i).TeamId).Formazione.PlayersInCampo
    '                    sr.WriteLine(res)
    '                    Debug.WriteLine(res)
    '                Next

    '                sr.Close()
    '                sr.Dispose()

    '            End If

    '        Catch ex As Exception
    '            Debug.WriteLine(ex.Message)
    '        End Try

    '    Next

    'End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        Dim prob As New WebData.ProbableFormations(appSett)
        prob.GetProbableFormation("", False)
    End Sub
End Class
