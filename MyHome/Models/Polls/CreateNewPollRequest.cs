namespace MyHome.Models.Polls
{
    public class CreateNewPollRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] ListingIds { get; set; }
    }
}