using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "ChangePassword_NewPassword_Required")]
        [Display(Name = "ChangePassword_NewPassword")]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        [Display(Name = "ChangePassword_ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
