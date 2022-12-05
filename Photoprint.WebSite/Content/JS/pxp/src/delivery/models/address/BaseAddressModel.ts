
import { AddressService, SuggestType} from "../../_services/AddressService";
import { CityInfo, ICityState } from "../city/CityInfo";
import { IPxpLatLng } from "../_maps/_baseMapManager";
import { BaseDeliveryMap } from "../types/standartDeliveryType/_baseStandartType/viewModels/BaseDeliveryMap";
import { IAddressSuggest, SuggestedStreetItem } from "./suggests/SuggestedStreetItem";
import { SuggestHouseDropdown } from "./suggests/SuggestHouseDropdown";
import { SuggestStreetDropdown } from "./suggests/SuggestStreetDropdown";
import { SuggestFullAddressDropdown } from "./suggests/SuggestFullAddressDropdown";
import { DeliveryMessageBlockType, DeliveryMessagesManager, IMessagesMaster } from "../../controllers/MessagesManager";
import { CountryInfo } from "../city/CountrySelector";
import { IPxpAddress, IPxpDeliveryAddress, IPxpMapBounds } from "../types/_base/BaseDeliveryPointItem";


export class BaseAddressModel {
    private _handlers: IAddressModelHandlers | null;
    private _addressLocalStorageKey = "pxp_delivery_beforeSelectedAddress";

    isRewriteProcess: boolean;
    isAddressLinesView: boolean;
    fullAddressDropdown: SuggestFullAddressDropdown | null;
    streetDropdown: SuggestStreetDropdown | null;
    houseDropdown: SuggestHouseDropdown | null;

    messagesMaster: IMessagesMaster;
    messagesManager: DeliveryMessagesManager;

    currentCity: KnockoutObservable<CityInfo | null>;
    currentCountry: KnockoutObservable<CountryInfo>;

    isPostcodeVisible: KnockoutObservable<boolean>;
    canFindeAddress: KnockoutObservable<boolean>;
    isAddressCanBeUsed: KnockoutObservable<boolean>;
    lat: KnockoutObservable<number | null>;
    lon: KnockoutObservable<number | null>;
    country: KnockoutObservable<string>;
    street: KnockoutObservable<string | null>;
    house: KnockoutObservable<string>;
    flat: KnockoutObservable<string | null>;
    addressLine1: KnockoutObservable<string>;
    addressLine2: KnockoutObservable<string | null>;
    postalCode: KnockoutObservable<string>;
    addressPointData: KnockoutObservable<IPointData | null>;

    houseFocus: KnockoutObservable<boolean>;
    flatFocus: KnockoutObservable<boolean>;
    addressLine1Focus: KnockoutObservable<boolean>;

    isAddressCorrectForExternalCalculations: KnockoutObservable<boolean>;
    // localization
    readonly loc_addressStreetLabel: string;
    readonly loc_addressHouseLabel: string;
    readonly loc_addressFlatLabel: string;
    readonly loc_addressAddressLine1Label: string;
    readonly loc_addressAddressLine2Label: string;
    readonly loc_addressPostalCodeLabel: string;
    readonly loc_noDeliveryAddressAlert: string;

    // comp
    isAddressFull: KnockoutComputed<boolean>;
    isFullDropdownVisible: KnockoutReadonlyComputed<boolean>;
    isStreetDropdownVisible: KnockoutReadonlyComputed<boolean>;
    isHouseDropdownVisible: KnockoutReadonlyComputed<boolean>;
    houseText: KnockoutReadonlyComputed<string>;

