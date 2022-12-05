using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexGoAvailableTariff
    {
        [JsonProperty("minimal_price")]
        public string MinimalPrice { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}