using System.Collections.Generic;
using System.IO;
using Photoprint.Core.Models;

namespace Photoprint.Core.Services
{
    public interface IShippingProviderService : IDeliveryPriceCalculator
    {
        ShippingServiceProviderImportResult SyncAddresses(Photolab photolab, Postal postal);

        void GetCreateOrderRegistration(Photolab photolab, Order order, IShippingServiceProviderSettings providerSettings, string comment = null, ShippingRegisterType type = ShippingRegisterType.RegisterAsSeparated);
        void GetDeleteOrderRegistration(Photolab photolab, Order order, IShippingServiceProviderSettings providerSettings);
        void GetShippingDocuments(Order order, Stream outputStream);
        AddressValidationResult TestDelivery(IShippingServiceProviderSettings settings, OrderAddress address, Postal postal);
        string GetLocalizedString(string input, Photolab photolab, Language language);
        OrderStatus GetOrderStatus(Photolab photolab, Order order, IShippingServiceProviderSettings providerSettings, bool skipHistory = false);
        IReadOnlyCollection<PaymentSystemType> GetRestrictedPaymentTypes(Order order);
    }
}