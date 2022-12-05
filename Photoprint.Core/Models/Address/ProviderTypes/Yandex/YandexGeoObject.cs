using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class YandexGeoObject
		{
			public YandexGeoObjectMetaDataProperty metaDataProperty { get; set; }
			public string name { get; set; }
			public string description { get; set; }
			public YandexBound boundedBy { get; set; }
			public YandexGeoObjectPoint Point { get; set; }
		}
		public class YandexGeoObjectMetaDataProperty
		{
			public YandexGeoObjectMetaDataPropertyGeocoderMetaData GeocoderMetaData { get; set; }
		}
		public class YandexGeoObjectMetaDataPropertyGeocoderMetaData
		{
			public string precision { get; set; }
			public string text { get; set; }
			public string kind { get; set; }
			public YandexGeoObjectMetaDataPropertyGeocoderMetaDataAddress Address { get; set; }
			public object AddressDetails { get; set; }
		}
		public class YandexGeoObjectMetaDataPropertyGeocoderMetaDataAddress
		{	
			public string country_code { get; set; }
			public string formatted { get; set; }
			public string postal_code { get; set; }
			public IReadOnlyCollection<YandexGeoObjectMetaDataPropertyGeocoderMetaDataAddressComponent> Components { get; set; }
		}
		public class YandexGeoObjectMetaDataPropertyGeocoderMetaDataAddressComponent
		{
			// kind types
			//		 country	Россия
			//		province	Сибирский федеральный округ
			//		province	Томская область								да, в ответе два province
			//			area	городской округ Томск
			//		locality	Томск
			//		  street	улица Белинского
			//		   house	18

			public string kind { get; set; }
			public string name { get; set; }
		}
		public class YandexGeoObjectPoint
		{
			public string pos { get; set; }
		}

        public class YandexBound
        {
            public YandexEnvelope Envelope { get; set; }
        }

        public class YandexEnvelope
        {
            public string lowerCorner { get; set; }
            public string upperCorner { get; set; }
        }
}
