using Classy.DotNet.Mvc.Controllers;
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

        public IDictionary<string, string> ToDictionary()
        {
            var list = new Dictionary<string, string>();
            list.Add("IsRenter", IsRenter.ToString());
            list.Add("IsHomeOwner", IsHomeOwner.ToString());
            return list;
        }

        public UserMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new UserMetadata();
            if (metadata.ContainsKey("IsRenter")) IsRenter = Convert.ToBoolean(metadata["IsRenter"]);
            if (metadata.ContainsKey("IsHomeOwner")) IsHomeOwner = Convert.ToBoolean(metadata["IsHomeOwner"]);
            return output;
        }

        public UserMetadata FromStringArray(string[] strings)
        {
            throw new NotImplementedException();
        }


        public string ToSlug(UserMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}