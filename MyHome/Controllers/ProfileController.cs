using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Controllers;
using MyHome.Models;

namespace MyHome.Controllers
{
    public class ProfileController : Classy.DotNet.Mvc.Controllers.ProfileController<ProfessionalMetadata, ProfileReviewCriteria>
    {
        private readonly string MANDRILL_API_KEY = "ndg42WcyRHVLtLbvGqBjUA";

        public ProfileController()
            : base("MyHome.Controllers") {
                base.OnContactProfessional += ProfileController_OnContactProfessional;
        }

        public void ProfileController_OnContactProfessional(object sender, ContactProfessionalArgs<ProfessionalMetadata> e) {
            // email professional
            var message = new EmailMessage
            {
                subject = string.Format("פניה מאתר מיי הום: {0}", e.Subject),
                to = new List<EmailAddress> {
                    new EmailAddress {
                        email = e.ProfessionalProfile.ContactInfo.Email
                    }
                }
            };
            message.AddHeader("Reply-To", e.ReplyToEmail);
            message.AddGlobalVariable("CONTENT", e.Content);
            var api = new MandrillApi(MANDRILL_API_KEY);
            var sendResponse = api.SendMessage(message, "notification_contact_pro", null);
        }
    }
}