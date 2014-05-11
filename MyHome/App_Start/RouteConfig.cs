﻿using MyHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Mvc.Localization;

namespace MyHome
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

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

            #endregion

            #region // collections
            
            var collectionController = new Classy.DotNet.Mvc.Controllers.CollectionController();
            collectionController.RegisterRoutes(routes);

            #endregion

            #region // localization

            var localizationController = new Classy.DotNet.Mvc.Controllers.LocalizationController();
            localizationController.RegisterRoutes(routes);

            #endregion 

            var staticController = new MyHome.Controllers.StaticPagesController();
            staticController.RegisterRoutes(routes);
        }
    }
}
