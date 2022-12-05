import { DeliveryWindowSettings } from "../delivery/windowSettings/models/WindowSettingsModel";
import { pxpGlobal } from "../globals/pxp";
import { IDeliveryAddressProperties } from "../ui/components/Modals/SelectShippingModal/DeliveryProperties/DeliveryProperties";
import AdminBaseService from "./AdminBaseService";

function formatParams(params: any) {
    return "?" + Object
        .keys(params)
        .map(function (key) {
            return key + "=" + encodeURIComponent(params[key])
        })
        .join("&")
}
export interface IShippingDTO {
    Id: number;
    Title: string;
    Type: number;
    TypeString: string;
}
export interface IGetShippingDataResponse {
    Addresses: IAddressHierarchyDTO | null;
    ShippingId: number;
    IsClientDelivery: boolean;
    IsPostCodeRequired: boolean;
    IsSeparateAddress: boolean;
    ShippingProviderType: string | null;

}
export interface IAddressHierarchyDTO {
    Countries: ICountryInfoDTO[];
}

export interface ICountryInfoDTO {
    Country: string;
    Regions: IRegionsInfoDTO[];
}

export interface IRegionsInfoDTO {
    Region: string;
    Cities: ICityInfoDTO[];
}

export interface ICityInfoDTO {
    City: string;
    Addresses: IAddressInfoDTO[];
}

export interface IAddressInfoDTO {
    Id: number | null;
    AddressLine: string;
    AddressName: string;
    Street: string;
    House: string;
    Latitude: string;
    Longitude: string;
    PostalCode?: string;
    DeliveryProperties: IDeliveryAddressProperties
}
export interface IAddressRequestDTO {
    Id: number;
    ShippingId: number;
    Country: string;
    Region:string;
    City:string;
    Street:string;
    House:string;
    Flat: string;
    AddressLine:string;
    Latitude:string,
    Longitude:string;
    PostalCode:string;
    DeliveryProperties?: IDeliveryAddressProperties;
}
export interface IShippingPriceDTO {
    success: string
    price: number;
    properties: IDeliveryAddressProperties;

}
export interface ICdekCourierCallRequest {
    orderIds: number[];
    year: number;
    month: number;
    day: number;
    fromHour: number;
    fromMinute: number;
    toHour: number;
    toMinute: number;
    dPointId: number;
    userId: number;
    commentary: string;
}
interface IParseJSONResult {
    success: boolean;
    value: any
}
interface IUpdateShippingSettingsDTO {
    Settings: DeliveryWindowSettings;
    PhotolabId: number;
}
const tryParseJSON = (text: string): IParseJSONResult => {
    try {
        JSON.parse(text);
        return { success: true, value: JSON.parse(text) };
    }
    catch (error) {
        return { success: false, value: text };
    }
}
export class ShippingService extends AdminBaseService {
    static async callCurrier({ ...requestData }: ICdekCourierCallRequest): Promise<string> {
        var url = "/api/shipping/cdek/validateCallCourierDataOrders";
        var data = { ...requestData } as ICdekCourierCallRequest
        return new Promise<string>((resolve, reject) => {

            const xhr = new XMLHttpRequest();
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const result = JSON.parse(xhr.responseText);
                    resolve(result);
                } else {
                    reject(tryParseJSON(xhr.responseText).value);
                }
            };
            xhr.send(JSON.stringify(data));
        });
    }
    static async sendToPostal(orderIds: number[], type: number): Promise<string> {
        var url = " /api/shipping/sendOrderToPostal";
        var data = { OrderIds: orderIds, Type: type }
        return new Promise<string>((resolve, reject) => {

            const xhr = new XMLHttpRequest();
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const result = JSON.parse(xhr.responseText);
                    resolve(result);
                } else {
                    reject(tryParseJSON(xhr.responseText).value);
                }
            };
            xhr.send(JSON.stringify(data));
        });
    }
    static async getShippingList(frontendId: number | null | undefined,totalPrice: number | null): Promise<IShippingDTO[]> {
        const data = {
            photolabId: frontendId,
            totalPrice: totalPrice
        };
        var url = '/api/shipping/list' + formatParams(data);
        return this.get(url);
    };
    static async getDPoints() {
        var url = "/api/shipping/dPoints?photolabId=" + pxpGlobal.frontend?.id;
        return new Promise<[]>((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open("GET", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const result = JSON.parse(xhr.responseText);
                    resolve(result);
                } else {
                    reject(tryParseJSON(xhr.responseText).value);
                }
            };
            xhr.send();
        });
    }
    static async getShippingPrice( weight: string, languageId: number, address: IAddressRequestDTO): Promise<IShippingPriceDTO> {
        const data = {
            weight,
            ShippingAddress: address
        };
        var url = '/api/shipping/price';
        return new Promise<IShippingPriceDTO>((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const result = JSON.parse(xhr.responseText);
                    resolve(result);
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(data));
        });
    }
    static async getShippingData(shippingId: number): Promise<IGetShippingDataResponse> {
        const data = {
            shippingId
        };
        var url = '/api/shipping/shippingData' + formatParams(data);
        return new Promise<IGetShippingDataResponse>((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open("GET", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const result = JSON.parse(xhr.responseText);
                    resolve(result);
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send();
        });
    }
    
    static async updateShippingSettings(photolabId: number, settings: DeliveryWindowSettings): Promise<string> {
        var request = {
            PhotolabId: photolabId,
            Settings: settings
        } as IUpdateShippingSettingsDTO
        var url = '/api/shipping/settings/update';
        return this.post<string>(url,request);
    }
    
 }

