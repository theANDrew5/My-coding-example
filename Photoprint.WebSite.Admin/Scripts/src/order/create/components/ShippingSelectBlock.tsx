import { Empty, Select } from 'antd';
import React, { useEffect, useState } from 'react';
import { rs } from '../../../globals/pxp';
import { Address, SelectShippingModal, Shipping } from '../../../ui/components/Modals/SelectShippingModal/SelectShippingModal';
import { createOrderStore } from '../CreateOrderManager';
import { ShippingStore } from '../Models/ShippingStore';

interface IShippingSelectBlockProps {
    totalPrice: number;
}
export interface IShippingResult {
    adddress: Address;
    shipping: Shipping;

}
export const ShippingSelectBlock = (props: IShippingSelectBlockProps) => {

    const { shoppingCartStore, shippingStore } = createOrderStore
    const [isModalVisible, setIsModalVisible] = useState(false);
    const selectedShippingResultChangeHandle = (shippingResult: IShippingResult) => {
        console.log(`selected shippingResult:`, shippingResult)
        createOrderStore.shippingStore = new ShippingStore(shippingResult);
    }

    return (
        <div id="shippingSelector">
            <h2 id="shipping">
                {rs.Order.Create.SelectShippingTitle}&nbsp;
                <span>(<a onClick={() => { setIsModalVisible(true) }} className="semilink">{rs.Order.Create.SelectShippingTrigger}</a>)</span>
            </h2>

            <div className="dl-group-wrapper">
                <div className="dl-group">
                    {shippingStore != null
                        ?
                        <>
                            <dl>
                                <dt>{rs.Order.Create.SelectedShippingTitle}</dt>
                                <dd>{shippingStore.shipping.title}</dd>
                            </dl>
                            <dl>
                                <dt>{rs.Order.Create.SelectedShippingAddress}</dt>
                                <dd>
                                    {shippingStore.address.id != null
                                        ? (shippingStore.address.address) + "; "
                                        : <>
                                            {shippingStore.address.street != "" && (shippingStore.address.street) + ";"}
                                            {shippingStore.address.house != "" && (shippingStore.address.house) + ";"}
                                            {shippingStore.address.flat != "" && (shippingStore.address.flat) + ";"}
                                        </>
                                    }

                                    {shippingStore.address.city != "" && (shippingStore.address.city) + "; "}
                                    {shippingStore.address.region != "" && shippingStore.address.region + "; "}
                                    {shippingStore.address.country != "" && shippingStore.address.country + "; "}
                                </dd>
                            </dl>
                            {/* <dl>
                                <dt>{rs.Order.Create.SelectedShippingPrice}</dt>
                                <dd><span style={{ color: "red" }}>????</span></dd>
                            </dl> */}
                        </>
                        : <Empty description={rs.Order.Create.ShippingNotSelected} />
                    }
                </div>
            </div>
            <SelectShippingModal
                title={rs.Order.Create.SelectShippingModalTitle}
                onChange={selectedShippingResultChangeHandle}
                onClose={() => { setIsModalVisible(false); }}
                isOpen={isModalVisible}
                totalPrice={props.totalPrice}
            />
        </div>
    );
}