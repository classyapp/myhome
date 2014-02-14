using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class SearchListingsViewModel<TListingMetadata>
    {
        public string Tag { get; set; }
        public TListingMetadata Metadata { get; set; }
        public LocationView Location { get; set; }
        public double? PriceMin { get; set; }
        public double? PriceMax { get; set; }
        public int Page { get; set; }

        //
        public IList<ListingView> Results { get; set; }
        public long Count { get; set; }

        public SearchListingsViewModel()
        {
            Page = 1;
        }
    }
}
