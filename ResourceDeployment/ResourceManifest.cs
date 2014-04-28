using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Deployment
{
    public class Resource
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public IDictionary<string, string> Values { get; set; }
    }

    public class ResourceManifest
    {
        public string AppId { get; set; }
        public IList<Resource> Resources { get; set; }
    }
}