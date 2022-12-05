using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public sealed class RussainPostPostal
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set;}
        [JsonProperty("operator-postcode")]
        public string Postcode { get; set;}
        [JsonProperty("ops-address")]
        public string Address { get; set;}

        public UserPostOffice Convert()
        {
            return new UserPostOffice()
            {
                Enabled = this.Enabled,
                Postcode = this.Postcode,
                Address = this.Address
            };
        }
    }

    public sealed class UserPostOffice
    {
        public bool Enabled { get; set;}
        public string Postcode { get; set;}
        public string Address { get; set;}
    }
}
