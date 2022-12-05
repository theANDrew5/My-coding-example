import { SuggestedStreetItem } from "./SuggestedStreetItem";
import { CityInfo } from "../../city/CityInfo";
import { ISuggestDropdownHandlers, SuggestBaseDropdown } from "./_base/SuggestBaseDropdown";

export class SuggestStreetDropdown extends SuggestBaseDropdown{

    constructor(currentCity: KnockoutObservable<CityInfo | null>, handlers: ISuggestDropdownHandlers) {
        super(currentCity, handlers, ".adr_street");
    }

    update(items: SuggestedStreetItem[] | null) {
        if (items == null) {
            this.isShown(false);
            return;
        }
        var city = this.currentCity();
        if (city === null) return;

        var filteredItems: Array<SuggestedStreetItem> = [];
        for (var item of items) {
            if (item.city.Title !== city.title || item.street == null) continue;
            filteredItems.push(item);
        }

        this.list(Object.values(filteredItems));
        this.isShown(items.length > 0);
    }

}
