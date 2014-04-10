using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class SearchProfileViewModel<TProMetadata>
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public TProMetadata Metadata { get; set; }
        public IList<ProfileView> Results { get; set; }
        public long Count { get; set; }
        public int Page { get; set; }
        public string Format { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public SearchProfileViewModel()
        {
            Page = 1;
            Format = "html";
        }

        public string ToSlug()
        {
            return Path.Combine(
                this.Category ?? string.Empty,
                this.City ?? string.Empty,
                this.Country ?? string.Empty, 
                this.Name ?? string.Empty).ToLower().Replace('\\', '/');
        }
    }
}
