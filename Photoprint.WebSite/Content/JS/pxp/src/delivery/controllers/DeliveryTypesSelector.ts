import { IPickPointPluginDeliveryTypeLocalization, PickPointPluginDeliveryType } from '../models/types/pluginDeliveryTypes/pickpointPluginDeliveryType/PickPointPluginDeliveryType';
import { ISafeRoutePluginDeliveryTypeHandlers, ISafeRoutePluginDeliveryTypeLocalization, SafeRoutePluginDeliveryType } from '../models/types/pluginDeliveryTypes/safeRoutePluginDeliveryType/SafeRoutePluginDeliveryType';
import { DeliveryMessageBlockType, DeliveryMessagesManager, DeliveryMessageType, IMessagesMaster } from './MessagesManager';
import { DeliveryManagerService } from '../_services/DeliveryManagerService';
import { IDeliverySettingSelector } from '../models/WindowSettings';
import { CityInfo } from '../models/city/CityInfo';
import { IWeakUserData } from './RecipientManager';
import { CountryInfo } from "../models/city/CountrySelector";
import { BaseDeliveryTypeModel, IAvailableDeliveryTypesDto } from "../models/types/_base/BaseDeliveryTypeModel";
import { IPointDeliveryTypeLocalization, PointDeliveryType } from '../models/types/standartDeliveryType/pointDeliveryType/PointDeliveryType';
import { AddressSelectDeliveryType, IAddressSelectDeliveryTypeLocalization } from '../models/types/standartDeliveryType/addressSelectDeliveryType/AddressSelectDeliveryType';

export class DeliveryTypeSelector {
    private _handlers: IDeliveryTypeSelectorHandlers | null;
    private _settings: IDeliverySettingSelector;
    private _localization: IDeliveryTypeSelectorLocalization;

    readonly instanceGuid: string;
   
    messagesMaster: IMessagesMaster;
    messagesManager: DeliveryMessagesManager;

    isLoading: KnockoutObservable<boolean>;
    currentCountry: KnockoutObservable<CountryInfo>;
    currentCity: KnockoutObservable<CityInfo | null>;
    selectedDeliveryType: KnockoutObservable<BaseDeliveryTypeModel | null>;
    deliveryTypes: KnockoutObservableArray<BaseDeliveryTypeModel | null>;
    selectorItemClass: KnockoutComputed<string>;

    // localization
    readonly loc_deliveryTitle: string;
    readonly loc_selectCityAlert: string;
    readonly loc_noDeliveryAlert: string;

    // comp
    isDeliveryTypesListEmpty: KnockoutReadonlyComputed<boolean>;
    isCurrentCityEmpty: KnockoutReadonlyComputed<boolean>;
    isDeliveryTypesListEmptyMessage: KnockoutReadonlyComputed<boolean>;
    deliveryPrice: KnockoutComputed<number>;

