using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class GetCitiesResponce
    {
        [JsonProperty("geonames")]
        public List<CdekCity> Cities { get; set; }
    }
}
