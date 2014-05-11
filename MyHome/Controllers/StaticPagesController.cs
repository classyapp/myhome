using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Localization;

namespace MyHome.Controllers
{
    public class StaticPagesController : Classy.DotNet.Mvc.Controllers.DefaultController
    {
        public StaticPagesController() : base("MyHome.Controllers") { }

        public override void RegisterStaticRoutes(System.Web.Routing.RouteCollection routes)
        {
            routes.MapRoute(
                name: "Sitemap",
                url: "sitemap.xml",
                defaults: new { controller = "StaticPages", action = "Terms" }
            );

            routes.MapRouteForSupportedLocales(
                name: "Terms",
                url: "terms",
                defaults: new { controller = "StaticPages", action = "Terms" }
            );

            routes.MapRouteForSupportedLocales(
                name: "Privacy",
                url: "privacy",
                defaults: new { controller = "StaticPages", action = "Privacy" }
            );

            routes.MapRouteForSupportedLocales(
                name: "Carrers",
                url: "careers",
                defaults: new { controller = "StaticPages", action = "Careers" }
            ); 
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