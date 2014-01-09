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
        [Required]
        public string ServicesProvided { get; set; }
        [Required]
        public string AreasServed { get; set; }
        [Required]
        public int JobCostFrom { get; set; }
        [Required]
        public int JobCostTo { get; set; }
        [Required]
        public string CostDetails { get; set; }
        public string Awards { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            list.Add("LicenseNo", LicenseNo);
            list.Add("ServicesProvided", ServicesProvided);
            list.Add("AreasServed", AreasServed);
            list.Add("JobCostFrom", JobCostFrom.ToString());
            list.Add("JobCostTo", JobCostTo.ToString());
            list.Add("CostDetails", CostDetails);
            if (!string.IsNullOrEmpty(Awards)) list.Add("Awards", Awards);
            return list;
        }

        public ProfessionalMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new ProfessionalMetadata();
            if (metadata.ContainsKey("LicenseNo")) LicenseNo = metadata["LicenseNo"];
            if (metadata.ContainsKey("ServicesProvided")) ServicesProvided = metadata["ServicesProvided"];
            if (metadata.ContainsKey("AreasServed")) AreasServed = metadata["AreasServed"];
            if (metadata.ContainsKey("JobCostFrom")) JobCostFrom = Convert.ToInt32(metadata["JobCostFrom"]);
            if (metadata.ContainsKey("JobCostTo")) JobCostTo = Convert.ToInt32(metadata["JobCostTo"]);
            if (metadata.ContainsKey("CostDetails")) CostDetails = metadata["CostDetails"];
            if (metadata.ContainsKey("Awards")) Awards = metadata["Awards"];
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