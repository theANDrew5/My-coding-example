using System;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class BaseDeliveryRequest
    {
        public int FrontendId { get; set; }
        public int? LanguageId { get; set; }
        public string InstanceGuid { get; set; }

    }
}