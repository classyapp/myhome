using Classy.DotNet.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Classy.DotNet.Mvc.Attributes;

namespace MyHome.Models
{
    public class UserMetadata : IMetadata<UserMetadata>
    {
        public IDictionary<string, string> ToDictionary()
        {
            return null;
        }

        public UserMetadata FromDictionary(IDictionary<string, string> metadata)
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

            return metadata;
        }
    }
}