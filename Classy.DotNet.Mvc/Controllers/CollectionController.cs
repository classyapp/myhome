﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ViewModels.Collection;
using Classy.DotNet.Mvc.ActionFilters;
using Classy.DotNet.Services;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.Controllers
{
    public class CollectionController : BaseController
    {
        public CollectionController() : base() { }
        public CollectionController(string ns) : base(ns) { }

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRouteWithName(
                name: "AddListingToCollection",
                url: "collection/new",
                defaults: new { controller = "Collection", action = "AddListingToCollection" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "EditCollection",
                url: "collection/{collectionId}/edit",
                defaults: new { controller = "Collection", action = "EditCollection" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "CollectionDetails",
                url: "collection/{collectionId}/{slug}",
                defaults: new { controller = "Collection", action = "CollectionDetails", slug = "show" },
                namespaces: new string[] { Namespace }
            );
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CollectionDetails(string collectionId, string view)
        {
            try
            {
                var service = new ListingService();
                var collection = service.GetCollectionById(collectionId, true, true, false);
                return View((view ?? "grid").ToLower() == "list" ? "CollectionDetailsList" : "CollectionDetailsGrid", collection);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult AddListingToCollection(string[] listingIds)
        {
            try
            {
                var model = new AddToCollectionViewModel();
                model.CollectionList = GetCollectionList(model.CollectionId);
                return PartialView("AddListingToCollectionModal", model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult AddListingToCollection(AddToCollectionViewModel model)
        {
            try
            {
                // manual validation
                if (string.IsNullOrEmpty(model.CollectionId) && string.IsNullOrEmpty(model.Title))
                {
                    ModelState.AddModelError("Title", "The title field is required");
                }

                if (ModelState.IsValid)
                {
                    var service = new ListingService();
                    // create new collection
                    if (string.IsNullOrEmpty(model.CollectionId))
                    {
                        var collection = service.CreateCollection(model.Title, null, model.ListingIds);
                        model.CollectionId = collection.Id;
                    }
                    // add listings to collection
                    else
                    {
                        service.AddListingToCollection(model.CollectionId, model.ListingIds);
                    }
                }
                model.CollectionList = GetCollectionList(model.CollectionId);
                return PartialView("AddListingToCollectionModal", model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult EditCollection(string collectionId)
        {
            var service = new ListingService();
            var collection = service.GetCollectionById(collectionId, true, false, false);
            return View(collection);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult EditCollection(string collectionId, FormCollection values)
        {
            var service = new ListingService();
            // build comments list
            IList<IncludedListing> listings = new List<IncludedListing>();
            foreach (var key in values.AllKeys)
            {
                if (key.StartsWith("comment_"))
                {
                    listings.Add(new IncludedListing { Comments = values[key], ListingId = key.Substring(8) });
                }
            }
            var collection = service.UpdateCollection(collectionId, values["Title"], values["Content"], listings);
            return View(collection);
        }

        private SelectList GetCollectionList(string selectedCollectionId)
        {
            var service = new ListingService();
            var collectionList = service.GetCollectionsByProfileId(AuthenticatedUserProfile.Id, false, false, false);
            return new SelectList(collectionList, "Id", "Title", selectedCollectionId);
        }
    }
}
