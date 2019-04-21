using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Config;
using Classy.DotNet.Mvc.Extensions;
using Classy.DotNet.Mvc.ViewModels.Listing;
using Classy.DotNet.Services;
using Classy.DotNet.Mvc.ActionFilters;
using System.Net;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using Classy.DotNet.Mvc.Attributes;
using System.Web;

namespace Classy.DotNet.Mvc.Controllers
{
    public class ListingController<TListingMetadata, TListingGridViewModel> : BaseController
        where TListingMetadata : IMetadata<TListingMetadata>, new()
        where TListingGridViewModel : IListingGridViewModel, new()
    {
        public virtual string ListingTypeName { get { return "Listing"; } }

        public ListingController() : base() { }
        public ListingController(string ns) : base(ns) { }

        public EventHandler<ListingUpdateArgs> OnUpdateListing;
        public EventHandler<ListingCommentEventArgs> OnPostedComment;
        public EventHandler<ListingLoadedEventArgs<TListingMetadata>> OnListingLoaded;

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

            routes.MapRouteWithName(
                name: string.Concat("Create", ListingTypeName, "NoCollection"),
                url: string.Concat(ListingTypeName.ToLower(), "/new/nocollection"),
                defaults: new { controller = ListingTypeName, action = "CreateListingNoCollection" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRoute(
                name: string.Concat("PostCommentFor", ListingTypeName),
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

            routes.MapRoute(
                name: string.Concat("Translate", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/translate/{cultureCode}"),
                defaults: new { controller = ListingTypeName, action = "TranslateListing", cultureCode = UrlParameter.Optional },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: string.Concat(ListingTypeName, "Details"),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}--{slug}"),
                defaults: new { controller = ListingTypeName, action = "GetListingById", slug = "show" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat(ListingTypeName, "MoreInfo"),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/more"),
                defaults: new { controller = ListingTypeName, action = "GetListingMoreInfo" },
                namespaces: new string[] { Namespace }
            );
            
            routes.MapRouteForSupportedLocales(
                name: string.Concat("Search", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{*filters}"),
                defaults: new { controller = ListingTypeName, action = "Search", filters = "", listingType = ListingTypeName },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat("FreeSearch", ListingTypeName),
                url: "search/{q}",
                defaults: new { controller = ListingTypeName, action = "FreeSearch" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Format("PublicProfile{0}s", ListingTypeName),
                url: string.Concat("profile/{profileId}/all/", string.Format("{0}s", ListingTypeName.ToLower())),
                defaults: new { controller = ListingTypeName, action = "ShowListingsByType" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Format("EditMultiple{0}s", ListingTypeName),
                url: "listings/edit-multiple",
                defaults: new { controller = ListingTypeName, action = "EditMultipleListings" },
                namespaces: new string[] { Namespace }
            );
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditMultipleListings(string[] listingIds, int? editorsRank, string room, string style)
        {
            var listingService = new ListingService();
            listingService.EditMultipleListings(listingIds, editorsRank, room, style);

            return new JsonResult();
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
            model.IsGoogleConnected = AuthenticatedUserProfile.IsGoogleConnected;
            model.IsFacebookConnected = AuthenticatedUserProfile.IsFacebookConnected;
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
            model.CollectionList = Request.IsAuthenticated ? GetCollectionList(model.CollectionId, CollectionType.PhotoBook) : new SelectList(new List<CollectionView>());
            model.CollectionType = CollectionType.PhotoBook;

            return View(string.Concat("Create", ListingTypeName, "FromUrl"), model);
        }

        //
        // POST: /{ListingTypeName}/new/fromurl
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateListingFromUrl(CreateListingFromUrlViewModel<TListingMetadata> model)
        {
            // parse the listing from the origin url
            // TODO: add an event that allows subclasses to parse the listing from the page at the originUrl

            // create the listing
            var listingService = new ListingService();
            var listing = listingService.CreateListing(
                model.Title,
                model.Content,
                ListingTypeName,
                model.Categories,
                null,
                model.Metadata != null ? model.Metadata.ToDictionary() : null,
                model.ExternalMediaUrl);

            // add to the selected collection
            var includedListings = new List<IncludedListingView> { new IncludedListingView { Id = listing.Id, ListingType = ListingTypeName, ProfileId = AuthenticatedUserProfile.Id } };
            if (string.IsNullOrEmpty(model.CollectionId))
            {
                string collectionType = string.IsNullOrEmpty(model.CollectionType) ? CollectionType.PhotoBook : model.CollectionType;
                var collection = listingService.CreateCollection(collectionType, model.Title, model.Content, includedListings);
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
            if (Request.IsAjaxRequest())
            {
                return Json(new { listing = listing, collectionId = model.CollectionId });
            }
            else
            {
                return View(string.Concat("Create", ListingTypeName, "FromUrl"), model);
            }
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

            try
            {
                var listing = service.CreateListing(
                    model.Title,
                    string.Empty,
                    ListingTypeName,
                    model.Categories,
                    model.PricingInfo,
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
                    string url = Url.RouteUrl(string.Format("PublicProfile{0}s", ListingTypeName), new { profileId = (User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Id }) + string.Format("?{0}sUploaded=1", ListingTypeName.ToLower());
                    return Redirect(url);
                }
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

        //
        // GET: /{ListingTypeName}/{listingId}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        [ImportModelStateFromTempData]
        public ActionResult GetListingById(string listingId)
        {
            var service = new ListingService();

            if (MobileRedirect.IsMobileDevice())
                if (ListingTypeName == "Product")
                    return Redirect("~/Mobile/App.html#/Product/" + listingId);

            try
            {
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
                var listingLoadedEventArgs = new ListingLoadedEventArgs<TListingMetadata> {
                    ListingDetailsViewModel = model
                };
                if (OnListingLoaded != null)
                    OnListingLoaded(this, listingLoadedEventArgs);
                model.ExtraData = listingLoadedEventArgs.ListingDetailsViewModel.ExtraData;

                return View(string.Concat(ListingTypeName, "Details"), model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // POST: /{ListingTypeName}/{listingId}/comments/new
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [ExportModelStateToTempData]
        public ActionResult PostComment(string listingId, string content)
        {
            try
            {
                var service = new ListingService();
                var comment = service.PostComment(listingId, content, ListingService.ObjectType.Listing);
                var listing = service.GetListingById(listingId, false, true, false, false, false);
                if (OnPostedComment != null)
                {
                    OnPostedComment(this, new ListingCommentEventArgs { Comment = comment, Listing = listing });
                }
                TempData["PostComment_Success"] = true;
            }
            catch (ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }

            if (HttpContext.Request.UrlReferrer.ToString().Contains("/Mobile/"))
                return Content("OK");

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

        [AuthorizeWithRedirect("Home")]
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
                    false, 
                    true);
                if (!listing.CanEdit()) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                var listingMetadata = new TListingMetadata().FromDictionary(listing.Metadata);
                var model = new UpdateListingViewModel<TListingMetadata>
                {
                    Id = listing.Id,
                    Title = listing.Title,
                    Content = listing.Content,
                    ExternalMedia = listing.ExternalMedia,
                    Metadata = listingMetadata,
                    DefaultCulture = listing.DefaultCulture,
                    IsEditor = AuthenticatedUserProfile.IsEditor || AuthenticatedUserProfile.IsAdmin,
                    Hashtags = listing.Hashtags,
                    EditorKeywords = listing.TranslatedKeywords != null && listing.TranslatedKeywords.ContainsKey("en") ? listing.TranslatedKeywords["en"] : new[] { "" },
                    TranslatedKeywords = listing.TranslatedKeywords,
                    SearchableKeywords = listing.SearchableKeywords,
                    EditorsRank = listing.EditorsRank,
                    PricingInfo = listing.PricingInfo,
                    Categories = (listing.Categories == null ? null : listing.Categories.ToArray())
                };
                return View(string.Format("Edit{0}", ListingTypeName), model);
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
                var updatedListingArgs = new ListingUpdateArgs
                {
                    IsEditor = AuthenticatedUserProfile.IsEditor,
                    EditorKeywords = model.EditorKeywords
                };

                if (OnUpdateListing != null)
                {
                    OnUpdateListing(this, updatedListingArgs);
                }

                var fields = ListingUpdateFields.Title | ListingUpdateFields.Content |
                    ListingUpdateFields.Metadata | ListingUpdateFields.Hashtags;

                Dictionary<string, string> errors = new Dictionary<string, string>();

                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    errors.Add("Title", Localizer.Get("CreateListing_TitleRequired"));
                }
                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    errors.Add("Content", Localizer.Get("CreateListing_ContentRequired"));
                }
                if (string.IsNullOrWhiteSpace(Request["Categories[]"]))
                {
                    errors.Add("Categories", Localizer.Get("CreateListing_CategoryRequired"));
                }
                else
                {
                    model.Categories = Request["Categories[]"].Split(',');
                }

                if (model.EditorKeywords != null && AuthenticatedUserProfile.IsEditor) fields |= ListingUpdateFields.EditorKeywords;
                if (model.PricingInfo != null)
                {
                    fields |= ListingUpdateFields.Pricing;
                    if (string.IsNullOrWhiteSpace(Request["Images[]"]))
                    {
                        if (Request["HasImages"] == "false")
                        {
                            errors.Add("Images", Localizer.Get("CreateListing_ImagesRequired"));
                        }
                    }
                    else
                    {
                        model.PricingInfo.BaseOption.MediaFiles = Request["Images[]"].Split(',').Select(key => new MediaFileView { Key = key }).ToArray(); ;
                    }
                    // Assemble Variant dictionary
                    if (model.PricingInfo.PurchaseOptions != null)
                    {
                        for (int i = 0; i < model.PricingInfo.PurchaseOptions.Count; i++)
                        {
                            Dictionary<string, string> vprops = new Dictionary<string, string>();
                            var po = model.PricingInfo.PurchaseOptions[i];
                            if (po.Color != null && po.Color != "_") { vprops.Add("Color", po.Color); }
                            if (po.Design != null && po.Design != "_") { vprops.Add("Design", po.Design); }
                            if (po.Size != null && po.Size != "_") { vprops.Add("Size", po.Size); }
                            po.VariantProperties = vprops;
                        }
                    }
                    ValidatePricingInfo(model.Id, model.PricingInfo, errors);
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
                if (ModelState.IsValid)
                {
                    var service = new ListingService();
                    var listing = service.UpdateListing(
                        model.Id,
                        model.Title,
                        model.Content,
                        model.Categories,
                        model.PricingInfo,
                        (model.Metadata == null ? null : model.Metadata.ToDictionary()),
                        model.Hashtags,
                        updatedListingArgs.TranslatedKeywords,
                        fields);

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { listingId = listing.Id });
                    }
                    return Redirect(Url.RouteUrl(string.Format("{0}Details", ListingTypeName), new { listingId = listing.Id, slug = "show" }) + "?msg=" + string.Format("Edit{0}_Success", ListingTypeName));
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { errors = errors });
                    }
                    else
                    {
                        return PartialView(string.Format("Edit{0}", ListingTypeName), model);
                    }
                } 
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

        private void ValidatePricingInfo(string listingId, PricingInfoView pricingInfoView, Dictionary<string,string> errors)
        {
            if (!AppView.SupportedCurrencies.Any(c => c.Value == pricingInfoView.CurrencyCode))
            {
                errors.Add("PricingInfo.CurrencyCode", Localizer.Get("Product_CurrencyRequired"));
            }
            if (pricingInfoView.BaseOption == null)
            {
                errors.Add("PricingInfo.BaseOption", Localizer.Get("Product_BasicDetailsRequired"));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pricingInfoView.BaseOption.ProductUrl))
                {
                    errors.Add("PricingInfo.BaseOption.ProductUrl", Localizer.Get("Product_ProductUrlRequired"));
                }
            }
            if (pricingInfoView.PurchaseOptions != null)
            {
                Dictionary<string, int> skus = new Dictionary<string, int>();
                for (int i = 0; i < pricingInfoView.PurchaseOptions.Count; i++)
                {
                    if (pricingInfoView.PurchaseOptions[i].Available)
                    {
                        if (
                            !string.IsNullOrEmpty(
                                Request["PricingInfo.PurchaseOptions[" + i.ToString() + "].MediaFiles[]"]))
                        {
                            pricingInfoView.PurchaseOptions[i].MediaFiles =
                                Request["PricingInfo.PurchaseOptions[" + i.ToString() + "].MediaFiles[]"].Split(',')
                                    .Select(key => new MediaFileView {Key = key})
                                    .ToArray();
                        }
                        else if (!pricingInfoView.PurchaseOptions[i].HasImages)
                        {
                            errors.Add("PricingInfo.PurchaseOptions[" + i.ToString() + "].Images",
                                Localizer.Get("Product_ImagesRequired"));
                        }
                        ValidatePurchaseOption(pricingInfoView.PurchaseOptions[i], i, errors, skus);
                    }
                }

                if (skus.Count == pricingInfoView.PurchaseOptions.Count) // Unique within the product
                {
                    // Validate global uniqueness within Vendor
                    var service = new ListingService();
                    List<string> duplicateSkus = service.ValidateUniqueSKUs(listingId, skus);

                    if (duplicateSkus.Count > 0)
                    {
                        for (int i = 0; i < pricingInfoView.PurchaseOptions.Count; i++)
                        {
                            if (duplicateSkus.Contains(pricingInfoView.PurchaseOptions[i].SKU))
                            {
                                errors.Add("PricingInfo.PurchaseOptions[" + i.ToString() + "].SKU", Localizer.Get("Product_UniqueSKURequired"));
                            }
                        }
                    }
                }

            }
            else
            {
                if (pricingInfoView.BaseOption.Price <= 0)
                {
                    errors.Add("PricingInfo.BaseOption.Price", Localizer.Get("Product_ValidPriceRequired"));
                }
                if (pricingInfoView.BaseOption.Quantity <= 0)
                {
                    errors.Add("PricingInfo.BaseOption.Quantity", Localizer.Get("Product_ValidQuantityRequired"));
                }
                if (string.IsNullOrWhiteSpace(pricingInfoView.BaseOption.SKU))
                {
                    errors.Add("PricingInfo.BaseOption.SKU", Localizer.Get("Product_SKURequired"));
                }
                else
                {
                    // Validate global uniqueness within Vendor
                    var service = new ListingService();
                    List<string> duplicateSkus = service.ValidateUniqueSKUs(listingId,
                        new Dictionary<string, int> {{pricingInfoView.BaseOption.SKU, 0}});
                    if (duplicateSkus.Count > 0)
                    {
                        errors.Add("PricingInfo.BaseOption.SKU", Localizer.Get("Product_UniqueSKURequired"));
                    }
                }
                if (string.IsNullOrWhiteSpace(pricingInfoView.BaseOption.Width))
                {
                    errors.Add("PricingInfo.BaseOption.Width", Localizer.Get("Product_WidthRequired"));
                }
                if (string.IsNullOrWhiteSpace(pricingInfoView.BaseOption.Depth))
                {
                    errors.Add("PricingInfo.BaseOption.Depth", Localizer.Get("Product_DepthRequired"));
                }
                if (string.IsNullOrWhiteSpace(pricingInfoView.BaseOption.Height))
                {
                    errors.Add("PricingInfo.BaseOption.Height", Localizer.Get("Product_HeightRequired"));
                }
            }

            // ensure unique sku and no duplicate variants
            if (pricingInfoView.PurchaseOptions != null)
            {
                Dictionary<string, int> variants = new Dictionary<string, int>();
                foreach (var option in pricingInfoView.PurchaseOptions)
                {
                    string[] arr = option.VariantProperties.Select(x => string.Join(";", x.Key.ToLowerInvariant(), x.Value.ToLowerInvariant())).ToArray();
                    Array.Sort(arr);
                    string variant = string.Join(";", arr);
                    if (variants.ContainsKey(variant))
                    {
                        errors.Add("PricingInfo", Localizer.Get("Product_DuplicateVariationDetected"));
                    }
                    else
                    {
                        variants.Add(variant, 0);
                    }
                }
            }
        }

        private void ValidatePurchaseOption(PurchaseOptionView option, int idx, Dictionary<string, string> errors, Dictionary<string, int> skus)
        {
            if (string.IsNullOrWhiteSpace(option.SKU))
            {
                errors.Add(string.Format("PricingInfo.PurchaseOptions[{0}].SKU", idx), Localizer.Get("Product_SKURequired"));
            }
            else
            {
                if (skus.ContainsKey(option.SKU))
                {
                    errors.Add(string.Format("PricingInfo.PurchaseOptions[{0}].SKU", idx), Localizer.Get("Product_UniqueSKURequired"));
                }
                else
                {
                    skus.Add(option.SKU, idx);
                }
            }
            if (option.Price <= 0)
            {
                errors.Add(string.Format("PricingInfo.PurchaseOptions[{0}].Price", idx), Localizer.Get("Product_ValidPriceRequired"));
            }
            if (option.Quantity <= 0)
            {
                errors.Add(string.Format("PricingInfo.PurchaseOptions[{0}].Quantity", idx), Localizer.Get("Product_ValidQuantityRequired"));
            }
            if (string.IsNullOrWhiteSpace(option.Width))
            {
                errors.Add(string.Format("PricingInfo.PurchaseOptions[{0}].Width", idx), Localizer.Get("Product_WidthRequired"));
            }
            if (string.IsNullOrWhiteSpace(option.Depth))
            {
                errors.Add(string.Format("PricingInfo.PurchaseOptions[{0}].Depth", idx), Localizer.Get("Product_DepthRequired"));
            }
            if (string.IsNullOrWhiteSpace(option.Height))
            {
                errors.Add(string.Format("PricingInfo.PurchaseOptions[{0}].Height", idx), Localizer.Get("Product_HeightRequired"));
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
                    return Json(new { error = "Not Authorized" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() });
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FreeSearch(FreeSearchListingsRequest request)
        {
            var amount = request.Amount ?? 25; // put this default value in settings somewhere
            var page = request.Page ?? 1;

            var listingService = new ListingService();
            var searchResults = listingService.FreeSearch(request.Q, amount, page);

            // order profiles so those with cover photos come first
            var orderedpProfiles = searchResults.ProfilesResults.Results.Where(x => !x.CoverPhotos.IsNullOrEmpty())
                .Concat(searchResults.ProfilesResults.Results.Where(x => x.CoverPhotos.IsNullOrEmpty())).ToList();

            var viewModel = new FreeSearchListingsViewModel {
                Amount = amount,
                Location = null,
                Page = page,
                Q = request.Q,
                TotalResults = searchResults.ListingsResults.Total,
                Results = searchResults.ListingsResults.Results.Select(x => x.ToListingView()).ToList(),
                RelatedProfessionals = orderedpProfiles,
                RelatedProducts = searchResults.ProductsResults.Results.Select(x => x.ToListingView()).ToList()
            };

            if (Request.IsAjaxRequest())
                return PartialView(string.Concat(ListingTypeName, "Grid"), new TListingGridViewModel { Results = viewModel.Results });

            return View(viewModel);
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
                Dictionary<string, string[]> searchMetadata = null;
                // add the filters from the url
                if (filters != null)
                {
                    var strings = filters.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (model.Metadata == null) model.Metadata = new TListingMetadata();
                    string tag;
                    LocationView location = null;
                    searchMetadata = model.Metadata.ParseSearchFilters(strings, out tag, ref location);
                    model.Tag = tag;
                    model.Location = location;
                }
                // search
                var results = service.SearchListings(
                    null,
                    string.IsNullOrEmpty(model.Tag) ? null : model.Tag.Split(' ', '-'),
                    new[] {model.Category},
                    new[] {ListingTypeName},
                    searchMetadata,
                    model.PriceMin,
                    model.PriceMax,
                    model.Location,
                    model.Page);
                model.Results = results.Results;
                model.Count = results.Count;
                model.PagingUrl = Url.RouteUrlForCurrentLocale("Search" + ListingTypeName);

                if (model.Metadata == null) model.Metadata = new TListingMetadata();

                if (Request.IsAjaxRequest())
                {
                    return PartialView(string.Concat(ListingTypeName, "Grid"), new TListingGridViewModel { Results = model.Results });
                }
                else
                {
                    return View(model);
                }
            }
            catch (ClassyException cex)
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
            return RedirectToRoute(string.Concat("Search", ListingTypeName), new { filters = slug });
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
                var listings = listingService.GetListingsByProfileId(profileId, includeDrafts, false);

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

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult TranslateListing(string listingId, string cultureCode)
        {
            var listingService = new ListingService();
            TranslateListingViewModel model = null;
            ListingTranslationView translation = null;

            if (cultureCode == null)
            {
                var listing = listingService.GetListingById(listingId, false, false, false, false, false);
                model = new TranslateListingViewModel { ListingId = listingId, CultureCode = listing.DefaultCulture, Title = listing.Title, Content = listing.Content };
            }
            else
            {
                translation = listingService.GetTranslation(listingId, cultureCode);
            }

            if (Request.Headers["Accept"].ToLower().Contains("text/html"))
            {
                return PartialView(model);
            }
            else
            {
                return Json(translation, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TranslateListing(TranslateListingViewModel model)
        {
            try
            {
                var listingService = new ListingService();
                if (string.IsNullOrEmpty(model.Action))
                {
                    listingService.SaveTranslation(model.ListingId, new ListingTranslationView
                    {
                        Culture = model.CultureCode,
                        Title = model.Title,
                        Content = model.Content
                    });
                    return Json(new { IsValid = true, SuccessMessage = Localizer.Get("EditListing_SaveTranslation_Success") });
                }
                else
                {
                    listingService.DeleteTranslation(model.ListingId, model.CultureCode);
                    return Json(new { IsValid = true, SuccessMessage = Localizer.Get("EditListing_DeleteTranslation_Success") });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetListingMoreInfo(ListingMoreInfoViewModel model)
        {
            var listingService = new ListingService();
            Dictionary<string, string[]> _metadata = null;
            Dictionary<string, string[]> _query = null;
            if (model.Metadata != null)
            {
                _metadata = model.Metadata.ToDictionary(p => p.Key, p => p.Value.Split(','));
            }
            if (model.Query != null)
            {
                _query = model.Query.ToDictionary(p => p.Key, p => p.Value.Split(','));
            }
            var info = listingService.GetLisingMoreInfo(model.ListingId, _metadata, _query);
            info.Metadata = _metadata;

            return PartialView("MoreInfo", info);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateListingNoCollection()
        {
            var model = new CreateListingNoCollectionViewModel<TListingMetadata>();
            model.PricingInfo = new PricingInfoView() { CurrencyCode = Request[Classy.DotNet.Responses.AppView.CurrencyCookieName] ?? Classy.DotNet.Responses.AppView.DefaultCurrency };
            model.PricingInfo.PurchaseOptions = new List<PurchaseOptionView>();
            //model.PricingInfo.PurchaseOptions.Add(new PurchaseOptionView { SKU = "123456", VariantProperties = new Dictionary<string, string> { { "Color", "Red" } } });
            return View(string.Format("Create{0}", ListingTypeName), model);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateListingNoCollection(CreateListingNoCollectionViewModel<TListingMetadata> model)
        {
            Dictionary<string, string> errors = new Dictionary<string,string>();
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                errors.Add("Title", Localizer.Get("CreateListing_TitleRequired"));
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                errors.Add("Content", Localizer.Get("CreateListing_ContentRequired"));
            }
            if (string.IsNullOrWhiteSpace(Request["Categories[]"]))
            {
                errors.Add("Categories", Localizer.Get("CreateListing_CategoryRequired"));
            }
            else
            {
                model.Categories = Request["Categories[]"].Split(',');
            }
            if (string.IsNullOrWhiteSpace(Request["Images[]"]))
            {
                errors.Add("Images", Localizer.Get("CreateListing_ImagesRequired"));
            }
            else
            {
                model.PricingInfo.BaseOption.MediaFiles = Request["Images[]"].Split(',').Select(key => new MediaFileView { Key = key }).ToArray();
            }
            if (model.PricingInfo != null)
            {
                // Assemble Variant dictionary
                if (model.PricingInfo.PurchaseOptions != null)
                {
                    for (int i = 0; i < model.PricingInfo.PurchaseOptions.Count; i++)
                    {
                        Dictionary<string, string> vprops = new Dictionary<string, string>();
                        var po = model.PricingInfo.PurchaseOptions[i];
                        if (po.Color != null && po.Color != "_") { vprops.Add("Color", po.Color); }
                        if (po.Design != null && po.Design != "_") { vprops.Add("Design", po.Design); }
                        if (po.Size != null && po.Size != "_") { vprops.Add("Size", po.Size); }
                        po.VariantProperties = vprops;
                    }
                }

                ValidatePricingInfo(null, model.PricingInfo, errors);
            }
            if (Request.IsAjaxRequest())
            {
                if (errors.Count > 0)
                {
                    return Json(new { errors = errors });
                }
                else
                {
                    // Save the lsiting
                    var listingService = new ListingService();
                    var listing = listingService.CreateListing(
                        model.Title,
                        model.Content,
                        ListingTypeName,
                        model.Categories,
                        model.PricingInfo,
                        (model.Metadata == null ? null : model.Metadata.ToDictionary()),
                        Request.Files);


                    return Json (new { listingId = listing.Id });
                }
            }
            else
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(string.Format("Create{0}", ListingTypeName), model);
            }
        }
    }
}
