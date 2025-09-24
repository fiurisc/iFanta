Imports System.Net.Http
Imports System.Text
Imports Newtonsoft

Public Class Form1

    Dim year As Integer = 2025

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim data As String = "FORMAZIONI"
        Dim show As Boolean = True
        Dim dirs As String = My.Application.Info.DirectoryPath
        Dim str As New System.Text.StringBuilder

        If data = "" Then data = "ALL"

        'Dim dic As New Dictionary(Of Integer, Integer)
        'dic.Add(0, 4)
        'dic.Add(1, 2)
        'dic.Add(2, 8)
        'dic.Add(3, 0)
        'dic.Add(4, 6)
        'dic.Add(5, 3)
        'dic.Add(6, 9)
        'dic.Add(7, 7)
        'dic.Add(8, 5)
        'dic.Add(9, 1)

        'For ind As Integer = 0 To 9
        '    Dim files() As String = IO.Directory.GetFiles("D:\iFanta\tornei\2022-2023\stemmi", ind & "*")
        '    For Each f As String In files
        '        Dim fd As String = "D:\iFanta\tornei\2022-2023\stemmi\tmp\" & IO.Path.GetFileName(f).Replace(ind & "-", dic(ind) & "-")
        '        IO.File.Copy(f, fd, True)
        '    Next
        'Next

        data = data

        'If Data = "MATCH" OrElse Data = "ALL" Then
        '    Dim wData As New WebData.MatchData
        '    Str.Append(wData.GetCalendarMatchs(dirs, Show))
        'End If
        'If data = "PLAYER-QUOTES" OrElse data = "ALL" Then
        '    Dim wData As New WebData
        '    str.Append(wData.GetPlayersQuote(dirs, show))
        'End If
        'If data = "PLAYER-DATA" OrElse data = "ALL" Then
        '    Dim wData As New WebData
        '    str.Append(wData.GetPlayersData(dirs, show))
        ''End If
        'If data = "FORMAZIONI" OrElse data = "ALL" Then
        '    Dim wData As New WebData.ProbableFormations
        '    str.Append(wData.GetGazzetta(My.Application.Info.DirectoryPath, show))
        '    str.Append(wData.GetFantaGazzetta(My.Application.Info.DirectoryPath, show))
        '    str.Append(wData.GetPianetaFantacalcio(My.Application.Info.DirectoryPath, show))
        '    str.Append(wData.GetSky(My.Application.Info.DirectoryPath, show))
        '    str.Append(wData.GetCds(My.Application.Info.DirectoryPath, show))
        'End If

        'Dim s As String = str.ToString
        's = s

        'If show Then
        '    'Response.Write(str.ToString)
        'End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim wData As New WebData(year)
        wData.GetPlayersQuote(My.Application.Info.DirectoryPath, True, True)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim wData As New WebData(year)
        wData.GetPlayersData(My.Application.Info.DirectoryPath, False)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim wData As New WebData.MatchData(year)
        wData.GetCalendarMatchs(My.Application.Info.DirectoryPath, True)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim wData As New WebData(year)
        wData.GetRanking(My.Application.Info.DirectoryPath, False)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim wData As New WebData.ProbableFormations(year)
        wData.GetGazzetta(My.Application.Info.DirectoryPath, False)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim wData As New WebData.ProbableFormations(year)
        wData.GetFantacalcio(My.Application.Info.DirectoryPath, False)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim wData As New WebData.ProbableFormations(year)
        wData.GetSky(My.Application.Info.DirectoryPath, False)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim wData As New WebData.ProbableFormations(year)
        wData.GetPianetaFantacalcio(My.Application.Info.DirectoryPath, False)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim wData As New WebData.ProbableFormations(year)
        wData.GetCds(My.Application.Info.DirectoryPath, False)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        'Determino i link delle varie partite'
        Dim enc As String = "utf-8"
        Dim html As String = WebData.GetPage("https://sport.sky.it/calcio/serie-a/probabili-formazioni/", "POST", "", enc)

        If html <> "" Then

            IO.File.WriteAllText("prova.txt", html, System.Text.Encoding.GetEncoding(enc))

        End If

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click

        WebData.LoadWebPlayers(My.Application.Info.DirectoryPath & "\web\" & CStr(year) & "\data\players-quote.txt")

        'Dim line() As String = IO.File.ReadAllLines(My.Application.Info.DirectoryPath & "\web\" & CStr(year) & "\data\pform-gazzetta-player.txt")
        'Dim str As New System.Text.StringBuilder

        'For i As Integer = 0 To line.Length - 1
        '    Dim s() As String = line(i).Split(CChar("|"))
        '    If s.Length = 2 Then
        '        Dim pm As WebData.PlayerMatch = WebData.CheckName(s(0))
        '        str.AppendLine(pm.OriName & "|" & pm.NewName)
        '        'If pm.NewName <> "" Then
        '        '    str.AppendLine(pm.OriName & "|" & pm.NewName)
        '        'End If
        '    End If
        'Next
        'IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\web\" & CStr(year) & "\data\pform-gazzetta-player-res.txt", str.ToString)

        'Dim res As WebData.PlayerMatch = WebData.CheckName("PAQUETA’")
        'res = res

    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Dim data As String = WebData.GetRoseTorneo(My.Application.Info.DirectoryPath, year, 1)
        data = ""
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        'Determino i link delle varie partite'
        Dim year = "2025-26"
        Dim enc As String = "utf-8"
        Dim html As String = WebData.GetPage("https://www.legaseriea.it/api/stats/live/Classificacompleta?CAMPIONATO=A&STAGIONE=" & year & "&TURNO=UNICO&GIRONE=UNI", "POST", "", enc)
        Dim strout As New System.Text.StringBuilder
        If html <> "" Then
            Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
            Dim dict As Dictionary(Of String, Object) = jss.Deserialize(Of Dictionary(Of String, Object))(html)
            For Each ab As Dictionary(Of String, Object) In dict("data")
                strout.Append(ab("CODSQUADRA") & ",")
                strout.Append(ab("Giocate") & ",")
                strout.Append(ab("VinteCasa") & ",")
                strout.Append(ab("VinteFuori") & ",")
                strout.Append(ab("PareggiateCasa") & ",")
                strout.Append(ab("PareggiateFuori"))
                strout.Append(ab("PerseCasa") & ",")
                strout.Append(ab("PerseFuori") & ",")
                strout.Append(ab("RetiFatteCasa") & ",")
                strout.Append(ab("RetiFatteFuori") & ",")
                strout.Append(ab("RetiSubiteCasa") & ",")
                strout.Append(ab("RetiSubiteFuori"))
                strout.AppendLine()
            Next
            'Dim jsonResulttodict = System.Web.Script.Serialization.JavaScriptConverter JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(rawresp)
            'Dim firstItem = jsonResulttodict.item("id")
            IO.File.WriteAllText(year & ".txt", strout.ToString())

        End If
    End Sub

End Class
