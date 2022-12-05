<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ShippingProviderInfo.ascx.cs" Inherits="Photoprint.WebSite.Admin.Controls.ShippingProviderInfo" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Photoprint.WebSite.Admin.Controls" %>
<%@ Import Namespace="Photoprint.Core" %>

<%@ Register Src="~/Views/Shipping/Controls/ExgarantRegisterController.ascx" TagPrefix="pr" TagName="ExgarantRegisterController" %>
<asp:PlaceHolder ID="plhdSuccessMessage" Visible="false" runat="server">
    <div class="message success">
        <p>
            <asp:Literal ID="litSuccessMessage" runat="server" />
        </p>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhdErrorMessage" Visible="false" runat="server">
    <div class="message error">
        <p>
            <asp:Literal ID="litErrorMessage" runat="server" />
        </p>
    </div>
</asp:PlaceHolder>

<% if (CurrentOrder != null && !(CurrentOrder.DeliveryAddress is null))
    { %>
        <% var postal = GetOrderPostal(); %>
        <% if (postal != null)
            { %>
                <% var ddeliverySettings = postal.ServiceProviderSettings as DDeliveryServiceProviderSettings; %>
                <% if (ddeliverySettings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div>
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.OrderNumberInDDelivery) %> <a target="_blank" href="https://ddelivery.ru/cabinet/orders/<%: CurrentOrder.TrackingNumber %>/view"><%: CurrentOrder.TrackingNumber %></a>
                </div>
                <% } %>
                <%
                    var ddeliverInfo = CurrentOrder?.DeliveryAddress?.DeliveryProperties?.DDeliveryAddressInfo != null
                        ? CurrentOrder.DeliveryAddress.DeliveryProperties.DDeliveryAddressInfo.Result
                        : null;
                %>
                <% if (ddeliverInfo != null)
                    { %>
                <div style="padding: 10px 0 0 0">
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.FullCost) %> <%: ddeliverInfo.TotalPrice %>
                </div>
                <div>
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.FenceCost) %> <%: ddeliverInfo.PickupPrice %>
                </div>
                <div>
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.AdjustedPrice) %> <%: ddeliverInfo.TotalPriceCorrected %>
                </div>
                <% } %>
                <div class="buttons">
                    <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                        { %>
                    <asp:Button CssClass="small" runat="server" ID="butDDeliveryRegister" OnClick="DDeliveryRegisterOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.SendOrderToDDelivery %>" />
                    <% } %>
                </div>
                <% } %>

                <% var ddeliveryV2Settings = postal.ServiceProviderSettings as DDeliveryV2ServiceProviderSettings; %>
                <% if (ddeliveryV2Settings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div>
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.OrderNumberInSafeRoute) %> <a target="_blank" href="https://cabinet.saferoute.ru/cabinet/orders/<%: CurrentOrder.TrackingNumber %>/view"><%: CurrentOrder.TrackingNumber %></a>
                </div>
                <div style="padding: 20px 0 20px;">
                    <a target="_blank" href="/content/shipping-document/ddeliveryV2/<%= CurrentOrder.Id %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadOrderReceipt) %></a>
                </div>
                <div class="buttons">
                    <asp:Button CssClass="small delete" runat="server" OnClick="DDeliveryV2CancelOrder" Text="<%$ RM: Shipping.ShippingProviderInfo.CancelTheApplication %>" />
                </div>
                <% }
                    else
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" OnClick="DDeliveryV2RegisterOrder" Text="<%$ RM: Shipping.ShippingProviderInfo.SendOrderToSafeRoute %>" />
                </div>
                <% } %>

                <% } %>

        <% if (postal.ServiceProviderSettings is CdekServiceProviderSettings cdekSettings && 
               !(CurrentOrder.DeliveryAddress.DeliveryProperties?.CdekAddressInfo is null))
           { 
                var cdekProperties = CurrentOrder.DeliveryAddress.DeliveryProperties.CdekAddressInfo;    
                if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div style="padding: 0 0 20px;">
                    <a target="_blank" href="/content/shipping-document/cdek/<%= CurrentOrder.Id %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadOrderReceipt) %></a>
                </div>
                <div style="padding: 0 0 20px;">
                    <a target="_blank" href="/content/shipping-document/cdek/<%= CurrentOrder.Id %>/barCode"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadOrderBarcode) %></a>
                </div>
                
                <% }
                else
                {%>
                <span>Код тарифа:</span>
                <input id="txtCdekTariff" name="txtCdekTariff" type="text" class="text" style="display: none" readonly/>
                <select id="selCdekTariff" name="selCdekTariff" style="display: none">
                    <option value="" selected disabled hidden>Выберите тариф</option>
                </select>
                <button class="small" id="cdekChageTeriff" style="display: none">Изменить тариф</button>
                <script>
                    const tariffCodesMoreThanOne = JSON.parse('<%:cdekSettings.TariffCodesMoreThanOne%>'.toLowerCase());
                    const propTariff = '<%:cdekProperties.Tariff%>';
                    const settingsTariff = '<%: cdekSettings.TariffTypeCode%>';

                    const select = document.querySelector("#selCdekTariff");
                    const input = document.querySelector("#txtCdekTariff");
                    
                    const elementsRender = () => {
                        if (tariffCodesMoreThanOne) {
                            select.style.display = 'block';
                            settingsTariff.split(',').forEach((splited) => {
                                let tariff = splited.trim();
                                let option =new Option(tariff, tariff);
                                if (propTariff === tariff)
                                    option.selected = true;
                                select.appendChild(option);
                            });
                        } else {
                            input.value = settingsTariff;
                            input.style.display = 'block';
                        }
                    }
                    elementsRender();                          
                </script>
                <%} %>

                <div class="buttons">
                    <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                        { %>
                    <button class="small" id="cdekRegisterOrder"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.RegisterInSDEK) %></button>
                    <div class="modal-box" id="cdekRegisterModalBox">
                        <h4><%= RM.GetString(RS.Shipping.ShippingProviderInfo.RegistrationInSDEK) %></h4>
                        <fieldset>
                            <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.Register) %></label>
                            <select name="cdekRegisterType">
                                <option value="1"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.OnlyCurrentOrder) %></option>
                                <option value="2"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.TheCurrentOrderAndEverythingRelatedToIt) %></option>
                            </select>
                        </fieldset>
                        <div class="buttons">
                            <asp:Button CssClass="small" runat="server" ID="butCdekRegisterOrder" OnClick="CdekRegisterOrderClick" Text="<%$ RM: Order.Info.CdekGetNumber %>" />
                        </div>
                    </div>
                    <script type="text/javascript">
                        $('#cdekRegisterOrder').click(function () { $('#cdekRegisterModalBox').jqmShow(); return false; });

                    </script>
                    <% }
                        else
                        { %>
                    <asp:Button OnClientClick="<%$ RM: General.ConfirmDeleteJavaScript %>" CssClass="small delete" runat="server" ID="butCdekDeleteOrder" OnClick="CdekDeleteOrderClick" Text="<%$ RM: Order.Info.CdekDeleteNumber %>" />
                    <% if (IsEnabledCdekTariff(cdekSettings.TariffTypeCode) && string.IsNullOrEmpty(CurrentOrder.Properties.CalledCourierNumber))
                        { %>
                    <button class="small" id="registerCurrier" style="margin: 5px"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.CallACourier) %></button>
                    <div class="modal-box" id="registerCurrierModalBox">
                        <h4><%= RM.GetString(RS.Shipping.ShippingProviderInfo.CallACourier) %></h4>
                        <fieldset>
                            <style>
                                fieldset .ab select {
                                    min-width: 0;
                                    width: 100px;
                                }

                                fieldset label {
                                    padding: 5px 0 3px 5px;
                                }
                            </style>
                            <div id="dpointsList">
                                <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.PointOfIssue) %></label>
                                <select style="margin: 0 5px" id="dpoints" name="courier-dpoint"
                                    data-bind="options: dpoints, optionsText: 'Title', value: currentDPoint">
                                </select>
                                <div data-bind="visible: currentDPointIsDefined">
                                    <label data-bind="text: getText()"></label>
                                </div>
                            </div>
                            <div id="usersList">
                                <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.Employee) %></label>
                                <select style="margin: 0 5px" id="crew" name="courier-crew"
                                    data-bind="options: users, optionsText: 'Name', value: currentUser">
                                </select>
                                <div data-bind="visible: currentUserIsDefined">
                                    <label data-bind="text: getText()"></label>
                                </div>
                            </div>
                            <div class="ab">
                                <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.Date) %></label>
                                <%= DateInfoController.GetDropdownListsHtml("when-date", DateTime.Today + TimeSpan.FromDays(1))%>
                            </div>
                            <div class="ab">
                                <div class="ab-a">
                                    <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.SinceWhen) %></label>
                                    <%= TimeInfoController.GetDropdownListsHtml("start-time", DateTime.UtcNow, 9, 15, 10) %>
                                </div>
                                <div class="ab-b">
                                    <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.HowLong) %></label>
                                    <%= TimeInfoController.GetDropdownListsHtml("end-time", DateTime.UtcNow, 12, 18, 10) %>
                                </div>
                                <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.TheTimePeriodMustBeAtLeast) %></label>
                            </div>
                            <div style="width: 99%; margin: 0 5px;">
                                <label><%= RM.GetString(RS.Shipping.Comment) %></label>
                                <input type="text" id="txtCommentary" class="text" />
                            </div>
                            <div id="errorWindow" data-bind="visible: isVisible">
                                <div class="message error" data-bind="html: text"></div>
                            </div>
                        </fieldset>
                        <div class="buttons">
                            <button type="button" id="validationButton" onclick="check(); return;"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.RegisterAnApplication) %></button>
                        </div>
                    </div>
                    <script type="text/javascript">
                        $('#registerCurrier').click(function () { $('#registerCurrierModalBox').jqmShow(); return false; });

                        function DPointsList(dpoints) {
                            var self = this;

                            self.dpoints = ko.observableArray(dpoints);
                            self.currentDPoint = ko.observable();
                            self.currentDPointIsDefined = ko.computed(function () {
                                var curDPoint = self.currentDPoint();
                                return typeof curDPoint !== "undefined" && curDPoint !== null;
                            });
                            self.getText = ko.computed(function () {
                                var curDPoint = self.currentDPoint();
                                if (self.currentDPointIsDefined()) {
                                    return `<%= RM.GetString(RS.Shipping.ShippingProviderInfo.Street) %>${curDPoint.Street}<%= RM.GetString(RS.Shipping.ShippingProviderInfo.Home) %>${curDPoint.House}<%= RM.GetString(RS.Shipping.ShippingProviderInfo.Office) %>${curDPoint.Flat}`
                                    return "";
                                }
                            });
                        }
                        var dpointsView = new DPointsList(<%= Newtonsoft.Json.JsonConvert.SerializeObject(DPoints.Select(x => new { Id = x.Id, Title = x.AdminTitle, Street = x.Address.Street, House = x.Address.House, Flat = x.Address.Flat}).AsList()) %>);
                        ko.applyBindings(dpointsView, document.getElementById("dpointsList"));

                        function UsersList(users) {
                            var self = this;

                            self.users = ko.observableArray(users);
                            self.currentUser = ko.observable();
                            self.currentUserIsDefined = ko.computed(function () {
                                var curuser = self.currentUser();
                                return typeof curuser !== "undefined" && curuser !== null;
                            });
                            self.getText = ko.computed(function () {
                                var curUser = self.currentUser();
                                if (self.currentUserIsDefined() && (typeof curUser.Phone !== "undefined" && curUser.Phone !== null)) {
                                    return "<%= RM.GetString(RS.Shipping.ShippingProviderInfo.Phone) %>" + curUser.Phone;
                                }
                                return "<%= RM.GetString(RS.Shipping.ShippingProviderInfo.PhoneNumberNotProvided) %>";
                            });
                            $.each(self.users, function (i, e) {
                                if (e.Id == <%= LoggedInUser.Id %>) {
                                    self.currentUser(e);
                                    return;
                                }
                            });
                        }
                        var usersView = new UsersList(<%= Newtonsoft.Json.JsonConvert.SerializeObject(Crew.Select(x => new { Id = x.Id, Name = x.DisplayName, Phone = x.GetCleanPhone() }).AsList()) %>);
                        ko.applyBindings(usersView, document.getElementById("usersList"));

                        function check() { errorWindow.checkValidation(); }
                        function ErrorWindow() {
                            var self = this;
                            self.isVisible = ko.observable(false);
                            self.text = ko.observable("");
                            self.checkValidation = function () {
                                var data = JSON.stringify({
                                    CurrentPostalId: <%= CurrentOrder.ShippingId %>,
                                    CurrentOrderId: <%= CurrentOrder.Id %>,
                                    Year: document.forms[0]["<%=DateInfoController.YearFullId("when-date")%>"].value,
                                    Month: document.forms[0]["<%=DateInfoController.MonthFullId("when-date")%>"].value,
                                    Day: document.forms[0]["<%=DateInfoController.DayFullId("when-date")%>"].value,
                                    FromHour: document.forms[0]["<%=TimeInfoController.HourFullId("start-time")%>"].value,
                                    FromMinute: document.forms[0]["<%=TimeInfoController.MinuteFullId("start-time")%>"].value,
                                    ToHour: document.forms[0]["<%=TimeInfoController.HourFullId("end-time")%>"].value,
                                    ToMinute: document.forms[0]["<%=TimeInfoController.MinuteFullId("end-time")%>"].value,
                                    DPointId: dpointsView.currentDPoint().Id,
                                    UserId: usersView.currentUser().Id,
                                    Commentary: document.getElementById('txtCommentary').value
                                });
                                $.ajax({
                                    type: "post",
                                    url: "/api/shipping/cdek/validateCallCourierData",
                                    data: data,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function () {
                                        self.isVisible(false);
                                        window.location.reload('<%= Request.Url.AbsolutePath %>');
                                    },
                                    error: function (response) {
                                        self.isVisible(true);
                                        self.text(response.responseText.substring(1, response.responseText.length - 1));
                                    }
                                });
                            }
                        }
                        var errorWindow = new ErrorWindow();
                        ko.applyBindings(errorWindow, document.getElementById("errorWindow"));
                    </script>
                    <% } %>
                    <% } %>
                </div>
                <% if (!string.IsNullOrEmpty(CurrentOrder.Properties.CalledCourierNumber))
                    { %>
                <div class="note">
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.TheCourierCallHasBeenRegisteredNumber) %> <%= CurrentOrder.Properties.CalledCourierNumber %>. <%= RM.GetString(RS.Shipping.ShippingProviderInfo.DetailsInTheOrderHistory) %>
                </div>
                <% } %>
                <% } %>

                <% var evropochtaSettings = postal.ServiceProviderSettings as EvropochtaServiceProviderSettings; %>
                <% if (evropochtaSettings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div class="message info">
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.PostalNumber) %> <%= CurrentOrder.TrackingNumber %>
                    <br />
                    <a target="_blank" href="https://evropochta.by/mvc/application/order/address.label/?number=<%= CurrentOrder.TrackingNumber %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadOrderLabel) %></a>
                </div>
                <div class="buttons">
                    <asp:Button CssClass="small" OnClick="EvropochtaUpdateStatus" Text="<%$ RM: Shipping.ShippingProviderInfo.CheckOrderStatusFromEvropochta %>" runat="server" />
                    <div style="margin: 10px 0 0 0"></div>
                    <asp:Button CssClass="small delete" OnClick="EvropochtaDeleteOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.RemoveLinkingAnOrderToEvropochta %>" runat="server" />
                </div>
                <div class="message warning">
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.ToRemoveAnOrderFrom) %> <a target="_blank" href="https://evropochta.by/">Evropochta</a> <%= RM.GetString(RS.Shipping.ShippingProviderInfo.ContactTheirTechnicalSupport) %>
                </div>
                <% }
                    else
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" OnClick="EvropochtaRegisterOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.RegisterWithEvropochta %>" runat="server" />
                </div>
                <% } %>
                <% } %>

                <% var boxberrySettings = postal.ServiceProviderSettings as BoxberryServiceProviderSettings; %>
                <% if (boxberrySettings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div style="padding: 0 0 20px;">
                    <a target="_blank" href="/content/shipping-document/boxberry/<%= CurrentOrder.Id %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadOrderReceipt) %></a>
                </div>
                <div class="buttons">
                    <asp:Button CssClass="small" OnClick="BoxberryUpdateStatus" Text="<%$ RM: Shipping.ShippingProviderInfo.CheckOrderStatusFromBoxberry %>" runat="server" />
                    <div style="margin: 10px 0 0 0"></div>
                    <asp:Button CssClass="small delete" OnClick="BoxberryDeleteOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.DeleteOrderFromBoxberry %>" runat="server" />
                </div>
                <% }
                    else
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" OnClick="BoxberryRegisterOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.RegisterWithBoxberry %>" runat="server" />
                </div>
                <% } %>
                <% } %>

                <% var novaSettings = postal.ServiceProviderSettings as NovaposhtaServiceProviderSettings; %>
                <% if (novaSettings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div style="padding: 0 0 20px;">
                    <span class="note"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DocumentsForSimferopol) %></span>
                    <ul>
                        <li><a target="_blank" href="http://orders.novaposhta.ua/pformn.php?o=<%: CurrentOrder.TrackingNumber %>&num_copy=1&token=<%:novaSettings.AuthKey %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.Receipt) %></a></li>
                        <li><a target="_blank" href="http://orders.novaposhta.ua/print_formm.php?o=<%: CurrentOrder.TrackingNumber %>&token=<%:novaSettings.AuthKey %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.PrintForm) %></a></li>
                    </ul>
                </div>
                <div style="padding: 0 0 20px;">
                    <span class="note"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DocumentsForKyiv) %></span>
                    <ul>
                        <li><a target="_blank" href="http://orders.novaposhta.ua/pformn.php?o=<%: CurrentOrder.TrackingNumber %>&num_copy=1&token=<%:novaSettings.AuthKey2 %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.Receipt) %></a></li>
                        <li><a target="_blank" href="http://orders.novaposhta.ua/print_formm.php?o=<%: CurrentOrder.TrackingNumber %>&token=<%:novaSettings.AuthKey2 %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.PrintForm) %></a></li>
                    </ul>
                </div>
                <% } %>
                <div class="buttons" style="text-align: left;">
                    <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                        { %>
                    <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.GetInvoices) %></label>
                    <asp:Button CssClass="small" runat="server" ID="butGetNovaposhtaDocs" OnClick="GetNovaposhtaDocs" Text="<%$ RM: Shipping.ShippingProviderInfo.Simferopol %>" />
                    <asp:Button CssClass="small" runat="server" ID="butGetNovaposhtaDocs2" OnClick="GetNovaposhtaDocs2" Text="<%$ RM: Shipping.ShippingProviderInfo.Kyiv %>" />
                    <% }
                        else
                        { %>
                    <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DeleteInvoices) %></label>
                    <asp:Button OnClientClick="<%$ RM: General.ConfirmDeleteJavaScript %>" CssClass="small delete" runat="server" ID="butNovaposhtaDeleteOrder" OnClick="NovaposhtaDeleteOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.Simferopol %>" />
                    <asp:Button OnClientClick="<%$ RM: General.ConfirmDeleteJavaScript %>" CssClass="small delete" runat="server" ID="butNovaposhtaDeleteOrder2" OnClick="NovaposhtaDeleteOrderClick2" Text="<%$ RM: Shipping.ShippingProviderInfo.Kyiv %>" />
                    <% } %>
                </div>
                <% } %>

                <% var imSettings = postal.ServiceProviderSettings as ImLogisticsServiceProviderSettings; %>
                <% if (imSettings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div>
                    <%: CurrentOrder.TrackingNumber %>
                </div>
                <% } %>
                <div class="buttons">
                    <% if (CurrentOrder.TrackingNumberStatus == TrackingNumberStatus.None)
                        { %>
                    <asp:Button CssClass="small" runat="server" ID="butGetImLogisticsDocs" OnClick="GetImLogisticsDocs" Text="<%$ RM: Shipping.ShippingProviderInfo.SendAnApplicationToImLogistics %>" />
                    <% }
                        else
                        { %>
                    <asp:Button CssClass="small" runat="server" ID="butImUpdateStatus" OnClick="ImLogisticsUpdateClick" Text="<%$ RM: Shipping.ShippingProviderInfo.CheckTheStatusOfImLogistics %>" />
                    <asp:Button OnClientClick="<%$ RM: General.ConfirmDeleteJavaScript %>" CssClass="small delete" runat="server" ID="butImDeleteOrder" OnClick="ImLogisticsDeleteOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.DeleteFromImLogisticsDatabase %>" />
                    <% } %>
                </div>
                <% } %>

                <% var imlV2Settings = postal.ServiceProviderSettings as ImlV2ServiceProviderSettings; %>
                <% if (imlV2Settings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div>
                    <%: CurrentOrder.TrackingNumber %>
                </div>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="btnImlV2UpdateStatus" OnClick="ImlV2UpdateStatusClick" Text="<%$ RM: Shipping.ShippingProviderInfo.CheckTheStatusOfImLogistics %>" />
                </div>
                <% }
                    else
                    {%>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="btnImlV2Register" OnClick="RegisterImlV2Order" Text="<%$ RM: Shipping.ShippingProviderInfo.SendAnApplicationToImLogistics %>" />
                </div>
                <% } %>
                <% } %>


                <% var novaV2Settings = postal.ServiceProviderSettings as NovaposhtaV2ServiceProviderSettings; %>
                <% if (novaV2Settings != null)
                    { %>
                <% if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div style="padding: 0 0 20px;">
                    <ul>
                        <li><a target="_blank" href="https://my.novaposhta.ua/orders/printDocument/orders[]/<%: CurrentOrder.TrackingNumber %>/type/pdf/apiKey/<%:novaV2Settings.ApiKey %>"><%=RM.GetString(RS.Order.Info.NovaposhtaPrintingForm)%></a></li>
                    </ul>
                    <ul>
                        <li><a target="_blank" href="https://my.novaposhta.ua/orders/printMarkings/orders[]/<%: CurrentOrder.TrackingNumber %>/type/pdf/apiKey/<%:novaV2Settings.ApiKey %>"><%=RM.GetString(RS.Order.Info.NovaposhtaSticker)%></a></li>
                    </ul>
                </div>
                <% } %>
                <div class="buttons" style="text-align: left;">
                    <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                        { %>
                    <asp:Button CssClass="small" runat="server" ID="Button2" OnClick="GetNovaposhtaV2Docs" Text="<%$ RM: Order.Info.NovaposhtaCreateDocument %>" />
                    <% }
                        else
                        { %>
                    <asp:Button OnClientClick="<%$ RM: General.ConfirmDeleteJavaScript %>" CssClass="small delete" runat="server" ID="butNovaposhtaV2DeleteOrder" OnClick="NovaposhtaV2DeleteOrderClick2" Text="<%$ RM: Order.Info.NovaposhtaDeleteDocument %>" />
                    <% } %>
                </div>
                <% } %>

                <% var yandexSettings = postal.ServiceProviderSettings as YandexDeliveryServiceProviderSettings; %>
                <% if (yandexSettings != null)
                    { %>
                <div class="buttons" style="text-align: left;">
                    <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                        { %>
                    <asp:Button CssClass="small" runat="server" ID="btnYandexRegister" OnClick="RegisterYandexOrder" Text="<%$ RM: Shipping.YandexDelivery.CreateDraft %>" />
                    <% }
                        else
                        { %>
                    <ul>
                        <li>
                            <a target="_blank" href="https://partner.market.yandex.ru/delivery/21611230/orders/item/<%:(CurrentOrder.Properties.ShippingRegistrationResult as YandexDeliveryRegistrationResult)?.OrderId ?? 0 %>"><%:RM.GetString(RS.Shipping.YandexDelivery.OrderLink) %></a>
                        </li>
                        <li>
                            <asp:Button CssClass="small" runat="server" ID="btnYandexUpdateStatus" OnClick="UpdateYandexOrder" Text="<%$ RM: Shipping.YandexDelivery.UpdateStatus %>" />
                        </li>
                        <li>
                            <asp:Button CssClass="small delete" runat="server" ID="btnYandexDelete" OnClick="DeleteYandexOrder" Text="<%$ RM: Shipping.YandexDelivery.DeleteOrder %>" />
                        </li>
                    </ul>
                    <% } %>
                </div>
                <% } %>

                <% var photomaxSettings = postal.ServiceProviderSettings as PhotomaxServiceProviderSettings; %>
                <% if (photomaxSettings != null && CurrentOrder.Id.ToString() != CurrentOrder.Number)
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="btnPhotomaxUpdate" OnClick="PhotomaxUpdateStatusClick" Text="<%$ RM: Shipping.Postal.PhotomaxUpdateStatus %>" />
                </div>
                <% } %>

                <% var omnivaSettings = postal.ServiceProviderSettings as OmnivaServiceProviderSettings; %>
                <% if (omnivaSettings != null)
                    { %>
                <%if (!string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div>
                    <label>Receiver email</label>
                    <input type="text" class="text" id="txtOmnivaEmail" />
                    <span class="hint"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.EmailToWhichAddressCardPDFWillBeSent) %></span>
                </div>
                <div class="buttons">
                    <button type="button" class="small" id="btnOmnivaAddressCard"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.SendAddressCard) %></button>
                </div>
                <script>
                    $('#btnOmnivaAddressCard').click(function () {
                        var email = $('#txtOmnivaEmail').val();
                        if (email.length == 0) {
                            $("#notify_container").notify("create",
                                "error-template",
                                { text: 'Fill in the "Receiver email" field' });
                            return;
                        }
                        $.ajax({
                            type: "POST",
                            url: "/api/shipping/omniva/sendAddressCardToEmail?orderId=" + <%=CurrentOrder.Id%> + "&email=" + email,
                            async: false,
                            error: function (xhr, textStatus, errorThrown) {
                                sendToPostalUpdate = $("#notify_container").notify("create",
                                    "error-template",
                                    { text: 'Errot while sending: ' + textStatus });
                            },
                            success: function (result) {
                                sendToPostalUpdate = $("#notify_container").notify("create",
                                    "success-template",
                                    { text: 'Address card was successful sent to email:' + email });
                            }
                        });
                    });
                </script>
                <%}
                    else
                    {%>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="btnOmnivaRegister" OnClick="OmnivaRegisterClick" Text="Register" />
                </div>
                <% } %>
                <% } %>

                <% var exgarantSettings = postal.ServiceProviderSettings as ExgarantServiceProviderSettings; %>
                <% if (exgarantSettings != null)
                    { %>
                <pr:ExgarantRegisterController runat="server" ID="prExgarantRegisterController" />
                <% } %>

                <% var justinSettings = postal.ServiceProviderSettings as JustinServiceProviderSettings; %>
                <% if (justinSettings != null)
                    { %>
                <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="butJustinRegisterOrder" OnClick="JustinRegisterOrderClick" Text="<%$ RM: Order.Info.JustinGetNumber %>" />
                </div>
                <% }%>
                <% } %>

                <% var econtSettings = postal.ServiceProviderSettings as EcontServiceProviderSettings; %>
                <% if (econtSettings != null)
                    { %>
                <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="butEcontRegisterOrder" OnClick="EcontRegisterOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.SendToEcont %>" />
                </div>
                <% }
                    else
                    {%>
                <div>
                   <%= RM.GetString(RS.Shipping.ShippingProviderInfo.TrackNumber) %> <%: CurrentOrder.TrackingNumber %>
                </div>
                <div style="padding: 0 0 20px;">
                    <a target="_blank" href="/content/shipping-document/econt/<%= CurrentOrder.Id %>"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadOrderReceipt) %></a>
                </div>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="butEcontDeleteOrder" OnClick="EcontDeleteOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.DeleteFromEcont %>" />
                </div>
                <% } %>
                <% } %>

                <% var pickpointSettings = postal.ServiceProviderSettings as PickpointServiceProviderSettings; %>
                <% if (pickpointSettings != null)
                    { %>
                <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="butPickpointRegisterOrder" OnClick="PickpointRegisterOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.SendToPickpoint %>" />
                </div>
                <% }
                    else
                    {%>
                <div class="buttons">
                    <button type="button" class="small" id="btnPickpointAddressCard"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.GenerateLabelInPDF) %></button>
                </div>
                <script>
                    $('#btnPickpointAddressCard').click(function () {

                        $.ajax({
                            type: "GET",
                            url: "/api/shipping/pickpoint/getPickpointOrderLabelPdf?orderId=" + <%=CurrentOrder.Id%>,
                            async: false,
                            error: function (xhr, textStatus, errorThrown) {
                                sendToPostalUpdate = $("#notify_container").notify("create",
                                    "error-template",
                                    { text: 'Errot while sending: ' + textStatus });
                            },
                            success: function (result) {
                                const linkSource = `data:application/pdf;base64,${result}`;
                                const downloadLink = document.createElement("a");
                                const fileName = '<%=CurrentOrder.Id%>' + '-label.pdf';

                                downloadLink.href = linkSource;
                                downloadLink.download = fileName;
                                downloadLink.click();
                            }
                        });
                    });
                </script>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="butPickpointDeleteOrder" OnClick="PickpointDeleteOrderClick" Text="<%$ RM: Shipping.ShippingProviderInfo.RemoveFromPickpoint %>" />
                </div>
                <% } %>
                <% } %>

                <% if (postal.ServiceProviderSettings is PostnlServiceProviderSettings)
                    { %>
                <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="Button1" OnClick="PostnlRegisterOrderClick" Text="<%$ RM: Order.Info.PostnlGetNumber %>" />
                </div>
                <% } %>
                <% } %>

                <% if (postal.ServiceProviderSettings is UkrposhtaServiceProviderSettings)
                    { %>
                <% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber))
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="btnUkrposhtaRegisterOrder" OnClick="UkrposhtaRegisterOrderClick" Text="<%$ RM: Shipping.Postal.UkrposhtaRegisterOrder %>" />
                </div>
                <% }
                    else
                    { %>
                <div class="buttons">
                    <asp:Button CssClass="small" runat="server" ID="btnUkrposhtaGetOrderStatus" OnClick="UkrposhtaGetOrderStatusClick" Text="<%$ RM: Shipping.Postal.UkrposhtaUpdateStatus %>" />
                </div>
                <% } %>
                <% } %>

                <% var yandexGoSettings = postal.ServiceProviderSettings as YandexGoServiceProviderSettings; %>
                <% if (yandexGoSettings != null)
                    { %>
                <h3><%= RM.GetString(RS.Shipping.ShippingProviderInfo.YandexGoTitle) %></h3>

                <% if (CurrentOrder.Properties.ShippingRegistrationResult == null)
                    { %>
                <div id="yandexGoReg">
                    <div id="yandexGoReadyDate">
                        <h4><%:RM.GetString("Shipping.YandexGo.CertainTime")%></h4>
                        <div class="dl-group-wrapper">
                            <div class="dl-group">
                                <dl>
                                    <dt><%: RM.GetString(RS.Order.Create.ReadyDate) %></dt>
                                    <dd>
                                        <input type="time" id="readyTime" name="readyTime" />
                                        <input type="date" id="readyDate" name="readyDate" />
                                    </dd>
                                </dl>
                            </div>
                        </div>
                    </div>
                    <ul>
                        <li>
                            <input type="button" class="button yaGo" value="<%:RM.GetString("Shipping.YandexGo.SetTime")%>" id="setTime" style="display: none" />
                            <span id="timeInfo" style="display: none"></span>
                            <input type="button" class="button yaGo" value="<%:RM.GetString("Shipping.YandexGo.Registration")%>" id="registerOrder" style="display: none" />
                        </li>
                    </ul>
                </div>
                <%}
                    else if (CurrentOrder.Properties.ShippingRegistrationResult is YandexGoRegistrationResult result)
                    {%>
                <ul id="yandexGoControl">
                    <li>
                        <span id="yandexInfo"><%:RM.GetString("Shipping.YandexGo.OrderId")%> <b><%=result.OrderId %></b></span>
                        <a href="https://dostavka.yandex.ru/account/cargo" target="_blank"><%:RM.GetString("Shipping.YandexGo.PersonalCabinet")%></a>
                    </li>
                    <li>
                        <input type="button" class="button yaGo" value="<%:RM.GetString("Shipping.YandexGo.Status")%>" id="statusOrder" />
                        <span id="statusOrderInfo" style="display: none"></span>
                    </li>
                    <li>
                        <input type="button" class="button yaGo" value="<%:RM.GetString("Shipping.YandexGo.Confirm")%>" id="acceptOrder" style="display: none" />
                        <input type="button" class="button yaGo" value="<%:RM.GetString("Shipping.YandexGo.Cancel")%>" id="cancelOrder" style="display: none" />
                        <span id="acceptOrderInfo" style="display: none"></span>
                    </li>
                </ul>
                <%}
                    else
                    {%>
                <span><%:RM.GetString("Shipping.YandexGo.DataNotFound")%></span>
                <%}%>
                <style>
                    .yaGo {
                        border: solid 1px #8bb24d;
                        border-radius: 4px;
                        background: #d0f0b5;
                        cursor: pointer;
                    }
                    .yaGo:active {
                        border: solid 1px #B2E06C;
                        border-top: solid 1px #7f936f;
                        border-left: solid 1px #7f936f;
                        text-shadow: #f6ffee 0 -1px 0;
                    }
                </style>
                <script>
                    const currentOrderId = <%=CurrentOrder.Id%>;
                    const token = '<%=yandexGoSettings.AccessToken%>';
                    let isAccept = false;

                    let $setTime = $('#setTime');
                    let $registerOrder = $('#registerOrder');
                    let $acceptOrder = $('#acceptOrder');
                    let $statusOrder = $('#statusOrder');
                    let $cancelOrder = $('#cancelOrder');

                    let $timeInfo = $('#timeInfo');
                    let $statusOrderInfo = $('#statusOrderInfo');
                    let $acceptOrderInfo = $('#acceptOrderInfo');
                    let $cancelOrderInfo = $('#cancelOrderInfo');


                    $setTime.click(function () {
                        let due = new Date(`${$('#readyDate').val()} ${$('#readyTime').val()}:00`);
                        setTime(due.toJSON());
                    });

                    $registerOrder.click(function () {
                        $(this).prop('disabled', true);
                        registerOrder();
                    });
                    $acceptOrder.click(function () {
                        $(this).prop('disabled', true);
                        acceptOrder();
                        $statusOrderInfo.hide()
                    });
                    $statusOrder.click(function () {
                        $(this).prop('disabled', true);
                        statusOrder();
                        $(this).prop('disabled', false);
                    });
                    $cancelOrder.click(function () {
                        $(this).prop('disabled', true);
                        cancelOrder();
                        $statusOrderInfo.hide()
                    });

                    $('#yandexGoReadyDate').change(function () {
                        $timeInfo.hide();
                        if ($('#readyDate').val() && $('#readyTime').val()) {
                            $setTime.show(128);
                            $registerOrder.hide(128);
                        }
                    });

                    function setTime(due) {
                        $.ajax({
                            url: `/api/shippings/yandexGo/setTime?orderId=${currentOrderId}&due=${due}`,
                            type: "POST",
                            success: function (response) {
                                $timeInfo.text(`<%:RM.GetString("Shipping.YandexGo.WillBeCreatedBy") %> ${response}`).css('color', 'green').show(64);
                                $registerOrder.show();
                                $setTime.hide();
                            },
                            error: function (response) {
                                $timeInfo.text(response.responseText).css('color', 'red').show(64);
                                return;
                            }
                        })
                    };

                    function registerOrder() {
                        $.ajax({
                            url: `/api/shippings/yandexGo/registerOrder?orderId=${currentOrderId}`,
                            type: "POST",
                            success: function (response) {
                                window.location.reload(false);
                            },
                            error: function (response) {
                                console.log("error", response)
                                return;
                            }
                        })
                    };

                    function acceptOrder() {
                        $.ajax({
                            url: `/api/shippings/yandexGo/acceptOrder?orderId=${currentOrderId}&token=${token}`,
                            type: "GET",
                            success: function (response) {
                                isAccept = true;
                                $acceptOrder.hide();
                            },
                            error: function (response) {
                                $acceptOrderInfo.text(response.responseText).show(64);
                                return;
                            }
                        })
                    };

                    function cancelOrder() {
                        $.ajax({
                            url: `/api/shippings/yandexGo/cancelOrder?orderId=${currentOrderId}&token=${token}`,
                            type: "GET",
                            success: function (response) {
                                $acceptOrder.hide();
                                $cancelOrder.hide();
                                $cancelOrderInfo.text('<%:RM.GetString("Shipping.YandexGo.OrderCanceled") %>').show(64);
                            },
                            error: function (response) {
                                $cancelOrderInfo.text(response.responseText).show(64);
                                return;
                            }
                        })
                    };

                    function statusOrder() {
                        $.ajax({
                            url: `/api/shippings/yandexGo/getStatus?orderId=${currentOrderId}&token=${token}`,
                            type: "GET",
                            success: function (response) {
                                $statusOrderInfo.html(`<b>${response.status}</b>`).show(64);
                                if (response.AvailableCancelState) {
                                    isAccept ? $acceptOrder.hide() : $acceptOrder.show(64);
                                    $cancelOrder.show();
                                }
                            },
                            error: function (response) {
                                console.log("error", response)
                                return;
                            }
                        })
                    };
                </script>
                <% } %>

                <% if (postal.ServiceProviderSettings is RussianPostServiceProviderSettings russianPostSettings )
                { 
                    var rpRegResult = CurrentOrder.Properties.ShippingRegistrationResult as RussianPostRegistrationResult; 
                    var rpAddressInfo = CurrentOrder.DeliveryAddress?.DeliveryProperties?.RussianPostAddressInfo;
                    if (rpAddressInfo != null)
                    { %>
                    <div id="rpControl" data-bind="css: { 'loading-wheel': isLoading }">
                        <!-- ko ifnot: isRegisterOrderView -->
                        <div class="AB rp-list-item-spacer">
                            <div class="AB-A">
                                <div>
                                    <span><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.ShipmentIdentifier) %>
                                        <span data-bind="html: departureId"></span>
                                    </span>
                                </div>
                                <div>
                                    <a href="https://otpravka.pochta.ru/dashboard#/prepare"><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.PrepareShipping) %></a>
                                </div>
                            </div>
                            <div class="AB-B">
                                <button class="small" data-bind="click: unRegisterOrder"><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.DeleteShipment) %></button>
                            </div>
                        </div>
                        <div class="AB">
                            <div class="AB-A">
                                <!-- ko if: hasDeliveryStatus -->
                                <span data-bind="html: deliveryStatus"></span>
                                <!-- /ko -->
                            </div>
                            <div class="AB-B">
                                <button class="small" data-bind="click: getStatus"><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.RefreshStatus) %></button>
                            </div>
                        </div>
                        <!-- ko if: deleteError -->
                        <div>
                            <span class="message error"><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.Errors.DeleteShipmentError) %></span>
                        </div>
                        <!-- /ko -->
                        <!-- ko if: statusUpdateError -->
                        <div>
                            <span class="message error"><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.Errors.StatusUpdateError) %></span>
                        </div>
                        <!-- /ko -->
                        <!-- /ko -->
                        <!-- ko if: isRegisterOrderView -->
                        <!-- ko ifnot: onError -->
                        <div>
                            <!-- ko if: postalDefined -->
                            <div class="A rp-list-item-spacer">
                                <span><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.DefinedPostal) %></span>
                                <input class="text" data-bind="value: orderPostal().text" disabled/>
                            </div>
                            <!-- /ko -->
                            <!-- ko ifnot: postalDefined -->
                            <div class="A">
                                <span><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.PostalSelect) %></span>
                            </div>
                            <div class="A rp-list-item-spacer">
                                <select data-bind="options: rpService.userPostals, optionsText: 'text', optionValue: 'operator-postcode', value: orderPostal"></select>
                            </div>
                            <!-- /ko -->
                            <!-- ko if: showRgisterButton -->
                            <div class="A">
                                <button class="small" data-bind="click: registerOrder"><%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.RegisterShipment) %></button>
                            </div>
                            <!-- /ko -->
                        </div>
                        <!-- /ko -->
                        <!-- ko if: onError -->
                            <div>
                                <span class="message error" data-bind="html: errorMessage"></span>
                            </div>
                        <!-- /ko -->
                        <!-- /ko -->
                        
                    </div>
                    <script>
                        window.rpService = new rpService();
                        var rpService = window.rpService;

                        function rpService() {
                            var self = this

                            self.orderId =  <%= CurrentOrder.Id %>;
                            self.orderProps = JSON.parse('<%= rpAddressInfo.ToJSON() %>');
                            //observables
                            self.isLoading = ko.observable(false);
                            self.departureId = ko.observable(JSON.stringify(<%= rpRegResult?.ShippingRegistrationId %>) ?? "");
                            const operation = "<%= rpRegResult?.CurrentOperation %>" ?? "";
                            const attr = "<%= rpRegResult?.CurrentOperationAttr %>" ?? "";
                            self.hasDeliveryStatus = ko.observable(operation.length>0);
                            self.deliveryStatus = ko.observable(self.hasDeliveryStatus() ? operation + " " + attr : "");
                            if (self.orderProps.PostalFrom != undefined) {
                                self.postalDefined = ko.observable(true);
                                let postalFrom = self.orderProps.PostalFrom;
                                postalFrom.text = postalFrom.Postcode + ', ' + postalFrom.Address;
                                self.orderPostal = ko.observable(postalFrom);
                            } else {
                                self.postalDefined = ko.observable(false);
                                self.orderPostal = ko.observable(undefined);
                            }
                            self.userPostals = ko.observableArray();
                            self.onError = ko.observable(false);
                            self.errorMessage = ko.observable("");
                            self.deleteError = ko.observable(false);
                            self.statusUpdateError = ko.observable(false);
                            //comp
                            self.isRegisterOrderView = ko.pureComputed(isRegisterOrderViewComp, self);
                            self.showRgisterButton = ko.pureComputed(showRgisterButtonComp, self);
                            //functions

                            self.registerOrder = function () {
                                const data = JSON.stringify({
                                    OrderId: self.orderId,
                                    OrderPostal: self.orderPostal()
                                });
                                self.isLoading(true);
                                $.ajax({
                                    url: `/api/shippings/russianPost/registerOrder`,
                                    type: "POST",
                                    data: data,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function(response) {
                                        window.location.reload(window.location.href+"#shipping");
                                    },
                                    error: function(response) {
                                        self.onError(true);
                                        self.errorMessage("<%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.Errors.RegisterShipmentError) %>")
                                        console.log("error", response);
                                        return;
                                    },
                                    complete: function() {
                                        self.isLoading(false);
                                    }
                                });
                            };

                            self.unRegisterOrder = function () {
                                const data = JSON.stringify({
                                    OrderId: self.orderId
                                });
                                self.isLoading(true);
                                $.ajax({
                                    url: `/api/shippings/russianPost/unRegisterOrder`,
                                    type: "DELETE",
                                    data: data,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function(response) {
                                        window.location.reload(window.location.href+"#shipping");
                                    },
                                    error: function(response) {
                                        self.deleteError(true);
                                        console.log("error", response)
                                        return;
                                    },
                                    complete: function() {
                                        self.isLoading(false);
                                    }
                                });
                            };

                            self.getStatus = function() {
                                self.isLoading(true);
                                const url = `/api/shippings/russianPost/shippingStatus?orderId=` + self.orderId;
                                $.ajax({
                                    url: url,
                                    type: "GET",
                                    success: function(response) {
                                        self.deliveryStatus(response.CurrentOperation + " " + response.CurrentOperationAttr);
                                        self.statusUpdateError(false);
                                    },
                                    error: function(response) {
                                        self.statusUpdateError(true);
                                        console.log("error", response)
                                        return;
                                    },
                                    complete: function() {
                                        self.hasDeliveryStatus(true)
                                        self.isLoading(false);
                                    }
                                });
                            }

                            self.getPostals = function() {
                                self.isLoading(true);
                                const url = `/api/shippings/russianPost/postals?orderId=` + self.orderId;
                                $.ajax({
                                    url: url,
                                    type: "GET",
                                    success: function(response) {
                                        response.forEach(p => p.text = p.Postcode + ", " + p.Address);
                                        self.userPostals(response.filter(p => p.Enabled));
                                    },
                                    error: function(response) {
                                        self.onError(true);
                                        self.errorMessage("<%: RM.GetString(RS.Shipping.RussianPost.RgisterShipping.Errors.UpdatePostailsError) %>");
                                        console.log("error", response);
                                        return;
                                    },
                                    complete: function() {
                                        self.hasDeliveryStatus(true)
                                        self.isLoading(false);
                                    }
                                });
                            }

                            function isRegisterOrderViewComp() {
                                return !self.departureId().length > 0;
                            }
                            
                            function showRgisterButtonComp() {
                                return self.orderPostal() !== undefined;
                            }

                            //calls
                            if (self.isRegisterOrderView() && !self.postalDefined())
                                self.getPostals();
                        }
                        $(document).ready(() => {
                            ko.applyBindings(window.rpService, $("#rpControl")[0]);
                        });
                    </script>
                    <style>
                        .rp-list-item-spacer {
                            margin-bottom: 10px
                        }
                    </style>

                <%
                    }
                }%>

    <% var dpdSettings = postal.ServiceProviderSettings as DpdServiceProviderSettings; %>
    <% if (dpdSettings != null)
        { %>
     <label for="dpdInfo"><b>DPD:</b></label>
     <div id="dpdInfo" class="message" style="display: none"></div>
      <% if (CurrentOrder.Properties.ShippingRegistrationResult == null)
        { %>
        <div id="dpdReadyDate">
            <div class="dl-group-wrapper">
                <div class="dl-group">
                    <dl>
                        <dt><%: RM.GetString("Shipping.Dpd.DatePickup") %></dt>
                        <dd>
                            <input type="date" id="dpdDate" name="dpdDate" />
                        </dd>
                    </dl>
                </div>
            </div>
        </div>
       <div id="dpdDateInfo" class="message" style="display: none"></div>
       <input type="button" class="button dpd" value="<%:RM.GetString("Shipping.Dpd.SetTime")%>" id="dpdSetDate" style="display: none" />
       <input type="button" class="button dpd" value="<%:RM.GetString("Shipping.Dpd.Registration")%>" id="dpdRegisterOrder" style="display: none" />

     <% }  else if (CurrentOrder.Properties.ShippingRegistrationResult is DpdRegistrationResult result && !result.RegistrationRemoved)
        {%>
    <div>
        <div class="message info"><%:RM.GetString("Shipping.Dpd.RegOrderInfo")%> <b><%: result.OrderId %></b></div>
        <input type="button" class="button dpd" value="<%:RM.GetString("Shipping.Dpd.Tracking")%>"id="dpdTrackingOrder" />
        <input type="button" class="button dpd" value="<%:RM.GetString("Shipping.Dpd.Cancel")%>"id="dpdCancelOrder" />
    </div>
         <% } else {%><span><%:RM.GetString("Shipping.Dpd.DataNotFound")%></span> <%}%>

   <style>
       .dpd {
           border: solid 1px #8bb24d;
           border-radius: 4px;
           background: #d0f0b5;
           cursor: pointer;
       }

       .dpd:active {
           border: solid 1px #B2E06C;
           border-top: solid 1px #7f936f;
           border-left: solid 1px #7f936f;
           text-shadow: #f6ffee 0 -1px 0;
       }
   </style>
   <script>
        const currentOrderId = <%=CurrentOrder.Id%>;
        const dpdAuth = '<%=dpdSettings.AuthSettings.ToJSON() %>';
        let $dpdSetDate = $('#dpdSetDate');
        let $dpdRegisterOrder = $('#dpdRegisterOrder');
        let $dpdCancelOrder = $('#dpdCancelOrder');
        let $dpdTrackingOrder = $('#dpdTrackingOrder');
        let $dpdDateInfo = $('#dpdDateInfo');
        let $dpdInfo = $('#dpdInfo');
        let $dpdDate= $('#dpdDate');

        statusRegOrder();

        $dpdSetDate.click(function () {
            let date = new Date($('#dpdDate').val());
            setDate(date.toJSON());
        });

        $dpdRegisterOrder.click(function () {
            let self = $(this);
            self.prop('disabled', true);
            registerOrder(() => self.prop('disabled', false))
        });

        $dpdCancelOrder.click(function () {
            let self = $(this);
            self.prop('disabled', true);
            cancelOrder(() => self.prop('disabled', false));
        });

        $dpdTrackingOrder.click(function () {
            let self = $(this);
            self.prop('disabled', true);
            trackingOrder(() => self.prop('disabled', false));
         });

        $('#dpdReadyDate').change(function () {
            $dpdDateInfo.hide();
            if ($('#dpdDate').val()) {
                $dpdSetDate.show(128);
                $dpdRegisterOrder.hide(128);
            }
        });

        function setDate(date) {
            $.ajax({
                url: `/api/shippings/dpd/setDate?orderId=${currentOrderId}&date=${date}`,
                type: "POST",
                success: function (response) {
                    $dpdDateInfo.text(`<%:RM.GetString("Shipping.Dpd.WillBeCreatedBy") %> ${response}`).css('color', 'green').show(64);
                    $dpdRegisterOrder.show();
                    $dpdSetDate.hide();
                },
                error: function (response) {
                    $dpdDateInfo.text(response.responseText).css('color', 'red').show(64);
                    return;
                }
            })
        };

       function registerOrder(callback) {
            $.ajax({
                url: `/api/shippings/dpd/registerOrder?orderId=${currentOrderId}`,
                type: "POST",
                success: function (response) {
                    window.location.reload(false);
                },
                error: function (response) {
                    console.log("error registerOrder dpd:", response);
                    callback();
                    return;
                }
            })
        };

       function trackingOrder(callback) {
            $.ajax({
                url: `/api/shippings/dpd/trackingOrder?orderId=${currentOrderId}`,
                type: "POST",
                data: dpdAuth,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $dpdInfo.html(`<b><ol>${response}<ol></b>`).show(100);
                    callback();
                },
                error: function (response) {
                    $dpdInfo.html(`<b>${response.responseText}</b>`).show(100);
                    callback();
                    return;
                }
            })
        };

       function statusRegOrder() {
            $.ajax({
                url: `/api/shippings/dpd/getOrderRegStatus?orderId=${currentOrderId}`,
                type: "POST",
                data: dpdAuth,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.Status != "NotDataFound") {}
                    $dpdInfo.html(`<%= RM.GetString(RS.Shipping.ShippingProviderInfo.CurrentOrderRegistrationStatusInTheDPDSystem)%> <br><b>${response.ErrorMessage}</b>`).show(100);
                },
                error: function (response) {
                    $dpdInfo.html(`<b>${response.responseText}</b>`).show(100);
                    return;
                }
            })
        };

       function cancelOrder(callback) {
            $.ajax({
                url: `/api/shippings/dpd/cancelOrder?orderId=${currentOrderId}`,
                type: "POST",
                data: dpdAuth,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $dpdCancelOrder.hide();
                    $dpdInfo.html(`<%= RM.GetString(RS.Shipping.ShippingProviderInfo.OrderCancellationResult)%> <b>Статус [${response.Status}] ${response.ErrorMessage}</b>`).show(100);
                    callback();
                },
                error: function (response) {
                    $dpdInfo.html(`<b>${response.responseText}</b>`).show(100);
                    callback();
                    return;
                }
            })
        };
   
   </script>
    <% } %>

 <% var cdekV2Settings = postal.ServiceProviderSettings as CDEKv2ServiceProviderSettings;
    var cdekRegResult = CurrentOrder.Properties?.ShippingRegistrationResult as CDEKv2RegistrationResult;
    if (cdekV2Settings != null)
        { %>
    <label for="cdekInfo"><b>CDEK:</b></label>
    <div id="cdekInfo" class="message" style="display: none"></div>
    <div id ="cdekCourierInfoBlock" style="display: none">
        <label for="cdekCourierInfo"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.CourierInfo) %></label>
        <div id="cdekCourierInfo" class="message"></div>
    </div>
            <% if (CurrentOrder.Properties.ShippingRegistrationResult == null)
        { %>
<div class="buttons" style="text-align: left">
    <input type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.RegisterAnApplication) %>" id="cdekRegOrder" />
