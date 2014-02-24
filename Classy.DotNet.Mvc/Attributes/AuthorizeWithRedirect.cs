using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Classy.DotNet.Mvc.Attributes
{
    public class AuthorizeWithRedirect : FilterAttribute, IAuthorizationFilter
    {
        private string _routeName = null;
        private string[] _permissions = null;

        /// <summary>
        /// Redirect to a specific route if user is not logged in
        /// </summary>
        public AuthorizeWithRedirect() { }

        /// <summary>
        /// Redirect to a specific route if user is not logged in
        /// </summary>
        /// <param name="routeName">The route name to use in the redirect</param>
        public AuthorizeWithRedirect(string routeName) 
        {
            _routeName = routeName;
        }

        public AuthorizeWithRedirect(string routeName, string[] permissions)
        {
            _routeName = routeName;
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var isAuthorized = filterContext.HttpContext.Request.IsAuthenticated;
            if (isAuthorized && _permissions != null)
            {
                var profile = (filterContext.HttpContext.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile;
                foreach (var p in _permissions)
                {
                    isAuthorized &= profile.Permissions.Contains(p);
                }
            }
            if (!isAuthorized)
            {
                if (_routeName == null) filterContext.Result = new RedirectResult("~/");
                else filterContext.Result = new RedirectToRouteResult(_routeName, null);
            }
        }
    }
}
