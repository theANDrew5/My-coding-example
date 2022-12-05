import React, { useState, useEffect } from "react";
import { rs } from "../../globals/pxp";
import { resultModel as rm } from "./ShippingWindowSettingsManager";
import { DeliveryWindowMapType } from "./models/WindowSettingsModel";
import { SwitchCheckbox } from "../../components/SwitchCheckbox";
import { observer } from "mobx-react";
import { action } from "mobx";
import { CityAddressCountry } from "./models/WindowSettingsModel";
import { FormInstance } from "antd/lib/form/Form";
import { Form } from "antd";
interface ISWDefaultSettings {
    form: FormInstance<any>;
}
const SWDefaultSettings = observer((props: ISWDefaultSettings) => {
    const [isNewDeliveryWindowEnabled, setIsNewDeliveryWindowEnabled] = useState<boolean>(false);
    const [useMiddleName, setUseMiddleName] = useState<boolean>(false);
    const [useShippingFromPreviousOrder, setUseShippingFromPreviousOrder] = useState<boolean>(false);
    const [mapType, setMapType] = useState<DeliveryWindowMapType>(DeliveryWindowMapType.Yandex);
    const [yandexMapKey, setYandexMapKey] = useState<string>('');
    const [yandexMapCountryLimiter, setYandexMapCountryLimiter] = useState<CityAddressCountry>(CityAddressCountry.NoCountry);
    const [googleMapKey, setGoogleMapKey] = useState<string>('');
    const [googleMapCountryLimiter, setGoogleMapCountryLimiter] = useState<CityAddressCountry>(CityAddressCountry.NoCountry);
    const [isAdditionalNotificationEmailEnabled, setIsAdditionalNotificationEmailEnabled] = useState<boolean>(false);
    const [isAdditionalNotificationPhoneNumberEnabled, setIsAdditionalNotificationPhoneNumberEnabled] = useState<boolean>(false);
    
    const mapKeyValidator = () => {
        if (mapType == DeliveryWindowMapType.Yandex) {
            if (isNewDeliveryWindowEnabled && yandexMapKey == '')
                return Promise.reject(new Error(rs.Validators.Required));
        }
        else {
            if (isNewDeliveryWindowEnabled && googleMapKey == '')
                return Promise.reject(new Error(rs.Validators.Required));
        }
        return Promise.resolve();
    }
    // like constructor
    useEffect(() => {
        setIsNewDeliveryWindowEnabled(rm.isNewDeliveryWindowEnabled);
        setUseMiddleName(rm.useMiddleName);
        setUseShippingFromPreviousOrder(rm.useShippingFromPreviousOrder);  
        setIsAdditionalNotificationEmailEnabled(rm.isAdditionalNotificationEmailEnabled);
        setIsAdditionalNotificationPhoneNumberEnabled(rm.isAdditionalNotificationPhoneNumberEnabled);
        if (rm.mapSettings != null) {
            setMapType(rm.mapSettings.MapType);
            switch (rm.mapSettings.MapType) {
                case DeliveryWindowMapType.Yandex: 
                    setYandexMapKey(rm.mapSettings.YandexMapSettings?.ApiKey?? ""); 
                    setYandexMapCountryLimiter(rm.mapSettings.YandexMapSettings?.CountryLimiter?? CityAddressCountry.NoCountry);
                    break;
                case DeliveryWindowMapType.Google: 
                    setGoogleMapKey(rm.mapSettings.GoogleMapSettings?.ApiKey?? ""); 
                    setGoogleMapCountryLimiter(rm.mapSettings.GoogleMapSettings?.CountryLimiter?? CityAddressCountry.NoCountry);
                    break;
            }
        }
    }, []);
    
    useEffect(action(() => {
        rm.isNewDeliveryWindowEnabled = isNewDeliveryWindowEnabled;
        rm.useMiddleName = useMiddleName;
        rm.useShippingFromPreviousOrder = useShippingFromPreviousOrder;
        rm.isAdditionalNotificationEmailEnabled = isAdditionalNotificationEmailEnabled;
        rm.isAdditionalNotificationPhoneNumberEnabled = isAdditionalNotificationPhoneNumberEnabled;
        switch (mapType) {
            case DeliveryWindowMapType.Yandex:
                rm.mapSettings = { MapType: DeliveryWindowMapType.Yandex, YandexMapSettings: {ApiKey: yandexMapKey, CountryLimiter: yandexMapCountryLimiter} };
                break;
            case DeliveryWindowMapType.Google:
                rm.mapSettings = { MapType: DeliveryWindowMapType.Google, GoogleMapSettings: {ApiKey: googleMapKey, CountryLimiter: googleMapCountryLimiter} };
                break;
            default:
                rm.mapSettings = null;
                break;
        }
    }));

    return (
        <>
<h2>{rs.General.BaseInfo}</h2>
<fieldset>
    <ol>
        <li className="checkbox">
            <SwitchCheckbox
                title={rs.Shipping.WindowSettings.EnableNewDeliveryWindow}
                checked={isNewDeliveryWindowEnabled}
                onChange={(v) => { setIsNewDeliveryWindowEnabled(v) }}
            />
        </li>
        <li className="checkbox">
             <SwitchCheckbox
                 title={rs.Shipping.WindowSettings.PickpointSettings.UseAdditionalEmailForNotifications}
                 checked={isAdditionalNotificationEmailEnabled}
                 onChange={(v) => { setIsAdditionalNotificationEmailEnabled(v) }}
            />
        </li>
        <li className="checkbox">
            <SwitchCheckbox
                title={rs.Shipping.WindowSettings.PickpointSettings.UseAdditionalPhoneForNotifications}
                checked={isAdditionalNotificationPhoneNumberEnabled}
                onChange={(v) => { setIsAdditionalNotificationPhoneNumberEnabled(v) }}
            />
        </li>
        <li className="checkbox">
            <SwitchCheckbox
                title={rs.Shipping.WindowSettings.UseShippingFromPreviousOrder}
                checked={useShippingFromPreviousOrder}
                onChange={(v) => { setUseShippingFromPreviousOrder(v) }}
            />
        </li>
        <li className="checkbox">
            <SwitchCheckbox
                title={rs.Shipping.WindowSettings.UseMiddleName}
                checked={useMiddleName}
                onChange={(v) => { setUseMiddleName(v) }}
            />
        </li>
        <li>
            <label>{rs.Shipping.WindowSettings.MapTypeTitle}</label>
            <span className="radio">
                <input id="chkYandexMap" type="radio"
                       checked={mapType == DeliveryWindowMapType.Yandex}
                       onChange={(e) => { setMapType(DeliveryWindowMapType.Yandex) }}
                />
                <label htmlFor="chkYandexMap">{rs.Shipping.WindowSettings.MapYandexTypeTitle}</label>
                {
                                mapType === DeliveryWindowMapType.Yandex
                                    ? 
                                    <>
                                        <Form form={props.form}>
                                            <ol>
                                                <Form.Item
                                                    name='yandexMapKey'
                                                    rules={[{ validator: mapKeyValidator }]}>
                                                    <li className="abc" >
                                                        <div className="abc-a">
                                                            <label>{rs.Shipping.WindowSettings.MapKey}</label>
                                                            <input id='yandexMapKey' className="text" type="text" autoComplete="true"
                                                                value={yandexMapKey}
                                                                onChange={(e) => setYandexMapKey(e.target.value)}
                                                            />
                                                        </div>
                                                    </li>
                                                </Form.Item>
                                                <li className="abc">
                                                    <div className="abc-a">
                                                        <label>{rs.Shipping.WindowSettings.MapSettings.CountryFilter}</label>
                                                        <select style={{fontSize:'1em'}} value={yandexMapCountryLimiter}
                                                            onChange={(e) => setYandexMapCountryLimiter(parseInt(e.target.value))}>
                                                            <option value={CityAddressCountry.NoCountry}>{rs.Shipping.WindowSettings.MapSettings.Country.NoCountry}</option>
                                                            <option value={CityAddressCountry.Russia}>{rs.Shipping.WindowSettings.MapSettings.Country.Russia}</option>
                                                            <option value={CityAddressCountry.Ukraine}>{rs.Shipping.WindowSettings.MapSettings.Country.Ukraine}</option>
                                                            <option value={CityAddressCountry.Belarus}>{rs.Shipping.WindowSettings.MapSettings.Country.Belarus}</option>
                                                            <option value={CityAddressCountry.Kazakhstan}>{rs.Shipping.WindowSettings.MapSettings.Country.Kazakhstan}</option>
                                                            <option value={CityAddressCountry.Bulgaria}>{rs.Shipping.WindowSettings.MapSettings.Country.Bulgaria}</option>
                                                            <option value={CityAddressCountry.USA}>{rs.Shipping.WindowSettings.MapSettings.Country.USA}</option>
                                                        </select>
                                                    </div>
                                                </li>
                                            </ol>
                                        </Form>
                                    </>
                                    : <></>
                            }
                <br/>
                <input id="chkGoogleMap" type="radio"
                       checked={mapType == DeliveryWindowMapType.Google}
                       onChange={(e) => { setMapType(DeliveryWindowMapType.Google) }}
                />
                <label htmlFor="chkGoogleMap">{rs.Shipping.WindowSettings.MapGoogleTypeTitle}</label>
                {
                                mapType === DeliveryWindowMapType.Google
                                    ? 
                                    <>
                                        <Form form={props.form}>
                                            <ol>
                                                <Form.Item
                                                    name='googleMapKey'
                                                    rules={[{ validator: mapKeyValidator }]}>
                                                    <li className="abc">
                                                        <div className="abc-a">
                                                            <label>{rs.Shipping.WindowSettings.MapKey}</label>
                                                            <input className="text" type="text" autoComplete="true"
                                                                value={googleMapKey}
                                                                onChange={(e) => setGoogleMapKey(e.target.value)}
                                                            />
                                                        </div>
                                                    </li>
                                                </Form.Item>
                                                <li className="abc">
                                                    <div className="abc-a">
                                                        <label>{rs.Shipping.WindowSettings.MapSettings.CountryFilter}</label>
                                                        <select style={{ fontSize: '1em' }} value={googleMapCountryLimiter}
                                                            onChange={(e) => setGoogleMapCountryLimiter(parseInt(e.target.value))}>
                                                            <option value={CityAddressCountry.NoCountry}>{rs.Shipping.WindowSettings.MapSettings.Country.NoCountry}</option>
                                                            <option value={CityAddressCountry.Russia}>{rs.Shipping.WindowSettings.MapSettings.Country.Russia}</option>
                                                            <option value={CityAddressCountry.Ukraine}>{rs.Shipping.WindowSettings.MapSettings.Country.Ukraine}</option>
                                                            <option value={CityAddressCountry.Belarus}>{rs.Shipping.WindowSettings.MapSettings.Country.Belarus}</option>
                                                            <option value={CityAddressCountry.Kazakhstan}>{rs.Shipping.WindowSettings.MapSettings.Country.Kazakhstan}</option>
                                                            <option value={CityAddressCountry.Bulgaria}>{rs.Shipping.WindowSettings.MapSettings.Country.Bulgaria}</option>
                                                            <option value={CityAddressCountry.USA}>{rs.Shipping.WindowSettings.MapSettings.Country.USA}</option>
                                                        </select>
                                                    </div>
                                                </li>
                                            </ol>
                                        </Form>
                                    </>
                                    : <></>
                            }
                            <br/>
                        </span>
                    </li>
                </ol>
            </fieldset>
        </>
)
})
export default SWDefaultSettings;
