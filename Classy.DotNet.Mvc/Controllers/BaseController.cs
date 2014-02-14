using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.Controllers
{
    public abstract class BaseController : Controller
    {
        public string Namespace { get; private set; }
        public int PageSize { get; set; }
        public int PagesCount { get; set; }

        public BaseController()
            : this("Classy.DotNet.Mvc.Controllers")
        {
        }

        public BaseController(string ns)
        {
            Namespace = ns;
            EnsureAppSettings();
            PageSize = (int)System.Web.HttpContext.Current.Cache["PAGESIZE"];
            PagesCount = (int)System.Web.HttpContext.Current.Cache["PAGESCOUNT"];

        }

        private void EnsureAppSettings()
        {
            if (System.Web.HttpContext.Current.Cache["PAGESIZE"] == null)
            {
                var service = new SettingsService();
                AppSettingsResponse settings = service.GetAppSettings();
                System.Web.HttpContext.Current.Cache["PAGESIZE"] = settings.PageSize;
                System.Web.HttpContext.Current.Cache["PAGESCOUNT"] = settings.PagesCount;
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            Localization.Localizer.Initialize(RouteData.DataTokens[Localizer.ROUTE_LOCALE_DATA_TOKEN_KEY] as string, Request["showResourceKeys"] != null);
            return base.BeginExecuteCore(callback, state);
        }

        public abstract void RegisterRoutes(RouteCollection routes);

        public ProfileView AuthenticatedUserProfile
        {
            get
            {
                if (User.Identity == null || !User.Identity.IsAuthenticated) return null;
                else return (User.Identity as ClassyIdentity).Profile;
            }
        }

        public void AddModelErrors(ClassyException eex)
        {
            foreach (var e in eex.Errors)
            {
                var key = string.Concat(e.FieldName, e.ErrorCode);
                var message = Localizer.Get(key);
                ModelState.AddModelError(e.FieldName ?? key, message);
            }
        }
    }
}
