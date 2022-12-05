import { pxpGlobal } from "../../globals/pxp";
import { IDeliveryAddressProperties, IPxpDeliveryAddress } from "../models/types/_base/BaseDeliveryPointItem";

export class DelvieryPriceService {
    static getPrice(addresses: Array<IPxpDeliveryAddress>): Promise<Array<IDeliveryGetPriceResult>> {
        var request = <IDeliveryGetPriceRequest>{
            FrontendId: pxpGlobal.frontend.frontendId,
            Addresses: addresses
        };
        return new Promise((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open("POST", `/api/delivery/price`);
            xhr.setRequestHeader("Content-Type", "application/json;charset=utf-8");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const response = <Array<IDeliveryGetPriceResult>>JSON.parse(xhr.responseText);;
                    resolve(response);
                }
                else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(request));
        });
    }   
}

interface IDeliveryGetPriceRequest {
    FrontendId: number;
    Addresses: Array<IPxpDeliveryAddress>
}

export interface IDeliveryGetPriceResult {
    ShippingId: number;
    ShippingAddressId: number;
    CalculationResult: IDeliveryPriceCalculationResult;
}

export interface IDeliveryPriceCalculationResult {
    Success: boolean;
    Price: number;
    Cost: number;
    Discount: number;
    MaxPeriod: number;
    MinPeriod: number;
    Properties: IDeliveryAddressProperties;
}
