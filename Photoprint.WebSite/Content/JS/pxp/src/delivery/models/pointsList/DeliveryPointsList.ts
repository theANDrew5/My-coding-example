import { event } from "yandex-maps";
import { CityInfo } from "../city/CityInfo";
import { CountryInfo } from "../city/CountrySelector";
import { PointDeliveryPointItem } from "../types/standartDeliveryType/pointDeliveryType/PointDeliveryPointItem";
import { PointsDeliveryPointsMap } from "../types/standartDeliveryType/pointDeliveryType/viewModels/PointsDeliveryPointsMap";
import { BoundsFilter } from "./filters/BoundsFilter";
import { ShippingFilter } from "./filters/TypeFilter";
import { PointsMapPlaceSearch } from "./PointsMapPlaceSearch";

export class DeliveryPointsList {
    private _handlers: IPointsDeliveryPointsListHandlers | null;

    template: string = "pxp__deliveryManager__pointsList";
    isFilterProviderTypeEnabled: boolean;
    isSearchFilterEnabled: boolean;
    //observables
    currentCity: KnockoutObservable<CityInfo | null>;
    currentCountry: KnockoutObservable<CountryInfo>;
    selectedPoint: KnockoutObservable<PointDeliveryPointItem | null>;
    availablePoints: KnockoutObservableArray<PointDeliveryPointItem>;
    selectedItems: KnockoutObservableArray<PointDeliveryPointItem>;

    isPanelVisible: KnockoutObservable<boolean>;
    filterByBounds: KnockoutObservable<boolean>;
    selectedShippingFilter: KnockoutObservable<number>;
    searchString: KnockoutObservable<string>;
    searchStringFocus: KnockoutObservable<boolean>;

    byBoundsFilter: BoundsFilter;
    typeFilter: ShippingFilter | null;
    placeSearch: PointsMapPlaceSearch | null;

    // localization
    readonly loc_filterByBounds: string;
    readonly loc_emptyList: string;
    readonly loc_allCompanies: string;
    readonly loc_placeholderText: string; 

    // comp
    isListEmpty: KnockoutComputed<boolean>;
    visiblePoints: KnockoutComputed<Array<PointDeliveryPointItem>>;
    togglePanelVisibleBtnText: KnockoutReadonlyComputed<string>;
    isShippingFilterVisible: KnockoutComputed<boolean>;
    shippingFilters: KnockoutComputed<Array<IPickPointProviderFilterModel>>;

    constructor(
        currentCity: KnockoutObservable<CityInfo | null>,
        currentCountry: KnockoutObservable<CountryInfo>,
        availablePoints: KnockoutObservableArray<PointDeliveryPointItem>, 
        selectedPoint: KnockoutObservable<PointDeliveryPointItem | null>,
        localisation: IPointListLocalisation,
        settings: IPointsListSettings) {

        this._handlers = null;

        this.isFilterProviderTypeEnabled = settings.isFilterProviderTypeEnabled;
        this.isSearchFilterEnabled = settings.isSearchFilterEnabled;

        this.currentCity = currentCity;
        this.currentCountry = currentCountry;
        this.selectedPoint = selectedPoint;
        this.availablePoints = availablePoints;
        this.selectedItems = ko.observableArray([]);

        this.isPanelVisible = ko.observable(false);
        this.filterByBounds = ko.observable(true);
        this.selectedShippingFilter = ko.observable(-1);
        this.searchString = ko.observable("");
        this.searchStringFocus = ko.observable(false);
        
        this.byBoundsFilter = new BoundsFilter(this.availablePoints, this.filterByBounds);
        this.typeFilter = settings.isFilterProviderTypeEnabled? 
            new ShippingFilter(this.availablePoints, this.selectedShippingFilter):
            null;
        this.placeSearch = settings.isSearchFilterEnabled?
            new PointsMapPlaceSearch(this.currentCity, this.currentCountry, this.searchString, this.searchStringFocus):
            null;

        // localization
        this.loc_filterByBounds = localisation.FilterByBounds;
        this.loc_emptyList = localisation.EmptyList;
        this.loc_allCompanies = localisation.AllCompanies;
        this.loc_placeholderText = localisation.PlaceholderText;

        // comp
        this.visiblePoints = ko.pureComputed(this._visiblePointsComp, this);
        this.isListEmpty = ko.pureComputed(this._isListEmptyComp, this);
        this.togglePanelVisibleBtnText = ko.pureComputed(this._togglePanelVisibleBtnTextComp, this);
        this.isShippingFilterVisible = ko.pureComputed(this._isShippingFilterVisibleComp, this);
        this.shippingFilters = ko.pureComputed(this._shippingFiltersComp, this);

        // subscribes
        this.selectedPoint.subscribe(item => {
            this.selectedItems().forEach(i => i.isSelectedOnList(false));
            item?.isSelectedOnList(true);
        }, this);

        //binds
        this.togglePanelVisibleOnSmallDisplay = this.togglePanelVisibleOnSmallDisplay.bind(this);
        this.selectOnList = this.selectOnList.bind(this);
        this.selectItemOnList = this.selectItemOnList.bind(this);
        this.deSelectItemOnList = this.deSelectItemOnList.bind(this);

        //extends
        //this.selectedItems.extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
    }

