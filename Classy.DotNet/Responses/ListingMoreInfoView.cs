﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class ListingMoreInfoView
    {
        public string CollectionType { get; set; }
        public IList<ListingView> CollectionLisitngs { get; set; }
        public IList<CollectionView> Collections { get; set; }
        public IList<ListingView> SearchResults { get; set; }
        public ProfileView Profile { get; set; }
    }
}
