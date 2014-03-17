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
    public class DiscussionController : Classy.DotNet.Mvc.Controllers.ListingController<MyHome.Models.DiscussionMetadata, MyHome.Models.PhotoGridViewModel>
    {
        public DiscussionController()
            : base("MyHome.Controllers") { }

        public override string ListingTypeName
        {
	        get 
	        { 
		         return "Discussion";
	        }
        }
    }
}