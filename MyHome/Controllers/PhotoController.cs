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

namespace MyHome.Controllers
{
    public class PhotoController : Classy.DotNet.Mvc.Controllers.ListingController<MyHome.Models.PhotoMetadata, MyHome.Models.PhotoGridViewModel>
    {
        public PhotoController()
            : base("MyHome.Controllers") {
                base.OnUpdateListing += PhotoController_OnUpdateListing;
        }

        private void PhotoController_OnUpdateListing(object sender, ListingUpdateArgs e)
        {
            var supportedCultures = Localizer.GetList("supported-cultures").Select(x => x.Value).Where(x => x != "en").ToList();

            if (e.IsEditor && e.Hashtags != null)
            {
                var translator = new GoogleTranslationService();
                var translatedHashtags = new Dictionary<string, IList<string>>();
                foreach (var englisKeyword in e.Hashtags)
                {
                    foreach (var language in supportedCultures)
                    {
                        if (!translatedHashtags.ContainsKey(language))
                            translatedHashtags.Add(language, new List<string>());

                        translatedHashtags[language].Add(translator.Translate(englisKeyword, "en", language));
                    }
                }
                e.EditorKeywords = translatedHashtags;
            }
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