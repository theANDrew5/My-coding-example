function ShippingAddressPartInput(title, textKey, onChange) {
    let self = this;
    self.title = title;
    self.textKey = textKey;
    self.onChange = onChange;

    self.possibleValues = ko.observableArray();
    self.selectedValue = ko.observable();
    self.selectedValueInput = ko.observable();
    self.selectedValueResult = ko.pureComputed(function() {
        let val = self.selectedValue();
        if (self.readonly()) {
            return val != null ? val[self.textKey] : "";
        } else {
            if (self.inputVisible()) {
                return self.selectedValueInput();
            } else {
                return val != null ? val[self.textKey] : "";
            }
        }
    });
    
    self.selectedValue.subscribe(function() { if (typeof self.onChange == "function") self.onChange(); });
    
    self.inputVisible = ko.observable(false);
    self.readonly = ko.observable(false);
    self.readonlyAuto = ko.observable(false);

    self.update = function(data, defaultValue) {
        self.possibleValues(data);
        let initValue = null;
        if (!!defaultValue) {
            initValue = defaultValue;
        } else {
            initValue = data != null && data.length > 0 ? data[0] : null;
        }
        self.selectedValue(initValue);
        if (self.readonlyAuto()) {
            if (initValue == null || typeof initValue == "string") {
                self.inputVisible(true);
                self.selectedValueInput(initValue);
            } else {
                self.inputVisible(data != null && data.length === 1 && initValue[self.textKey].length === 0);
                self.readonly(data != null && data.length === 1 && initValue[self.textKey].length > 0);
            }
        }
    };
    self.resetState = function() {
        self.possibleValues(null);
        self.selectedValue(null);
        self.selectedValueInput(null);
        self.readonly(false);
        self.readonlyAuto(false);
        self.inputVisible(false);
    }
}
function ShippingAddressLineInput(title, textKey, propsKey, onChange) {
    let self = this;
    self.title = title;
    self.textKey = textKey;
    self.onChange = onChange;

    self.possibleValues = ko.observableArray();
    self.selectedValue = ko.observable();
    self.selectedValueInput = ko.observable();
    self.selectedValueResult = ko.pureComputed(function() {
        let val = self.selectedValue();
        if (self.readonly()) {
            return { id: val["Id"], address: val[self.textKey], properties: val[propsKey] };
        } else {
            if (self.inputVisible()) {
                if (self.isSeparateAddress()) {
                    return {
                        id: val != null ? val["Id"] : null, 
                        street: self.selectedStreetInput(),
                        house: self.selectedHouseInput(),
                        flat: self.selectedFlatInput(), 
                        properties: val[propsKey]
                    };
                } else {
                    return { id: val != null ? val["Id"] : null, address: self.selectedValueInput(), properties: val[propsKey] };
                }
            }
            else 
                return { id: val["Id"], address: val[self.textKey], properties: val[propsKey]};
        }
    });
    
    self.selectedValue.subscribe(function() { if (typeof self.onChange == "function") self.onChange(); });
    
    self.inputVisible = ko.observable(false);
    self.readonly = ko.observable(false);
    self.isSeparateAddress = ko.observable(false);

    self.selectedStreetInput = ko.observable();
    self.selectedHouseInput = ko.observable();
    self.selectedFlatInput = ko.observable();

    self.update = function(data, shippingConfig) {
        self.possibleValues(data);
        let initValue = {};
        if (!shippingConfig || !shippingConfig.addressInfo.address) {
            initValue = data != null && data.length > 0 ? data[0] : null;
        } else {
            initValue.AddressLine = shippingConfig.addressInfo.address;
            initValue.Id = shippingConfig.addressInfo.id;
            
            self.selectedStreetInput(shippingConfig.addressInfo.street);
            self.selectedHouseInput(shippingConfig.addressInfo.house);
            self.selectedFlatInput(shippingConfig.addressInfo.flat);
            self.selectedValueInput(initValue.Address);
        }

        self.selectedValue(initValue);
    };
    
    self.resetState = function() {
        self.possibleValues(null);
        self.selectedValue(null);
        self.selectedValueInput(null);
        self.readonly(false);
        self.inputVisible(false);
        self.isSeparateAddress(false);
    }
}

