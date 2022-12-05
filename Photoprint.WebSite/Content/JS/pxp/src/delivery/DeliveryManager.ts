import { pxpGlobal } from "../globals/pxp";
import { DeliveryTotalPriceController, IDeliveryTotalPriceControllerHandlers, IDeliveryTotalPriceControllerInitData, IDeliveryTotalPriceControllerLocalization } from "./controllers/DeliveryTotalPriceController";
import { DeliveryTypeSelector, IDeliveryTypeSelectorHandlers, IDeliveryTypeSelectorLocalization } from './controllers/DeliveryTypesSelector';
import { IRecipientLocalization, RecipientManager } from "./controllers/RecipientManager";
import { CitySelector, ICitySelectorHandlers, ICitySelectorLocalization } from "./controllers/CitySelector";
import { DeliveryMessageBlockType, DeliveryMessagesManager, MasterDeliveryMessagesManager } from "./controllers/MessagesManager";
import { DeliveryManagerService } from "./_services/DeliveryManagerService";
import { IDeliverySettings } from "./models/WindowSettings";
import { RenderToPage } from "../utils/RenderToPage";

pxpGlobal.utilities.registerLoadingComponent("pxp-delivery",
    () => {
        return RenderToPage.renderAllWithLoad({
            pxp__deliveryManager__baseTemplate: import("./_templates/DeliveryBaseTemplate.html"),

            pxp__deliveryManager__deliveryTypesListTemplate: import("./_templates/deliveryTypes/TypesListTemplate.html"),
            pxp__deliveryManager__selectAddressTemplate: import("./_templates/deliveryTypes/SelectAddressTemplate.html"),
            pxp__deliveryManager__pointTemplate: import("./_templates/deliveryTypes/PointTemplate.html"),
            pxp__deliveryManager__plugins__pickpointDelivery: import("./_templates/deliveryTypes/plugins/PickpointPluginDeliveryTemplate.html"),
            pxp__deliveryManager__plugins__safeRouteDelivery: import("./_templates/deliveryTypes/plugins/SafeRoutePluginDeliveryTemplate.html"),
            pxp__deliveryManager__address: import("./_templates/AddressTemplate.html"),
            pxp__deliveryManager__pointsList:import("./_templates/DeliveryPointsList.html"),
            pxp__deliveryManager__citySelector: import("./_templates/CitySelectorTemplate.html"),
            pxp__deliveryManager__recipient: import("./_templates/RecipientTemplate.html"),
            pxp__deliveryManager__messages: import("./_templates/MessagesManager.html"),
            pxp__deliveryManager__totalPriceBlock: import("./_templates/DeliveryTotalPriceBlock.html"),
            pxp__deliveryManager__pointInfoBlock: import("./_templates/deliveryTypes/point/PointInfoTemplate.html")
        }, {
            pxp__deliveryManager__style: import("./_css/DeliveryManagerStyle.css")
        }).then(() => {
            return DeliveryManagerService.getInitData().then((resolve) => {
                if (resolve?.RedirectUrl) {
                    window.location.assign(resolve!.RedirectUrl);
                }
                var manager = new DeliveryManager(resolve.InstanceGuid, resolve.Settings, resolve.TotalPriceInitData, resolve.Localization);
                (<any>window).pxpDelivery = manager;

                return manager;
            });
        });
    },
    "pxp__deliveryManager__baseTemplate"
);

export class DeliveryInit {
    static render(container: HTMLElement) {
        container.innerHTML = '';
        container.appendChild(document.createRange().createContextualFragment("<pxp-delivery></pxp-delivery>"))
        ko.applyBindings(()=>{}, container);
    }
}

class DeliveryManager {
    citySelector: CitySelector;
    recipent: RecipientManager;
    deliveryTypesSelector: DeliveryTypeSelector;
    masterMessagesManager: MasterDeliveryMessagesManager;
    totalPriceController: DeliveryTotalPriceController;

    messageManager: DeliveryMessagesManager;

    // localization
    readonly loc_pageTitle: string;

    // comp
    isLoading: KnockoutComputed<boolean>;

    constructor(instaceGuid: string, settings: IDeliverySettings, totalPriceInitData: IDeliveryTotalPriceControllerInitData, localization: IDeliveryLocalization) {
        this.masterMessagesManager = new MasterDeliveryMessagesManager();
        this.citySelector = new CitySelector(localization.CitySelectorLocalization, this.masterMessagesManager, settings.SelectorSettings.MapSettings.Data.CountryLimiter);
        this.recipent = new RecipientManager(settings.RecepientSettings, localization.RecipientLocalization, this.masterMessagesManager);
        this.deliveryTypesSelector = new DeliveryTypeSelector(instaceGuid,this.citySelector.selectedCity, 
            this.citySelector.countrySelector.selectedCountry, settings.SelectorSettings, localization.DeliverySelectorLocalization, this.masterMessagesManager);
        this.totalPriceController = new DeliveryTotalPriceController(totalPriceInitData, settings.TotalPriceBlockSettings, localization.TotalPriceControllerLocalization, this.masterMessagesManager);

        this.messageManager = this.masterMessagesManager.getOrCreateManager(DeliveryMessageBlockType.General);

        // localization
        this.loc_pageTitle = localization.PageTitle;
        
        // comp
        this.isLoading = ko.pureComputed(this._isLoadingComp, this);

        // handlers
        this.citySelector.initHandlers(<ICitySelectorHandlers>{
            TypeSelctorIsLoading: this.deliveryTypesSelector.isLoading
        })
        this.deliveryTypesSelector.initHandlers(<IDeliveryTypeSelectorHandlers>{
            recipent_userData: this.recipent.userData,
            recipent_setUserData: this.recipent.setUserData,
            totlaPriceController_refreshPrice: this.totalPriceController.refreshPrice
        });
        this.totalPriceController.initHandlers(<IDeliveryTotalPriceControllerHandlers>{
            recipent_userData: this.recipent.userData,
            deliveryTypesSelector_deliveryPrice: this.deliveryTypesSelector.deliveryPrice,
            deliveryTypesSelector_selectedType: this.deliveryTypesSelector.selectedDeliveryType
        });

        // calls
        this.deliveryTypesSelector.init();
    }

    // comp
    private _isLoadingComp(): boolean {
        return this.totalPriceController.isOrderInCreation();
    }
}

export interface IDeliveryLocalization {
    PageTitle: string;
    DeliverySelectorLocalization: IDeliveryTypeSelectorLocalization;
    CitySelectorLocalization: ICitySelectorLocalization;
    RecipientLocalization: IRecipientLocalization;
    TotalPriceControllerLocalization: IDeliveryTotalPriceControllerLocalization;
}