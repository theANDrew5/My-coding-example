import { pxpGlobal } from "../../globals/pxp";
import { DeliveryDisplayType  } from "./DeliveryTypesSelector";
import { DeliveryManagerService } from "../_services/DeliveryManagerService";
import { IDeliverySettingsTotalPriceBlock } from "../models/WindowSettings";
import { DelvieryPriceService } from "../_services/DeliveryPriceService";
import { IWeakUserData, } from "./RecipientManager";
import { DeliveryMessageBlockType, DeliveryMessageType, IMessagesMaster } from "./MessagesManager";
import { BaseDeliveryPointItem, IPxpDeliveryAddress } from "../models/types/_base/BaseDeliveryPointItem";
import { BaseDeliveryTypeModel } from "../models/types/_base/BaseDeliveryTypeModel";

export class DeliveryTotalPriceController {
    private _handlers: IDeliveryTotalPriceControllerHandlers | null = null;
    
    showLinkToBackInShoppingCart: boolean;
    linkToBackInShoppingCart: string;
    readonly messagesMaster: IMessagesMaster;

    shoppingCartPrice: KnockoutObservable<number>;
    isOrderInCreation: KnockoutObservable<boolean>;
    isPriceInUpdate: KnockoutObservable<boolean>;
    orderCreateError: KnockoutObservable<boolean>;
    refershPriceError: KnockoutObservable<boolean>;
    orderCreateErrorMessage:KnockoutObservable<string>

    refreshPriceSubscribtion: KnockoutSubscription | null;
    selectedPointSubscribtion: KnockoutSubscription | null;

    deliveryPrice: KnockoutObservable<number>;
    deliveryCost: KnockoutObservable<number>;
    deliveryDiscount: KnockoutObservable<number>; 
    isDeliveryCostVisible: KnockoutObservable<boolean>;
    isDeliveryDiscountVisible: KnockoutObservable<boolean>;

    readonly goodsDiscount: number;
    readonly goodsDiscountPriceFormatted: string;
    readonly isGoodsDiscountVisible: boolean;

    // localization
    readonly loc_createOrder: string;
    readonly loc_yourOrder: string;
    readonly loc_shoppingCartPrice: string;
    readonly loc_goodsDiscount: string;
    readonly loc_shippingDiscount: string;
    readonly loc_deliveryPrice: string;
    readonly loc_totalPrice: string;
    readonly loc_goToShoppingCart: string;
    readonly loc_cantCreateOrder: string;
    readonly loc_refershPriceError: string;

    // comp
    isCreateOrderButtonEnable: KnockoutComputed<boolean>; 
    totalPrice: KnockoutComputed<number>;
    shoppingCartPriceFormatted: KnockoutComputed<string>;
    totalPriceFormatted: KnockoutComputed<string>;
    deliveryCostFormatted: KnockoutComputed<string>;
    deliveryDiscountPriceFormatted: KnockoutComputed<string>;

