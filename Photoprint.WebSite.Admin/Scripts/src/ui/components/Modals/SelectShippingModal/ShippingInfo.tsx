import { Col, Input, Row, Select } from 'antd';
import Title from 'antd/lib/typography/Title';
import React, { useEffect, useMemo, useState } from 'react';
import { rs } from '../../../../globals/pxp';
import { IAddressInfoDTO, ICityInfoDTO, ICountryInfoDTO, IRegionsInfoDTO } from '../../../../services/ShippingService';
import { SelectFilter } from '../../../utils/SelectFilter';
import { DeliveryAddressProperties, DeliveryProperties, DeliveryPropertiesFactory } from './DeliveryProperties/DeliveryProperties';
import { Address, ErrorMessage, Shipping, ShippingData } from './SelectShippingModal';
const { Option } = Select;

interface IShippingInfoProps {
    selectedShipping: Shipping;
    shippingData: ShippingData;
    onChange: (address: Address) => void;
    updShippingData: (data: ShippingData) => void,
    setShippingInfoReady: (ready: boolean) => void,
    selectedAddress: Address;
    isLoading: (value: React.SetStateAction<boolean>) => void;
    setErrorMessage: (value: React.SetStateAction<ErrorMessage | null>) => void;
}
class DisplayShippingSettings {
    countrySelectView: boolean;
    country: ICountryInfoDTO | null;
    regions: IRegionsInfoDTO[]
    regionSelectView: boolean;
    region: IRegionsInfoDTO | null;
    cities: ICityInfoDTO[];
    citySelectView: boolean;
    city: ICityInfoDTO | null;
    addressa: IAddressInfoDTO[]
    addressSelectView: boolean;
    address: IAddressInfoDTO | null;
    constructor() {
        this.countrySelectView = false;
        this.country = null;
        this.regions = [];
        this.regionSelectView = false;
        this.region = null;
        this.cities = [];
        this.citySelectView = false;
        this.city = null;
        this.addressa = [];
        this.addressSelectView = false;
        this.address = null;
    }
}

