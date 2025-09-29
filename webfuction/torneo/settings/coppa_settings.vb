Namespace Torneo
    Partial Class LegaSettings

        Public Class CoppaSettings

            Public Property TipoSecondoTurno As String = "playoff"
            Public Property PlayOffGiorone1Team() As Integer() = New Integer() {-1, -1, -1}
            Public Property PlayOffGiorone2Team() As Integer() = New Integer() {-1, -1, -1}
            Public Property PlayOffGiorone1Match() As Integer() = New Integer() {24, 27, 29}
            Public Property PlayOffGiorone2Match() As Integer() = New Integer() {24, 27, 29}
            Public Property QuartiDiFinale1Team() As Integer() = New Integer() {-1, -1, -1}
            Public Property QuartiDiFinale2Team() As Integer() = New Integer() {-1, -1, -1}
            Public Property QuartiDiFinale3Team() As Integer() = New Integer() {-1, -1, -1}
            Public Property QuartiDiFinale4Team() As Integer() = New Integer() {-1, -1, -1}
            Public Property QuartiDiFinale1Match() As Integer() = New Integer() {24, 27}
            Public Property QuartiDiFinale2Match() As Integer() = New Integer() {24, 27}
            Public Property QuartiDiFinale3Match() As Integer() = New Integer() {24, 27}
            Public Property QuartiDiFinale4Match() As Integer() = New Integer() {24, 27}
            Public Property Semifinale1Team() As Integer() = New Integer() {-1, -1}
            Public Property Semifinale2Team() As Integer() = New Integer() {-1, -1}
            Public Property Semifinale1Match() As Integer() = New Integer() {32, 33}
            Public Property Semifinale2Match() As Integer() = New Integer() {32, 33}
            Public Property FinaleTeam() As Integer() = New Integer() {-1, -1}
            Public Property FinaleMatch() As Integer() = New Integer() {34, 35}

        End Class

    End Class
End Namespace
