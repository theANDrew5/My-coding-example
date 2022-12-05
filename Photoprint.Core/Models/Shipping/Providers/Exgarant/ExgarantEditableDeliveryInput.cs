using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photoprint.Core.Models
{
    public class ExgarantEditableDeliveryInput
    {
        public string Phone { get; set; }
        public string ExtensionNumber { get; set; }
        public int Places { get; set; } = 1;
        public DateTime DeliveryDate { get; set; } = DateTime.Now.AddDays(1);
        public string DeliveryIntervalStart { get; set; } = "10:00";
        public string DeliveryIntervalEnd { get; set; } = "18:00";
        public bool ReturnDocuments { get; set; } = false;
        public bool InformBySms { get; set; } = false;
        public bool Cash { get; set; } = false;
        public bool DeclaredPrice { get; set; } = false;
    }
}
