﻿using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class SearchProfileViewModel<TProMetadata>
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public TProMetadata Metadata { get; set; }
        public IList<ProfileView> Results { get; set; }
        public long Count { get; set; }
        public int Page { get; set; }
        public string Format { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public SearchProfileViewModel()
        {
            Page = 1;
            Format = "html";
        }
    }
}