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
using Classy.DotNet.Mvc.Attributes;
using System.Web;

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

            routes.MapRouteWithName(
                name: string.Concat("Create", ListingTypeName, "FromUrl"),
                url: string.Concat(ListingTypeName.ToLower(), "/new/fromurl"),
                defaults: new { controller = ListingTypeName, action = "CreateListingFromUrl" },
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
                name: string.Concat("Unfavorite", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/unfavorite"),
                defaults: new { controller = ListingTypeName, action = "UnfavoriteListing" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat("Edit", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/edit"),
                defaults: new { controller = ListingTypeName, action = "EditListing" },
                namespaces: new string[] { Namespace }
            );
            
            routes.MapRoute(
                name: string.Concat("Delete", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/delete"),
                defaults: new { controller = ListingTypeName, action = "DeleteListing" },
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
                defaults: new { controller = ListingTypeName, action = "ShowListingsByType" },
                namespaces: new string[] { Namespace }
            );

        }

        //
        // GET: /{ListingTypeName}/new
        // 
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateListing()
        {
            CreateListingViewModel<TListingMetadata> model = new CreateListingViewModel<TListingMetadata>();
            string collectionType = AuthenticatedUserProfile.IsProfessional ? CollectionType.Project : CollectionType.PhotoBook;
            model.CollectionList = GetCollectionList(model.CollectionId, collectionType);
            model.CollectionType = collectionType;
            return View(string.Concat("Create", ListingTypeName), model);
        }

        //
        // GET: /{ListingTypeName}/new/fromurl
        // 
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateListingFromUrl(string originUrl, string externalMediaUrl)
        {
            var model = new CreateListingFromUrlViewModel<TListingMetadata>();
            model.OriginUrl = originUrl;
            model.ExternalMediaUrl = externalMediaUrl;
            model.CollectionList = Request.IsAuthenticated ? GetCollectionList(model.CollectionId, CollectionType.PhotoBook) : null;

            return View(string.Concat("Create", ListingTypeName, "FromUrl"), model);
        }

        //
        // POST: /{ListingTypeName}/new/fromurl
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateListingFromUrl(CreateListingFromUrlViewModel<TListingMetadata> model)
        {
            // create the listing
            var listingService = new ListingService();
            var listing = listingService.CreateListing(
                model.Title,
                model.Content,
                ListingTypeName,
                model.PricingInfo.ToPricingInfo(),
                model.Metadata != null ? model.Metadata.ToDictionary() : null,
                model.ExternalMediaUrl);

            // add to the selected collection
            var includedListings = new List<IncludedListingView> { new IncludedListingView { Id = listing.Id, ListingType = ListingTypeName, ProfileId = AuthenticatedUserProfile.Id } };
            if (string.IsNullOrEmpty(model.CollectionId))
            {
                var collection = listingService.CreateCollection(CollectionType.PhotoBook, model.Title, model.Content, includedListings);
                model.CollectionId = collection.Id;
            }
            else listingService.AddListingToCollection(model.CollectionId, includedListings);

            // search for a matching professional
            var originDomain = new Uri(model.OriginUrl).Host;
            var profileService = new ProfileService();
            var matches = profileService.SearchProfiles(originDomain, null, null, null, true, true, 1);

            // if professional found, create a web clips collection and add the listing
            var pro = matches.Count > 0 ? matches.Results[0] : null;
            if (pro != null)
            {
                var collections = listingService.GetCollectionsByProfileId(pro.Id, CollectionType.WebPhotos, false, false, false);
                var collection = collections.FirstOrDefault();
                if (collection == null)
                {
                    listingService.CreateCollection(pro.Id, CollectionType.WebPhotos, "web-photos", null, includedListings);
                }
                else
                {
                    listingService.AddListingToCollection(collection.Id, includedListings);
                }
            }

            // load collections
            model.CollectionList = Request.IsAuthenticated ? GetCollectionList(model.CollectionId, CollectionType.PhotoBook) : null;

            TempData["CreateListing_Success"] = true;
            return View(string.Concat("Create", ListingTypeName, "FromUrl"), model);
        }

        private SelectList GetCollectionList(string selectedCollectionId, string type)
        {
            var service = new ListingService();
            var collectionList = service.GetCollectionsByProfileId(AuthenticatedUserProfile.Id, type, false, false, false);
            return new SelectList(collectionList, "Id", "Title", selectedCollectionId);
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

            var service = new ListingService();

            // check colelction exists
            if (string.IsNullOrEmpty(model.CollectionId))
            {
                var collection = service.CreateCollection(model.CollectionType, model.Title, model.Content, new IncludedListingView[0]);
                model.CollectionId = collection.Id;
            }

            PricingInfoView pricingInfo = model.PricingInfo.ToPricingInfo();

            try
            {
                var listing = service.CreateListing(
                    model.Title,
                    string.Empty,
                    ListingTypeName,
                    pricingInfo,
                    (model.Metadata == null ? null : model.Metadata.ToDictionary()),
                    Request.Files);
                service.AddListingToCollection(model.CollectionId, new IncludedListingView[] { 
                    new IncludedListingView { ListingType = ListingTypeName, Id = listing.Id, Comments = string.Empty } });

                if (Request.AcceptTypes.Contains("application/json"))
                {
                    return Json(new { listing = listing, collectionId = model.CollectionId });
                }
                else
                {
                    string url = Url.RouteUrl("PublicProfilePhotos", new { profileId = (User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Id }) + "?photosUploaded=1";
                    return Redirect(url);
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
                TempData["PostComment_Success"] = true;
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
        // POST: /{ListingTypeName}/{listingId}/unfavorite
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnfavoriteListing(string listingId)
        {
            try
            {
                var service = new ListingService();
                service.UnfavoriteListing(listingId);
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
                if (ModelState.IsValid)
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

                    return Json(new { IsValid = true });
                }
                else return PartialView(string.Format("Edit{0}ListingModal", ListingTypeName), model);
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

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteListing(string listingId)
        {
            try
            {
                var service = new ListingService();
                var listing = service.GetListingById(listingId, false, true, false, false, false);
                if (listing.Profile.CanEdit())
                {
                    service.DeleteListing(listingId);
                    return Json(new { listingId = listingId });
                }
                else
                {
                    return Json(new { error = "Not Authorized"});
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() });
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
                var results = service.SearchListings(
                    model.Tag,
                    ListingTypeName,
                    model.Metadata != null ? model.Metadata.ToDictionary() : null,
                    model.PriceMin,
                    model.PriceMax,
                    model.Location,
                    model.Page);
                model.Results = results.Results;
                model.Count = results.Count;

                if (model.Metadata == null) model.Metadata = new TListingMetadata();

                if (Request.IsAjaxRequest())
                {
                    return PartialView("PhotoGrid", model.Results);
                }
                else
                {
                    return View(model);
                }
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
        // GET: /profile/{ProfileId}/all/{ListingTypeName}s
        // 
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ShowListingsByType(string profileId)
        {
            try
            {
                var profileService = new ProfileService();
                var profile = profileService.GetProfileById(profileId, false, true, false, false, false, false);

                var listingService = new ListingService();
                bool includeDrafts = (Request.IsAuthenticated && profileId == AuthenticatedUserProfile.Id);
                var listings = listingService.GetListingsByProfileId(profileId, includeDrafts);

                var model = new ShowListingByTypeViewModel<TListingMetadata>
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
