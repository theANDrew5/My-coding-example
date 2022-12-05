using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photoprint.Core.Models
{
    public class RPShippingCalculationCoastTariffAPIRequest
    {
        public RussianPostMailType MailType { get; set; }
        public RussianPostMailCategory MailCategory { get; set; }
        public string IndexFrom { get; set; }
        public string IndexTo { get; set; }
        public int Weight { get; set; }
        public bool CompletenessChecking { get; set; }//38
        public bool SMSNoticeRecipient { get; set; } //44

        public RPShippingCalculationCoastTariffAPIRequest (
            RussianPostMailType mailType, string indexFrom, string indexTo, int weight, RussianPostMailCategory mailCategory = RussianPostMailCategory.ORDINARY)
        {
            MailType = mailType;
            MailCategory = mailCategory;
            IndexFrom = indexFrom;
            IndexTo = indexTo;
            Weight = weight;
        }
        public string GetRequestParams()
        {
           
            string result = $"&object={(int)MailType}0{(int)MailCategory}&from={IndexFrom}&to={IndexTo}&weight={Weight}";

            var servises = new List<int>(2);
            if (CompletenessChecking)
                servises.Add(38);
            if (SMSNoticeRecipient)
                servises.Add(44);
            if (servises.Count > 0)
                result += $"&service={string.Join(",", servises)}";

            return result;
        }

    }
}
