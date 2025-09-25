<%@ Page Language="VB" ValidateRequest="false" %>
<script runat="server">
		
    Private Sub AggiornaStatoTornei(sender As Object, e As EventArgs)
		
		Dim anniact As String=Request.Form("anno")
		Dim y As New Generic.List(Of String)
		
		if anniact<>"" then
			y.AddRange(anniact.Split(CChar(",")))
		end if
		
		'Messaggio.Text=y.Count
		
		Dim d() as String=IO.Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath("/public/ifanta/update/tornei"))
	
		For i As Integer=0 To d.Length-1
		
			Dim torneo As String=IO.Path.GetFileName(d(i))
			Dim line() As String=IO.File.ReadAllLines(d(i) & "/settings.txt")
			Dim str As New System.Text.StringBuilder
			Dim act As Boolean=false
			
			If y.Contains(torneo) Then act=True
			
			For k As Integer=0 to line.length-1
				if line(k).Contains("Active") then
						if act Then
							str.AppendLine("Active = 'True'")
							else
							str.AppendLine("Active = 'False'")
						End If
					Else
						str.AppendLine(line(k))
				end if
			Next
			
			IO.File.WriteAllText(d(i) & "/settings.txt",str.ToString)
			
		Next
	
		'Messaggio.Text="ActivaRequest.Form("anno")
    End Sub
</script>

<html>
    <head>
        <title>I Web Form - Esempio # 3</title>
    </head>
<body>

<div style='Color:#F00;font-size:15px;font-family:arial;font-weight:bold;'>Attiva/disattiva tornei</div>
<div style='border:1px solid #CCC;padding:5px;width:200px;'>
<form id="Modulo" runat="server">
<%

	Dim d() as String=IO.Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath("/public/ifanta/update/tornei"))
	
	For i As Integer=0 To d.Length-1
	
		Dim torneo As String=IO.Path.GetFileName(d(i))
		Dim line() As String=IO.File.ReadAllLines(d(i) & "/settings.txt")
		Dim act As Boolean=false
		
		For k As Integer=0 to line.length-1
			if line(k).Contains("Active = 'True'") then
				act=true
			end if
		Next
		If act Then
			response.write("<input id='" & torneo & "' type='checkbox' name='anno' value='" & torneo & "' checked><label for='A'>" & torneo & "</label></span>")
			Else
			response.write("<input id='" & torneo & "' type='checkbox' name='anno' value='" & torneo & "'><label for='A'>" & torneo & "</label></span>")
		End If
		response.write("<br>")
	Next
%>
    <br>
    <asp:Button runat="server" Text="Aggiorna stato tornei" OnClick="AggiornaStatoTornei" />
    <br><br>
	
</form>
</div>
</body>
</html>