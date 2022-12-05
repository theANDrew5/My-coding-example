using System.Collections.Generic;
using System.Linq;

namespace Photoprint.Core.Models
{
    public class ParcelFixedPacking : IParcelPacking
    {
        public ParcelSize FixedSize { get; set; }

        public ParcelFixedPacking(int width, int height, int length)
        {
            FixedSize = new ParcelSize(width, height, length);
        }

        public ParcelSize GetParcelSize(IReadOnlyCollection<IPurchasableItem> items)
        {
            var maxWidth = items.Max(i => i.Width);
            var maxHeight = items.Max(i => i.Height);
            var maxLenght = items.Max(i => i.Length);

            return maxWidth > 0 && maxHeight > 0 && maxLenght > 0
                ? new ParcelSize(maxWidth, maxHeight, maxLenght) : FixedSize;
        }
    }
}