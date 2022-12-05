namespace Photoprint.Core.Models.Postnl
{
    public class PostnlLocation
    {
        public int LocationCode{get;set;}
        public string Name { get; set; }
        public int Distance { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public PostnlAddress Address { get; set; }
        public string PartnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string RetailNetworkID { get; set; }
        public string Saleschannel { get; set; }
        public string TerminalType { get; set; }
    }
}
