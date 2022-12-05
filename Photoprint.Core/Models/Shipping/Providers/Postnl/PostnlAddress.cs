namespace Photoprint.Core.Models.Postnl
{
    public class PostnlAddress
    {
        public string AddressType { get; set; } // 01 Receiver, 02 Sender, 03 Alternative sender, 04 Collection address, 08 Return address, 09 Delivery address(for use with Pick up at PostNL location)
        public string Area{ get; set; }
        public string Buildingname{ get; set; }
        public string City{ get; set; }
        public string CompanyName{ get; set; }
        public string Countrycode{ get; set; }
        public string Department{ get; set; }
        public string Doorcode{ get; set; }
        public string FirstName{ get; set; }
        public string Floor{ get; set; }
        public string HouseNr{ get; set; }
        public string HouseNrExt{ get; set; }
        public string Name{ get; set; }
        public string Region{ get; set; }
        public string Street{ get; set; }
        public string StreetHouseNrExt{ get; set; } //Combination of Street, HouseNr and HouseNrExt.Please see Guidelines for the explanation.
        public string Zipcode{ get; set; }
    }
}
