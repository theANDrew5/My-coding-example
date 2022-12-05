import { pxpGlobal, rs } from '../../../../globals/pxp';
import React from 'react';
import { Col, Row, Skeleton } from 'antd';
import Title from 'antd/lib/typography/Title';






export const SkeletonShipping = (props: { isClientDelivery: boolean }) => {
    const { isClientDelivery } = props;
    return (

        <>
            <div className="dl-group">
                <dl style={{ height: 33 }}>
                    <dt><Skeleton title={false} paragraph={{ style: { marginBottom: 0 }, rows: 1, width: ["80%"], className: "text-skeleton" }} active /></dt>
                    <dd >
                        <Skeleton title={false} paragraph={{ style: { marginBottom: 0 }, rows: 1, width: ["40%"], className: "text-skeleton" }} active />
                    </dd>
                </dl>
                <dl style={{ height: 33 }}>
                    <dt><Skeleton title={false} paragraph={{ style: { marginBottom: 0 }, rows: 1, width: ["80%"], className: "text-skeleton" }} active /></dt>
                    <dd>
                        <Skeleton title={false} paragraph={{ style: { marginBottom: 0 }, rows: 1, width: ["40%"], className: "text-skeleton" }} active />
                    </dd>
                </dl>


            </div>
            <h3>
                {rs.Order.Info.SelectShippingAddressLabel}
            </h3>
            {/* <Skeleton title={{ width: 200, style: { height: 20 } }} paragraph={{ rows: 0 }} active /> */}
            <Row gutter={[8, 8]}>
                <Col span={8}>
                    <Title level={5}>{rs.Shipping.Address.Country}</Title>
                    <Skeleton.Input style={{ width: 200 }} active />
                </Col>
                <Col span={8}>
                    <Title level={5}>{rs.Shipping.Address.Region}</Title>
                    <Skeleton.Input style={{ width: 200 }} active />
                </Col>
                <Col span={8}>
                    <Title level={5}>{rs.Shipping.Address.City}</Title>
                    <Skeleton.Input style={{ width: 200 }} active />
                </Col>
                {isClientDelivery ?
                    <>
                        <Col span={8}>
                            <Title level={5}>{rs.Shipping.Address.Street}</Title>
                            <Skeleton.Input style={{ width: 200 }} active />
                        </Col>
                        <Col span={8}>
                            <Title level={5}>{rs.Shipping.Address.House}</Title>
                            <Skeleton.Input style={{ width: 200 }} active />
                        </Col>
                        <Col span={8}>
                            <Title level={5}>{rs.Shipping.Address.Room}</Title>
                            <Skeleton.Input style={{ width: 200 }} active />
                        </Col>
                    </>
                    : <Col span={24}>
                        <Title level={5}>{rs.Shipping.Address.Street}</Title>
                        <Skeleton.Input style={{ width: 650 }} active />
                    </Col>
                }
            </Row>



        </>
    );
}