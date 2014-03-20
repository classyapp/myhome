using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Controllers;
using MyHome.Models;
using Classy.DotNet;
using Classy.DotNet.Mvc;
using Classy.DotNet.Mvc.Localization;

namespace MyHome.Controllers
{
    public class ReviewController : Classy.DotNet.Mvc.Controllers.ReviewController<ProfileReviewMetadata, ProfileReviewCriteria, ProfessionalMetadata>
    {
        private readonly string MANDRILL_API_KEY = "ndg42WcyRHVLtLbvGqBjUA";

        public ReviewController()
            : base("MyHome.Controllers") 
        {
            base.OnReviewPosted += ReviewController_OnReviewPosted;
        }

        void ReviewController_OnReviewPosted(object sender, ReviewPostedArgs e)
        {
            // currently no email if proxy
            if (e.ReviewResponse.RevieweeProfile.IsProxy) return;

            // email professional
            var message = new EmailMessage
            {
                subject = string.Format(Localizer.Get("Mail_NewReview_Subject"), e.ReviewResponse.ReviewerProfile.GetProfileName()),
                to = new List<EmailAddress> {
                    new EmailAddress {
                        email = e.ReviewResponse.RevieweeProfile.ProfessionalInfo.CompanyContactInfo.Email
                    }
                },
            };
            message.AddGlobalVariable("CONTENT", string.Format(Localizer.Get("Mail_NewReview_Body"),
                e.ReviewResponse.RevieweeProfile.ProfessionalInfo.CompanyContactInfo.Name, 
                e.ReviewResponse.ReviewerProfile.ContactInfo.Name, 
                e.ReviewResponse.Review.Content, 
                string.Concat("https://www.homelab.com/", VirtualPathUtility.ToAbsolute(string.Concat("~/profile/", e.ReviewResponse.RevieweeProfile.Id, "/claim")))));
            var api = new MandrillApi(MANDRILL_API_KEY);
            var sendResponse = api.SendMessage(message, "notification_new_review", null);
        }
    }
}