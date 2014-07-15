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
    public class HomePageController : BaseController
    {
        public HomePageController() : base() { }
        public HomePageController(string ns) : base(ns) { }

        public override void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            routes.MapRouteForSupportedLocales(
                name: "Home",
                url: "",
                defaults: new { controller = "HomePage", action = "Home" },
                namespaces: null
            );
        }

        public ActionResult Home()
        {
            var service = new ListingService();

            // get latest photos
            var photos = service.SearchListings(
                null,
                null,
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
                Listings = photos.Results,
                Collections = collections
            };

            return View(model);
        }
    }
}
