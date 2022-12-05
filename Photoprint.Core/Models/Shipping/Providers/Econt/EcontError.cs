namespace Photoprint.Core.Models
{
    public class EcontError
    {
        public string type { get; set; }
        public string message { get; set; }
        public object[] fields { get; set; }
        public object[] innerErrors { get; set; }
    }


}