    constructor(currentCity: KnockoutObservable<CityInfo | null>, currentCountry: KnockoutObservable<CountryInfo>, isAddressLineView: boolean, isPostcodeVisible: boolean, localization: IAddressModelLocalization, master: IMessagesMaster) {
        //bind
        this.finalState = this.finalState.bind(this);
        this.findStreets = this.findStreets.bind(this);
        this.setNewAddress = this.setNewAddress.bind(this);
        this.setNewAddressFromMap = this.setNewAddressFromMap.bind(this);
        this._setDataInFields = this._setDataInFields.bind(this);
        this._selectAddressFromSuggestDropdown = this._selectAddressFromSuggestDropdown.bind(this);
        this._getPointData = this._getPointData.bind(this);

        this._handlers = null;

        this.houseFocus = ko.observable(false);
        this.flatFocus = ko.observable(false);
        this.addressLine1Focus = ko.observable(false);

        this.isRewriteProcess = false;
        this.isAddressLinesView = isAddressLineView;

        this.fullAddressDropdown = !this.isAddressLinesView? null : new SuggestFullAddressDropdown(currentCity, {
            selectItem: this._selectAddressFromSuggestDropdown,
            nextFocusElement: this.addressLine1Focus
        });
        this.streetDropdown = this.isAddressLinesView? null : new SuggestStreetDropdown(currentCity, {
            selectItem: this._selectAddressFromSuggestDropdown,
            nextFocusElement: this.houseFocus
        });
        this.houseDropdown = this.isAddressLinesView? null : new SuggestHouseDropdown(currentCity, {
            selectItem: this._selectAddressFromSuggestDropdown,
            nextFocusElement: this.flatFocus
        });

        this.messagesMaster = master;
        this.messagesManager = this.messagesMaster.getOrCreateManager(DeliveryMessageBlockType.Address);

        this.canFindeAddress = ko.observable(true);
        this.isAddressCanBeUsed = ko.observable(true);

        this.currentCity = currentCity;
        this.currentCountry = currentCountry;

        this.isPostcodeVisible = ko.observable(isPostcodeVisible);
        this.lat = ko.observable(null);
        this.lon = ko.observable(null);
        this.country = ko.observable("");
        this.street = ko.observable(null);
        this.house = ko.observable("");
        this.flat = ko.observable(""); 
        this.addressLine1 = ko.observable("");
        this.addressLine2 = ko.observable("");
        this.postalCode = ko.observable("");
        this.addressPointData = ko.observable(null);
        this.isAddressCorrectForExternalCalculations = ko.observable(false);

        // localization
        this.loc_addressStreetLabel = localization.AddressStreetLabel;
        this.loc_addressHouseLabel = localization.AddressHouseLabel;
        this.loc_addressFlatLabel = localization.AddressFlatLabel;
        this.loc_addressAddressLine1Label = localization.AddressLine1Label;
        this.loc_addressAddressLine2Label = localization.AddressLine2Label;
        this.loc_addressPostalCodeLabel = localization.PostalCodeLabel;
        this.loc_noDeliveryAddressAlert = localization.NoDeliveryAddressAlert;

        // comp
        this.isAddressFull = ko.pureComputed(this._addressFullComp, this);
        this.isFullDropdownVisible = ko.pureComputed(this._isFullDropdownVisibleComp, this)
        this.isStreetDropdownVisible = ko.pureComputed(this._isStreetDropdownVisibleComp, this);
        this.isHouseDropdownVisible = ko.pureComputed(this._isHouseDropdownVisibleComp, this);
        this.houseText = ko.pureComputed(this._houseTextComp, this);

        // subscribes
        //TODO request ratelimiter
        this.street.subscribe((val) => { if (!this.isRewriteProcess && !this.isAddressLinesView && val!=null) this.findStreets(val); }, this);
        this.houseText.subscribe((val) => { if (!this.isRewriteProcess && !this.isAddressLinesView) this.findHouses(val); }, this);
        this.addressLine1.subscribe((val) => { if (!this.isRewriteProcess && this.isAddressLinesView) this.findAddresses(val); }, this);
        this.addressLine2.subscribe((val) => { if (!this.isRewriteProcess && this.isAddressLinesView) this.flat(val); }, this);
        this.isAddressFull.subscribe((val) => { this._setAddressPointDataReady(val)});
        this.addressPointData.subscribe((val) => {
            if (val == null) {
                this._handlers?.map.createPointOnMap?.(null);
            }
        }, this);
        this.currentCity.subscribe((val) => {
            if (val == null) {
                this.street("");
                this.house("");
                this.flat("");
                this.postalCode("");
                this.addressLine1('');
                this.addressLine2('');
            }
        });
        // extends
        this.isAddressCorrectForExternalCalculations.extend({ notify: 'always' });
    }

    initHandlers(handlers: IAddressModelHandlers) {
        this._handlers = handlers;
    }

    getAddressBefore() {
        const dataFromLocalStorage = localStorage.getItem(this._addressLocalStorageKey);
        if (dataFromLocalStorage == null || dataFromLocalStorage.length === 0) return;
        const addressBefore = <IPointData>JSON.parse(dataFromLocalStorage);
        if (addressBefore != null) {
            if (!this.currentCity()?.equals(addressBefore.city)) {
                localStorage.removeItem(this._addressLocalStorageKey);
                return;
            } 
            this._setDataInFields(addressBefore);
            this._handlers?.map.createPointOnMap?.(addressBefore);
        }
    }

