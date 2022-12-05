<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourierSelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.CourierShippingSelector" %>
<%@ Import Namespace="Photoprint.Core" %>
<%@ Import Namespace="Photoprint.WebSite.Controls" %>
<h2 class="main"><%=RM.GetString(RS.Shop.Checkout.Home)%></h2>
<fieldset class="profile courier" id="courier_<%: InitializedCourier != null ? InitializedCourier.Id : 0 %>">
	<ol>
	    <% if (InitializedCourier != null && InitializedCourier.IsEnableCompanyName) { %>
	    <li class="shipping-company-name">
			<label for="<%= txtCompanyName.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.CompanyNameLabel) %></label>
			<asp:TextBox ID="txtCompanyName" runat="server" CssClass="text" />					    
		</li>
        <% } %>
		<li class="shipping-first-name">
			<label for="<%=txtFirstName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.FirstNameLabel)%></label>
			<asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="Shipping" CssClass="text" />					    
			<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
            <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regFirstName" Runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
        </li>
        <% if(UseMiddleName){ %>
                <li class="shipping-middle-name" style="display: block">
					<label for="<%=txtMiddleName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.MiddleNameLabel)%></label>
					<asp:TextBox ID="txtMiddleName" runat="server" ValidationGroup="Shipping" CssClass="text" /> 
                    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqMiddleName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtMiddleName"  />
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
		<li class="shipping-last-name">
			<label for="<%=txtLastName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.LastNameLabel)%></label>
			<asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />					    
			<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
            <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.LastNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regLastName" Runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
		</li>  
		<li class="shipping-phone">
			<label for="<%=txtPhone.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.Phone)%></label>
			<asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />
            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
        </li>
        <asp:PlaceHolder ID="plhdAddressLine1" runat="server" Visible="false">
            <li class="shipping-address1">
                <label for="<%=txtAddressLine1.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressLine1)%></label>
                <asp:TextBox ID="txtAddressLine1" TextMode="MultiLine" runat="server" ValidationGroup="Shipping" CssClass="text" />
                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqAddressLine1" ValidationGroup="Shipping" runat="server" ControlToValidate="txtAddressLine1" />
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plhdFullAddress" Visible="false">
            <li class="shipping-street">
                <label for="<%=txtStreet.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressStreet)%></label>
                <asp:TextBox ID="txtStreet" runat="server" ValidationGroup="Shipping" CssClass="text" />
                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqStreet" ValidationGroup="Shipping" runat="server" ControlToValidate="txtStreet" />
            </li>
            <li class="shipping-house">
                <label for="<%=txtHouse.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressHouse)%></label>
                <asp:TextBox ID="txtHouse" runat="server" ValidationGroup="Shipping" CssClass="text" />
                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqHouse" ValidationGroup="Shipping" runat="server" ControlToValidate="txtHouse" />
            </li>
            <li class="shipping-flat">
                <label for="<%=txtFlat.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressFlat)%></label>
                <asp:TextBox ID="txtFlat" runat="server" ValidationGroup="Shipping" CssClass="text" />
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhdAddressLine2" runat="server" Visible="false">
            <li class="shipping-address2">
                <label for="<%=txtAddressLine2.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.CourierAddressLine2)%></label>
                <asp:TextBox ID="txtAddressLine2" TextMode="SingleLine" runat="server" ValidationGroup="Shipping" CssClass="text" />
            </li>
        </asp:PlaceHolder>
        <li class="shipping-city">
			<% if (HasAdditionalAddresses)
			   {%>
                <label for="<%=ddlCities.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.City)%></label>
                <prs:DecoratedDropDownList ID="ddlCities" runat="server" AutoPostBack="true" onChange="$('#ddlTarget').val('ddlCities');" />
                <asp:CustomValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" ID="cstDdlCity" runat="server" OnServerValidate="cstDddlCity_ServerValidate" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="ddlCities" />
			<%   }
			   else
			   { %>
				<asp:HiddenField runat="server" ID="addressId"/>
                <label for="<%=txtCity.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.City)%></label>
                <asp:TextBox ID="txtCity" runat="server" CssClass="text" />
                <asp:CustomValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" OnServerValidate="cstTxtCity_ServerValidate" ID="cstTxtCity" ValidationGroup="Shipping" runat="server" ControlToValidate="txtCity" />
			<%   }%>
		</li>   
		<li class="shipping-postal-code">
			<label for="<%=txtPostalCode.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.PostalCode)%></label>
			<asp:TextBox ID="txtPostalCode" runat="server" CssClass="text postalcode" />
			<% if (((Courier)Shipping).IsIndexRequired) {%>
				<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqIndex" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPostalCode" />	
			<% } %>
		</li>
		<asp:PlaceHolder ID="plhdState" runat="server">
			<li class="shipping-state">
				<label for="<%=txtState.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.State)%></label>
				<asp:TextBox ID="txtState" runat="server" CssClass="text" />
				<% if (((Courier)Shipping).IsRegionRequired) {%>
					<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqState" ValidationGroup="Shipping" runat="server" ControlToValidate="txtState" />
				<% } %>
		</li>
		</asp:PlaceHolder>
		<asp:PlaceHolder ID="plhdCountry" runat="server">
			<li class="shipping-country">
				<label for="<%=txtCountry.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.Country)%></label>
				<asp:TextBox ID="txtCountry" runat="server" CssClass="text" />
				<asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqCountry" ValidationGroup="Shipping" runat="server" ControlToValidate="txtCountry" />
			</li>
		</asp:PlaceHolder>
		<li class="shipping-description">    
			<label for="<%=txtDescription.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.Description)%></label>
			<asp:TextBox ID="txtDescription" runat="server" CssClass="text" TextMode="MultiLine" />
		</li>
	</ol>
</fieldset>
<% TryGetShippingPrice(out var shippingPrice); %>
<% var selectedShipping = InitializedCourier; %>
<% var selectedAddress = GetSelectedOrderAddress(); %>
<script type="text/javascript">
	$(document).ready(function () {
		var shippingInfo = {
			address: "<%= selectedAddress != null ? HttpUtility.HtmlAttributeEncode(selectedAddress.ToString()) : string.Empty %>",
			addressZipCode: <%= !string.IsNullOrWhiteSpace(selectedAddress?.PostalCode) ? $"\"{HttpUtility.HtmlAttributeEncode(selectedAddress.PostalCode)}\"" : "null" %>,
			shippingId: parseInt("<%: selectedShipping != null ? selectedShipping.Id : 0 %>"),
			shippingTitle: "<%: selectedShipping != null ? selectedShipping.GetTitle(CurrentLanguage) : string.Empty %>",
			shippingPrice: parseFloat("<%= shippingPrice.ToInteropPriceString() %>"),
			shippingPriceString: "<%= Utility.GetPrice(shippingPrice, WebSiteGlobal.CurrentPhotolab) %>",
			shippingPricePaidSeparately: "<%= Newtonsoft.Json.JsonConvert.SerializeObject(selectedShipping != null && selectedShipping.IsShippingPricePaidSeparately) %>" === 'true'
		};
		$(document).trigger('onShippingChanged', shippingInfo);
	});
</script>