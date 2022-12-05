using System;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class AddressControl : BaseControl
{
        private ShippingAddress _address;

        public ShippingAddress Address
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(adressState.Value))
                {
                    var jObj = JObject.Parse(adressState.Value);
                    var dto = jObj["Address"]?.ToObject<AddressDTO>();
                    if (!(dto is null))
                    {
                        _address.Merge(dto);
                    }
                }
                return _address;
            }
            private set => _address = value;
        }

        private ShippingAddress _contractorAddress;

        public ShippingAddress ContractorAddress
        {
            get
            {
                if (string.IsNullOrWhiteSpace(adressState.Value)) return _contractorAddress;
                var jObj = JObject.Parse(adressState.Value);
                var dto = jObj["ContractorAddress"]?.ToObject<AddressDTO>();
                if (!(dto is null))
                {
                    _contractorAddress.Merge(dto);
                }
                return _contractorAddress;
            }
            private set => _contractorAddress = value;
        }

        private Shipping _currentShipping { get; set; }

        protected bool FullAddressMode =>
            _currentShipping.Type != ShippingType.Courier && !(_currentShipping is Postal postal &&
                                                               (postal.PostalType == PostalType.ToClientDelivery ||
                                                                postal.ShippingServiceProviderType ==
                                                                ShippingServiceProviderType.General));

        protected bool UseValidation
        {
            get
            {
                var isNewDeliveryAvalivable = PhotolabSettingsService
                    .GetSettings<DeliveryWindowSettings>(CurrentFrontend)?.IsNewDeliveryWindowEnabled ?? false;
                if (!(_currentShipping is Postal postal) || postal.ShippingServiceProviderType == ShippingServiceProviderType.General) return isNewDeliveryAvalivable;
                return isNewDeliveryAvalivable && (!postal.ServiceProviderSettings.UpdateShippingAddressesAutomatically || Address.DisableSynchronization);

            }
        }

        protected bool ContractorAddressAvailable { get; set; }

        private bool _contractorAddressNotEqual;

        public bool ContractorAddressNotEqual
        {
            get
            {
                if (string.IsNullOrWhiteSpace(adressState.Value)) return _contractorAddressNotEqual;
                var jObj = JObject.Parse(adressState.Value);
                _contractorAddressNotEqual = jObj["ContractorAddressNotEqual"]?.ToObject<bool>() ?? false;
                return _contractorAddressNotEqual;
            }
            set => _contractorAddressNotEqual = value;
        }
        protected bool IsNewDelivery { get; set; }

        protected bool CanEdit => !(_currentShipping is Postal postal) || postal.ShippingServiceProviderType == ShippingServiceProviderType.General 
                                    || !postal.ServiceProviderSettings.UpdateShippingAddressesAutomatically 
                                    || Address.DisableSynchronization;

        public void InitControl(ShippingAddress address, Shipping shipping,
            bool isContractorAddressAvailable = false,
            bool isContractorAddressNotEqual = false, ShippingAddress contractorAddress = null, bool isNewDelivery = false)
        {
            Address = address;
            _currentShipping = shipping;
            ContractorAddressAvailable = isContractorAddressAvailable;
            ContractorAddressNotEqual = isContractorAddressNotEqual;
            ContractorAddress = contractorAddress ?? new ShippingAddress() {IsContractorAddress = false};
            IsNewDelivery = isNewDelivery;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}