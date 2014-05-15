using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.Controllers;

namespace MyHome.Models
{
    public class VendorMetadata : IMetadata<VendorMetadata>
    {
        private static class Keys
        {
            public static readonly string SigneeName = "SigneeName";
            public static readonly string SigneeTitle = "SigneeTitle";
        }

        [BooleanRequired(ErrorMessage = "VendorMetadata_Terms_Required")]
        public bool AgreeToTerms { get; set; }
        [Required]
        [Display(Name = "VendorMetadata_SigneeName")]
        public string SigneeName { get; set; }
        [Required]
        [Display(Name = "VendorMetadata_SigneeTitle")]
        public string SigneeTitle { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(SigneeName)) { dictionary.Add(Keys.SigneeName, SigneeName); }
            if (!string.IsNullOrWhiteSpace(SigneeTitle)) { dictionary.Add(Keys.SigneeTitle, SigneeTitle); }

            return dictionary;
        }

        public IDictionary<string, string> ToTranslationsDictionary()
        {
            return new Dictionary<string, string>();
        }

        public VendorMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            VendorMetadata _metadata = new VendorMetadata();
            if (metadata.ContainsKey(Keys.SigneeName)) { _metadata.SigneeName = metadata[Keys.SigneeName]; }
            if (metadata.ContainsKey(Keys.SigneeTitle)) { _metadata.SigneeTitle = metadata[Keys.SigneeTitle]; }

            return _metadata;
        }

        public Dictionary<string, string[]> ParseSearchFilters(string[] filters, out string keyword, ref Classy.DotNet.Responses.LocationView location)
        {
            keyword = null;
            return new Dictionary<string, string[]>();
        }

        public string GetSearchFilterSlug(string keyword, Classy.DotNet.Responses.LocationView location)
        {
            return string.Empty;
        }
    }
}