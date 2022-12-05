<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToStorageDeliveryMailSelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.ToStorageMailShippingSelector" %>
<%@ Import Namespace="Photoprint.Core" %>

<script src="/Content/js/chosen.jquery.min.js"></script>
<h2 class="main"><%=RM.GetString("Shop.Checkout.PostalShipping")%></h2>
<asp:UpdatePanel ID="upCourier" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="ddlRegions" EventName="SelectedIndexChanged" />
		<asp:AsyncPostBackTrigger ControlID="ddlCountries" EventName="SelectedIndexChanged" />
		<asp:AsyncPostBackTrigger ControlID="ddlCities" EventName="SelectedIndexChanged" />
		<asp:AsyncPostBackTrigger ControlID="ddlAddresses" EventName="SelectedIndexChanged" />
	</Triggers>
    <ContentTemplate>		
        <fieldset class="profile postal-storage" id="postal_<%: InitializedPostal?.Id ?? 0 %>">
	        <ol>
				<li class="shipping-first-name">
					<label for="<%=txtFirstName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.FirstNameLabel)%></label>
					<asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="Shipping" CssClass="text" />					    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM:Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
				    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regFirstName" Runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>
				<li class="shipping-last-name">
					<label for="<%=txtLastName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.LastNameLabel)%></label>
					<asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />					    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
				    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.LastNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regLastName" Runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>
                <% if(UseMiddleName){ %>
                <li class="shipping-middle-name">
					<label for="<%=txtMiddleName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.MiddleNameLabel)%></label>
					<asp:TextBox ID="txtMiddleName" runat="server" ValidationGroup="Shipping" CssClass="text" /> 
                    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqMiddleName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtMiddleName" />
				    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.MiddleNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regMiddleName" Runat="server" ControlToValidate="txtMiddleName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>
                <%if (CurrentPhotolab.Properties?.AllowRegisterWithoutMiddleName ?? false) {%>
                        <li class="shipping-withoutMiddlename">
                            <asp:CheckBox id="chkAllowRegisterWithoutMiddleName" Text="<%$ RM: Account.ContinueWithoutMiddleName %>"  runat="server" CssClass="checkbox"  />
                        </li>
                        <script type="text/javascript">
                            $("#<%= chkAllowRegisterWithoutMiddleName.ClientID%>").change(function () {
                                $(".shipping-middle-name").toggle();
                                if ($("#<%= chkAllowRegisterWithoutMiddleName.ClientID%>").attr("checked") == 'checked')
                                    $("#<%= txtMiddleName.ClientID%>").val("тест");
                                else
                                    $("#<%= txtMiddleName.ClientID%>").val("");
                            });
                        </script>
                    <% }%>
                <% } %>
				<li class="shipping-phone">
					<label for="<%=txtPhone.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.Phone)%></label>
					<asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
				</li>	 
				<asp:PlaceHolder ID="plhdCountries" runat="server">
					<li class="shipping-country">
						<label for="<%=ddlCountries.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.Country)%></label>
						<asp:DropDownList ID="ddlCountries" runat="server" AutoPostBack="true" />
						<asp:CustomValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" ID="cstCountry" runat="server" OnServerValidate="cstCountry_ServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlCountries" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdRegions" runat="server">
					<li class="shipping-state">
						<label for="<%=ddlRegions.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.State)%></label>
						<asp:DropDownList ID="ddlRegions" runat="server" AutoPostBack="true" onChange="$('#ddlTarget').val('ddlRegions');" />
						<asp:CustomValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" ID="cstRegion" runat="server" OnServerValidate="cstRegion_ServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlRegions" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdCities" runat="server">
					<li class="shipping-city">
						<label for="<%=ddlCities.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.City)%></label>
						<prs:DecoratedDropDownList ID="ddlCities" runat="server" AutoPostBack="true" onChange="$('#ddlTarget').val('ddlCities');" />
						<asp:CustomValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" ID="cstCity" runat="server" OnServerValidate="cstCity_ServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlCities" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdAddresses" runat="server">
					<li class="shipping-address1">
						<label for="<%=ddlAddresses.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.StorageAddress)%></label>
						 <prs:DecoratedDropDownList ID="ddlAddresses" runat="server" AutoPostBack="true" onChange="$('#ddlTarget').val('ddlAddresses');" />
						<asp:CustomValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" ID="cstAddress" runat="server" OnServerValidate="cstAddress_ServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlAddresses" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdDescription" runat="server">
					<li class="shipping-description">
						<asp:Literal runat="server" ID="litDescription" />
					</li>
				</asp:PlaceHolder>
                <asp:PlaceHolder ID="plhdMaxWeight" runat="server" Visible="False">
					<li class="shipping-max-weight">
					    <div class="message warning"><asp:Literal runat="server" ID="litMaxWeight" /></div>						
					</li>
				</asp:PlaceHolder>
	        </ol>
			<input type="hidden" id="ddlTarget" name="ddlTarget"/>
        </fieldset>
		<% TryGetShippingPrice(out var shippingPrice); %>
		<% var selectedShipping = InitializedPostal; %>
		<% var selectedAddress = GetSelectedOrderAddress(); %>
		<div id="selectedShippingInfo"
			data-address="<%= selectedAddress != null ? HttpUtility.HtmlAttributeEncode(selectedAddress.ToString()) : string.Empty %>"
			data-address-zip-code="<%= selectedAddress != null ? HttpUtility.HtmlAttributeEncode(selectedAddress.PostalCode) : string.Empty %>"
			data-shipping-id="<%= selectedShipping != null ? HttpUtility.HtmlAttributeEncode(selectedShipping.Id.ToString()) : string.Empty %>"
			data-shipping-title="<%= selectedShipping != null ? HttpUtility.HtmlAttributeEncode(selectedShipping.GetTitle(CurrentLanguage)) : string.Empty %>"
			data-shipping-price="<%= shippingPrice.ToInteropPriceString() %>"
			data-shipping-price-string="<%= Utility.GetPrice(shippingPrice, WebSiteGlobal.CurrentPhotolab) %>"
			data-shipping-price-paid-separately="<%= selectedShipping != null ? Newtonsoft.Json.JsonConvert.SerializeObject(selectedShipping.IsShippingPricePaidSeparately) : "false" %>">
		</div>
	</ContentTemplate>
