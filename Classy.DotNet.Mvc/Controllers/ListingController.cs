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

            routes.MapRouteForSupportedLocales(
                name: string.Concat("Search", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{*filters}"),
                defaults: new { controller = ListingTypeName, action = "Search", filters = "", listingType = ListingTypeName },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: string.Concat(ListingTypeName, "Details"),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/{slug}"),
                defaults: new { controller = ListingTypeName, action = "GetListingById", slug = "show" },
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
            if (!ModelState.IsValid) return View(string.Concat("Create", ListingTypeName), model);

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

                return View(string.Concat("Create", ListingTypeName));
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

        //
        // GET: /{ListingTypeName}/search/{tag}/{*filters}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Search(SearchListingsViewModel<TListingMetadata> model)
        {
            try
            {
                var service = new ListingService();
                // add the filters from the url
                if (model.Filters != null)
                {
                    var strings = model.Filters.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    model.Metadata = new TListingMetadata().FromStringArray(strings);
                }
                var listings = service.SearchListings(
                    model.Tag,
                    model.ListingType,
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
        // POST: /{ListingTypeName}/search/room/style?tag=
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Search(SearchListingsViewModel<TListingMetadata> model, object dummyforpost)
        {
            // TODO: this next line works by chance for photos search.. should be replaced with some real logic 
            var f = model.Metadata.ToSlug();
            return RedirectToRoute(string.Concat("Search",ListingTypeName), new { tag = model.Tag, filters = f });
        }
    }
}
