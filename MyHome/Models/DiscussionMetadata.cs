using Classy.DotNet.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class DiscussionMetadata : IMetadata<DiscussionMetadata>
    {
        [Required]
        public string Category { get; set; }

        public IList<Classy.Models.Response.CustomAttributeView> ToCustomAttributeList()
        {
            throw new NotImplementedException();
        }

        public DiscussionMetadata FromCustomAttributeList(IList<Classy.Models.Response.CustomAttributeView> metadata)
        {
            throw new NotImplementedException();
        }
    }
}