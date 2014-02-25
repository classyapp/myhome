using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Mvc.Validation;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class AskForReviewModel
    {
        public string ProfileId { get; set; }
        [Required(ErrorMessage="AskForReview_ContactsRequired")]
        [Display(Name = "AskForReview_SelectContacts")]
        [EveryItemIs(Validators = new Type[] { typeof(EmailAddressAttribute) })]
        public IList<string> Contacts { get; set; }
        [Display(Name = "AskForReview_Message")]
        public string Message { get; set; }
        [Display(Name = "AskForReview_SetAsDefaultMessage")]
        public bool SaveAsDefault { get; set; }
        public bool NeedAuthentication { get; set; }
        public IList<GoogleContactView> GoogleContacts { get; set; }
    }
}
