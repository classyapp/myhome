using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc
{
    public static class ImageExtensions
    {
        #region // profile 

        public static MvcHtmlString AvatarUrl(this ProfileView profile)
        {
            return AvatarUrl(profile, null, false);
        }

        public static MvcHtmlString AvatarUrl(this ProfileView profile, int? width, bool cropSquare)
        {
            if (profile.Avatar == null) return new MvcHtmlString(AppView.DefaultProfileImage);
            else return new MvcHtmlString(profile.Avatar.Url);
        }

        #endregion

        #region // collections

        public static MvcHtmlString ThumbnailUrl(this CollectionView collection, int? width, bool cropSquare)
        {
            var url = "/img/missing-thumb.png";

            if (collection.Thumbnails.Count() > 0)
            {
                var thumbnail = collection.Thumbnails.FirstOrDefault(x => x.Width == width.Value);
                if (width.HasValue && thumbnail != null)
                {
                    url = thumbnail.Url;
                }
            }

            return new MvcHtmlString(url);
        }

        #endregion
    }
}
