namespace Photoprint.Core.Models
{
    public class EvropochtaRequestParams
    {
        public string CRC { get; set; }
        public EvropochtaRequestPacket Packet { get; set; }

        public EvropochtaRequestParams(string crc, string token, string methodName, string serviceNumber, object requestData)
        {
            CRC = crc;
            Packet = new EvropochtaRequestPacket(token, methodName, serviceNumber, requestData);
        }
    }
    public class EvropochtaRequestPacket
    {
        public string JWT { get; set; }
        public string MethodName { get; set; }
        public string ServiceNumber { get; set; }
        public object Data { get; set; }

        public EvropochtaRequestPacket(string token, string methodName, string serviceNumber, object requestData)
        {
            JWT = token;
            MethodName = methodName;
            ServiceNumber = serviceNumber;
            Data = requestData;
        }
    }
}
