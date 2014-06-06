using System;

namespace Classy.DotNet.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MapRouteAttribute : Attribute
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public object Defaults { get; set; }
        public string Namespace { get; set; }

        public MapRouteAttribute(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}