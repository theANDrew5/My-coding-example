export enum ToponimType {
	Country,
	Region,
	City
}

export interface ICitySuggestDTO {
	Title: string;
    Description: string | null;
    Longitude: string;
	Latitude: string;
    GeoId: string;
	Type: ToponimType
}

export class CitySuggest {
	readonly title: string;
    readonly description: string | null;
    readonly geoid: string;
	readonly longitude: string;
	readonly latitude: string;
	readonly search: string;
	readonly type: ToponimType;

	constructor(dto: ICitySuggestDTO) {
		this.title = dto.Title;
		this.description = dto.Description;
		this.geoid = dto.GeoId;
		this.longitude = dto.Longitude;
		this.latitude = dto.Latitude;
		this.search = this.description==null? `${this.title}`: `${this.title}, ${this.description}`;
		this.type = dto.Type;
	}

    getDto():ICitySuggestDTO {
        return {
			Title: this.title,
			Description: this.description,
			Latitude: this.latitude,
			Longitude: this.longitude,
			GeoId: this.geoid,
			Type: this.type
        }
    }
}

export interface ICityState{
	Title: string | null;
	Area: string | null;
	Region: string | null;
	Country: string | null;
	Latitude: string | null;
	Longitude: string | null;
	Description: string | null;
    Bound: ICityBound | null;
	Type: ToponimType
}

export interface ILatLong {
	latitude: string,
	longitude: string
}

export interface ICityBound {
    UpperLongitude: string;
    UpperLatitude: string;
    LowerLongitude: string;
    LowerLatitude: string;
}

export class CityInfo {

	readonly title: string;
	readonly area: string;
	readonly region: string;
	readonly country: string;
	readonly description: string | null;

	readonly coords: ILatLong | null;
	readonly bound: ICityBound | null;

	readonly type: ToponimType;

	constructor(dto: ICityState) {

		this.title = dto.Title ?? '';
		this.area = dto.Area ?? '';
		this.region = dto.Region ?? '';
		this.country = dto.Country ?? '';
		this.description = dto.Description;
		

		this.coords = !!dto.Latitude && !!dto.Longitude 
			? {latitude: dto.Latitude, longitude: dto.Longitude} : null;
        this.bound = dto.Bound;

        if (this.description == "") {
			if (this.country!="")
				this.description=this.country;
			if (this.region!="")
				this.description=this.description.length!=0? `${this.region} ,`.concat(this.description) : this.region;
			if (this.area!="")
				this.description=this.description.length!=0? `${this.area} ,`.concat(this.description) : this.area
        }
		this.type = dto.Type;
	}

    getDto(): ICityState {
        return {
            Country: this.country,
            Region: this.region,
            Area: this.area,
            Title: this.title,
            Latitude: this.coords?.latitude ?? null,
            Longitude: this.coords?.longitude ?? null,
            Description: this.description,
            Bound: this.bound,
			Type: this.type
		};
    }

    equals(city: ICityState):boolean {
		return this.title == city.Title && this.area == city.Area && this.region == city.Region && this.country == city.Country;
    }

}