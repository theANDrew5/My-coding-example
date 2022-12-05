import { pxpGlobal } from "../../../../../globals/pxp";
import { BaseStandartDeliveryPointItem, IBaseStandartDeliveryPointItemLocalization, IPxpBaseStandartDeliveryPointDto } from "../_baseStandartType/BaseStandartDeliveryPointItem";
import { PxpDistributionPointType, PxpDeliveryPointProviderType, IDeliveryAddressProperties } from "../../_base/BaseDeliveryPointItem";

export interface IPointFilterVisible {
    Value: boolean;
    Reason: PointFilterReason | null;
}
export enum PointFilterReason {
    // base
    Unknown = 0,
    Bounds = 11, // границы карты

    // in inherit types
    Shipping = 20, // id доставки
}

export class PointDeliveryPointItem extends BaseStandartDeliveryPointItem {

    isLoading: KnockoutObservable<boolean>;
    isSelectedOnList: KnockoutObservable<boolean>;

    isVisibleByFilter: boolean; // для карты
    isListedByFilter: boolean; // для списка

    readonly description: string;
    readonly workTime: string;
    readonly phone: string;
    readonly address: string;
    readonly longitude: number | null;
    readonly latitude: number | null;
    readonly coords: number[] | null;
    readonly isTitleEmpty: boolean;
    readonly isAddressEmpty: boolean;
    readonly isDescriptionEmpty: boolean;
    readonly isPhoneEmpty: boolean;
    readonly isWorkTimeEmpty: boolean;
    readonly isCoordsNull: boolean;
    readonly isAvailablePoint: boolean;

    geoObjectOnMap: MapMarkerType | null;

    private _filters: [PointFilterReason, boolean][] = [[PointFilterReason.Bounds, true], [PointFilterReason.Shipping, true]];

    // localization
    readonly loc_addressTitle: string;
    readonly loc_phoneTitle: string;
    readonly loc_workTimeTitle: string;
    readonly loc_descriptionTitle: string;
    readonly loc_selectTitle: string;
    readonly loc_selectedTitle: string;
    readonly loc_inCalculate: string;
    readonly loc_selectError: string;
    readonly loc_goBack: string;

    readonly loc_deliveryPointCdekTitle: string;
    readonly loc_deliveryPointIMLTitle: string;
    readonly loc_deliveryPointBoxberryTitle: string;
    readonly loc_deliveryPointPickPointTitle: string;
    readonly loc_deliveryPointNovaposhtaTitle: string;
    readonly loc_deliveryPointYandexDeliveryTitle: string;
    readonly loc_deliveryPointOfficeTitle: string;
    readonly loc_deliveryPointGeneralOfficeTitle: string;
    readonly loc_deliveryPointRussianPostTitle: string;
    readonly loc_deliveryPointDpdTitle: string;
    readonly loc_deliveryPointCdekV2Title: string;

    // comp
    priceText: KnockoutComputed<string>;
    deliveryPeriodText: KnockoutComputed<string>;
    selectBtnText: KnockoutComputed<string>;
    selectBtnAttr: KnockoutComputed<object>;

