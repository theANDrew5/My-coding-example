<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PickpointDeliverySelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.PickpointDeliverySelector" %>
<%@ Import Namespace="Photoprint.Core" %>
<h2 class="main"><%=RM.GetString("Shop.Checkout.PostalShipping")%></h2>
<asp:UpdatePanel ID="upCourier" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="txtAddress" EventName="TextChanged" />
    </Triggers>
    <ContentTemplate>
        <fieldset class="profile postal" id="postal_<%: CurrentPostal?.Id ?? 0 %>">
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
                <% if(UseMiddleName) { %>
                <li class="shipping-middle-name">
                    <label for="<%=txtMiddleName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.MiddleNameLabel)%></label>
                    <asp:TextBox ID="txtMiddleName" runat="server" ValidationGroup="Shipping" CssClass="text" />
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstMiddleNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtMiddleName" />
                    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqMiddleName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtMiddlename" />
                    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.MiddleNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" ID="regMiddleName" runat="server" ControlToValidate="txtMiddleName" ValidationExpression="^[а-яёєiїґА-ЯЁЄIЇҐ][а-яёєiїґА-ЯЁЄIЇҐ0-9-_\s]*$" />
                </li>
                <% } %>
                <li class="shipping-phone">
                    <label for="<%=txtPhone.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.Phone)%></label>
                    <asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />
                    <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstPhoneCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstPhoneCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtPhone" />
                    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
                </li>
                <li style="padding-bottom: 15px;">
                    <script type="text/javascript" src="https://pickpoint.ru/select/postamat.js" charset="utf-8"></script>
                    <a href="#" onclick="PickPoint.open(my_function, '<%= Settings.IKN %>'); return false">Выбрать постамат</a>
                    <script type="text/javascript">
                        function my_function(result) {                        
                            document.getElementById('pickpointWidgetResult').value = JSON.stringify(result);
                            document.getElementById('<%=txtAddress.ClientID %>').value = result['name'] + '. ' + result['address'];
                            __doPostBack('<%=txtAddress.ClientID %>', "TextChanged");
                        };
                    </script>
                </li>
                <asp:PlaceHolder ID="plhdAddress" runat="server">
                    <li class="shipping-country">
                        <label for="<%=txtAddress.ClientID %>">Выбран:</label>
                        <asp:TextBox ID="txtAddress" runat="server" Enabled="false" ValidationGroup="Shipping" AutoPostBack="True" />
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqAddress" ValidationGroup="Shipping" runat="server" ControlToValidate="txtAddress" InitialValue="0"/>
                    </li>
                </asp:PlaceHolder>
            </ol>
        </fieldset>
        <% TryGetShippingPrice(out var shippingPrice); %>
        <% var selectedShipping = CurrentPostal; %>
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
<input type="hidden" id="pickpointWidgetResult" name="pickpointWidgetResult" />

<script type="text/javascript">
    $(document).ready(function () {
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
            $('.postal select, .postal textarea, .postal input').attr("disabled", "disabled");
        });
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
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
