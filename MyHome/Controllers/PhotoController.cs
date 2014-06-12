using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Mvc.ViewModels.Listing;
using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet;
using Classy.DotNet.Services;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using MyHome.Models;

namespace MyHome.Controllers
{
    public class PhotoController : ListingController<PhotoMetadata, PhotoGridViewModel>
    {
        private const string MANDRILL_API_KEY = "ndg42WcyRHVLtLbvGqBjUA";

        public PhotoController()
            : base("MyHome.Controllers") {
                base.OnUpdateListing += PhotoController_OnUpdateListing;
                base.OnPostedComment += PhotoController_OnPostedComment;
        }

        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRouteForSupportedLocales(
                name: string.Concat("UntaggedSearch", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/untagged/{date}"),
                defaults: new { controller = ListingTypeName, action = "UntaggedSearch", filters = "", listingType = ListingTypeName },
                namespaces: new string[] { Namespace }
            );

            base.RegisterRoutes(routes);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult UntaggedSearch(SearchUntaggedListingsViewModel model, string date)
        {
            try
            {
                var service = new ListingService();

                // search
                var searchResults = service.SearchUntaggedListings(model.Page, new[] { ListingTypeName }, date);

                var pageModel = new SearchListingsViewModel<PhotoMetadata>
                {
                    Results = searchResults.Results,
                    Count = searchResults.Count,
                    Metadata = new PhotoMetadata(),
                    Page = model.Page,
                    PagingUrl = Url.RouteUrlForCurrentLocale("UntaggedSearch" + ListingTypeName)
                };

                if (Request.IsAjaxRequest())
                {
                    return PartialView(string.Concat(ListingTypeName, "Grid"), new PhotoGridViewModel { Results = pageModel.Results });
                }
                else
                {
                    return View("Search", pageModel);
                }
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        private void PhotoController_OnUpdateListing(object sender, ListingUpdateArgs e)
        {
            var supportedCultures = AppView.SupportedCultures.Select(x => x.Value).Where(x => x != "en").ToList();

            if (!e.IsEditor || e.EditorKeywords == null) return;
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

        private void PhotoController_OnPostedComment(object sender, ListingCommentEventArgs e)
        {
            // email professional
            var message = new EmailMessage
            {
                subject = Localizer.Get("PhotoComment_Notification_Subject", e.Listing.Profile.DefaultCulture ?? "en"),
                html = string.Format(Localizer.Get("PhotoComment_Notification_Body", e.Listing.Profile.DefaultCulture ?? "en"), e.Listing.Profile.GetProfileName(), AuthenticatedUserProfile.GetProfileName(),
                        string.Concat("https://", AppView.Hostname, Url.RouteUrl(ListingTypeName + "Details"), "?utm_source=internal&utm_medium=email&utm_campaign=comments")),
                to = new List<EmailAddress> {
                    new EmailAddress {
                        email = e.Listing.Profile.IsProfessional ? e.Listing.Profile.ProfessionalInfo.CompanyContactInfo.Email : e.Listing.Profile.ContactInfo.Email
                    }
                },
                from_email = "team@homelab.com"
            };

            var dirRegex = new System.Text.RegularExpressions.Regex("[\\p{IsHebrew}]");
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
