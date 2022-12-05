using System.Linq;

namespace Photoprint.Core.Models
{
    public class EvropochtaOrderRequest
    {
        public int GoodsId { get; set; }
        public int PostDeliveryTypeId { get; }
        public int PostalWeightId { get; set; }
        public decimal CashOnDeliverySum { get; set; }
        public decimal CashOnDeliveryDeclareValueSum { get; set; }
        public int WarehouseIdStart { get; set; }
        public int WarehouseIdFinish { get; set; }
        public int Adress1IdReciever { get; set; }
        public string PhoneNumberReciever { get; set; }
        public string EMailReciever { get; set; }
        public string Name1Reciever { get; set; }
        public string Name2Reciever { get; set; }
        public string Name3Reciever { get; set; }
        public string InfoSender { get; set; }
        public string PostalItemExternalId { get; set; }

        public EvropochtaOrderRequest(EvropochtaServiceProviderSettings settings, string orderId, Order order, User userOwner, int addressIdFinish, PostalType postalType, bool sendCustomDeclareSum)
        {
            GoodsId = settings.GoodsTypeId;
            PostalWeightId = settings.WeightTypeId;
            PhoneNumberReciever = string.IsNullOrEmpty(order.DeliveryAddress?.Phone) ? userOwner.GetCleanPhone() : new string(order.DeliveryAddress.Phone.Where(char.IsDigit).ToArray());
            EMailReciever = userOwner.Email;
            Name1Reciever = order.DeliveryAddress.LastName ?? userOwner.LastName;
            Name2Reciever = order.DeliveryAddress.FirstName ?? userOwner.FirstName;
            Name3Reciever = order.DeliveryAddress.MiddleName ?? userOwner.MiddleName;
            InfoSender = order.ShippingComment ?? string.Empty;
            PostalItemExternalId = orderId;

            WarehouseIdStart = settings.WarehouseIdStart;

            if (postalType == PostalType.ToStorageDelivery)
            {
                PostDeliveryTypeId = 1;
                WarehouseIdFinish = addressIdFinish;
            }
            else
            {
                PostDeliveryTypeId = 2;
                Adress1IdReciever = addressIdFinish;
            }

            if (sendCustomDeclareSum && settings.SendCustomDeclareSum && order.PaymentStatus != OrderPaymentStatus.Paid)
            {
                CashOnDeliveryDeclareValueSum = settings.IsFullAmountToPay ? order.AmountToPay : order.Price;
                CashOnDeliverySum = CashOnDeliveryDeclareValueSum;
            }
            if (settings.SendStandartDeclareSum && order.PaymentStatus != OrderPaymentStatus.Paid)
            {
                CashOnDeliveryDeclareValueSum = settings.IsFullAmountToPay ? order.AmountToPay : order.Price;
                CashOnDeliverySum = CashOnDeliveryDeclareValueSum;
            }
        }
    }

    public class EvropochtaOrderResponse
    {
        public EvropochtaOrderResponse() { }

        public string Number { get; set; }
        public string PostalItemExternalId { get; set; }
        public decimal ShipmentPriceWithTax { get; set; }
        public string ShipmentPriceWithTaxComments { get; set; }
    }
}
