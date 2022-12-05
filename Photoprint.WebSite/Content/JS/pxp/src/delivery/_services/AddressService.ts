import { pxpGlobal } from "../../globals/pxp";
import { CityInfo, ICityState, ToponimType } from "../models/city/CityInfo";
import { CitySuggest, ICitySuggestDTO } from "../models/city/CitySuggest";
import { ObjectCache } from "../../utils/ObjectCache";
import { IAddressSuggest } from "../models/address/suggests/SuggestedStreetItem";
import { IAddressData, IBoundedPointData, IPointData } from "../models/address/BaseAddressModel";
import { IPxpLatLng } from "../models/_maps/_baseMapManager";

export class AddressService {
	private static readonly _reverseCityCache: ObjectCache<CityInfo> = new ObjectCache<CityInfo>();
	private static readonly _citySuggestCache: ObjectCache<Array<CitySuggest>> = new ObjectCache<Array<CitySuggest>>();
	private static readonly _cityAddressCache: ObjectCache<Array<CityInfo>> = new ObjectCache<Array<CityInfo>>();
	private static readonly _streetsCache: ObjectCache<IAddressSuggest[]> = new ObjectCache<IAddressSuggest[]>()
	private static readonly throttleTimeoutMs: number = 250;
	private static throttleTimeoutId: number | undefined = undefined;

	static findCityReverse(countryId: number): Promise<CityInfo> {
		return new Promise<CityInfo>((resolve, reject) => {
			var isAvailable = ("geolocation" in navigator);
			if (!isAvailable) { reject(); return; }

			navigator.geolocation.getCurrentPosition(
				(location) => {
					const lat = location?.coords?.latitude?.toString();
					const lng = location?.coords?.longitude?.toString();
					if (lat === null || lng === null) { reject(); return; }

					const key = `${countryId}:${lat}_${lng}`;
					const inCache = this._reverseCityCache.get(key);
					if (inCache != null) {
						resolve(inCache);
						return;
					}
					this.getCityAddressesByCoords(countryId, lat, lng)
						.then(res => {
                            if (res.length == 0) {
								reject();
								return;
                            }
							resolve(res[0]);
						})
                        .catch(() => {
							reject();
							return;
                        })
				},
				() => { 
					reject(); 
					return; 
				}
			);
		}); 
    }

	static suggestCities(countryId: number, searchQuery: string): Promise<CitySuggest[] | null> {
		return new Promise<CitySuggest[] | null>((resolve, reject) => {
			const search = searchQuery.trim();
			if (search === "") {
				resolve(null);
				return;
			}

			var key = `${countryId}:${search}`;
			const inCache = this._citySuggestCache.get(key);
			if (inCache !== null) {
				resolve(inCache);
				return;
			}

			clearTimeout(this.throttleTimeoutId);

			this.throttleTimeoutId = window.setTimeout(() => {
				var request: IAddressRequest = {
					FrontendId: pxpGlobal.frontend.frontendId,
					LanguageId: pxpGlobal.frontend.languageId,
                    Data: {
						Country: countryId,
						Query: searchQuery,
						Type: ToponimType.City
                    }
				}

				const xhr = new XMLHttpRequest();
				xhr.open("POST", '/api/address/suggestCity');
				xhr.setRequestHeader("Content-Type", "application/json");
				xhr.onreadystatechange = () => {
					if (xhr.readyState !== XMLHttpRequest.DONE) return;
					if (xhr.status > 199 && xhr.status < 300) {
						const data = <ICitySuggestDTO[]>JSON.parse(xhr.responseText);
						resolve(data.map(c => new CitySuggest(c)));
					} else {
						reject(xhr.responseText);
					}
				};
				xhr.send(JSON.stringify(request));
			}, this.throttleTimeoutMs);
		});
	}

