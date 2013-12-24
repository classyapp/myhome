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
    public class ReviewController : Classy.DotNet.Mvc.Controllers.ReviewController<ProfessionalMetadata, ProfileReviewCriteria>
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
            if (!e.ReviewResponse.RevieweeProfile.IsProxy) return;

            // email professional
            var message = new EmailMessage
            {
                subject = string.Format("{0} נתנה לך ציון של 3 כוכבים באתר רילי", e.ReviewResponse.ReviewerProfile.ContactInfo.Name),
                to = new List<EmailAddress> {
                    new EmailAddress {
                        email = e.ReviewResponse.RevieweeProfile.ContactInfo.Email
                    }
                },
            };
            message.AddGlobalVariable("AGENT_NAME", e.ReviewResponse.RevieweeProfile.SellerInfo.ContactInfo.Name);
            message.AddGlobalVariable("REVIEWER_NAME", e.ReviewResponse.ReviewerProfile.ContactInfo.Name);
            message.AddGlobalVariable("REVIEW", e.ReviewResponse.Review.Content);
            message.AddGlobalVariable("CLAIM_URL", VirtualPathUtility.ToAbsolute(string.Concat("~/profile/", e.ReviewResponse.RevieweeProfile.Id, "/claim")));
            var api = new MandrillApi(MANDRILL_API_KEY);
            var sendResponse = api.SendMessage(message, "notification_new_review", null);
        }
    }
}