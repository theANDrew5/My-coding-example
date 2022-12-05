using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Photoprint.Core.Models
{
    public class AddressInfoData
    {
        public AddressSuggest Suggest { get; set; }
        public LatLong Coords;
        public CityAddress City { get; set; }
        public string Query { get; set; }
        public string FullQuery => string.IsNullOrWhiteSpace(Query) || City is null
            ? null : $"{Query}, {City.Title}, {City.Description}";
    }


    public class LatLong
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public bool Ok => !string.IsNullOrWhiteSpace(Latitude) && !string.IsNullOrWhiteSpace(Longitude);
    }
}
