using System.Collections.Generic;

namespace Classy.DotNet.Responses
{
    public class FreeSearchResultsView<T>
    {
        public int Total { get; set; }
        public List<T> Results { get; set; }
    }

    public class PhotoSearchResult // this is like ListingIndexDto in classy right now
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ListingType { get; set; }
        public string[] Keywords { get; set; } 
        public string[] Metadata { get; set; }
    }
}