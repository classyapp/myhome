using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Security
{
    public class ResetPasswordViewModel
    {
        public string Hash { get; set; }
        [Display(Name = "ResetPassword_Password")]
        [Required(ErrorMessage = "ResetPassword_Password_Required")]
        public string Password { get; set; }
        [Display(Name = "ResetPassword_ConfirmPassword")]
        [Compare("Password", ErrorMessage = "ResetPassword_PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
