using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class UpdateListingViewModel<TListingMetadata>
    {
        // basic
        [Required]
        public string Id { get; set; }
        [Display(Name = "UpdateListing_Title")]
        [Required(ErrorMessage = "UpdateListing_Title_Required")]
        public string Title { get; set; }
        [Display(Name = "UpdateListing_Content")]
        [Required(ErrorMessage = "UpdateListing_Content_Required")]
        public string Content { get; set; }
        public IList<MediaFileView> ExternalMedia { get; set; }

        // meta
        public TListingMetadata Metadata { get; set; }
    }
}
