import { CityInfo } from "../../../../city/CityInfo";
import { IDeliverySettingsMap } from "../../../../WindowSettings";
import { IPxpLatLng } from "../../../../_maps/_baseMapManager";
import { BaseDeliveryMap } from "../../_baseStandartType/viewModels/BaseDeliveryMap";
import { IAddressMap, IPointData } from "../../../../address/BaseAddressModel";
import { CountryInfo } from "../../../../city/CountrySelector";
import { AddressService } from "../../../../../_services/AddressService";


export class AddressSelectDeliveryPointMap extends BaseDeliveryMap implements IAddressMap {
    private _handlers: ICourierMapHandlers;

    createPointOnMap: ((point: IPointData | null, forse?: boolean) => void) | null;
    setAddressByName: ((addressName: string | null) => void) | null

    mapPointCoords: KnockoutObservable<IPxpLatLng | null>;

    // comp
    isPointSelected: KnockoutComputed<boolean>;

    constructor( 
        mapSettings: IDeliverySettingsMap,
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>,
        handlers: ICourierMapHandlers
    ) {
        super(mapSettings, currentCity, currentCountry);
        this.setNewPoint = this.setNewPoint.bind(this);
        this.afterRenderMap = this.afterRenderMap.bind(this);

        this._handlers = handlers;

        this.createPointOnMap = null;
        this.setAddressByName = null;

        this.mapPointCoords = ko.observable(null);

        // comp
        this.isPointSelected = ko.pureComputed(this._isPointSelectedComp, this);
    }

    afterRenderMap() {
        var point = this._handlers.selectedAddressPointData();
        if (point != null) this.createPointOnMap?.(point, true);
    }

    // нужно вызывать в случае, если поиск информации по точке пойдет на бекенд
    setNewPointCoords(data: IPxpLatLng | null) {
        if (data==null) return;
        AddressService.findStreetAddressDataByCoords(data)
            .then((point) => {this.setNewPoint(point)});
    }

    // нужно вызывать, если у вас уже есть вся информация по точке с карты
    setNewPoint(data: IPointData | null) {
        this._handlers.setNewAddressByReadyPoint(data);
    }

    // comp
    private _isPointSelectedComp(): boolean {
        return this.mapPointCoords() != null
    }
}

export interface ICourierMapHandlers {
    setNewAddressByReadyPoint(addressPoint: IPointData | null): void;
    selectedAddressPointData: KnockoutComputed<IPointData | null>;
}