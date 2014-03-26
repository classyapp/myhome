using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class TranslatePhotoViewModel
    {
        public string ListingId { get; set; }
        public string CultureCode { get; set; }

        [Display(Name = "TranslatePhoto_Title")]
        public string Title { get; set; }
        [Display(Name = "TranslatePhoto_Content")]
        public string Content { get; set; }

        public string Action { get; set; }
    }
}
