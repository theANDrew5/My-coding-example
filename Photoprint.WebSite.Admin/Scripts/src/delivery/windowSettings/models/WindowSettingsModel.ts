import { makeAutoObservable } from "mobx";

export interface IDeliveryWindowSettings {
    IsNewDeliveryWindowEnabled: boolean;
    UseMiddleName: boolean;
    UseShippingFromPreviousOrder: boolean;
    IsAdditionalNotificationEmailEnabled: boolean;
    IsAdditionalNotificationPhoneNumberEnabled: boolean;
    MapSettings: IDeliveryWindowMapSettings | null;
    AddressSelectSettings: IDeliveryWindowAddressSelectSettings | null;
    PickpointsSettings: IDeliveryWindowPickpointsSettings | null;
}
export class DeliveryWindowSettings {
    isNewDeliveryWindowEnabled: boolean;
    useMiddleName: boolean;
    useShippingFromPreviousOrder: boolean;
    isAdditionalNotificationEmailEnabled: boolean;
    isAdditionalNotificationPhoneNumberEnabled: boolean;
    mapSettings: IDeliveryWindowMapSettings | null;
    addressSelectSettings: IDeliveryWindowAddressSelectSettings | null;
    pickpointsSettings: IDeliveryWindowPickpointsSettings | null;

    constructor() {
        this.isNewDeliveryWindowEnabled = false;
        this.useMiddleName = false;
        this.useShippingFromPreviousOrder = false;
        this.isAdditionalNotificationEmailEnabled = false;
        this.isAdditionalNotificationPhoneNumberEnabled = false;
        this.mapSettings = null;
        this.addressSelectSettings = null;
        this.pickpointsSettings = null;
        
        // calls
        makeAutoObservable(this)
    }

    init(data: IDeliveryWindowSettings | null) {
        if (data == null) return;
        this.isNewDeliveryWindowEnabled = data.IsNewDeliveryWindowEnabled;
        this.useMiddleName = data.UseMiddleName;
        this.useShippingFromPreviousOrder = data.UseShippingFromPreviousOrder;
        this.isAdditionalNotificationEmailEnabled = data.IsAdditionalNotificationEmailEnabled;
        this.isAdditionalNotificationPhoneNumberEnabled = data.IsAdditionalNotificationPhoneNumberEnabled;
        this.mapSettings = data.MapSettings;
        this.addressSelectSettings = data.AddressSelectSettings;
        this.pickpointsSettings = data.PickpointsSettings;
    }
}

export interface IDeliveryWindowMapSettings {
    MapType: DeliveryWindowMapType;
    YandexMapSettings?: IMapSettings;
    GoogleMapSettings?: IMapSettings;
}
export interface IMapSettings {
    CountryLimiter: CityAddressCountry;
    ApiKey: string;
}
export enum DeliveryWindowMapType {
    Yandex = 0,
    Google = 1
}

export interface IDeliveryWindowPickpointsSettings {
    IsOfficesInAnotherBlock: boolean;
    IsFilterProviderTypeEnabled: boolean;
    IsSerchStringEnabled: boolean;
}

export interface IDeliveryWindowAddressSelectSettings {
    UsePostcode: boolean;
    UseAddressLines: boolean;
}

export enum CityAddressCountry
{
    NoCountry = 0,
    Russia = 1,
    Ukraine = 2,
    Belarus = 3,
    Kazakhstan = 4,
    Bulgaria = 5,
    USA = 6
}