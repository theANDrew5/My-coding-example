import { pxpGlobal } from '../../../../../globals/pxp';
import { BaseStandartDeliveryPointItem, IBaseStandartDeliveryPointItemLocalization, IPxpBaseStandartDeliveryPointDto } from '../_baseStandartType/BaseStandartDeliveryPointItem';
import { IPxpAddress, IPxpDeliveryAddress, PxpDistributionPointType } from '../../_base/BaseDeliveryPointItem';

export class CourierDeliveryPointItem extends BaseStandartDeliveryPointItem {
    readonly postCodeRequired: boolean;

    // localization
    readonly loc_deliveryCourierCdekTitle: string;
    readonly loc_deliveryCourierIMLTitle: string;
    readonly loc_deliveryCourierGeneralCourierTitle: string;
    readonly loc_descriptionTitle: string;

    // comp
    priceText: KnockoutReadonlyComputed<string>;

    constructor(dto: IPxpCourierDeliveryPointDto, localization: IAddressSelectDeliveryPointItemLocalization) {
        super(dto, localization);

        this.postCodeRequired = dto.PostCodeRequired;

        // localization
        this.loc_deliveryCourierCdekTitle = localization.CourierCdekTitle;
        this.loc_deliveryCourierIMLTitle = localization.CourierIMLTitle;
        this.loc_deliveryCourierGeneralCourierTitle = localization.CourierGeneralCourierTitle;
        this.loc_descriptionTitle = localization.DescriptionTitle;

        // comp
        this.priceText = ko.pureComputed(this._priceTextComp, this);
        // calls
        this.title = (dto.ShippingTitle ?? '') != '' ? dto.ShippingTitle! : this.getTitleFromProvider();
    }

    //
    getTitleFromProvider() {
        switch (this.pointType) {
            case PxpDistributionPointType.Cdek:
            case PxpDistributionPointType.CDEKv2: return this.loc_deliveryCourierCdekTitle;
            case PxpDistributionPointType.ImLogistics: return this.loc_deliveryCourierIMLTitle;
            case PxpDistributionPointType.Unknown:
            default:
                return this.loc_deliveryCourierGeneralCourierTitle;
        }
    }

    mergeUserAddress(userAddress: IPxpAddress):IPxpDeliveryAddress {
        if (userAddress != null) {
            this.addressData.Street = userAddress.Street;
            this.addressData.House = userAddress.House;
            this.addressData.Flat = userAddress.Flat
            this.addressData.PostalCode = userAddress.PostalCode;
            this.addressData.AddressLine = userAddress.AddressLine;
        }
        return this.addressData;
    }

    // comp
    private _priceTextComp(): string {
        var price = this.deliveryPrice();

        if (price <= 0) return this.loc_deliveryFreeTitle;
        return pxpGlobal.utilities.getPriceString(price);
    }
}

export interface IAddressSelectDeliveryPointItemLocalization extends IBaseStandartDeliveryPointItemLocalization {
    CourierCdekTitle: string;
    CourierIMLTitle: string;
    CourierGeneralCourierTitle: string;
    DescriptionTitle: string;
}

export interface IPxpCourierDeliveryPointDto extends IPxpBaseStandartDeliveryPointDto {
    Title: string;
    PostCodeRequired: boolean;
}