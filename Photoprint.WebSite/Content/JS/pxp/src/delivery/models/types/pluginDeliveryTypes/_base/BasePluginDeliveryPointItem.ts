import { pxpGlobal } from "../../../../../globals/pxp";
import { BaseDeliveryPointItem, IBaseDeliveryPointItemLocalization, IPxpBaseDeliveryPointDto } from "../../_base/BaseDeliveryPointItem";

export abstract class BasePluginDeliveryPointItem extends BaseDeliveryPointItem {
    // localization
    loc_addressTitle: string;
    loc_pointNameTitle: string;

    // comp
    priceText: KnockoutReadonlyComputed<string>;

    constructor(model: IPxpBaseDeliveryPointDto, localization: IBasePluginDeliveryPointItemLocalization) {
        super(model, localization);
        // localization
        this.loc_addressTitle = localization.AddressTitle;
        this.loc_pointNameTitle = localization.PointNameTitle;

        // comp
        this.priceText = ko.pureComputed(this._priceTextComp, this);
    }

    // comp    
    private _priceTextComp(): string {
        var price = this.deliveryPrice();

        if (price <= 0) return this.loc_deliveryFreeTitle;
        return pxpGlobal.utilities.getPriceString(price);
    }
}

export interface IBasePluginShippingData {
    ShippingId: number;
}

export interface IBasePluginDeliveryPointItemLocalization extends IBaseDeliveryPointItemLocalization { 
    AddressTitle: string
    PointNameTitle: string
}