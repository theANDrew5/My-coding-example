using System.Collections.Generic;
using System.Linq;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.DataAccessLayer.Proxies
{
	public class CourierProxy : Courier
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IShippingPriceService _shippingPriceService;

        public override IReadOnlyCollection<ShippingAddress> ShippingAddresses => _shippingAddressService.GetList(this);

        public override IReadOnlyCollection<ShippingAddressPrice> ShippingAddressPrices => _shippingPriceService.GetListByShipping(this);

        public override bool IsAvailableWeightConstrain => ShippingAddressPrices.Any(ap => ap.PriceList.IsAvailableWeightConstrain);
        public override double MaximumWeight => IsAvailableWeightConstrain ? ShippingAddressPrices.Max(ap => ap.PriceList.MaximumWeight) : 0.0;

        public CourierProxy(IShippingAddressService shippingAddressService, IShippingPriceService shippingPriceService)
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

				_contractorAddress = ShippingAddresses.FirstOrDefault(x => x.IsContractorAddress) ?? new ShippingAddress(){IsContractorAddress = true};
				return _contractorAddress;
			}
			set => _contractorAddress = value;
		}

        private ShippingAddress _address;
        public override ShippingAddress Address
        {
            get
            {
                if (_address != null) return _address;

                var shippingAddress = ShippingAddresses.FirstOrDefault(x => !x.IsContractorAddress) ?? new ShippingAddress();
                _address = shippingAddress;

                return _address;
            }
            set => _address = value;
        }
    }
}