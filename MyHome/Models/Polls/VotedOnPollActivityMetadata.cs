using System.Collections.Generic;
using Classy.DotNet.Models.LogActivity;

namespace MyHome.Models.Polls
{
    public class VotedOnPollActivityMetadata : ILogActivityMetadata<VotedOnPollActivityMetadata>
    {
        public string Vote { get; set; }

        public VotedOnPollActivityMetadata DeserializeMetadata(Dictionary<string, string> metadata)
        {
            return new VotedOnPollActivityMetadata {
                Vote = metadata.ContainsKey("Vote") ? metadata["Vote"] : ""
            };
        }

        public Dictionary<string, string> SerializeMetadata()
        {
            return new Dictionary<string, string> {
                {"Vote", Vote ?? ""}
            };
        }
    }
}