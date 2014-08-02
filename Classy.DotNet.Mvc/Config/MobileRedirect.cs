using System.Web;

namespace Classy.DotNet.Mvc.Config
{
    public class MobileRedirect
    {
        public static bool IsMobileDevice()
        {
            var context = HttpContext.Current;
            if (context == null)
                return false;

            return context.Request.Browser.IsMobileDevice;
        }
    }
}