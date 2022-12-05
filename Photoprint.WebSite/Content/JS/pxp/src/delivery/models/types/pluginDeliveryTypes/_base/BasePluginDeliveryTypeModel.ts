import { BaseDeliveryTypeModel, IAvailableDeliveryTypesDto, IBaseDeliveryTypeLocalization } from "../../_base/BaseDeliveryTypeModel";
import { DeliveryDisplayType } from "../../../../controllers/DeliveryTypesSelector";
import { BasePluginDeliveryPointItem } from "./BasePluginDeliveryPointItem";
import { CityInfo } from "../../../city/CityInfo";
import { IMessagesMaster } from "../../../../controllers/MessagesManager";
import { CountryInfo } from "../../../city/CountrySelector";
import { IDeliveryGetPriceResult } from "../../../../_services/DeliveryPriceService";
import { IPxpDeliveryAddress } from "../../_base/BaseDeliveryPointItem";

export abstract class BasePluginDeliveryTypeModel extends BaseDeliveryTypeModel {    
    abstract onSelectUpdate(): void;
    abstract type: DeliveryDisplayType;
    abstract template: string;
    abstract selectedPoint: KnockoutObservable<BasePluginDeliveryPointItem | null>;

    readonly shippingId: number;

    // localization
    readonly loc_pointSelectedTitle: string;

    // comp
    readyForCalculations: KnockoutReadonlyComputed<boolean>;//from base

    constructor(
        dto: IAvailableDeliveryTypesDto, 
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>,
        localization: IBasePluginDeliveryTypeLocalization,
        master: IMessagesMaster,
    ) {
        super(dto, currentCity, currentCountry, localization, master);

        this.shippingId = this.shippingIds[0];

        this.onSelectUpdate = this.onSelectUpdate.bind(this);

        // localization
        this.loc_pointSelectedTitle = localization.PointSelectedTitle

        // comp
        this.readyForCalculations = ko.pureComputed(this._readyForCalculationsComp, this)
    }

    //from base
    getFinalStateForCalculations(): IPxpDeliveryAddress[] {
        if (!this.readyForCalculations())
            return [];
        return [this.selectedPoint()!.addressData];
    }
    processPriceResult(results: IDeliveryGetPriceResult[] | null): void {
        let point = this.selectedPoint();
        if (point == null) return;
        const result = results?.find(r => point!.shippingId == r.ShippingId);
        point.applyCalculationResut(result?.CalculationResult ?? null);;  
    }

    // comp
    private _readyForCalculationsComp(): boolean {
        return this.selectedPoint()!=null;
    }
}

export interface IBasePluginDeliveryTypeLocalization extends IBaseDeliveryTypeLocalization { 
    PointSelectedTitle: string;
}