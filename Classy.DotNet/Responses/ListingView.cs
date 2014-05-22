using System.Collections.Generic;

namespace Classy.DotNet.Responses
{
    public class ListingView 
    {
        public ListingView() { }
        //
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public IList<string> Categories { get; set; }
        public string ListingType { get; set; }
        public IList<MediaFileView> ExternalMedia { get; set; }
        public LocationView Location { get; set; }
        public int CommentCount { get; set; }
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public int BookingCount { get; set; }
        public int PurchaseCount { get; set; }
        public int AddToCollectionCount { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        //
        public IList<CommentView> Comments { get; set; }
        //
        public bool HasPricingInfo { get; set; }
        public PricingInfoView PricingInfo { get; set; }
        //
        public bool HasContactInfo { get; set; }
        public ContactInfoView ContactInfo { get; set; }
        // 
        public bool HasShippingInfo { get; set; }
        public int? DomesticRadius { get; set; }
        public decimal? DomesticShippingPrice { get; set; }
        public decimal? InternationalShippingPrice { get; set; }
        //
        public bool HasInventoryInfo { get; set; }
        public int? Quantity { get; set; }
        //
        public IList<string> Hashtags { get; set; }
        public IDictionary<string, IList<string>> TranslatedKeywords { get; set; }
        public IList<string> SearchableKeywords { get; set; } 
        public IList<string> EditorKeywords { get; set; }
        public int EditorsRank { get; set; }
        //
        public ProfileView Profile { get; set; }
        public IList<ProfileView> FavoritedBy { get; set; }
        //
        public IDictionary<string, string> Metadata { get; set; }
        //
        public bool HasSchedulingInfo { get; set; }
        public TimeslotScheduleView SchedulingTemplate { get; set; }
        public IList<BookedTimeslotView> BookedTimeslots { get; set; }

        public string DefaultCulture { get; set; }
    }

    // A light-weight version of ListingView that holds the basic details
    // of a listing for displaying in search results, or other lists
    public class ListingViewSummary
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ListingType { get; set; }
        public IList<MediaFileView> ExternalMedia { get; set; }
        public LocationView Location { get; set; }
        public int CommentCount { get; set; }
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public int BookingCount { get; set; }
        public int PurchaseCount { get; set; }
        public int AddToCollectionCount { get; set; }
        public int DisplayOrder { get; set; }
        public bool HasPricingInfo { get; set; }
        public PricingInfoView PricingInfo { get; set; }
        public IList<string> Hashtags { get; set; }
    }

    public static class ListingViewSummaryExtensions
    {
        public static ListingView ToListingView(this ListingViewSummary summary)
        {
            return new ListingView {
                Id = summary.Id,
                Title = summary.Title,
                Content = summary.Content,
                ListingType = summary.ListingType,
                ExternalMedia = summary.ExternalMedia,
                Location = summary.Location,
                CommentCount = summary.CommentCount,
                FavoriteCount = summary.FavoriteCount,
                ViewCount = summary.ViewCount,
                ClickCount = summary.ClickCount,
                BookingCount = summary.BookingCount,
                PurchaseCount = summary.PurchaseCount,
                AddToCollectionCount = summary.AddToCollectionCount,
                DisplayOrder = summary.DisplayOrder,
                HasPricingInfo = summary.HasPricingInfo,
                PricingInfo = summary.PricingInfo,
                Hashtags = summary.Hashtags
            };
        }
    }
}
