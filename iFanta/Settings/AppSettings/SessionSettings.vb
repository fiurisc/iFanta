Partial Class AppSettings
    Public Class SessionSettings

        Private _lastlegaselected As String = ""
        Private _tabselect As Integer = 0
        Private _subtabselect As Integer = 0
        Private _impexpdefaultdirectory As Boolean = True
        Private _impexpdirectory As String = ""

        Public Property LastLegaSelected() As String
            Get
                Return _lastlegaselected
            End Get
            Set(ByVal value As String)
                _lastlegaselected = value
            End Set
        End Property

        Public Property TabSelect() As Integer
            Get
                Return _tabselect
            End Get
            Set(ByVal value As Integer)
                _tabselect = value
            End Set
        End Property

        Public Property SubTabSelect() As Integer
            Get
                Return _subtabselect
            End Get
            Set(ByVal value As Integer)
                _subtabselect = value
            End Set
        End Property

        Public Property UseDefaultDirectoryForImportAndExport() As Boolean
            Get
                Return _impexpdefaultdirectory
            End Get
            Set(ByVal value As Boolean)
                _impexpdefaultdirectory = value
            End Set
        End Property

        Public Property LastImportAndExportDirectory() As String
            Get
                Return _impexpdirectory
            End Get
            Set(ByVal value As String)
                _impexpdirectory = value
            End Set
        End Property

    End Class
End Class