    constructor(
        initData: IDeliveryTotalPriceControllerInitData, 
        settings: IDeliverySettingsTotalPriceBlock, 
        localization: IDeliveryTotalPriceControllerLocalization,
        master: IMessagesMaster) {
        //binds
        this.createOrder = this.createOrder.bind(this);
        this.refreshPrice = this.refreshPrice.bind(this);
        this.initHandlers = this.initHandlers.bind(this);
        this._finalState = this._finalState.bind(this);
        this._subscribtionManger = this._subscribtionManger.bind(this);

        this.showLinkToBackInShoppingCart = settings.ShowLinkToBackInShoppingCart;
        this.linkToBackInShoppingCart = settings.LinkToBackInShoppingCart;
        this.messagesMaster = master;

        this.isOrderInCreation = ko.observable(false);
        this.isPriceInUpdate = ko.observable(false);
        this.orderCreateError = ko.observable(false);
        this.refershPriceError = ko.observable(false);
        this.shoppingCartPrice = ko.observable(initData.ShoppingCartPrice);
        this.refreshPriceSubscribtion = null;
        this.selectedPointSubscribtion = null;

        this.isDeliveryCostVisible = ko.observable(false);
        this.isDeliveryDiscountVisible = ko.observable(false);
        this.deliveryPrice = ko.observable(0);
        this.deliveryCost = ko.observable(0);
        this.deliveryDiscount = ko.observable(0);

        this.goodsDiscount = initData.DiscountPrice;
        this.goodsDiscountPriceFormatted = "&#45;" + pxpGlobal.utilities.getPriceString(this.goodsDiscount);
        this.isGoodsDiscountVisible = this.goodsDiscount > 0;

        // localization
        this.loc_createOrder = localization.CreateOrder;
        this.loc_yourOrder = localization.YourOrder;
        this.loc_shoppingCartPrice = localization.ShoppingCartPrice;
        this.loc_goodsDiscount = localization.DiscountPrice;
        this.loc_shippingDiscount = localization.DiscountPrice;
        this.loc_deliveryPrice = localization.DeliveryPrice;
        this.loc_totalPrice = localization.TotalPrice;
        this.loc_goToShoppingCart = localization.GoToShoppingCart;
        this.loc_cantCreateOrder = localization.CantCreateOrder;
        this.loc_refershPriceError = localization.RefershPriceError;

        this.orderCreateErrorMessage = ko.observable(this.loc_cantCreateOrder);

        // comp
        this.isCreateOrderButtonEnable = ko.pureComputed(this._isCreateOrderButtonEnableComp, this);
        this.totalPrice = ko.pureComputed(this._totalPriceComp, this);
        this.shoppingCartPriceFormatted = ko.pureComputed(this._shoppingCartPriceFormattedComp, this);
        this.totalPriceFormatted = ko.pureComputed(this._totalPriceFormattedComp, this);
        this.deliveryCostFormatted = ko.pureComputed(this._shippingCostFormattedComp, this);
        this.deliveryDiscountPriceFormatted = ko.pureComputed(this._shippingDiscountPriceFormattedComp, this);

        //messages
        this.messagesMaster.addMessages([
            {
                text: this.orderCreateErrorMessage,
                call: this.orderCreateError,
                blockType: DeliveryMessageBlockType.Recipient,
                type: DeliveryMessageType.Error
            },
            {
                text: this.loc_refershPriceError,
                call: this.refershPriceError,
                blockType: DeliveryMessageBlockType.Recipient,
                type: DeliveryMessageType.Error
            }
        ]);
    }

    initHandlers(handlers: IDeliveryTotalPriceControllerHandlers) {
        this._handlers = handlers;
        this._handlers.deliveryTypesSelector_selectedType.subscribe(val => {
                this._subscribtionManger(val);
        });
    }

    refreshPrice() {
        if (this.isPriceInUpdate()) return;
        this.isPriceInUpdate(true);

        var selectedType = this._handlers?.deliveryTypesSelector_selectedType();
        if (selectedType == null) {
            this.refershPriceError(true);
            this.isPriceInUpdate(false);
            return;
        }

        var deliveryStates = selectedType.getFinalStateForCalculations();
        if (deliveryStates.length == 0) {
            this.refershPriceError(selectedType.type != DeliveryDisplayType.Pickpoint
                && selectedType.type != DeliveryDisplayType.Office);
            this.isPriceInUpdate(false);
            return;
        }

        DelvieryPriceService.getPrice(deliveryStates)
            .then(result => {
                this.refershPriceError(false);
                selectedType!.processPriceResult(result);
            })
            .catch(() => {
                this.refershPriceError(selectedType!.type != DeliveryDisplayType.Pickpoint
                && selectedType!.type != DeliveryDisplayType.Office);
                selectedType!.processPriceResult(null);
            })
            .finally(() => {
                this.isPriceInUpdate(false);
            });
    }

    createOrder() {
        var finalState = this._finalState();
        if (finalState == null) return;

        if (this.isOrderInCreation()) return;
        this.isOrderInCreation(true);

        DeliveryManagerService.createOrder(finalState).then(
            (response) => { 
                if (response.IsSuccessful) {
                    this.orderCreateError(false);
                    (<any>window.location) = response.RedirectUrl;
                    return;
                } else {
                    this.orderCreateErrorMessage(response.Message.Text);
                    this.orderCreateError(true);
                }
            },
            () => {
                this.orderCreateErrorMessage(this.loc_cantCreateOrder);
                this.orderCreateError(true);
            }
        ).finally(() => {
            DeliveryManagerService.storeShippingFinalState(finalState!.ShippingData);
            this.isOrderInCreation(false);
        })
    }

