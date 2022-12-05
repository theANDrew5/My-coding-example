using System.Collections.Generic;
using Photoprint.Core.Models;

namespace Photoprint.Core.Repositories
{
	public interface IShippingRepository
	{
        // CREATE
		Shipping Create(Shipping shipping);

        // GET
        Shipping GetById(int shippingId);

        IReadOnlyCollection<Shipping> GetList(int customWorkItemId, int photolabId);
	    IReadOnlyCollection<Shipping> GetList(int photolabId, ShippingType? type = null, bool isExistingAddress = false);
	    IReadOnlyCollection<int> GetIdsListByServiceProvider(int photolabId, ShippingType shippingType, ShippingServiceProviderType serviceProvider);
	    IReadOnlyCollection<Shipping> GetListByPayment(int photolabId, ShippingType type, int paymentId, bool isExistingAddress);
	    IReadOnlyCollection<Shipping> GetListByPayment(int photolabId, int paymentId, bool isExistingAddress);
	    IReadOnlyCollection<Shipping> GetListByUser(int userId, int photolabId,  bool isExistingAddress);
        IReadOnlyCollection<ShippingSmall> GetSmallList(int photolabId, bool hideDisabled, bool forMobile = false);
		IReadOnlyCollection<Shipping> GetListByIds(IReadOnlyCollection<int> shippingIds);
		
		IReadOnlyCollection<ShippingSmall> GetSmallListByCustomWorkItem(CustomWorkItem item, Photolab photolab);
	    IReadOnlyCollection<ShippingSmallInCustomWorkItem> GetSmallListByCustomWorkItemWithActivity(CustomWorkItem item, Photolab photolab);
        IReadOnlyCollection<ShippingSmallInCustomWorkItem> GetConstrainedSmallListByCustomWorkItemWithActivity(CustomWorkItem item, Photolab photolab);
        IReadOnlyCollection<ShippingWithUsersDTO> GetUsersInShippings();

        IReadOnlyDictionary<int, IReadOnlyCollection<int>> GetUserRestrictions(int userId);
	    ShippingsToUsersBindingDTO GetUserRestrictionsAll(int companyAccountId);

        IReadOnlyCollection<int> GetIdsListByCartItems(IReadOnlyCollection<int> materialsIds, IReadOnlyCollection<int> productsIds, IReadOnlyCollection<int> gfProductsIds, int photolabId);
	    IReadOnlyCollection<int> GetIdsListByCustomWorksItems(IReadOnlyCollection<int> itemsIds, int photolabId);
        IReadOnlyDictionary<int, IReadOnlyCollection<int>> GetRestrictionsListByDiscount(int photolabId);

        int GetShippingsCountByPhotolab(IEnumerable<int> photolabIds);

        // UPDATE
        void Update(Shipping shipping);
        void UpdateDiscountBindings(int discountId, IEnumerable<int> shippingsIds);

        void AddOperator(int userId, IEnumerable<int> shippingIds);
	    void RemoveOperator(int userId, IEnumerable<int> shippingIds);

	    void BindShippingToPayment(int shippingId, int paymentId);
	    void UnbindShippingFromPayment(int shippingId, int paymentId);

	    void BindShippingsToCustomWorkItem(IReadOnlyCollection<int> shippingIds, CustomWorkItem item);
	    void UnbindShippingsFromCustomWorkItem(IReadOnlyCollection<int> shippingIds, CustomWorkItem item);

	    void ActivateBindingToCustomWortItemShipping(int itemId, int shippingId);
	    void DeactivateBindingToCustomWortItemShipping(int itemId, int shippingId);

        // DELETE
        void Delete(int shippingId);
        IReadOnlyCollection<Shipping> GetShippingUsages(ShippingServiceProviderType? type = null);
        IReadOnlyCollection<Shipping> GetListByProviderType(ShippingServiceProviderType providerType);
    }
}