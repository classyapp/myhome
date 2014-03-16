using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class CreateListingFromUrlViewModel<TListingMetadata>
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
        public string ExternalMediaUrl { get; set; }

        // meta
        public TListingMetadata Metadata { get; set; }

        // select lists
        public SelectList CollectionList { get; set; }
    }
}
