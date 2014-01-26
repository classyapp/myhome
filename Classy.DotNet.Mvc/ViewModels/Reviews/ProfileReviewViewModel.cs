using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Classy.DotNet.Mvc.ViewModels.Reviews
{
    public class ScaleItem {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class ProfileReviewViewModel<TProMetadata, TReviewSubCriteria>
    {
        public string ProfileId { get; set; }
        public bool IsProxy { get; set; }
        [Required(ErrorMessage="PostReview_SubCriteria_Required")]
        public TReviewSubCriteria SubCriteria { get; set; }
        public List<ScaleItem> Scale1to5
        {
            get
            {
                return new List<ScaleItem> {
                    new ScaleItem { Text = "1", Value = 1 },
                    new ScaleItem { Text = "2", Value = 2 },
                    new ScaleItem { Text = "3", Value = 3 },
                    new ScaleItem { Text = "4", Value = 4 },
                    new ScaleItem { Text = "5", Value = 5 }
                };
            }
        }
        [Display(Name="PostReview_Comments")]
        [Required(ErrorMessage="PostReview_Comments_Required")]
        public string Comments { get; set; }
        public ExtendedContactInfoView ContactInfo { get; set; }
        public TProMetadata Metadata { get; set; }
    }
}