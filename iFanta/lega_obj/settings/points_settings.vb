Partial Class LegaObject
    Partial Class LegaSettings
        Public Class PointsSettings

            Private _sitereferenceforpoints As New List(Of String)
            Private _sitereferenceforbonus As String = ""
            Private _ammpt As Integer = -5
            Private _esppt As Integer = -10
            Private _goals As Integer = -10
            Private _goalf As New Dictionary(Of String, Integer)
            Private _rigsb As New Dictionary(Of String, Integer)
            Private _autg As New Dictionary(Of String, Integer)
            Private _ass As New Dictionary(Of String, Integer)

            Sub New()
                _goalf.Add("P", 30)
                _goalf.Add("D", 30)
                _goalf.Add("C", 30)
                _goalf.Add("A", 30)
                _rigsb.Add("P", -30)
                _rigsb.Add("D", -30)
                _rigsb.Add("C", -30)
                _rigsb.Add("A", -30)
                _autg.Add("P", -10)
                _autg.Add("D", -20)
                _autg.Add("C", -20)
                _autg.Add("A", -20)
                _ass.Add("P", 10)
                _ass.Add("D", 10)
                _ass.Add("C", 10)
                _ass.Add("A", 10)
            End Sub

            Public Property SiteReferenceForPoints As List(Of String)
                Get
                    Return _sitereferenceforpoints
                End Get
                Set(value As List(Of String))
                    _sitereferenceforpoints = value
                End Set
            End Property

            Public Property SiteReferenceForBonus As String
                Get
                    Return _sitereferenceforbonus
                End Get
                Set(value As String)
                    _sitereferenceforbonus = value
                End Set
            End Property

            Public Property Admonition As Integer
                Get
                    Return _ammpt
                End Get
                Set(value As Integer)
                    _ammpt = value
                End Set
            End Property

            Public Property Expulsion As Integer
                Get
                    Return _esppt
                End Get
                Set(value As Integer)
                    _esppt = value
                End Set
            End Property

            Public Property Assist As Dictionary(Of String, Integer)
                Get
                    Return _ass
                End Get
                Set(value As Dictionary(Of String, Integer))
                    _ass = value
                End Set
            End Property

            Public Property GoalScored As Dictionary(Of String, Integer)
                Get
                    Return _goalf
                End Get
                Set(value As Dictionary(Of String, Integer))
                    _goalf = value
                End Set
            End Property

            Public Property GoalConceded As Integer
                Get
                    Return _goals
                End Get
                Set(value As Integer)
                    _goals = value
                End Set
            End Property

            Public Property OwnGoal As Dictionary(Of String, Integer)
                Get
                    Return _autg
                End Get
                Set(value As Dictionary(Of String, Integer))
                    _autg = value
                End Set
            End Property

            Public Property MissedPenalty As Dictionary(Of String, Integer)
                Get
                    Return _rigsb
                End Get
                Set(value As Dictionary(Of String, Integer))
                    _rigsb = value
                End Set
            End Property

        End Class

    End Class
End Class
