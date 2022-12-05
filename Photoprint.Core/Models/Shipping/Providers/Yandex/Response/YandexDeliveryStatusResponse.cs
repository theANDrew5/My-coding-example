using Newtonsoft.Json;
using System;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryStatusResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("status")]
        public YandexDeliveryStatus Status { get; set; }
    }

    public class YandexDeliveryStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("datetime")]
        public DateTime Datetime { get; set; }
    }
    }
