import React, { ChangeEvent, useEffect, useRef, useState } from "react"
import { rs } from "../../../globals/pxp"
import { Input, Select, Row, Col, Form, message, notification} from "antd"
import { AddressService} from "../../../services/AddressService";
import { AddressInfo, AddressSuggest, AddressSuggestType, IAddressInfoDTO } from "../../../Address/AddressInfo";
import { CityInfo, CitySuggest, ToponimType, ICityState, ICitySuggestDTO} from "../../../Address/CityInfo";
import { useForm } from "antd/lib/form/Form";
import * as Message from "antd/lib/message";
import * as Notification from "antd/lib/notification";
import ReactDOM from "react-dom";


export interface IAddressInputInitData {
    CountryId: number;
    FullAddressMode: boolean;
    UseValidation: boolean;
    InitialAddress: IAddressInfoDTO;
    IsContractor: boolean;
    SetAddressFinalData: (data?: IAddressInfoDTO) => void;
    IsNewDelivery: boolean;
}
export enum ddCityOptionType {
	CitySuggest,
    CityInfo
}
class CityddOption {
    type: ddCityOptionType;
    searchType: ToponimType;
    title: string;
    description: string | null;
    dto: ICitySuggestDTO | ICityState;

    constructor(obj: CitySuggest | CityInfo) {
        this.type = obj instanceof CitySuggest ? ddCityOptionType.CitySuggest : ddCityOptionType.CityInfo;
        this.searchType = obj.type;
        this.title = obj.title;
        this.description = obj.description;
        this.dto = obj.getDto();
    }

    getLabel(): string {
        return this.description != null ? `${this.title}, ${this.description}` : this.title;
    }
}

