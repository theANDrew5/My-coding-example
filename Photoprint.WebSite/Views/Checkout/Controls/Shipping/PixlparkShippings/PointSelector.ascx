<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PointSelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.DistributionPointSelector" %>
<%@ Import Namespace="Photoprint.Core" %>

<% if (UrlManager.CurrentPage == SiteLinkType.UserOrder) { %>
	<div class="modal-box wide long" id="mapModal">
		<h4>
			<asp:Literal ID="litModalTitle" runat="server" />
			<button class="button-close" type="button">×</button>
		</h4>
		<div class="dpoint-info"></div>
	</div>
<% } %>
<div class="DistributionPoint" id="dpoint_<%: InitializedPoint != null ? InitializedPoint.Id : 0 %>">
	<% if (IsVisibleTitle) {%>
		<h2 class="main"><%=RM.GetString("Shop.Checkout.DeliveryDPoint")%></h2>
	<% } %>	
	<div class="dpoint-info">
		<div class="dpoint-map" style="<%= UrlManager.CurrentPage == SiteLinkType.UserOrder ? "display:none" : "display:block" %>">
			<% if (SelectedDistributionPoint.MapType == Photoprint.Core.Models.MapType.GMap) { %>
				<% var staticMapSrc = $"http://maps.google.com/maps/api/staticmap?zoom=14&size={SelectedDistributionPoint.GMapStaticSize}";
				   staticMapSrc += $"&maptype=roadmap&markers=size:mid|color:red|label:A|{SelectedDistributionPoint.Latitude},";
				   staticMapSrc += $"{SelectedDistributionPoint.Longitude}&sensor=false";
				   staticMapSrc += $"&key={GMapKey}";  %>
				<a class="map" href="https://www.google.com/maps/place/<%=SelectedDistributionPoint.Latitude %>,<%=SelectedDistributionPoint.Longitude %>" target="_blank">
					<img src="<%= staticMapSrc %>" alt="map" />
				</a>
			<% } else if (SelectedDistributionPoint.MapType == Photoprint.Core.Models.MapType.Custom) { %>
				<div class="custom-map"><%= SelectedDistributionPoint.CustomMapCode %></div>
			<%} else if (System.IO.File.Exists(SelectedDistributionPoint.ThumbnailImagePath)) {%>
				<asp:HyperLink CssClass="map" ID="refFullMap" runat="server"><img alt="" src="" id="imgMap" runat="server" /></asp:HyperLink>
			<% } else {%>
				<div class="nomap">
					<p><%=RM.GetString("General.NoMap")%></p>
					<div class="topLayer"><img alt="" src="~/content/images/nomap.png" runat="server"/></div>
				</div>                
			<% } %>
		</div>
	
		<div class="info">
			<asp:Literal ID="litPointTitle" runat="server" />
			<p>
				<% if (UrlManager.CurrentPage == SiteLinkType.UserOrder) { %>
					<a href="#" class="semilink statusLnk" onclick="showMapModal();"><b><asp:Literal ID="litLinkTitle" runat="server" /></b></a><br />
				<% } else { %>
					<b><asp:Literal ID="litTitle" runat="server" /></b><br />
				<% } %>
				<em><%=RM.GetString("Photolab.Shipping.DPoint.OfficeHours")%></em> <asp:Literal ID="litOfficeHours" runat="server" /><br />
				<em><%=RM.GetString("Photolab.Shipping.DPoint.Address")%></em> <asp:Literal ID="litAddress" runat="server" /><br />
				<em><%=RM.GetString("Photolab.Shipping.DPoint.Phone")%></em> <asp:Literal ID="litPhone" runat="server" /><br />
			</p>
			<p><asp:Literal ID="litDescription" runat="server" /></p>   
		</div>
	</div>
	<% if (IsVisibleRecipientFields) {%>
		<fieldset class="profile">
			<ol>
				<li class="shipping-first-name">
					<label for="<%=txtFirstName.ClientID%>"><%=RM.GetString("Shop.Checkout.FirstNameLabel")%></label>
					<asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="Shipping" CssClass="text" />					    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regFirstName" Runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>
                <% if(UseMiddleName){ %>
                <li class="shipping-middle-name">
					<label for="<%=txtMiddleName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.MiddleNameLabel)%></label>
					<asp:TextBox ID="txtMiddleName" runat="server" ValidationGroup="Shipping" CssClass="text" /> 
                    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqMiddleName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtMiddleName" />
				    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.MiddleNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regMiddleName" Runat="server" ControlToValidate="txtMiddleName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>
                <% } %>
				<li class="shipping-last-name">
					<label for="<%=txtLastName.ClientID%>"><%=RM.GetString("Shop.Checkout.LastNameLabel")%></label>
					<asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />					    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.LastNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regLastName" Runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>  
				<li class="shipping-phone">
					<label for="<%=txtPhone.ClientID%>"><%=RM.GetString("Shop.Checkout.Phone")%></label>
					<asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
				</li>
				<%if (UseAdditionalEmail) {%>
					<li class="shipping-email">
						<label for="<%=txtShippingEmail.ClientID%>"><%=RM.GetString("Shop.Checkout.EmailLabel")%></label>
						<asp:TextBox ID="txtShippingEmail" runat="server" ValidationGroup="Shipping" CssClass="text" />
						<asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.EmailRequiredFormat %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regShippingEmail" Runat="server" ControlToValidate="txtShippingEmail" ValidationExpression=".*?@?[^@]*\.+.*"/>
					</li>
				<%} %>
			</ol>    
		</fieldset>
	<%}%>
</div>
<% TryGetShippingPrice(out var shippingPrice); %>
<% var selectedShipping = InitializedPoint; %>
<% var selectedAddress = GetSelectedOrderAddress(); %>
<script type="text/javascript">
	$(document).ready(function () {
		var shippingInfo = {
			address: <%= Newtonsoft.Json.JsonConvert.SerializeObject(selectedAddress?.ToString() ?? string.Empty) %>,
			addressZipCode: <%= !string.IsNullOrWhiteSpace(selectedAddress?.PostalCode) ? $"\"{HttpUtility.HtmlAttributeEncode(selectedAddress.PostalCode)}\"" : "null" %>,
			shippingId: parseInt("<%: selectedShipping != null ? selectedShipping.Id : 0 %>"),
			shippingTitle: "<%: selectedShipping?.GetTitle(WebSiteGlobal.CurrentLanguage) ?? string.Empty %>",
			shippingPrice: parseFloat("<%= shippingPrice.ToInteropPriceString() %>"),
			shippingPriceString: "<%= Utility.GetPrice(shippingPrice, WebSiteGlobal.CurrentPhotolab) %>",
			shippingPricePaidSeparately: "<%= Newtonsoft.Json.JsonConvert.SerializeObject(selectedShipping != null && selectedShipping.IsShippingPricePaidSeparately) %>" === 'true'
		};
		$(document).trigger('onShippingChanged', shippingInfo);
	});
</script>
<% if (UrlManager.CurrentPage == SiteLinkType.UserOrder) { %>
	<script type="text/javascript">
        function showMapModal() {
            $('#mapModal').jqmShow();
            $(document).find('.dpoint-map').children().appendTo($(document).find('#mapModal').children('.dpoint-info'));
        }
    </script>
<% } %>