function ShippingAddressSelector(defaultData) {
    let self = this;
    self.defaultData = defaultData;
    self.isLoading = ko.observable(false);
    self.selectedShipping = ko.observable(null);
    self.shippingData = ko.observable(null);
    self.initAddress = ko.observable(null);
    
    function getDefaultElem(key, arr, val) {
        if (!self.selectedShipping() 
            || self.selectedShipping().Id !== self.defaultData.shippingId)
            return null;
        
        if (!arr || !val) return val;
        
        let result =  ko.utils.arrayFirst(arr, function(item){
            return item[key] === val;
        });
        
        if (!result)
            result = val;
        
        return result;
    }
    
    function getDefaultAddress() {
        if (!self.selectedShipping() || self.selectedShipping().Id !== self.defaultData.shippingId) 
            return null;
        else 
            return self.defaultData;
    }

    self.onCountryChanged = function () { 
        let isValueSelected = !!self.countryInput.selectedValue();
        self.regionInput.update( isValueSelected ? self.countryInput.selectedValue().Regions : null, 
        getDefaultElem("Region",  isValueSelected ? self.countryInput.selectedValue().Regions : null, self.defaultData.state)); 
    };
    self.onRegionChanged = function () { 
        let isValueSelected = !!self.regionInput.selectedValue();
        self.cityInput.update(isValueSelected ? self.regionInput.selectedValue().Cities : null,
        getDefaultElem("City",  isValueSelected ? self.regionInput.selectedValue().Cities : null, self.defaultData.city));
    };
    self.onCityChanged = function () { 
        self.addressLineInput.update(!!self.cityInput.selectedValue() ? self.cityInput.selectedValue().Addresses : null, getDefaultAddress()); 
    };
    self.onAddressChanged = function () {

    };

    self.countryInput = new ShippingAddressPartInput("Страна:", "Country", self.onCountryChanged);
    self.regionInput = new ShippingAddressPartInput("Регион:", "Region", self.onRegionChanged);
    self.cityInput = new ShippingAddressPartInput("Город:", "City", self.onCityChanged);
    self.addressLineInput = new ShippingAddressLineInput("Адрес:", "AddressLine", "DeliveryProperties", self.onAddressChanged);

    self.initShippingData = function(shipping, data) {
        self.selectedShipping(shipping);
        self.shippingData(data);

        self.countryInput.resetState();
        self.regionInput.resetState();
        self.cityInput.resetState();
        self.addressLineInput.resetState();

        if (self.selectedShipping().Type === 1) { //point
            self.countryInput.readonly(true);
            self.regionInput.readonly(true);
            self.cityInput.readonly(true);
            self.addressLineInput.readonly(true);
        } 
        else if (self.selectedShipping().Type === 2) { //courier
            self.countryInput.readonlyAuto(true);
            self.regionInput.readonlyAuto(true);
            self.cityInput.readonlyAuto(true);
            self.addressLineInput.inputVisible(true);
        } 
        else { // postal
            if (data.Addresses == null) {
                self.countryInput.inputVisible(true);
                self.regionInput.inputVisible(true);
                self.cityInput.inputVisible(true);
                self.addressLineInput.inputVisible(true);
            } else {
                if (self.shippingData().IsClientDelivery) {
                    self.countryInput.readonlyAuto(true);
                    self.regionInput.readonlyAuto(true);
                    self.cityInput.readonlyAuto(true);
                    self.addressLineInput.inputVisible(true);
                } else {
                    self.countryInput.readonlyAuto(true);
                    self.regionInput.readonlyAuto(true);
                    self.cityInput.readonlyAuto(true);
                }
            }
        }

        self.countryInput.update(data.Addresses != null ? data.Addresses.Countries : null, 
            getDefaultElem("Country",  data.Addresses != null ? data.Addresses.Countries : null, self.defaultData.country));
        self.addressLineInput.isSeparateAddress(data.IsClientDelivery && data.IsSeparateAddress);
        self.isLoading(false);
    };
    

    self.isMyLocationAvailable = ko.pureComputed(function() { return false; if (navigator.geolocation) return true; return false; });
    self.onMyLocationFound = function(position) { console.log(position);};
    self.findMyLocation = function() { navigator.geolocation.getCurrentPosition(self.onMyLocationFound); };

    self.shippingDetailsCache = new Array();
    self.loadShippingDetails = function(shipping) {
        if (typeof shipping === 'undefined' || shipping === null || self.isLoading()) return;
        self.isLoading(true);
        const dataInCache = self.shippingDetailsCache[shipping.Id];
        if (typeof dataInCache !== "undefined") {
            self.initShippingData(shipping, dataInCache);
            return;
        }
        $.ajax({ type: "GET", url: "/api/shipping/addressList?shippingId=" + shipping.Id,
            success: function (result) {
                self.shippingDetailsCache[result.ShippingId] = result;
                if (shipping.Id === result.ShippingId) {
                    self.initShippingData(shipping, result);
                }
            }
        });
    };

    self.resultAddress = ko.pureComputed(function () {
        if (self.isLoading() || self.shippingData() == null) return null;
        const address = self.addressLineInput.selectedValueResult();
        const country = self.countryInput.selectedValueResult();
        const region = self.regionInput.selectedValueResult();
        const city = self.cityInput.selectedValueResult();
        const result = new SelectedShippingAddress(self.selectedShipping().Id, self.selectedShipping().Title, country, region, city, address);
        if (!self.initAddress() && !!self.defaultData.shippingId) {
            self.initAddress(result);
        }
        return result;
    });
}
class ShippingSelectorModal {
    constructor (photolabId, defaultShippingAddress, onShippingSelectedCallback) {
        let self = this;
        self.photolabId = photolabId;
        self.addressSelector = new ShippingAddressSelector(defaultShippingAddress);

        self.shippings = ko.observableArray();
        self.shippingTypes = ko.observableArray();
        self.selectedShipping = ko.observable(null);
        self.selectedShipping.subscribe(() => self.addressSelector.loadShippingDetails(self.selectedShipping()));

        self.isShippingsLoaded = false;
        self.isShippingsLoading = ko.observable();

        self.onShippingSelectedCallback = onShippingSelectedCallback;

        self.isResultAddressValid = ko.pureComputed(() => {
            const addr = self.addressSelector.resultAddress();
            return addr !== null && addr.isValid();
        });

        $("#selectShippingModal").jqm({ closeClass: "button-close" });
        ko.applyBindings(self, document.getElementById("selectShippingModal"));

        self.loadShippings(defaultShippingAddress);
    }
    
