using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages.Html;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using Classy.DotNet.Services;
using System.Globalization;
using System.Web.Routing;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.Localization
{
    public static class Localizer
    {
        public const string CULTURE_COOKIE_NAME = "classy.env.culture";
        public const string SUPPORTED_CULTURES_CACHE_KEY = "classy.cache.supported-languages";
        public const string SUPPORTED_COUNTRIES_CACHE_KEY = "classy.cache.supported-countries";
        public const string SUPPORTED_CURRENCIES_CACHE_KEY = "classy.cache.supported-currencies";
        public const string ROUTE_LOCALE_DATA_TOKEN_KEY = "classy.routetoken.locale";

        static Localizer()
        {
            // init db
        }

        public static void Initialize()
        {
            Initialize(null);
        }
        public static void Initialize(string forceCulture)
        {
            var cookie = HttpContext.Current.Request.Cookies[CULTURE_COOKIE_NAME];
            string cultureName = forceCulture ?? (cookie != null ? cookie.Value : null);
            if (cultureName != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureName);
            }
            if (SupportedCultures.SingleOrDefault(x => x.Name == System.Threading.Thread.CurrentThread.CurrentUICulture.Name) == null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
        }

        public static IEnumerable<CultureInfo> SupportedCultures
        {
            get
            {
                var languages = HttpRuntime.Cache[SUPPORTED_CULTURES_CACHE_KEY] as List<CultureInfo>;
                if (languages == null)
                {
                    var cultures = GetList("supported-cultures");
                    languages = new List<CultureInfo>();
                    foreach(var li in cultures)
                    {
                        languages.Add(CultureInfo.CreateSpecificCulture(li.Value));
                    }
                    HttpRuntime.Cache[SUPPORTED_CULTURES_CACHE_KEY] = languages;
                }
                return languages;
            }
        }

        public static string Get(string key)
        {
            return Get(key, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }

        public static string Get(string key, string culture)
        {
            string value = null;
            LocalizationResourceView resource = HttpRuntime.Cache[key] as LocalizationResourceView;
            if (resource == null)
            {
                var service = new LocalizationService();
                resource = service.GetResourceByKey(key);
                if (resource != null) HttpRuntime.Cache[key] = resource;
            }
            if (resource != null)
            {
                value = HttpUtility.HtmlDecode(resource.Values.SingleOrDefault(x => x.Key == culture).Value);
            }
            return value ?? string.Concat(key, "_", culture);
        }

        public static SelectList GetList(string key)
        {
            LocalizationListResourceView resource = HttpRuntime.Cache[key] as LocalizationListResourceView;
            if (resource == null)
            {
                var service = new LocalizationService();
                resource = service.GetListResourceByKey(key);
                if (resource != null) HttpRuntime.Cache[key] = resource;
            }
            if (resource != null)
            {
                var items = from item in resource.ListItems
                            select new
                            {
                                Text = item.Text.ContainsKey(System.Threading.Thread.CurrentThread.CurrentUICulture.Name) ? 
                                    item.Text[System.Threading.Thread.CurrentThread.CurrentUICulture.Name] : 
                                    string.Concat(item.Value, "_", System.Threading.Thread.CurrentThread.CurrentUICulture.Name),
                                Value = HttpUtility.HtmlDecode(item.Value)
                            };
                return new SelectList(items, "Value", "Text");
            }
            return null;
        }

        public static string[] GetAllKeys()
        {
            var service = new LocalizationService();
            return service.GetResourceKeys();
        }

        // RouteCollection extension to map a route and pass its name as a datatoken
        public static void MapRouteWithName(
            this RouteCollection routes, 
            string name, 
            string url, 
            object defaults, 
            string[] namespaces)
        {
            Route route = routes.MapRoute(name, url, defaults, namespaces);
            route.DataTokens = new RouteValueDictionary();
            route.DataTokens.Add("RouteName", name);
            route.DataTokens.Add("OriginalRouteName", name);
        }

        // RouteCollection extension to map a route for all supported cultures
        public static void MapRouteForSupportedLocales(
            this RouteCollection routes, 
            string name, 
            string url, 
            object defaults, 
            string[] namespaces)
        {
            Route route;

            // map the route directly
            if (name != "Default") // edge case where the default route must be mapped after the localized versions of it
            { 
                route = routes.MapRoute(name, url, defaults, namespaces);
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens.Add("RouteName", name);
                route.DataTokens.Add("OriginalRouteName", name);
            }

            // then add another route for each supported culture
            foreach (var culture in SupportedCultures)
            {
                var routeName = GetRouteNameForLocale(name, culture.Name);
                route = routes.MapRoute(
                    routeName,
                    string.Concat(culture.Name, "/", url), 
                    defaults, 
                    namespaces);
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens.Add("RouteName", routeName);
                route.DataTokens.Add("OriginalRouteName", name);
                route.DataTokens.Add(ROUTE_LOCALE_DATA_TOKEN_KEY, culture.Name);
            }

            // map the route directly
            if (name == "Default") // edge case where the default route must be mapped after the localized versions of it
            {
                route = routes.MapRoute(name, url, defaults, namespaces);
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens.Add("RouteName", name);
                route.DataTokens.Add("OriginalRouteName", name);
            }
        }

        // Url extension to get a link to a route in the current culture
        public static string RouteUrlForCurrentLocale(this System.Web.Mvc.UrlHelper url, string routeName)
        {
            return RouteUrlForCurrentLocale(url, routeName, new { });
        }

        // Url extension to get a link to a route in the current culture
        public static string RouteUrlForCurrentLocale(this System.Web.Mvc.UrlHelper url, string routeName, object routeValues)
        {
            string name = GetRouteNameForLocale(routeName, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            if (url.RouteCollection[name] != null)
                return url.RouteUrl(name, routeValues);
            else
                return url.RouteUrl(routeName, routeValues);
        }

        // Url extension to get a link to a route in a specific culture
        public static string RouteUrlForLocale(this System.Web.Mvc.UrlHelper url, string routeName, string cultureCode)
        {
            string name = GetRouteNameForLocale(routeName, cultureCode);
            if (url.RouteCollection[name] != null)
                return url.RouteUrl(name);
            else
                return url.RouteUrl(routeName);
        }

        // Html extension to get a link to a route for the current culture
        public static MvcHtmlString RouteLinkForCurrentLocale(this System.Web.Mvc.HtmlHelper html, string linkText, string routeName)
        {
            return RouteLinkForCurrentLocale(html, linkText, routeName, null);
        }

        // Html extension to get a link to a route for the current culture
        public static MvcHtmlString RouteLinkForCurrentLocale(this System.Web.Mvc.HtmlHelper html, string linkText, string routeName, object routeValues)
        {
            string name = GetRouteNameForLocale(routeName, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            if (html.RouteCollection[name] != null)
                return html.RouteLink(linkText, name, routeValues);
            else
                return html.RouteLink(linkText, routeName, routeValues);
        }

        // Html extension to get a link to a route in a specific culture
        public static MvcHtmlString RouteLinkForLocale(this System.Web.Mvc.HtmlHelper html, string linkText, string routeName, string cultureName)
        {
            string name = GetRouteNameForLocale(routeName, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            if (html.RouteCollection[name] != null)
                return html.RouteLink(linkText, name);
            else
                return html.RouteLink(linkText, routeName);
        }

        private static string GetRouteNameForLocale(string name, string cultureCode)
        {
            return string.Concat(name, "_", cultureCode);
        }

    }
}
