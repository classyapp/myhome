using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    }
}