	static getCityAddressesBySuggest(countryId: number, suggest: CitySuggest): Promise<CityInfo[]> {
		return new Promise<CityInfo[]>((resolve, reject) => {

			var key = `${countryId}:${suggest.search}`;
			const inCache = this._cityAddressCache.get(key);
			if (inCache !== null) {
				resolve(inCache);
				return;
			}

			clearTimeout(this.throttleTimeoutId);

			this.throttleTimeoutId = window.setTimeout(() => {
				var request: IAddressRequest = {
					FrontendId: pxpGlobal.frontend.frontendId,
					LanguageId: pxpGlobal.frontend.languageId,
                    Data: {
						Suggest: suggest,
						Country: countryId
                    }
				}

				const xhr = new XMLHttpRequest();
				xhr.open("POST", '/api/address/cityInfo');
				xhr.setRequestHeader("Content-Type", "application/json");
				xhr.onreadystatechange = () => {
					if (xhr.readyState !== XMLHttpRequest.DONE) return;
					if (xhr.status > 199 && xhr.status < 300) {
						const data = (<ICityState[]>JSON.parse(xhr.responseText)).map(cs => new CityInfo(cs));
						this._cityAddressCache.set(key, data);
						resolve(data);
					} else {
						reject(xhr.responseText);
					}
				};
				xhr.send(JSON.stringify(request));
			}, this.throttleTimeoutMs);
		});
    }

	static getCityAddressesByQuery(countryId: number, query: string): Promise<CityInfo[]> {
        return new Promise<CityInfo[]>((resolve, reject) => {

            var key = `${countryId}:${query}`;
            const inCache = this._cityAddressCache.get(key);
            if (inCache !== null) {
                resolve(inCache);
                return;
            }

            clearTimeout(this.throttleTimeoutId);

            this.throttleTimeoutId = window.setTimeout(() => {
                var request: IAddressRequest = {
                    FrontendId: pxpGlobal.frontend.frontendId,
                    LanguageId: pxpGlobal.frontend.languageId,
                    Data: {
						Country: countryId,
						Query: query
                    }
                }

                const xhr = new XMLHttpRequest();
                xhr.open("POST", '/api/address/cityInfo');
                xhr.setRequestHeader("Content-Type", "application/json");
                xhr.onreadystatechange = () => {
                    if (xhr.readyState !== XMLHttpRequest.DONE) return;
                    if (xhr.status > 199 && xhr.status < 300) {
                        const data = (<ICityState[]>JSON.parse(xhr.responseText)).map(s => new CityInfo(s));
                        this._cityAddressCache.set(key, data);
                        resolve(data);
                    } else {
                        reject(xhr.responseText);
                    }
                };
                xhr.send(JSON.stringify(request));
            }, this.throttleTimeoutMs);
        });
    }

	static getCityAddressesByCoords(countryId: number, latitude: string, longitude: string): Promise<CityInfo[]> {
        return new Promise<CityInfo[]>((resolve, reject) => {

            var key = `${countryId}:${latitude},${longitude}`;
            const inCache = this._cityAddressCache.get(key);
            if (inCache !== null) {
                resolve(inCache);
                return;
            }

            clearTimeout(this.throttleTimeoutId);

            this.throttleTimeoutId = window.setTimeout(() => {
                var request: IAddressRequest = {
                    FrontendId: pxpGlobal.frontend.frontendId,
                    LanguageId: pxpGlobal.frontend.languageId,
                    Data: {
						Country: countryId,
						Coords: {Latitude: latitude, Longitude: longitude}
                    }
                }

                const xhr = new XMLHttpRequest();
                xhr.open("POST", '/api/address/cityInfo');
                xhr.setRequestHeader("Content-Type", "application/json");
                xhr.onreadystatechange = () => {
                    if (xhr.readyState !== XMLHttpRequest.DONE) return;
                    if (xhr.status > 199 && xhr.status < 300) {
                        const data = (<ICityState[]>JSON.parse(xhr.responseText)).map(s => new CityInfo(s));
                        this._cityAddressCache.set(key, data);
                        resolve(data);
                    } else {
                        reject(xhr.responseText);
                    }
                };
                xhr.send(JSON.stringify(request));
            }, this.throttleTimeoutMs);
        });
    }

	static findSuggestAddresses( 
		query: string,
		city: ICityState,
		type: SuggestType,
		street?: string | null): Promise<IAddressSuggest[]> {
		return new Promise((resolve, reject) => {
			const cache = this._streetsCache.get(`${city.Title}:${query}`);
			if (cache != null) {
				resolve(cache);
				return;
			}

			var request: IAddressRequest = {
				FrontendId: pxpGlobal.frontend.frontendId,
				LanguageId: pxpGlobal.frontend.languageId,
                Data: {
					Query: query,
					Street: street,
					City: city,
					Type: type
                }
			}
			const xhr = new XMLHttpRequest();
			xhr.open("POST", '/api/address/suggestAddress');
			xhr.setRequestHeader("Content-Type", "application/json");
			xhr.onreadystatechange = () => {
				if (xhr.readyState === XMLHttpRequest.DONE) return;
				if (xhr.status > 199 && xhr.status < 300) {
					if (xhr.responseText.length === 0) return;

					const response: IAddressSuggest[] = JSON.parse(xhr.responseText);
					this._streetsCache.set(`${city}:${query}`, response)
					resolve(response);
				}
				else {
					reject(xhr.responseText);
				}
			};
			xhr.send(JSON.stringify(request));
		});
	}

