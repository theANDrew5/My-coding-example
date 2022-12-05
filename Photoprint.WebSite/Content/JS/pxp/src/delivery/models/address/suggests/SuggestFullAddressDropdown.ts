import { CityInfo } from "../../city/CityInfo";
import { SuggestedStreetItem } from "./SuggestedStreetItem";
import { ISuggestDropdownHandlers, SuggestBaseDropdown } from "./_base/SuggestBaseDropdown";

export class SuggestFullAddressDropdown extends SuggestBaseDropdown {

    constructor(currentCity: KnockoutObservable<CityInfo | null>, handlers: ISuggestDropdownHandlers) {
        super(currentCity, handlers, ".adr_addressLine1");
    }

    update(items: SuggestedStreetItem[] | null) {
        if (items == null) {
            this.isShown(false);
            return;
        }
        const city = this.currentCity();
        if (city === null) return;

        const filteredItems: { [key: string]: SuggestedStreetItem } = { };
        for (let item of items) {
            if (item.city.Title !== city.title || item.street == null) continue;
            if (Object.keys(filteredItems).indexOf(item.house) > 0) continue;

            filteredItems[item.addressForDropdown] = item;
        }

        this.list(Object.values(filteredItems));
        this.isShown(items.length > 0);
    }

}