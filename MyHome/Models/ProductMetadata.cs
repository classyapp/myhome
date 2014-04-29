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
        public IDictionary<string, string> ToDictionary()
        {
            return null;
        }

        public ProductMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            return null;
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

            foreach (var property in this.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(TranslatableAttribute), true).Any()))
            {
                string value = (string)property.GetValue(this);
                if (!string.IsNullOrEmpty(value))
                    metadata.Add(property.Name, value);
            }

            return metadata;
        }
    }
}