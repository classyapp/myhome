using Classy.DotNet.Responses;

namespace MyHome.Models
{
    public class PhotoActionsViewModel
    {
        public ListingView Listing { get; set; }
        public bool ShowFavorite { get; set; }
        public bool ShowCollect { get; set; }
        public bool ShowEdit { get; set; }
        public bool ShowRemove { get; set; }
        public bool ShowShare { get; set; }
        public bool IsStatic { get; set; }
    }
}
