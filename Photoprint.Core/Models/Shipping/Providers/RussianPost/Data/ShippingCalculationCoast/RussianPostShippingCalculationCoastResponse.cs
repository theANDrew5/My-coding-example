using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostShippingCalculationCoastResponse
    {
        [JsonProperty("total-rate")]
        public int TotalRate { get; set; }

        [JsonProperty("total-vat")]
        public int TotalVat { get; set; }

        [JsonProperty("delivery-time")]
        public RussianPostDeliveryTime DeliveryTime { get; set; }

        [JsonProperty("avia-rate")]
        public AviaRate AviaRate { get; set; }

        [JsonProperty("inventory-rate")]
        public InventoryRate InventoryRate { get; set; }

        [JsonProperty("vsd-rate")]
        public VsdRate VsdRate { get; set; }
        [JsonIgnore]
        public decimal TotalPrice
        {
            get
            {
                return ((decimal)(TotalRate+TotalVat))/100;//все значения отдаются в копейках
            }
        }

    }

    public class BaseRate
    {
        [JsonProperty("rat")]
        public int Rate { get; set; }

        [JsonProperty("vat")]
        public int Vat { get; set; }
    }

    public class RussianPostDeliveryTime
    {
        [JsonProperty("max-days")]
        public int MaxDays { get; set; }

        [JsonProperty("min-days")]
        public int MinDays { get; set; }
    }

    public class InventoryRate : BaseRate
    {
    }

    public class VsdRate : BaseRate
    {
    }

    public class AviaRate : BaseRate
    {
    }
}
