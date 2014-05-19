using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet;
using System.Text.RegularExpressions;
using Classy.DotNet.Services;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;

namespace MyHome.Controllers
{
    public class PhotoController : Classy.DotNet.Mvc.Controllers.ListingController<MyHome.Models.PhotoMetadata, MyHome.Models.PhotoGridViewModel>
    {
        private readonly string MANDRILL_API_KEY = "ndg42WcyRHVLtLbvGqBjUA";

        public PhotoController()
            : base("MyHome.Controllers") {
                base.OnUpdateListing += PhotoController_OnUpdateListing;
                base.OnPostedComment += PhotoController_OnPostedComment;
        }

        private void PhotoController_OnUpdateListing(object sender, ListingUpdateArgs e)
        {
            var supportedCultures = Localizer.GetList("supported-cultures").Select(x => x.Value).Where(x => x != "en").ToList();

            if (e.IsEditor && e.EditorKeywords != null)
            {
                var translator = new GoogleTranslationService();
                var translatedHashtags = new Dictionary<string, IList<string>>();
                translatedHashtags.Add("en", e.EditorKeywords);
                foreach (var englisKeyword in e.EditorKeywords)
                {
                    foreach (var language in supportedCultures)
                    {
                        if (!translatedHashtags.ContainsKey(language))
                            translatedHashtags.Add(language, new List<string>());

                        translatedHashtags[language].Add(translator.Translate(englisKeyword, "en", language));
                    }
                }
                e.TranslatedKeywords = translatedHashtags;
            }
        }

        private void PhotoController_OnPostedComment(object sender, ListingCommentEventArgs e)
        {
            // email professional
            var message = new EmailMessage
            {
                subject = Localizer.Get("PhotoComment_Notification_Subject", e.Comment.Profile.DefaultCulture ?? "en"),
                html = string.Format(Localizer.Get("PhotoComment_Notification_Body", e.Comment.Profile.DefaultCulture ?? "en"), e.Comment.Profile.GetProfileName(), AuthenticatedUserProfile.GetProfileName(),
                        string.Concat("https://", AppView.Hostname, Url.RouteUrl(ListingTypeName + "Details"), "?utm_source=internal&utm_medium=email&utm_campaign=comments")),
                to = new List<EmailAddress> {
                    new EmailAddress {
                        email = e.Comment.Profile.ContactInfo.Email
                    }
                },
                from_email = "team@homelab.com"
            };

            System.Text.RegularExpressions.Regex dirRegex = new System.Text.RegularExpressions.Regex("[\\p{IsHebrew}]");
            if (dirRegex.IsMatch(message.html.Substring(0, Math.Min(message.html.Length, 10))))
            {
                message.html = "<div style=\"direction: rtl\">" + message.html + "</div>";
            }

            message.AddHeader("Reply-To", "team@homelab.com");
            var api = new MandrillApi(MANDRILL_API_KEY);
            var sendResponse = api.SendMessage(message);   
        }

        public override string ListingTypeName
        {
	        get 
	        { 
		         return "Photo";
	        }
        }
    }
}
