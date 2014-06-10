using System;
using Classy.DotNet.Mvc.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Mvc.Localization;
using MyHome.Controllers;

namespace MyHome
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var controllers = new[] {
                typeof (SecurityController),
                typeof (ReviewController),
                typeof (ProfileController),
                typeof (PhotoController),
                typeof (ProductController),
                typeof (DiscussionController),
                typeof (CollectionController),
                typeof (LocalizationController),
                typeof (StaticPagesController),
                typeof (HomePageController),
                typeof (SearchController),
                typeof (PollController)
            };

            //var controllerAssemblies = new[] {Assembly.GetExecutingAssembly(), typeof (HomePageController).Assembly};
            //            var controllers = controllerAssemblies.SelectMany(x => x.GetTypes().Where(t => t.IsSubclassOf(typeof (BaseController)) && !t.IsGenericType));
            foreach (var controller in controllers)
            {
                var controllerInstance = Activator.CreateInstance(controller);
                var baseControllerInstance = (controllerInstance) as BaseController;

                if (baseControllerInstance == null)
                    throw new Exception("Could not register routes in " + controller.Name);

                baseControllerInstance.RegisterRoutes(routes);
            }

            routes.IgnoreRoute("Mobile/{*path}");
            
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
