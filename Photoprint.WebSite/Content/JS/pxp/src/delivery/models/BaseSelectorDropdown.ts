
export abstract class BaseSelectorDropdown <ItemT> {
	onItemSelectedCallback: (city: ItemT | undefined) => void;

	isVisible: KnockoutObservable<boolean>;
    selectedItem: KnockoutObservable<ItemT | null>;
    itemList: KnockoutObservableArray<ItemT | null>;

    abstract readonly listSelector: string;
    abstract readonly elementsSelector: string;

	constructor(onItemSelectedCallback: (city: ItemT | undefined) => void, initItems?: Array<ItemT>) {

        this.onItemSelectedCallback = onItemSelectedCallback;

        this.isVisible = ko.observable(false);
        this.selectedItem = ko.observable(initItems?.[0]?? null);
		this.itemList = ko.observableArray(initItems?? new Array<ItemT>());

		//binds
        this.highlightItemByRelativeIndex = this.highlightItemByRelativeIndex.bind(this);
        this.selectItem = this.selectItem.bind(this);
        this.update = this.update.bind(this);
        this.show = this.show.bind(this);
        this.onItemHover = this.onItemHover.bind(this);
        this._manageKeyListener = this._manageKeyListener.bind(this);
        this._keyListener = this._keyListener.bind(this);

		//subs
        this.isVisible.subscribe((val) => {this._manageKeyListener(val);});
	}

    private _manageKeyListener(isShown: boolean) {
        if (isShown) {
            document.addEventListener('keydown', this._keyListener);
        } else if(typeof(isShown)!="undefined") {
            document.removeEventListener('keydown', this._keyListener);
        }
    }

    private _keyListener(event: KeyboardEvent) {
        switch (event.keyCode) {
        case 40: // keyDown
            this.highlightItemByRelativeIndex(+1);
            break;
        case 38: // keyUp
            this.highlightItemByRelativeIndex(-1);
            break;
        case 13: // enter
            this.selectItem(this.selectedItem());
        default:
            return;
        }
    }

	show() {
		this.isVisible(true);
	}

	hide() {
		this.isVisible(false);
	}

	highlightItemByRelativeIndex(delta: number) {
		const list = this.itemList();
		if (list == null || list.length === 0) return;

		const currentSelectedItem = this.selectedItem();
		const currentIndex = currentSelectedItem !== null ? list.indexOf(currentSelectedItem) : -1;
		let newIndex = currentIndex + delta;
		if (newIndex < 0) newIndex = list.length - 1;
		if (newIndex > list.length - 1) newIndex = 0;
		this.selectedItem(list[newIndex]);

		const domList = document.querySelector(this.listSelector);
		if (domList == null) return;

		const domCityElements = document.querySelectorAll(this.elementsSelector);
		const targetDomElement = domCityElements.length > newIndex ? domCityElements[newIndex] as HTMLElement : null;
		if (targetDomElement == null) return;

		const topPos = targetDomElement.offsetTop;
		const bottomPos = targetDomElement.clientHeight + topPos;
		const isInView = domList.scrollTop <= topPos && domList.scrollTop + domList.clientHeight >= bottomPos;
		if (isInView) return; 

		if (domList.scrollTop + domList.clientHeight < bottomPos) {
            domList.scrollTop = topPos - domList.clientHeight + targetDomElement.clientHeight;
        }

        if (domList.scrollTop > topPos) {
            domList.scrollTop = topPos;
        }
	}

	onItemHover(item: ItemT) {
		this.selectedItem(item);
	}

	update(itemList: Array<ItemT> | null) {
		this.itemList(itemList ?? new Array<ItemT>());
		if (itemList == null || itemList.length === 0) {
			this.selectedItem(null);
		} else {
			this.selectedItem(itemList[0]);
		}

		this.itemList.notifySubscribers();
		this.isVisible.notifySubscribers();
	}

	selectItem(item: ItemT | null) {
		if (item === null) return;
		this.onItemSelectedCallback?.(item);
	}
}