using Classy.DotNet.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TwitterBootstrapMVC;

namespace MyHome
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/fwlink/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Bootstrap.Configure();

            // localization of display and validation attribtues
            ModelMetadataProviders.Current = new Classy.DotNet.Mvc.Localization.MyLocalizationProvider();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            ClassyAuth.SetPrincipal();
        }
    }
}
