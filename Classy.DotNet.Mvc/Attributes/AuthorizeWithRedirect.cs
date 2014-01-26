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

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                Fail(filterContext);
            }
        }

        protected void Fail(AuthorizationContext filterContext)
        {
            if (_routeName == null) filterContext.Result = new RedirectResult("~/");
            else filterContext.Result = new RedirectToRouteResult(_routeName, null);
        }
    }
}
