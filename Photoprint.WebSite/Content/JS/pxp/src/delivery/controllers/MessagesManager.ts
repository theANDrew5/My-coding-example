export class MasterDeliveryMessagesManager implements IMessagesMaster {

    protected allMessages: KnockoutObservableArray<DeliveryMessage>;
    private _messageManagers: Array<DeliveryMessagesManager>; 

    constructor() {
        this.allMessages = ko.observableArray([]);

        this._messageManagers = [];

        //binds
        this.addMessage = this.addMessage.bind(this);

        //this.addMessages([
        //        {
        //            text: "Info test",
        //            call: ko.observable(true),
        //            type: DeliveryMessageType.Info,
        //            blockType: DeliveryMessageBlockType.Delivery,
        //        },
        //        {
        //            text: "Warning test",
        //            call: ko.observable(true),
        //            type: DeliveryMessageType.Warning,
        //            blockType: DeliveryMessageBlockType.Recipient,
        //        },
        //        {
        //            text: "Error test",
        //            call: ko.observable(true),
        //            type: DeliveryMessageType.Error,
        //            blockType: DeliveryMessageBlockType.Recipient,
        //        } 
        //]);
    }

    getOrCreateManager(type: DeliveryMessageBlockType): DeliveryMessagesManager {
        var manager = this._messageManagers.find(m=>m.blockType === type);
        if (manager == undefined) {
            this._messageManagers.push( new DeliveryMessagesManager(type, this.allMessages));
            manager = this._messageManagers[this._messageManagers.length-1];
        }
        return manager!;
    }

    addMessage(message: IDeliveryMessageInput) {
        var index = this.allMessages().findIndex(x => ko.unwrap(x.text) == ko.unwrap(message.text));
        if (index > 0) {
            this.allMessages().splice(index, 1);
        }

        this.allMessages.push(new DeliveryMessage(message));

        return this.allMessages()[this.allMessages.length - 1];
    }

    addMessages(messages: IDeliveryMessageInput[]): void {
        messages.forEach(m=>this.addMessage(m));
    }
}

 export class DeliveryMessagesManager {
    blockType: DeliveryMessageBlockType;
    masterMessages: KnockoutObservableArray<DeliveryMessage>;
    // comp
    allMessages: KnockoutComputed<Array<DeliveryMessage>>;

    infoMessages: KnockoutComputed<Array<DeliveryMessage>>;
    infoMessagesExist: KnockoutComputed<boolean>;

    warningsMessages: KnockoutComputed<Array<DeliveryMessage>>;
    warningsMessagesExist: KnockoutComputed<boolean>;

    errorMessages: KnockoutComputed<Array<DeliveryMessage>>;
    errorMessagesExist: KnockoutComputed<boolean>;

    constructor(blockType: DeliveryMessageBlockType, masterMessagesObservable: KnockoutObservableArray<DeliveryMessage>) {
        this.blockType = blockType;
        this.masterMessages = masterMessagesObservable;
        // comp
        this.allMessages = ko.pureComputed(this._allMessagesComp, this);

        this.infoMessages = ko.pureComputed(this._infoMessagesComp, this);
        this.infoMessagesExist = ko.pureComputed(this._infoMessagesExistComp, this);

        this.warningsMessages = ko.pureComputed(this._warningsMessagesComp, this); 
        this.warningsMessagesExist = ko.pureComputed(this._warningsMessagesExistComp, this);

        this.errorMessages = ko.pureComputed(this._errorMessagesComp, this);
        this.errorMessagesExist = ko.pureComputed(this._errorMessagesExistComp, this);
    }

    // comp
    private _allMessagesComp(): Array<DeliveryMessage> {
        return this.masterMessages().filter((x) => { return x.blockType === this.blockType});
    }

    private _infoMessagesComp(): Array<DeliveryMessage> {
        return this.allMessages().filter((x) => { return x.type === DeliveryMessageType.Info });
    }
    private _infoMessagesExistComp(): boolean {
        return this.infoMessages().length > 0;
    }
    private _warningsMessagesComp(): Array<DeliveryMessage> {
        return this.allMessages().filter((x) => { return x.type === DeliveryMessageType.Warning });
    }
    private _warningsMessagesExistComp(): boolean {
        return this.warningsMessages().length > 0;
    }
    private _errorMessagesComp(): Array<DeliveryMessage> {
        return this.allMessages().filter((x) => { return x.type === DeliveryMessageType.Error });
    }
    private _errorMessagesExistComp(): boolean {
        return this.errorMessages().length > 0;
    }

}

class DeliveryMessage {
    text: KnockoutObservable<string>;
    call: KnockoutComputed<boolean>;
    type: DeliveryMessageType;
    blockType: DeliveryMessageBlockType;

    constructor(message: IDeliveryMessageInput) {
        if (typeof message.text == "string")
            this.text = ko.observable(message.text);
        else
            this.text = message.text;
        this.type = message.type;
        this.blockType = message.blockType;

        if (message?.callOnNegative == true) {
            if (message?.isShown != null) {
                this.call = ko.pureComputed(() => {return (!message.call() && message.isShown!())});
            } else {
                this.call = ko.pureComputed(() => {return (!message.call())});
            }
        } else {
            if (message?.isShown != null) {
                this.call = ko.pureComputed(() => {return (message.call() && message.isShown!())});
            } else {
                this.call = ko.pureComputed(() => {return (message.call())});
            }
        }
    }
}

export interface IMessagesMaster {
    getOrCreateManager(type: DeliveryMessageBlockType): DeliveryMessagesManager
    addMessage(message: IDeliveryMessageInput): void;
    addMessages(messages: IDeliveryMessageInput[]): void;
}

export interface IDeliveryMessageResponce {   
    Text: string,
    Type: DeliveryMessageType,
}

export interface IDeliveryMessageInput {   
    text: string | KnockoutObservable<string>,
    call: KnockoutObservable<boolean> | KnockoutComputed<boolean> | KnockoutReadonlyComputed<boolean>,
    type: DeliveryMessageType,
    blockType: DeliveryMessageBlockType,
    callOnNegative?: boolean,
    isShown?: KnockoutObservable<boolean> | KnockoutComputed<boolean> | KnockoutReadonlyComputed<boolean>
}

export enum DeliveryMessageType {
    Info = 0,
    Warning = 1,
    Error = 2
}

export enum DeliveryMessageBlockType {
    General = 0,
    City = 1,
    Delivery=2,
    Address = 3,
    Recipient = 4
}

