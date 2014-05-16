using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class PurchaseOptionView
    {
        public string Title { get; set; }
        public Dictionary<string, string> VariantProperties { get; set; } // Key: Size, Color, Model, etc. Value: Smal, Medium, Large, etc.
        public string SKU { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public double? CompareAtPrice { get; set; }
        public MediaFileView[] MediaFiles { get; set; }
        public string DefaultImage { get; set; }
    }
}
