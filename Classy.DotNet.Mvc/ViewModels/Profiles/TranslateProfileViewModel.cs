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

        [Display(Name = "TranslateProfle_Language")]
        public string CultureCode { get; set; }

        [Display(Name = "TranslateProfile_BusinessDescription")]
        public string BusinessDescription { get; set; }

        public string Action { get; set; }
    }
}