	static findStreetAddressDataBySuugest(suggest: IAddressSuggest): Promise<IBoundedPointData> {
		return new Promise((resolve, reject) => {
			var request: IAddressRequest = {
				FrontendId: pxpGlobal.frontend.frontendId,
				LanguageId: pxpGlobal.frontend.languageId,
                Data: {
					Suggest: suggest
                }
			}

			const xhr = new XMLHttpRequest();
			xhr.open("POST", '/api/address/addressInfo');
			xhr.setRequestHeader("Content-Type", "application/json");
			xhr.onreadystatechange = () => {
				if (xhr.readyState === XMLHttpRequest.DONE) return;
				if (xhr.status > 199 && xhr.status < 300) {
                    if (xhr.responseText.length === 0) {
						return;
                    }
					const address: IAddressData = JSON.parse(xhr.responseText);
                    if (address == null) {
						reject();
						return;
                    }
					resolve(this._getPointData(address));
				}
				else {
					reject(xhr.responseText);
				}
			};
			xhr.send(JSON.stringify(request));
		});
	}

	static findStreetAddressDataByCoords(coodrs: IPxpLatLng): Promise<IPointData> {
		return new Promise((resolve, reject) => {
			var request: IAddressRequest = {
				FrontendId: pxpGlobal.frontend.frontendId,
				LanguageId: pxpGlobal.frontend.languageId,
                Data: {
					Coords: {Latitude: coodrs.lat, Longitude: coodrs.lng}
                }
			}

			const xhr = new XMLHttpRequest();
			xhr.open("POST", '/api/address/addressInfo');
			xhr.setRequestHeader("Content-Type", "application/json");
			xhr.onreadystatechange = () => {
				if (xhr.readyState === XMLHttpRequest.DONE) return;
				if (xhr.status > 199 && xhr.status < 300) {
					if (xhr.responseText.length === 0){
						return;
                    }
					const address: IAddressData = JSON.parse(xhr.responseText);
					if (address==null){
						reject();
						return;
                    }
                    
					resolve(this._getPointData(address));
				}
				else {
					reject(xhr.responseText);
				}
			};
			xhr.send(JSON.stringify(request));
		});
	}

    private static _getPointData(dto: IAddressData): IBoundedPointData {
		return {
            city: {
				country: dto.Country,
				region: dto.Region,
				title: dto.City
            },
			street: dto.Street,
			house: dto.House,
			flat: '',
			postalCode: dto.PostalCode ?? '',
			lat: parseFloat(dto.Latitude?? ''),
			lon: parseFloat(dto.Longitude?? ''),
			description: dto.Description,
            bounds: dto.Bounds != null 
				? {
					upLat: parseFloat (dto.Bounds.UpperLatitude),
					upLon: parseFloat (dto.Bounds.UpperLongitude),
					lowLat: parseFloat (dto.Bounds.LowerLatitude),
					lowLon: parseFloat (dto.Bounds.LowerLongitude)				}
				: null
        }
    }
}

export interface IAddressRequest {
	FrontendId: number;
	LanguageId: number | null;
	Data: ISuggestCitiesData | IFindCitiesData | IFindSuggestedAddressData | IFindAddressInfoData;
}

export interface IFindCityReverseRequest {
	FrontendId: number;
	LanguageId: number;
	Lat: string;
	Lng: string;
}
export interface ISuggestCitiesData {
    Country: number;
    Query: string;
	Type: ToponimType;
}
export interface IFindCitiesData {
    Country: number;
    Suggest?: CitySuggest;
    Query?: string;
	Coords?: IPxpLatLng;
}
export interface IFindSuggestedAddressData {
    Query: string;
	Street?: string | null;
	City: ICityState;
	Type: SuggestType,
}

export enum SuggestType {
    Street,
    House
}

export interface IFindAddressInfoData {
	Suggest?: IAddressSuggest;
	Coords?: LatLong;
	Query?: string;
}

interface LatLong {
	Latitude: string | null,
    Longitude: string | null
}