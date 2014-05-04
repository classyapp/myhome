using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Deployment
{
    public class ResourceValuesContainsInvalidCultureException : Exception
    {
        public ResourceValuesContainsInvalidCultureException(string key) : base(string.Format("One of the values in resource {0} contains an unsupported culture", key)) { }
    }
}