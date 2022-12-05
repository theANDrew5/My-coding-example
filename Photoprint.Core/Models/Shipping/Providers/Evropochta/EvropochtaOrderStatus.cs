using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class EvropochtaOrderStatus
    {
        private static readonly IDictionary<string, OrderStatus> _infoTrackMapping = new Dictionary<string, OrderStatus>
        {
            { "Почтовое отправление в доставке на центральный сортировочный пункт", OrderStatus.Shipped},
            { "Почтовое отправление прибыло на центральный сортировочный пункт", OrderStatus.Shipped},
            { "В пути на ОПС назначения", OrderStatus.Shipped },
            { "Почтовое отправление прибыло на ОПС выдачи", OrderStatus.ShippedToStorage},
            { "Почтовое отправление выдано. Наложенный платеж оплачен", OrderStatus.Delivered },
            { "Заявка отменена, срок предоставления почтового отправления истек", OrderStatus.Cancelled }
        };

        /// <summary>
        /// Пытаемся получить статус по общей информации о текущем состоянии почтовой заявки
        /// </summary>
        /// <param name="infoTrack"></param>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        public static bool TryGetStatus(string infoTrack, out OrderStatus orderStatus)
        {
            orderStatus = OrderStatus.None;

            if (!string.IsNullOrEmpty(infoTrack) && _infoTrackMapping.TryGetValue(infoTrack, out OrderStatus newStatus))
            {
                orderStatus = newStatus;
                return true;
            }

            return false;
        }
    }
    public class EvropochtaTrackingResponse
    {
        EvropochtaTrackingResponse() { }

        public string InfoTrack { get; set; }
    }
}
