using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.Controllers
{
    public class MediaFileController : BaseController
    {
        public MediaFileController() : base() { }
        public MediaFileController(string ns) : base(ns) { }

        public override void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            routes.MapRoute(
                name: "AddMediaFile",
                url: "mediafile",
                defaults: new { controller = "MediaFile", action = "AddFile" },
                namespaces: new string[] { Namespace }
            );
            routes.MapRoute(
                name: "RemoveMediaFile",
                url: "mediafile/delete",
                defaults: new { controller = "MediaFile", action = "DeleteFile" },
                namespaces: new string[] { Namespace }
            );
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddFile()
        {
            if (Request.Files.Count != 1)
            {
                throw new InvalidOperationException("Invalid or missing files.");
            }

            var service = new MediaFileService();
            string fileId = service.AddTempFile(Request.Files[0]);

            return Json(new { fileId = fileId });
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteFile(string fileId, string listingId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                throw new InvalidOperationException("Invalid or missing file.");
            }

            var service = new MediaFileService();
            try
            {
                if (string.IsNullOrWhiteSpace(listingId))
                {
                    service.DeleteTempFile(fileId);
                }
                else
                {
                    service.RemoveImageFromListing(listingId, fileId);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            return Json(new {});
        }
    }
}