    finalState(): IPxpAddress {
        localStorage.setItem(this._addressLocalStorageKey, JSON.stringify(this._getPointData()));

        return {
            Country: this.currentCity()!.country,
            Region: this.currentCity()!.region,
            City: this.currentCity()!.title,
            Street: this.street(),
            House: this.house(),
            Flat: this.flat(),
            AddressLine: this.addressLine1(),
            PostalCode: this.postalCode(),
            Latitude: this.lat()?.toString() ?? null,
            Longitude: this.lon()?.toString() ?? null,
        }
    }

    protected findAddresses(query: string) {
        this.flat("");
        this.postalCode('');

        if (query.length === 0) {
            this.fullAddressDropdown?.update(null);
            this.house("");
            return;
        }
        const city = this.currentCity()?.title;
        if (city == null || city === "") return;
        const splitedQuery = query.split(", ", 2);
        const searchStreet = splitedQuery[0] !== this.street();
        if (searchStreet) {
            this.house("");
        }
        AddressService.findSuggestAddresses(
            searchStreet? splitedQuery[0] : splitedQuery[1],
            this.currentCity()!.getState(),
            searchStreet? SuggestType.Street : SuggestType.House,
            searchStreet? undefined : this.street())
            .then((result) => {
                const list = result?.map((obj) => { return new SuggestedStreetItem(obj); }) ?? [];
                this.canFindeAddress(list.length>0);
                this.fullAddressDropdown?.update(list);
            })
            .catch(() => {
                this.canFindeAddress(false);
            });
    }
    protected findStreets(query: string): void {
        this.house("");
        this.flat("");
        this.postalCode('');

        if (query.length == 0) {
            this.streetDropdown?.update(null);
            return;
        } 
        const city = this.currentCity()?.title;
        if (city == null || city === "") return;
        AddressService.findSuggestAddresses(
            query,
            this.currentCity()!.getState(),
            SuggestType.Street)
            .then((result) => {
                const list = result?.map((obj) => { return new SuggestedStreetItem(obj); }) ?? [];
                this.canFindeAddress(list.length>0);
                this.streetDropdown?.update(list);
            })
            .catch(() => {
                this.canFindeAddress(false);
            });
    }
    protected findHouses(query: string): void {
        this.flat('');
        this.postalCode('');

        if (query.length == 0) {
            this.houseDropdown?.update(null);
            return;
        } 
        const city = this.currentCity()?.title ?? "";
        if (city === "") return;
        AddressService.findSuggestAddresses(
            query,
            this.currentCity()!.getState(),
            SuggestType.House,
            this.street()?? "")
            .then((result) => {
                const list = result?.map((obj) => { return new SuggestedStreetItem(obj); }) ?? [];
                this.canFindeAddress(list.length>0);
                this.houseDropdown?.update(list);
            })
            .catch(() => {
                this.canFindeAddress(false);
            });
    }

    // мы не знаем о точке ничего, кроме координат или названия
    setNewAddress(suggest: IAddressSuggest) {
        if (this.isRewriteProcess) return; 
        AddressService.findStreetAddressDataBySuugest(suggest)
            .then((point) => {
                if (point == null) return;
                debugger;
                if (!this.currentCity()?.equals(point.city)) {
                    localStorage.setItem(this._addressLocalStorageKey, JSON.stringify(point));
                    this._updateCity(point);
                } 
                this._setDataInFields(point);
                this._handlers?.map.createPointOnMap?.(point);
                this.canFindeAddress(true);
            })
            .catch(() => {
                this.canFindeAddress(false);
            })  
    }


    // у нас есть все данные по точке кроме квартиры
    setNewAddressFromMap(point: IPointData | null) {
        if (point == null || this.isRewriteProcess) return;
        localStorage.setItem(this._addressLocalStorageKey, JSON.stringify(point));
        if (!this.currentCity()?.equals(point.city)?? true) {
            this._updateCity(point);
            return;
        }  
        this._setDataInFields(point);
        this._handlers?.map?.createPointOnMap?.(point);
        this.canFindeAddress(true);
    }

    private _updateCity(point: IPointData) {
        const lat = point.lat?.toString();
        const lon = point.lon?.toString();
        if (lat == null || lon == null)
            return;
        const countryId = this.currentCountry().id;
        AddressService.getCityAddressesByCoords(countryId, lat, lon)
            .then(ci => {
                localStorage.setItem(this._addressLocalStorageKey, JSON.stringify(point));
                this.currentCity(ci[0]);
            });
    }

