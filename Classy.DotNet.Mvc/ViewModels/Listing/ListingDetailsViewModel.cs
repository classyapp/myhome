using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class ListingDetailsViewModel<TListingMetadata>
    {
        public ListingView Listing { get; set; }
        public TListingMetadata Metadata { get; set; }
        public object ExtraData { get; set; }
    }
}
