using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photoprint.Core.Models
{
    public class RPShippingCalculationCoastTariffAPIResponce
    {
        [JsonProperty("pay")]
        public int TotalRate { get; set; }
        [JsonProperty("nds")]
        public int TotalVat { get; set; }
        [JsonProperty("paynds")]
        public int TotalRateAndVat { get; set; }
        [JsonIgnore]
        public decimal TotalPrice
        {
            get
            {
                return (decimal)TotalRateAndVat/100; //все значения отдаются в копейках
            }
        }
    }
}