export const ShippingInfo = (props: IShippingInfoProps) => {
    const { selectedShipping, shippingData, selectedAddress, updShippingData, setShippingInfoReady,
        isLoading, setErrorMessage } = props;
    const { addresses, isClientDelivery, isPostCodeRequired } = shippingData;
    const [deliveryPropertiesReady, setDeliveryPropertiesReady] = useState(false);

    let settings = new DisplayShippingSettings();
    if (addresses) {
        settings.countrySelectView = addresses && (addresses.Countries.length > 1 || addresses.Countries[0].Country != "");
        settings.country = addresses && settings.countrySelectView ? addresses.Countries.find(c => c.Country == selectedAddress.country) ?? null : null;
        settings.regions = settings.country != null ? settings.country.Regions : addresses?.Countries[0].Regions;
        settings.regionSelectView = settings.regions.length > 1 || settings.regions[0].Region != "";
        settings.region = settings.regionSelectView ? settings.regions.find(r => r.Region == selectedAddress.region) ?? null : null;
        settings.cities = settings.region != null ? settings.region.Cities : settings.regions[0].Cities;
        settings.citySelectView = settings.cities.length > 1 || settings.cities[0].City != "";
        settings.city = settings.citySelectView ? settings.cities.find(city => city.City == selectedAddress.city) ?? null : null;
        settings.addressa = settings.city != null ? settings.city.Addresses : settings.cities[0].Addresses;
        settings.addressSelectView = settings.addressa.length > 1 || settings.addressa[0].AddressLine != "" || settings.addressa[0].AddressName != "";
        settings.address = settings.addressSelectView ? settings.addressa.find(a => a.Id == selectedAddress.id) ?? null : null;
    }

    //memo
    const isAddressReadyComp = (): boolean => {
        if (!isClientDelivery) {
            return (selectedAddress.country != null && selectedAddress.country != '') &&
                (selectedAddress.region != null && selectedAddress.region != '') &&
                (selectedAddress.city != null && selectedAddress.city != '') &&
                (selectedAddress.address != null && selectedAddress.address != '');
        } else {
            return (selectedAddress.country != null && selectedAddress.country != '') &&
                (selectedAddress.region != null && selectedAddress.region != '') && 
                (selectedAddress.city != null && selectedAddress.city != '') &&
                (selectedAddress.street != null && selectedAddress.street != '') &&
                (selectedAddress.house != null && selectedAddress.house != '') &&
                (selectedAddress.flat != null && selectedAddress.flat != '')  && 
                (!isPostCodeRequired || (selectedAddress.postalCode != null && selectedAddress.postalCode != ''));
        }
    }
    const isAddressReady = useMemo(isAddressReadyComp, [selectedAddress]);

    //effects

    const shippingInfoReady = () => {
        setShippingInfoReady (isAddressReady && deliveryPropertiesReady);
    }
    useEffect(shippingInfoReady,
        [isAddressReady, deliveryPropertiesReady])

    //handlers
    const handleCountryChange = (country: string) => {
        console.log("country change", country);
        var newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.country = country;
        if (addresses && settings.countrySelectView) {
            let countryObj = addresses.Countries.find(c => c.Country == country)!
            if (countryObj.Regions[0].Region != "")
                newAddress.region = countryObj.Regions[0].Region;
            if (countryObj.Regions[0].Cities[0].City != "")
                newAddress.city = countryObj.Regions[0].Cities[0].City;
            newAddress.id = countryObj.Regions[0].Cities[0].Addresses[0].Id;
            if (!shippingData.isClientDelivery && countryObj.Regions[0].Cities[0].Addresses[0].AddressLine != "") {
                newAddress.address = countryObj.Regions[0].Cities[0].Addresses[0].AddressLine;
            }
        }
        props.onChange(newAddress)
    }
    const handleRegionChange = (Region: string) => {
        console.log("Region change", Region);
        var newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.region = Region;
        if (addresses) {
            let countryObj = addresses.Countries.find(c => c.Country == newAddress.country) ?? addresses.Countries[0];
            let RegionObj = countryObj.Regions.find(s => s.Region == Region) ?? countryObj.Regions[0];
            if (RegionObj.Cities[0].City != "")
                newAddress.city = RegionObj.Cities[0].City;
            newAddress.id = RegionObj.Cities[0].Addresses[0].Id;
            if (!shippingData.isClientDelivery && RegionObj.Cities[0].Addresses[0].AddressLine != "") {
                newAddress.address = RegionObj.Cities[0].Addresses[0].AddressLine;
            }
        }
        props.onChange(newAddress)
    }
    const handleCityChange = (city: string) => {
        console.log("city change", city);
        var newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.city = city;
        if (city == rs.Shipping.Address.ChooseAnotherCity) {
            newAddress.region='';
            newAddress.city='';
            newAddress.address='';
            setErrorMessage({show: false});
        }
        if (addresses) {
            let countryObj = addresses.Countries.find(c => c.Country == newAddress.country) ?? addresses.Countries[0];
            let RegionObj = countryObj.Regions.find(s => s.Region == newAddress.region) ?? countryObj.Regions[0];
            let cityObj = RegionObj.Cities.find(s => s.City == city) ?? RegionObj.Cities[0];
            newAddress.id = cityObj.Addresses[0].Id;
            if (!isClientDelivery && cityObj.Addresses[0].AddressLine != "") {
                newAddress.address = cityObj.Addresses[0].AddressLine;
            }
        }
        props.onChange(newAddress)
    }
    const handlAddressChange = (value: { value: number | null, label: string }) => {
        console.log("address change", value);
        var newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        if (value.value != null) {
            let address = shippingData.addresses
                ?.Countries.find(c => c.Country == selectedAddress.country)
                ?.Regions.find(r => r.Region == selectedAddress.region)
                ?.Cities.find(c => c.City == selectedAddress.city)
                ?.Addresses.find(a => a.Id == value.value);
            if (address == null) return;
            newAddress.id = address.Id;
            newAddress.address = address.AddressLine;
            newAddress.street = address.Street;
            newAddress.house = address.House;
            newAddress.latitude = address.Latitude;
            newAddress.postalCode = address.PostalCode ?? '';
            newAddress.deliveryProperties = DeliveryPropertiesFactory.getObj(address.DeliveryProperties);
        } else 
            newAddress.address = value.label;
        
        props.onChange(newAddress)
    }
    const handlStreetChange = (street: string) => {
        console.log("street change", street);
        var newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.street = street;
        props.onChange(newAddress)
    }
    const handlHouseChange = (house: string) => {
        console.log("house change", house);
        var newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.house = house;
        props.onChange(newAddress)
    }
    const handlFlatChange = (flat: string) => {
        console.log("flat change", flat);
        var newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.flat = flat;
        props.onChange(newAddress)
    }
    const handlePostCodeChage = (postCode: string) => {
        let newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.postalCode = postCode;
        props.onChange(newAddress);
    }

    const deliveryPropertiesChange = (deliveryProps: DeliveryAddressProperties) => {
        let newAddress: Address = JSON.parse(JSON.stringify(selectedAddress));
        newAddress.deliveryProperties = deliveryProps;
        props.onChange(newAddress);
    }

    return (
        <>
            <div className="dl-group">
                <dl>
                    <dt>{rs.Order.Info.ShippingTypeLabel}</dt>
                    <dd>
                        <span>{selectedShipping?.typeString}</span>
                        {shippingData && shippingData.shippingProviderType &&
                            <>&nbsp;
                                <span>
                                    (<strong>{shippingData.shippingProviderType}</strong>)
                        </span>
                            </>
                        }
                    </dd>
                </dl>
                <dl>
                    <dt>{rs.Order.Info.ShippingFormLabel}</dt>
                    <dd>
                        {
                            props.shippingData.isClientDelivery
                                ? <span>{rs.Order.Info.WarehouseDoorDelivery}</span>
                                : <span>{rs.Order.Info.WarehouseWarehouseDelivery}</span>
                        }
                    </dd>
                </dl>
            </div >
            <DeliveryProperties 
                shippingData={props.shippingData}
                selectedAddress={selectedAddress}
                onChange={deliveryPropertiesChange}
                updShippingData={updShippingData}
                setPropertiesReady={setDeliveryPropertiesReady}
                isLoading={isLoading}
                setErrorMessage={setErrorMessage}/>
            <h3>
                {rs.Order.Info.SelectShippingAddressLabel}
            </h3>
            <Row gutter={[8, 8]}>
                <Col span={8}>
                    <Title level={5}>{rs.Shipping.Address.Country}</Title>
                    {
                        addresses && settings.country ?
                            <Select
                                disabled={
                                    selectedShipping.type == 1 //point
                                    || addresses.Countries.length == 1
                                }
                                style={{ width: "100%" }}
                                value={settings.country.Country}
                                onChange={handleCountryChange}
                                filterOption={SelectFilter}
                            >
                                {
                                    addresses.Countries.map(c =>
                                        <Option key={c.Country} value={c.Country}>
                                            {c.Country}
                                        </Option>
                                    )
                                }


                            </Select>
                            : <Input onChange={(e) => { handleCountryChange(e.target.value) }} value={selectedAddress.country} />
                    }

                </Col>
                <Col span={8}>
                    <Title level={5}>{rs.Shipping.Address.Region}</Title>
                    {
                        addresses && settings.region ?
                            <Select
                                disabled={
                                    selectedShipping.type == 1 //point
                                    || settings.regions.length == 1
                                }
                                style={{ width: "100%" }}
                                value={settings.region.Region}
                                onChange={handleRegionChange}
                                filterOption={SelectFilter}
                            >
                                {
                                    addresses &&
                                    settings.regions.map(c =>
                                        <Option key={c.Region} value={c.Region}>
                                            {c.Region}
                                        </Option>
                                    )
                                }


                            </Select>
                            : <Input onChange={(e) => { handleRegionChange(e.target.value) }} value={selectedAddress.region} />
                    }
                </Col>
                <Col span={8}>
                    <Title level={5}>{rs.Shipping.Address.City}</Title>
                    {
                        addresses && settings.city ?
                            <Select
                                disabled={
                                    selectedShipping.type == 1 //point
                                    || settings.cities.length == 1
                                }
                                style={{ width: "100%" }}
                                value={settings.city.City}
                                onChange={handleCityChange}
                                filterOption={SelectFilter}
                            >
                                {
                                    addresses &&
                                    settings.cities.map(c =>
                                        <Option key={c.City} value={c.City}>
                                            {c.City}
                                        </Option>
                                    )
                                }


                            </Select>
                            : <Input onChange={(e) => { handleCityChange(e.target.value) }} value={selectedAddress.city} />
                    }
                </Col>
                {isClientDelivery
                    ?
                    <>
                        {isPostCodeRequired?
                            <>
                                <Col span={12}>
                                    <Title level={5}>{rs.Shipping.Address.Street}</Title>
                                    <Input onChange={(e) => { handlStreetChange(e.target.value) }} value={selectedAddress.street} />
                                </Col>
                                <Col span={4}>
                                    <Title level={5}>{rs.Shipping.Address.House}</Title>
                                    <Input onChange={(e) => { handlHouseChange(e.target.value) }} value={selectedAddress.house} />
                                </Col>
                                <Col span={4}>
                                    <Title level={5}>{rs.Shipping.Address.Room}</Title>
                                    <Input onChange={(e) => { handlFlatChange(e.target.value) }} value={selectedAddress.flat} />
                                </Col>
                                <Col span={4}>
                                    <Title level={5}>{rs.Shipping.Address.PostalCode}</Title>
                                    <Input onChange={(e) => { handlePostCodeChage(e.target.value) }} value={selectedAddress.postalCode} />
                                </Col>
                            </>:
                            <>
                                <Col span={12}>
                                    <Title level={5}>{rs.Shipping.Address.Street}</Title>
                                    <Input onChange={(e) => { handlStreetChange(e.target.value) }} value={selectedAddress.street} />
                                </Col>
                                <Col span={6}>
                                    <Title level={5}>{rs.Shipping.Address.House}</Title>
                                    <Input onChange={(e) => { handlHouseChange(e.target.value) }} value={selectedAddress.house} />
                                </Col>
                                <Col span={6}>
                                    <Title level={5}>{rs.Shipping.Address.Room}</Title>
                                    <Input onChange={(e) => { handlFlatChange(e.target.value) }} value={selectedAddress.flat} />
                                </Col>
                            </>
                        }
                    </>
                    :
                    <Col span={24}>
                        <Title level={5}>{rs.Shipping.Address.Street}</Title>
                        {
                            addresses && settings.address ?
                                <Select
                                    labelInValue
                                    disabled={
                                        selectedShipping.type == 1 //point
                                        || settings.addressa.length == 1
                                    }
                                    style={{ width: "100%" }}
                                    value={{ label: settings.address.AddressLine != "" ? settings.address.AddressLine : settings.address.AddressName, value: settings.address.Id! }}
                                    onChange={handlAddressChange}
                                    filterOption={SelectFilter}
                                >
                                    {
                                        addresses &&
                                        settings.addressa.map(a =>
                                            a.Id!=null?
                                            <Option key={a.Id} value={a.Id}>
                                                {a.AddressLine != "" ? a.AddressLine : a.AddressName}
                                            </Option>
                                            :
                                            <>
                                            </>
                                        )
                                    }


                                </Select>
                                : <Input onChange={(e) => { handlAddressChange({ label: e.target.value, value: null }) }} value={selectedAddress.address} />
                        }
                    </Col>
                }
            </Row>
        </>


    );
}