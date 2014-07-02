using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Services;

namespace Classy.DotNet.Mvc.Controllers
{
    public class SearchController : BaseController
    {
        public SearchController() : base() { }
        public SearchController(string ns) : base(ns) { }

        public override void RegisterRoutes(RouteCollection routes)
        {
            RegisterRoutesByAttributes(routes, "Search");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [MapRoute("SearchListingsSuggestions", "search/listings/suggest")]
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
        [MapRoute("SearchProfilesSuggestions", "search/profiles/suggest")]
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
        [MapRoute("SearchKeywordsSuggestions", "search/keywords/suggest")]
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

        [AcceptVerbs(HttpVerbs.Get)]
        [MapRoute("SearchProductsSuggestions", "search/products/suggest")]
        public ActionResult SearchProductsSuggestions(string q)
        {
            try
            {
                var service = new SearchService();
                var suggestions = service.SearchProductsSuggestions(q);

                return Json(suggestions, JsonRequestBehavior.AllowGet);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }
    }
}
