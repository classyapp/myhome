using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class TranslateListingViewModel
    {
        public string ListingId { get; set; }
        [Display(Name="TransateListing_Language")]
        public string CultureCode { get; set; }
        [Display(Name = "TranslateListing_Title")]
        public string Title { get; set; }
        [Display(Name = "TranslateListing_Content")]
        public string Content { get; set; }

        public string Action { get; set; }
    }
}
