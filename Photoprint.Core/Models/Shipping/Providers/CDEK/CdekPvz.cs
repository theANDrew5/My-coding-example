using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photoprint.Core.Models
{
    public class CdekPvz
    {
        public string Id { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string CityId { get; set; }
        public string Name { get; set; }
        public string WorkTime { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public string cX { get; set; }
        public string cY { get; set; }
        public bool Dressing { get; set; }
        public bool Cash { get; set; }
        public string Station { get; set; }
        public string Site { get; set; }
        public string Metro { get; set; }
        public string AddressComment { get; set; }
        public CdekWeightLimit CdekWeightLim { get; set; }
        public IEnumerable<string> Picture { get; set; }
        public string Path { get; set; }
    }

    public class CdekWeightLimit
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public CdekWeightLimit(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
