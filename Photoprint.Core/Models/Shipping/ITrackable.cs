namespace Photoprint.Core.Models
{
	public interface ITrackable
	{
		/// <summary>
		/// String used for url formating to display order tracking information
		/// </summary>
		string TrackingUrlFormatString { get; set; }

		/// <summary>
		/// Tracking site url
		/// </summary>
		string TrackingServiceUrl { get; set; }

		/// <summary>
		/// Tracking site url
		/// </summary>
		string TrackingFieldName { get; set; }
		

		bool IsTrackingSet { get; }

		bool IsGetRequest { get; }


		bool IsTrackingChangeAllowed { get; }
	}
}