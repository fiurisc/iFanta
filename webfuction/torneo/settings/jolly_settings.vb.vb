Namespace Torneo
    Partial Class TorneoSettings
        Public Class JollySettings

            Private _enajollyplayer As Boolean = True
            Private _enajollyportire As Boolean = False
            Private _enajollydifensore As Boolean = True
            Private _enajollycentrocampista As Boolean = True
            Private _enajollyattaccante As Boolean = False
            Private _maximumnbrjollyplayable As Integer = 4
            Private _maximumnbrjollyplayableforday As Integer = 1

            Public Property EnableJollyPlayer() As Boolean
                Get
                    Return _enajollyplayer
                End Get
                Set(ByVal value As Boolean)
                    _enajollyplayer = value
                End Set
            End Property

            Public Property EnableJollyPlayerGoalkeeper() As Boolean
                Get
                    Return _enajollyportire
                End Get
                Set(ByVal value As Boolean)
                    _enajollyportire = value
                End Set
            End Property

            Public Property EnableJollyPlayerDefender() As Boolean
                Get
                    Return _enajollydifensore
                End Get
                Set(ByVal value As Boolean)
                    _enajollydifensore = value
                End Set
            End Property

            Public Property EnableJollyPlayerMidfielder() As Boolean
                Get
                    Return _enajollycentrocampista
                End Get
                Set(ByVal value As Boolean)
                    _enajollycentrocampista = value
                End Set
            End Property

            Public Property EnableJollyPlayerForward() As Boolean
                Get
                    Return _enajollyattaccante
                End Get
                Set(ByVal value As Boolean)
                    _enajollyattaccante = value
                End Set
            End Property

            Public Property MaximumNumberJollyPlayable() As Integer
                Get
                    Return _maximumnbrjollyplayable
                End Get
                Set(ByVal value As Integer)
                    _maximumnbrjollyplayable = value
                End Set
            End Property

            Public Property MaximumNumberJollyPlayableForDay() As Integer
                Get
                    Return _maximumnbrjollyplayableforday
                End Get
                Set(ByVal value As Integer)
                    _maximumnbrjollyplayableforday = value
                End Set
            End Property

        End Class

    End Class
End Namespace
