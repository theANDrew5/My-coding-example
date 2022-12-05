using System;
using System.Collections.Generic;
using System.Text;

namespace Photoprint.Core.Models
{
    public class SuggestAddressData
    {
        public SuggestType Type { get; set; }
        public CityAddress City { get; set; }
        public string Street { get; set; }
        public string Query { get; set; }
        public string FullQuery 
        { 
            get
            {
                if (City is null) return null;
                var result = $"{City.Country}, {City.Region}, {City.Title}";
                return !string.IsNullOrWhiteSpace(Street)
                    ? string.Concat(result, $", {Street}, {Query}")
                    : string.Concat(result, $", {Query}");

            } 
        }
    }

    public enum SuggestType
    {
        Street,
        House
    }
}
