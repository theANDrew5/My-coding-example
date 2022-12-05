namespace Photoprint.Core.Models
{
    public class CDEKv2AddressInfo
    {
        public string RegionCode { get; set; }
        public string CityCode { get; set; }
        public string Type { get; set; }
        public string PvzCode {get;set;}
        public bool IsReception { get; set; }
        public bool IsHandout { get; set; }
        public CDEKv2Tariff SelectedTariff { get; set; }
    }
}
