
Imports System.Data
Imports System.Threading

Namespace WebData
    Public Class ProbableMatchResult

        Dim appSett As New Torneo.PublicVariables
        Dim mdataw As MatchsData
        Dim dirTemp As String = ""
        Dim dirData As String = ""

        Sub New(appSett As Torneo.PublicVariables)
            Me.appSett = appSett
            mdataw = New MatchsData(appSett)
            dirTemp = appSett.WebDataPath & "temp\"
            dirData = appSett.WebDataPath & "data\pmatchs\"
        End Sub


        Public Function GetProbableMatchResult(show As Boolean) As String

            Dim rmsg As String = ""
            Dim fileTemp As String = dirTemp & "pronostici.txt"
            Dim fileLog As String = dirData & "pronostici.log"
            Dim str As New System.Text.StringBuilder
            Dim logs As New System.Text.StringBuilder

            Try

                Dim matchs As List(Of Torneo.MatchsData.Match) = mdataw.GetMatchsData()

                Dim html As String = Functions.GetPage(appSett, "https://www.superpronostici.it/pronostici/calcio/it/serie-a", "UTF-8")
                IO.File.WriteAllText(fileTemp, html, System.Text.Encoding.Default)

                logs.AppendLine("Loading matchs")
                logs.AppendLine("Year -> " & appSett.Year)
                logs.AppendLine("Calendario match:")
                logs.AppendLine("---------------------------")
                For Each t As Torneo.MatchsData.Match In matchs
                    logs.AppendLine(t.Giornata & " -> " & t.TeamA & " vs " & t.TeamB)
                Next
                logs.AppendLine(matchs.Count.ToString())


                If html <> "" Then

                    logs.AppendLine("Reading html page")

                    Dim start As Boolean = False
                    Dim sq As New List(Of String)
                    Dim sqid As Integer = 0
                    Dim pstate As String = "Titolare"
                    Dim team As String = ""

                    Dim wdata As New Dictionary(Of Integer, List(Of Torneo.ProbableMatchResult.ProbableResult))
                    Dim giornata As Integer = -1
                    Dim hometeam As String = ""
                    Dim awayteam As String = ""

                    Dim m1 As Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(IO.File.ReadAllText(fileTemp, System.Text.Encoding.Default), "(""giornata"":\d+,""matchId"":\d+,""homeTeam"":""\w+"",""awayTeam"":""\w+"")|(""prob_1"":[\d\.]{1,},""prob_x"":[\d\.]{1,},""prob_2"":[\d\.]{1,})")
                    Dim wstart As New List(Of Integer)

                    For Each m As Text.RegularExpressions.Match In m1

                        If m.Value.Contains("matchId") Then
                            giornata = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=giornata"":)\d+").Value)
                            Dim matchid As String = System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=matchId"":)\d+").Value
                            hometeam = Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=homeTeam"":)""\w+""").Value.Replace("""", "").Replace("homeTeam:", ""))
                            awayteam = Functions.CheckTeamName(System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=awayTeam"":)""\w+""").Value.Replace("""", "").Replace("awayTeam:", ""))
                            logs.AppendLine("Match -> " & matchid & " - " & hometeam & " vs " & awayteam)
                            If wstart.Contains(giornata) AndAlso matchs.Where(Function(x) x.Giornata = (giornata) AndAlso CDate(x.Time) < Date.Now).Count > 0 Then
                                wstart.Add(giornata)
                            End If
                        ElseIf m.Value.Contains("prob_1") Then
                            Dim prob1 As Integer = Convert.ToInt32(Convert.ToDouble(System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=prob_1"":)[\d\.]{1,}").Value.Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)))
                            Dim probx As Integer = Convert.ToInt32(Convert.ToDouble(System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=prob_x"":)[\d\.]{1,}").Value.Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)))
                            Dim prob2 As Integer = Convert.ToInt32(Convert.ToDouble(System.Text.RegularExpressions.Regex.Match(m.Value, "(?<=prob_2"":)[\d\.]{1,}").Value.Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)))
                            If prob1 + probx + prob2 <> 100 Then
                                prob2 = 100 - prob1 - probx
                            End If
                            logs.AppendLine("Probabilities -> 1: " & prob1 & ", X: " & probx & ", 2: " & prob2)
                            If wdata.ContainsKey(giornata) = False Then wdata.Add(giornata, New List(Of Torneo.ProbableMatchResult.ProbableResult))
                            wdata(giornata).Add(New Torneo.ProbableMatchResult.ProbableResult With {.Match = hometeam & "-" & awayteam, .TeamHome = hometeam, .TeamAway = awayteam, .Prob1 = Convert.ToInt32(prob1), .ProbX = Convert.ToInt32(probx), .Prob2 = Convert.ToInt32(prob2)})

                        End If
                    Next

                    For Each g As Integer In wdata.Keys
                        If wstart.Contains(g) = False Then
                            Dim fileName As String = dirData & "\" & g & ".json"
                            If IO.Directory.Exists(dirData) = False Then IO.Directory.CreateDirectory(dirData)
                            Dim out As String = WriteData(wdata(g), fileName)
                            rmsg = WebData.Functions.SerializzaOggetto(wdata(g), False)
                        End If
                    Next
                End If

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
                rmsg = ex.Message
            End Try

            IO.File.WriteAllText(fileLog, logs.ToString)

            Return rmsg

        End Function


        Function GetDataFileName(giornata As Integer) As String
            Return appSett.WebDataPath & "data\pmatchs\" & giornata & ".json"
        End Function

        Public Function WriteData(Data As List(Of Torneo.ProbableMatchResult.ProbableResult), fileDestiNazione As String) As String

            Dim json As String = ""
            Try
                json = WebData.Functions.SerializzaOggetto(Data, False)
                IO.File.WriteAllText(fileDestiNazione, json, System.Text.Encoding.Default)
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return json

        End Function

    End Class
End Namespace