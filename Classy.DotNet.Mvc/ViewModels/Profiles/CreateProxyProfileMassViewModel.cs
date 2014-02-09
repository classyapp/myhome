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
        [Required(ErrorMessage = "CreateProxyProfileMass_File_Required")]
        public HttpPostedFileBase File { get; set; }
    }
}