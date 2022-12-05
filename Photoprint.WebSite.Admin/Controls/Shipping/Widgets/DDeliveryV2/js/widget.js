function SaferouteWidget(resultInputId, postalId, products, totalWeight, paidSaparately) {
    var self = this;
    self.postalId = postalId;
    self.products = products;
    self.totalWeight = totalWeight;
    self.resultInputId = resultInputId;
    self.paidSaparately = paidSaparately;

    self.startWidget = function () {
        var widget = new SafeRouteCartWidget("saferoute-cart-widget", {
            apiScript: self.getApiScript(),
            products: self.products,
            weight: self.totalWeight
        });

        widget.on("change", function (data) {
            var input = document.getElementById(self.resultInputId);
            if (input !== undefined && input !== null) {
                input.value = JSON.stringify({
                    city: data.city,
                    delivery: data.delivery,
                    deliveryDate: data.deliveryDate,
                    contacts: data.contacts,
                    comment: data.comment});
            }

            var price = document.getElementById("txtDeliveryPrice");
            if (price !== undefined && price !== null) {
                var tprice;
                if (data.delivery.type == 1) {
                    tprice = data.delivery.totalPrice;
                    price.value = tprice === undefined ? 0 : tprice;
                } else {
                    if (data.delivery.type == 2 || data.delivery.type == 3) {
                        tprice = data.delivery.totalPrice;
                        price.value = tprice === undefined ? 0 : tprice;
                    }
                }
            }});
        widget.on("error", function (errors) {});
        widget.on("done", function (response) {
            $("html, body").animate({ scrollTop: 9999 }, "slow");
        });
    }

    self.getApiScript = function() {
        return "/api/shippings/ddeliveryV2?postalId=" + self.postalId;
    }
}