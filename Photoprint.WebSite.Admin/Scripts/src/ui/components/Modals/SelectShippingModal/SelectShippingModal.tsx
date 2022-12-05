import { pxpGlobal } from '../../../../globals/pxp';
import { useRef, useState } from 'react';
import React from 'react';
import { Modal } from '../../../../components/Modal';
import { IAddressHierarchyDTO, IGetShippingDataResponse, IShippingDTO, ShippingService } from '../../../../services/ShippingService';
import { Col, Empty, Row } from 'antd';
import { ShippingInfo } from './ShippingInfo';
import { ShippingSelect } from './ShippingSelect';
import { SkeletonShipping } from './SkeletonShipping';
import { IShippingResult } from '../../../../order/create/components/ShippingSelectBlock';
import { DeliveryAddressProperties, DeliveryPropertiesFactory } from './DeliveryProperties/DeliveryProperties';



interface ISelectShippingModalProps {
    onClose?: () => void;
    onChange?: (address: IShippingResult) => void;
    title: string;
    isOpen: boolean;
    totalPrice: number;
}
export class Shipping {
    id: number;
    title: string;
    type: number;
    typeString: string;
    data: ShippingData | null;
    constructor(shippingDTO: IShippingDTO) {
        this.id = shippingDTO.Id;
        this.title = shippingDTO.Title;
        this.type = shippingDTO.Type;
        this.typeString = shippingDTO.TypeString;
        this.data = null;
    }
}
export class ShippingData {
    addresses: IAddressHierarchyDTO | null;
    shippingId: number;
    isClientDelivery: boolean;
    isPostCodeRequired: boolean;
    isSeparateAddress: boolean;
    shippingProviderType: string | null;
    constructor(dto: IGetShippingDataResponse) {
        this.shippingId = dto.ShippingId;
        this.shippingProviderType = dto.ShippingProviderType;
        this.isSeparateAddress = dto.IsSeparateAddress;
        this.isPostCodeRequired = dto.IsPostCodeRequired;
        this.isClientDelivery = dto.IsClientDelivery;
        this.addresses = dto.Addresses;


    }
}
export class Address {
    id: number | null;
    country: string;
    region: string;
    city: string;
    address: string;
    street: string;
    house: string;
    flat: string;
    latitude: string | null;
    longitude: string | null;
    postalCode: string;
    deliveryProperties?: DeliveryAddressProperties

    constructor() {
        this.id = null;
        this.country = "";
        this.region = "";
        this.city = "";
        this.address = "";
        this.street = "";
        this.house = "";
        this.flat = "";
        this.latitude = null;
        this.longitude = null;
        this.postalCode = "";
        this.deliveryProperties = new DeliveryAddressProperties();
    }
}

export interface ErrorMessage {
    show: boolean,
    text?: string
}

