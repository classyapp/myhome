using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classy.DotNet.Responses;
using Classy.DotNet.Mvc.ViewModels.Listing;

namespace MyHome.Models
{
    public class PhotoGridViewModel : IListingGridViewModel
    {
        public PhotoGridViewModel()
        {
            ThumbnailSize = 270;
        }
        public IList<ListingView> Results { get; set; }
        public bool ShowCopyrightMessages { get; set; }
        public int ThumbnailSize { get; set; }
    }
}