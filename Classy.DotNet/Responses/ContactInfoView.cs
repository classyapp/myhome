using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class ContactInfoView
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public LocationView Location { get; set; }
        public string WebsiteUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUsername { get; set; }
        public string LinkedInProfileUrl { get; set; }

        public string Name { get { return string.Concat(FirstName, !string.IsNullOrEmpty(FirstName) ? " " : "", LastName); } }
    }

    public class ExtendedContactInfoView : ContactInfoView
    {
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
