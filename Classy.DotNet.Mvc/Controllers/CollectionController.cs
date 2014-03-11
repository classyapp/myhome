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
using Classy.DotNet.Mvc.Attributes;

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

            routes.MapRouteWithName(
                name: "DeleteCollection",
                url: "collection/{collectionId}/delete",
                defaults: new { controller = "Collection", action = "DeleteCollection" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "RemoveListing",
                url: "collection/{collectionId}/remove/{listingId}",
                defaults: new { controller = "Collection", action = "RemoveListing" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "CollectionDetails",
                url: "collection/{collectionId}/{view}/{slug}",
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
        public ActionResult AddListingToCollection(string listingId)
        {
            try
            {
                var model = new AddToCollectionViewModel();
                model.CollectionList = GetCollectionList(model.CollectionId, CollectionType.PhotoBook);
                model.IncludedListings = new IncludedListingView[]
                {
                    new IncludedListingView {
                        Id = listingId
                    }
                };
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
                        var collection = service.CreateCollection(CollectionType.PhotoBook, model.Title, null, model.IncludedListings);
                        model.CollectionId = collection.Id;
                    }
                    // add listings to collection
                    else
                    {
                        service.AddListingToCollection(model.CollectionId, model.IncludedListings);
                    }

                    return Json(new { IsValid = true });
                }
            }
            catch (ClassyException cex)
            {
                if (cex.IsValidationError())
                {
                    AddModelErrors(cex);
                }
                else return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
            model.CollectionList = GetCollectionList(model.CollectionId, CollectionType.PhotoBook); 
            return PartialView("AddListingToCollectionModal", model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [AuthorizeWithRedirect("Home")]
        public ActionResult EditCollection(string collectionId)
        {
            var service = new ListingService();
            var collection = service.GetCollectionById(collectionId, true, false, false);

            return View(new EditCollectionViewModel
            {
                CollectionId = collectionId,
                Title = collection.Title,
                Content = collection.Content,
                Listings = collection.Listings,
                IncludedListings = collection.IncludedListings.ToArray()
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult EditCollection(EditCollectionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var service = new ListingService();
                var collection = service.UpdateCollection(model.CollectionId, model.Title, model.Content, model.IncludedListings);
                TempData["UpdateCollectionSuccess"] = true;

                return View(new EditCollectionViewModel
                {
                    CollectionId = collection.Id,
                    Title = collection.Title,
                    Content = collection.Content,
                    Listings = collection.Listings,
                    IncludedListings = collection.IncludedListings.ToArray()
                });
            }
            catch (Exception ex)
            {
                TempData["UpdateCollectionError"] = ex.Message;
                return View(model);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult RemoveListing(string collectionId, string listingId)
        {
            try
            {
                var service = new ListingService();
                service.RemoveListingFromCollection(collectionId, new string[] { listingId });

                return Json(new { collectionId = collectionId, listingId = listingId });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult DeleteCollection(string collectionId)
        {
            try
            {
                var service = new ListingService();
                service.DeleteCollection(collectionId);
                TempData["DeleteCollection_Success"] = true;

                return RedirectToRoute("PublicProfile", new { profileId = AuthenticatedUserProfile.Id });
            }
            catch (Exception ex)
            {
                TempData["DeleteCollection_Error"] = ex.Message;
                return RedirectToAction("CollectionDetails", new { collectionId = collectionId, view = "grid", slug = "public" });
            }
        }

        private SelectList GetCollectionList(string selectedCollectionId, string collectionType)
        {
            var service = new ListingService();
            var collectionList = service.GetCollectionsByProfileId(AuthenticatedUserProfile.Id, collectionType, false, false, false);
            return new SelectList(collectionList, "Id", "Title", selectedCollectionId);
        }
    }
}
