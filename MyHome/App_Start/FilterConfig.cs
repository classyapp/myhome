using System.Web;
using System.Web.Mvc;

namespace MyHome
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // error handling is dealt with via web.config & iis
           //  filters.Add(new HandleErrorAttribute());
        }
    }
}
