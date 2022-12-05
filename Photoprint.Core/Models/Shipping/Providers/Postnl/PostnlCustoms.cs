using System.Collections.Generic;

namespace Photoprint.Core.Models.Postnl
{
    // The Customs type is mandatory for GlobalPack shipments and not allowed for any other shipment types.
    public class PostnlCustoms
    {
        public bool Certificate { get; set; } // At least one of the three types Certificate, Invoice or License is mandatory for Commercial Goods, Commercial Sample and Returned Goods
        public string CertificateNr { get; set; } // Mandatory when Certificate is true.
        public bool License { get; set; } // At least one of the three types Certificate, Invoice or License is mandatory for Commercial Goods, Commercial Sample and Returned Goods
        public string LicenseNr { get; set; } // Mandatory when License is true.
        public bool Invoice { get; set; } // At least one of the three types Certificate, Invoice or License is mandatory for Commercial Goods, Commercial Sample and Returned Goods
        public string InvoiceNr { get; set; } // Mandatory when Invoice is true
        public bool HandleAsNonDeliverable { get; set; } // Determines what to do when the shipment cannot be delivered the first time (if this is set to true, the shipment will be returned after the first failed attempt)
        public string Currency { get; set; } // Currency code,only EUR and USS are allowed
        public string ShipmentType { get; set; } //Type of shipment,possible values: Gift,Documents,Commercial Goods, Commercial Sample,Returned Goods
        public IEnumerable<PostnlContent> Content { get; set; }
    }
}
