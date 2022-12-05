using System;
using System.Collections.Generic;
using System.Linq;
using Photoprint.Core.Models;
using Photoprint.Core.Repositories;
using Photoprint.Core.Services;

namespace Photoprint.DataAccessLayer.Proxies
{
	public class DistributionPointProxy : DistributionPoint
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IShippingPriceService _shippingPriceService;

        private IEnumerable<ShippingAddress> _shippingAddresses;

        private ShippingPrices _priceList;
        public override ShippingPrices PriceList
        {
            get => _priceList ?? _shippingPriceService
                    .GetById(new PhotolabSmall() { Id = PhotolabId }, Address.PriceId).PriceList;
            set => _priceList = value;
        }


        public override bool IsAvailableWeightConstrain => PriceList.IsAvailableWeightConstrain;
        public override double MaximumWeight => IsAvailableWeightConstrain ? PriceList.MaximumWeight : double.MaxValue;
            
        public DistributionPointProxy(IShippingAddressService shippingAddressService,
            IShippingPriceService shippingPriceService)
		{
			_shippingAddressService = shippingAddressService;
            _shippingPriceService = shippingPriceService;

            _shippingAddresses = null;
		}

		private ShippingAddress _contractorAddress;
		public override ShippingAddress ContractorAddress
		{
			get
			{
				if (_contractorAddress != null) return _contractorAddress;

				_shippingAddresses = _shippingAddresses ?? _shippingAddressService.GetList(this);
				_contractorAddress = _shippingAddresses.FirstOrDefault(x => x.IsContractorAddress) ?? new ShippingAddress(){IsContractorAddress = true};
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

                _shippingAddresses = _shippingAddresses ?? _shippingAddressService.GetList(this);
                var shippingAddress = _shippingAddresses.FirstOrDefault(x => !x.IsContractorAddress) ?? new ShippingAddress();

                _address = shippingAddress;

                return _address;
            }
            set => _address = value;
        }

    }
}