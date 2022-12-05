
import { type } from "jquery";
import { AddressInfo, AddressSuggest, AddressSuggestType, IAddressInfoDTO, IAddressSuggestDTO } from "../Address/AddressInfo";
import { CityInfo, ToponimType, CitySuggest, ICityState, ICitySuggestDTO } from "../Address/CityInfo";
import { pxpGlobal } from "../globals/pxp";
import { ObjectCache } from "../utils/ObjectCache";
import AdminBaseService from "./AdminBaseService";

interface IAddressRequest {
    FrontendId?: number;
	LanguageId?: number;
	Data: ISuggestCitiesData | ICityInfoData | IAddressSuggestData | IAddressInfoData
}

interface ISuggestCitiesData {
    CountryId: number;
    Query: string;
	Type: ToponimType;
}

interface ICityInfoData {
    CountryId: number;
    Suggest?: ICitySuggestDTO;
    Query?: string;
}

interface IAddressSuggestData {
    Query: string;
	Street?: string;
	City: ICityState;
	Type: AddressSuggestType,
}

export interface IAddressInfoData {
	Suggest?: IAddressSuggestDTO;
	Coords?: LatLong;
	Query?: string;
}

interface LatLong {
	Latitude?: string,
    Longitude?: string
}

export class AddressService extends AdminBaseService {
	private static readonly _citySuggestCache: ObjectCache<Array<CitySuggest>> = new ObjectCache<Array<CitySuggest>>();
	private static readonly _cityInfoCache: ObjectCache<Array<CityInfo>> = new ObjectCache<Array<CityInfo>>();
	private static readonly _addressSuggestCache: ObjectCache<Array<AddressSuggest>> = new ObjectCache<Array<AddressSuggest>>();
	private static readonly _addressInfoCache: ObjectCache<AddressInfo> = new ObjectCache<AddressInfo>();
	private static throttleTimeoutId: number | undefined = undefined;
	private static readonly throttleTimeoutMs: number = 250;

    static suggestCities(countryId: number, type: ToponimType, searchQuery: string): Promise<CitySuggest[]> {
		return new Promise<CitySuggest[]>((resolve, reject) => {
			const search = searchQuery.trim();
			if (!search) {
				reject();
				return;
			}

			var key = `${countryId}:${type}:${search}`;
			const inCache = this._citySuggestCache.get(key);
			if (inCache !== null) {
				resolve(inCache);
				return;
			}

			clearTimeout(this.throttleTimeoutId);

			this.throttleTimeoutId = window.setTimeout(() => {
				var request: IAddressRequest = {
					FrontendId: pxpGlobal.frontend?.id,
					LanguageId: pxpGlobal.systemLanguageId,
                    Data: {
						Query: search,
						CountryId: countryId,
						Type: type
                    }
				}

				this.post<ICitySuggestDTO[]>('/api/admin/address/suggestCity', request)
                    .then((data) => {
						let result = data.map(c => new CitySuggest(c));
						this._citySuggestCache.set(key, result);
						resolve(result);
                    })
                    .catch((data) => {
						reject(data);
                    });
			}, this.throttleTimeoutMs);
		});
	}

	static getCityInfo(countryId: number, suggest: CitySuggest): Promise<CityInfo[]> {
		return new Promise<CityInfo[]>((resolve, reject) => {

			var key = `${countryId}:${suggest.type}:${suggest.search}`;
			const inCache = this._cityInfoCache.get(key);
			if (inCache !== null) {
				resolve(inCache);
				return;
			}

			clearTimeout(this.throttleTimeoutId);

			this.throttleTimeoutId = window.setTimeout(() => {
				var request: IAddressRequest = {
					FrontendId: pxpGlobal.frontend?.id,
					LanguageId: pxpGlobal.systemLanguageId,
                    Data: {
						CountryId: countryId,
						Suggest: suggest.getDto()
                    }
				}

				this.post<ICityState[]>('/api/admin/address/cityInfo', request)
					.then((data) => {
						let result = data.map(c => new CityInfo(c));
						this._cityInfoCache.set(key, result);
						resolve(result);
					})
					.catch((data) => {
						reject(data);
					});
					
			}, this.throttleTimeoutMs);
		});
    }

