using System.Collections.Generic;

namespace Photoprint.Core.Models.Postnl
{
    public class PostnlRequestData
    {
        public PostnlCustomer Customer { get; set; }
        public PostnlShipments Shipments { get; set; }
        public string ProductCodeDelivery { get; set; }
    }
    public class PostnlShipment
    {
        public string MainBarcode { get; set; }
        public string Barcode { get; set; }
        public string ShipmentAmount { get; set; }
        public string ShipmentCounter { get; set; }
        public PostnlCustomer Customer { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string Reference { get; set; }
        public string DeliveryDate { get; set; }
        public PostnlDimension Dimension { get; set; }
        public PostnlAmount Amount { get; set; }
        public IEnumerable<PostnlAddress> Address { get; set; }
        public string Event { get; set; }
        public string Expectation { get; set; }
        public IEnumerable<PostnlProductOption> ProductOptions { get; set; }
        public PostnlStatus Status { get; set; }
        public IEnumerable<PostnlStatus> OldStatus { get; set; }
    }

    public class PostnlShipments
    {
        public PostnlCustomer Customer { get; set; }
        public IEnumerable<PostnlAddress> Addresses { get; set; }
        public IEnumerable<PostnlAmount> Amounts { get; set; }
        public string Barcode { get; set; }
        public string CollectionTimeStampStart { get; set; }
        public string CollectionTimeStampEnd { get; set; }
        public IEnumerable<PostnlContact> Contacts { get; set; } // Content of the shipment.Mandatory for Extra @Home shipments
        public string CostCenter { get; set; } // Cost center of the shipment.This value will appear on your invoice
        public string CustomerOrderNumber { get; set; } // Order number of the customer
        public IEnumerable<PostnlCustoms> Customs { get; set; }
        public string DeliveryAddress { get; set; } // Delivery address specification.This field is mandatory when AddressType on the Address is 09.
        public string DeliveryDate { get; set; } // The delivery date of the shipment.We strongly advice to use the DeliveryDate service to get this date when using delivery options like Evening/Morning/Sameday delivery etc.For those products, this field is mandatory (please regard the Guidelines section). Format: dd-MM-yyyy hh:mm:ss
        public string DeliveryTimeStampStart { get; set; } // The delivery date and the specific starting time of the shipment delivery. This can be retrieved from the DeliveryDate webservice using the MyTime delivery option.Format: dd-MM-yyyy hh:mm:ss
        public string DeliveryTimeStampEnd { get; set; } // The delivery date and the specific end time of the shipment delivery.This can be retrieved from the DeliveryDate webservice using the MyTime delivery option.Format: dd-MM-yyyy hh:mm:ss
        public IEnumerable<PostnlDimension> Dimension { get; set; }
        public string DownPartnerBarcode { get; set; } // Barcode of the downstream network partner of PostNL Pakketten.
        public string DownPartnerID { get; set; } // Identification of the downstream network partner of PostNL Pakketten.
        public string DownPartnerLocation { get; set; } //Identification of the location of the downstream network partner of PostNL Pakketten.Mandatory for Pickup at PostNl Location Belgium: LD-01
        public IEnumerable<PostnlGroup> Groups { get; set; }
        public int IDType { get; set; } // Type of the ID document.Mandatory for ID Check products.See Products for possible values
        public string IDNumber { get; set; } // Document number of the ID document.Mandatory for ID Check products
        public string IDExpiration { get; set; } // Expiration date of the ID document.Mandatory for ID Check products
        public int ProductCodeCollect { get; set; } // Second product code of a shipment
        public string ProductCodeDelivery { get; set; } // Product code of the shipment
        public IEnumerable<PostnlProductOption> ProductOptions { get; set; } // Product options for the shipment, mandatory for certain products, see the Products page of this webservice
        public string ReceiverDateOfBirth { get; set; } // Date of birth.Mandatory for Age check products
        public string Reference { get; set; } // Your own reference of the shipment.Mandatory for Extra @Home shipments; for E @H this is used to create your order number, so this should be unique for each request.
        public string ReferenceCollect { get; set; } // Additional reference of the collect order of the shipment
        public string Remark { get; set; } // Remark of the shipment.
        public string ReturnBarcode { get; set; } // Return barcode of the shipment. Mandatory for Label in the Box (return label) shipments.
        public string ReturnReference { get; set; } // Return reference of the shipment
        public string TimeslotID { get; set; } // ID of the chosen timeslot as returned by the timeslot webservice
    }

    public class PostnlCustomer
    {
        public string CustomerCode { get; set; }
        public string CustomerNumber { get; set; }
        public string Name { get; set; }
    }

    public class PostnlDimension
    {
        public string Height { get; set; } // Height of the shipment in milimeters(mm).
        public string Length { get; set; } // Length of the shipment in milimeters(mm).
        public string Volume { get; set; } // Volume of the shipment in centimeters(cm3). Mandatory for E @H-products
        public string Weight { get; set; } // Weight of the shipment in grams. Approximate weight suffices
        public string Width { get; set; } // Width of the shipment in milimeters(mm).
    }

    public class PostnlProductOption
    {
        public string Characteristic { get; set; } // The characteristic of the ProductOption.Mandatory for some products, please see the Products page
        public string Option { get; set; } // The product option code for this ProductOption.Mandatory for some products, please see the Products page
    }

    public class PostnlStatus
    {
        public string TimeStamp { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string PhaseCode { get; set; }
        public string PhaseDescription { get; set; }
    }
}
