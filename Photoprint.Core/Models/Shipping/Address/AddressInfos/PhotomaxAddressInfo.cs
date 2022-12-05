namespace Photoprint.Core.Models
{
    public class PhotomaxAddressInfo
    {
        public string PickupPointId { get; set; }

        public PhotomaxDeliveryType DeliveryType { get; set; }
        public string OwnerId { get; set; }
        public string Route { get; set; } //договорились его не передавать
        public string CityId { get; set; }
        public string ExternalId { get; set; }
        public decimal DeliveryAdditionalPrice { get; set; }
        public PhotomaxAddressType AddressType { get; set; }
        public bool SendCodesFromTransportCompanyIntegration { get; set; }
        public bool IsSendShippingCode { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public string SkuId { get; set; }
        public string SkuTitle { get; set; }
        public string ContainmentTerm { get; set; }
    }

    public enum PhotomaxAddressType
    {
        /// <summary>
        /// Доставка фотоэкспертом
        /// </summary>
        Photomax,

        /// <summary>
        /// Забирает ТК у фотоэксперта, доставляет до точки выдачи
        /// </summary>
        TcToPickupPoint,

        /// <summary>
        /// Забирает ТК у фотоэксперта, доставляет в офис сотрудникам
        /// </summary>
        TcToOffice,

        /// <summary>
        /// Забирает сотрудник самостоятельно у фотоэксперта
        /// </summary>
        EmployeeToPickupPoint
    }
}