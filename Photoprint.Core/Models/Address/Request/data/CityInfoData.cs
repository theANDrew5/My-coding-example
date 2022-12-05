using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Photoprint.Core.Models
{
    public class CityInfoData
    {
        public CityAddressCountry Country { get; set; }
        [CanBeNull]
        public CitySuggest Suggest { get; set; }

        [CanBeNull]
        public string Query { get; set; }
        [CanBeNull]
        public LatLong Coords { get; set; }
    }
}
