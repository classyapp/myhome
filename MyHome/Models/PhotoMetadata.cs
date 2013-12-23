using Classy.DotNet.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class PhotoMetadata : IMetadata<PhotoMetadata>
    {
        public IList<Classy.Models.Response.CustomAttributeView> ToCustomAttributeList()
        {
            throw new NotImplementedException();
        }

        public PhotoMetadata FromCustomAttributeList(IList<Classy.Models.Response.CustomAttributeView> metadata)
        {
            throw new NotImplementedException();
        }
    }
}