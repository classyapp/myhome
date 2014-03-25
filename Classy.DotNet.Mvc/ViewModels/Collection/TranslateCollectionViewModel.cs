using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Collection
{
    public class TranslateCollectionViewModel
    {
        public string CollectionId { get; set; }

        public string CultureCode { get; set; }

        [Display(Name = "TranslateCollection_Title")]
        public string Title { get; set; }
        [Display(Name = "TranslateCollection_Content")]
        public string Content { get; set; }

        public string Action { get; set; }
    }
}
