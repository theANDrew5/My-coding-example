import React, { useEffect } from "react";
import { Address, ErrorMessage, ShippingData } from "../SelectShippingModal";
import { IYandexDeliveryAddressInfo, YandexDeliveryAddressInfo, YandexDeliveryAddressInfoFactory, YandexDeliveryProperties } from "./Deliveries/YandexDeliveryProperties";

export interface IDeliveryAddressProperties {
    yandexDeliveryAddressInfo?: IYandexDeliveryAddressInfo;
    cdekAddressInfo?: any;
}

export class DeliveryAddressProperties {
    discriminator = 'object';
    yandexDeliveryAddressProperties?: YandexDeliveryAddressInfo;
    cdekAddressInfo?: any;
}

export class DeliveryPropertiesFactory {
    static getObj(input?: IDeliveryAddressProperties | DeliveryAddressProperties): DeliveryAddressProperties | undefined {
        if (input == undefined) return undefined;
        let result = new DeliveryAddressProperties();
        if ((input as any).discriminator === 'object') {
            let obj = input as DeliveryAddressProperties;
            result.yandexDeliveryAddressProperties = 
                YandexDeliveryAddressInfoFactory.getObj(obj.yandexDeliveryAddressProperties);
        } else {
            let dto = input as IDeliveryAddressProperties;
            result.yandexDeliveryAddressProperties = 
                YandexDeliveryAddressInfoFactory.getObj(dto.yandexDeliveryAddressInfo);
        }
        result.cdekAddressInfo = input.cdekAddressInfo;
        return result;
    }
    static getDto(obj?: DeliveryAddressProperties): IDeliveryAddressProperties | undefined {
        if (obj == undefined) return undefined;
        return {
            yandexDeliveryAddressInfo: YandexDeliveryAddressInfoFactory.getDto(obj.yandexDeliveryAddressProperties),
            cdekAddressInfo: obj.cdekAddressInfo
        };
    }
}

interface IDeliveryPropertiesProps {
    shippingData: ShippingData,
    selectedAddress: Address,
    onChange: (properties: DeliveryAddressProperties) => void,
    updShippingData: (data: ShippingData) => void,
    setPropertiesReady: (ready: boolean) => void,
    isLoading: (value: React.SetStateAction<boolean>) => void,
    setErrorMessage: (value: React.SetStateAction<ErrorMessage | null>) => void,
}

export const DeliveryProperties = (props: IDeliveryPropertiesProps) => {
    const {shippingData, selectedAddress, onChange, updShippingData, setPropertiesReady,
        isLoading, setErrorMessage} = props
    const { shippingProviderType } = props.shippingData;

    const yandexPropertiesChange = (addresInfo: YandexDeliveryAddressInfo) => {
        let newProps = DeliveryPropertiesFactory.getObj(selectedAddress.deliveryProperties) ?? new DeliveryAddressProperties();
        newProps.yandexDeliveryAddressProperties = addresInfo;
        onChange(newProps);
    }

    useEffect(() => {
        switch (shippingProviderType) {
            case 'YandexDelivery':
                return;
            default:
                setPropertiesReady(true);
        }
    }, []);

    switch (shippingProviderType) {
        case 'YandexDelivery':
            return(
                <>
                    <YandexDeliveryProperties
                        shippingData={shippingData}
                        selectedAddress={selectedAddress}
                        onChange={yandexPropertiesChange}
                        updShippingData={updShippingData}
                        setPropertiesReady={setPropertiesReady}
                        isLoading={isLoading}
                        setErrorMessage={setErrorMessage}/>       
                </>);
            
        default:
            return(
                <>
                        
                </>
            );  
    }
}