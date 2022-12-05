using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Photoprint.Core.Models
{
    public sealed class MapBounds
    {
        private string _upperLongitude = string.Empty;

        public double? DoubleUpperLongitude => double.TryParse(_upperLongitude,NumberStyles.Any, CultureInfo.InvariantCulture, out var inDouble)? inDouble: (double?)null;

        public string UpperLongitude
        {
            get => _upperLongitude;
            set => _upperLongitude = value ?? string.Empty;
        }

        private string _upperLatitude = string.Empty;
        public double? DoubleUpperLatitude => double.TryParse(_upperLatitude,NumberStyles.Any, CultureInfo.InvariantCulture, out var inDouble)? inDouble: (double?)null;
        public string UpperLatitude
        {
            get => _upperLatitude;
            set => _upperLatitude=value ?? string.Empty;
        }

        private string _loverLongitude = string.Empty;
        public double? DoubleLowerLongitude => double.TryParse(_loverLongitude,NumberStyles.Any, CultureInfo.InvariantCulture, out var inDouble)? inDouble: (double?)null;
        public string LowerLongitude
        {
            get => _loverLongitude;
            set => _loverLongitude = value ?? string.Empty;
        }

        private string _loverLatitude = string.Empty;
        public double? DoubleLowerLatitude => double.TryParse(_loverLatitude,NumberStyles.Any, CultureInfo.InvariantCulture, out var inDouble)? inDouble: (double?)null;
        public string LowerLatitude
        {
            get => _loverLatitude;
            set => _loverLatitude = value ?? string.Empty;
        }
        
        public bool IsAvailable => DoubleUpperLongitude != null && DoubleUpperLatitude != null && DoubleLowerLongitude != null && DoubleLowerLatitude != null;

        public double? DoubleRadius
        {
            get
            {
                if (!IsAvailable) return null;

                var dLat = _degreesToRadians((double)(DoubleUpperLatitude - DoubleLowerLatitude));
                var dLon = _degreesToRadians((double)(DoubleUpperLongitude - DoubleLowerLongitude));

                var lat1 = _degreesToRadians((double)DoubleUpperLatitude);
                var lat2 = _degreesToRadians((double)DoubleLowerLatitude);

                var a = Math.Pow(Math.Sin(dLat/2),2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon/2),2); 
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

                return _earthRadiusM * c / 2 * 1000;
            }
        }

        [CanBeNull] public string Radius => DoubleRadius?.ToString("0.000000", CultureInfo.InvariantCulture);

        public bool IsCoordInBound(double latitude, double Longitude)
        {
            if (!IsAvailable) return false;
            return (DoubleUpperLatitude >= latitude && DoubleLowerLatitude <= latitude) &&
                   (DoubleUpperLongitude >= Longitude && DoubleLowerLongitude <= Longitude);
        }

        public bool IsCoordInBound(string latitude, string longitude)
        {
            if (!IsAvailable || string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude)) return false;

            if (!double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var latDouble))
                return false;
            if (!double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lngDouble))
                return false;

            return IsCoordInBound(latDouble, lngDouble);
        }

        private readonly double _earthRadiusM = 6371.0;
        private double _degreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

    }
}
