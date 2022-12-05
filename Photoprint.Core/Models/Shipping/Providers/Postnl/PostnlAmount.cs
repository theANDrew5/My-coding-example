namespace Photoprint.Core.Models.Postnl
{
    public class PostnlAmount
    {
        public string AmountType { get; set; }// 01 Cash on delivery (COD), 02 Insured, 04 Commercial route China, 12 for Inco terms DDP Commercial route China
        public string AccountName { get; set; }
        public string BIC { get; set; }
        public string Currency { get; set; }
        public string IBAN { get; set; }
        public string Reference { get; set; }
        public string TransactionNumber { get; set; }
        public string Value { get; set; }
    }
}
