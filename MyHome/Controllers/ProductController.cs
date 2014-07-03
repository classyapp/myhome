using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.ViewModels.Listing;
using Classy.DotNet.Services;
using MyHome.Models;

namespace MyHome.Controllers
{
    public class ProductController : ListingController<ProductMetadata, PhotoGridViewModel>
    {
        public ProductController() : base("MyHome.Controllers") { }

        public override string ListingTypeName { get { return "Product"; } }

        public override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);
            RegisterRoutesByAttributes(routes, ListingTypeName);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [MapRoute("ProductSearch", "products/search")]
        public ActionResult ProductSearch()
        {
            try
            {
                var metadata = new Dictionary<string, string[]>();

                var service = new ListingService();
                var searchResults = service.SearchListings(null, new[] {ListingType.Product}, metadata, null, null, null, 0);

                var model = new SearchListingsViewModel<ProductMetadata> {
                    Count = searchResults.Count,
                    Results = searchResults.Results.ToList()
                };

                return View("Search", model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }
    }
}