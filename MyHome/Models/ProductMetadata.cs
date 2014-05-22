using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.ViewModels.Listing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classy.DotNet.Mvc.Attributes;

namespace MyHome.Models
{
    public class ProductMetadata : IMetadata<ProductMetadata>
    {
        public string Style { get; set; }
        public string Width { get; set; }
        public string Depth { get; set; }
        public string Height { get; set; }
        public string Materials { get; set; }
        public string Manufacturer { get; set; }
        public string Designer { get; set; }
        public string ProductUrl { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Style)) list.Add("Style", Style);
            if (!string.IsNullOrEmpty(Width)) list.Add("Width", Width);
            if (!string.IsNullOrEmpty(Depth)) list.Add("Depth", Depth);
            if (!string.IsNullOrEmpty(Height)) list.Add("Height", Height);
            if (!string.IsNullOrEmpty(Materials)) list.Add("Materials", Materials);
            if (!string.IsNullOrEmpty(Manufacturer)) list.Add("Manufacturer", Manufacturer);
            if (!string.IsNullOrEmpty(Designer)) list.Add("Designer", Designer);
            if (!string.IsNullOrEmpty(ProductUrl)) list.Add("ProductUrl", ProductUrl);
            return list;
        }

        public ProductMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new ProductMetadata();
            if (metadata.ContainsKey("Style")) output.Style = metadata["Style"];
            if (metadata.ContainsKey("Width")) output.Width = metadata["Width"];
            if (metadata.ContainsKey("Depth")) output.Depth = metadata["Depth"];
            if (metadata.ContainsKey("Height")) output.Height = metadata["Height"];
            if (metadata.ContainsKey("Materials")) output.Materials = metadata["Materials"];
            if (metadata.ContainsKey("Manufacturer")) output.Manufacturer = metadata["Manufacturer"];
            if (metadata.ContainsKey("Designer")) output.Designer = metadata["Designer"];
            if (metadata.ContainsKey("ProductUrl")) output.ProductUrl = metadata["ProductUrl"];
            return output;
        }

        public Dictionary<string, string[]> ParseSearchFilters(string[] filters, out string keyword, ref Classy.DotNet.Responses.LocationView location)
        {
            throw new NotImplementedException();
        }

        public string GetSearchFilterSlug(string keyword, Classy.DotNet.Responses.LocationView location)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> ToTranslationsDictionary()
        {
            IDictionary<string, string> metadata = new Dictionary<string, string>();

            return metadata;
        }
    }
}