Partial Public Class wData
    Public Class ThreadItem

        Public Sub New()

        End Sub

        Public Sub New(ByVal Thread As System.Threading.Thread)
            Me.Thread = Thread
        End Sub

        Public Sub New(ByVal Site As String, ByVal StartUp As Boolean)
            Me.Site = Site
            Me.StartUp = StartUp
        End Sub

        Public Sub New(ByVal Type As String, ByVal Site As String, ByVal CodeSite As String, ByVal StartUp As Boolean)
            Me.Type = Type
            Me.Site = Site
            Me.CodeSite = CodeSite
            Me.StartUp = StartUp
        End Sub

        Public Property Type() As String = ""
        Public Property Site() As String = ""
        Public Property CodeSite() As String = ""
        Public Property StartTime() As Date = Date.Now
        Public Property StartUp() As Boolean = False
        Public Property FileName() As String = ""
        Public Property Thread() As System.Threading.Thread
        Public Property Tag() As Object

    End Class
End Class