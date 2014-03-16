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

namespace MyHome.Controllers
{
    public class PhotoController : Classy.DotNet.Mvc.Controllers.ListingController<MyHome.Models.PhotoMetadata>
    {
        public PhotoController()
            : base("MyHome.Controllers") {
                OnParseListingFromUrl += PhotoController_OnParseListingFromUrl;
        }

        public void PhotoController_OnParseListingFromUrl(object sender, CreateListingFromUrlArgs<MyHome.Models.PhotoMetadata> args)
        {
            if (args.Metadata == null) args.Metadata = new Models.PhotoMetadata();
            
            // parse the photo url
            var photoRegex = new Regex("photoUrl=([^&]*)");
            var match = photoRegex.Match(args.RequestUri.Query);
            args.ExternalMediaUrl = match.Groups[1].Value;

            // the origin url
            var creditRegex = new Regex("originUrl=([^&]*)");
             match = creditRegex.Match(args.RequestUri.Query);
            args.Metadata.CopyrightMessage = match.Groups[1].Value;
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