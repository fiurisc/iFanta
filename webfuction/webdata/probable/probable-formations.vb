Namespace WebData
    Public Class ProbableFormations

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

        Shared Function WriteData(wp As Dictionary(Of String, wPlayer), filed As String) As String

            Dim strdata As New System.Text.StringBuilder

            Try

                For Each pkey As String In wp.Keys
                    strdata.AppendLine(pkey & "|" & wp(pkey).Name & "|" & wp(pkey).Team & "|" & wp(pkey).Site & "|" & wp(pkey).State & "|" & wp(pkey).Percentage & "|" & wp(pkey).Info)
                Next

                IO.File.WriteAllText(filed, strdata.ToString, System.Text.Encoding.UTF8)

            Catch ex As Exception
                WebData.Functions.WriteError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message)
            End Try

            Return strdata.ToString

        End Function

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

            Private _name As String = ""
            Private _team As String = ""
            Private _site As String = ""
            Private _state As String = "sconosciuto"
            Private _info As String = ""
            Private _perc As Integer = 0

            Sub New()

            End Sub

            Sub New(ByVal Name As String, ByVal Team As String, Site As String, ByVal State As String, ByVal Info As String, Percentage As Integer)
                _name = Name
                _team = Team
                _site = Site
                _state = State
                _info = Info
                _perc = Percentage
            End Sub

            Public Property Name() As String
                Get
                    Return _name
                End Get
                Set(ByVal value As String)
                    _name = value
                End Set
            End Property

            Public Property Team() As String
                Get
                    Return _team
                End Get
                Set(ByVal value As String)
                    _team = value
                End Set
            End Property

            Public Property Site() As String
                Get
                    Return _site
                End Get
                Set(ByVal value As String)
                    _site = value
                End Set
            End Property

            Public Property State() As String
                Get
                    Return _state
                End Get
                Set(ByVal value As String)
                    _state = value
                End Set
            End Property

            Public Property Info As String
                Get
                    Return _info
                End Get
                Set(ByVal value As String)
                    _info = value
                End Set
            End Property

            Public Property Percentage As Integer
                Get
                    Return _perc
                End Get
                Set(value As Integer)
                    _perc = value
                End Set
            End Property
        End Class
    End Class
End Namespace