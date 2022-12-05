using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class RussianPostPassport
    {
        [JsonProperty("passportElements")]
        public List<PasportElement> PasportElements { get; set; }
    }
}
