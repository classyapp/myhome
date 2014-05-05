using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classy.DotNet.Mvc.Controllers
{
    public class ListingUpdateArgs
    {
        public bool IsEditor { get; set; }
        public IList<string> Hashtags { get; set; }
        // here we only receive the keywords in English
        public IDictionary<string, IList<string>> EditorKeywords { get; set; }
    }
}
