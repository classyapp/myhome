using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ViewModels.Listing;
using Classy.DotNet.Services;
using ServiceStack.Text;
using Classy.DotNet.Mvc.ActionFilters;
using System.Net;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.Controllers
{
    public class ListingController<TListingMetadata> : BaseController
        where TListingMetadata : IMetadata<TListingMetadata>, new()
    {
        public virtual string ListingTypeName { get { return "Listing"; } }

        public ListingController() : base() { }
        public ListingController(string ns) : base(ns) { }

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRouteWithName(
                name: string.Concat("Create", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/new"),
                defaults: new { controller = ListingTypeName, action = "CreateListing" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat("PostCommentFor" ,ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/comments/new"),
                defaults: new { controller = ListingTypeName, action = "PostComment" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat("Favorite", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/favorite"),
                defaults: new { controller = ListingTypeName, action = "FavoriteListing" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat("Edit", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/edit"),
                defaults: new { controller = ListingTypeName, action = "EditListing" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: string.Concat(ListingTypeName, "Details"),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}--{slug}"),
                defaults: new { controller = ListingTypeName, action = "GetListingById", slug = "show" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: string.Concat("Search", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{*filters}"),
                defaults: new { controller = ListingTypeName, action = "Search", filters = "", listingType = ListingTypeName },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Format("PublicProfile{0}s", ListingTypeName),
                url: string.Concat("profile/{profileId}/all/", string.Format("{0}s", ListingTypeName.ToLower())),
                defaults: new { controller = ListingTypeName, action = "Index" },
                namespaces: new string[] { Namespace }
            );

        }

        //
        // GET: /{ListingTypeName}/new
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateListing()
        {
            return View(string.Concat("Create", ListingTypeName));
        }

        // POST: /{ListingTypeName}/new
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateListing(CreateListingViewModel<TListingMetadata> model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.AcceptTypes.Contains("application/json"))
                {
                    return Json(new { error = "invalid model" });
                }
                else
                {
                    return View(string.Concat("Create", ListingTypeName), model);
                }
            }

            PricingInfoView pricingInfo = null;
            if (model.PricingInfo != null)
            {
                pricingInfo = new PricingInfoView()
                {
                    SKU = model.PricingInfo.SKU,
                    Price = model.PricingInfo.Price,
                    CompareAtPrice = model.PricingInfo.CompareAtPrice,
                    Quantity = model.PricingInfo.Quantity.Value,
                    DomesticRadius = model.PricingInfo.DomesticRadius,
                    DomesticShippingPrice = model.PricingInfo.DomesticShippingPrice,
                    InternationalShippingPrice = model.PricingInfo.InternationalShippingPrice
                };
            }
            try
            {
                var service = new ListingService();
                var listing = service.CreateListing(
                    model.Title,
                    model.Content,
                    ListingTypeName,
                    pricingInfo,
                    (model.Metadata == null ? null : model.Metadata.ToDictionary()),
                    Request.Files);

                TempData["CreateListingSuccess"] = listing;

                if (Request.AcceptTypes.Contains("application/json"))
                {
                    return Json(listing);
                }
                else
                {
                    return View(string.Concat("Create", ListingTypeName));
                }
            }
            catch(ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                    return View(string.Concat("Create", ListingTypeName));
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }
        }

        //
        // GET: /{ListingTypeName}/{listingId}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        [ImportModelStateFromTempData]
        public ActionResult GetListingById(string listingId)
        {
            try
            {
                var service = new ListingService();
                var listing = service.GetListingById(
                    listingId,
                    true,
                    true,
                    true,
                    true,
                    true);
                var listingMetadata = new TListingMetadata().FromDictionary(listing.Metadata);
                var model = new ListingDetailsViewModel<TListingMetadata>
                {
                    Listing = listing,
                    Metadata = listingMetadata
                };
                return View(string.Concat(ListingTypeName, "Details"), model);
            }
            catch(ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // POST: /{ListingTypeName}/{listingId}/comments/new
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ExportModelStateToTempData]
        public ActionResult PostComment(string listingId, string content)
        {
            try
            {
                var service = new ListingService();
                service.PostComment(listingId, content);
            }
            catch(ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }

            return RedirectToAction("GetListingById", new { listingId = listingId });    
        }

        //
        // POST: /{ListingTypeName}/{listingId}/favorite
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FavoriteListing(string listingId)
        {
            try
            {
                var service = new ListingService();
                service.FavoriteListing(listingId);
            }
            catch (ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }

            return Json(new { IsValid = true });
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult EditListing(string listingId)
        {
            try
            {
                var service = new ListingService();
                var listing = service.GetListingById(
                    listingId,
                    false,
                    false,
                    false,
                    false,
                    false);
                var listingMetadata = new TListingMetadata().FromDictionary(listing.Metadata);
                var model = new UpdateListingViewModel<TListingMetadata>
                {
                    Id = listing.Id,
                    Title = listing.Title,
                    Content = listing.Content,
                    ExternalMedia = listing.ExternalMedia,
                    Metadata = listingMetadata
                };
                return PartialView(string.Format("Edit{0}ListingModal", ListingTypeName), model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditListing(UpdateListingViewModel<TListingMetadata> model)
        {
            try
            {
                try
                {
                    var service = new ListingService();
                    var listing = service.UpdateListing(
                        model.Id,
                        model.Title,
                        model.Content,
                        ListingTypeName,
                        null,
                        (model.Metadata == null ? null : model.Metadata.ToDictionary()),
                        null);

                    TempData["EditListingSuccess"] = true;

                    return PartialView(string.Format("Edit{0}ListingModal", ListingTypeName),
                        new UpdateListingViewModel<TListingMetadata> 
                        {
                            Id = listing.Id,
                            Title = listing.Title,
                            Content = listing.Content,
                            ExternalMedia = listing.ExternalMedia,
                            Metadata = new TListingMetadata().FromDictionary(listing.Metadata)
                        });
                }
                catch (ClassyException cvx)
                {
                    if (cvx.IsValidationError())
                    {
                        AddModelErrors(cvx);
                        return View(string.Concat("Create", ListingTypeName));
                    }
                    else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
                }
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // GET: /{ListingTypeName}/{*filters}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Search(SearchListingsViewModel<TListingMetadata> model, string filters)
        {
            try
            {
                var service = new ListingService();
                // add the filters from the url
                if (filters != null)
                {
                    var strings = filters.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (model.Metadata == null) model.Metadata = new TListingMetadata();
                    string tag;
                    LocationView location = null;
                    model.Metadata.ParseSearchFilters(strings, out tag, ref location);
                    model.Tag = tag;
                    model.Location = location;
                }
                // search
                var listings = service.SearchListings(
                    model.Tag,
                    ListingTypeName,
                    model.Metadata != null ? model.Metadata.ToDictionary() : null,
                    model.PriceMin,
                    model.PriceMax,
                    model.Location);
                model.Results = listings;

                if (model.Metadata == null) model.Metadata = new TListingMetadata();
                return View(model);
            }
            catch(ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        // 
        // POST: /{ListingTypeName}/{*filters}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Search(SearchListingsViewModel<TListingMetadata> model, object dummyforpost)
        {
            if (model.Metadata == null) model.Metadata = new TListingMetadata();
            var slug = model.Metadata.GetSearchFilterSlug(model.Tag, model.Location);
            return RedirectToRoute(string.Concat("Search",ListingTypeName), new { filters = slug });
        }

        //
        // GET: /profile/{profileId}/photos
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(string profileId)
        {
            try
            {
                var profileService = new ProfileService();
                var profile = profileService.GetProfileById(profileId, false, true, false, false, false);

                var listingService = new ListingService();
                bool includeDrafts = (User.Identity.IsAuthenticated && profileId == (User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Id);
                var listings = listingService.GetListingsByProfileId(profileId, true);

                var model = new ListingsViewModel<TListingMetadata>
                {
                    Profile = profile,
                    Listings = listings,
                    Metadata = default(TListingMetadata)
                };

                return View(model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }
    }
}
