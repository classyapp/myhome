using System.Collections.Generic;

namespace Classy.DotNet.Responses
{
    public class FreeSearchResultsView<T>
    {
        public int Total { get; set; }
        public List<T> Results { get; set; }
    }

    public class FreeSearchResultsView
    {
        public FreeSearchResultsView<ListingViewSummary> ListingsResults { get; set; }
        public FreeSearchResultsView<ProfileView> ProfilesResults { get; set; }
        public FreeSearchResultsView<ListingViewSummary> ProductsResults { get; set; }
    }

    public class PhotoSearchResult // this is like ListingIndexDto in classy right now
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string ListingType { get; set; }
        public string[] Keywords { get; set; }

        public int CommentCount { get; set; }
        public int FavoriteCount { get; set; }
        public int FlagCount { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public int PurchaseCount { get; set; }
        public int BookingCount { get; set; }
        public int AddToCollectionCount { get; set; }

        public string ImageUrl { get; set; }

        public string[] Metadata { get; set; }
    }
}