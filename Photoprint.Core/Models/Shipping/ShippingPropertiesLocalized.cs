namespace Photoprint.Core.Models
{
    public class ShippingPropertiesLocalized: EntityProperties
    {
        public ShippingPropertiesLocalized() : base() { }
        public ShippingPropertiesLocalized(string xml) : base(xml) { }

        private const string _titleLocalizedKey = "titleLocalized";
        private LocalizableString _titleLocalized;
        public LocalizableString TitleLocalized
        {
            get
            {
                if (_titleLocalized != null) return _titleLocalized;
                if (!string.IsNullOrWhiteSpace(base[_titleLocalizedKey]))
                {
                    _titleLocalized = LocalizableString.FromJson(base[_titleLocalizedKey]);
                }
                return _titleLocalized ?? new LocalizableString();
            }
            set => base[_titleLocalizedKey] = value.ToJson();
        }

        private const string _descriptionKey = "description";
        public string Description
        {
            get => DeserializeItem<string>(_descriptionKey);
            set => SerializeItem(_descriptionKey, value);
        }

        private const string _officeHoursKey = "officeHours";
        public string OfficeHours
        {
            get => DeserializeItem<string>(_officeHoursKey);
            set => SerializeItem(_officeHoursKey, value);
        }
    }
}
