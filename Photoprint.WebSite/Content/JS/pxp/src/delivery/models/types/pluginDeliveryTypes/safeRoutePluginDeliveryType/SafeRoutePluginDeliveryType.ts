import { ISafeRoutePluginDeliveryPointItemLocalization, SafeRoutePluginDeliveryPointItem } from "./SafeRoutePluginDeliveryPointItem";
import { BasePluginDeliveryTypeModel, IBasePluginDeliveryTypeLocalization } from "../_base/BasePluginDeliveryTypeModel";
import { IPxpBaseDeliveryPointDto, IPxpDeliveryAddress, PxpDeliveryPointProviderType, PxpDistributionPointType } from "../../_base/BaseDeliveryPointItem";
import { DeliveryManagerService } from "../../../../_services/DeliveryManagerService";
import { DeliveryDisplayType } from "../../../../controllers/DeliveryTypesSelector";
import { IAvailableDeliveryTypesDto } from "../../_base/BaseDeliveryTypeModel";
import { IBasePluginShippingData } from "../_base/BasePluginDeliveryPointItem";
import { IWeakUserData } from "../../../../controllers/RecipientManager";
import { CityInfo } from "../../../city/CityInfo";
import { Url } from "../../../../../utils/Url";
import { IMessagesMaster } from "../../../../controllers/MessagesManager";
import { CountryInfo } from "../../../city/CountrySelector";

export class SafeRoutePluginDeliveryType extends BasePluginDeliveryTypeModel {
    private readonly __saferouteBundleUrl: string = "https://widgets.saferoute.ru/cart/api.js";
    private readonly __saferouteContainerId: string = "pxp__saferoute_plugin_container";
    
    private _handlers: ISafeRoutePluginDeliveryTypeHandlers;
    type: DeliveryDisplayType = DeliveryDisplayType.DeliveryPlaginSafeRoute; // from base
    template: string = "pxp__deliveryManager__plugins__safeRouteDelivery"; // from base
    
    products: IProductMinSaferoute[]; 
    totalWeight: number;
    isShippingPricePaidSeparately: boolean;
    widget: any | null;
    
    dataFromWidget: KnockoutObservable<any | null>;
    scriptLoaded: KnockoutObservable<boolean>;
    selectedPoint: KnockoutObservable<SafeRoutePluginDeliveryPointItem | null>;

    // localization
    readonly loc_safeRouteItemLocalization: ISafeRoutePluginDeliveryPointItemLocalization;

    constructor(
        dto: IAvailableDeliveryTypesDto, 
        currentCity: KnockoutObservable<CityInfo | null>, 
        currentCountry: KnockoutObservable<CountryInfo>,        
        localization: ISafeRoutePluginDeliveryTypeLocalization,
        handlers: ISafeRoutePluginDeliveryTypeHandlers,
        master: IMessagesMaster
    ) {
        super(dto, currentCity, currentCountry, localization, master);
        this.afterRender = this.afterRender.bind(this);
        this.startWidget = this.startWidget.bind(this);
        this.setWidgetDataInItem = this.setWidgetDataInItem.bind(this);

        this._handlers = handlers;

        this.products = [];
        this.totalWeight = 0;
        this.isShippingPricePaidSeparately = false;

        this.dataFromWidget = ko.observable(null);
        this.scriptLoaded = ko.observable(false);
        this.selectedPoint = ko.observable(null);

        // localization
        this.loc_safeRouteItemLocalization = localization.SafeRoutePluginDeliveryPointItemLocalization;
    }

    onSelectUpdate(): void {
        if (this.isLoading()) return;
        this.isLoading(true);  // этот true закроется в afterRender

        if (!this.isFullyInited()) {
            Promise.all([
                DeliveryManagerService.getPluginShippingData<ISafeRoutePluginShippingData>(this.type).then((data) => {
                    this.products = data.Products;
                    this.totalWeight = data.TotalWeight;
                    this.isShippingPricePaidSeparately = data.IsShippingPricePaidSeparately;
                }),
            ]).then(() => {
                this.isFullyInited(true);
            })
        };
    }
    
    afterRender() {
        if (this.scriptLoaded()) {
            this.startWidget();
            this.isLoading(false);
        } else {
            Url.loadScriptAsync(this.__saferouteBundleUrl).then(() => {
                this.scriptLoaded(true);

                this.startWidget();
                this.isLoading(false);
            })
        }
    }

