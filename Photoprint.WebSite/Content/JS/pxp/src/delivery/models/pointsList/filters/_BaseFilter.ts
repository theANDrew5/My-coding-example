import { PointDeliveryPointItem, PointFilterReason } from "../../types/standartDeliveryType/pointDeliveryType/PointDeliveryPointItem";
import { PointsDeliveryPointsMap } from "../../types/standartDeliveryType/pointDeliveryType/viewModels/PointsDeliveryPointsMap";

export abstract class BaseFilter {
    abstract filterType: PointFilterReason;
    protected availablePoints: KnockoutObservableArray<PointDeliveryPointItem>;
    protected handlers: IBaseFilterHandlers | null;

    constructor(availablePoints: KnockoutObservableArray<PointDeliveryPointItem>) {
        this.availablePoints = availablePoints;
        this.handlers = null;
        //binds
        this.filterPoints = this.filterPoints.bind(this);
    }

    abstract filterPoints(skipCallback: boolean): void;

    initHandlers(handlers: IBaseFilterHandlers) {
        this.handlers = handlers;
    }
    
}
export interface IBaseFilterHandlers {
    map: PointsDeliveryPointsMap | null;
}
