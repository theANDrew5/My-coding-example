import { DeliveryDisplayType } from "../../controllers/DeliveryTypesSelector";
import { PointDeliveryPointItem } from "../types/standartDeliveryType/pointDeliveryType/PointDeliveryPointItem";
import { PointsDeliveryPointsMap } from "../types/standartDeliveryType/pointDeliveryType/viewModels/PointsDeliveryPointsMap";

export abstract class BaseMapManager {
    protected static toggleLoadingState(element: HTMLElement): void {
        var cont = element.classList.contains("loading-wheel");

        if (cont) {
            // delete loading
            element.classList.remove("loading-wheel");
            element.style.zIndex = "";
        } else {
            // add loading
            const prevElement = element.firstChild;
            if (prevElement != null) element.removeChild(prevElement);

            element.classList.add("loading-wheel");  
            element.style.height = "100%";
            element.style.zIndex = "100";
        }
    }

    private static _getMapCacheKey(mapManager: PointsDeliveryPointsMap, shippingType: DeliveryDisplayType) {
        return `${mapManager.currentCity()?.title}:${shippingType}`;
    }
    protected static getMapFromCache<T extends ExternalMapObjectType>(mapManager: PointsDeliveryPointsMap, shippingType: DeliveryDisplayType): T | null {
        const key = this._getMapCacheKey(mapManager, shippingType);
        var map = mapManager.cache.get(key);
        if (map == null) return null;

        return map as T;
    }
    protected static setMapToCache<T extends ExternalMapObjectType>(map: T, mapManager: PointsDeliveryPointsMap, shippingType: DeliveryDisplayType): void {
        const key = this._getMapCacheKey(mapManager, shippingType);
        mapManager.cache.set(key, map);
    }

    // данный метод возвращает массив массивов DeliveryPointItem,  
    // где внутренний массив - это точки с одинаковыми координатами
    protected static getPointsGroups(points: PointDeliveryPointItem[]): PointDeliveryPointItem[][] {
        let groupsDict: { [key: string]: PointDeliveryPointItem[] } = {};
        for (const point of points) {
            if (point.isCoordsNull) continue;

            //let key = `${point.shippingId}_${point.latitude}_${point.longitude}`;
            let key = `${point.latitude?.toFixed(4)}_${point.longitude?.toFixed(4)}`;
            if (groupsDict[key] == null) {
                groupsDict[key] = [];
            }
            groupsDict[key].push(point);
        }
        return Object.values(groupsDict);
    } ///0.00003
}

export type ExternalMapObjectType = ymaps.Map | google.maps.Map;

export interface IPxpLatLng {
    lat: string | null;
    lng: string | null;
}