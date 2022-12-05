import { Col, Form, Input, Row } from "antd";
import React from "react";
import { AddressInfo, IAddressInfoDTO } from "../../Address/AddressInfo";
import { rs } from "../../globals/pxp";


export interface IAddressComponentData {
    FullAddressMode: boolean;
    InitialAddress: IAddressInfoDTO;
}
export const Address = (initData: IAddressComponentData) => {
    const {FullAddressMode, InitialAddress} = initData;
    const addressModel = new AddressInfo(InitialAddress);
    const {city, street, house, flat, postalCode, latitude, longitude, getAddressLine} = addressModel;

        return (
        <>
            <Form>
                <Row gutter={[16, 8]}>
                    <Col span={8}>
                        <Form.Item
                            label={rs.Shipping.Address.City}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                                <Input value={city?.title}/>
                        </Form.Item>
                    </Col>
                    <Col span={8}>
                        <Form.Item
                            label={rs.Shipping.Address.Region}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                                <Input value={city?.region}/>
                        </Form.Item>
                    </Col>
                    <Col span={8}>
                        <Form.Item
                            label={rs.Shipping.Address.Country}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                                <Input value={city?.country}/>
                        </Form.Item>
                    </Col>
                    {
                        FullAddressMode &&
                        <>
                            <Col span={8}>
                                <Form.Item
                                    label={rs.Shipping.Address.Street}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                        <Input value={street}/>
                                </Form.Item>
                            </Col>
                            <Col span={8}>
                                <Form.Item
                                    label={rs.Shipping.Address.House}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                        <Input value={house}/>
                                </Form.Item>
                            </Col>
                            <Col span={8}>
                                <Form.Item
                                    label={rs.Shipping.Address.Flat}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                        <Input value={flat}/>
                                </Form.Item>
                            </Col>
                        </>
                    }
                    <Col span={8}>
                        <Form.Item
                            label={rs.Shipping.Address.Latitude}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                                <Input value={latitude}/>
                        </Form.Item>
                    </Col>
                    <Col span={8}>
                        <Form.Item
                            label={rs.Shipping.Address.Longitude}
                            labelCol={{ span: 24}}
                            wrapperCol={{span: 24}}>
                                <Input value={longitude}/>
                        </Form.Item>
                    </Col>
                    {
                        FullAddressMode &&
                        <>
                            <Col span={8}>
                                <Form.Item
                                    label={rs.Shipping.Address.PostalCode}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                        <Input value={postalCode}/>
                                </Form.Item>
                            </Col>
                            <Col span={24}>
                                <Form.Item
                                    label={rs.Shipping.Address.Title}
                                    labelCol={{ span: 24}}
                                    wrapperCol={{span: 24}}>
                                        <Input value={getAddressLine()}/>
                                </Form.Item>                               
                            </Col>
                        </>
                    }
                </Row>
            </Form>
        </>
    );
}