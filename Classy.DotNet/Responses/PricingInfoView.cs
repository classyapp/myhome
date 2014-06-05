using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class PricingInfoView
    {
        public string CurrencyCode { get; set; }
        public IList<PurchaseOptionView> PurchaseOptions { get; set; }
    }
}
