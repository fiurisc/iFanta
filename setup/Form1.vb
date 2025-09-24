
Imports System.IO
Imports Setup
Imports System.Web
Imports System.Net

Public Class Form1

    Private _localdir As String = My.Application.Info.DirectoryPath
    Private Thrd1 As Threading.Thread
    Private padd = 20
    Private _txtprg1 As String = ""
    Private _valueprg As Integer = 0
    Private _maxprb As Integer = 0
    Private remdir As String = "http://www.ifantacalcio.it/public/ifanta/update"

    Private Function CopyProgress(ByVal totalFileSize As Int64, ByVal totalBytesTransferred As Int64, ByVal streamSize As Int64, ByVal streamBytesTransferred As Int64, ByVal dwStreamNumber As Int32, ByVal dwCallbackReason As Int32, ByVal hSourceFile As Int32, ByVal hDestinationFile As Int32, ByVal lpData As Int32) As Int32
        Dim val As Integer = CInt((totalBytesTransferred / totalFileSize) * 1000)
        If val > prb1.Value Then prb1.Value = val
        Return val
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lbby.Text = My.Application.Info.Copyright
        lbdest2.Text = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Call SetTheme()
    End Sub

    Sub SetTheme()

        Try

            pic1.Left = IForm1.LX + padd
            pic1.Top = IForm1.TY + padd * 2
            lbname.Left = pic1.Left + pic1.Width
            lbname.Top = pic1.Top
            lbinfo.Left = lbname.Left
            lbinfo.Top = lbname.Top + lbname.Height

            lbdest1.Left = lbname.Left
            lbdest1.Top = lbinfo.Top + lbinfo.Height + 10
            lbdest2.Left = lbdest1.Left
            lbdest2.Top = lbdest1.Top + lbdest1.Height
            lbdest2.Width = IForm1.RX - lbdest2.Left - padd - btnfolder.Width + 1

            btnfolder.Top = lbdest2.Top
            btnfolder.Left = lbdest2.Left + lbdest2.Width - 1
            btnfolder.Text = ""

            Me.Height = lbdest2.Top + 150

            IButton1.Left = IForm1.RX - padd - IButton1.Width
            IButton1.Top = IForm1.BY - IButton1.Height - padd \ 2
            IButton2.Left = IButton1.Left - IButton2.Width - 4
            IButton2.Top = IButton1.Top

            ILine1.Top = IButton1.Top - ILine1.Height - 5
            ILine1.Width = IForm1.RX - IForm1.LX - padd * 2
            ILine1.Left = IForm1.LX + padd
            prb1.Top = ILine1.Top - prb1.Height - 5
            prb1.Width = ILine1.Width
            prb1.Left = ILine1.Left
            lbstatus.Left = ILine1.Left
            lbstatus.Top = prb1.Top - lbstatus.Height - 5

            lbby.Left = IForm1.LX + padd - 3
            lbby.Top = IButton1.Top + IButton1.Height \ 2 - lbby.Height \ 2

        Catch ex As Exception

        End Try

    End Sub

    Public Sub MakeSystemFolder(ByVal Directory As String)
        If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory
        If IO.Directory.Exists(Directory) = False Then IO.Directory.CreateDirectory(Directory)
    End Sub

    Sub StartInstall()

        Try
            Dim fup As String = "ifup.exe"

            _txtprg1 = "Creazione delle cartelle in corso..."
            MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
            Call MakeSystemFolder(lbdest2.Text & "\iFanta")
            Call MakeSystemFolder(lbdest2.Text & "\iFanta\system")
            Call MakeSystemFolder(lbdest2.Text & "\iFanta\tornei")
            System.Threading.Thread.Sleep(1000)

            _txtprg1 = "Copia dei file necessari in corso..."
            MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
            Call CopyFile(lbdest2.Text & "\iFanta\system\general.txt", remdir & "/system/general.txt")
            Call CopyFile(lbdest2.Text & "\iFanta\ICSharpCode.SharpZipLib.dll", remdir & "/ICSharpCode.SharpZipLib.dll")
            Call CopyFile(lbdest2.Text & "\iFanta\" & fup, remdir & "/" & fup)

            System.Threading.Thread.Sleep(1000)
            _txtprg1 = "Avvio aggiornamento applicazione..."
            MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
            If IO.File.Exists(lbdest2.Text & "\iFanta\" & fup) Then
                System.Diagnostics.Process.Start(lbdest2.Text & "\iFanta\" & fup, "Installazione")
                MyBase.Invoke(New MethodInvoker(AddressOf Fine))
            Else
                _txtprg1 = "Installazione non riuscita"
                MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
            End If
        Catch ex As Exception
            _txtprg1 = "Installazione non riuscita"
            MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
        End Try
        
    End Sub

    Sub CopyFile(ByVal Destination As String, ByVal Source As String)

        _maxprb = 1000
        _valueprg = 0
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateMaxPrbProgress))
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateValuePrbProgress))

        prb1.Max = 1000
        prb1.Value = 0

        _txtprg1 = "Copia " & Source.Replace(remdir, "") & " in corso..."
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))

        Call DownloadChunks(Source, Destination)

        _txtprg1 = "Pronto"
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateValuePrbProgress))

    End Sub

    Sub DownloadChunks(ByVal sURL As String, ByVal Filename As String)

        Try

            Dim URLReq As HttpWebRequest
            Dim URLRes As HttpWebResponse
            Dim FileStreamer As New FileStream(Filename, FileMode.Create)
            Dim bBuffer(999) As Byte
            Dim iBytesRead As Integer
            Dim compleated As Boolean = False

            sURL = sURL.Replace("\", "/")

            Try
                URLReq = CType(WebRequest.Create(sURL), HttpWebRequest)
                URLRes = CType(URLReq.GetResponse, HttpWebResponse)
                Dim sChunks As Stream = URLReq.GetResponse.GetResponseStream
                _maxprb = CInt(URLRes.ContentLength)
                MyBase.Invoke(New MethodInvoker(AddressOf UpdateMaxPrbProgress))

                Do
                    iBytesRead = sChunks.Read(bBuffer, 0, 1000)
                    FileStreamer.Write(bBuffer, 0, iBytesRead)
                    _valueprg = _valueprg + iBytesRead
                    MyBase.Invoke(New MethodInvoker(AddressOf UpdateValuePrbProgress))
                Loop Until iBytesRead = 0
                _valueprg = _maxprb
                MyBase.Invoke(New MethodInvoker(AddressOf UpdateValuePrbProgress))
                sChunks.Close()
                compleated = True
            Catch

            End Try

            FileStreamer.Close()
            FileStreamer.Dispose()

            If compleated = False AndAlso IO.File.Exists(Filename) Then
                IO.File.Delete(Filename)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Sub Fine()
        'Chiudo il programma'
        Me.Close()
    End Sub

    Sub UpdateProgress()
        lbstatus.Text = _txtprg1
        lbstatus.Refresh()
    End Sub

    Sub UpdateMaxPrbProgress()
        prb1.Max = _maxprb
    End Sub

    Sub UpdateValuePrbProgress()
        If _valueprg <= prb1.Max Then
            prb1.Value = _valueprg
        Else
            prb1.Value = prb1.Max
        End If
    End Sub

    Sub ShowError(ByVal Message As String)
        MessageBox.Show(Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub IButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IButton2.Click
        'Avvio l'operazione di cattura messages'
        Dim TStart As New Threading.ThreadStart(AddressOf StartInstall)
        Thrd1 = New Threading.Thread(TStart)
        Thrd1.Priority = Threading.ThreadPriority.Highest
        Thrd1.Start()
    End Sub

    Private Sub IButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IButton1.Click
        Me.Close()
    End Sub

    Private Sub IButton1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles IButton1.MouseEnter
        IButton1.BorderSize = 1
        IButton1.ForeColor = Color.Red
    End Sub

    Private Sub IButton1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles IButton1.MouseLeave
        IButton1.BorderSize = 0
        IButton1.ForeColor = Color.FromArgb(80, 80, 80)
    End Sub

    Private Sub IButton2_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles IButton2.MouseEnter
        IButton2.BorderSize = 1
        IButton2.ForeColor = Color.Red
    End Sub

    Private Sub IButton2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles IButton2.MouseLeave
        IButton2.BorderSize = 0
        IButton2.ForeColor = Color.FromArgb(80, 80, 80)
    End Sub

    Private Sub btnfolder_Click(sender As Object, e As EventArgs) Handles btnfolder.Click
        Try
            Dim ofile As New FolderBrowserDialog
            ofile.SelectedPath = lbdest2.Text
            If ofile.ShowDialog = DialogResult.OK Then
                lbdest2.Text = ofile.SelectedPath
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class
