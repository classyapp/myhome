using Classy.DotNet.Mvc.ModelBinders;
using Classy.DotNet.Mvc.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class RequestReviewViewModel
    {
        [Required]
        [Display(Name="Email_Addresses")]
        [EveryItemIs(Validators = new Type[] {typeof(EmailAddressAttribute)})]
        public IList<string> Emails { get; set; }
        [Required]
        [Display(Name="Content")]
        public string Content { get; set; }
    }
}
