using System;
using System.Collections.Generic;
using System.Linq;

namespace Classy.DotNet.Responses
{
    public class PricingInfoView
    {
        public string CurrencyCode { get; set; }
        public PurchaseOptionView BaseOption { get; set; }
        public IList<PurchaseOptionView> PurchaseOptions { get; set; }

        public string GetVariantOptionValues(string key)
        {
            try
            {
                return String.Join(",", PurchaseOptions.Select(po => po.VariantProperties[key]).Distinct());
            }
            catch
            {
                return string.Empty;
            }
        }

        public string MSRPRange(string currencySign)
        {
            if (PurchaseOptions != null)
            {
                var minPrice = PurchaseOptions.Min(p => p.CompareAtPrice.GetValueOrDefault(0));
                var maxPrice = PurchaseOptions.Max(p => p.CompareAtPrice.GetValueOrDefault(0));

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
                var minPrice = PurchaseOptions.Min(p => p.Price);
                var maxPrice = PurchaseOptions.Max(p => p.Price);

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

        public Dictionary<string, string[]> GetVariationOptions()
        {
            var options = new Dictionary<string,string[]>();
            if (PurchaseOptions != null)
            {
                var keys = new[] { "Color", "Design", "Size" };
                foreach (var key in keys)
                {
                    string[] values = PurchaseOptions.Select(p => p.VariantProperties.ContainsKey(key) ? p.VariantProperties[key] : null).Distinct().ToArray();
                    if (values.Length > 0)
                    {
                        options.Add(key, values);
                    }
                }
            }

            return options;
        }
    }
}
