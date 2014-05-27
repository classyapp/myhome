﻿using System.Collections.Generic;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Extensions;
using Classy.DotNet.Responses;

namespace MyHome.Models.Polls
{
    public class PollMetadata : IMetadata<PollMetadata>
    {
        public List<string> Listings { get; set; }
        public List<string> Votes { get; set; }

        public PollMetadata()
        {
            Listings = new List<string>(4);
            Votes = new List<string>(4);
        }

        public IDictionary<string, string> ToDictionary()
        {
            var properties = new Dictionary<string, string>();

            if (!Listings.IsNullOrEmpty())
                Listings.Indexed().ForEach(x => properties.Add("Listing_" + x.Key, x.Value));

            if (!Votes.IsNullOrEmpty())
                Votes.Indexed().ForEach(x => properties.Add("Vote_" + x.Key, x.Value.ToString()));

            return properties;
        }

        public IDictionary<string, string> ToTranslationsDictionary()
        {
            throw new System.NotImplementedException();
        }

        public PollMetadata FromDictionary(IDictionary<string, string> metadata)
        {
            var pollMetadata = new PollMetadata();

            for (var i=0; metadata.ContainsKey("Listing_" + i); i++)
                pollMetadata.Listings.Add(metadata["Listing_" + i]);
            for (var i=0; metadata.ContainsKey("Vote_" + i); i++)
                pollMetadata.Votes.Add(metadata["Vote_" + i]);

            return pollMetadata;
        }

        public Dictionary<string, string[]> ParseSearchFilters(string[] filters, out string keyword, ref LocationView location)
        {
            throw new System.NotImplementedException();
        }

        public string GetSearchFilterSlug(string keyword, LocationView location)
        {
            throw new System.NotImplementedException();
        }
    }
}