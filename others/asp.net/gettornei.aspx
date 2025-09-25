<%@ Import Namespace="System.net" %>
<%@ Import Namespace="System.io" %>
<%@ Import Namespace="System.xml" %>
<%@ Import Namespace="System.data" %>
<SCRIPT LANGUAGE="VB" RUNAT="SERVER">
    
    Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim d() As String = IO.Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath("/public/ifanta/update/tornei"))
        For i As Integer = 0 To d.Length - 1
            Response.Write(IO.Path.GetFileName(d(i)) & System.Environment.NewLine)
        Next
    End Sub
    
</SCRIPT>