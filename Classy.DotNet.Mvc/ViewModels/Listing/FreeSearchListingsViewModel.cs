﻿using Classy.DotNet.Responses;
using System.Collections.Generic;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class FreeSearchListingsViewModel
    {
        public string Q { get; set; }
        public LocationView Location { get; set; }
        public int Page { get; set; }
        public int Amount { get; set; }
        public IList<ListingView> Results { get; set; }
        public IList<ProfileView> RelatedProfessionals { get; set; }
        public IList<ListingView> RelatedProducts { get; set; }
        public long TotalResults { get; set; }

        public FreeSearchListingsViewModel()
        {
            Page = 1;
        }
    }
}
