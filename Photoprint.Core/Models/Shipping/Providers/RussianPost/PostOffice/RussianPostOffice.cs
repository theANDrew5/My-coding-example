using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostoffice
    {
        [JsonProperty("address-source")]
        public string AddressSource { get; set; }

        [JsonProperty("current-day-working-hours")]
        public CurrentDayWorkingHours CurrentDayWorkingHours { get; set; }

        [JsonProperty("distance")]
        public int Distance { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("isclosed")]
        public bool IsClosed { get; set; }

        [JsonProperty("is-private-category")]
        public bool IsPrivateCategory { get; set; }

        [JsonProperty("is-temporary-closed")]
        public bool IsTemporaryClosed { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("nearest-office-postalcode")]
        public string NearestOfficePostalcode { get; set; }

        [JsonProperty("nearest-postoffice")]
        public NearestPostoffice NearestPostoffice { get; set; }

        [JsonProperty("phones")]
        public Phone[] Phones { get; set; }

        [JsonProperty("postal-code")]
        public string PostalCode { get; set; }

        [JsonProperty("prescribed")]
        public bool Prescribed { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("service-groups")]
        public ServiceGroups[] ServiceGroups { get; set; }

        [JsonProperty("settlement")]
        public string Settlement { get; set; }

        [JsonProperty("temporary-closed-reason")]
        public string TemporaryClosedReason { get; set; }

        [JsonProperty("type-code")]
        public string TypeCode { get; set; }

        [JsonProperty("type-id")]
        public int TypeId { get; set; }

        [JsonProperty("ufps-code")]
        public string UfpsCode { get; set; }

        [JsonProperty("working-hours")]
        public WorkingHours[] WorkingHours { get; set; }

        [JsonProperty("works-on-saturdays")]
        public bool WorksOnSaturdays { get; set; }

        [JsonProperty("works-on-sundays")]
        public bool WorksOnSundays { get; set; }

        public ShippingAddress GetAddress()
        {
            if (IsTemporaryClosed)
                return null;
            return new ShippingAddress
            {
                Region = Region,
                City = Settlement,
                AddressLine1 = AddressSource,
                Longitude = Longitude,
                Latitude = Latitude,
                PostalCode = PostalCode
            };
        }
    }

    public class CurrentDayWorkingHours
    {
        [JsonProperty("weekday-id")]
        public int WeekdayId { get; set; }

        [JsonProperty("weekday-name")]
        public string WeekdayName { get; set; }
    }

    public class NearestPostoffice
    {
        [JsonProperty("address-source")]
        public string AddressSource { get; set; }

        [JsonProperty("current-day-working-hours")]
        public CurrentDayWorkingHours CurrentDayWorkingHours { get; set; }

        [JsonProperty("distance")]
        public int Distance { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("holidays")]
        public Holiday[] Holidays { get; set; }

        [JsonProperty("is-closed")]
        public bool IsClosed { get; set; }

        [JsonProperty("is-private-category")]
        public bool Isprivatecategory { get; set; }

        [JsonProperty("is-temporary-closed")]
        public bool IsTemporaryClosed { get; set; }

        [JsonProperty("latitude")]
        public int Latitude { get; set; }

        [JsonProperty("longitude")]
        public int Longitude { get; set; }

        [JsonProperty("nearest-office-postalcode")]
        public string NearestOfficePostalcode { get; set; }

        [JsonProperty("postalcode")]
        public string Postalcode { get; set; }

        [JsonProperty("prescribed")]
        public bool Prescribed { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("settlement")]
        public string Settlement { get; set; }

        [JsonProperty("temporary-closed-reason")]
        public string TemporaryClosedReason { get; set; }

        [JsonProperty("type-code")]
        public string TypeCode { get; set; }

        [JsonProperty("type-id")]
        public int TypeId { get; set; }

        [JsonProperty("working-hours")]
        public WorkingHours[] WorkingHours { get; set; }

        [JsonProperty("work-son-saturdays")]
        public bool WorkSonSaturdays { get; set; }

        [JsonProperty("work-son-sundays")]
        public bool WorkSonSundays { get; set; }
    }

    public class Holiday
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("schedule")]
        public Schedule Schedule { get; set; }
    }

    public class Schedule
    {
        [JsonProperty("weekday-id")]
        public int WeekdayId { get; set; }

        [JsonProperty("weekday-name")]
        public string WeekdayName { get; set; }
    }

    public class WorkingHours
    {
        [JsonProperty("weekday-id")]
        public int WeekdayId { get; set; }

        [JsonProperty("weekday-name")]
        public string WeekdayName { get; set; }
    }

    public class Phone
    {
        [JsonProperty("is-fax")]
        public bool IsFax { get; set; }

        [JsonProperty("phone-number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phone-town-code")]
        public string PhoneTownCode { get; set; }

        [JsonProperty("phone-type-name")]
        public string PhoneTypeName { get; set; }
    }

    public class ServiceGroups
    {
        [JsonProperty("group-id")]
        public int GroupId { get; set; }

        [JsonProperty("group-name")]
        public string GroupName { get; set; }
    }
}
