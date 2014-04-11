using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Security
{
    public class ForgotPasswordViewModel
    {
        [Display(Name = "Login_Email")]
        public string Email { get; set; }
    }
}
