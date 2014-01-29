using System;
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

namespace Classy.DotNet.Mvc.Controllers
{

    public class ProfileController<TProMetadata, TReviewSubCriteria> : BaseController
        where TProMetadata : IMetadata<TProMetadata>, new()
        where TReviewSubCriteria : IReviewSubCriteria<TReviewSubCriteria>, new()
    {
        public ProfileController() : base() { }
        public ProfileController(string ns) : base(ns) { }

        public EventHandler<ContactProfessionalArgs<TProMetadata>> OnContactProfessional; 

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRouteForSupportedLocales(
                name: "MyProfile",
                url: "profile/me",
                defaults: new { controller = "Profile", action = "MyProfile" },
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
                url: "profile/me/gopro",
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
        // GET: /profile/me
        // 
        [AuthorizeWithRedirect("Index")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MyProfile()
        {
            return View(AuthenticatedUserProfile);
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
                var profile = service.GetProfileById(profileId, true, true, true, true, true);
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
                if (profile.IsProfessional) return RedirectToRoute("Home");
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

                return RedirectToRoute("MyProfile");
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

                var profiles = service.SearchProfiles(
                    model.Name, 
                    model.Category, 
                    model.Location,
                    model.Metadata != null ? model.Metadata.ToDictionary() : null, 
                    true);
                if (Request.IsAjaxRequest())
                {
                    return Json(profiles, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.Results = profiles;
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
                    professionalInfo, 
                    model.Metadata.ToDictionary(), 
                    "CreateProfessionalProfile");
                
                return RedirectToRoute("MyProfile");
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
