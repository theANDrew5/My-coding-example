import { BaseStandartDeliveryTypeModel, IBaseStandartDeliveryTypeLocalization } from "../_baseStandartType/BaseStandartDeliveryTypeModel";
import { PointsDeliveryPointsMap, IPointsDeliveryPointsMapHandlers } from "./viewModels/PointsDeliveryPointsMap";
import { IPxpBaseStandartDeliveryPointDto } from "../_baseStandartType/BaseStandartDeliveryPointItem";
import { DeliveryDisplayType } from "../../../../controllers/DeliveryTypesSelector";
import { IAvailableDeliveryTypesDto } from "../../_base/BaseDeliveryTypeModel";
import { IDeliverySettingsMap, IDeliverySettingsPickpoint } from "../../../WindowSettings";
import { CityInfo } from "../../../city/CityInfo";
import { IMessagesMaster, DeliveryMessageBlockType, DeliveryMessageType } from "../../../../controllers/MessagesManager";
import { DeliveryPointsList } from "../../../pointsList/DeliveryPointsList";
import { CountryInfo } from "../../../city/CountrySelector";
import { IDeliveryGetPriceResult } from "../../../../_services/DeliveryPriceService";
import { DeliveryManagerService } from "../../../../_services/DeliveryManagerService";
import { IPxpDeliveryAddress } from "../../_base/BaseDeliveryPointItem";
import { IPointDeliveryPointItemLocalization, IPxpPointDeliveryPointDto, PointDeliveryPointItem } from "./PointDeliveryPointItem";

export class PointDeliveryType extends BaseStandartDeliveryTypeModel {
    type: DeliveryDisplayType; // from base
    template = "pxp__deliveryManager__pointTemplate"; // from base
    private readonly _listSettings?: IDeliverySettingsPickpoint;

    map: PointsDeliveryPointsMap;
    list: DeliveryPointsList;
    pointInfoTemplate = "pxp__deliveryManager__pointInfoBlock";
       
    selectedPoint: KnockoutObservable<PointDeliveryPointItem | null>; // from base;
    allPoints: KnockoutObservableArray<PointDeliveryPointItem>; // from base;
    readyForCalculations: KnockoutReadonlyComputed<boolean>;// from base
    availablePoints: KnockoutObservableArray<PointDeliveryPointItem>;

    // localization
    readonly loc_pointsTitle: string;
    readonly loc_pointSelectedTitle: string;
    readonly loc_placeholderText: string;
    readonly loc_found: string;
    readonly loc_from: string;
    readonly loc_pointsViewMapLabel: string;
    readonly loc_pointsViewListLabel: string;
    readonly loc_showOnTheMap: string;
    readonly loc_filterByBounds: string;
    readonly loc_allCompanies: string;
    readonly loc_selectPointWarning: string;
    readonly loc_pointItemLocalization: IPointDeliveryPointItemLocalization;

    // comp
    pointsCount: KnockoutReadonlyComputed<number>;
    availablePointsCount: KnockoutReadonlyComputed<number>;
    showSelectPointWarning: KnockoutReadonlyComputed<boolean>;

    public constructor(
        instaceGuid: string,
        mapSettings: IDeliverySettingsMap,
        dto: IAvailableDeliveryTypesDto, 
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>,
        localization: IPointDeliveryTypeLocalization,
        master: IMessagesMaster,
        listSettings: IDeliverySettingsPickpoint | undefined = undefined
    ) {
        super(instaceGuid, mapSettings, dto, currentCity, currentCountry, localization, master);
        
        this.type = dto.Type;
        this._listSettings = listSettings;

        this.selectPoint = this.selectPoint.bind(this);
        this.onSelectUpdate = this.onSelectUpdate.bind(this);
        this.processDeliveryPointItems = this.processDeliveryPointItems.bind(this);
        this.pointInfoScroll = this.pointInfoScroll.bind(this);

        this.selectedPoint = ko.observable(null);
        this.allPoints = ko.observableArray([]);
        this.readyForCalculations = ko.pureComputed(this._readyForCalculationsComp, this);
        this.availablePoints = ko.observableArray([]);
        
        // localization
        this.loc_pointsTitle = localization.PointsTitle;
        this.loc_pointSelectedTitle = localization.PointSelectedTitle;
        this.loc_placeholderText = localization.PlaceholderText;
        this.loc_found = localization.Found;
        this.loc_from = localization.From;
        this.loc_pointsViewMapLabel = localization.PointsViewMapLabel;
        this.loc_pointsViewListLabel = localization.PointsViewListLabel;
        this.loc_showOnTheMap = localization.ShowOnTheMap;
        this.loc_filterByBounds = localization.FilterByBounds;
        this.loc_allCompanies = localization.AllCompanies;
        this.loc_selectPointWarning = localization.SelectPointWarning;
        this.loc_pointItemLocalization = localization.PointItemLocalization;

        this.list = new DeliveryPointsList(this.currentCity, this.currentCountry, this.availablePoints, this.selectedPoint, 
            {
                EmptyList:"На видимой области карты</br>точки выдачи отсутствуют", 
                FilterByBounds: this.loc_filterByBounds,
                AllCompanies: this.loc_allCompanies,
                PlaceholderText: this.loc_placeholderText
            },
            {
                isFilterProviderTypeEnabled: this._listSettings?.IsFilterProviderTypeEnabled?? false,
                isSearchFilterEnabled: this._listSettings?.IsSearchFilterEnabled?? false
            });
        this.map = new PointsDeliveryPointsMap(this.mapSettings, this.availablePoints, this.currentCity, this.currentCountry, this.searchString);

        // comp
        this.pointsCount = ko.pureComputed(this._pointsCountComp, this);
        this.availablePointsCount = ko.pureComputed(this._availablePointsCountComp, this);
        this.showSelectPointWarning = ko.pureComputed(this._showSelectPointWarning, this);

        // subscribes
        this.map.isMapRendered.subscribe((val) => { 
            if (val) {
                if (!this.isFullyInited())
                    this._init();
                const points = this.list.selectedItems();
                if (points.length == 0) return;
                window.setTimeout(() => {this.map.callBubbleCallback?.(points[0].geoObjectOnMap);
                this.list.selectedItems.notifySubscribers()}, 150);
            } 
        }, this);

        //extends
        this.readyForCalculations.extend({ notify: 'always' });

        //messages
        master.addMessage(
            {
                text: this.loc_selectPointWarning,
                call: this.showSelectPointWarning,
                blockType: DeliveryMessageBlockType.Delivery,
                type: DeliveryMessageType.Warning,
                isShown: this.isSelected
            });

    }

