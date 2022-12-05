function SaferouteWidget(resultInputId, name, phoneNumber, postalId, products, totalWeight, paidSaparately) {
    var self = this;
    self.name = name;
    self.phoneNumber = phoneNumber;
    self.postalId = postalId;
    self.products = products;
    self.totalWeight = totalWeight;
    self.resultInputId = resultInputId;
    self.paidSaparately = paidSaparately;

    self.startWidget = function () {
        var widget = new SafeRouteCartWidget("saferoute-cart-widget", {
            apiScript: self.getApiScript(),
            userFullName: self.name,
            userPhone: self.phoneNumber,
            products: self.products,
            weight: self.totalWeight
        });
        var shippingInfo;
        widget.on("done", function (response) {
            $(document).trigger('onShippingChoosed', shippingInfo);
        });
        widget.on("error", function (errors) {});
        widget.on("change", function (data) {
            var input = document.getElementById(self.resultInputId);
            if (input !== undefined && input !== null) {
                input.value = JSON.stringify(data);
                var price = data.delivery != null ? data.delivery.totalPrice : 0;
                shippingInfo = {
                    address: "",
                    addressZipCode: null,
                    shippingId: self.postalId,
                    shippingPrice: price,
                    shippingPriceString: "",
                    shippingPricePaidSeparately: self.paidSaparately
                };
                
            }
        });
    }

    self.getApiScript = function() {
        return "/api/shippings/ddeliveryV2?postalId=" + self.postalId;
    }
}