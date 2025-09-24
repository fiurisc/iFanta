Partial Class AppSettings
    Public Class MainWindowSettings

        Private _favorites As New List(Of String)
        Private _winsize As Integer = 0
        Private _winview As Integer = 0
        Private _mnufontsize As Integer = 11
        Private _hotkeys As New Dictionary(Of Integer, HotKeyItem)

        Public Property WindowsView() As Integer
            Get
                Return _winview
            End Get
            Set(ByVal value As Integer)
                _winview = value
            End Set
        End Property

        Public Property WindowsSize() As Integer
            Get
                Return _winsize
            End Get
            Set(ByVal value As Integer)
                _winsize = value
            End Set
        End Property

        Public Property MenuFontSize() As Integer
            Get
                Return _mnufontsize
            End Get
            Set(ByVal value As Integer)
                _mnufontsize = value
            End Set
        End Property

        Public Property Favorites() As List(Of String)
            Get
                Return _favorites
            End Get
            Set(ByVal value As List(Of String))
                _favorites = value
            End Set
        End Property

        Public Property HotKeys() As Dictionary(Of Integer, HotKeyItem)
            Get
                Return _hotkeys
            End Get
            Set(ByVal value As Dictionary(Of Integer, HotKeyItem))
                _hotkeys = value
            End Set
        End Property

    End Class
End Class