using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Classy.DotNet.Responses;

namespace Classy.DotNet
{
    public static class CollectionViewExtensions
    {
        public static bool CanEdit(this CollectionView collection)
        {
            var isAdmin = false;
            var isCollectionOwner = false;
            var context = HttpContext.Current;
            if (context != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                isAdmin = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.IsAdmin;
                isCollectionOwner = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Id == collection.ProfileId;
            }
            return isAdmin || isCollectionOwner;
        }
    }
}
