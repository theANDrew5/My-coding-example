import { makeObservable, observable } from 'mobx';
import { Address, Shipping } from '../../../ui/components/Modals/SelectShippingModal/SelectShippingModal';
import { IShippingResult } from '../components/ShippingSelectBlock';

export class ShippingStore {
    address: Address;
    shipping: Shipping;
    
    constructor(shippingResult:IShippingResult) {
        makeObservable(this, {
            address:observable,
            shipping:observable,
           
        })
        this.address = shippingResult.adddress;
        this.shipping = shippingResult.shipping;
        
    }
}