using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class EditProfileViewModel<TProMetadata, TUserMetadata>
    {
        // account info
        public string ProfileId { get; set; }
        public string AvatarUrl { get; set; }
        [Display(Name = "EditProfile_Username")]
        public string Username { get; set; }
        [Display(Name = "EditProfile_Email")]
        [Required(ErrorMessage = "EditProfile_Email_Required")]
        public string Email { get; set; }

        // pro info
        public bool IsProfessional { get; set; }
        [Display(Name = "EditProfile_CompanyPhone")]
        public string Phone { get; set; }
        [Display(Name = "EditProfile_CompanyWebsiteUrl")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "EditProfile_ProCategory")]
        public string Category { get; set; }
        [Display(Name = "EditProfile_CompanyName")]
        public string CompanyName { get; set; }
        public TProMetadata ProfessionalMetadata { get; set; }

        // contact info
        [Display(Name = "EditProfile_FirstName")]
        public string FirstName { get; set; }
        [Display(Name = "EditProfile_LastName")]
        public string LastName { get; set; }
        [Display(Name = "EditProfile_Street1")]
        public string Street1 { get; set; }
        [Display(Name = "EditProfile_Street2")]
        public string Street2 { get; set; }
        [Display(Name = "EditProfile_City")]
        public string City { get; set; }
        [Display(Name = "EditProfile_Country")]
        public string Country { get; set; }
        [Display(Name = "EditProfile_PostalCode")]
        public string PostalCode { get; set; }
        public TUserMetadata UserMetadata { get; set; }
    }
}
