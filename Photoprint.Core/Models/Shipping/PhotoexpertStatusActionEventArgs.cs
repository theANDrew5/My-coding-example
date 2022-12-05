using System;

namespace Photoprint.Core.Models
{
    public class PhotoexpertStatusActionEventArgs : EventArgs
    {
        public Order Order { get; }
        public Photolab Photolab { get; }
        public PhotoexpertStatusState Status { get; }

        public PhotoexpertStatusActionEventArgs(Order order, Photolab photolab, PhotoexpertStatusState status)
        {
            Order = order;
            Photolab = photolab;
            Status = status;
        }
    }
}
