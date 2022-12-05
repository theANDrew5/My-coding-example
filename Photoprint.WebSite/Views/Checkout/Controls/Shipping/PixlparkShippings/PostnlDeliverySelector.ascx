<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PostnlDeliverySelector.ascx.cs" Inherits="Photoprint.WebSite.Controls.PostnlDeliverySelector" %>

<fieldset class="profile postal" id="postal_<%: InitializedPostal?.Id ?? 0 %>">
    <ol>
        <li class="shipping-first-name">
            <label for="<%= txtFirstName.ClientID %>"><%=RM.GetString(RS.Shop.Checkout.FirstNameLabel)%></label>
            <asp:TextBox runat="server" ID="txtFirstName" CssClass="text" ValidationGroup="Shipping" />
            <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtFirstName" />				    
            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqFirstName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtFirstName" />
            <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regFirstName" runat="server" ControlToValidate="txtFirstName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
        </li>
        <li class="shipping-last-name">
            <label for="lastName"><%=RM.GetString(RS.Shop.Checkout.LastNameLabel)%></label>
            <asp:TextBox ID="txtLastName" runat="server" ValidationGroup="Shipping" CssClass="text" />
            <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstLastNameCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstNameCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtLastName" />		    
            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqLastName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtLastName" />
            <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.FirstNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regLastName" runat="server" ControlToValidate="txtLastName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
        </li>
        <% if(UseMiddleName){ %>
                <li class="shipping-middle-name">
					<label for="<%=txtMiddleName.ClientID%>"><%=RM.GetString(RS.Shop.Checkout.MiddleNameLabel)%></label>
					<asp:TextBox ID="txtMiddleName" runat="server" ValidationGroup="Shipping" CssClass="text" /> 
                    <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Validators.Required %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqMiddleName" ValidationGroup="Shipping" runat="server" ControlToValidate="txtMiddleName" />
				    <asp:RegularExpressionValidator ErrorMessage="<%$ RM: Common.MiddleNameRequiredCyrillic %>" Display="Dynamic" ForeColor="" CssClass="validator" ValidationGroup="Shipping" Id="regMiddleName" Runat="server" ControlToValidate="txtMiddleName" ValidationExpression="^[а-яёА-ЯЁ][а-яёА-ЯЁ0-9-_\s]*$"/>
                </li>
        <% } %>
        <li class="shipping-phone">
            <label for="phone"><%=RM.GetString(RS.Shop.Checkout.Phone)%></label>
            <asp:TextBox ID="txtPhone" runat="server" ValidationGroup="Shipping" CssClass="text mobile-phone" />	
            <asp:CustomValidator Display="Dynamic" ForeColor="" ID="cstPhoneCorrect" ErrorMessage="<%$ RM: Validators.Invalid %>" runat="server" OnServerValidate="CstPhoneCorrect" CssClass="validator" ValidationGroup="Shipping" ControlToValidate="txtPhone" />
            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPhone" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPhone" />
        </li>
        <li>
            <label for="<%= ddlCountries.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.Country) %></label>
            <asp:DropDownList runat="server" ID="ddlCountries" ValidationGroup="Shipping" data-bind="value: selectedCountry,
                                                                                                     options: availableCountries,
                                                                                                     event: { change: onCountryOrPostalChanged }" />
        </li>
        <li>
            <label for="<%= txtPostalCode.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.PostalCode) %></label>
            <asp:TextBox runat="server" ID="txtPostalCode" ValidationGroup="Shipping" data-bind="value: postalCode,
                                                                                                 event: {change: onCountryOrPostalChanged}
                                                                                                 , valueUpdate: 'afterkeydown'"></asp:TextBox>
            <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqPostalCode" ValidationGroup="Shipping" runat="server" ControlToValidate="txtPostalCode" InitialValue="" />
        </li>
        <li>
            <asp:PlaceHolder runat="server" ID="plhdRegion" Visible="False">
                <label for="<%= ddlRegions.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.Region) %></label>
                <asp:DropDownList runat="server" ID="ddlRegions" ValidationGroup="Shipping" data-bind="value: selectedRegion,
                                                                                                       options: availableRegions,
                                                                                                       event: { change: getCities }" />
                <asp:RequiredFieldValidator ErrorMessage="<%$ RM: Common.EmptyField %>" Display="Dynamic" ForeColor="" CssClass="validator" ID="reqRegion" ValidationGroup="Shipping" runat="server" ControlToValidate="ddlRegions" />
            </asp:PlaceHolder>
        </li>
        <li>
            <label for="<%= ddlCities.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.City) %></label>
            <asp:DropDownList runat="server" ID="ddlCities" ValidationGroup="Shipping" data-bind="value: selectedCity,
                                                                                                  options: availableCities,
                                                                                                  event: { change: onCityChanged }" />
        </li>
        <asp:PlaceHolder runat="server" ID="plhdClient" Visible="False">
            <li>
                <label for="<%= ddlStreets.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.AddressStreet) %></label>
                <asp:DropDownList runat="server" ID="ddlStreets" data-bind="value: selectedStreet,
                                                                            options: availableStreets,
                                                                            event: { change: getHouses }" />
            </li>
            <li>
                <label for="<%= ddlHouses.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.AddressHouse) %></label>
                <asp:DropDownList runat="server" ID="ddlHouses" data-bind="value: selectedHouse,
                                                                           options: availableHouses,
                                                                           event: { change: setAddress }" />
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plhdStorage" Visible="False">
            <li>
                <label for="<%= ddlStorages.ClientID %>"><%= RM.GetString(RS.Shop.Checkout.StorageAddress) %></label>
                <asp:DropDownList runat="server" ID="ddlStorages" data-bind="value: selectedStorage,
                                                                             options: availableStorages,
                                                                             optionsText: 'Value',
                                                                             optionsValue: 'Key',
                                                                             event: { change: setAddress }" />
                <input type="hidden" id="storage" name="storage"/>
            </li>
        </asp:PlaceHolder>
    </ol>
    <input type="hidden" id="address" name="address"/>
