import { ExternalMapObjectType } from "../../../../_maps/_baseMapManager";
import { ObjectCache } from "../../../../../../utils/ObjectCache";
import { IDeliverySettingsMap, PxpMapType } from "../../../../WindowSettings";
import { CityInfo } from "../../../../city/CityInfo";
import { CountryInfo, CityAddressCountry } from "../../../../city/CountrySelector";

export abstract class BaseDeliveryMap {
    abstract afterRenderMap(): void;

    isYandexMap: boolean = true;
    isGoogleMap: boolean = false;
    cache: ObjectCache<ExternalMapObjectType>;
    authData: IDeliveryMapAuthData;
    
    isMapRendered: KnockoutObservable<boolean>;
    currentCity: KnockoutObservable<CityInfo | null>;
    currentCountry: KnockoutObservable<CountryInfo>;

    constructor(
        mapSettings: IDeliverySettingsMap,
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>
    ) {
        this.isYandexMap = mapSettings.MapType === PxpMapType.Yandex;
        this.isGoogleMap = mapSettings.MapType === PxpMapType.Google;
        this.cache = new ObjectCache<ExternalMapObjectType>(); // используется в BaseMapManager
        this.authData = mapSettings.Data;

        this.isMapRendered = ko.observable(false);
        this.currentCity = currentCity;
        this.currentCountry = currentCountry;

        // subscribe
        this.isMapRendered.subscribe((val) => {
            if (val) this.afterRenderMap?.();
        }, this);

        // calls
        Promise.all([this._loadMapData()]);
	}

    private _loadMapData(): Promise<void> {
        return new Promise(() => {
            if (this.isYandexMap) {
                return import("../../../../_maps/YmapBindings").then((YmapBindings) => {
                    return YmapBindings.default();
                })
            }

            if (this.isGoogleMap) {
                return import("../../../../_maps/GoogleMapBinding").then((GoogleMapBinding) => {
                    return GoogleMapBinding.default();
                })
            }
        })
    }
}

export interface IDeliveryMapAuthData {
    CountryLimiter: CityAddressCountry;
    ApiKey: string;
}
