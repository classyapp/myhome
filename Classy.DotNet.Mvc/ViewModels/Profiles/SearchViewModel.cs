using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class SearchViewModel<TProMetadata>
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Filters { get; set; } // other properties of this model are parsed out of this by the search action
        public TProMetadata Metadata { get; set; }
        public bool ProfessionalsOnly { get; set; }
        public IList<ProfileView> Results { get; set; }
    }
}
