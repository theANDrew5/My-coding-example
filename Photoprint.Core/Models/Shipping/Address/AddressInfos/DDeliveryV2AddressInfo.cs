using JetBrains.Annotations;

namespace Photoprint.Core.Models
{
    public class DDeliveryV2AddressInfo
    {
        public DDeliveryV2CalculatorResult PriceCalcResult { get; set; }
        public int DeliveryType { get; set; }
        public int? PointId { get; set; }
        public int DeliveryCompanyId { get; set; }
        public int CityToId { get; set; }
        public string StreetTo { get; set; }
        public string HouseTo { get; set; }
        public string FlatTo { get; set; }
        public string CityToFias { get; set; }
        public string CityToKladr { get; set; }
    }

    public class DDeliveryV2CalculatorResult
    {
        public decimal PriceDelivery { get; set; }
        public decimal? PriceSorting { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
