using System;
using System.Collections.Generic;
using System.Text;

namespace Photoprint.Core.Models
{
    public class CitySuggestData
    {
        public CityAddressCountry Country { get; set; }
        public string Query { get; set; }
        public ToponymType Type { get; set;}
    }
}
