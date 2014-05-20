using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Localization;
using System.Net;

namespace MyHome.Controllers
{
    public class StaticPagesController : Classy.DotNet.Mvc.Controllers.BaseController
    {
        public StaticPagesController() : base("MyHome.Controllers") { }

        public override void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            routes.MapRoute(
                name: "Sitemap",
                url: "sitemap-test.xml",
                defaults: new { controller = "StaticPages", action = "Sitemap" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "Terms",
                url: "terms",
                defaults: new { controller = "StaticPages", action = "Terms" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "Privacy",
                url: "privacy",
                defaults: new { controller = "StaticPages", action = "Privacy" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteForSupportedLocales(
                name: "Careers",
                url: "careers",
                defaults: new { controller = "StaticPages", action = "Careers" },
                namespaces: new string[] { Namespace }
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

        public ActionResult Sitemap()
        {
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            //return Content("<result>ok</result>", "text/xml");
        }
    }
}