    constructor(dto: IPxpPointDeliveryPointDto, localization: IPointDeliveryPointItemLocalization) {
        super(dto, localization);

        this.description = dto.Description?.trim() ?? '';
        this.workTime = dto.WorkTime?.trim() ?? '';
        this.phone = dto.Phone?.trim() ?? '';
        this.title = '';
        this.address = dto.Address?.AddressLine?.trim() ?? '';
        this.longitude = null;
        this.latitude = null;
        this.coords = null;
        this.isDescriptionEmpty = this.description.length === 0;
        this.isPhoneEmpty = this.phone.length === 0;
        this.isWorkTimeEmpty = this.workTime.length === 0;
        this.isCoordsNull = true;
        this.isAvailablePoint = false;

        this.isLoading = ko.observable(false);
        this.isSelectedOnList = ko.observable(false);
        this.isVisibleByFilter = true;
        this.isListedByFilter = true;
        this.deliveryPrice(dto.Price ?? 0);
        this.geoObjectOnMap = null;

        // localization
        this.loc_addressTitle = localization.AddressTitle;
        this.loc_phoneTitle = localization.PhoneTitle;
        this.loc_workTimeTitle = localization.WorkTimeTitle;
        this.loc_descriptionTitle = localization.DescriptionTitle;
        this.loc_selectTitle = localization.SelectTitle;
        this.loc_selectedTitle = localization.SelectedTitle;
        this.loc_inCalculate = localization.InCalculate;
        this.loc_selectError = localization.SelectError;
        this.loc_goBack = localization.GoBack;

        this.loc_deliveryPointCdekTitle = localization.ProviderTitles.DeliveryPointCdekTitle;
        this.loc_deliveryPointBoxberryTitle = localization.ProviderTitles.DeliveryPointBoxberryTitle;
        this.loc_deliveryPointIMLTitle = localization.ProviderTitles.DeliveryPointIMLTitle;
        this.loc_deliveryPointPickPointTitle = localization.ProviderTitles.DeliveryPointPickPointTitle;
        this.loc_deliveryPointNovaposhtaTitle = localization.ProviderTitles.DeliveryPointNovaposhtaTitle;
        this.loc_deliveryPointYandexDeliveryTitle = localization.ProviderTitles.DeliveryPointYandexDeliveryTitle;
        this.loc_deliveryPointOfficeTitle = localization.ProviderTitles.DeliveryPointOfficeTitle;
        this.loc_deliveryPointRussianPostTitle = localization.ProviderTitles.DeliveryPointRussianPostTitle;
        this.loc_deliveryPointGeneralOfficeTitle = localization.ProviderTitles.DeliveryPointGeneralOfficeTitle;
        this.loc_deliveryPointDpdTitle = localization.ProviderTitles.DeliveryPointDpdTitle;
        this.loc_deliveryPointCdekV2Title = localization.ProviderTitles.DeliveryPointCdekV2Title;

        // comp
        this.priceText = ko.pureComputed(this._priceTextComp, this);
        this.deliveryPeriodText = ko.pureComputed(this._deliveryPeriodComp, this);
        this.selectBtnText = ko.pureComputed(this._selectBtnTextComp, this);
        this.selectBtnAttr = ko.pureComputed(this._selectBtnAttrComp, this);

        // calls
        this.title = dto.Title == null || dto.Title.length === 0 ? this.getTitleFromProvider() : dto.Title;

        var latitudeRaw = dto.Address?.Latitude; // широта
        if (latitudeRaw != null && latitudeRaw.length > 0) {
            this.latitude = parseFloat(latitudeRaw)
            if (isNaN(this.latitude)) this.latitude = null;
        }
        var longitudeRaw = dto.Address?.Longitude; // долгота
        if (longitudeRaw != null && longitudeRaw.length > 0) {
            this.longitude = parseFloat(longitudeRaw);
            if (isNaN(this.longitude)) this.longitude = null;
        }
        this.isCoordsNull = this.latitude == null || this.longitude == null;
        this.coords = [this.latitude ?? 0, this.longitude ?? 0];
        this.isTitleEmpty = this.title.length === 0;
        this.isAddressEmpty = this.address.length === 0;
        // решение по доступности
        this.isAvailablePoint = !this.isTitleEmpty && !this.isAddressEmpty && !this.isCoordsNull;
    }


