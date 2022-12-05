<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PointSelector.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.DistributionPointSelector" %>

<h2><%=RM.GetString(RS.Order.Delivery.Title)%></h2>


<fieldset>
	<ol>
		<li>
<div class="DistributionPoint" style="zoom:1;overflow:hidden;">
	<% if (SelectedPoint.MapType == Photoprint.Core.Models.MapType.GMap) { %>
		<a class="map" href="https://www.google.com/maps/place/<%=SelectedPoint.Latitude %>,<%=SelectedPoint.Longitude %>" target="_blank">
		    <img src="https://maps.google.com/maps/api/staticmap?center=<%=SelectedPoint.Latitude %>,<%=SelectedPoint.Longitude %>&zoom=14&size=<%=SelectedPoint.GMapStaticSize %>&maptype=roadmap&mobile=true&markers=size:small|color:red|<%=SelectedPoint.Latitude %>,<%=SelectedPoint.Longitude %>&key=<%= GMapKey %>&sensor=false" alt="map" />
		</a>
	<%} else if (SelectedPoint.MapType == Photoprint.Core.Models.MapType.Custom) {%>
		<div class="custom-map"><%= SelectedPoint.CustomMapCode %></div>
	<%} else if (System.IO.File.Exists(SelectedPoint.ThumbnailImagePath)) {%>
		<asp:HyperLink CssClass="map" ID="refFullMap" runat="server"><img alt="" src="" id="imgMap" runat="server" /></asp:HyperLink>
	<% } else {%>
    <div class="nomap">
		  <p><%=RM.GetString(RS.General.NoMap)%></p>
		  <div class="topLayer"><img alt="" src="~/content/style/images/nomap.png" runat="server"/></div>
	  </div>                
  <%}%>
    <div class="info">
        <asp:Literal ID="litPointTitle" runat="server" />
        <p>
            <b><asp:Literal ID="litTitle" runat="server" /></b><br />
			<em><%=RM.GetString(RS.Order.Delivery.OfficeHours)%></em> <asp:Literal ID="litOfficeHours" runat="server" /><br />
			<em><%=RM.GetString(RS.Order.Delivery.AddressLine1)%></em> <asp:Literal ID="litAddress" runat="server" />
			<em><%=RM.GetString(RS.Order.Delivery.Phone)%></em> <asp:Literal ID="litPhone" runat="server" /><br />
		</p>
        <p><asp:Literal ID="litDescription" runat="server" /></p>   
    </div>
</div>
		</li>
    </ol>
</fieldset>