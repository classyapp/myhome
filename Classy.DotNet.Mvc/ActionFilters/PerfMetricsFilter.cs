using System.Linq;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Config;
using Classy.DotNet.Mvc.Config.PerfMetrics;
using Classy.DotNet.Mvc.Extensions;

namespace Classy.DotNet.Mvc.ActionFilters
{
    public class PerfMetricsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            if (httpContext == null)
                return;

            var request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            var perfFlag = request.QueryString.Get("perf");
            if (perfFlag == null || perfFlag != true.ToString().ToLower())
                return;

            httpContext.Items[PerfMetricsProvider.PerfMetricsEnabledKey] = true.ToString();
        }
    }
}