    initHandlers(handlers: IPointsDeliveryPointsListHandlers) {
        this._handlers = handlers;

        this.byBoundsFilter.initHandlers({
            map: this._handlers.map});
        this.typeFilter?.initHandlers({
            map: this._handlers.map
        });
        this.placeSearch?.initMap(this._handlers.map);
    }


    selectOnList(item: PointDeliveryPointItem | undefined) {
        if (item == null || item.isSelectedOnList()) return;
        this.availablePoints().forEach(i => i.isSelectedOnList(false));
        item.isSelectedOnList(true);
        this.selectedItems([item]);
        this._handlers?.map?.callBubbleCallback?.(item.geoObjectOnMap);
        if (document.querySelector('.delivery-view-list-container_toggle-button') != undefined)
            this.togglePanelVisibleOnSmallDisplay();
    }

    //для точки выбраной с карты, бабл уже на карте
    selectItemOnList(items: PointDeliveryPointItem [] | null) {
        if (items == null) return;
        this.availablePoints().forEach(i => i.isSelectedOnList(false));
        items.forEach(i => i.isSelectedOnList(true));
        this.selectedItems(items);
    }

    deSelectItemOnList(items: PointDeliveryPointItem [] | null) {
        if (items == null) return;
        items.forEach(i => i.isSelectedOnList(false));
        this.selectedItems(this.availablePoints().filter(p => p.isSelectedOnList()));
    }
    
    // крутим до адреса в листе
    scrollToObjectInList(item: PointDeliveryPointItem) {
        var listEl: HTMLElement | null = document?.getElementById?.('delivery-points-list');
        if (listEl == null) return;

        var el: HTMLElement | null = document.getElementById(item.addressData.Id?.toString()??'');
        if (el == null) return;

        // элемент на виду в листе
        if (el.offsetTop > listEl.scrollTop && (el.offsetTop + el.offsetHeight) < (listEl.scrollTop + listEl.offsetHeight)) return;

        listEl.scrollTop = el.offsetTop;
    }
    
    togglePanelVisibleOnSmallDisplay() {
        this.isPanelVisible(!this.isPanelVisible());
    }

    // comp
    private _selectedItemsComp(): Array<PointDeliveryPointItem> {
        return this.availablePoints().filter(p => p.isSelectedOnList());
    }
    private _visiblePointsComp(): Array<PointDeliveryPointItem> {
        return this.availablePoints().filter(p => p.isListedByFilter);
    }
    private _isListEmptyComp(): boolean {
        return this.visiblePoints().length === 0;
    }
    private _togglePanelVisibleBtnTextComp(): string {
        return this.isPanelVisible() ? "&#60;&#60;" : "&#62;&#62;";
    }
    private _isShippingFilterVisibleComp(): boolean {
        return this.isFilterProviderTypeEnabled && this.shippingFilters().length > 2;
    }
    private _shippingFiltersComp(): Array<IPickPointProviderFilterModel> {
        var result: Array<IPickPointProviderFilterModel> = [{ shippingId: -1, text: this.loc_allCompanies }]

        var parsedShippings: Array<number> = [];
        for (var point of this.availablePoints()) {
            var shippingId = point.shippingId;
            if (parsedShippings.indexOf(shippingId) > -1) continue;

            parsedShippings.push(shippingId);
            result.push(<IPickPointProviderFilterModel>{
                shippingId: shippingId,
                text: point.shippingTitle
            });
        }

        return parsedShippings.length <= 1 ? [] : result;
    }
}

export interface IPointsDeliveryPointsListHandlers {
    map: PointsDeliveryPointsMap | null;
}

export interface IPickPointProviderFilterModel {
    shippingId: number;
    text: string;
}

export interface IPointListLocalisation {
    FilterByBounds: string;
    EmptyList: string;
    AllCompanies: string;
    PlaceholderText: string;
}

export interface  IPointsListSettings{
    isFilterProviderTypeEnabled: boolean;
    isSearchFilterEnabled: boolean;
}