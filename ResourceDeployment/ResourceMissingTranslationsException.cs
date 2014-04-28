using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Deployment
{
    public class ResourceMissingTranslationsException : Exception
    {
        public ResourceMissingTranslationsException(string key) : base(string.Format("Resource {0} is missing translations in some supported cultures", key)) { }
    }
}