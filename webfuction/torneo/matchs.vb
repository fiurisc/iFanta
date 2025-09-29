Namespace Torneo
    Public Class MatchsData

        Public Shared Function GetMatchCurrentDayJson() As String

            Dim cday As String = "38"

            Try
                If Torneo.General.dataFromDatabase Then
                    Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM current_championship_day")
                    If ds.Tables.Count > 0 Then
                        cday = ds.Tables(0).Rows(0).Item("gio").ToString()
                    End If
                Else

                    Dim j As String = IO.File.ReadAllText(WebData.MatchsData.GetMatchFileName())
                    Dim dicdata As Dictionary(Of String, Dictionary(Of String, Match)) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Match)))(GetMatchDataJson("-1"))

                    For d As Integer = 38 To 1 Step -1
                        Dim ds As String = d.ToString()
                        If dicdata.ContainsKey(ds) Then
                            For Each mid As String In dicdata(ds).Keys
                                Dim dt As Date = CDate(dicdata(ds)(mid).Time).AddHours(-2)
                                If dt < Now Then
                                    cday = ds
                                End If
                            Next
                        End If
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return "{" & cday & "}"

        End Function

        Public Shared Function GetMatchDataJson(Day As String) As String

            If Torneo.General.dataFromDatabase Then

                Dim dicdata As New Dictionary(Of String, Dictionary(Of String, Match))
                Dim mtxdata As List(Of Match) = GetMatchData(Day)

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

                Dim j As String = IO.File.ReadAllText(WebData.MatchsData.GetMatchFileName())
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

        Public Shared Function GetMatchDataPlayersJson(Day As String) As String

            General.dataFromDatabase = False

            If General.dataFromDatabase Then

                Dim dicdata As New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer)))
                Dim mtxdata As List(Of MatchPlayer) = GetMatchDataPlayers(Day)

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

                Dim j As String = IO.File.ReadAllText(WebData.MatchsData.GetMatchPlayersFileName())
                Dim dicdata As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer))) = WebData.Functions.DeserializeJson(Of Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer))))(j)

                If Day <> "-1" Then
                    Dim chiaviDaRimuovere = dicdata.Keys.Where(Function(k) k <> Day).ToList()
                    For Each chiave In chiaviDaRimuovere
                        dicdata.Remove(chiave)
                    Next
                End If

                Return WebData.Functions.SerializzaOggetto(dicdata, True)

            End If

        End Function

        Public Shared Sub UpdateMatchData(newdata As Dictionary(Of String, Dictionary(Of String, Match)))
            Try

                Dim mtxdata As List(Of Match) = GetMatchData(newdata.Keys.ToList())
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
                        ElseIf WebData.Functions.GetCustomHashCode(olddata(g)(mi)) <> WebData.Functions.GetCustomHashCode(m) Then
                            sqlupdate.Add("UPDATE tbmatch SET teama='" & m.TeamA & "',teamb='" & m.TeamB & "',timem='" & m.Time & "',goala='" & m.GoalA & "',goalb='" & m.GoalB & "' WHERE gio=" & g & " AND idmatch=" & mi)
                        End If
                    Next
                Next

                Functions.ExecuteSql(sqlinsert)
                Functions.ExecuteSql(sqlupdate)

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Private Shared Function GetMatchData(day As String) As List(Of Match)
            Return GetMatchData(New List(Of String) From {day})
        End Function

        Private Shared Function GetMatchData(days As List(Of String)) As List(Of Match)

            Dim mtxtdata As New List(Of Match)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM tbmatch " & If(days.Count > 0 AndAlso days.Contains("-1") = False, " WHERE gio IN ( " & WebData.Functions.ConvertListStringToString(days, ",") & ")", ""))

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim m As New Match
                        m.Giornata = If(row.Item("gio") IsNot DBNull.Value, Convert.ToInt32(row.Item("gio")), 1)
                        m.MatchId = If(row.Item("idmatch") IsNot DBNull.Value, Convert.ToInt32(row.Item("idmatch")), 0)
                        m.TeamA = row.Item("teama").ToString()
                        m.TeamB = row.Item("teamb").ToString()
                        m.Time = Convert.ToDateTime(row.Item("timem")).ToString("yyyy/MM/dd HH:mm:ss")
                        m.GoalA = row.Item("goala").ToString()
                        m.GoalB = row.Item("goalb").ToString()
                        mtxtdata.Add(m)
                    Next
                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Shared Sub UpdateMatchDataPlayers(newdata As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MatchPlayer))))
            Try

                Dim mtxtdata As List(Of MatchPlayer) = GetMatchDataPlayers(newdata.Keys.ToList())
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
                    For Each t In newdata(g).Keys
                        For Each n In newdata(g)(t).Keys
                            Dim mp As MatchPlayer = newdata(g)(t)(n)
                            Dim key As String = n & "/" & t
                            If olddata.ContainsKey(g) = False OrElse olddata(g).ContainsKey(key) = False Then
                                sqlinsert.Add("INSERT INTO tbtabellini (gio,nome,squadra,mm,tit,sos,sub,amm,esp,ass,gf,gs,ag,rigp,rigs) values (" & g & ",'" & mp.Nome & "','" & mp.Squadra & "'," & mp.Minuti & "," & mp.Titolare & "," & mp.Sostituito & "," & mp.Subentrato & "," & mp.Ammonizione & "," & mp.Espulsione & "," & mp.Assists & "," & mp.GoalFatti & "," & mp.GoalSubiti & "," & mp.AutoGoal & "," & mp.RigoriParati & "," & mp.RigoriSbagliati & ")")
                            ElseIf WebData.Functions.GetCustomHashCode(olddata(g)(key)) <> WebData.Functions.GetCustomHashCode(mp) Then
                                sqlupdate.Add("UPDATE tbtabellini SET nome='" & mp.Nome & "',squadra='" & mp.Squadra & "',mm=" & mp.Minuti & ",tit=" & mp.Titolare & ",sos=" & mp.Sostituito & ",sub=" & mp.Subentrato & ",amm=" & mp.Ammonizione & ",esp=" & mp.Espulsione & ",ass=" & mp.Assists & ",gf=" & mp.GoalFatti & ",gs=" & mp.GoalSubiti & ",ag=" & mp.AutoGoal & ",rigp=" & mp.RigoriParati & ",rigs=" & mp.RigoriSbagliati & " WHERE gio=" & g & " AND nome='" & mp.Nome & "' AND squadra='" & mp.Squadra & "'")
                            End If
                        Next
                    Next
                Next

                Functions.ExecuteSql(sqlinsert)
                Functions.ExecuteSql(sqlupdate)

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try
        End Sub

        Private Shared Function GetMatchDataPlayers(day As String) As List(Of MatchPlayer)
            Return GetMatchDataPlayers(New List(Of String) From {day})
        End Function

        Private Shared Function GetMatchDataPlayers(days As List(Of String)) As List(Of MatchPlayer)

            Dim mtxtdata As New List(Of MatchPlayer)

            Try
                Dim ds As System.Data.DataSet = Functions.ExecuteSqlReturnDataSet("SELECT * FROM tbtabellini" & If(days.Count > 0 AndAlso days.Contains("-1") = False, " WHERE gio IN ( " & WebData.Functions.ConvertListStringToString(days, ",") & ")", ""))

                If ds.Tables.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim row As DataRow = ds.Tables(0).Rows(i)
                        Dim m As New MatchPlayer
                        m.Giornata = If(row.Item("gio") IsNot DBNull.Value, Convert.ToInt32(row.Item("gio")), 1)
                        m.Nome = row.Item("nome").ToString()
                        m.Squadra = row.Item("squadra").ToString()
                        m.Minuti = If(row.Item("mm") IsNot DBNull.Value, Convert.ToInt32(row.Item("mm")), 0)
                        m.Titolare = If(row.Item("tit") IsNot DBNull.Value, Convert.ToInt32(row.Item("tit")), 0)
                        m.Sostituito = If(row.Item("sos") IsNot DBNull.Value, Convert.ToInt32(row.Item("sos")), 0)
                        m.Subentrato = If(row.Item("sub") IsNot DBNull.Value, Convert.ToInt32(row.Item("sub")), 0)
                        m.Ammonizione = If(row.Item("amm") IsNot DBNull.Value, Convert.ToInt32(row.Item("amm")), 0)
                        m.Espulsione = If(row.Item("esp") IsNot DBNull.Value, Convert.ToInt32(row.Item("esp")), 0)
                        m.Assists = If(row.Item("ass") IsNot DBNull.Value, Convert.ToInt32(row.Item("ass")), 0)
                        m.GoalFatti = If(row.Item("gf") IsNot DBNull.Value, Convert.ToInt32(row.Item("gf")), 0)
                        m.GoalSubiti = If(row.Item("gs") IsNot DBNull.Value, Convert.ToInt32(row.Item("gs")), 0)
                        m.AutoGoal = If(row.Item("ag") IsNot DBNull.Value, Convert.ToInt32(row.Item("ag")), 0)
                        m.RigoriParati = If(row.Item("rigp") IsNot DBNull.Value, Convert.ToInt32(row.Item("rigp")), 0)
                        m.RigoriSbagliati = If(row.Item("rigs") IsNot DBNull.Value, Convert.ToInt32(row.Item("rigs")), 0)
                        mtxtdata.Add(m)
                    Next

                End If
            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return mtxtdata

        End Function

        Public Class Match

            Public Property Giornata As Integer = 1
            Public Property MatchId As Integer = 0
            Public Property TeamA As String = ""
            Public Property TeamB As String = ""
            Public Property Time As String = Now.ToString("yyyy/MM/dd HH:mm:ss")
            Public Property GoalA As String = ""
            Public Property GoalB As String = ""

        End Class

        Public Class MatchPlayer
            Public Property Giornata As Integer = 1
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
            Public Property eventType As String = ""
            Public Property minute As String = ""
            Public Property player As String = ""
        End Class

    End Class
End Namespace

