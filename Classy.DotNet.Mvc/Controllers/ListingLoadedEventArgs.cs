using System.Collections.Generic;
using Classy.DotNet.Mvc.ViewModels.Listing;

namespace Classy.DotNet.Mvc.Controllers
{
    public class ListingLoadedEventArgs<TListingMetadata> where TListingMetadata : IMetadata<TListingMetadata>, new()
    {
        public ListingDetailsViewModel<TListingMetadata> ListingDetailsViewModel { get; set; }
    }
}
