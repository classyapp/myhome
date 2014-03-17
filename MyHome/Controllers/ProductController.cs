using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyHome.Controllers
{
    public class ProductController : Classy.DotNet.Mvc.Controllers.ListingController<MyHome.Models.ProductMetadata, MyHome.Models.PhotoGridViewModel>
    {
        public ProductController()
            : base("MyHome.Controllers") { }

        public override string ListingTypeName
        {
	        get 
	        { 
		         return "Product";
	        }
        }
	}
}