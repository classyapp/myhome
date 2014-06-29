using Classy.DotNet.Mvc.Controllers;
using MyHome.Models;

namespace MyHome.Controllers
{
    public class ProductController : ListingController<ProductMetadata, PhotoGridViewModel>
    {
        public ProductController() : base("MyHome.Controllers") { }

        public override string ListingTypeName
        {
            get { return "Product"; }
        }
    }
}