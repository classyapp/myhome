using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class EditProfileViewModel<TProMetadata>
    {
        public string ProfileId { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        [Display(Name = "EditProfile_Username")]
        [Required(ErrorMessage = "EditProfile_Username_Required")]
        public string Username { get; set; }
        [Display(Name = "EditProfile_Email")]
        [Required(ErrorMessage = "EditProfile_Email_Required")]
        public string Email { get; set; }
    }
}
