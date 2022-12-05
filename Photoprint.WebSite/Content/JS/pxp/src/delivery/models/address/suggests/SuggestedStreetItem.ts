import { ICityState } from "../../city/CityInfo";


export class SuggestedStreetItem {
    lat: number | null;
    lon: number | null;
    city: ICityState;
    street: string;
    house: string;
    description: string;
    geoId: string;
    addressForDropdown: string;

    constructor(dto: IAddressSuggest) {
        this.lat = dto.Latitude == null ? null : parseFloat(dto.Latitude);
        this.lon = dto.Longitude == null ? null : parseFloat(dto.Longitude);
        this.city = dto.City;
        this.street = dto.Street;
        this.house = dto.House;
        this.description = dto.Description;
        this.geoId = dto.GeoId;
        this.addressForDropdown = `${this.street}, ${this.house??""}`;
    }

    getSuggest():IAddressSuggest {
        return <IAddressSuggest>{
            Latitude: this.lat,
            Longitude: this.lon,
            City: this.city,
            Street: this.street,
            House: this.house,
            Description: this.description,
            GeoId: this.geoId
        }
    }
}

export interface IAddressSuggest {
    Latitude: string | null;
    Longitude: string | null;
    City: ICityState;
    House: string;
    Street: string;
    Description: string;
    GeoId: string;
}