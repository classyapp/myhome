using System.Web.Mvc;
using Classy.DotNet.Mvc.ActionFilters;

namespace MyHome
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AppHarbor.Web.RequireHttpsAttribute());
            // error handling is dealt with via web.config & iis
           //  filters.Add(new HandleErrorAttribute());

            filters.Add(new FeatureSwitchFilter());
        }
    }
}