</div>
 <% }  else if (!cdekRegResult.RegistrationRemoved)
        {%>

<div class="dl-group">
    <dl>
        <dd>
            <select id="cdekFormat">
                <option value="0" selected><%= RM.GetString(RS.Shipping.ShippingProviderInfo.PrintFormat) %></option>
                <option value="A4">A4</option>
                <option value="A5">A5</option>
                <option value="A6">A6</option>
            </select>
        </dd>
        <dd>
            <select id="cdekLang">
                <option value="0" selected><%= RM.GetString(RS.Shipping.ShippingProviderInfo.PrintLanguage) %></option>
                <option value="RUS">RUS</option>
                <option value="ENG">ENG</option>
            </select>
        </dd>
        <dd>
            <div class="buttons" style="padding: 4px">
                <input type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.FormationOfBarcode) %>" id="cdekFormingBarcode" />
                <a class="pdf-icon" id="cdekDownloadBarcode" style="display: none"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadBarcode) %></a>
            </div>
        </dd>
    </dl>
</div>
<div class="dl-group">
    <dl>
        <dd>
            <div class="buttons" style="text-align: left; padding: 4px">
                <input type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.GeneratingAReceipt) %>" id="cdekFormingReceipt" />
            </div>
        </dd>
        <dd>
            <a class="pdf-icon" id="cdekDownloadReceipt" style="display: none"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.DownloadReceipt) %></a>
        </dd>
    </dl>
