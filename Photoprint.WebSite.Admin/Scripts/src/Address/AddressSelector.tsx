import { Col, Row } from 'antd';
import React, { useEffect, useState } from "react";
import ReactDOM from 'react-dom';
import { SwitchCheckbox } from '../components/SwitchCheckbox';
import { rs } from "../globals/pxp";
import { Address } from '../ui/components/Address';
import { AddressInput } from '../ui/components/Inputs/AddressInput';
import { IAddressInfoDTO } from "./AddressInfo";


export interface IAddressSelectorInitData {
    CanEdit: boolean;
    CountryId: number;
    FullAddressMode: boolean;
    UseValidation: boolean;
    InitialAddress: IAddressInfoDTO;
    ContractorAddressAvailable: boolean;
    ContractorAddressNotEqual: boolean;
    InitialContractorAddress: IAddressInfoDTO;
    SetSelectorFinalData: (data?: IAddressSelectorFinalData) => void;
    IsNewDelivery: boolean;
}

interface IAddressSelectorFinalData {
    Address?: IAddressInfoDTO;
    ContractorAddressNotEqual: boolean;
    ContractorAddress?: IAddressInfoDTO;
}

export const AddressSelector = (initData: IAddressSelectorInitData) => {
    const [finalData, setFinalData] = useState<IAddressSelectorFinalData>({} as IAddressSelectorFinalData);

    const {CanEdit, CountryId, FullAddressMode, UseValidation, InitialAddress, ContractorAddressAvailable, 
        ContractorAddressNotEqual, InitialContractorAddress, SetSelectorFinalData, IsNewDelivery} = initData;

    const [showContractorAddress, setShowContractorAddress] = useState(ContractorAddressAvailable && ContractorAddressNotEqual);

    const handleAddressChange = (address?: IAddressInfoDTO) => {
        setFinalData({
            Address : address,
            ContractorAddressNotEqual: showContractorAddress,
            ContractorAddress: finalData?.ContractorAddress
        });
    }

    const handleContractorAddressChange = (address?: IAddressInfoDTO) => {
        setFinalData({
            Address : finalData?.Address,
            ContractorAddressNotEqual: showContractorAddress,
            ContractorAddress: address
        });
    }

    useEffect(() => {
        SetSelectorFinalData(finalData);
    }, [finalData])

    return (
        <>
            <h2>{rs.General.SubMenu.AddressTitle}</h2>
            <fieldset className={'container-wrapper'}>
                <Row gutter={[16, 8]}>
                    <Col span={24}>
                        {
                            CanEdit ? 
                                <AddressInput
                                    CountryId={CountryId}
                                    FullAddressMode={FullAddressMode}
                                    UseValidation={UseValidation}
                                    InitialAddress={InitialAddress}
                                    IsContractor={false}
                                    SetAddressFinalData={handleAddressChange}
                                    IsNewDelivery={IsNewDelivery}                                />
                                :
                                <Address
                                    FullAddressMode={FullAddressMode}
                                    InitialAddress={InitialAddress}/>
                        }
                        
                    </Col>                                   
                    {
                        ContractorAddressAvailable && 
                        <>
                            <Col span={8}>
                                <SwitchCheckbox 
                                    checked={showContractorAddress}
                                    onChange={setShowContractorAddress}
                                    title={rs.Shipping.ContractorAddressNotEqual}
                                    type={'green'}/>
                            </Col>
                            {
                                showContractorAddress &&
                                <Col span={24}>
                                    <AddressInput
                                        CountryId={CountryId}
                                        FullAddressMode={FullAddressMode}
                                        UseValidation={UseValidation}
                                        InitialAddress={InitialContractorAddress}
                                        IsContractor={true}
                                        SetAddressFinalData={handleContractorAddressChange}
                                        IsNewDelivery={IsNewDelivery}/>
                                </Col> 
                            }
                        </>
                    }
                </Row>
            </fieldset>
        </>
    );
}


export const render = (container: HTMLElement, initData: IAddressSelectorInitData) => {
    ReactDOM.render(<AddressSelector
        CanEdit={initData.CanEdit}
        CountryId={initData.CountryId}
        FullAddressMode={initData.FullAddressMode}
        UseValidation={initData.UseValidation}
        InitialAddress={initData.InitialAddress}
        ContractorAddressAvailable={initData.ContractorAddressAvailable}
        ContractorAddressNotEqual={initData.ContractorAddressNotEqual}
        InitialContractorAddress={initData.InitialContractorAddress}
        SetSelectorFinalData={initData.SetSelectorFinalData}
        IsNewDelivery={initData.IsNewDelivery} />, container)
}