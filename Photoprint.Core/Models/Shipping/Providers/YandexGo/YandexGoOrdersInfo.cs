using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class YandexGoOrdersInfo
    {
        [JsonProperty("claims")]
        public IReadOnlyCollection<YandexGoOrderInfo> Claims { get; }
    }
}