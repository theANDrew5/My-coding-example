<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExgarantRegisterController.ascx.cs" Inherits="Photoprint.WebSite.Admin.Views.Shipping.Controls.ExgarantRegisterController" %>


<asp:PlaceHolder ID="plhdExgarantError" Visible="false" runat="server">
	<div class="message error">
		<p><asp:Literal id="litExgarantError" runat="server" /></p>
	</div>
</asp:PlaceHolder>
    
<div class="modal-box" id="ExGarantDeliveryForm" data-bind="afterHtmlRender: initPlugins">
    <h4><%:RM.GetString(RS.Order.Info.ExGarant.EditDocumentFields)%></h4>
    <fieldset>
        <ul>
            <li>
                <div class="Ab">
                    <div class="Ab-A">
                        <label for="deliveryExtensionNumber">Внутренний номер</label>
                        <input id="deliveryExtensionNumber" class="text block" type="text" data-bind="value: ExtensionNumber"/>
                    </div>
                    <div class="Ab-b">
                        <label for="deliveryPlaces">Кол-во мест:</label>
                        <input id="deliveryPlaces" class="text block" type="text" data-bind="value: Places"/>
                    </div>
                </div>
            </li>
            <li>
                <div class="Abc">
                    <div class="Abc-A">
                        <label for="deliveryDate">Дата доставки</label>
                        <input id="deliveryDate" class="text block" type="text" 
                            data-bind="datepicker: DeliveryDate, datepickerOptions: ddOptions"/>
                    </div>
                    <div class="Abc-b">
                        <label for="deliveryIntervalStart">Время с:</label>
                        <input id="deliveryIntervalStart" class="text block" type="text" data-bind="value: DeliveryIntervalStart"/>
                    </div>
                    <div class="Abc-c">
                        <label for="deliveryIntervalEnd">до:</label>
                        <input id="deliveryIntervalEnd" class="text block" type="text" data-bind="value: DeliveryIntervalEnd"/>
                    </div>
                </div>
            </li>
            <li>
                <label for="deliveryPhone">Номер телефона:</label>
                <input id="deliveryPhone" class="text block" type="text" data-bind="value: Phone"/>
            </li>
            <li>
                <span class="checkbox">
                    <input id="returnDocuments" type="checkbox" data-bind="checked: ReturnDocuments"/>
                    <label for="returnDocuments">возвращать документы</label>
                </span>
            </li>
            <li>
                <span class="checkbox">
                    <input id="informBySms" type="checkbox" data-bind="checked: InformBySms"/>
                    <label for="informBySms">информировать по SMS</label>
                </span>
            </li>
            <li>
                <span class="checkbox">
                    <input id="cashPaid" type="checkbox" data-bind="checked: Cash"/>
                    <label for="cashPaid">оплата наложным платежом</label>
                </span>
            </li>
            <li>
                <span class="checkbox">
                    <input id="declaredPrice" type="checkbox" data-bind="checked: DeclaredPrice"/>
                    <label for="declaredPrice">объявленная стоимость</label>
                </span>
            </li>
        </ul>
    </fieldset>
    <div class="buttons">
        <a href="#" class="semilink big button-close"><%=RM.GetString(RS.General.Close)%></a>
        <asp:Button CssClass="small" runat="server"
                    OnClick="RegisterExgarantClick" Text="<%$ RM: Order.Info.ExGarant.SendRequest %>" />
    </div>
</div>

