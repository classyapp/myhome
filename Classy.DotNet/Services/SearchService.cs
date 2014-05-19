using System.Collections.Generic;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class SearchService : ServiceBase
    {
        private static readonly string GET_SEARCH_SUGGESTIONS = ENDPOINT_BASE_URL + "/search/suggest?q={0}";

        public List<SearchSuggestion> GetSearchSuggestions(string q)
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var suggestions = client.DownloadString(string.Format(GET_SEARCH_SUGGESTIONS, q));
                var response = suggestions.FromJson<List<SearchSuggestion>>();
                return response;
            }
        }
    }
}