</div>
<div class="buttons" style="margin: 7px">
    <input type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.CallACourier) %>" id="cdekCallCourier"/>
    <input style="display: none" type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.ApplicationInformation) %>" id="cdekCourierInfoBtn" />
    <input style="display: none" type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.CancelCourier) %>" id="cdekCancelCourier"/>
</div>
<div class="buttons">
    <input type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.ApplicationInformation) %>" id="cdekOrderInfo" />
    <input type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.CancelTheApplication) %>" id="cdekCancelOrder" />
</div>

                <div class="modal-box" id="cdekCallCourierModalBox">
    <h4><%= RM.GetString(RS.Shipping.ShippingProviderInfo.RegistrationOfAnApplicationForACourierCall) %></h4>
    <fieldset>
        <div>
            <dl>
                <dd>
                    <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.WaitingDateForTheCourier) %></label>
                </dd>
                <dd>
                    <input type="date" id="cdekWaitingDate" name="cdekWaitingDate" />
                </dd>
            </dl>
            <dl>
                <dd class="note">
                    <%= RM.GetString(RS.Shipping.ShippingProviderInfo.AnOrderCreatedOnTheCurrentDateAfter) %>
                </dd>
            </dl>
            <dl>
                <dd>
                    <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.CourierStartTime) %></label>
                </dd>
                <dd>
                    <input type="time" id="cdekStartTime" name="cdekStartTime" />
                </dd>
            </dl>
            <dl>
                <dd>
                    <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.EndTimeForCourier) %></label>
                </dd>
                <dd>
                    <input type="time" id="cdekEndTime" name="cdekEndTime" />
                </dd>
            </dl>
            <dl>
                <dd class="note">
                   <%= RM.GetString(RS.Shipping.ShippingProviderInfo.NotEarlierAndNotLater) %>
                </dd>
            </dl>
        </div>
        <div style="margin: 20px"></div>
        <div>
            <label><%= RM.GetString(RS.Shipping.ShippingProviderInfo.CommentToTheApplicationForTheCourier) %></label>
            <input type="text" id="cdekComment" class="text" />
        </div>
        <div style="margin: 20px"></div>
        <div>
            <label>
                <input type="checkbox" name="cdekNeedCall" id="cdekNeedCall" value="value">
                <b> <%= RM.GetString(RS.Shipping.ShippingProviderInfo.NeedToCallTheSender) %></b>
            </label>
        </div>
        <div style="margin: 20px"></div>
        <div id="cdekCallCourierError" style="display: none"></div>
    </fieldset>

    <div class="buttons">
        <button type="button" id="cdekRegCallCourier"> <%= RM.GetString(RS.Shipping.ShippingProviderInfo.RegisterAnApplication) %></button>
    </div>
