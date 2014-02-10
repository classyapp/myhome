using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class ShowListingByTypeViewModel<TListingMetadata>
    {
        public ProfileView Profile { get; set; }
        public IList<ListingView> Listings { get; set; }
        public TListingMetadata Metadata { get; set; }
    }
}
