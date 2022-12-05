using System;
using System.Collections.Generic;
using System.Linq;
using Photoprint.Core.Models;
using Photoprint.Core.Repositories;
using Photoprint.Core.Services;

namespace Photoprint.DataAccessLayer.Proxies
{
    public class PostalProxy : Postal
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IShippingPriceService _shippingPriceService;

        
        public override IReadOnlyCollection<ShippingAddress> ShippingAddresses =>
            _shippingAddressService.GetList(this);

        public override IReadOnlyCollection<ShippingAddressPrice> ShippingAddressPrices => _shippingPriceService.GetListByShipping(this);

        private double? WeightConstrainByProvider => ShippingAddresses.Max(sa => sa.MaxWeight);

        private double? WeightConstrainByPrice
        {
            get
            {
                var priceWithWeightConstrain =
                    ShippingAddressPrices.Where(ap => ap.PriceList.IsAvailableWeightConstrain).ToList();
                if (!priceWithWeightConstrain.Any()) return null;
                return priceWithWeightConstrain.Max(ap => ap.PriceList.MaximumWeight);
            }
        }

        public override bool IsAvailableWeightConstrain => WeightConstrainByProvider.HasValue || WeightConstrainByPrice.HasValue;
        // ShippingAddress.MaxWeight - максимальный вес от провайдера
        // ShippingAddressPrice.MaximumWeight - максимальный вес, заданный в настройках
        // За максимальный вес шиппинга берём минимальное значение обоих максимумумов обоих параметров
        // потому что они перекрывают друг друга
        public override double MaximumWeight => IsAvailableWeightConstrain 
            ? Math.Min(WeightConstrainByProvider ?? double.MaxValue, WeightConstrainByPrice ?? double.MaxValue)
            : double.MaxValue;

        public PostalProxy(IShippingAddressService shippingAddressService, IShippingPriceService shippingPriceService)
        {
            _shippingAddressService = shippingAddressService;
            _shippingPriceService = shippingPriceService;
        }

        private ShippingAddress _contractorAddress;

        public override ShippingAddress ContractorAddress
        {
            get
            {
                if (_contractorAddress != null) return _contractorAddress;

                _contractorAddress = ShippingAddresses.FirstOrDefault(x => x.IsContractorAddress) ??
                                     new ShippingAddress() { IsContractorAddress = true };
                return _contractorAddress;
            }
            set => _contractorAddress = value;
        }
    }
}