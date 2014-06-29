using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class PurchaseOptionView
    {
        public bool HasImages { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> VariantProperties { get; set; } // Key: Size, Color, Model, etc. Value: Smal, Medium, Large, etc.
        public string Color {
            get {
                return GetVariantProperty("Color");
            }
            set {
                SetVariantProperty("Color", value);
            }
        }
        public string Design {
            get
            {
                return GetVariantProperty("Design");
            }
            set
            {
                SetVariantProperty("Design", value);
            }
        }
        public string Size {
            get
            {
                return GetVariantProperty("Size");
            }
            set
            {
                SetVariantProperty("Size", value);
            }
        }
        public string Width { get; set; }
        public string Depth { get; set; }
        public string Height { get; set; }
        public string SKU { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public string ProductUrl { get; set; }
        public double? CompareAtPrice { get; set; }
        public MediaFileView[] MediaFiles { get; set; }
        public string DefaultImage { get; set; }
        public bool Available { get; set; }

        private void SetVariantProperty(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                VariantProperties.Remove(key);
            }
            else
            {
                VariantProperties[key] = value;
            }
        }

        private string GetVariantProperty(string key)
        {
            return VariantProperties.ContainsKey(key) ? VariantProperties[key] : null;
        }

        public PurchaseOptionView()
        {
            VariantProperties = new Dictionary<string, string>();
        }

        public string GetVariantKey(bool full)
        {
            if (full)
            {
                return string.Join(",", new string[] {
                    VariantProperties.ContainsKey("Color") ? VariantProperties["Color"] : "_",
                    VariantProperties.ContainsKey("Design") ? VariantProperties["Design"] : "_",
                    VariantProperties.ContainsKey("Size") ? VariantProperties["Size"] : "_",
                });
            }
            else
            {
                string[] values = new string[VariantProperties.Count];
                string[] keys = new string[] {"Color", "Design", "Size" };
                int i = 0;

                for (int j = 0; j < keys.Length; j++)
                {
                    if (VariantProperties.ContainsKey(keys[j]))
                    {
                        values[i] = VariantProperties[keys[j]];
                        i++;
                    }
                }

                return string.Join(",", values);
            }
        }
        public string UID { get; set; }
    }
}
