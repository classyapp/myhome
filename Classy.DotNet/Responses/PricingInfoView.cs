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
        public PurchaseOptionView BaseOption { get; set; }
        public IList<PurchaseOptionView> PurchaseOptions { get; set; }

        public string GetVariantOptionValues(string key)
        {
            return String.Join(",", PurchaseOptions.Select(po => po.VariantProperties[key]).Distinct());
        }

        public string MSRPRange(string currencySign)
        {
            if (PurchaseOptions != null)
            {
                double minPrice = PurchaseOptions.Min(p => p.CompareAtPrice.GetValueOrDefault(0));
                double maxPrice = PurchaseOptions.Max(p => p.CompareAtPrice.GetValueOrDefault(0));

                if (minPrice > 0 && maxPrice > 0)
                {
                    if (minPrice == maxPrice)
                    {
                        return string.Format("{0} {1:N2}", currencySign,  maxPrice);
                    }
                    return string.Format("{0} {1:N2} - {0} {2:N2}", currencySign, minPrice, maxPrice);
                }
            }

            return null;
        }

        public string PriceRange(string currencySign)
        {
            if (PurchaseOptions != null)
            {
                double minPrice = PurchaseOptions.Min(p => p.Price);
                double maxPrice = PurchaseOptions.Max(p => p.Price);

                if (minPrice > 0 && maxPrice > 0)
                {
                    if (minPrice == maxPrice)
                    {
                        return string.Format("{0} {1:N2}", currencySign, maxPrice);
                    }
                    return string.Format("{0} {1:N2} - {0} {2:N2}", currencySign, minPrice, maxPrice);
                }
            }

            return null;
        }
    }
}
