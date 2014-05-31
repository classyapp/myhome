using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classy.DotNet.Mvc.Controllers
{
    public class LoadPublicProfileEventArgs<TProMetadata>
    {
        public ProfileView Profile;
        public TProMetadata TypedMetadata;
        public IList<ProfileView> RelatedProfiles { get; set; }
        public IList<ListingView> RelatedListings { get; set; }
    }
}
