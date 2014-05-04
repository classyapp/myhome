using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Deployment
{
    public class ResourceMissingFieldException : Exception
    {
        public ResourceMissingFieldException(string key, string field) : base(string.Format("Resource {0} is missing the {1} field", key, field)) { }
    }
}