    private _setDataInFields(point: IPointData) {
        this.isRewriteProcess = true;

        this.lat(point.lat);
        this.lon(point.lon);
        this.country(point.city?.country);
        this.street(point.street);
        this.house(point.house);
        this.addressLine1(`${point.street}, ${point.house}`);
        this.flat(point.flat ?? '');
        this.addressLine2(point.flat ?? '');
        this.postalCode(point.postalCode ?? '');

        this.isRewriteProcess = false;
        this._setAddressCorrect();
    }

    // выбор точки из дропдауна с названиями улицы + дома
    private _selectAddressFromSuggestDropdown(item: IAddressSuggest) { 
        this.isRewriteProcess = true;   //для того чтобы не запускались сабскрайберы street и house
        this.street(item?.Street ?? this.street());
        this.house(item?.House ?? this.house());
        if (this.isAddressLinesView) this.addressLine1(`${this.street()}, ${this.house()}`);
        this.isRewriteProcess = false;
        if (this.street() == null || this.house() === '') return;
        this.setNewAddress(item);
    }

    private _setAddressPointDataReady(ready: boolean) {
        if (ready) {
            this.addressPointData({
                city: this.currentCity()!.getPointData(),
                street: this.street()!,
                house: this.house(),
                flat: this.flat(),
                postalCode: this.postalCode(),
                lat: this.lat(),
                lon: this.lon(),
                description: null
            });
        } else {
            this.addressPointData(null);
        }

    }

    private _getPointData(): IPointData {
        return {
            city: this.currentCity()!.getPointData(),
            street: this.street()!,
            house: this.house(),
            flat: this.flat(),
            lat: this.lat(),
            lon: this.lon(),
            postalCode: this.postalCode(),
            description: null
        }
    }

    private _setAddressCorrect() {
        this.isAddressCorrectForExternalCalculations(
            this.canFindeAddress() 
            && this.isAddressFull() 
            && (!this.isPostcodeVisible() || (this.postalCode()?.trim() ?? '').length > 0));
    }
    // comp
    private _addressFullComp():boolean {
        const city = this.currentCity()?.title?.trim() ?? '';
        const house = this.house()?.trim() ?? '';
        return city.length > 0 && house.length > 0;
    }
    private _isFullDropdownVisibleComp(): boolean {
        return this.fullAddressDropdown != null && this.fullAddressDropdown.list().length > 0 && this.fullAddressDropdown.isShown();
    }
    private _isStreetDropdownVisibleComp(): boolean {
        return this.streetDropdown != null && this.streetDropdown.list().length > 0 && this.streetDropdown.isShown();
    }
    private _isHouseDropdownVisibleComp(): boolean {
        return this.houseDropdown != null && this.houseDropdown.list().length > 0 && this.houseDropdown.isShown();
    }
    private _houseTextComp(): string {
        const house = this.house()?.trim() ?? '';
        return house === '' ? '' : house;
    }

}

export interface IAddressModelLocalization {
    AddressStreetLabel: string;
    AddressHouseLabel: string;
    AddressFlatLabel: string;
    AddressLine1Label: string;
    AddressLine2Label: string;
    PostalCodeLabel: string;
    NoDeliveryAddressAlert: string;
}
export interface IAddressModelHandlers {
    map: IAddressMap
}

// универсальная модель данных точки
export interface IPointCityData {
    country: string;
    region: string;
    title: string;
}
export interface IPointData {
    lat: number | null;
    lon: number | null;
    city: IPointCityData;
    street: string;
    house: string;
    flat: string | null;
    postalCode: string;
    description: string | null;
}

export interface IBoundedPointData extends IPointData {
    bounds: IPointBounds | null;
}

export interface IPointBounds {
    upLon: number;
    upLat: number;
    lowLon: number;
    lowLat: number;
}

export interface IAddressData {
    Latitude: string | null;
    Longitude: string | null;
    Country: string;
    Region: string;
    Area: string | null;
    City: string;
    District: string | null;
    Street: string;
    House: string;
    PostalCode: string | null;
    Description: string | null;
    Bounds: IPxpMapBounds | null;
}

export interface IAddressMap extends BaseDeliveryMap{
    mapPointCoords: KnockoutObservable<IPxpLatLng | null>;
    setNewPoint(data: IPointData | null): void;
    createPointOnMap: ((point: IPointData | null, forse?: boolean) => void) | null;
    setAddressByName: ((addressName: string | null) => void) | null
}