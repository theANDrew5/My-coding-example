import { Col, Row, Select } from "antd";
import Title from "antd/lib/typography/Title";
import React, { useEffect, useRef, useState } from "react";
import { rs } from "../../../../../../globals/pxp";
import { IAddressHierarchyDTO } from "../../../../../../services/ShippingService";
import { IYandexDeliveryShipping, YandexDeliveryService } from "../../../../../../services/YndexDeliveryService";
import { SelectFilter } from "../../../../../utils/SelectFilter";
import { Address, ErrorMessage, ShippingData } from "../../SelectShippingModal";

const { Option } = Select;

export enum YandexDeliveryType {
    COURIER = 0,
    PICKUP = 1,
    POST = 2,
    DEFAULT = 255
}

export interface IYandexDeliveryAddressInfo {
    GeoId?: string,
    PickPointId?: number,
    TariffId?: number,
    PartnerId?: number,
    DeliveryType?: YandexDeliveryType
}

export class YandexDeliveryAddressInfo {
    private _discriminator = 'object';
    geoId?: string;
    pickPointId?: number;
    tariffId?: number;
    partnerId?: number;
    deliveryType?: YandexDeliveryType;
}

export class YandexDeliveryAddressInfoFactory {

    static getObj(input?: IYandexDeliveryAddressInfo | YandexDeliveryAddressInfo): YandexDeliveryAddressInfo | undefined {
        if (input == undefined)
            return undefined;
        let result = new YandexDeliveryAddressInfo();
        if ((input as any)._discriminator === 'object') {
            let obj = input as YandexDeliveryAddressInfo;
            result.geoId = obj.geoId,
            result.pickPointId = obj.pickPointId;
            result.tariffId = obj.tariffId;
            result.partnerId = obj.partnerId;
            result.deliveryType = obj.deliveryType;
        } else {
            let dto = input as IYandexDeliveryAddressInfo;
            result.geoId = dto.GeoId;
            result.pickPointId = dto.PickPointId;
            result.tariffId = dto.TariffId;
            result.partnerId = dto.PartnerId;
            result.deliveryType = dto.DeliveryType;
        }
        return result;
    }

    static getDto(obj?: YandexDeliveryAddressInfo): IYandexDeliveryAddressInfo | undefined {
        if (obj == undefined)
            return undefined;
        return {
            GeoId: obj.geoId,
            PickPointId: obj.pickPointId,
            TariffId: obj.tariffId,
            PartnerId: obj.partnerId,
            DeliveryType: obj.deliveryType
        };
    }
}

export class YandexDeliveryShipping {
    tariffId: number
    tariffName: number
    partnerId: number
    partnerName: string
    pickupPointIds: Array<number>
    cost: string
    type: YandexDeliveryType
    
    constructor(dto: IYandexDeliveryShipping) {
        this.tariffId = dto.TariffId;
        this.tariffName = dto.TariffName;
        this.partnerId = dto.PartnerId;
        this.partnerName = dto.PartnerName;
        this.pickupPointIds = dto.PickupPointIds;
        this.cost = dto.Cost;
        this.type = dto.Type;
    }

    getDto(): IYandexDeliveryShipping {
        return {
            TariffId: this.tariffId,
            TariffName: this.tariffName,
            PartnerId: this.partnerId,
            PartnerName: this.partnerName,
            PickupPointIds: this.pickupPointIds,
            Cost: this.cost,
            Type: this.type
        } 
    }

}

export interface IYandexDeliveryOptions {
    shippings?: Array<YandexDeliveryShipping>,
    selectedShipping?: YandexDeliveryShipping,
}

interface IYandexDeliveryPropertiesProps {
    shippingData: ShippingData,
    selectedAddress: Address,
    onChange: (properties: YandexDeliveryAddressInfo) => void,
    updShippingData: (data: ShippingData) => void,
    setPropertiesReady: (ready: boolean) => void,
    isLoading: (value: React.SetStateAction<boolean>) => void;
    setErrorMessage: (value: React.SetStateAction<ErrorMessage | null>) => void;
}

