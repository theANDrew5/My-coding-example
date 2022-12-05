using System.Collections.Generic;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class AvailableShippingResult
    {
        public IReadOnlyList<int> ShippingList { get; }
        public bool IsDefaultType { get; }
        public string Title { get; }
        public string TitleNote { get; }           
        public DeliveryDisplayType Type { get; }
        public int AddressCount { get; }

        public AvailableShippingResult(DeliveryDisplayType type, IReadOnlyList<int> shippingIds, string title, string titleNote, int addressCount, double maxWeight, bool isDefault)
        {
            ShippingList = shippingIds;
            IsDefaultType = isDefault;
            Type = type;
            Title = title;
            TitleNote = titleNote;
            AddressCount = addressCount;
        }

        public class PrefinalData
        {
            public List<int> ShippingList { get; }
            public bool IsDefaultType { get; set; }
            public double MaxWeight { get; set; }
            public int AddressCount { get; set; }

            public PrefinalData(int shippingId)
            {
                ShippingList = new List<int> { shippingId };
            }
        }
    }
}