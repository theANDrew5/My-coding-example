namespace Photoprint.WebSite.API.Models.Delivery
{
    public enum DeliveryMessageType
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public class DeliveryMessage
    {
        public DeliveryMessageType Type { get; set; }
        public string Text { get; set; }

        public DeliveryMessage(DeliveryMessageType type, string message)
        {
            Type = type;
            Text = message;
        }
    }
}