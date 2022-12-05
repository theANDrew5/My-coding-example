using System;
using System.Collections.Generic;
using Photoprint.Core.Models;

namespace Photoprint.Core.Services
{
	public interface IShippingProviderResolverService
	{
		IShippingProviderService GetProvider(Postal postal);
	    IShippingProviderService GetProvider(ShippingServiceProviderType providerType);
	    Tuple<bool, IEnumerable<OrderActionResult>> RegisterOrderInShipping(Order order);
        IReadOnlyCollection<PaymentSystemType> GetRestrictedPaymentTypes(Order order);
        IShippingProviderService GetProvider(ShippingSmallToDeliveryModel shippingSmall);
    }
}