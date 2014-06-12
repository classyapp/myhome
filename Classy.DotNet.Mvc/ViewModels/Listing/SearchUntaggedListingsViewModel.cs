using Classy.DotNet.Responses;
using System.Collections.Generic;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class SearchUntaggedListingsViewModel
    {
        public int Page { get; set; }
        //
        public IList<ListingView> Results { get; set; }
        public long Count { get; set; }

        public SearchUntaggedListingsViewModel()
        {
            Page = 1;
        }
    }
}
