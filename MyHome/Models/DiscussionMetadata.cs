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

        public IDictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("Category", Category);
            return dict;
        }

        public DiscussionMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new DiscussionMetadata();
            if (metadata.ContainsKey("Category")) Category = metadata["Category"];
            return output;
        }

        public DiscussionMetadata FromStringArray(string[] strings)
        {
            throw new NotImplementedException();
        }


        public string ToSlug(DiscussionMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}