    startWidget() {
        this.selectedPoint(null);
        var userData = this._handlers.recipent_userData();
        var userFullName = userData == null ? '' : userData.LastName + " " + userData.FirstName;

        var widget: any = new (<any>window.SafeRouteCartWidget)(
            this.__saferouteContainerId, 
            {
                apiScript: "/api/shippings/ddeliveryV2?postalId=" + this.shippingId,
                products: this.products,
                weight: this.totalWeight,
                regionName: this.currentCity()?.title ?? '',
                userFullName: userFullName,
                userEmail: userData.Email,
                userPhone: userData.Phone
            }
        );

        widget.on("done", (response: { id: number, confirmed: boolean }) => {
            this.setWidgetDataInItem();
        });
        widget.on("error", (errors: string[]) => {
            errors.forEach((x) => { console.log(x); })
        });
        widget.on("change", (data: any) => {
            this.dataFromWidget(data);
        });
    }

    setWidgetDataInItem() {
        this.selectedPoint(null);

        var data = this.dataFromWidget();
        if (data == null) return;

        var deliveryType = data.delivery.type;
        var isMyDelivery = data.delivery.is_my_delivery;
        var deliveryCompanyId: number;
        var pointId: number | null = null;
        var priceDelivery: number = data.delivery.totalPrice;
        var totalPrice: number = data.delivery.totalPrice;
        var priceSorting: number | null = null;
        let addressLine: string;
        let description: string;
                
        if (isMyDelivery) {
            deliveryCompanyId = parseInt((<string>data.delivery.deliveryCompanyId).replace('my_', ''));
        } else {                
            deliveryCompanyId = data.delivery.deliveryCompanyId;
        }

        // ПВЗ
        if (deliveryType === 1) {

            if (isMyDelivery) {
                pointId = parseInt((<string>data.delivery.point.id).replace('my_', ''));
            } else {                
                pointId = data.delivery.point.id;
            }
            addressLine = data.delivery.point.address;
            description = data.delivery.point.description;
        } else { // Курьер/почта
            addressLine = `${data.contacts.address.street}, ${data.contacts.address.building}, ${data.contacts.address.apartment}`;
            description = '';
            priceSorting = data.delivery.price_sorting;
        }
        let nameSplit = ((data.city.name) as string).split(', ', 2);
        var dataToCreate: IPxpBaseDeliveryPointDto = {
            ProviderType: PxpDeliveryPointProviderType.DDeliveryV2,
            PointType: PxpDistributionPointType.DDelivery,
            Title: '',
            ShippingTitle: null,
            ShippingDescription: null,
            Address: <IPxpDeliveryAddress>{
                ShippingId: this.shippingId,
                Country: nameSplit[0],//data.city.countryIsoCode
                Region: data.city.region,
                City: nameSplit[1],
                Street: data.contacts.address.street,
                House: data.contacts.address.building,
                Flat: data.contacts.address.apartment,
                AddressLine: addressLine,
                PostalCode: data.contacts.address.zipCode,
                Description: description,
                DeliveryProperties: {
                ddeliveryV2AddressInfo: {
                        PriceCalcResult: {
                            PriceDelivery: priceDelivery,
                            PriceSorting: priceSorting,
                            TotalPrice: totalPrice
                        },
                        DeliveryType: deliveryType,
                        PointId: pointId,
                        DeliveryCompanyId: deliveryCompanyId,
                        CityToId: data.city.id,
                        StreetTo: data.contacts.address.street,
                        HouseTo: data.contacts.address.building,
                        FlatTo: data.contacts.address.apartment,
                        CityToFias: data.city.fias,
                        CityToKladr: data.city.kladr
                    }
                }
            },
            /*SF_Description: description*/
        };
        this.selectedPoint(new SafeRoutePluginDeliveryPointItem(dataToCreate, this.loc_safeRouteItemLocalization));

        this._handlers.recipent_setUserData({
            Phone: data.contacts.phone, 
            OrderCommentary: data.comment,
            Email: data.contacts.email,
        });
    }

    //from base
    isCreateOrderEnabled(): boolean {
        return this.selectedPoint() != null;
    }
}

export interface ISafeRoutePluginShippingData extends IBasePluginShippingData {
    Products: IProductMinSaferoute[];
    TotalWeight: number;
    IsShippingPricePaidSeparately: boolean;
}

interface IProductMinSaferoute {
    name: string;
    count: number;
    price: number;
}

export interface ISafeRoutePluginDeliveryTypeLocalization extends IBasePluginDeliveryTypeLocalization { 
    SafeRoutePluginDeliveryPointItemLocalization: ISafeRoutePluginDeliveryPointItemLocalization;
}

export interface ISafeRoutePluginDeliveryTypeHandlers {
    recipent_userData: KnockoutComputed<IWeakUserData>;
    recipent_setUserData: (data: IWeakUserData) => void;
}