    constructor(
        instanceGuid: string,
        currentCity: KnockoutObservable<CityInfo | null>,
        currentCountry: KnockoutObservable<CountryInfo>, 
        settings: IDeliverySettingSelector,
        localization: IDeliveryTypeSelectorLocalization,
        master: IMessagesMaster
    ) {
        this.selectDeliveryType = this.selectDeliveryType.bind(this);

        this._handlers = null;
        this._settings = settings;
        this._localization = localization;

        this.instanceGuid = instanceGuid;

        this.messagesMaster = master;
        this.messagesManager = this.messagesMaster.getOrCreateManager(DeliveryMessageBlockType.Delivery);

        this.isLoading = ko.observable(false);
        this.currentCity = currentCity;
        this.currentCountry = currentCountry;
        this.selectedDeliveryType = ko.observable(null);
        this.deliveryTypes = ko.observableArray(null);
        this.selectorItemClass = ko.pureComputed(this._selectorClassComp, this);

        // localization
        this.loc_deliveryTitle = localization.DeliveryTitle;
        this.loc_selectCityAlert = localization.SelectCityAlert;
        this.loc_noDeliveryAlert = localization.NoDeliveryForCity;
        
        //binds
        this._mapDeliveryTypes = this._mapDeliveryTypes.bind(this);

        // comp
        this.isDeliveryTypesListEmpty = ko.pureComputed(this._isDeliveryTypesListEmptyComp, this);
        this.isCurrentCityEmpty = ko.pureComputed(this._isCurrentCityEmptyComp, this);
        this.isDeliveryTypesListEmptyMessage = ko.pureComputed(this._isDeliveryTypesListEmptyMessageComp, this);
        this.deliveryPrice = ko.pureComputed(this._deliveryPriceComp, this);

        // subscribes
        this.currentCity.subscribe(this._updateDeliveryTypes, this);

        // extends
        this.selectedDeliveryType.extend({ rateLimit: { timeout: 150, method: "notifyWhenChangesStop" } });

        //messages
        master.addMessages([
            {
                text: this.loc_selectCityAlert,
                call: this.isCurrentCityEmpty,
                blockType: DeliveryMessageBlockType.Delivery,
                type: DeliveryMessageType.Warning
            },
            {
                text: this.loc_noDeliveryAlert,
                call: this.isDeliveryTypesListEmptyMessage,
                blockType: DeliveryMessageBlockType.Delivery,
                type: DeliveryMessageType.Error
            }
        ]);
    }
    
    initHandlers(handlers: IDeliveryTypeSelectorHandlers) {
        this._handlers = handlers;
    }

    init() {
        this._updateDeliveryTypes();
    }

    selectDeliveryType(deliveryType: BaseDeliveryTypeModel) {
        if (deliveryType.isSelected()) return;

        const selectedDeliveryType = this.selectedDeliveryType();
        selectedDeliveryType?.isSelected(false);
        deliveryType.isSelected(true);
        this.selectedDeliveryType(deliveryType);
    }

    private _updateDeliveryTypes() {
        const city = this.currentCity();
        console.log(city);
        if (city == null) return;

        if (this.isLoading()) return;
        this.isLoading(true);

        const beforeType = this.selectedDeliveryType()?.type ?? null;
        this.selectedDeliveryType(null);

        DeliveryManagerService.getAvailableDeliveryTypes(this.instanceGuid, city.originalData)
            .then((response) => {
                this._mapDeliveryTypes(response, beforeType);
            },
            (reject) => {
                console.log("load shippings error: " + reject);
            }
        ).then(() => {
            this.isLoading(false);
        });
    }

