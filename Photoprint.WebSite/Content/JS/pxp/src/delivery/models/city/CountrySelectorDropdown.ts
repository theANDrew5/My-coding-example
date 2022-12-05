import { BaseSelectorDropdown } from "../BaseSelectorDropdown";
import { CountryInfo } from "./CountrySelector";

declare type SuggestSelectedCallback =  (suggest: CountryInfo | undefined) => void;
export class CountrySelectorDropdown extends BaseSelectorDropdown<CountryInfo>{
    listSelector = "";
    elementsSelector = "";

    constructor(callback: SuggestSelectedCallback, countries: Array<CountryInfo>) {
        super(callback, countries);
    }
}