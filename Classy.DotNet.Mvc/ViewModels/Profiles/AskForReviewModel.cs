using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Mvc.ModelBinders;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Services;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    [ModelBinder(typeof(CommaSeparatedToList))]
    public class AskForReviewModel
    {
        public string ProfileId { get; set; }
        [Required(ErrorMessage="AskForReview_ContactsRequired")]
        [Display(Name = "AskForReview_SelectContacts_Label")]
        [EveryItemIs(Validators = new Type[] { typeof(EmailAddressAttribute) })]
        [CommaSeparated]
        public IList<string> Contacts { get; set; }
        [Display(Name = "AskForReview_Message_Label")]
        public string Message { get; set; }
        [Display(Name = "AskForReview_SetAsDefaultMessage_Label")]
        public bool SaveAsDefault { get; set; }
        public bool IsGoogleConnected { get; set; }
        public IList<GoogleContactView> GoogleContacts { get; set; }
    }
}