    loadShippings(shippingConfig) {
        if (this.isShippingsLoaded || this.isShippingsLoading()) return;
        this.isShippingsLoading(true);
        const self = this;
        $.ajax({
            type: "GET",
            url: "/api/shipping/list?photolabId=" + self.photolabId + "&totalPrice=null",
            success: function (result) {
                self.shippings(result);
                self.shippingTypes([]);
                for (let i = 0; i < self.shippings().length; i++) {
                    if (self.shippingTypes.indexOf(self.shippings()[i].TypeString) < 0)
                        self.shippingTypes.push(self.shippings()[i].TypeString);
                }
                if (self.shippings().length > 0) {
                    let defaultShipping = null;
                    if (!!shippingConfig.shippingId) {
                        defaultShipping = ko.utils.arrayFirst(self.shippings(), function (item) {
                            return item.Id === shippingConfig.shippingId
                        });
                    }
                    
                    if (!!defaultShipping) {
                        self.selectedShipping(defaultShipping);
                    }
                    else if (self.shippings().length > 0) {
                        self.selectedShipping(self.shippings()[0]);
                    }
                }

                self.isShippingsLoading(false);
                self.isShippingsLoaded = true;
            }
        });
    }
            
    selectAddressHandler() {
        if (this.isResultAddressValid()) {
            let addr = this.addressSelector.resultAddress();
            this.onShippingSelectedCallback(addr);
            this.hideModal();
        }
    }

    hideModal() { $('#selectShippingModal').jqmHide(); }
    showModal() { $('#selectShippingModal').jqmShow(); }
}

