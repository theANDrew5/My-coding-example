using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2CalculatorResponse
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("delivery_sum")]
        public decimal DeliverySum { get; set; }

        [JsonProperty("weight_calc")]
        public int WeightCalc { get; set; }

        [JsonProperty("period_min")]
        public int PeriodMin { get; set; }

        [JsonProperty("period_max")]
        public int PeriodMax { get; set; }

        [JsonProperty("total_sum")]
        public decimal TotalSum { get; set; }

        [JsonProperty("errors")]
        public CDEKv2Error[] Errors { get; set; }
    }

    public class CDEKv2CalculatorTariffListResponse {
        [JsonProperty("tariff_codes")]
        public CDEKv2CalculatorTariffListItem[] TariffList { get; set; }
        [JsonProperty("errors")]
        public CDEKv2Error[] Errors { get; set; }
    }

    public class CDEKv2CalculatorTariffListItem
    {
        [JsonProperty("tariff_code")]
        public int TariffCode { get; set; }
        [JsonProperty("tariff_name")]
        public string TariffName { get; set; }
        [JsonProperty("tariff_description")]
        public string TariffDescription { get; set; }
        [JsonProperty("delivery_mode")]
        public CDEKv2TariffMode Mode { get; set; }
        [JsonProperty("delivery_sum")]
        public decimal DeliverySum { get; set; }
        
        [JsonProperty("period_min")]
        public int PeriodMin { get; set; }

        [JsonProperty("period_max")]
        public int PeriodMax { get; set; }

    }
}
