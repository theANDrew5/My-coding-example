import { IDeliveryMapAuthData } from "./types/standartDeliveryType/_baseStandartType/viewModels/BaseDeliveryMap";

export interface IDeliverySettings {
    SelectorSettings: IDeliverySettingSelector;
    RecepientSettings: IDeliverySettingsRecepient;
    TotalPriceBlockSettings: IDeliverySettingsTotalPriceBlock;
}

export interface IDeliverySettingSelector {
    UseShippingFromPreviousOrder: boolean;
    MapSettings: IDeliverySettingsMap;
    PickpointSettings: IDeliverySettingsPickpoint;
    AddressSelectSettings: IDeliverySettingsAddressSelect;
}
export interface IDeliverySettingsMap {
    Data: IDeliveryMapAuthData
    MapType: PxpMapType;
}
export enum PxpMapType {
    Yandex = 0,
    Google = 1
}
export interface IDeliverySettingsPickpoint {
    IsFilterProviderTypeEnabled: boolean;
    IsSearchFilterEnabled: boolean;
}
export interface IDeliverySettingsAddressSelect {
    UsePostcode: boolean;
    UseAddressLines: boolean;
}

export interface IDeliverySettingsRecepient {
    UseMiddleName: boolean; 
    IsCyrillic: boolean;
    UseAdditionalEmail: boolean;
    UseAdditionalPhone: boolean;
}

export interface IDeliverySettingsTotalPriceBlock {
    ShowLinkToBackInShoppingCart: boolean;
    LinkToBackInShoppingCart: string;
}