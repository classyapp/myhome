using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ActionFilters;
using Classy.DotNet.Services;
using Classy.DotNet.Mvc.Localization;

namespace Classy.DotNet.Mvc.Controllers
{
    public class SearchController : BaseController
    {
        public SearchController() : base() { }
        public SearchController(string ns) : base(ns) { }

        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "SearchSuggestions",
                url: "search/suggest",
                defaults: new { controller = "Search", action = "GetSearchSuggestions" },
                namespaces: new string[] { Namespace }
            );
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSearchSuggestions(string q)
        {
            try
            {
                var service = new SearchService();
                var suggestions = service.GetSearchSuggestions(q);

                return Json(suggestions);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }
    }
}