</div>

<% } else {%>
<span><%= RM.GetString(RS.Shipping.ShippingProviderInfo.RequestDeletedOrNotFound) %></span> 
<div class="note"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.MoreInformationInTheOrderHistory) %></div>
    <div class="buttons">
        <button type="button" id="cdekReRegOrder"><%= RM.GetString(RS.Shipping.ShippingProviderInfo.ReRegistrationOfTheApplication) %></button>
    </div>

<%}%>

<script>
    const orderId = <%=CurrentOrder.Id%>;
    const cdekAuth = '<%= cdekV2Settings.AuthSettings.ToJSON() %>';
    const uuidOrder = '<%= cdekRegResult?.Uuid %>';
    const errorMsg = '<%= RM.GetString(RS.Shipping.ShippingProviderInfo.UnhandledErrorServerResponse)%>';

    let $cdekInfo = $('#cdekInfo');

    if ('<%= cdekRegResult?.RegistrationRemoved ?? true%>' != 'True') {
        orderInfo();
        $cdekInfo.show(100);
    }

    let $cdekRegOrder = $('#cdekRegOrder');
    let $cdekCancelOrder = $('#cdekCancelOrder');
    let $cdekOrderInfo = $('#cdekOrderInfo');
    let $cdekReRegOrder = $('#cdekReRegOrder');

    let $cdekCallCourierError = $('#cdekCallCourierError');
    let $cdekCallCourier = $('#cdekCallCourier');
    let $cdekCancelCourier = $('#cdekCancelCourier');
    let $cdekCallCourierModalBox = $('#cdekCallCourierModalBox');
    let $cdekRegCallCourier = $('#cdekRegCallCourier');
    let $cdekCallCourierInfo = $('#cdekCourierInfo');
    let $cdekCourierInfoBlock = $('#cdekCourierInfoBlock');
    let $cdekCourierInfoBtn = $('#cdekCourierInfoBtn');

    let $cdekWaitingDate = $('#cdekWaitingDate');
    let $cdekStartTime = $('#cdekStartTime');
    let $cdekEndTime = $('#cdekEndTime');
    let $cdekComment = $('#cdekComment');
    let $cdekNeedCall = $('#cdekNeedCall');
    
    let uuidReceipt;
    let uuidBarcode;
    let urlBarcode;
    let urlReceipt;
    
    let uuidCallCourier = '<%= cdekRegResult?.CallCourierUuid ?? string.Empty%>';

    if (uuidCallCourier != '') {
        $cdekCallCourier.hide(150);
        $cdekCancelCourier.show(150);
        $cdekCourierInfoBtn.show(150);
        getCourierInfo(() => {$cdekCourierInfoBlock.show(150)});
    }

    let $cdekFormingReceipt = $('#cdekFormingReceipt');
    let $cdekDownloadReceipt = $('#cdekDownloadReceipt');

    let $cdekFormat = $('#cdekFormat');
    let $cdekLang = $('#cdekLang');
    let $cdekFormingBarcode = $('#cdekFormingBarcode');
    let $cdekDownloadBarcode = $('#cdekDownloadBarcode');

    $cdekFormat.change(function () {
        urlBarcode = null;
        $cdekFormingBarcode.show(150);
        $cdekDownloadBarcode.hide();
    });

    $cdekLang.change(function () {
        urlBarcode = null;
        $cdekFormingBarcode.show(150);
        $cdekDownloadBarcode.hide();
    });
    
    $cdekRegOrder.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        registerOrder(() => self.prop('disabled', false));
    });

    $cdekReRegOrder.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        registerOrder(() => self.prop('disabled', false));
    });

    $cdekCallCourier.click(function () {
        $cdekCallCourierModalBox.jqmShow();
        return false;
    });
    
    $cdekRegCallCourier.click(function () {
        let self = $(this);
        self.prop('disabled', true);

        let request = {
                order_uuid: uuidOrder,
                intake_date: $cdekWaitingDate.val(),
                intake_time_from: $cdekStartTime.val(),
                intake_time_to: $cdekEndTime.val(),
                comment: $cdekComment.val(),
                need_call: $cdekNeedCall.is(':checked'),
            };

        if (validateCallCourier(request)) {
            registerCallCourier(request, () => self.prop('disabled', false));
            return;
        }
        self.prop('disabled', false)
    });
    
    $cdekCancelCourier.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        registerCancelCourier(() => self.prop('disabled', false));
    });
    
    $cdekCourierInfoBtn.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        getCourierInfo(() => self.prop('disabled', false));
    })

    $cdekCancelOrder.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        cancelOrder(() => self.prop('disabled', false));
    });

    $cdekOrderInfo.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        orderInfo(() => self.prop('disabled', false));
    });

    $cdekFormingReceipt.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        formingReceipt(() => self.prop('disabled', false));
    });

    $cdekDownloadReceipt.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        if (urlReceipt) {
            getPdf(urlReceipt, 'Receipt');
            self.prop('disabled', false)
            return;
        }
        orderReceipt(() => self.prop('disabled', false));
    });

    $cdekFormingBarcode.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        
        if ($cdekFormat.val() == '0' || $cdekLang.val() == '0') {
            $cdekInfo.html('<b style="color:red">Выбраны не все параметры</b>')
            self.prop('disabled', false);
            return;
        }
        formingBarcode(
            $cdekFormat.val(),
            $cdekLang.val(),
            () => self.prop('disabled', false)
        );
    });

    $cdekDownloadBarcode.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        orderBarcode(() => self.prop('disabled', false));
    });

    function registerOrder(callback) {
        $.ajax({
            url: `/api/shippings/cdek/register?orderId=${orderId}`,
            type: "GET",
            success: function () {
                window.location.reload(false);
            },
            error: function (response) {
                $cdekInfo.html(`<b>${response.responseText}</b>`)
                callback();
                return;
            }
        })
    };

    function cancelOrder(callback) {
        $.ajax({
            url: `/api/shippings/cdek/register?orderId=${orderId}`,
            type: "DELETE",
            success: function () {
                window.location.reload(false);
            },
            error: function (response) {
                $cdekInfo.html(`<b>${response.responseText}</b>`);
                callback();
                return;
            }
        })
    };

    function orderInfo(callback) {
        $.ajax({
            url: `/api/shippings/cdek/orderInfo?orderId=${orderId}` ,
            type: "GET",
            data: cdekAuth,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response == 'v2_entity_not_found'
                    || response == 'error_validate_im_dep_number_has_already_had_integration') {
                    window.location.reload(false);
                    return;
                }
                $cdekInfo.html(response);
                if(callback) callback();
            },
            error: function (response) {
                $cdekInfo.html(`<b>${response.responseText}</b>`);
                if(callback) callback();
                return;
            }
        })
    };

    function formingReceipt(callback) {
        fetch(`/api/shippings/cdek/formingReceipt?orderId=${orderId}`)
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.json();
            })
            .then(data => {
                $cdekInfo.html(data.Info).show(100);
                if (data.Uuid) {
                    uuidReceipt = data.Uuid;
                    $cdekDownloadReceipt.show();
                }
                callback();
            })
            .catch(resp => resp.json().then(error => {
                $cdekInfo.html(`<b>${error.Message}</b>`).show(100);
                callback();
            }));
    };

    function orderReceipt(callback) {
        fetch(`/api/shippings/cdek/getOrderReceipt?orderId=${orderId}&uuid=${uuidReceipt}`)
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.json();
            })
            .then(data => {
                $cdekInfo.html(data.Info).show(100);
                if (data.Url) {
                    urlReceipt = data.Url;
                    getPdf(urlReceipt, `Receipt ${orderId}`);
                }
                callback();
            })
            .catch(resp => resp.json().then(error => {
                $cdekInfo.html(`<b>${error.Message}</b>`).show(100);
                callback();
            }));
    };
        
    function formingBarcode(format,lang,callback) {
        fetch(`/api/shippings/cdek/formingBarcode?orderId=${orderId}&format=${format}&lang=${lang}`)
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.json();
            })
            .then(data => {
                uuidBarcode = data.Uuid;
                $cdekInfo.html(data.Info).show(100);
                $cdekFormingBarcode.hide(150);
                $cdekDownloadBarcode.show(150);
                callback();
            })
            .catch(resp => resp.json().then(error => {
                $cdekInfo.html(`<b>${error.Message}</b>`).show(100);
                callback();
            }));
    };

    function orderBarcode(callback) {
        fetch(`/api/shippings/cdek/getOrderBarcode?orderId=${orderId}&uuid=${uuidBarcode}`)
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.json();
            })
            .then(data => {
                $cdekInfo.html(data.Info).show(100);
                if (data.Url) {
                    urlBarcode = data.Url;
                    getPdf(urlBarcode, `Barcode ${orderId}`);
                }
                callback();
            })
            .catch(resp => resp.json().then(error => {
                if (error.Message.includes('v2_entity_not_found')) {
                    $cdekFormingBarcode.show(150);
                    $cdekDownloadBarcode.hide(150);
                }
                $cdekInfo.html(`<b>${error.Message}</b>`).show(100);
                callback();
            }));
    };

    function getPdf(url, name) {
        fetch(`/api/shippings/cdek/getPdf?orderId=${orderId}&url=${url}`,
            {
                Method: 'GET',
                headers: {
                  'Content-Type': 'application/octet-stream'
                }
        })
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.blob();
            })
          .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            // the filename you want
            a.download = `${name}.pdf`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
          })
          .catch(resp => resp.json().then(error => {
               $cdekInfo.html(`<b>${error.Message}</b>`).show(100);
                callback();
            }));
    }

    function validateCallCourier(CallCourier) {
        let _result = true;
         $.ajax({
            url: '/api/shippings/cdek/validate',
            type: "POST",
            data: JSON.stringify(CallCourier),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (response) {
                if (!response.isValid) {
                    $cdekCallCourierError.html(`<b style="color:red">${response.message}</b>`).show();
                    _result = false;
                }
            },
            error: function (response) {
                $cdekCallCourierError.html(`<b style="color:red">${errorMsg} </b><span>${response.responseText}</span>`).show()
                _result = false;
            }
         });

        return _result;
    };
    
    function registerCallCourier(request, callback) {
        fetch(`/api/shippings/cdek/intakes?orderId=${orderId}`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)
        })
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.json();
            })
            .then(data => {
                window.location.reload(false);
                return;
            })
            .catch(resp => resp.json().then(error => {
                $cdekCallCourierInfo.html(`<b>${error.Message}</b>`);
                $cdekCourierInfoBlock.show(150);
                $cdekCallCourierModalBox.jqmHide();
                callback();
            }));
    };
    
    function registerCancelCourier(callback) {
        fetch(`/api/shippings/cdek/intakes?orderId=${orderId}`, {
            method: 'DELETE',
            headers: {
              'Content-Type': 'application/json'
            },
        })
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.json();
            })
            .then(data => {
                window.location.reload(false);
                return;
            })
            .catch(resp => resp.json().then(error => {
                $cdekCallCourierInfo.html(`<b>${error.Message}</b>`);
                $cdekCourierInfoBlock.show(150);
                callback();
            }));
    };
    
    function getCourierInfo(callback) {
        fetch(`/api/shippings/cdek/intakes?orderId=${orderId}`, {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json'
            },
        })
            .then(resp => {
                if (!resp.ok) {
                  throw resp;
                }
                return resp.json();
            })
            .then(data => {
                $cdekCallCourierInfo.html(data);
                $cdekCourierInfoBlock.show(150);
                callback();
            })
            .catch(resp => resp.json().then(error => {
                $cdekCallCourierInfo.html(`<b>${error.Message}</b>`);
                $cdekCourierInfoBlock.show(150);
                callback();
            }));
    };

</script>

<% } %>
<% if(postal.ServiceProviderSettings is UspsServiceProviderSettings uspsSettings)
                    {%>

 <label for="uspsInfo"><b>USPS:</b></label>
<div id="uspsInfo" class="message" style="display: none"></div>
<div class="buttons" style="text-align: left">
    <input type="button" value="<%= RM.GetString(RS.Shipping.ShippingProviderInfo.RegisterAnApplication) %>" id="uspsRegOrder" />
</div>
<script>

    const orderId = <%=CurrentOrder.Id%>;
    let $uspsRegOrder = $('#uspsRegOrder');
    let $uspsInfo = $('#uspsInfo');

    $uspsRegOrder.click(function () {
        let self = $(this);
        self.prop('disabled', true);
        registerOrder(() => self.prop('disabled', false));
    });

    function registerOrder(callback) {
        $.ajax({
            url: `/api/shippings/usps/registerOrder?orderId=${orderId}`,
            type: "POST",
            success: function () {
                window.location.reload(false);
            },
            error: function (response) {
                $uspsInfo.html(`<b>${response.responseText}</b>`)
                callback();
                return;
            }
        })
    };
</script>
                  <%  } %>

<% } %>

<% } %>
