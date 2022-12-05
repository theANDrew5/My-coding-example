import { BaseDeliveryTypeModel, IAvailableDeliveryTypesDto, IBaseDeliveryTypeLocalization } from '../../_base/BaseDeliveryTypeModel';
import { BaseStandartDeliveryPointItem, IPxpBaseStandartDeliveryPointDto } from './BaseStandartDeliveryPointItem';
import { DeliveryManagerService } from '../../../../_services/DeliveryManagerService';
import { DeliveryDisplayType } from '../../../../controllers/DeliveryTypesSelector';
import { IDeliverySettingsMap } from '../../../WindowSettings';
import { CityInfo } from '../../../city/CityInfo';
import { IMessagesMaster } from '../../../../controllers/MessagesManager';
import { CountryInfo } from "../../../city/CountrySelector";

export abstract class BaseStandartDeliveryTypeModel extends BaseDeliveryTypeModel {

    protected readonly instaceGuid: string;

    protected abstract processDeliveryPointItems(dto: Array<IPxpBaseStandartDeliveryPointDto>): void;
    protected pointsProcessed: boolean;

    abstract type: DeliveryDisplayType;
    abstract template: string;
    abstract allPoints: KnockoutObservableArray<BaseStandartDeliveryPointItem>;
    abstract selectedPoint: KnockoutObservable<BaseStandartDeliveryPointItem | null>;

    private _isDeliveryAddressesUpdate: boolean;
    
    mapSettings: IDeliverySettingsMap;
    searchString: KnockoutObservable<string>;
    
    constructor(
        instaceGuid: string,
        mapSettings: IDeliverySettingsMap,
        dto: IAvailableDeliveryTypesDto, 
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>,
        localization: IBaseStandartDeliveryTypeLocalization,
        master: IMessagesMaster
    ) {
        super(dto, currentCity, currentCountry, localization, master);

        this.instaceGuid = instaceGuid;

        this.updateDeliveryAddresses = this.updateDeliveryAddresses.bind(this);
        this.onSelectUpdate = this.onSelectUpdate.bind(this);

        this._isDeliveryAddressesUpdate = false;
        this.pointsProcessed = false;

        this.mapSettings = mapSettings;
        this.searchString = ko.observable("");
        
        // subscribes
        //this.searchString.subscribe(() => { this.filterPoints(this.availablePoints()); }, this);

        // extends
        this.searchString.extend({ deferred: true });

        //binds
        this.onSelectUpdate = this.onSelectUpdate.bind(this);
    }

    updateDeliveryAddresses<T extends IPxpBaseStandartDeliveryPointDto>(callbackOnParse: (addresses: T[]) => void) {
        const city = this.currentCity();
        if (!this.isSelected() || city == null || this.isLoading() || this._isDeliveryAddressesUpdate) return;

        this.isLoading(true);
        this._isDeliveryAddressesUpdate = true;
        DeliveryManagerService.getDeliveryAddresses<T>(this.instaceGuid, city!, this.type, this.shippingIds).then(
            (response) => {
                callbackOnParse(response);                         
            },
            (reject) => {
                console.log(reject);
            }
        ).then(() => {
            this.isLoading(false);
            this._isDeliveryAddressesUpdate = false;
        });
    }

    onSelectUpdate(): void {
        if (!this.pointsProcessed)
            this.updateDeliveryAddresses(this.processDeliveryPointItems);
    }
}

export interface IBaseStandartDeliveryTypeLocalization extends IBaseDeliveryTypeLocalization { }