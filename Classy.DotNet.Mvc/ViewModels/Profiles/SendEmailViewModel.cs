using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Mvc.Attributes;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class SendEmailViewModel
    {
        public string ProfileId { get; set; }
        [Required(ErrorMessage = "SendEmail_RecipientsRequired")]
        [Display(Name = "SendEmail_Recipients")]
        [EveryItemIs(Validators = new Type[] { typeof(EmailAddressAttribute) })]
        public IList<string> Contacts { get; set; }
        [Required(ErrorMessage = "SendEmail_SubjectRequired")]
        [Display(Name = "SendEmail_Subject")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "SendEmail_BodyRequired")]
        [Display(Name = "SendEmail_Body")]
        public string Body { get; set; }
    }
}
