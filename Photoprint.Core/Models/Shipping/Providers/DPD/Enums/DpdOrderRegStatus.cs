using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public enum DpdOrderRegStatus
    {
        /// <summary>
        /// заказ на доставку успешно создан с номером, указанным в поле orderNum.
        /// </summary>
        [XmlEnum("OK")]
        OK,
        [XmlEnum("Error")]
        Error,
        [XmlEnum("Canceled")]
        Canceled,
        /// <summary>
        /// заказ на доставку принят, но нуждается в ручной доработке сотрудником DPD,
        /// (например, по причине того, что адрес доставки не распознан автоматически).
        /// Номер заказа будет присвоен ему, когда это доработка будет произведена.
        /// </summary>
        [XmlEnum("OrderPending")]
        OrderPending,
        [XmlEnum("OrderDuplicate")]
        OrderDuplicate,
        [XmlEnum("OrderError")]
        OrderError,
        [XmlEnum("OrderCancelled")]
        OrderCancelled,
        [XmlEnum("NotFound")]
        NotFound,
        [XmlEnum("CallDPD")]
        CallDPD,
        [XmlEnum("no-data-found")]
        NotDataFound,
        [XmlEnum("CanceledPreviously")]
        CanceledPreviously,
    }
}
