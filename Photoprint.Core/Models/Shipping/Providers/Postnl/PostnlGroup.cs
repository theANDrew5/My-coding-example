namespace Photoprint.Core.Models.Postnl
{
    public class PostnlGroup
    {
        public string GroupType { get; set; } // Group sort that determines the type of group that is indicated. 01 Collection request, 03 Multiple parcels in one shipment (multicolli), 04 Single parcel in one shipment
        public string GroupSequence { get; set; } // Sequence number of the collo within the complete shipment (e.g.collo 2 of 4) Mandatory for multicollo shipments
        public string GroupCount { get; set; } // Total number of colli in the shipment(in case of multicolli shipments) Mandatory for multicollo shipments
        public string MainBarcode { get; set; } // Main barcode for the shipment(in case of multicolli shipments) Mandatory for multicollo shipments
    }
}
