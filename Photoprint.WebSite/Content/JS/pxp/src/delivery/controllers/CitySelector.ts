import { AddressService } from "../_services/AddressService";
import { Transliterator } from "../../utils/Transliterator";
import { CitySuggest, ICitySuggest } from "../models/city/CitySuggest";
import { CityInfo, ICityState } from "../models/city/CityInfo";
import { DeliveryMessageBlockType, DeliveryMessagesManager, IMessagesMaster } from "./MessagesManager";
import { CitySelectorDropdown } from "../models/city/CitySelectorDropdown";
import { CityAddressCountry, CountrySelector, ICountriesLocalisation } from "../models/city/CountrySelector";
import { SelectorMode } from "../models/city/SelectorMode";

export class CitySelector {	
    //эта константа юзается в скриптах на сайтах
    //destination-pl.js Яркий
	private readonly _externalCityKey = "pxp_delivery_externalSelectedCity"
	//
    private readonly _cityLocStorageKey = "pxp_delivery_selectedCity"; 

    isReverseSearchAvailable: boolean;
    cityDropdown: CitySelectorDropdown;
	transliterator: Transliterator;
    messagesMaster: IMessagesMaster;
	messagesManager: DeliveryMessagesManager;
    countrySelector: CountrySelector;

    isSearching: KnockoutObservable<boolean>;
	isReverseSearchError: KnockoutObservable<boolean>;
	isReverseSearchLoading: KnockoutObservable<boolean>;
	focusSearchQuery: KnockoutObservable<boolean>;
	searchQuery: KnockoutObservable<string>;
	selectedCity: KnockoutObservable<CityInfo | null>;
	mode: KnockoutObservable<SelectorMode>;
	typeSelectorIsLoading?: KnockoutObservable<boolean>;

    // localization
	readonly loc_selectorTitle: string;
	readonly loc_errorAutoGeoposAlert: string;
	readonly loc_noCityResults: string;
	readonly loc_change: string;
	readonly loc_deliveryValidationErrorAlert: string;
	
	// comp
    isEditMode: KnockoutReadonlyComputed<boolean>;
    isViewMode: KnockoutReadonlyComputed<boolean>;
	isHideMode: KnockoutReadonlyComputed<boolean>;

	constructor(localization: ICitySelectorLocalization, master: IMessagesMaster, countryLimiter: CityAddressCountry) {
		this.changeCity = this.changeCity.bind(this);
		this.findUserGeoLocation = this.findUserGeoLocation.bind(this);
		this.suggestCities = this.suggestCities.bind(this);
		this.onCitiesSuggested = this.onCitiesSuggested.bind(this);
		this.onSuggestSelected = this.onSuggestSelected.bind(this);

        this.isReverseSearchAvailable = false;
        this.cityDropdown = new CitySelectorDropdown(this.onSuggestSelected);
        this.transliterator = new Transliterator();
		this.messagesMaster = master;
		this.messagesManager = this.messagesMaster.getOrCreateManager(DeliveryMessageBlockType.City);
        this.countrySelector = new CountrySelector(localization.Countries, countryLimiter);

        this.isSearching = ko.observable(false);
        this.isReverseSearchError = ko.observable(false);
		this.isReverseSearchLoading = ko.observable(false);
		this.focusSearchQuery = ko.observable(true);
		this.searchQuery = ko.observable("");
		this.selectedCity = ko.observable<CityInfo | null>(null);
		this.mode = ko.observable(SelectorMode.Edit);
        
        // localization
		this.loc_selectorTitle = localization.SelectorTitle;
		this.loc_errorAutoGeoposAlert = localization.ErrorAutoGeoposAlert;
		this.loc_noCityResults = localization.NoCityResults;
		this.loc_change = localization.Change;
		this.loc_deliveryValidationErrorAlert = localization.DeliveryValidationErrorAlert;
        
		// comp
		this.isEditMode = ko.pureComputed(this._isEditModeComp, this);
		this.isViewMode = ko.pureComputed(this._isViewModeComp, this);
		this.isHideMode = ko.pureComputed(this._isHideModeComp, this);

		// subscribes
		this.searchQuery.subscribe(() => { this.suggestCities() }, this);
		this.selectedCity.subscribe((val) => { 
			if (val!=null)
				localStorage.setItem(this._cityLocStorageKey, JSON.stringify(val?.originalData));});

		// extends
		this.focusSearchQuery.extend({ notify: "always" });

		// calls
        this._autoGetcity();
    }

    initHandlers(handlers: ICitySelectorHandlers) {
		this.typeSelectorIsLoading = handlers.TypeSelctorIsLoading;
    }

    findUserGeoLocation() {
        this.isReverseSearchLoading(true);
		const countryId = this.countrySelector.selectedCountry().id;

		AddressService.findCityReverse(countryId)
			.then((city) => {
				this.searchQuery(city.title);
				this.isReverseSearchError(false);
			})
            .catch(() => {
				this.searchQuery("");
                this.cityDropdown.update(null);
                this.isReverseSearchError(true);
            })
			.finally(() => {
				this.isReverseSearchLoading(false);
				this.focusSearchQuery(true);
			});
    }

	suggestCities() {
		const search = this.formatSearchQuery(this.searchQuery());
		if (search != null && search.length > 0 && this.selectedCity() == null ) {
			this.cityDropdown.show();
			this.isReverseSearchError(false);
			this.isSearching(true);
			const countryId = this.countrySelector.selectedCountry().id;
			AddressService.suggestCities(countryId, search)
				.then((resolve) => 
					this.onCitiesSuggested(resolve));
		} else {
			this.cityDropdown.hide();
		}

		this.focusSearchQuery(true);
	}

