import { pxpGlobal, rs } from '../../../../globals/pxp';
import React from 'react';
import { Select } from 'antd';
import Title from 'antd/lib/typography/Title';
import { Shipping } from './SelectShippingModal';
const { Option, OptGroup } = Select;
import groupBy from 'lodash/groupBy'
import { SelectFilter } from '../../../utils/SelectFilter';




interface IShippingSelectProps {
    selectedShipping: Shipping | null;
    isLoading: boolean;
    onChange: (id: number) => void;
    shippingList: Shipping[]

}


export const ShippingSelect = (props: IShippingSelectProps) => {
    const { selectedShipping, isLoading, shippingList } = props;
    const grouppedShippings = groupBy(shippingList, "typeString");
    return (
        <>
            <Title level={5}>{rs.Order.Create.SelectShippingShippingType}</Title>
            <Select
                onChange={props.onChange}
                disabled={isLoading}
                loading={isLoading}
                value={selectedShipping?.id}
                style={{ width: '100%' }}
                filterOption={SelectFilter}
                showSearch
            >
                {
                    grouppedShippings && Object.entries(grouppedShippings).map(([key, value]) =>
                        <OptGroup key={key} label={key}>
                            {
                                value.map(m =>
                                    <Option key={m.id} value={m.id} label={m.title}>
                                        <>
                                            {m.title}
                                        </>
                                    </Option>
                                )
                            }
                        </OptGroup>
                    )
                }
            </Select>
        </>

    );
}