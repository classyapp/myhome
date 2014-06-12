using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;
using Classy.DotNet.Services;

namespace Classy.DotNet.Responses
{
    public class AppView
    {
        public static int PageSize { get; set; }
        public static int PagesCount { get; set; }
        public static string DefaultProfileImage { get; set; }
        public static string GPSLocationCookieName { get; set; }
        public static string GPSOriginCookieName { get; set; }
        public static string CountryCookieName { get; set; }
        public static string CultureCookieName { get; set; }
        public static string CurrencyCookieName { get; set; }
        public static string DefaultCountry { get; set; }
        public static string DefaultCulture { get; set; }
        public static string DefaultCurrency { get; set; }
        public static string Hostname { get; set; }

        public static IList<CurrencyListItemView> SupportedCurrencies { get; set; }
        public static IList<ListItemView> SupportedCultures { get; set; }
        public static IList<ListItemView> SupportedCountries { get; set; }
        public static IList<ListItemView> ProductCategories { get; set; }

        static AppView()
        {
            var service = new SettingsService();
            AppSettingsResponse settings = service.GetAppSettings();
            PageSize = settings.PageSize;
            PagesCount = settings.PagesCount;
            DefaultProfileImage = settings.DefaultProfileImage;
            GPSLocationCookieName = settings.GPSLocationCookieName;
            GPSOriginCookieName = settings.GPSOriginCookieName;
            CountryCookieName = settings.CountryCookieName;
            CultureCookieName = settings.CultureCookieName;
            CurrencyCookieName = settings.CurrencyCookieName;
            DefaultCountry = settings.DefaultCountry;
            DefaultCulture = settings.DefaultCulture;
            DefaultCurrency = settings.DefaultCurrency;
            Hostname = settings.Hostname;

            SupportedCurrencies = settings.SupportedCurrencies;
            SupportedCountries = settings.SupportedCountries;
            SupportedCultures = settings.SupportedCultures;
            ProductCategories = settings.ProductCategories;
        }
    }
}
