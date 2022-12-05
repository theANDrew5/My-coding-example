<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UkrposhtaDeliverySelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.UkrposhtaDeliverySelector" %>
<%@ Import Namespace="Photoprint.Core" %>
<h2 class="main"><%=RM.GetString("Shop.Checkout.PostalShipping")%></h2>
<asp:UpdatePanel ID="upCourier" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
	<Triggers>
	    <asp:AsyncPostBackTrigger ControlID="ddlStates" EventName="SelectedIndexChanged" />
	    <asp:AsyncPostBackTrigger ControlID="ddlRegions" EventName="SelectedIndexChanged"/>
	    <asp:AsyncPostBackTrigger ControlID="ddlCities" EventName="SelectedIndexChanged" />
	    <asp:AsyncPostBackTrigger ControlID="ddlStreets" EventName="SelectedIndexChanged" />
	    <asp:AsyncPostBackTrigger ControlID="ddlAddresses" EventName="SelectedIndexChanged"/>
	</Triggers>
    <ContentTemplate>
        <fieldset class="profile postal" id="postal_<%: InitializedPostal?.Id ?? 0 %>">
	        <ol>
				<li class="shipping-first-name">
					<label for="<%=txtFirstName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.FirstNameLabel)%></label>
					<asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="Shipping" CssClass="text" />	
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtFirstName" />				    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regFirstName" Runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёєiїґА-ЯЁЄIЇҐ][а-яёєiїґА-ЯЁЄIЇҐ0-9-_\s]*$" />
                </li>
				<li class="shipping-last-name">
					<label for="<%=txtLastName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.LastNameLabel)%></label>
					<asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstLastNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtLastName" />		    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regLastName" Runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёєiїґА-ЯЁЄIЇҐ][а-яёєiїґА-ЯЁЄIЇҐ0-9-_\s]*$" />
                </li>
                <li class="shipping-middle-name">
                    <label for="<%=txtMiddleName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.MiddleNameLabel)%></label>
                    <asp:TextBox ID="txtMiddleName" runat="server" ValidationGroup="Shipping" CssClass="text" />
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstMiddleNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtMiddleName" />
                    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqMiddleName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtMiddlename" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.MiddleNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" ID="regMiddleName" runat="server" ControlToValidate="txtMiddleName" ValidationExpression="^[а-яёєiїґА-ЯЁЄIЇҐ][а-яёєiїґА-ЯЁЄIЇҐ0-9-_\s]*$" />
                </li>
                <%if (CurrentPhotolab.Properties?.AllowRegisterWithoutMiddleName ?? false) {%>
                        <li class="shipping-withoutMiddlename">
                            <asp:CheckBox id="chkAllowRegisterWithoutMiddleName"   Text="<%$ RM: Account.ContinueWithoutMiddleName %>"  runat="server" CssClass="checkbox"  />
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
	            <li class="shipping-phone">
	                <label for="<%=txtPhone.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.Phone)%></label>
	                <asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />	
	                <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstPhoneCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstPhoneCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtPhone" />
	                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
	            </li>
	            <li class="shipping-state">
	                <label for="<%=ddlStates.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.State)%></label>
	                <asp:DropDownList ID="ddlStates" ValidationGroup="Shipping" runat="server" AutoPostBack="True" />
	                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqDdlStates" ValidationGroup="Shipping" runat="server" ControlToValidate="ddlStates" InitialValue="0" />
	                <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstState" runat="server" OnServerValidate="CstStateServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlStates" />
	            </li>
	            <li class="shipping-region">
	                <label for="<%=ddlRegions.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.AreaRegion)%></label>
	                <asp:DropDownList ID="ddlRegions" ValidationGroup="Shipping" runat="server" AutoPostBack="True" />
	                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqDdlRegions" ValidationGroup="Shipping" runat="server" ControlToValidate="ddlRegions" InitialValue="0" />
	                <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstRegion" runat="server" OnServerValidate="CstAreaRegionServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlRegions" />
	            </li>
	            <li class="shipping-city">
	                <label for="<%=ddlCities.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.City)%></label>
	                <asp:DropDownList ID="ddlCities" ValidationGroup="Shipping" runat="server" AutoPostBack="True" />
	                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqDdlCities" ValidationGroup="Shipping" runat="server" ControlToValidate="ddlCities" InitialValue="0" />
	                <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstCity" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstCityServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlCities" />
	            </li>
                <asp:PlaceHolder runat="server" ID="plhdClientShipping" Visible="False">
                    <%--<li class="shipping-postal-code">
                        <label for="<%=txtPostalCode.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.PostalCode)%></label>
                        <asp:TextBox ID="txtPostalCode" runat="server" CssClass="text postalcode" Enabled="False" />
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqIndex" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPostalCode" />
                    </li>--%>
                    <li>
                        <label for="<%=ddlStreets.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.AddressStreet) %></label>
                        <asp:DropDownList ID="ddlStreets" ValidationGroup="Shipping" runat="server" AutoPostBack="True"/>
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqDdlStreets" ValidationGroup="Shipping" runat="server" ControlToValidate="ddlStreets" InitialValue="0" />
                        <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstStreet" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstStreetServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlStreets" />
                    </li>
					<li>
				        <label for="<%=ddlHouses.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressHouse)%></label>
                        <asp:DropDownList ID="ddlHouses" ValidationGroup="Shipping" runat="server"/>
					    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqDdlHouses" ValidationGroup="Shipping" runat="server" ControlToValidate="ddlHouses" InitialValue="0" />
					    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstHouse" ErrorMessage="<%$ RM: Validators,Invalid %>" runat="server" OnServerValidate="CstHouseServerCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlHouses" />
				    </li>
					<li>
						<label for="<%=txtAddressFlat.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressFlat)%></label>
						<asp:TextBox ID="txtAddressFlat" runat="server" ValidationGroup="Shipping" CssClass="text" />	
                        <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstFlatCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstFlatCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtAddressFlat" />
						<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" ID="reqAddressFlat" ControlToValidate="txtAddressFlat" ValidationGroup="Shipping" runat="server" Display="Dynamic" ForeColor="" CssClass="validator" />
					</li>
				</asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plhdStorageShipping" Visible="False">
                    <li class="shipping-address1">
                        <label for="<%=ddlAddresses.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.StorageAddress)%></label>
                        <asp:DropDownList ID="ddlAddresses" ValidationGroup="Shipping" runat="server" AutoPostBack="true" onChange="$('#ddlTarget').val('ddlAddresses');" />
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqDdlAddresses" ValidationGroup="Shipping" runat="server" ControlToValidate="ddlAddresses" InitialValue="0" />
                        <asp:CustomValidator ErrorMessage="<%$ RM: Validators.Invalid %>" Display="Dynamic" ForeColor="" ID="cstAddress" runat="server" OnServerValidate="CstAddressServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlAddresses" />
                    </li>
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
                </asp:PlaceHolder>
            </ol>
            <input type="hidden" id="ddlTarget" name="ddlTarget"/>
        </fieldset>
        <% TryGetShippingPrice(out var shippingPrice); %>
        <% var selectedShipping = InitializedPostal; %>
        <% var selectedAddress = GetSelectedOrderAddress(); %>
        <div id="selectedShippingInfo"
             data-address="<%= HttpUtility.HtmlAttributeEncode(selectedAddress?.ToString() ?? string.Empty) %>"
             data-address-zip-code="<%= HttpUtility.HtmlAttributeEncode(selectedAddress?.PostalCode ?? string.Empty) %>"
             data-shipping-id="<%= HttpUtility.HtmlAttributeEncode(selectedShipping?.Id.ToString() ?? string.Empty) %>"
             data-shipping-title="<%= HttpUtility.HtmlAttributeEncode(selectedShipping?.GetTitle(CurrentLanguage) ?? string.Empty) %>"
             data-shipping-price="<%= shippingPrice.ToInteropPriceString() %>"
             data-shipping-price-string="<%= Utility.GetPrice(shippingPrice, CurrentPhotolab) %>"
             data-shipping-price-paid-separately="<%= Newtonsoft.Json.JsonConvert.SerializeObject(selectedShipping?.IsShippingPricePaidSeparately ?? false) %>">
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    $(document).ready(function () {
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function() {
            $('.postal select, .postal textarea, .postal input').attr("disabled", "disabled");
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
    });
</script>