using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class PhotoActionsViewModel
    {
        public ListingView Listing { get; set; }
        public bool ShowFavorite { get; set; }
        public bool ShowCollect { get; set; }
        public bool ShowEdit { get; set; }
        public bool ShowRemove { get; set; }
    }
}