export const YandexDeliveryProperties = (props: IYandexDeliveryPropertiesProps) => {
    const { isClientDelivery, shippingId, addresses } = props.shippingData;
    const { selectedAddress, onChange, updShippingData, setPropertiesReady,
        isLoading, setErrorMessage, shippingData } = props;

    const defaultAddresses: IAddressHierarchyDTO = 
    { Countries: [{ Country: "Россия", Regions: 
        [{ Region: '', Cities: 
            [{ City: '', Addresses: 
                [{
                    Id: 0, Street: '', House: '', AddressLine: '',AddressName:'', Latitude: '', Longitude: '',
                    DeliveryProperties: {}}]
            }]
        }]
    }]};
    const yandexDeliveryAddressProperties =
        selectedAddress.deliveryProperties?.yandexDeliveryAddressProperties ??
        {deliveryType: YandexDeliveryType.DEFAULT} as YandexDeliveryAddressInfo;
    const { geoId, deliveryType } = yandexDeliveryAddressProperties;
    const [options, setOptions] = useState<IYandexDeliveryOptions>({});
    const {shippings, selectedShipping} = options;
    const updateRef = useRef({timeout: 0, cityFound: false})

    const addressChangeResolver = () => {
        if ((selectedAddress.city == null || selectedAddress.city == '') && updateRef.current.cityFound) {
            setNewShippings();
            updateRef.current.cityFound = false;
            updAddresses(defaultAddresses);
            return;
        }
        if (geoId == null) {
            if (updateRef.current.timeout)
                clearTimeout(updateRef.current.timeout)
            updateRef.current.timeout =
                window.setTimeout(() => { getCityAddresses(selectedAddress); }, 3000);
            return;
        } else {
            if (!isClientDelivery) {
                if (updateRef.current.timeout)
                    clearTimeout(updateRef.current.timeout)
                updateRef.current.timeout =
                    window.setTimeout(() => { pickupAddressChangeProc(selectedAddress); }, 5);
                return;
            }
            else {
                switch (deliveryType) {
                    case YandexDeliveryType.POST:
                        if (updateRef.current.timeout)
                            clearTimeout(updateRef.current.timeout)
                        updateRef.current.timeout = 
                            window.setTimeout(() => { postalAddressChangeProc(selectedAddress); }, 3000);
                        return;
                    case YandexDeliveryType.COURIER:
                        if (updateRef.current.timeout)
                            clearTimeout(updateRef.current.timeout)
                        updateRef.current.timeout = 
                            window.setTimeout(() => { courierAddressChangeProc(selectedAddress); }, 3000);
                        return;
                    default:
                        return;
                }   
            
            }
        }
        
    }
    useEffect(addressChangeResolver, [selectedAddress]);
    

    const isPropertiesReady = () => {
        if (yandexDeliveryAddressProperties == null) {
            setPropertiesReady(false);
            return;
        }  
        setPropertiesReady(
            ((deliveryType==YandexDeliveryType.COURIER) ||
            (yandexDeliveryAddressProperties.pickPointId!=null && yandexDeliveryAddressProperties.pickPointId!=0)) &&
            (yandexDeliveryAddressProperties.tariffId!=null && yandexDeliveryAddressProperties.tariffId!=0) &&
            (yandexDeliveryAddressProperties.partnerId!=null && yandexDeliveryAddressProperties.partnerId!=0) &&
            (yandexDeliveryAddressProperties.deliveryType==deliveryType))
    }
    useEffect(isPropertiesReady, [yandexDeliveryAddressProperties]);

    const setDefaultAddressesOnLoad = () => {
        updAddresses(defaultAddresses);
    }
    useEffect(setDefaultAddressesOnLoad, []);
    //handlers
    const deliveryTypeHandling = (type: YandexDeliveryType) => {
        if (type == YandexDeliveryType.POST) {
            let newData: ShippingData = JSON.parse(JSON.stringify(shippingData));
            newData.isPostCodeRequired = true;
            updShippingData(newData);
        } else {
            let newData: ShippingData = JSON.parse(JSON.stringify(shippingData));
            newData.isPostCodeRequired = false;
            updShippingData(newData);
        }
        let newOptions = JSON.parse(JSON.stringify(options));
        newOptions.deliveryType = type;
        setOptions(newOptions);
        let newProps = YandexDeliveryAddressInfoFactory.getObj(yandexDeliveryAddressProperties)!;
        newProps.deliveryType = type;
        onChange(newProps);
    }

    const pickpointDeliveryShippingHandling = (id: number) => {
        let selectedShipping = shippings!.find(s => s.tariffId==id); 
        if (selectedShipping == undefined) return;
        let newOptions = JSON.parse(JSON.stringify(options));
        newOptions.selectedShipping = selectedShipping;
        setOptions(newOptions);
        isLoading(true);
        YandexDeliveryService.getPickupPoints(shippingId, selectedShipping)
            .then((points) => {
                points.Countries[0].Regions[0].Cities[0].Addresses = 
                points.Countries[0].Regions[0].Cities[0].
                    Addresses.filter(address => {
                        let props = address.DeliveryProperties.yandexDeliveryAddressInfo;
                        return props!=undefined 
                            && (props?.DeliveryType==YandexDeliveryType.PICKUP 
                            || (address.PostalCode!=null && address.PostalCode!=''))
                    });
                //костыль чтобы не перезагружать страницу
                points.Countries[0].Regions[0].Cities.push({ 
                    City: rs.Shipping.Address.ChooseAnotherCity, Addresses: [{
                        Id: null, AddressLine: '',AddressName:'', Street: '', House: '', Latitude: '', Longitude: '', DeliveryProperties: {}}]                     
                    });
                updAddresses(points);
                setErrorMessage({show:false});
            })
            .catch(() => {
                setErrorMessage({
                    show: true,
                    text: rs.Shipping.YandexDelivery.OrderCreate.ShippingsFindError
                });
            })
            .finally(() => {
                isLoading(false)
            })
    }

    const postalDeliveryShippingHandling = (id: number) => {
        let selectedShipping = shippings!.find(s => s.tariffId==id); 
        if (selectedShipping == undefined) return;
        let newOptions = JSON.parse(JSON.stringify(options));
        newOptions.selectedShipping = selectedShipping;
        setOptions(newOptions);
        let newProps = YandexDeliveryAddressInfoFactory.getObj(yandexDeliveryAddressProperties)!;
        newProps.partnerId = selectedShipping.partnerId;
        newProps.tariffId = selectedShipping.tariffId;
        newProps.pickPointId = selectedShipping.pickupPointIds[0];
        onChange(newProps);
    }

    const courierDeliveryShippingHandling = (id: number) => {
        let selectedShipping = shippings!.find(s => s.tariffId==id); 
        if (selectedShipping == undefined) return;
        let newOptions = JSON.parse(JSON.stringify(options));
        newOptions.selectedShipping = selectedShipping;
        setOptions(newOptions);
        let newProps = YandexDeliveryAddressInfoFactory.getObj(yandexDeliveryAddressProperties)!;
        newProps.partnerId = selectedShipping.partnerId;
        newProps.tariffId = selectedShipping.tariffId;
        onChange(newProps);
    }

    //
    const setNewShippings = (shippings?: Array<YandexDeliveryShipping>) => {
        let newOptions: IYandexDeliveryOptions = JSON.parse(JSON.stringify(options));
        newOptions.shippings=shippings;
        if(shippings==undefined)
            newOptions.selectedShipping=undefined;
        setOptions(newOptions);
    }

    const updAddresses = (newAddresses: IAddressHierarchyDTO) => {
        let newShippingData: ShippingData = JSON.parse(JSON.stringify(shippingData));
        newShippingData.addresses = newAddresses;
        updShippingData(newShippingData);
    }
    const getCityAddresses = (address: Address) => {
        if (address.city != null && address.city != '') {
            isLoading(true);
            YandexDeliveryService.getAddressesByTerm(shippingId, address)
            .then((addresses) => {
                addresses.Countries[0].Regions[0].Cities.push({ 
                    City: rs.Shipping.Address.ChooseAnotherCity, Addresses: [{
                        Id:null, AddressLine: '',AddressName: '', Street: '', House: '', Latitude: '', Longitude: '', DeliveryProperties: {}}]                     
                    });
                updAddresses(addresses);
                setErrorMessage({show:false});
                updateRef.current.cityFound = true;
            }).catch(() => {
                setErrorMessage({
                    show: true,
                    text: rs.Shipping.YandexDelivery.OrderCreate.CityFindError
                });
                setNewShippings();
            }).finally(() => {
                isLoading(false)});
        }
    }

    const pickupAddressChangeProc = (address: Address) => {
        if (shippings !== undefined)
            return;
        isLoading(true);
        YandexDeliveryService.getAvailableShippings(
            shippingId, address)
            .then((shippings) => {
                setNewShippings(shippings);
                setErrorMessage({ show: false });
            })
            .catch(() => {
                setErrorMessage({show: true, text: 'Транспортные компании не найдены'})
            })
            .finally(() => 
                isLoading(false));
    }
    const postalAddressChangeProc = (address: Address) => {
        let adddressReady = (address.street != null && address.street != '') &&
            (address.house != null && address.house != '') &&
            (address.flat != null && address.flat != '') &&
            (address.postalCode != null && address.postalCode != '');
        if (shippings !== undefined || !adddressReady)
            return;
        isLoading(true);
        YandexDeliveryService.getAvailableShippings(
            shippingId, address)
            .then((shippings) => {
                setNewShippings(shippings);
                setErrorMessage({ show: false });
            }).catch(() => {
                setErrorMessage({show: true, text: 'Отделения не найдены'})
            })
            .finally(() => isLoading(false));
    }
    const courierAddressChangeProc = (address: Address) => {
        let adddressReady = (address.street != null && address.street != '') &&
            (address.house != null && address.house != '') &&
            (address.flat != null && address.flat != '');
        if (shippings !== undefined || !adddressReady)
            return;
        isLoading(true);
        YandexDeliveryService.getAvailableShippings(
            shippingId, address)
            .then((shippings) => {
                setNewShippings(shippings);
                setErrorMessage({ show: false });
            })
            .catch(() => {
                setErrorMessage({show: true, text: 'Транспортные компании не найдены'})
            })
            .finally(() => isLoading(false));
    }
    
    if (isClientDelivery) {
        return (
            <>
                <Row gutter={[14, 8]} style={{ marginTop: '15px' }}>
                    <Col span={14}>
                        <Title level={5}>{rs.Shipping.YandexDelivery.OrderCreate.DeliveryTypesTitle}</Title>
                        <Select
                            style={{ width: "100%" }}
                            value={deliveryType}
                            onChange={deliveryTypeHandling}
                            filterOption={SelectFilter}>
                            <Option value={YandexDeliveryType.DEFAULT} disabled hidden>
                                {rs.Shipping.YandexDelivery.OrderCreate.DeliveryTypes.Default}
                            </Option>
                            <Option value={YandexDeliveryType.COURIER}>
                                {rs.Shipping.YandexDelivery.OrderCreate.DeliveryTypes.Courier}
                            </Option>
                            <Option value={YandexDeliveryType.POST}>
                                {rs.Shipping.YandexDelivery.OrderCreate.DeliveryTypes.Post}
                            </Option>
                        </Select>
                    </Col>
                </Row>
                {
                    shippings?
                            deliveryType==YandexDeliveryType.POST?
                                <Row gutter={[14, 8]} style={{ marginTop: '15px' }}>
                                <Col span={14}>
                                    <Title level={5}>{rs.Shipping.YandexDelivery.OrderCreate.DeliveryPostalShippingsTitle}</Title>
                                    <Select
                                        style={{ width: "100%" }}
                                        value={selectedShipping?.tariffId??0}
                                        onChange={postalDeliveryShippingHandling}
                                        filterOption={SelectFilter}>
                                        <Option value={0} disabled hidden>
                                            {rs.Shipping.YandexDelivery.OrderCreate.DeliveryPostalShippingsDefault}
                                        </Option>
                                        {
                                            shippings &&
                                            shippings.map(v =>
                                                <Option key={v.tariffId} value={v.tariffId}>
                                                    {`${v.tariffName} ${v.cost}р.`}
                                                </Option>
                                            )
                                        }
                                    </Select>
                                </Col>
                            </Row>
                            :
                            <Row gutter={[14, 8]} style={{ marginTop: '15px' }}>
                            <Col span={14}>
                                <Title level={5}>{rs.Shipping.YandexDelivery.OrderCreate.DeliveryShippingsTitle}</Title>
                                <Select
                                    style={{ width: "100%" }}
                                    value={selectedShipping?.tariffId??0}
                                    onChange={courierDeliveryShippingHandling}
                                    filterOption={SelectFilter}>
                                    <Option value={0} disabled hidden>
                                        {rs.Shipping.YandexDelivery.OrderCreate.DeliveryShippingsDefault}
                                    </Option>
                                    {
                                        shippings &&
                                        shippings.map(v =>
                                            <Option key={v.tariffId} value={v.tariffId}>
                                                {`${v.tariffName} ${v.cost}р.`}
                                            </Option>
                                        )
                                    }
                                </Select>
                            </Col>
                        </Row>
                        :
                        <> </>
                }
            </>
        );
    }
    else {
        return (
            <>
                {
                    shippings?
                        <Row gutter={[15, 8]} style={{ marginTop: '15px' }}>
                            <Col span={15}>
                                <Title level={5}>{rs.Shipping.YandexDelivery.OrderCreate.DeliveryShippingsTitle}</Title>
                                <Select
                                    style={{ width: "100%" }}
                                    value={selectedShipping?.tariffId??0}
                                    onChange={pickpointDeliveryShippingHandling}
                                    filterOption={SelectFilter}>
                                    <Option value={0} disabled hidden>
                                        {rs.Shipping.YandexDelivery.OrderCreate.DeliveryShippingsDefault}
                                    </Option>
                                    {
                                        shippings &&
                                        shippings.map(v =>
                                            <Option key={v.tariffId} value={v.tariffId}>
                                                {`${v.tariffName} ${v.cost}р.`}
                                            </Option>
                                        )
                                    }
                                </Select>
                            </Col>
                        </Row>
                        :
                        <> </>
                }
            </>
        );
    }

}