    // comp    
    private _priceTextComp(): string {
        if (!this.calculationWasCorrect()) return this.loc_inCalculate;
        var price = this.deliveryPrice();
        if (price <= 0) return this.loc_deliveryFreeTitle;
        return pxpGlobal.utilities.getPriceString(price);
    }
    private _deliveryPeriodComp(): string {
        if (!this.calculationWasCorrect()) return '';
        if (this.minPeriod() > 0 && this.maxPeriod() > 0)
            return this.loc_twoPointDeliveryPeriod.replace('{0}', this.minPeriod()!.toString()).replace('{1}', this.maxPeriod()!.toString());

        if (this.minPeriod() > 0)
            return this.loc_oneMinPointDeliveryPeriod.replace('{0}', this.minPeriod()!.toString());

        if (this.maxPeriod() > 0)
            return this.loc_oneMaxPointDeliveryPeriod.replace('{1}', this.maxPeriod()!.toString());

        return '';
    }
    private _selectBtnTextComp(): string {
        return this.isSelected() ? this.loc_selectedTitle : this.loc_selectTitle;
    }
    private _selectBtnAttrComp(): object {
        return {
            'disabled': this.isSelected()
        };
    }

    //own
    setFilter(value: boolean, reason: PointFilterReason): void {
        const hasFilter = this._filters.find(f => f[0] == reason);
        if (hasFilter != null) hasFilter[1] = value;
        else this._filters.push([reason, value]);
        this.isVisibleByFilter = !this._filters.filter(f => f[0] != PointFilterReason.Bounds).some(f => !f[1]);
        this.isListedByFilter = !this._filters.some(f => !f[1]);
    }

    //from base
    getTitleFromProvider() {
        switch (this.pointType) {
            case PxpDistributionPointType.Cdek: return this.loc_deliveryPointCdekTitle;
            case PxpDistributionPointType.Boxberry: return this.loc_deliveryPointBoxberryTitle;
            case PxpDistributionPointType.Pickpoint: return this.loc_deliveryPointPickPointTitle;
            case PxpDistributionPointType.ImLogistics: return this.loc_deliveryPointIMLTitle;
            case PxpDistributionPointType.Novaposhta: return this.loc_deliveryPointNovaposhtaTitle;
            case PxpDistributionPointType.YandexDelivery: return this.loc_deliveryPointYandexDeliveryTitle;
            case PxpDistributionPointType.RussianPost: return this.loc_deliveryPointRussianPostTitle;
            case PxpDistributionPointType.DPD: return this.loc_deliveryPointDpdTitle;
            case PxpDistributionPointType.CDEKv2: return this.loc_deliveryPointCdekV2Title;
            case PxpDistributionPointType.Unknown:
            default:
                if (this.providerType === PxpDeliveryPointProviderType.General) {
                    return this.loc_deliveryPointGeneralOfficeTitle;
                }
                return this.loc_deliveryPointOfficeTitle;
        }
    }
}



export type MapMarkerType = ymaps.GeoObject | google.maps.Marker;

export interface IPointDeliveryPointItemLocalization extends IBaseStandartDeliveryPointItemLocalization {
    AddressTitle: string;
    PhoneTitle: string;
    WorkTimeTitle: string;
    DescriptionTitle: string;
    SelectTitle: string;
    SelectedTitle: string;
    InCalculate: string;
    SelectError: string;
    GoBack: string;
    ProviderTitles: IProviderTitlesLocalization;
}

export interface IPxpPointDeliveryPointDto extends IPxpBaseStandartDeliveryPointDto {
    Description: string | null;
    Phone: string;
    WorkTime: string;
    Price: number;
}

export interface IBasePointFinalAddressData {
    ShippingAddressId: number,
    DeliveryProperties: IDeliveryAddressProperties
}

export interface IProviderTitlesLocalization {
    DeliveryPointCdekTitle: string;
    DeliveryPointBoxberryTitle: string;
    DeliveryPointIMLTitle: string;
    DeliveryPointPickPointTitle: string;
    DeliveryPointNovaposhtaTitle: string;
    DeliveryPointYandexDeliveryTitle: string;
    DeliveryPointOfficeTitle: string;
    DeliveryPointRussianPostTitle: string;
    DeliveryPointGeneralOfficeTitle: string;
    DeliveryPointDpdTitle: string;
    DeliveryPointCdekV2Title: string;
}