</asp:UpdatePanel>


<script type="text/javascript">

	$(document).ready(function () {
		Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function() {
			$('.postal-storage select, .postal-storage textarea, .postal-storage input').attr("disabled", "disabled");
		});
		Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() {
			var selectedShippingInfo = $('#selectedShippingInfo');
			var shippingInfo = selectedShippingInfo.size() === 1 ? {
				address: selectedShippingInfo.attr('data-address'),
				addressZipCode: selectedShippingInfo.attr('data-address-zip-code'),
				shippingId: selectedShippingInfo.attr('data-shipping-id'),
				shippingTitle: selectedShippingInfo.attr('data-shipping-title'),
				shippingPrice: parseFloat(selectedShippingInfo.attr('data-shipping-price')),
				shippingPriceString: selectedShippingInfo.attr('data-shipping-price-string'),
				shippingPricePaidSeparately: selectedShippingInfo.attr('data-shipping-price-paid-separately') === 'true'
			} : null;
			$(document).trigger('onShippingChanged', shippingInfo);
		});

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            $('#<%:ddlAddresses.ClientID%>').chosen({width: "100%", "no_results_text": "<%:RM.GetString(RS.General.ChosenPlugin.NoResultsText)%>", "placeholder_text_single": "<%:RM.GetString(RS.Shop.Checkout.StorageAddress)%>" });
            $('#scriptFor-<%:ddlAddresses.ClientID%>').remove();

            $('#<%:ddlCities.ClientID%>').chosen({width: "100%", "no_results_text": "<%:RM.GetString(RS.General.ChosenPlugin.NoResultsText)%>", "placeholder_text_single": "<%:RM.GetString(RS.Shop.Checkout.SelectCity)%>" });
            $('#scriptFor-<%:ddlCities.ClientID%>').remove();
        });
	});
</script>
