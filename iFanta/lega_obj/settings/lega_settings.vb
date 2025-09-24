Partial Class LegaObject

    Public Class LegaSettings

        Enum eSubstitutionType As Integer
            Normal = 0
            NormalAndChangeModule = 1
            ChangeModule = 2
        End Enum

        Public Property Active As Boolean = False
        Public Property Nome() As String = "TORNEO"
        Public Property Year As String = "2001"
        Public Property MailAdmin As String = ""
        Public Property NumberOfTeams() As Integer = 10
        Public Property NumberOfDays() As Integer = 38
        Public Property Points As PointsSettings = New PointsSettings
        Public Property ConteggiaGoalFattiPerVittoria() As Boolean = True
        Public Property ConteggiaGoalSubitiPerVittoria() As Boolean = True
        Public Property NumberOfReserve() As Integer = 7
        Public Property ForcedGoalkeeperAsFirstReserve() As Boolean = True
        Public Property NumberOfSubstitution() As Integer = 3
        Public Property SubstitutionType() As eSubstitutionType = eSubstitutionType.Normal
        Public Property Bonus() As BonusSettings = New BonusSettings
        Public Property Coppa() As CoppaSettings = New CoppaSettings
        Public Property Jolly() As JollySettings = New JollySettings
        Public Property Admin() As Boolean = False
        Public Property EnableTraceReconfirmations As Boolean = False

    End Class

End Class
