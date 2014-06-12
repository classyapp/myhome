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
        public string CollectionType { get; set; }
        [Required(ErrorMessage = "CreateListingFromUrl_CollectionNameRequired")]
        public string Title { get; set; }
        [Display(Name = "CreateListingFromUrl_Content")]
        public string Content { get; set; }
        public string OriginUrl { get; set; }
        public string ExternalMediaUrl { get; set; }
        public string[] Categories { get; set; }

        // meta
        public TListingMetadata Metadata { get; set; }

        // select lists
        public SelectList CollectionList { get; set; }
    }
}
