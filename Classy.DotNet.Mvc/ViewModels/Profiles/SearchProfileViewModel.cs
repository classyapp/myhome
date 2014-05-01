using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;

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
            string invalidString = string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars())));

            return Path.Combine(
                Regex.Replace(this.Category ?? string.Empty, invalidString, string.Empty),
                Regex.Replace(this.City ?? string.Empty, invalidString, string.Empty),
                Regex.Replace(this.Country ?? string.Empty,  invalidString, string.Empty),
                Regex.Replace(this.Name ?? string.Empty, invalidString, string.Empty)).ToLower().Replace('\\', '/');
        }
    }
}
