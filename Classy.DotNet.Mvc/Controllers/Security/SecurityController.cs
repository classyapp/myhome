using Classy.DotNet.Security;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Mvc.ViewModels.Security;
using Classy.DotNet.Services;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.Controllers.Security
{
    public class SecurityController<TMetadata> : BaseController
        where TMetadata : IMetadata<TMetadata>, new()
    {
        public event EventHandler<ProfileView> OnProfileRegistered;

        public SecurityController() : base() { }
        public SecurityController(string ns) : base(ns) { }

        private readonly string USER_EXTENDED_METADATA_KEY = "UserExtendedProfile";

        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Security", action = "Logout" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "Login",
                url: "login",
                defaults: new { controller = "Security", action = "Login" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRouteWithName(
                name: "ResetPassword",
                url: "reset/{resetHash}",
                defaults: new { controller = "Security", action = "ResetPassword" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRouteForSupportedLocales(
                name: "ForgotPassword",
                url: "forgot",
                defaults: new { controller = "Security", action = "ForgotPassword" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRoute(
                name: "AuthenticateFacebookUser",
                url: "login/fb",
                defaults: new { controller = "Security", action = "AuthenticateWithFacebook" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRoute(
                name: "AuthenticateGoogleUser",
                url: "login/google",
                defaults: new { controller = "Security", action = "AuthenticateWithGoogle" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRouteWithName(
                name: "Register",
                url: "register",
                defaults: new { controller = "Security", action = "Register" },
                namespaces: new string[] { Namespace });

            routes.MapRouteWithName(
                name: "CompleteRegistration",
                url: "register/more",
                defaults: new { controller = "Security", action = "CompleteRegistration" },
                namespaces: new string[] { Namespace });

            routes.MapRouteWithName(
                name: "ResendEmailVerification",
                url: "profile/verifyemail/send",
                defaults: new { controller = "Security", action = "ResendEmailVerification" },
                namespaces: new string[] { Namespace });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Login()
        {
            var model = new LoginViewModel
            {
                RedirectUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "~/"
            };

            if (Request.IsAjaxRequest())
            {
                return PartialView("LoginModal", model);
            }
            else
            {
                return View(model);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                var isValid = ClassyAuth.AuthenticateUser(model.Email, model.Password, model.RememberMe);
                if (!isValid)
                {
                    ModelState.AddModelError("Invalid", Localization.Localizer.Get("Login_InvalidCredentials"));
                    return View(model);
                }
                else
                {
                    var returnUrl = string.IsNullOrEmpty(model.RedirectUrl) ? "~/" : Uri.UnescapeDataString(model.RedirectUrl);
                    return Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToRoute("Index");

            TempData["ForgotPassword_RequestSuccess"] = ClassyAuth.RequestPasswordReset(model.Email);
            
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ResetPassword(string resetHash)
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            
            // verify password request
            if (ClassyAuth.VerifyResetRequest(resetHash))
            {
                return View(new ResetPasswordViewModel { Hash = resetHash });
            }
            else
            {
                return Redirect("/");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            if (string.IsNullOrEmpty(model.Hash))
            {
                TempData["ResetPassword_Error"] = Localizer.Get("ResetPassword_InvalidUrl");
            }
            else
            {
                // get user auth by hash
                if (ClassyAuth.ResetPassword(model.Hash, model.Password))
                {
                    return Redirect("/login");
                }
                else
                {
                    TempData["ResetPassword_Error"] = Localizer.Get("ResetPassword_Failure");
                }
            }
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AuthenticateWithFacebook(string token)
        {
            try
            {
                var isValid = ClassyAuth.AuthenticateOrConnectFacebookUser(token);
                if (isValid) return Json(new { IsValid = true, Profile = (User.Identity as ClassyIdentity).Profile });
                else return Json(new { IsValid = false });
            }
            catch (WebException wex)
            {
                return new HttpStatusCodeResult((wex.Response as HttpWebResponse).StatusCode);
            }
            catch (UnauthorizedAccessException uex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, uex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AuthenticateWithGoogle(string token)
        {
            try
            {
                var isValid = ClassyAuth.AuthenticateOrConnectGoogleUser(token);
                if (isValid) return Json(new { IsValid = true, Profile = (User.Identity as ClassyIdentity).Profile });
                else return Json(new { IsValid = false });
            }
            catch (WebException wex)
            {
                return new HttpStatusCodeResult((wex.Response as HttpWebResponse).StatusCode);
            }
            catch (UnauthorizedAccessException uex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, uex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Logout()
        {
            try
            {
                ClassyAuth.Logout();
                var returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "~/";
                return new RedirectResult(returnUrl);
            }
            catch (WebException wex)
            {
                return new HttpStatusCodeResult((wex.Response as HttpWebResponse).StatusCode);
            }
            catch (UnauthorizedAccessException uex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, uex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Register(string referrerUrl, bool? forceProRegistration)
        {
            if (Request.IsAuthenticated) return RedirectToRoute("PublicProfile");

            var model = new RegistrationViewModel<TMetadata>
            {
                ReferrerUrl = referrerUrl,
                ForceProRegistration = forceProRegistration.HasValue && forceProRegistration.Value,
                IsProfessional = forceProRegistration.HasValue && forceProRegistration.Value
            };
            return View(model);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompleteRegistration(RegistrationViewModel<TMetadata> model)
        {
            var profile = (User.Identity as ClassyIdentity).Profile;
            if (model.Metadata != null)
            {
                var service = new ProfileService();
                profile = service.UpdateProfile(
                    profile.Id, 
                    null, 
                    null,
                    model.Metadata.ToDictionary(), 
                    null,
                    null,
                    UpdateProfileFields.Metadata);
            }

            if (OnProfileRegistered != null)
                OnProfileRegistered(this, profile);

            if (!string.IsNullOrEmpty(model.ReferrerUrl) && !model.IsProfessional) return Redirect(HttpUtility.UrlDecode(model.ReferrerUrl));
            else
            {
                
                if (model.IsProfessional) 
                    return RedirectToRoute("CreateProfessionalProfile", new { ProfileId = profile.Id, ReferrerUrl = model.ReferrerUrl });
                else 
                    return RedirectToRoute("PublicProfile", new { ProfileId = profile.Id, ReferrerUrl = model.ReferrerUrl });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(RegistrationViewModel<TMetadata> model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                if (!ClassyAuth.Register(model.Username, model.Email, model.Password))
                    throw new Exception("what happened?");

                return CompleteRegistration(model);
            }
            catch (ClassyException eex)
            {
                AddModelErrors(eex);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ResendEmailVerification()
        {
            var profile = (User.Identity as ClassyIdentity).Profile;
            if (OnProfileRegistered != null)
                OnProfileRegistered(this, profile);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}