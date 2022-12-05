using System.Collections.Generic;
using System.Linq;

namespace Photoprint.Core.Models
{
    public class EvropochtaCalculationTariffRequest
    {
        public int GoodsId { get; set; }
        public int PostDeliveryTypeId { get; set; }
        public int PostalWeightId { get; set; }
        public decimal CashOnDeliverySum { get; set; }
        public decimal CashOnDeliveryDeclareValueSum { get; set; }
        public int WarehouseIdFinish { get; set; }
        public int Adress1IdReciever { get; set; }

        public EvropochtaCalculationTariffRequest(OrderAddress selectedAddress, IEnumerable<IPurchasableItem> items, EvropochtaServiceProviderSettings settings, PostalType postalType, int address1Id)
        {
            GoodsId = settings.GoodsTypeId;
            PostDeliveryTypeId = settings.DeliveryTypeId;
            PostalWeightId = settings.WeightTypeId;
            CashOnDeliverySum = items.Sum(i => i.Price);
            CashOnDeliveryDeclareValueSum = items.Sum(i => i.Price);
            WarehouseIdFinish = postalType == PostalType.ToStorageDelivery ? selectedAddress?.DeliveryProperties?.EvropochtaAddressInfo?.WarehouseId ?? 0 : 0;
            Adress1IdReciever = address1Id;
        }
    }
}
