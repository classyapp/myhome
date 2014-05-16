using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class PricingViewModel
    {
        // TODO: How do we share models with Classy.Models.Response but still have front-end validation mechanism?
        [Required]
        public string SKU { get; set; }
        [Range(0, Int16.MaxValue)]
        public double? Price { get; set; }
        [Range(0, Int16.MaxValue)]
        public double? CompareAtPrice { get; set; }

        [Required]
        [Range(1, Int16.MaxValue)]
        public int? Quantity { get; set; }

        [Range(0, Int16.MaxValue)]
        public int? DomesticRadius { get; set; }
        [Range(0, Int16.MaxValue)]
        public decimal? DomesticShippingPrice { get; set; }
        [Range(0, Int16.MaxValue)]
        public decimal? InternationalShippingPrice { get; set; }
    }

    public static class PricingViewModelExtensitons {
        public static PricingInfoView ToPricingInfo(this PricingViewModel pricing)
        {
            if (pricing == null) return null;

             return new PricingInfoView()
            {
                //SKU = pricing.SKU,
                //Price = pricing.Price,
                //CompareAtPrice = pricing.CompareAtPrice,
                //Quantity = pricing.Quantity.Value,
                //DomesticRadius = pricing.DomesticRadius,
                //DomesticShippingPrice = pricing.DomesticShippingPrice,
                //InternationalShippingPrice = pricing.InternationalShippingPrice
            };
        }
    }
}