	onCitiesSuggested(results: ICitySuggest[] | null) {
		if (results === null) return;
		this.cityDropdown.update(results);
		this.isSearching(false);
        this.focusSearchQuery(true);
	}

	formatSearchQuery(query: string) {
		const countryId = this.countrySelector.selectedCountry().id;
		return countryId === 1 ? this.transliterator.latToRus(query) : query;
    }

	onSuggestSelected(suggest: ICitySuggest | undefined) {
		if (suggest == null) return;

		if (suggest instanceof CitySuggest) {
			if (suggest === (this.selectedCity() as ICitySuggest)) {
				this.mode(SelectorMode.View);
				this.cityDropdown.hide();
				return;
			}
			const countryId = this.countrySelector.selectedCountry().id;
			AddressService.getCityAddressesBySuggest(countryId, suggest)
			.then((cities) => {
                if (cities.length == 1) {
					this.onSuggestSelected(cities[0]);
					return;
                }
				this.onCitiesSuggested(cities)
			});
		}
        else if (suggest instanceof CityInfo) {
			if (suggest === this.selectedCity()) {
				this.mode(SelectorMode.View);
				this.cityDropdown.hide();
				return;
			}
			// set city in localstorage 
			localStorage.setItem(this._cityLocStorageKey, JSON.stringify(suggest.originalData));

			this.selectedCity(suggest);
			if (suggest !== null) {
				this.searchQuery(suggest.title);
				this.mode(SelectorMode.View);
				this.cityDropdown.hide();
			}
        }
    }

	changeCity() {
		this.mode(SelectorMode.Edit);
		this.focusSearchQuery(true);
		
		// сразу выделение всего названия в инпуте
		(document.getElementById('deliveryCitySelector') as HTMLInputElement)?.setSelectionRange(0, this.selectedCity()?.title.length ?? 0);

		this.selectedCity(null);
		this.suggestCities();
	}

    // поиск города, который мог быть ранее уже выбран на этой странице
    private _autoGetcity() {
		this._getExternalCity()
			.then(city => {
				this.mode(SelectorMode.Hide);
				this.selectedCity(city);
				return;
			})
			.catch(() => {
				this._getPrevCity()
                    .then(city => {
						this.mode(SelectorMode.View)
						this.selectedCity(city);
						return;
                    })
                    .catch(() => {
						this.findUserGeoLocation();
						return;
                    })
			});
    }
    private _getExternalCity(): Promise<CityInfo> {
		return new Promise<CityInfo>((resolve, reject) => {
			const jSting = localStorage.getItem(this._externalCityKey);
			if (jSting == null && jSting == '') {
				reject();
				return;
			}
			const countryId = this.countrySelector.selectedCountry().id;
			const exCity = <ICityState>JSON.parse(jSting!);
			let query = exCity.Title;
			if (exCity.Area !== "" && exCity.Area !== undefined)
				query = `${exCity.Area}, ${query}`;
			if (exCity.Region !=="" && exCity.Area !== undefined)
				query = `${exCity.Region}, ${query}`;
			AddressService.getCityAddressesByQuery(countryId, query)
				.then((cities) => {
                    if (cities[0] == undefined) {
						reject();
						return;
                    }
					resolve(cities[0]);
					return;
				})
				.catch(() => {
					reject();
					return;
				})
		});

    }
	private _getPrevCity(): Promise<CityInfo> {
        return new Promise<CityInfo>((resolve, reject) => {
			const jSting = localStorage.getItem(this._cityLocStorageKey);
			if (jSting == null && jSting == '') {
				reject();
				return;
			}
			const prevCity = <ICityState>JSON.parse(jSting!);

			const hasTitle = prevCity.Title != null || prevCity.Title != '';
            if (!hasTitle) {
				reject();
				return;
            }
			const hasCoords = prevCity.Latitude != undefined && prevCity.Latitude != "" 
				&& prevCity.Longitude != undefined && prevCity.Longitude != "";
			if (hasTitle && hasCoords) {
				resolve(new CityInfo(prevCity));
				return;
			}
			const countryId = this.countrySelector.selectedCountry().id;
			let query = prevCity.Title;
			let hasArea = prevCity.Area !== "" && prevCity.Area !== undefined;
			if (hasArea)
				query = `${prevCity.Area}, ${query}`;
			let hasRegion = prevCity.Region !=="" && prevCity.Area !== undefined;
			if (hasRegion)
				query = `${prevCity.Region}, ${query}`;
			AddressService.getCityAddressesByQuery(countryId, query)
				.then((cities) => {
					const city = cities.find(s => s.title == prevCity.Title 
						&& (!hasArea || s.area == prevCity.Area)
						&& (!hasRegion || s.region == prevCity.Region));
                    if (city == undefined) {
						reject();
						return;
                    }
					resolve(city);
					return;
				})
                .catch(() => {
					reject();
					return;
                });
        });
    }

	// comp
	private _isEditModeComp(): boolean {
		return this.mode() === SelectorMode.Edit;
    }
	private _isViewModeComp(): boolean {
		return this.mode() === SelectorMode.View;
    }
    private _isHideModeComp(): boolean {
		return this.mode() === SelectorMode.Hide;
    }
}

export interface ICitySelectorLocalization {
	SelectorTitle: string,
	ErrorAutoGeoposAlert: string,
	NoCityResults: string,
	Change: string,
	DeliveryValidationErrorAlert: string,
    Countries: ICountriesLocalisation
}

export interface ICitySelectorHandlers {
	TypeSelctorIsLoading: KnockoutObservable<boolean>;
}