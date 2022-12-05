using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public enum DpdParcelStatus
    {
        /// <summary>
        /// оформлен новый заказ по инициативе клиента
        /// </summary>
        [XmlEnum("NewOrderByClient")]
        NewOrderByClient,
        /// <summary>
        /// заказ отменен
        /// </summary>
        [XmlEnum("NotDone")]
        NotDone,
        /// <summary>
        /// посылка находится на терминале приема отправления
        /// </summary>
        [XmlEnum("OnTerminalPickup")]
        OnTerminalPickup,
        /// <summary>
        /// посылка находится в пути (внутренняя перевозка DPD)
        /// </summary>
        [XmlEnum("OnRoad")]
        OnRoad,
        /// <summary>
        /// посылка находится на транзитном терминале
        /// </summary>
        [XmlEnum("OnTerminal")]
        OnTerminal,
        /// <summary>
        /// посылка находится на терминале доставки
        /// </summary>
        [XmlEnum("OnTerminalDelivery")]
        OnTerminalDelivery,
        /// <summary>
        /// посылка выведена на доставку
        /// </summary>
        [XmlEnum("Delivering")]
        Delivering,
        /// <summary>
        /// посылка доставлена получателю
        /// </summary>
        [XmlEnum("Delivered")]
        Delivered,
        /// <summary>
        /// посылка утеряна
        /// </summary>
        [XmlEnum("Lost")]
        Lost,
        /// <summary>
        /// с посылкой возникла проблемная ситуация
        /// </summary>
        [XmlEnum("Problem")]
        Problem,
        /// <summary>
        /// посылка возвращена с доставки 
        /// </summary>
        [XmlEnum("ReturnedFromDelivery")]
        ReturnedFromDelivery,
        /// <summary>
        /// оформлен новый заказ по инициативе DPD
        /// </summary>
        [XmlEnum("NewOrderByDPD")]
        NewOrderByDPD
    }

}
