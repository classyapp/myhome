﻿using System.Collections.Generic;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class SearchService : ServiceBase
    {
        private static readonly string GET_SEARCH_LISTINGS_SUGGESTIONS = ENDPOINT_BASE_URL + "/search/listing/suggest?q={0}";
        private static readonly string GET_SEARCH_PROFILES_SUGGESTIONS = ENDPOINT_BASE_URL + "/search/profile/suggest?q={0}";

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
    }
}