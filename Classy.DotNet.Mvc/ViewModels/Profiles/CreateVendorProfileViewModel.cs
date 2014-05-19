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
    public class CreateVendorProfileViewModel<TMetadata>
    {
        public string ProfileId { get; set; }
        [Display(Name = "Vendor_Email")]
        [Required(ErrorMessage = "Vendor_Email_Required")]
        public string Email { get; set; }
        [Display(Name = "Vendor_Phone")]
        [Required(ErrorMessage = "Vendor_Phone_Required")]
        public string Phone { get; set; }
        [Display(Name = "Vendor_Fax")]
        public string Fax { get; set; }
        [Display(Name = "Vendor_WebsiteUrl")]
        [Required(ErrorMessage = "Vendor_WebsiteUrl_Required")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "Vendor_Category")]
        [Required(ErrorMessage = "Vendor_Category_Required")]
        public string Category { get; set; }
        [Display(Name = "Vendor_Street1")]
        [Required(ErrorMessage = "Vendor_Street1_Required")]
        public string Street1 { get; set; }
        [Display(Name = "Vendor_City")]
        [Required(ErrorMessage = "Vendor_City_Required")]
        public string City { get; set; }
        [Display(Name = "Vendor_Country")]
        [Required(ErrorMessage = "Vendor_Country_Required")]
        public string Country { get; set; }
        [Display(Name = "Vendor_PostalCode")]
        [Required(ErrorMessage = "Vendor_PostalCode_Required")]
        public string PostalCode { get; set; }
        [Display(Name = "Vendor_CompanyName")]
        [Required(ErrorMessage = "Vendor_CompanyName_Required")]
        public string CompanyName { get; set; }
        [Display(Name = "Vendor_DefaultCulture")]
        public string DefaultCulture { get; set; }
        public TMetadata Metadata { get; set; }
    }
}