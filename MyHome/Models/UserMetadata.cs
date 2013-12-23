using Classy.DotNet.Mvc.Controllers;
using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class UserMetadata : IMetadata<UserMetadata>
    {
        public bool IsRenter { get; set; }
        public bool IsHomeOwner { get; set; }

        public IList<CustomAttributeView> ToCustomAttributeList()
        {
            var list = new List<CustomAttributeView>();
            list.Add(new CustomAttributeView { Key = "IsRenter", Value = IsRenter.ToString() });
            list.Add(new CustomAttributeView { Key = "IsHomeOwner", Value = IsHomeOwner.ToString() });
            return list;
        }

        public UserMetadata FromCustomAttributeList(IList<CustomAttributeView> metadata)
        {
            var output = new UserMetadata();
            if (metadata.Any(x => x.Key == "IsRenter")) IsRenter = Convert.ToBoolean(metadata.Single(x => x.Key == "IsRenter").Value);
            if (metadata.Any(x => x.Key == "IsHomeOwner")) IsHomeOwner = Convert.ToBoolean(metadata.Single(x => x.Key == "IsHomeOwner").Value);
            return output;
        }
    }
}