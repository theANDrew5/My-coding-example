<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DDeliveryV2Selector.ascx.cs" Inherits="Photoprint.WebSite.Controls.DDeliveryV2Selector" %>

<% if (PreparingIsSuccessful) { %>
<html>
    <head>
        <script src="https://widgets.saferoute.ru/cart/api.js?new"></script>
        <script src="<%= FileHelper.JS.DdeliveryWidgetUrl %>"></script>
    </head>
    <body>
        <fieldset>
            <div id="saferoute-cart-widget"></div>
            <input hidden name="saferouteWidgetResult" id="saferouteWidgetResult"/>
            <script type="text/javascript">
                var widgetSR = new SaferouteWidget(
                    'saferouteWidgetResult',
                    <%= Newtonsoft.Json.JsonConvert.SerializeObject(LoggedInUser.FullName) %>,
                    <%= Newtonsoft.Json.JsonConvert.SerializeObject(LoggedInUser.GetCleanPhone()) %>,
                    <%= CurrentPostal.Id %>,
                    <%= ProductsListJson %>,
                    <%= Newtonsoft.Json.JsonConvert.SerializeObject(Math.Round(TotalWeight,3)) %>,
                    <%= CurrentPostal.IsShippingPricePaidSeparately.ToString().ToLower() %>                    
                );
                widgetSR.startWidget();

                $(document).on('onShippingChoosed', function (e, shippingInfo) {
                    var request = { PhotolabId: <%= CurrentPhotolab.Id %>, ShippingId: <%= CurrentPostal.Id %>, UserId: <%= LoggedInUser.Id %>, Price: shippingInfo.shippingPrice };
                    $.ajax({
                        url: "/api/shippings/getcalculatedprice",
                        type: "POST", contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(request),
                        success: function (response) {
                            shippingInfo.shippingPrice = response;
                            $(document).trigger('onShippingChanged', shippingInfo);
                        },
                        complete: function() {
                            $(document).trigger('onShippingChanged', shippingInfo);

                            var comment = document.getElementById("addOrderComment");
                            if (comment === undefined || comment === null) {
                                var cart = document.getElementById("order_content");
                                if (cart !== undefined && cart !== null) {
                                    $("html, body").animate({ scrollTop: cart.offsetTop - 50 }, "slow");
                                }
                            } else {
                                $("html, body").animate({ scrollTop: comment.offsetTop - 50 }, "slow");
                            }
                        }
                    });
                });
            
            </script>
        </fieldset>
    </body>
</html>
<% } else { %>
    <fieldset>
       <span><%=RM.GetString(RS.Shop.Checkout.DDeliveryV2.WidgetPreparingError)%></span> 
    </fieldset>
<% } %>
