Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory

Partial Class ImpExp
    Public Class ImpExpRose

        Public Shared Sub ExportHtml(ByVal Id As Integer, ByVal Nome As String)
            Call ExportHtml(Id, Nome, "")
        End Sub

        Public Shared Sub ExportHtml(ByVal Id As Integer, ByVal Nome As String, ByVal Directory As String)
            Dim t As New LegaObject.Team(Id, Nome)
            t.Load()
            Call ExportHtml(t, Directory)
        End Sub

        Public Shared Sub ExportHtml(ByVal Team As LegaObject.Team)
            Call ExportHtml(Team, "")
        End Sub

        Public Shared Sub ExportHtml(ByVal Team As LegaObject.Team, ByVal Directory As String)
            Dim t As New List(Of LegaObject.Team)
            t.Add(Team)
            Call ExportHtml(t, Directory)
        End Sub

        Public Shared Sub ExportHtml(ByVal Teams As List(Of LegaObject.Team))
            Call ExportHtml(Teams, "")
        End Sub

        Public Shared Sub ExportHtml(ByVal Teams As List(Of LegaObject.Team), ByVal Directory As String)

            Call MakeSystemFolder(GetLegaExpDataDirectory)

            Dim prefix As String = "ROSE"
            Dim fname As String = ""
            Dim width As Integer = 330
            Dim allrose As Boolean = False

            Select Case Teams.Count
                Case 0
                    Exit Sub
                Case 1
                    prefix = "ROSA " & Teams(0).Nome
                    fname = GetHtmlFileName(Teams(0).IdTeam, Teams(0).Nome, Directory, "html")
                Case Else
                    allrose = True
                    width = 310
                    fname = GetHtmlFileName(-1, "", Directory, "html")
            End Select

            Dim str As New System.Text.StringBuilder

            str.AppendLine(GetHeaderPage(prefix & " " & currlega.Settings.Nome, "div.divtable{border-radius: 4px;;box-shadow: 0px 5px 5px #888888;border:1px solid #AAA;float:left;width:" & width & "px;margin:10px;}.tdbt{border-top:1px solid #AAA;height:28px;}td{padding:1 2 1 2;}tr.odd {background-color:#FAFAFA;}table{border-collapse:collapse;}.trheader{background-color:#FFFF90;color:#F00;height:18px;}.nome{Color:#F00;font-size:14px;font-family:Arial;font-weight:bold;}.allenatore{Color:#AAA;font-size:11px;font-family:Arial;font-weight:bold;}.costo{Color:#0000CC;}"))
            For i As Integer = 0 To Teams.Count - 1
                str.AppendLine(GetTableHtml(Teams(i), width, allrose = True))
            Next

            'str.AppendLine("</div>")
            str.AppendLine(GetFooterPage())

            IO.File.WriteAllText(fname, str.ToString)

            Call ShowCompleated(prefix, fname)

        End Sub

        Shared Sub ImportHtml(ByVal Id As Integer, ByVal Nome As String, ByVal Directory As String)
            Dim fname As String = GetHtmlFileName(Id, Nome, Directory, "html")

            If IO.File.Exists(fname) = False Then
                Dim dlg As New System.Windows.Forms.OpenFileDialog
                dlg.InitialDirectory = Directory
                dlg.Filter = "Html files (*.html)|*.html"
                If dlg.ShowDialog = DialogResult.OK Then
                    fname = dlg.FileName
                End If
            End If

            If IO.File.Exists(fname) Then
                Dim line() As String = IO.File.ReadAllLines(fname)
                Dim teamid As Integer = 0
                For i As Integer = 0 To line.Length - 1
                    If line(i).StartsWith("<title>") Then
                        If line(i).StartsWith("<title>ROSE") = False AndAlso line(i).StartsWith("<title>ROSA") = False Then
                            ShowError("Errore", "File non valido : " & System.Environment.NewLine & fname)
                            Exit Sub
                        End If
                    ElseIf line(i).StartsWith("<!--TEAM") Then
                        teamid = CInt(System.Text.RegularExpressions.Regex.Match(line(i), "\d+").Value.Trim)
                        If Id <> -1 AndAlso teamid <> Id Then
                            ShowError("Errore", "File non valido : " & System.Environment.NewLine & fname)
                            Exit Sub
                        End If
                        currlega.Teams(teamid).Players.Clear()
                        For k As Integer = 0 To 24
                            currlega.Teams(teamid).Players.Add(New LegaObject.Team.Player(i, SystemFunction.General.GetRuoloFromId(i), "GIOCATORE " & CStr(k + 1), "", 0))
                        Next
                    ElseIf line(i).StartsWith("<!--NAME") Then
                        currlega.Teams(teamid).Nome = System.Text.RegularExpressions.Regex.Match(line(i).Trim, "(?<=<!--NAME=)[a-zA-Z0-9\’\'\.\s+]{1,}").Value.Trim.Replace("'", "’")
                    ElseIf line(i).StartsWith("<!--ALLE") Then
                        currlega.Teams(teamid).Allenatore = System.Text.RegularExpressions.Regex.Match(line(i).Trim, "(?<=<!--ALLE=)[a-zA-Z0-9\’\'\.\s+]{1,}").Value.Trim.Replace("'", "’")
                    ElseIf line(i).StartsWith("<!--PRES") Then
                        currlega.Teams(teamid).Allenatore = System.Text.RegularExpressions.Regex.Match(line(i).Trim, "(?<=<!--PRES=)[a-zA-Z0-9\’\'\.\s+]{1,}").Value.Trim.Replace("'", "’")
                    ElseIf line(i).StartsWith("<!--PLAYER") Then
                        Dim str() As String = System.Text.RegularExpressions.Regex.Match(line(i).Trim, "(?<=<!--PLAYER=)[a-zA-Z0-9\’\'\.\s+|]{1,}").Value.Trim.Split(CChar("|"))
                        If str.Length = 6 Then
                            Dim idrosa As Integer = CInt(str(0)) - 1
                            If idrosa >= 0 AndAlso idrosa < currlega.Teams(teamid).Players.Count Then
                                currlega.Teams(teamid).Players(idrosa).Riconfermato = CInt(str(1))
                                currlega.Teams(teamid).Players(idrosa).Ruolo = str(2)
                                currlega.Teams(teamid).Players(idrosa).Nome = str(3).Replace("'", "’")
                                currlega.Teams(teamid).Players(idrosa).Costo = CInt(str(4))
                                currlega.Teams(teamid).Players(idrosa).QIni = CInt(str(5))
                            End If
                        End If
                    End If
                Next
        
                currlega.Teams(0).Save(currlega.Teams, True, True)

            Else
                ShowError("Errore", "File non trovato : " & System.Environment.NewLine & fname)
            End If
        End Sub

        Public Shared Function GetTableHtml(ByVal Team As LegaObject.Team, ByVal Width As Integer, ByVal AllRose As Boolean) As String
            Dim str As New System.Text.StringBuilder
            str.AppendLine("<!--TEAM=" & Team.IdTeam & "-->")
            str.AppendLine("<!--NAME=" & Team.Nome.Replace("’", "'") & "-->")
            str.AppendLine("<!--ALLE=" & Team.Allenatore.Replace("’", "'") & "-->")
            str.AppendLine("<!--PRES=" & Team.Presidente.Replace("’", "'") & "-->")
            For i As Integer = 0 To Team.Players.Count - 1
                str.Append("<!--PLAYER=" & Team.Players(i).IdRosa)
                str.Append("|" & Team.Players(i).Riconfermato)
                str.Append("|" & Team.Players(i).Ruolo)
                str.Append("|" & Team.Players(i).Nome.Replace("’", "'"))
                str.Append("|" & Team.Players(i).Squadra.Replace("’", "'"))
                str.Append("|" & Team.Players(i).Costo)
                str.Append("|" & Team.Players(i).QIni & "-->" & System.Environment.NewLine)
            Next
            str.AppendLine("<div class='divtable'>")
            str.AppendLine("  <table style='width:" & Width - 10 & "px;margin:4px;'>")
            Dim flog As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & Team.IdTeam & "-24x24.png"
            str.AppendLine("    <tr>")
            str.AppendLine("      <td>")
            str.AppendLine("        <table>")
            str.AppendLine("          <tr><td width='30px' rowspan='3'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertFileToBase64String(flog) & """ />")
            str.AppendLine("          <tr><td style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'><span class='nome'>" & Team.Nome.Replace("’", "'") & "</span></td></tr>")
            str.AppendLine("          <tr><td style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'><span class='allenatore'>" & Team.Allenatore.Replace("’", "'") & "</span></td></tr>")
            str.AppendLine("        </table>")
            str.AppendLine("      </td></tr>")
            str.AppendLine("    </tr>")
            str.AppendLine("    <tr>")
            str.AppendLine("      <td>")
            str.AppendLine("        <table style='Color:#505050;font-size:11px;font-family:Arial;font-weight:normal;width:100%;table-layout: fixed;'>")

            str.AppendLine("          <tr class='trheader'>")
            str.Append(GetHeaderTable("            ", AllRose))
            str.AppendLine("          </tr>")
            For i As Integer = 0 To Team.Players.Count - 1
                Dim s As String = Team.Players(i).Squadra.Replace("’", "'")
                If AllRose AndAlso s.Length > 3 Then
                    s = s.Substring(0, 3)
                End If
                If CBool(i Mod 2) Then
                    str.AppendLine("          <tr class='odd'>")
                Else
                    str.AppendLine("          <tr>")
                End If
                Dim pname As String = Team.Players(i).Nome.Replace("’", "'")
                If pname.Length > 12 Then pname = pname.Substring(0, 12) & "."
                If currlega.Settings.EnableTraceReconfirmations Then
                    If Team.Players(i).Riconfermato = 1 Then
                        str.AppendLine("            <td width='20px'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.star10r) & """ /></td>")
                    Else
                        str.AppendLine("            <td width='20px'>&nbsp</td>")
                    End If
                End If
                str.AppendLine("            <td style='Color:" & System.Drawing.ColorTranslator.ToHtml(SystemFunction.General.GetRuoloForeColor(Team.Players(i).Ruolo)) & ";'>" & Team.Players(i).Ruolo & "</td>")
                If imgnatcode.ContainsKey(Team.Players(i).NatCode) Then
                    str.AppendLine("            <td><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(imgnatcode(Team.Players(i).NatCode)) & """ /></td>")
                Else
                    str.AppendLine("            <td>&nbsp;</td>")
                End If

                str.AppendLine("            <td style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'>" & pname & "</td>")
                str.AppendLine("            <td style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'>" & s & "</td>")
                str.AppendLine("            <td align='right'><span class='costo'>" & Team.Players(i).Costo & "</span></td>")
                str.AppendLine("            <td align='right'>" & Team.Players(i).QIni & "</td>")
                str.AppendLine("            <td align='right'>" & Team.Players(i).QCur & "</td>")
                Dim d As Integer = Team.Players(i).QCur - Team.Players(i).QIni
                Select Case d
                    Case Is < 0
                        str.AppendLine("            <td align='right'><span class='costo'>" & d & "</span></td>")
                    Case Is > 0
                        str.AppendLine("            <td align='right'><span style='Color:#CC0000'>" & d & "</span></td>")
                    Case Else
                        str.AppendLine("            <td align='right'><span style='Color:#505050'>" & d & "</span></td>")
                End Select
                str.AppendLine("          </tr>")
            Next
            str.AppendLine("          <tr>")
            str.AppendLine("            <td class='tdbt' colspan='5'>&nbsp;</td>")
            str.AppendLine("            <td align='right' class='tdbt'><span class='costo'>" & Team.CostoTot & "</span></td>")
            str.AppendLine("            <td align='right' class='tdbt'>" & Team.QiniTot & "</td>")
            str.AppendLine("            <td align='right' class='tdbt'>" & Team.QcurTot & "</td>")
      
            Dim d1 As Integer = Team.QcurTot - Team.QiniTot
            Select Case d1
                Case Is < 0
                    str.AppendLine("            <td align='right' class='tdbt'><span style='Color:#0000CC'>" & d1 & "</span></td>")
                Case Is > 0
                    str.AppendLine("            <td align='right' class='tdbt'><span style='Color:#CC0000'>" & d1 & "</span></td>")
                Case Else
                    str.AppendLine("            <td align='right' class='tdbt'><span style='Color:#505050'>" & d1 & "</span></td>")
            End Select
            str.AppendLine("          </tr>")
            str.AppendLine("        </table>")
            str.AppendLine("      </td>")
            str.AppendLine("    </tr>")
            str.AppendLine("  </table>")
            str.AppendLine("</div>")
            Return str.ToString
        End Function

        Private Shared Function GetHeaderTable(Space As String, ByVal AllRose As Boolean) As String
            Dim str As New System.Text.StringBuilder
            If currlega.Settings.EnableTraceReconfirmations Then str.AppendLine(Space & "<td width='20px' title='Riconfermato da mercato precendente'>C</td>")
            str.AppendLine(Space & "<td width='15px' title='Ruolo giocatore'>R.</td>")
            str.AppendLine(Space & "<td width='20px'></td>")
            str.AppendLine(Space & "<td title='Nome giocatore'>Nome</td>")
            If AllRose Then
                str.AppendLine(Space & "<td width='35px' title='Nome squadra'>Squa.</td>")
            Else
                str.AppendLine(Space & "<td width='35px' title='Nome squadra'>Squadra</td>")
            End If
            str.AppendLine(Space & "<td align='right' width='25px' title=""Prezzo d'asta del giocatore"">Cos.</td>")
            str.AppendLine(Space & "<td align='right' width='25px' title='Quotazione giocatore di inizio campionato'>Q.I.</td>")
            str.AppendLine(Space & "<td align='right' width='25px' title='Quotazione giocatore attuale'>Q.A.</td>")
            str.AppendLine(Space & "<td align='right' width='25px' title='Differenza tra la quotazione attuale e quella iniziale'>Diff.</td>")
            Return str.ToString
        End Function

        Public Shared Function GetHtmlFileName(ByVal IdTeam As Integer, ByVal Nome As String, ByVal Directory As String, ByVal Extention As String) As String
            If Directory = "" Then Directory = GetLegaExpDataDirectory()
            If IdTeam = -1 Then
                Return Directory & "\ROSE-" & currlega.Settings.Nome & "." & Extention
            Else
                Return Directory & "\ROSA-" & CStr(IdTeam).PadLeft(2, CChar("0")) & "-" & Nome.Replace(" ", "-") & "-" & currlega.Settings.Nome & "." & Extention
            End If
        End Function

    End Class
End Class