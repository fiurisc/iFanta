Imports System.Net
Imports System.Security.Cryptography.X509Certificates

Namespace SystemFunction

    Public Class General


        Public Function AcceptAllCertifications(ByVal sender As Object, ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
            Return True
        End Function

        Public Shared Function SetDataClipBoardHtmlFormat(ByVal html As String, ByVal Title As String) As DataObject
            Dim dat As New DataObject()
            Try
                Dim prefix As String = "<html><head><title>HTML clipboard</title></head><body>"
                Dim suffix As String = "</body>"
                Dim header As String = "Version:0.9" & System.Environment.NewLine & "StartHTML:AAAAAA" & System.Environment.NewLine & "EndHTML:BBBBBB" & System.Environment.NewLine & "StartFragment:CCCCCC" & System.Environment.NewLine & "EndFragment:DDDDDD" & System.Environment.NewLine
                Dim result As String = header & prefix & html & suffix

                result = result.Replace("AAAAAA", header.Length.ToString("D6")).Replace("BBBBBB", result.Length.ToString("D6")).Replace("CCCCCC", (header + prefix).Length.ToString("D6")).Replace("DDDDDD", (header + prefix + html).Length.ToString("D6"))

                dat.SetData(DataFormats.Html, result)

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
            Return dat
        End Function

        Public Shared Sub CopyDataTable(ByVal All As Boolean, ByVal Dtg As DataGridView)

            Try
                Dim fname1 As String = SystemFunction.FileAndDirectory.GetTempDirectory & "\copy1.txt"
                Dim fname2 As String = SystemFunction.FileAndDirectory.GetTempDirectory & "\copy2.txt"

                Dim sw1 As New IO.StreamWriter(fname1)
                Dim sw2 As New IO.StreamWriter(fname2)

                sw1.WriteLine("<table border='0'>")

                If All Then
                    sw1.WriteLine("<tr>")
                    For k As Integer = 0 To Dtg.ColumnCount - 1
                        If Dtg.Columns(k).CellType.ToString.Contains("TextBox") Then
                            'Determino il colore della cella'
                            Dim ft As Font = Dtg.Columns(k).HeaderCell.Style.Font
                            If ft Is Nothing Then ft = Dtg.ColumnHeadersDefaultCellStyle.Font
                            If ft Is Nothing Then ft = Dtg.Columns(k).DefaultCellStyle.Font
                            If ft Is Nothing Then ft = Dtg.DefaultCellStyle.Font
                            Dim cl As Color = Dtg.Columns(k).HeaderCell.Style.ForeColor
                            If cl = Color.Empty Then cl = Dtg.ColumnHeadersDefaultCellStyle.ForeColor
                            If cl = Color.Empty Then cl = Dtg.Columns(k).DefaultCellStyle.ForeColor
                            If cl = Color.Empty Then cl = Dtg.DefaultCellStyle.ForeColor
                            sw1.WriteLine("<td style='color:" & System.Drawing.ColorTranslator.ToHtml(cl) & ";font-size:" & CStr(CInt(Math.Ceiling(ft.Size))) & "pt;font-family:" & ft.FontFamily.Name & ";font-weight:normal;'>" & Dtg.Columns(k).HeaderText & "</td>")
                            sw2.Write(Microsoft.VisualBasic.ChrW(9) & Dtg.Columns(k).HeaderText)
                        End If
                    Next
                    sw1.WriteLine("</tr>")
                End If
                Try
                    For i As Integer = 0 To Dtg.RowCount - 1

                        Dim found As Boolean = False

                        For k As Integer = 0 To Dtg.ColumnCount - 1
                            If Dtg.Columns(k).CellType.ToString.Contains("TextBox") Then
                                If Dtg.Item(k, i).Selected = True OrElse All = True Then
                                    If found = False Then sw1.WriteLine("<tr'>")
                                    'Determino il colore della cella'
                                    Dim ft As Font = Dtg.Item(k, i).Style.Font
                                    If ft Is Nothing Then ft = Dtg.Columns(k).DefaultCellStyle.Font
                                    If ft Is Nothing Then ft = Dtg.DefaultCellStyle.Font
                                    Dim cl As Color = Dtg.Item(k, i).Style.ForeColor
                                    If cl = Color.Empty Then cl = Dtg.Columns(k).DefaultCellStyle.ForeColor
                                    If cl = Color.Empty Then cl = Dtg.DefaultCellStyle.ForeColor
                                    Dim bl As Color = Dtg.Rows(i).DefaultCellStyle.BackColor
                                    If bl = Color.Empty Then bl = Dtg.DefaultCellStyle.BackColor

                                    Dim value As String = ""
                                    If Dtg.Columns(k).CellType.ToString.Contains("TextBox") Then
                                        value = CStr(Dtg.Rows(i).Cells(k).Value)
                                    Else
                                        value = CStr(Dtg.Rows(i).Cells(k).Tag)
                                    End If

                                    'Gestione clipboard html'
                                    Dim strike1 As String = ""
                                    Dim strike2 As String = ""
                                    Dim bold As String = "normal"
                                    If ft.Bold = True AndAlso Dtg.Columns(k).CellType.ToString.Contains("TextBox") Then bold = "bold"
                                    If ft.Strikeout = True AndAlso Dtg.Columns(k).CellType.ToString.Contains("TextBox") Then strike1 = "<strike>" : strike2 = "</strike>"
                                    sw1.WriteLine("<td bgcolor='" & System.Drawing.ColorTranslator.ToHtml(bl) & "' style='color:" & System.Drawing.ColorTranslator.ToHtml(cl) & ";font-size:" & CStr(CInt(Math.Ceiling(ft.Size))) & "pt;font-family:" & ft.FontFamily.Name & ";font-weight:" & bold & ";'>" & strike1 & "" & value & strike2 & "</td>")

                                    'Gestione clipboard text'
                                    If found = False Then
                                        sw2.Write(value)
                                    Else
                                        sw2.Write(Microsoft.VisualBasic.ChrW(9) & value)
                                    End If
                                    found = True
                                End If
                            End If
                        Next
                        If found = True Then
                            sw1.WriteLine("</tr>")
                            sw2.WriteLine("")
                        End If
                    Next

                Catch ex As Exception
                    Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
                End Try

                sw1.WriteLine("</table>")

                sw1.Flush()
                sw1.Close()
                sw1.Dispose()

                sw2.Flush()
                sw2.Close()
                sw2.Dispose()

                Dim dat As New DataObject()

                'Gestione clipboard html'
                If IO.File.Exists(fname1) Then
                    dat = SetDataClipBoardHtmlFormat(IO.File.ReadAllText(fname1), "COPY")
                End If

                'Gestione clipboard text'
                If IO.File.Exists(fname2) Then
                    dat.SetData(DataFormats.Text, IO.File.ReadAllText(fname2))
                End If

                'Setto i dati nella clipboard'
                My.Computer.Clipboard.SetDataObject(dat)

            Catch ex As Exception
                ShowError("Error", ex.Message)
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub

        Public Shared Sub LoadImageCache()

            matchimgkey.Clear()
            matchpreskey.Clear()
            matchratingkey.Clear()

            Try
                Dim patt As String = "match-n-*.tmp"
                If AppSett.Personal.Theme.FlatStyle Then patt = "match-f-*.tmp"
                Dim f() As String = IO.Directory.GetFiles(SystemFunction.FileAndDirectory.GetTempImageDirectory, patt)
                For i As Integer = 0 To f.Length - 1
                    Dim s() As String = IO.Path.GetFileNameWithoutExtension(f(i)).Split(CChar("-"))
                    If s.Length = 5 Then
                        matchimgkey.Add(s(2) & "-" & s(3) & "-" & s(4), CType(Image.FromFile(f(i)), Bitmap))
                        If f(i) = "unkm" Then
                            matchimgkey.Add("?-?-?", CType(Image.FromFile(f(i)), Bitmap))
                        End If
                    End If
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
            Try
                Dim patt As String = "pres-n-*.tmp"
                If AppSett.Personal.Theme.FlatStyle Then patt = "pres-f-*.tmp"
                Dim f() As String = IO.Directory.GetFiles(SystemFunction.FileAndDirectory.GetTempImageDirectory, patt)
                For i As Integer = 0 To f.Length - 1
                    Dim s() As String = IO.Path.GetFileNameWithoutExtension(f(i)).Split(CChar("-"))
                    If s.Length = 5 Then
                        matchpreskey.Add(s(2) & "-" & s(3) & "-" & s(4), CType(Image.FromFile(f(i)), Bitmap))
                    End If
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
            Try
                Dim patt As String = "rat-n-*.tmp"
                If AppSett.Personal.Theme.FlatStyle Then patt = "rat-f-*.tmp"
                Dim f() As String = IO.Directory.GetFiles(SystemFunction.FileAndDirectory.GetTempImageDirectory, patt)
                For i As Integer = 0 To f.Length - 1
                    Dim s() As String = IO.Path.GetFileNameWithoutExtension(f(i)).Split(CChar("-"))
                    If s.Length = 3 Then
                        matchratingkey.Add(CInt(s(2)), CType(Image.FromFile(f(i)), Bitmap))
                    End If
                Next
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

        End Sub

        Public Shared Function GetRatingRosa(ByVal Player As LegaObject.Team.Player, ByVal RatingType As LegaObject.Team.eRatingType) As Integer

            Dim rt As Integer = -1

            If webdata.DayProbableFormation > 1 Then
                Select Case RatingType
                    Case LegaObject.Team.eRatingType.RendimentoCosto
                        If Player.Costo > 0 Then
                            rt = CInt(((Player.StatisticAll.Sum_Pt / (webdata.DayProbableFormation - 1)) / Player.Costo) * 50)
                        Else
                            rt = 0
                        End If
                    Case LegaObject.Team.eRatingType.RendimentoQuotazione

                        If Player.QCur > 0 Then
                            rt = CInt((Player.StatisticAll.Sum_Pt / Player.QCur) / webdata.DayProbableFormation)
                        Else
                            rt = 0
                        End If

                    Case Else

                        Dim rt1 As Integer = -1
                        Dim rt2 As Integer = -1
                        Dim avg As Double = Player.StatisticAll.Avg_Pt

                        If avg < 4 Then avg = 4
                        If avg > 12 Then avg = 12
                        rt1 = CInt(rt1 + avg * 5)

                        If Player.StatisticAll.nPartite > Player.StatisticLast.nPartite Then
                            rt2 = CInt(((Player.StatisticAll.pGiocate - Player.StatisticLast.pGiocate) / (Player.StatisticAll.nPartite - Player.StatisticLast.nPartite)) * 5)
                            If Player.StatisticLast.nPartite > 0 Then
                                rt2 = rt2 + CInt((Player.StatisticLast.pGiocate / Player.StatisticLast.nPartite) * 20) + Player.Variation * 5
                            End If
                        End If

                        rt2 = rt2 + CInt((Player.StatisticLast.Avg_mm - 45) / 20)
                        rt = rt1 + rt2

                End Select
            Else
                If Player.QCur > 0 Then
                    rt = Player.QCur * 4
                Else
                    rt = 0
                End If
            End If

            If rt > 100 Then rt = 100
            If rt < 0 Then rt = 0

            Return rt

        End Function

        Public Shared Function GetRatingForma(ByVal Player As LegaObject.Formazione.PlayerFormazione, ByVal Giornata As Integer) As Integer

            Dim rt As Integer = -1

            Try
                Dim fc As Boolean = True 'flag fuori casa"
                Dim nome As String = Player.Nome
                Dim fteam As Integer = 40
                Dim rtini As Double = 0
                Dim rtteam As Double = 0
                Dim rtp As Double = 0
                Dim avv As String = ""
                Dim k As Integer = 4
                Dim avg As Double = 0

                'Detemino se la squadra gioca in casa'
                If Player.Match.TeamA <> Player.Squadra Then
                    avv = Player.Match.TeamA
                    fc = True
                Else
                    fc = False
                    avv = Player.Match.TeamB
                End If

                Dim wt As New wData.wTeam

                If webdata.WebTeams.ContainsKey(Player.Squadra) Then wt = webdata.WebTeams(Player.Squadra)

                If GenSett.TeamRating.ContainsKey(Player.Squadra) Then rtini = GenSett.TeamRating(Player.Squadra)

                'Fattore rating iniziale'
                Dim gmax As Integer = 5

                If wt.PartiteGiocate < gmax Then
                    If wt.PartiteGiocate > 0 Then
                        rtini = (rtini / 10) * (((gmax - wt.PartiteGiocate) / gmax) * fteam)
                    Else
                        rtini = (rtini / 10) * fteam
                    End If
                Else
                    rtini = 0
                End If

                'Fattore rating squadra'
                If wt.PartiteGiocate > 0 Then
                    Dim ref As Integer = 0
                    If wt.PartiteGiocate < gmax Then
                        ref = CInt((wt.PartiteGiocate / gmax) * fteam)
                    Else
                        ref = 40
                    End If
                    If webdata.WebTeams.ContainsKey(avv) Then
                        rtteam = ((wt.Punti - webdata.WebTeams(avv).Punti) / (wt.PartiteGiocate * 2)) * (ref / 2) + ref / 2
                    Else
                        rtteam = 0
                    End If
                Else
                    rtteam = 0
                End If

                If fc Then
                    If Player.Ruolo = "P" Then
                        If wt.GoalSubiti = 0 Then wt.GoalSubiti = 1
                        If wt.GoalSubiti_Fuori = 0 Then wt.GoalSubiti_Fuori = 1
                        rtp = 20 - (wt.GoalSubiti_Fuori / wt.GoalSubiti_Fuori) * 20
                        avg = Player.StatisticAll.Avg_Pt
                        If avg < 4 Then avg = 4
                        If avg > 9 Then avg = 9
                        rtp = rtp + avg * 6
                    Else
                        If wt.GoalFatti = 0 Then wt.GoalFatti = 1
                        If wt.GoalFatti_Fuori = 0 Then wt.GoalFatti_Fuori = 1
                        avg = Player.StatisticAll.Avg_Pt
                        If avg < 4 Then avg = 4
                        If avg > 9 Then avg = 9
                        rtp = rtp + avg * 6
                        k = 3
                        Select Case Player.Ruolo
                            Case "D" : k = 15
                            Case "C" : k = 9
                            Case "A" : k = 7
                        End Select
                        rtp = rtp + CInt((Player.StatisticAll.Gf / wt.GoalFatti_Fuori) * k)
                    End If
                Else
                    If Player.Ruolo = "P" Then
                        If wt.GoalSubiti = 0 Then wt.GoalSubiti = 1
                        If wt.GoalSubiti_Fuori = 0 Then wt.GoalSubiti_Dentro = 1
                        rtp = 20 - (wt.GoalSubiti_Dentro / wt.GoalSubiti) * 20
                        avg = Player.StatisticAll.Avg_Pt
                        If avg < 4 Then avg = 4
                        If avg > 9 Then avg = 9
                        rtp = rtp + avg * 6
                    Else
                        If wt.GoalFatti = 0 Then wt.GoalFatti = 1
                        If wt.GoalFatti_Dentro = 0 Then wt.GoalFatti_Dentro = 1
                        avg = Player.StatisticAll.Avg_Pt
                        If avg < 4 Then avg = 4
                        If avg > 9 Then avg = 9
                        rtp = rtp + avg * 6
                        k = 3
                        Select Case Player.Ruolo
                            Case "D" : k = 15
                            Case "C" : k = 9
                            Case "A" : k = 7
                        End Select
                        rtp = rtp + CInt((Player.StatisticAll.Gf / wt.GoalFatti_Dentro) * k)
                    End If
                End If

                'Aggiungo eventuali Bonus/Malus'
                rtp = rtp + Player.Variation * 5
                If Player.Ruolo = "P" Then
                    rtp = rtp - Player.StatisticLast.Gs * 2
                End If
                rtp = rtp + Player.StatisticLast.Gf * 3
                rtp = rtp + Player.StatisticLast.RigT * 5

                rt = CInt(rtini + rtteam + rtp)

                'Alzo/Abasso il reating se la squadra gioca dentro casa o fuori'
                If fc = False Then
                    rt = CInt(rt * (1 + rtteam / 120))
                Else
                    rt = CInt(rt * (rtteam / 120 + 0.65))
                End If

                If rt > 100 Then rt = 100
                If rt < 0 Then rt = 0

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return rt

        End Function

        Public Shared Function CheckMudule(ByVal Ruolo As String, ByVal CurrP As Integer, ByVal CurrD As Integer, ByVal CurrC As Integer, ByVal CurrA As Integer) As Boolean

            Dim ris As Boolean = False

            Dim tot As Integer = CurrP + CurrD + CurrC + CurrA + 1

            Select Case Ruolo
                Case "P" : CurrP = CurrP + 1
                Case "D" : CurrD = CurrD + 1
                Case "C" : CurrC = CurrC + 1
                Case "A" : CurrA = CurrA + 1
            End Select

            If CurrP < 2 AndAlso CurrD < 4 AndAlso CurrC < 5 AndAlso CurrA < 4 Then
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 3 AndAlso CurrC < 5 AndAlso CurrA < 3 Then '343'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 4 AndAlso CurrC < 6 AndAlso CurrA < 3 Then '352'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 4 AndAlso CurrA < 4 Then '433'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 5 AndAlso CurrA < 3 Then '442'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 5 AndAlso CurrC < 6 AndAlso CurrA < 2 Then '451'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 6 AndAlso CurrC < 4 AndAlso CurrA < 3 Then '532'
                ris = True
            ElseIf CurrP < 2 AndAlso CurrD < 6 AndAlso CurrC < 5 AndAlso CurrA < 2 Then '541'
                ris = True
            End If

            Return ris

        End Function

        Public Shared Function GetRuoloFromId(ByVal id As Integer) As String
            Dim r As String = "P"
            Select Case id
                Case Is < 3 : r = "P"
                Case Is < 11 : r = "D"
                Case Is < 19 : r = "C"
                Case Else : r = "A"
            End Select
            Return r
        End Function

        Public Shared Function GetRuoloForeColor(ByVal id As Integer) As Color
            Dim cl As Color = Color.White
            Dim ruolo As String = GetRuoloFromId(id)
            Select Case ruolo
                Case "P" : cl = Color.Orange
                Case "D" : cl = Color.Green
                Case "C" : cl = Color.Red
                Case "A" : cl = Color.Blue
            End Select
            Return cl
        End Function

        Public Shared Function GetRuoloForeColor(ByVal Ruolo As String) As Color
            Dim cl As Color = Color.White
            Select Case Ruolo
                Case "P" : cl = Color.Orange
                Case "D" : cl = Color.Green
                Case "C" : cl = Color.Red
                Case "A" : cl = Color.Blue
            End Select
            Return cl
        End Function

        Public Shared Function GetRuoloForeColorDisable(ByVal id As Integer) As Color
            Dim cl As Color = Color.White
            Dim ruolo As String = GetRuoloFromId(id)
            Select Case ruolo
                Case "P" : cl = Color.FromArgb(255, 180, 130)
                Case "D" : cl = Color.FromArgb(130, 255, 130)
                Case "C" : cl = Color.FromArgb(255, 130, 130)
                Case "A" : cl = Color.FromArgb(130, 130, 255)
            End Select
            Return cl
        End Function

        Public Shared Function GetRuoloForeColorDisable(ByVal Ruolo As String) As Color
            Dim cl As Color = Color.White
            Select Case Ruolo
                Case "P" : cl = Color.FromArgb(255, 180, 130)
                Case "D" : cl = Color.FromArgb(130, 255, 130)
                Case "C" : cl = Color.FromArgb(255, 130, 130)
                Case "A" : cl = Color.FromArgb(130, 130, 255)
            End Select
            Return cl
        End Function

        Public Shared Function SetFieldStringData(data As String, Optional defvalue As String = "") As String
            If data = "" OrElse data Is Nothing Then
                Return CStr(data)
            Else
                Return defvalue
            End If
        End Function

        Public Shared Function SetFieldIntegerData(data As Integer, Optional defvalue As String = "") As String
            If data > 0 Then
                Return CStr(data)
            Else
                Return defvalue
            End If
        End Function

        Public Shared Function SetFieldIntegerData(data As Integer, Prefix As String, Optional defvalue As String = "") As String
            If data > 0 Then
                Return Prefix & CStr(data)
            Else
                Return defvalue
            End If
        End Function

        Public Shared Function SetFieldSingleData(data As Single, Optional defvalue As String = "") As String
            If data > 0 Then
                Return CStr(data)
            Else
                Return defvalue
            End If
        End Function

        Public Shared Function SetFieldSingleData(data As Single, Prefix As String, Optional defvalue As String = "") As String
            If data > 0 Then
                Return Prefix & CStr(data)
            Else
                Return defvalue
            End If
        End Function

        ''' <summary>Consente di verficare che una stringa sia un numero</summary>
        ''' <param name="Value">Stringa da analizzare</param>
        Public Shared Function IsNumeric(ByVal Value As String) As Boolean

            If Value <> "" AndAlso System.Text.RegularExpressions.Regex.Match(Value, "\d+").Length = Value.Length Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Shared Function ValidateServerCertificate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal sslPolicyErrors As Net.Security.SslPolicyErrors) As Boolean
            If sslPolicyErrors = Net.Security.SslPolicyErrors.None Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function UploadFile(ByVal FileName As String, ByVal FileDest As String, LegaPath As Boolean) As Boolean
            Dim err As Boolean = False

            Try

                Dim writer As System.IO.Stream
                Dim Uri As String = If(LegaPath = True, "ftp://www.ifantacalcio.it/ifantacalcio.it/public/ifanta/update/tornei/" & FileDest, "ftp://www.ifantacalcio.it/ifantacalcio.it/public/ifanta/" & FileDest)
                Dim FTP1 As Net.FtpWebRequest = CType(Net.FtpWebRequest.Create(Uri), FtpWebRequest)

                FTP1.Credentials = New System.Net.NetworkCredential("7974741@aruba.it", "Anxanum1969")
                FTP1.Method = System.Net.WebRequestMethods.Ftp.UploadFile
                FTP1.EnableSsl = True
                FTP1.Proxy = Nothing

                ServicePointManager.ServerCertificateValidationCallback = Function(s, certificate, chain, sslPolicyErrors) True

                Dim _File As System.IO.FileInfo = New System.IO.FileInfo(FileName)
                Dim _fileContents As Byte() = New Byte(CInt(_File.Length - 1)) {}
                Dim fr As System.IO.FileStream = _File.OpenRead()

                fr.Read(_fileContents, 0, Convert.ToInt32(_File.Length))
                fr.Close()
                writer = FTP1.GetRequestStream()
                writer.Write(_fileContents, 0, _fileContents.Length)
                writer.Close()

            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Call WriteError("UploadFile", "Update", ex.Message)
                err = True
            End Try
            Return err
        End Function
    End Class


End Namespace
