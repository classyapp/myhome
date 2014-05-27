using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Responses;
using Classy.DotNet.Services;
using MyHome.Models;
using MyHome.Models.Polls;

namespace MyHome.Controllers
{
    public class PollController : Classy.DotNet.Mvc.Controllers.ListingController<PollMetadata, PhotoGridViewModel>
    {
        public PollController() : base("MyHome.Controllers") { }

        public override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.MapRoute(
                name: "SelectListingsModal",
                url: "polls/create/select-listings-modal",
                defaults: new { controller = "Poll", action = "SelectListingsModal" },
                namespaces: new string[] { Namespace }
            );
        }

        public override string ListingTypeName
        {
	        get {  return "Poll"; }
        }

        public ActionResult SelectListingsModal()
        {
            var listingService = new ListingService();
            string collectionType = AuthenticatedUserProfile.IsProfessional ? CollectionType.Project : CollectionType.PhotoBook;
            var collections = listingService.GetCollectionsByProfileId(AuthenticatedUserProfile.Id, collectionType, false, false, false);

            return View("SelectListingsModal", collections);
        }
	}
}