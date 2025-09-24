Imports System.Text.RegularExpressions
Imports System.IO
Imports iFanta.SystemFunction.FileAndDirectory
Imports iFanta.SystemFunction.DataBase

Partial Class LegaObject

    <Serializable()>
    Public Class Formazione

        Sub New()

        End Sub

        Sub New(ByVal IdTeam As Integer, ByVal Nome As String, ByVal Allenatore As String)
            Me.IdTeam = IdTeam
            Me.Nome = Nome
            Me.Allenatore = Allenatore
        End Sub

        Sub New(ByVal IdTeam As Integer, ByVal Nome As String, ByVal Allenatore As String, ByVal Giornata As Integer)
            Me.IdTeam = IdTeam
            Me.Nome = Nome
            Me.Allenatore = Allenatore
            Me.Giornata = Giornata
        End Sub

        Sub New(ByVal Giornata As Integer, ByVal IdTeam As Integer)
            Me.Giornata = Giornata
            Me.IdTeam = IdTeam
        End Sub

        Public Property IdTeam() As Integer = 0
        Public Property Giornata() As Integer = 1
        Public Property Nome() As String = ""
        Public Property Allenatore() As String = ""
        Public Property Modulo() As String = ""
        Public Property NumberPlayerInCampo() As Integer = 0
        Public Property BonusDifesa() As Single = 0
        Public Property BonusCentroCampo() As Single = 0
        Public Property BonusAttacco() As Single = 0
        Public Property ModuleSubstitution() As Boolean = False
        Public Property Pt() As Single = 0
        Public ReadOnly Property Titolari() As List(Of String) = New List(Of String)
        Public ReadOnly Property Panchinari() As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
        Public Property Players() As List(Of PlayerFormazione) = New List(Of PlayerFormazione)

        Shared Sub Delete(Top As Boolean)
            Dim tb As String = GetTableName(Top)
            ExecuteSql("DELETE FROM " & tb & ";", conn)
        End Sub

        Shared Sub Delete(ByVal Giornata As Integer, Top As Boolean)
            Dim tb As String = GetTableName(Top)
            ExecuteSql("DELETE FROM " & tb & " WHERE gio=" & Giornata & ";", conn)
        End Sub

        Sub DeleteBonus(ByVal IdTeam As Integer, Top As Boolean)
            Dim tb As String = GetTableName(Top)
            ExecuteSql("DELETE FROM " & tb & " WHERE idteam=" & IdTeam & " and type>9;", conn)
        End Sub

        Shared Sub Delete(ByVal Giornata As Integer, ByVal IdTeam As Integer, Top As Boolean)
            Dim tb As String = GetTableName(Top)
            ExecuteSql("DELETE FROM " & tb & " WHERE gio=" & Giornata & " AND idteam=" & IdTeam & ";", conn)
        End Sub

        Shared Sub Delete(ByVal Giornata As Integer, ByVal Teams As List(Of Formazione), Top As Boolean)
            Dim lst As New List(Of Integer)
            For i As Integer = 0 To lst.Count - 1
                lst.Add(Teams(i).IdTeam)
            Next
            Call Delete(Giornata, lst, Top)
        End Sub

        Shared Sub Delete(ByVal Giornata As Integer, ByVal IdList As List(Of Integer), Top As Boolean)
            Dim tb As String = GetTableName(Top)
            ExecuteSql("DELETE FROM " & tb & " WHERE gio=" & Giornata & " AND idteam in (" & SystemFunction.Convertion.ConvertListIntegerToString(IdList, ",") & ");", conn)
        End Sub

        Sub DeleteBonus(ByVal Giornata As Integer, ByVal IdTeam As Integer, Top As Boolean)
            Dim tb As String = GetTableName(Top)
            ExecuteSql("DELETE FROM " & tb & " WHERE gio=" & Giornata & " AND idteam=" & IdTeam & " and type>9;", conn)
        End Sub

        Shared Function GetTableName(Top As Boolean) As String
            Dim tb As String = tbforma
            If Top Then tb = "tbformazionitop"
            Return tb
        End Function

        Sub Save()
            Call Save(Me.Players, Me.Giornata, Me.IdTeam, False)
        End Sub

        Sub Save(Players As List(Of PlayerFormazione), Giornata As Integer)
            Call Save(Players, Giornata, Me.IdTeam, False)
        End Sub

        Sub Save(Top As Boolean)
            Call Save(Me.Players, Me.Giornata, Me.IdTeam, Top)
        End Sub

        Shared Sub Save(Players As List(Of PlayerFormazione), Giornata As Integer, IdTeam As Integer, Top As Boolean)
            Dim lst As New List(Of Formazione)
            lst.Add(New Formazione(Giornata, IdTeam))
            lst(0).Players.AddRange(Players)
            Call Save(lst, Giornata, Top)
        End Sub

        Shared Sub Save(fList As List(Of Formazione), Giornata As Integer, Top As Boolean)

            Dim tb As String = GetTableName(Top)
            Dim r As Integer = 0
            Dim str As New System.Text.StringBuilder
            Dim sins As String = "INSERT INTO " & tb & " (gio,idteam,idrosa,type,idformazione,incampo,ruolo,nome,squadra,vote,amm,esp,ass,autogol,gs,gf,rigs,rigp,pt) VALUES "

            Try

                For k As Integer = 0 To fList.Count - 1

                    If fList(k) IsNot Nothing Then

                        For i As Integer = 0 To fList(k).Players.Count - 1
                            str.Append(",(" & Giornata & ",")
                            str.Append(fList(k).IdTeam & ",")
                            str.Append(fList(k).Players(i).IdRosa & ",")
                            str.Append(fList(k).Players(i).Type & ",")
                            str.Append(fList(k).Players(i).IdFormazione & ",")
                            str.Append(fList(k).Players(i).InCampo & ",")
                            str.Append("'" & fList(k).Players(i).Ruolo & "',")
                            str.Append("'" & fList(k).Players(i).Nome & "',")
                            str.Append("'" & fList(k).Players(i).Squadra & "',")
                            str.Append(fList(k).Players(i).Dati.Vt * 10 & ",")
                            str.Append(fList(k).Players(i).Dati.Amm & ",")
                            str.Append(fList(k).Players(i).Dati.Esp & ",")
                            str.Append(fList(k).Players(i).Dati.Ass & ",")
                            str.Append(fList(k).Players(i).Dati.AutG & ",")
                            str.Append(fList(k).Players(i).Dati.Gs & ",")
                            str.Append(fList(k).Players(i).Dati.Gf & ",")
                            str.Append(fList(k).Players(i).Dati.RigS & ",")
                            str.Append(fList(k).Players(i).Dati.RigP & ",")
                            str.Append(fList(k).Players(i).Dati.Pt * 10 & ")")
                            r += 1
                        Next
                        If r > blkrec Then
                            Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)
                            str = New System.Text.StringBuilder
                            r = 0
                        End If
                    End If
                Next

                If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                'Salvataggio punti da bonus'
                sins = "INSERT INTO " & tb & " (gio,idteam,type,pt) VALUES "
                str = New System.Text.StringBuilder
                r = 0

                For k As Integer = 0 To fList.Count - 1

                    If fList(k) IsNot Nothing Then

                        If fList(k).BonusDifesa > 0 Then
                            str.AppendLine(",(" & Giornata & "," & fList(k).IdTeam & ",10," & fList(k).BonusDifesa * 10 & ")")
                            r += 1
                        End If
                        If fList(k).BonusCentroCampo > 0 Then
                            str.AppendLine(",(" & Giornata & "," & fList(k).IdTeam & ",20," & fList(k).BonusCentroCampo * 10 & ")")
                            r += 1
                        End If
                        If fList(k).BonusAttacco > 0 Then
                            str.AppendLine(",(" & Giornata & "," & fList(k).IdTeam & ",30," & fList(k).BonusAttacco * 10 & ")")
                            r += 1
                        End If
                        If fList(k).ModuleSubstitution Then
                            str.AppendLine(",(" & Giornata & "," & fList(k).IdTeam & ",40,1)")
                            r += 1
                        End If
                    End If
                Next

                If r > 0 Then Call ExecuteSql(sins & str.ToString.Substring(1) & ";", conn)

                currlega.GiornataCorrente = -1
                currlega.Classifica.History.Clear()

            Catch ex As Exception
                Call WriteError("Formazione", "Save", ex.Message)
            End Try

        End Sub

        Sub SaveBasic()

            Dim tb As String = tbforma
            Dim str As New System.Text.StringBuilder

            If Me.Players.Count > 0 Then
                For i As Integer = 0 To Me.Players.Count - 1
                    str.Append(",(")
                    str.Append(Me.Giornata & ",")
                    str.Append(Me.IdTeam & ",")
                    str.Append(Me.Players(i).IdRosa & ",")
                    str.Append(Me.Players(i).Type & ",")
                    str.Append(Me.Players(i).IdFormazione & ",")
                    str.Append("'" & Me.Players(i).Ruolo & "',")
                    str.Append("'" & Me.Players(i).Nome & "',")
                    str.Append("'" & Me.Players(i).Squadra & "',")
                    str.Append("-100,")
                    str.Append("-100)")
                Next
                ExecuteSql("INSERT INTO " & tb & " (gio,idteam,idrosa,type,idformazione,ruolo,nome,squadra,vote,pt) VALUES " & str.ToString.Substring(1), conn)
            End If

            currlega.GiornataCorrente = -1
            currlega.Classifica.History.Clear()

        End Sub

        Sub Load()
            Call Load(False, False)
        End Sub

        Sub Load(ByVal Top As Boolean)
            Load(Top, False)
        End Sub

        Sub Load(ByVal Top As Boolean, ByVal OnlyTypeZero As Boolean)
            Try

                Dim nd As Integer = 0
                Dim nc As Integer = 0
                Dim na As Integer = 0
                Dim str As New System.Text.StringBuilder
                Dim tb As String = "formazione"

                If Top Then tb = tb & "_top"

                Me.Players.Clear()
                Me.Titolari.Clear()
                Me.Panchinari.Clear()
                _Pt = 0
                _bonusdifesa = 0
                _bonuscentrocampo = 0
                _bonusattacco = 0

                str.Append("SELECT * FROM " & tb & " WHERE idteam=" & Me.IdTeam & " and gio=" & Me.Giornata & " ")

                If OnlyTypeZero Then str.Append(" AND type=0 ")

                If Top Then
                    str.Append("ORDER BY pt DESC;")
                Else
                    str.Append("ORDER BY idrosa,idformazione,nometeam;")
                End If

                Dim ds As DataSet = ExecuteSqlReturnDataSet(str.ToString, conn)

                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then

                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1

                        Dim t As Integer = CInt(ds.Tables(0).Rows(i).Item("type"))

                        Select Case t
                            Case 0, 1, 2

                                Dim p As PlayerFormazione = GetPlayerFromDataRow(ds.Tables(0).Rows(i))

                                Me.Players.Add(p)

                                If p.Type = 1 Then
                                    If Me.Titolari.Contains(p.Nome) = False Then Me.Titolari.Add(p.Nome)
                                    Select Case p.Ruolo
                                        Case "D" : nd += 1
                                        Case "C" : nc += 1
                                        Case "A" : na += 1
                                    End Select
                                Else
                                    If Me.Panchinari.ContainsKey(p.Nome) = False Then Me.Panchinari.Add(p.Nome, p.IdRosa)
                                End If

                                If p.InCampo = 1 AndAlso p.Dati.Pt > -10 Then
                                    _pt += p.Dati.Pt
                                End If

                                If (p.Type = 2 AndAlso p.IdFormazione > currlega.Settings.NumberOfReserve + 11) Then
                                    p.Type = 0
                                End If

                            Case 10
                                _bonusdifesa = CSng(CInt(ds.Tables(0).Rows(i).Item("pt")) / 10)
                                _pt += _bonusdifesa
                            Case 20
                                _bonuscentrocampo = CSng(CInt(ds.Tables(0).Rows(i).Item("pt")) / 10)
                                _pt += _bonuscentrocampo
                            Case 30
                                _bonusattacco = CSng(CInt(ds.Tables(0).Rows(i).Item("pt")) / 10)
                                _pt += _bonusattacco
                            Case 40
                                _modulesubstitution = True
                        End Select
                    Next

                Else

                    'In caso contrario carico la rosa'

                    Dim pl As New List(Of Team.Player)
                    pl = LegaObject.Team.Sort(currlega.Teams(0).GetPlayer(_nome, "", True, Me.Giornata), "", False)
                    For i As Integer = 0 To pl.Count - 1
                        pl(i).IdRosa = i + 1
                        Me.Players.Add(New PlayerFormazione(pl(i)))
                    Next


                End If

                ds.Dispose()

                _modulo = nd & "-" & nc & "-" & na

            Catch ex As Exception
                Dim a As String = ex.Message
            End Try
        End Sub

        Public Function GetPlayerFromDataRow(data As Data.DataRow) As PlayerFormazione

            Dim p As New PlayerFormazione

            'Dati generali'
            p.IdRosa = ReadFieldIntegerData(data.Item("idrosa"), -1)
            p.IdFormazione = ReadFieldIntegerData(data.Item("idformazione"))
            p.Type = ReadFieldIntegerData(data.Item("type"))
            p.InCampo = ReadFieldIntegerData(data.Item("incampo"))
            p.Ruolo = ReadFieldStringData(data.Item("ruolo"), "?")
            p.Nome = ReadFieldStringData(data.Item("nome"))
            p.Squadra = ReadFieldStringData(data.Item("squadra"))
            p.Nat = ReadFieldStringData(data.Item("nat"))
            p.NatCode = ReadFieldStringData(data.Item("natcode"))

            'Dati macth'
            p.Match.TeamA = ReadFieldStringData(data.Item("teama"))
            p.Match.TeamB = ReadFieldStringData(data.Item("teamb"))
            p.Match.Time = ReadFieldTimeData(data.Item("timem"))

            'Dati formazione'
            p.Dati.Gs = ReadFieldIntegerData(data.Item("gs"))
            p.Dati.Gf = ReadFieldIntegerData(data.Item("gf"))
            p.Dati.Amm = ReadFieldIntegerData(data.Item("amm"))
            p.Dati.Esp = ReadFieldIntegerData(data.Item("esp"))
            p.Dati.Ass = ReadFieldIntegerData(data.Item("ass"))
            p.Dati.RigP = ReadFieldIntegerData(data.Item("rigp"))
            p.Dati.RigS = ReadFieldIntegerData(data.Item("rigs"))
            p.Dati.Vt = CSng(ReadFieldIntegerData(data.Item("vote")) / 10)
            p.Dati.Pt = CSng(ReadFieldIntegerData(data.Item("pt")) / 10)

            'Dati statistici totali'
            p.StatisticAll = p.GetStatsticAllDataFromDataRow(data)

            'Dati statistici ultime giornate'
            p.StatisticLast = p.GetStatsticLastlDataFromDataRow(data)

            'Calcolo variazioni ultimi giorni
            p.Variation = p.GetVariation(p.StatisticAll, p.StatisticLast)

            Return p

        End Function

        Public Shared Function Sort(ByVal Data As List(Of PlayerFormazione), ByVal SortKey As String, ByVal Reverse As Boolean) As List(Of PlayerFormazione)
            Dim a() As PlayerFormazione = Data.ToArray
            Dim s As New LegaObject.Team.PlayerSorter(Team.PlayerSorter.eType.Forma, SortKey, Reverse)
            Array.Sort(a, s)
            Dim ris As New List(Of PlayerFormazione)
            ris.AddRange(a)
            Return ris
        End Function

        Public Function Clone() As Formazione

            Dim formatter As Runtime.Serialization.IFormatter = New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Dim ms As New IO.MemoryStream
            formatter.Serialize(ms, Me)
            ms.Seek(0, IO.SeekOrigin.Begin)

            Dim pCopy As Formazione = CType(formatter.Deserialize(ms), Formazione)

            Return pCopy

        End Function

        <Serializable()>
        Public Class PlayerFormazione

            Inherits Team.Player

            Sub New()

            End Sub

            Sub New(p As Team.Player)
                Me.Ruolo = p.Ruolo
                Me.IdTeam = p.IdTeam
                Me.IdRosa = p.IdRosa
                Me.Costo = p.Costo
                Me.QIni = p.QIni
                Me.QCur = p.QCur
                Me.Nome = p.Nome
                Me.Squadra = p.Squadra
                Me.Nat = p.Nat
                Me.NatCode = p.NatCode
                Me.Match = p.Match.Clone
                Me.StatisticAll = p.StatisticAll.Clone
                Me.StatisticLast = p.StatisticLast.Clone
                Me.Variation = p.Variation
            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Jolly As Integer, ByVal Type As Integer, ByVal IdFormazione As Integer, ByVal Ruolo As String, ByVal Nome As String)
                Me.IdRosa = IdRosa
                Me.Jolly = Jolly
                Me.Type = Type
                Me.IdFormazione = IdFormazione
                Me.Ruolo = Ruolo
                Me.Nome = Nome
            End Sub

            Sub New(ByVal IdRosa As Integer, ByVal Jolly As Integer, ByVal Type As Integer, ByVal IdFormazione As Integer, ByVal Ruolo As String, ByVal Nome As String, ByVal InCampo As Integer)
                Me.IdRosa = IdRosa
                Me.Jolly = Jolly
                Me.Type = Type
                Me.IdFormazione = IdFormazione
                Me.Ruolo = Ruolo
                Me.Nome = Nome
                Me.InCampo = InCampo
            End Sub

            Public Property IdFormazione() As Integer = 0
            Public Property Jolly() As Integer = 0
            Public Property InCampo() As Integer = 0
            Public Property Type() As Integer = 0
            Public Property Dati As DataForma = New DataForma

            Public Function Clone() As PlayerFormazione

                Dim formatter As Runtime.Serialization.IFormatter = New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                Dim ms As New IO.MemoryStream
                formatter.Serialize(ms, Me)
                ms.Seek(0, IO.SeekOrigin.Begin)

                Dim pCopy As PlayerFormazione = CType(formatter.Deserialize(ms), PlayerFormazione)

                Return pCopy

            End Function

            <Serializable()>
            Public Class DataForma

                Public Property AutG() As Integer = 0
                Public Property Gs() As Integer = 0
                Public Property Gf() As Integer = 0
                Public Property Amm() As Integer = 0
                Public Property Esp() As Integer = 0
                Public Property Ass() As Integer = 0
                Public Property RigT() As Integer = 0
                Public Property RigS() As Integer = 0
                Public Property RigP() As Integer = 0
                Public Property Vt As Single = 0
                Public Property Pt As Single = 0
                Public Property Titolare() As Integer = 0
                Public Property Sostituito() As Integer = 0
                Public Property Subentrato() As Integer = 0
                Public Property mm() As Integer = 0

            End Class

        End Class

    End Class

End Class
