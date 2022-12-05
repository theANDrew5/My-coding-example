namespace Photoprint.Core.Models
{
    public class EvropochtaDeliveryTypeModel
    {
        public EvropochtaDeliveryTypeModel() { }

        public int PostalDeliveryTypeId { get; set; }
        public string PostalDeliveryTypeNameWeb { get; set; }
    }
    public class EvropochtaPostalTypeModel
    {
        public EvropochtaPostalTypeModel() { }

        public int GoodsId { get; set; }
        public string GoodsName { get; set; }
    }
    public class EvropochtaWeightTypeModel
    {
        public EvropochtaWeightTypeModel() { }

        public int PostalWeightTypeId { get; set; }
        public string PostalWeightTypeNameWeb { get; set; }
    }
}
