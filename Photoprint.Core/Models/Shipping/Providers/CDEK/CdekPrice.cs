using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CdekPrice
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("tariffId")]
        public int Tariff { get; set; }
    }
}
