Imports System.Data
Imports System.Runtime.InteropServices.ComTypes

Namespace Torneo
    Public Class MatchsData

        Dim appSett As New PublicVariables

        Sub New(appSett As PublicVariables)
            Me.appSett = appSett
        End Sub

        Public Function ApiGetMatchsCurrentDay() As String

            Dim cday As String = "38"

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Determino la giornata corrente dei matchs per l'anno: " & appSett.Year)

            Try
                If appSett.DataFromDatabase Then
                    Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM current_championship_day")
                    If ds.Tables.Count > 0 Then
                        cday = ds.Tables(0).Rows(0).Item("gio").ToString()
                    End If
                Else

                    Dim j As String = IO.File.ReadAllText(WebData.MatchsData.GetMatchFileName(appSett))
                    Dim dicdata As Dictionary(Of String, Dictionary(Of String, Match)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Match)))(ApiGetMatchsData("-1"))
                    Dim found As Boolean = False

                    For d As Integer = 38 To 1 Step -1
                        Dim ds As String = d.ToString()
                        If dicdata.ContainsKey(ds) Then
                            For Each mid As String In dicdata(ds).Keys
                                Dim dt As Date = CDate(dicdata(ds)(mid).Time).AddHours(-2)
                                If dt < Now Then
                                    cday = ds
                                    found = True
                                    Exit For
                                End If
                            Next
                        End If
                        If found Then Exit For
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return cday

        End Function

        Public Function ApiGetMatchsData(Day As String) As String

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Richiedo la lista dei matchs per la giornata: " & Day & " dell'anno: " & appSett.Year)

            If appSett.DataFromDatabase Then

                Dim dicdata As New Dictionary(Of String, Dictionary(Of String, Match))
                Dim mtxdata As List(Of Match) = GetMatchsData(Day)

                For Each m As Match In mtxdata

                    Dim g As String = m.Giornata.ToString()
                    Dim mi As String = m.MatchId.ToString()

                    If dicdata.ContainsKey(g) = False Then dicdata.Add(g, New Dictionary(Of String, Match))
                    If dicdata(g).ContainsKey(mi) = False Then
                        dicdata(g).Add(mi, m)
                    End If
                Next

                Return WebData.Functions.SerializzaOggetto(dicdata, True)

            Else

                Dim j As String = IO.File.ReadAllText(WebData.MatchsData.GetMatchFileName(appSett))
                Dim dicdata As Dictionary(Of String, Dictionary(Of String, Match)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Match)))(j)

                If Day <> "-1" Then
                    Dim chiaviDaRimuovere = dicdata.Keys.Where(Function(k) k <> Day).ToList()
                    For Each chiave In chiaviDaRimuovere
                        dicdata.Remove(chiave)
                    Next
                End If

                Return WebData.Functions.SerializzaOggetto(dicdata, True)

            End If

        End Function

        Public Function ApiGetMatchsDataPlayers(startDay As String, endDay As String) As String

            If appSett.DataFromDatabase Then

                Dim dicdata As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer)))
                Dim mtxdata As List(Of MatchPlayer) = GetMatchsDataPlayers(startDay, endDay)

                For Each m As MatchPlayer In mtxdata

                    Dim g As String = m.Giornata.ToString()
                    Dim s As String = m.Squadra.ToString()
                    Dim n As String = m.Nome.ToString()

                    If dicdata.ContainsKey(g) = False Then dicdata.Add(g, New Dictionary(Of String, Dictionary(Of String, MatchPlayer)))
                    If dicdata(g).ContainsKey(s) = False Then dicdata(g).Add(s, New Dictionary(Of String, MatchPlayer))
                    If dicdata(g)(s).ContainsKey(n) = False Then
                        dicdata(g)(s).Add(n, m)
                    End If
                Next

                Return WebData.Functions.SerializzaOggetto(dicdata, True)

            Else

                Dim j As String = IO.File.ReadAllText(WebData.MatchsData.GetMatchPlayersFileName(appSett))
                Dim dicdata As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer))) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer))))(j)

                Dim chiaviDaRimuovere = dicdata.Keys.Where(Function(k) k < startDay OrElse k > endDay).ToList()
                For Each chiave In chiaviDaRimuovere
                    dicdata.Remove(chiave)
                Next

                Return WebData.Functions.SerializzaOggetto(dicdata, True)

            End If

        End Function

        Public Function ApiGetMatchDetails(Day As String, MatchId As String) As String

            WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Info, "Determino il dettaglio del match: " & Day & "/" & MatchId & " per l'anno: " & appSett.Year)

            Dim data As New MatchDetails
            Dim tmpdatap As List(Of MatchPlayer) = GetMatchsDataPlayers("SELECT * FROM tbtabellini WHERE gio =" & Day & " AND matchid =" & MatchId)
            Dim tmpdatae As List(Of MatchEvent) = GetMatchDataEvents(Day, MatchId)

            data.MatchData = GetMatchData(Day, MatchId)

            For i As Integer = 0 To tmpdatap.Count - 1
                If data.MatchPlayers.ContainsKey(tmpdatap(i).Squadra) = False Then data.MatchPlayers.Add(tmpdatap(i).Squadra, New List(Of MatchPlayer))
                data.MatchPlayers(tmpdatap(i).Squadra).Add(tmpdatap(i))
            Next

            For i As Integer = 0 To tmpdatae.Count - 1
                If data.MatchEvents.ContainsKey(tmpdatae(i).Minuto.ToString()) = False Then data.MatchEvents.Add(tmpdatae(i).Minuto.ToString(), New List(Of MatchEvent))
                data.MatchEvents(tmpdatae(i).Minuto.ToString()).Add(tmpdatae(i))
            Next

            Return WebData.Functions.SerializzaOggetto(data, True)

        End Function

        Public Sub UpdateMatchData(newdata As Dictionary(Of String, Dictionary(Of String, Match)))
            Try

                Dim mtxdata As List(Of Match) = GetMatchsData(newdata.Keys.ToList())
                Dim olddata As New Dictionary(Of String, Dictionary(Of String, Match))

                For Each m As Match In mtxdata

                    Dim g As String = m.Giornata.ToString()
                    Dim mi As String = m.MatchId.ToString()

                    If olddata.ContainsKey(g) = False Then olddata.Add(g, New Dictionary(Of String, Match))
                    If olddata(m.Giornata.ToString()).ContainsKey(mi) = False Then
                        olddata(g).Add(mi, m)
                    End If
                Next

                Dim sqlinsert As New List(Of String)
                Dim sqlupdate As New List(Of String)

                For Each g In newdata.Keys
                    For Each mi In newdata(g).Keys
                        Dim m As Match = newdata(g)(mi)
                        If olddata.ContainsKey(g) = False OrElse olddata(g).ContainsKey(mi) = False Then
                            sqlinsert.Add("INSERT INTO tbmatch (gio,idmatch,teama,teamb,timem,goala,goalb) values (" & g & "," & mi & ",'" & m.TeamA & "','" & m.TeamB & "','" & m.Time & "','" & m.GoalA & "','" & m.GoalB & "')")
                        Else
                            olddata(g)(mi).RecordId = -1
                            If WebData.Functions.GetCustomHashCode(olddata(g)(mi)) <> WebData.Functions.GetCustomHashCode(m) Then
                                sqlupdate.Add("UPDATE tbmatch SET teama='" & m.TeamA & "',teamb='" & m.TeamB & "',timem='" & m.Time & "',goala='" & m.GoalA & "',goalb='" & m.GoalB & "' WHERE gio=" & g & " AND idmatch=" & mi)
                            End If
                        End If
                    Next
                Next

                Dim sqldelete As New List(Of String)

                For Each g In olddata.Keys
                    For Each k In olddata(g).Keys
                        If olddata(g)(k).RecordId <> -1 Then
                            sqldelete.Add("DELETE FROM tbmatch WHERE id=" & olddata(g)(k).RecordId)
                        End If
                    Next
                Next

                Functions.ExecuteSql(appSett, sqlinsert)
                Functions.ExecuteSql(appSett, sqlupdate)
                Functions.ExecuteSql(appSett, sqldelete)

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Function GetMatchsData(day As String) As List(Of Match)
            Return GetMatchsData(New List(Of String) From {day})
        End Function

        Private Function GetMatchData(day As String, MatchId As String) As Match

            Dim m As New Match

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM tbmatch WHERE gio=" & day & " AND idmatch=" & MatchId)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        m.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        m.Giornata = Functions.ReadFieldIntegerData(row.Item("gio"), 1)
                        m.MatchId = Functions.ReadFieldIntegerData(row.Item("idmatch"), 0)
                        m.TeamA = Functions.ReadFieldStringData(row.Item("teama").ToString())
                        m.TeamB = Functions.ReadFieldStringData(row.Item("teamb").ToString())
                        m.Time = Convert.ToDateTime(row.Item("timem")).ToString("yyyy/MM/dd HH:mm:ss")
                        m.GoalA = Functions.ReadFieldStringData(row.Item("goala").ToString())
                        m.GoalB = Functions.ReadFieldStringData(row.Item("goalb").ToString())
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return m

        End Function

        Private Function GetMatchsData(days As List(Of String)) As List(Of Match)

            Dim mtxtdata As New List(Of Match)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM tbmatch " & If(days.Count > 0 AndAlso days.Contains("-1") = False, " WHERE gio IN ( " & WebData.Functions.ConvertListStringToString(days, ",") & ")", ""))

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim m As New Match
                        m.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        m.Giornata = Functions.ReadFieldIntegerData(row.Item("gio"), 1)
                        m.MatchId = Functions.ReadFieldIntegerData(row.Item("idmatch"), 0)
                        m.TeamA = Functions.ReadFieldStringData(row.Item("teama").ToString())
                        m.TeamB = Functions.ReadFieldStringData(row.Item("teamb").ToString())
                        m.Time = Convert.ToDateTime(row.Item("timem")).ToString("yyyy/MM/dd HH:mm:ss")
                        m.GoalA = Functions.ReadFieldStringData(row.Item("goala").ToString())
                        m.GoalB = Functions.ReadFieldStringData(row.Item("goalb").ToString())
                        mtxtdata.Add(m)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Sub UpdateMatchsDataPlayers(newdata As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer)))))
            Try

                Dim mtxtdata As List(Of MatchPlayer) = GetMatchsDataPlayers(newdata.Keys.ToList().Select(Function(x) CInt(x)).Min.ToString(), newdata.Keys.ToList().Select(Function(x) CInt(x)).Max.ToString())
                Dim olddata As New Dictionary(Of String, Dictionary(Of String, MatchPlayer))

                For Each data As MatchPlayer In mtxtdata
                    Dim g As String = data.Giornata.ToString()
                    Dim key As String = data.Nome & "/" & data.Squadra
                    If olddata.ContainsKey(g) = False Then olddata.Add(g, New Dictionary(Of String, MatchPlayer))
                    If olddata(g).ContainsKey(key) = False Then
                        olddata(g).Add(key, data)
                    End If
                Next

                Dim sqlinsert As New List(Of String)
                Dim sqlupdate As New List(Of String)

                For Each g In newdata.Keys
                    For Each idm In newdata(g).Keys
                        For Each t In newdata(g)(idm).Keys
                            For Each n In newdata(g)(idm)(t).Keys
                                Dim mp As MatchPlayer = newdata(g)(idm)(t)(n)
                                Dim key As String = n & "/" & t
                                If olddata.ContainsKey(g) = False OrElse olddata(g).ContainsKey(key) = False Then
                                    sqlinsert.Add("INSERT INTO tbtabellini (gio,matchid,ruolo,nome,squadra,mm,tit,sos,sub,amm,esp,ass,gf,gs,ag,rigp,rigs) values (" & g & "," & idm & ",'" & mp.Ruolo & "','" & mp.Nome & "','" & mp.Squadra & "'," & mp.Minuti & "," & mp.Titolare & "," & mp.Sostituito & "," & mp.Subentrato & "," & mp.Ammonizione & "," & mp.Espulsione & "," & mp.Assists & "," & mp.GoalFatti & "," & mp.GoalSubiti & "," & mp.AutoGoal & "," & mp.RigoriParati & "," & mp.RigoriSbagliati & ")")
                                Else
                                    olddata(g)(key).RecordId = -1
                                    If WebData.Functions.GetCustomHashCode(olddata(g)(key)) <> WebData.Functions.GetCustomHashCode(mp) Then
                                        sqlupdate.Add("UPDATE tbtabellini SET ruolo='" & mp.Ruolo & "',matchid=" & mp.MatchId & ",nome='" & mp.Nome & "',squadra='" & mp.Squadra & "',mm=" & mp.Minuti & ",tit=" & mp.Titolare & ",sos=" & mp.Sostituito & ",sub=" & mp.Subentrato & ",amm=" & mp.Ammonizione & ",esp=" & mp.Espulsione & ",ass=" & mp.Assists & ",gf=" & mp.GoalFatti & ",gs=" & mp.GoalSubiti & ",ag=" & mp.AutoGoal & ",rigp=" & mp.RigoriParati & ",rigs=" & mp.RigoriSbagliati & " WHERE gio=" & g & " AND nome='" & mp.Nome & "' AND squadra='" & mp.Squadra & "'")
                                    End If
                                End If
                            Next
                        Next
                    Next

                Next

                Dim sqldelete As New List(Of String)

                For Each g In olddata.Keys
                    For Each k In olddata(g).Keys
                        If olddata(g)(k).RecordId <> -1 Then
                            sqldelete.Add("DELETE FROM tbtabellini WHERE id=" & olddata(g)(k).RecordId)
                        End If
                    Next
                Next

                Functions.ExecuteSql(appSett, sqlinsert)
                Functions.ExecuteSql(appSett, sqlupdate)
                Functions.ExecuteSql(appSett, sqldelete)

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Public Sub UpdateMatchsDataEvents(newdata As Dictionary(Of String, Dictionary(Of String, SortedDictionary(Of Integer, List(Of MatchEvent)))))
            Try

                Dim mtxtdata As List(Of MatchEvent) = GetMatchDataEvents(newdata.Keys.ToList().Select(Function(x) CInt(x)).Min.ToString(), newdata.Keys.ToList().Select(Function(x) CInt(x)).Max.ToString())
                Dim olddata As New Dictionary(Of String, Dictionary(Of String, MatchEvent))

                For Each data As MatchEvent In mtxtdata
                    Dim g As String = data.Giornata.ToString()
                    Dim key As String = data.MatchId & "/" & data.Minuto & "/" & data.EventType & "/" & data.Nome & "/" & data.Squadra
                    If olddata.ContainsKey(g) = False Then olddata.Add(g, New Dictionary(Of String, MatchEvent))
                    If olddata(g).ContainsKey(key) = False Then
                        olddata(g).Add(key, data)
                    End If
                Next

                Dim sqlinsert As New List(Of String)
                Dim sqlupdate As New List(Of String)

                For Each g In newdata.Keys
                    For Each idm In newdata(g).Keys
                        For Each min In newdata(g)(idm).Keys
                            For Each mev As MatchEvent In newdata(g)(idm)(min)
                                Dim key As String = idm & "/" & mev.Minuto & "/" & mev.EventType & "/" & mev.Nome & "/" & mev.Squadra
                                If olddata.ContainsKey(g) = False OrElse olddata(g).ContainsKey(key) = False Then
                                    sqlinsert.Add("INSERT INTO tbcronaca (mday,matchid,minutes,eventtype,team,player) values (" & g & "," & idm & "," & min & ",'" & mev.EventType & "','" & mev.Squadra & "','" & mev.Nome & "')")
                                Else
                                    olddata(g)(key).RecordId = -1
                                End If
                            Next
                        Next
                    Next
                Next

                Dim sqldelete As New List(Of String)

                For Each g In olddata.Keys
                    For Each k In olddata(g).Keys
                        If olddata(g)(k).RecordId <> -1 Then
                            sqldelete.Add("DELETE FROM tbcronaca WHERE id=" & olddata(g)(k).RecordId)
                        End If
                    Next
                Next

                Functions.ExecuteSql(appSett, sqlinsert)
                Functions.ExecuteSql(appSett, sqlupdate)
                Functions.ExecuteSql(appSett, sqldelete)

            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try
        End Sub

        Private Function GetMatchsDataPlayers(startDay As String, endDay As String) As List(Of MatchPlayer)
            Return GetMatchsDataPlayers("SELECT * FROM tbtabellini WHERE gio >=" & startDay & " AND gio<=" & endDay)
        End Function

        Private Function GetMatchsDataPlayers(sqlstring As String) As List(Of MatchPlayer)

            Dim mtxtdata As New List(Of MatchPlayer)

            Try

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, sqlstring)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim m As New MatchPlayer
                        m.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        m.Giornata = Functions.ReadFieldIntegerData(row.Item("gio"), 1)
                        m.MatchId = Functions.ReadFieldIntegerData(row.Item("matchid"), 0)
                        m.Ruolo = Functions.ReadFieldStringData(row.Item("ruolo").ToString())
                        m.Nome = Functions.ReadFieldStringData(row.Item("nome").ToString())
                        m.Squadra = Functions.ReadFieldStringData(row.Item("squadra").ToString())
                        m.Minuti = Functions.ReadFieldIntegerData(row.Item("mm"), 0)
                        m.Titolare = Functions.ReadFieldIntegerData(row.Item("tit"), 0)
                        m.Sostituito = Functions.ReadFieldIntegerData(row.Item("sos"), 0)
                        m.Subentrato = Functions.ReadFieldIntegerData(row.Item("sub"), 0)
                        m.Ammonizione = Functions.ReadFieldIntegerData(row.Item("amm"), 0)
                        m.Espulsione = Functions.ReadFieldIntegerData(row.Item("esp"), 0)
                        m.Assists = Functions.ReadFieldIntegerData(row.Item("ass"), 0)
                        m.GoalFatti = Functions.ReadFieldIntegerData(row.Item("gf"), 0)
                        m.GoalSubiti = Functions.ReadFieldIntegerData(row.Item("gs"), 0)
                        m.AutoGoal = Functions.ReadFieldIntegerData(row.Item("ag"), 0)
                        m.RigoriParati = Functions.ReadFieldIntegerData(row.Item("rigp"), 0)
                        m.RigoriSbagliati = Functions.ReadFieldIntegerData(row.Item("rigs"), 0)
                        mtxtdata.Add(m)
                    Next

                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Private Function GetMatchDataEvents(Day As String, MatchId As String) As List(Of MatchEvent)

            Dim mtxtdata As New List(Of MatchEvent)

            Try

                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet(appSett, "SELECT * FROM tbcronaca WHERE mday=" & Day & " AND matchid=" & MatchId)

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim m As New MatchEvent
                        m.RecordId = Functions.ReadFieldIntegerData(row.Item("id"), 0)
                        m.Giornata = Functions.ReadFieldIntegerData(row.Item("mday"), 1)
                        m.MatchId = Functions.ReadFieldIntegerData(row.Item("matchid"), 0)
                        m.Minuto = Functions.ReadFieldIntegerData(row.Item("minutes"), 0)
                        m.EventType = Functions.ReadFieldStringData(row.Item("eventtype").ToString())
                        m.Squadra = Functions.ReadFieldStringData(row.Item("team").ToString())
                        m.Nome = Functions.ReadFieldStringData(row.Item("player").ToString())
                        mtxtdata.Add(m)
                    Next

                End If
            Catch ex As Exception
                WebData.Functions.WriteLog(appSett, WebData.Functions.eMessageType.Errors, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Class MatchDetails
            Public Property MatchData As New Match
            Public Property MatchPlayers As New Dictionary(Of String, List(Of MatchPlayer))
            Public Property MatchEvents As New Dictionary(Of String, List(Of MatchEvent))
        End Class

        Public Class Match
            Public Property RecordId As Integer = 1
            Public Property Giornata As Integer = 1
            Public Property MatchId As Integer = 0
            Public Property TeamA As String = ""
            Public Property TeamB As String = ""
            Public Property Time As String = Now.ToString("yyyy/MM/dd HH:mm:ss")
            Public Property GoalA As String = ""
            Public Property GoalB As String = ""

        End Class

        Public Class MatchPlayer
            Public Property RecordId As Integer = 1
            Public Property Giornata As Integer = 1
            Public Property MatchId As Integer = 0
            Public Property Ruolo As String = ""
            Public Property Nome As String = ""
            Public Property Squadra As String = ""
            Public Property Minuti As Integer = 90
            Public Property Titolare As Integer = 0
            Public Property Sostituito As Integer = 0
            Public Property Subentrato As Integer = 0
            Public Property Ammonizione As Integer = 0
            Public Property Espulsione As Integer = 0
            Public Property Assists As Integer = 0
            Public Property GoalFatti As Integer = 0
            Public Property GoalSubiti As Integer = 0
            Public Property AutoGoal As Integer = 0
            Public Property RigoriParati As Integer = 0
            Public Property RigoriSbagliati As Integer = 0
        End Class

        Public Class MatchEvent
            Public Property RecordId As Integer = 1
            Public Property Giornata As Integer = 1
            Public Property MatchId As Integer = 0
            Public Property Minuto As Integer = 0
            Public Property EventType As String = ""
            Public Property Squadra As String = ""
            Public Property Nome As String = ""

            Sub New()

            End Sub

            Sub New(EventType As String, Squadra As String, Nome As String)
                Me.EventType = EventType
                Me.Squadra = Squadra
                Me.Nome = Nome
            End Sub

        End Class

    End Class
End Namespace

