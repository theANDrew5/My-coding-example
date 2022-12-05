<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DDeliverySelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.DDeliverySelector" %>
<%@ Import Namespace="Photoprint.WebSite.Modules" %>

<script src="/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/js/ddelivery.js?"></script>
<div id="ddeliveryWrapper" data-bind="visible: true" style="display: none">
    <div data-bind="visible: selectorVisible">
        <h2 class="main">Выберите способ доставки</h2>
        <div class="row">
            <div class="col-md-12">
                 <% if (Settings.ShowSelectorInModalWindow) {%>
                    <button type="button" data-bind="visible: type() == 0, click: selectCompany">выбрать службу доставки</button>
                <%} else { %>
                    <div id="ddelivery_cover">
                        <div id="ddelivery_container"></div>
                    </div>
                <% }%>
            </div>
        </div>
    </div>
    
    <div id="companyInfo" data-bind="visible: company() != null">
        <h2 class="main" data-bind="text: selectedCompanyName"></h2>
        <div class="row">
            <div class="col-md-4">
                <fieldset>
                    <ol class="ddelivery-courier">
                        <li>
                            <label>>Служба доставки <span class="note">(<a href="#" class="semilink" data-bind="click: selectCompany">изменить</a>)</span>:</label>
                            <span data-bind="text: selectedCompanyName"></span>
                        </li>
                        <li>
                            <label>Город доставки:</label>
                            <span data-bind="text: cityTitle"></span>
                        </li>
                        <li>
                            <label>Стоимость доставки:</label>
                            <span data-bind="text: selectedCompanyPrice"></span>
                        </li>
                    </ol>
                </fieldset>
            </div>
            <div class="col-md-8">
                <fieldset class="">
                    <ol>
                        <li class="shipping-first-name">
                            <label for="<%=txtFirstName.ClientID%>"><%=RM.GetString("Shop.Checkout.FirstNameLabel")%></label>
                            <asp:TextBox ID="txtFirstName" runat="server" ValidationGroup="Shipping" CssClass="text" />                     
                            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
                            <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regFirstName" Runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                        </li>
                        <li class="shipping-last-name">
                            <label for="<%=txtLastName.ClientID%>"><%=RM.GetString("Shop.Checkout.LastNameLabel")%></label>
                            <asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />                      
                            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
                            <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regLastName" Runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                        </li>  
                        <li class="shipping-phone">
                            <label for="<%=txtPhone.ClientID%>"><%=RM.GetString("Shop.Checkout.Phone")%></label>
                            <asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />
                            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
                        </li>    
                        <li class="shipping-postal-code" data-bind="visible: type() == 2">
                            <label for="<%=txtPostalCode.ClientID %>"><%=RM.GetString("Shop.Checkout.PostalCode")%></label>
                            <asp:TextBox ID="txtPostalCode" runat="server" CssClass="text postalcode" />
                            <asp:CustomValidator ClientValidationFunction="ddeliverySelectorModel.validateIndexField" Display="Dynamic" ErrorMessage="<%$ RM: Common.EmptyField %>" ValidateEmptyText="True" CssClass="validator" ID="reqIndex" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPostalCode"></asp:CustomValidator>
                        </li>
                        <li class="shipping-street" data-bind="visible: type() == 2">
                            <label for="<%=txtStreet.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressStreet)%></label>
                            <asp:TextBox ID="txtStreet" runat="server" ValidationGroup="Shipping" CssClass="text" />
                            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqStreet" ValidationGroup="Shipping" runat="server" ControlToValidate="txtStreet" />
                        </li>
                        <li class="shipping-house" data-bind="visible: type() == 2">
                            <label for="<%=txtHouse.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressHouse)%></label>
                            <asp:TextBox ID="txtHouse" runat="server" ValidationGroup="Shipping" CssClass="text" />
                            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqHouse" ValidationGroup="Shipping" runat="server" ControlToValidate="txtHouse" />
                        </li>
                        <li class="shipping-flat" data-bind="visible: type() == 2">
                            <label for="<%=txtFlat.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.AddressFlat)%></label>
                            <asp:TextBox ID="txtFlat" runat="server" ValidationGroup="Shipping" CssClass="text" />
                            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFlat" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFlat" />
                        </li>
                    </ol>
                </fieldset>
            </div>
        </div>
    </div>
    <input type="hidden" name="selectedCityId" data-bind="value: ko.toJSON(cityId)"/>
    <input type="hidden" name="selectedCompany" data-bind="value: ko.toJSON(company)"/>
    <input type="hidden" id="pointSelected" name="selectedPoint"  data-bind="value: ko.toJSON(point)"/>
</div>

