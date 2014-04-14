using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class SendEmailViewModel
    {
        public string ProfileId { get; set; }
        [Required(ErrorMessage = "SendEmail_RecipientsRequired")]
        public string Recipients { get; set; }
        [Required(ErrorMessage = "SendEmail_SubjectRequired")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "SendEmail_BodyRequired")]
        public string Body { get; set; }
    }
}
