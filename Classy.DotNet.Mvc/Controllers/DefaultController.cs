using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Mvc.Localization;
using System.Web.Mvc;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.Controllers
{
    public class DefaultController : BaseController
    {
        public override void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            routes.MapRouteForSupportedLocales(
                name: "Home",
                url: "",
                defaults: new { controller = "Default", action = "Index" },
                namespaces: null
            );

            routes.MapRouteForSupportedLocales(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional },
                namespaces: null
            );
        }

        public ActionResult Index()
        {
            var service = new ListingService();
            // get latest photos
            var photos = service.SearchListings(
                null,
                "Photo",
                null,
                null,
                null,
                null);
            var model = photos;

            return View(model);
        }
    }
}