	static suggestAddresses( 
		query: string,
		city: ICityState,
		type: AddressSuggestType,
		street?: string): Promise<AddressSuggest[]> {
		return new Promise((resolve, reject) => {
			var key = (street && street!='')
				? `${city.Title}:${street}:${query}`
				: `${city.Title}:${query}`;
			const cache = this._addressSuggestCache.get(key);
			if (cache != null) {
				resolve(cache);
				return;
			}

			clearTimeout(this.throttleTimeoutId);

			this.throttleTimeoutId = window.setTimeout(() => {
				var request: IAddressRequest = {
					FrontendId: pxpGlobal.frontend?.id,
					LanguageId: pxpGlobal.systemLanguageId,
                    Data: {
						Query: query,
						Street: street,
						City: city,
						Type: type
                    }
				}

				this.post<IAddressSuggestDTO[]>('/api/admin/address/suggestAddress', request)
					.then((data) => {
						let result = data.map(a => new AddressSuggest(a, type));
						this._addressSuggestCache.set(key, result);
						resolve(result);
					})
					.catch((data) => {
						reject(data);
					}
				);
					
			}, this.throttleTimeoutMs);
		});
	}

	static getAddressInfoBySuggest(suggest: AddressSuggest): Promise<AddressInfo | null> {
		return new Promise((resolve, reject) => {

			var key = `by_suggest:${suggest.search}`;
			const cache = this._addressInfoCache.get(key);
			if (cache != null) {
				resolve(cache);
				return;
			}

			clearTimeout(this.throttleTimeoutId);

			this.throttleTimeoutId = window.setTimeout(() => {
				var request: IAddressRequest = {
					FrontendId: pxpGlobal.frontend?.id,
					LanguageId: pxpGlobal.systemLanguageId,
					Data: {
						Suggest: <IAddressSuggestDTO>{
							Latitude: suggest.lat,
							Longitude: suggest.lon,
							City: suggest.city,
							Street: suggest.street,
							House: suggest.house,
							Description: suggest.description,
							GeoId: suggest.geoId
						}
					}
				}

				this.post<IAddressInfoDTO>('/api/admin/address/addressInfo', request)
					.then((data) => {
                        if (!data) {
							resolve(null);
							return
                        };
						const result = new AddressInfo(data);
						this._addressInfoCache.set(key, result);
						resolve(result);
					})
					.catch((data) => {
						reject(data);
					}
				);
					
			}, this.throttleTimeoutMs);
		});
	} 

	static getAddressInfoByCoords(latitude: string, longitude: string): Promise<AddressInfo | null> {
		return new Promise((resolve, reject) => {

			var key = `by_coords:${latitude},${longitude}`;
			const cache = this._addressInfoCache.get(key);
			if (cache != null) {
				resolve(cache);
				return;
			}

			clearTimeout(this.throttleTimeoutId);

			this.throttleTimeoutId = window.setTimeout(() => {
				var request: IAddressRequest = {
					FrontendId: pxpGlobal.frontend?.id,
					LanguageId: pxpGlobal.systemLanguageId,
					Data: {
						Coords: {Latitude: latitude, Longitude: longitude}
					}
				}

				this.post<IAddressInfoDTO>('/api/admin/address/addressInfo', request)
					.then((data) => {
						if (!data) {
							resolve(null);
							return
                        };
						const result = new AddressInfo(data);
						this._addressInfoCache.set(key, result);
						resolve(result);
					})
					.catch((data) => {
						reject(data);
					}
				);
					
			}, this.throttleTimeoutMs);
		});
	} 

}