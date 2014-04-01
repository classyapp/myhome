using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;
using Classy.DotNet.Mvc.Localization;

namespace Classy.DotNet.Mvc.ViewModels.Collection
{
    public class EditCollectionViewModel
    {
        public string CollectionId { get; set; }
        public string CollectionType { get; set; }
        [Display(Name = "EditCollection_Title")]
        [Required(ErrorMessage = "EditCollection_Title_Required")]
        public string Title { get; set; }
        [Display(Name = "EditCollection_Content")]
        public string Content { get; set; }
        [Display(Name = "EditCollection_IncludedListings")]
        [Required(ErrorMessage = "EditCollection_IncludedListings_Required")]
        public IncludedListingView[] IncludedListings { get; set; }
        public IList<ListingView> Listings { get; set; }
        public string DefaultCulture { get; set; }

        public string GetLocalizedString(string key)
        {
            return Localizer.Get(string.Format(key, CollectionType));
        }
    }
}
