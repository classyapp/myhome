using System.Collections.Generic;
using Classy.DotNet.Responses;

namespace MyHome.Models.Polls
{
    public class PollViewExtraData
    {
        public List<ListingView> Listings { get; set; }
        public string UserVote { get; set; }
    }
}