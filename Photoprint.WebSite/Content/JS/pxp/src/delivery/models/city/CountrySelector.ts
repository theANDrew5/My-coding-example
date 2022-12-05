import { SelectorMode } from "./SelectorMode";
import { CountrySelectorDropdown } from "./CountrySelectorDropdown";

export class CountryInfo {
    readonly id: CityAddressCountry;
    readonly title: string;

    constructor(countries: ICountriesLocalisation,id: CityAddressCountry) {
        this.id = id;
        switch (id) {
        case CityAddressCountry.Russia:
            this.title = countries.Russia;
            break;
        case CityAddressCountry.Ukraine:
            this.title = countries.Ukraine;
            break;
        case CityAddressCountry.Belarus:
            this.title = countries.Belarus;
            break;
        case CityAddressCountry.Kazakhstan:
            this.title = countries.Kazakhstan;
            break;
        case CityAddressCountry.Bulgaria:
            this.title = countries.Bulgaria;
            break;
        default:
            this.title = countries.NoCountry;
            break;
        }
    }
}

export enum CityAddressCountry {
    NoCountry = 0,
	Russia = 1,
	Ukraine = 2,
	Belarus = 3,
	Kazakhstan = 4,
    Bulgaria = 5
}

export class CountrySelector {
    countryDropdown: CountrySelectorDropdown;
    selectedCountry: KnockoutObservable<CountryInfo>;
    mode: KnockoutObservable<SelectorMode>;
    isViewMode: KnockoutComputed<boolean>;
    isEditMode: KnockoutComputed<boolean>;
    readonly isVisible: boolean;
    protected countries: Array<CountryInfo>;
    private readonly _countryLocStorageKey = "pxp_delivery__beforeSelectedCountry";

    //localization
    readonly loc_countryFilter: string;

    constructor(localization: ICountriesLocalisation, countryLimiter: CityAddressCountry) {
        //binds
        this.onSelectedCountry = this.onSelectedCountry.bind(this);
        this.changeCountry = this.changeCountry.bind(this);
        this._getCountryBefore = this._getCountryBefore.bind(this);
        //set
        this.selectedCountry = ko.observable<CountryInfo>(new CountryInfo(localization, countryLimiter));
        this.mode = ko.observable(SelectorMode.View);
        this.isVisible = countryLimiter === CityAddressCountry.NoCountry;
        if (this.isVisible) {
            const beforeCountry = this._getCountryBefore();
            if (beforeCountry != null) this.selectedCountry(beforeCountry);
        }
        this.countries = [
            new CountryInfo(localization, 0),
            new CountryInfo(localization, 1),
            new CountryInfo(localization, 2),
            new CountryInfo(localization, 3),
            new CountryInfo(localization, 4),
            new CountryInfo(localization, 5)
        ];
        this.countryDropdown = new CountrySelectorDropdown(this.onSelectedCountry, this.countries);
        //localisation
        this.loc_countryFilter = localization.CountryFilter;
        //comp
        this.isViewMode = ko.pureComputed(() => {return this.mode() === SelectorMode.View}, this);
        this.isEditMode = ko.pureComputed(() => {return this.mode() === SelectorMode.Edit}, this);
        //subs
        this.selectedCountry.subscribe((val) => {
            if (val == null) return;
            localStorage.setItem(this._countryLocStorageKey, JSON.stringify(val));
        }, this);
    }

    protected changeCountry() {
        this.mode(SelectorMode.Edit);
        this.countryDropdown.show();
    }

    onSelectedCountry(country: CountryInfo | undefined): void {
        if (country == null) return;
        this.selectedCountry(country);
        this.mode(SelectorMode.View);
    }

    private _getCountryBefore(): CountryInfo | null {
        const beforeCountryStr = localStorage.getItem(this._countryLocStorageKey);
        if (beforeCountryStr == null || beforeCountryStr.length === 0) return null;
        return JSON.parse(beforeCountryStr) as CountryInfo;
    }

}

export interface ICountriesLocalisation {
    CountryFilter: string;
    NoCountry: string,
    Russia: string,
    Ukraine: string,
    Belarus: string,
    Kazakhstan: string,
    Bulgaria: string
}