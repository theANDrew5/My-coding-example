using System.IO;

namespace Photoprint.Core.Models
{
	public class MapInput
	{
		public MapType MapType { get; set; }
		public Stream MapFile { get; set; }

		public int ThumbnailWidth { get; set; }
		public int ThumbnailHeight { get; set; }

		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public int Zoom { get; set; }

		public string CustomMapCode { get; set; }
	}
}