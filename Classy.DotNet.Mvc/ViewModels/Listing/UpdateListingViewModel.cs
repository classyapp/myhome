using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ModelBinders;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    [ModelBinder(typeof(CommaSeparatedToList))]
    public class UpdateListingViewModel<TListingMetadata>
    {
        // basic
        [Required]
        public string Id { get; set; }
        [Display(Name = "UpdateListing_Title")]
        [Required(ErrorMessage = "UpdateListing_Title_Required")]
        public string Title { get; set; }
        [Display(Name = "UpdateListing_Content")]
        public string Content { get; set; }
        public IList<MediaFileView> ExternalMedia { get; set; }
        public bool IsEditor { get; set; }
        [CommaSeparatedAttribute]
        public IList<string> Hashtags { get; set; }

        // meta
        public TListingMetadata Metadata { get; set; }

        public string DefaultCulture { get; set; }
    }
}
