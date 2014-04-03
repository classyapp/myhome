using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Localization
{
    public class ManageResourcesViewModel
    {
        public SelectList SupportedCultures { get; set; }
        public string[] UntranslatedResourceKeys { get; set; }
        public string[] TranslatedResourceKeys { get; set; }
        
        [Required]
        public string ResourceKey { get; set; }
        [Required]
        public string SelectedCulture { get; set; }
        [Required]
        public string ResourceValue { get; set; }
        
    }
}
