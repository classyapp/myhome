﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Config;
using Classy.DotNet.Security;
using Classy.DotNet.Mvc.ViewModels.Profiles;
using Classy.DotNet.Services;
using System.Net;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.ActionFilters;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Net.Mail;
using System.Web;

namespace Classy.DotNet.Mvc.Controllers
{

    public class ProfileController<TProMetadata, TReviewSubCriteria, TUserMetadata, TVendorMetadata> : BaseController
        where TProMetadata : IMetadata<TProMetadata>, new()
        where TReviewSubCriteria : IReviewSubCriteria<TReviewSubCriteria>, new()
        where TUserMetadata : IMetadata<TUserMetadata>, new()
        where TVendorMetadata: IMetadata<TVendorMetadata>, new()
    {
        public ProfileController() : base() { }
        public ProfileController(string ns) : base(ns) { }

        public EventHandler<LoadPublicProfileEventArgs<TProMetadata>> OnLoadPublicProfile;
        public EventHandler<ContactProfessionalArgs<TProMetadata>> OnContactProfessional;
        public EventHandler<ParseProfilesCsvLineArgs<TProMetadata>> OnParseProfilesCsvLine;
        public EventHandler<AskForReviewArgs<TProMetadata>> OnAskForReview;

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRouteWithName(
                name: "CreateProxyProfile",
                url: "profile/new/proxy",
                defaults: new { controller = "Profile", action = "CreateProxyProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "CreateProxyProfileMass",
                url: "profile/new/proxy/mass",
                defaults: new { controller = "Profile", action = "CreateProxyProfileMass" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "VerifyProfileEmail",
                url: "profile/verify/{hash}",
                defaults: new { controller = "Profile", action = "VerifyProfileEmail" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "EditProfile",
                url: "profile/{ProfileId}/edit",
                defaults: new { controller = "Profile", action = "EditProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ChangeProfileImage",
                url: "profile/{ProfileId}/editimage",
                defaults: new { controller = "Profile", action = "ChangeProfileImage" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "AskForReview",
                url: "profile/askreview",
                defaults: new { controller = "Profile", action = "AskForReview" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ChangePassword",
                url: "profile/{ProfileId}/changepassword",
                defaults: new { controller = "Profile", action = "ChangePassword" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ClaimProxyProfile",
                url: "profile/{profileId}/claim",
                defaults: new { controller = "Profile", action = "ClaimProxyProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "TranslateProfile",
                url: "profile/{profileId}/translate/{cultureCode}",
                defaults: new { controller = "Profile", action = "Translate", cultureCode = UrlParameter.Optional },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "SendEmail",
                url: "profile/{profileId}/sendemail",
                defaults: new { controller = "Profile", action = "SendEmail" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "SearchProfiles",
                url: "profile/search/{*filters}",
                defaults: new { controller = "Profile", action = "Search", filters = "" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "CreateProfessionalProfile",
                url: "profile/gopro",
                defaults: new { controller = "Profile", action = "CreateProfessionalProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "CreateVendorProfile",
                url: "profile/govendor",
                defaults: new { controller = "Profile", action = "CreateVendorProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "UploadCatalog",
                url: "profile/newcatalog",
                defaults: new { controller = "Profile", action = "UploadCatalog" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ProfileJobs",
                url: "profile/jobs",
                defaults: new { controller = "Profile", action = "ProfileJobs" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ProfileJobErrors",
                url: "profile/job/{jobid}/errors",
                defaults: new { controller = "Profile", action = "ProfileJobErrors" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "FollowProfile",
                url: "profile/{username}/follow",
                defaults: new { controller = "Profile", action = "FollowProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "UnfollowProfile",
                url: "profile/{username}/unfollow",
                defaults: new { controller = "Profile", action = "UnfollowProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "ContactProfessional",
                url: "profile/{ProfessionalProfileId}/contact",
                defaults: new { controller = "Profile", action = "ContactProfessional" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "LoadFacebookAlbums",
                url: "profile/social/facebook/albums/{*albumId}",
                defaults: new { controller = "Profile", action = "LoadFacebookAlbums", albumId = UrlParameter.Optional },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "SelectCoverPhotos",
                url: "profile/{profileId}/photos",
                defaults: new { controller = "Profile", action = "SelectCoverPhotos" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRouteForSupportedLocales(
                name: "PublicProfile",
                url: "profile/{profileId}/{slug}",
                defaults: new { controller = "Profile", action = "PublicProfile", slug = "public" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "MobileAuthenticate",
                url: "mobile/authenticate",
                defaults: new { controller = "Profile", action = "MobileAuthenticate" },
                namespaces: new[] { Namespace }
            );
        }

        #region // actions

        //
        // GET: /profile/new/proxy
        //
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateProxyProfile()
        {
            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MobileAuthenticate()
        {
            var identity = User.Identity as ClassyIdentity;
            return Json(identity, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /profile/new/proxy/mass
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateProxyProfileMass(CreateProxyProfileMassViewModel<TProMetadata> model)
        {
            if (OnParseProfilesCsvLine == null) ModelState.AddModelError("OnParseCsvLine", Localizer.Get("CreateProxy_MassUploadNotImplemented"));
            if (!ModelState.IsValid) return View("CreateProxyProfile", model);

            // get file stream
            var reader = new StreamReader(model.File.InputStream);

            // setup event args
            var args = new ParseProfilesCsvLineArgs<TProMetadata>();
            args.IsHeaderLine = true;
            args.Metadata = new TProMetadata();

            // professional categories or validation
            var proCategories = Localizer.GetList("professional-categories");

            // loop file lines and call profile service to create proxies
            string line;
            var service = new ProfileService();
            var batchId = Guid.NewGuid().ToString();
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                TextFieldParser parser = new TextFieldParser(new StringReader(line));
                parser.SetDelimiters(",");
                args.LineValues = parser.ReadFields();
                args.LineCount = index;

                // let the implementation handle parsing
                OnParseProfilesCsvLine(this, args);

                // validate professional info
                if (!args.IsHeaderLine)
                {
                    if (args.ProfessionalInfo == null)
                    {
                        throw new ArgumentNullException(string.Format("args.ProfessionalInfo cannot return null from OnParseProfilesCsvLine. Line {0}", index));
                    }
                    else
                    {
                        if (!proCategories.Any(x => x.Value == args.ProfessionalInfo.Category))
                        {
                            throw new ArgumentOutOfRangeException(string.Format("Invalid category from OnParseProfileCsvLine. Value: {0}, Line: {1}", args.ProfessionalInfo.Category, index));
                        }
                    }
                }

                if (args.ProfessionalInfo != null) service.CreateProxyProfile(batchId, args.ProfessionalInfo, args.Metadata.ToDictionary());
                args.IsHeaderLine = false;
                index++;
            }

            TempData["UploadSuccess"] = Localizer.Get("CreateProxyMass_UploadSuccess");
            return RedirectToRoute("CreateProxyProfile");
        }

        //
        // GET: /profile/edit
        // 
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult EditProfile(string profileId)
        {
            var service = new ProfileService();
            var profile = profileId == AuthenticatedUserProfile.Id ? AuthenticatedUserProfile : service.GetProfileById(profileId);
            if (!profile.CanEdit()) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var contactInfo = profile.ContactInfo ?? new ExtendedContactInfoView();
            contactInfo.Location = contactInfo.Location ?? new LocationView();
            contactInfo.Location.Address = contactInfo.Location.Address ?? new PhysicalAddressView();
            var proContactInfo = profile.ProfessionalInfo ?? new ProfessionalInfoView();
            proContactInfo.CompanyContactInfo = proContactInfo.CompanyContactInfo ?? new ExtendedContactInfoView();
            proContactInfo.CompanyContactInfo.Location = proContactInfo.CompanyContactInfo.Location ?? new LocationView();
            proContactInfo.CompanyContactInfo.Location.Address = proContactInfo.CompanyContactInfo.Location.Address ?? new PhysicalAddressView();
            var proMetadata = profile.IsProfessional ? (profile.Metadata != null ? new TProMetadata().FromDictionary(profile.Metadata) : new TProMetadata()) : default(TProMetadata);
            var userMetadata = !profile.IsProfessional ? (profile.Metadata != null ? new TUserMetadata().FromDictionary(profile.Metadata) : new TUserMetadata()) : default(TUserMetadata);
            var model = new EditProfileViewModel<TProMetadata, TUserMetadata>
            {
                ProfileId = profile.Id,
                DefaultCulture = profile.DefaultCulture,
                AvatarUrl = profile.AvatarUrl(150, true).ToString(),
                FirstName = profile.IsProfessional ? proContactInfo.CompanyContactInfo.FirstName : contactInfo.FirstName,
                LastName = profile.IsProfessional ? proContactInfo.CompanyContactInfo.LastName : contactInfo.LastName,
                Street1 = profile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.Street1 : contactInfo.Location.Address.Street1,
                Street2 = profile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.Street2 : contactInfo.Location.Address.Street2,
                City = profile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.City : contactInfo.Location.Address.City,
                Country = profile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.Country : contactInfo.Location.Address.Country,
                PostalCode = profile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.PostalCode : contactInfo.Location.Address.PostalCode,
                Username = profile.UserName,
                Email = profile.IsProfessional ? proContactInfo.CompanyContactInfo.Email : contactInfo.Email,
                Phone = profile.IsProfessional ? proContactInfo.CompanyContactInfo.Phone : contactInfo.Phone,
                IsProfessional = profile.IsProfessional,
                Category = proContactInfo.Category,
                CompanyName = proContactInfo.CompanyName,
                WebsiteUrl = proContactInfo.CompanyContactInfo.WebsiteUrl,
                ProfessionalMetadata = proMetadata,
                UserMetadata = userMetadata
            };
            return View(model);
        }

        //
        // POST: /profile/edit
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult EditProfile(EditProfileViewModel<TProMetadata, TUserMetadata> model)
        {
            var fields = UpdateProfileFields.None;
            IDictionary<string, string> metadata = null;

            // validation
            if (model.IsProfessional)
            {
                fields |= UpdateProfileFields.ProfessionalInfo | UpdateProfileFields.Metadata;
                if (string.IsNullOrEmpty(model.CompanyName)) ModelState.AddModelError("CompanyName", Localizer.Get("EditProfile_CompanyName_Required"));
                if (string.IsNullOrEmpty(model.Country)) ModelState.AddModelError("Country", Localizer.Get("EditProfile_Country_Required"));

                if (model.ProfessionalMetadata != null) metadata = model.ProfessionalMetadata.ToDictionary();
            }
            else
            {
                fields |= UpdateProfileFields.ContactInfo;
                if (model.UserMetadata != null) metadata = model.UserMetadata.ToDictionary();
            }

            // update
            if (ModelState.IsValid)
            {
                var service = new ProfileService();
                service.UpdateProfile(
                    model.ProfileId,
                    new ExtendedContactInfoView
                    {
                        Phone = model.Phone,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Location = new LocationView
                        {
                            Address = new PhysicalAddressView
                            {
                                Street1 = model.Street1,
                                Street2 = model.Street2,
                                City = model.City,
                                Country = model.Country,
                                PostalCode = model.PostalCode
                            }
                        },
                        Email = model.Email,
                        WebsiteUrl = model.WebsiteUrl
                    },
                    new ProfessionalInfoView
                    {
                        CompanyName = model.CompanyName,
                        Category = model.Category,
                        CompanyContactInfo = new ExtendedContactInfoView
                        {
                            Phone = model.Phone,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Location = new LocationView
                            {
                                Address = new PhysicalAddressView
                                {
                                    Street1 = model.Street1,
                                    Street2 = model.Street2,
                                    City = model.City,
                                    Country = model.Country,
                                    PostalCode = model.PostalCode
                                }
                            },
                            Email = model.Email,
                            WebsiteUrl = model.WebsiteUrl
                        }
                    },
                    metadata != null ? metadata : null,
                    model.DefaultCulture,
                    null,
                    fields);
                TempData["EditProfile_Success"] = true;
                return Redirect(Url.RouteUrl("PublicProfile", new { ProfileId = model.ProfileId, Slug = AuthenticatedUserProfile.GetProfileName().ToSlug() }) + "?EditProfile_Success=true");
            }
            else return View(model);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeProfileImage(string profileId)
        {
            if (AuthenticatedUserProfile.Id != profileId && !AuthenticatedUserProfile.Permissions.Contains("admin")) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (Request.Files.Count == 1)
            {
                var service = new ProfileService();
                string key = service.UpdateProfile(
                    profileId,
                    Request.Files[0]);

                return Json(new { url = string.Format("//{0}/thumbnail/{1}?Width=150&format=json", System.Configuration.ConfigurationManager.AppSettings["Classy:CloudFrontDistributionUrl"], key) });
            }
            else
                throw new InvalidOperationException("Invalid number of images");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AskForReview()
        {
            try
            {
                AskForReviewModel model = new AskForReviewModel();
                if (Request.IsAuthenticated)
                {
                    model.ProfileId = AuthenticatedUserProfile.Id;
                    var service = new ProfileService();
                    var contacts = service.GetGoogleContacts();
                    model.IsGoogleConnected = AuthenticatedUserProfile.IsGoogleConnected;
                    model.GoogleContacts = contacts;
                    model.IsProfessional = AuthenticatedUserProfile.IsProfessional;
                }
                return View(model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AskForReview(AskForReviewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new ProfileService();
                AskForReviewArgs<TProMetadata> args = new AskForReviewArgs<TProMetadata>();
                args.Emails = model.Contacts;
                args.Message = model.Message;
                args.Profile = AuthenticatedUserProfile;
                args.ReviewLink = string.Concat(Request.Url.Scheme, "://", AppView.Hostname, Url.RouteUrl("PostProfileReview", new { profileId = AuthenticatedUserProfile.Id }), "?utm_source=ask_for_review&utm_medium=email");

                if (OnAskForReview != null)
                {
                    OnAskForReview(this, args);
                }
            }
            else
            {
                var service = new ProfileService();
                var contacts = service.GetGoogleContacts();
                model.IsGoogleConnected = (contacts == null);
                model.GoogleContacts = contacts;
                return View(model);
            }

            return RedirectToAction("PublicProfile", new { profileId = AuthenticatedUserProfile.Id });
        }

        //
        // GET: /profile/{ProfileId}/{Slug}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        [ImportModelStateFromTempData]
        public ActionResult PublicProfile(string profileId, string ead)
        {
            if (MobileRedirect.IsMobileDevice())
                return Redirect("~/Mobile/App.html#/Profile/" + profileId);

            try
            {
                var service = new ProfileService();
                var profile = service.GetProfileById(profileId, true, true, true, true, true, true);
                var metadata = new TProMetadata().FromDictionary(profile.Metadata);
                var subCriteria = new TReviewSubCriteria().FromDictionary(profile.ReviewAverageSubCriteria);
                var model = new PublicProfileViewModel<TProMetadata, TReviewSubCriteria>
                {
                    Profile = profile,
                    TypedMetadata = metadata,
                    ReviewSubCriteria = subCriteria,
                    EnableAdditionalContent = !string.IsNullOrEmpty(ead)
                };

                if (OnLoadPublicProfile != null)
                {
                    var args = new LoadPublicProfileEventArgs<TProMetadata>
                    {
                        Profile = profile,
                        TypedMetadata = metadata
                    };
                    OnLoadPublicProfile(this, args);

                    model.RelatedListings = args.RelatedListings;
                    model.RelatedProfiles = args.RelatedProfiles;
                }

                return View(profile.IsProxy ? "ProxyLandingPage" : "PublicProfile", model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // GET: /profile/{ProfileId}/claim
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ClaimProxyProfile(string profileId)
        {
            try
            {
                var profile = AuthenticatedUserProfile;
                if (profile != null && profile.IsProfessional) return RedirectToRoute("Home");
                var service = new ProfileService();
                var proxy = service.GetProfileById(profileId);
                if (!proxy.IsProxy) return RedirectToRoute("Home");

                var metadata = new TProMetadata().FromDictionary(profile != null ? profile.Metadata : proxy.Metadata);
                var model = new ClaimProfileViewModel<TProMetadata>
                {
                    ProfileId = profileId,
                    Email = proxy.ProfessionalInfo.CompanyContactInfo.Email,
                    Phone = proxy.ProfessionalInfo.CompanyContactInfo.Phone,
                    WebsiteUrl = proxy.ProfessionalInfo.CompanyContactInfo.WebsiteUrl,
                    FirstName = proxy.ProfessionalInfo.CompanyContactInfo.FirstName,
                    LastName = proxy.ProfessionalInfo.CompanyContactInfo.LastName,
                    Category = proxy.ProfessionalInfo.Category,
                    CompanyName = proxy.ProfessionalInfo.CompanyName,
                    Street1 = proxy.ProfessionalInfo.CompanyContactInfo.Location.Address.Street1,
                    City = proxy.ProfessionalInfo.CompanyContactInfo.Location.Address.City,
                    Country = proxy.ProfessionalInfo.CompanyContactInfo.Location.Address.Country,
                    PostalCode = proxy.ProfessionalInfo.CompanyContactInfo.Location.Address.PostalCode,
                    DefaultCulture = proxy.DefaultCulture,
                    Metadata = metadata
                };
                return View(model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // POST: /profile/{proxyId}/claim
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClaimProxyProfile(ClaimProfileViewModel<TProMetadata> model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                var professionalInfo = new ProfessionalInfoView
                {
                    CompanyName = model.CompanyName,
                    Category = model.Category,
                    CompanyContactInfo = new ExtendedContactInfoView
                    {
                        Email = model.Email,
                        Phone = model.Phone,
                        WebsiteUrl = model.WebsiteUrl,
                        Location = new LocationView
                        {
                            Address = new PhysicalAddressView
                            {
                                Street1 = model.Street1,
                                City = model.City,
                                Country = model.Country,
                                PostalCode = model.PostalCode
                            }
                        },
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    }
                };

                var service = new ProfileService();
                var claim = service.ClaimProfileProxy(
                    model.ProfileId,
                    professionalInfo,
                    model.Metadata.ToDictionary(),
                    model.DefaultCulture);
                service.ApproveProxyClaim(claim.Id);

                return RedirectToRoute("PublicProfile", new { ProfileId = AuthenticatedUserProfile.Id });
            }
            catch (ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                    return View(model);
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }
        }

        // 
        // GET: /profile/search
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Search(SearchProfileViewModel<TProMetadata> model, string filters)
        {
            try
            {
                var location = new LocationView();
                if (model.CountryCode == null) // First request
                {
                    // Get data from cookies
                    var gpsCookie = System.Web.HttpContext.Current.Request.Cookies[AppView.GPSLocationCookieName];
                    if (gpsCookie != null && !string.IsNullOrEmpty(gpsCookie.Value))
                    {
                        var coords = Newtonsoft.Json.JsonConvert.DeserializeObject<GPSLocation>(gpsCookie.Value);
                        location.Coords = new CoordsView { Latitude = coords.Latitude, Longitude = coords.Longitude };
                        model.Country = string.Empty;
                        model.CountryCode = "current-location";
                    }
                    System.Web.HttpCookie countryCookie = System.Web.HttpContext.Current.Request.Cookies[Classy.DotNet.Responses.AppView.CountryCookieName];
                    if (countryCookie != null)
                    {
                        location.Address = new PhysicalAddressView { Country = countryCookie.Value };
                        model.CountryCode = countryCookie.Value;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model.CountryCode)) model.CountryCode = Request.Cookies[AppView.CountryCookieName].Value;

                    if (model.Country == "current-location")
                    {
                        // first search, when defaulted to current-location doesn't send coordinates via querystring
                        if (model.Longitude.HasValue)
                        {
                            location.Coords = new CoordsView { Longitude = model.Longitude.Value, Latitude = model.Latitude.Value };
                        }
                        else
                        {
                            return View(model);
                        }
                    }
                    else
                    {
                        location.Address = new PhysicalAddressView { Country = model.CountryCode, City = model.City };
                    }
                }
                var service = new ProfileService();
                var resutls = service.SearchProfiles(
                    model.Name,
                    model.Category,
                    location,
                    model.Metadata != null ? model.Metadata.ToDictionary() : null,
                    true,
                    false,
                    model.Page);

                model.Results = resutls.Results;
                model.Count = resutls.Count;

                if (Request.IsAjaxRequest())
                {
                    if (model.Format == "json")
                        return Json(model.Results, JsonRequestBehavior.AllowGet);
                    else
                        return PartialView("ProfileGrid", model.Results);
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
        // POST: /profile/search
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Search(SearchProfileViewModel<TProMetadata> model, object dummyforpost)
        {
            return RedirectToRoute("SearchProfiles", new { filters = model.ToSlug() });
        }

        // 
        // GET: /profile/me/gopro
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateProfessionalProfile(string referrerUrl)
        {
            try
            {
                var service = new ProfileService();
                var profile = service.GetProfileById(AuthenticatedUserProfile.Id);
                var metadata = new TProMetadata().FromDictionary(profile.Metadata);
                var model = new CreateProfessionalProfileViewModel<TProMetadata>
                {
                    ProfileId = profile.Id,
                    Email = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Email : null,
                    Phone = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Phone : null,
                    WebsiteUrl = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.WebsiteUrl : null,
                    FirstName = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.FirstName : null,
                    LastName = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.LastName : null,
                    Category = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.Category : null,
                    CompanyName = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyName : null,
                    Street1 = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.Street1 : null,
                    City = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.City : null,
                    Country = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.Country : null,
                    PostalCode = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.PostalCode : null,
                    DefaultCulture = profile.DefaultCulture,
                    Metadata = metadata,
                    ReferrerUrl = referrerUrl
                };
                return View(model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        // 
        // POST: /profile/me/gopro
        [AuthorizeWithRedirect("Login")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateProfessionalProfile(CreateProfessionalProfileViewModel<TProMetadata> model)
        {
            if (!ModelState.IsValid) return View(model);

            var professionalInfo = new ProfessionalInfoView
            {
                CompanyName = model.CompanyName,
                CompanyContactInfo = new ExtendedContactInfoView
                {
                    Email = model.Email,
                    Phone = model.Phone,
                    WebsiteUrl = model.WebsiteUrl,
                    Location = new LocationView
                    {
                        Address = new PhysicalAddressView
                        {
                            Street1 = model.Street1,
                            City = model.City,
                            Country = model.Country,
                            PostalCode = model.PostalCode
                        }
                    },
                    FirstName = model.FirstName,
                    LastName = model.LastName
                },
                Category = model.Category
            };
            try
            {
                var service = new ProfileService();
                var profile = service.UpdateProfile(
                    AuthenticatedUserProfile.Id,
                    null,
                    professionalInfo,
                    model.Metadata.ToDictionary(),
                    model.DefaultCulture,
                    null,
                    UpdateProfileFields.ProfessionalInfo | UpdateProfileFields.Metadata);

                if (!string.IsNullOrEmpty(model.ReferrerUrl)) 
                {
                    var returnUrl = string.IsNullOrEmpty(model.ReferrerUrl) ? "~/" : Uri.UnescapeDataString(model.ReferrerUrl);
                    return Redirect(returnUrl);
                }
                return RedirectToRoute("PublicProfile", new { ProfileId = AuthenticatedUserProfile.Id });
            }
            catch (ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                    return View(model);
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }
        }

        //
        // GET: /profile/{ProfessionalProfileId}/contact
        [AcceptVerbs((HttpVerbs.Get))]
        public ActionResult ContactProfessional(string professionalProfileId)
        {
            try
            {
                var service = new ProfileService();
                var profile = service.GetProfileById(professionalProfileId);
                var model = new ContactProfessionalViewModel
                {
                    ProfessionalProfileId = professionalProfileId,
                    ProfessionalName = profile.GetProfileName()
                };
                return PartialView("ContactProfessional", model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // POST: /profile/{ProfessionalProfileId}/contact
        [AcceptVerbs((HttpVerbs.Post))]
        public ActionResult ContactProfessional(ContactProfessionalViewModel model, object dummy)
        {
            try
            {
                // when user is not logged-in, ReplyToEmail is required
                if (!Request.IsAuthenticated && string.IsNullOrEmpty(model.ReplyToEmail))
                {
                    ModelState.AddModelError("ReplyToEmail", Localizer.Get("ContactPro_ReplyToEmail_Required"));
                }

                if (ModelState.IsValid)
                {
                    var service = new ProfileService();
                    var profile = service.GetProfileById(model.ProfessionalProfileId);

                    var metadata = new TProMetadata();
                    metadata.FromDictionary(profile.Metadata);
                    var args = new ContactProfessionalArgs<TProMetadata>
                    {
                        ReplyToEmail = (Request.IsAuthenticated) ? (User.Identity as ClassyIdentity).Profile.ContactInfo.Email : model.ReplyToEmail,
                        Subject = model.Subject,
                        Content = model.Content,
                        ProfessionalProfile = profile,
                        TypedMetadata = metadata
                    };

                    if (OnContactProfessional != null)
                        OnContactProfessional(this, args);

                    //TODO: this doesn't belong in the frontend 
                    var analytics = new AnalyticsService();
                    analytics.LogActivity(Request.IsAuthenticated ? (User.Identity as ClassyIdentity).Profile.Id : "guest", "contact-profile", model.ProfessionalProfileId);

                    return Json(new { IsValid = true });
                }
                else return PartialView(model);
            }
            catch (ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }

            return PartialView("ContactProfessional", model);
        }

        //
        // POST: /profile/{username}/follow
        //
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FollowProfile(string username)
        {
            try
            {
                var service = new ProfileService();
                service.FollowProfile(username);
            }
            catch (ClassyException cvx)
            {
                return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        //
        // POST: /profile/{username}/unfollow
        //
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnfollowProfile(string username)
        {
            try
            {
                var service = new ProfileService();
                service.UnfollowProfile(username);
            }
            catch (ClassyException cvx)
            {
                return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LoadFacebookAlbums(string albumId)
        {
            try
            {
                var service = new ProfileService();
                var albums = service.GetFacebookAlbums();
                if (string.IsNullOrEmpty(albumId))
                {
                    var model = new LoadFacebookAlbumsViewModel
                    {
                        Albums = albums.Where(x => x.Photos != null).ToList()
                    };
                    return PartialView(model);
                }
                else
                {
                    return Json(new { Album = albums.SingleOrDefault(x => x.Id == albumId) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ChangePassword(string profileId)
        {
            if (AuthenticatedUserProfile.Id != profileId && !AuthenticatedUserProfile.Permissions.Contains("admin")) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            var model = new ChangePasswordViewModel { ProfileId = profileId };
            return PartialView(model);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = new ProfileService();
                    service.ChangePassword(model.NewPassword, model.ProfileId);
                    return Json(new { IsValid = true });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else return PartialView(model);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SelectCoverPhotos(string profileId)
        {
            try
            {
                var profileService = new ProfileService();
                var profile = profileService.GetProfileById(profileId, false, true, false, false, false, false);

                var listingService = new ListingService();
                bool includeDrafts = (Request.IsAuthenticated && profileId == AuthenticatedUserProfile.Id);
                var listings = listingService.GetListingsByProfileId(profileId, includeDrafts, false);

                return PartialView(listings);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelectCoverPhotos(string profileId, string[] keys)
        {
            try
            {
                var profileService = new ProfileService();
                profileService.UpdateProfile(profileId, null, null, null, null, keys, UpdateProfileFields.CoverPhotos);

                return Json(new { url = Url.RouteUrl("PublicProfile", new { profileId = profileId }) });
            }
            catch (ClassyException cex)
            {
                return Json(new { error = Localizer.Get("SelectCoverPhotos_Error") });
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Translate(string profileId, string cultureCode)
        {
            var profileService = new ProfileService();
            TranslateProfileViewModel<TProMetadata> model = null;
            ProfileTranslationView translation = null;

            if (cultureCode == null)
            {
                var profile = profileService.GetProfileById(profileId);
                model = new TranslateProfileViewModel<TProMetadata>
                {
                    ProfileId = profileId,
                    CultureCode = profile.DefaultCulture,
                    CompanyName = profile.ProfessionalInfo.CompanyName,
                    Metadata = (new TProMetadata()).FromDictionary(profile.Metadata)
                };
            }
            else
            {
                translation = profileService.GetTranslation(profileId, cultureCode);
            }

            if (!Request.IsAjaxRequest())
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
        public ActionResult Translate(TranslateProfileViewModel<TProMetadata> model)
        {
            try
            {
                var profileService = new ProfileService();
                var metadata = (Dictionary<string, string>)model.Metadata.ToTranslationsDictionary();

                if ((model.CompanyName == null || string.IsNullOrEmpty(model.CompanyName.Trim())) &&
                    metadata.Count == 0)
                {
                    profileService.DeleteTranslation(model.ProfileId, model.CultureCode);
                    return Json(new { IsValid = true, SuccessMessage = Localizer.Get("EditProfile_DeleteTranslation_Success") });
                }
                else
                {
                    profileService.SaveTranslation(model.ProfileId, new ProfileTranslationView
                    {
                        CultureCode = model.CultureCode,
                        CompanyName = model.CompanyName,
                        Metadata = metadata
                    });
                    return Json(new { IsValid = true, SuccessMessage = Localizer.Get("EditProfile_SaveTranslation_Success") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = Localizer.Get("EditProfile_SaveTranslation_Failed") });
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SendEmail(string profileId)
        {
            return PartialView(new SendEmailViewModel { ProfileId = profileId });
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendEmail(SendEmailViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check email addresses
                    List<MailAddress> emails = new List<MailAddress>();
                    foreach (var address in model.Reciepients)
                    {
                        try
                        {
                            emails.Add(new MailAddress(address));
                        }
                        catch
                        {
                            ModelState.AddModelError("Contacts", Localizer.Get("SendEmail_InvalidEmail"));
                            return PartialView(model);
                        }
                    }

                    var profileService = new ProfileService();
                    profileService.SendEmail(emails.ToArray(), model.Subject, model.Body.Replace("\r\n", "<br/>"));

                    return Json(new { IsValid = true, SuccessMessage = Localizer.Get("SendEmail_Success") });
                }
                return Json(new { IsValid = false, ErrorMessage = Localizer.Get("SendEmail_Failure") });
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }

        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult VerifyProfileEmail(string hash)
        {
            var profileService = new ProfileService();
            var response = profileService.VerifyEmail(hash);
           
            return Redirect(Url.RouteUrl("PublicProfile", new { profileId = AuthenticatedUserProfile.Id }) + "?EmailVerified=" + response.Verified.ToString());
        }

        // 
        // GET: /profile/me/gopro
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateVendorProfile()
        {
            try
            {
                var service = new ProfileService();
                var profile = service.GetProfileById(AuthenticatedUserProfile.Id);
                var metadata = new TVendorMetadata().FromDictionary(profile.Metadata);
                var model = new CreateVendorProfileViewModel<TVendorMetadata>
                {
                    ProfileId = profile.Id,
                    Email = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Email : profile.ContactInfo.Email,
                    Phone = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Phone : null,
                    WebsiteUrl = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.WebsiteUrl : null,
                    Category = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.Category : null,
                    CompanyName = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyName : null,
                    Street1 = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.Street1 : null,
                    City = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.City : null,
                    Country = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.Country : null,
                    PostalCode = profile.ProfessionalInfo != null ? profile.ProfessionalInfo.CompanyContactInfo.Location.Address.PostalCode : null,
                    DefaultCulture = profile.DefaultCulture,
                    Metadata = metadata
                };
                return View(model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        // 
        // POST: /profile/me/gopro
        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateVendorProfile(CreateVendorProfileViewModel<TVendorMetadata> model)
        {
            if (!ModelState.IsValid) return View(model);

            var professionalInfo = new ProfessionalInfoView
            {
                CompanyName = model.CompanyName,
                CompanyContactInfo = new ExtendedContactInfoView
                {
                    Email = model.Email,
                    Phone = model.Phone,
                    WebsiteUrl = model.WebsiteUrl,
                    Location = new LocationView
                    {
                        Address = new PhysicalAddressView
                        {
                            Street1 = model.Street1,
                            City = model.City,
                            Country = model.Country,
                            PostalCode = model.PostalCode
                        }
                    }
                },
                Category = model.Category,
                PaymentDetails = new BankAccountView()
            };
            try
            {
                var service = new ProfileService();
                var profile = service.UpdateProfile(
                    AuthenticatedUserProfile.Id,
                    null,
                    professionalInfo,
                    model.Metadata.ToDictionary(),
                    model.DefaultCulture,
                    null,
                    UpdateProfileFields.ProfessionalInfo | UpdateProfileFields.Metadata);

                return RedirectToRoute("PublicProfile", new { ProfileId = AuthenticatedUserProfile.Id });
            }
            catch (ClassyException cvx)
            {
                if (cvx.IsValidationError())
                {
                    AddModelErrors(cvx);
                    return View(model);
                }
                else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
            }
        }

        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult UploadCatalog()
        {
            return View();
        }

        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadCatalog(UploadProductCatalogViewModel model,  HttpPostedFileBase catalog)
        {
            if (catalog == null)
            {
                ModelState.AddModelError(string.Empty, Localizer.Get("Uploadcatalog_FileRequired"));
                return View(model);
            }
            else
            {
                var profileService = new ProfileService();
                profileService.UploadCatalog(AuthenticatedUserProfile.Id, (int)model.CatalogTemplateType, model.OverwriteListings, model.UpdateImages, catalog);

                return Redirect(Url.RouteUrl("ProfileJobs"));
            }
        }

        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ProfileJobs()
        {
            JobService service = new JobService();
            IList<JobView> jobs = service.GetJobsStatus(AuthenticatedUserProfile.Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ProfileJobsRows", jobs);
            }
            else
            {
                return View(jobs);
            }
        }

        [AuthorizeWithRedirect("Home")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ProfileJobErrors(string jobid)
        {
            JobService service = new JobService();
            string errors = service.GetJobErrors(jobid);
            return File(Encoding.UTF8.GetBytes(errors), "text/csv", string.Format("errors_{0}.csv", jobid));
        }
        #endregion
    }
}
