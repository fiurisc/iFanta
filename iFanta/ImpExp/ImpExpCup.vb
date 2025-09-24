Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory

Partial Class ImpExp
    Public Class ImpExpCup

        Public Shared Sub ExportHtml(ByVal Cup As LegaObject.Coppa)
            Call ExportHtml(Cup, "")
        End Sub

        Public Shared Sub ExportHtml(ByVal Cup As LegaObject.Coppa, ByVal Directory As String)

            Dim fname As String = GetHtmlFileName("ITALIA", Directory, "html")
            Dim width As Integer = 250
            Dim str As New System.Text.StringBuilder

            Call MakeSystemFolder(GetLegaExpDataDirectory)

            str.AppendLine(GetHeaderPage("COPPA ITALIA", "div.divtable{border-radius: 4px;box-shadow: 0px 5px 5px #888888;border:1px solid #AAA;float:left;width:410px;margin-right:10px;padding:5 5 20 5;}div.divtable1{border-radius: 4px;box-shadow: 0px 5px 5px #888888;border:1px solid #AAA;width:410px;margin-right:10px;padding:5 5 20 5;}table{border-collapse:collapse;font-size:12px;font-family:Arial;font-weight:normal;}td{padding:1 2 1 2;}tr.odd {background-color:#FAFAFA;}.trbt td{border-top:1px solid #AAA;}.tdbt{border-left:1px solid #EEE;font-weight:bold;}.tdlast{border-left:1px solid #EEE;font-weight:bold;border-bottom:1px solid #AAA;}"))

            str.AppendLine("<div align='center'>")
            str.AppendLine("<div align='center' style='color:red;font-weight:bold;height:40px;line-height:50px;'>GIRONI ELIMINATORI</div>")
            str.AppendLine(GetHtmlGirone(Cup, Cup.GironiEliminatori, True))

            If Cup.TipoSecondoTurno = "quarti" Then
                str.AppendLine("<br/><div align='center' style='color:red;font-weight:bold;height:40px;line-height:50px;'>QUARTI DI FINALE</div>")
                str.AppendLine("<div align='center'>")
                str.AppendLine(GetHtmlFinale("Quarti di finale", Cup, Cup.QuartiDiFinale, True))
            Else
                str.AppendLine("<br/><div align='center' style='color:red;font-weight:bold;height:40px;line-height:50px;'>PLAY OFF</div>")
                str.AppendLine("<div align='center'>")
                str.AppendLine(GetHtmlGirone(Cup, Cup.PlayOff, False))
            End If

            str.AppendLine("<br/><div align='center' style='color:red;font-weight:bold;height:40px;line-height:50px;'>SEMIFINALE</div>")
            str.AppendLine("<div align='center'>")
            str.AppendLine(GetHtmlFinale("Semifinale", Cup, Cup.SemiFinali, True))

            str.AppendLine("<br/><div align='center' style='color:red;font-weight:bold;height:40px;line-height:50px;'>FINALE</div>")
            str.AppendLine("<div align='center'>")
            str.AppendLine(GetHtmlFinale("Finale", Cup, Cup.Finale, True))

            str.AppendLine("</div>")
            str.AppendLine(GetFooterPage())

            IO.File.WriteAllText(fname, str.ToString)

            Call ShowCompleated("coppe", fname)

        End Sub

        Private Shared Function GetHtmlGirone(ByVal cup As LegaObject.Coppa, ByVal gir As List(Of LegaObject.Coppa.Girone), ByVal AndRit As Boolean) As String
            Dim str As New System.Text.StringBuilder
            str.AppendLine("<table width='840px'>")
            For i As Integer = 0 To gir.Count - 1

                Dim g As New List(Of LegaObject.Coppa.Girone.ClasaGirone)
                Dim a() As LegaObject.Coppa.Girone.ClasaGirone = gir(i).Clasa.ToArray
                Dim s As New LegaObject.Coppa.Girone.ClasaSorter("pt", True)
                Array.Sort(a, s)
                g.Clear()
                g.AddRange(a)

                str.AppendLine("<td>")
                str.AppendLine("<div class='divtable'>")
                str.AppendLine("<table width='400px'>")
                str.AppendLine("<tr><td>")
                str.AppendLine("<table width='400px' style='font-weight:bold;'>")
                str.AppendLine("<tr style='color:red;' height='25px'>")
                str.AppendLine(GetHeaderClasaGirone())
                str.AppendLine("</tr>")
                For j As Integer = 0 To g.Count - 1
                    Dim sc As String = "tdbt"
                    If j = g.Count - 1 Then sc = "tdlast"
                    str.AppendLine("<tr class='trbt'>")
                    If g(j).TeamId <> -1 Then
                        str.Append("<td class='" & sc & "'>" & currlega.Teams(g(j).TeamId).Nome & "</td>")
                    Else
                        str.Append("<td class='" & sc & "'>&nbsp</td>")
                    End If
                    str.Append("<td align='center' class='" & sc & "' style='color:#F00;'>" & g(j).PartiteGiocate & "</td>")
                    str.Append("<td align='center' class='" & sc & "'>" & g(j).Vittorie & "</td>")
                    str.Append("<td align='center' class='" & sc & "'>" & g(j).Pareggi & "</td>")
                    str.Append("<td align='center' class='" & sc & "'>" & g(j).Scofitte & "</td>")
                    str.Append("<td align='center' class='" & sc & "'>" & g(j).GoalFatti & "</td>")
                    str.Append("<td align='center' class='" & sc & "'>" & g(j).GoalSubiti & "</td>")
                    str.Append("<td align='center' class='" & sc & "' style='color:#00F;'>" & g(j).Pt & "</td>")
                    str.AppendLine("</tr>")
                Next
                str.AppendLine("</table>")
                str.AppendLine("</td></tr>")
                str.AppendLine("<tr><td style='padding-top:20px'>")
                str.AppendLine("<table width='400px' style='5font-weight:bold;'>")
                For j As Integer = 0 To gir(i).Partite.Count - 1

                    If gir(i).Partite(j).Index = 0 Then
                        If AndRit Then
                            str.AppendLine("<tr style='color:red;font-weight:bold;' height=25px'>")
                            str.Append("<td>" & gir(i).Partite(j).Giornata & "&#176 Giornata (" & gir(i).Partite(j).GiornataAndata & "&#176/" & cup.GironiEliminatori(i).Partite(j).GiornataRitorno & "&#176)</td>")
                            str.Append("<td align='center' colspan=3>And.</td>")
                            str.Append("<td>|</td>")
                            str.Append("<td align='center' colspan=3>Rit.</td>")
                            str.AppendLine("</tr>")
                        Else
                            str.AppendLine("<tr style='color:red;font-weight:bold;' height=25px'>")
                            str.Append("<td>" & gir(i).Partite(j).Giornata & "&#176 Giornata (" & gir(i).Partite(j).GiornataAndata & "&#176)</td>")
                            str.Append("<td align='center' colspan=3>Ris.</td>")
                            str.AppendLine("</tr>")
                        End If
                    End If
                    str.AppendLine("<tr class='trbtw'>")
                    Dim n1 As String = "&nbsp"
                    Dim n2 As String = "&nbsp"

                    If gir(i).Partite(j).TeamId1 <> -1 Then n1 = currlega.Teams(gir(i).Partite(j).TeamId1).Nome
                    If gir(i).Partite(j).TeamId2 <> -1 Then n2 = currlega.Teams(gir(i).Partite(j).TeamId2).Nome

                    Dim strm As String = ""

                    If n1.Length > 15 Then n1 = n1.Substring(0, 14)
                    If n2.Length > 15 Then n2 = n2.Substring(0, 14)

                    strm = n1 & " - " & n2
                    str.Append("<td>" & strm & "</td>")
                    str.Append("<td>" & cup.GetGoalString(gir(i).Partite(j).GoalAnd1) & "</td>")
                    str.Append("<td>-</td>")
                    str.Append("<td>" & cup.GetGoalString(gir(i).Partite(j).GoalAnd2) & "</td>")
                    If AndRit Then
                        str.Append("<td>|</td>")
                        str.Append("<td>" & cup.GetGoalString(gir(i).Partite(j).GoalRit1) & "</td>")
                        str.Append("<td>-</td>")
                        str.Append("<td>" & cup.GetGoalString(gir(i).Partite(j).GoalRit2) & "</td>")
                    End If
                    str.AppendLine("</tr>")
                Next
                str.AppendLine("</table>")
                str.AppendLine("</td></tr>")
                str.AppendLine("</table>")
                str.AppendLine("</div>")
                str.AppendLine("</td>")
                If i < gir.Count - 1 Then str.AppendLine("<td witdh='30px'></td>")
            Next
            str.AppendLine("</table>")

            Return str.ToString
        End Function

        Private Shared Function GetHtmlFinale(ByVal PrefixMatch As String, ByVal cup As LegaObject.Coppa, ByVal par As List(Of LegaObject.Coppa.Girone.PartitaGirone), ByVal AndRit As Boolean) As String
            Dim str As New System.Text.StringBuilder
            str.AppendLine("<div class='divtable1'>")
            str.AppendLine("<table width='400px'>")
            str.AppendLine("<tr><td>")
            For j As Integer = 0 To par.Count - 1

                If par(j).Index = 0 Then
                    If AndRit Then
                        str.AppendLine("<tr style='color:red;font-weight:bold;' height=25px'>")
                        str.Append("<td>" & PrefixMatch & " (" & par(j).GiornataAndata & "&#176/" & par(j).GiornataRitorno & "&#176)</td>")
                        str.Append("<td align='center' colspan=3>And.</td>")
                        str.Append("<td>|</td>")
                        str.Append("<td align='center' colspan=3>Rit.</td>")
                        str.AppendLine("</tr>")
                    Else
                        str.AppendLine("<tr style='color:red;font-weight:bold;' height=25px'>")
                        str.Append("<td>" & PrefixMatch & " (" & par(j).GiornataAndata & "&#176)</td>")
                        str.Append("<td align='center' colspan=3>Ris.</td>")
                        str.AppendLine("</tr>")
                    End If
                End If
                str.AppendLine("<tr class='trbtw'>")
                Dim n1 As String = ""
                Dim n2 As String = ""
                Dim strm As String = ""

                If par(j).TeamId1 <> -1 Then
                    n1 = currlega.Teams(par(j).TeamId1).Nome
                End If
                If par(j).TeamId2 <> -1 Then
                    n2 = currlega.Teams(par(j).TeamId2).Nome
                End If

                If n1.Length > 15 Then n1 = n1.Substring(0, 14)
                If n2.Length > 15 Then n2 = n2.Substring(0, 14)

                strm = n1 & " - " & n2
                str.Append("<td>" & strm & "</td>")
                str.Append("<td>" & cup.GetGoalString(par(j).GoalAnd1) & "</td>")
                str.Append("<td>-</td>")
                str.Append("<td>" & cup.GetGoalString(par(j).GoalAnd2) & "</td>")
                If AndRit Then
                    str.Append("<td>|</td>")
                    str.Append("<td>" & cup.GetGoalString(par(j).GoalRit1) & "</td>")
                    str.Append("<td>-</td>")
                    str.Append("<td>" & cup.GetGoalString(par(j).GoalRit2) & "</td>")
                End If
                str.AppendLine("</tr>")
            Next
            str.AppendLine("</table>")
            str.AppendLine("</div>")

            Return str.ToString
        End Function

        Shared Function GetHtmlFileName(ByVal Type As String, ByVal Directory As String, ByVal Extention As String) As String
            If Directory = "" Then Directory = GetLegaExpDataDirectory()
            Return Directory & "\COPPA-" & Type & "-" & currlega.Settings.Nome & "." & Extention
        End Function

    End Class
End Class