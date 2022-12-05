using System;
using System.Text;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public static class ShippingAddressDBConstraints
    {
        public static class Max
        {
            public const int AddressNameLength = 200;
            public const int ShippingHoursDescriptionLength = 500;
            public const int WorkTimeLength = 300;
            public const int ProcurementTimeLength = 250;
        }
    }

    // Дублирование ID и DeliveryProperties было сделано для классов ShippingAddress и OrderAddress специально
    // суть - у абстрактного Address нет никаких причин иметь подобные данные, так как это получается запутыванием контекста
    // например: вы передаете Address в качестве параметра метода, однако вне контекста вообще непонятно чьи это ID и DeliveryProperties
    // поэтому, чтобы избежать путаницы было принятно решение продублировать данные поля в классах
    public sealed class ShippingAddress : Address, IId
    {
        public int Id { get; set; }
        public DeliveryAddressProperties DeliveryProperties { get; set; }

        public int ShippingId { get; set; }
        public bool AvailableOnSite { get; set; }
        public int MinimumShippingHours { get; set; }
        public int MaximumShippingHours { get; set; }
        public LocalizableString TitleLocalized { get; set; }
        public double? MaxWeight { get; set; } // ИСПОЛЬЗУЕТСЯ ТОЛЬКО В ТК ДОСТАВКАХ
        public bool IsProviderWeightConstrain => MaxWeight.HasValue;
        public bool IsContractorAddress { get; set; }
        public bool DisableSynchronization { get; set; }
        public int? PriceId { get; set; }


        private string _addressName = string.Empty;
        public string AddressName
        {
            get => _addressName;
            set => _addressName = value.Cut(ShippingAddressDBConstraints.Max.AddressNameLength);
        }

        private string _shippingHoursDescription = string.Empty;
        public string ShippingHoursDescription
        {
            get => _shippingHoursDescription;
            set => _shippingHoursDescription = value.Cut(ShippingAddressDBConstraints.Max.ShippingHoursDescriptionLength);
        }

        private string _procurementTime = string.Empty;
        public string ProcurementTime
        {
            get => _procurementTime;
            set => _procurementTime = value.Cut(ShippingAddressDBConstraints.Max.ProcurementTimeLength);
        }

        private string _workTime = string.Empty;
        public string WorkTime
        {
            get => _workTime;
            set => _workTime = value.Cut(ShippingAddressDBConstraints.Max.WorkTimeLength);
        }

        private int? _position;
        public int Position
	    {
	        get => (int) (_position ?? (_position = 10000));
	        set => _position = value;
	    }

        public string DisplayDescription
        {
            get
            {
                var displayDescription = new StringBuilder(Description);
                if (!string.IsNullOrEmpty(WorkTime) || !string.IsNullOrEmpty(ProcurementTime) || !string.IsNullOrEmpty(Phone))
                {
                    displayDescription.Append(" (");
                    if (!string.IsNullOrEmpty(Phone))
                    {
                        displayDescription.Append(Phone);
                        if (!string.IsNullOrEmpty(WorkTime) || !string.IsNullOrEmpty(ProcurementTime))
                        {
                            displayDescription.Append(", ");
                        }
                    }
                    if (!string.IsNullOrEmpty(WorkTime))
                    {
                        displayDescription.Append(WorkTime);
                        if (!string.IsNullOrEmpty(ProcurementTime))
                        {
                            displayDescription.Append(", ");
                        }
                    }
                    if (!string.IsNullOrEmpty(ProcurementTime))
                    {
                        displayDescription.Append($"{ProcurementTime}");
                    }
                    displayDescription.Append(")");
                }

                return displayDescription.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShippingAddress address)) return false;
            return
                base.Equals(address) &&
                //ShippingId == address.ShippingId &&
                AddressName.Trim().Equals(address.AddressName.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                ShippingHoursDescription.Trim().Equals(address.ShippingHoursDescription.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                ProcurementTime.Trim().Equals(address.ProcurementTime.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                WorkTime.Trim().Equals(address.WorkTime.Trim(),
                    StringComparison.InvariantCultureIgnoreCase);
        }

        public void Merge(ShippingAddress address)
        {
            base.Merge(address);
            //ShippingId = address.ShippingId;
            AddressName = address.AddressName;
            ShippingHoursDescription = address.ShippingHoursDescription;
            ProcurementTime = address.ProcurementTime;
            WorkTime = address.WorkTime;
            MaxWeight = address.MaxWeight;
        }

        public ShippingAddress() : base()
        {
            AvailableOnSite = true;
            AddressName = string.Empty;
            MinimumShippingHours = 0;
            MaximumShippingHours = 0;
            ShippingHoursDescription = string.Empty;
            TitleLocalized = new LocalizableString();
            MaxWeight = null;
            IsContractorAddress = false;
            ProcurementTime = string.Empty;
            WorkTime = null;
            DisableSynchronization = false;
            PriceId = null;
            Position = 0;

            DeliveryProperties = new DeliveryAddressProperties();
        }

        public ShippingAddress(ShippingAddress shippingsAddress) : base(shippingsAddress)
        {
            AvailableOnSite = shippingsAddress.AvailableOnSite;
            AddressName = shippingsAddress.AddressName;
            MinimumShippingHours = shippingsAddress.MinimumShippingHours;
            MaximumShippingHours = shippingsAddress.MaximumShippingHours;
            ShippingHoursDescription = shippingsAddress.ShippingHoursDescription;
            TitleLocalized = LocalizableString.FromJson(TitleLocalized?.ToJson());
            MaxWeight = shippingsAddress.MaxWeight;
            IsContractorAddress = shippingsAddress.IsContractorAddress;
            ProcurementTime = shippingsAddress.ProcurementTime;
            WorkTime = shippingsAddress.WorkTime;
            DisableSynchronization = shippingsAddress.DisableSynchronization;
            PriceId = shippingsAddress.PriceId;
            Position = shippingsAddress.Position;

            DeliveryProperties = new DeliveryAddressProperties(shippingsAddress?.DeliveryProperties?.SerializeToString());
        }

        public ShippingAddress(Address address, DeliveryAddressProperties properties) : base(address)
        {
            DeliveryProperties = new DeliveryAddressProperties(properties.SerializeToString());
        }

        public ShippingAddress(ShippingAddressDTO dto): this(new Address(dto), dto.DeliveryProperties.ToObject<DeliveryAddressProperties>())
        {
            Id = dto.Id ?? 0;
            ShippingId = dto.ShippingId;
        }
        public override string ToString() => base.ToString();


        private string _cacheTag;
        public string GetCacheTag()
        {
            if (_cacheTag == null)
            {
                _cacheTag = GetCacheTag(GetType(), Id);
            }
            return _cacheTag;
        }
        public static string GetCacheTag(Type type, int id)
        {
            return $"{type.Name.ToLower()} (id:{id})";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}