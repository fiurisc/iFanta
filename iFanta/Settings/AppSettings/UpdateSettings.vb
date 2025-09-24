Partial Class AppSettings
    Public Class UpdateSettings

        Private _enabledupdate As Boolean = True
        Private _forcecheckupdatestartup As Boolean = False

        Public Property EnableUpdate() As Boolean
            Get
                Return _enabledupdate
            End Get
            Set(ByVal value As Boolean)
                _enabledupdate = value
            End Set
        End Property

        Public Property ForceCheckUpdateStartup() As Boolean
            Get
                Return _forcecheckupdatestartup
            End Get
            Set(ByVal value As Boolean)
                _forcecheckupdatestartup = value
            End Set
        End Property

    End Class
End Class