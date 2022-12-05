import { PointDeliveryPointItem, PointFilterReason } from "../../types/standartDeliveryType/pointDeliveryType/PointDeliveryPointItem";
import { BaseFilter, IBaseFilterHandlers} from "./_BaseFilter";


export class ShippingFilter extends BaseFilter{
    filterType: PointFilterReason = PointFilterReason.Shipping;
    protected selectedDistributionPointFilter: KnockoutObservable<number>;

    constructor(
        availablePoints: KnockoutObservableArray<PointDeliveryPointItem>,
        filter: KnockoutObservable<number>) {
        super(availablePoints);

        this.selectedDistributionPointFilter = filter;

        //subs
        this.selectedDistributionPointFilter.subscribe(() => { this.filterPoints()}, this)
    }

    filterPoints(skipCallback: boolean = false) {
        var sFilter = this.selectedDistributionPointFilter();
        if (sFilter < 0) {
            this.availablePoints().forEach(p => {
                p.setFilter(true, this.filterType);
            });
        }
        else {
            this.availablePoints().forEach((p) => {
                var rezVal = p.shippingId === sFilter;
                p.setFilter(rezVal, this.filterType);
            });
        }
        this.handlers?.map?.refreshMap?.();
        this.availablePoints.notifySubscribers();
    }
}

export interface ITypeHandlers extends IBaseFilterHandlers{
}

