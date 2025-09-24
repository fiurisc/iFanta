Partial Class AppSettings
    Public Class BackupSettings

        Private _enable As Boolean = True
        Private _enablecompression As Boolean = True
        Private _exeonlyaftermodify As Boolean = True
        Private _history As Integer = 15

        Public Property Enable() As Boolean
            Get
                Return _enable
            End Get
            Set(ByVal value As Boolean)
                _enable = value
            End Set
        End Property

        Public Property ExecuteBackupOnlyAfterModify() As Boolean
            Get
                Return _exeonlyaftermodify
            End Get
            Set(ByVal value As Boolean)
                _exeonlyaftermodify = value
            End Set
        End Property

        Public Property EnableCompression() As Boolean
            Get
                Return _enablecompression
            End Get
            Set(ByVal value As Boolean)
                _enablecompression = value
            End Set
        End Property

        Public Property History() As Integer
            Get
                Return _history
            End Get
            Set(ByVal value As Integer)
                _history = value
            End Set
        End Property

    End Class
End Class