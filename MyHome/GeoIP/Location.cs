using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.GeoIP
{
    public class Location
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public Location(Newtonsoft.Json.Linq.JToken json)
        {
            if (json != null)
            {
                CountryCode = json["country"].Value<string>("iso_code");
                Country = json["country"]["names"].Value<string>("en");
            }
        }
    }
}
