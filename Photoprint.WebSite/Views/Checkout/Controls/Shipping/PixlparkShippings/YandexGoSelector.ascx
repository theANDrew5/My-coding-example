<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YandexGoSelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.YandexGoSelector" %>
<%= YandexMapScript %>
<div>
    <fieldset>
        <h1>YandexGo</h1>
        <h3><%: RM.GetString("Shop.Checkout.YandexGo.SelectBuilding") %></h3>
        <div id="map" style="display: flex; height: 500px"></div>
        <h4 id="estimateResult"></h4>
    </fieldset>
    <fieldset>
        <ol>
            <li class="shipping-first-name">
                <label for="<%=txtFirstName.ClientID%>"><%=RM.GetString("Shop.Checkout.FirstNameLabel")%></label>
                <asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="Shipping" CssClass="text" />
                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
                <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$" />
            </li>
            <li class="shipping-last-name">
                <label for="<%=txtLastName.ClientID%>"><%=RM.GetString("Shop.Checkout.LastNameLabel")%></label>
                <asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />
                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
                <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.LastNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$" />
            </li>
            <li class="shipping-phone">
                <label for="<%=txtPhone.ClientID%>"><%=RM.GetString("Shop.Checkout.Phone")%></label>
                <asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />
                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
            </li>
            <li>
                <input type="button" id="btnYandexGoСonfirm" value="<%: RM.GetString("Shop.Checkout.YandexGo.Confirm") %>">
                <span id="errorСonfirm" style="display: none"></span>
            </li>
        </ol>
    </fieldset>
</div>
<input type="hidden" name="yandexGoDeliveryData" id="yandexGoDeliveryData">
<script type="text/javascript">
    var selectedAddress;
    var coordinate;
    var deliveryPrice;

    ymaps.ready(init);

    function init() {
        $("#ctl00_cphMainPanel_butNext").hide();
        var geolocation = ymaps.geolocation;
        var myPlacemark;

        var inputSearch = new ymaps.control.SearchControl({
            options: { size: 'large' }
        });

        var myMap = new ymaps.Map("map", {
            center: [55.76, 37.64],
            zoom: 10,
            controls: ['geolocationControl', 'zoomControl', inputSearch]
        });

        geolocation.get({
            provider: 'auto',
            mapStateAutoApply: true
        }).then(function (result) {
            myMap.geoObjects.add(result.geoObjects);
        });

        myMap.events.add('click', function (e) {
            var coords = e.get('coords');
            $("#ctl00_cphMainPanel_butNext").hide();
            if (myPlacemark) {
                myPlacemark.geometry.setCoordinates(coords);
            }
            else {
                myPlacemark = createPlacemark(coords);
                myMap.geoObjects.add(myPlacemark);
                myPlacemark.events.add('dragend', function () {
                    getAddress(myPlacemark.geometry.getCoordinates());
                });
            }
            getAddress(coords);
        });

        // Создание метки.
        function createPlacemark(coords) {
            return new ymaps.Placemark(coords, {
                iconCaption: 'поиск...'
            }, {
                preset: 'islands#violetDotIconWithCaption',
                draggable: true
            });
        }

        function getAddress(coords) {
            myPlacemark.properties.set('iconCaption', 'поиск...');
            ymaps.geocode(coords).then(function (res) {
                var firstGeoObject = res.geoObjects.get(0);
                importData(coords, getImportAddress(firstGeoObject), firstGeoObject.getPremiseNumber());
                myPlacemark.properties
                    .set({
                        iconCaption: [
                            firstGeoObject.getLocalities().length ? firstGeoObject.getLocalities() : firstGeoObject.getAdministrativeAreas(),
                            firstGeoObject.getThoroughfare() || firstGeoObject.getPremise()
                        ].filter(Boolean).join(', '),
                        balloonContent: firstGeoObject.getAddressLine()
                    });
            });
        }
    }

    $("#btnYandexGoСonfirm").click(function () {
        saveYandexGoData();
        $("#ctl00_cphMainPanel_butNext").show();
        $("#errorСonfirm").hide();
        $('html, body').animate({
            scrollTop: $("#ctl00_cphMainPanel_butNext").offset().top
        }, 580);
    });
    
    function getImportAddress(geoObject) {
        let fullAddress = geoObject.getAddressLine();
        let separatedAddress = fullAddress.split(', ');
        return {
            Country: geoObject.getCountry(),
            Region: geoObject.getAdministrativeAreas()[0],
            City: geoObject.getLocalities()[0],
            Street: separatedAddress[separatedAddress.length - 2],
            House: separatedAddress[separatedAddress.length - 1],
            FullAddress: fullAddress
        }
    }

    function importData(coord, address, premiseNumber) {
        console.log("premiseNumber", premiseNumber);
        if (premiseNumber == undefined) {
            $("#liPersonalData").hide();
            return;
        };
        coordinate = coord;
        selectedAddress = address;
        var request = {
            PhotolabId: <%= CurrentPhotolab.Id %>,
            ShippingId: <%= CurrentPostal.Id %>,
            UserId: <%= LoggedInUser.Id %>,
            Country: selectedAddress.Country,
            Region: selectedAddress.Region,
            City: selectedAddress.City,
            Longitude: coordinate[1],
            Latitude: coordinate[0]
        };
        estimate(request);
        $("#liPersonalData").show(200);
    }

    function saveYandexGoData() {
        var phone = document.querySelector('#ctl00_cphMainPanel_ctl00_ctl00_txtPhone').value;
        phone = phone.replace(/[^0-9]/g, '');
       
        $("#yandexGoDeliveryData").val(
            JSON.stringify({
                Address: selectedAddress,
                Longitude: coordinate[1],
                Latitude: coordinate[0],
                Cost: deliveryPrice,
                FirstName: $('#<%= txtFirstName.ClientID %>').val(),
                LastName: $('#<%= txtLastName.ClientID %>').val(),
                Phone: '+' + phone
            }));
    };

    function estimate(request) {
        $.ajax({
            url: "/api/shippings/yandexGo/checkPrice",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (response) {
                console.log("response", response)
                deliveryPrice = response.Price;
                var shippingInfo = {
                    shippingPrice: parseFloat(deliveryPrice)
                };
                $(document).trigger('onShippingChanged', shippingInfo);
                $('#estimateResult').html(`<br><%: RM.GetString("Shop.Checkout.YandexGo.Address") %> ${selectedAddress.FullAddress}</br><br> <%: RM.GetString("Shop.Checkout.YandexGo.DeliveryPrice") %> ${deliveryPrice} </br><br> <%: RM.GetString("Shop.Checkout.YandexGo.Distance") %> ${parseFloat(response.DistanceMeters).toFixed(2)} <%: RM.GetString("Shop.Checkout.YandexGo.Meters") %></br>`)
            },
            error: function (response) {
                return;
            }
        })
    };
</script>
