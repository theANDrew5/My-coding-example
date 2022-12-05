import { IPointCityData } from "../address/BaseAddressModel";
import { IPxpMapBounds } from "../types/_base/BaseDeliveryPointItem";
import { ICitySuggest } from "./CitySuggest";

export class CityInfo implements ICitySuggest {
	readonly originalData: ICityState;

	readonly title: string;
	readonly area: string;
	readonly region: string;
	readonly country: string;
	readonly description: string;
	readonly latitude: number;
	readonly longitude: number;
	readonly coords: number[];
	readonly bound: IPxpMapBounds;
	readonly type: ToponimType;

	constructor(cityState: ICityState) {
		this.originalData = cityState;

		this.title = cityState.Title;
		this.area = cityState.Area;
		this.region = cityState.Region;
		this.country = cityState.Country;
		this.description = cityState.Description;
		this.type = cityState.Type;

		this.latitude = parseFloat(cityState.Latitude);
		if (isNaN(this.latitude)) this.latitude = 0;

		this.longitude = parseFloat(cityState.Longitude);
		if (isNaN(this.longitude)) this.longitude = 0;

		this.coords = [this.latitude, this.longitude];
        this.bound = cityState.Bound;

        if (this.description == "") {
			if (this.country!="")
				this.description=this.country;
			if (this.region!="")
				this.description=this.description.length!=0? `${this.region} ,`.concat(this.description) : this.region;
			if (this.area!="")
				this.description=this.description.length!=0? `${this.area} ,`.concat(this.description) : this.area
        }
	}

    getPointData(): IPointCityData {
        return {
			country: this.country,
			region: this.region,
			title: this.title
        }
    }

    getState(): ICityState {
        return {
            Country: this.country,
            Region: this.region,
            Area: this.area,
            Title: this.title,
            Latitude: this.latitude.toString(),
            Longitude: this.longitude.toString(),
            Description: this.description,
            Bound: this.bound,
			Type: this.type
    };
    }

    equals(city: IPointCityData):boolean {
		return this.title == city.title && this.region == city.region && this.country == city.country;
    }

}

export enum ToponimType {
	Country,
	Region,
	City
}

export interface ICityState{
	Title: string;
	Area: string;
	Region: string;
	Country: string;
	Latitude: string;
	Longitude: string;
	Description: string;
    Bound: IPxpMapBounds;
	Type: ToponimType
}