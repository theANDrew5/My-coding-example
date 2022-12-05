
using System;

namespace Photoprint.Core.Models
{
    public class RussianPostofficeSearchOptions
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Top { get; set; }
        public string Filter { get; set; }
        public int SearchRadius { get; set; } //км
        public DateTime CurrentDateTime { get; set; }
        public bool HidePrivate { get; set; } // Исключать не публичные отделения(Опционально)
        public bool FilterByOfficeType { get; set; }

        //GeoObject, получаемый для адреса в сервисе Яндекса. См. api.yandex.ru. Требует также заполненного параметра 'yandex-address'.
        public string YandexAddress { get; set; }
        public string GeoObject { get; set; }

        public RussianPostofficeSearchOptions(double latitude, double longitude, RussianPostofficeFilter filter)
        {
            Latitude = latitude;
            Longitude = longitude;
            Filter = filter.ToString();
        }
    }
}
