using Classy.DotNet.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class PhotoMetadata : IMetadata<PhotoMetadata>
    {
        public IDictionary<string, string> ToDictionary()
        {
            return null;
        }

        public PhotoMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            return null;
        }
    }
}