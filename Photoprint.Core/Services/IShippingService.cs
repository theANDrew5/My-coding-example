using System.Collections.Generic;
using JetBrains.Annotations;
using Photoprint.Core.Models;

namespace Photoprint.Core.Services
{
    public interface IShippingService
	{
        // CREATE
        Postal Create(PostalInput postalInput);
		DistributionPoint Create(DistributionPointInput input);
		Courier Create(CourierInput input);

		// UPDATE
		void Update(Shipping shipping);
        void UpdatePaymentBinding(Photolab photolab, Payment payment, IEnumerable<Shipping> shippings);
		void UpdatePositions(Dictionary<Shipping, int> positions);
		void UpdateShippingsBinding(Discount discount, IEnumerable<int> shippingsId);
		void UpdateMap(DistributionPoint distributionPoint, MapInput input);
		void UpdateShippingBindings(Photolab photolab, CustomWorkItem item, IReadOnlyCollection<ShippingSmall> shippings);

        int DuplicateShipping(Shipping sourceShipping, Photolab target);

		void ActivateBindedToCustomWorkItemShipping(int itemId, int shippingId, Photolab photolab);
		void DeactivateBindedToCustomWorkItemShipping(int itemId, int shippingId, Photolab photolab);

		void AddOperator(CompanyAccount account, User user, IEnumerable<int> shippingIds);
		void RemoveOperator(CompanyAccount account, User user, IEnumerable<int> shippingIds);

		// DELETE
		void Delete(int shippingId);

        // GET
		TK GetById<TK>(int shippingId) where TK : Shipping;

		IReadOnlyCollection<Shipping> GetList(int photolabId);
		IReadOnlyCollection<Shipping> GetList(CustomWorkItem item, int photolabId);
	    int GetShippingsCountByPhotolab(IEnumerable<Photolab> photolabs);

        IReadOnlyCollection<ShippingSmall> GetSmallList(Photolab photolab, bool hideDisabled, bool forMobile = false);
	    IReadOnlyCollection<ShippingSmall> GetSmallListByUser(User user, Photolab photolab, bool hideDisabled, out int totalCount);
	    IReadOnlyCollection<ShippingSmall> GetSmallListByIds(Photolab photolab, IEnumerable<int> shippingsIds);
		IReadOnlyCollection<Shipping> GetListByIds(IReadOnlyCollection<int> shippingsIds);
		IReadOnlyCollection<ShippingSmall> GetSmallListByCustomWorkItem(CustomWorkItem item, Photolab photolab);
        IReadOnlyCollection<ShippingSmallInCustomWorkItem> GetSmallListByCustomWorkItemWithActivity(CustomWorkItem item, Photolab photolab);
	    IReadOnlyCollection<ShippingSmallInCustomWorkItem> GetAllSmallListByCustomWorkItemWithActivity(CustomWorkItem item, Photolab photolab);

		IReadOnlyCollection<TK> GetList<TK>(int photolabId) where TK : Shipping;
		IReadOnlyCollection<TK> GetList<TK>(Photolab photolab) where TK : Shipping;
	    IReadOnlyCollection<TK> GetList<TK>(Photolab photolab, Payment payment) where TK : Shipping;

        IReadOnlyCollection<TK> GetListByServiceProvider<TK>(Photolab photolab, ShippingServiceProviderType serviceProvider) where TK : Shipping;

	    IReadOnlyCollection<TK> GetListByUser<TK>(User user, int photolabId, bool withAddress) where TK : Shipping;
	    IReadOnlyCollection<TK> GetAvailableList<TK>(Photolab photolab) where TK : Shipping;
	    IReadOnlyCollection<TK> GetAvailableList<TK>(Photolab photolab, IEnumerable<ShoppingCartItem> items, bool forMobileApp = false) where TK : Shipping;


        IEnumerable<TK> GetListByIds<TK>(IReadOnlyCollection<int> shippingIds) where TK : Shipping;
        IReadOnlyCollection<int> GetRestrictedIdsListByDiscount(Discount discount);

        UserShippingBindings GetShippingBindings([NotNull] CompanyAccount account, [NotNull] User user);
	    ShippingsToUsersBindingDTO GetUserRestrictionsAll(CompanyAccount account);
        bool TrySyncAddressRepository(Postal postal, IReadOnlyCollection<ShippingAddress> forDelete,
            IReadOnlyCollection<ShippingAddress> forUpdate, IReadOnlyCollection<ShippingAddress> forAdd, out IReadOnlyCollection<ShippingAddress> added);

        IReadOnlyCollection<Shipping> GetShippingUsages(ShippingServiceProviderType? type = null);
		IReadOnlyCollection<ShippingSmall> GetListWithPriceRestrictions(int photolabId, decimal? totalPrice);
        IReadOnlyCollection<int> GetAvailableShippingsIdsByCartItems(IEnumerable<ShoppingCartItem> items, Photolab photolab);
        IReadOnlyCollection<Postal> GetListByProviderType(ShippingServiceProviderType providerType);
    }
}