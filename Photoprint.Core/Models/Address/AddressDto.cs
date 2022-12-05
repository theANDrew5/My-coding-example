using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Photoprint.Core.Models
{
    public class AddressDTO
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Flat { get; set; }
        public string PostalCode { get; set; }
        public string AddressLine { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Description { get; set; }
        public AddressDTO(Address address)
        {
            Country = address.Country;
            Region = address.Region;
            City = address.City;
            Street = address.Street;
            House = address.House;
            Flat = address.Flat;
            Longitude = address.Longitude;
            Latitude = address.Latitude;
            PostalCode = address.PostalCode;
            AddressLine = address.ToAddressString();
            Description = address.Description;
        }
        public AddressDTO()
        {
        }
    }

    public class ShippingAddressDTO: AddressDTO
    {
        public int? Id { get; set; }
        public int ShippingId { get; set; }
        public JObject DeliveryProperties { get; set; }

        public ShippingAddressDTO(ShippingAddress address): base(address)
        {
            Id = address.Id;
            ShippingId = address.ShippingId;
            DeliveryProperties = JObject.FromObject(address.DeliveryProperties);
        }

        public ShippingAddressDTO() : base()
        {

        }
    }
}
