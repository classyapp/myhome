using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyHome.Models
{
    public class ProfileReviewMetadata : IMetadata<ProfileReviewMetadata>
    {
        [Display(Name="ReviewMetadata_Relationship")]
        [Required(ErrorMessage="ReviewMetadata_Relationship_Required")]
        public string Relationship { get; set; }
        public SelectList RelationshipOptions { get { return new SelectList(Localizer.GetList("relationship-with-pro")); } }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            list.Add("Relationship", Relationship);
            return list;
        }

        public ProfileReviewMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new ProfileReviewMetadata();
            if (metadata.ContainsKey("Relationship")) output.Relationship = metadata["IsRenter"];
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