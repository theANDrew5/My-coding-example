import React, { Component, useState, useEffect, useRef } from 'react';
import ReactDOM from 'react-dom';
import { LoadingContentComponent } from '../../components/LoadingContentComponent';
import { IDeliveryWindowSettings } from './models/WindowSettingsModel';

const ShippingWindowSettingsManager = React.lazy(
    () => import(/* webpackChunkName: "ShippingWindowSettingsManager" */ './ShippingWindowSettingsManager')
);

interface ILazyShippingWindowSettingsManager {
  options: IDeliveryWindowSettings
}

class LazyShippingWindowSettingsManager extends Component<ILazyShippingWindowSettingsManager> {    
    render() {
        return (
            <React.Suspense fallback={<LoadingContentComponent />}>
            <ShippingWindowSettingsManager settings={this.props.options} />
            </React.Suspense>
        );
    }
}

export function render(container: HTMLDivElement, settings: IDeliveryWindowSettings) {
    ReactDOM.render(<LazyShippingWindowSettingsManager options={settings}/>, container);
}