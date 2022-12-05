import { AddressService, SuggestType } from "../../_services/AddressService";
import { IBoundedPointData } from "../address/BaseAddressModel";
import { IAddressSuggest, SuggestedStreetItem } from "../address/suggests/SuggestedStreetItem";
import { ISuggestDropdownHandlers, SuggestBaseDropdown } from "../address/suggests/_base/SuggestBaseDropdown";
import { CityInfo } from "../city/CityInfo";
import { CountryInfo } from "../city/CountrySelector";
import { PointsDeliveryPointsMap } from "../types/standartDeliveryType/pointDeliveryType/viewModels/PointsDeliveryPointsMap";

export class PointsMapPlaceSearch {

    protected searchDropdown: SuggestSearchDropdown;

    private _searchStreet: string;
    private _searchHouse: string;
    private _rewritePoc: boolean;

    private _map: PointsDeliveryPointsMap | null;

    currentCity: KnockoutObservable<CityInfo | null>;
    currentCountry: KnockoutObservable<CountryInfo>;

    searchString: KnockoutObservable<string>;
    searchStringFocus: KnockoutObservable<boolean>;

    isSearchDropdownVisible: KnockoutReadonlyComputed<boolean>;

    constructor(
        currentCity: KnockoutObservable<CityInfo | null>,
        currentCountry: KnockoutObservable<CountryInfo>,
        searchString: KnockoutObservable<string>,
        searchStringFocus: KnockoutObservable<boolean>) {

        this.currentCity = currentCity;
        this.currentCountry = currentCountry;

        this.searchString = searchString;
        this.searchStringFocus = searchStringFocus;
        
        this._searchStreet="";
        this._searchHouse="";
        this._rewritePoc=false;
        this._map = null;

        //comp
        this.isSearchDropdownVisible = ko.pureComputed(this._isSearchDropdownVisibleComp, this)
        //binds
        this._selectAddressFromSuggestDropdown = this._selectAddressFromSuggestDropdown.bind(this);
        //subs
        this.searchString.subscribe(val => this._findPlaces(val), this);;

        this.searchDropdown = new SuggestSearchDropdown(this.currentCity, {
            selectItem: this._selectAddressFromSuggestDropdown, 
            nextFocusElement: this.searchStringFocus});

    }

    initMap(map: PointsDeliveryPointsMap | null) {
        this._map = map;
    }

    private _selectAddressFromSuggestDropdown(item: IAddressSuggest | null) { 
        if (item == null) return;
        this._searchStreet = item.Street;
        this._searchHouse = item.House ?? '';// ?? this._searchHouse;
        this._rewritePoc = true;
        this.searchString(`${this._searchStreet}, ${this._searchHouse}`);
        this._rewritePoc = false;
        AddressService.findStreetAddressDataBySuugest(item)
            .then(pointData => this._map?.setPointByPointData?.(pointData))
            .catch(er => this._map?.setPointByPointData?.(
                {description: item.Description, 
                    lat: item.Latitude != null ? parseFloat(item.Latitude) : null, 
                    lon: item.Longitude !=null ? parseFloat(item.Longitude) : null
                } as IBoundedPointData))
            .finally(() => {
                this.searchString.notifySubscribers();
                this.searchDropdown.isShown(false);
            })
    }

    private _findPlaces(query: string) {
        if (query == '' || query == null) {
            this.searchDropdown?.update(null);
            return;
        }
        const city = this.currentCity()?.title;
        if (city == null || city === '') return;
        const splitedQuery = query.split(", ", 2);
        if (splitedQuery[0] == this._searchStreet && splitedQuery[1] == this._searchHouse)
            return;
        const searchStreet = splitedQuery[0] !== this._searchStreet;
        AddressService.findSuggestAddresses(
            searchStreet? splitedQuery[0] : splitedQuery[1],
            this.currentCity()!.getState(),
            searchStreet? SuggestType.Street : SuggestType.House,
            searchStreet? null : this._searchStreet)
            .then((result) => {
                const list = result?.map((obj) => { return new SuggestedStreetItem(obj); }) ?? [];
                this.searchDropdown.update(list);
            })
            .catch(() => {
               this.searchDropdown.update(null);
            });
    }

    private _isSearchDropdownVisibleComp(): boolean {
        return this.searchDropdown != null && this.searchDropdown.list().length > 0 && this.searchDropdown.isShown();
    }
}

export class SuggestSearchDropdown extends SuggestBaseDropdown {

    constructor(currentCity: KnockoutObservable<CityInfo | null>, handlers: ISuggestDropdownHandlers) {
        super(currentCity, handlers, ".delivery-search-control");
    }

    update(items: SuggestedStreetItem[] | null) {
        if (items == null) {
            this.isShown(false);
            return;
        }
        var city = this.currentCity();
        if (city === null) return;

        var filteredItems: { [key: string]: SuggestedStreetItem } = { };
        for (var item of items) {
            if (item.city.Title !== city.title || item.street == null) continue;
            if (Object.keys(filteredItems).indexOf(item.house) > 0) continue;

            filteredItems[item.addressForDropdown] = item;
        }

        this.list(Object.values(filteredItems));
        this.isShown(items.length > 0);
    }

}