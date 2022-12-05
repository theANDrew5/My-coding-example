import { pxpGlobal } from '../../globals/pxp';
import { IDeliveryFinalState, IDeliveryTotalPriceControllerInitData } from '../controllers/DeliveryTotalPriceController';
import { IBasePluginShippingData } from '../models/types/pluginDeliveryTypes/_base/BasePluginDeliveryPointItem';
import { IAvailableDeliveryTypesDto } from '../models/types/_base/BaseDeliveryTypeModel';
import { DeliveryDisplayType } from '../controllers/DeliveryTypesSelector';
import { IDeliveryMessageResponce } from '../controllers/MessagesManager';
import { IDeliverySettings } from '../models/WindowSettings';
import { IDeliveryLocalization } from '../DeliveryManager';
import { ObjectCache } from '../../utils/ObjectCache';
import { IPxpBaseDeliveryPointDto, IPxpDeliveryAddress } from '../models/types/_base/BaseDeliveryPointItem';
import { CityInfo, ICityState } from '../models/city/CityInfo';

export class DeliveryManagerService {
    private static _cache: ObjectCache<Array<IDeliveryManagerServiceResponse>> = new ObjectCache<Array<IDeliveryManagerServiceResponse>>();
    private static readonly _shippingFinalStateStorageKey = "pxp_delivery_shippingAddressFinalState";

    static getInitData(): Promise<IDeliveryManagerServiceInitDataResponse> {
        return new Promise((resolve, reject) => {
            var request: IDeliveryBaseRequest = {
                FrontendId: pxpGlobal.frontend.frontendId,
                LanguageId: pxpGlobal.frontend.languageId
            }

            const url = '/api/delivery/initData';
            const xhr = new XMLHttpRequest();
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const response: IDeliveryManagerServiceInitDataResponse = JSON.parse(xhr.responseText);
                    resolve(response);
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(request));
        });
    }

        static getAvailableDeliveryTypes(instanceGuid: string, city: ICityState): Promise<IAvailableDeliveryTypesDto[]> {
        return new Promise((resolve, reject) => {
            const cache: IAvailableDeliveryTypesDto[] | null = this._cache.get(city.Title) as IAvailableDeliveryTypesDto[];
            if (cache != null) {
                resolve(cache);
                return;
            }
            var request: IGetAvailableDeliveryTypesRequest = {
                FrontendId: pxpGlobal.frontend.frontendId,
                LanguageId: pxpGlobal.frontend.languageId,
                InstanceGuid: instanceGuid,
                City: city,
            };
            const url = '/api/delivery/shippings';
            const xhr = new XMLHttpRequest();
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const response: IAvailableDeliveryTypesDto[] = JSON.parse(xhr.responseText);
                    this._cache.set(city.Title, response);
                    resolve(response);
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(request));
        });
    }

    static getDeliveryAddressesForSingleDelivery<T extends IPxpBaseDeliveryPointDto>(instaceGuid: string, city: CityInfo, type: DeliveryDisplayType, shippingId: number): Promise<T[]> {
        return new Promise((resolve, reject) => {
            const cache: T[] | null = this._cache.get(`${city.title}:${type}`) as T[];
            if (cache != null) {
                resolve(cache);
                return;
            }
            var request: IGetShippingAddressesRequest = {
                FrontendId: pxpGlobal.frontend.frontendId,
                LanguageId: pxpGlobal.frontend.languageId,
                InstanceGuid: instaceGuid,
                City: city.originalData,
                Type: type,
                ShippingIds: [shippingId] 
            }

            const xhr = new XMLHttpRequest();
            xhr.open("POST", '/api/delivery/addresses');
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const response: T[] = JSON.parse(xhr.responseText);
                    this._cache.set(`${city}:${type}`, response);
                    resolve(response);
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(request));
        });
    }


    static getDeliveryAddresses<T extends IPxpBaseDeliveryPointDto>(instaceGuid: string, city: CityInfo, type: DeliveryDisplayType, shippingIds: number[]): Promise<T[]> {
        return new Promise((resolve, reject) => {
            var promises = shippingIds.map(sId => this.getDeliveryAddressesForSingleDelivery<T>(instaceGuid, city, type, sId));
            Promise.all(promises)
                .then(data => {
                    var addresses = data.reduce((a, b)=> a.concat(b));
                    resolve(addresses);
                })
                .catch(error => reject(error));

        });
    }

    static getPluginShippingData<T extends IBasePluginShippingData>(type: DeliveryDisplayType): Promise<T> {
        return new Promise((resolve, reject) => {
            var request: IGetPluginShippingDataRequest = {
                FrontendId: pxpGlobal.frontend.frontendId,
                Type: type
            };

            const xhr = new XMLHttpRequest();
            xhr.open("POST", '/api/delivery/plugins/initData');
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const response: T = JSON.parse(xhr.responseText);
                    resolve(response);
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(request));
        });
    }

    static createOrder(finalState: IDeliveryFinalState): Promise<IDeliveryCreateOrderResponse> {
        return new Promise((resolve, reject) => {
            var request: IDeliveryCreateOrderRequest = {
                FrontendId: pxpGlobal.frontend.frontendId,
                LanguageId: pxpGlobal.frontend.languageId,
                DeliveryModel: finalState
            }

            const url = `/api/delivery/orderCreate`;
            const xhr = new XMLHttpRequest();
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const response: IDeliveryCreateOrderResponse = JSON.parse(xhr.responseText);
                    resolve(response);
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(request));
        });
    }

    static storeShippingFinalState(state: IPxpDeliveryAddress) {
        localStorage.setItem(this._shippingFinalStateStorageKey, JSON.stringify(state));
    }

    static getStoredShippingFinalState(): IPxpDeliveryAddress | undefined {
        var jString = localStorage.getItem(this._shippingFinalStateStorageKey);
        if (jString == undefined || jString == '')
            return undefined;
        return <IPxpDeliveryAddress>JSON.parse(jString);
    }
}

interface IDeliveryBaseRequest {
    FrontendId: number;
    LanguageId: number | null;
}

interface IGetAvailableDeliveryTypesRequest extends IDeliveryBaseRequest{
    InstanceGuid: string;
    City: ICityState;
}
interface IGetShippingAddressesRequest extends IDeliveryBaseRequest {
    InstanceGuid: string;
    City: ICityState;
    Type: DeliveryDisplayType;
    ShippingIds: number [];
}
interface IGetPluginShippingDataRequest {
    FrontendId: number;
    Type: DeliveryDisplayType;
}
interface IDeliveryCreateOrderRequest extends IDeliveryBaseRequest {
    DeliveryModel: IDeliveryFinalState;
}
export interface IDeliveryManagerServiceInitDataResponse {
    RedirectUrl?: string;
    Localization: IDeliveryLocalization;
    Settings: IDeliverySettings;
    TotalPriceInitData: IDeliveryTotalPriceControllerInitData;
    InstanceGuid: string;
}
export interface IDeliveryCreateOrderResponse {
    IsSuccessful: boolean;
    RedirectUrl: string;
    Message: IDeliveryMessageResponce;
}

type IDeliveryManagerServiceResponse = IPxpBaseDeliveryPointDto | IAvailableDeliveryTypesDto;