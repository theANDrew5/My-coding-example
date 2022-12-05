import { ICitySuggest } from "./CitySuggest";
import { BaseSelectorDropdown } from "../BaseSelectorDropdown";

declare type SuggestSelectedCallback =  (suggest: ICitySuggest | undefined) => void;
export class CitySelectorDropdown extends BaseSelectorDropdown<ICitySuggest> {
    listSelector = ".delivery_location_selector__citysuggest";
    elementsSelector = ".delivery_location_selector__citysuggest_city";

	constructor(callback: SuggestSelectedCallback) {
        super(callback);
    }


}