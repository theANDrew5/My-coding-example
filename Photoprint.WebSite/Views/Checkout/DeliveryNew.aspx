<%@ Page Language="C#" MasterPageFile="~/Templates/Placeholder.Master" EnableEventValidation="false" AutoEventWireup="false" EnableViewState="false" CodeBehind="DeliveryNew.aspx.cs" Inherits="Photoprint.WebSite.DeliveryNew" %>
<%@ Import Namespace="Photoprint.WebSite.Modules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMainPanel" runat="server">
	<%= FileHelper.JS.RenderOnce(FileHelper.JS.DeliveryManagerUrl, Context) %>
	
	<pr:Skinner ID="Skinner" runat="server" Control="OrderHeader" View="Checkout" />

	<div id="pxpDeliveryContainer"></div>
    <script>
        pxpFrontend.delivery.DeliveryInit.render(document.getElementById('pxpDeliveryContainer'))
    </script>
</asp:Content>