    private _mapDeliveryTypes(dto: IAvailableDeliveryTypesDto[], beforeType: DeliveryDisplayType | null) {
        var mapSettings = this._settings.MapSettings;
        if (mapSettings == null) throw "map not initialized!";

        const availableDiveryTypes = [] as BaseDeliveryTypeModel [];
        var defaultType: BaseDeliveryTypeModel | null = null;
        for (var display of dto) {
                    
            switch (display.Type) {
                case DeliveryDisplayType.Courier:
                    availableDiveryTypes.push(new AddressSelectDeliveryType(this.instanceGuid, mapSettings, display, this.currentCity, this.currentCountry, 
                        this._settings.AddressSelectSettings, this._localization.AddressSelectDeliveryLocalization, this.messagesMaster));
                    break;
                case DeliveryDisplayType.DeliveryByAddress:
                    availableDiveryTypes.push(new AddressSelectDeliveryType(this.instanceGuid, mapSettings, display, this.currentCity, this.currentCountry,
                        this._settings.AddressSelectSettings, this._localization.AddressSelectDeliveryLocalization, this.messagesMaster));
                    break;
                case DeliveryDisplayType.Pickpoint:
                    availableDiveryTypes.push(new PointDeliveryType(this.instanceGuid, mapSettings, display, this.currentCity, this.currentCountry, 
                        this._localization.PointDeliveryLocalization, this.messagesMaster, this._settings.PickpointSettings));
                    break;
                case DeliveryDisplayType.Office:
                    availableDiveryTypes.push(new PointDeliveryType(this.instanceGuid, mapSettings, display, this.currentCity, this.currentCountry, 
                        this._localization.PointDeliveryLocalization, this.messagesMaster));
                    break;
                case DeliveryDisplayType.DeliveryPlaginPickPoint:                         
                    availableDiveryTypes.push(new PickPointPluginDeliveryType(display, this.currentCity, this.currentCountry, this._localization.PickPointPluginDeliveryTypeLocalization, 
                        this.messagesMaster));
                    break;
                case DeliveryDisplayType.DeliveryPlaginSafeRoute:
                    var saferouteHandlers = <ISafeRoutePluginDeliveryTypeHandlers> {
                        recipent_userData: this._handlers?.recipent_userData,
                        recipent_setUserData: this._handlers?.recipent_setUserData,
                    }
                    var srType = new SafeRoutePluginDeliveryType(display, this.currentCity, this.currentCountry, this._localization.SafeRoutePluginDeliveryTypeLocalization, 
                        saferouteHandlers, this.messagesMaster);
                    availableDiveryTypes.push(srType);
                           
                    srType.selectedPoint.subscribe((res) => {
                        if (res == null) return;
                        this._handlers?.totlaPriceController_refreshPrice();
                    });
                    break;
            }
            if (display.IsDefaultType)
                defaultType = availableDiveryTypes[availableDiveryTypes.length-1];
        }

        this.deliveryTypes(availableDiveryTypes);
        if (this._settings.UseShippingFromPreviousOrder) {
            const prevState = DeliveryManagerService.getStoredShippingFinalState();
            if (prevState != undefined)
                defaultType =
                    this.deliveryTypes().find(dt => dt?.shippingIds.some(sId => sId === prevState.ShippingId)) ?? defaultType;
        } else if(beforeType!= null)
            defaultType = this.deliveryTypes().find(dt => dt?.type == beforeType) ?? defaultType;
                
        defaultType = defaultType ?? this.deliveryTypes()[0];
        if (defaultType!=null)
            this.selectDeliveryType(defaultType);
    }

    // comp
    private _isDeliveryTypesListEmptyComp(): boolean {
        return this.deliveryTypes().length == 0;
    }
    private _isCurrentCityEmptyComp(): boolean {
        return this.currentCity() == null;
    }
    private _isDeliveryTypesListEmptyMessageComp(): boolean {
        return !this.isLoading() && !this.isCurrentCityEmpty() && this.isDeliveryTypesListEmpty();
    }
    private _deliveryPriceComp(): number {
        var selectedType = this.selectedDeliveryType();
        return selectedType?.finalDeliveryPrice() ?? 0;
    }

    private _selectorClassComp(): string {
        return this.deliveryTypes().length>3
        ? "delivery__selector-item_more-than-3"
        : "delivery__selector-item";
    }
}

export enum DeliveryDisplayType {
    Courier, // курьерки
    DeliveryByAddress, // доставки без синхронизации адресов НЕ КУРЬЕРКИ
    Pickpoint, // точки выдачи
    Office, // офисы компании
    DeliveryPlaginPickPoint, // pickpoint плагин
    DeliveryPlaginSafeRoute // saferoute (ddelivery) плагин
}

export interface IDeliveryTypeSelectorLocalization {
    DeliveryTitle: string,
    SelectCityAlert: string,
    NoDeliveryForCity: string,
    AddressSelectDeliveryLocalization: IAddressSelectDeliveryTypeLocalization;
    PointDeliveryLocalization: IPointDeliveryTypeLocalization;
    PickPointPluginDeliveryTypeLocalization: IPickPointPluginDeliveryTypeLocalization;
    SafeRoutePluginDeliveryTypeLocalization: ISafeRoutePluginDeliveryTypeLocalization;
}

export interface IDeliveryTypeSelectorHandlers {
    recipent_userData: KnockoutComputed<IWeakUserData>;
    recipent_setUserData: (data: IWeakUserData) => void;
    totlaPriceController_refreshPrice: () => void;
};