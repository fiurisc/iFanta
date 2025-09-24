
Imports System.IO
Imports System.Net

Public Class Form1

    Private appsett As New ifup.GeneralSettings
    Private _localdir As String = My.Application.Info.DirectoryPath
    Private StartApp As String = "Aggiornamento"
    Private mtx(4) As String

    Private _Integercut As New List(Of String)
    Private up As New ifup.UpdateList
    Private _flagupdate As String = My.Application.Info.DirectoryPath & "\update.flag"

    Private _dirempty As New List(Of String)
    Private _txtprg1 As String = ""
    Private _txtprg2 As String = ""
    Private _valueprg As Integer = 0
    Private _maxprb As Integer = 0
    Dim Thrd1 As Threading.Thread

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            Timer1.Enabled = False
            'Avvio l'operazione di cattura messages'
            Dim TStart As New Threading.ThreadStart(AddressOf UpdateApp)
            Thrd1 = New Threading.Thread(TStart)
            Thrd1.Priority = Threading.ThreadPriority.Highest
            Thrd1.Start()
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End
        End Try
    End Sub

    Sub UpdateApp()
        Try

            DeleteAppLog()
            WriteOnLog("Start update process")

            Dim type As String = "Aggiornamento"
            Dim restore As Boolean = False

            WriteOnLog("Make system directory")

            'Creazione cartelle di sistema'
            Call MakeSystemFolder("temp")
            Call MakeSystemFolder("system")
            Call MakeSystemFolder("tornei")
            Call MakeSystemFolder("theme")

            'Aggiorno le impostazioni sulla base di quelle presenti sul server'
            WriteOnLog("Download settings file")
            Try
                Dim fsett As String = GeneralSettings.GetSysSettingsFileName
                fsett = appsett.Update & fsett.Replace(My.Application.Info.DirectoryPath, "")
                Call CopyFile(GeneralSettings.GetSysSettingsFileName, fsett)
                'Call Download(fsett, GeneralSettings.GetSysSettingsFileName)
            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
            End Try
            appsett.ReadSystemSettings()

            'Cancello i file temporanei'
            up.EmptyTempCacheXml()

            'Verifico quali file sono da aggiornare'
            up.ServerUpdate = appsett.Update

            Try
                'Aggiorno il file'
                _txtprg2 = "Controllo versioni in corso..."
                MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))

                If StartApp = "ripristino" Then type = "Restore" : restore = True

                up.ReadUpdate(restore)

                'Aggiorno se necessario i file di configurazione'
                If up.UpdateFileCofigurationList.Count > 0 Then
                    For i As Integer = 0 To up.UpdateFileCofigurationList.Count - 1
                        'Scarico il file'
                        Call CopyFile(_localdir & up.UpdateFileCofigurationList(i), appsett.Update & up.UpdateFileCofigurationList(i))
                        'Call Download(appsett.Update & up.UpdateFileCofigurationList(i), _localdir & up.UpdateFileCofigurationList(i))
                    Next
                End If

                'Aggiorno tutto il resto del pacchetto'
                If up.UpdateFileList.Count > 0 Then

                    'Controllo le directory/Integercut e chiudo gli applicativi'
                    Call CheckIntegercut()

                    'Chiudo tutte le applicazioni del pacchetto che devono essere aggiornate'
                    _txtprg1 = "Chiusura applicazioni.."
                    MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))

                    up.KillApplication()

                    System.Threading.Thread.Sleep(100)

                    For i As Integer = 0 To up.UpdateFileList.Count - 1

                        Dim ext As String
                        Dim dir1 As String
                        Dim dir2 As String

                        _txtprg1 = type & " (" & i + 1 & "/" & up.UpdateFileList.Count & ") in corso..."
                        MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))

                        ext = Path.GetExtension(up.UpdateFileList(i).Name)
                        dir1 = Path.GetDirectoryName(up.UpdateFileList(i).Name)

                        If dir1 <> "\" Then
                            dir2 = _localdir & dir1
                            If IO.Directory.Exists(dir2) = True AndAlso dir1.Contains("tornei") = False Then
                                Dim ed As Boolean = False
                                For k As Integer = 0 To _dirempty.Count - 1
                                    If dir2.Contains(_dirempty(k)) Then
                                        ed = True
                                    End If
                                Next
                                If ed Then
                                    IO.Directory.Delete(dir2, True)
                                    IO.Directory.CreateDirectory(dir2)
                                End If
                            Else
                                If IO.Directory.Exists(dir2) = False Then IO.Directory.CreateDirectory(dir2)
                            End If
                        End If

                        Call CopyFile(_localdir & up.UpdateFileList(i).Name, appsett.Update & up.UpdateFileList(i).Name)

                        If ext = ".zip" Then
                            Call UnZip(_localdir & up.UpdateFileList(i).Name, _localdir & dir1)
                            _txtprg2 = "Estrazione " & up.UpdateFileList(i).Name & " completata"
                            MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
                            System.Threading.Thread.Sleep(100)
                        End If
                    Next

                    Call CreateIntegercut()

                End If

                If IO.File.Exists(_flagupdate) = True Then
                    IO.File.Delete(_flagupdate)
                End If

                _txtprg1 = "Aggiornamento completato"
                _txtprg2 = "Avvio applicazione in corso..."

                MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))

                If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & appsett.DefaultApplication) = True Then
                    up.KillApplication(appsett.DefaultApplication)
                    Shell(My.Application.Info.DirectoryPath & "\" & appsett.DefaultApplication, AppWinStyle.NormalFocus)
                End If

            Catch ex As Exception
                Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
                ShowError("Errore" & System.Environment.NewLine & ex.Message)
            End Try

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

        MyBase.Invoke(New MethodInvoker(AddressOf Fine))

    End Sub

    Public Sub MakeSystemFolder(ByVal Directory As String)
        WriteOnLog("Create directory -> " & Directory)
        If Directory.Contains(":") = False Then Directory = My.Application.Info.DirectoryPath & "\" & Directory
        If IO.Directory.Exists(Directory) = False Then IO.Directory.CreateDirectory(Directory)
    End Sub

    Sub Fine()
        'Chiudo il programma'
        Me.Close()
    End Sub

    Sub UpdateProgress()
        Label1.Text = _txtprg1
        Label1.Refresh()
        Label2.Text = _txtprg2
        Label2.Refresh()
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

    Sub Download(ByVal fs As String, ByVal fd As String)
        Dim myWebClient As New System.Net.WebClient()
        Try
            myWebClient.DownloadFile(fs.Replace("\", "/"), fd)
            WriteOnLog("Download " & fd & " completed!")
        Catch ex As Exception
            WriteOnLog("Download " & fd & " error -> " & ex.Message)
        End Try
        myWebClient.Dispose()
    End Sub

    Sub CheckIntegercut()

        Dim dir As String = My.Computer.FileSystem.SpecialDirectories.Desktop

        If up.UpdateFileList.Count > 0 Then
            For i As Integer = 0 To up.UpdateFileList.Count - 1
                Dim name As String = up.UpdateFileList.Item(i).Name
                Dim ext As String = Path.GetExtension(up.UpdateFileList.Item(i).Name)

                If ext = ".exe" Then
                    If File.Exists(_localdir & name) = False And File.Exists(dir & name.Replace(".exe", "lnk")) = False Then
                        _Integercut.Add(name.Replace(".exe", ""))
                    End If
                End If
            Next
        End If

    End Sub

    Private Sub appShortcutToDesktop(ByVal linkName As String)
        Dim deskDir As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)

        Using writer As StreamWriter = New StreamWriter(deskDir & "\" & linkName & ".url")
            Dim app As String = System.Reflection.Assembly.GetExecutingAssembly().Location
            app = "C:\Users\ferna\Documents\iFanta\ifanta.exe"
            writer.WriteLine("[InternetShortcut]")
            writer.WriteLine("URL=file:///" & app)
            writer.WriteLine("IconIndex=0")
            Dim icon As String = app.Replace("\"c, "/"c)
            writer.WriteLine("IconFile=" & icon)
        End Using
    End Sub

    Sub CreateIntegercut()

        Dim TargetPath As String = ""

        For i As Integer = 0 To _Integercut.Count - 1
            TargetPath = _localdir & _Integercut.Item(i).ToString.Replace(".net", "")
            TargetPath = TargetPath & ".exe"
            MakeIntegercut(TargetPath, _Integercut.Item(i).ToString & ".lnk", _Integercut.Item(i).ToString)
        Next

    End Sub

    Sub MakeIntegercut(ByVal TargetPath As String, ByVal IntegerCutPath As String, ByVal IntegerCutname As String)

        Dim VbsObj As Object
        VbsObj = CreateObject("WScript.Shell")

        Dim MyIntegercut As Object
        IntegerCutPath = VbsObj.SpecialFolders(IntegerCutPath)
        MyIntegercut = VbsObj.CreateShortcut(IntegerCutPath & IntegerCutname & ".lnk")
        MyIntegercut.TargetPath = TargetPath
        MyIntegercut.IconLocation = TargetPath & "," & 0
        MyIntegercut.Save()

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            Thrd1.Abort()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            _dirempty.Add("theme")
            _dirempty.Add("tornei")

            If My.Application.CommandLineArgs.Count > 0 Then
                StartApp = My.Application.CommandLineArgs(0)
            End If

            If StartApp <> "" Then
                IForm1.WindowsTitle = StartApp
                Me.Text = StartApp
                Label1.Text = StartApp & " in corso.."
            End If
            _txtprg1 = Label1.Text

        Catch ex As Exception

        End Try

        'Carico le impostazioni'
        appsett.ReadSystemSettings()
        appsett.ReadSettings()

        IForm1.Theme = appsett.Theme.Name
        IForm1.FlatStyle = appsett.FlatStyle
        prb1.FlatStyle = appsett.FlatStyle

        Timer1.Enabled = True

    End Sub

    Sub CopyFile(ByVal Destination As String, ByVal Source As String)

        WriteOnLog("Download " & Source & " in progress...")

        _maxprb = 1000
        _valueprg = 0
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateMaxPrbProgress))
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateValuePrbProgress))

        prb1.Max = 1000
        prb1.Value = 0

        _txtprg2 = "Copia " & Source.Replace(appsett.Update & "\", "") & " in corso..."
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))

        Call DownloadChunks(Source.Replace("\", "/"), Destination)

        _txtprg2 = "Pronto"

        WriteOnLog("Download " & Source & " completed!")

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

            FileStreamer.Flush()
            FileStreamer.Close()
            FileStreamer.Dispose()

            If compleated = False AndAlso IO.File.Exists(Filename) Then
                IO.File.Delete(Filename)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Sub UnZip(ByVal Path As String, ByVal Destination As String)

        WriteOnLog("Extraction " & Path & " in progress...")

        _maxprb = 1
        _valueprg = 0

        MyBase.Invoke(New MethodInvoker(AddressOf UpdateMaxPrbProgress))
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateValuePrbProgress))

        _txtprg2 = "Estrazione " & Path & " in corso..."
        Zip.UpZipFile(Path, Destination)

        _valueprg = 0

        MyBase.Invoke(New MethodInvoker(AddressOf UpdateProgress))
        MyBase.Invoke(New MethodInvoker(AddressOf UpdateValuePrbProgress))

        WriteOnLog("Extraction " & Path & " completed!")

    End Sub

    Sub ShowError(ByVal Message As String)
        MessageBox.Show(Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
