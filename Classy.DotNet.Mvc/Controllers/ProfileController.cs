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
                url: "profile/search/{location}/{category}",
                defaults: new { controller = "Profile", action = "Search", category = "", location = "city" },
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
        [AuthorizeWithRedirect("Index")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ClaimProxyProfile(string profileId)
        {
            try
            {
                var profile = AuthenticatedUserProfile;
                var metadata = new TProMetadata().FromDictionary(profile.Metadata);
                var model = new ClaimProfileViewModel<TProMetadata>
                {
                    ProfileId = profileId,
                    ProfessionalInfo = profile.ProfessionalInfo,
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

                var service = new ProfileService();
                var claim = service.ClaimProfileProxy(
                    model.ProfileId,
                    model.ProfessionalInfo,
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
        public ActionResult Search(SearchViewModel<TProMetadata> model)
        {
            try
            {
                var service = new ProfileService();
                var metadata = model.Metadata != null ? model.Metadata.ToDictionary() : null;
                // we pass in a string location to be able to set it via URLs (for SEO)
                LocationView location = null;
                if (model.Location != "city")
                {
                    location = new LocationView
                    {
                        // TODO: get long/lat by city name, or pass city name and get long/lat on server?
                    };
                }
                else model.Location = "";

                var profiles = service.SearchProfiles(model.Name, model.Category, location, metadata, model.ProfessionalsOnly);
                if (Request.AcceptTypes.Contains("application/json"))
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
            return Search(model);
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
                    ProfessionalInfo = profile.ProfessionalInfo,
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

            try
            {
                var service = new ProfileService();
                var profile = service.UpdateProfile(
                    AuthenticatedUserProfile.Id, 
                    model.ProfessionalInfo, 
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
        // POST: /profile/{ProfessionalProfileId}/contact
        [AcceptVerbs((HttpVerbs.Post))]
        [ExportModelStateToTempData]
        public ActionResult ContactProfessional(ContactProfessionalViewModel model)
        {
            try
            {
                var service = new ProfileService();
                var profile = service.GetProfileById(model.ProfessionalProfileId);

                // when user is not logged-in, ReplyToEmail is required
                if (!Request.IsAuthenticated && string.IsNullOrEmpty(model.ReplyToEmail))
                {
                    ModelState.AddModelError("ReplyToEmail", "Please enter reply-to email");
                }

                if (ModelState.IsValid)
                {
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

                    TempData["ContactSuccess"] = "ההודעה נשלחה. בעל המקצוע יצור עמך קשר בכתובת האימייל שהזנת בטופס הפניה";
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

            return RedirectToRoute("PublicProfile", new { profileId = model.ProfessionalProfileId });
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
