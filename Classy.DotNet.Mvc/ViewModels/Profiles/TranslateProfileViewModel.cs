using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class TranslateProfileViewModel<TProMetadata>
    {
        public string ProfileId { get; set; }

        [Display(Name = "TranslateProfile_Language")]
        public string CultureCode { get; set; }

        [Display(Name = "TranslateProfile_CompanyName")]
        public string CompanyName { get; set; }

        public TProMetadata Metadata { get; set; }
    }
}
