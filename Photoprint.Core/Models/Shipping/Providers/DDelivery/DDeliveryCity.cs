using Newtonsoft.Json.Linq;

namespace Photoprint.Core.Models.DDelivery
{
	public class DDeliveryCity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Region { get; set; }
		public string DisplayName { get; set; }
		public string Area { get; set; }
		public string Priority { get; set; }
	}
}