    protected unselectPoint(item: PointDeliveryPointItem): void {
        if (item !== null) {
            item.isSelected(false);
        }

        this.selectedPoint(null);
    }

    protected selectPoint(item: PointDeliveryPointItem) {
        if (item == null) return;

        const selectedPoint = this.selectedPoint();
        selectedPoint?.isSelected(false);
        item.isSelected(true);
        this.selectedPoint(item);
    }

    protected pointInfoScroll() {
        this._scrollToElement('.delivery-selected-point');
    }


    private _init() {
        this.list.initHandlers({
            map: this.map
        });
        this.map.initHandlers(<IPointsDeliveryPointsMapHandlers>{
            type_selectPoint: this.selectPoint,
            list_scrollToObjectInList: this.list.scrollToObjectInList,
            list_selectItemOnList: this.list.selectItemOnList,
            list_deSelectItemOnList: this.list.deSelectItemOnList
        });
        this.isFullyInited(true);
    }

    //from base
    protected processDeliveryPointItems(dto: Array<IPxpPointDeliveryPointDto>): void {
        const items = dto
            .map((obj) => new PointDeliveryPointItem(obj, this.loc_pointItemLocalization));
        // все точки, что пришли в ответе
        this.allPoints(items);
        // все точки, что можно вывести клиенту
        const availablePoints = items.filter((p) => { return p.isAvailablePoint });
        this.availablePoints(availablePoints);
        this.pointsProcessed = true;
        const prevState = DeliveryManagerService.getStoredShippingFinalState();
        if (prevState != undefined) {
            this.list.selectOnList(this.availablePoints().find(p => p.shippingId==prevState.ShippingId && p.addressData.Id==prevState.Id));
            return;
        }  
        if (this.availablePoints().length === 1)
            this.list.selectOnList(this.availablePoints()[0]);
    }
    getFinalStateForCalculations(): Array<IPxpDeliveryAddress> {
        if (!this.readyForCalculations())
            return [];
        let points = this.list.selectedItems()!;
        points[0].isLoading(true);
        return points.map(p => p.addressData);
    }
    processPriceResult(results: IDeliveryGetPriceResult[] | null): void {
        let points = this.list.selectedItems();
        if (points.length == 0) return; 
        points[0].isLoading(false);
        points.forEach(point => {
            const result = results?.find(r => point.id == r.ShippingAddressId);
            point.applyCalculationResut(result?.CalculationResult ?? null);
        });
    }
    isCreateOrderEnabled(): boolean {
        return this.selectedPoint()!=null;
    }

    // comp
    private _pointsCountComp(): number {
        return this.list?.visiblePoints().length?? 0;
    }

    private _availablePointsCountComp(): number {
        return this.availablePoints().length;
    }
    private _showSelectPointWarning(): boolean {
        let items = this.list!.selectedItems();
        if (items.length == 0) return false;
        return items.some(i => i.calculationWasCorrect()) && this.selectedPoint()==null;
    }

    private _readyForCalculationsComp(): boolean {
        return this.list.selectedItems().some(item => item.calculationWasCorrect() == null);
    }
    private _scrollToElement(selector: string) {
        var element = document.querySelector(selector);
        if (element == null || !(element instanceof HTMLElement)) return;
        let rect = element.getBoundingClientRect();
/*        if (window.pageYOffset>element.offsetTop) return;*/
        window.scroll(0, rect.top + window.pageYOffset-150);
    }
}

export interface IPointDeliveryTypeLocalization extends IBaseStandartDeliveryTypeLocalization {
    PointsTitle: string;
    PointSelectedTitle: string;
    PlaceholderText: string;
    Found: string;
    From: string;
    PointsViewMapLabel: string;
    PointsViewListLabel: string;
    ShowOnTheMap: string;
    FilterByBounds: string;
    AllCompanies: string;
    SelectPointWarning: string;
    PointItemLocalization: IPointDeliveryPointItemLocalization;
}