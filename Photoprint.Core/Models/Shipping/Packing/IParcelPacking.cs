using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public interface IParcelPacking
    {
        ParcelSize GetParcelSize(IReadOnlyCollection<IPurchasableItem> items);
    }
}
