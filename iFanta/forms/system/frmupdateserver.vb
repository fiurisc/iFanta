Imports iFanta.LegaObject
Imports iFanta.SystemFunction.FileAndDirectory
Imports Microsoft.VisualBasic.Logging

Public Class frmupdateserver

    Dim start As Boolean = True
    Private WithEvents up As New System.Net.WebClient

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        IForm1.WindowsTitle = My.Application.Info.ProductName
        lbby.Text = My.Application.Info.Copyright

        IForm1.SetTheme(AppSett.Personal.Theme.Name, AppSett.Personal.Theme.FlatStyle)

        'Setto il tema corrente'
        Call SetTheme()
        'Setto l'altezza della finestra'
        Call SetWindowsHeight()

        start = False

    End Sub

    Private Sub IForm1_ThemeChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles IForm1.ThemeChange
        If start Then Exit Sub
        Call SetTheme()
    End Sub

    Sub SetTheme()

        Try

            lbtorneo2.Text = currlega.Settings.Nome

            lbtorneo2.Top = IForm1.GetTopBarTopPosition(1)
            lbtorneo2.Height = IForm1.TH1
            lbtorneo2.Left = IForm1.RX - 12 - lbtorneo2.Width
            lbtorneo2.Background = IForm1.GetTopBarImage(1)
            lbtorneo1.Top = lbtorneo2.Top
            lbtorneo1.Height = lbtorneo2.Height
            lbtorneo1.Left = lbtorneo2.Left - lbtorneo1.Width
            lbtorneo1.Background = lbtorneo2.Background

            tlbaction.Left = IForm1.RX - padd - tlbaction.Width
            tlbaction.Top = IForm1.BY - tlbaction.Height - padd \ 2
            tlbaction.FlatStyle = AppSett.Personal.Theme.FlatStyle

            ILine1.Top = tlbaction.Top - ILine1.Height - 5
            ILine1.Width = IForm1.RX - IForm1.LX - padd * 2
            ILine1.Left = IForm1.LX + padd
            prb1.Top = ILine1.Top - prb1.Height - 5
            prb1.Width = ILine1.Width
            prb1.FlatStyle = AppSett.Personal.Theme.FlatStyle
            prb1.Left = ILine1.Left
            lbstatus.Left = ILine1.Left
            lbstatus.Top = prb1.Top - lbstatus.Height - 5

            pic1.Left = padd
            pic1.Top = padd + IForm1.TY
            lbmain.Left = pic1.Left + pic1.Width + 5
            lbmain.Top = pic1.Top

            lbmain.Text = "Aggiornamento dati torneo " & currlega.Settings.Nome

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = IForm1.BY - padd \ 2 - lbby.Height

        Catch ex As Exception

        End Try

    End Sub

    Sub SetWindowsHeight()

        chkconf.Visible = chkopt.Value
        chkdata.Visible = chkopt.Value
        chkimg.Visible = chkopt.Value

        If chkopt.Value Then
            Me.Height = chkimg.Top + 120
        Else
            Me.Height = chkopt.Top + 120
        End If

    End Sub

    Sub RunUpdate()

        Dim a(2) As Boolean
        Dim err As Boolean = False

        If chkopt.Value = False OrElse chkconf.Value Then a(0) = True
        If chkopt.Value = False OrElse chkdata.Value Then a(1) = True
        If chkopt.Value = False OrElse chkimg.Value Then a(2) = True

        prb1.Max = 5

        prb1.Value = 0
        lbstatus.Text = "Pronto"

        For i As Integer = 0 To a.Length - 1
            If a(i) Then
                prb1.Value = i + 1
                Select Case i
                    Case 0
                        lbstatus.Text = "Aggiornamento file configurazioni torneo..."
                        lbstatus.Update()
                        err = SystemFunction.General.UploadFile(GetLegaSettingsFileName, currlega.Settings.Nome & "/" & IO.Path.GetFileName(SystemFunction.FileAndDirectory.GetLegaSettingsFileName), True)
                        If err = True Then Exit For
                    Case 1
                        lbstatus.Text = "Aggiornamento dati torneo..."
                        lbstatus.Update()
                        Dim flist As List(Of String) = MakeWedDataFiles.MakeFiles()
                        For Each fname In flist
                            If IO.File.Exists(fname) Then
                                err = SystemFunction.General.UploadFile(fname, "web/" & currlega.Settings.Year & "/torneo/" & IO.Path.GetFileName(fname), False)
                                If err Then Exit For
                            End If
                        Next
                        Call CompressData()
                        err = SystemFunction.General.UploadFile(GetLegaDataDirectory() & "/data.zip", currlega.Settings.Nome & "/data/data.zip", True)
                        If err = True Then Exit For
                    Case 2
                        lbstatus.Text = "Aggiornamento stemmi torneo..."
                        lbstatus.Update()
                        err = SystemFunction.General.UploadFile(GetLegaCoatOfArmsLegsDirectory() & "/stemmi.zip", currlega.Settings.Nome & "/stemmi/stemmi.zip", True)
                        If err = True Then Exit For
                End Select
                prb1.Value = i + 2
                System.Threading.Thread.Sleep(1000)
            End If
        Next
        lbstatus.Text = "Aggiornamento versioni..."
        lbstatus.Update()
        Try
            wData.GetPage(GenSett.Server & "/makeversion.aspx", "GET", "")
        Catch ex As Exception
            Call WriteError("RunUpdate", GenSett.Server & "/makeversion.aspx", ex.Message)
            err = True
        End Try
        If err Then
            prb1.Value = 0
            lbstatus.Text = "Errore"
            lbstatus.Update()
        Else
            prb1.Value = 0
            lbstatus.Text = "Completato"
            lbstatus.Update()
        End If

    End Sub

    Private Function MakeDataJavaScriptFile() As List(Of String)

        Dim frose As String = GetTempDirectory() & "\rose.txt"
        Dim fteam As String = GetTempDirectory() & "\team.txt"
        Dim fclasa As String = GetTempDirectory() & "\classifica.txt"
        Dim fform As String = GetTempDirectory() & "\formazioni.txt"
        Dim flist As New List(Of String)
        Dim sb As New System.Text.StringBuilder

        'Salvo le rose'
        currlega.LoadTeams(True, True)
        For i As Integer = 0 To currlega.Teams.Count - 1
            For k As Integer = 0 To currlega.Teams(i).Players.Count - 1
                sb.Append(i)
                sb.Append("|" & k)
                sb.Append("|" & currlega.Teams(i).Players(k).Ruolo)
                sb.Append("|" & currlega.Teams(i).Players(k).Nome)
                sb.Append("|" & currlega.Teams(i).Players(k).Squadra)
                sb.Append("|" & currlega.Teams(i).Players(k).Riconfermato)
                sb.Append("|" & currlega.Teams(i).Players(k).Costo)
                sb.Append("|" & currlega.Teams(i).Players(k).QIni)
                sb.Append("|" & currlega.Teams(i).Players(k).QCur)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.Gs)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.Gf)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.Amm)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.Esp)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.Ass)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.Avg_Pt)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.pGiocate)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticAll.Titolare)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticLast.pGiocate)
                sb.Append("|" & currlega.Teams(i).Players(k).StatisticLast.Titolare)
                sb.AppendLine("|" & currlega.Teams(i).Players(k).StatisticLast.mm)
            Next
        Next
        IO.File.WriteAllText(frose, sb.ToString())
        flist.Add(frose)

        'Salvo la lista dei team'
        sb = New System.Text.StringBuilder
        For i As Integer = 0 To currlega.Teams.Count - 1
            sb.AppendLine(i & "|" & currlega.Teams(i).Nome & "|" & currlega.Teams(i).Allenatore & "|" & SystemFunction.Convertion.ConvertFileToBase64String(GetLegaCoatOfArmsLegsDirectory() & "\" & currlega.Teams(i).IdTeam & "-24x24.png"))
        Next
        IO.File.WriteAllText(fteam, sb.ToString())
        flist.Add(fteam)

        'Salvo la lista la classifica'
        sb = New System.Text.StringBuilder
        currlega.Classifica.Load(currlega.GiornataCorrente(), False)
        For i As Integer = 0 To currlega.Classifica.Item.Count - 1
            sb.AppendLine(i & "|" & currlega.Classifica.Item(i).Postion & "|" & currlega.Classifica.Item(i).PreviewPostion & "|" & currlega.Classifica.Item(i).Nome & "|" & currlega.Classifica.Item(i).Allenatore & "|" & currlega.Classifica.Item(i).WinnerDay & "|" & currlega.Classifica.Item(i).Pt & "|" & currlega.Classifica.Item(i).PtGio & "|" & currlega.Classifica.Item(i).PtFirst & "|" & currlega.Classifica.Item(i).PtPreviews & "|" & currlega.Classifica.Item(i).Avg & "|" & currlega.Classifica.Item(i).Min & "|" & currlega.Classifica.Item(i).Max & "|" & currlega.Classifica.Item(i).PuntiPersi & "|" & currlega.Classifica.Item(i).PercentualePuntiPersi & "|" & currlega.Classifica.Item(i).NbrWinner & "|" & currlega.Classifica.Item(i).NumeroGiocateIn10)
        Next
        IO.File.WriteAllText(fclasa, sb.ToString())
        flist.Add(fclasa)

        'Salvo le formazioni'
        sb = New System.Text.StringBuilder
        For k As Integer = 1 To currlega.GiornataCorrente()
            currlega.LoadFormazioni(k, "", False)
            For j As Integer = 0 To currlega.Formazioni.Count - 1

                Dim Formazione As Formazione = currlega.Formazioni(j)

                Formazione.Players = Formazione.Players.OrderBy(Function(x) x.IdRosa).ToList()

                For i As Integer = 0 To Formazione.Players.Count - 1
                    sb.Append(k)
                    sb.Append(currlega.Formazioni(j).IdTeam)
                    sb.Append("|" & Formazione.Players(i).IdRosa)
                    sb.Append("|" & Formazione.Players(i).Jolly)
                    sb.Append("|" & Formazione.Players(i).Type)
                    sb.Append("|" & Formazione.Players(i).IdFormazione)
                    sb.Append("|" & Formazione.Players(i).InCampo)
                    sb.Append("|" & Formazione.Players(i).Ruolo)
                    sb.Append("|" & Formazione.Players(i).Nome.Replace("’", "'"))
                    sb.Append("|" & Formazione.Players(i).Squadra.Replace("’", "'"))
                    sb.Append("|" & Formazione.Players(i).Dati.Vt * 10)
                    sb.Append("|" & Formazione.Players(i).Dati.Amm)
                    sb.Append("|" & Formazione.Players(i).Dati.Esp)
                    sb.Append("|" & Formazione.Players(i).Dati.Ass)
                    sb.Append("|" & Formazione.Players(i).Dati.AutG)
                    sb.Append("|" & Formazione.Players(i).Dati.Gs)
                    sb.Append("|" & Formazione.Players(i).Dati.Gf)
                    sb.Append("|" & Formazione.Players(i).Dati.RigS)
                    sb.Append("|" & Formazione.Players(i).Dati.RigP)
                    sb.AppendLine("|" & Formazione.Players(i).Dati.Pt * 10)
                Next
                sb.AppendLine(Formazione.BonusDifesa * 10 & "|" & Formazione.BonusCentroCampo * 10 & "|" & Formazione.BonusAttacco * 10 & "|" & CInt(Formazione.ModuleSubstitution))
            Next
        Next
        IO.File.WriteAllText(fform, sb.ToString())
        flist.Add(fform)

        Return flist

    End Function

    Private Sub CompressData()

        Dim fs As String = GetLegaDataDirectory() & "\data.db"
        Dim ft As String = GetTempDirectory() & "\data.db"
        Dim fz As String = GetLegaDataDirectory() & "\data.zip"

        Try
            IO.File.Copy(fs, ft, True)
            SystemFunction.Zip.ZipFile(ft, fz)
            If IO.File.Exists(ft) Then IO.File.Delete(ft)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub tlbaction_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs, ByVal ButtonIndex As Integer) Handles tlbaction.ButtonClick
        Select Case ButtonIndex
            Case 0 : Call RunUpdate()
            Case 1 : Me.Close()
        End Select
    End Sub

    Private Sub chkopt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkopt.Click
        Call SetWindowsHeight()
    End Sub

End Class
