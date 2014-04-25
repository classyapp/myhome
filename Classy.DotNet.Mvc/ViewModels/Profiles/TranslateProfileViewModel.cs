using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class TranslateProfileViewModel
    {
        public string ProfileId { get; set; }

        [Display(Name = "TranslateProfile_Language")]
        public string CultureCode { get; set; }

        [Display(Name = "TranslateProfile_CompanyName")]
        public string CompanyName { get; set; }

        [Display(Name = "TranslateProfile_BusinessDescription")]
        [System.Web.Mvc.AllowHtml]
        public string BusinessDescription { get; set; }

        [Display(Name = "TranslateProfile_ServicesProvided")]
        [System.Web.Mvc.AllowHtml]
        public string ServicesProvided { get; set; }
    }
}
