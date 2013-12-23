using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.ViewModels.Profiles;
using Classy.Models.Response;
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

        public IList<CustomAttributeView> ToCustomAttributeList()
        {
            var list = new List<CustomAttributeView>();
            list.Add(new CustomAttributeView { Key = "LicenseNo", Value = LicenseNo });
            list.Add(new CustomAttributeView { Key = "ServicesProvided", Value = ServicesProvided });
            list.Add(new CustomAttributeView { Key = "AreasServed", Value = AreasServed});
            list.Add(new CustomAttributeView { Key = "JobCostFrom", Value = JobCostFrom.ToString() });
            list.Add(new CustomAttributeView { Key = "JobCostTo", Value = JobCostTo.ToString() });
            list.Add(new CustomAttributeView { Key = "CostDetails", Value = CostDetails});
            if (!string.IsNullOrEmpty(Awards)) list.Add(new CustomAttributeView { Key = "Awards", Value = Awards });
            return list;
        }

        public ProfessionalMetadata FromCustomAttributeList(IList<CustomAttributeView> metadata)
        {
            var output = new ProfessionalMetadata();
            if (metadata.Any(x => x.Key == "LicenseNo")) LicenseNo = metadata.Single(x => x.Key == "LicenseNo").Value;
            if (metadata.Any(x => x.Key == "ServicesProvided")) ServicesProvided = metadata.Single(x => x.Key == "ServicesProvided").Value;
            if (metadata.Any(x => x.Key == "AreasServed")) AreasServed = metadata.Single(x => x.Key == "AreasServed").Value;
            if (metadata.Any(x => x.Key == "JobCostFrom")) JobCostFrom = Convert.ToInt32(metadata.Single(x => x.Key == "JobCostFrom").Value);
            if (metadata.Any(x => x.Key == "JobCostTo")) JobCostTo = Convert.ToInt32(metadata.Single(x => x.Key == "JobCostTo").Value);
            if (metadata.Any(x => x.Key == "CostDetails")) CostDetails = metadata.Single(x => x.Key == "CostDetails").Value;
            if (metadata.Any(x => x.Key == "Awards")) Awards = metadata.Single(x => x.Key == "Awards").Value;
            return output;
        }
    }
}