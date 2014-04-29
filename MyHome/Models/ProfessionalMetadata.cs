using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ModelBinders;
using Classy.DotNet.Mvc.Helpers;
using Classy.DotNet.Mvc.Attributes;
using Html;

namespace MyHome.Models
{
    public class ProfessionalMetadata : IMetadata<ProfessionalMetadata>
    {
        [Display(Name="ProMetadata_LicenseNo")]
        //[Required(ErrorMessage="ProMetadata_LicenseNo_Required")]
        public string LicenseNo { get; set; }
        [Display(Name = "ProMetadata_ServicesProvided")]
        [System.Web.Mvc.AllowHtml]
        [Translatable]
        public string ServicesProvided { get; set; }
        [Display(Name = "ProMetadata_AreasServed")]
        public string AreasServed { get; set; }
        public int? JobCostFrom { get; set; }
        public int? JobCostTo { get; set; }
        [Display(Name = "ProMetadata_CostDetails")]
        public string CostDetails { get; set; }
        [Display(Name = "ProMetadata_Awards")]
        public string Awards { get; set; }
        [Display(Name = "ProMetadata_BusinessDescription")]
        [System.Web.Mvc.AllowHtml]
        [Translatable]
        public string BusinessDescription { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(LicenseNo)) list.Add("LicenseNo", LicenseNo);
            if (!string.IsNullOrEmpty(ServicesProvided)) list.Add("ServicesProvided", ServicesProvided);
            if (!string.IsNullOrEmpty(AreasServed)) list.Add("AreasServed", AreasServed);
            if (JobCostFrom.HasValue) list.Add("JobCostFrom", JobCostFrom.ToString());
            if (JobCostTo.HasValue) list.Add("JobCostTo", JobCostTo.ToString());
            if (!string.IsNullOrEmpty(CostDetails)) list.Add("CostDetails", CostDetails);
            if (!string.IsNullOrEmpty(BusinessDescription)) list.Add("BusinessDescription", BusinessDescription);
            if (!string.IsNullOrEmpty(Awards)) list.Add("Awards", Awards);
            return list;
        }

        public ProfessionalMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new ProfessionalMetadata();
            if (metadata.ContainsKey("LicenseNo")) output.LicenseNo = metadata["LicenseNo"];
            if (metadata.ContainsKey("ServicesProvided")) output.ServicesProvided = metadata["ServicesProvided"];
            if (metadata.ContainsKey("AreasServed")) output.AreasServed = metadata["AreasServed"];
            if (metadata.ContainsKey("JobCostFrom") && !string.IsNullOrEmpty(metadata["JobCostFrom"])) output.JobCostFrom = Convert.ToInt32(metadata["JobCostFrom"]);
            if (metadata.ContainsKey("JobCostTo") && !string.IsNullOrEmpty(metadata["JobCostTo"])) output.JobCostTo = Convert.ToInt32(metadata["JobCostTo"]);
            if (metadata.ContainsKey("CostDetails")) output.CostDetails = metadata["CostDetails"];
            if (metadata.ContainsKey("Awards")) output.Awards = metadata["Awards"];
            if (metadata.ContainsKey("BusinessDescription")) output.BusinessDescription = metadata["BusinessDescription"];
            return output;
        }

        public Dictionary<string, string[]> ParseSearchFilters(string[] filters, out string keyword, ref Classy.DotNet.Responses.LocationView location)
        {
            keyword = null;
            return null;
        }

        public string GetSearchFilterSlug(string keyword, Classy.DotNet.Responses.LocationView location)
        {
            return null;
        }


        public IDictionary<string, string> ToTranslationsDictionary()
        {
            IDictionary<string, string> metadata = new Dictionary<string, string>();
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes = new string[] { "style" };
            sanitizer.AllowedCssProperties = new string[] { "direction" };

            if (!string.IsNullOrEmpty(this.BusinessDescription))
                metadata.Add("BusinessDescription", sanitizer.Sanitize(this.BusinessDescription));

            if (!string.IsNullOrEmpty(this.ServicesProvided))
                metadata.Add("ServicesProvided", sanitizer.Sanitize(this.ServicesProvided));

            return metadata;
        }
    }
}