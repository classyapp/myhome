using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classy.DotNet.Responses;

namespace MyHome.Views.ViewModels
{
    public class PhotoGridViewModel
    {
        public IList<ListingView> Photos { get; set; }
        public bool ShowCopyrightMessages { get; set; }
        public int ThumbnailSize { get; set; }
    }
}