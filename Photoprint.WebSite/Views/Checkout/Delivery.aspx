<%@ Page Language="C#" MasterPageFile="~/Templates/Placeholder.Master" EnableEventValidation="false" AutoEventWireup="true" EnableViewState="false" CodeBehind="Delivery.aspx.cs" Inherits="Photoprint.WebSite.Delivery" %>
<%@ Import Namespace="Photoprint.WebSite.Modules" %>
<%@ Register TagPrefix="pr" TagName="ShoppingCartSummary" src="~/Views/Checkout/Controls/ShoppingCartSummary.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphMainPanel" runat="server">
	<script type="text/javascript">
        $(document).ready(function () {
            var elems = $('.mobile-phone');
            for (var i = 0; i< elems.length; i++) {
                window.PhoneInputControl.process({element: elems[i]});
            }
        });
    </script>

	<pr:Skinner ID="Skinner" runat="server" Control="OrderHeader" View="Checkout" />

	<div class="A">
		<h1><%=RM.GetString(RS.Shop.Checkout.DeliveryTitle)%></h1>
		<% var text = RM.GetString(RS.Shop.Checkout.SelectShippingType); %>
        <% if (!string.IsNullOrWhiteSpace(text)) { %><p><%=text %></p><% } %>
		
		<asp:placeholder runat="server" id="plhdError" Visible="false">
            <div class="message warning">
		        <asp:Label runat="server" ID="txtErrMessage" Text="<%$ RM: Shop.Checkout.ShippingNotSelectedError %>"></asp:Label>
			</div>
		</asp:placeholder>
		<asp:placeholder runat="server" id="plhdCheckoutValidation" Visible="false">
			<div class="message warning"><asp:Literal ID='litCheckoutValidation' runat="server" /></div>
		</asp:placeholder>
		<asp:placeholder runat="server" id="plhdMinimumShoppingCartItemsPrice" Visible="false">
			<div class="message information"><asp:Literal ID='litMinimumShoppingCartItemsPrice' runat="server" /></div>
		</asp:placeholder>

		<% if (LoggedInUser != null &&  LoggedInUser.Status == UserStatus.Blocked) {%>
			<div class="message warning"><%=RM.GetString(RS.Shop.Checkout.UserIsBlockedError)%></div>
		<% } %>


		<asp:Placeholder ID="plhdShippingProvider" runat="server"/>
		
		<div id="addOrderComment" class="order-comment" style="display: none">
			<fieldset>
				<ol>
					<li>
						<label for="txtOrderComment"><%= RM.GetString(RS.Shop.Checkout.OrderCommentLabel) %></label>
						<asp:TextBox runat="server" ID="txtOrderComment" TextMode="MultiLine" CssClass="text" />
						<div class="hint"><%= RM.GetString(RS.Shop.Checkout.OrderCommentHint) %></div>
					</li>
				</ol>
			</fieldset>
		</div>
		<script type="text/javascript">
		    $(document).on('onShippingChanged', function(e, shipping) {
		        $('#addOrderComment').toggle(shipping != null);
		    });
		</script>
        <%if (UserCompanyInfo != null  && UserCompanyInfo.Role != UserCompanyRole.Guest) { %>
          <fieldset>
            <div class="checkbox">
                <asp:CheckBox runat="server" Checked="true" ID="chkByCompany" TextMode="MultiLine"/>
                <label for="chkByCompany"><%= RM.GetString(RS.Shop.Checkout.CreateOrderFromOrganization) %></label>
            </div>
          </fieldset>
        <%} %>
		<pr:ShoppingCartSummary ID="mgOrderSummary" runat="server" />


		<div id="messagePageInvalid" class="message warning page-validator" style="display: none"><%=RM.GetString(RS.Validators.PageInvalid)%></div>
		<% if (LoggedInUser != null && LoggedInUser.Status != UserStatus.Blocked) { %>
			<div class="buttons">
				<% if (CurrentPhotolab.Properties.SkipShoppingCartAfterEditor) { %>
					<a href="<%: new UrlManager { CurrentLanguageCulture = UrlManager.CurrentLanguageCulture }.GetHRefUrl(SiteLinkType.ShoppingCart) %>"><%: RM.GetString(RS.ShoppingCart.GoToShoppingCart) %></a> <%: RM.GetString(RS.General.Or, false) %>
				<% } %>
				<div class="ok"><prs:AutoDisableButton ID="butNext" runat="server" Text="{0} &rarr;" OnClick="NextStepClick" /></div>
			</div>
			<% if (RenderGoogleAnalytics) { %>
				<script>
				    function selectShippingGoogleTrack() {
				        try {
				            var shippingName = $('.delivery-selector input:checked').siblings('label').text();
				            pxlga('cst.ec:setAction', 'checkout_option', { 'step': 2, 'option': shippingName });
				            pxlga('cst.send', 'event', 'Checkout', 'Option', { 'hitCallback': function() {} });

				            pxlga('pxl.ec:setAction', 'checkout_option', { 'step': 2, 'option': shippingName });
				            pxlga('pxl.send', 'event', 'Checkout', 'Option', { 'hitCallback': function() {} });
				        } catch (e) {

				        } 
						
				        setTimeout(function() { <%: butNext.TrackingCallbackFunctionName %>(); }, 300);
				    }
				</script>
			<% } %>
		<% } %>
		<script type="text/javascript">
		    $(document).ready(function () {
		        $("div.buttons div.ok input").click(function () {
		            window.setTimeout(function () {
		                var visibleAllerts = $('.message.warning:visible:not(".page-validator")').size(); // обычно: message warning alert alert-danger
		                var visibleValidators = $('span.validator:visible').size(); // специфично для подгруженных контролов, например PointSelector.ascx
		                //
		                if (visibleAllerts > 0 || visibleValidators > 0) {
		                    $("#messagePageInvalid").show();
		                } else {
		                    $("#messagePageInvalid").hide();
		                }
		            }, 100);
		        });
		    });

		    var clientFirstName = "<%= LoggedInUser.FirstName %>";
		    var clientLastName = "<%= LoggedInUser.LastName %>";
		</script>

	</div>
</asp:Content>
