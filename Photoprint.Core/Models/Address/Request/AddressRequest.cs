using System;
using System.Collections.Generic;
using System.Text;

namespace Photoprint.Core.Models
{
    public interface IAddressRequest
    {
        int FrontendId { get; set; }
        int? LanguageId { get; set; }
        bool HasData { get; }
    }
    public sealed class AddressRequest<TData>: IAddressRequest
    where TData : class
    {
        public int FrontendId { get; set; }
        public int? LanguageId { get; set; }
        public bool HasData => !(Data is null);
        public TData Data { get; set; }
    }
}
