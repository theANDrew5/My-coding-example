using System;

namespace Photoprint.Core.Models
{
    public class YandexGoAddressInfo
    {
        public decimal Cost { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public Address Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}