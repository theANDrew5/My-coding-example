using System;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdState
    {
        [XmlElement(ElementName = "clientOrderNr")]
        public string ClientOrderNr { get; set; }
        
        [XmlElement(ElementName = "clientParcelNr")]
        public string ClientParcelNr { get; set; }
        
        [XmlElement(ElementName = "dpdOrderNr")]
        public string DpdOrderNr { get; set; }
        
        [XmlElement(ElementName = "dpdParcelNr")]
        public string DpdParcelNr { get; set; }
        
        [XmlElement(ElementName = "pickupDate")]
        public DateTime PickupDate { get; set; }
        
        [XmlElement(ElementName = "dpdOrderReNr")]
        public string DpdOrderReNr { get; set; }
        
        [XmlElement(ElementName = "dpdParcelReNr")]
        public string DpdParcelReNr { get; set; }
        
        [XmlElement(ElementName = "isReturn")]
        public string IsReturn { get; set; }
        
        [XmlElement(ElementName = "planDeliveryDate")]
        public string PlanDeliveryDate { get; set; }
        
        [XmlElement(ElementName = "orderPhysicalWeight")]
        public string OrderPhysicalWeight { get; set; }
        
        [XmlElement(ElementName = "orderVolume")]
        public string OrderVolume { get; set; }
        
        [XmlElement(ElementName = "orderVolume")]
        public string OrderVolumeWeight { get; set; }
        
        [XmlElement(ElementName = "orderPayWeight")]
        public string OrderPayWeight { get; set; }
        
        [XmlElement(ElementName = "orderCost")]
        public string OrderCost { get; set; }
        
        [XmlElement(ElementName = "parcelPhysicalWeight")]
        public string ParcelPhysicalWeight { get; set; }
        
        [XmlElement(ElementName = "parcelVolume")]
        public string ParcelVolume { get; set; }
        
        [XmlElement(ElementName = "parcelVolumeWeight")]
        public string ParcelVolumeWeight { get; set; }
        
        [XmlElement(ElementName = "parcelPayWeight")]
        public string ParcelPayWeight { get; set; }
        
        [XmlElement(ElementName = "parcelLength")]
        public string ParcelLength { get; set; }
        
        [XmlElement(ElementName = "parcelWidth")]
        public string ParcelWidth { get; set; }
        
        [XmlElement(ElementName = "parcelHeight")]
        public string ParcelHeight { get; set; }
        
        [XmlElement(ElementName = "newState")]
        public DpdParcelStatus NewState { get; set; }
        
        [XmlElement(ElementName = "transitionTime")]
        public string TransitionTime { get; set; }
        
        [XmlElement(ElementName = "terminalCode")]
        public string TerminalCode { get; set; }
        
        [XmlElement(ElementName = "terminalCity")]
        public string TerminalCity { get; set; }
        
        [XmlElement(ElementName = "incidentCode")]
        public string IncidentCode { get; set; }
        
        [XmlElement(ElementName = "incidentName")]
        public string IncidentName { get; set; }
        
        [XmlElement(ElementName = "consignee")]
        public string Consignee { get; set; }
    }
}
