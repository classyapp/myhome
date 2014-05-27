using System.Collections.Generic;

namespace Classy.DotNet.Responses
{
    public class SearchResultsView<T>
    {
        public IList<T> Results { get; set; }
        public long Count { get; set; }
        public int PageSize { get; set; }
        public int PagesCount { get; set; }
    }
}
