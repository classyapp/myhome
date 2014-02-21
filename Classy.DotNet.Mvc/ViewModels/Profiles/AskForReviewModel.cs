using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class AskForReviewModel
    {
        public string ProfileId { get; set; }
        [Required(ErrorMessage="AskForReview_ContactsRequired")]
        [Display(Name = "AskForReview_SelectContacts")]
        public string Contacts { get; set; }
        [Required(ErrorMessage = "AskForReview_MessageRequired")]
        [Display(Name = "AskForReview_Message")]
        public string Message { get; set; }
        public bool SaveAsDefault { get; set; }
    }
}
