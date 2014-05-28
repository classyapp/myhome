﻿using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Mvc.Extensions;
using Classy.DotNet.Responses;
using Classy.DotNet.Services;
using MyHome.Models;
using MyHome.Models.Polls;

namespace MyHome.Controllers
{
    public class PollController : Classy.DotNet.Mvc.Controllers.ListingController<PollMetadata, PhotoGridViewModel>
    {
        public PollController() : base("MyHome.Controllers") { }

        public override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.MapRoute(
                name: "SelectListingsModal",
                url: "polls/create/select-listings-modal",
                defaults: new { controller = "Poll", action = "SelectListingsModal" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "SelectListingPhotosModal",
                url: "polls/create/select-photos-modal",
                defaults: new { controller = "Poll", action = "SelectPhotosModal" },
                namespaces: new string[] { Namespace }
            );
        }

        public override string ListingTypeName
        {
	        get {  return "Poll"; }
        }

        public ActionResult SelectListingsModal()
        {
            var listingService = new ListingService();
            string collectionType = AuthenticatedUserProfile.IsProfessional ? CollectionType.Project : CollectionType.PhotoBook;
            var collections = listingService.GetCollectionsByProfileId(AuthenticatedUserProfile.Id, collectionType, false, false, false);

            return PartialView("SelectListingsModal", collections);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelectPhotosModal(string collectionId)
        {
            var listingService = new ListingService();
            var collectionView = listingService.GetCollectionById(collectionId, true, false, false, false);

            var listings = collectionView.Listings.Select(x => new {
                Id = x.Id,
                Title = x.Title,
                Image = x.ExternalMedia.IsNullOrEmpty() ? string.Empty : x.ExternalMedia[0].Key
            });

            return Json(listings);
        }
	}
}