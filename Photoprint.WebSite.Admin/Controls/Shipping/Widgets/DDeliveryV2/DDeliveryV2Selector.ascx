<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DDeliveryV2Selector.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.DDeliveryV2Selector" %>
<html>
    <head>
        <script src="https://widgets.saferoute.ru/cart/api.js?new"></script>
        <script src="/Controls/Shipping/Widgets/DDeliveryV2/js/widget.js"></script>
    </head>
    <body>
        <fieldset>
            <% if (PreparingIsSuccessful) { %>
                <div id="saferoute-cart-widget"></div>
                <input hidden name="saferouteWidgetResult" id="ddeliveryWidgetResult"/>
                <script type="text/javascript">
                            var widgetSR = new SaferouteWidget(
                                'ddeliveryWidgetResult',
                                <%=CurrentPostal?.Id ?? 0%>,
                                <%=ProductsListJson%>,
                                <%= Newtonsoft.Json.JsonConvert.SerializeObject(Math.Round(TotalWeight, 3)) %>,
                                <%=CurrentPostal?.IsShippingPricePaidSeparately.ToString().ToLower() ?? "false"%>
                            );
                            widgetSR.startWidget();
            </script>
            <% } else { %>
                <fieldset>
                    <span><%=RS.Order.Delivery.DDeliveryV2.WidgetPreparingError %></span> 
        
                </fieldset>
            <% } %>
        </fieldset>         
    </body>
</html>

