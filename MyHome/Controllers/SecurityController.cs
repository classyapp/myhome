using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet;
using Classy.Models.Response;

namespace MyHome.Controllers
{
    public class SecurityController : Classy.DotNet.Mvc.Controllers.Security.SecurityController<MyHome.Models.UserMetadata>
    {
        public SecurityController()
            : base("MyHome.Controllers") 
        {
            base.OnProfileRegistered += SecurityController_OnProfileRegistered;
        }

        void SecurityController_OnProfileRegistered(object sender, ProfileView profile)
        {
            // send welcome email
        }
    }
}