using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Admin.API.Models
{
    public class ShippingPriceRequestDTO
    {
        public string weight { get; set; }
        public int languageId { get; set; }
        public ShippingAddressDTO ShippingAddress { get; set; }
    }
    public class AddressRequestDTO
    {
        public int? Id { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Flat { get; set; }
        public string PostalCode { get; set; }
        public JObject DeliveryProperties { get; set; }
    }
}