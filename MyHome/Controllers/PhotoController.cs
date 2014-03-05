using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet;

namespace MyHome.Controllers
{
    public class PhotoController : Classy.DotNet.Mvc.Controllers.ListingController<MyHome.Models.PhotoMetadata>
    {
        public PhotoController()
            : base("MyHome.Controllers") { }

        public override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.MapRoute(
                "CreatePhotoFromUrl",
                "web/photo/from",
                new { controller = "Photo", action = "CreatePhotoFromUrl" });
        }

        public ActionResult CreatePhotoFromUrl(string url)
        {
            ViewBag.Url = url;
            return View();
        }

        public override string ListingTypeName
        {
	        get 
	        { 
		         return "Photo";
	        }
        }
    }
}