using System.Web.Mvc;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.ActionFilters
{
    public class AppSettingsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var settingsService = new SettingsService();
            filterContext.Controller.ViewBag.AppSettings = settingsService.GetAppSettings();
        }
    }
}