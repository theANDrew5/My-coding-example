import { CityInfo } from "../../city/CityInfo";
import { SuggestedStreetItem } from "./SuggestedStreetItem";
import { ISuggestDropdownHandlers, SuggestBaseDropdown } from "./_base/SuggestBaseDropdown";

export class SuggestHouseDropdown extends SuggestBaseDropdown {

    constructor(currentCity: KnockoutObservable<CityInfo | null>, handlers: ISuggestDropdownHandlers) {
        super(currentCity, handlers, ".adr_house");
    }

    update(items: SuggestedStreetItem[] | null) {
        if (items == null) {
            this.isShown(false);
            return;
        }
        var city = this.currentCity();
        if (city === null) return;

        var filteredItems: { [key: string]: SuggestedStreetItem } = { };
        for (var item of items) {
            if (item.city.Title !== city.title) continue;
            if (Object.keys(filteredItems).indexOf(item.house) > 0) continue;

            filteredItems[item.house] = item;
        }

        this.list(Object.values(filteredItems));
        this.isShown(items.length > 0);
    }
}