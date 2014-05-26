using System.Collections.Generic;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Extensions;
using Classy.DotNet.Responses;

namespace MyHome.Models.Polls
{
    public class PollMetadata : IMetadata<PollMetadata>
    {
        public List<string> ImageUrls { get; set; }
        public List<string> Votes { get; set; }

        public PollMetadata()
        {
            ImageUrls = new List<string>(4);
            Votes = new List<string>(4);
        }

        public IDictionary<string, string> ToDictionary()
        {
            var properties = new Dictionary<string, string>();

            if (!ImageUrls.IsNullOrEmpty())
                ImageUrls.Indexed().ForEach(x => properties.Add("Image_" + x.Key, x.Value));

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

            for (var i=0; metadata.ContainsKey("Image_" + i); i++)
                pollMetadata.ImageUrls.Add(metadata["Image_" + i]);
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