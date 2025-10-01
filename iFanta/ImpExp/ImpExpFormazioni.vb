Imports System.Linq
Imports iFanta.SystemFunction.FileAndDirectory

Partial Class ImpExp
    Public Class ImpExpFormazioni

        Private Shared rh As Integer = 17

        Public Shared Function ExportHtml(ByVal Giornata As Integer, ByVal Formazione As LegaObject.Formazione, ByVal Top As Boolean, SuppressMessage As Boolean) As String
            Return ExportHtml(Giornata, Formazione, Top, "", SuppressMessage)
        End Function

        Public Shared Function ExportHtml(ByVal Giornata As Integer, ByVal Formazione As LegaObject.Formazione, ByVal Top As Boolean, ByVal Directory As String, SuppressMessage As Boolean) As String
            Dim f As New List(Of LegaObject.Formazione)
            f.Add(Formazione)
            Return ExportHtml(Giornata, f, "", Top, Directory, SuppressMessage)
        End Function

        Public Shared Function ExportHtml(ByVal Giornata As Integer, ByVal Id As Integer, ByVal Top As Boolean, SuppressMessage As Boolean) As String
            Dim f As List(Of LegaObject.Formazione) = currlega.GetFormazioni(Giornata, Id, Top)
            Return ExportHtml(Giornata, f, "", Top, "", SuppressMessage)
        End Function

        Public Shared Function ExportHtml(ByVal Giornata As Integer, ByVal Top As Boolean, ByVal Directory As String, SuppressMessage As Boolean) As String
            Dim f As List(Of LegaObject.Formazione) = currlega.GetFormazioni(Giornata, -1, Top)
            Return ExportHtml(Giornata, f, "", Top, Directory, SuppressMessage)
        End Function

        Public Shared Function ExportHtml(ByVal Giornata As Integer, ByVal Id As Integer, ByVal Top As Boolean, ByVal Directory As String, SuppressMessage As Boolean) As String
            Dim f As List(Of LegaObject.Formazione) = currlega.GetFormazioni(Giornata, Id, Top)
            Return ExportHtml(Giornata, f, "", Top, Directory, SuppressMessage)
        End Function

        Public Shared Function ExportHtml(ByVal Giornata As Integer, ByVal Formazioni As List(Of LegaObject.Formazione), ByVal TeamName As String, ByVal Top As Boolean, SuppressMessage As Boolean) As String
            Return ExportHtml(Giornata, Formazioni, TeamName, Top, SuppressMessage)
        End Function

        Public Shared Function ExportHtml(ByVal Giornata As Integer, ByVal Formazioni As List(Of LegaObject.Formazione), ByVal TeamName As String, ByVal Top As Boolean, ByVal Directory As String, SuppressMessage As Boolean) As String

            Dim fname As String = ""
            Dim prefix As String = "FORMAZIONI"
            Dim width As Integer = 335
            Dim h As Integer = 100 + (13 + currlega.Settings.NumberOfReserve) * rh + 15

            Call MakeSystemFolder(GetLegaExpDataDirectory)

            If Formazioni.Count > 0 Then

                If TeamName <> "" Then
                    prefix = "FORMAZIONI " & TeamName & " " & TeamName
                    fname = GetHtmlFileName(-1, -1, TeamName, Top, Directory, "html")
                Else
                    If Formazioni.Count = 1 Then
                        prefix = "FORMAZIONE " & Formazioni(0).Nome & " " & currlega.Settings.Nome & " " & Giornata & "a Giornata"
                        fname = GetHtmlFileName(Giornata, Formazioni(0).IdTeam, Formazioni(0).Nome, Top, Directory, "html")
                    Else
                        prefix = "FORMAZIONE " & currlega.Settings.Nome & " " & Giornata & "a Giornata"
                        fname = GetHtmlFileName(Giornata, -1, "", Top, Directory, "html")
                    End If
                End If

                Dim css As New System.Text.StringBuilder
                Dim str As New System.Text.StringBuilder

                If currlega.Settings.Bonus.EnableBonusDefense Then
                    h = h + rh
                End If
                If currlega.Settings.Bonus.EnableCenterField Then
                    h = h + rh
                End If
                If currlega.Settings.Bonus.EnableBonusAttack Then
                    h = h + rh
                End If
                If currlega.Settings.SubstitutionType <> LegaObject.LegaSettings.eSubstitutionType.Normal Then
                    h = h + rh
                End If

                css.AppendLine()
                css.AppendLine("table{font-size:11px;font-family:Arial;font-weight:normal;border-collapse:collapse;}")
                css.AppendLine("div.divtable1{border-radius: 4px;box-shadow: 0px 5px 5px #888888;border:1px solid #AAA;float:left;width:345px;height:" & h & "px;margin: 10px;}")
                css.AppendLine("div.divtable2{border-radius: 4px;box-shadow: 0px 5px 5px #F08080;border:1px solid #FFCC00;float:left;width:345px;height:" & h & "px;margin:10px;background-color:#FFB;}")
                css.AppendLine(".tdbt{border-top:1px solid #AAA;height:28px;}")
                css.AppendLine("table{border-collapse:collapse;border-spacing: 0;}")
                css.AppendLine("tr.odd1 {background-color:#FAFAFA;}")
                css.AppendLine("tr.odd2 {background-color:#FFFF90;}")
                css.AppendLine("td{padding:1 1 1 1;}")
                css.AppendLine(".trheader{background-color:#FFFF90;color:#F00;height:18px;}")
                css.AppendLine(".nome{Color:#F00;font-size:14px;font-family:Arial;font-weight:bold;}")
                css.AppendLine(".allenatore{Color:#AAA;font-size:11px;font-family:Arial;font-weight:bold;}")
                css.AppendLine(".costo{Color:#0000CC;}.puntitot{Color:#00F;font-size:14px;font-family:Arial;font-weight:bold;}")
                str.AppendLine(GetHeaderPage(prefix, css.ToString))

                For i As Integer = 0 To Formazioni.Count - 1
                    str.AppendLine(GetTableHtml(Formazioni(i), width))
                Next
                str.AppendLine("<div style='margin:10px'>")
                'str.AppendLine(GetLegenda("forma"))
                str.AppendLine("</div>")
                str.AppendLine(GetFooterPage())

                IO.File.WriteAllText(fname, str.ToString)

                If SuppressMessage = False Then Call ShowCompleated(prefix, fname)

            End If

            Return fname

        End Function

        Shared Sub ImportHtml(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal Nome As String, SuppressOpenFileDialog As Boolean)
            Call ImportHtml(Giornata, IdTeam, Nome, False, "", SuppressOpenFileDialog)
        End Sub

        Shared Sub ImportHtml(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal Nome As String, ByVal Directory As String, SuppressOpenFileDialog As Boolean)
            Call ImportHtml(Giornata, IdTeam, Nome, False, Directory, SuppressOpenFileDialog)
        End Sub

        Shared Sub ImportHtml(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal Nome As String, ByVal Top As Boolean, SuppressOpenFileDialog As Boolean)
            Call ImportHtml(Giornata, IdTeam, Nome, Top, SuppressOpenFileDialog)
        End Sub

        Shared Sub ImportHtml(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal Nome As String, ByVal Top As Boolean, ByVal Directory As String, SuppressOpenFileDialog As Boolean)

            Dim fname As String = GetHtmlFileName(Giornata, IdTeam, Nome, Top, Directory, "html")
            Dim checkgio As Boolean = False

            If IO.File.Exists(fname) = False AndAlso SuppressOpenFileDialog = False Then
                Dim dlg As New System.Windows.Forms.OpenFileDialog
                dlg.InitialDirectory = Directory
                dlg.Filter = "Html files (*.html)|*.html"
                If dlg.ShowDialog = DialogResult.OK Then
                    fname = dlg.FileName
                End If
            End If

            If IO.File.Exists(fname) Then

                Dim lst As New List(Of LegaObject.Formazione)
                Dim lsttop As New List(Of LegaObject.Formazione)
                Dim line() As String = IO.File.ReadAllLines(fname)
                Dim teamid As Integer = -1
                Dim f As New LegaObject.Formazione
                Dim idrosa As New List(Of String)

                For i As Integer = 0 To line.Length - 1
                    If line(i).StartsWith("<title>") Then
                        Dim ggf As Integer = CInt(System.Text.RegularExpressions.Regex.Match(line(i), "\d+(?=a\s+Giornata)").Value.Trim)
                        If ggf <> Giornata AndAlso checkgio Then
                            ShowError("Errore", "File non valido : " & System.Environment.NewLine & fname)
                            Exit Sub
                        End If
                    ElseIf line(i).StartsWith("<!--TEAM") Then
                        teamid = CInt(System.Text.RegularExpressions.Regex.Match(line(i), "\d+").Value.Trim)
                        idrosa.Clear()
                        If IdTeam <> -1 AndAlso teamid <> IdTeam Then
                            ShowError("Errore", "File non valido : " & System.Environment.NewLine & fname)
                            Exit Sub
                        End If
                        f = New LegaObject.Formazione(Giornata, teamid)
                    ElseIf line(i).StartsWith("<!--ALLE") Then
                        f.Allenatore = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=<!--ALLE=)[a-zA-Z0-9\’\'\.\s+]{1,}").Value.Trim.Replace("'", "’")
                    ElseIf line(i).StartsWith("<!--PLAYER") Then
                        Dim str() As String = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=<!--PLAYER=)[a-zA-Z0-9\s+\|\’\'\.\-\=]{1,}").Value.Trim.Split(CChar("|"))

                        If str.Length = 15 OrElse str.Length = 16 OrElse str.Length = 18 Then

                            Dim p As New LegaObject.Formazione.PlayerFormazione

                            If str.Length = 15 Then
                                'Gestione formato molto vecchio'
                                p.IdRosa = CInt(str(0))
                                p.Ruolo = str(1)
                                p.Nome = str(2).Replace("'", "’")
                                If str(3) = "0" Then
                                    p.Type = 0
                                    p.Jolly = 0
                                Else
                                    If str(4) = "0" OrElse str(4) = "2" Then
                                        p.Type = 1
                                    End If
                                    If str(4) = "1" OrElse str(4) = "3" Then
                                        p.Type = 2
                                    End If
                                    If str(4) = "2" OrElse str(4) = "3" Then
                                        p.Jolly = 1
                                    End If
                                End If
                                p.InCampo = CInt(str(5))
                                p.Dati.Vt = CSng(str(6)) / 10
                                p.Dati.Amm = CInt(str(7))
                                p.Dati.Esp = CInt(str(8))
                                p.Dati.Ass = CInt(str(9))
                                p.Dati.Gs = CInt(str(10))
                                p.Dati.Gf = CInt(str(11))
                                p.Dati.RigS = CInt(str(12))
                                p.Dati.RigP = CInt(str(13))
                                p.Dati.Pt = CSng(str(14).Replace("--", "")) / 10
                            ElseIf str.Length = 18 Then
                                If line(i).Contains("JOLLY") Then
                                    'Gestione formato 2022/23'
                                    For k As Integer = 0 To str.Length - 1
                                        Dim s() As String = str(k).Split(CChar("="))
                                        If s.Length = 1 Then
                                            p.IdRosa = CInt(s(0))
                                        ElseIf s.Length = 2 Then
                                            Select Case s(0)
                                                Case "RUOLO" : p.Ruolo = s(1)
                                                Case "NOME" : p.Nome = s(1).Replace("'", "’").ToUpper()
                                                Case "SQUADRA" : p.Squadra = s(1).Replace("'", "’").ToUpper()
                                                Case "JOLLY" : p.Jolly = CInt(s(1))
                                                Case "IDFORMAZIONE" : p.IdFormazione = If(s(1) <> "undefined", CInt(s(1)), 0)
                                                Case "TYPE" : p.Type = CInt(s(1))
                                                Case "INCAMPO" : p.InCampo = CInt(s(1))
                                                Case "VT" : p.Dati.Vt = CSng(s(1)) / 10
                                                Case "AMM" : p.Dati.Amm = CInt(s(1))
                                                Case "ESP" : p.Dati.Esp = CInt(s(1))
                                                Case "ASS" : p.Dati.Ass = CInt(s(1))
                                                Case "AUTG" : p.Dati.AutG = CInt(s(1))
                                                Case "GS" : p.Dati.Gs = CInt(s(1))
                                                Case "GF" : p.Dati.Gf = CInt(s(1))
                                                Case "RIGS" : p.Dati.RigS = CInt(s(1))
                                                Case "RIGP" : p.Dati.RigP = CInt(s(1))
                                                Case "PT" : p.Dati.Pt = CSng(s(1).Replace("--", "")) / 10
                                            End Select
                                        End If
                                    Next
                                ElseIf line(i).Contains("RUOLO") Then
                                    'Gestione formato 2021/22'
                                    For k As Integer = 0 To str.Length - 1
                                        Dim s() As String = str(k).Split(CChar("="))
                                        If s.Length = 1 Then
                                            p.IdRosa = CInt(s(0))
                                        ElseIf s.Length = 2 Then
                                            Select Case s(0)
                                                Case "RUOLO" : p.Ruolo = s(1)
                                                Case "NOME" : p.Nome = s(1).Replace("'", "’").ToUpper()
                                                Case "SCHIE"
                                                    p.Type = CInt(s(1))
                                                Case "TYPE"
                                                    If p.Type = 1 Then
                                                        Dim t As Integer = CInt(s(1))
                                                        If t = 2 OrElse t = 3 Then
                                                            p.Jolly = 1
                                                        End If
                                                        If t = 0 OrElse t = 2 Then
                                                            p.Type = 1
                                                        End If
                                                        If t = 1 OrElse t = 3 Then
                                                            p.Type = 2
                                                        End If
                                                    End If
                                                Case "INCAMPO" : p.InCampo = CInt(s(1))
                                                Case "VT" : p.Dati.Vt = CSng(s(1)) / 10
                                                Case "AMM" : p.Dati.Amm = CInt(s(1))
                                                Case "ESP" : p.Dati.Esp = CInt(s(1))
                                                Case "ASS" : p.Dati.Ass = CInt(s(1))
                                                Case "AUTG" : p.Dati.AutG = CInt(s(1))
                                                Case "GS" : p.Dati.Gs = CInt(s(1))
                                                Case "GF" : p.Dati.Gf = CInt(s(1))
                                                Case "RIGS" : p.Dati.RigS = CInt(s(1))
                                                Case "RIGP" : p.Dati.RigP = CInt(s(1))
                                                Case "PT" : p.Dati.Pt = CSng(s(1).Replace("--", "")) / 10
                                            End Select
                                        End If
                                    Next
                                Else
                                    'Gestione vecchio formato'
                                    p.IdRosa = CInt(str(0))
                                    p.Ruolo = str(1)
                                    p.Nome = str(2).Replace("'", "’")
                                    If str(3) = "0" Then
                                        p.Type = 0
                                        p.Jolly = 0
                                    Else
                                        If str(4) = "0" OrElse str(4) = "2" Then
                                            p.Type = 1
                                        End If
                                        If str(4) = "1" OrElse str(4) = "3" Then
                                            p.Type = 2
                                        End If
                                        If str(4) = "2" OrElse str(4) = "3" Then
                                            p.Jolly = 1
                                        End If
                                    End If
                                    p.InCampo = CInt(str(5))
                                    p.Dati.Vt = CSng(str(6)) / 10
                                    p.Dati.Amm = CInt(str(7))
                                    p.Dati.Esp = CInt(str(8))
                                    p.Dati.Ass = CInt(str(9))
                                    p.Dati.AutG = CInt(str(10))
                                    p.Dati.Gs = CInt(str(11))
                                    p.Dati.Gf = CInt(str(12))
                                    p.Dati.RigS = CInt(str(13))
                                    p.Dati.RigP = CInt(str(14))
                                    p.Dati.Pt = CSng(str(15).Replace("--", "")) / 10
                                End If
                            ElseIf str.Length = 17 Then
                                For k As Integer = 0 To str.Length - 1
                                    Dim s() As String = str(k).Split(CChar("="))
                                    If s.Length = 1 Then
                                        p.IdRosa = CInt(s(0))
                                    ElseIf s.Length = 2 Then
                                        Select Case s(0)
                                            Case "RUOLO" : p.Ruolo = s(1)
                                            Case "NOME" : p.Nome = s(1).Replace("'", "’").ToUpper()
                                            Case "SQUADRA" : p.Squadra = s(1).Replace("'", "’")
                                            Case "SCHIE"
                                                p.Type = CInt(s(1))
                                            Case "TYPE"
                                                If p.Type = 1 Then
                                                    Dim t As Integer = CInt(s(1))
                                                    If t = 2 OrElse t = 3 Then
                                                        p.Jolly = 1
                                                    End If
                                                    If t = 0 OrElse t = 2 Then
                                                        p.Type = 1
                                                    End If
                                                    If t = 1 OrElse t = 3 Then
                                                        p.Type = 2
                                                    End If
                                                End If
                                            Case "INCAMPO" : p.InCampo = CInt(s(1))
                                            Case "VT" : p.Dati.Vt = CSng(s(1)) / 10
                                            Case "AMM" : p.Dati.Amm = CInt(s(1))
                                            Case "ESP" : p.Dati.Esp = CInt(s(1))
                                            Case "ASS" : p.Dati.Ass = CInt(s(1))
                                            Case "AUTG" : p.Dati.AutG = CInt(s(1))
                                            Case "GS" : p.Dati.Gs = CInt(s(1))
                                            Case "GF" : p.Dati.Gf = CInt(s(1))
                                            Case "RIGS" : p.Dati.RigS = CInt(s(1))
                                            Case "RIGP" : p.Dati.RigP = CInt(s(1))
                                            Case "PT" : p.Dati.Pt = CSng(s(1).Replace("--", "")) / 10
                                        End Select
                                    End If
                                Next
                            End If

                            Dim keyp As String = CStr(p.IdRosa)
                            If idrosa.Contains(keyp) = False AndAlso p.IdRosa > 0 AndAlso (p.IdRosa < 26) Then
                                f.Players.Add(p)
                            End If
                        End If

                    ElseIf line(i).StartsWith("<!--BONUS-D") Then
                        If currlega.Settings.Bonus.EnableBonusDefense Then
                            f.BonusDifesa = CSng(CInt(System.Text.RegularExpressions.Regex.Match(line(i), "\d+").Value.Trim) / 10)
                        End If
                    ElseIf line(i).StartsWith("<!--BONUS-C") Then
                        If currlega.Settings.Bonus.EnableCenterField Then
                            f.BonusCentroCampo = CSng(CInt(System.Text.RegularExpressions.Regex.Match(line(i), "\d+").Value.Trim) / 10)
                        End If
                    ElseIf line(i).StartsWith("<!--BONUS-A") Then
                        If currlega.Settings.Bonus.EnableBonusAttack Then
                            f.BonusAttacco = CSng(CInt(System.Text.RegularExpressions.Regex.Match(line(i), "\d+").Value.Trim) / 10)
                        End If
                    ElseIf line(i).StartsWith("<!--CHANGE-MOD") Then
                        If currlega.Settings.SubstitutionType <> LegaObject.LegaSettings.eSubstitutionType.Normal Then
                            f.ModuleSubstitution = CBool(System.Text.RegularExpressions.Regex.Match(line(i), "\d+").Value.Trim)
                        End If
                    ElseIf line(i).StartsWith("<!--END") Then
                        If teamid <> -1 Then
                            lst.Add(f)
                        End If
                        teamid = -1
                    End If
                Next

                LegaObject.Formazione.Delete(Giornata, lst, Top)
                LegaObject.Formazione.Save(lst, Giornata, Top)

                If Top = False Then
                    lst = currlega.CompileTopFormazioni(lst)
                    LegaObject.Formazione.Delete(Giornata, True)
                    LegaObject.Formazione.Save(lst, Giornata, True)
                End If

            Else

                ShowError("Errore", "File non trovato : " & System.Environment.NewLine & fname)
                Call WriteError("ImpExp", "ImportFormazioniHtml ", Giornata & " " & IdTeam & " " & Nome)

            End If

        End Sub

        Shared Function GetHtmlFileName(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal Nome As String, ByVal Top As Boolean, ByVal Directory As String, ByVal Extention As String) As String
            Dim pref As String = ""
            If Directory = "" Then Directory = GetLegaExpDataDirectory()
            If Top Then pref = "TOP-"
            If Giornata = -1 Then
                Return Directory & "\" & pref & "FORMAZIONI-" & Nome.Replace(" ", "-") & "-" & currlega.Settings.Nome & "." & Extention
            Else
                If IdTeam = -1 Then
                    Return Directory & "\" & pref & "FORMAZIONI-" & currlega.Settings.Nome & "-GIO-" & CStr(Giornata).PadLeft(2, CChar("0")) & "." & Extention
                Else
                    Return Directory & "\" & pref & "FORMAZIONE-" & CStr(IdTeam).PadLeft(2, CChar("0")) & "-" & Nome.Replace(" ", "-") & "-" & currlega.Settings.Nome & "-GIO-" & CStr(Giornata).PadLeft(2, CChar("0")) & "." & Extention
                End If
            End If
        End Function

        Shared Function GetTableHtml(ByVal Formazione As LegaObject.Formazione, ByVal Width As Integer) As String
            Dim h As Integer = 50 + 21 * rh + 8
            Dim str As New System.Text.StringBuilder
            str.Append("<!--TEAM=" & Formazione.IdTeam & "-->" & System.Environment.NewLine)
            str.Append("<!--ALLE=" & Formazione.Allenatore.Replace("’", "'") & "-->" & System.Environment.NewLine)

            Formazione.Players = Formazione.Players.OrderBy(Function(x) x.IdRosa).ToList()

            For i As Integer = 0 To Formazione.Players.Count - 1
                str.Append("<!--PLAYER=" & Formazione.Players(i).IdRosa)
                str.Append("|JOLLY=" & Formazione.Players(i).Jolly)
                str.Append("|TYPE=" & Formazione.Players(i).Type)
                str.Append("|IDFORMAZIONE=" & Formazione.Players(i).IdFormazione)
                str.Append("|INCAMPO=" & Formazione.Players(i).InCampo)
                str.Append("|RUOLO=" & Formazione.Players(i).Ruolo)
                str.Append("|NOME=" & Formazione.Players(i).Nome.Replace("’", "'"))
                str.Append("|SQUADRA=" & Formazione.Players(i).Squadra.Replace("’", "'"))
                str.Append("|VT=" & Formazione.Players(i).Dati.Vt * 10)
                str.Append("|AMM=" & Formazione.Players(i).Dati.Amm)
                str.Append("|ESP=" & Formazione.Players(i).Dati.Esp)
                str.Append("|ASS=" & Formazione.Players(i).Dati.Ass)
                str.Append("|AUTG=" & Formazione.Players(i).Dati.AutG)
                str.Append("|GS=" & Formazione.Players(i).Dati.Gs)
                str.Append("|GF=" & Formazione.Players(i).Dati.Gf)
                str.Append("|RIGS=" & Formazione.Players(i).Dati.RigS)
                str.Append("|RIGP=" & Formazione.Players(i).Dati.RigP)
                str.Append("|PT=" & Formazione.Players(i).Dati.Pt * 10 & "-->" & System.Environment.NewLine)
            Next
            If currlega.Settings.Bonus.EnableBonusDefense Then
                str.AppendLine("<!--BONUS-D=" & Formazione.BonusDifesa * 10 & "-->")
                h = h + rh
            End If
            If currlega.Settings.Bonus.EnableCenterField Then
                str.AppendLine("<!--BONUS-C=" & Formazione.BonusCentroCampo * 10 & "-->")
                h = h + rh
            End If
            If currlega.Settings.Bonus.EnableBonusAttack Then
                str.AppendLine("<!--BONUS-A=" & Formazione.BonusAttacco * 10 & "-->")
                h = h + rh
            End If
            If currlega.Settings.SubstitutionType <> LegaObject.LegaSettings.eSubstitutionType.Normal Then
                str.AppendLine("<!--CHANGE-MOD=" & CInt(Formazione.ModuleSubstitution) & "-->")
                h = h + rh
            End If

            Dim flog As String = SystemFunction.FileAndDirectory.GetLegaCoatOfArmsLegsDirectory & "\" & Formazione.IdTeam & "-24x24.png"

            Formazione.Players = Formazione.Players.OrderBy(Function(x) x.IdFormazione).ToList()

            str.AppendLine("<!--END-->")
            If Formazione.NumberPlayerInCampo < 11 AndAlso Formazione.NumberPlayerInCampo > 0 Then
                str.AppendLine("<div class='divtable2'>")
            Else
                str.AppendLine("<div class='divtable1'>")
            End If
            str.AppendLine("  <table style='width:" & Width & "px;height:" & h & "px;margin:4px;'>")
            str.AppendLine("    <tr height='50px'>")
            str.AppendLine("      <td>")
            str.AppendLine("        <table style='width:100%;table-layout: fixed;'>")
            str.AppendLine("          <tr>")
            str.AppendLine("            <td width='30px' rowspan='2'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertFileToBase64String(flog) & """ /></td>")
            str.AppendLine("            <td><span class='nome' style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'>" & Formazione.Nome.Replace("’", "'") & "</span></td>")
            str.AppendLine("            <td width='80px' align='right'><span class='puntitot'>pt:&nbsp;" & Formazione.Pt & "</span></td>")
            str.AppendLine("          </tr>")
            str.AppendLine("          <tr>")
            str.AppendLine("            <td><span class='allenatore'>" & Formazione.Allenatore.Replace("’", "'") & "</span></td>")
            str.AppendLine("            <td width='80px' align='right' style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'><span class='allenatore'>modulo:&nbsp;" & Formazione.Modulo & "</span></td>")
            str.AppendLine("          </tr>")
            str.AppendLine("        </table>")
            str.AppendLine("      </td>")
            str.AppendLine("    </tr>")
            str.AppendLine("    <tr height='" & rh * 12 & "px'>")
            str.AppendLine("      <td valign='top'>")
            str.AppendLine("        <table style='width:100%;table-layout:fixed;'>")
            str.AppendLine("          <tr class='trheader' height='" & rh & "px'>")
            str.Append(GetHeaderTable())
            str.AppendLine("          </tr>")
            str.AppendLine(GetTableRowPlayer(Formazione, 1))
            str.AppendLine("        </table>")
            str.AppendLine("      </td>")
            str.AppendLine("    </tr>")
            str.AppendLine("    <tr height='" & rh * 8 & "px'>")
            str.AppendLine("      <td valign='top'>")
            str.AppendLine("        <table style='width:100%;table-layout:fixed;'>")
            str.AppendLine("          <tr class='trheader'>")
            str.Append(GetHeaderTable())
            str.AppendLine("          </tr>")
            str.Append(GetTableRowPlayer(Formazione, 2))
            str.AppendLine("        </table>")
            str.AppendLine("      </td>")
            str.AppendLine("    </tr>")
            str.AppendLine("    <tr><td height='4px'></td></tr>")
            'Determino l'eventuale bonus difesa'
            If currlega.Settings.Bonus.EnableBonusDefense Then
                Dim strb As String = "Nessun bonus difesa"
                If Formazione.BonusDifesa > 0 Then
                    If currlega.Settings.Bonus.BonusDefense(3) = Formazione.BonusDifesa * 10 Then
                        strb = "Bonus difesa a 3 (+" & currlega.Settings.Bonus.BonusDefense(3) / 10 & "pt)"
                    ElseIf currlega.Settings.Bonus.BonusDefense(4) = Formazione.BonusDifesa * 10 Then
                        strb = "Bonus difesa a 4 (+" & currlega.Settings.Bonus.BonusDefense(4) / 10 & "pt)"
                    ElseIf currlega.Settings.Bonus.BonusDefense(5) = Formazione.BonusDifesa * 10 Then
                        strb = "Bonus difesa a 5 (+" & currlega.Settings.Bonus.BonusDefense(5) / 10 & "pt)"
                    End If
                End If
                str.AppendLine("    <tr>")
                str.AppendLine("      <td>")
                str.AppendLine("        <table width='100%'>")
                str.AppendLine("          <tr><td height='" & rh & "px'><span style='Color:#505050;font-size:11px;font-family:Arial;font-weight:bold;'>&nbsp;*&nbsp;" & strb & "</span></td><td align='right'><span style='Color:#505050;font-size:13px;font-family:Arial;font-weight:bold;'>" & Formazione.NumberPlayerInCampo & "/11</span></td></tr>")
                str.AppendLine("        </table>")
                str.AppendLine("      </td>")
                str.AppendLine("    </tr>")
            End If
            'Determino l'eventuale bonus centrocampo'
            If currlega.Settings.Bonus.EnableCenterField Then
                Dim strb As String = "Nessun bonus centrocampo"
                If Formazione.BonusCentroCampo > 0 Then
                    If currlega.Settings.Bonus.BonusCenterField(3) = Formazione.BonusCentroCampo * 10 Then
                        strb = "Bonus centrocampo a 3 (+" & currlega.Settings.Bonus.BonusCenterField(3) / 10 & "pt)"
                    ElseIf currlega.Settings.Bonus.BonusCenterField(4) = Formazione.BonusCentroCampo * 10 Then
                        strb = "Bonus centrocampo a 4 (+" & currlega.Settings.Bonus.BonusCenterField(4) / 10 & "pt)"
                    ElseIf currlega.Settings.Bonus.BonusCenterField(5) = Formazione.BonusCentroCampo * 10 Then
                        strb = "Bonus centrocampo a 5 (+" & currlega.Settings.Bonus.BonusCenterField(5) / 10 & "pt)"
                    End If
                End If
                str.AppendLine("    <tr><td colspan='2' height='" & rh & "px'><span style='Color:#505050;font-size:11px;font-family:Arial;font-weight:bold;'>&nbsp;*&nbsp;" & strb & "</span></td></tr>")
            End If
            'Determino l'eventuale bonus attacco'
            If currlega.Settings.Bonus.EnableBonusAttack Then
                Dim strb As String = "Nessun bonus attacco"
                If Formazione.BonusAttacco > 0 Then
                    If currlega.Settings.Bonus.BonusAttack(2) = Formazione.BonusAttacco * 10 Then
                        strb = "Bonus attacco a 2 (+" & currlega.Settings.Bonus.BonusAttack(2) / 10 & "pt)"
                    ElseIf currlega.Settings.Bonus.BonusAttack(3) = Formazione.BonusAttacco * 10 Then
                        strb = "Bonus attacco a 3 (+" & currlega.Settings.Bonus.BonusAttack(3) / 10 & "pt)"
                    End If
                End If
                str.AppendLine("    <tr><td colspan='2' height='" & rh & "px'><span style='Color:#505050;font-size:11px;font-family:Arial;font-weight:bold;'>&nbsp;*&nbsp;" & strb & "</span></td></tr>")
            End If
            If currlega.Settings.SubstitutionType <> LegaObject.LegaSettings.eSubstitutionType.Normal Then
                Dim strb As String = "Nessun cambio modulo"
                If Formazione.ModuleSubstitution = True Then strb = "Cambio modulo eseguito"
                str.AppendLine("    <tr><td colspan='2' height='" & rh & "px'><span style='Color:#505050;font-size:11px;font-family:Arial;font-weight:bold;'>&nbsp;*&nbsp;" & strb & "</span></td></tr>")
            End If
            str.AppendLine("  </table>")
            str.AppendLine("</div>")
            Return str.ToString
        End Function

        Private Shared Function GetTableRowPlayer(ByVal Formazione As LegaObject.Formazione, ByVal Type As Integer) As String

            Dim str As New System.Text.StringBuilder
            Dim f As Integer = 11
            Dim c As Integer = 0

            If Type = 2 Then f = currlega.Settings.NumberOfReserve

            For i As Integer = 0 To Formazione.Players.Count - 1
                Dim classstr As String = ""
                If CBool(c Mod 2) Then
                    If Formazione.NumberPlayerInCampo < 11 AndAlso Formazione.NumberPlayerInCampo > 0 Then
                        classstr = " class='odd2'"
                    Else
                        classstr = " class='odd1'"
                    End If
                End If
                If Formazione.Players(i).Type = Type Then
                    Dim rs As String = "&nbsp;"
                    If Formazione.Players(i).InCampo = 1 Then
                        str.AppendLine("          <tr style='color:black;'" & classstr & " height='" & rh & "px'>")
                        If Formazione.Players(i).Type = 1 Then rs = "*" Else rs = "&nbsp;"
                        str.AppendLine("            <td>" & rs & "</td>")
                        str.AppendLine("            <td style='Color:" & System.Drawing.ColorTranslator.ToHtml(SystemFunction.General.GetRuoloForeColor(Formazione.Players(i).Ruolo)) & ";'>" & Formazione.Players(i).Ruolo & "</td>")
                    Else
                        str.AppendLine("          <tr style='color:#AAA;'" & classstr & " height='" & rh & "px'>")
                        str.AppendLine("            <td>&nbsp;</td>")
                        str.AppendLine("            <td style='Color:" & System.Drawing.ColorTranslator.ToHtml(SystemFunction.General.GetRuoloForeColorDisable(Formazione.Players(i).Ruolo)) & ";'>" & Formazione.Players(i).Ruolo & "</td>")
                    End If
                    Dim n As String = Formazione.Players(i).Nome.Replace("’", "'")
                    str.AppendLine("            <td>")
                    str.AppendLine("              <table style='width:130px;table-layout:fixed;'>")
                    str.AppendLine("                <tr style='color:black;'" & classstr & " height='" & rh & "px'>")
                    str.AppendLine("                  <td style='overflow:hidden;white-space:nowrap;text-overflow:ellipsis;'>" & n & "</td>")
                    If Formazione.Players(i).Jolly > 0 Then
                        str.AppendLine("                  <td width='15px'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.star10) & """ />")
                    Else
                        str.AppendLine("                  <td width='15px'>&nbsp;</td>")
                    End If
                    str.AppendLine("                </tr>")
                    str.AppendLine("              </table>")
                    str.AppendLine("            </td>")
                    If Formazione.Players(i).Dati.Vt > -10 Then
                        str.AppendLine("            <td align='center'>" & Formazione.Players(i).Dati.Vt & "</td>")
                    Else
                        If Formazione.Players(i).Dati.Vt = -20 Then
                            str.AppendLine("            <td align='center'>s.v.</td>")
                        Else
                            str.AppendLine("            <td align='center'>-</td>")
                        End If
                    End If
                    If Formazione.Players(i).Dati.Vt > -10 Then
                        If Formazione.Players(i).Dati.Amm > 0 Then
                            If Formazione.Players(i).InCampo = 1 Then
                                str.AppendLine("            <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.ammonito) & """ /></td>")
                            Else
                                str.AppendLine("            <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertBitMapToBase64String(SystemFunction.DrawingAndImage.ConvDisable(My.Resources.ammonito, 0.5)) & """ /></td>")
                            End If
                        Else
                            str.AppendLine("            <td align='center'>&nbsp;</td>")
                        End If
                        If Formazione.Players(i).Dati.Esp > 0 Then
                            If Formazione.Players(i).InCampo = 1 Then
                                str.AppendLine("            <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.espulso) & """ /></td>")
                            Else
                                str.AppendLine("            <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertBitMapToBase64String(SystemFunction.DrawingAndImage.ConvDisable(My.Resources.espulso, 0.5)) & """ /></td>")
                            End If
                        Else
                            str.AppendLine("            <td align='center'>&nbsp;</td>")
                        End If
                        If Formazione.Players(i).Dati.Ass > 0 Then
                            If Formazione.Players(i).InCampo = 1 Then
                                str.AppendLine("            <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.ass) & """ /></td>")
                            Else
                                str.AppendLine("            <td align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertBitMapToBase64String(SystemFunction.DrawingAndImage.ConvDisable(My.Resources.ass, 0.5)) & """ /></td>")
                            End If
                        Else
                            str.AppendLine("            <td align='center'>&nbsp;</td>")
                        End If
                        If Formazione.Players(i).Dati.AutG > 0 Then
                            If Formazione.Players(i).InCampo = 1 Then
                                str.AppendLine("            <td align='center'><span><img <img style='margin-top:-3px;vertical-align:middle' src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.autogoal16) & """ /></span>" & Formazione.Players(i).Dati.AutG & "</td>")
                            Else
                                str.AppendLine("            <td align='center'><span><img style='margin-top:-3px;vertical-align:middle' src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertBitMapToBase64String(SystemFunction.DrawingAndImage.ConvDisable(My.Resources.autogoal16, 0.5)) & """ /></span>" & Formazione.Players(i).Dati.AutG & "</td>")
                            End If
                        Else
                            str.AppendLine("            <td align='center'>&nbsp;</td>")
                        End If

                        If Formazione.Players(i).Dati.Gs + Formazione.Players(i).Dati.Gf > 0 Then
                            If Formazione.Players(i).InCampo = 1 Then
                                str.AppendLine("            <td align='center'><span><img <img style='margin-top:-3px;vertical-align:middle' src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.pallone16) & """ /></span>" & Formazione.Players(i).Dati.Gs + Formazione.Players(i).Dati.Gf & "</td>")
                            Else
                                str.AppendLine("            <td align='center'><span><img style='margin-top:-3px;vertical-align:middle' src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertBitMapToBase64String(SystemFunction.DrawingAndImage.ConvDisable(My.Resources.pallone16, 0.5)) & """ /></span>" & Formazione.Players(i).Dati.Gs + Formazione.Players(i).Dati.Gf & "</td>")
                            End If
                        Else
                            str.AppendLine("            <td align='center'>&nbsp;</td>")
                        End If

                        If Formazione.Players(i).Dati.RigS + Formazione.Players(i).Dati.RigP > 0 Then
                            str.AppendLine("            <td align='center'>" & Formazione.Players(i).Dati.RigS & "/" & Formazione.Players(i).Dati.RigP & "</td>")
                        Else
                            str.AppendLine("            <td align='center'>&nbsp;</td>")
                        End If

                    Else
                        str.AppendLine("            <td align='center'>&nbsp;</td>")
                        str.AppendLine("            <td align='center'>&nbsp;</td>")
                        str.AppendLine("            <td align='center'>&nbsp;</td>")
                        str.AppendLine("            <td align='center'>&nbsp;</td>")
                        str.AppendLine("            <td align='center'>&nbsp;</td>")
                        str.AppendLine("            <td align='center'>&nbsp;</td>")
                    End If
                    If Formazione.Players(i).InCampo = 1 Then
                        If Formazione.Players(i).Dati.Pt > -10 Then
                            str.AppendLine("            <td align='center' style='Color:#00F;'>" & Formazione.Players(i).Dati.Pt & "</td>")
                        Else
                            If Formazione.Players(i).Dati.Pt = -20 Then
                                str.AppendLine("            <td align='center' style='Color:#00F;'>s.v.</td>")
                            Else
                                str.AppendLine("            <td align='center' style='Color:#00F;'>-</td>")
                            End If
                        End If
                    Else
                        If Formazione.Players(i).Dati.Pt > -10 Then
                            str.AppendLine("            <td align='center' style='Color:#9090FF;'>" & Formazione.Players(i).Dati.Pt & "</td>")
                        Else
                            If Formazione.Players(i).Dati.Pt = -20 Then
                                str.AppendLine("            <td align='center' style='Color:#9090FF;'>s.v.</td>")
                            Else
                                str.AppendLine("            <td align='center' style='Color:#9090FF;'>-</td>")
                            End If
                        End If
                    End If
                    str.AppendLine("          </tr>")
                    c = c + 1
                End If
            Next
            If c <> f Then
                For i As Integer = c To f - 1
                    str.AppendLine("          <tr><td colspan='10'>&nbsp;</td></tr>")
                Next
            End If

            Return str.ToString

        End Function

        Shared Function GetHeaderTable() As String
            Dim str As New System.Text.StringBuilder
            str.AppendLine("            <td width='5px' align='center'></td>")
            str.AppendLine("            <td width='12px' align='center'>R.</td>")
            str.AppendLine("            <td>Nome</td>")
            str.AppendLine("            <td width='15px' align='center'>vt</td>")
            str.AppendLine("            <td width='15px' align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.ammonito) & """ /></td>")
            str.AppendLine("            <td width='15px' align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.espulso) & """ /></td>")
            str.AppendLine("            <td width='20px' align='center'><img src=""data:image/jpeg;base64," & SystemFunction.Convertion.ConvertImageToBase64String(My.Resources.ass) & """ /></td>")
            str.AppendLine("            <td width='23px' align='center'>ag</td>")
            str.AppendLine("            <td width='23px' align='center'>gs/f</td>")
            str.AppendLine("            <td width='23px' align='center'>r.s/p</td>")
            str.AppendLine("            <td width='20px' align='center'>pt</td>")
            Return str.ToString
        End Function

    End Class
End Class