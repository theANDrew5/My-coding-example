using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
		public class YandexSuggestResult
        {
			[JsonProperty("name")]
			public string Name { get; set; }
			[JsonProperty("local")]
			public bool Local { get; set; }
            [JsonProperty("type")]
			public string Type { get; set; }
            [JsonProperty("desc")]
			public string Description { get; set; }
            [JsonProperty("lat")]
			public string Latitude { get; set; }
            [JsonProperty("lon")]
			public string Longitude { get; set; }
            [JsonProperty("kind")]
			public string Kind { get; set; }
			[JsonProperty("geoid")]
			public string GeoId { get; set; }
        }
}