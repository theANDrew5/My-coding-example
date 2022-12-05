using System;

namespace Photoprint.Core.Models
{
    public static class OrderAddressDBConstraints
    {
        public static class Max
        {
            public const int FirstNameLength = 200;
            public const int LastNameLength = 200;
            public const int MiddleNameLength = 200;
            public const int CompanyNameLength = 250;
        }
    }

    // Дублирование ID и DeliveryProperties было сделано для классов ShippingAddress и OrderAddress специально
    // суть - у абстрактного Address нет никаких причин иметь подобные данные, так как это получается запутыванием контекста
    // например: вы передаете Address в качестве параметра метода, однако вне контекста вообще непонятно чьи это ID и DeliveryProperties
    // поэтому, чтобы избежать путаницы было принятно решение продублировать данные поля в классах
    public sealed class OrderAddress : Address, IId
    {
        public int Id { get; set; }
        public DeliveryAddressProperties DeliveryProperties { get; set; }


        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => _firstName = value.Cut(OrderAddressDBConstraints.Max.FirstNameLength);
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => _lastName = value.Cut(OrderAddressDBConstraints.Max.LastNameLength);
        }

        private string _middleName = string.Empty;
        public string MiddleName
        {
            get => _middleName;
            set => _middleName = value.Cut(OrderAddressDBConstraints.Max.MiddleNameLength);
        }

        private string _companyName = string.Empty;
        public string CompanyName
        {
            get => _companyName;
            set => _companyName = value.Cut(OrderAddressDBConstraints.Max.CompanyNameLength);
        }

        private string _fullName;
        public string FullName => _fullName ?? (_fullName = (FirstName + " " + (string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : (MiddleName + " ")) + LastName).Trim());

        public int? ShippingAddressId { get; set; }

        public OrderAddress() : base()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            MiddleName = string.Empty;
            CompanyName = string.Empty;

            DeliveryProperties = new DeliveryAddressProperties();
        }

        public OrderAddress(ShippingAddress shippingsAddress) : base(shippingsAddress)
        {
            ShippingAddressId = shippingsAddress?.Id;

            FirstName = string.Empty;
            LastName = string.Empty;
            MiddleName = string.Empty;
            CompanyName = string.Empty;

            DeliveryProperties = new DeliveryAddressProperties(shippingsAddress?.DeliveryProperties?.SerializeToString());
        }

        public OrderAddress(ShippingAddress shippingsAddress, string firstName, string lastName, string phone = null, string middleName = null) 
            : this(shippingsAddress)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            MiddleName = middleName;
        }

        public string ToAlternativeString(bool includeCompanyname)
        {
            var result = string.Empty;
            if (includeCompanyname)
            {
                result = string.Format("{1}\n{0}", result, CompanyName);
            }

            if ((!string.IsNullOrWhiteSpace(AddressLine1) || !string.IsNullOrWhiteSpace(AddressLine2)) && string.IsNullOrEmpty(Street) && string.IsNullOrEmpty(House))
            {
                result = Utility.AddressMaker(result, AddressLine1);
                result = Utility.AddressMaker(result, AddressLine2);
            }
            else
            {
                result = Utility.AddressMaker(result, House);
                result += string.Format(" {0}", Street);
                result = Utility.AddressMaker(result, Flat);
            }
            result = Utility.AddressMaker(result, City);
            result = Utility.AddressMaker(result, Region);
            result = Utility.AddressMaker(result, PostalCode);

            if (!includeCompanyname)
            {
                result = Utility.AddressMaker(result, Country);
            }

            if (!string.IsNullOrWhiteSpace(Description) && !includeCompanyname)
            {
                result = $"{result} ({Description})";
            }

            return result;
        }

        public string ToString(bool includeZipCode, bool includeCountry, bool useAlternativeOrder)
        {
            var result = useAlternativeOrder ? ToAlternativeString(true) : ToString(includeZipCode, includeCountry);
            return string.Format("{1}\n{0}", result, CompanyName);
        }


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


        public OrderAddress Clone(CloneParams @params = null)
        {
            OrderAddress newOrderAddress;

            if (@params == null || !@params.HideAddress)
            {
                newOrderAddress = new OrderAddress
                {
                    City = City ?? string.Empty,
                    Country = Country ?? string.Empty,
                    District = District ?? string.Empty,
                    Region = Region ?? string.Empty,
                    PostalCode = PostalCode ?? string.Empty,
                    AddressLine1 = AddressLine1 ?? string.Empty,
                    AddressLine2 = AddressLine2 ?? string.Empty,
                    Street = Street ?? string.Empty,
                    House = House ?? string.Empty,
                    Flat = Flat ?? string.Empty,
                    Latitude = Latitude ?? string.Empty,
                    Longitude = Longitude ?? string.Empty,
                    Description = Description ?? string.Empty
                };
            }
            else
            {
                newOrderAddress = new OrderAddress();
            }

            newOrderAddress.Phone = Phone ?? string.Empty;
            newOrderAddress.FirstName = FirstName ?? string.Empty;
            newOrderAddress.LastName = LastName ?? string.Empty;
            newOrderAddress.MiddleName = MiddleName ?? string.Empty;
            newOrderAddress.ShippingAddressId = ShippingAddressId;
            newOrderAddress.DeliveryProperties = new DeliveryAddressProperties(DeliveryProperties?.SerializeToString());

            return newOrderAddress;
        }

        // Inner classes
        public class CloneParams
        {
            public bool HideAddress { get; set; }
        }
    }
}
