﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class SearchResultsView
    {
        public IList<ListingView> Results { get; set; }
        public long Count { get; set; }
    }
}
