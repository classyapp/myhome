using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet;
using Classy.DotNet.Responses;
using Classy.DotNet.Mvc.Localization;

namespace MyHome.Controllers
{
    public class SecurityController : Classy.DotNet.Mvc.Controllers.Security.SecurityController<MyHome.Models.UserMetadata>
    {
        private readonly string MANDRILL_API_KEY = "ndg42WcyRHVLtLbvGqBjUA";

        public SecurityController()
            : base("MyHome.Controllers") 
        {
            base.OnProfileRegistered += SecurityController_OnProfileRegistered;
        }

        void SecurityController_OnProfileRegistered(object sender, ProfileView profile)
        {
            // send welcome email
            // with a verify email link in the follwing format /profile/verify/{profile.Metadata["EmailHash"]}
            //string body = null;
            //if (!string.IsNullOrEmpty(profile.ContactInfo.FirstName))
            //{
            //    body = string.Format(Localizer.Get("WelcomeEmail_BodyWithName", profile.DefaultCulture ?? "en", true),
            //        string.Format("{0} {1}", profile.ContactInfo.FirstName, profile.ContactInfo.LastName,
            //        string.Concat("https://" + AppView.Hostname + Url.RouteUrl("VerifyProfileEmail", new { hash = profile.Metadata["EmailHash"] }))));
            //}
            //else
            //{
            //    body = string.Format(Localizer.Get("WelcomeEmail_BodyNoName", profile.DefaultCulture ?? "en", true),
            //        string.Concat("https://" + AppView.Hostname + Url.RouteUrl("VerifyProfileEmail", new { hash = profile.Metadata["EmailHash"] })));
            //}
            //var message = new EmailMessage
            //{
            //    subject = string.Format(Localizer.Get("WelcomeEmail_Subject", profile.DefaultCulture ?? "en")),
            //    to = new List<EmailAddress> {
            //        new EmailAddress {
            //            email = profile.ContactInfo.Email
            //        }
            //    },
            //    from_email = "team@homelab.com", 
            //    html = body
            //};
            //System.Text.RegularExpressions.Regex dirRegex = new System.Text.RegularExpressions.Regex("[\\p{IsHebrew}]");
            //if (dirRegex.IsMatch(message.html.Substring(0, Math.Min(message.html.Length, 10))))
            //{
            //    message.html = "<div style=\"direction: rtl\">" + message.html + "</div>";
            //}
            //message.AddHeader("Reply-To", "team@homelab.com");
            //var api = new MandrillApi(MANDRILL_API_KEY);
            //var sendResponse = api.SendMessage(message);
        }
    }
}