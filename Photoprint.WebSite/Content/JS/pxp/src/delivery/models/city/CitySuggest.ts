import { CityInfo, ToponimType } from "./CityInfo";

export class CitySuggest implements ICitySuggest {
	
	title: string;
    description: string;
    geoid: string;
	longitude: number | null;
	latitude: number | null;
	search: string;
	type: ToponimType;

	constructor(dto: ICitySuggestDTO) {
		this.title = dto.Title;
		this.description = dto.Description;
		this.geoid = dto.GeoId;
		this.longitude = parseFloat(dto.Longitude);
		this.latitude = parseFloat(dto.Latitude);
		this.search = this.description==null? `${this.title}`: `${this.title}, ${this.description}`;
		this.type = dto.Type;
	}
}

export interface ICitySuggest {
	title: string;
    description: string;
}

export interface ICitySuggestDTO {
	Title: string;
    Description: string;
    Longitude: string;
	Latitude: string;
    GeoId: string;
	Type: ToponimType;
}