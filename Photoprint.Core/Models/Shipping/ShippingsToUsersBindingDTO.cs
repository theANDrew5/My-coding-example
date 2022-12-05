using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class ShippingsToUsersBindingDTO
    {
        private readonly Dictionary<int, Dictionary<int, List<int>>> _bindingsByUser;
        private readonly Dictionary<int, List<int>> _bindingsByShipping;

        public IReadOnlyCollection<int> GetShippingIdsByUser(User user, PhotolabSmall photolab)
        {
            if (_bindingsByUser == null) return Array.Empty<int>();
            if (_bindingsByUser.TryGetValue(user.Id, out var shippingsByPhotolab))
            {
                if (shippingsByPhotolab.TryGetValue(photolab.Id, out var result)) return result;
                return Array.Empty<int>();
            }
            return Array.Empty<int>();
        }

        public IReadOnlyCollection<int> GetUserIdsByShipping(Shipping shipping)
        {
            if (_bindingsByShipping == null || shipping == null) return Array.Empty<int>();
            if (_bindingsByShipping.TryGetValue(shipping.Id, out var result)) return result;
            return Array.Empty<int>();
        }

        public ShippingsToUsersBindingDTO(IReadOnlyCollection<(int userId, int photolabId, int shippingId)> bindings)
        {
            if (bindings == null || bindings.Count <= 0) return;

            var rawBindingsByUser = new Dictionary<int, Dictionary<int, List<int>>>();
            var rawBindingsByShipping = new Dictionary<int, List<int>>();
            foreach (var (uid, pid, sid) in bindings)
            {
                if (!rawBindingsByShipping.ContainsKey(sid)) rawBindingsByShipping.Add(sid, new List<int>());
                if (!rawBindingsByShipping[sid].Contains(uid)) rawBindingsByShipping[sid].Add(uid);

                if (!rawBindingsByUser.ContainsKey(uid)) 
                    rawBindingsByUser.Add(uid, new Dictionary<int, List<int>>());

                if (!rawBindingsByUser[uid].ContainsKey(pid))
                    rawBindingsByUser[uid].Add(pid, new List<int>());

                if (!rawBindingsByUser[uid][pid].Contains(sid))
                    rawBindingsByUser[uid][pid].Add(sid);
            }

            _bindingsByUser = rawBindingsByUser;
            _bindingsByShipping = rawBindingsByShipping;
        }
    }
}