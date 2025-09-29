Namespace Torneo
    Partial Class LegaSettings
        Public Class BonusSettings

            Public Property EnableBonusDefense() As Boolean = True
            Public Property BonudDefenseSource() As String = "points"
            Public Property BonusDefenseOverEqual() As Integer = 6
            Public Property BonusDefense() As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
            Public Property EnableCenterField() As Boolean = False
            Public Property BonudCenterFieldSource() As String = "points"
            Public Property BonusCenterFieldOverEqual() As Integer = 6
            Public Property BonusCenterField() As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
            Public Property EnableBonusAttack() As Boolean = False
            Public Property BonudAttackSource() As String = "points"
            Public Property BonusAttackOverEqual() As Integer = 6
            Public Property BonusAttack() As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)

        End Class

    End Class
End Namespace
