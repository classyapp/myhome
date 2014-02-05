using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Collection
{
    public class EditCollectionViewModel
    {
        public string CollectionId { get; set; }
        [Display(Name = "EditCollection_Title")]
        [Required]
        public string Title { get; set; }
        [Display(Name = "EditCollection_Content")]
        public string Content { get; set; }
        [Required]
        [Display(Name = "EditCollection_Title")]
        public IncludedListing[] IncludedListings { get; set; }
        public IList<ListingView> Listings { get; set; }
    }
}
