<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToClientDeliveryMailSelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.ToClientMailShippingSelector" %>
<%@ Import Namespace="Photoprint.Core" %>
<h2 class="main"><%=RM.GetString("Shop.Checkout.PostalShipping")%></h2>
<asp:UpdatePanel ID="upCourier" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="ddlRegions" EventName="SelectedIndexChanged" />
		<asp:AsyncPostBackTrigger ControlID="ddlCountry" EventName="SelectedIndexChanged" />
		<asp:AsyncPostBackTrigger ControlID="ddlCities" EventName="SelectedIndexChanged" />
	</Triggers>
    <ContentTemplate>
        <fieldset class="profile postal-client" id="postal_<%: InitializedPostal != null ? InitializedPostal.Id : 0 %>">
	        <ol>
				<li class="shipping-first-name">
					<label for="<%=txtFirstName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.FirstNameLabel)%></label>
					<asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="Shipping" CssClass="text" />	
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtFirstName" />				    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regFirstName" Runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>
				<li class="shipping-last-name">
					<label for="<%=txtLastName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.LastNameLabel)%></label>
					<asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstLastNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtLastName" />		    
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regLastName" Runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
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
                <% } %>
				<li class="shipping-phone">
					<label for="<%=txtPhone.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.Phone)%></label>
					<asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />	
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstPhoneCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstPhoneCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtPhone" />
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
				</li>	 
				<asp:PlaceHolder ID="plhdCountrySelector" runat="server">
					<li class="shipping-country">
						<label for="<%=ddlCountry.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.Country)%></label>
						<asp:DropDownList ID="ddlCountry" runat="server"  AutoPostBack="True" />
						<asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstCountry" runat="server" OnServerValidate="CstCountryServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlCountry" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdCountryInput" runat="server" Visible="false">
					<li class="shipping-country">
						<label for="<%=txtCountry.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.Country)%></label>
						<asp:TextBox ID="txtCountry" runat="server" CssClass="text" />
						<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ValidationGroup="Shipping" ID="reqCountry" runat="server" ControlToValidate="txtCountry" ForeColor="" CssClass="validator" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdStateSelector" runat="server" Visible="false">
					<li class="shipping-state">
						<label for="<%=ddlRegions.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.State)%></label>
						<asp:DropDownList ID="ddlRegions" runat="server" AutoPostBack="True"  />
						<asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstRegion" runat="server" OnServerValidate="CstRegionServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlRegions" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdStateInput" runat="server" Visible="false">
					<li class="shipping-state">
						<label for="<%=txtState.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.State)%></label>
						<asp:TextBox ID="txtState" runat="server" CssClass="text" />
						<% if (((Postal)Shipping).IsRegionRequired) {%>
							<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqState" ValidationGroup="Shipping" runat="server" ControlToValidate="txtState" />
						<% } %>
					</li>
				</asp:PlaceHolder>
	            <asp:PlaceHolder ID="plhdRegionInput" runat="server" Visible="false">
	                <li class="shipping-region">
	                    <label for="<%=txtRegion.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.AreaRegion)%></label>
	                    <asp:TextBox ID="txtRegion" runat="server" CssClass="text" />
	                    <asp:CustomValidator Display="Dynamic" ErrorMessage="<%$ RM: Validators.Invalid %>" ForeColor="" ID="cstRegionCorrect" runat="server" OnServerValidate="CstAreaRegionServerValidate" CssClass="validator" ValidationGroup="Shipping" ValidateEmptyText="True" ControlToValidate="txtRegion" />
                    </li>
	            </asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdCitySelector" runat="server" Visible="false">
					<li class="shipping-city">
						<label for="<%=ddlCities.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.City)%></label>
						<asp:DropDownList ID="ddlCities" runat="server" AutoPostBack="True" />
						<asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstCity" runat="server" OnServerValidate="CstCityServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlCities" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhdCityInput" runat="server" Visible="false">
					<li class="shipping-city">
						<label for="<%=txtCity.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.City)%></label>
						<asp:TextBox ID="txtCity" runat="server" AutoPostBack="True" CssClass="text" EnableViewState="false" />
                        <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstCityCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstCityCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtCity" />
						<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqCity" ValidationGroup="Shipping" runat="server" ControlToValidate="txtCity" />
					</li>
				</asp:PlaceHolder>
				<li class="shipping-postal-code">
			        <label for="<%=txtPostalCode.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.PostalCode)%></label>
			        <asp:TextBox ID="txtPostalCode" runat="server" CssClass="text postalcode" />
					<% if (((Postal)Shipping).IsIndexRequired) {%>
					    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqIndex" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPostalCode" />	
					<% } %>
		        </li>
				<asp:PlaceHolder runat="server" ID="plhdAddressSeparate" Visible="False">
				    <li>
				        <label for="<%= txtAddressStreet.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.AddressStreet) %></label>
				        <asp:TextBox ID="txtAddressStreet" runat="server" ValidationGroup="Shipping" CssClass="text" />
				        <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstStreetCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstStreetCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtAddressStreet" />
				        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" ID="reqAddressStreet" ControlToValidate="txtAddressStreet" ValidationGroup="Shipping" runat="server" Display="Dynamic" ForeColor="" CssClass="validator" />
				    </li>
					<li>
				        <label for="<%=txtAddressHouse.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressHouse)%></label>
				        <asp:TextBox ID="txtAddressHouse" runat="server" ValidationGroup="Shipping" CssClass="text" />
				        <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstHouseCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstHouseCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtAddressHouse" />
				        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" ID="reqAddressHouse" ControlToValidate="txtAddressHouse" ValidationGroup="Shipping" runat="server" Display="Dynamic" ForeColor="" CssClass="validator" />
				    </li>
					<li>
						<label for="<%=txtAddressFlat.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressFlat)%></label>
						<asp:TextBox ID="txtAddressFlat" runat="server" ValidationGroup="Shipping" CssClass="text" />	
                        <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstFlatCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstFlatCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtAddressFlat" />
						<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" ID="reqAddressFlat" ControlToValidate="txtAddressFlat" ValidationGroup="Shipping" runat="server" Display="Dynamic" ForeColor="" CssClass="validator" />
					</li>
				</asp:PlaceHolder>
				<asp:PlaceHolder runat="server" ID="plhdAddressLines" Visible="False">
					<li class="shipping-address1">
						<label for="<%=txtAddressLine1.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressLine1)%></label>
						<asp:TextBox ID="txtAddressLine1" TextMode="MultiLine" runat="server" ValidationGroup="Shipping" CssClass="text" />
						<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqAddressLine1" ValidationGroup="Shipping" runat="server" ControlToValidate="txtAddressLine1" />
					</li>
					<asp:PlaceHolder ID="plhdAddressLine2" runat="server" Visible="false">
						<li class="shipping-address2">
							<label for="<%=txtAddressLine1.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressLine2)%></label>
							<asp:TextBox ID="txtAddressLine2" TextMode="SingleLine" runat="server" ValidationGroup="Shipping" CssClass="text" />
						</li>
					</asp:PlaceHolder>
				</asp:PlaceHolder>
	            
	            <%-- Boxberry еще не доработали систему, при которой будет возможность через сайт указывать желательное время доставки клиенту.
                     Функцинал ниже рабочий, но без валидации времени.
                <asp:PlaceHolder runat="server" ID="plhdCourierChooseTime" Visible="False">
                    <li class="shipping-courier-choose-time">
                        <div class="ab">
                            <label><%=RM.GetString(RS.Shop.Checkout.CourierDeliveryTimeText)%></label>
                            <div class="ab-a">
                                <label><%=RM.GetString(RS.Shop.Checkout.CourierDeliveryTimeFrom)%></label>
                                <%= TimeInfoController.GetDropdownListsHtml("choose-time-from", 10) %>
                            </div>
                            <div class="ab-b">
                                <label><%=RM.GetString(RS.Shop.Checkout.CourierDeliveryTimeTo)%></label>
                                <%= TimeInfoController.GetDropdownListsHtml("choose-time-to", 10) %>
                            </div>
                        </div>
                    </li>
                    <li class="shipping-courier-commentary">
                        <div>
                            <label><%=RM.GetString(RS.Shop.Checkout.CourierDeliveryCommentary)%></label>
                            <asp:TextBox runat="server" CssClass="text" ID="txtCourierCommentary"></asp:TextBox>
                        </div>
                    </li>
                </asp:PlaceHolder>--%>
	        </ol>
        </fieldset>
		<% TryGetShippingPrice(out var shippingPrice); %>
		<% var selectedShipping = InitializedPostal; %>
		<% var selectedAddress = GetSelectedOrderAddress(); %>
		<div id="selectedShippingInfo"
			data-address="<%= selectedAddress != null ? HttpUtility.HtmlAttributeEncode(selectedAddress.ToString()) : String.Empty %>"
			data-address-zip-code="<%= selectedAddress != null ? HttpUtility.HtmlAttributeEncode(selectedAddress.PostalCode) : String.Empty %>"
			data-shipping-id="<%= selectedShipping != null ? HttpUtility.HtmlAttributeEncode(selectedShipping.Id.ToString()) : String.Empty %>"
			data-shipping-title="<%= selectedShipping != null ? HttpUtility.HtmlAttributeEncode(selectedShipping.GetTitle(CurrentLanguage)) : String.Empty %>"
			data-shipping-price="<%= shippingPrice.ToInteropPriceString() %>"
			data-shipping-price-string="<%= Utility.GetPrice(shippingPrice, WebSiteGlobal.CurrentPhotolab) %>"
			data-shipping-price-paid-separately="<%= selectedShipping != null ? Newtonsoft.Json.JsonConvert.SerializeObject(selectedShipping.IsShippingPricePaidSeparately) : "false" %>">
		</div>
	</ContentTemplate>
</asp:UpdatePanel>
<script type="text/javascript">
    $(document).ready(function () {
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function() {
            $('.postal-client select, .postal-client textarea, .postal-client input').attr("disabled", "disabled");
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