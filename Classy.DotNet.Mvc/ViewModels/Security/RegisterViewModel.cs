using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Security
{
    public class RegistrationViewModel<TMetadata>
    {
        [Display(Name="Register_IsProfessional")]
        public bool IsProfessional { get; set; }
        [Display(Name="Register_Email")]
        [Required(ErrorMessage="Register_Email_Required")]
        [EmailAddress(ErrorMessage="Register_Email_Invalid")]
        public string Email { get; set; }
        [Display(Name="Register_Username")]
        [Required(ErrorMessage="Register_Username_Required")]
        public string Username { get; set; }
        [Display(Name="Register_Password")]
        [Required(ErrorMessage="Register_Password_Required")]
        public string Password { get; set; }

        public TMetadata Metadata { get; set; }
    }
}
