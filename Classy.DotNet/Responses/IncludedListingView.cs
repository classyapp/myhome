using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class IncludedListingView : IListing
    {
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string Comments { get; set; }
        public string ListingType { get; set; }

        public IncludedListingView()
        {
            // backward compatability
            ListingType = "photo";
        }
    }
}
