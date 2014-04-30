using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ViewModels.Localization;
using Classy.DotNet.Services;
using System.Web;
using Classy.DotNet.Mvc.Localization;
using System.Text.RegularExpressions;
using Classy.DotNet.Mvc.Attributes;

namespace Classy.DotNet.Mvc.Controllers
{
    public class LocalizationController : BaseController
    {
        public LocalizationController() : base() { }
        public LocalizationController(string ns) : base(ns) { }

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "EnvironmentSettings",
                url: "settings/locale",
                defaults: new { controller = "Localization", action = "EnvironmentSettings" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ManageResources",
                url: "resource/manage",
                defaults: new { controller = "Localization", action = "ManageResources" },
                namespaces: new string[] { Namespace }
            );
        }

        //
        // GET: /resource/manage
        //
        [AcceptVerbs(HttpVerbs.Get)]
        [AuthorizeWithRedirect("Home", new string[] { "cms" } )]
        public ActionResult ManageResources(string resourceKey)
        {
            var model = new ManageResourcesViewModel {
                SupportedCultures = Localizer.GetList("supported-cultures").AsSelectList(),
                MissingResourceKeys = Localizer.GetMissingKeys(),
                SelectedCulture = GetEnvFromContext().CultureCode,
                ResourceKey = resourceKey
            };
            if (!string.IsNullOrEmpty(model.ResourceKey) && !string.IsNullOrEmpty(model.SelectedCulture))
            {
                if (resourceKey.StartsWith("List__"))
                {
                    var regex = new Regex("List__(.*)_(.*)", RegexOptions.Compiled);
                    var matches = regex.Match(resourceKey);
                    var key = matches.Groups[1].Value;
                    var item = Localizer.GetList(key).SingleOrDefault(x => x.Value == matches.Groups[2].Value);
                    model.ResourceValue = item != null ? item.Text : resourceKey;
                }
                else
                {
                    var service = new LocalizationService();
                    var resource = service.GetResourceByKey(resourceKey, false);
                    if (resource != null && resource.Values.ContainsKey(model.SelectedCulture))
                    {
                        model.ResourceValue = resource.Values[model.SelectedCulture];
                        model.ResourceDescription = resource.Description;
                    }
                    else model.ResourceValue = null;
                }
            }
            return View(model);
        }

        //
        // POST: /resource/manage
        //
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult ManageResources(ManageResourcesViewModel model, object dummy)
        {
            model.ResourceValue = model.ResourceValue;
            model.MissingResourceKeys = Localizer.GetMissingKeys();
            model.SelectedCulture = GetEnvFromContext().CultureCode;
            var service = new LocalizationService();
            if (model.ResourceKey.StartsWith("List__"))
            {
                var regex = new Regex("List__(.*)_(.*)", RegexOptions.Compiled);
                var matches = regex.Match(model.ResourceKey);
                var key = matches.Groups[1].Value;
                var value = matches.Groups[2].Value;
                service.SetListResourceValue(key, model.SelectedCulture, value, model.ResourceValue);
                HttpRuntime.Cache.Remove(key);
            }
            else
            {
                service.SetResourceValues(model.ResourceKey, new Dictionary<string, string> { { model.SelectedCulture, model.ResourceValue } });
                HttpRuntime.Cache.Remove(model.ResourceKey);
            }
            model.SupportedCultures = Localizer.GetList("supported-cultures").AsSelectList();
            return View(model);
        }

        //
        // GET: /profile/settings/env
        //
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult EnvironmentSettings()
        {
            var model = GetEnvFromContext();
            return PartialView(model);
        }

        //
        // POST: /profile/settings/env
        //
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EnvironmentSettings(EnvironmentSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                SetContextEnvFromModel(model);
            }
            return Json(new { IsValid = true });
        }


        private EnvironmentSettingsViewModel GetEnvFromContext()
        {
            return new EnvironmentSettingsViewModel
            {
                CultureCode = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                CultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.DisplayName,
                CountryCode = System.Web.HttpContext.Current.Request.Cookies[Classy.DotNet.Responses.AppView.CountryCookieName].Value
            };
        }

        private void SetContextEnvFromModel(EnvironmentSettingsViewModel model)
        {
            Response.Cookies.Add(new System.Web.HttpCookie(Classy.DotNet.Responses.AppView.CultureCookieName)
            {
                Value = model.CultureCode,
                Expires = DateTime.UtcNow.AddYears(30)
            });
            Response.Cookies.Add(new System.Web.HttpCookie(Classy.DotNet.Responses.AppView.CountryCookieName)
            {
                Value = model.CountryCode,
                Expires = DateTime.UtcNow.AddYears(30)
            });
        }
    }
}
