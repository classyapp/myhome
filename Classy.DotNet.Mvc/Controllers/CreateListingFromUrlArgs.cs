using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classy.DotNet.Mvc.Controllers
{
    public class CreateListingFromUrlArgs<TListingMetadata>
    {
        public Uri RequestUri { get; set; }
        public ListingView Listing { get; set; }
        public TListingMetadata Metadata { get; set; }
        public string ExternalMediaUrl { get; set; }
    }
}
