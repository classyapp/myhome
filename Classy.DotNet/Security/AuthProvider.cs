using System.Web;

namespace Classy.DotNet.Security
{
    public static class AuthProvider
    {
        public static bool IsEditor()
        {
            var context = HttpContext.Current;
            if (context == null)
                return false;

            var identity = HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity;
            if (identity == null || !identity.IsAuthenticated)
                return false;

            return identity.Profile.IsEditor;
        }

        public static bool IsAdmin()
        {
            var context = HttpContext.Current;
            if (context == null)
                return false;

            var identity = HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity;
            if (identity == null || !identity.IsAuthenticated)
                return false;

            return identity.Profile.IsAdmin;
        }
    }
}