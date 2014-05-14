using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;

namespace Classy.DotNet.Mvc.ViewModels.Default
{
    public class HomeViewModel
    {
        public IList<ListingView> Listings { get; set; }
        public IList<CollectionView> Collections { get; set; }
    }
}
