using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.ViewModels.Listing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classy.DotNet.Mvc.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MyHome.Models
{
    public class ProductMetadata : IMetadata<ProductMetadata>
    {
        [Display(Name = "ProductMetadata_Style")]
        [Required(ErrorMessage = "ProductMetadata_Style_Required")]
        public string Style { get; set; }
        [Display(Name = "ProductMetadata_Materials")]
        public string Materials { get; set; }
        [Display(Name = "ProductMetadata_Manufacturer")]
        public string Manufacturer { get; set; }
        [Display(Name = "ProductMetadata_Designer")]
        public string Designer { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Style)) list.Add("Style", Style);
            if (!string.IsNullOrEmpty(Materials)) list.Add("Materials", Materials);
            if (!string.IsNullOrEmpty(Manufacturer)) list.Add("Manufacturer", Manufacturer);
            if (!string.IsNullOrEmpty(Designer)) list.Add("Designer", Designer);
            return list;
        }

        public ProductMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new ProductMetadata();
            if (metadata.ContainsKey("Style")) output.Style = metadata["Style"];
            if (metadata.ContainsKey("Materials")) output.Materials = metadata["Materials"];
            if (metadata.ContainsKey("Manufacturer")) output.Manufacturer = metadata["Manufacturer"];
            if (metadata.ContainsKey("Designer")) output.Designer = metadata["Designer"];
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