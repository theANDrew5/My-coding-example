import { IDeliveryPriceCalculationResult } from "../../../_services/DeliveryPriceService";
import { IPickpointAddressProperties } from "../pluginDeliveryTypes/pickpointPluginDeliveryType/PickPointPluginDeliveryPointItem";
import { IDDeliveryV2AddressProperties } from "../pluginDeliveryTypes/safeRoutePluginDeliveryType/SafeRoutePluginDeliveryPointItem";

export abstract class BaseDeliveryPointItem {

    readonly shippingId: number;
    readonly shippingTitle: string | null;
    readonly shippingDescription: string | null;

    readonly providerType: PxpDeliveryPointProviderType;
    readonly pointType: PxpDistributionPointType;
    readonly addressData: IPxpDeliveryAddress;

    readonly isSelected: KnockoutObservable<boolean>;
    readonly calculationWasCorrect: KnockoutObservable<boolean | null>;
    readonly deliveryPrice: KnockoutObservable<number>;
    readonly deliveryCost: KnockoutObservable<number>;
    readonly deliveryDiscount: KnockoutObservable<number>;
    readonly minPeriod: KnockoutObservable<number>;
    readonly maxPeriod: KnockoutObservable<number>;
    
    readonly isPeriodVisible: KnockoutComputed<boolean>;
    readonly isVisible: KnockoutComputed<boolean>; // в курьерке скрываем в точкевыдачи показываем ошибку калькуляции

    title: string;

    // localization
    readonly loc_priceTitle: string;
    readonly loc_deliveryFreeTitle: string;
    readonly loc_oneMinPointDeliveryPeriod: string;
    readonly loc_oneMaxPointDeliveryPeriod: string;
    readonly loc_twoPointDeliveryPeriod: string;
    readonly loc_deliveryPeriodTitle: string;

    constructor(model: IPxpBaseDeliveryPointDto, localization: IBaseDeliveryPointItemLocalization) {
        this.shippingId = model.Address.ShippingId;
        this.shippingTitle = model.ShippingTitle;
        this.shippingDescription = model.ShippingDescription;

        this.providerType = model.ProviderType;
        this.pointType = model.PointType;
        this.title = model.Title;
        this.addressData = model.Address;

        this.isSelected = ko.observable(false);
        this.calculationWasCorrect = ko.observable(null);
        this.deliveryPrice = ko.observable(0);
        this.deliveryCost = ko.observable(0);
        this.deliveryDiscount = ko.observable(0);
        this.minPeriod = ko.observable(0);
        this.maxPeriod = ko.observable(0);

        // localizaion
        this.loc_priceTitle = localization.PriceTitle;
        this.loc_deliveryFreeTitle = localization.FreeTitle;
        this.loc_deliveryPeriodTitle = localization.PeriodTitle;
        this.loc_oneMinPointDeliveryPeriod = localization.PeriodFrom;
        this.loc_oneMaxPointDeliveryPeriod = localization.PeriodTo;
        this.loc_twoPointDeliveryPeriod = localization.PeriodArrange;

        //comp
        this.isVisible = ko.pureComputed(this._isVisibleComp, this);
        this.isPeriodVisible = ko.pureComputed(this._isPeriodVisibleComp, this);
    }

    applyCalculationResut(calcResult: IDeliveryPriceCalculationResult | null) {
        if (calcResult == null) {
            this.calculationWasCorrect(false);
            return;
        }
        this.calculationWasCorrect(calcResult.Success);
        if (!calcResult.Success) return;
        this.deliveryPrice(calcResult.Price);
        this.deliveryCost(calcResult.Cost);
        this.deliveryDiscount(calcResult.Discount);
        this.addressData.DeliveryProperties = calcResult.Properties;
    }

    //comp
    private _isVisibleComp(): boolean {
        return this.calculationWasCorrect() ?? true;
    }
    private _isPeriodVisibleComp(): boolean {
        return this.minPeriod() > 0 && this.maxPeriod() > 0 ;
    }
}

export interface IBaseDeliveryPointItemLocalization {
    PriceTitle: string;
    FreeTitle: string;
    PeriodTitle: string;
    PeriodArrange: string;
    PeriodFrom: string;
    PeriodTo: string;
}

//BaseShippingPointDto
export interface IPxpBaseDeliveryPointDto {
    ProviderType: PxpDeliveryPointProviderType;
    PointType: PxpDistributionPointType;
    Title: string;
    ShippingTitle: string | null;
    ShippingDescription: string | null;
    Address: IPxpDeliveryAddress;
}

// Изменил тут? Не забудь изменить в DistributionPointType.cs!
export enum PxpDistributionPointType {
    Unknown = 0,
    Cdek = 1,
    ImLogistics = 2,
    DDelivery = 3,
    Novaposhta = 4,
    YandexDelivery = 5,
    Photomax = 6,
    Exgarant = 7,
    Boxberry = 8,
    Ukrposhta = 9,
    Postnl = 10,
    Justin = 11,
    Omniva = 12,
    Econt = 13,
    Evropochta = 14,
    Pickpoint = 15,
    RussianPost = 16,
    DPD = 17,
    CDEKv2 = 18,
    USPS = 19
}

// Изменил тут? Не забудь изменить в ShippingServiceProviderType.cs!
export enum PxpDeliveryPointProviderType {
    General = 0,
    Cdek = 2,
    ImLogistics = 3,
    DDelivery = 4,
    NovaposhtaV2 = 5,
    YandexDelivery = 6,
    ImlV2 = 7,
    Photomax = 8,
    Exgarant = 9,
    Boxberry = 10,
    DDeliveryV2 = 11,
    Ukrposhta = 12,
    Postnl = 13,
    Justin = 14,
    YandexGo = 15,
    Omniva = 16,
    Econt = 17,
    Evropochta = 18,
    Pickpoint = 19,
    RussianPost = 20,
    DPD = 21,
    CDEKv2 = 22,
    USPS = 23
}

export interface IPxpAddress {
    Country: string;
    Region: string;
    City: string;
    Street: string | null;
    House: string;
    Flat: string | null;
    AddressLine: string;
    PostalCode: string;
    Latitude: string | null;
    Longitude: string | null;
    Description?: string;
}

export interface IPxpMapBounds {
    UpperLongitude: string;
    UpperLatitude: string;
    LowerLongitude: string;
    LowerLatitude: string;
}

//ShippingAddressDto
export interface IPxpDeliveryAddress extends IPxpAddress {
    Id?: number;
    ShippingId: number;
    DeliveryProperties?: IDeliveryAddressProperties;
}

export interface IDeliveryAddressProperties {
    RussianPostAddressInfo?: any;
    yandexDeliveryAddressInfo?: any;
    ddeliveryV2AddressInfo?: IDDeliveryV2AddressProperties;
    pickpointAddressInfo?: IPickpointAddressProperties;
    cdekAddressInfo?: any;
}