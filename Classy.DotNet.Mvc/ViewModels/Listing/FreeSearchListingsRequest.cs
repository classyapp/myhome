namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class FreeSearchListingsRequest
    {
        public string Q { get; set; }
        public int? Page { get; set; }
        public int? Amount { get; set; }
    }
}