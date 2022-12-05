import React, { useEffect } from "react";
import { useState } from "react";
import { resultModel as rm } from "./ShippingWindowSettingsManager";
import { IDeliveryWindowPickpointsSettings } from "./models/WindowSettingsModel";
import { SwitchCheckbox } from "../../components/SwitchCheckbox";
import { rs } from "../../globals/pxp";
import { observer } from "mobx-react";
import { action } from "mobx";

const SWPickpointSettings = observer(() => {
    const [isOfficesInAnotherBlock, setIsOfficesInAnotherBlock] = useState<boolean>(false);
    const [isFilterProviderTypeEnabled, setIsFilterProviderTypeEnabled] = useState<boolean>(false);
    const [isSerchStringEnabled, setIsSerchStringEnabled] = useState<boolean>(false);

    // like constructor
    useEffect(() => {
        if (rm.pickpointsSettings == null) return;
        setIsOfficesInAnotherBlock(rm.pickpointsSettings.IsOfficesInAnotherBlock);
        setIsFilterProviderTypeEnabled(rm.pickpointsSettings.IsFilterProviderTypeEnabled);
        setIsSerchStringEnabled(rm.pickpointsSettings.IsSerchStringEnabled);
    }, []);

    useEffect(action(() => {
        rm.pickpointsSettings = {
            IsOfficesInAnotherBlock: isOfficesInAnotherBlock,
            IsFilterProviderTypeEnabled: isFilterProviderTypeEnabled,
            IsSerchStringEnabled: isSerchStringEnabled,
        } as IDeliveryWindowPickpointsSettings
    }));

    return (
        <>
            <h2>{rs.Shipping.WindowSettings.PickpointSettings.PickupPointSettings}</h2>
            <fieldset>
                <ol>
                    <li className="checkbox">
                        <SwitchCheckbox
                            title={rs.Shipping.WindowSettings.PickpointSettings.IsOfficesInAnotherBlock}
                            checked={isOfficesInAnotherBlock}
                            onChange={(v) => { setIsOfficesInAnotherBlock(v) }}
                        />
                    </li>
                    <li className="checkbox">
                        <SwitchCheckbox
                            title={rs.Shipping.WindowSettings.PickpointSettings.IsFilterProviderTypeEnabled}
                            checked={isFilterProviderTypeEnabled}
                            onChange={(v) => { setIsFilterProviderTypeEnabled(v) }}
                        />
                    </li>
                    <li className="checkbox">
                        <SwitchCheckbox
                            title={rs.Shipping.WindowSettings.PickpointSettings.IsSerchStringEnabled}
                            checked={isSerchStringEnabled}
                            onChange={(v) => { setIsSerchStringEnabled(v) }}
                        />
                    </li>                  
                </ol>
            </fieldset>
        </>
    )
})
export default SWPickpointSettings;
