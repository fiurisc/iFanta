<%@ Import Namespace="System.net" %>
<%@ Import Namespace="System.io" %>
<%@ Import Namespace="System.xml" %>
<%@ Import Namespace="System.data" %>
<SCRIPT LANGUAGE="VB" RUNAT="SERVER">
    
    Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        
		Dim show As String = System.Web.HttpContext.Current.Request.QueryString("data")
		Dim dirs as String=System.Web.HttpContext.Current.Server.MapPath("/public/ifanta/")
		Dim wData As New WebData
		
		if show="show" then
			Response.write(wData.GetPlayersQuote(dirs,True))
		else
			wData.GetPlayersQuote(dirs,False)
			Response.Write("Get players quote compleated!!")
		end if
		
    End Sub

</SCRIPT>