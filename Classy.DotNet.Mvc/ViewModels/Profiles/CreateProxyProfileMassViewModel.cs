using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class CreateProxyProfileMassViewModel<TProMetadata>
    {
        [Display(Name = "CreateProxyProfileMass_Category")]
        [Required(ErrorMessage = "CreateProxyProfileMass_Category_Required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "CreateProxyProfileMass_File_Required")]
        public HttpPostedFileBase File { get; set; }
    }
}