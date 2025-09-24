Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory

Partial Class ImpExp
    Public Class ImpExpStatistic

        Shared Function GetHtmlFileName(ByVal Title As String, ByVal Directory As String, ByVal Extention As String) As String
            If Directory = "" Then Directory = GetLegaExpDataDirectory()
            Return Directory & "\STATISTICHE-" & currlega.Settings.Nome & "-" & Title & "." & Extention
        End Function

        Public Shared Sub ExportHtml(ByVal List As List(Of LegaObject.Team.Player), ByVal Title As String)
            Call ExportHtml(List, Title, "")
        End Sub

        Public Shared Sub ExportHtml(ByVal List As List(Of LegaObject.Team.Player), ByVal Title As String, ByVal Directory As String)

            Dim fname As String = GetHtmlFileName(Title, Directory, "html")
            Dim width As Integer = 250
            Dim str As New System.Text.StringBuilder
            Dim pp As Double = 1
            If currlega.Classifica.PtMin <> currlega.Classifica.PtMax Then pp = 90 / (currlega.Classifica.PtMax - currlega.Classifica.PtMin + 5)

            Call MakeSystemFolder(GetLegaExpDataDirectory)

            str.AppendLine(GetHeaderPage("STATISTICHE " & Title, "div.divtable{border-radius: 4px;box-shadow: 0px 5px 5px #888888;border:1px solid #AAA;float:left;width:980px;margin-right:10px;padding:5 5 20 5;}table{border-collapse:collapse;font-size:12px;font-family:Arial;font-weight:normal;}td{padding:1 2 1 2;}tr.odd {background-color:#FAFAFA;}.trbt td{border-top:1px solid #AAA;}.tdbt{border-left:1px solid #EEE;}.tdbtlast{border-left:1px solid #EEE;background-color:#FFFFAA;}"))
            str.AppendLine("  <div class='divtable'>")
            str.AppendLine("    <table width='100%'>")
            str.AppendLine("      <tr>")
            str.AppendLine("        <td height='40' valign='bottom1'><span style='Color:#F00;font-size:22px;font-family:arial;font-weight:bold;'>Statistiche</span></td>")
            str.AppendLine("        <td height='40' valign='bottom1' align='right'><span style='vertical-align:middle;font-size:20px;font-family:arial;font-weight:bold;color:#00F;'>" & Title & "</span></td>")
            str.AppendLine("      </tr>")
            str.AppendLine("      <tr>")
            str.AppendLine("        <td colspan='2'>")
            str.AppendLine("          <table width='100%' style='table-layout:fixed;'>")
            str.AppendLine("            <tr style='color:red;'>")
            str.Append(GetHeaderTableStatistic())
            str.AppendLine("            </tr>")
            For i As Integer = 0 To List.Count - 1
                str.AppendLine("            <tr class='trbt'>")
                str.AppendLine("              <td style='Color:" & System.Drawing.ColorTranslator.ToHtml(SystemFunction.General.GetRuoloForeColor(List(i).Ruolo)) & ";'>" & List(i).Ruolo & "</td>")
                str.AppendLine("              <td align='center' class='tdbt' style='font-weight:bold;overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'>" & List(i).Nome.Replace("’", "'") & "</td>")
                str.AppendLine("              <td align='center' class='tdbt' style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'>" & List(i).Squadra.Replace("’", "'") & "</td>")
                str.AppendLine("              <td align='center' class='tdbt' style='color:#00F;font-weight:bold;'>" & List(i).Costo & "</td>")
                str.AppendLine("              <td align='center' class='tdbt' style='color:#00F;font-weight:bold;'>" & List(i).QCur & "</td>")

                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.Amm, "tdbt", "color:#FF9000;"))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.Esp, "tdbt", "color:#F00;"))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.Ass, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.Gs, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.Gf, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.RigT, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.RigS, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.RigP, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.pGiocate, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.Titolare, "tdbt", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticAll.Subentrato, "tdbt", ""))

                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.Gs, "tdbtlast", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.Gf, "tdbtlast", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.Ass, "tdbtlast", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.pGiocate, "tdbtlast", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.Titolare, "tdbtlast", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.Subentrato, "tdbtlast", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.Avg_mm, "tdbtlast", ""))
                str.AppendLine("              " & AddDataHtml(List(i).StatisticLast.mm, "tdbtlast", ""))

                str.AppendLine("              <td align='center' class='tdbt' style='color:#00F;font-weight:bold;'>" & List(i).StatisticAll.Avg_Vt & "</td>")
                str.AppendLine("              <td align='center' class='tdbt' style='color:#00F;font-weight:bold;'>" & List(i).StatisticAll.Avg_Pt & "</td>")

                str.AppendLine("            </tr>")
            Next
            str.AppendLine("          </table>")
            str.AppendLine("        </td></tr>")
            str.AppendLine("      </tr>")
            str.AppendLine("    </table>")
            str.AppendLine("  </div>")
            'str.AppendLine(GetLegenda("statistic"))
            str.AppendLine(GetFooterPage())

            IO.File.WriteAllText(fname, str.ToString)

            Call ShowCompleated("statistiche", fname)

        End Sub

        Private Shared Function AddDataHtml(data As Integer, style As String, extrastyle As String) As String
            If data > 0 Then
                Return "<td align='center' class='" & style & "' style='" & extrastyle & "'>" & data & "</td>"
            Else
                Return "<td align='center' class='" & style & "' style='" & extrastyle & "'>-</td>"
            End If
        End Function

    End Class
End Class