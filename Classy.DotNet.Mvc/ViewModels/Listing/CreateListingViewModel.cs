using Classy.DotNet.Responses;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{    
    public class CreateListingViewModel<TListingMetadata>
    {
        // basic
        public string CollectionId { get; set; }
        [Required]
        public string CollectionType { get; set; }
        [Required(ErrorMessage = "CreateListing_PhotoBookTitleRequired")]
        public string Title { get; set; }
        [Display(Name = "CreateListing_CollectionContent")]
        public string Content { get; set; }
        public PricingViewModel PricingInfo { get; set; }
        public LocationView Location { get; set; }
        public bool AutoPublish { get; set; }
        public bool IsGoogleConnected { get; set; }
        public bool IsFacebookConnected { get; set; }
        // meta
        public TListingMetadata Metadata { get; set; }
        [Required(ErrorMessage = "CreateListing_FilesRequired")]
        public string DummyFile { get; set; }

        // TODO: products and bookable items

        public SelectList CollectionList { get; set; }
    }
}
