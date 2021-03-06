﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using Classy.DotNet.Services;
using System.Web.Routing;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.Localization
{
    public static class Localizer
    {
        public const string ROUTE_LOCALE_DATA_TOKEN_KEY = "classy.routetoken.locale";

        private static bool _showResourceKeys = false;

        static Localizer()
        {
            // init db
        }

        public static void Initialize()
        {
            Initialize(null, false);
        }

        public static void Initialize(string forceCulture, bool showResourceKeys)
        {
            _showResourceKeys = showResourceKeys;

            var supportedCultures = AppView.SupportedCultures;

            // first we check if the culture is forced (by using a direct url containing a culture)
            // second we check if there is a culture cookie 
            var cookie = HttpContext.Current.Request.Cookies[AppView.CultureCookieName];
            string cultureName = forceCulture ?? (cookie != null ? cookie.Value : null);
            if (cultureName != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureName);
            }
            // third, use only the country part of the browser default culture
            else 
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name.Substring(0,2));
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0,2));
            }
            // if the culture we ended up with is not supported, fallback to the default culture for the app
            if (!supportedCultures.Any(x => x.Value == System.Threading.Thread.CurrentThread.CurrentUICulture.Name))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(AppView.DefaultCulture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(AppView.DefaultCulture);
            }

            // init country
            cookie = HttpContext.Current.Request.Cookies[AppView.CountryCookieName];
            Classy.DotNet.Mvc.GeoIP.Location location = Helpers.IPLocator.GetLocationByRequestIP();
            if (cookie == null)
            {
                var supportedCountries = AppView.SupportedCountries;
                string countryCode = supportedCountries.Any(c => c.Value == location.CountryCode) ? location.CountryCode : AppView.DefaultCountry;
                cookie = new HttpCookie(AppView.CountryCookieName, countryCode);
                cookie.Expires = DateTime.Now.AddMonths(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            // init gps coordinates cookie 
            cookie = HttpContext.Current.Request.Cookies[AppView.GPSOriginCookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(AppView.GPSOriginCookieName, "auto");
                cookie.Expires = DateTime.Now.AddMonths(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            cookie = HttpContext.Current.Request.Cookies[AppView.GPSLocationCookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(AppView.GPSLocationCookieName, Newtonsoft.Json.JsonConvert.SerializeObject(new { latitude = location.Latitude, longitude = location.Longitude }));
                cookie.Expires = DateTime.Now.AddMonths(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            cookie = HttpContext.Current.Request.Cookies[AppView.CurrencyCookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(AppView.CurrencyCookieName, AppView.DefaultCurrency);
                cookie.Expires = DateTime.Now.AddMonths(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

        }

        public static string Get(string key)
        {
            return Get(key, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }

        public static string Get(string key, bool processMarkdown)
        {
            return Get(key, System.Threading.Thread.CurrentThread.CurrentUICulture.Name, processMarkdown);
        }

        public static string Get(string key, string culture, bool processMarkdown = true)
        {
            string value = null;
            LocalizationResourceView resource = HttpRuntime.Cache[key] as LocalizationResourceView;
            if (resource == null)
            {
                var service = new LocalizationService();
                resource = service.GetResourceByKey(key, processMarkdown);
                if (resource != null) HttpRuntime.Cache[key] = resource;
            }
            if (resource != null)
            {
                value = HttpUtility.HtmlDecode(resource.Values.SingleOrDefault(x => x.Key == culture).Value);
            }

            var output = value ?? key;
            if (_showResourceKeys && !string.IsNullOrEmpty(value)) output = string.Concat(output, " [", key, "]");
            return output;
        }

        public static IEnumerable<LocalizedListItem> GetList(string key)
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
                            select new LocalizedListItem
                            {
                                Text = GetListResourceText(key, item, _showResourceKeys),
                                Value = item.Value,
                                ParentValue = item.ParentValue
                            };
                return items;
            }
            return null;
        }

        public static string GetText(this IEnumerable<LocalizedListItem> list, string key)
        {
            if (!list.Any(x => x.Value == key)) return null;
            return list.First(x => x.Value == key).Text;
        }

        private static string GetListResourceText(string key, ListItemView item, bool showResourceKeys) {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            var text = HttpUtility.HtmlDecode(item.Text.ContainsKey(culture) ? item.Text[culture] : item.Value);
            var output = _showResourceKeys ?
                string.Concat((text == item.Value) ? null : text, "[List__", key, "_", item.Value, "]") :
                text;
            return output;
        }

        public static string[] GetMissingKeys()
        {
            var service = new LocalizationService();
            return service.GetMissingResources(System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }

        public static IList<string> GetCitiesByCountryCode(string countryCode)
        {
            var cacheKey = string.Concat("CitiesIn", countryCode);
            IList<string> cities = HttpRuntime.Cache[cacheKey] as IList<string>;
            if (cities == null)
            {
                var service = new LocalizationService();
                cities = service.GetCitiesByCountry(countryCode);
                HttpRuntime.Cache.Insert(cacheKey, cities, null, DateTime.Now.AddDays(1), TimeSpan.Zero);
            }
            return cities;
        }

        public static IList<string> GetCitiesWithCountries()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            var cacheKey = string.Concat("CitiesWithCountries", culture);
            IList<string> citiesWithCountries = HttpRuntime.Cache[cacheKey] as IList<string>;
            if (citiesWithCountries == null)
            {
                var supportedCountries = AppView.SupportedCountries;
                citiesWithCountries = new List<string>();
                foreach (var country in supportedCountries)
                {
                    citiesWithCountries.Add(country.Text[System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName]);
                    (citiesWithCountries as List<string>).AddRange(GetCitiesByCountryCode(country.Value).Select(c => string.Format("{0}, {1}", c, country.Text[System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName])));
                }
                HttpRuntime.Cache.Insert(cacheKey, citiesWithCountries, null, DateTime.Now.AddDays(1), TimeSpan.Zero);
            }
            return citiesWithCountries;
        }

        #region // localization of routes

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
            var cultures = AppView.SupportedCultures;
            foreach (var culture in cultures)
            {
                var cultureName = culture.Value.Substring(0, 2);
                var routeName = GetRouteNameForLocale(name, cultureName);
                route = routes.MapRoute(
                    routeName,
                    string.Concat(cultureName, "/", url), 
                    defaults, 
                    namespaces);
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens.Add("RouteName", routeName);
                route.DataTokens.Add("OriginalRouteName", name);
                route.DataTokens.Add(ROUTE_LOCALE_DATA_TOKEN_KEY, cultureName);
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
            return RouteUrlForLocale(url, routeName, cultureCode, null);
        }

        public static string RouteUrlForLocale(this System.Web.Mvc.UrlHelper url, string routeName, string cultureCode, object routeValues)
        {
            string name = GetRouteNameForLocale(routeName, cultureCode);
            if (url.RouteCollection[name] != null)
                return url.RouteUrl(name, routeValues);
            else
                return url.RouteUrl(routeName, routeValues);
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

        #endregion

    }
}
