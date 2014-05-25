using System.Threading;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.Controllers
{
    public class SearchController : BaseController
    {
        public SearchController() : base() { }
        public SearchController(string ns) : base(ns) { }

        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "SearchListingsSuggestions",
                url: "search/listings/suggest",
                defaults: new { controller = "Search", action = "SearchListingsSuggestions" },
                namespaces: new string[] { Namespace }
            );
            routes.MapRoute(
                name: "SearchProfilesSuggestions",
                url: "search/profiles/suggest",
                defaults: new { controller = "Search", action = "SearchProfilesSuggestions" },
                namespaces: new string[] { Namespace }
            );
            routes.MapRoute(
                name: "SearchKeywordsSuggestions",
                url: "search/keywords/suggest",
                defaults: new { controller = "Search", action = "SearchKeywordsSuggestions" },
                namespaces: new string[] { Namespace }
            );
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SearchListingsSuggestions(string q)
        {
            try
            {
                var service = new SearchService();
                var suggestions = service.SearchListingsSuggestions(q);

                return Json(suggestions, JsonRequestBehavior.AllowGet);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SearchProfilesSuggestions(string q)
        {
            try
            {
                var service = new SearchService();
                var suggestions = service.SearchProfilesSuggestions(q);

                return Json(suggestions, JsonRequestBehavior.AllowGet);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SearchKeywordsSuggestions(string q)
        {
            try
            {
                var service = new SearchService();
                var suggestions = service.SearchKeywordsSuggestions(q);

                return Json(suggestions, JsonRequestBehavior.AllowGet);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }
    }
}
