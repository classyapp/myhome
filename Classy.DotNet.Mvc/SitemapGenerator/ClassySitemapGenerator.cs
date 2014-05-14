using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.SitemapGenerator
{
    public abstract class ClassySitemapGenerator : BaseSitemapIndexGenerator
    {
        protected UrlHelper Url { get; set; }
 
        public ClassySitemapGenerator(UrlHelper urlHelper)
        {
            Url = urlHelper;
        }

        public abstract void GenerateStaticNodes();
        public abstract void GenerateListingNodes();
        public abstract void GenerateProfessionalNodes();

        protected override void GenerateUrlNodes()
        {
            //GenerateStaticNodes();
            //GenerateListingNodes();
            GenerateProfessionalNodes();
        }
    }
}
