using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Classy.DotNet.Mvc.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Mvc.Localization;

namespace MyHome
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            Debugger.Launch();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var controllerAssemblies = new[] {Assembly.GetExecutingAssembly(), typeof (HomePageController).Assembly};

            var controllers = controllerAssemblies.SelectMany(x => x.GetTypes().Where(t => t.IsSubclassOf(typeof (BaseController)) && !t.IsGenericType));
            foreach (var controller in controllers)
            {
                var controllerInstance = Activator.CreateInstance(controller);
                var baseControllerInstance = (controllerInstance) as BaseController;

                if (baseControllerInstance == null)
                    throw new Exception("Could not register routes in " + controller.Name);

                baseControllerInstance.RegisterRoutes(routes);
            }
            
            // default asp.net mvc route pattern
            routes.MapRouteForSupportedLocales(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "HomePage", action = "Home", id = UrlParameter.Optional },
                namespaces: new string[] { "Classy.DotNet.Mvc.Controllers" }
            );
        }
    }
}
