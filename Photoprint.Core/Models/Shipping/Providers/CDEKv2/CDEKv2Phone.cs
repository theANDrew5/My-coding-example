using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Phone
    {
        private readonly Regex _phoneClean = new Regex("[^0-9+]+", RegexOptions.Compiled);
        [JsonProperty("number")]
        private string _number;
        public string Number { get => _phoneClean.Replace(_number,""); set => _number = value; }
    }
}
