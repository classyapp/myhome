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
        public static MvcHtmlString AvatarUrl(this ProfileView profile)
        {
            return AvatarUrl(profile, null, false);
        }

        public static MvcHtmlString AvatarUrl(this ProfileView profile, int? width, bool cropSquare)
        {
            if (profile.Avatar == null) return new MvcHtmlString(AppView.DefaultProfileImage);
            else return new MvcHtmlString(profile.Avatar.Url);


        }
    }
}
