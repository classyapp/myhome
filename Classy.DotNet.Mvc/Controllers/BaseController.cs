using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.Extensions;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using System;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.Controllers
{
    public abstract class BaseController : Controller
    {
        public string Namespace { get; private set; }
        
        public BaseController()
            : this("Classy.DotNet.Mvc.Controllers")
        {
        }

        public BaseController(string ns)
        {
            Namespace = ns;
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            Localization.Localizer.Initialize(RouteData.DataTokens[Localizer.ROUTE_LOCALE_DATA_TOKEN_KEY] as string, Request["showResourceKeys"] != null);
            return base.BeginExecuteCore(callback, state);
        }

        public abstract void RegisterRoutes(RouteCollection routes);

        public void RegisterRoutesByAttributes(RouteCollection routes, string listingTypeName)
        {
            GetType().GetMethods().ForEach(x =>
            {
                var routeAttribute = x.GetCustomAttributes(typeof(MapRouteAttribute), true);
                if (routeAttribute.IsNullOrEmpty())
                    return;

                var attribute = routeAttribute[0] as MapRouteAttribute;

                routes.MapRoute(
                    attribute.Name,
                    attribute.Url,
                    new { controller = listingTypeName, action = x.Name },
                    new[] { Namespace });
            });
        }

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
