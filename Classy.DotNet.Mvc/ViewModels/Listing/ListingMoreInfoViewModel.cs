using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class ListingMoreInfoViewModel
    {
        public string ListingId { get; set; }
        public Dictionary<string,string> Metadata { get; set; }
        public Dictionary<string,string> Query { get; set; }
    }
}
