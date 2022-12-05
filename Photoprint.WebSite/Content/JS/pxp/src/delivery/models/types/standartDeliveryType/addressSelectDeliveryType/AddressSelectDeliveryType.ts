import { CourierDeliveryPointItem, IAddressSelectDeliveryPointItemLocalization, IPxpCourierDeliveryPointDto } from "./AddressSelectDeliveryPointItem";
import { BaseStandartDeliveryTypeModel, IBaseStandartDeliveryTypeLocalization } from "../_baseStandartType/BaseStandartDeliveryTypeModel";
import { DeliveryDisplayType } from "../../../../controllers/DeliveryTypesSelector";
import { IDeliverySettingsAddressSelect, IDeliverySettingsMap } from "../../../WindowSettings";
import { IAvailableDeliveryTypesDto } from "../../_base/BaseDeliveryTypeModel";
import { CityInfo } from "../../../city/CityInfo"
import { BaseAddressModel, IAddressModelLocalization } from "../../../address/BaseAddressModel";
import { AddressSelectDeliveryPointMap, ICourierMapHandlers } from "./viewModels/AddressSelectDeliveryPointMap";
import { DeliveryMessageBlockType, DeliveryMessageType, IMessagesMaster } from "../../../../controllers/MessagesManager";
import { CountryInfo } from "../../../city/CountrySelector";
import { IDeliveryGetPriceResult } from "../../../../_services/DeliveryPriceService";
import { DeliveryManagerService } from "../../../../_services/DeliveryManagerService";
import { IPxpDeliveryAddress } from "../../_base/BaseDeliveryPointItem";


export class AddressSelectDeliveryType extends BaseStandartDeliveryTypeModel {
    type: DeliveryDisplayType = DeliveryDisplayType.Courier; // from base
    template: string = "pxp__deliveryManager__selectAddressTemplate"; // from base

    address: BaseAddressModel;
    map: AddressSelectDeliveryPointMap;

    selectedPoint: KnockoutObservable<CourierDeliveryPointItem | null>; // from base;
    noDelivery: KnockoutObservable<boolean>;
    allPoints: KnockoutObservableArray<CourierDeliveryPointItem>;
    selectorItemClass: KnockoutComputed<string>;

    // localization
    readonly loc_addressSelectDeliveryPointItemLocalization: IAddressSelectDeliveryPointItemLocalization;
    readonly loc_addressRecipientTitle: string;
    readonly loc_deliverySelection: string;
    readonly loc_noDelivery: string;

    // comp
    isShippingsVisible: KnockoutReadonlyComputed<boolean>;
    readyForCalculations: KnockoutReadonlyComputed<boolean>;

    constructor(
        instaceGuid: string,
        mapSettings: IDeliverySettingsMap,
        dto: IAvailableDeliveryTypesDto, 
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>, 
        settings: IDeliverySettingsAddressSelect,
        localization: IAddressSelectDeliveryTypeLocalization,
        master: IMessagesMaster
    ) {
        super(instaceGuid, mapSettings, dto, currentCity, currentCountry, localization, master);
        //binds
        this.onSelectUpdate = this.onSelectUpdate.bind(this);
        this.processDeliveryPointItems = this.processDeliveryPointItems.bind(this);
        this.selectShipping = this.selectShipping.bind(this);

        this.address = new BaseAddressModel(currentCity, currentCountry, settings.UseAddressLines, settings.UsePostcode, localization.AddressModelLocalization, this.messagesMaster);
        this.map = new AddressSelectDeliveryPointMap(mapSettings, this.currentCity, this.currentCountry, <ICourierMapHandlers>{
            setNewAddressByReadyPoint: this.address.setNewAddressFromMap,
            selectedAddressPointData: this.address.addressPointData
        });

        this.selectedPoint = ko.observable(null);
        this.noDelivery = ko.observable(false);
        this.allPoints = ko.observableArray([]);
        this.selectorItemClass = ko.pureComputed(this._selectorClassComp, this);

        // localization
        this.loc_addressSelectDeliveryPointItemLocalization = localization.AddressSelectDeliveryPointItemLocalization;
        this.loc_addressRecipientTitle = localization.RecipientTitle;
        this.loc_deliverySelection = localization.DeliverySelection;
        this.loc_noDelivery = localization.NoDelivery;

        // comp
        this.isShippingsVisible = ko.pureComputed(this._isCourierDeliveriesVisibleComp, this);
        this.readyForCalculations = ko.pureComputed(this._readyForCalculations, this);

        // calls
        this.address.initHandlers({
            map: this.map
        });
         //messages
        this.messagesMaster.addMessages([
            {
                text: this.address.loc_noDeliveryAddressAlert,
                call: this.address.canFindeAddress,
                blockType: DeliveryMessageBlockType.Address,
                type: DeliveryMessageType.Warning,
                callOnNegative: true,
                isShown: this.isSelected
            },
            {
                text: this.loc_noDelivery,
                call: this.noDelivery,
                blockType: DeliveryMessageBlockType.Delivery,
                type: DeliveryMessageType.Error,
                isShown: this.address.isAddressCorrectForExternalCalculations
            }
        ]);
        //subs
        this.map.isMapRendered.subscribe((val) => { 
            if (val && !this.isFullyInited()) { 
                this.isFullyInited(true);
                this.address.getAddressBefore();
            } 
        }, this);
    }

