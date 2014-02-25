﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Security;
using Classy.DotNet.Mvc.ViewModels.Profiles;
using Classy.DotNet.Services;
using System.Net;
using ServiceStack.Text;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.ActionFilters;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Classy.DotNet.Mvc.Controllers
{

    public class ProfileController<TProMetadata, TReviewSubCriteria, TUserMetadata> : BaseController
        where TProMetadata : IMetadata<TProMetadata>, new()
        where TReviewSubCriteria : IReviewSubCriteria<TReviewSubCriteria>, new()
        where TUserMetadata : IMetadata<TUserMetadata>, new ()
    {
        public ProfileController() : base() { }
        public ProfileController(string ns) : base(ns) { }

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

            routes.MapRouteForSupportedLocales(
                name: "EditProfile",
                url: "profile/edit",
                defaults: new { controller = "Profile", action = "EditProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "AskForReview",
                url: "profile/askreview",
                defaults: new { controller = "Profile", action = "AskForReview" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ClaimProxyProfile",
                url: "profile/{profileId}/claim",
                defaults: new { controller = "Profile", action = "ClaimProxyProfile" },
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

            routes.MapRoute(
                name: "FollowProfile",
                url: "profile/{username}/follow",
                defaults: new { controller = "Profile", action = "FollowProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "ContactProfessional",
                url: "profile/{ProfessionalProfileId}/contact",
                defaults: new { controller = "Profile", action = "ContactProfessional" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "PublicProfile",
                url: "profile/{profileId}/{slug}",
                defaults: new { controller = "Profile", action = "PublicProfile", slug = "public" },
                namespaces: new string[] { Namespace }
            );
        }

        #region // actions

        //
        // GET: /profile/new/proxy
        //
        [AuthorizeWithRedirect("Index")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateProxyProfile()
        {
            return View();
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
        [AuthorizeWithRedirect("Index")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult EditProfile()
        {
            var contactInfo = AuthenticatedUserProfile.ContactInfo ?? new ExtendedContactInfoView();
            contactInfo.Location = contactInfo.Location ?? new LocationView();
            contactInfo.Location.Address = contactInfo.Location.Address ?? new PhysicalAddressView();
            var proContactInfo = AuthenticatedUserProfile.ProfessionalInfo ?? new ProfessionalInfoView();
            proContactInfo.CompanyContactInfo = proContactInfo.CompanyContactInfo ?? new ExtendedContactInfoView();
            proContactInfo.CompanyContactInfo.Location = proContactInfo.CompanyContactInfo.Location ?? new LocationView();
            proContactInfo.CompanyContactInfo.Location.Address = proContactInfo.CompanyContactInfo.Location.Address ?? new PhysicalAddressView();
            var proMetadata = AuthenticatedUserProfile.IsProfessional ? (AuthenticatedUserProfile.Metadata != null ? new TProMetadata().FromDictionary(AuthenticatedUserProfile.Metadata) : new TProMetadata()) : default(TProMetadata);
            var userMetadata = !AuthenticatedUserProfile.IsProfessional ? (AuthenticatedUserProfile.Metadata != null ? new TUserMetadata().FromDictionary(AuthenticatedUserProfile.Metadata) : new TUserMetadata()) : default(TUserMetadata);

            var model = new EditProfileViewModel<TProMetadata, TUserMetadata>
            {
                ProfileId = AuthenticatedUserProfile.Id,
                FirstName = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.FirstName : contactInfo.FirstName,
                LastName = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.LastName : contactInfo.LastName,
                Street1 = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.Street1 : contactInfo.Location.Address.Street1,
                Street2 = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.Street2 : contactInfo.Location.Address.Street2,
                City = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.City : contactInfo.Location.Address.City,
                Country = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.Country : contactInfo.Location.Address.Country,
                PostalCode = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.Location.Address.PostalCode : contactInfo.Location.Address.PostalCode,
                ThumbnailUrl = AuthenticatedUserProfile.ThumbnailUrl,
                Username = AuthenticatedUserProfile.UserName,
                Email = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.Email : contactInfo.Email,
                Phone = AuthenticatedUserProfile.IsProfessional ? proContactInfo.CompanyContactInfo.Phone : contactInfo.Phone,
                IsProfessional = AuthenticatedUserProfile.IsProfessional,
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
        public ActionResult EditProfile(EditProfileViewModel<TProMetadata, TUserMetadata> model)
        {
            var fields = UpdateProfileFields.None;
            dynamic metadata;

            // validation
            if (model.IsProfessional)
            {
                fields |= UpdateProfileFields.ProfessionalInfo | UpdateProfileFields.Metadata;
                if (string.IsNullOrEmpty(model.CompanyName)) ModelState.AddModelError("CompanyName", Localizer.Get("EditProfile_CompanyName_Required"));
                if (string.IsNullOrEmpty(model.Category)) ModelState.AddModelError("Category", Localizer.Get("EditProfile_Category_Required"));
                if (string.IsNullOrEmpty(model.Phone)) ModelState.AddModelError("Phone", Localizer.Get("EditProfile_Phone_Required"));
                if (string.IsNullOrEmpty(model.Street1)) ModelState.AddModelError("Street1", Localizer.Get("EditProfile_Street1_Required"));
                if (string.IsNullOrEmpty(model.City)) ModelState.AddModelError("City", Localizer.Get("EditProfile_City_Required"));
                if (string.IsNullOrEmpty(model.Country)) ModelState.AddModelError("Country", Localizer.Get("EditProfile_Country_Required"));
                if (string.IsNullOrEmpty(model.PostalCode)) ModelState.AddModelError("PostalCode", Localizer.Get("EditProfile_PostalCode_Required"));
                if (string.IsNullOrEmpty(model.FirstName)) ModelState.AddModelError("FirstName", Localizer.Get("EditProfile_FirstName_Required"));
                if (string.IsNullOrEmpty(model.LastName)) ModelState.AddModelError("LastName", Localizer.Get("EditProfile_PostalCode_Required"));
                metadata = model.ProfessionalMetadata;
            }
            else
            {
                fields |= UpdateProfileFields.ContactInfo;
                metadata = model.UserMetadata;
            }

            // update
            if (ModelState.IsValid)
            {
                var service = new ProfileService();
                service.UpdateProfile(
                    AuthenticatedUserProfile.Id,
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
                        Email = model.Email
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
                            Email = model.Email
                        }
                    },
                    metadata != null ? metadata.ToDictionary() : null,
                    fields);
                TempData["EditProfile_Success"] = true;
                return RedirectToRoute("PublicProfile", new { ProfileId = model.ProfileId, Slug = AuthenticatedUserProfile.GetProfileName().ToSlug() });
            }
            else return View(model);
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AskForReview()
        {
            AskForReviewModel model = new AskForReviewModel();
            model.ProfileId = AuthenticatedUserProfile.Id;
            var service = new ProfileService();
            var contacts = service.GetGoogleContacts();
            model.NeedAuthentication = (contacts == null);
            model.GoogleContacts = contacts;
            return View(model);
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
                args.ReviewLink = Url.RouteUrl("PostProfileReview", new { profileId = AuthenticatedUserProfile.Id }, Request.Url.Scheme);

                if (OnAskForReview != null)
                {
                    OnAskForReview(this, args);
                }
            }
            else
            {
                var service = new ProfileService();
                var contacts = service.GetGoogleContacts();
                model.NeedAuthentication = (contacts == null);
                model.GoogleContacts = contacts;
                return View(model);
            }

            return RedirectToAction("PublicProfile", new { profileId = AuthenticatedUserProfile.Id});
        }

        //
        // GET: /profile/{ProfileId}/{Slug}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        [ImportModelStateFromTempData]
        public ActionResult PublicProfile(string profileId)
        {
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
                    ReviewSubCriteria = subCriteria
                };

                return View(profile.IsProfessional ? "PublicProfessionalProfile" : "PublicProfile", model);
            }
            catch(ClassyException cex)
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
                    Metadata = metadata
                };
                return View(model);
            }
            catch(ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // POST: /profile/{proxyId}/claim
        [AuthorizeWithRedirect("Index")]
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
                    model.Metadata.ToDictionary());
                service.ApproveProxyClaim(claim.Id);

                return RedirectToRoute("PublicProfile", new { ProfileId = model.ProfileId });
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
        public ActionResult Search(SearchViewModel<TProMetadata> model, string filters)
        {
            try
            {
                var service = new ProfileService();
                // add the filters from the url
                if (filters != null)
                {
                    var strings = filters.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (model.Metadata == null) model.Metadata = new TProMetadata();
                    string name;
                    LocationView location = null;
                    model.Metadata.ParseSearchFilters(strings, out name, ref location);
                }

                var resutls = service.SearchProfiles(
                    model.Name, 
                    model.Category, 
                    model.Location,
                    model.Metadata != null ? model.Metadata.ToDictionary() : null, 
                    true,
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
        public ActionResult Search(SearchViewModel<TProMetadata> model, object dummyforpost)
        {
            if (model.Metadata == null) model.Metadata = new TProMetadata();
            var slug = model.Metadata.GetSearchFilterSlug(model.Name, model.Location);
            return RedirectToRoute("SearchProfiles", new { filters = slug });
        }

        // 
        // GET: /profile/me/gopro
        [AuthorizeWithRedirect("Index")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateProfessionalProfile()
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
        [AuthorizeWithRedirect("Index")]
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
        //[ExportModelStateToTempData]
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

                    var analytics = new AnalyticsService();
                    //TODO: this doesn't belong in the frontend 
                    analytics.LogActivity(Request.IsAuthenticated ? (User.Identity as ClassyIdentity).Profile.Id : "guest", "contact-profile", model.ProfessionalProfileId);

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
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
        [AuthorizeWithRedirect("Index")]
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

        #endregion
    }
}
