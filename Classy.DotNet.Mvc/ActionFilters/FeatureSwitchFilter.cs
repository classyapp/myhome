using System.Linq;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Config;

namespace Classy.DotNet.Mvc.ActionFilters
{
    public class FeatureSwitchFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            if (httpContext == null)
                return;

            var request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            var featureSwitches = request.QueryString.Get("feature-switch");
            if (featureSwitches == null || !featureSwitches.Any())
                return;

            var featuresOn = featureSwitches.ToLower().Split(';');
            FeatureSwitchProvider.TurnOnFeatures(featuresOn);
        }
    }
}