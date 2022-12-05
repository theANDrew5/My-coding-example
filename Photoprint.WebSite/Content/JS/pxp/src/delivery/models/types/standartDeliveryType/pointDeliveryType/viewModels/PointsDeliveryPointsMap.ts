import { BaseDeliveryMap } from "../../_baseStandartType/viewModels/BaseDeliveryMap";
import { IDeliverySettingsMap } from "../../../../WindowSettings";
import { CityInfo } from "../../../../city/CityInfo";
import { CountryInfo } from "../../../../city/CountrySelector";
import { MapMarkerType, PointDeliveryPointItem } from "../PointDeliveryPointItem";
import { IBoundedPointData } from "../../../../address/BaseAddressModel";

export class PointsDeliveryPointsMap extends BaseDeliveryMap {
    private _handlers: IPointsDeliveryPointsMapHandlers | null;
    
    callBubbleCallback: ((geoObj: MapMarkerType | null) => void) | null;
    setCenterByAddressName: ((addressName: string | null, zoom?: number)=> void) | null;
    setPointByPointData: ((suggest: IBoundedPointData)=>void) | null;
    refreshMap: (()=>void) | null;
    resetMapView: ((item: PointDeliveryPointItem | null)=>void) | null;

    searchString: KnockoutObservable<string>;
    currentMapVisibleCoordsBounds: KnockoutObservable<number[][] | null>;
    availablePoints: KnockoutObservableArray<PointDeliveryPointItem>;

    // localization
    readonly loc_from: string;

    // comp
    isAvailablePointsExist: KnockoutComputed<boolean>;

    constructor(
        mapSettings: IDeliverySettingsMap,
        availablePoints: KnockoutObservableArray<PointDeliveryPointItem>,
        currentCity: KnockoutObservable<CityInfo | null>,
        currentCountry: KnockoutObservable<CountryInfo>,
        searchString: KnockoutObservable<string>
    ) {
        super(mapSettings, currentCity, currentCountry);

        this.selectItemsOnList = this.selectItemsOnList.bind(this);
        this.selectPointFromMap = this.selectPointFromMap.bind(this);
        this.callGeoObjOnMap = this.callGeoObjOnMap.bind(this);
        this.afterRenderMap = this.afterRenderMap.bind(this);

        this._handlers = null;

        this.callBubbleCallback = null;
        this.setCenterByAddressName = null;
        this.setPointByPointData = null;
        this.refreshMap = null;
        this.resetMapView = null;

        this.isMapRendered = ko.observable(false);
        this.availablePoints = availablePoints;
        this.searchString = searchString;
        this.currentMapVisibleCoordsBounds = ko.observable(null);

        // localization
        this.loc_from = "от";

        // comp
        this.isAvailablePointsExist = ko.pureComputed(this._isAvailablePointsExistComp, this);

        // extends
        this.currentMapVisibleCoordsBounds.extend({ rateLimit: { timeout: 150, method: "notifyWhenChangesStop" } });

    }

    initHandlers(handlers: IPointsDeliveryPointsMapHandlers) {
        this._handlers = handlers;
    }

    afterRenderMap() { }
    
    selectPointFromMap(item: PointDeliveryPointItem | null) {
        if (item == null) return;
        this._stopBlinking();
        this._handlers?.type_selectPoint(item);
        this._handlers?.list_scrollToObjectInList(item);
    }
    
    selectItemsOnList(items: PointDeliveryPointItem []) {
        this._handlers?.list_selectItemOnList(items);
    }
    deselectItemsOnList(items: PointDeliveryPointItem []) {
        this._handlers?.list_deSelectItemOnList(items);
    }
    // вызываем бабл точки на карте
    callGeoObjOnMap(item: PointDeliveryPointItem | null) {
        if (item == null) return;

        var geoObj = item.geoObjectOnMap;
        if (geoObj == null) return;

        (<any>geoObj).__pxpDId = item.addressData.Id;
        this.callBubbleCallback?.(geoObj);       
    }
    
    // comp
    private _isAvailablePointsExistComp(): boolean {
        return this.availablePoints().length > 0;
    }

    private _stopBlinking() {
        const element = document.querySelector('.item-btn__blink');
        element?.classList.remove('item-btn__blink');
    }
}

export interface IPointsDeliveryPointsMapHandlers {
    type_selectPoint: (item: PointDeliveryPointItem) => void;
    list_scrollToObjectInList: (item: PointDeliveryPointItem) => void;
    list_selectItemOnList: (items: PointDeliveryPointItem [] | null) => void;
    list_deSelectItemOnList: (items: PointDeliveryPointItem [] | null) => void;
}