<script type="text/javascript">
    var useModalWindow = <%= Newtonsoft.Json.JsonConvert.SerializeObject(Settings.ShowSelectorInModalWindow)%>;

    function DDeliverySelectorModel() {
        var self = this;
        self.type = ko.observable(0); // 0: null, 1: point, 2: courier
        self.cityId = ko.observable();
        self.cityTitle = ko.observable();
        self.indexValEnabled = ko.observable("<%:InitializedPostal.IsIndexRequired%>" === "True");
        self.company = ko.observable();
        self.point = ko.observable();
        self.selectorVisible = ko.computed(function() {
            return !useModalWindow || self.company() == null;
        });

        self.selectedCompanyName = ko.computed(function() {
            var c = self.company();
            return c != null ? c.DeliveryCompanyName : "не выбрано";
        });
        self.selectedCompanyPrice = ko.computed(function () {
            var c = self.company();
            return c != null ? c.TotalPriceCorrected.toString().replace('.', ',') + ' руб.' : "-";
        });

        self.validateIndexField = function (source, args) {
            var result = self.indexValEnabled() || !!args.Value.trim() || !!self.point();
            args.IsValid = result === true;
        };

        self.updateValidatorsState = function () {
            var state = self.type() === 2;

            var el = document.getElementById('<%= reqIndex.ClientID %>');
            if (el != null) ValidatorEnable(el, state);

            el = document.getElementById('<%= reqHouse.ClientID %>');
            if (el != null) ValidatorEnable(el, state);
            el = document.getElementById('<%= reqFlat.ClientID %>');
            if (el != null) ValidatorEnable(el, state);
            el = document.getElementById('<%= reqStreet.ClientID %>');
            if (el != null) ValidatorEnable(el, state);
        }

        self.selectCourier = function (data) {
            self.cityId(data.cityId);
            self.cityTitle(data.cityTitle);
            self.company(data.company);
            self.point(null);
            self.type(2);

            self.updateValidatorsState();
            self.onShippingChanged();
        }
        self.selectPoint = function (data) {
            self.cityId(data.cityId);
            self.cityTitle(data.cityTitle);
            self.company(data.company);
            self.point(data.point);
            self.type(1);

            self.updateValidatorsState();
            self.onShippingChanged();
        }

        self.onShippingChanged = function() {
            if (self.company() == null) {
                $(document).trigger('onShippingChanged', null);
                return;
            }
            var shippingInfo = {
                address: "",
                addressZipCode: null,
                shippingId: parseInt("<%: InitializedPostal != null ? InitializedPostal.Id : 0 %>"),
                shippingTitle: self.company().DeliveryCompanyName,
                shippingPrice: parseFloat(self.company().TotalPriceCorrected),
                shippingPriceString: "",
                shippingPricePaidSeparately: "<%= Newtonsoft.Json.JsonConvert.SerializeObject(InitializedPostal != null && InitializedPostal.IsShippingPricePaidSeparately) %>" === 'true'
            };
            $(document).trigger('onShippingChanged', shippingInfo);
        }

        self.selectCompany = function() {
            self.type(0);
            self.company(null);
            self.onShippingChanged();
            DDeliveryIntegration.openPopup();
        }
    }

    var ddeliverySelectorModel = new DDeliverySelectorModel();
    $(document).ready(function() {
        ko.applyBindings(ddeliverySelectorModel, $('#ddeliveryWrapper')[0]);
    });

    var topWindow = parent;
    while (topWindow != topWindow.parent) topWindow = topWindow.parent;
    if (typeof (topWindow.DDeliveryIntegration) == 'undefined')
        topWindow.DDeliveryIntegration = (function () {
            var th = {};
            var status = 'Выберите условия доставки';
            th.getStatus = function () { return status; };

            function afterSelect() {
                if (useModalWindow) {
                    document.body.removeChild(document.getElementById('ddelivery_cover'));
                    document.getElementsByTagName('body')[0].style.overflow = "";
                } else {
                    $('html, body').animate({
                        scrollTop: $("#companyInfo").offset().top
                    }, 500);
                }
            }

            function createIframeContainer() {
                var cover = document.createElement('div');
                cover.id = 'ddelivery_cover';
                cover.appendChild(div);
                document.body.appendChild(cover);
                document.getElementById('ddelivery_container').style.display = 'block';
                document.body.style.overflow = 'hidden';
            }

            th.openPopup = function () {
                if (useModalWindow) {
                    createIframeContainer();
                }

                var callback = {
                    courierChange: function (data) {
                        ddeliverySelectorModel.selectCourier(data);
                        afterSelect();
                    },
                    mapPointChange: function (data) {
                        ddeliverySelectorModel.selectPoint(data);
                        afterSelect();
                    },
                    close: function (data) {
                        afterSelect();
                    }
                };

                var params = {
                    city_id: ddeliverySelectorModel.cityId(),
                    hideCloseButton: !useModalWindow
                };

                DDelivery.delivery('ddelivery_container', '<%= new UrlManager().GetHRefUrl("DDeliveryShippingIframe") %>?sid=<%: InitializedPostal.Id %>', params, callback);

                if (!useModalWindow) {
                    $('html, body').animate({
                        scrollTop: $("#ddelivery_cover").offset().top
                    }, 500);
                }

                return void (0);
            };
            if (useModalWindow) {
                var style = document.createElement('STYLE');
                style.innerHTML =
                    " #delivery_info_ddelivery_all a{display: none;} " +
                    "#ddelivery_container { overflow:hidden;background: #eee;width: 1000px; margin: 0px auto;padding: 0px; }" +
                    "#ddelivery_cover > * {-webkit-transform: translateZ(0px);}" +
                    "#ddelivery_cover {zoom: 1;z-index:9999;position: fixed;bottom: 0;left: 0;top: 0;right: 0; overflow: auto;-webkit-overflow-scrolling: touch;background-color: #000; background: rgba(0, 0, 0, 0.5); filter: progid:DXImageTransform.Microsoft.gradient(startColorstr = #7F000000, endColorstr = #7F000000); "

                var body = document.getElementsByTagName('body')[0];
                body.appendChild(style);
                var div = document.createElement('div');
                div.id = 'ddelivery_container';
                body.appendChild(div);
            }
            
            return th;
        })();
    var DDeliveryIntegration = topWindow.DDeliveryIntegration;
    if (!useModalWindow) {
        DDeliveryIntegration.openPopup();
    }
</script>