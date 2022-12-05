<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="YandexDeliverySelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.YandexDeliverySelector" %>
<%@ Import Namespace="Photoprint.WebSite.Modules" %>
<div id="yandex-delivery-container">
    <fieldset>
        <span id="error" style="display: none"></span>
        <ol>
            <li>
                <h3><%: RM.GetString(RS.Shop.Checkout.YandexDelivery.SpecifyCityHeader) %></h3>
                <ol>
                    <li>
                        <input type="text" class="text" placeholder="<%: RM.GetString(RS.Shop.Checkout.YandexDelivery.City) %>" id="txtYandexCity">
                    </li>
                    <li id="liDeliveryType" style="display: none">
                        <label for="deliveryType">Тип доставки</label>
                        <select id="deliveryType">
                            <option value="none">Не выбрано</option>
                            <option value="PICKUP">В пункт выдачи</option>
                            <option value="POST">На почту</option>
                        </select>
                    </li>
                    <li id="liDeliveryOptions" style="display: none">
                        <label for="deliveryOptions">Доступные варианты доставки</label>
                        <select id="deliveryOptions"></select>
                    </li>
                    <li id="liPickupPoints" style="display: none">
                        <label for="pickupPoints">Доступные точки выдачи</label>
                        <select id="pickupPoints"></select>
                    </li>
                </ol>
            </li>
            <li id="liPersonalData" style="display: none">
                <h3><%: RM.GetString(RS.Shop.Checkout.YandexDelivery.SpecifyRecipientHeader) %></h3>
                <h4 class="subheader"><%: RM.GetString(RS.Shop.Checkout.YandexDelivery.SpecifyRecipientSubHeader) %></h4>
                <ol>
                    <li>
                        <asp:TextBox ID="txtYandexFirstName" runat="server" ValidationGroup="Shipping" placeholder="<%$ RM: Shop.Checkout.YandexDelivery.FirstName %>" CssClass="text js-YandexFirstName" />
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqYandexFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtYandexFirstName" />
                        <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" ID="regFirstName" runat="server" ControlToValidate="txtYandexFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$" />
                    </li>
                    <li>
                        <asp:TextBox ID="txtYandexLastName" runat="server" ValidationGroup="Shipping" placeholder="<%$ RM: Shop.Checkout.YandexDelivery.LastName %>" CssClass="text js-YandexLastName" />
                        <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqYandexLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtYandexLastName" />
                        <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" ID="regLastName" runat="server" ControlToValidate="txtYandexLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$" />
                    </li>
                    <li>
                        <input type="text" id="txtYandexPhone" placeholder="<%: RM.GetString(RS.Shop.Checkout.YandexDelivery.Phone) %>">
                    </li>
                    <li>
                        <input type="button" id="btnYandexСonfirm" value="Подтвердить">
                         <span id="errorСonfirm" style="display: none"></span>
                    </li>
                </ol>
            </li>
        </ol>
    </fieldset>
</div>

