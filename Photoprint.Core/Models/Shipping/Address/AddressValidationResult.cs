using System;

namespace Photoprint.Core.Models
{
    public class AddressValidationResult
    {
        public static AddressValidationResult GetValidState()
        {
            return new AddressValidationResult()
            {
                IsCityValid = true,
                IsFlatValid = true,
                IsHouseValid = true,
                IsNameValid = true,
                IsPhoneValid = true,
                IsStreetValid = true,
                IsRegionValid = true,
                UnknownError = false
            };
        }

        public bool IsValid => !UnknownError && 
                               IsRegionValid.GetValueOrDefault(true) &&
                               IsCityValid.GetValueOrDefault(true) && 
                               IsStreetValid.GetValueOrDefault(true) && 
                               IsHouseValid.GetValueOrDefault(true) && 
                               IsPhoneValid.GetValueOrDefault(true) && 
                               IsFlatValid.GetValueOrDefault(true) && 
                               IsNameValid.GetValueOrDefault(true) &&
                               IsIndexValid.GetValueOrDefault(true);

        public string Message { get; set; } = String.Empty;
        public bool? IsRegionValid { get; set; }
        public bool? IsCityValid { get; set; }
        public bool? IsStreetValid { get; set; }
        public bool? IsHouseValid { get; set; }
        public bool? IsPhoneValid { get; set; }
        public bool? IsFlatValid { get; set; }
        public bool? IsNameValid { get; set; }
        public bool? IsIndexValid { get; set; }
        public bool UnknownError { get; set; }
    }
}