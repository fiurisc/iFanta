Namespace WebData
    Public Class ProbableFormations

        Public Shared dirTemp As String = Functions.DataPath & "temp\"
        Public Shared dirData As String = Functions.DataPath & "data\pforma\"

        Shared Function GetDataFileName(site As String) As String
            Return dirData & site.ToLower() & ".json"
        End Function

        Shared Sub AddInfo(Name As String, Team As String, Site As String, State As String, Info As String, Percentage As Integer, wp As Dictionary(Of String, wPlayer))
            If wp.ContainsKey(Name & "/" & Team) = False Then
                If State = "Ballottaggio" Then State = "Panchina"
                wp.Add(Name & "/" & Team, New wPlayer(Name, Team, Site, State, Info, Percentage))
            Else
                If wp(Name & "/" & Team).Info <> "" Then Info = "," & Info
                If State = "Ballottaggio" Then State = wp(Name & "/" & Team).State
                wp(Name & "/" & Team).Info = wp(Name & "/" & Team).Info & Info
                wp(Name & "/" & Team).Percentage = Percentage
            End If
        End Sub

        Shared Function WriteData(day As Integer, wp As Dictionary(Of String, wPlayer), filed As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                For Each pkey As String In wp.Keys
                    strdata.AppendLine(day & "/" & pkey & "|" & wp(pkey).Name & "|" & wp(pkey).Team & "|" & wp(pkey).Site & "|" & wp(pkey).State & "|" & wp(pkey).Percentage & "|" & wp(pkey).Info)
                Next

                IO.File.WriteAllText(filed, strdata.ToString, System.Text.Encoding.UTF8)

            Catch ex As Exception
                Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString

        End Function

        Public Class wPlayer

            Sub New()

            End Sub

            Sub New(ByVal Name As String, ByVal Team As String, Site As String, ByVal State As String, ByVal Info As String, Percentage As Integer)
                _Name = Name
                _Team = Team
                _Site = Site
                _State = State
                _Info = Info
                _Percentage = Percentage
            End Sub

            Public Property Name() As String = ""
            Public Property Team() As String = ""
            Public Property Site() As String = ""
            Public Property State() As String = "sconosciuto"
            Public Property Info As String = ""
            Public Property Percentage As Integer = 0

        End Class
    End Class
End Namespace