export const AddressInput = (initData: IAddressInputInitData) => {

    const { CountryId, FullAddressMode, UseValidation, InitialAddress, IsContractor, SetAddressFinalData, IsNewDelivery } = initData;

    const [addressModel, setAddressModel] = useState(new AddressInfo(InitialAddress));
    const {city, street, house, flat, postalCode, latitude, longitude, getAddressLine} = addressModel;

    const [ddCityValues, setddCityValues] = useState <CityddOption[]>([]);
    const [ddAddressSuggestValues, setddAddressSuggestValues] = useState<AddressSuggest[]>([]);
    
    const [isCityFetching, setIsCityFetching] = useState (false);
    const [isStreetFetching, setIsStreetFetching] = useState (false);
    const [isHouseFetching, setIsHouseFetching] = useState (false);
    const [isAddressInfoFetching, setIsAddressInfoFetching] = useState (false);

    const [form] = useForm();

    const isMapsNotAvailable = useRef(false);
    const isInfoNotFound = useRef(false);
    const useFormValidation = UseValidation && FullAddressMode;
    const isValid = useRef(false);

    useEffect(() => {
        document.querySelector('form')?.addEventListener('submit', (event) => {
            if (!UseValidation) return;
            if (FullAddressMode) {
                if(isValid.current) return;
                const submiter = (event as any).submitter;
                event.preventDefault();
                form.validateFields()
                    .then((result) => {
                        isValid.current = true;
                        setTimeout(() => {submiter?.click()},500);
                    })
                    .catch((data) => {
                        isValid.current = false;
                        form.scrollToField('city', {block:'center', behavior:'smooth'});
                    });
            }
            const values = form.getFieldsValue();
            if ((values.city ?? '') == '' && (values.region ?? '') == '' && (values.country ?? '') == '') {
                event.preventDefault();
                form.scrollToField('city', {block:'center', behavior:'smooth'});
                message.open(cityValidateMessage);
            }
        })
    }, []);

    const badSettingsMessage: Message.ArgsProps = {
        type: 'error',
        content: rs.Shipping.Address.BadMapSettingsMessage,
        className:'error',
        duration: 20 
    }
    const mapsNotAvailableMessage: Message.ArgsProps = {
        type: 'error',
        content: rs.Shipping.Address.MapsNotAvailableMessage,
        className:'error',
        duration: 20 
    }
    const cityValidateMessage: Message.ArgsProps = {
        type: 'error',
        content: 'Необходимо заполнить хотя бы один топоним',
        className:'error',
        duration: 20 
    }
    const infoNotFoundNotificationKey = 'infoNotFound'
    const infoNotFoundNotification: Notification.ArgsProps = {
        type: 'warning',
        key: infoNotFoundNotificationKey,
        message: rs.Shipping.Address.InfoNotFoundMessage,
        duration: 0
    }

    const infoNotFoundByCoordsNotificationKey = 'infoNotFound'
    const infoNotFoundByCoordsNotification: Notification.ArgsProps = {
        type: 'warning',
        key: infoNotFoundNotificationKey,
        message: rs.Shipping.Address.InfoNotFoundByCoordsMessage,
        duration: 0
    }

    const baseFetchCitySuggest = (type: ToponimType, search: string) => {
        if (search) {
            if (isMapsNotAvailable.current) {
                setCityInfo(_getCityInfoByType(type, search));
                setddCityValues([]);
                return;
            }
            setIsCityFetching(true);
            AddressService.suggestCities(CountryId, type, search)
                .then((values) => {
                    if (values.length == 0) {
                        notification.open(infoNotFoundNotification);
                        isInfoNotFound.current = true;
                        setCityInfo(_getCityInfoByType(type, search));
                    } else {
                        isInfoNotFound.current = false;
                        notification.close(infoNotFoundNotificationKey);
                    }
                    setddCityValues(values.map(v => new CityddOption(v)));
                })
                .catch((data) => {
                    message.open(data.Message == 'Bad maps provider settings!'
                        ? badSettingsMessage : mapsNotAvailableMessage);
                    isMapsNotAvailable.current = true;
                    setCityInfo(_getCityInfoByType(type, search));
                    setddCityValues([]);
                })
                .finally(() => {
                    setIsCityFetching(false);
                });
            
        } else {
            setddCityValues([]);
        }
        
        function _getCityInfoByType(type: ToponimType, search: string): CityInfo {
            switch (type) {
                case ToponimType.Country:
                    return new CityInfo({Country: search, Type: ToponimType.Country} as ICityState);
                case ToponimType.Region:
                    return new CityInfo({Region: search, Type: ToponimType.Region} as ICityState);
                case ToponimType.City:
                    return new CityInfo({Title: search, Type: ToponimType.City} as ICityState);
            }
        }
    }
    
    const fetchCountrySuggest = (search: string) => baseFetchCitySuggest(ToponimType.Country, search);
    const fetchRegionSuggest = (search: string) => baseFetchCitySuggest(ToponimType.Region, search);
    const fetchCitiesSuggest = (search: string) => baseFetchCitySuggest(ToponimType.City, search);

    const ddCitySelect = (value: string, option: {label:string, value:string}) => {
        let val = JSON.parse(option.value) as CityddOption;
        if (val.type == ddCityOptionType.CitySuggest) {
            let suggest = new CitySuggest(val.dto as ICitySuggestDTO);
			AddressService.getCityInfo(CountryId, suggest)
                .then((cities) => {
                    if (cities.length == 0) {
                        setCityInfo(GetCityInfoByType(suggest));
                    }
                    if (cities.length == 1) {
                        setCityInfo(cities[0]);
                        setddCityValues([]);
					    return;
                    }
				    setddCityValues(cities.map(c => new CityddOption(c)));
			    })
                .catch((data) => {
                    message.open(data.Message == 'Bad maps provider settings!'
                        ? badSettingsMessage : mapsNotAvailableMessage);
                    isMapsNotAvailable.current = true;
                    setCityInfo(GetCityInfoByType(suggest));
                    setddCityValues([]);
                });
		}
        else if (val.type==ddCityOptionType.CityInfo) {
			setCityInfo(new CityInfo(val.dto as ICityState));
            setddCityValues([]);
        }

        function GetCityInfoByType(suggest: CitySuggest):CityInfo {
            switch (suggest.type) {
                case ToponimType.Country:
                    return new CityInfo({Country: suggest.title, Type: ToponimType.Country} as ICityState);
                case ToponimType.Region:
                    return new CityInfo({Region: suggest.title, Type: ToponimType.Region} as ICityState);
                case ToponimType.City:
                    return new CityInfo({Title: suggest.title, Type: ToponimType.City} as ICityState);
            }
        }
    }

    const setCityInfo = (city: CityInfo) => {
        let newModel = addressModel.copy(IsNewDelivery && !(isMapsNotAvailable.current || isInfoNotFound.current));
        newModel.city = city;
        if (!FullAddressMode) {
            newModel.latitude = city.coords?.latitude ?? '';
            newModel.longitude = city.coords?.longitude ?? '';
        }
		setAddressModel(newModel);
    }

    const fetchStreetSuggest = (search: string) => {
        if (!city || city.title=='')
            return;
        if (search) {
            if (isMapsNotAvailable.current) {
                setStreet(search);
                setddAddressSuggestValues([]);
                return;
            }
            setIsStreetFetching(true);
            AddressService.suggestAddresses(search, city.getDto(), AddressSuggestType.Street)
                .then((values) => {
                    if (values.length == 0) {
                        if (!isInfoNotFound.current) {
                            notification.open(infoNotFoundNotification);
                            isInfoNotFound.current = true;
                        }
                        setStreet(search);
                    } else {
                        isInfoNotFound.current = false;
                        notification.close(infoNotFoundNotificationKey);
                    }
                    setddAddressSuggestValues(values);
                })
                .catch((data) => {
                    message.open(mapsNotAvailableMessage);
                    isMapsNotAvailable.current = true;
                    setStreet(search);
                    setddAddressSuggestValues([]);
                })
                .finally(() => {
                    setIsStreetFetching(false);
                });
            
        } else {
            setddAddressSuggestValues([]);
        }
    }

    const ddStreetSuggestSelect = (value: string, option: {label:string, value:string}) => {
        let suggest = JSON.parse(option.value) as AddressSuggest;
        setStreet(suggest.street);
    }

    const setStreet = (value: string) => {
        let newModel = addressModel.copy(!(isMapsNotAvailable.current || isInfoNotFound.current));
        newModel.city = addressModel.city;
        newModel.street = value;
		setAddressModel(newModel);
    }

    const fetchHouseSuggest = (search: string) => {
        if (!city || city.title=='' || !street || street=='')
            return

        if (search) {
            if (isMapsNotAvailable.current) {
                setHouse(search);
                setddAddressSuggestValues([]);
                return;
            }
            setIsHouseFetching(true);
            AddressService.suggestAddresses(search, city.getDto(), AddressSuggestType.House, street)
                .then((values) => {
                    if (values.length == 0) {
                        if (!isInfoNotFound.current) {
                            notification.open(infoNotFoundNotification);
                            isInfoNotFound.current = true;
                        }
                        setHouse(search);
                    } else {
                        isInfoNotFound.current = false;
                        notification.close(infoNotFoundNotificationKey);
                    }
                    setddAddressSuggestValues(values);
                })
                .catch((data) => {
                    message.open(mapsNotAvailableMessage);
                    isMapsNotAvailable.current = true;
                    setHouse(search);
                    setddAddressSuggestValues([]);
                })
                .finally(() => {
                    setIsHouseFetching(false);
                });
            
        } else {
            setddAddressSuggestValues([]);
        }
    }

    const ddHouseSuggestSelect = (value: string, option: {label:string, value:string}) => {
        let suggest = JSON.parse(option.value) as AddressSuggest;
        setHouse(suggest.house, suggest);
    }

    const setHouse = (value: string, suggest?: AddressSuggest) => {
        if (suggest) {
            getAddressInfoBySuggest(suggest);
            return;
        }
        let newModel = addressModel.copy();
        newModel.city = addressModel.city;
        newModel.street = addressModel.street;
        newModel.house = value;
        setAddressModel(newModel);
    }

    const handleLatChange = (event: ChangeEvent<HTMLInputElement>) => {
        const lat = event.target.value;
        if (latitude == lat) return;
        let newModel = addressModel.copy(!(isMapsNotAvailable.current || isInfoNotFound.current));
        newModel.latitude = lat;
		setAddressModel(newModel);
    }

    const handleLonChange = (event: ChangeEvent<HTMLInputElement>) => {
        const lon = event.target.value;
        if (longitude == lon) return;
        if (isMapsNotAvailable.current) {
            let newModel = addressModel.copy();
            newModel.latitude = addressModel.latitude;
            newModel.longitude = lon;
		    setAddressModel(newModel);
            return;
        }
        getAddressInfoByCoords(latitude, lon);
    }

    const handleFlatChage = (event: ChangeEvent<HTMLInputElement>) => {
        let newModel = addressModel.copy();
        newModel.flat = event.target.value;
		setAddressModel(newModel);
    }

    const handlePostalCodeChage = (event: ChangeEvent<HTMLInputElement>) => {
        let newModel = addressModel.copy();
        newModel.postalCode = event.target.value;
		setAddressModel(newModel);
    }

    const handleAddressLineChage = (event: ChangeEvent<HTMLInputElement>) => {
        let newModel = addressModel.copy();
        newModel.addressLine = event.target.value;
		setAddressModel(newModel);
    }

    const getAddressInfoBySuggest = (suggest: AddressSuggest) => {
        if (!suggest) return;
        setIsAddressInfoFetching(true);
        AddressService.getAddressInfoBySuggest(suggest)
            .then((info) => {
                if (!info) {
                    notification.open(infoNotFoundNotification);
                    isInfoNotFound.current = true;
                    return;
                }
                notification.close(infoNotFoundNotificationKey);
                isInfoNotFound.current = false;
                setAddressModel(info);
            })
            .catch((data) => {
                message.open(data.Message == 'Bad maps provider settings!'
                    ? badSettingsMessage : mapsNotAvailableMessage);
                isMapsNotAvailable.current = true;
            })
            .finally(() => {
                setIsAddressInfoFetching(false);
            });
    }

    const getAddressInfoByCoords = (latitude: string, longitude: string) => {
        if (latitude == '' || longitude == '') return;
        setIsAddressInfoFetching(true);
        AddressService.getAddressInfoByCoords(latitude, longitude)
            .then((info) => {
                if (!info) {
                    notification.open(infoNotFoundByCoordsNotification);
                    isInfoNotFound.current = true;
                    return;
                }
                notification.close(infoNotFoundByCoordsNotificationKey);
                isInfoNotFound.current = false;
                setAddressModel(info);
            })
            .catch((data) => {
                message.open(data.Message == 'Bad maps provider settings!'
                    ? badSettingsMessage : mapsNotAvailableMessage);
                isMapsNotAvailable.current = true;
            })
            .finally(() => {
                setIsAddressInfoFetching(false);
            });
    }

    useEffect(() => {

        form.setFieldsValue({
            city: addressModel.city?.title,
            region : addressModel.city?.region,
            country: addressModel.city?.country,
            street: addressModel.street,
            house: addressModel.house,
            latitude: addressModel.latitude,
            longitude: addressModel.longitude,
            postalCode: addressModel.postalCode
        });

        SetAddressFinalData(addressModel.getDto());

    }, [addressModel]);


    return (
        <>
            <Form 
                form={form}
                className={isAddressInfoFetching ? 'loading-wheel' : undefined}>
                <Row gutter={[16, 8]}>
                    <Col span={8}>
                        <Form.Item
                            name='city'
                            rules={[{ required: useFormValidation, message: rs.Validators.Required }]}
                            label={rs.Shipping.Address.City}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                            <Select
                                showSearch
                                allowClear
                                notFoundContent={isCityFetching ? <div className={'loading-wheel'}/> : null}
                                value={city?.title}
                                showArrow={false}
                                options={ddCityValues
                                    .filter(v => v.searchType == ToponimType.City)
                                    .map((val) => { return {label: val.getLabel(), value: JSON.stringify(val)}})}
                                onSearch={fetchCitiesSuggest}
                                onSelect={ddCitySelect as any}/>
                        </Form.Item>
                    </Col>
                    <Col span={8}>
                        <Form.Item
                            name='region'
                            rules={[{ required: useFormValidation, message: rs.Validators.Required }]}
                            label={rs.Shipping.Address.Region}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                            <Select
                                showSearch
                                allowClear
                                notFoundContent={isCityFetching ? <div className={'loading-wheel'}/> : null}
                                value={city?.region}
                                showArrow={false}
                                options={ddCityValues
                                    .filter(v => v.searchType == ToponimType.Region)
                                    .map((val) => { return {label: val.getLabel(), value: JSON.stringify(val)}})}
                                onSearch={fetchRegionSuggest}
                                onSelect={ddCitySelect as any}/>
                        </Form.Item>
                    </Col>
                    <Col span={8}>
                        <Form.Item
                            name='country'
                            rules={[{ required: useFormValidation, message: rs.Validators.Required }]}
                            label={rs.Shipping.Address.Country}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                            <Select
                                showSearch
                                allowClear
                                notFoundContent={isCityFetching ? <div className={'loading-wheel'}/> : null}
                                value={city?.country}
                                showArrow={false}
                                options={ddCityValues
                                    .filter(v => v.searchType == ToponimType.Country)
                                    .map((val) => { return {label: val.getLabel(), value: JSON.stringify(val)}})}
                                onSearch={fetchCountrySuggest}
                                onSelect={ddCitySelect as any}/>
                        </Form.Item>
                    </Col>
                    {
                        FullAddressMode &&
                        <>
                            <Col span={8}>
                                <Form.Item
                                    name='street'
                                    label={rs.Shipping.Address.Street}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                    <Select 
                                        showSearch
                                        allowClear
                                        notFoundContent={isStreetFetching ? <div className={'loading-wheel'}/> : null}
                                        value={street}
                                        showArrow={false}
                                        options={ddAddressSuggestValues
                                            .filter((val) => val.type == AddressSuggestType.Street)
                                            .map((val) => { return { label: `${val.street}, ${val.description}`, value: JSON.stringify(val) } })}
                                        onSearch={fetchStreetSuggest}
                                        onSelect={ddStreetSuggestSelect as any}/>
                                </Form.Item>
                            </Col>
                            <Col span={8}>
                                <Form.Item
                                    name='house'
                                    rules={[{ required: useFormValidation, message: rs.Validators.Required }]}
                                    label={rs.Shipping.Address.House}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                    <Select 
                                        showSearch
                                        allowClear
                                        notFoundContent={isHouseFetching ? <div className={'loading-wheel'}/> : null}
                                        value={house}
                                        showArrow={false}
                                        options={ddAddressSuggestValues
                                            .filter((val) => val.type == AddressSuggestType.House)
                                            .map((val) => { return { label: val.house, value: JSON.stringify(val) } })}
                                        onSearch={fetchHouseSuggest}
                                        onSelect={ddHouseSuggestSelect as any}/>
                                </Form.Item>
                            </Col>
                            <Col span={8}>
                                <Form.Item
                                    label={rs.Shipping.Address.Flat}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                    <Input value={flat} onChange={handleFlatChage}/>
                                </Form.Item>
                            </Col>
                            <Col span={8}>
                                <Form.Item
                                    name='latitude'
                                    rules={[{ required: useFormValidation, message: rs.Validators.Required }]}
                                    label={rs.Shipping.Address.Latitude}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                    <Input value={latitude} onBlur={handleLatChange}/>
                                </Form.Item>
                            </Col>
                            <Col span={8}>
                                <Form.Item
                                    name='longitude'
                                    rules={[{ required: useFormValidation, message: rs.Validators.Required }]}
                                    label={rs.Shipping.Address.Longitude}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                    <Input value={longitude} onBlur={handleLonChange}/>
                                </Form.Item>
                            </Col>
                            <Col span={8}>
                                <Form.Item
                                    name='postalCode'
                                    rules={[{ required: useFormValidation, message: rs.Validators.Required }]}
                                    label={rs.Shipping.Address.PostalCode}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                    <Input value={postalCode} onChange={handlePostalCodeChage}/>
                                </Form.Item>
                            </Col>
                            {!IsContractor && 
                                <Col span={24}>
                                    <Form.Item
                                        label={rs.Shipping.Address.Title}
                                        labelCol={{ span: 24}}
                                        wrapperCol={{span: 24}}>
                                        <Input value={getAddressLine()} onChange={handleAddressLineChage}/>
                                    </Form.Item>                               
                                </Col>
                            }
                        </>
                    }
                </Row>
            </Form>
        </>
    );
}

export const render = (container: HTMLElement, initData: IAddressInputInitData) => {
    ReactDOM.render(<AddressInput 
        CountryId={initData.CountryId}
        FullAddressMode={initData.FullAddressMode}
        UseValidation={initData.UseValidation}
        InitialAddress={initData.InitialAddress}
        IsContractor={initData.IsContractor}
        SetAddressFinalData={initData.SetAddressFinalData}
        IsNewDelivery={initData.IsNewDelivery} />, container)
}