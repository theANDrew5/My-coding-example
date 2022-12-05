import React, { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { rs } from "../../globals/pxp";
import { resultModel as rm } from "./ShippingWindowSettingsManager";
import { IDeliveryWindowAddressSelectSettings } from "./models/WindowSettingsModel";
import { SwitchCheckbox } from "../../components/SwitchCheckbox";
import { action } from "mobx";

const SWCourierSettings = observer(() => {
    const [usePostcode, setUsePostcode] = useState<boolean>(false);
    const [useAddressLines, setUseAddressLines] = useState<boolean>(false);
    
    // init effect
    useEffect(() => {
        if (rm.addressSelectSettings == null) return;
        setUsePostcode(rm.addressSelectSettings.UsePostcode);
        setUseAddressLines(rm.addressSelectSettings.UseAddressLines);
    }, []);

    useEffect(action(() => { 
        rm.addressSelectSettings = {
            UsePostcode: usePostcode,
            UseAddressLines: useAddressLines
        } as IDeliveryWindowAddressSelectSettings
    }));

    return (
    <>
        <h2>{rs.Shipping.WindowSettings.AddressSelectSettings.AddressSelectDeliverySettings }</h2>
        <fieldset>
            <ol>
                <li className="checkbox">
                    <SwitchCheckbox
                            title={rs.Shipping.WindowSettings.AddressSelectSettings.UsePostcode}
                            checked={usePostcode}  
                            onChange={  (v) => {  setUsePostcode(v); } } 
                            />
                </li>
            </ol>
            <ol>
                <li className="checkbox">
                    <SwitchCheckbox 
                            title={rs.Shipping.WindowSettings.AddressSelectSettings.UseAddressLines}
                            checked={useAddressLines} 
                            onChange={ (v) => { setUseAddressLines(v) } }
                            />
                </li>
            </ol>
        </fieldset>
    </>
    )
})
export default SWCourierSettings;