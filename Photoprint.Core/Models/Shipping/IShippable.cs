namespace Photoprint.Core.Models
{
	public interface IShippable : IDimensions
	{
		double TotalWeight { get; }
		int Quantity { get; }
	}
}