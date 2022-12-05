import { IPxpBaseDeliveryPointDto } from "../../_base/BaseDeliveryPointItem";
import { BasePluginDeliveryPointItem, IBasePluginDeliveryPointItemLocalization } from "../_base/BasePluginDeliveryPointItem";

export class PickPointPluginDeliveryPointItem extends BasePluginDeliveryPointItem {

    constructor(model: IPxpBaseDeliveryPointDto, localization: IPickPointPluginDeliveryPointItemLocalization) {
        super(model, localization);
    }
}

export interface IPickPointPluginDeliveryPointItemLocalization extends IBasePluginDeliveryPointItemLocalization { }

export interface IPickpointAddressProperties {
    PostamatCode: string;
    PostamatId: string;
    CityId: string;
}