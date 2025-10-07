Imports System.Net.Http
Imports System.Text
Imports Newtonsoft

Public Class Form1

    Dim year As String = "2025"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim data As String = "FORMAZIONI"
        Dim show As Boolean = True
        Dim dirs As String = My.Application.Info.DirectoryPath
        Dim str As New System.Text.StringBuilder

        WebData.Functions.Year = year
        WebData.Functions.InitPath(My.Application.Info.DirectoryPath & "\tornei\", My.Application.Info.DirectoryPath & "\tornei\")
        Torneo.PublicVariables.DataFromDatabase = True

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        WebData.PlayersQuotes.GetPlayersQuotes(False)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        WebData.PlayersData.GetPlayersData(False)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        WebData.MatchsData.GetDataMatchs(False)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        WebData.Ranking.GetRanking(False)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        WebData.ProbableFormations.GetGazzetta(False)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        WebData.ProbableFormations.GetFantacalcio(False)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        WebData.ProbableFormations.GetSky(False)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        WebData.ProbableFormations.GetPianetaFantacalcio(False)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        WebData.ProbableFormations.GetCds(False)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        'Determino i link delle varie partite'
        Dim enc As String = "utf-8"
        Dim html As String = WebData.Functions.GetPage("https://sport.sky.it/calcio/serie-a/probabili-formazioni/", enc)

        If html <> "" Then

            IO.File.WriteAllText("prova.txt", html, System.Text.Encoding.GetEncoding(enc))

        End If

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Dim htmlstr As String = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\tornei\2025\Formazioni-2025-giornata-03.html")
        Dim json As String = RegularExpressions.Regex.Match(htmlstr, "(?<=\<script\>const dati \= ).*(?=;\<\/script\>)").Value
        Torneo.FormazioniData.ApiAddFormazione("3", "-1", False, json)
        IO.File.WriteAllText(AppContext.BaseDirectory & "test.json", json)
        json = ""
        'Dim lastid As Integer = DataTorneo.GetRecordIdFromUpdate(My.Application.Info.DirectoryPath, "2025", "tbdati", 300000)
        'DataTorneo.UpdateMatchData(My.Application.Info.DirectoryPath, "2025")
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Dim data As String = Torneo.CoppaData.ApiGetCoppa()
        IO.File.WriteAllText(AppContext.BaseDirectory & "test.json", data)
        data = ""
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

End Class
