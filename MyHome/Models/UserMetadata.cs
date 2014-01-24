using Classy.DotNet.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class UserMetadata : IMetadata<UserMetadata>
    {
        [Display(Name="UserMetadata_IsRenter")]
        public bool IsRenter { get; set; }
        [Display(Name="UserMetadata_IsOwner")]
        public bool IsHomeOwner { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            list.Add("IsRenter", IsRenter.ToString());
            list.Add("IsHomeOwner", IsHomeOwner.ToString());
            return list;
        }

        public UserMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new UserMetadata();
            if (metadata.ContainsKey("IsRenter")) output.IsRenter = Convert.ToBoolean(metadata["IsRenter"]);
            if (metadata.ContainsKey("IsHomeOwner")) output.IsHomeOwner = Convert.ToBoolean(metadata["IsHomeOwner"]);
            return output;
        }



        public void ParseSearchFilters(string[] filters, out string keyword, ref Classy.DotNet.Responses.LocationView location)
        {
            throw new NotImplementedException();
        }

        public string GetSearchFilterSlug(string keyword, Classy.DotNet.Responses.LocationView location)
        {
            throw new NotImplementedException();
        }
    }
}