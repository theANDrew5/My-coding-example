using System;

namespace Photoprint.Core.Models
{
    public class ParcelSize
    {
        public ParcelSize(int widthMm, int heightMm, int lengthMm)
        {
            WidthMm = widthMm;
            HeightMm = heightMm;
            LengthMm = lengthMm;
        }

        public int WidthMm { get; set; }

        public int HeightMm { get; set; }

        public int LengthMm { get; set; }

        public double WidthCm => WidthMm / (double)10;

        public double HeightCm => HeightMm / (double)10;

        public double LengthCm => LengthMm / (double)10;
        public double LengthM => LengthMm / (double)1000;
        public double HeightM => HeightMm / (double)1000;
        public double WidthM => WidthMm / (double)1000;

        public double VolumeM3 => WidthMm * HeightMm * LengthMm / Math.Pow(10,9);

        public double VolumeCm3 => WidthCm * HeightCm * LengthCm;
    }
}