Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory

Partial Class ImpExp
    Public Class ImpExpRating

        Shared Function GetHtmlHistoryFileName(ByVal Type As String, ByVal Directory As String, ByVal Extention As String) As String
            If Directory = "" Then Directory = GetLegaExpDataDirectory()
            Return Directory & "\STORICO-" & Type & "-" & currlega.Settings.Nome & "." & Extention
        End Function

        Shared Function GetHtmlFileName(ByVal Top As Boolean, ByVal Directory As String, ByVal Extention As String) As String
            If Directory = "" Then Directory = GetLegaExpDataDirectory()
            If Top Then
                Return Directory & "\CLASSIFICA-" & currlega.Settings.Nome & "-TOP." & Extention
            Else
                Return Directory & "\CLASSIFICA-" & currlega.Settings.Nome & "." & Extention
            End If
        End Function

        Public Shared Sub ExportHistoryHtml(ByVal Type As String)
            Call ExportHistoryHtml(Type, "")
        End Sub

        Public Shared Sub ExportHistoryHtml(ByVal Type As String, ByVal Directory As String)

            Dim fname As String = GetHtmlHistoryFileName(Type, Directory, "html")
            Dim width As Integer = 250
            Dim str As New System.Text.StringBuilder

            Call MakeSystemFolder(GetLegaExpDataDirectory)

            currlega.Classifica.LoadHistory()

            str.AppendLine(GetHeaderPage("STORICO " & Type & " " & currlega.Settings.Nome, "div.divtable{border-radius: 4px;box-shadow: 0px 5px 5px #888888;border:1px solid #AAA;float:left;margin-right:10px;padding:5px;}table{border-collapse:collapse;font-size:12px;font-family:Arial;font-weight:normal;}td{padding:1 2 1 2;}tr.odd {background-color:#FAFAFA;}.trbt td{border-top:1px solid #AAA;}.tdbt{border-left:1px solid #EEE;height:38px;}"))
            str.AppendLine("  <div class='divtable'>")
            str.AppendLine("    <table width='100%'>")
            str.AppendLine("      <tr><td height='40' valign='bottom'><span style='Color:#F00;font-size:22px;font-family:arial;font-weight:bold;'>STORICO " & Type & " " & currlega.Settings.Nome & "</span></td></tr>")
            str.AppendLine("      <tr>")
            str.AppendLine("        <td>")
            str.AppendLine("          <table style='table-layout:fixed;'>")
            str.AppendLine("            <tr style='color:red;'>")
            str.AppendLine(GetHeaderTableRatingHistory())
            str.AppendLine("            </tr>")
            For i As Integer = 0 To currlega.Classifica.History.Count - 1
                str.AppendLine("            <tr class='trbt'>")
                str.AppendLine("              <td><span style='color:#000;font-weight:bold;'>" & currlega.Classifica.History(i).Nome.Replace("’", "'") & "</span><br/><span style='Color:#909090;font-size:12px;font-family:arial;font-weight:normal;'>" & currlega.Classifica.History(i).Allenatore.Replace("’", "'") & "</span></td>")
                For k As Integer = 0 To currlega.Classifica.History(i).Giornate.Count - 1
                    Dim cssstr As String = ""
                    If k = 0 Then cssstr = "style='color:#00F;font-weight:bold;'"
                    If currlega.Classifica.History(i).Giornate(k).Pt <> -1 Then
                        Select Case Type
                            Case "PUNTI" : str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).Pt / 10 & "</td>")
                            Case "AMMONIOZINI" : str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).Ammonizioni & "</td>")
                            Case "ESPULSIONI" : str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).Espulsioni & "</td>")
                            Case "ASSIST" : str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).Assist & "</td>")
                            Case "GOAL SUBITI" : str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).GoalSubiti & "</td>")
                            Case "GOAL FATTI" : str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).GoalFatti & "</td>")
                            Case "VITTORIE"
                                If currlega.Classifica.History(i).Giornate(k).Vittoria > 0 Then
                                    If k = 0 Then
                                        str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).Vittoria & "</td>")
                                    Else
                                        str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">X</td>")
                                    End If
                                End If
                            Case "GIOCATE IN 10"
                                If currlega.Classifica.History(i).Giornate(k).GiocataIn10 > 0 Then
                                    If k = 0 Then
                                        str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).GiocataIn10 & "</td>")
                                    Else
                                        str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">X</td>")
                                    End If
                                End If
                            Case "JOLLY"
                                If currlega.Classifica.History(i).Giornate(k).Jolly > 0 Then
                                    If k = 0 Then
                                        str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).Jolly & "</td>")
                                    Else
                                        str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">X</td>")
                                    End If
                                End If
                            Case "POSIZIONE GIORNATA"
                                If currlega.Classifica.History(i).Giornate(k).Posizione = 1 Then
                                    str.AppendLine("<td align='center' class='tdbt' style='color:red;'><b>" & currlega.Classifica.History(i).Giornate(k).Posizione & "</b></td>")
                                Else
                                    str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).Posizione & "</td>")
                                End If
                            Case "POSIZIONE GENERALE"
                                If currlega.Classifica.History(i).Giornate(k).PosizioneGenerale = 1 Then
                                    str.AppendLine("<td align='center' class='tdbt' style='color:red;'><b>" & currlega.Classifica.History(i).Giornate(k).PosizioneGenerale & "</b></td>")
                                Else
                                    str.AppendLine("<td align='center' class='tdbt' " & cssstr & ">" & currlega.Classifica.History(i).Giornate(k).PosizioneGenerale & "</td>")
                                End If
                        End Select
                    Else
                        str.AppendLine("<td align='center' class='tdbt'>&nbsp;</td>")
                    End If

                Next
                str.AppendLine("</tr>")
            Next
            str.AppendLine("</table>")
            str.AppendLine("</td></tr>")
            str.AppendLine("</table>")
            str.AppendLine("</div>")

            IO.File.WriteAllText(fname, str.ToString)

            Call ShowCompleated("classifica", fname)

        End Sub

        Public Shared Sub ExportHtml(ByVal Top As Boolean)
            Call ExportHtml("", Top)
        End Sub

        Public Shared Sub ExportHtml(ByVal Directory As String, ByVal Top As Boolean)

            Dim fname As String = GetHtmlFileName(Top, Directory, "html")
            Dim width As Integer = 250
            Dim str As New System.Text.StringBuilder
            Dim jlist As New Dictionary(Of Integer, Integer)
            Dim pp As Double = 1

            If currlega.Classifica.PtMin <> currlega.Classifica.PtMax Then pp = 80 / (currlega.Classifica.PtMax - currlega.Classifica.PtMin + 5)

            Call MakeSystemFolder(GetLegaExpDataDirectory)

            'Determino il numero dei jolly utilizzati dai vari team'
            jlist = currlega.GetNumbersJollyUsedByTeams

            str.AppendLine(GetHeaderPage("CLASSIFICA " & currlega.Settings.Nome, "div.divtable{border-radius: 4px;box-shadow: 0px 5px 5px #888888;border:1px solid #AAA;float:left;width:1050px;margin-right:10px;padding:5px;}table{border-collapse:collapse;font-size:12px;font-family:Arial;font-weight:normal;}td{padding:1 2 1 2;}tr.odd {background-color:#FAFAFA;}.trbt td{border-top:1px solid #AAA;height:45px;}.tdbt{border-left:1px solid #EEE;height:38px;}"))
            str.AppendLine("  <div class='divtable'>")
            str.AppendLine("    <table width='100%'>")
            str.AppendLine("      <tr><td height='40' valign='bottom'><span style='Color:#F00;font-size:22px;font-family:arial;font-weight:bold;'>CLASSIFICA " & currlega.Settings.Nome & "</span></td></tr>")
            str.AppendLine("      <tr>")
            str.AppendLine("        <td>")
            str.AppendLine("          <table>")
            str.AppendLine("            <tr style='color:red;'>")
            str.AppendLine(GetHeaderTableRating())
            str.AppendLine("            </tr>")
            For i As Integer = 0 To currlega.Classifica.Item.Count - 1
                If currlega.Classifica.Item(i).WinnerDay = 1 Then
                    str.AppendLine("            <tr class='trbt' bgcolor='#FFFFC0' height='30px'>")
                Else
                    str.AppendLine("            <tr class='trbt' height='30px'>")
                End If
                Dim imgvar As Bitmap = My.Resources.empty
                If currlega.Classifica.Item(i).PreviewPostion > 0 Then
                    If currlega.Classifica.Item(i).PreviewPostion > currlega.Classifica.Item(i).Postion Then
                        imgvar = My.Resources.import16
                    ElseIf currlega.Classifica.Item(i).PreviewPostion < currlega.Classifica.Item(i).Postion Then
                        imgvar = My.Resources.export16
                    Else
                        imgvar = My.Resources.w4
                    End If
                End If
                str.AppendLine("              <td align='center'><span style='color:#F00;font-weight:bold;'>" & currlega.Classifica.Item(i).Postion & "</span></td>")
                str.AppendLine("              <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertBitMapToBase64String(imgvar) & """ /></td>")
                Dim flog As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & currlega.Classifica.Item(i).IdTeam & "-24x24.png"
                str.AppendLine("              <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertFileToBase64String(flog) & """ /></td>")
                str.AppendLine("              <td><span style='color:#000;font-weight:bold;'>" & currlega.Classifica.Item(i).Nome.Replace("’", "'") & "</span></td>")
                str.AppendLine("              <td align='center' class='tdbt'><span style='color:#00F;font-weight:bold;'>" & currlega.Classifica.Item(i).Pt & "</span></td>")
                str.AppendLine("              <td align='center' class='tdbt'><span style='color:#00F;font-weight:bold;'>" & currlega.Classifica.Item(i).PtGio & "</span></td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).PtFirst & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).PtPreviews & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).Avg & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).Min & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).Max & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).Ammonizioni & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).Espulsioni & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).Assist & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).GoalSubiti & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).GoalFatti & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).PuntiPersi & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).PercentualePuntiPersi & "%</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).PtBonus & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).NbrWinner & "</td>")
                str.AppendLine("              <td align='center' class='tdbt'>" & currlega.Classifica.Item(i).NumeroGiocateIn10 & "</td>")
                If jlist.ContainsKey(currlega.Classifica.Item(i).IdTeam) Then
                    str.AppendLine("              <td align='center' class='tdbt'>" & jlist(currlega.Classifica.Item(i).IdTeam) & "</td>")
                Else
                    str.AppendLine("              <td align='center' class='tdbt'>0</td>")
                End If
                If currlega.Classifica.Item(i).WinnerDay = 1 Then
                    str.AppendLine("              <td align='center' class='tdbt' style='background-color:" & System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(165, 255, 50)) & "'>" & currlega.Classifica.Item(i).FantaMister & "</td>")
                Else
                    str.AppendLine("              <td align='center' class='tdbt' style='background-color:" & System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(196, 255, 196)) & "'>" & currlega.Classifica.Item(i).FantaMister & "</td>")
                End If
                Dim w As Integer = CInt(10 + (currlega.Classifica.Item(i).Pt - currlega.Classifica.PtMin) * pp)
                str.AppendLine("              <td width='80'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertBitMapToBase64String(SystemFunction.DrawingAndImage.GetImageRating(New Rectangle(0, 0, 80, 30), w, 100, 1)) & """ /></td>")
                'str.AppendLine("<td width=80><table width='" & w & "' height=15 style='background-color:#0F0;border-collapse: collapse;cellspacing:0;border:1px solid #008000;margin:4px;'><tr><td>&nbsp;</td></tr></table></td>")
                str.AppendLine("</tr>")
            Next

            str.AppendLine("</table>")
            str.AppendLine("</td></tr>")
            str.AppendLine("</table>")
            str.AppendLine("</div>")
            'str.AppendLine(GetLegenda("rating"))
            str.AppendLine(GetFooterPage())

            IO.File.WriteAllText(fname, str.ToString)

            Call ShowCompleated("classifica", fname)

        End Sub

    End Class
End Class