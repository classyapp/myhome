using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc
{
    [Flags]
    public enum ListingThumbnailOptions
    {
        /// <summary>
        /// set the width and height attributes on the rendered img tag
        /// </summary>
        SetWidthHeight = 0,
        /// <summary>
        /// whether or not the thumbnail will be lazy loaded
        /// </summary>
        LazyLoad = 1,
        /// <summary>
        /// whether or not to add an img tag inside a noscript tag (only relevant when using the LazyLoad option)
        /// </summary>
        AddNoScript = 2
    }

    public static class ListingViewExtensions
    {
        public static MvcHtmlString Thumbnail(this ListingView listing, ListingThumbnailOptions options, int width, int? height = null)
        {
            var html = new StringBuilder();

            return new MvcHtmlString(html);
        }
    }
}