    selectShipping(item: CourierDeliveryPointItem | undefined) {
        if (item == undefined) return;

        this.selectedPoint()?.isSelected(false);
        item.isSelected(true);
        this.selectedPoint(item);
    }

    // from base
    getFinalStateForCreateOrder(): IPxpDeliveryAddress | undefined {
        return this.selectedPoint()?.mergeUserAddress(this.address.finalState());
    }
    getFinalStateForCalculations(): Array<IPxpDeliveryAddress> {
        if (!this.readyForCalculations())
            return [];
        this.allPoints().forEach(p => p.calculationWasCorrect(false));
        return this.allPoints().map(p => {
            p.mergeUserAddress(this.address.finalState());
            return p.addressData;
        });

    }

    processPriceResult(results: IDeliveryGetPriceResult[] | null): void {
        this.allPoints().forEach(point => {
            const result = results?.find(r => point.id == r.ShippingAddressId);
            point.applyCalculationResut(result?.CalculationResult ?? null);
        });

        this.noDelivery(this.allPoints().every(p => !p.calculationWasCorrect()));
    }

    protected processDeliveryPointItems(dto: Array<IPxpCourierDeliveryPointDto>): void {
        var items = dto.map((obj) => { return new CourierDeliveryPointItem(obj, this.loc_addressSelectDeliveryPointItemLocalization) });
        // все точки, что пришли в ответе
        this.allPoints(items);
        if (!this.address.isPostcodeVisible())
            this.address.isPostcodeVisible(items.some( i => i.postCodeRequired));
        this.pointsProcessed = true;
        this.allPoints.notifySubscribers();
        const prevState = DeliveryManagerService.getStoredShippingFinalState();
        if (prevState != undefined) 
            this.selectShipping(this.allPoints().find(p => p.shippingId==prevState.ShippingId && p.addressData.Id==prevState.Id)?? this.allPoints()[0]);
    }

    isCreateOrderEnabled(): boolean {
        return this.isShippingsVisible() && this.selectedPoint() != null;
    }

    // comp
    private _isCourierDeliveriesVisibleComp(): boolean {
        if (!this.address.isAddressCorrectForExternalCalculations()) return false;

        return this.allPoints().some(p => p.isVisible);
    }
    private _readyForCalculations(): boolean {
        return this.address.isAddressCorrectForExternalCalculations()
            && this.allPoints().length>0;
    }
    private _selectorClassComp(): string {
        return document.querySelectorAll('.delivery__selector-item_more-than-3').length>0
        ? "delivery__selector-item_more-than-3"
        : "delivery__selector-item";
    }
}
export interface IAddressSelectDeliveryTypeLocalization extends IBaseStandartDeliveryTypeLocalization {
    RecipientTitle: string;
    DeliverySelection: string;
    NoDelivery: string;
    AddressModelLocalization: IAddressModelLocalization;
    AddressSelectDeliveryPointItemLocalization: IAddressSelectDeliveryPointItemLocalization;
}