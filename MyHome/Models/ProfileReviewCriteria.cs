using Classy.DotNet.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyHome.Models
{
    public class ProfileReviewCriteria : IReviewSubCriteria<ProfileReviewCriteria>
    {
        [Display(Name="ReviewCriteria_Professionalism")]
        [Required(ErrorMessage="ReviewCriteria_Professionalism_Required")]
        public decimal Professionalism { get; set; }
        [Display(Name = "ReviewCriteria_Availability")]
        [Required(ErrorMessage = "ReviewCriteria_Availability_Required")]
        public decimal Availability { get; set; }
        [Display(Name = "ReviewCriteria_Creativity")]
        [Required(ErrorMessage = "ReviewCriteria_Creativity_Required")]
        public decimal Creativity { get; set; }
        [Display(Name = "ReviewCriteria_ServiceLevel")]
        [Required(ErrorMessage = "ReviewCriteria_ServiceLevel_Required")]
        public decimal ServiceLevel { get; set; }

        public decimal CalculateScore()
        {
            return Math.Round(
                Professionalism * .25m +
                Availability * .25m +
                Creativity * .25m +
                ServiceLevel * .25m);
        }

        public IDictionary<string, decimal> ToDictionary()
        {
            var dict = new Dictionary<string, decimal>();
            dict.Add("Professionalism", Professionalism);
            dict.Add("Availability", Availability);
            dict.Add("Creativity", Creativity);
            dict.Add("ServiceLevel", ServiceLevel);
            return dict;
        }


        public ProfileReviewCriteria FromDictionary(IDictionary<string, decimal> subCriteria)
        {
            if (subCriteria == null) return null;

            return new ProfileReviewCriteria
            {
                Professionalism = subCriteria["Professionalism"],
                Availability = subCriteria["Availability"],
                Creativity = subCriteria["Creativity"],
                ServiceLevel = subCriteria["ServiceLevel"]
            };
        }
    }
}