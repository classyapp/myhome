using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Collection
{
    public class AddToCollectionViewModel
    {
        [Display(Name = "AddToCollection_CollectionId")]
        public string CollectionId { get; set; }
        [Display(Name = "AddToCollection_Title")]
        public string Title { get; set; }
        public string CollectionType { get; set; }
        public SelectList CollectionList { get; set; }
        public IncludedListingView[] IncludedListings { get; set; }
    }
}
