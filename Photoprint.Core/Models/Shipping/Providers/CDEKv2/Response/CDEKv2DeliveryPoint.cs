using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2DeliveryPoint
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public CDEKv2Location Location { get; set; }

        [JsonProperty("address_comment")]
        public string AddressComment { get; set; }

        [JsonProperty("nearest_station")]
        public string NearestStation { get; set; }

        [JsonProperty("nearest_metro_station")]
        public string NearestMetroStation { get; set; }

        [JsonProperty("work_time")]
        public string WorkTime { get; set; }

        [JsonProperty("phones")]
        public CDEKv2Phone[] Phones { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("owner_code")]
        public string OwnerCode { get; set; }

        [JsonProperty("take_only")]
        public bool TakeOnly { get; set; }

        [JsonProperty("is_dressing_room")]
        public bool IsDressingRoom { get; set; }

        [JsonProperty("is_handout")]
        public bool IsHandout { get; set; }

        [JsonProperty("is_reception")]
        public bool IsReception { get; set; }

        [JsonProperty("have_cashless")]
        public bool HaveCashless { get; set; }

        [JsonProperty("have_cash")]
        public bool HaveCash { get; set; }

        [JsonProperty("allowed_cod")]
        public bool AllowedCod { get; set; }

        [JsonProperty("weight_min")]
        public float? WeightMin { get; set; }

        [JsonProperty("weight_max")]
        public float? WeightMax { get; set; }
    }
}
