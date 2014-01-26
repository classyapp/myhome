using Classy.DotNet.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.Attributes
{
    public class AuthorizeProfileTypeWithRedirect : AuthorizeWithRedirect
    {
        public bool IsProfessional { get; set; }
        public AuthorizeProfileTypeWithRedirect(string routeName)
            : base(routeName)
        {

        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result == null)
            {
                if (IsProfessional && !(filterContext.HttpContext.User as ClassyIdentity).Profile.IsProfessional)
                {
                    Fail(filterContext);
                }
            }
        }
    }
}
