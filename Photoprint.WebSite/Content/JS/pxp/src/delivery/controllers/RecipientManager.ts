import { IDeliverySettingsRecepient } from '../models/WindowSettings';
import { DeliveryMessageBlockType, DeliveryMessagesManager, IMessagesMaster } from './MessagesManager';
import { UserService } from '../_services/UserService';

export class RecipientManager {
    isUsedMiddleName: boolean;
    isCyrillic: boolean;
    isUsedAdditionalEmail: boolean;
    isUsedAdditionalPhone: boolean;

    messagesMaster: IMessagesMaster;
    messagesManager: DeliveryMessagesManager;

    isLoading: KnockoutObservable<boolean>;
    canBeOrderByUserCompany: KnockoutObservable<boolean>;
    isOrderFromUserCompany: KnockoutObservable<boolean>;
    firstName: KnockoutObservable<string>;
    lastName: KnockoutObservable<string>;
    middleName: KnockoutObservable<string>;
    email: KnockoutObservable<string>;
    additionalEmail: KnockoutObservable<string>;
    phone: KnockoutObservable<string>;
    additionalPhone: KnockoutObservable<string>;
    comment: KnockoutObservable<string | null>;

    // localization
    readonly loc_recipientTitle: string;
    readonly loc_firstNameLabel: string;
    readonly loc_middleNameLabel: string;
    readonly loc_lastNameLabel: string;
    readonly loc_emailLabel: string;
    readonly loc_additionalEmailLabel: string;
    readonly loc_additionalPhoneLabel: string;
    readonly loc_phoneLabel: string;
    readonly loc_commentLabel: string;
    readonly loc_isOrderFromUserCompanyLabel: string;

    // comp
    userData: KnockoutComputed<IWeakUserData>;

    constructor(
        settings: IDeliverySettingsRecepient,
        localization: IRecipientLocalization,
        master: IMessagesMaster
    ) {
        this.loadUserData = this.loadUserData.bind(this);
        this.setUserData = this.setUserData.bind(this);

        this.isUsedMiddleName = settings.UseMiddleName;
        this.isCyrillic = settings.IsCyrillic;
        this.isUsedAdditionalEmail = settings.UseAdditionalEmail;
        this.isUsedAdditionalPhone = settings.UseAdditionalPhone;

        this.messagesMaster = master;
		this.messagesManager = this.messagesMaster.getOrCreateManager(DeliveryMessageBlockType.Recipient);

        this.isLoading = ko.observable(false);
        
        this.canBeOrderByUserCompany = ko.observable(false);
        this.isOrderFromUserCompany = ko.observable(false); 
        this.firstName = ko.observable("");
        this.lastName = ko.observable("");
        this.middleName = ko.observable("");
        this.email = ko.observable("");
        this.additionalEmail = ko.observable("");
        this.phone = ko.observable("");
        this.additionalPhone = ko.observable("");
        this.comment = ko.observable(null);

        // localization
        this.loc_recipientTitle = localization.RecipientTitle;
        this.loc_firstNameLabel = localization.FirstNameLabel;
        this.loc_middleNameLabel = localization.MiddleNameLabel;
        this.loc_lastNameLabel = localization.LastNameLabel;
        this.loc_emailLabel = localization.EmailLabel;
        this.loc_phoneLabel = localization.PhoneLabel;
        this.loc_commentLabel = localization.CommentLabel;
        this.loc_isOrderFromUserCompanyLabel = localization.IsOrderFromUserCompanyLabel;
        this.loc_additionalEmailLabel = localization.AdditionalEmailLabel;
        this.loc_additionalPhoneLabel = localization.AdditionalPhoneLabel;

        // comp
        this.userData = ko.pureComputed(this._userDataComp, this);

        // extends
        this.userData.extend({ rateLimit: { timeout: 50, method: "notifyWhenChangesStop" } });

        // init
        this.loadUserData();
    }

    loadUserData() {
        if (this.isLoading()) return;
        this.isLoading(true);

        UserService.getUserData().then(
            (response) => {
                this.canBeOrderByUserCompany(response.CanBeOrderByUserCompany);
                this.firstName(response.Recipient.FirstName);
                this.lastName(response.Recipient.LastName);
                this.email(response.Recipient.Email);
                this.phone(response.Recipient.Phone);
                this.middleName(response.Recipient.MiddleName ?? "");
                this.comment(response.Recipient.Comment);
            },
            (reject) => {
                console.log("loading user data error:" + reject);
            }
        ).then(() => {
            this.isLoading(false);
        });
    }

    setUserData(data: IWeakUserData) {
        if (typeof data.FirstName !== "undefined") this.firstName(data.FirstName);
        if (typeof data.LastName !== "undefined") this.lastName(data.LastName);
        if (typeof data.MiddleName !== "undefined") this.middleName(data.MiddleName);
        if (typeof data.Email !== "undefined") this.email(data.Email);
        if (typeof data.Phone !== "undefined") this.phone(data.Phone);
        if (typeof data.OrderCommentary !== "undefined") this.comment(data.OrderCommentary);
    }

    // comp
    private _userDataComp(): IWeakUserData {
        return {
            FirstName: this.firstName(),
            LastName: this.lastName(),
            MiddleName: this.middleName(),
            Email: this.email(),
            Phone: this.phone(),
            OrderCommentary: this.comment() ?? "",
            AdditionalEmail: this.additionalEmail(),
            AdditionalPhone: this.additionalPhone(),
            FromUserCompany: this.isOrderFromUserCompany()
        }
    }
}

export interface IWeakUserData {
    FirstName?: string;
    LastName?: string;
    MiddleName?: string;
    Email?: string;
    Phone?: string;
    OrderCommentary?: string;
    AdditionalEmail?: string;
    AdditionalPhone?: string;
    FromUserCompany?: boolean;
}

export interface IRecipientLocalization {
    RecipientTitle: string;
    FirstNameLabel: string;
    MiddleNameLabel: string;
    LastNameLabel: string;
    EmailLabel: string;
    AdditionalEmailLabel: string;
    AdditionalPhoneLabel: string;
    PhoneLabel: string;
    CommentLabel: string;
    IsOrderFromUserCompanyLabel: string;
} 