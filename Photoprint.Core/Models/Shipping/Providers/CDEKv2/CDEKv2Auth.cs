using Photoprint.Core.Configuration;

namespace Photoprint.Core.Models
{
    public class CDEKv2Auth
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsTestMode { get; set; }
        public bool UseProxy => Settings.UseProxyForCdek;
    }
}
