using System.Collections.Generic;

namespace Classy.DotNet.Responses
{
    public class ListingMoreInfoView
    {
        public string CollectionType { get; set; }
        public IList<ListingView> CollectionLisitngs { get; set; }
        public IList<CollectionView> Collections { get; set; }
        public IList<ListingView> SearchResults { get; set; }
        public IDictionary<string, string[]> Metadata { get; set; }
    }
}
