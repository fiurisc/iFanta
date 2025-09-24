Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase
Imports System.IO
Imports System.Text
Imports System.Net

Public Class wData

    'Variable'
    Private thr As New List(Of wData.ThreadItem)
    Private _intconn As InternetConnection.ConnType

    'COGNOME NOME/NOME LENGHT/LIST NOME ASSOCIATI'
    Private _wplayerkey As New Dictionary(Of String, wPlayerKey)

    Public Property EnableUpdate As Boolean = True
    Public Property ForceUpdateStartup As Boolean = False
    Public Property Progress() As Integer = 0
    Public Property YearReference As String = ""
    Public Property TempDirectory() As String = ""
    Public Property WebSiteList() As List(Of String) = New List(Of String)
    Public Property EnableFeatureData() As Dictionary(Of String, Boolean) = New Dictionary(Of String, Boolean) From {{"ProbableLineUps", True}, {"Match", True}}
    Public Property DayProbableFormation() As Integer = -1
    Public Property NumSitePlayer() As Integer = 0
    Public Property AllPlayers() As Dictionary(Of String, Dictionary(Of String, List(Of String))) = New Dictionary(Of String, Dictionary(Of String, List(Of String)))
    Public Property WebTeamPlayers() As Dictionary(Of String, wTeamPlayers) = New Dictionary(Of String, wTeamPlayers)
    Public Property WebPlayers() As Dictionary(Of String, wPlayer) = New Dictionary(Of String, wPlayer)
    Public Property WebTeams() As Dictionary(Of String, wTeam) = New Dictionary(Of String, wTeam)

    Sub New()
        Me.WebSiteList.Clear()
        If _enablefeaturedata("ProbableLineUps") Then
            Me.WebSiteList.Add("Probabili formazioni ""Gazzetta""")
            Me.WebSiteList.Add("Probabili formazioni ""Fantacalcio""")
            Me.WebSiteList.Add("Probabili formazioni ""Pianeta Fantacalcio""")
            'Me.WebSiteList.Add("Probabili formazioni ""SKY""")
            'Me.WebSiteList.Add("Probabili formazioni ""Corriere""")
            'Me.WebSiteList.Add("Probabili formazioni ""Kataweb""")
            'Me.WebSiteList.Add("Probabili formazioni ""TiscaliSport""")
        End If
        If _EnableFeatureData("Match") Then
            Me.WebSiteList.Add("Match serie A")
            Me.WebSiteList.Add("Classifica serie A")
            Me.WebSiteList.Add("Tabellini partite")
        End If
        Me.WebSiteList.Add("Quotazioni giocatori")
        Me.WebSiteList.Add("Dati giocatori")
    End Sub

    Sub UpdateWebData(Active As Boolean, ByVal StartUp As Boolean, ByVal ListWebSite As List(Of String), ByVal intconn As InternetConnection.ConnType)

        Dim cwebplayer As Boolean = False
        Dim upp As Boolean = False

        _intconn = intconn
        Me.Progress = 0

        If Me.YearReference = "" Then Active = False

        Try

            If Active Then

                thr.Clear()

                For i As Integer = 0 To Me.WebSiteList.Count - 1

                    Dim t As New ThreadItem(Me.WebSiteList(i), StartUp)
                    Dim add As Boolean = True

                    If ListWebSite.Contains(Me.WebSiteList(i)) Then
                        Select Case Me.WebSiteList(i)
                            Case "Match serie A"
                                t.Type = "MATCH"
                                t.FileName = "matchs-data.txt"
                                add = Not (StartUp)
                            Case "Tabellini partite"
                                t.Type = "TABELLINI"
                                t.FileName = "matchs-detail-data.txt"
                                add = Not (StartUp)
                            Case "Quotazioni giocatori"
                                t.Type = "PLAYERS QUOTE"
                                t.FileName = "players-quote.txt"
                                add = Not (StartUp)
                            Case "Dati giocatori"
                                t.Type = "PLAYERS DATA"
                                t.FileName = "players-data.txt"
                                add = Not (StartUp)
                            Case "Classifica serie A"
                                t.Type = "CLASSIFICA"
                                t.FileName = "ranking-data.txt"
                            Case "Probabili formazioni ""Gazzetta"""
                                t.Type = "FORMAZIONI"
                                t.CodeSite = "Gazzetta"
                                t.FileName = "pform-gazzetta.txt"
                            Case "Probabili formazioni ""Fantacalcio"""
                                t.Type = "FORMAZIONI"
                                t.CodeSite = "Fantacalcio"
                                t.FileName = "pform-fantacalcio.txt"
                            Case "Probabili formazioni ""Pianeta Fantacalcio"""
                                t.Type = "FORMAZIONI"
                                t.CodeSite = "Pianeta Fantacalcio"
                                t.FileName = "pform-pianeta-fantacalcio.txt"
                            Case "Probabili formazioni ""SKY"""
                                t.Type = "FORMAZIONI"
                                t.CodeSite = "SKY"
                                t.FileName = "pform-sky.txt"
                            Case "Probabili formazioni ""Corriere"""
                                t.Type = "FORMAZIONI"
                                t.CodeSite = "Corriere"
                                t.FileName = "pform-cds.txt"
                        End Select

                        If add Then
                            t.Thread = New Threading.Thread(AddressOf DownloadWebData)
                            thr.Add(t)
                        End If

                    End If

                Next

                'Lancio i vari Thread'
                For i As Integer = 0 To thr.Count - 1
                    thr(i).Thread.Name = CStr(i)
                    thr(i).Thread.Priority = Threading.ThreadPriority.Highest
                    If ListWebSite.Contains(thr(i).Site) Then
                        thr(i).Thread.Start()
                        thr(i).Thread.Join()
                    End If
                Next

                _numsiteplayer = 0
                _webplayers.Clear()

                Dim st() As String = {"FORMAZIONI", "PLAYERS QUOTE", "PLAYERS DATA", "MATCH", "TABELLINI", "CLASSIFICA"}

                SystemFunction.FileAndDirectory.MakeSystemFolder(Me.TempDirectory & "\webdata")
                SystemFunction.FileAndDirectory.MakeSystemFolder(Me.TempDirectory & "\webdata\" & Me.YearReference)

                For j As Integer = 0 To st.Length - 1
                    For l As Integer = 0 To thr.Count - 1
                        If thr(l).Type = st(j) Then
                            Dim filen As String = Me.TempDirectory & "\webdata\" & Me.YearReference & "\" & thr(l).FileName
                            If IO.File.Exists(filen) Then
                                Select Case thr(l).Type
                                    Case "MATCH" : upp = upp Or LoadMatchData(filen)
                                    Case "TABELLINI" : upp = upp Or LoadMatchDetail(filen)
                                    Case "CLASSIFICA" : LoadRankingData(filen)
                                    Case "PLAYERS DATA" : upp = upp Or LoadPlayersData(filen)
                                    Case "PLAYERS QUOTE" : upp = upp Or LoadPlayersQuote(filen)
                                    Case "FORMAZIONI" : LoadProbableLineUpsData(filen)
                                End Select
                            End If
                        End If
                    Next
                Next
            End If

            If upp Then Call UpdatePlayerTbFromView(StartUp)
            If Active Then Call DetectDayProbableFormation() Else Me.DayProbableFormation = -1
            Call CompileWebTeamPlayers()

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

    End Sub

    Public Sub DownloadWebData()
        Try

            'Determino il gruppo di riferimento'
            Dim ng As Integer = CInt(Threading.Thread.CurrentThread.Name)
            If ng < 0 Then Exit Sub

            Dim link As String = linksitedata & Me.YearReference & "/data/" & thr(ng).FileName
            Dim dirdest As String = GetTempDirectory() & "\webdata\" & Me.YearReference
            Dim filenamed As String = dirdest & "\" & thr(ng).FileName

            Call MakeSystemFolder(dirdest)

            If (thr(ng).StartUp = False OrElse IO.File.Exists(filenamed) = False) AndAlso _intconn <> InternetConnection.ConnType.offline Then
                IO.File.WriteAllText(filenamed, wData.GetPage(link, "GET", ""))
            End If

            Me.Progress += 1

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try
    End Sub

    Sub StopThread()
        For i As Integer = 0 To thr.Count - 1
            Try
                If thr(i).Thread.IsAlive Then
                    thr(i).Thread.Abort()
                End If
            Catch ex As Exception

            End Try
        Next
    End Sub

    Public Class wTeamPlayers

        Public Property Titolari() As List(Of wTeamPlayer) = New List(Of wTeamPlayer)
        Public Property Panchinari() As List(Of wTeamPlayer) = New List(Of wTeamPlayer)

        Public Class wTeamPlayer

            Sub New(ByVal Ruolo As String, ByVal None As String)
                Me.Ruolo = Ruolo
                Me.Nome = None
            End Sub

            Public Property Ruolo() As String = ""
            Public Property Nome() As String = ""
        End Class
    End Class

    Public Class wTeam

        Private _name As String = ""
        Private _pt() As Integer = {0, 0, 0} 'punti tot / punti dentro / punit fuori'
        Private _pg() As Integer = {0, 0, 0} 'partite giocate tot / partite giocate dentro / partite giocate fuori'
        Private _vit() As Integer = {0, 0, 0} 'vittorie tot / vittorie dentro / vittorie fuori'
        Private _per() As Integer = {0, 0, 0} 'sconfitte tot / sconfitte dentro / sconfitte fuori'
        Private _par() As Integer = {0, 0, 0} 'pareggi tot / pareggi dentro / pareggi fuori'
        Private _gf() As Integer = {0, 0, 0} 'goal fatti tot / goal fatti dentro / goal fatti fuori'
        Private _gs() As Integer = {0, 0, 0} 'goal subiti tot / goal subiti dentro / goal subiti fuori'

        Public Sub New()

        End Sub

        Public Sub New(ByVal Name As String)
            _name = Name
        End Sub

        Public Sub New(ByVal Name As String, ByVal pt() As Integer, ByVal pg() As Integer, ByVal vit() As Integer, ByVal par() As Integer, ByVal per() As Integer, ByVal gf() As Integer, ByVal gs() As Integer)
            _name = Name
            _pt = pt
            _pg = pg
            _vit = vit
            _par = par
            _per = per
            _gf = gf
            _gs = gs
        End Sub

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Punti() As Integer
            Get
                Return _pt(0)
            End Get
            Set(ByVal value As Integer)
                _pt(0) = value
            End Set
        End Property

        Public Property Punti_Dentro() As Integer
            Get
                Return _pt(1)
            End Get
            Set(ByVal value As Integer)
                _pt(1) = value
            End Set
        End Property

        Public Property Punti_Fuori() As Integer
            Get
                Return _pt(2)
            End Get
            Set(ByVal value As Integer)
                _pt(2) = value
            End Set
        End Property

        Public Property PartiteGiocate() As Integer
            Get
                Return _pg(0)
            End Get
            Set(ByVal value As Integer)
                _pg(0) = value
            End Set
        End Property

        Public Property PartiteGiocate_Dentro() As Integer
            Get
                Return _pg(1)
            End Get
            Set(ByVal value As Integer)
                _pg(1) = value
            End Set
        End Property

        Public Property PartiteGiocate_Fuori() As Integer
            Get
                Return _pg(2)
            End Get
            Set(ByVal value As Integer)
                _pg(2) = value
            End Set
        End Property

        Public Property Vittorie() As Integer
            Get
                Return _vit(0)
            End Get
            Set(ByVal value As Integer)
                _vit(0) = value
            End Set
        End Property

        Public Property Vittorie_Dentro() As Integer
            Get
                Return _vit(1)
            End Get
            Set(ByVal value As Integer)
                _vit(1) = value
            End Set
        End Property

        Public Property Vittorie_Fuori() As Integer
            Get
                Return _vit(2)
            End Get
            Set(ByVal value As Integer)
                _vit(2) = value
            End Set
        End Property

        Public Property Pareggi() As Integer
            Get
                Return _par(0)
            End Get
            Set(ByVal value As Integer)
                _par(0) = value
            End Set
        End Property

        Public Property Pareggi_Dentro() As Integer
            Get
                Return _par(1)
            End Get
            Set(ByVal value As Integer)
                _par(1) = value
            End Set
        End Property

        Public Property Pareggi_Fuori() As Integer
            Get
                Return _par(2)
            End Get
            Set(ByVal value As Integer)
                _par(2) = value
            End Set
        End Property

        Public Property Sconfitte() As Integer
            Get
                Return _per(0)
            End Get
            Set(ByVal value As Integer)
                _per(0) = value
            End Set
        End Property

        Public Property Sconfitte_Dentro() As Integer
            Get
                Return _per(1)
            End Get
            Set(ByVal value As Integer)
                _per(1) = value
            End Set
        End Property

        Public Property Sconfitte_Fuori() As Integer
            Get
                Return _per(2)
            End Get
            Set(ByVal value As Integer)
                _per(2) = value
            End Set
        End Property

        Public Property GoalFatti() As Integer
            Get
                Return _gf(0)
            End Get
            Set(ByVal value As Integer)
                _gf(0) = value
            End Set
        End Property

        Public Property GoalFatti_Dentro() As Integer
            Get
                Return _gf(1)
            End Get
            Set(ByVal value As Integer)
                _gf(1) = value
            End Set
        End Property

        Public Property GoalFatti_Fuori() As Integer
            Get
                Return _gf(2)
            End Get
            Set(ByVal value As Integer)
                _gf(2) = value
            End Set
        End Property

        Public Property GoalSubiti() As Integer
            Get
                Return _gs(0)
            End Get
            Set(ByVal value As Integer)
                _gs(0) = value
            End Set
        End Property

        Public Property GoalSubiti_Dentro() As Integer
            Get
                Return _gs(1)
            End Get
            Set(ByVal value As Integer)
                _gs(1) = value
            End Set
        End Property

        Public Property GoalSubiti_Fuori() As Integer
            Get
                Return _gs(2)
            End Get
            Set(ByVal value As Integer)
                _gs(2) = value
            End Set
        End Property
    End Class
End Class
