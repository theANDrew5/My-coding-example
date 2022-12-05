﻿using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryRecipient
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        
        [JsonProperty("middleName")]
        public string MiddleName { get; set; }
        
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("address")]
        public YandexDeliveryAddress Address { get; set; }
        
        [JsonProperty("pickupPointId")]
        public string PickupPointId { get; set; }

        [JsonProperty("fullAddress")]
        public string FullAddress { get; set; }
    }
}
