using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class ProfileTranslationView
    {
        public string Culture { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class ListingTranslationView
    {
        public string Culture { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
