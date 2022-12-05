import { CityInfo, ICityState } from "./CityInfo";

export enum AddressSuggestType {
    Street,
    House
}

export interface IAddressSuggestDTO {
    Latitude: string;
    Longitude: string;
    City: ICityState;
    House: string;
    Street: string;
    Description: string;
    GeoId: string;
    SearchString: string;
}

export class AddressSuggest {
	lat: string;
    lon: string;
    city: ICityState;
    street: string;
    house: string;
    description: string;
    geoId: string;
    search: string;
    type: AddressSuggestType;

    constructor(dto: IAddressSuggestDTO, type: AddressSuggestType) {
        this.lat = dto.Latitude;
        this.lon = dto.Longitude;
        this.city = dto.City;
        this.street = dto.Street;
        this.house = dto.House;
        this.description = dto.Description;
        this.geoId = dto.GeoId;
        this.search = dto.SearchString;
        this.type = type;
    }

    //getDto():IAddressSuggestDTO {
    //    return <IAddressSuggestDTO>{
    //        Latitude: this.lat,
    //        Longitude: this.lon,
    //        City: this.city,
    //        Street: this.street,
    //        House: this.house,
    //        Description: this.description,
    //        GeoId: this.geoId
    //    }
    //}
}

export interface IAddressInfoDTO {
    Latitude: string;
    Longitude: string;
    Country: string;
    Region: string;
    Area: string;
    City: string;
    District?: string;
    Street?: string;
    House: string;
    Flat?: string;
    PostalCode?: string;
    AddressLine?: string;
}


export class AddressInfo {
    city?: CityInfo;
    street: string;
    house: string;
    flat: string;
    postalCode: string;
    latitude: string;
    longitude: string;
    addressLine: string;

    constructor(data: IAddressInfoDTO | AddressInfo) {

        this.getAddressLine = this.getAddressLine.bind(this);
        this.copy = this.copy.bind(this);

        const dto = (data instanceof AddressInfo)? undefined : data;
        const obj = (data instanceof AddressInfo)? data : undefined;
        if (dto) {
            this.city = (dto.City!='' || dto.Region!='' || dto.Country!='')
                ? new CityInfo({
                    Title: dto.City?? '',
                    Region: dto.Region?? '',
                    Country: dto.Country?? '',
                } as ICityState)
                : undefined;
            this.street = dto.Street ?? '';
            this.house = dto.House;
            this.flat = dto.Flat ?? '';
            this.postalCode = dto.PostalCode ?? '';
            this.latitude = dto.Latitude;
            this.longitude = dto.Longitude;
            this.addressLine = dto.AddressLine?? '';
            return;
        }
        if (obj) {
            this.city = obj.city
            this.street = obj.street;
            this.house = obj.house;
            this.flat = obj.flat;
            this.postalCode = obj.postalCode;
            this.addressLine = obj.addressLine;
            this.latitude = obj.latitude;
            this.longitude = obj.longitude;
            return;
        }

        this.street = '';
        this.house = '';
        this.flat = '';
        this.postalCode = '';
        this.addressLine = '';
        this.latitude = '';
        this.longitude = '';
    }

    public getAddressLine1(): string {
        return (this.street!='' && this.house!='')
        ? `${this.street}, ${this.house}` : '';
    }

    public getAddressLine(): string {
        if(this.addressLine && this.addressLine!='')
            return this.addressLine;
        let result =this.getAddressLine1();
        if( result == '')
            return result;
        if(this.flat!='')
            result = `${result} - ${this.flat}`;
        return result;
    }

    public copy(clearData?: boolean): AddressInfo {
        const result = new AddressInfo(this);
        if (clearData??false) {
            result.city = undefined;
            result.street = '';
            result.house = '';
            result.flat = '';
            result.latitude = '';
            result.longitude = '';
            result.postalCode = '';
            result.latitude = '';
            result.longitude = '';
        }
        return result;
    }

    getDto(): IAddressInfoDTO {
        return <IAddressInfoDTO>{
            Country: this.city?.country ?? '',
            Region: this.city?.region?? '',
            Area: this.city?.area ?? '',
            City: this.city?.title ?? '',
            Street: this.street,
            House: this.house,
            Flat: this.flat,
            PostalCode: this.postalCode,
            AddressLine: this.getAddressLine(),
            Latitude: this.latitude,
            Longitude: this.longitude
        }
    }
}