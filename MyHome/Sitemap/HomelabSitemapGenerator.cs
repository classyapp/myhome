using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Mvc.SitemapGenerator;

namespace MyHome.Sitemap
{
    public class HomelabSitemapGenerator : Classy.DotNet.Mvc.SitemapGenerator.ClassySitemapGenerator
    {
        private static IEnumerable<LocalizedListItem> _supportedCultures = Localizer.GetList("supported-cultures");

        public HomelabSitemapGenerator(UrlHelper urlHelper) : base(urlHelper) { }

        public override void GenerateStaticNodes()
        {
            foreach(var culture in _supportedCultures)
            {
                WriteUrlLocation(Url.RouteUrlForLocale("Home", culture.Value), UpdateFrequency.Daily, DateTime.UtcNow);
            }
        }

        public override void GenerateListingNodes()
        {
            
        }

        public override void GenerateProfessionalNodes()
        {
            
        }
    }
}