<input type="hidden" name="yandexDeliveryData" id="yandexDeliveryData">
<script type="text/javascript" src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/jquery.maskedinput.js"></script>
<script type="text/javascript">
    var token = '<%:YandexDeliverySettings.Token%>';
    var senderId = '<%:YandexDeliverySettings.SenderId%>';
    var deliveryOptions = [];
    var pickupPoints = [];
    
    $(document).ready(function () {
        $('#<%= txtYandexFirstName.ClientID %>').val('<%:LoggedInUser.FirstName%>');
        $('#<%= txtYandexLastName.ClientID %>').val('<%:LoggedInUser.LastName%>');
        $("#txtYandexPhone").val('<%:LoggedInUser.GetCleanPhone()%>').mask("+7(999)999-99-99");

        var butNext = $("#ctl00_cphMainPanel_butNext");
        butNext.hide();

        $("#txtYandexCity").on('keyup', function () {
            butNext.hide();
            $(this).val().length > 2 ? $("#liDeliveryType").show(150) : $("#liDeliveryType").hide(150);
        }).on("keydown", function () {
            $("#deliveryType").val('none');
            $("#error, #liDeliveryOptions, #liPickupPoints, #liPersonalData").hide(150);
        });

        $("#deliveryType").on("change", function () {
            butNext.hide();
            $("#liDeliveryOptions, #liPickupPoints, #liPersonalData, #error").hide();
            if ($(this).val() != "none" && $("#txtYandexCity").val().trim()) {
                let deliveryOptionsRequest = {
                    Term: $("#txtYandexCity").val().trim(),
                    DeliveryType: $(this).val(),
                    Token: token,
                    SenderId: senderId
                }
                getDeliveryOptions(deliveryOptionsRequest);
            }
        });

        $("#deliveryOptions").on("change", function () {
            butNext.hide();
            let price;
            if ($(this).val() != "none") {
                getPickupPoints(deliveryOptions[$(this).val()].PickupPointIds);
                price = deliveryOptions[$(this).val()]?.Cost ?? 0;
            }
            else {
                $("#liPickupPoints").hide();
                price = 0;
            }
            changedPrice(price);
        });

        $("#pickupPoints").on("change", function () {
            butNext.hide();
            $("#liPersonalData").show(150);
        });

        $("#<%= txtYandexFirstName.ClientID %>, #<%= txtYandexLastName.ClientID %>, #txtYandexPhone").on("keyup", function () {
            $("#errorСonfirm").hide();
            butNext.hide();
        });

        $("#btnYandexСonfirm").click(function () {
            if (validateForm()) {
                saveYandexDeliveryData();
                $("#errorСonfirm").hide();
                butNext.show(150);
                $('html, body').animate({
                    scrollTop: butNext.offset().top
                }, 580);
            }
            else {
                $("#errorСonfirm").text("заполните все поля формы").show();
            }
        });
    });

    function changedPrice(price) {
                var shippingInfo = {
                    address: "",
                    addressZipCode: null,
                    shippingId: parseInt("<%: InitializedPostal?.Id ?? 0 %>"),
                    shippingPrice: parseFloat(price),
                    shippingPriceString: "",
                    shippingPricePaidSeparately: "<%= Newtonsoft.Json.JsonConvert.SerializeObject(InitializedPostal != null && InitializedPostal.IsShippingPricePaidSeparately) %>" === 'true'
                };
                $(document).trigger('onShippingChanged', shippingInfo);
    };
    function getDeliveryOptions(deliveryOptionsRequest) {
        $.ajax({
            url: "/api/shippings/yadex/getDeliveryOptions",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(deliveryOptionsRequest),
            success: function (response) {
                if (response.length) {
                    $("#deliveryOptions").find('option').remove().end().append($('<option/>').val('none').text('Не выбрано'));
                    $.each(response, function (index, value) {
                        $("<option/>", {
                            value: index,
                            text: `${value.PartnerName} (стоимость доставки: ${value.Cost} руб.)`
                        }
                        ).appendTo($("#deliveryOptions"));

                        deliveryOptions = response;
                        $("#liDeliveryOptions").show(150);
                    });
                };
            },
            error: function (response) {
                var result = JSON.parse(response.responseText);
                $("#error").text(result.Error).show(150);
                return;
            }
        })
    };
    function getPickupPoints(ids) {
        $.ajax({
            url: "/api/shippings/yadex/getPickupPoints",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ ids: ids, token: "<%=YandexDeliverySettings.Token%>" }),
            success: function (response) {
                if (response.length) {
                    pickupPoints = response;
                    $("#pickupPoints").find('option').remove().end().append($('<option/>').val('none').text('Не выбрано'));
                    $.each(response, function (index, value) {
                        $("<option/>", {
                            value: index,
                            text: value.address.addressString,
                        }
                        ).appendTo($("#pickupPoints"));
                    });
                    $("#liPickupPoints").show(150);
                };
            },
            error: function (response) {
                var result = JSON.parse(response.responseText);
                $("#error").text(result.Error).show(150);
                return;
            }
        })
    };
    function saveYandexDeliveryData() {
        let opt = deliveryOptions[$("#deliveryOptions").val()];
        $("#yandexDeliveryData").val(
            JSON.stringify({
                DeliveryType: opt.Type,
                PartnerId: opt.PartnerId,
                TariffId: opt.TariffId,
                Cost: opt.Cost,
                PickupPoint: pickupPoints[$("#pickupPoints").val()],
                FirstName: $('#<%= txtYandexFirstName.ClientID %>').val(),
                LastName: $('#<%= txtYandexLastName.ClientID %>').val(),
                Phone: $("#txtYandexPhone").val()
            }));
    };
    function validateForm() {
        if ($("#deliveryType").val() != "none" &&
            $("#deliveryOptions").val() != "none" &&
            $("#pickupPoints").val() != "none" &&
            $('#<%= txtYandexFirstName.ClientID %>').val().trim() != "" &&
            $('#<%= txtYandexLastName.ClientID %>').val().trim() != "" &&
            $("#txtYandexPhone").val().trim() != "") {
            $("#errorСonfirm").hide();
            return true;
        } 
        return false;
    };
</script>



