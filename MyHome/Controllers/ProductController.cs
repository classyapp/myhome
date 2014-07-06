using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Localization;
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
        public ActionResult ProductSearch(SearchListingsViewModel<PhotoMetadata> searchRequest)
        {
            try
            {
                var metadata = new Dictionary<string, string[]>();

                var service = new ListingService();
                var searchResults = service.SearchListings(
                    null,
                    new[] { ListingType.Product },
                    metadata,
                    searchRequest.PriceMin,
                    searchRequest.PriceMax,
                    searchRequest.Location,
                    searchRequest.Page);

                var model = new SearchListingsViewModel<ProductMetadata> {
                    Count = searchResults.Count,
                    Results = searchResults.Results.ToList(),
                    PagingUrl = Url.RouteUrlForCurrentLocale("ProductSearch")
                };

                if (Request.IsAjaxRequest())
                    return PartialView("ProductGrid", new PhotoGridViewModel { Results = model.Results });

                return View("Search", model);
            }
            catch (ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }
    }
}