function SelectedShippingAddress(shippingId, shippingTitle, country, region, city, addressInfo) {
    let self = this;
    self.shippingId = shippingId;
    self.shippingTitle = shippingTitle;
    self.country = country;
    self.region = region;
    self.city = city;
    self.addressInfo = addressInfo;

    self.addressLine = typeof self.addressInfo.address != "undefined"
            ? self.addressInfo.address
            : self.addressInfo.street + ', ' + self.addressInfo.house + ', ' + self.addressInfo.flat;

    self.fullAddress = function() {
        let result = self.addressLine;
        if (!!self.city && self.city.length > 0)
            result += '; ' + self.city;
        if (!!self.region && self.region.length > 0)
            result += '; ' + self.region;
        if (!!self.country && self.country.length > 0)
            result += '; ' + self.country;

        return result;
    };

    self.isValid = function() {
        if (self.shippingId === null || self.shippingId === 0 || self.addressInfo === null) return false;

        const address = typeof self.addressInfo.address != "undefined"
            ? self.addressInfo.address
            : self.addressInfo.street + ', ' + self.addressInfo.house + ', ' + self.addressInfo.flat;
        return address != null && address.length > 0;
    }
}

class ShippingSelector {
    constructor(photolabId, defaultShippingAddress, onShippingAddressSelected) {
        this.photolabId = photolabId;
        this.defaultShippingAddress = defaultShippingAddress;
        this.onShippingAddressSelected = onShippingAddressSelected;

        let selectedAddress = null;
        if (this.defaultShippingAddress.shippingId > 0) {
            var addr = this.defaultShippingAddress;
            selectedAddress = new SelectedShippingAddress(addr.shippingId, addr.title, addr.country, addr.region, addr.city, addr.addressInfo);
        }
        this.selectedShippingAddress = ko.observable(selectedAddress);


        this.shippingPrice = ko.observable(0.00);
        this.shippingPriceString = ko.pureComputed(() => {
            if (window.pxpa.frontend !== null) return window.pxpa.frontend.getPriceString(this.shippingPrice());
            return this.shippingPrice().toString();
        });
        this.shippingPriceError = ko.observable("");
        this.serializedShippingInfo = ko.computed(() => { 
            return ko.toJSON(this.selectedShippingAddress); });
        
        this.isShippingSelected = ko.computed({
           read: () => {
               return !!this.selectedShippingAddress() && !!this.selectedShippingAddress().shippingId 
                   && !!this.selectedShippingAddress().shippingId > 0;
           },
           owner: this 
        });
        
        this.modal = null;
        this.isModalLoading = ko.observable(false);
        
        if (this.selectedShippingAddress() !== null && typeof this.onShippingAddressSelected === "function") {
            setTimeout(() => { this.onShippingAddressSelected(this.selectedShippingAddress()); }, 500);
        }
        

        ko.applyBindings(this, document.getElementById("shippingSelector"));
    }

    show() {
        if (this.modal !== null) { this.modal.showModal(); return; }
        if (this.isModalLoading()) return;
        this.isModalLoading(true);
        $("body").addClass('loading-wheel');

        $.ajax({
            type: "GET", url: "/modals/shippingAddressSelector", error: function (xhr, textStatus, errorThrown) { console.log(errorThrown); },
            success: (data) => {
                var wrapper = $("#pxp-modal-box-form-wrapper").length > 0 ? $("#pxp-modal-box-form-wrapper") : $("body");
                $(data).appendTo(wrapper);
                this.modal = new ShippingSelectorModal(this.photolabId, this.defaultShippingAddress, (a) => this.onShippingSelectedCallback(a));
                this.modal.addressSelector.initAddress.subscribe((newVal) => {
                    if (newVal) {
                        this.selectedShippingAddress(newVal);
                    }
                });
                $("body").removeClass('loading-wheel');
                this.modal.showModal();
                this.isModalLoading(false);

            }
        });
    }

    reset() { this.selectedShippingAddress(null); }
    
    onShippingSelectedCallback(selectedShippingAddress) {
        if (!selectedShippingAddress || !selectedShippingAddress.isValid()) return; // suppress phantom callback
        this.selectedShippingAddress(selectedShippingAddress);
        if (typeof this.onShippingAddressSelected === "function")
            this.onShippingAddressSelected(this.selectedShippingAddress());
    }
}

export {ShippingSelector};