using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.ModelBinders;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    [ModelBinder(typeof(CommaSeparatedToList))]
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
