using System;

namespace Photoprint.Core.Models
{
    public class BoxberryAddressInfo
    {
        /// <summary>
        /// Код точки откуда покупатель заберет заказ
        /// </summary>
        public string PlaceCode { get; set; }

        // Ниже данные по доставке до дома

        public DateTime TimeFrom1 { get; set; }
        public DateTime TimeTo1 { get; set; }
        public DateTime TimeFrom2 { get; set; }
        public DateTime TimeTo2 { get; set; }
        public string Commentary { get; set; }
    }
}
