using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class ProfessionalMetadata : IMetadata<ProfessionalMetadata>
    {
        [Required]
        public string LicenseNo { get; set; }
        public string ServicesProvided { get; set; }
        public string AreasServed { get; set; }
        public int? JobCostFrom { get; set; }
        public int? JobCostTo { get; set; }
        public string CostDetails { get; set; }
        public string Awards { get; set; }
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