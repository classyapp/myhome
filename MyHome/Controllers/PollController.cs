using MyHome.Models;
using MyHome.Models.Polls;

namespace MyHome.Controllers
{
    public class PollController : Classy.DotNet.Mvc.Controllers.ListingController<PollMetadata, PhotoGridViewModel>
    {
        public PollController() : base("MyHome.Controllers") { }

        public override string ListingTypeName
        {
	        get 
	        { 
		         return "Poll";
	        }
        }
	}
}