using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public static class BoxberryOrderStatus
    {
        // Статусы на русском согласно документации
        private static readonly IDictionary<string, OrderStatus> _statusNames = new Dictionary<string, OrderStatus>
        {
            { "Принято к доставке", OrderStatus.Shipped },
            { "Передано на сортировку", OrderStatus.Shipped },
            { "Отправлен на сортировочный терминал", OrderStatus.Shipped },
            { "Отправлено в город назначения", OrderStatus.Shipped },
            { "Передано на курьерскую доставку", OrderStatus.Shipped },
            { "Поступило в пункт выдачи", OrderStatus.ShippedToStorage },
            { "Выдано", OrderStatus.Delivered },
            { "Возвращено с курьерской доставки", OrderStatus.Returned },
            { "Готовится к возврату", OrderStatus.Shipped },
            { "Отправлено в пункт приема", OrderStatus.Shipped },
            { "Возвращено в пункт приема", OrderStatus.Returned },
            { "Возвращено в ИМ", OrderStatus.Cancelled }
        };

        public static bool TryGetOrderStatus(string statusName, out OrderStatus status)
        {
            status = _statusNames.ContainsKey(statusName) ? _statusNames[statusName] : OrderStatus.None;
            return _statusNames.ContainsKey(statusName);
        }
    }

    public class BoxberryStatusResponseModel
    {
        public List<BoxberryStatus> statuses { get; set; }
        public bool PD { get; set; }
        public decimal sum { get; set; }
        public double Weight { get; set; }
        public string PaymentMethod { get; set; }
        public string err { get; set; }
        
        public class BoxberryStatus
        {
            public string Date { get; set; }
            public string Name { get; set; }
            public string Comment { get; set; }
        }
    }
}
