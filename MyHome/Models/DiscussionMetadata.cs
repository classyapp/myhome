﻿using Classy.DotNet.Mvc.Controllers;
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
            if (!string.IsNullOrEmpty(Category)) dict.Add("Category", Category);
            return dict;
        }

        public DiscussionMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var output = new DiscussionMetadata();
            if (metadata.ContainsKey("Category")) output.Category = metadata["Category"];
            return output;
        }

        public DiscussionMetadata FromDictionary(IDictionary<string, string> metadata, bool processMarkdown)
        {
            return FromDictionary(metadata, true);
        }

        public Dictionary<string, string[]> ParseSearchFilters(string[] filters, out string keyword, ref Classy.DotNet.Responses.LocationView location)
        {
            throw new NotImplementedException();
        }

        public string GetSearchFilterSlug(string keyword, Classy.DotNet.Responses.LocationView location)
        {
            throw new NotImplementedException();
        }
    }
}