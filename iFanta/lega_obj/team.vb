Imports System.Text.RegularExpressions
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase

Partial Class LegaObject

    Public Class Team

        Enum eLoadType As Integer
            Rosa = 0
            Formazione = 1
            Statistiche = 2
        End Enum

        Enum eRatingType As Integer
            Empty = -1
            Rendimento = 0
            RendimentoCosto = 1
            RendimentoQuotazione = 2
        End Enum

        Private _nome As String = ""
        Private _allenatore As String = ""
        Private _presidente As String = ""
        Private _list As New List(Of Player)
        Private _idteam As Integer = 0
        Private _loaddatastate As Boolean = False
        Private _loadplayerstate As Boolean = False
        Private _costotot As Integer = 0
        Private _qinitot As Integer = 0
        Private _qcurtot As Integer = 0
        Private _ratingtype As eRatingType = eRatingType.Empty

        Sub New(ByVal IdTeam As Integer)
            _idteam = IdTeam
        End Sub

        Sub New(ByVal IdTeam As Integer, ByVal Nome As String)
            _idteam = IdTeam
            _nome = Nome
        End Sub

        Sub New(ByVal IdTeam As Integer, ByVal Nome As String, ByVal Allenatore As String, ByVal Presidente As String)
            _idteam = IdTeam
            _nome = Nome
            _allenatore = Allenatore
            _presidente = Presidente
            For i As Integer = 0 To 24
                Dim r As String = "P"
                Select Case i
                    Case Is < 3 : r = "P"
                    Case Is < 11 : r = "D"
                    Case Is < 19 : r = "C"
                    Case Else : r = "A"
                End Select
                _list.Add(New Player(i + 1, r, "GIOCATORE " & CStr(i + 1), "", 0))
            Next
        End Sub

        Public Property IdTeam() As Integer
            Get
                Return _idteam
            End Get
            Set(ByVal value As Integer)
                _idteam = value
            End Set
        End Property

        Public Property Nome() As String
            Get
                Return _nome
            End Get
            Set(ByVal value As String)
                _nome = value
            End Set
        End Property

        Public Property Allenatore() As String
            Get
                Return _allenatore
            End Get
            Set(ByVal value As String)
                _allenatore = value
            End Set
        End Property

        Public Property Presidente() As String
            Get
                Return _presidente
            End Get
            Set(ByVal value As String)
                _presidente = value
            End Set
        End Property

        Public Property CostoTot() As Integer
            Get
                Return _costotot
            End Get
            Set(ByVal value As Integer)
                _costotot = value
            End Set
        End Property

        Public Property QiniTot() As Integer
            Get
                Return _qinitot
            End Get
            Set(ByVal value As Integer)
                _qinitot = value
            End Set
        End Property

        Public Property QcurTot() As Integer
            Get
                Return _qcurtot
            End Get
            Set(ByVal value As Integer)
                _qcurtot = value
            End Set
        End Property

        Public Property Players() As List(Of Player)
            Get
                Return _list
            End Get
            Set(ByVal value As List(Of Player))
                _list = value
            End Set
        End Property

        Public Property LoadDataState() As Boolean
            Get
                Return _loaddatastate
            End Get
            Set(ByVal value As Boolean)
                _loaddatastate = value
            End Set
        End Property

        Public Property LoadPlayerState() As Boolean
            Get
                Return _loadplayerstate
            End Get
            Set(ByVal value As Boolean)
                _loadplayerstate = value
            End Set
        End Property

        Public Property RatingType() As eRatingType
            Get
                Return _ratingtype
            End Get
            Set(ByVal value As eRatingType)
                _ratingtype = value
            End Set
        End Property

        Sub Load()
            Call Load(True, True)
        End Sub

        Sub Load(ByVal LoadData As Boolean, ByVal LoadPlayers As Boolean)
            Try

                If LoadData AndAlso _loaddatastate = False Then
                    Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT * FROM " & tbteam & " WHERE idteam=" & _idteam & ";", conn)
                    If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                        _nome = ds.Tables(0).Rows(0).Item("nome").ToString
                        _allenatore = ds.Tables(0).Rows(0).Item("allenatore").ToString
                        _presidente = ds.Tables(0).Rows(0).Item("presidente").ToString
                    End If
                    ds.Dispose()
                    _loaddatastate = True
                End If

                If LoadPlayers AndAlso _loadplayerstate = False Then
                    _list = GetPlayer(_nome, "")
                    _loadplayerstate = True
                End If

            Catch ex As Exception

            End Try

        End Sub

        Function GetPlayer(ByVal Search As String, ByVal Filter As String) As List(Of Player)
            Return GetPlayer(Search, Filter, False, 0)
        End Function

        Function GetPlayer(ByVal Search As String, ByVal Filter As String, AddMatchInfo As Boolean, Giornata As Integer) As List(Of Player)

            Dim lst As New List(Of Player)

            Dim frosa As Boolean = False
            Dim srose As Boolean = False 'flag che indica se viene caricata una singola rosa'
            Dim orderbyteam As Boolean = True

            For i As Integer = 0 To currlega.Teams.Count - 1
                If currlega.Teams(i).Nome = Search Then frosa = True : Exit For
            Next
            If Search = "TUTTE" OrElse frosa Then orderbyteam = True

            _costotot = 0
            _qcurtot = 0
            _qinitot = 0

            If Filter.ToLower = "tutti" Then Filter = ""

            Try

                Dim sqlstr As New System.Text.StringBuilder

                If AddMatchInfo Then
                    sqlstr.Append("SELECT p.*, m.timem, m.teama, m.teamb FROM player as p LEFT JOIN tbmatch AS m ON (m.gio=" & Giornata & " AND (m.teama=p.squadra or m.teamb=p.squadra))")
                Else
                    sqlstr.Append("SELECT * FROM player as p")
                End If

                If Search = "TUTTE" Then
                    sqlstr.Append(" WHERE p.team Is not Null")
                ElseIf Search = "TUTTI" Then
                    sqlstr.Append(" WHERE p.nome Is not Null")
                ElseIf Search = "SVINCOLATI" Then
                    sqlstr.Append(" WHERE p.team Is Null")
                Else
                    For i As Integer = 0 To currlega.Teams.Count - 1
                        If currlega.Teams(i).Nome = Search Then
                            srose = True
                            Exit For
                        End If
                    Next

                    lst.Clear()

                    If srose Then

                        sqlstr.Append(" WHERE p.team='" & Search & "'")

                        For i As Integer = 0 To 24
                            Dim r As String = "P"
                            Select Case i
                                Case Is < 3 : r = "P"
                                Case Is < 11 : r = "D"
                                Case Is < 19 : r = "C"
                                Case Else : r = "A"
                            End Select
                            lst.Add(New Team.Player(i + 1, r, "", "", 0))
                        Next
                    Else
                        sqlstr.Append(" WHERE p.nome='" & Search & "'")
                    End If

                End If

                If Filter <> "" Then
                    Dim fstring As String = "P"
                    Select Case Filter.ToLower
                        Case "portieri" : fstring = "P"
                        Case "difensori" : fstring = "D"
                        Case "centrocampisti" : fstring = "C"
                        Case "attaccanti" : fstring = "A"
                    End Select
                    sqlstr.Append(" AND p.ruolo='" & fstring & "'")
                End If
                If orderbyteam Then
                    sqlstr.Append(" ORDER by p.idteam,p.idrosa")
                Else
                    sqlstr.Append(" ORDER by p.nome")
                End If

                Dim ds As DataSet = ExecuteSqlReturnDataSet(sqlstr.ToString & ";", conn)

                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then

                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim p As New Player
                        Dim data As DataRow = ds.Tables(0).Rows(i)

                        'Dati generali'
                        p.IdTeam = ReadFieldIntegerData(data.Item("idteam"), 0)
                        p.IdRosa = ReadFieldIntegerData(data.Item("idrosa"), -1)
                        p.Riconfermato = ReadFieldIntegerData(data.Item("riconfermato"), 0)
                        p.Ruolo = ReadFieldStringData(data.Item("ruolo"), "?")
                        p.Nome = ReadFieldStringData(data.Item("nome"))
                        p.Squadra = ReadFieldStringData(data.Item("squadra"))
                        p.Nat = ReadFieldStringData(data.Item("nat"))
                        p.NatCode = ReadFieldStringData(data.Item("natcode"))
                        p.Costo = ReadFieldIntegerData(data.Item("costo"), 0)
                        p.QIni = ReadFieldIntegerData(data.Item("qini"), 0)
                        p.QCur = ReadFieldIntegerData(data.Item("qcur"), 0)

                        If AddMatchInfo Then
                            p.Match.Time = CDate(ReadFieldTimeData(data.Item("timem"), Date.Now))
                            p.Match.TeamA = ReadFieldStringData(data.Item("teama"))
                            p.Match.TeamB = ReadFieldStringData(data.Item("teamb"))
                        End If

                        'Dati statistici totali'
                        p.StatisticAll = p.GetStatsticAllDataFromDataRow(data)

                        'Dati statistici ultime giornate'
                        p.StatisticLast = p.GetStatsticLastlDataFromDataRow(data)

                        'Calcolo variazioni ultimi giorni
                        p.Variation = p.GetVariation(p.StatisticAll, p.StatisticLast)

                        p.Rating = SystemFunction.General.GetRatingRosa(p, eRatingType.Rendimento)

                        If frosa Then
                            lst(p.IdRosa - 1) = p
                            _qcurtot += p.QCur
                            _costotot += p.Costo
                            _qinitot += p.QIni
                        Else
                            lst.Add(p)
                        End If

                    Next
                End If

            Catch ex As Exception
                Dim f As String = ex.Message
            End Try

            Return lst

        End Function

        Public Shared Function Sort(ByVal Data As List(Of Player), ByVal Type As String, ByVal Reverse As Boolean) As List(Of Player)
            Dim a() As Player = Data.ToArray
            Dim s As New PlayerSorter(PlayerSorter.eType.Rosa, Type, Reverse)
            Array.Sort(a, s)
            Dim ris As New List(Of Player)
            ris.AddRange(a)
            Return ris
        End Function

        Sub Save()
            Call Save(True, True)
        End Sub

        Sub Save(ByVal Data As Boolean, ByVal Players As Boolean)

            Dim lst As New List(Of Team)
            lst.Add(Me)

            Call Save(lst, Data, Players)

        End Sub

        Sub Save(List As List(Of Team), ByVal Data As Boolean, ByVal Players As Boolean)
            Try

                Dim idlist As New List(Of Integer)

                For i As Integer = 0 To List.Count - 1
                    idlist.Add(List(i).IdTeam)
                Next
                Call Delete(idlist, Data, Players)

                If Data Then
                    Dim str As New System.Text.StringBuilder
                    str.Append("INSERT INTO " & tbteam & " (idteam,nome,allenatore,presidente) VALUES ")
                    For i As Integer = 0 To List.Count - 1
                        str.Append("(" & List(i).IdTeam & ",'" & List(i).Nome.Replace("'", "’") & "','" & List(i).Allenatore.Replace("'", "’") & "','" & List(i).Presidente.Replace("'", "’") & "')")
                        If i < List.Count - 1 Then str.Append(",")
                    Next
                    ExecuteSql(str.ToString & ";", conn)
                    _loaddatastate = False
                End If

                If Players Then

                    Dim str As New System.Text.StringBuilder
                    Dim r As Integer = 0
                    Dim sins As String = "INSERT INTO " & tbrose & " (idteam,idrosa,ruolo,nome,costo,qini,riconfermato) VALUES "

                    For i As Integer = 0 To List.Count - 1
                        For k As Integer = 0 To List(i).Players.Count - 1
                            str.Append(",(" & List(i).IdTeam & "," & k + 1 & ",'" & List(i).Players(k).Ruolo & "','" & List(i).Players(k).Nome.Replace("'", "’") & "'," & List(i).Players(k).Costo & ",")
                            If List(i).Players(k).QIni <> 0 Then str.Append(List(i).Players(k).QIni) Else str.Append("null")
                            str.Append("," & List(i).Players(k).Riconfermato & ")")
                            r += 1
                            If r > blkrec Then
                                Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                                str = New System.Text.StringBuilder
                                r = 0
                            End If
                        Next
                    Next

                    If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                    _loadplayerstate = False
                End If
            Catch ex As Exception

            End Try
        End Sub

        Sub SetLoadState(ByVal Data As Boolean, ByVal Players As Boolean)
            _loaddatastate = Data
            _loadplayerstate = Data
        End Sub

        Sub Delete()
            Call Delete(True, True)
        End Sub

        Sub Delete(ByVal Data As Boolean, ByVal Players As Boolean)
            Call Delete(_idteam, Data, Players)
        End Sub

        Sub Delete(IdTeam As Integer, ByVal Data As Boolean, ByVal Players As Boolean)

            Dim lst As New List(Of Integer)

            If IdTeam <> -1 Then lst.Add(IdTeam)

            Call Delete(lst, Data, Players)

        End Sub

        Sub Delete(IdTeams As List(Of Integer), ByVal Data As Boolean, ByVal Players As Boolean)
            Try

                If Data Then
                    If IdTeams Is Nothing OrElse IdTeams.Count = 0 Then
                        ExecuteSql("DELETE FROM " & tbteam & ";", conn)
                    Else
                        ExecuteSql("DELETE FROM " & tbteam & " WHERE idteam in (" & SystemFunction.Convertion.ConvertListIntegerToString(IdTeams, ",") & ");", conn)
                    End If
                End If

                If Players Then
                    If IdTeams Is Nothing OrElse IdTeams.Count = 0 Then
                        ExecuteSql("DELETE FROM " & tbrose & ";", conn)
                    Else
                        ExecuteSql("DELETE FROM " & tbrose & " WHERE idteam in (" & SystemFunction.Convertion.ConvertListIntegerToString(IdTeams, ",") & ");", conn)
                    End If
                End If

            Catch ex As Exception

            End Try
        End Sub

        Public Class TeamSorter
            Implements IComparer

            Private _type As String = ""
            Private _revers As Boolean = False

            Sub New()

            End Sub

            Sub New(ByVal Type As String, ByVal Revers As Boolean)
                _type = Type
                _revers = Revers
            End Sub

            Public Property Type() As String
                Get
                    Return _type
                End Get
                Set(ByVal value As String)
                    _type = value
                End Set
            End Property

            Public Property Revers() As Boolean
                Get
                    Return _revers
                End Get
                Set(ByVal value As Boolean)
                    _revers = value
                End Set
            End Property

            Public Overridable Overloads Function Compare(ByVal Item1 As Object, ByVal Item2 As Object) As Integer Implements IComparer.Compare

                Dim d1 As Team = CType(Item1, Team)
                Dim d2 As Team = CType(Item2, Team)
                Dim ris As Integer = 1
                Dim str1 As String = ""
                Dim str2 As String = ""

                Select Case Type.ToLower
                    Case "nome"
                        str1 = str1 & d1.Nome.PadRight(20, CChar("X"))
                        str2 = str2 & d2.Nome.PadRight(20, CChar("X"))
                    Case "diffq"
                        str1 = str1 & CStr(d1.QcurTot - d1.QiniTot).PadLeft(4, CChar("0"))
                        str2 = str2 & CStr(d2.QcurTot - d2.QiniTot).PadLeft(4, CChar("0"))
                End Select

                ris = String.Compare(str1, str2)
                If _revers Then
                    ris = -ris
                End If

                Return ris

            End Function

        End Class

        Public Class PlayerSorter
            Implements IComparer

            Enum eType As Integer
                Rosa = 0
                Forma = 1
            End Enum

            Private _type As eType = eType.Rosa
            Private _sortkey As String = ""
            Private _revers As Boolean = False

            Sub New()

            End Sub

            Sub New(ByVal Type As eType, ByRef SortKey As String, ByVal Revers As Boolean)
                _type = Type
                _sortkey = SortKey
                _revers = Revers
            End Sub

            Public Property Type() As eType
                Get
                    Return _type
                End Get
                Set(ByVal value As eType)
                    _type = value
                End Set
            End Property

            Public Property SortKey() As String
                Get
                    Return _sortkey
                End Get
                Set(ByVal value As String)
                    _sortkey = value
                End Set
            End Property

            Public Property Revers() As Boolean
                Get
                    Return _revers
                End Get
                Set(ByVal value As Boolean)
                    _revers = value
                End Set
            End Property

            Public Overridable Overloads Function Compare(ByVal Item1 As Object, ByVal Item2 As Object) As Integer Implements IComparer.Compare

                Dim d1 As New Formazione.PlayerFormazione
                Dim d2 As New Formazione.PlayerFormazione
                Dim ris As Integer = 1
                Dim str1 As String = ""
                Dim str2 As String = ""

                If Type = eType.Forma Then
                    d1 = CType(Item1, Formazione.PlayerFormazione)
                    d2 = CType(Item2, Formazione.PlayerFormazione)
                ElseIf Type = eType.Rosa Then
                    d1 = New Formazione.PlayerFormazione(CType(Item1, Player))
                    d2 = New Formazione.PlayerFormazione(CType(Item2, Player))
                End If

                Select Case SortKey.ToLower
                    Case "", "r."
                        str1 = str1 & d1.Ruolo.Replace("D", "B").Replace("A", "D").Replace("P", "A") & "-" & d1.Nome
                        str2 = str2 & d2.Ruolo.Replace("D", "B").Replace("A", "D").Replace("P", "A") & "-" & d2.Nome
                    Case "forma"
                        str1 = str1 & CStr(d1.Type) & "-" & CStr(d1.IdFormazione).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.Type) & "-" & CStr(d2.IdFormazione).PadLeft(3, CChar("0"))
                    Case "idrosa"
                        str1 = str1 & CStr(d1.IdTeam).PadLeft(3, CChar("0")) & CStr(d1.IdRosa).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.IdTeam).PadLeft(3, CChar("0")) & CStr(d2.IdRosa).PadLeft(3, CChar("0"))
                    Case "nome"
                        str1 = str1 & CStr(d1.Nome).PadRight(20, CChar("X"))
                        str2 = str2 & CStr(d2.Nome).PadRight(20, CChar("X"))
                    Case "squadra", "squa."
                        str1 = str1 & CStr(d1.Squadra).PadRight(20, CChar("X"))
                        str2 = str2 & CStr(d2.Squadra).PadRight(20, CChar("X"))
                    Case "cos.", "costo"
                        str1 = str1 & CStr(d1.Costo).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.Costo).PadLeft(3, CChar("0"))
                    Case "qcur"
                        str1 = str1 & CStr(d1.QCur).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.QCur).PadLeft(3, CChar("0"))
                    Case "gs_tot"
                        str1 = str1 & CStr(d1.StatisticAll.Gs).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.Gs).PadLeft(3, CChar("0"))
                    Case "gf_tot"
                        str1 = str1 & CStr(d1.StatisticAll.Gf).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.Gf).PadLeft(3, CChar("0"))
                    Case "amm_tot"
                        str1 = str1 & CStr(d1.StatisticAll.Amm).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.Amm).PadLeft(3, CChar("0"))
                    Case "esp_tot"
                        str1 = str1 & CStr(d1.StatisticAll.Esp).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.Esp).PadLeft(3, CChar("0"))
                    Case "ass_tot"
                        str1 = str1 & CStr(d1.StatisticAll.Ass).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.Ass).PadLeft(3, CChar("0"))
                    Case "rigt_tot"
                        str1 = str1 & CStr(d1.StatisticAll.RigT).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.RigT).PadLeft(3, CChar("0"))
                    Case "rigp_tot"
                        str1 = str1 & CStr(d1.StatisticAll.RigP).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.RigP).PadLeft(3, CChar("0"))
                    Case "rigs_tot"
                        str1 = str1 & CStr(d1.StatisticAll.RigS).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.RigS).PadLeft(3, CChar("0"))
                    Case "pgio_tot"
                        str1 = str1 & CStr(d1.StatisticAll.pGiocate).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticAll.pGiocate).PadLeft(3, CChar("0"))
                    Case "pgio_last"
                        str1 = str1 & CStr(d1.StatisticLast.pGiocate).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticLast.pGiocate).PadLeft(3, CChar("0"))
                    Case "tit_last"
                        str1 = str1 & CStr(d1.StatisticLast.Titolare).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticLast.Titolare).PadLeft(3, CChar("0"))
                    Case "avg_mm_last"
                        str1 = str1 & CStr(d1.StatisticLast.Avg_mm).PadLeft(3, CChar("0"))
                        str2 = str2 & CStr(d2.StatisticLast.Avg_mm).PadLeft(3, CChar("0"))
                    Case "avg_vt_tot"
                        str1 = str1 & If(d1.StatisticAll.Avg_Vt < 0, "0", "1") & CStr(CInt(d1.StatisticAll.Avg_Vt * 100)).PadLeft(4, CChar("0"))
                        str2 = str2 & If(d2.StatisticAll.Avg_Vt < 0, "0", "1") & CStr(CInt(d2.StatisticAll.Avg_Vt * 100)).PadLeft(4, CChar("0"))
                    Case "avg_pt_tot"
                        str1 = str1 & If(d1.StatisticAll.Avg_Pt < 0, "0", "1") & CStr(CInt(d1.StatisticAll.Avg_Pt * 100)).PadLeft(4, CChar("0"))
                        str2 = str2 & If(d2.StatisticAll.Avg_Pt < 0, "0", "1") & CStr(CInt(d2.StatisticAll.Avg_Pt * 100)).PadLeft(4, CChar("0"))
                    Case "rating"
                        str1 = str1 & If(d1.Rating < 0, "0", "1") & CStr(d1.Rating).PadLeft(3, CChar("0"))
                        str2 = str2 & If(d2.Rating < 0, "0", "1") & CStr(d2.Rating).PadLeft(3, CChar("0"))
                    Case "pt"
                        str1 = str1 & If(d1.Dati.Pt < 0, "0", "1") & CStr(d1.Dati.Pt * 10).PadLeft(4, CChar("0"))
                        str2 = str2 & If(d2.Dati.Pt < 0, "0", "1") & CStr(d2.Dati.Pt * 10).PadLeft(4, CChar("0"))
                End Select

                ris = String.Compare(str1, str2)
                If _revers Then
                    ris = -ris
                End If

                Return ris

            End Function

        End Class

        <Serializable()>
        Public Class Player

            Private _idteam As Integer = 0
            Private _idrosa As Integer = 0

            Private _ruolo As String = ""
            Private _nome As String = ""
            Private _squadra As String = ""
            Private _nat As String = ""
            Private _natcode As String = ""
            Private _riconfermato As Integer = 0
            Private _costo As Integer = 0
            Private _qini As Integer = 0
            Private _qcur As Integer = 0

            Private _match As New PlayerMatch
            Private _all As New StatisticData
            Private _last As New StatisticData

            Private _rating As Integer = 0
            Private _var As Integer = 0
            Private _tag As String = ""

            Sub New()

            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String)
                _idrosa = IdRosa
                _ruolo = Ruolo
                _nome = Nome
                _squadra = Squadra
            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String, ByVal Costo As Integer)
                _idrosa = IdRosa
                _ruolo = Ruolo
                _nome = Nome
                _squadra = Squadra
                _costo = Costo
            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal Squadra As String, ByVal Costo As Integer, ByVal Qini As Integer)
                _idrosa = IdRosa
                _ruolo = Ruolo
                _nome = Nome
                _squadra = Squadra
                _costo = Costo
                _qini = Qini
            End Sub

            Public Property IdTeam() As Integer
                Get
                    Return _idteam
                End Get
                Set(ByVal value As Integer)
                    _idteam = value
                End Set
            End Property

            Public Property IdRosa() As Integer
                Get
                    Return _idrosa
                End Get
                Set(ByVal value As Integer)
                    _idrosa = value
                End Set
            End Property

            Public Property Ruolo() As String
                Get
                    Return _ruolo
                End Get
                Set(ByVal value As String)
                    _ruolo = value
                End Set
            End Property

            Public Property Nome() As String
                Get
                    Return _nome
                End Get
                Set(ByVal value As String)
                    _nome = value
                End Set
            End Property

            Public Property Squadra() As String
                Get
                    Return _squadra
                End Get
                Set(ByVal value As String)
                    _squadra = value
                End Set
            End Property

            Public Property Nat() As String
                Get
                    Return _nat
                End Get
                Set(ByVal value As String)
                    _nat = value
                End Set
            End Property

            Public Property NatCode() As String
                Get
                    Return _natcode
                End Get
                Set(ByVal value As String)
                    _natcode = value
                End Set
            End Property

            Public Property Costo() As Integer
                Get
                    Return _costo
                End Get
                Set(ByVal value As Integer)
                    _costo = value
                End Set
            End Property

            Public Property QIni() As Integer
                Get
                    Return _qini
                End Get
                Set(ByVal value As Integer)
                    _qini = value
                End Set
            End Property

            Public Property QCur() As Integer
                Get
                    Return _qcur
                End Get
                Set(ByVal value As Integer)
                    _qcur = value
                End Set
            End Property

            Public Property Riconfermato As Integer
                Get
                    Return _riconfermato
                End Get
                Set(value As Integer)
                    _riconfermato = value
                End Set
            End Property

            Public Property Match As PlayerMatch
                Get
                    Return _match
                End Get
                Set(value As PlayerMatch)
                    _match = value
                End Set
            End Property

            Public Property StatisticAll As StatisticData
                Get
                    Return _all
                End Get
                Set(value As StatisticData)
                    _all = value
                End Set
            End Property

            Public Property StatisticLast As StatisticData
                Get
                    Return _last
                End Get
                Set(value As StatisticData)
                    _last = value
                End Set
            End Property

            Public Property Variation() As Integer
                Get
                    Return _var
                End Get
                Set(ByVal value As Integer)
                    _var = value
                End Set
            End Property

            Public Property Rating() As Integer
                Get
                    Return _rating
                End Get
                Set(ByVal value As Integer)
                    _rating = value
                End Set
            End Property

            Public Property Tag() As String
                Get
                    Return _tag
                End Get
                Set(ByVal value As String)
                    _tag = value
                End Set
            End Property

            Public Function GetStatsticAllDataFromDataRow(data As Data.DataRow) As StatisticData

                Dim d As New StatisticData

                'Dati statistici totali'
                d.Gs = ReadFieldIntegerData(data.Item("gs_tot"))
                d.Gf = ReadFieldIntegerData(data.Item("gf_tot"))
                d.Amm = ReadFieldIntegerData(data.Item("amm_tot"))
                d.Esp = ReadFieldIntegerData(data.Item("esp_tot"))
                d.Ass = ReadFieldIntegerData(data.Item("ass_tot"))
                d.RigT = ReadFieldIntegerData(data.Item("rigt_tot"))
                d.RigS = ReadFieldIntegerData(data.Item("rigs_tot"))
                d.RigP = ReadFieldIntegerData(data.Item("rigp_tot"))
                d.Sum_Vt = CInt(ReadFieldIntegerData(data.Item("sum_vt_tot")) / 10)
                d.Avg_Vt = CSng(ReadFieldIntegerData(data.Item("avg_vt")) / 10)
                d.Sum_Pt = CInt(ReadFieldIntegerData(data.Item("sum_pt_tot")) / 10)
                d.Avg_Pt = CSng(ReadFieldIntegerData(data.Item("avg_pt")) / 10)
                d.nPartite = ReadFieldIntegerData(data.Item("nump_tot"))
                d.pGiocate = ReadFieldIntegerData(data.Item("pgio_tot"))
                d.Titolare = ReadFieldIntegerData(data.Item("tit_tot"))
                d.Sostituito = ReadFieldIntegerData(data.Item("sos_tot"))
                d.Subentrato = ReadFieldIntegerData(data.Item("sub_tot"))
                d.mm = ReadFieldIntegerData(data.Item("mm_tot"))

                Return d

            End Function

            Public Function GetStatsticLastlDataFromDataRow(data As Data.DataRow) As StatisticData

                Dim d As New StatisticData

                'Dati statistici ultime giornate'
                d.Gs = ReadFieldIntegerData(data.Item("gs_last"))
                d.Gf = ReadFieldIntegerData(data.Item("gf_last"))
                d.Ass = ReadFieldIntegerData(data.Item("ass_last"))
                d.Sum_Vt = CInt(ReadFieldIntegerData(data.Item("sum_vt_last")) / 10)
                d.Avg_Vt = CSng(ReadFieldIntegerData(data.Item("avg_vt_last")) / 10)
                d.Sum_Pt = CInt(ReadFieldIntegerData(data.Item("sum_pt_last")) / 10)
                d.Avg_Pt = CSng(ReadFieldIntegerData(data.Item("avg_pt_last")) / 10)
                d.nPartite = ReadFieldIntegerData(data.Item("nump_last"))
                d.pGiocate = ReadFieldIntegerData(data.Item("pgio_last"))
                d.Titolare = ReadFieldIntegerData(data.Item("tit_last"))
                d.Sostituito = ReadFieldIntegerData(data.Item("sos_last"))
                d.Subentrato = ReadFieldIntegerData(data.Item("sub_last"))
                d.mm = ReadFieldIntegerData(data.Item("mm_last"))
                d.Avg_mm = ReadFieldIntegerData(data.Item("avg_mm_last"))
                Return d

            End Function

            Public Function GetVariation(d1 As StatisticData, d2 As StatisticData) As Integer

                Dim var As Integer = -2

                'Calcolo variazioni ultimi giorni
                If d1.pGiocate > 0 AndAlso d1.nPartite > 0 AndAlso d2.nPartite > 0 Then

                    Dim val2 As Single = d2.Sum_Pt \ d2.nPartite
                    Dim val1 As Single = d1.Sum_Pt \ d1.nPartite

                    If val2 > val1 + 0.3 Then
                        var = 1
                    ElseIf val2 < val1 - 0.3 Then
                        var = -1
                    Else
                        var = 0
                    End If
                Else
                    var = -2
                End If

                Return var

            End Function

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
            End Sub

            <Serializable()> _
            Public Class PlayerMatch

                Private _teama As String = ""
                Private _teamb As String = ""
                Private _time As DateTime = Date.Now

                Public Property TeamA As String
                    Get
                        Return _teama
                    End Get
                    Set(value As String)
                        _teama = value
                    End Set
                End Property

                Public Property TeamB As String
                    Get
                        Return _teamb
                    End Get
                    Set(value As String)
                        _teamb = value
                    End Set
                End Property

                Public Property Time As DateTime
                    Get
                        Return _time
                    End Get
                    Set(value As DateTime)
                        _time = value
                    End Set
                End Property

                Public Function Clone() As PlayerMatch

                    Dim formatter As Runtime.Serialization.IFormatter = New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    Dim ms As New IO.MemoryStream
                    formatter.Serialize(ms, Me)
                    ms.Seek(0, IO.SeekOrigin.Begin)

                    Dim pCopy As PlayerMatch = CType(formatter.Deserialize(ms), PlayerMatch)

                    Return pCopy

                End Function

            End Class

            <Serializable()> _
            Public Class StatisticData

                Private _gs As Integer = 0
                Private _gf As Integer = 0
                Private _amm As Integer = 0
                Private _esp As Integer = 0
                Private _ass As Integer = 0
                Private _rigt As Integer = 0
                Private _rigs As Integer = 0
                Private _rigp As Integer = 0
                Private _nump As Integer = 0
                Private _pgio As Integer = 0
                Private _tit As Integer = 0
                Private _sos As Integer = 0
                Private _sub As Integer = 0
                Private _mm As Integer = 0
                Private _sum_vt As Integer = 0
                Private _avg_vt As Single = 0
                Private _sum_pt As Integer = 0
                Private _avg_pt As Single = 0
                Private _avg_mm As Integer = 0

                Public Property Gs() As Integer
                    Get
                        Return _gs
                    End Get
                    Set(ByVal value As Integer)
                        _gs = value
                    End Set
                End Property

                Public Property Gf() As Integer
                    Get
                        Return _gf
                    End Get
                    Set(ByVal value As Integer)
                        _gf = value
                    End Set
                End Property

                Public Property Amm() As Integer
                    Get
                        Return _amm
                    End Get
                    Set(ByVal value As Integer)
                        _amm = value
                    End Set
                End Property

                Public Property Esp() As Integer
                    Get
                        Return _esp
                    End Get
                    Set(ByVal value As Integer)
                        _esp = value
                    End Set
                End Property

                Public Property Ass() As Integer
                    Get
                        Return _ass
                    End Get
                    Set(ByVal value As Integer)
                        _ass = value
                    End Set
                End Property

                Public Property RigT() As Integer
                    Get
                        Return _rigt
                    End Get
                    Set(ByVal value As Integer)
                        _rigt = value
                    End Set
                End Property

                Public Property RigS() As Integer
                    Get
                        Return _rigs
                    End Get
                    Set(ByVal value As Integer)
                        _rigs = value
                    End Set
                End Property

                Public Property RigP() As Integer
                    Get
                        Return _rigp
                    End Get
                    Set(ByVal value As Integer)
                        _rigp = value
                    End Set
                End Property

                Public Property nPartite() As Integer
                    Get
                        Return _nump
                    End Get
                    Set(ByVal value As Integer)
                        _nump = value
                    End Set
                End Property

                Public Property pGiocate() As Integer
                    Get
                        Return _pgio
                    End Get
                    Set(ByVal value As Integer)
                        _pgio = value
                    End Set
                End Property

                Public Property Titolare() As Integer
                    Get
                        Return _tit
                    End Get
                    Set(ByVal value As Integer)
                        _tit = value
                    End Set
                End Property

                Public Property Sostituito() As Integer
                    Get
                        Return _sos
                    End Get
                    Set(ByVal value As Integer)
                        _sos = value
                    End Set
                End Property

                Public Property Subentrato() As Integer
                    Get
                        Return _sub
                    End Get
                    Set(ByVal value As Integer)
                        _sub = value
                    End Set
                End Property

                Public Property mm() As Integer
                    Get
                        Return _mm
                    End Get
                    Set(ByVal value As Integer)
                        _mm = value
                    End Set
                End Property

                Public Property Avg_mm() As Integer
                    Get
                        Return _avg_mm
                    End Get
                    Set(ByVal value As Integer)
                        _avg_mm = value
                    End Set
                End Property

                Public Property Sum_Vt As Integer
                    Get
                        Return _sum_vt
                    End Get
                    Set(value As Integer)
                        _sum_vt = value
                    End Set
                End Property

                Public Property Avg_Vt As Single
                    Get
                        Return _avg_vt
                    End Get
                    Set(value As Single)
                        _avg_vt = value
                    End Set
                End Property

                Public Property Sum_Pt As Integer
                    Get
                        Return _sum_pt
                    End Get
                    Set(value As Integer)
                        _sum_pt = value
                    End Set
                End Property

                Public Property Avg_Pt As Single
                    Get
                        Return _avg_pt
                    End Get
                    Set(value As Single)
                        _avg_pt = value
                    End Set
                End Property

                Public Function Clone() As StatisticData

                    Dim formatter As Runtime.Serialization.IFormatter = New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    Dim ms As New IO.MemoryStream
                    formatter.Serialize(ms, Me)
                    ms.Seek(0, IO.SeekOrigin.Begin)

                    Dim pCopy As StatisticData = CType(formatter.Deserialize(ms), StatisticData)

                    Return pCopy

                End Function
            End Class
        End Class
    End Class

End Class
