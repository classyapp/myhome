using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Classy.DotNet
{
    public static class ProfileViewExtensions
    {
        public static bool CanEdit(this ProfileView profile)
        {
            var isAdmin = false;
            var isProfileOwner = false;
            var context = HttpContext.Current;
            if (context != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                isAdmin = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Permissions.Contains("admin");
                isProfileOwner = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Id == profile.Id;
            }
            return isAdmin || isProfileOwner;
        }
    }
}
