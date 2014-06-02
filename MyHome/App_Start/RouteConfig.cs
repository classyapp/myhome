using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

            /*
            var controllerAssemblies = new[] {Assembly.GetExecutingAssembly(), typeof (HomePageController).Assembly};
                        var controllers = controllerAssemblies.SelectMany(x => x.GetTypes().Where(t => t.IsSubclassOf(typeof (BaseController)) && !t.IsGenericType));
            foreach (var controller in controllers)
            {
                var controllerInstance = Activator.CreateInstance(controller);
                var baseControllerInstance = (controllerInstance) as BaseController;
                var securityController = new MyHome.Controllers.SecurityController();
                securityController.RegisterRoutes(routes);

                if (baseControllerInstance == null)
                    throw new Exception("Could not register routes in " + controller.Name);
            
                baseControllerInstance.RegisterRoutes(routes);
            }
            */

            #region // security controller

            var securityController = new MyHome.Controllers.SecurityController();
            securityController.RegisterRoutes(routes);

            #endregion

            #region // reviews

            var reviewController = new MyHome.Controllers.ReviewController();
            reviewController.RegisterRoutes(routes);

            #endregion

            #region // profiles

            var profileController = new MyHome.Controllers.ProfileController();
            profileController.RegisterRoutes(routes);

            #endregion

            #region // listing types

            var photoController = new MyHome.Controllers.PhotoController();
            photoController.RegisterRoutes(routes);

            var productController = new MyHome.Controllers.ProductController();
            productController.RegisterRoutes(routes);

            var discussionController = new MyHome.Controllers.DiscussionController();
            discussionController.RegisterRoutes(routes);

            var collectionController = new Classy.DotNet.Mvc.Controllers.CollectionController();
            collectionController.RegisterRoutes(routes);

                #endregion

            #region // localization

            var localizationController = new Classy.DotNet.Mvc.Controllers.LocalizationController();
            localizationController.RegisterRoutes(routes);

            #endregion

            #region // static pages

            var staticController = new MyHome.Controllers.StaticPagesController();
            staticController.RegisterRoutes(routes);

            #endregion

            #region // home page

            var homePageController = new Classy.DotNet.Mvc.Controllers.HomePageController();
            homePageController.RegisterRoutes(routes);

            var searchController = new SearchController();
            searchController.RegisterRoutes(routes);

            var pollController = new PollController();
            pollController.RegisterRoutes(routes);

            #endregion

            
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
