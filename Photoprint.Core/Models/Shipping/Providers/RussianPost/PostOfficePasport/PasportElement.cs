using System.Collections.Generic;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class PasportElement
    {
        [JsonProperty("address")]
        public PasportAddress Address { get; set; }
        [JsonProperty("addressFias")]
        public PasportAddressFias AddressFias { get; set; }
        [JsonProperty("Ecom")]
        public int Ecom { get; set; }
        [JsonProperty("ecomOptions")]
        public PasportEcomOptions EcomOptions { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("onlineParcel")]
        public int OnlineParcel { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("workTime")]
        public List<string> WorkTime { get; set; }

    }

    public class PasportAddress
    {
        [JsonProperty("addressType")]
        public PassportAddressType AddressType { get; set; }
        [JsonProperty("area")]
        public string Area { get; set; }
        [JsonProperty("house")]
        public string House { get; set; }
        [JsonProperty("index")]
        public string Index { get; set; }
        [JsonProperty("manualInput")]
        public bool ManualInput { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("place")]
        public string Place { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
    }

    public class PasportAddressFias
    {
        [JsonProperty("addGarCode")]
        public string AddGarCode { get; set; }
        [JsonProperty("locationGarCode")]
        public string LocationGarCode { get; set; }
        [JsonProperty("regGarId")]
        public string RegGarId { get; set; }
    }

    public class PasportEcomOptions
    {
        [JsonProperty("cardPayment")]
        public bool CardPayment { get; set; }
        [JsonProperty("cashPayment")]
        public bool CashPayment { get; set; }
        [JsonProperty("contentsChecking")]
        public bool ContentsChecking { get; set; }
        [JsonProperty("functionalityChecking")]
        public bool FunctionalityChecking { get; set; }
        [JsonProperty("partialRedemption")]
        public bool PartialRedemption { get; set; }
        [JsonProperty("returnAvailable")]
        public bool returnAvailable { get; set; }
        [JsonProperty("weightLimit")]
        public float WeightLimit { get; set; }
        [JsonProperty("withFitting")]
        public bool WithFitting { get; set; }
    }

    public enum PassportAddressType
    {
        DEFAULT,
    }
}
