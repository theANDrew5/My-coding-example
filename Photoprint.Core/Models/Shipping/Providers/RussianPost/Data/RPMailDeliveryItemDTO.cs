using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photoprint.Core.Models
{
    public class RPMailDeliveryItemDTO
    {
        public int ShippingId { get; set; }
        public RussianPostMailType MailType { get; set; }
        public RussianPostMailCategory MailCategory { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string PriceTitle { get; set; }
        public string DurationTitle { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }

    }
}
