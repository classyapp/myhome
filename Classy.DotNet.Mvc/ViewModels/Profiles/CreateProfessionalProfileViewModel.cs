using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class CreateProfessionalProfileViewModel<TProMetadata>
    {
        public string ProfileId { get; set; }
        [Display(Name = "ClaimProxy_Email")]
        [Required(ErrorMessage = "ClaimProxy_Email_Required")]
        public string Email { get; set; }
        [Display(Name = "ClaimProxy_Phone")]
        [Required(ErrorMessage = "ClaimProxy_Phone_Required")]
        public string Phone { get; set; }
        [Display(Name = "ClaimProxy_WebsiteUrl")]
        [Required(ErrorMessage = "ClaimProxy_WebsiteUrl_Required")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "ClaimProxy_Category")]
        [Required(ErrorMessage = "ClaimProxy_Category_Required")]
        public string Category { get; set; }
        [Display(Name = "ClaimProxy_FirstName")]
        [Required(ErrorMessage = "ClaimProxy_FirstName_Required")]
        public string FirstName { get; set; }
        [Display(Name = "ClaimProxy_LastName")]
        [Required(ErrorMessage = "ClaimProxy_LastName_Required")]
        public string LastName { get; set; }
        [Display(Name = "ClaimProxy_Street1")]
        [Required(ErrorMessage = "ClaimProxy_Street1_Required")]
        public string Street1 { get; set; }
        [Display(Name = "ClaimProxy_City")]
        [Required(ErrorMessage = "ClaimProxy_City_Required")]
        public string City { get; set; }
        [Display(Name = "ClaimProxy_Country")]
        [Required(ErrorMessage = "ClaimProxy_Country_Required")]
        public string Country { get; set; }
        [Display(Name = "ClaimProxy_PostalCode")]
        [Required(ErrorMessage = "ClaimProxy_PostalCode_Required")]
        public string PostalCode { get; set; }
        [Display(Name = "ClaimProxy_CompanyName")]
        [Required(ErrorMessage = "ClaimProxy_CompanyName_Required")]
        public string CompanyName { get; set; }
        [Display(Name = "ClaimProxy_DefaultCulture")]
        public string DefaultCulture { get; set; }
        public TProMetadata Metadata { get; set; }
        public string ReferreUrl { get; set; }
    }
}