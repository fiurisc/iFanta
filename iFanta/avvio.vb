Imports System.Data.OleDb
Imports System.Data.SQLite
Imports System.IO
Imports System.Threading
Imports iControl
Imports iFanta.SystemFunction.DataBase
Imports iFanta.SystemFunction.FileAndDirectory

Module avvio

    Public GenSett As New GeneralSettings
    Public AppSett As New AppSettings
    Public currlega As New LegaObject
    Public applist As New ApplicationCollection
    Public backup As New BackupData
    Public imgnatcode As New Dictionary(Of String, Bitmap)

    Sub Main()

#If DEBUG Then
        CopyData(AppContext.BaseDirectory & "tornei\data.db", AppContext.BaseDirectory & "tornei\2025.accdb")

        LoadRose()
#End If

        Dim intconn As New InternetConnection.ConnType

        intconn = InternetConnection.Type

        'Creazione cartelle di sistema'
        Call MakeSystemFolder(GetTempDirectory)
        Call MakeSystemFolder(GetLegheDirectory())
        Call MakeSystemFolder(GetThemeDirectory)
        Call MakeSystemFolder(GetSystemDirectory)

        'Carico le impostazioni'
        GenSett.ReadSettings()
        AppSett.ReadSettings()
        AppSett.Personal.Theme.ReadSettings()
        backup.Compress = AppSett.Backup.EnableCompression
        backup.History = AppSett.Backup.History
        webdata.TempDirectory = GetTempDirectory()
        backup.TempDirectory = GetTempDirectory()

        'Impostazioni iControl'
        iControl.CommonFunction.SetDefaultColor(AppSett.Personal.Theme.GetThemeDefaultColor(True), True)
        iControl.CommonFunction.SetDefaultColor(AppSett.Personal.Theme.GetThemeDefaultColor(False), False)
        iControl.Setting.Theme(AppSett.Personal.Theme.Name)
        iControl.Setting.FlatStyle(AppSett.Personal.Theme.FlatStyle)
        iControl.Setting.Language(iMsgBox.Language.Italian)

        'Creo le cartelle di sistema'
        Call MakeSystemFolder(GetTempDirectory)
        Call MakeSystemFolder("theme")
        Call MakeSystemFolder("img\flags")
        Call MakeSystemFolder(GetTempDirectory() & "\image")

        Call DeleteOldFiles(GetTempDirectory() & "\image", "*.*", Date.Now.AddDays(-5))
        Call DeleteDirectory(GetTempDirectory() & "\prob", True)
        Call DeleteDirectory(GetTempDirectory() & "\tabellini", True)
        Call DeleteDirectory(GetTempDirectory() & "\match", True)
        Call DeleteDirectory(GetTempDirectory() & "\player", True)
        Call DeleteFile("update.exe")
        Call DeleteFile("update.dll")

        'Apro lo splash screen'
        Dim splashscreen As New iControl.splashscreen
        splashscreen.Icon = My.Resources.app
        splashscreen.SetParameater(SystemFunction.DrawingAndImage.GetBackgroundImage, AppSett.Personal.Theme.GetThemeSettings, GenSett.Company, "Shareware", "2013/07/01", "False", My.Application.Info.Copyright)
        splashscreen.Show()

        If IO.Directory.Exists(My.Application.Info.DirectoryPath & "\img\flags") Then
            Dim f() As String = IO.Directory.GetFiles(My.Application.Info.DirectoryPath & "\img\flags", "*.png")
            If f.Length > 0 Then
                For i As Integer = 0 To f.Length - 1
                    Dim code As String = IO.Path.GetFileNameWithoutExtension(f(i)).ToUpper
                    If imgnatcode.ContainsKey(code) = False Then
                        imgnatcode.Add(code, New System.Drawing.Bitmap(f(i)))
                    End If
                Next
            End If
        End If

        'Apro lo splash screen'
        splashscreen.Phase("Avvio...", 0)
        System.Threading.Thread.Sleep(150)

        splashscreen.Phase("Controllo aggiornamenti...", 20)

        If intconn <> InternetConnection.ConnType.offline Then Call Update(True)

        splashscreen.Phase("Caricamento tornei...", 30)

        legalist.AddRange(IO.Directory.GetDirectories(GetLegheDirectory))

        If legalist.Count > 0 Then

            If legalist.Count > 1 Then

                Dim frm As New frmload

                currlega.Settings.Nome = ""

                If frm.ShowDialog() = DialogResult.OK Then

                    Dim dir As String = frm.Directory

                    '***********************************************'
                    'Carico i dati del torneo'
                    currlega.Settings.Nome = IO.Path.GetFileName(dir)
                    currlega.ReadSettings()
                    webdata.YearReference = currlega.Settings.Year

                    backup.DataDirectory = GetLegaDataDirectory()
                    backup.BackupDirectory = GetLegaBackupDirectory()

                    'Creo le sotto directory del torneo'
                    Call MakeLegaDirectoryes()

                    Call DeleteFile(GetSystemDirectory() & "\general.txt")
                    Call DeleteFile(GetSystemDirectory() & "\my.txt")
                    Call DeleteFileOnDirectory(GetTempDirectory, "*.*")
                    Call DeleteDirectory(GetTempDirectory() & "\match", True)
                    Call DeleteDirectory(GetTempDirectory() & "\player", True)
                    Call DeleteDirectory(GetTempDirectory() & "\prob", True)
                    Call DeleteDirectory(GetTempDirectory() & "\tabellini", True)

                    Try
                        splashscreen.Phase("Apro la connessione ai dati...", 40)
                        System.Threading.Thread.Sleep(300)
                        Call OpenConnection()
                        If conn.State <> ConnectionState.Open Then
                            Call ShowError("Errore", "Connessione dati fallita")
                            Dim fname As String = GetLegaDataDirectory() & "\data.zip"
                            If IO.File.Exists(fname) Then IO.File.Delete(fname)
                            currlega.Settings.Nome = ""
                        Else
                            currlega.LoadTeams(False)
                            splashscreen.Phase("Aggiornamento dati partite/giocatori...", 60)
                            If conn.State = ConnectionState.Open Then webdata.AllPlayers = currlega.GetDictionaryTeamRolePlayerList : currlega.GetCurrentLegaDay()
                            webdata.UpdateWebData(currlega.Settings.Active, Not (webdata.ForceUpdateStartup), webdata.WebSiteList, intconn)
                        End If
                        SystemFunction.General.LoadImageCache()
                    Catch ex As Exception
                        Call ShowError("Errore", "Connessione dati fallita")
                        Call WriteError("Avvio", "Main", ex.Message)
                    End Try
                    splashscreen.Phase("Aggiornamento configurazioni...", 90)

                End If

                frm.Dispose()

            End If

        End If

        'Chiusura splash screen e avvio finestra principale'
        splashscreen.Close()
        frmgenerale.ShowDialog()

        webdata.StopThread()

        'Salvataggio impostazioni'
        AppSett.SaveSettings()

        'Salvataggio impostazioni lega'
        If currlega.Settings.Nome <> "" Then
            currlega.SaveSettings()
            webdata.UpdatePlayerTbFromView(False)
            Call CompactDb()
            If conn.State = ConnectionState.Open Then conn.Close()
        End If

    End Sub

    Public Sub LoadRose()

        Dim fname As String = AppContext.BaseDirectory & "\rose.html"
        Dim pl As New List(Of List(Of List(Of String)))
        Dim imglist As New List(Of String)
        Dim alllist As New List(Of String)

        If IO.File.Exists(fname) Then
            Dim line() As String = IO.File.ReadAllLines(fname)
            Dim teamid As Integer = 0
            For i As Integer = 0 To line.Length - 1
                If line(i).StartsWith("<!--NAME") Then

                    pl.Add(New List(Of List(Of String)))

                ElseIf line(i).StartsWith("<!--PLAYER") Then

                    Dim str() As String = System.Text.RegularExpressions.Regex.Match(line(i).Trim.Replace("’", "'"), "(?<=<!--PLAYER=)[a-zA-Z0-9\'\-\.\s+|]{1,}").Value.Trim.Split(CChar("|"))

                    pl(pl.Count - 1).Add(New List(Of String))

                    If str.Length = 6 OrElse str.Length = 7 Then

                        Dim idrosa As Integer = CInt(str(0)) - 1

                        If idrosa >= 0 AndAlso idrosa < 25 Then

                            Dim r As String = str(2)
                            Dim n As String = str(3)
                            Dim s As String = "UNK"

                            If str.Length = 7 Then
                                s = str(4)
                            End If

                            pl(pl.Count - 1)(pl(pl.Count - 1).Count - 1).Add(r)
                            pl(pl.Count - 1)(pl(pl.Count - 1).Count - 1).Add(n)
                            pl(pl.Count - 1)(pl(pl.Count - 1).Count - 1).Add(s)
                        End If
                    End If
                ElseIf line(i).Trim().StartsWith("<tr><td width='30px' rowspan='3'>") Then
                    Dim img As String = line(i).Replace("<tr><td width='30px' rowspan='3'><img src=""data:image/jpeg;base64,", "").Replace(""" />", "")
                    imglist.Add(img.Trim())
                ElseIf line(i).Contains("<span class='allenatore'>") Then
                    Dim all As String = System.Text.RegularExpressions.Regex.Match(line(i), "(?<=<span class='allenatore'>).*(?=\</span)").Value
                    alllist.Add(all)
                End If
            Next
        Else

        End If

        Dim sb As New System.Text.StringBuilder
        For i As Integer = 0 To pl.Count - 1
            sb.AppendLine("rose[" & i & "]=[]")
            For k As Integer = 0 To pl(i).Count - 1
                sb.Append("rose[" & i & "][" & k & "]=[")
                For j As Integer = 0 To pl(i)(k).Count - 1
                    If j > 0 Then sb.Append(",")
                    sb.Append("""" & pl(i)(k)(j) & """")
                Next
                sb.AppendLine("];")
            Next
        Next

        sb.AppendLine()
        For i As Integer = 0 To imglist.Count - 1
            sb.AppendLine("teamimg[" & i & "]=""" & imglist(i) & """;")
        Next

        sb.AppendLine()
        For i As Integer = 0 To alllist.Count - 1
            sb.AppendLine("teamall[" & i & "]=""" & alllist(i) & """;")
        Next

        IO.File.WriteAllText(AppContext.BaseDirectory & "\rose.txt", sb.ToString())

    End Sub

    Public Sub CopyData(sqlitePath As String, accessPath As String)
        ' Connessione a SQLite
        Dim sqliteConn As New System.Data.SQLite.SQLiteConnection($"Data Source={sqlitePath};")
        sqliteConn.Open()

        ' Connessione a Access
        Dim accessConn As New OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={accessPath};Persist Security Info=False;")
        accessConn.Open()

        ' Recupera tutte le tabelle SQLite
        Dim getTablesCmd As New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", sqliteConn)
        Dim reader = getTablesCmd.ExecuteReader()

        While reader.Read()
            Dim tableName As String = reader("name").ToString()

            ' Recupera i dati dalla tabella SQLite
            Dim selectCmd As New SQLiteCommand($"SELECT * FROM [{tableName}];", sqliteConn)
            Dim sqliteDataReader = selectCmd.ExecuteReader()

            ' Prepara comando di inserimento per Access
            Dim schemaTable = sqliteDataReader.GetSchemaTable()
            Dim columnNames As New List(Of String)
            For Each row As System.Data.DataRow In schemaTable.Rows
                columnNames.Add(row("ColumnName").ToString())
            Next

            Dim columnsStr = String.Join(",", columnNames)
            Dim placeholders = String.Join(",", columnNames.Select(Function(c) "?"))

            Dim insertCmd As New OleDbCommand($"INSERT INTO [{tableName}] ({columnsStr}) VALUES ({placeholders})", accessConn)

            For Each col In columnNames
                insertCmd.Parameters.Add(New OleDbParameter())
            Next

            ' Inserisce i dati
            While sqliteDataReader.Read()
                For i = 0 To columnNames.Count - 1
                    insertCmd.Parameters(i).Value = sqliteDataReader(columnNames(i))
                Next
                insertCmd.ExecuteNonQuery()
            End While

            sqliteDataReader.Close()
        End While

        reader.Close()
        accessConn.Close()
        sqliteConn.Close()

        MsgBox("Dati copiati con successo da SQLite a Access.")
    End Sub

    Public Sub WriteError(ByVal Form As String, ByVal SubName As String, ByVal ErrMsg As String)
        Try
            IO.File.AppendAllText(My.Application.Info.DirectoryPath & "\error.log", Date.Now.ToString("yyyy/MM/dd HH:mm:ss") & "|" & Form & "|" & SubName & "|" & ErrMsg & System.Environment.NewLine)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub ShowError(ByVal Title As String, ByVal Text As String)
        iControl.iMsgBox.ShowMessage(Text, Title, iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgError)
    End Sub

    Public Sub ShowInfo(ByVal Title As String, ByVal Text As String)
        iControl.iMsgBox.ShowMessage(Text, Title, iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgInfo)
    End Sub

    Public Sub ShowAlert(ByVal Title As String, ByVal Text As String)
        iControl.iMsgBox.ShowMessage(Text, Title, iControl.iMsgBox.MsgStyle.OkOnly, iControl.iMsgBox.Icona.MsgAlert)
    End Sub

    Sub Update(StartUp As Boolean)

        Try
            If AppSett.Update.EnableUpdate Then

                '************** Gestione Upgrade **************'
                Dim up As New ifupdll.UpdateList

                If (StartUp AndAlso (up.CheckFlagUpdate OrElse AppSett.Update.ForceCheckUpdateStartup)) OrElse (StartUp = False AndAlso up.CheckFlagUpdate = False) Then

                    up.RemoteFile = GenSett.UpdateFileVersion
                    up.ServerType = CType(GenSett.UpdateType, ifupdll.UpdateList.stype)
                    up.ServerUpdate = GenSett.Update
                    up.DirectorySystemFile = GenSett.SystemDirectory
                    up.Theme = AppSett.Personal.Theme.Name
                    up.FlatStyle = AppSett.Personal.Theme.FlatStyle
                    up.Language = ifupdll.UpdateList.eLanguage.Italian
                    up.ReadUpdate()

                    'Aggiorno la lista degli applicativi'
                    If up.UpdateSystemFileList.Count > 0 Then
                        up.UpdateSystemFile()
                        GenSett.ReadSettings()
                        AppSett.ReadSettings()
                    End If

                    'Verifico se ci sono versioni nuove'
                    If up.UpdateFileList.Count > 0 Then
                        If up.ShowPopup = DialogResult.OK Then
                            System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\ifup.exe")
                            End
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

    Sub BackgroundActivity()

        Try

            'Eliminazione backup obsoleti'
            backup.DeleteOldBackup()

            'Controllo aggiornamenti'
            Call Update(False)

            'Avvio l'aggiornamenti dei dati dal web'
            If currlega.Settings.Nome <> "" Then
                Dim intconn As New InternetConnection.ConnType
                intconn = InternetConnection.Type
                If conn.State = ConnectionState.Open Then webdata.AllPlayers = currlega.GetDictionaryTeamRolePlayerList
                If webdata.ForceUpdateStartup = False Then webdata.UpdateWebData(currlega.Settings.Active, False, webdata.WebSiteList, intconn)
            End If

            'Elimino le righe del log vecchie'
            Dim fname As String = My.Application.Info.DirectoryPath & "\error.log"
            Dim l As New List(Of String)
            If IO.File.Exists(fname) Then l.AddRange(IO.File.ReadAllLines(fname))
            If l.Count > 1000 Then l.RemoveRange(0, l.Count - 1000)
            IO.File.WriteAllLines(fname, l.ToArray)

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

End Module
