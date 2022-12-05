import { DeliveryUtils } from "../../DeliveryUtils";
import { PointDeliveryPointItem, PointFilterReason } from "../../types/standartDeliveryType/pointDeliveryType/PointDeliveryPointItem";
import { BaseFilter, IBaseFilterHandlers} from "./_BaseFilter";


export class BoundsFilter extends BaseFilter{
    filterType: PointFilterReason = PointFilterReason.Bounds; //from base
    protected filterByBounds: KnockoutObservable<boolean>;
    constructor(availablePoints: KnockoutObservableArray<PointDeliveryPointItem>,
        filterByBounds: KnockoutObservable<boolean>) {
        super(availablePoints);
        this.handlers = null;
        this.filterByBounds = filterByBounds;
        //subs
        this.filterByBounds.subscribe(() => { this.filterPoints(); }, this);
    }
    //form base
    initHandlers(handlers: IBaseFilterHandlers) {
        this.handlers = handlers;
        this.handlers.map?.currentMapVisibleCoordsBounds.subscribe(() => { this.filterPoints(); }, this);
    }
    filterPoints() {
        if (!this.filterByBounds()) {
            this.availablePoints().forEach(p => {
                p.setFilter(true, this.filterType);
            });
        } else {
            var mapBounds =  this.handlers?.map?.currentMapVisibleCoordsBounds();
            if (mapBounds != null) {
                this.availablePoints().forEach((p) => {
                    var resVal: boolean;
                    if (mapBounds == null || p.isCoordsNull || p.coords == null) resVal = false;
                    else resVal = DeliveryUtils.isPointInBounds(mapBounds, p.coords);
                    p.setFilter(resVal, this.filterType);
                });
            }
        }
        this.availablePoints.notifySubscribers();
    }
}
