using System.Collections.Generic;
using System.Linq;

namespace Photoprint.Core.Models
{
    public class UserShippingBindings
    {
        public class ShippingBinding
        {
            public bool IsAllAvailable { get; }
            public IReadOnlyCollection<int> ShippingIds { get; }

            public ShippingBinding(IReadOnlyCollection<int> shippingIds)
            {
                IsAllAvailable = shippingIds == null || shippingIds.Count == 0;
                ShippingIds = shippingIds;
            }

            public bool HasAccessTo(int shippingId)
            {
                return IsAllAvailable || ShippingIds != null && ShippingIds.Contains(shippingId);
            }
        }
        public int UserId { get; }
        public IReadOnlyDictionary<int, ShippingBinding> Bindings { get; }

        public bool HasAccessTo(int photolabId, int shippingId)
        {
            if (Bindings.ContainsKey(photolabId))
            {
                if (Bindings[photolabId] == null) return false;
                return Bindings[photolabId].HasAccessTo(shippingId);
            }
            return false;
        }

        public UserShippingBindings(int userId, IReadOnlyDictionary<int, ShippingBinding> bindings)
        {
            UserId = userId;
            Bindings = bindings;
        }
    }
}