import { IPxpBaseDeliveryPointDto } from "../../_base/BaseDeliveryPointItem";
import { BasePluginDeliveryPointItem, IBasePluginDeliveryPointItemLocalization } from "../_base/BasePluginDeliveryPointItem";

export class SafeRoutePluginDeliveryPointItem extends BasePluginDeliveryPointItem {

    isDescriptionVisible: boolean;

    // localization
    readonly loc_courierTypeTitle: string;
    readonly loc_pointTypeTitle: string;
    readonly loc_mailTypeTitle: string;
    readonly loc_mainTitle: string;

    constructor(model: IPxpBaseDeliveryPointDto, localization: ISafeRoutePluginDeliveryPointItemLocalization) {
        super(model, localization);

        let data = this.addressData.DeliveryProperties?.ddeliveryV2AddressInfo;
        this.isDescriptionVisible = this.addressData.Description != undefined;
        
        this.deliveryPrice(data?.PriceCalcResult.TotalPrice ?? 0);

        // localization
        this.loc_courierTypeTitle = localization.CourierTypeTitle;
        this.loc_pointTypeTitle = localization.PointNameTitle;
        this.loc_mailTypeTitle = localization.MailTypeTitle;
        this.loc_mainTitle = '';

        // calls
        switch (data?.DeliveryType) {
            case 1: this.loc_mainTitle = this.loc_pointTypeTitle; break;
            case 2: this.loc_mainTitle = this.loc_courierTypeTitle; break;
            case 3: this.loc_mainTitle = this.loc_mailTypeTitle; break;
        }
    }
}

export interface ISafeRoutePluginDeliveryPointItemLocalization extends IBasePluginDeliveryPointItemLocalization {
    CourierTypeTitle: string;
    PointTypeTitle: string;
    MailTypeTitle: string;
}

export interface IDDeliveryV2AddressProperties {
    PriceCalcResult: IDDeliveryV2CalculatorResult
    DeliveryType: number;
    PointId: number | null;
    DeliveryCompanyId: number;
    CityToId: number;
    StreetTo: string;
    HouseTo: string;
    FlatTo: string;
    CityToFias: string;
    CityToKladr: string;
}

export interface IDDeliveryV2CalculatorResult {
    PriceDelivery: number;
    PriceSorting: number | null;
    TotalPrice: number;
}