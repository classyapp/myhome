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
                name: "Terms",
                url: "terms",
                defaults: new { controller = "Default", action = "Terms" },
                namespaces: null
            );

            routes.MapRouteForSupportedLocales(
                name: "Privacy",
                url: "privacy",
                defaults: new { controller = "Default", action = "Privacy" },
                namespaces: null
            );

            routes.MapRouteForSupportedLocales(
                name: "Carrers",
                url: "careers",
                defaults: new { controller = "Default", action = "Careers" },
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
                new string[] { "Photo" },
                null,
                null,
                null,
                null,
                1);

            // get featured collections
            var collections = service.GetApprovedCollections(null, 5, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);

            var model = new ViewModels.Default.HomeViewModel
            {
                Photos = photos.Results,
                Collections = collections
            };

            return View(model);
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Careers()
        {
            return View();
        }
    }
}
