using System.Collections.Generic;
using System.Linq;
using NLog;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.Core.Repositories
{
    public interface IShippingAddressRepository
	{
		ShippingAddress Create(ShippingAddress shippingAddress);
        IReadOnlyCollection<ShippingAddress> CreateList(IReadOnlyCollection<ShippingAddress> addresses);
		void Update(ShippingAddress shippingAddress);
        void UpdateList(IReadOnlyCollection<ShippingAddress> addresses);
        void UpdateListStatus(IReadOnlyCollection<int> addressesIds, int shippingId, bool status);
        void UpdateShippingAddressPositions(IDictionary<int, int> positions);
	    void Copy(int sourceShippingId, int targetShippingId, int targetPhotolabId);
        void Delete(IReadOnlyCollection<int> shippingAddressIds);

		ShippingAddress GetById(int shippingAddressId);
        IReadOnlyCollection<ShippingAddress> GetListByIds(IReadOnlyCollection<int> addressesIds);


        IReadOnlyCollection<ShippingAddress> GetList(int shippingId);

        void DeleteAll(int shippingId);
        void SetNewAddressList(int shippitngId, IReadOnlyCollection<ShippingAddress> addresses);

        IReadOnlyCollection<(ShippingSmallToDeliveryModel shipping, ShippingAddress address)>
            GetShippingsSmallWithAddresses(int photolabId,
                decimal currentOrderPrice,
                decimal currentOrderWeight,
                string cityTitle,
                IReadOnlyCollection<int> availableShippingsIds);
    }
}