import { BaseDeliveryPointItem, IBaseDeliveryPointItemLocalization, IPxpBaseDeliveryPointDto, IPxpDeliveryAddress, PxpDeliveryPointProviderType, PxpDistributionPointType } from "../../_base/BaseDeliveryPointItem";

export abstract class BaseStandartDeliveryPointItem extends BaseDeliveryPointItem {   

    id: number;
    readonly isShippingDescriptionVisible: boolean;
    abstract getTitleFromProvider(): string;

    constructor(model: IPxpBaseStandartDeliveryPointDto, localization: IBaseStandartDeliveryPointItemLocalization) {
        super(model, localization)
        
        this.id = model.Address.Id!;
        this.isShippingDescriptionVisible = this.shippingDescription != null && this.shippingDescription != '';
    }
}

export interface IBaseStandartDeliveryPointItemLocalization extends IBaseDeliveryPointItemLocalization { }

export interface IPxpBaseStandartDeliveryPointDto extends IPxpBaseDeliveryPointDto {
    ShippingAddressId: number; 
}

