namespace Photoprint.Core.Models
{
    public class UspsAuth
    {
        public string UserId { get; set; }
        public string UserPassword { get; set; }
        public bool IsTestMode { get; set; }
    }
}
