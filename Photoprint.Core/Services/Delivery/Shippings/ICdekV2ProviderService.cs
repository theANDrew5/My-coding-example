using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Photoprint.Core.Models;

namespace Photoprint.Core.Services
{
    public interface ICdekV2ProviderService: IShippingProviderService
    {
        (string status, string error) GetOrderInfo(Order order);
        ((string uuid, string info) requestResult, string error) RequestFormingBarcode(Order order, string format,
            string lang);
        ((string info, string url) requestResult, string error) GetFormedBarcode(Order order, string uuid);
        ((string uuid, string info) requestResult, string error) RequestFormingReceipt(Order order);
        ((string url, string info) requestResult, string error) GetFormedReceipt(Order order, string uuid);
        void GetFile(Order order, string url, Stream outputStream, out string error);
        (string requestResult, string error) CallCourier(Order order, CDEKv2CallCourier request);
        (string requestResult, string error) CancelCourier(Order order);
        (string requestResult, string error) GetCourierInfo(Order order);
    }
}
