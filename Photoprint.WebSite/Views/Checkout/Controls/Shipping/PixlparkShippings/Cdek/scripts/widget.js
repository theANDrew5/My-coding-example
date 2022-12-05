function CDEKWidget(resultInputId, courierId, postalId, languageId, cityFrom, cityTo, goods, hideProfiles, paidSeparately, yandexKey, photolabId, userId) {
    var self = this;
    self.resultInputId = resultInputId;
    self.courierPostalId = courierId;
    self.postalId = postalId;
    self.languageId = languageId;
    self.cityFrom = cityFrom;
    self.cityTo = cityTo;
    self.goods = goods;
    self.hideProfiles = hideProfiles;
    self.paidSeparately = paidSeparately;
    self.photolabId = photolabId;
    self.userId = userId;

    self.startWidget = function (templatePath) {
        var widget = new ISDEKWidjet({
            hideMessages: true,
            hidedelt: self.hideProfiles,
            defaultCity: self.cityTo !== null ? self.cityTo : "auto",
            cityFrom: self.cityFrom,
            link: "cdek-widget",
            path: "https://www.cdek.ru/website/edostavka/template/scripts/",
            templatepath: templatePath,
            servicepath: self.getApiScript(),
            goods: self.goods,
            onChoose: self.choosePVZ,
            yandexKey: yandexKey
        });

        document.getElementById("aspnetForm").onsubmit = function () { return false; }
    }

    self.choosePVZ = function (wat) {
        debugger;
        var input = document.getElementById(self.resultInputId);
        if (input !== undefined && input !== null) {
            var request = {
                PhotolabId: self.photolabId,
                ShippingId: self.postalId = postalId,
                Country: "Россия",
                City: wat.cityName,
                Address: wat.PVZ.Address,
                UserId: self.userId,
            };
            $.ajax({
                url: "/api/shippings/getcalculatedprice",
                type: "POST", contentType: "application/json; charset=utf-8",
                data: JSON.stringify(request),
                success: function (response) {
                    if (!response.success) throw("Delivery price calculation error");
                    let cdekProps = JSON.parse(response.properties).cdekAddressInfo;
                    input.value = JSON.stringify({
                        id: wat.id,
                        city: wat.city,
                        cityName: wat.cityName,
                        price: response.price,
                        term: wat.term,
                        tarif: cdekProps.Tariff,
                        PVZ: {
                            Name: wat.PVZ.Name,
                            WorkTime: wat.PVZ.WorkTime,
                            Address: wat.PVZ.Address,
                            Phone: wat.PVZ.Phone,
                            Note: wat.PVZ.Note,
                            cX: wat.PVZ.cX,
                            cY: wat.PVZ.cY
                        }
                    });
                    var shippingInfo = {
                        shippingPrice: response.price,
                        shippingPriceString: "",
                        shippingPricePaidSeparately: self.paidSeparately,
                        city: wat.cityName,
                        address: wat.PVZ.Address
                    };
                    $(document).trigger('onShippingChanged', shippingInfo);
                },
                complete: function () {
                    var comment = document.getElementById("addOrderComment");
                    if (comment === undefined || comment === null) {
                        var cart = document.getElementById("order_content");
                        if (cart !== undefined && cart !== null) {
                            $("html, body").animate({ scrollTop: cart.offsetTop - 50 }, "slow");
                        }
                    } else {
                        $("html, body").animate({ scrollTop: comment.offsetTop - 50 }, "slow");
                    }
                },
                error: function (xhr, textStatus, error) {
                    console.log("error", error);
                }
            });
        }
    }

    self.getApiScript = function () {
        if (self.courierPostalId === null)
            return "/api/shippings/cdek?postalId=" + self.postalId + "&languageId=" + self.languageId;
        return "/api/shippings/cdek?courierId=" + self.courierPostalId + "&postalId=" + self.postalId + "&languageId=" + self.languageId;
    }
}