export const SelectShippingModal = (props: ISelectShippingModalProps) => {
    const xhrRefList = useRef(0);
    const xhrRefData = useRef(0);
    const [isLoading, setIsLoading] = useState(false);
    const [dataLoading, setDataLoading] = useState(true);
    const [shippingDataReady, setShippingInfoReady] = useState(false);
    const [shippingList, setShippingList] = useState<Shipping[]>([]);
    const [selectedShipping, setSelectedShipping] = useState<number | null>(null);
    const [selectedAddress, setSelectedAddress] = useState<Address>(new Address());
    const [errorMessage, setErrorMessage] = useState<ErrorMessage | null>(null);

    const closeModal = () => { if (props.onClose) props.onClose(); }
    const saveClick = () => {
        if (props.onChange) props.onChange({ adddress: selectedAddress, shipping: shippingList.find(s => s.id == selectedShipping)! } as IShippingResult);
        closeModal();

    }
    const shipping = shippingList.find(s => s.id == selectedShipping) ?? null;
    const selectAddress = (shippingData: ShippingData) => {
        var selectedAddress = new Address();
        if (shippingData.addresses) {
            let country = shippingData.addresses.Countries[0];
            selectedAddress.country = country.Country;
            let state = country.Regions[0];
            selectedAddress.region = state.Region;
            let city = state.Cities[0];
            selectedAddress.city = city.City;
            let props = city.Addresses[0].DeliveryProperties;
            selectedAddress.deliveryProperties = DeliveryPropertiesFactory.getObj(props);
            let address = city?.Addresses[0];
            selectedAddress.id = address.Id;
            if (!shippingData.isClientDelivery) {
                selectedAddress.address = address.AddressLine;
                selectedAddress.street = address.Street;
                selectedAddress.house = address.House;
                selectedAddress.latitude = address.Latitude;
                selectedAddress.postalCode = address.PostalCode?? '';
                selectedAddress.deliveryProperties = DeliveryPropertiesFactory.getObj(address.DeliveryProperties);

            }

        }
        setSelectedAddress(selectedAddress);

    }
    const loadShippingData = (shippingId: number, shippingsProps?: Shipping[]) => {
        var shippings: Shipping[] = shippingsProps != undefined ? shippingsProps : shippingList;
        if (shippings.find(s => s.id == shippingId)?.data) {
            selectAddress(shippings.find(s => s.id == shippingId)?.data!);
            return;
        }
        xhrRefData.current += 1;
        const ref = xhrRefData.current;
        setDataLoading(true);
        ShippingService.getShippingData(shippingId)
            .then((data) => {
                if (xhrRefData.current != ref)
                    return;
                const shippingData = new ShippingData(data);
                setShippingList(shippings.map(s => s.id != shippingId ? s : { ...s, data: shippingData }))
                selectAddress(shippingData);
                setDataLoading(false);

            })
            .catch((error) => {
                console.log(error);
            })
    }
    const loadShippingList = () => {
        setIsLoading(true);
        xhrRefList.current += 1;
        const ref = xhrRefList.current;
        ShippingService.getShippingList(pxpGlobal.frontend?.id, props.totalPrice)
            .then((shippingsDTo) => {
                if (xhrRefList.current != ref)
                    return;
                var shippings = shippingsDTo.map(s => new Shipping(s))
                setShippingList(shippings);
                setIsLoading(false);
                setShippingInfoReady(false);
                if (shippings.length == 0)
                    return;
                setSelectedShipping(shippings[0].id)
                loadShippingData(shippings[0].id, shippings);
            })
            .catch((error) => {
                console.log(error);
            })

    }
    const handleAfterOpen = () => {
        shippingList.length = 0;
        loadShippingList();
    }
    const handleShippingChange = (id: number) => {
        setSelectedShipping(id)

        loadShippingData(id);
    }

    const modalProps = {
        title: props.title,
        onClose: closeModal,
        onSave: saveClick,
        minWidth: "750px",
        isOpen: props.isOpen,
    };

    const updateShippingData = (newData: ShippingData) => {
        let newShippingList = JSON.parse(JSON.stringify(shippingList)) as Array<Shipping>;
        const shipping = newShippingList.find(s => s.id==selectedShipping!)!;
        shipping.data = newData;
        setShippingList(newShippingList);
        selectAddress(newData);
    }

    return (
        <>
            { props.isOpen &&
                < Modal
                    onAfterOpen={handleAfterOpen}
                    {...modalProps}
                    isSaveEnabled={!isLoading && !dataLoading && shippingDataReady}
                    isLoading={isLoading} >
                    <div className="modal_form">
                        <Row style={{ marginBottom: 15 }}>
                            <Col span={24}>
                                <ShippingSelect
                                    isLoading={isLoading}
                                    onChange={handleShippingChange}
                                    selectedShipping={shipping}
                                    shippingList={shippingList}
                                />
                            </Col>
                        </Row>
                        {
                            errorMessage?.show?
                                <div className="message error">
                                    <span>{errorMessage!.text}</span>
                                </div>
                                :
                                <> </>
                        }
                        {
                            selectedShipping == null
                                ? <Empty />
                                :
                                dataLoading || shipping?.data == null
                                    ?
                                    <SkeletonShipping
                                        isClientDelivery={shipping?.data?.isClientDelivery ?? false}
                                    />
                                    :
                                    <ShippingInfo
                                        selectedShipping={shipping}
                                        shippingData={shipping.data}
                                        onChange={(address: Address) => { setSelectedAddress(address) }}
                                        updShippingData={updateShippingData}
                                        setShippingInfoReady={setShippingInfoReady}
                                        selectedAddress={selectedAddress}
                                        isLoading={setIsLoading}
                                        setErrorMessage={setErrorMessage}
                                    />
                        }

                    </div>
                </Modal >
            }
        </>
    );
}