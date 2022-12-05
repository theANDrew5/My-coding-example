<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PixlparkShippingProviderControl.ascx.cs" Inherits="Photoprint.WebSite.Controls.PixlparkShippingProviderControl" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Photoprint.WebSite.Modules" %>
<%@ Import Namespace="Photoprint.WebSite.Shared" %>
<%@ Import Namespace="Photoprint.Core" %>


<% if (ShippingTypes.Any()) { %>
	<table class="delivery-selector">
		<thead>
			<tr>
				<th><%=RM.GetString(RS.Shop.Checkout.DeliveryTypes)%></th>
				<th><%=RM.GetString(RS.Shop.Checkout.DeliveryWays)%></th>
			</tr>
		</thead>
		<tbody>
			<% var shippingsForDisplay = WebSiteGlobal.CurrentPhotolab.Properties.HideNotAvailableShippings ?
			       AllAvailableShippings :
			       FrontendShippingService.GetAvailableList<Shipping>(WebSiteGlobal.CurrentPhotolab, Enumerable.Empty<ShoppingCartItem>());
			   shippingsForDisplay = shippingsForDisplay.Where(shipping => !BannedProviders.Contains(shipping.ShippingServiceProviderType));
                foreach (var type in ShippingTypes) { %>
			<tr class="<%:type.ToString() %>">
				<td style="vertical-align: top; width: 50%" >
					<label class="type"><%= RM.GetString("Order.Shipping.Title" + type, false)%></label>
				</td>
				<td style="width: 50%">
					<% foreach (var shipping in shippingsForDisplay.Where(s => s.Type == type)) {
                            var isChecked = GetSelectedShipping() != null && GetSelectedShipping().Id == shipping.Id;
                            var url = new UrlManager().GetHRefUrl(SiteLinkType.CheckoutShipping);
                            var (success, price) = TryGetApproximateShippingPrice(shipping, out var isFixed);
                            var notAvailableMaterialTypes = GetNotAvailableMaterialTypesForShipping(shipping);
                            double maxWeight = shipping.MaximumWeight;
                            var checkoutWeightConstraint = CheckoutWeightConstraint(shipping);
                            var showWarningBox = ShowWarningBox(notAvailableMaterialTypes.Any(), checkoutWeightConstraint, shipping);
						%>
						<div class="shipping<%=shipping.Id %>" style="padding: 4px 0;">
                            <% if (showWarningBox) { %>
						    <div class="shipping-not-available-warning-box">
						        <input type="radio" disabled="disabled" />     
                            <% } else { %>
			                    <input name="selectedShipping" <%= isChecked ? "checked='checked'" : string.Empty %> type="radio" value="<%= string.Format("{0}?sid={1}", url, shipping.Id) %>" id="shipping<%=shipping.Id %>" /> 
						        <input type="hidden" id="_<%= shipping.Id %>" value="<%= price.ToString(CultureInfo.InvariantCulture).Replace(",", ".") %>" />
                                <% } %>
							
                            <label for="shipping<%=shipping.Id %>"><%: shipping.GetTitle(CurrentLanguage) %></label>
							<% if (price > 0.001m) { %>
						            <strong>(<%: !isFixed ? RM.GetString("General.From", false) : "+"%> <%= SiteUtils.GetPriceFormated(price, WebSiteGlobal.CurrentPhotolab) %>)</strong>
							<% } %>
						    
						    <% if (!string.IsNullOrWhiteSpace(shipping.DescriptionLocalized[CurrentLanguage])) { %>
						        <div><%= shipping.DescriptionLocalized[CurrentLanguage] %></div>
						    <% } %>

                            <% if (showWarningBox)
                               {
                                    if (notAvailableMaterialTypes.Any())
                                    { %>
                                        <label><%: RM.GetString(RS.Photolab.Shipping.ShippingNotAvailableBecauseTypesWarning) %></label>
                                        <ul>
                                        <% foreach (var materialType in notAvailableMaterialTypes) { %> 
                                                <li style="margin-bottom: 0"><%= materialType%></li>
                                        <% } %>
                                        </ul>
                                 <% }
                                    else
                                    {
                                        if (!checkoutWeightConstraint)
                                        {%>
                                            <label><%: RM.GetString(RS.Photolab.Shipping.ShippingWeightWarning) %>: <%:maxWeight%> <%:RM.GetString(RS.General.kg)%></label>
                                      <%}
                                        else
                                        {
                                            if (!AllAvailableShippings.Contains(shipping))
                                            { %>
                                                <label><%: RM.GetString(RS.Photolab.Shipping.ShippingNotAvailableWarning) %></label>
                                         <% } 
                                        }%>
						         <% } %>
						        </div>
				            <% } %>
						</div>
					<% } %>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
<% } else { %>
	<div class="message warning"><%= RM.GetString(RS.Shop.Checkout.ShippingUnavailable)%></div>
<% } %>

<div class="update_progress" style="display: none;"><span><%=RM.GetString("General.Loading")%></span></div>

<script type="text/javascript">
	$(".delivery-selector input:not(:checked)").click(function () {
		$('.update_progress').show();
		window.location = $(this).val() + '#updatableFields';
		return true; 
	});

	function onShippingPageLoaded(sender, args) {
	    var elems = $('.mobile-phone');
	    for (var i = 0; i< elems.length; i++) {
	        window.PhoneInputControl.process({element: elems[i]});
	    }
	}
	if (typeof (Sys) != 'undefined') {
		Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(onShippingPageLoaded);
	}
<% if(IsSingleDeliveryOperator) {%>
    if (!window.location.search.length) {
        window.location += "?sid=<%: GetSingleDeliveryId()%>#updatableFields";
    }
<% } %>
</script>
<br />

<div id="updatableFields">
	<div id="shipping_<%= GetSelectedShipping() != null ? GetSelectedShipping().Id : 0 %>">
		<asp:PlaceHolder ID="plhdShippingSelector" runat="server" />       
	</div>
	<asp:CustomValidator runat="server" ID="cstPrice" Display="Dynamic" CssClass="message error" ErrorMessage="<%$ RM: Delivery.TotalPriceBlock.RefershPriceError %> " OnServerValidate="cstPrice_ServerValidate"></asp:CustomValidator>
</div>