    // comp
    private _isCreateOrderButtonEnableComp(): boolean {
        if (this.isOrderInCreation()) return false;

        var selectedType = this._handlers?.deliveryTypesSelector_selectedType();
        if (selectedType == null) return false;
        
        return selectedType.isCreateOrderEnabled();
    }
    private _totalPriceComp(): number {
        var totalPrice = this.shoppingCartPrice() + this.deliveryPrice() - this.goodsDiscount;
        return totalPrice;
    }
    private _shoppingCartPriceFormattedComp(): string {
        return pxpGlobal.utilities.getPriceString(this.shoppingCartPrice());
    }
    private _shippingDiscountPriceFormattedComp(): string {
        return "&#45;" + pxpGlobal.utilities.getPriceString(this.deliveryDiscount());
    }
    private _shippingCostFormattedComp(): string {
        return pxpGlobal.utilities.getPriceString(this.deliveryCost());
    }
    private _totalPriceFormattedComp(): string {
        return pxpGlobal.utilities.getPriceString(this.totalPrice());
    }
    private _finalState(): IDeliveryFinalState | null {
        var userData = this._handlers?.recipent_userData();
        var selectedType = this._handlers?.deliveryTypesSelector_selectedType();
        var deliveryState = selectedType?.getFinalStateForCreateOrder();
        if (deliveryState == null || userData == null) return null;
        var finalModel: IDeliveryFinalState = {
            ShippingData: deliveryState,
            UserData: userData
        }
        return finalModel;
    }

    private _subscribtionManger(deliveryType: BaseDeliveryTypeModel | null) {
        if (this.refreshPriceSubscribtion!=null)
            this.refreshPriceSubscribtion.dispose();
        if (this.selectedPointSubscribtion!=null)
            this.selectedPointSubscribtion.dispose();
        this.refershPriceError(false);
        if(deliveryType == null)
            return;
        deliveryType.readyForCalculations.extend({ notify: 'always' });
        this.refreshPriceSubscribtion = deliveryType.readyForCalculations.subscribe((val) => {
            if (val) this.refreshPrice();
        }, this);
        this.selectedPointSubscribtion = deliveryType.selectedPoint.subscribe(this._selectedPointSubscriber, this);
    }

    private _selectedPointSubscriber(point: BaseDeliveryPointItem | null) {
        if (point == null || !point.calculationWasCorrect()) {
            this.isDeliveryCostVisible(false);
            this.isDeliveryDiscountVisible(false);
            this.deliveryPrice(0);
            this.deliveryCost(0);
            this.deliveryDiscount(0);
            return;
        }
        const price = point.deliveryPrice();
        const cost = point.deliveryCost();
        const discount = point.deliveryDiscount();

        this.isDeliveryCostVisible(true);
        this.isDeliveryDiscountVisible(discount > 0);
        this.deliveryPrice(price);
        this.deliveryCost(cost);
        this.deliveryDiscount(discount);
    }
}

export interface IDeliveryTotalPriceControllerInitData {
    ShoppingCartPrice: number;
    DiscountPrice: number;
}

export interface IDeliveryTotalPriceControllerLocalization {
    CreateOrder: string;
    YourOrder: string;
    ShoppingCartPrice: string;
    DiscountPrice: string;
    DeliveryPrice: string;
    TotalPrice: string;
    GoToShoppingCart: string;
    CantCreateOrder: string;
    RefershPriceError: string;
}
export interface IDeliveryTotalPriceControllerHandlers {
    recipent_userData: KnockoutComputed<IWeakUserData>;
    deliveryTypesSelector_deliveryPrice: KnockoutComputed<number>;
    deliveryTypesSelector_selectedType: KnockoutComputed<BaseDeliveryTypeModel>;
}

export interface IDeliveryFinalState {
    ShippingData: IPxpDeliveryAddress;
    UserData: IWeakUserData;
}