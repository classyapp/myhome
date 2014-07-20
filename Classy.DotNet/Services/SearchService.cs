using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class SearchService : ServiceBase
    {
        private static readonly string GET_SEARCH_LISTINGS_SUGGESTIONS = ENDPOINT_BASE_URL + "/search/listing/suggest?q={0}";
        private static readonly string GET_SEARCH_PROFILES_SUGGESTIONS = ENDPOINT_BASE_URL + "/search/profile/suggest?q={0}";
        private static readonly string GET_SEARCH_PRODUCTS_SUGGESTIONS = ENDPOINT_BASE_URL + "/search/product/suggest?q={0}";
        private static readonly string GET_SEARCH_KEYWORDS_SUGGESTIONS = ENDPOINT_BASE_URL + "/search/keywords/suggest?q={0}&lang={1}";
        
        public List<SearchSuggestion> SearchListingsSuggestions(string q)
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var suggestions = client.DownloadString(string.Format(GET_SEARCH_LISTINGS_SUGGESTIONS, q));
                var response = suggestions.FromJson<List<SearchSuggestion>>();
                return response;
            }
        }

        public List<SearchSuggestion> SearchProfilesSuggestions(string q)
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var suggestions = client.DownloadString(string.Format(GET_SEARCH_PROFILES_SUGGESTIONS, q));
                var response = suggestions.FromJson<List<SearchSuggestion>>();
                return response;
            }
        }

        public List<SearchSuggestion> SearchKeywordsSuggestions(string q)
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var suggestions = client.DownloadString(string.Format(GET_SEARCH_KEYWORDS_SUGGESTIONS, q, Thread.CurrentThread.CurrentUICulture.Name));
                var response = suggestions.FromJson<List<SearchSuggestion>>();
                return response;
            }
        }

        public List<SearchSuggestion> SearchProductsSuggestions(string q)
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var suggestions = client.DownloadString(string.Format(GET_SEARCH_PRODUCTS_SUGGESTIONS, q));
                var response = suggestions.FromJson<List<SearchSuggestion>>().Take(1).ToList();
                return response;
            }
        }

        public List<MobileSearchSuggestion> MobileSearchSuggestions(string q)
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var rooms = Localizer.GetList("rooms");
                var styles = Localizer.GetList("room-styles");
                var productCategories = Localizer.GetList("product-categories");

                var regex = new Regex("\\b" + q + "\\w*\\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                var suggestedRooms = rooms.Where(x => regex.Match(x.Text).Success).ToList();
                var suggestedStyles = styles.Where(x => regex.Match(x.Text).Success).ToList();
                var suggestedCategories = productCategories.Where(x => regex.Match(x.Text).Success).ToList();

                return new List<MobileSearchSuggestion> {
                    new MobileSearchSuggestion {
                        Name = Localizer.Get("Mobile_SearchAutoSuggest_RoomsSectionTitle"),
                        Suggestions = suggestedRooms.Select(x => new SearchSuggestion {Key = x.Text, Value = x.Value}).ToList()
                    },
                    new MobileSearchSuggestion {
                        Name = Localizer.Get("Mobile_SearchAutoSuggest_StylesSectionTitle"),
                        Suggestions = suggestedStyles.Select(x => new SearchSuggestion { Key = x.Text, Value = x.Value }).ToList()
                    },
                    new MobileSearchSuggestion {
                        Name = Localizer.Get("Mobile_SearchAutoSuggest_ProductCategoriesSectionTitle"),
                        Suggestions = suggestedCategories.Select(x => new SearchSuggestion { Key = x.Text, Value = x.Value }).ToList()
                    }
                };
            }
        }
    }

    public class MobileSearchSuggestion
    {
        public string Name { get; set; }
        public List<SearchSuggestion> Suggestions { get; set; }
    }
}