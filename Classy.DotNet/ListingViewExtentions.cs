using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Classy.DotNet
{
    public static class ListingViewExtensions
    {
        public static bool CanEdit(this ListingView listing)
        {
            var isAdmin = false;
            var isOwner = false;
            var context = HttpContext.Current;
            if (context != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                isAdmin = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.IsAdmin;
                isOwner = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Id == listing.ProfileId;
            }
            return isAdmin || isOwner;
        }
    }
}
