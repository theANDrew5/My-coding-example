import React from "react";
import ReactDOM from "react-dom";
import { Select } from "antd";

const { Option } = Select;

export interface IOption {
    label: string;
    optionText: string | null;
    value: any;
}

export interface IDropdownSelectInitData {
    options: IOption[];
    selectedOptions: IOption | IOption[] | null;
    plaseholder: string;
    multiSelect: boolean;
    label?: string;
    onChange: (selctedOptions: any)=>void;
}

export const DropdownSelect = (initdata: IDropdownSelectInitData) => {
    const {options, selectedOptions, plaseholder, multiSelect, label, onChange} = initdata;

    const handleChange = (change: string) => {
        if (Array.isArray(change)) {
            onChange(change.map(c => JSON.parse(c)));
            return;
        }
        onChange(JSON.parse(change));
    }
    return(
        <>
            {
                label != undefined &&
                <label>{label}</label>
            }
            <Select
                mode={multiSelect ? 'multiple' :  undefined}
                allowClear
                style={{ width: '100%' }}
                placeholder={plaseholder}
                optionLabelProp='label'
                showArrow={true}
                defaultValue={selectedOptions != null
                    ? Array.isArray(selectedOptions) ? selectedOptions.map(o => { return { label: o.label, value: JSON.stringify(o.value) } })
                        : {label: selectedOptions.label, value: JSON.stringify(selectedOptions.value)}
                    : undefined}
                onChange={handleChange as any}>
                {
                    options.map(o => <Option 
                        label={o.label}
                        value={JSON.stringify(o.value)}>
                            {o.optionText??o.label}
                        </Option>)
                }
                </Select>
        </>);
}

export const render = (container: HTMLElement, initData: IDropdownSelectInitData) => {
    ReactDOM.render(<DropdownSelect
        options={initData.options}
        selectedOptions={initData.selectedOptions}
        plaseholder={initData.plaseholder}
        multiSelect={initData.multiSelect}
        label={initData.label}
        onChange={initData.onChange}
        />, container)
}