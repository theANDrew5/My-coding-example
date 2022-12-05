import { IPickPointPluginDeliveryPointItemLocalization, PickPointPluginDeliveryPointItem } from "./PickPointPluginDeliveryPointItem";
import { BasePluginDeliveryTypeModel, IBasePluginDeliveryTypeLocalization } from "../_base/BasePluginDeliveryTypeModel";
import { IPxpBaseDeliveryPointDto, IPxpDeliveryAddress, PxpDeliveryPointProviderType, PxpDistributionPointType } from "../../_base/BaseDeliveryPointItem";
import { DeliveryManagerService } from "../../../../_services/DeliveryManagerService";
import { DeliveryDisplayType } from "../../../../controllers/DeliveryTypesSelector";
import { IAvailableDeliveryTypesDto } from "../../_base/BaseDeliveryTypeModel";
import { IBasePluginShippingData } from "../_base/BasePluginDeliveryPointItem";
import { CityInfo } from "../../../city/CityInfo";
import { Url } from "../../../../../utils/Url";
import { IMessagesMaster } from "../../../../controllers/MessagesManager";
import { CountryInfo } from "../../../city/CountrySelector";

export class PickPointPluginDeliveryType extends BasePluginDeliveryTypeModel {
    private readonly __pickpointBundleUrl: string = "https://pickpoint.ru/select/postamat.js";
    private readonly __pickpointContainerId: string = "pxp__pickpoint_plugin_container";
    private _pickpointPluginController: IPickPointPluginController | null;

    type: DeliveryDisplayType = DeliveryDisplayType.DeliveryPlaginPickPoint; // from base
    template: string = "pxp__deliveryManager__plugins__pickpointDelivery"; // from base
    ikn: string;

    selectedPoint: KnockoutObservable<PickPointPluginDeliveryPointItem | null>; // from base
    
    // localization
    readonly loc_pickPointPluginDeliveryPointItemLocalization: IPickPointPluginDeliveryPointItemLocalization;

    constructor(
        dto: IAvailableDeliveryTypesDto, 
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>,        
        localization: IPickPointPluginDeliveryTypeLocalization,
        master: IMessagesMaster
    ) {
        super(dto, currentCity, currentCountry, localization, master);
        this.afterRender = this.afterRender.bind(this);
        this.onSelectUpdate = this.onSelectUpdate.bind(this);
        this.onPickPointSelected = this.onPickPointSelected.bind(this);

        this._pickpointPluginController = null;

        this.ikn = '';

        this.selectedPoint = ko.observable(null);

        // localization
        this.loc_pickPointPluginDeliveryPointItemLocalization = localization.PickPointPluginDeliveryPointItemLocalization;
    }
    
    onSelectUpdate(): void {
        if (this.isLoading()) return;
        this.isLoading(true); // этот true закроется в afterRender

        if (!this.isFullyInited()) {
            DeliveryManagerService.getPluginShippingData<IPickPointPluginShippingData>(this.type).then((data) => {
                this.ikn = data.IKN;
            });
        }
    }

    afterRender() {
        this.selectedPoint(null);
        Url.loadScriptAsync(this.__pickpointBundleUrl).then(() => {
            this._pickpointPluginController = (<any>window).PickPoint;
            this._pickpointPluginController?.siteShowWithCallback(
                this.__pickpointContainerId,
                this.onPickPointSelected,
                <IRenderPickpointParams>{
                    city: this.currentCity()?.title ?? '',
                    hideCloseButton: true,
                    hideFilterPanel: true,
                    disableFilters: true,
                    noheader: true,
                    ikn: this.ikn
                }
            );            
        }).then(() => { 
            this.isLoading(false);
        });
    }

    onPickPointSelected(result: any) {
        this.selectedPoint(null);
        var item = new PickPointPluginDeliveryPointItem(
            {
                Title: result.nameStrict,
                ShippingTitle: null,
                ShippingDescription: null,
                ProviderType: PxpDeliveryPointProviderType.Pickpoint,
                PointType: PxpDistributionPointType.Pickpoint,
                Address: <IPxpDeliveryAddress>{
                    ShippingId: this.shippingId,
                    Country: result.country,
                    City: result.cityname,
                    AddressLine: result.address,
                    Region: result.region,
                    House: result.house,
                    Latitude: result.latitude.toString(),
                    Longitude: result.longitude.toString(),
                    PostalCode: result.postcode,
                    Description: result.name,
                    DeliveryProperties: {
                        pickpointAddressInfo: {
                            PostamatCode: result.id,
                            PostamatId: result.bdid,
                            CityId: result.cityid
                        }
                    }
                }
            },
            this.loc_pickPointPluginDeliveryPointItemLocalization
        );
        this.selectedPoint(item);
    }

    //from base
    isCreateOrderEnabled(): boolean {
        return true;
    }
}

export interface IPickPointPluginShippingData extends IBasePluginShippingData {
    IKN: string;
}

interface IPickPointPluginController {
    open: (callback: Function, params: IRenderPickpointParams) => boolean
    siteShowWithCallback: (elementId: string, callback: Function, params: IRenderPickpointParams) => boolean
    close: () => void
}

interface IRenderPickpointParams {
    city: string;
    fromcity: string;
    cities: string[];
    pointmode: number;
    wfilters: boolean;
    noselect: boolean;
    hideCloseButton: boolean;
    hideFilterPanel: boolean;
    disableFilters: boolean;
    noheader: boolean;
    ikn: string;
}

export interface IPickPointPluginDeliveryTypeLocalization extends IBasePluginDeliveryTypeLocalization{
    PickPointPluginDeliveryPointItemLocalization: IPickPointPluginDeliveryPointItemLocalization
}