Imports System.Text.RegularExpressions
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase

Public Class LegaObject

    Private _sett As New LegaSettings
    Private _fname As String = ""
    Private _fnamemy As String = ""
    Private _fadmin As String = ""

    Private _teams As New List(Of Team)
    Private _forma As New List(Of Formazione)

    Private _clasa As New sClassifica
    Private _lastdayforma As Integer = 1

    Private _roseteam As String = ""
    Private _rosedetail As Boolean = False
    Private _staticteam As String = ""
    Private _staticdetail As Boolean = False
    Private _comformdetail As Boolean = False
    Private _classificationdetail As Boolean = False
    Private _lastmailto As New List(Of String)
    Private _currday As Integer = -1

    Sub New()

    End Sub

    Sub New(ByVal Nome As String, ByVal NumeroTeam As Integer, ByVal NumeroGiornate As Integer)
        _sett.Nome = Nome
        _sett.NumberOfTeams = NumeroTeam
        _sett.NumberOfDays = NumeroGiornate
    End Sub

    Public Property Settings() As LegaSettings
        Get
            Return _sett
        End Get
        Set(ByVal value As LegaSettings)
            _sett = value
        End Set
    End Property

    Public Property GiornataCorrente() As Integer
        Get
            If _currday = -1 Then
                GetCurrentLegaDay()
            End If
            Return _currday
        End Get
        Set(ByVal value As Integer)
            _currday = value
        End Set
    End Property

    Public Property LastDaySelectFormation() As Integer
        Get
            Return _lastdayforma
        End Get
        Set(ByVal value As Integer)
            _lastdayforma = value
        End Set
    End Property

    Public Property RoseTeam() As String
        Get
            Return _roseteam
        End Get
        Set(ByVal value As String)
            _roseteam = value
        End Set
    End Property

    Public Property RoseDetail() As Boolean
        Get
            Return _rosedetail
        End Get
        Set(ByVal value As Boolean)
            _rosedetail = value
        End Set
    End Property

    Public Property StaticTeam() As String
        Get
            Return _staticteam
        End Get
        Set(ByVal value As String)
            _staticteam = value
        End Set
    End Property

    Public Property StaticDetail() As Boolean
        Get
            Return _staticdetail
        End Get
        Set(ByVal value As Boolean)
            _staticdetail = value
        End Set
    End Property

    Public Property CompileFormationsDetail() As Boolean
        Get
            Return _comformdetail
        End Get
        Set(ByVal value As Boolean)
            _comformdetail = value
        End Set
    End Property

    Public Property ClassificationDetail() As Boolean
        Get
            Return _classificationdetail
        End Get
        Set(ByVal value As Boolean)
            _classificationdetail = value
        End Set
    End Property

    Public Property LastMailToList As List(Of String)
        Get
            Return _lastmailto
        End Get
        Set(value As List(Of String))
            _lastmailto = value
        End Set
    End Property

    Public Property Teams() As List(Of Team)
        Get
            Return _teams
        End Get
        Set(ByVal value As List(Of Team))
            _teams = value
        End Set
    End Property

    Public Property Formazioni() As List(Of Formazione)
        Get
            Return _forma
        End Get
        Set(ByVal value As List(Of Formazione))
            _forma = value
        End Set
    End Property

    Public Property Classifica() As sClassifica
        Get
            Return _clasa
        End Get
        Set(ByVal value As sClassifica)
            _clasa = value
        End Set
    End Property

    Sub GetCurrentLegaDay()
        If _currday = -1 Then
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT max(gio) as curr FROM tbformazioni where pt>-100;", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 AndAlso ds.Tables(0).Rows(0).Item(0) IsNot System.DBNull.Value Then
                _currday = CInt(ds.Tables(0).Rows(0).Item(0))
            End If
        End If
    End Sub

    Sub Inizialize()
        _teams.Clear()
        For i As Integer = 0 To _sett.NumberOfTeams - 1
            _teams.Add(New Team(i, "TEAM " & CStr(i + 1), "Allenatore " & CStr(i + 1), "Presidente " & CStr(i + 1)))
        Next
    End Sub

    Sub LoadTeams(Players As Boolean, Optional Force As Boolean = False)

        Dim up As Boolean = False

        Try
            For i As Integer = 0 To _teams.Count - 1
                If _teams(i).LoadDataState = False OrElse Force Then
                    _teams(i).Nome = "TEAM " & i + 1
                    _teams(i).Allenatore = ""
                    _teams(i).Presidente = ""
                End If
                If Players AndAlso (Teams(i).LoadPlayerState = False OrElse Force) Then
                    up = True
                    For k As Integer = 0 To _teams(i).Players.Count - 1
                        _teams(i).Players(k).Nome = "GIOCATORE " & k + 1
                        _teams(i).Players(k).Squadra = ""
                        _teams(i).Players(k).Costo = 0
                        _teams(i).Players(k).QIni = 0
                        _teams(i).Players(k).QCur = 0
                        _teams(i).Players(k).NatCode = ""
                        _teams(i).Players(k).Riconfermato = 0
                    Next
                End If
            Next

            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT * FROM " & tbteam & ";", conn)

            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim idteam As Integer = CInt(ds.Tables(0).Rows(i).Item("idteam"))
                    If idteam > -1 AndAlso idteam < _teams.Count Then
                        _teams(idteam).Nome = ds.Tables(0).Rows(i).Item("nome").ToString
                        _teams(idteam).Allenatore = ds.Tables(0).Rows(i).Item("allenatore").ToString
                        _teams(idteam).Presidente = ds.Tables(0).Rows(i).Item("presidente").ToString
                        _teams(idteam).CostoTot = 0
                        _teams(idteam).QiniTot = 0
                        _teams(idteam).QcurTot = 0
                        _teams(idteam).LoadDataState = True
                        If Players Then _teams(idteam).LoadPlayerState = True
                    End If
                Next
            End If
            ds.Dispose()

            If up Then
                Dim p As List(Of Team.Player) = currlega.Teams(0).GetPlayer("TUTTE", "")
                For i As Integer = 0 To p.Count - 1
                    If p(i).IdRosa > 0 AndAlso p(i).IdRosa <= Teams(p(i).IdTeam).Players.Count Then
                        _teams(p(i).IdTeam).Players(p(i).IdRosa - 1) = p(i)
                    End If
                Next
            End If

            For t As Integer = 0 To _teams.Count - 1
                For p As Integer = 0 To _teams(t).Players.Count - 1
                    _teams(t).CostoTot += _teams(t).Players(p).Costo
                    _teams(t).QiniTot += _teams(t).Players(p).QIni
                    _teams(t).QcurTot += _teams(t).Players(p).QCur
                Next
            Next t

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Sub DeleteTeamData()
        Try
            ExecuteSql("DELETE FROM " & tbteam & ";", conn)
            ExecuteSql("DELETE FROM " & tbrose & ";", conn)
            For i As Integer = 0 To _sett.NumberOfTeams - 1
                _teams(i).SetLoadState(False, False)
            Next
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub

    Function GetNumbersJollyUsedByTeams() As Dictionary(Of Integer, Integer)
        Dim jolly As New Dictionary(Of Integer, Integer)
        Try
            Dim ds As DataSet = ExecuteSqlReturnDataSet("select idteam,sum(jolly) as jolly from tbformazioni group by idteam", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    jolly.Add(CInt(ds.Tables(0).Rows(i).Item("idteam")), CInt(ds.Tables(0).Rows(i).Item("jolly")))
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return jolly
    End Function

    Function GetFormazioni(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal Top As Boolean) As List(Of LegaObject.Formazione)
        If IdTeam <> -1 Then
            Return GetFormazioni(Giornata, _teams(IdTeam).Nome, Top)
        Else
            Return GetFormazioni(Giornata, "", Top)
        End If
    End Function

    Function GetFormazioni(ByVal Giornata As Integer, ByVal TeamName As String, ByVal Top As Boolean) As List(Of LegaObject.Formazione)

        Dim f As New List(Of Formazione)

        Dim str As New System.Text.StringBuilder
        Dim gg As Integer = 0
        Dim id As Integer = 0
        Dim oldgg As Integer = -1
        Dim oldid As Integer = -1
        Dim idf As Integer = -1
        Dim nd As Integer = 0
        Dim nc As Integer = 0
        Dim na As Integer = 0
        Dim tb As String = "formazioni"

        If Top Then tb = tb & "_top"

        Try
            str.Append("SELECT * FROM " & tb & " ")
            If Giornata <> -1 AndAlso TeamName = "" Then
                str.Append("WHERE gio=" & Giornata & " ")
                For i As Integer = 0 To currlega.Teams.Count - 1
                    f.Add(New Formazione(i, currlega.Teams(i).Nome, currlega.Teams(i).Allenatore.ToString, Giornata))
                Next
            Else
                str.Append("WHERE nometeam='" & TeamName & "' ")
                If Giornata <> -1 Then str.Append("AND gio=" & Giornata & " ")
            End If
            str.Append("ORDER BY gio,idteam,idrosa;")

            Dim ds As DataSet = ExecuteSqlReturnDataSet(str.ToString, conn)

            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then

                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                    Dim t As Integer = CInt(ds.Tables(0).Rows(i).Item("type"))
                    Dim data As DataRow = ds.Tables(0).Rows(i)
                    Dim g As String = ds.Tables(0).Rows(i).Item("nome").ToString

                    gg = CInt(ds.Tables(0).Rows(i).Item("gio"))
                    id = CInt(ds.Tables(0).Rows(i).Item("idteam"))

                    If (gg <> oldgg OrElse id <> oldid) Then
                        If TeamName <> "" Then
                            f.Add(New Formazione(id, ds.Tables(0).Rows(i).Item("nometeam").ToString, ds.Tables(0).Rows(i).Item("allenatore").ToString, gg))
                            idf += 1
                        Else
                            idf = id
                        End If
                        nd = 0
                        nc = 0
                        na = 0
                    End If

                    oldgg = gg
                    oldid = id

                    If idf > -1 Then
                        Select Case t
                            Case 0, 1, 2

                                Dim p As New Formazione.PlayerFormazione

                                'Dati generali'
                                p.IdRosa = ReadFieldIntegerData(data.Item("idrosa"), -1)
                                p.Jolly = ReadFieldIntegerData(data.Item("jolly"))
                                p.Type = ReadFieldIntegerData(data.Item("type"))
                                p.IdFormazione = ReadFieldIntegerData(data.Item("idformazione"))
                                p.InCampo = ReadFieldIntegerData(data.Item("incampo"))
                                p.Ruolo = ReadFieldStringData(data.Item("ruolo"), "?")
                                p.Nome = ReadFieldStringData(data.Item("nome"))
                                Dim tt As String = data.Item("nome").ToString
                                p.Squadra = ReadFieldStringData(data.Item("squadra"))
                                p.Nat = ReadFieldStringData(data.Item("nat"))
                                p.NatCode = ReadFieldStringData(data.Item("natcode"))

                                'Dati formazione'
                                p.Dati.Gs = ReadFieldIntegerData(data.Item("gs"))
                                p.Dati.Gf = ReadFieldIntegerData(data.Item("gf"))
                                p.Dati.AutG = ReadFieldIntegerData(data.Item("autogol"))
                                p.Dati.Amm = ReadFieldIntegerData(data.Item("amm"))
                                p.Dati.Esp = ReadFieldIntegerData(data.Item("esp"))
                                p.Dati.Ass = ReadFieldIntegerData(data.Item("ass"))
                                p.Dati.RigP = ReadFieldIntegerData(data.Item("rigp"))
                                p.Dati.RigS = ReadFieldIntegerData(data.Item("rigs"))
                                p.Dati.Vt = CSng(ReadFieldIntegerData(data.Item("vote")) / 10)
                                p.Dati.Pt = CSng(ReadFieldIntegerData(data.Item("pt")) / 10)

                                f(idf).Players.Add(p)

                                'Determino il modulo e lista titolari e panchinari'
                                If p.Type = 1 Then
                                    If f(idf).Titolari.Contains(p.Nome) = False Then
                                        f(idf).Titolari.Add(p.Nome)
                                        Select Case p.Ruolo
                                            Case "D" : nd += 1
                                            Case "C" : nc += 1
                                            Case "A" : na += 1
                                        End Select
                                    End If
                                ElseIf p.Type = 2 Then
                                    If f(idf).Panchinari.ContainsKey(p.Nome) = False Then f(idf).Panchinari.Add(p.Nome, p.IdRosa)
                                End If

                                f(idf).Modulo = CStr(nd) & "-" & CStr(nc) & "-" & CStr(na)
                                If p.Type > 0 AndAlso p.InCampo = 1 AndAlso p.Dati.Pt > -10 Then
                                    f(idf).Pt += p.Dati.Pt
                                End If
                                If p.InCampo = 1 Then f(idf).NumberPlayerInCampo += 1
                            Case 10
                                f(idf).BonusDifesa = CSng(CInt(ds.Tables(0).Rows(i).Item("pt")) / 10)
                                f(idf).Pt += f(idf).BonusDifesa
                            Case 20
                                f(idf).BonusCentroCampo = CSng(CInt(ds.Tables(0).Rows(i).Item("pt")) / 10)
                                f(idf).Pt += f(idf).BonusCentroCampo
                            Case 30
                                f(idf).BonusAttacco = CSng(CInt(ds.Tables(0).Rows(i).Item("pt")) / 10)
                                f(idf).Pt += f(idf).BonusAttacco
                            Case 40
                                f(idf).ModuleSubstitution = CBool(ds.Tables(0).Rows(i).Item("pt"))
                        End Select

                    End If
                Next
            End If
            ds.Dispose()
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

        Return f

    End Function

    Sub LoadFormazioni(ByVal Giornata As Integer, ByVal TeamName As String, ByVal Top As Boolean)
        _forma = GetFormazioni(Giornata, TeamName, Top)
    End Sub

    Function CompileTopFormazione(ByVal Giornata As Integer, ByVal IdTeam As Integer) As Formazione
        Dim lst As List(Of Formazione) = GetFormazioni(Giornata, IdTeam, False)
        If lst.Count > 0 Then
            Return CompileTopFormazioni(lst)(0)
        Else
            Return Nothing
        End If
    End Function

    Function CompileTopFormazioni(ByVal Giornata As Integer) As List(Of Formazione)
        Dim lst As List(Of Formazione) = GetFormazioni(Giornata, "", False)
        Return CompileTopFormazioni(lst)
    End Function

    Function CompileTopFormazioni(lst As List(Of Formazione)) As List(Of Formazione)

        Try

            For k As Integer = 0 To lst.Count - 1

                Dim ntit As Integer = 0
                Dim np As Integer = 0
                Dim nd As Integer = 0
                Dim nc As Integer = 0
                Dim na As Integer = 0
                Dim np1 As Integer = 0
                Dim nd1 As Integer = 0
                Dim nc1 As Integer = 0
                Dim na1 As Integer = 0
                Dim ndgoodd As Integer = 0
                Dim ndgoodc As Integer = 0
                Dim ndgooda As Integer = 0

                For i As Integer = 0 To lst(k).Players.Count - 1
                    If lst(k).Players(i).Type >= 0 AndAlso lst(k).Players(i).Dati.Pt > -10 Then
                        Select Case lst(k).Players(i).Ruolo
                            Case "P" : np1 += 1
                            Case "D" : nd1 += 1
                            Case "C" : nc1 += 1
                            Case "A" : na1 += 1
                        End Select
                    End If
                Next

                lst(k).ModuleSubstitution = False
                lst(k).Players = LegaObject.Formazione.Sort(lst(k).Players, "pt", True)

                For i As Integer = 0 To lst(k).Players.Count - 1
                    lst(k).Players(i).IdFormazione = 0
                    lst(k).Players(i).Type = 0
                    lst(k).Players(i).InCampo = 0
                    If lst(k).Players(i).Type >= 0 AndAlso lst(k).Players(i).Dati.Pt > -10 Then
                        If SystemFunction.General.CheckMudule(lst(k).Players(i).Ruolo, np, nd, nc, na) Then
                            Select Case lst(k).Players(i).Ruolo
                                Case "P"
                                    np += 1
                                Case "D"
                                    If currlega.Settings.Bonus.EnableBonusDefense Then ndgoodd += GetGoodForBonus(lst(k).Players(i))
                                    nd += 1
                                Case "C"
                                    If currlega.Settings.Bonus.EnableCenterField Then ndgoodc += GetGoodForBonus(lst(k).Players(i))
                                    nc += 1
                                Case "A"
                                    If currlega.Settings.Bonus.EnableCenterField Then ndgooda += GetGoodForBonus(lst(k).Players(i))
                                    na += 1
                            End Select
                            lst(k).Players(i).Type = 1
                            lst(k).Players(i).InCampo = 1
                            ntit += 1
                        End If
                    End If
                Next

                Dim idforma As Integer = 1
                lst(k).Players = LegaObject.Formazione.Sort(lst(k).Players, "idrosa", False)
                For i As Integer = 0 To lst(k).Players.Count - 1
                    If lst(k).Players(i).InCampo = 1 Then
                        lst(k).Players(i).IdFormazione = idforma
                        idforma += 1
                    End If
                Next

                If currlega.Settings.Bonus.EnableBonusDefense AndAlso ndgoodd = nd Then
                    Dim bonus As Single = 0
                    Select Case ndgoodd
                        Case 3 : bonus = CSng(currlega.Settings.Bonus.BonusDefense(3) / 10)
                        Case 4 : bonus = CSng(currlega.Settings.Bonus.BonusDefense(4) / 10)
                        Case Is > 4 : bonus = CSng(currlega.Settings.Bonus.BonusDefense(5) / 10)
                    End Select
                    lst(k).BonusDifesa = bonus
                Else
                    lst(k).BonusDifesa = 0
                End If

                If currlega.Settings.Bonus.EnableCenterField AndAlso ndgoodc = nc Then
                    Dim bonus As Single = 0
                    Select Case ndgoodc
                        Case 3 : bonus = CSng(currlega.Settings.Bonus.BonusCenterField(3) / 10)
                        Case 4 : bonus = CSng(currlega.Settings.Bonus.BonusCenterField(4) / 10)
                        Case Is > 4 : bonus = CSng(currlega.Settings.Bonus.BonusCenterField(5) / 10)
                    End Select
                    lst(k).BonusCentroCampo = bonus
                Else
                    lst(k).BonusCentroCampo = 0
                End If

                If currlega.Settings.Bonus.EnableBonusAttack AndAlso ndgooda = na Then
                    Dim bonus As Single = 0
                    Select Case ndgooda
                        Case 2 : bonus = CSng(currlega.Settings.Bonus.BonusAttack(2) / 10)
                        Case Is > 2 : bonus = CSng(currlega.Settings.Bonus.BonusAttack(3) / 10)
                    End Select
                    lst(k).BonusAttacco = bonus
                Else
                    lst(k).BonusAttacco = 0
                End If

            Next

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
        End Try

        Return lst

    End Function


    Sub GetAutomaticFormation(ByVal Giornata As Integer)
        For i As Integer = 0 To currlega.Formazioni.Count - 1
            currlega.Formazioni(i).Players = currlega.GetAutomaticFormation(currlega.Formazioni(i).IdTeam, Giornata)
            currlega.Formazioni(i).Save()
        Next
    End Sub

    Function GetAutomaticFormation(ByVal IdTeam As Integer, ByVal Giornata As Integer) As List(Of Formazione.PlayerFormazione)

        'Dim fc As New List(Of Formazione.PlayerFormazione)
        'Dim ft As New List(Of Formazione.PlayerFormazione)
        Dim f As New List(Of Formazione.PlayerFormazione)
        'Dim r As New LegaObject.Team(IdTeam, "")
        'Dim rp As New List(Of LegaObject.Team.Player)
        'Dim portteam As String = ""

        ''Carico la rosa'
        'r.Load(True, True, True)

        'rp.AddRange(r.Players)

        'Dim dir As String = GetLegaTemDirectory()
        'Dim str As New System.Text.StringBuilder

        'For i As Integer = 0 To rp.Count - 1
        '    str.AppendLine(rp(i).Nome & "|" & rp(i).Squadra & "|" & rp(i).Rating)
        'Next
        'IO.File.WriteAllText(dir & "\" & IdTeam & "-rating.txt", str.ToString)

        ''Calcolo il rating dei vari giocatori'
        'rp = CalculateRatingPresenze(Giornata, IdTeam, False, rp)

        ''Determino la formazione automatica'
        'f = DetectAutomaticForma(IdTeam, False, rp)

        ''Controllo la formazione'
        'Dim recheck As Boolean = False
        'Dim pc As New List(Of String)
        'For i As Integer = f.Count - 1 To 0 Step -1
        '    If f(i).Tag = "1" AndAlso f(i).Type = 0 Then
        '        'Verifico un panchinaro utile'
        '        Dim good As Boolean = False
        '        For j As Integer = 0 To f.Count - 1
        '            If f(j).Type = 1 AndAlso f(i).Ruolo = f(j).Ruolo AndAlso pc.Contains(f(j).Nome & "-" & f(j).Squadra) = False AndAlso pc.Count < 3 Then
        '                If webdata.WebPlayers.ContainsKey(Giornata & "-" & f(j).Nome & "-" & f(j).Squadra) Then
        '                    Dim wp As wData.wPlayer = webdata.WebPlayers(Giornata & "-" & f(j).Nome & "-" & f(j).Squadra)
        '                    If wp.Titolare / webdata.NumSitePlayer > 0.79 Then
        '                        good = True
        '                        pc.Add(f(j).Nome & "-" & f(j).Squadra)
        '                        Exit For
        '                    End If
        '                End If
        '            End If
        '        Next
        '        If good = False Then
        '            recheck = True
        '        Else
        '            For h As Integer = 0 To rp.Count - 1
        '                If f(i).Nome = rp(h).Nome AndAlso f(i).Squadra = rp(h).Squadra Then
        '                    rp(h).Tag = "0"
        '                    Exit For
        '                End If
        '            Next
        '        End If
        '    End If
        'Next
        'If recheck Then
        '    For h As Integer = rp.Count - 1 To 0 Step -1
        '        rp(h).Type = 0
        '        rp(h).Schierato = 0
        '    Next
        '    'Calcolo il rating dei vari giocatori'
        '    rp = CalculateRatingPresenze(Giornata, IdTeam, True, rp)
        '    'Determino la formazione automatica'
        '    f = DetectAutomaticForma(IdTeam, True, rp)
        'End If

        ''Verifico se ci sno attaccanti della stessa squadra'
        'If CheckSameTeamForward(f) Then
        '    f = DetectAutomaticForma(IdTeam, True, f)
        'End If

        Return f

    End Function

    Private Function CheckSameTeamForward(ByVal f As List(Of Formazione.PlayerFormazione)) As Boolean
        Dim recheck As Boolean = False
        Dim attsq As New List(Of String)
        For i As Integer = 0 To f.Count - 1
            If f(i).Type = 1 AndAlso f(i).Ruolo = "A" Then
                If attsq.Contains(f(i).Squadra) = True Then
                    f(i).Rating = f(i).Rating - 20
                    For k As Integer = i + 1 To f.Count - 1
                        If f(k).Type = 2 AndAlso f(k).Ruolo = "A" Then
                            For h As Integer = 0 To f.Count - 1
                                If f(h).Type = 0 AndAlso f(h).Nome = f(k).Nome Then
                                    f(h).Rating = f(h).Rating + 20
                                    Exit For
                                End If
                            Next
                            Exit For
                        End If
                    Next
                    recheck = True
                Else
                    attsq.Add(f(i).Squadra)
                End If
            End If
        Next
        If recheck Then
            For h As Integer = f.Count - 1 To 0 Step -1
                If f(h).Type = 2 Then
                    f.RemoveAt(h)
                Else
                    f(h).Type = 0
                End If
            Next
        End If
        Return recheck
    End Function

    Private Function CalculateRatingPresenze(ByVal Giornata As Integer, ByVal IdTeam As Integer, ByVal rechek As Boolean, ByVal rp As List(Of LegaObject.Formazione.PlayerFormazione)) As List(Of LegaObject.Formazione.PlayerFormazione)

        Dim dir As String = GetLegaTemDirectory()
        Dim str As New System.Text.StringBuilder

        For i As Integer = 0 To rp.Count - 1
            If rp(i).Nome = "MATRI" Then
                rp(i).Nome = rp(i).Nome
            End If
            rp(i).Rating = SystemFunction.General.GetRatingForma(rp(i), Giornata)
            If webdata.WebPlayers.ContainsKey(Giornata & "-" & rp(i).Nome & "-" & rp(i).Squadra) Then
                Dim wp As wData.wPlayer = webdata.WebPlayers(Giornata & "-" & rp(i).Nome & "-" & rp(i).Squadra)
                Dim perctit As Integer = CInt((wp.Titolare * 100) / webdata.NumSitePlayer) 'percentuale di titolarieta'
                If rp(i).Ruolo <> "P" AndAlso wp.Titolare / webdata.NumSitePlayer > 0.39 AndAlso (rp(i).Tag = "0" OrElse rp(i).Tag = "") Then
                    rp(i).Rating = rp(i).Rating + 80
                    If perctit < 50 Then
                        rp(i).Tag = "1"
                    Else
                        rp(i).Tag = "0"
                    End If
                    If rp(i).Tag = "0" AndAlso perctit > 65 Then
                        Select Case rp(i).Ruolo
                            Case "P" : rp(i).Rating = rp(i).Rating + 0
                            Case "D" : rp(i).Rating = rp(i).Rating + 0
                            Case "C" : rp(i).Rating = rp(i).Rating + 7
                                If currlega.Settings.SubstitutionType = LegaSettings.eSubstitutionType.ChangeModule Then
                                    rp(i).Rating = rp(i).Rating + 5
                                End If
                            Case "A"
                                rp(i).Rating = rp(i).Rating + 22
                                If currlega.Settings.SubstitutionType = LegaSettings.eSubstitutionType.ChangeModule Then
                                    rp(i).Rating = rp(i).Rating + 5
                                End If
                        End Select
                    Else
                        If perctit < 20 Then
                            rp(i).Rating = rp(i).Rating - 90
                        Else
                            '    Select Case rp(i).Ruolo
                            '        Case "P" : rp(i).Rating = rp(i).Rating + 0
                            '        Case "D" : rp(i).Rating = rp(i).Rating + 0
                            '        Case "C" : rp(i).Rating = rp(i).Rating + 2
                            '        Case "A" : rp(i).Rating = rp(i).Rating + 8
                            'End Select
                            rp(i).Rating = rp(i).Rating - 30
                        End If
                    End If
                Else
                    rp(i).Tag = "0"
                    rp(i).Rating = rp(i).Rating + wp.Titolare * 16
                    rp(i).Rating = rp(i).Rating + CInt((wp.Panchina * 5) * (0.7 + rp(i).StatisticLast.pGiocate / 3))
                    rp(i).Rating = rp(i).Rating + CInt((webdata.NumSitePlayer - (wp.Titolare + wp.Panchina)) * (rp(i).StatisticLast.pGiocate))
                    If rp(i).Tag = "0" AndAlso wp.Titolare > 1 Then
                        Select Case rp(i).Ruolo
                            Case "P" : rp(i).Rating = rp(i).Rating + 0
                            Case "D" : rp(i).Rating = rp(i).Rating + 0
                            Case "C" : rp(i).Rating = rp(i).Rating + 2
                            Case "A" : rp(i).Rating = rp(i).Rating + 8
                        End Select
                    Else
                        If wp.Titolare < 2 AndAlso rp(i).StatisticLast.pGiocate < 2 AndAlso wp.Panchina < 3 Then
                            rp(i).Rating = rp(i).Rating - 90
                        Else
                            Select Case rp(i).Ruolo
                                Case "P" : rp(i).Rating = rp(i).Rating + 0
                                Case "D" : rp(i).Rating = rp(i).Rating + 0
                                Case "C" : rp(i).Rating = rp(i).Rating + 2
                                Case "A" : rp(i).Rating = rp(i).Rating + 8
                            End Select
                        End If
                    End If
                End If
            End If
            If rp(i).Rating < 0 Then rp(i).Rating = 0
            str.AppendLine(rp(i).Nome & "|" & rp(i).Squadra & "|" & rp(i).Rating)
        Next
        If rechek Then
            IO.File.WriteAllText(dir & "\" & IdTeam & "-rating-presence-recheck.txt", str.ToString)
        Else
            IO.File.WriteAllText(dir & "\" & IdTeam & "-rating-presence.txt", str.ToString)
        End If
        Return rp
    End Function

    'Private Function DetectAutomaticForma(ByVal IdTeam As Integer, ByVal rechek As Boolean, ByRef rp As List(Of LegaObject.Team.Player)) As List(Of LegaObject.Formazione.PlayerFormazione)

    '    Dim f As New List(Of LegaObject.Team.Player)
    '    Dim ft As New List(Of LegaObject.Team.Player)
    '    Dim fc As New List(Of LegaObject.Team.Player)

    '    Dim portteam As String = ""

    '    'Ordino i giocatori sulla base del loro rating'
    '    fc = LegaObject.Team.Sort(rp, "rating", True)

    '    'Determino i titolari'
    '    Dim ntit As Integer = 0
    '    Dim np As Integer = 0
    '    Dim nd As Integer = 0
    '    Dim nc As Integer = 0
    '    Dim na As Integer = 0

    '    For i As Integer = 0 To fc.Count - 1
    '        fc(i).Type = 0
    '        fc(i).Schierato = 0
    '        If CheckMudule1(fc(i).Ruolo, np, nd, nc, na) Then
    '            fc(i).Schierato = 1
    '            fc(i).Type = 0
    '            fc(i).IdRosa = i
    '            Select Case fc(i).Ruolo
    '                Case "P" : np = np + 1 : portteam = fc(i).Squadra
    '                Case "D" : nd = nd + 1
    '                Case "C" : nc = nc + 1
    '                Case "A" : na = na + 1
    '            End Select
    '            ntit = ntit + 1
    '            'If ntit > 11 Then Exit For
    '        End If
    '    Next

    '    'Ordino i giocatori sulla base ruolo'
    '    ft = LegaObject.Team.Sort(fc, "", False)
    '    For i As Integer = 0 To ft.Count - 1
    '        ft(i).IdRosa = i + 1
    '    Next
    '    f.AddRange(ft)

    '    'Determino i panchinari'
    '    Dim p() As String = {"P", "A", "C", "D"}
    '    Dim ind As Integer = 1
    '    Dim s As Integer = 0

    '    'Controllo se esiste un secondo portire della stessa squadra'
    '    For i As Integer = 0 To fc.Count - 1
    '        If fc(i).Schierato = 0 AndAlso fc(i).Ruolo = "P" AndAlso portteam = fc(i).Squadra Then
    '            f.Add(New Team.Player(1, fc(i).Ruolo, fc(i).Nome, fc(i).Squadra, 1, 1, 0))
    '            ind = ind + 1
    '            s = 1
    '            Exit For
    '        End If
    '    Next

    '    'Determino il resto dei panchinari'
    '    For i As Integer = s To p.Length - 1

    '        Dim nump As Integer = 1
    '        Dim maxp As Integer = 2

    '        If p(i) = "P" Then maxp = 1

    '        For k As Integer = 0 To fc.Count - 1
    '            If fc(k).Schierato = 0 AndAlso fc(k).Ruolo = p(i) Then
    '                f.Add(New Team.Player(ind, fc(k).Ruolo, fc(k).Nome, fc(k).Squadra, 1, 1, 0))
    '                nump = nump + 1
    '                ind = ind + 1
    '            End If
    '            If nump > maxp Then Exit For
    '        Next
    '    Next

    '    Dim dir As String = GetLegaTemDirectory()
    '    Dim str As New System.Text.StringBuilder
    '    For i As Integer = 0 To f.Count - 1
    '        str.AppendLine(f(i).Ruolo & "|" & f(i).Nome & "|" & f(i).Squadra & "|" & f(i).Rating & "|" & f(i).Schierato & "|" & f(i).Type)
    '    Next
    '    If rechek Then
    '        IO.File.WriteAllText(dir & "\" & IdTeam & "-rating-tot.txt", str.ToString)
    '    Else
    '        IO.File.WriteAllText(dir & "\" & IdTeam & "-rating-tot-recheck.txt", str.ToString)
    '    End If

    '    Return f
    'End Function

    Shared Function GetGoodForBonus(ByVal p As Formazione.PlayerFormazione) As Integer
        If currlega.Settings.Bonus.EnableBonusDefense Then
            Dim val As Single = p.Dati.Pt
            If currlega.Settings.Bonus.BonudAttackSource = "vote" Then val = p.Dati.Vt
            If val >= currlega.Settings.Bonus.BonusAttackOverEqual Then
                Return 1
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function

    Function GetLegaPlayerList() As List(Of String)
        Dim l As New List(Of String)
        Try
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT tbrose.nome AS nome FROM tbrose;", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    l.Add(ds.Tables(0).Rows(i).Item("nome").ToString)
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return l
    End Function

    Function GetFreePlayerList() As Dictionary(Of String, String)
        Return GetFreePlayerList(New List(Of String))
    End Function

    Function GetFreePlayerList(ByVal Role As String) As Dictionary(Of String, String)
        Dim r As New List(Of String)
        r.Add(Role)
        Return GetFreePlayerList(r)
    End Function

    Function GetFreePlayerList(ByVal RoleList As List(Of String)) As Dictionary(Of String, String)
        Dim l As New Dictionary(Of String, String)
        Try
            Dim str As String = ""
            If RoleList.Count > 0 Then
                For i As Integer = 0 To RoleList.Count - 1
                    str = str & "or player.ruolo='" & RoleList(i) & "' "
                Next
                If str.Length > 0 Then
                    str = " AND (" & str.Substring(3) & ")"
                End If
            End If
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT player.ruolo,player.nome,player.squadra FROM player LEFT JOIN tbrose ON tbrose.nome = player.nome WHERE tbrose.nome is null " & str & " ORDER BY player.nome;", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    l.Add(ds.Tables(0).Rows(i).Item("nome").ToString(), ds.Tables(0).Rows(i).Item("squadra").ToString())
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return l
    End Function

    Function GetDictionaryFreePlayerList() As Dictionary(Of String, Dictionary(Of String, String))
        Return GetDictionaryFreePlayerList(New List(Of String))
    End Function

    Function GetDictionaryFreePlayerList(ByVal Role As String) As Dictionary(Of String, Dictionary(Of String, String))
        Dim r As New List(Of String)
        r.Add(Role)
        Return GetDictionaryFreePlayerList(r)
    End Function

    Function GetDictionaryFreePlayerList(ByVal RoleList As List(Of String)) As Dictionary(Of String, Dictionary(Of String, String))
        Dim l As New Dictionary(Of String, Dictionary(Of String, String))
        Try
            Dim str As String = ""
            If RoleList.Count > 0 Then
                For i As Integer = 0 To RoleList.Count - 1
                    str = str & "or player.ruolo='" & RoleList(i) & "' "
                Next
                If str.Length > 0 Then
                    str = " AND (" & str.Substring(3) & ")"
                End If
            End If
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT player.ruolo,player.nome,player.squadra FROM player LEFT JOIN tbrose ON tbrose.nome = player.nome WHERE tbrose.nome is null " & str & " ORDER BY player.ruolo,player.nome;", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim r As String = ds.Tables(0).Rows(i).Item("ruolo").ToString
                    Dim n As String = ds.Tables(0).Rows(i).Item("nome").ToString
                    If l.ContainsKey(r) = False Then
                        l.Add(r, New Dictionary(Of String, String))
                    End If
                    l.Item(r).Add(ds.Tables(0).Rows(i).Item("nome").ToString(), ds.Tables(0).Rows(i).Item("squadra").ToString())
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return l
    End Function

    Function GetDictionaryTeamPlayerList() As Dictionary(Of String, List(Of String))
        Dim l As New Dictionary(Of String, List(Of String))
        Try
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT tbplayer.squadra,tbplayer.nome AS nome FROM tbplayer;", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If l.ContainsKey(ds.Tables(0).Rows(i).Item("squadra").ToString) = False Then l.Add(ds.Tables(0).Rows(i).Item("squadra").ToString, New List(Of String))
                    l(ds.Tables(0).Rows(i).Item("squadra").ToString).Add(ds.Tables(0).Rows(i).Item("nome").ToString)
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return l
    End Function

    Function GetDictionaryTeamRolePlayerList() As Dictionary(Of String, Dictionary(Of String, List(Of String)))
        Dim l As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
        Try
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT tbplayer.squadra,tbplayer.ruolo,tbplayer.nome AS nome FROM tbplayer;", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim sq As String = ds.Tables(0).Rows(i).Item("squadra").ToString
                    Dim r As String = ds.Tables(0).Rows(i).Item("ruolo").ToString
                    Dim nome As String = ds.Tables(0).Rows(i).Item("nome").ToString
                    If l.ContainsKey(sq) = False Then l.Add(sq, New Dictionary(Of String, List(Of String)))
                    If l(sq).ContainsKey(r) = False Then l(sq).Add(r, New List(Of String))
                    l(sq)(r).Add(nome)
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return l
    End Function

    Function GetAllPlayerList() As List(Of String)
        Return GetAllPlayerList(New List(Of String))
    End Function

    Function GetAllPlayerList(ByVal Role As String) As List(Of String)
        Dim r As New List(Of String)
        r.Add(Role)
        Return GetAllPlayerList(r)
    End Function

    Function GetAllPlayerList(ByVal RoleList As List(Of String)) As List(Of String)
        Dim l As New List(Of String)
        Try
            Dim str As String = ""
            If RoleList.Count > 0 Then
                For i As Integer = 0 To RoleList.Count - 1
                    str = str & "or ruolo='" & RoleList(i) & "' "
                Next
                If str.Length > 0 Then
                    str = str.Substring(3)
                End If
            End If
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT player.nome AS nome FROM " & str & " player;", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    l.Add(ds.Tables(0).Rows(i).Item("nome").ToString)
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return l
    End Function

    Function GetDictionaryAllPlayerListAndRoule(ByVal RoleList As List(Of String)) As Dictionary(Of String, String)
        Dim l As New Dictionary(Of String, String)
        Try
            Dim str As String = ""
            If RoleList IsNot Nothing AndAlso RoleList.Count > 0 Then
                For i As Integer = 0 To RoleList.Count - 1
                    str = str & "or ruolo='" & RoleList(i) & "' "
                Next
                If str.Length > 0 Then
                    str = "WHERE " & str.Substring(3)
                End If
            End If
            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT player.nome AS nome,player.squadra,player.ruolo AS ruolo FROM player " & str & ";", conn)
            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim nome As String = ds.Tables(0).Rows(i).Item("nome").ToString.ToLower
                    Dim squadra As String = ds.Tables(0).Rows(i).Item("squadra").ToString.ToLower
                    Dim r As String = ds.Tables(0).Rows(i).Item("ruolo").ToString
                    Dim key As String = squadra & "-" & nome
                    If l.ContainsKey(key) = False Then l.Add(key, r)
                Next
            End If
        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
        Return l
    End Function

    Function GetDataChart(ByVal TypeOfData As String, ByVal Nome As String) As List(Of Double)

        Dim l As New List(Of Double)
        Dim f As String = "gs"
        Dim divide As Boolean = False

        Select Case TypeOfData
            Case "gs" : f = "gs"
            Case "gf" : f = "gf"
            Case "amm" : f = "amm"
            Case "ass" : f = "ass"
            Case "esp" : f = "esp"
            Case "p.g." : f = "1"
            Case "tit" : f = "tit"
            Case "rigt" : f = "rigt"
            Case "rigp" : f = "rigp"
            Case "rigs" : f = "rigs"
            Case "min" : f = "mm"
            Case "vt.", "m.v." : f = "voto" : divide = True
            Case "pt.", "m.p." : f = "pt" : divide = True
        End Select

        For i As Integer = 0 To currlega.Settings.NumberOfDays - 1
            l.Add(-100)
        Next

        Try

            Dim ds As DataSet = ExecuteSqlReturnDataSet("SELECT gio," & f & " AS val FROM (select dat.*,tab.mm,tab.tit from tbdati as dat left join tbtabellini as tab on (tab.gio=dat.gio and tab.nome=dat.nome and tab.squadra=dat.squadra) WHERE dat.nome='" & Nome & "') order by gio;", conn)

            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim gg As Integer = CInt(ds.Tables(0).Rows(i).Item("gio")) - 1
                    l(gg) = ReadFieldIntegerData(ds.Tables(0).Rows(i).Item("val"), 0)
                    If divide = True Then l(gg) = l(gg)
                Next
            End If

        Catch ex As Exception
            Call WriteError(System.Reflection.MethodBase.GetCurrentMethod.DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try

        Return l

    End Function

End Class