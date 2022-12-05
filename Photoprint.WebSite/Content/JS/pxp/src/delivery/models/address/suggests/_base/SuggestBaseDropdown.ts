
import { IAddressSuggest, SuggestedStreetItem } from "../SuggestedStreetItem";
import { CityInfo } from "../../../city/CityInfo";



export abstract class SuggestBaseDropdown {
    private readonly _handlers: ISuggestDropdownHandlers;
    private readonly _elementClassName: string;
    private _selectedItem: SuggestedStreetItem | null;
    
    isShown: KnockoutObservable<boolean>;
    currentCity: KnockoutObservable<CityInfo | null>;
    list: KnockoutObservableArray<SuggestedStreetItem>;


    protected constructor(currentCity: KnockoutObservable<CityInfo | null>, handlers: ISuggestDropdownHandlers, className: string) {

        this.update = this.update.bind(this);
        this.selectItem = this.selectItem.bind(this);
        this._manageKeyListener = this._manageKeyListener.bind(this);
        this._keyListener = this._keyListener.bind(this);
        this._itemSelector = this._itemSelector.bind(this);

        this._handlers = handlers;
        this._elementClassName = className;
        this._selectedItem = null;

        this.isShown = ko.observable(false);
        this.currentCity = currentCity;
        this.list = ko.observableArray([]);
        
        this.isShown.subscribe((val) => {this._manageKeyListener(val)});
    }

    abstract update(items: SuggestedStreetItem[] | null): void;

    selectItem (item: SuggestedStreetItem | null) {
        if (item == null) return;
        this._selectedItem = item;
        this._handlers.selectItem(item.getSuggest());
        this.isShown(false);
        if (this._handlers.nextFocusElement != null)
            this._handlers.nextFocusElement(true);
    }

    private _manageKeyListener(isShown: boolean) {
        if (isShown) {
            document.addEventListener("keydown", this._keyListener);
        } else {
            document.removeEventListener("keydown", this._keyListener);
        }
    }

    private _keyListener(event: KeyboardEvent) {
        switch (event.keyCode) {
            case 40:
                if (this._selectedItem != null) {
                    const unsetIndex = this.list().indexOf(this._selectedItem);
                    const setIndex = unsetIndex == this.list().length-1 ? 0 : unsetIndex+1;
                    this._selectedItem = this.list()[setIndex];
                    this._itemSelector(setIndex, unsetIndex);
                } else {
                    this._selectedItem = this.list()[0];
                    this._itemSelector(0);
                }
                break;
            case 38:
                if (this._selectedItem != null) {
                    const unsetIndex = this.list().indexOf(this._selectedItem);
                    const setIndex = unsetIndex == 0 ? this.list().length-1 : unsetIndex-1;
                    this._selectedItem = this.list()[setIndex];
                    this._itemSelector(setIndex, unsetIndex);
                } else {
                   this._selectedItem = this.list()[this.list().length-1];
                   this._itemSelector(this.list().length-1);
                }
                break;
            case 13:
                if (this._selectedItem == null) return;
                this.selectItem(this._selectedItem);
                break;
            default:
                return;
        }
    }

    private _itemSelector(setIndex: number, unsetIndex: number | null =null) {
        const container = document.querySelector(this._elementClassName);
        if (container == null) return;
        const dropdown = container.querySelector(".delivery_dropdown_suggest");
        if (dropdown == null) return;
        const dropdownElements = dropdown.querySelectorAll(".delivery_dropdown_suggest__item");
        if (dropdownElements == null) return;
        const tragetElement = dropdownElements[setIndex] as HTMLElement;
        if (tragetElement == null) return;

        tragetElement.classList.add("delivery_dropdown_suggest__item_hover");
        if (unsetIndex != null) dropdownElements[unsetIndex]?.classList.remove("delivery_dropdown_suggest__item_hover");
		const topPos = tragetElement.offsetTop;
		const bottomPos = tragetElement.clientHeight + topPos;
		const isInView = dropdown.scrollTop <= topPos && dropdown.scrollTop + dropdown.clientHeight >= bottomPos;
		if (isInView) return;

        if (dropdown.scrollTop + dropdown.clientHeight < bottomPos) {
            dropdown.scrollTop = topPos - dropdown.clientHeight + tragetElement.clientHeight;
        }

        if (dropdown.scrollTop > topPos) {
            dropdown.scrollTop = topPos;
        }
    }

    selectedItem(): IAddressSuggest | undefined {
        return this._selectedItem?.getSuggest();
    }
}

export interface ISuggestDropdownHandlers {
    selectItem: (item: IAddressSuggest) => void;
    nextFocusElement: KnockoutObservable<boolean> | null;
}