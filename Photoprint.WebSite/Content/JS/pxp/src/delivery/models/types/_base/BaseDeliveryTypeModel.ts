import { DeliveryDisplayType } from "../../../controllers/DeliveryTypesSelector";
import { BaseDeliveryPointItem, IPxpDeliveryAddress } from "./BaseDeliveryPointItem";
import { CityInfo } from "../../city/CityInfo";
import { IMessagesMaster } from "../../../controllers/MessagesManager";
import { CountryInfo } from "../../city/CountrySelector";
import { IDeliveryGetPriceResult } from "../../../_services/DeliveryPriceService";

export abstract class BaseDeliveryTypeModel {
    abstract onSelectUpdate(): void;
    abstract isCreateOrderEnabled(): boolean;
    abstract getFinalStateForCalculations(): Array<IPxpDeliveryAddress>;
    abstract processPriceResult(results: Array<IDeliveryGetPriceResult> | null): void;

    abstract type: DeliveryDisplayType;
    abstract template: string;
    abstract selectedPoint: KnockoutObservable<BaseDeliveryPointItem | null>;
    abstract readyForCalculations: KnockoutReadonlyComputed<boolean>;

    isFullyInited: KnockoutObservable<boolean>;
    title: KnockoutObservable<string>;
    description: KnockoutObservable<string>;
    isLoading: KnockoutObservable<boolean>;
    isSelected: KnockoutObservable<boolean>;
    currentCity: KnockoutObservable<CityInfo | null>;
    currentCountry: KnockoutObservable<CountryInfo>;

    readonly shippingIds: number[];

    protected messagesMaster: IMessagesMaster;

    // comp
    isPointSelected: KnockoutComputed<boolean>;
    finalDeliveryPrice: KnockoutComputed<number>;

    constructor(
        dto: IAvailableDeliveryTypesDto, 
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>,
        localization: IBaseDeliveryTypeLocalization,
        master: IMessagesMaster
    ) {
        this.isFullyInited = ko.observable(false);
        this.title = ko.observable(dto.Title);
        this.description = ko.observable(dto.TitleNote);
        this.currentCity = currentCity;
        this.currentCountry = currentCountry;
        this.isLoading = ko.observable(false);
        this.isSelected = ko.observable(false);

        this.shippingIds = dto.ShippingList;

        this.messagesMaster = master;

        // comp
        this.isPointSelected = ko.pureComputed(this._isPointSelectedComp, this);
        this.finalDeliveryPrice = ko.pureComputed(this._finalDeliveryPriceComp, this);

        // subscribes
        this.isSelected.subscribe((val) => { if (val) { this.onSelectUpdate(); } }, this);
    }

    // virtual
    afterRender() { }

    // comp
    private _isPointSelectedComp(): boolean {
        return this.selectedPoint() != null;
    }
    private _finalDeliveryPriceComp(): number {
        var selectedItem = this.selectedPoint();
        return selectedItem?.deliveryPrice() ?? 0;
    }
    getFinalStateForCreateOrder(): IPxpDeliveryAddress | undefined {
        return this.selectedPoint()?.addressData;
    }
}

export interface IAvailableDeliveryTypesDto {
    Type: DeliveryDisplayType;
    ShippingList: number[];
    Title: string;
    TitleNote: string;
    IsDefaultType: boolean;
}

export interface IBaseDeliveryTypeLocalization { }