</fieldset>

<script type="text/javascript">
    function PostnlViewModel() {
        var self = this;

        self.shippingId = <%= InitializedPostal.Id %>;

        self.availableCountries = ko.observableArray([]);
        self.selectedCountry = ko.observable("");

        self.postalCode = ko.observable("");

        self.availableRegions = ko.observableArray([]);
        self.selectedRegion = ko.observable("");

        self.availableCities = ko.observableArray([]);
        self.selectedCity = ko.observable("");

        self.availableStreets = ko.observableArray([]);
        self.selectedStreet = ko.observable("");

        self.availableHouses = ko.observableArray([]);
        self.selectedHouse = ko.observable("");

        self.availableStorages = ko.observableArray([]);
        self.selectedStorage = ko.observable("");

        self.getCountries = function () {
            $.ajax({
                url: "/api/shippings/postnl/getCountries?shippingId=" + self.shippingId,
                type: "GET",
                error: function(error) {
                    console.log(error.responseText);
                },
                success: function(countries) {
                    self.availableCountries(countries);
                }
            });
        }
        self.getCountries();

        self.onCountryOrPostalChanged = function () {
            if (self.postalCode().length > 0) {
                if (<%= InitializedPostal.IsRegionRequired.ToString().ToLowerInvariant() %> === "true")
                    self.getRegions();
                else
                    self.getCities();
            }
        }

        self.getRegions = function () {
            if (self.selectedCountry !== "" && self.selectedCountry() != undefined) {
                $.ajax({
                    url: "/api/shippings/postnl/getRegions?shippingId=" + self.shippingId + "&postalCode=" + self.postalCode() + "&country=" + self.selectedCountry(),
                    type: "GET",
                    error: function(error) {
                        console.log(error.responseText);
                    },
                    success: function (cities) {
                        self.availableCities(cities);
                    }
                });
            }
        }

        self.getCities = function () {
            if (self.selectedCountry() !== "" && self.selectedCountry() != undefined) {
                $.ajax({
                    url: "/api/shippings/postnl/getCities?shippingId=" + self.shippingId + "&postalCode=" + self.postalCode() + "&country=" + self.selectedCountry() + "&region=" + self.selectedRegion(),
                    type: "GET",
                    error: function(error) {
                        console.log(error.responseText);
                    },
                    success: function (cities) {
                        self.availableCities(cities);
                    }
                });
            }
        }

        self.onCityChanged = function() {
            self.getStreets();
            self.getStorages();
        }

        self.getStreets = function() {
            if (self.selectedCity() !== "" && self.selectedCity() != undefined) {
                $.ajax({
                    url: "/api/shippings/postnl/getStreets?shippingId=" + self.shippingId + "&postalCode=" + self.postalCode() + "&country=" + self.selectedCountry() + "&region=" + self.selectedRegion() + "&city=" + self.selectedCity(),
                    type: "GET",
                    error: function(error) {
                        console.log(error.responseText);
                    },
                    success: function (data) {
                        self.availableStreets(data);
                    }
                });
            }
        }

        self.getStorages = function () {
            if (self.selectedCity() !== "" && self.selectedCity() != undefined) {
                $.ajax({
                    url: "/api/shippings/postnl/getStorages?shippingId=" + self.shippingId + "&postalCode=" + self.postalCode() + "&country=" + self.selectedCountry() + "&region=" + self.selectedRegion() + "&city=" + self.selectedCity(),
                    type: "GET",
                    error: function(error) {
                        console.log(error.responseText);
                    },
                    success: function (data) {
                        self.availableStorages(data);
                        console.log("lel", self.selectedStorage());
                    }
                });
            }
        }

        self.getHouses = function () {
            if (self.selectedStreet() !== "" && self.selectedStreet() != undefined) {
                $.ajax({
                    url: "/api/shippings/postnl/getHouses?shippingId=" + self.shippingId + "&postalCode=" + self.postalCode() + "&country=" + self.selectedCountry() + "&region=" + self.selectedRegion() + "&city=" + self.selectedCity() + "&street=" + self.selectedStreet(),
                    type: "GET",
                    error: function(error) {
                        console.log(error.responseText);
                    },
                    success: function(houses) {
                        self.availableHouses(houses);
                    }
                });
            }
        }

        self.setAddress = function() {
            var address = document.getElementById("address");
            address.value = JSON.stringify({
                Country: self.selectedCountry(),
                City: self.selectedCity(),
                Street: self.selectedStreet(),
                House: self.selectedHouse(),
                PostalCode: self.postalCode()
            });
            var storage = document.getElementById("storage");
            storage.value = JSON.stringify(self.selectedStorage());
        }
    }

    ko.applyBindings(new PostnlViewModel, document.getElementById("postal_<%: InitializedPostal?.Id ?? 0 %>"));
</script>