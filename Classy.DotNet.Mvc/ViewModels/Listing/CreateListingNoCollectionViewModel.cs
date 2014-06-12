using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class CreateListingNoCollectionViewModel<TListingMetadata>
    {
        [Required(ErrorMessage="CreateListing_TitleRequired")]
        public string Title { get; set; }
        [Required(ErrorMessage = "CreateListing_ContentRequired")]
        public string Content { get; set; }
        [Required(ErrorMessage = "CreateListing_CategoryRequired")]
        public string[] Categories { get; set; }
        public PricingInfoView PricingInfo { get; set; }

        public TListingMetadata Metadata { get; set; }
    }
}
