import React, { useLayoutEffect } from 'react';
import SWDefaultSettings from './DefaultSettings';
import SWCourierSettings from './AddressSelectSettings';
import SWPickpointSettings from './PickpointSettings';
import { DeliveryWindowSettings, IDeliveryWindowSettings } from './models/WindowSettingsModel';
import { observer } from 'mobx-react';
import { pxpGlobal, rs } from '../../globals/pxp';
import { ShippingService } from '../../services/ShippingService';
import { message } from 'antd';
import { useForm } from 'antd/lib/form/Form';

interface IShippingWindowSettingsProps {
    settings: IDeliveryWindowSettings | null
}

export var resultModel = new DeliveryWindowSettings();
const ShippingWindowSettings = observer((props: IShippingWindowSettingsProps) => {
    // like constructor
    useLayoutEffect(() => {
        resultModel.init(props.settings);
    }, [])

    const [form] = useForm();
    const onClickSave = () => {
        form.validateFields().then(() => {
            ShippingService.updateShippingSettings(pxpGlobal.frontend?.id!, resultModel)
                .then(() => {
                    message.success({ content: rs.Shipping.WindowSettings.SuccessSaveSettings });
                })
                .catch(() => {
                    message.error({ content: rs.Shipping.WindowSettings.ErrorSaveSettings });
                });
        })
    };
    
    return (
        <>
            <SWDefaultSettings form={form}/>
            <SWPickpointSettings />
            <SWCourierSettings />
            <div className="buttons">
                <button className="button" type="button" onClick={onClickSave}>
                    {rs.General.Save}
                </button>
            </div>
        </>
    );
});
export default ShippingWindowSettings;