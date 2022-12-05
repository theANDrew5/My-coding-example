import { IAddressHierarchyDTO, IAddressRequestDTO } from "./ShippingService";
import { Address } from "../ui/components/Modals/SelectShippingModal/SelectShippingModal";
import { YandexDeliveryShipping, YandexDeliveryType } from "../ui/components/Modals/SelectShippingModal/DeliveryProperties/Deliveries/YandexDeliveryProperties";
import { DeliveryPropertiesFactory } from "../ui/components/Modals/SelectShippingModal/DeliveryProperties/DeliveryProperties";

//Requests
interface IGeoIdRequest {
    ShippingId: number,
    CityAddress: IAddressRequestDTO
}

interface IShippingsRequest {
    ShippingId: number,
    SelectedAddressDto: IAddressRequestDTO
}

interface IPickupPointsRequest {
    ShippingId: number,
    DeliveryShipping: IYandexDeliveryShipping
}

//Responces
export interface IYandexDeliveryShipping {
    TariffId: number,
    TariffName: number,
    PartnerId: number,
    PartnerName: string,
    PickupPointIds: Array<number>,
    Cost: string
    Type: YandexDeliveryType
}

export class YandexDeliveryService {
    
    private static _formatParams(params: any) {
        return "?" + Object
            .keys(params)
            .map(function (key) {
                return key + "=" + encodeURIComponent(params[key])
            })
            .join("&")
    }

    static async getAddressesByTerm(shippingId: number, address: Address): Promise<IAddressHierarchyDTO> {
        const request = <IGeoIdRequest>{
            ShippingId: shippingId,
            CityAddress: {
                Country: address.country,
                Region: address.region,
                City: address.city
            }
        }
        const url = '/api/shippings/yandexDelivery/getAddressesByTerm';
        return new Promise<IAddressHierarchyDTO>((resolve, reject) => {
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
            xhr.send(JSON.stringify(request));
        });

    }

    static async getAvailableShippings(shippingId: number, selectedAddress: Address): Promise<Array<YandexDeliveryShipping>> {
        const request = <IShippingsRequest>{
            ShippingId: shippingId,
            SelectedAddressDto: {
                Country: selectedAddress.country,
                Region: selectedAddress.region,
                City: selectedAddress.city,
                Street: selectedAddress.street,
                House: selectedAddress.house,
                PostalCode: selectedAddress.postalCode,
                DeliveryProperties: DeliveryPropertiesFactory.getDto(selectedAddress.deliveryProperties)
            }
        }
        const url = '/api/shippings/yandexDelivery/getDeliveryShippings';
        return new Promise<Array<YandexDeliveryShipping>>((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const result = JSON.parse(xhr.responseText) as Array<IYandexDeliveryShipping>;
                    resolve(result.map(dto =>  new YandexDeliveryShipping(dto)));
                } else {
                    reject(xhr.responseText);
                }
            };
            xhr.send(JSON.stringify(request));
        });
    }

    static async getPickupPoints(shippingId: number, 
        shipping: YandexDeliveryShipping):Promise<IAddressHierarchyDTO> {
        const request: IPickupPointsRequest = {
            ShippingId: shippingId,
            DeliveryShipping: shipping.getDto()
        }
        const url = '/api/shippings/yandexDelivery/pickpoints';
        return new Promise<IAddressHierarchyDTO>((resolve, reject) => {
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
            xhr.send(JSON.stringify(request));
        });
    }

}