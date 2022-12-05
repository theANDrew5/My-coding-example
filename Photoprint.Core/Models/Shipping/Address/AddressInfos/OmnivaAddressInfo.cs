using System;
namespace Photoprint.Core.Models
{
    public class OmnivaAddressInfo
    {
        public OmnivaLocationType Type { get; set; }
        public string PostalCode { get; set; }
    }
    public enum OmnivaLocationType : byte
    {
        ParcelMachine = 0,
        PostOffice = 1
    }
}