<% var objectId = (CurrentOrder.Properties.ShippingRegistrationResult as ExgarantRegistrationResult)?.ObjectId;%>
<% if (string.IsNullOrWhiteSpace(CurrentOrder.TrackingNumber) && string.IsNullOrWhiteSpace(objectId)) { %>
<div class="buttons">
    <button class="small" id="viewForm"><%: RM.GetString(RS.Order.Info.ExGarant.RegisterDocuments) %></button>
</div>
<% }else { %>
    <ol>
        <li>
            <a target="_blank" href="http://www.ex-garant.ru/hydra/index.php?action=delivery&order=<%:objectId %>">Редактирование заявки</a>
        </li>
        <li>
            <a target="_blank" href="http://www.ex-garant.ru/hydra/print2.php?order=<%:objectId %>">Квитанция 1</a>
        </li>
        <li>
            <a target="_blank" href="http://www.ex-garant.ru/hydra/print.php?order=<%:objectId %>">Квитанция 2</a>
        </li>
        <li>
            <a target="_blank" href="http://www.ex-garant.ru/hydra/label.php?order=<%:objectId %>">Ярлык</a>
        </li>
        <li>
            <a target="_blank" href="http://www.ex-garant.ru/hydra/label_xls.php?order=<%:objectId %>">Ярлык в XLS</a>
        </li>
        <li>
            <a target="_blank" href="http://www.ex-garant.ru/hydra/inc/cash_cheques_print.php?order=<%:objectId %>">Товарный чек</a>
        </li>
    </ol>
<div class="buttons">
    <asp:Button CssClass="small" runat="server" OnClick="UpdateExgarantStatusClick" Text="<%$ RM: Order.Info.ExGarant.UpdateStatus %>"/>
    <button class="small" id="viewForm"><%: RM.GetString(RS.Order.Info.ExGarant.UpdateDocuments) %></button>
</div>
<% } %>
    


<asp:HiddenField ID="txtDeliveryFields" runat="server"/>

<script type="text/javascript">
    ko.bindingHandlers.afterHtmlRender = {
        update: function (element, valueAccessor) {
            (valueAccessor())(element);
        }
    }
    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().datepickerOptions || {};

            $(element).datepicker(options);
            $(element).datepicker("widget").hide();

            ko.utils.registerEventHandler(element, "changeDate", function () {
                var observable = valueAccessor();
                observable($(element).datepicker("getDate"));
            });

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).datepicker("destroy");
            });
        },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                $el = $(element);

            if (String(value).indexOf('/Date(') === 0) {
                value = new Date(parseInt(value.replace(/\/Date\((.*?)\)\//gi, "$1")));
            }

            var current = $el.datepicker("getDate");
            if (value - current !== 0) {
                $el.datepicker("setDate", value);
            }
        }
    };

    function ExGarantFields(fields) {
        var self = this;
        self.Phone = ko.observable(fields.Phone);
        self.Places = ko.observable(fields.Places);
        self.DeliveryDate = ko.observable(new Date(fields.DeliveryDate));
        self.DeliveryIntervalStart = ko.observable(fields.DeliveryIntervalStart);
        self.DeliveryIntervalEnd = ko.observable(fields.DeliveryIntervalEnd);
        self.ExtensionNumber = ko.observable(fields.ExtensionNumber);
        self.ReturnDocuments = ko.observable(fields.ReturnDocuments);
        self.InformBySms = ko.observable(fields.InformBySms);
        self.Cash = ko.observable(fields.Cash);
        self.DeclaredPrice = ko.observable(fields.DeclaredPrice);


        self.ddOptions = {
            dateFormat: 'dd-mm-yy',
            minDate: new Date(),
            onSelect: function () {
                self.DeliveryDate($("#" + arguments[1].id).datepicker("getDate"));
            },
            beforeShow: function () {
                var datePickerWrapper = $("#ui-datepicker-div");
                if (!datePickerWrapper.hasClass("in-modal")) {
                    $("#ui-datepicker-div").addClass("in-modal");
                }
            }
        }

        self.setState = ko.computed(function() {
            $("#<%= txtDeliveryFields.ClientID%>").val(ko.toJSON(self));
        });

        self.initPlugins = function () {
            //add plugins if required
        }
    }

    $("#viewForm").on("click", function(e) {
        e.preventDefault();
        $("#ExGarantDeliveryForm").jqmShow();
    });
    var exgarantFields = new ExGarantFields(JSON.parse($("#<%: txtDeliveryFields.ClientID%>").val()));
    $(function() {
        ko.applyBindings(exgarantFields, $("#ExGarantDeliveryForm").get(0));
    });
</script>