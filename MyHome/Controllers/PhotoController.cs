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
            //if (e.IsEditor)
            {
                var translator = new GoogleTranslationService();
                var translatedHashtags = new Dictionary<string, IList<string>>();
                foreach (var englisKeyword in e.Hashtags)
                {
                    foreach (var language in GoogleTranslationService.SupportedLanguages.Except(new[] { "en" }))
                    {
                        if (!translatedHashtags.ContainsKey(language))
                            translatedHashtags.Add(language, new List<string>());

                        translatedHashtags[language].Add(translator.Translate(englisKeyword, "en", language));
                    }
                }
                e.EditorKeywords = translatedHashtags;
            }
        }

        private void PhotoController_OnPostedComment(object sender, ListingCommentEventArgs e)
        {
            // email professional
            var message = new EmailMessage
            {
                subject = string.Format(Localizer.Get("ListingComment_Notification_Subject"), ListingTypeName.ToLowerInvariant()),
                html = string.Format(Localizer.Get("ListingComment_Notification_Body"), e.Comment.Profile.ContactInfo.Name, ListingTypeName.ToLowerInvariant(),
                        string.Concat("https://", AppView.Hostname, Url.RouteUrl(ListingTypeName + "Details", new { listingId = e.ListingId, slug = ListingTypeName.ToLowerInvariant() }))),
                to = new List<EmailAddress> {
                    new EmailAddress {
                        email = e.Comment.Profile.ContactInfo.Email
                    }
                }
            };
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